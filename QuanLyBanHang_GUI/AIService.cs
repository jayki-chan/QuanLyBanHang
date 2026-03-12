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

namespace QuanLyBanHang_GUI
{
    /// <summary>
    /// Dịch vụ AI: gọi OpenAI Chat Completions API kết hợp truy vấn CSDL SQL Server
    /// thông qua cơ chế Function Calling (tool use).
    /// </summary>
    public class AIService
    {
        // HttpClient dùng chung (thread-safe)
        private static readonly HttpClient _http = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(120)
        };

        private readonly JavaScriptSerializer _json;
        private readonly NhanVienDTO          _user;
        private readonly List<object>         _history;   // Lịch sử hội thoại (trừ system)

        private static readonly string _configFile =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dbconfig.ini");

        // ── Đọc cấu hình từ dbconfig.ini ─────────────────────
        private string ApiKey  => ReadConfig("OpenAI_Key",   "");
        private string Model   => ReadConfig("OpenAI_Model", "gpt-4o-mini");

        private static string ReadConfig(string key, string defaultVal)
        {
            if (!File.Exists(_configFile)) return defaultVal;
            foreach (var line in File.ReadAllLines(_configFile))
            {
                var p = line.Split('=');
                if (p.Length >= 2 && p[0].Trim().Equals(key, StringComparison.OrdinalIgnoreCase))
                    return string.Join("=", p, 1, p.Length - 1).Trim();
            }
            return defaultVal;
        }

        public AIService(NhanVienDTO user)
        {
            _user    = user;
            _history = new List<object>();
            _json    = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
        }

        /// <summary>
        /// Gửi tin nhắn người dùng, chạy vòng lặp agentic (có thể gọi query_database nhiều lần),
        /// trả về câu trả lời cuối cùng.
        /// </summary>
        public async Task<string> SendMessageAsync(string userMessage)
        {
            _history.Add(new Dictionary<string, object>
            {
                ["role"]    = "user",
                ["content"] = userMessage
            });

            for (int iter = 0; iter < 8; iter++)
            {
                // Xây danh sách messages = system + toàn bộ lịch sử
                var messages = new List<object>();
                messages.Add(new Dictionary<string, object>
                {
                    ["role"]    = "system",
                    ["content"] = BuildSystemPrompt()
                });
                messages.AddRange(_history);

                var requestBody = new Dictionary<string, object>
                {
                    ["model"]       = Model,
                    ["messages"]    = messages,
                    ["tools"]       = BuildTools(),
                    ["tool_choice"] = "auto"
                };

                string responseJson = await CallOpenAIAsync(_json.Serialize(requestBody));
                var    response     = _json.Deserialize<Dictionary<string, object>>(responseJson);

                var    choices      = (ArrayList)response["choices"];
                var    choice       = (Dictionary<string, object>)choices[0];
                var    message      = (Dictionary<string, object>)choice["message"];
                string finishReason = (string)choice["finish_reason"];

                if (finishReason == "tool_calls")
                {
                    // AI yêu cầu thực thi công cụ → lưu lại message và thực thi
                    _history.Add(message);

                    var toolCalls = (ArrayList)message["tool_calls"];
                    foreach (Dictionary<string, object> tc in toolCalls)
                    {
                        string tcId   = (string)tc["id"];
                        var    func   = (Dictionary<string, object>)tc["function"];
                        string fname  = (string)func["name"];
                        string fargs  = (string)func["arguments"];
                        string result = ExecuteTool(fname, fargs);

                        _history.Add(new Dictionary<string, object>
                        {
                            ["role"]         = "tool",
                            ["tool_call_id"] = tcId,
                            ["content"]      = result
                        });
                    }
                }
                else
                {
                    // Phản hồi văn bản cuối cùng
                    object contentObj = message.ContainsKey("content") ? message["content"] : null;
                    string content    = contentObj as string ?? "(Không có nội dung)";

                    _history.Add(new Dictionary<string, object>
                    {
                        ["role"]    = "assistant",
                        ["content"] = content
                    });
                    return content;
                }
            }

            return "Xin lỗi, không thể hoàn thành yêu cầu sau nhiều lần thử.";
        }

        /// <summary>Xóa toàn bộ lịch sử hội thoại.</summary>
        public void ClearHistory() => _history.Clear();

        // ─────────────────────────────────────────────────────
        //  Thực thi công cụ
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

                    // Bảo mật: chỉ cho phép SELECT / WITH (CTE)
                    string upper = sql.TrimStart().ToUpperInvariant();
                    if (!upper.StartsWith("SELECT") && !upper.StartsWith("WITH"))
                        return "Lỗi bảo mật: chỉ được phép dùng câu lệnh SELECT hoặc WITH.";

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
        //  Định nghĩa công cụ (OpenAI Tools)
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
                        ["description"] = "Thực thi câu lệnh SELECT trên CSDL SQL Server (quanlybanhang1) để lấy dữ liệu phục vụ trả lời. Chỉ dùng SELECT hoặc WITH (CTE). Có thể gọi nhiều lần nếu cần.",
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
        //  System Prompt theo cấp bậc tài khoản
        // ─────────────────────────────────────────────────────

