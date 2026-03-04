namespace QuanLyNhanVien.StateMachine
{
    /// <summary>
    /// Tất cả trạng thái trong Phase 1 — Authentication
    /// </summary>
    public enum AuthState
    {
        LOGIN_FORM,        // A0: Màn hình đăng nhập
        AUTHENTICATING,    // A1: Đang xác thực
        AUTH_SUCCESS,      // A2: Xác thực thành công
        AUTH_FAILED,       // A3: Xác thực thất bại
        ACCOUNT_LOCKED,    // A4: Tài khoản bị khóa
        FORGOT_PASSWORD,   // A5: Quên mật khẩu — nhập email
        SEND_RESET_OTP,    // A7: Gửi OTP
        VERIFY_OTP,        // A7b: Xác thực OTP
        CHANGE_PW_FIRST,   // A6: Đổi mật khẩu lần đầu (bắt buộc)
        AUTHENTICATED      // → Chuyển sang Phase 2
    }
}
