using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuanLyNhanVien.BLL;
using QuanLyNhanVien.Models;
using QuanLyNhanVien.StateMachine;

namespace QuanLyNhanVien.Forms
{
    /// <summary>
    /// A6: CHANGE_PW_FIRST — Đổi mật khẩu bắt buộc (lần đăng nhập đầu tiên)
    /// Cũng dùng lại cho reset password sau OTP.
    /// </summary>
    public partial class frmChangePassword : Form
    {
        private readonly AuthBLL          _bll;
        private readonly AuthStateMachine _sm;
        private readonly bool             _isFirstLogin;

        public frmChangePassword(AuthStateMachine sm, AuthBLL bll, bool isFirstLogin)
        {
            _sm           = sm;
            _bll          = bll;
            _isFirstLogin = isFirstLogin;
            InitializeComponent();
            _sm.StateChanged += OnStateChanged;

            SetupUI();
        }

        private void SetupUI()
        {
            if (_isFirstLogin)
            {
                lbl_Title.Text    = "Đổi Mật Khẩu Bắt Buộc";
                lbl_Subtitle.Text = $"Xin chào {SessionModel.HoTen}! Đây là lần đầu đăng nhập.\nVui lòng đổi mật khẩu trước khi tiếp tục.";
                lbl_OldPWHint.Visible = false;
                txt_OldPW.Visible     = false;
                this.ControlBox       = false;  // Không cho đóng form khi first login
            }
            else
            {
                lbl_Title.Text    = "Đổi Mật Khẩu";
                lbl_Subtitle.Text = "Vui lòng nhập mật khẩu cũ và mật khẩu mới.";
            }
        }

        // ── STATE HANDLER ─────────────────────────────────────────────
        private void OnStateChanged(AuthState from, AuthState to)
        {
            if (InvokeRequired) { Invoke(new Action(() => OnStateChanged(from, to))); return; }

            if (to == AuthState.AUTHENTICATED)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        // ── SAVE ──────────────────────────────────────────────────────
        private async void btn_Save_Click(object sender, EventArgs e)
        {
            ClearError();
            SetControlsEnabled(false);

            bool ok;
            string error;

            if (_isFirstLogin)
            {
                (ok, error) = await Task.Run(() =>
                    _bll.ChangePasswordFirstLogin(
                        SessionModel.UserID,
                        txt_OldPW.Text,
                        txt_NewPW.Text,
                        txt_ConfirmPW.Text));
            }
            else
            {
                (ok, error) = await Task.Run(() =>
                    _bll.ResetPassword(
                        SessionModel.UserID,
                        txt_NewPW.Text,
                        txt_ConfirmPW.Text));
            }

            SetControlsEnabled(true);

            if (!ok)
            {
                ShowError(error);
                return;
            }

            MessageBox.Show("Mật khẩu đã được đổi thành công!", "Thành công",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            _sm.TriggerPasswordChanged();   // → AUTHENTICATED
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            if (_isFirstLogin)
            {
                MessageBox.Show("Bạn phải đổi mật khẩu trước khi sử dụng hệ thống.",
                    "Bắt buộc", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void chk_ShowPW_CheckedChanged(object sender, EventArgs e)
        {
            bool show = chk_ShowPW.Checked;
            txt_OldPW.UseSystemPasswordChar     = !show;
            txt_NewPW.UseSystemPasswordChar     = !show;
            txt_ConfirmPW.UseSystemPasswordChar = !show;
        }

        // ── UI HELPERS ────────────────────────────────────────────────
        private void SetControlsEnabled(bool enabled)
        {
            txt_OldPW.Enabled     = enabled;
            txt_NewPW.Enabled     = enabled;
            txt_ConfirmPW.Enabled = enabled;
            btn_Save.Enabled      = enabled;
            btn_Cancel.Enabled    = enabled && !_isFirstLogin;
        }

        private void ShowError(string msg)
        {
            lbl_Error.ForeColor = Color.FromArgb(248, 113, 113);
            lbl_Error.Text      = msg;
            lbl_Error.Visible   = true;
        }

        private void ClearError()
        {
            lbl_Error.Visible = false;
            lbl_Error.Text    = string.Empty;
        }
    }
}
