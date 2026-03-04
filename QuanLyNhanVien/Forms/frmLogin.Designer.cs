namespace QuanLyNhanVien.Forms
{
    partial class frmLogin
    {
        private System.ComponentModel.IContainer components = null;

        // Controls
        private System.Windows.Forms.Panel      pnl_Main;
        private System.Windows.Forms.Panel      pnl_Card;
        private System.Windows.Forms.Label      lbl_Title;
        private System.Windows.Forms.Label      lbl_Subtitle;
        private System.Windows.Forms.Label      lbl_UsernameHint;
        private System.Windows.Forms.TextBox    txt_Username;
        private System.Windows.Forms.Label      lbl_PasswordHint;
        private System.Windows.Forms.TextBox    txt_Password;
        private System.Windows.Forms.CheckBox   chk_ShowPW;
        private System.Windows.Forms.CheckBox   chk_RememberMe;
        private System.Windows.Forms.Button     btn_Login;
        private System.Windows.Forms.LinkLabel  lnk_ForgotPW;
        private System.Windows.Forms.Label      lbl_Error;
        private System.Windows.Forms.PictureBox pic_Loading;
        private System.Windows.Forms.Label      lbl_Version;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnl_Main       = new System.Windows.Forms.Panel();
            this.pnl_Card       = new System.Windows.Forms.Panel();
            this.lbl_Title      = new System.Windows.Forms.Label();
            this.lbl_Subtitle   = new System.Windows.Forms.Label();
            this.lbl_UsernameHint = new System.Windows.Forms.Label();
            this.txt_Username   = new System.Windows.Forms.TextBox();
            this.lbl_PasswordHint = new System.Windows.Forms.Label();
            this.txt_Password   = new System.Windows.Forms.TextBox();
            this.chk_ShowPW     = new System.Windows.Forms.CheckBox();
            this.chk_RememberMe = new System.Windows.Forms.CheckBox();
            this.btn_Login      = new System.Windows.Forms.Button();
            this.lnk_ForgotPW   = new System.Windows.Forms.LinkLabel();
            this.lbl_Error      = new System.Windows.Forms.Label();
            this.pic_Loading    = new System.Windows.Forms.PictureBox();
            this.lbl_Version    = new System.Windows.Forms.Label();

            this.pnl_Main.SuspendLayout();
            this.pnl_Card.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Loading)).BeginInit();
            this.SuspendLayout();

            // ── Form ──────────────────────────────────────────────────
            this.Text            = "Quản Lý Nhân Viên — Đăng Nhập";
            this.Size            = new System.Drawing.Size(1100, 680);
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox     = false;
            this.BackColor       = System.Drawing.Color.FromArgb(10, 14, 26);
            this.Font            = new System.Drawing.Font("Segoe UI", 9.5f);

            // ── pnl_Main (toàn form) ──────────────────────────────────
            this.pnl_Main.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.pnl_Main.BackColor = System.Drawing.Color.FromArgb(10, 14, 26);

            // ── pnl_Card (card trắng ở giữa) ─────────────────────────
            this.pnl_Card.Size      = new System.Drawing.Size(400, 500);
            this.pnl_Card.Location  = new System.Drawing.Point(350, 90);
            this.pnl_Card.BackColor = System.Drawing.Color.FromArgb(17, 24, 39);
            this.pnl_Card.Padding   = new System.Windows.Forms.Padding(36, 32, 36, 32);

            // ── lbl_Title ─────────────────────────────────────────────
            this.lbl_Title.Text      = "Đăng Nhập";
            this.lbl_Title.Font      = new System.Drawing.Font("Segoe UI", 22f, System.Drawing.FontStyle.Bold);
            this.lbl_Title.ForeColor = System.Drawing.Color.FromArgb(226, 232, 240);
            this.lbl_Title.AutoSize  = true;
            this.lbl_Title.Location  = new System.Drawing.Point(36, 36);

            // ── lbl_Subtitle ──────────────────────────────────────────
            this.lbl_Subtitle.Text      = "Hệ thống Quản Lý Nhân Viên";
            this.lbl_Subtitle.Font      = new System.Drawing.Font("Segoe UI", 9.5f);
            this.lbl_Subtitle.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lbl_Subtitle.AutoSize  = true;
            this.lbl_Subtitle.Location  = new System.Drawing.Point(36, 76);

            // ── lbl_UsernameHint ──────────────────────────────────────
            this.lbl_UsernameHint.Text      = "Tên đăng nhập";
            this.lbl_UsernameHint.Font      = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold);
            this.lbl_UsernameHint.ForeColor = System.Drawing.Color.FromArgb(148, 163, 184);
            this.lbl_UsernameHint.AutoSize  = true;
            this.lbl_UsernameHint.Location  = new System.Drawing.Point(36, 120);

            // ── txt_Username ──────────────────────────────────────────
            this.txt_Username.Location    = new System.Drawing.Point(36, 142);
            this.txt_Username.Size        = new System.Drawing.Size(328, 38);
            this.txt_Username.BackColor   = System.Drawing.Color.FromArgb(30, 41, 59);
            this.txt_Username.ForeColor   = System.Drawing.Color.FromArgb(226, 232, 240);
            this.txt_Username.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_Username.Font        = new System.Drawing.Font("Segoe UI", 10.5f);
            this.txt_Username.MaxLength   = 50;
            this.txt_Username.KeyDown    += new System.Windows.Forms.KeyEventHandler(this.txt_Username_KeyDown);

            // ── lbl_PasswordHint ──────────────────────────────────────
            this.lbl_PasswordHint.Text      = "Mật khẩu";
            this.lbl_PasswordHint.Font      = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold);
            this.lbl_PasswordHint.ForeColor = System.Drawing.Color.FromArgb(148, 163, 184);
            this.lbl_PasswordHint.AutoSize  = true;
            this.lbl_PasswordHint.Location  = new System.Drawing.Point(36, 196);

            // ── txt_Password ──────────────────────────────────────────
            this.txt_Password.Location             = new System.Drawing.Point(36, 218);
            this.txt_Password.Size                 = new System.Drawing.Size(328, 38);
            this.txt_Password.BackColor            = System.Drawing.Color.FromArgb(30, 41, 59);
            this.txt_Password.ForeColor            = System.Drawing.Color.FromArgb(226, 232, 240);
            this.txt_Password.BorderStyle          = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_Password.Font                 = new System.Drawing.Font("Segoe UI", 10.5f);
            this.txt_Password.UseSystemPasswordChar = true;
            this.txt_Password.MaxLength            = 100;
            this.txt_Password.KeyDown             += new System.Windows.Forms.KeyEventHandler(this.txt_Password_KeyDown);

            // ── chk_ShowPW ────────────────────────────────────────────
            this.chk_ShowPW.Text      = "Hiện mật khẩu";
            this.chk_ShowPW.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.chk_ShowPW.Font      = new System.Drawing.Font("Segoe UI", 8.5f);
            this.chk_ShowPW.AutoSize  = true;
            this.chk_ShowPW.Location  = new System.Drawing.Point(36, 264);
            this.chk_ShowPW.CheckedChanged += new System.EventHandler(this.chk_ShowPW_CheckedChanged);

            // ── chk_RememberMe ────────────────────────────────────────
            this.chk_RememberMe.Text      = "Nhớ tên đăng nhập";
            this.chk_RememberMe.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.chk_RememberMe.Font      = new System.Drawing.Font("Segoe UI", 8.5f);
            this.chk_RememberMe.AutoSize  = true;
            this.chk_RememberMe.Location  = new System.Drawing.Point(200, 264);

            // ── lbl_Error ─────────────────────────────────────────────
            this.lbl_Error.AutoSize   = false;
            this.lbl_Error.Size       = new System.Drawing.Size(328, 36);
            this.lbl_Error.Location   = new System.Drawing.Point(36, 292);
            this.lbl_Error.ForeColor  = System.Drawing.Color.FromArgb(248, 113, 113);
            this.lbl_Error.Font       = new System.Drawing.Font("Segoe UI", 8.5f);
            this.lbl_Error.Visible    = false;
            this.lbl_Error.TextAlign  = System.Drawing.ContentAlignment.MiddleLeft;

            // ── pic_Loading ───────────────────────────────────────────
            this.pic_Loading.Size     = new System.Drawing.Size(24, 24);
            this.pic_Loading.Location = new System.Drawing.Point(188, 290);
            this.pic_Loading.Visible  = false;
            this.pic_Loading.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;

            // ── btn_Login ─────────────────────────────────────────────
            this.btn_Login.Text      = "ĐĂNG NHẬP";
            this.btn_Login.Location  = new System.Drawing.Point(36, 340);
            this.btn_Login.Size      = new System.Drawing.Size(328, 46);
            this.btn_Login.BackColor = System.Drawing.Color.FromArgb(56, 189, 248);
            this.btn_Login.ForeColor = System.Drawing.Color.FromArgb(10, 14, 26);
            this.btn_Login.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Login.Font      = new System.Drawing.Font("Segoe UI", 10f, System.Drawing.FontStyle.Bold);
            this.btn_Login.FlatAppearance.BorderSize = 0;
            this.btn_Login.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btn_Login.Click    += new System.EventHandler(this.btn_Login_Click);

            // ── lnk_ForgotPW ──────────────────────────────────────────
            this.lnk_ForgotPW.Text             = "Quên mật khẩu?";
            this.lnk_ForgotPW.ActiveLinkColor  = System.Drawing.Color.FromArgb(56, 189, 248);
            this.lnk_ForgotPW.LinkColor        = System.Drawing.Color.FromArgb(56, 189, 248);
            this.lnk_ForgotPW.VisitedLinkColor = System.Drawing.Color.FromArgb(56, 189, 248);
            this.lnk_ForgotPW.Font             = new System.Drawing.Font("Segoe UI", 9f);
            this.lnk_ForgotPW.AutoSize         = true;
            this.lnk_ForgotPW.Location         = new System.Drawing.Point(130, 402);
            this.lnk_ForgotPW.LinkClicked     += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnk_ForgotPW_LinkClicked);

            // ── lbl_Version ───────────────────────────────────────────
            this.lbl_Version.Text      = "Quản Lý Nhân Viên v1.0 • .NET Framework 4.8";
            this.lbl_Version.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.lbl_Version.Font      = new System.Drawing.Font("Segoe UI", 8f);
            this.lbl_Version.AutoSize  = true;
            this.lbl_Version.Location  = new System.Drawing.Point(350, 610);

            // ── Assembly ──────────────────────────────────────────────
            this.pnl_Card.Controls.AddRange(new System.Windows.Forms.Control[] {
                lbl_Title, lbl_Subtitle,
                lbl_UsernameHint, txt_Username,
                lbl_PasswordHint, txt_Password,
                chk_ShowPW, chk_RememberMe,
                lbl_Error, pic_Loading,
                btn_Login, lnk_ForgotPW
            });

            this.pnl_Main.Controls.Add(pnl_Card);
            this.pnl_Main.Controls.Add(lbl_Version);
            this.Controls.Add(pnl_Main);

            this.pnl_Main.ResumeLayout(false);
            this.pnl_Card.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pic_Loading)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
