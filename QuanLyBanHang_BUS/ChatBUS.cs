using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using QuanLyBanHang_DAL;
using QuanLyBanHang_DTO;

namespace QuanLyBanHang_BUS
{
    /// <summary>
    /// BUS: Xử lý nghiệp vụ Chat với AI.
    ///  – Gọi OpenAI Chat Completions API (function calling)
    ///  – Lưu / tải lịch sử hội thoại từ CSDL
    ///  – Phân quyền system-prompt theo cấp bậc tài khoản
    ///
    /// API Key và Model AI lưu tại file dbconfig.ini
    /// đặt cùng thư mục với file .exe (AppDomain.CurrentDomain.BaseDirectory).
    ///   OpenAI_Key=sk-...
    ///   OpenAI_Model=gpt-4o-mini
    /// </summary>
    public class ChatBUS
    {
        // HttpClient dùng chung (thread-safe, tạo 1 lần)
        private static readonly HttpClient _http = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(120)
        };

        private readonly JavaScriptSerializer _json;
        private readonly NhanVienDTO          _user;
        private readonly ChatHistoryDAL       _dal;

        // Runtime history cho session hiện tại (để OpenAI biết ngữ cảnh)
        private readonly List<object> _runtimeHistory = new List<object>();
        private int _currentSessionId;

        // ─────────────────────────────────────────────────────
        //  Cấu hình (dbconfig.ini cùng thư mục exe)
        // ─────────────────────────────────────────────────────
        private static readonly string _cfgFile =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dbconfig.ini");

        /// <summary>Đọc OpenAI API Key từ dbconfig.ini.</summary>
        public string ApiKey  => ReadConfig("OpenAI_Key",   "");
        /// <summary>Đọc tên model AI từ dbconfig.ini (mặc định gpt-4o-mini).</summary>
        public string AiModel => ReadConfig("OpenAI_Model", "gpt-4o-mini");

        public static string ReadConfig(string key, string defaultVal = "")
        {
            if (!File.Exists(_cfgFile)) return defaultVal;
            foreach (var line in File.ReadAllLines(_cfgFile))
            {
                var p = line.Split('=');
                if (p.Length >= 2 &&
                    p[0].Trim().Equals(key, StringComparison.OrdinalIgnoreCase))
                    return string.Join("=", p, 1, p.Length - 1).Trim();
            }
            return defaultVal;
        }

        public ChatBUS(NhanVienDTO user)
        {
            _user = user;
            _json = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
            _dal  = new ChatHistoryDAL();
            _dal.EnsureTables();   // Tự tạo bảng nếu chưa có
        }

        // ─────────────────────────────────────────────────────
        //  Quản lý session
        // ─────────────────────────────────────────────────────

        /// <summary>Lấy toàn bộ sessions của user hiện tại (mới nhất trước).</summary>
        public List<ChatSessionDTO> GetSessions()
            => _dal.GetSessions(_user?.Username ?? "");

        /// <summary>Lấy messages của một session.</summary>
        public List<ChatMessageDTO> GetMessages(int sessionId)
            => _dal.GetMessages(sessionId);

        /// <summary>Tạo session mới, reset runtime history.</summary>
        public int CreateNewSession(string firstMessage = null)
        {
            string title = string.IsNullOrWhiteSpace(firstMessage)
                ? "Hội thoại mới"
                : (firstMessage.Length > 80 ? firstMessage.Substring(0, 80) : firstMessage);

            _currentSessionId = _dal.CreateSession(_user?.Username ?? "", title);
            _runtimeHistory.Clear();
            return _currentSessionId;
        }

        /// <summary>Tải lại session cũ: đọc messages từ DB và nạp vào runtime history.</summary>
        public void LoadSession(int sessionId)
        {
            _currentSessionId = sessionId;
            _runtimeHistory.Clear();

            var msgs = _dal.GetMessages(sessionId);
            foreach (var m in msgs)
                _runtimeHistory.Add(new Dictionary<string, object>
                {
                    ["role"]    = m.Role,
                    ["content"] = m.Content
                });
        }

