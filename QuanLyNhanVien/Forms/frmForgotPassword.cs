using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuanLyNhanVien.BLL;
using QuanLyNhanVien.StateMachine;

namespace QuanLyNhanVien.Forms
{
    /// <summary>
    /// A5: FORGOT_PASSWORD + A7: SEND_RESET_OTP + VERIFY_OTP
    /// Flow: Nhập email → Gửi OTP → Xác nhận OTP → Đổi mật khẩu mới
    /// </summary>
    public partial class frmForgotPassword : Form
    {
        private readonly AuthBLL          _bll;
        private readonly AuthStateMachine _sm;
        private string _pendingOTP;   // demo only — thực tế không lưu ở client

        public frmForgotPassword(AuthStateMachine sm, AuthBLL bll)
        {
            _sm  = sm;
            _bll = bll;
            InitializeComponent();
            _sm.StateChanged += OnStateChanged;

            // Bắt đầu tại A5
            ApplyState(AuthState.FORGOT_PASSWORD);
        }

        // ══════════════════════════════════════════════════════════════
        // STATE HANDLER
        // ══════════════════════════════════════════════════════════════
        private void OnStateChanged(AuthState from, AuthState to)
        {
            if (InvokeRequired) { Invoke(new Action(() => OnStateChanged(from, to))); return; }
            ApplyState(to);
        }

        private void ApplyState(AuthState state)
        {
            switch (state)
            {
                case AuthState.FORGOT_PASSWORD:
                    ShowPanel(pnl_Email);
                    lbl_Step.Text = "Bước 1/3 — Nhập email đã đăng ký";
                    ClearError();
                    txt_Email.Focus();
                    break;

                case AuthState.SEND_RESET_OTP:
                    ShowPanel(pnl_OTP);
                    lbl_Step.Text = "Bước 2/3 — Gửi mã OTP";
                    SetPanelEnabled(pnl_OTP, false);
                    lbl_OtpInfo.Text = "Đang gửi mã OTP...";
                    break;

                case AuthState.VERIFY_OTP:
                    SetPanelEnabled(pnl_OTP, true);
                    lbl_OtpInfo.Text = $"Mã OTP đã gửi đến email: {_sm.CurrentUser?.Email}";
                    lbl_Step.Text    = "Bước 2/3 — Xác nhận mã OTP";
                    txt_OTP.Focus();
                    break;

                case AuthState.CHANGE_PW_FIRST:
                    ShowPanel(pnl_NewPW);
                    lbl_Step.Text = "Bước 3/3 — Đặt mật khẩu mới";
                    txt_NewPW.Focus();
                    break;

                case AuthState.AUTHENTICATED:
                    MessageBox.Show("Mật khẩu đã được đặt lại thành công!\nVui lòng đăng nhập lại.",
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    break;
            }
        }

        // ══════════════════════════════════════════════════════════════
        // PANEL: EMAIL (A5)
        // ══════════════════════════════════════════════════════════════
        private async void btn_FindAccount_Click(object sender, EventArgs e)
        {
            ClearError();
            var email = txt_Email.Text.Trim();

            var (user, error) = _bll.FindUserByEmail(email);
            if (error != null) { ShowError(error); return; }

            _sm.TriggerSendOTP(user);   // → SEND_RESET_OTP

            // Gửi OTP async
            var (otp, otpError) = await Task.Run(() => _bll.CreateAndSendOTP(user.UserID, user.Email));
            if (otpError != null) { ShowError(otpError); return; }

            _pendingOTP = otp;  // demo: hiển thị OTP (thực tế gửi email/SMS)
            MessageBox.Show($"[DEMO] Mã OTP của bạn là: {otp}\n(Có hiệu lực trong 10 phút)",
                "OTP Gửi Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Information);

            _sm.TriggerOTPSent();   // → VERIFY_OTP
        }

        // ══════════════════════════════════════════════════════════════
        // PANEL: OTP (A7)
        // ══════════════════════════════════════════════════════════════
        private async void btn_VerifyOTP_Click(object sender, EventArgs e)
        {
            ClearError();
            var otpCode = txt_OTP.Text.Trim();

            var (ok, error) = await Task.Run(() =>
                _bll.VerifyOTP(_sm.CurrentUser.UserID, otpCode));

            if (!ok) { ShowError(error); return; }

            _sm.TriggerOTPVerified();   // → CHANGE_PW_FIRST
        }

        private void btn_ResendOTP_Click(object sender, EventArgs e)
        {
            _sm.TriggerSendOTP(_sm.CurrentUser);
            btn_FindAccount_Click(sender, e);
        }

        // ══════════════════════════════════════════════════════════════
        // PANEL: NEW PASSWORD
        // ══════════════════════════════════════════════════════════════
        private async void btn_SetNewPW_Click(object sender, EventArgs e)
        {
            ClearError();

            var (ok, error) = await Task.Run(() =>
                _bll.ResetPassword(_sm.CurrentUser.UserID, txt_NewPW.Text, txt_ConfirmPW.Text));

            if (!ok) { ShowError(error); return; }

            _sm.TriggerPasswordChanged();   // → AUTHENTICATED → đóng form
        }

        // ══════════════════════════════════════════════════════════════
        // SHOW/HIDE PASSWORD
        // ══════════════════════════════════════════════════════════════
        private void chk_ShowNewPW_CheckedChanged(object sender, EventArgs e)
        {
            txt_NewPW.UseSystemPasswordChar     = !chk_ShowNewPW.Checked;
            txt_ConfirmPW.UseSystemPasswordChar = !chk_ShowNewPW.Checked;
        }

        // ══════════════════════════════════════════════════════════════
        // NAVIGATION
        // ══════════════════════════════════════════════════════════════
        private void btn_Back_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // ══════════════════════════════════════════════════════════════
        // UI HELPERS
        // ══════════════════════════════════════════════════════════════
        private void ShowPanel(Panel panel)
        {
            pnl_Email.Visible = (panel == pnl_Email);
            pnl_OTP.Visible   = (panel == pnl_OTP);
            pnl_NewPW.Visible = (panel == pnl_NewPW);
            ClearError();
        }

        private void SetPanelEnabled(Panel panel, bool enabled)
        {
            foreach (Control ctrl in panel.Controls)
                ctrl.Enabled = enabled;
        }

        private void ShowError(string message)
        {
            lbl_Error.ForeColor = Color.FromArgb(248, 113, 113);
            lbl_Error.Text      = message;
            lbl_Error.Visible   = true;
        }

        private void ClearError()
        {
            lbl_Error.Visible = false;
            lbl_Error.Text    = string.Empty;
        }
    }
}
