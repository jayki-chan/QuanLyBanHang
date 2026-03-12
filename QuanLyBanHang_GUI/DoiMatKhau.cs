using QuanLyBanHang_BUS;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyBanHang_GUI
{
    /// <summary>
    /// Đổi mật khẩu cho tài khoản đang đăng nhập.
    /// Nhận username từ Form1 truyền vào.
    /// </summary>
    public partial class DoiMatKhau : Form
    {
        static readonly Color NavBlue   = Color.FromArgb(30, 55, 100);
        static readonly Color BgGray    = Color.FromArgb(245, 246, 250);
        static readonly Color BorderCol = Color.FromArgb(208, 214, 228);

        readonly string _username;
        readonly NhanVienBUS _bus = new NhanVienBUS();
        TextBox txtCu, txtMoi, txtXacNhan;
        Label lblStrength, lblError;
        Button btnDoi, btnTroVe;

        public DoiMatKhau(string username)
        {
            _username = username;
            BuildUI();
        }

        void BuildUI()
        {
            this.Text = "Đổi Mật Khẩu";
            this.ClientSize = new Size(460, 460);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BgGray;

            // ── Header ──────────────────────────────────────
            var pnlHeader = new Panel { BackColor = NavBlue, Dock = DockStyle.Top, Height = 52 };
            pnlHeader.Controls.Add(new Label
            {
                Dock = DockStyle.Fill, Text = "ĐỔI MẬT KHẨU",
                Font = new Font("Segoe UI Semibold", 13.5F, FontStyle.Bold),
                ForeColor = Color.White, TextAlign = ContentAlignment.MiddleCenter
            });

            // ── Card ────────────────────────────────────────
            var card = new Panel
            {
                BackColor = Color.White,
                Location = new Point(40, 70), Size = new Size(380, 318)
            };
            card.Paint += (s, e) =>
                e.Graphics.DrawRectangle(new Pen(BorderCol), 0, 0, card.Width - 1, card.Height - 1);

            // Avatar icon lớn ở trên
            var lblIcon = new Label
            {
                Text = "🔐", Location = new Point(0, 12), Size = new Size(380, 44),
                Font = new Font("Segoe UI", 22F), TextAlign = ContentAlignment.MiddleCenter
            };
            card.Controls.Add(lblIcon);

            var lblUser = new Label
            {
                Text = $"Tài khoản: {_username}",
                Location = new Point(0, 56), Size = new Size(380, 20),
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                ForeColor = NavBlue, TextAlign = ContentAlignment.MiddleCenter
            };
            card.Controls.Add(lblUser);

            int y = 88;
            CardField(card, "Mật khẩu hiện tại:", ref y, out txtCu,   isPass: true);
            CardField(card, "Mật khẩu mới:",       ref y, out txtMoi,  isPass: true);
            CardField(card, "Xác nhận mật khẩu:",  ref y, out txtXacNhan, isPass: true);

            // Strength
            lblStrength = new Label
            {
                Location = new Point(24, y), Size = new Size(332, 18),
                Font = new Font("Segoe UI", 8F, FontStyle.Italic), Text = ""
            };
            card.Controls.Add(lblStrength);
            y += 24;

            // Error
            lblError = new Label
            {
                Location = new Point(24, y), Size = new Size(332, 18),
                Font = new Font("Segoe UI", 8.5F), ForeColor = Color.FromArgb(180, 40, 40),
                Text = "", Visible = false
            };
            card.Controls.Add(lblError);

            txtMoi.TextChanged += (s, e) => UpdateStrength();

            this.Controls.Add(card);
            this.Controls.Add(pnlHeader);

            // ── Buttons ─────────────────────────────────────
            btnDoi   = MakeBtn("Đổi Mật Khẩu", NavBlue,       40, 402);
            btnTroVe = MakeBtn("Trở Về",            Color.White,  288, 402);
            btnTroVe.ForeColor = Color.FromArgb(48, 62, 90);
            btnTroVe.FlatAppearance.BorderColor = BorderCol;
            btnTroVe.FlatAppearance.BorderSize = 1;

            btnDoi.Click   += BtnDoi_Click;
            btnTroVe.Click += (s, e) => this.Close();

            this.Controls.Add(btnDoi);
            this.Controls.Add(btnTroVe);

            // Enter key
            txtCu.KeyDown       += (s, e) => { if (e.KeyCode == Keys.Enter) txtMoi.Focus(); };
            txtMoi.KeyDown      += (s, e) => { if (e.KeyCode == Keys.Enter) txtXacNhan.Focus(); };
            txtXacNhan.KeyDown  += (s, e) => { if (e.KeyCode == Keys.Enter) BtnDoi_Click(s, e); };
        }

        void CardField(Panel p, string label, ref int y, out TextBox txt, bool isPass = false)
        {
            p.Controls.Add(new Label
            {
                Text = label, Location = new Point(24, y), AutoSize = true,
                Font = new Font("Segoe UI", 8.5F), ForeColor = Color.FromArgb(68, 82, 110)
            });
            y += 18;
            txt = new TextBox
            {
                Location = new Point(24, y), Size = new Size(332, 26),
                Font = new Font("Segoe UI", 9.5F), BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(250, 251, 255)
            };
            if (isPass) txt.UseSystemPasswordChar = true;
            p.Controls.Add(txt);
            y += 34;
        }

        Button MakeBtn(string text, Color bg, int x, int y)
        {
            var b = new Button
            {
                Text = text, Location = new Point(x, y), Size = new Size(132, 36),
                Font = new Font("Segoe UI", 9F), BackColor = bg,
                ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            
            // Gán icon theo text
            var iconMap = new System.Collections.Generic.Dictionary<string, IconType>
            {
                { "Tải lại",       IconType.Reload  },
                { "Reload",        IconType.Reload  },
                { "Thêm",          IconType.Add     },
                { "Sửa",           IconType.Edit    },
                { "Lưu",           IconType.Save    },
                { "Hủy Bỏ",        IconType.Cancel  },
                { "Xóa",           IconType.Delete  },
                { "Xóa TK",        IconType.Delete  },
                { "Trở Về",        IconType.Back    },
                { "Test kết nối",  IconType.Test    },
                { "Lưu cấu hình",  IconType.Save    },
                { "Lưu Username",  IconType.Save    },
                { "Đặt lại MK",    IconType.Key     },
                { "Đổi Mật Khẩu",  IconType.Key     },
            };
            string cleanText = text.Trim();
            foreach (var kv in iconMap)
                if (cleanText.Contains(kv.Key))
                {
                    b.Image        = AppIcons.Get(kv.Value, 16, b.ForeColor == Color.White ? Color.White : Color.FromArgb(48,62,90));
                    b.ImageAlign   = ContentAlignment.MiddleLeft;
                    b.TextAlign    = ContentAlignment.MiddleCenter;
                    b.TextImageRelation = TextImageRelation.ImageBeforeText;
                    b.Padding      = new Padding(4, 0, 0, 0);
                    b.Text         = "  " + cleanText;
                    break;
                }

            return b;
        }

        void UpdateStrength()
        {
            string p = txtMoi.Text;
            if (p.Length == 0) { lblStrength.Text = ""; return; }
            int score = 0;
            if (p.Length >= 8) score++;
            if (System.Text.RegularExpressions.Regex.IsMatch(p, @"[A-Z]")) score++;
            if (System.Text.RegularExpressions.Regex.IsMatch(p, @"[0-9]")) score++;
            if (System.Text.RegularExpressions.Regex.IsMatch(p, @"[^a-zA-Z0-9]")) score++;

            string strengthText; Color strengthColor;
            if (score <= 1)      { strengthText = "Độ mạnh: Yếu";        strengthColor = Color.FromArgb(180, 40, 40); }
            else if (score == 2) { strengthText = "Độ mạnh: Trung bình"; strengthColor = Color.FromArgb(180, 120, 0); }
            else if (score == 3) { strengthText = "Độ mạnh: Khá";        strengthColor = Color.FromArgb(0, 140, 80); }
            else                 { strengthText = "Độ mạnh: Mạnh 💪";    strengthColor = Color.FromArgb(0, 100, 50); }
            lblStrength.Text = strengthText; lblStrength.ForeColor = strengthColor;
        }

        void SetError(string msg)
        {
            lblError.Text    = msg;
            lblError.Visible = !string.IsNullOrEmpty(msg);
        }

        void BtnDoi_Click(object s, EventArgs e)
        {
            SetError("");
            if (string.IsNullOrWhiteSpace(txtCu.Text))  { SetError("Nhập mật khẩu hiện tại."); return; }
            if (string.IsNullOrWhiteSpace(txtMoi.Text))  { SetError("Nhập mật khẩu mới."); return; }
            if (txtMoi.Text.Length < 6)                  { SetError("Mật khẩu mới phải từ 6 ký tự."); return; }
            if (txtMoi.Text != txtXacNhan.Text)           { SetError("Mật khẩu xác nhận không khớp."); return; }
            if (txtMoi.Text == txtCu.Text)               { SetError("Mật khẩu mới phải khác mật khẩu cũ."); return; }

            var (ok, msg) = _bus.ChangePassword(_username, txtCu.Text.Trim(), txtMoi.Text.Trim());
            if (ok)
            {
                FormHelper.ShowOK(msg);
                this.Close();
            }
            else
            {
                SetError(msg);
            }
        }
    }
}