        /// <summary>Xóa một session (cascade xóa messages).</summary>
        public void DeleteSession(int sessionId)
        {
            _dal.DeleteSession(sessionId);
            if (_currentSessionId == sessionId)
            {
                _currentSessionId = 0;
                _runtimeHistory.Clear();
            }
        }

        /// <summary>Id session đang mở hiện tại (0 nếu chưa chọn).</summary>
        public int CurrentSessionId => _currentSessionId;

        // ─────────────────────────────────────────────────────
        //  Gửi tin nhắn (agentic loop)
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Gửi tin nhắn người dùng, chạy vòng lặp agentic (tối đa 8 bước),
        /// lưu lịch sử vào DB, trả về câu trả lời cuối cùng.
        /// </summary>
        public async Task<string> SendMessageAsync(string userMessage)
        {
            // Tạo session mới nếu chưa có
            if (_currentSessionId == 0)
                CreateNewSession(userMessage);

            // Lưu user message vào DB
            _dal.SaveMessage(_currentSessionId, "user", userMessage);

            _runtimeHistory.Add(new Dictionary<string, object>
            {
                ["role"]    = "user",
                ["content"] = userMessage
            });

            for (int iter = 0; iter < 8; iter++)
            {
                var messages = new List<object>();
                messages.Add(new Dictionary<string, object>
                {
                    ["role"]    = "system",
                    ["content"] = BuildSystemPrompt()
                });
                messages.AddRange(_runtimeHistory);

                var reqBody = new Dictionary<string, object>
                {
                    ["model"]       = AiModel,
                    ["messages"]    = messages,
                    ["tools"]       = BuildTools(),
                    ["tool_choice"] = "auto"
                };

                string responseJson = await CallOpenAIAsync(_json.Serialize(reqBody));
                var resp     = _json.Deserialize<Dictionary<string, object>>(responseJson);
                var choices  = (ArrayList)resp["choices"];
                var choice   = (Dictionary<string, object>)choices[0];
                var msg      = (Dictionary<string, object>)choice["message"];
                string fin   = (string)choice["finish_reason"];

                if (fin == "tool_calls")
                {
                    _runtimeHistory.Add(msg);
                    var toolCalls = (ArrayList)msg["tool_calls"];
                    foreach (Dictionary<string, object> tc in toolCalls)
                    {
                        string tcId  = (string)tc["id"];
                        var    func  = (Dictionary<string, object>)tc["function"];
                        string fname = (string)func["name"];
                        string fargs = (string)func["arguments"];
                        string res   = ExecuteTool(fname, fargs);

                        _runtimeHistory.Add(new Dictionary<string, object>
                        {
                            ["role"]         = "tool",
                            ["tool_call_id"] = tcId,
                            ["content"]      = res
                        });
                    }
                }
                else
                {
                    string content = (msg.ContainsKey("content") ? msg["content"] as string : null)
                                     ?? "(Không có nội dung)";

                    _runtimeHistory.Add(new Dictionary<string, object>
                    {
                        ["role"]    = "assistant",
                        ["content"] = content
                    });

                    // Lưu reply vào DB
                    _dal.SaveMessage(_currentSessionId, "assistant", content);
                    return content;
                }
            }

            return "Xin lỗi, không thể hoàn thành yêu cầu sau nhiều lần thử.";
        }

        // ─────────────────────────────────────────────────────
        //  Thực thi công cụ (function calling)
        // ─────────────────────────────────────────────────────