        private string BuildSystemPrompt()
        {
            string role     = (_user?.Role ?? "").ToLower();
            string roleName = role == "admin"     ? "Quản trị viên"
                            : role == "sales"     ? "Nhân viên bán hàng"
                            : role == "warehouse" ? "Nhân viên kho hàng"
                            : "Người dùng";

            string perms = role == "admin"
                ? "Toàn quyền: xem và quản lý Thành phố, Khách hàng, Nhân viên, Sản phẩm, Hóa đơn, Chi tiết hóa đơn; Báo cáo đầy đủ; Cấu hình hệ thống; Quản lý người dùng."
                : role == "sales"
                ? "Xem/Quản lý: Thành phố, Khách hàng, Hóa đơn, Chi tiết hóa đơn. Báo cáo: KH theo TP, HĐ theo KH/SP. KHÔNG có quyền: Nhân viên, Cấu hình hệ thống."
                : "Chỉ xem/quản lý: Sản phẩm. Không có quyền truy cập dữ liệu khác.";

            string funcs = role == "admin"
                ? @"Menu Hệ thống: Cấu hình hệ thống, Quản lý người dùng, Đổi mật khẩu, Đăng xuất, Thoát.
Menu Xem Danh mục: Thành phố, Khách hàng, Nhân viên, Sản phẩm, Hóa đơn, Chi tiết hóa đơn.
Menu Quản lý Danh mục đơn: Thành phố, Khách hàng, Nhân viên, Sản phẩm, Hóa đơn, Chi tiết hóa đơn.
Menu Quản lý theo nhóm (Báo cáo): Khách hàng theo Thành phố, Hóa đơn theo KH/SP/NV, Chi tiết HĐ theo NV.
Menu Giúp đỡ: Chat với AI, Hướng dẫn sử dụng, Tác giả."
                : role == "sales"
                ? @"Menu Xem Danh mục: Thành phố, Khách hàng, Hóa đơn, Chi tiết hóa đơn.
Menu Quản lý Danh mục đơn: Thành phố, Khách hàng, Hóa đơn, Chi tiết hóa đơn.
Menu Quản lý theo nhóm (Báo cáo): Khách hàng theo Thành phố, Hóa đơn theo KH, Hóa đơn theo SP.
Menu Giúp đỡ: Chat với AI, Hướng dẫn sử dụng, Tác giả."
                : @"Menu Xem Danh mục: Sản phẩm.
Menu Quản lý Danh mục đơn: Sản phẩm.
Menu Giúp đỡ: Chat với AI, Hướng dẫn sử dụng, Tác giả.";

            return
$@"Bạn là trợ lý AI thông minh của phần mềm **Quản Lý Bán Hàng**. Luôn trả lời bằng tiếng Việt, rõ ràng, thân thiện, ngắn gọn.

## Thông tin người dùng hiện tại
- Tên: {(_user?.HoTen ?? "Chưa đăng nhập")}
- Cấp bậc: {roleName}
- Quyền hạn: {perms}

## Tính năng theo cấp bậc
{funcs}

## Cơ sở dữ liệu SQL Server (database: quanlybanhang1)
| Bảng           | Các cột chính                                                                                  |
|----------------|-----------------------------------------------------------------------------------------------|
| THANHPHO       | ThanhPho (PK VARCHAR), TenThanhPho (NVARCHAR)                                                  |
| KHACHHANG      | MaKH (PK), TenCty (NVARCHAR), DiaChi (NVARCHAR), ThanhPho (FK→THANHPHO), DienThoai           |
| NHANVIEN       | MaNV (PK), Ho, Ten, Nu (BIT), NgayNV (DATE), DiaChi, DienThoai, Hinh, Username, Matkhau, Role |
| SANPHAM        | MaSP (PK), TenSP (NVARCHAR), DonViTinh, DonGia (DECIMAL), Hinh                               |
| HOADON         | MaHD (PK), MaKH (FK), MaNV (FK), NgayLapHD (DATE), NgayNhanHang (DATE)                       |
| CHITIETHOADON  | MaHD+MaSP (PK), SoLuong (INT)                                                                 |

## Hướng dẫn quan trọng
- Khi cần số liệu thực tế từ CSDL để trả lời, hãy dùng công cụ `query_database`.
- Chỉ viết câu lệnh SELECT (không INSERT/UPDATE/DELETE/DROP/...).
- Chỉ cung cấp thông tin phù hợp quyền hạn của người dùng.
- Nếu bị hỏi về chức năng ngoài quyền hạn, thông báo lịch sự và gợi ý liên hệ admin.
- Hỗ trợ: tra cứu dữ liệu, giải thích cách dùng tính năng, thống kê, tư vấn nghiệp vụ.";
        }

        // ─────────────────────────────────────────────────────
        //  Gọi OpenAI API
        // ─────────────────────────────────────────────────────

        private async Task<string> CallOpenAIAsync(string requestJson)
        {
            string apiKey = ApiKey;
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new Exception(
                    "Chưa cấu hình OpenAI API Key.\n" +
                    "Vào menu  Hệ thống → Cấu hình hệ thống  để nhập API Key.");

            using (var req = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.openai.com/v1/chat/completions"))
            {
                req.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
                req.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");

                var resp = await _http.SendAsync(req);
                string body = await resp.Content.ReadAsStringAsync();

                if (!resp.IsSuccessStatusCode)
                    throw new Exception(
                        $"Lỗi OpenAI ({(int)resp.StatusCode}): {ExtractErrorMessage(body)}");

                return body;
            }
        }

        private string ExtractErrorMessage(string json)
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
