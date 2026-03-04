namespace QuanLyNhanVien.Forms
{
    partial class frmForgotPassword
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label      lbl_Title;
        private System.Windows.Forms.Label      lbl_Step;
        private System.Windows.Forms.Label      lbl_Error;

        // Panel Email (A5)
        private System.Windows.Forms.Panel      pnl_Email;
        private System.Windows.Forms.Label      lbl_EmailHint;
        private System.Windows.Forms.TextBox    txt_Email;
        private System.Windows.Forms.Button     btn_FindAccount;

        // Panel OTP (A7)
        private System.Windows.Forms.Panel      pnl_OTP;
        private System.Windows.Forms.Label      lbl_OtpInfo;
        private System.Windows.Forms.Label      lbl_OtpHint;
        private System.Windows.Forms.TextBox    txt_OTP;
        private System.Windows.Forms.Button     btn_VerifyOTP;
        private System.Windows.Forms.Button     btn_ResendOTP;

        // Panel New Password
        private System.Windows.Forms.Panel      pnl_NewPW;
        private System.Windows.Forms.Label      lbl_NewPWHint;
        private System.Windows.Forms.TextBox    txt_NewPW;
        private System.Windows.Forms.Label      lbl_ConfirmPWHint;
        private System.Windows.Forms.TextBox    txt_ConfirmPW;
        private System.Windows.Forms.CheckBox   chk_ShowNewPW;
        private System.Windows.Forms.Button     btn_SetNewPW;
        private System.Windows.Forms.Label      lbl_PwRules;

        private System.Windows.Forms.Button     btn_Back;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            // Khởi tạo tất cả controls
            this.lbl_Title        = new System.Windows.Forms.Label();
            this.lbl_Step         = new System.Windows.Forms.Label();
            this.lbl_Error        = new System.Windows.Forms.Label();

            this.pnl_Email        = new System.Windows.Forms.Panel();
            this.lbl_EmailHint    = new System.Windows.Forms.Label();
            this.txt_Email        = new System.Windows.Forms.TextBox();
            this.btn_FindAccount  = new System.Windows.Forms.Button();

            this.pnl_OTP          = new System.Windows.Forms.Panel();
            this.lbl_OtpInfo      = new System.Windows.Forms.Label();
            this.lbl_OtpHint      = new System.Windows.Forms.Label();
            this.txt_OTP          = new System.Windows.Forms.TextBox();
            this.btn_VerifyOTP    = new System.Windows.Forms.Button();
            this.btn_ResendOTP    = new System.Windows.Forms.Button();

            this.pnl_NewPW        = new System.Windows.Forms.Panel();
            this.lbl_NewPWHint    = new System.Windows.Forms.Label();
            this.txt_NewPW        = new System.Windows.Forms.TextBox();
            this.lbl_ConfirmPWHint= new System.Windows.Forms.Label();
            this.txt_ConfirmPW    = new System.Windows.Forms.TextBox();
            this.chk_ShowNewPW    = new System.Windows.Forms.CheckBox();
            this.btn_SetNewPW     = new System.Windows.Forms.Button();
            this.lbl_PwRules      = new System.Windows.Forms.Label();

            this.btn_Back         = new System.Windows.Forms.Button();

            this.pnl_Email.SuspendLayout();
            this.pnl_OTP.SuspendLayout();
            this.pnl_NewPW.SuspendLayout();
            this.SuspendLayout();

            // ── Form ──────────────────────────────────────────────────
            this.Text            = "Quên Mật Khẩu — Khôi phục tài khoản";
            this.Size            = new System.Drawing.Size(480, 520);
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.BackColor       = System.Drawing.Color.FromArgb(17, 24, 39);
            this.Font            = new System.Drawing.Font("Segoe UI", 9.5f);

            // ── lbl_Title ─────────────────────────────────────────────
            this.lbl_Title.Text      = "Khôi Phục Mật Khẩu";
            this.lbl_Title.Font      = new System.Drawing.Font("Segoe UI", 16f, System.Drawing.FontStyle.Bold);
            this.lbl_Title.ForeColor = System.Drawing.Color.FromArgb(251, 191, 36);
            this.lbl_Title.AutoSize  = true;
            this.lbl_Title.Location  = new System.Drawing.Point(30, 20);

            // ── lbl_Step ──────────────────────────────────────────────
            this.lbl_Step.Text      = "Bước 1/3 — Nhập email đã đăng ký";
            this.lbl_Step.Font      = new System.Drawing.Font("Segoe UI", 9f);
            this.lbl_Step.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lbl_Step.AutoSize  = true;
            this.lbl_Step.Location  = new System.Drawing.Point(30, 52);

            // ── lbl_Error ─────────────────────────────────────────────
            this.lbl_Error.AutoSize   = false;
            this.lbl_Error.Size       = new System.Drawing.Size(400, 32);
            this.lbl_Error.Location   = new System.Drawing.Point(30, 400);
            this.lbl_Error.ForeColor  = System.Drawing.Color.FromArgb(248, 113, 113);
            this.lbl_Error.Font       = new System.Drawing.Font("Segoe UI", 8.5f);
            this.lbl_Error.Visible    = false;
            this.lbl_Error.TextAlign  = System.Drawing.ContentAlignment.MiddleLeft;

            // ── PANEL EMAIL ────────────────────────────────────────────
            this.pnl_Email.Location  = new System.Drawing.Point(30, 80);
            this.pnl_Email.Size      = new System.Drawing.Size(400, 160);
            this.pnl_Email.BackColor = System.Drawing.Color.Transparent;

            this.lbl_EmailHint.Text      = "Địa chỉ email đã đăng ký";
            this.lbl_EmailHint.ForeColor = System.Drawing.Color.FromArgb(148, 163, 184);
            this.lbl_EmailHint.Font      = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold);
            this.lbl_EmailHint.AutoSize  = true;
            this.lbl_EmailHint.Location  = new System.Drawing.Point(0, 0);

            this.txt_Email.Location    = new System.Drawing.Point(0, 22);
            this.txt_Email.Size        = new System.Drawing.Size(400, 38);
            this.txt_Email.BackColor   = System.Drawing.Color.FromArgb(30, 41, 59);
            this.txt_Email.ForeColor   = System.Drawing.Color.FromArgb(226, 232, 240);
            this.txt_Email.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_Email.Font        = new System.Drawing.Font("Segoe UI", 10.5f);
            this.txt_Email.MaxLength   = 100;

            this.btn_FindAccount.Text      = "TÌM TÀI KHOẢN";
            this.btn_FindAccount.Location  = new System.Drawing.Point(0, 76);
            this.btn_FindAccount.Size      = new System.Drawing.Size(400, 44);
            this.btn_FindAccount.BackColor = System.Drawing.Color.FromArgb(251, 191, 36);
            this.btn_FindAccount.ForeColor = System.Drawing.Color.FromArgb(10, 14, 26);
            this.btn_FindAccount.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_FindAccount.FlatAppearance.BorderSize = 0;
            this.btn_FindAccount.Font      = new System.Drawing.Font("Segoe UI", 9.5f, System.Drawing.FontStyle.Bold);
            this.btn_FindAccount.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btn_FindAccount.Click    += new System.EventHandler(this.btn_FindAccount_Click);

            this.pnl_Email.Controls.AddRange(new System.Windows.Forms.Control[] {
                lbl_EmailHint, txt_Email, btn_FindAccount
            });

            // ── PANEL OTP ─────────────────────────────────────────────
            this.pnl_OTP.Location  = new System.Drawing.Point(30, 80);
            this.pnl_OTP.Size      = new System.Drawing.Size(400, 240);
            this.pnl_OTP.BackColor = System.Drawing.Color.Transparent;
            this.pnl_OTP.Visible   = false;

            this.lbl_OtpInfo.AutoSize   = false;
            this.lbl_OtpInfo.Size       = new System.Drawing.Size(400, 28);
            this.lbl_OtpInfo.ForeColor  = System.Drawing.Color.FromArgb(45, 212, 191);
            this.lbl_OtpInfo.Font       = new System.Drawing.Font("Segoe UI", 9f);
            this.lbl_OtpInfo.Location   = new System.Drawing.Point(0, 0);
            this.lbl_OtpInfo.TextAlign  = System.Drawing.ContentAlignment.MiddleLeft;

            this.lbl_OtpHint.Text      = "Nhập mã OTP (6 chữ số)";
            this.lbl_OtpHint.ForeColor = System.Drawing.Color.FromArgb(148, 163, 184);
            this.lbl_OtpHint.Font      = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold);
            this.lbl_OtpHint.AutoSize  = true;
            this.lbl_OtpHint.Location  = new System.Drawing.Point(0, 36);

            this.txt_OTP.Location    = new System.Drawing.Point(0, 58);
            this.txt_OTP.Size        = new System.Drawing.Size(400, 38);
            this.txt_OTP.BackColor   = System.Drawing.Color.FromArgb(30, 41, 59);
            this.txt_OTP.ForeColor   = System.Drawing.Color.FromArgb(226, 232, 240);
            this.txt_OTP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_OTP.Font        = new System.Drawing.Font("Courier New", 14f, System.Drawing.FontStyle.Bold);
            this.txt_OTP.MaxLength   = 6;
            this.txt_OTP.TextAlign   = System.Windows.Forms.HorizontalAlignment.Center;

            this.btn_VerifyOTP.Text      = "XÁC NHẬN OTP";
            this.btn_VerifyOTP.Location  = new System.Drawing.Point(0, 112);
            this.btn_VerifyOTP.Size      = new System.Drawing.Size(400, 44);
            this.btn_VerifyOTP.BackColor = System.Drawing.Color.FromArgb(45, 212, 191);
            this.btn_VerifyOTP.ForeColor = System.Drawing.Color.FromArgb(10, 14, 26);
            this.btn_VerifyOTP.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_VerifyOTP.FlatAppearance.BorderSize = 0;
            this.btn_VerifyOTP.Font      = new System.Drawing.Font("Segoe UI", 9.5f, System.Drawing.FontStyle.Bold);
            this.btn_VerifyOTP.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btn_VerifyOTP.Click    += new System.EventHandler(this.btn_VerifyOTP_Click);

            this.btn_ResendOTP.Text      = "Gửi lại OTP";
            this.btn_ResendOTP.Location  = new System.Drawing.Point(0, 166);
            this.btn_ResendOTP.Size      = new System.Drawing.Size(200, 34);
            this.btn_ResendOTP.BackColor = System.Drawing.Color.Transparent;
            this.btn_ResendOTP.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.btn_ResendOTP.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_ResendOTP.Font      = new System.Drawing.Font("Segoe UI", 8.5f);
            this.btn_ResendOTP.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btn_ResendOTP.Click    += new System.EventHandler(this.btn_ResendOTP_Click);

            this.pnl_OTP.Controls.AddRange(new System.Windows.Forms.Control[] {
                lbl_OtpInfo, lbl_OtpHint, txt_OTP, btn_VerifyOTP, btn_ResendOTP
            });

            // ── PANEL NEW PASSWORD ─────────────────────────────────────
            this.pnl_NewPW.Location  = new System.Drawing.Point(30, 80);
            this.pnl_NewPW.Size      = new System.Drawing.Size(400, 300);
            this.pnl_NewPW.BackColor = System.Drawing.Color.Transparent;
            this.pnl_NewPW.Visible   = false;

            this.lbl_PwRules.Text      = "≥ 8 ký tự • Chữ hoa • Số • Ký tự đặc biệt (!@#$...)";
            this.lbl_PwRules.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lbl_PwRules.Font      = new System.Drawing.Font("Segoe UI", 8f);
            this.lbl_PwRules.AutoSize  = true;
            this.lbl_PwRules.Location  = new System.Drawing.Point(0, 0);

            this.lbl_NewPWHint.Text      = "Mật khẩu mới";
            this.lbl_NewPWHint.ForeColor = System.Drawing.Color.FromArgb(148, 163, 184);
            this.lbl_NewPWHint.Font      = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold);
            this.lbl_NewPWHint.AutoSize  = true;
            this.lbl_NewPWHint.Location  = new System.Drawing.Point(0, 28);

            this.txt_NewPW.Location             = new System.Drawing.Point(0, 50);
            this.txt_NewPW.Size                 = new System.Drawing.Size(400, 38);
            this.txt_NewPW.BackColor            = System.Drawing.Color.FromArgb(30, 41, 59);
            this.txt_NewPW.ForeColor            = System.Drawing.Color.FromArgb(226, 232, 240);
            this.txt_NewPW.BorderStyle          = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_NewPW.Font                 = new System.Drawing.Font("Segoe UI", 10.5f);
            this.txt_NewPW.UseSystemPasswordChar = true;
            this.txt_NewPW.MaxLength            = 100;

            this.lbl_ConfirmPWHint.Text      = "Xác nhận mật khẩu mới";
            this.lbl_ConfirmPWHint.ForeColor = System.Drawing.Color.FromArgb(148, 163, 184);
            this.lbl_ConfirmPWHint.Font      = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold);
            this.lbl_ConfirmPWHint.AutoSize  = true;
            this.lbl_ConfirmPWHint.Location  = new System.Drawing.Point(0, 104);

            this.txt_ConfirmPW.Location             = new System.Drawing.Point(0, 126);
            this.txt_ConfirmPW.Size                 = new System.Drawing.Size(400, 38);
            this.txt_ConfirmPW.BackColor            = System.Drawing.Color.FromArgb(30, 41, 59);
            this.txt_ConfirmPW.ForeColor            = System.Drawing.Color.FromArgb(226, 232, 240);
            this.txt_ConfirmPW.BorderStyle          = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_ConfirmPW.Font                 = new System.Drawing.Font("Segoe UI", 10.5f);
            this.txt_ConfirmPW.UseSystemPasswordChar = true;
            this.txt_ConfirmPW.MaxLength            = 100;

            this.chk_ShowNewPW.Text      = "Hiện mật khẩu";
            this.chk_ShowNewPW.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.chk_ShowNewPW.Font      = new System.Drawing.Font("Segoe UI", 8.5f);
            this.chk_ShowNewPW.AutoSize  = true;
            this.chk_ShowNewPW.Location  = new System.Drawing.Point(0, 172);
            this.chk_ShowNewPW.CheckedChanged += new System.EventHandler(this.chk_ShowNewPW_CheckedChanged);

            this.btn_SetNewPW.Text      = "ĐẶT MẬT KHẨU MỚI";
            this.btn_SetNewPW.Location  = new System.Drawing.Point(0, 204);
            this.btn_SetNewPW.Size      = new System.Drawing.Size(400, 44);
            this.btn_SetNewPW.BackColor = System.Drawing.Color.FromArgb(52, 211, 153);
            this.btn_SetNewPW.ForeColor = System.Drawing.Color.FromArgb(10, 14, 26);
            this.btn_SetNewPW.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_SetNewPW.FlatAppearance.BorderSize = 0;
            this.btn_SetNewPW.Font      = new System.Drawing.Font("Segoe UI", 9.5f, System.Drawing.FontStyle.Bold);
            this.btn_SetNewPW.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btn_SetNewPW.Click    += new System.EventHandler(this.btn_SetNewPW_Click);

            this.pnl_NewPW.Controls.AddRange(new System.Windows.Forms.Control[] {
                lbl_PwRules, lbl_NewPWHint, txt_NewPW,
                lbl_ConfirmPWHint, txt_ConfirmPW,
                chk_ShowNewPW, btn_SetNewPW
            });

            // ── btn_Back ──────────────────────────────────────────────
            this.btn_Back.Text      = "← Quay lại đăng nhập";
            this.btn_Back.Location  = new System.Drawing.Point(30, 440);
            this.btn_Back.Size      = new System.Drawing.Size(200, 34);
            this.btn_Back.BackColor = System.Drawing.Color.Transparent;
            this.btn_Back.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.btn_Back.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Back.Font      = new System.Drawing.Font("Segoe UI", 8.5f);
            this.btn_Back.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btn_Back.Click    += new System.EventHandler(this.btn_Back_Click);

            // ── Controls ──────────────────────────────────────────────
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                lbl_Title, lbl_Step,
                pnl_Email, pnl_OTP, pnl_NewPW,
                lbl_Error, btn_Back
            });

            this.pnl_Email.ResumeLayout(false);
            this.pnl_OTP.ResumeLayout(false);
            this.pnl_NewPW.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