        private string ExecuteTool(string name, string argsJson)
        {
            try
            {
                var args = _json.Deserialize<Dictionary<string, object>>(argsJson);
                if (name == "query_database")
                {
                    string sql = args.ContainsKey("sql") ? args["sql"] as string : null;
                    if (string.IsNullOrWhiteSpace(sql))
                        return "Lỗi: tham số 'sql' bị thiếu.";

                    // Bảo mật: chỉ SELECT / WITH
                    string upper = sql.TrimStart().ToUpperInvariant();
                    if (!upper.StartsWith("SELECT") && !upper.StartsWith("WITH"))
                        return "Lỗi bảo mật: chỉ được phép dùng SELECT hoặc WITH.";

                    return ExecuteSelect(sql);
                }
                return $"Lỗi: công cụ '{name}' không tồn tại.";
            }
            catch (Exception ex)
            {
                return $"Lỗi khi gọi công cụ: {ex.Message}";
            }
        }

        private string ExecuteSelect(string sql)
        {
            try
            {
                using (var conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (var cmd    = new SqlCommand(sql, conn) { CommandTimeout = 30 })
                    using (var reader = cmd.ExecuteReader())
                    {
                        var sb   = new StringBuilder();
                        var cols = new List<string>();
                        for (int i = 0; i < reader.FieldCount; i++)
                            cols.Add(reader.GetName(i));

                        sb.AppendLine(string.Join(" | ", cols));
                        sb.AppendLine(new string('-', Math.Min(cols.Count * 16, 120)));

                        int rows = 0;
                        while (reader.Read() && rows < 500)
                        {
                            var vals = new List<string>();
                            for (int i = 0; i < reader.FieldCount; i++)
                                vals.Add(reader.IsDBNull(i) ? "NULL" : reader.GetValue(i).ToString());
                            sb.AppendLine(string.Join(" | ", vals));
                            rows++;
                        }

                        if (rows == 0)   sb.AppendLine("(Không có dữ liệu)");
                        if (rows == 500) sb.AppendLine("(Đã giới hạn 500 dòng)");
                        return sb.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Lỗi SQL: {ex.Message}";
            }
        }

        // ─────────────────────────────────────────────────────
        //  Định nghĩa công cụ OpenAI
        // ─────────────────────────────────────────────────────

        private List<Dictionary<string, object>> BuildTools()
        {
            return new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    ["type"] = "function",
                    ["function"] = new Dictionary<string, object>
                    {
                        ["name"]        = "query_database",
                        ["description"] = "Thực thi câu lệnh SELECT trên CSDL SQL Server (quanlybanhang1) để lấy dữ liệu phục vụ trả lời. Chỉ dùng SELECT hoặc WITH. Có thể gọi nhiều lần.",
                        ["parameters"]  = new Dictionary<string, object>
                        {
                            ["type"] = "object",
                            ["properties"] = new Dictionary<string, object>
                            {
                                ["sql"] = new Dictionary<string, object>
                                {
                                    ["type"]        = "string",
                                    ["description"] = "Câu lệnh SELECT hợp lệ trên SQL Server"
                                }
                            },
                            ["required"] = new[] { "sql" }
                        }
                    }
                }
            };
        }

        // ─────────────────────────────────────────────────────
        //  System prompt theo cấp bậc
        // ─────────────────────────────────────────────────────

