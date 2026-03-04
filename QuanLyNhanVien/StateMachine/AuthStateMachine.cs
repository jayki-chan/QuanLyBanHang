using System;
using QuanLyNhanVien.Models;

namespace QuanLyNhanVien.StateMachine
{
    /// <summary>
    /// State Machine xác thực — Phase 1.
    /// Quản lý tất cả chuyển trạng thái từ A0 → A7 / AUTHENTICATED.
    /// </summary>
    public class AuthStateMachine
    {
        public AuthState    CurrentState { get; private set; }
        public UserModel    CurrentUser  { get; private set; }
        public string       ErrorMessage { get; private set; }
        public int          RetryCount   { get; private set; }

        private const int MAX_RETRY = 3;

        // Sự kiện thông báo cho Form khi trạng thái thay đổi
        public event Action<AuthState, AuthState> StateChanged;  // (from, to)

        public AuthStateMachine()
        {
            CurrentState = AuthState.LOGIN_FORM;
        }

        // ── Chuyển trạng thái nội bộ ─────────────────────────────────
        private void Transition(AuthState newState)
        {
            var prev = CurrentState;
            CurrentState = newState;
            StateChanged?.Invoke(prev, newState);
        }

        // ══════════════════════════════════════════════════════════════
        // EVENTS — gọi từ Form khi user thực hiện hành động
        // ══════════════════════════════════════════════════════════════

        /// <summary>
        /// A0 → A1: Người dùng bấm nút Đăng Nhập
        /// Guard: username & password không rỗng
        /// </summary>
        public bool TriggerLogin(string username, string password)
        {
            if (CurrentState != AuthState.LOGIN_FORM) return false;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ErrorMessage = "Vui lòng nhập tên đăng nhập và mật khẩu.";
                return false;
            }

            ErrorMessage = null;
            Transition(AuthState.AUTHENTICATING);
            return true;
        }

        /// <summary>
        /// A1 → A2: Xác thực thành công
        /// </summary>
        public void TriggerAuthSuccess(UserModel user, int logID)
        {
            if (CurrentState != AuthState.AUTHENTICATING) return;

            CurrentUser  = user;
            RetryCount   = 0;
            ErrorMessage = null;

            SessionModel.SetSession(user, logID);
            Transition(AuthState.AUTH_SUCCESS);
        }

        /// <summary>
        /// A1 → A3: Xác thực thất bại
        /// A3 → A0 hoặc A4 tuỳ retry count
        /// </summary>
        public void TriggerAuthFailed(int currentRetry, bool isLocked)
        {
            if (CurrentState != AuthState.AUTHENTICATING) return;

            RetryCount   = currentRetry;
            ErrorMessage = isLocked
                ? $"Tài khoản bị khóa sau {MAX_RETRY} lần đăng nhập sai. Liên hệ Admin."
                : $"Sai tên đăng nhập hoặc mật khẩu. Còn {MAX_RETRY - currentRetry} lần thử.";

            Transition(isLocked ? AuthState.ACCOUNT_LOCKED : AuthState.AUTH_FAILED);

            // A3 → A0 tự động nếu chưa bị khóa
            if (!isLocked)
                Transition(AuthState.LOGIN_FORM);
        }

        /// <summary>
        /// A2 → A6: Phát hiện first_login = true
        /// </summary>
        public void TriggerFirstLogin()
        {
            if (CurrentState != AuthState.AUTH_SUCCESS) return;
            Transition(AuthState.CHANGE_PW_FIRST);
        }

        /// <summary>
        /// A2 → AUTHENTICATED: Đăng nhập hoàn tất
        /// </summary>
        public void TriggerEnterMain()
        {
            if (CurrentState != AuthState.AUTH_SUCCESS) return;
            Transition(AuthState.AUTHENTICATED);
        }

        /// <summary>
        /// A6 → AUTHENTICATED: Đổi mật khẩu thành công
        /// </summary>
        public void TriggerPasswordChanged()
        {
            if (CurrentState != AuthState.CHANGE_PW_FIRST) return;
            Transition(AuthState.AUTHENTICATED);
        }

        /// <summary>
        /// A0 → A5: Người dùng bấm "Quên mật khẩu?"
        /// </summary>
        public void TriggerForgotPassword()
        {
            if (CurrentState != AuthState.LOGIN_FORM) return;
            Transition(AuthState.FORGOT_PASSWORD);
        }

        /// <summary>
        /// A5 → A7: Gửi OTP (sau khi tìm thấy email)
        /// </summary>
        public void TriggerSendOTP(UserModel foundUser)
        {
            if (CurrentState != AuthState.FORGOT_PASSWORD) return;
            CurrentUser = foundUser;
            Transition(AuthState.SEND_RESET_OTP);
        }

        /// <summary>
        /// A7 → VERIFY_OTP: OTP đã gửi, chờ xác thực
        /// </summary>
        public void TriggerOTPSent()
        {
            if (CurrentState != AuthState.SEND_RESET_OTP) return;
            Transition(AuthState.VERIFY_OTP);
        }

        /// <summary>
        /// VERIFY_OTP → A6: OTP đúng, cho đặt mật khẩu mới
        /// </summary>
        public void TriggerOTPVerified()
        {
            if (CurrentState != AuthState.VERIFY_OTP) return;
            Transition(AuthState.CHANGE_PW_FIRST);
        }

        /// <summary>
        /// Bất kỳ trạng thái → A0: Quay lại màn hình đăng nhập
        /// </summary>
        public void TriggerBackToLogin()
        {
            CurrentUser  = null;
            ErrorMessage = null;
            RetryCount   = 0;
            Transition(AuthState.LOGIN_FORM);
        }
    }
}
