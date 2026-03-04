namespace QuanLyNhanVien.Models
{
    /// <summary>
    /// Singleton giữ thông tin phiên đăng nhập hiện tại.
    /// </summary>
    public static class SessionModel
    {
        public static int    UserID      { get; set; }
        public static string Username    { get; set; }
        public static string HoTen       { get; set; }
        public static string Role        { get; set; }
        public static string Email       { get; set; }
        public static string MaNhanVien  { get; set; }
        public static int    LoginLogID  { get; set; }   // ID bản ghi LoginHistory
        public static bool   IsLoggedIn  { get; set; }

        public static void SetSession(UserModel user, int logID)
        {
            UserID     = user.UserID;
            Username   = user.Username;
            HoTen      = user.HoTen;
            Role       = user.Role;
            Email      = user.Email;
            MaNhanVien = user.MaNhanVien;
            LoginLogID = logID;
            IsLoggedIn = true;
        }

        public static void Clear()
        {
            UserID     = 0;
            Username   = null;
            HoTen      = null;
            Role       = null;
            Email      = null;
            MaNhanVien = null;
            LoginLogID = 0;
            IsLoggedIn = false;
        }

        public static bool HasPermission(string requiredRole)
        {
            if (Role == "ADMIN") return true;
            if (requiredRole == "MANAGER" && (Role == "MANAGER")) return true;
            if (requiredRole == "HR_STAFF" && (Role == "MANAGER" || Role == "HR_STAFF")) return true;
            return Role == requiredRole;
        }
    }
}
