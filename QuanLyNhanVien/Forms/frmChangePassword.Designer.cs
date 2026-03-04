namespace QuanLyNhanVien.Forms
{
    partial class frmChangePassword
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label    lbl_Title;
        private System.Windows.Forms.Label    lbl_Subtitle;
        private System.Windows.Forms.Label    lbl_PwRules;
        private System.Windows.Forms.Label    lbl_OldPWHint;
        private System.Windows.Forms.TextBox  txt_OldPW;
        private System.Windows.Forms.Label    lbl_NewPWHint;
        private System.Windows.Forms.TextBox  txt_NewPW;
        private System.Windows.Forms.Label    lbl_ConfirmHint;
        private System.Windows.Forms.TextBox  txt_ConfirmPW;
        private System.Windows.Forms.CheckBox chk_ShowPW;
        private System.Windows.Forms.Label    lbl_Error;
        private System.Windows.Forms.Button   btn_Save;
        private System.Windows.Forms.Button   btn_Cancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lbl_Title      = new System.Windows.Forms.Label();
            this.lbl_Subtitle   = new System.Windows.Forms.Label();
            this.lbl_PwRules    = new System.Windows.Forms.Label();
            this.lbl_OldPWHint  = new System.Windows.Forms.Label();
            this.txt_OldPW      = new System.Windows.Forms.TextBox();
            this.lbl_NewPWHint  = new System.Windows.Forms.Label();
            this.txt_NewPW      = new System.Windows.Forms.TextBox();
            this.lbl_ConfirmHint= new System.Windows.Forms.Label();
            this.txt_ConfirmPW  = new System.Windows.Forms.TextBox();
            this.chk_ShowPW     = new System.Windows.Forms.CheckBox();
            this.lbl_Error      = new System.Windows.Forms.Label();
            this.btn_Save       = new System.Windows.Forms.Button();
            this.btn_Cancel     = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // ── Form ──────────────────────────────────────────────────
            this.Text            = "Đổi Mật Khẩu";
            this.Size            = new System.Drawing.Size(460, 560);
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.BackColor       = System.Drawing.Color.FromArgb(17, 24, 39);
            this.Font            = new System.Drawing.Font("Segoe UI", 9.5f);

            // ── lbl_Title ─────────────────────────────────────────────
            this.lbl_Title.Text      = "Đổi Mật Khẩu Bắt Buộc";
            this.lbl_Title.Font      = new System.Drawing.Font("Segoe UI", 16f, System.Drawing.FontStyle.Bold);
            this.lbl_Title.ForeColor = System.Drawing.Color.FromArgb(167, 139, 250);
            this.lbl_Title.AutoSize  = true;
            this.lbl_Title.Location  = new System.Drawing.Point(30, 22);

            // ── lbl_Subtitle ──────────────────────────────────────────
            this.lbl_Subtitle.AutoSize   = false;
            this.lbl_Subtitle.Size       = new System.Drawing.Size(380, 44);
            this.lbl_Subtitle.Location   = new System.Drawing.Point(30, 56);
            this.lbl_Subtitle.ForeColor  = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lbl_Subtitle.Font       = new System.Drawing.Font("Segoe UI", 9f);
            this.lbl_Subtitle.TextAlign  = System.Drawing.ContentAlignment.TopLeft;

            // ── lbl_PwRules ───────────────────────────────────────────
            this.lbl_PwRules.Text      = "Yêu cầu: ≥ 8 ký tự  •  Chữ hoa  •  Số  •  Ký tự đặc biệt (!@#$...)";
            this.lbl_PwRules.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lbl_PwRules.Font      = new System.Drawing.Font("Segoe UI", 8f);
            this.lbl_PwRules.AutoSize  = false;
            this.lbl_PwRules.Size      = new System.Drawing.Size(380, 20);
            this.lbl_PwRules.Location  = new System.Drawing.Point(30, 108);

            // ── Old password (chỉ hiện khi không phải first login) ────
            this.lbl_OldPWHint.Text      = "Mật khẩu hiện tại";
            this.lbl_OldPWHint.ForeColor = System.Drawing.Color.FromArgb(148, 163, 184);
            this.lbl_OldPWHint.Font      = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold);
            this.lbl_OldPWHint.AutoSize  = true;
            this.lbl_OldPWHint.Location  = new System.Drawing.Point(30, 136);

            this.txt_OldPW.Location             = new System.Drawing.Point(30, 158);
            this.txt_OldPW.Size                 = new System.Drawing.Size(380, 36);
            this.txt_OldPW.BackColor            = System.Drawing.Color.FromArgb(30, 41, 59);
            this.txt_OldPW.ForeColor            = System.Drawing.Color.FromArgb(226, 232, 240);
            this.txt_OldPW.BorderStyle          = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_OldPW.Font                 = new System.Drawing.Font("Segoe UI", 10.5f);
            this.txt_OldPW.UseSystemPasswordChar = true;
            this.txt_OldPW.MaxLength            = 100;

            // ── New password ──────────────────────────────────────────
            this.lbl_NewPWHint.Text      = "Mật khẩu mới";
            this.lbl_NewPWHint.ForeColor = System.Drawing.Color.FromArgb(148, 163, 184);
            this.lbl_NewPWHint.Font      = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold);
            this.lbl_NewPWHint.AutoSize  = true;
            this.lbl_NewPWHint.Location  = new System.Drawing.Point(30, 208);

            this.txt_NewPW.Location             = new System.Drawing.Point(30, 230);
            this.txt_NewPW.Size                 = new System.Drawing.Size(380, 36);
            this.txt_NewPW.BackColor            = System.Drawing.Color.FromArgb(30, 41, 59);
            this.txt_NewPW.ForeColor            = System.Drawing.Color.FromArgb(226, 232, 240);
            this.txt_NewPW.BorderStyle          = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_NewPW.Font                 = new System.Drawing.Font("Segoe UI", 10.5f);
            this.txt_NewPW.UseSystemPasswordChar = true;
            this.txt_NewPW.MaxLength            = 100;

            // ── Confirm password ──────────────────────────────────────
            this.lbl_ConfirmHint.Text      = "Xác nhận mật khẩu mới";
            this.lbl_ConfirmHint.ForeColor = System.Drawing.Color.FromArgb(148, 163, 184);
            this.lbl_ConfirmHint.Font      = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold);
            this.lbl_ConfirmHint.AutoSize  = true;
            this.lbl_ConfirmHint.Location  = new System.Drawing.Point(30, 280);

            this.txt_ConfirmPW.Location             = new System.Drawing.Point(30, 302);
            this.txt_ConfirmPW.Size                 = new System.Drawing.Size(380, 36);
            this.txt_ConfirmPW.BackColor            = System.Drawing.Color.FromArgb(30, 41, 59);
            this.txt_ConfirmPW.ForeColor            = System.Drawing.Color.FromArgb(226, 232, 240);
            this.txt_ConfirmPW.BorderStyle          = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_ConfirmPW.Font                 = new System.Drawing.Font("Segoe UI", 10.5f);
            this.txt_ConfirmPW.UseSystemPasswordChar = true;
            this.txt_ConfirmPW.MaxLength            = 100;

            // ── chk_ShowPW ────────────────────────────────────────────
            this.chk_ShowPW.Text      = "Hiện mật khẩu";
            this.chk_ShowPW.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.chk_ShowPW.Font      = new System.Drawing.Font("Segoe UI", 8.5f);
            this.chk_ShowPW.AutoSize  = true;
            this.chk_ShowPW.Location  = new System.Drawing.Point(30, 348);
            this.chk_ShowPW.CheckedChanged += new System.EventHandler(this.chk_ShowPW_CheckedChanged);

            // ── lbl_Error ─────────────────────────────────────────────
            this.lbl_Error.AutoSize   = false;
            this.lbl_Error.Size       = new System.Drawing.Size(380, 32);
            this.lbl_Error.Location   = new System.Drawing.Point(30, 376);
            this.lbl_Error.ForeColor  = System.Drawing.Color.FromArgb(248, 113, 113);
            this.lbl_Error.Font       = new System.Drawing.Font("Segoe UI", 8.5f);
            this.lbl_Error.Visible    = false;
            this.lbl_Error.TextAlign  = System.Drawing.ContentAlignment.MiddleLeft;

            // ── btn_Save ──────────────────────────────────────────────
            this.btn_Save.Text      = "LƯU MẬT KHẨU MỚI";
            this.btn_Save.Location  = new System.Drawing.Point(30, 418);
            this.btn_Save.Size      = new System.Drawing.Size(380, 46);
            this.btn_Save.BackColor = System.Drawing.Color.FromArgb(167, 139, 250);
            this.btn_Save.ForeColor = System.Drawing.Color.FromArgb(10, 14, 26);
            this.btn_Save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Save.FlatAppearance.BorderSize = 0;
            this.btn_Save.Font      = new System.Drawing.Font("Segoe UI", 10f, System.Drawing.FontStyle.Bold);
            this.btn_Save.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btn_Save.Click    += new System.EventHandler(this.btn_Save_Click);

            // ── btn_Cancel ────────────────────────────────────────────
            this.btn_Cancel.Text      = "Hủy";
            this.btn_Cancel.Location  = new System.Drawing.Point(30, 474);
            this.btn_Cancel.Size      = new System.Drawing.Size(380, 34);
            this.btn_Cancel.BackColor = System.Drawing.Color.Transparent;
            this.btn_Cancel.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.btn_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Cancel.Font      = new System.Drawing.Font("Segoe UI", 8.5f);
            this.btn_Cancel.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btn_Cancel.Click    += new System.EventHandler(this.btn_Cancel_Click);

            // ── Controls ──────────────────────────────────────────────
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                lbl_Title, lbl_Subtitle, lbl_PwRules,
                lbl_OldPWHint, txt_OldPW,
                lbl_NewPWHint, txt_NewPW,
                lbl_ConfirmHint, txt_ConfirmPW,
                chk_ShowPW, lbl_Error,
                btn_Save, btn_Cancel
            });

            this.ResumeLayout(false);
        }
    }
}
