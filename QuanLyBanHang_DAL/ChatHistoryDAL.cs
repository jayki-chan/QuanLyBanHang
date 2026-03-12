using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using QuanLyBanHang_DTO;

namespace QuanLyBanHang_DAL
{
    /// <summary>
    /// DAL: Thao tác CSDL cho lịch sử chat với AI.
    /// Bảng CHAT_HISTORY_SESSION và CHAT_HISTORY_MESSAGE.
    /// </summary>
    public class ChatHistoryDAL
    {
        // ── Tự tạo bảng nếu chưa tồn tại ─────────────────────
        public void EnsureTables()
        {
            const string sql = @"
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='CHAT_HISTORY_SESSION' AND xtype='U')
CREATE TABLE CHAT_HISTORY_SESSION (
    Id        INT IDENTITY(1,1) PRIMARY KEY,
    Username  VARCHAR(50)    NOT NULL DEFAULT '',
    Title     NVARCHAR(200)  NOT NULL DEFAULT N'Hội thoại mới',
    CreatedAt DATETIME       NOT NULL DEFAULT GETDATE()
);

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='CHAT_HISTORY_MESSAGE' AND xtype='U')
CREATE TABLE CHAT_HISTORY_MESSAGE (
    Id        INT IDENTITY(1,1) PRIMARY KEY,
    SessionId INT            NOT NULL REFERENCES CHAT_HISTORY_SESSION(Id) ON DELETE CASCADE,
    Role      VARCHAR(20)    NOT NULL,
    Content   NVARCHAR(MAX)  NOT NULL,
    CreatedAt DATETIME       NOT NULL DEFAULT GETDATE()
);";
            try
            {
                using (var conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(sql, conn))
                        cmd.ExecuteNonQuery();
                }
            }
            catch { /* Bỏ qua nếu không kết nối được */ }
        }

        // ── Lấy danh sách sessions theo user ──────────────────
        public List<ChatSessionDTO> GetSessions(string username)
        {
            var list = new List<ChatSessionDTO>();
            const string sql =
                "SELECT Id, Username, Title, CreatedAt " +
                "FROM CHAT_HISTORY_SESSION " +
                "WHERE Username = @u " +
                "ORDER BY CreatedAt DESC";
            try
            {
                using (var conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@u", username ?? "");
                        using (var rd = cmd.ExecuteReader())
                            while (rd.Read())
                                list.Add(new ChatSessionDTO
                                {
                                    Id        = (int)rd["Id"],
                                    Username  = rd["Username"].ToString(),
                                    Title     = rd["Title"].ToString(),
                                    CreatedAt = (DateTime)rd["CreatedAt"]
                                });
                    }
                }
            }
            catch { }
            return list;
        }

        // ── Lấy messages theo session ──────────────────────────
        public List<ChatMessageDTO> GetMessages(int sessionId)
        {
            var list = new List<ChatMessageDTO>();
            const string sql =
                "SELECT Id, SessionId, Role, Content, CreatedAt " +
                "FROM CHAT_HISTORY_MESSAGE " +
                "WHERE SessionId = @sid " +
                "ORDER BY CreatedAt ASC";
            try
            {
                using (var conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@sid", sessionId);
                        using (var rd = cmd.ExecuteReader())
                            while (rd.Read())
                                list.Add(new ChatMessageDTO
                                {
                                    Id        = (int)rd["Id"],
                                    SessionId = sessionId,
                                    Role      = rd["Role"].ToString(),
                                    Content   = rd["Content"].ToString(),
                                    CreatedAt = (DateTime)rd["CreatedAt"]
                                });
                    }
                }
            }
            catch { }
            return list;
        }

        // ── Tạo session mới, trả về Id ────────────────────────
        public int CreateSession(string username, string title)
        {
            const string sql =
                "INSERT INTO CHAT_HISTORY_SESSION (Username, Title) " +
                "OUTPUT INSERTED.Id VALUES (@u, @t)";
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@u", username ?? "");
                    cmd.Parameters.AddWithValue("@t", string.IsNullOrWhiteSpace(title) ? "Hội thoại mới" : title);
                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        // ── Lưu một tin nhắn ─────────────────────────────────
        public void SaveMessage(int sessionId, string role, string content)
        {
            const string sql =
                "INSERT INTO CHAT_HISTORY_MESSAGE (SessionId, Role, Content) " +
                "VALUES (@sid, @role, @content)";
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@sid",     sessionId);
                    cmd.Parameters.AddWithValue("@role",    role);
                    cmd.Parameters.AddWithValue("@content", content);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ── Xóa session (cascade xóa messages) ───────────────
        public void DeleteSession(int sessionId)
        {
            const string sql = "DELETE FROM CHAT_HISTORY_SESSION WHERE Id = @id";
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", sessionId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
