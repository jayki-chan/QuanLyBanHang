using System;
using System.Collections.Generic;

namespace QuanLyBanHang_DTO
{
    /// <summary>Lưu thông tin một tin nhắn trong cuộc hội thoại với AI.</summary>
    public class ChatMessageDTO
    {
        public int      Id        { get; set; }
        public int      SessionId { get; set; }
        /// <summary>"user" hoặc "assistant"</summary>
        public string   Role      { get; set; }
        public string   Content   { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>Lưu thông tin một phiên hội thoại (session) với AI.</summary>
    public class ChatSessionDTO
    {
        public int      Id        { get; set; }
        public string   Username  { get; set; }
        /// <summary>Tiêu đề rút gọn từ tin nhắn đầu tiên.</summary>
        public string   Title     { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<ChatMessageDTO> Messages { get; set; } = new List<ChatMessageDTO>();

        public override string ToString()
        {
            string t = Title ?? "Hội thoại mới";
            if (t.Length > 42) t = t.Substring(0, 42) + "…";
            return $"[{CreatedAt:dd/MM HH:mm}]  {t}";
        }
    }
}
