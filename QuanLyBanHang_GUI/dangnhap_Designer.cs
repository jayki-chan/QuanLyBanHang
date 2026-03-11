using System.Drawing;
using System.Windows.Forms;

namespace QuanLyBanHang_GUI
{
    partial class dangnhap
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlMain = new System.Windows.Forms.Panel();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblAppName = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.pnlForm = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblDivider = new System.Windows.Forms.Label();
            this.lblUserIcon = new System.Windows.Forms.Label();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.lblPassIcon = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblError = new System.Windows.Forms.Label();
            this.btnDangnhap = new System.Windows.Forms.Button();
            this.btnThoat = new System.Windows.Forms.Button();
            this.pnlMain.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.pnlForm.SuspendLayout();
            this.SuspendLayout();

            // === pnlMain (nền xám nhạt toàn form) ===
            this.pnlMain.BackColor = Color.FromArgb(245, 246, 250);
            this.pnlMain.Dock = DockStyle.Fill;
            this.pnlMain.Controls.Add(this.pnlHeader);
            this.pnlMain.Controls.Add(this.pnlForm);

            // === pnlHeader (dải tiêu đề xanh đậm trên cùng) ===
            this.pnlHeader.BackColor = Color.FromArgb(30, 55, 100);
            this.pnlHeader.Dock = DockStyle.Top;
            this.pnlHeader.Height = 90;
            this.pnlHeader.Controls.Add(this.lblAppName);
            this.pnlHeader.Controls.Add(this.lblSubtitle);

            this.lblAppName.AutoSize = false;
            this.lblAppName.Dock = DockStyle.None;
            this.lblAppName.Location = new Point(0, 15);
            this.lblAppName.Size = new Size(460, 34);
            this.lblAppName.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            this.lblAppName.ForeColor = Color.White;
            this.lblAppName.Text = "QUẢN LÝ BÁN HÀNG";
            this.lblAppName.TextAlign = ContentAlignment.MiddleCenter;

            this.lblSubtitle.AutoSize = false;
            this.lblSubtitle.Location = new Point(0, 52);
            this.lblSubtitle.Size = new Size(460, 22);
            this.lblSubtitle.Font = new Font("Segoe UI", 8.5F, FontStyle.Regular);
            this.lblSubtitle.ForeColor = Color.FromArgb(180, 210, 255);
            this.lblSubtitle.Text = "ĐH Công Nghiệp Việt Hung - Lập trình Windows Form Nâng Cao";
            this.lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;

