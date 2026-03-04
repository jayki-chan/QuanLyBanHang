using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuanLyNhanVien.BLL;
using QuanLyNhanVien.StateMachine;

namespace QuanLyNhanVien.Forms
{
    /// <summary>
    /// A0: LOGIN_FORM — Màn hình đăng nhập
    /// Điều phối trạng thái A0 → A1 → A2/A3/A4 → A5/A6/MAIN
    /// </summary>
    public partial class frmLogin : Form
    {
        private readonly AuthBLL          _bll = new AuthBLL();
        private readonly AuthStateMachine _sm  = new AuthStateMachine();

        public frmLogin()
        {
            InitializeComponent();
            _sm.StateChanged += OnStateChanged;
            ApplyState(AuthState.LOGIN_FORM);
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
                case AuthState.LOGIN_FORM:
                    SetFormEnabled(true);
                    ClearError();
                    txt_Password.Clear();
                    txt_Username.Focus();
                    break;

                case AuthState.AUTHENTICATING:
                    SetFormEnabled(false);
                    ShowInfo("Đang xác thực...");
                    break;

                case AuthState.AUTH_FAILED:
                    ShowError(_sm.ErrorMessage);
                    break;

                case AuthState.ACCOUNT_LOCKED:
                    SetFormEnabled(false);
                    ShowError(_sm.ErrorMessage);
                    btn_Login.Enabled = false;
                    break;

                case AuthState.AUTH_SUCCESS:
                    HandleAuthSuccess();
                    break;

                case AuthState.CHANGE_PW_FIRST:
                    OpenChangePassword();
                    break;

                case AuthState.AUTHENTICATED:
                    OpenMainDashboard();
                    break;

                case AuthState.FORGOT_PASSWORD:
                    OpenForgotPassword();
                    break;
            }
        }

        // ══════════════════════════════════════════════════════════════
        // EVENT HANDLERS
        // ══════════════════════════════════════════════════════════════

        // btn_Login → A0 → A1 → (A2 or A3)
        private async void btn_Login_Click(object sender, EventArgs e)
        {
            if (!_sm.TriggerLogin(txt_Username.Text.Trim(), txt_Password.Text))
            {
                ShowError(_sm.ErrorMessage);
                return;
            }

            // A1: Authenticating — chạy async để không block UI
            var (user, logID, error) = await Task.Run(() =>
                _bll.Login(txt_Username.Text.Trim(), txt_Password.Text));

            if (error != null)
            {
                // A3: Auth failed
                bool isLocked = error.Contains("bị khóa");
                _sm.TriggerAuthFailed(_sm.RetryCount + 1, isLocked);
                ShowError(error);
                return;
            }

            // A2: Auth success
            _sm.TriggerAuthSuccess(user, logID);
        }

        // Enter key → Login
        private void txt_Password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) btn_Login_Click(sender, e);
        }

        private void txt_Username_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) txt_Password.Focus();
        }

        // link Quên mật khẩu → A5
        private void lnk_ForgotPW_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _sm.TriggerForgotPassword();
        }

        // Show/hide mật khẩu
        private void chk_ShowPW_CheckedChanged(object sender, EventArgs e)
        {
            txt_Password.UseSystemPasswordChar = !chk_ShowPW.Checked;
        }

        // ══════════════════════════════════════════════════════════════
        // AUTH SUCCESS HANDLERS
        // ══════════════════════════════════════════════════════════════
        private void HandleAuthSuccess()
        {
            if (_sm.CurrentUser.FirstLogin)
                _sm.TriggerFirstLogin();      // → A6
            else
                _sm.TriggerEnterMain();       // → AUTHENTICATED
        }

        private void OpenChangePassword()
        {
            var frm = new frmChangePassword(_sm, _bll, isFirstLogin: true);
            frm.ShowDialog(this);
        }

        private void OpenForgotPassword()
        {
            var frm = new frmForgotPassword(_sm, _bll);
            frm.ShowDialog(this);
            // Sau khi đóng, quay lại A0
            if (_sm.CurrentState != AuthState.AUTHENTICATED)
                _sm.TriggerBackToLogin();
        }

        private void OpenMainDashboard()
        {
            // TODO Phase 2: mở frmDashboard
            MessageBox.Show(
                $"Chào mừng {Models.SessionModel.HoTen}!\nRole: {Models.SessionModel.Role}",
                "Đăng nhập thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Tạm thời: quay về Login sau demo
            // this.Hide();
            // var main = new frmDashboard();
            // main.Show();
        }

        // ══════════════════════════════════════════════════════════════
        // UI HELPERS
        // ══════════════════════════════════════════════════════════════
        private void SetFormEnabled(bool enabled)
        {
            txt_Username.Enabled  = enabled;
            txt_Password.Enabled  = enabled;
            btn_Login.Enabled     = enabled;
            lnk_ForgotPW.Enabled  = enabled;
            chk_ShowPW.Enabled    = enabled;
            chk_RememberMe.Enabled = enabled;

            pic_Loading.Visible   = !enabled;
        }

        private void ShowError(string message)
        {
            lbl_Error.ForeColor = Color.FromArgb(248, 113, 113);
            lbl_Error.Text      = message ?? string.Empty;
            lbl_Error.Visible   = !string.IsNullOrEmpty(message);
        }

        private void ShowInfo(string message)
        {
            lbl_Error.ForeColor = Color.FromArgb(148, 163, 184);
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
