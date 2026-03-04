using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using QuanLyNhanVien.DAL;
using QuanLyNhanVien.Models;

namespace QuanLyNhanVien.BLL
{
    public class AuthBLL
    {
        private readonly UserDAL _dal = new UserDAL();

        // ── Hash mật khẩu SHA256 ──────────────────────────────────────
        public static string HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                var sb = new StringBuilder();
                foreach (var b in bytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        // ── A1: Xác thực đăng nhập ────────────────────────────────────
        /// <returns>(user, logID, errorMsg)</returns>
        public (UserModel user, int logID, string error) Login(string username, string password)
        {
            try
            {
                var hash = HashPassword(password);
                var user = _dal.AuthLogin(username, hash);
                var ip   = GetLocalIP();

                if (user == null)
                {
                    // Tăng retry cho username tồn tại, hoặc giả lập retry
                    var (retry, locked) = _dal.IncrementRetry(username);
                    var logId = _dal.LogLogin(0, ip, false, $"Sai tài khoản/MK - retry={retry}");
                    var msg   = locked
                        ? "Tài khoản bị khóa sau nhiều lần đăng nhập sai."
                        : $"Sai tên đăng nhập hoặc mật khẩu. (Lần thử: {retry}/3)";
                    return (null, logId, msg);
                }

                if (!user.IsActive)
                    return (null, 0, "Tài khoản không còn hoạt động.");

                if (user.IsLocked)
                    return (null, 0, "Tài khoản bị khóa. Vui lòng liên hệ Admin.");

                // Đăng nhập thành công
                _dal.ResetRetry(user.UserID);
                var successLogId = _dal.LogLogin(user.UserID, ip, true, "Đăng nhập thành công");
                return (user, successLogId, null);
            }
            catch (Exception ex)
            {
                return (null, 0, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        // ── A6: Đổi mật khẩu lần đầu ─────────────────────────────────
        public (bool ok, string error) ChangePasswordFirstLogin(int userID, string oldPassword, string newPassword, string confirmPassword)
        {
            var validation = ValidateNewPassword(newPassword, confirmPassword);
            if (validation != null) return (false, validation);

            try
            {
                var newHash = HashPassword(newPassword);
                var ok = _dal.ChangePasswordFirstLogin(userID, newHash);
                return ok ? (true, null) : (false, "Không thể cập nhật mật khẩu.");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        // ── A5: Tìm user theo email ───────────────────────────────────
        public (UserModel user, string error) FindUserByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return (null, "Vui lòng nhập địa chỉ email.");

            if (!IsValidEmail(email))
                return (null, "Địa chỉ email không hợp lệ.");

            try
            {
                var user = _dal.FindUserByEmail(email.Trim());
                if (user == null)
                    return (null, "Không tìm thấy tài khoản với email này.");

                return (user, null);
            }
            catch (Exception ex)
            {
                return (null, $"Lỗi: {ex.Message}");
            }
        }

        // ── A7: Tạo và gửi OTP ───────────────────────────────────────
        public (string otpCode, string error) CreateAndSendOTP(int userID, string email)
        {
            try
            {
                var otp = GenerateOTP();
                _dal.CreateOTP(userID, otp, 10);

                // Trong thực tế: gửi email qua SMTP / SMS API
                // Hiện tại: hiện OTP lên MessageBox (demo)
                return (otp, null);
            }
            catch (Exception ex)
            {
                return (null, $"Lỗi gửi OTP: {ex.Message}");
            }
        }

        // ── A7: Xác thực OTP ─────────────────────────────────────────
        public (bool ok, string error) VerifyOTP(int userID, string otpCode)
        {
            if (string.IsNullOrWhiteSpace(otpCode))
                return (false, "Vui lòng nhập mã OTP.");

            try
            {
                var valid = _dal.VerifyOTP(userID, otpCode.Trim());
                return valid ? (true, null) : (false, "Mã OTP không đúng hoặc đã hết hạn.");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        // ── Reset mật khẩu sau OTP ───────────────────────────────────
        public (bool ok, string error) ResetPassword(int userID, string newPassword, string confirmPassword)
        {
            var validation = ValidateNewPassword(newPassword, confirmPassword);
            if (validation != null) return (false, validation);

            try
            {
                var hash = HashPassword(newPassword);
                var ok = _dal.ResetPassword(userID, hash);
                return ok ? (true, null) : (false, "Không thể đặt lại mật khẩu.");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        // ── Logout ───────────────────────────────────────────────────
        public void Logout(int logID)
        {
            if (logID > 0)
                _dal.LogLogout(logID);
            SessionModel.Clear();
        }

        // ── Helpers ──────────────────────────────────────────────────
        private string ValidateNewPassword(string newPw, string confirmPw)
        {
            if (string.IsNullOrWhiteSpace(newPw))
                return "Mật khẩu mới không được để trống.";
            if (newPw.Length < 8)
                return "Mật khẩu phải có ít nhất 8 ký tự.";
            if (!Regex.IsMatch(newPw, @"[A-Z]"))
                return "Mật khẩu phải chứa ít nhất 1 chữ hoa.";
            if (!Regex.IsMatch(newPw, @"[0-9]"))
                return "Mật khẩu phải chứa ít nhất 1 chữ số.";
            if (!Regex.IsMatch(newPw, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]"))
                return "Mật khẩu phải chứa ít nhất 1 ký tự đặc biệt.";
            if (newPw != confirmPw)
                return "Xác nhận mật khẩu không khớp.";
            return null;
        }

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private string GenerateOTP()
        {
            var rng = new Random();
            return rng.Next(100000, 999999).ToString();
        }

        private string GetLocalIP()
        {
            try
            {
                return System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName())
                    .AddressList[0]?.ToString() ?? "127.0.0.1";
            }
            catch { return "127.0.0.1"; }
        }
    }
}