            // === pnlForm (card trắng chứa các input) ===
            this.pnlForm.BackColor = Color.White;
            this.pnlForm.Location = new Point(40, 115);
            this.pnlForm.Size = new Size(380, 310);
            this.pnlForm.Controls.Add(this.lblTitle);
            this.pnlForm.Controls.Add(this.lblDivider);
            this.pnlForm.Controls.Add(this.lblUserIcon);
            this.pnlForm.Controls.Add(this.txtUser);
            this.pnlForm.Controls.Add(this.lblPassIcon);
            this.pnlForm.Controls.Add(this.txtPassword);
            this.pnlForm.Controls.Add(this.lblError);
            this.pnlForm.Controls.Add(this.btnDangnhap);
            this.pnlForm.Controls.Add(this.btnThoat);
            // Đổ bóng nhẹ bằng border
            this.pnlForm.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlForm_Paint);

            // lblTitle
            this.lblTitle.AutoSize = false;
            this.lblTitle.Location = new Point(0, 20);
            this.lblTitle.Size = new Size(380, 32);
            this.lblTitle.Font = new Font("Segoe UI Semibold", 13F, FontStyle.Bold);
            this.lblTitle.ForeColor = Color.FromArgb(30, 55, 100);
            this.lblTitle.Text = "Đăng Nhập";
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // lblDivider (đường kẻ ngang)
            this.lblDivider.AutoSize = false;
            this.lblDivider.Location = new Point(25, 58);
            this.lblDivider.Size = new Size(330, 1);
            this.lblDivider.BackColor = Color.FromArgb(230, 232, 240);

            // Username label icon
            this.lblUserIcon.AutoSize = false;
            this.lblUserIcon.Location = new Point(25, 75);
            this.lblUserIcon.Size = new Size(330, 18);
            this.lblUserIcon.Font = new Font("Segoe UI", 8F);
            this.lblUserIcon.ForeColor = Color.FromArgb(100, 110, 130);
            this.lblUserIcon.Text = "TÊN ĐĂNG NHẬP";

            // txtUser
            this.txtUser.Location = new Point(25, 96);
            this.txtUser.Size = new Size(330, 28);
            this.txtUser.Font = new Font("Segoe UI", 10F);
            this.txtUser.BorderStyle = BorderStyle.FixedSingle;
            this.txtUser.BackColor = Color.FromArgb(248, 249, 252);
            this.txtUser.Name = "txtUser";
            this.txtUser.KeyDown += new KeyEventHandler(this.txtUser_KeyDown);

            // Password label icon
            this.lblPassIcon.AutoSize = false;
            this.lblPassIcon.Location = new Point(25, 138);
            this.lblPassIcon.Size = new Size(330, 18);
            this.lblPassIcon.Font = new Font("Segoe UI", 8F);
            this.lblPassIcon.ForeColor = Color.FromArgb(100, 110, 130);
            this.lblPassIcon.Text = "MẬT KHẨU";

            // txtPassword
            this.txtPassword.Location = new Point(25, 159);
            this.txtPassword.Size = new Size(330, 28);
            this.txtPassword.Font = new Font("Segoe UI", 10F);
            this.txtPassword.BorderStyle = BorderStyle.FixedSingle;
            this.txtPassword.BackColor = Color.FromArgb(248, 249, 252);
            this.txtPassword.PasswordChar = '●';
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.KeyDown += new KeyEventHandler(this.txtPassword_KeyDown);

            // lblError
            this.lblError.AutoSize = false;
            this.lblError.Location = new Point(25, 198);
            this.lblError.Size = new Size(330, 18);
            this.lblError.Font = new Font("Segoe UI", 8.5F);
            this.lblError.ForeColor = Color.FromArgb(200, 50, 50);
            this.lblError.Text = "";
            this.lblError.Name = "lblError";
            this.lblError.Visible = false;

            // btnDangnhap
            this.btnDangnhap.Location = new Point(25, 228);
            this.btnDangnhap.Size = new Size(155, 38);
            this.btnDangnhap.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            this.btnDangnhap.Text = "Đăng nhập";
            this.btnDangnhap.BackColor = Color.FromArgb(30, 55, 100);
            this.btnDangnhap.ForeColor = Color.White;
            this.btnDangnhap.FlatStyle = FlatStyle.Flat;
            this.btnDangnhap.FlatAppearance.BorderSize = 0;
            this.btnDangnhap.Cursor = Cursors.Hand;
            this.btnDangnhap.Name = "btnDangnhap";
            this.btnDangnhap.Click += new System.EventHandler(this.btnDangnhap_Click);

            // btnThoat
            this.btnThoat.Location = new Point(200, 228);
            this.btnThoat.Size = new Size(155, 38);
            this.btnThoat.Font = new Font("Segoe UI", 10F);
            this.btnThoat.Text = "Thoát";
            this.btnThoat.BackColor = Color.White;
            this.btnThoat.ForeColor = Color.FromArgb(80, 90, 110);
            this.btnThoat.FlatStyle = FlatStyle.Flat;
            this.btnThoat.FlatAppearance.BorderColor = Color.FromArgb(200, 205, 215);
            this.btnThoat.Cursor = Cursors.Hand;
            this.btnThoat.Name = "btnThoat";
            this.btnThoat.Click += new System.EventHandler(this.btnThoat_Click);

            // === Form ===
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(460, 460);
            this.Controls.Add(this.pnlMain);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "dangnhap";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Đăng Nhập — Quản Lý Bán Hàng";
            this.BackColor = Color.FromArgb(245, 246, 250);
            this.pnlMain.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.pnlForm.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblAppName;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Panel pnlForm;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblDivider;
        private System.Windows.Forms.Label lblUserIcon;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.Label lblPassIcon;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Button btnDangnhap;
        private System.Windows.Forms.Button btnThoat;

        private void pnlForm_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            var pen = new System.Drawing.Pen(Color.FromArgb(220, 220, 230), 1);
            e.Graphics.DrawRectangle(pen, 0, 0, pnlForm.Width - 1, pnlForm.Height - 1);
        }
    }
}