        private string BuildSystemPrompt()
        {
            string role     = (_user?.Role ?? "").ToLower();
            string roleName = role == "admin"     ? "Quản trị viên"
                            : role == "sales"     ? "Nhân viên bán hàng"
                            : role == "warehouse" ? "Nhân viên kho hàng"
                            : "Người dùng";

            string perms = role == "admin"
                ? "Toàn quyền: Thành phố, Khách hàng, Nhân viên, Sản phẩm, Hóa đơn, Chi tiết HĐ; Báo cáo đầy đủ; Cấu hình hệ thống; Quản lý người dùng."
                : role == "sales"
                ? "Xem/Quản lý: Thành phố, Khách hàng, Hóa đơn, Chi tiết HĐ. Báo cáo: KH theo TP, HĐ theo KH/SP. KHÔNG có: Nhân viên, Cấu hình hệ thống."
                : "Chỉ xem/quản lý: Sản phẩm. Không có quyền truy cập dữ liệu khác.";

            string funcs = role == "admin"
                ? "Hệ thống: Cấu hình, Quản lý người dùng, Đổi MK, Đăng xuất.\n" +
                  "Xem Danh mục: Thành phố, Khách hàng, Nhân viên, Sản phẩm, Hóa đơn, Chi tiết HĐ.\n" +
                  "Quản lý đơn: tất cả danh mục.\n" +
                  "Báo cáo: KH theo TP, HĐ theo KH/SP/NV, Chi tiết HĐ theo NV."
                : role == "sales"
                ? "Xem Danh mục: Thành phố, Khách hàng, Hóa đơn, Chi tiết HĐ.\n" +
                  "Quản lý đơn: Thành phố, Khách hàng, Hóa đơn, Chi tiết HĐ.\n" +
                  "Báo cáo: KH theo TP, HĐ theo KH, HĐ theo SP."
                : "Xem Danh mục: Sản phẩm.\nQuản lý đơn: Sản phẩm.";

            return
$@"Bạn là trợ lý AI thông minh của phần mềm **Quản Lý Bán Hàng**. Luôn trả lời bằng tiếng Việt, rõ ràng, thân thiện, ngắn gọn.

## Người dùng hiện tại
- Tên: {_user?.HoTen ?? "Chưa đăng nhập"}
- Cấp bậc: {roleName}
- Quyền hạn: {perms}

## Tính năng được phép
{funcs}

## Cơ sở dữ liệu (quanlybanhang1 – SQL Server)
| Bảng              | Cột chính                                                                 |
|-------------------|---------------------------------------------------------------------------|
| THANHPHO          | ThanhPho (PK), TenThanhPho                                                |
| KHACHHANG         | MaKH (PK), TenCty, DiaChi, ThanhPho (FK), DienThoai                      |
| NHANVIEN          | MaNV (PK), Ho, Ten, Nu, NgayNV, DiaChi, DienThoai, Username, Matkhau, Role|
| SANPHAM           | MaSP (PK), TenSP, DonViTinh, DonGia, Hinh                                |
| HOADON            | MaHD (PK), MaKH (FK), MaNV (FK), NgayLapHD, NgayNhanHang                 |
| CHITIETHOADON     | MaHD+MaSP (PK), SoLuong                                                   |

## Hướng dẫn
- Khi cần dữ liệu thực tế, dùng công cụ `query_database` (chỉ SELECT/WITH).
- Chỉ trả lời trong phạm vi quyền hạn của người dùng.
- Nếu hỏi ngoài quyền hạn, thông báo lịch sự và gợi ý liên hệ admin.";
        }

        // ─────────────────────────────────────────────────────
        //  Gọi OpenAI API
        // ─────────────────────────────────────────────────────

        private async Task<string> CallOpenAIAsync(string requestJson)
        {
            string key = ApiKey;
            if (string.IsNullOrWhiteSpace(key))
                throw new Exception(
                    "Chưa cấu hình OpenAI API Key.\n" +
                    "Vào  Hệ thống → Cấu hình hệ thống  để nhập API Key.\n" +
                    "(Khóa được lưu trong file dbconfig.ini cùng thư mục exe)");

            using (var req = new HttpRequestMessage(
                       HttpMethod.Post,
                       "https://api.openai.com/v1/chat/completions"))
            {
                req.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", key);
                req.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");

                var resp = await _http.SendAsync(req);
                string body = await resp.Content.ReadAsStringAsync();

                if (!resp.IsSuccessStatusCode)
                    throw new Exception(
                        $"OpenAI lỗi ({(int)resp.StatusCode}): {ExtractError(body)}");

                return body;
            }
        }

        private string ExtractError(string json)
        {
            try
            {
                var d = _json.Deserialize<Dictionary<string, object>>(json);
                if (d.ContainsKey("error"))
                {
                    var e = d["error"] as Dictionary<string, object>;
                    if (e != null && e.ContainsKey("message"))
                        return (string)e["message"];
                }
            }
            catch { }
            return json.Length > 300 ? json.Substring(0, 300) + "..." : json;
        }
    }
}
