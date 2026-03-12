using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace QuanLyBanHang_GUI
{
    public partial class CauHinhHeThong : Form
    {
        static readonly Color NavBlue   = Color.FromArgb(30, 55, 100);
        static readonly Color BgGray    = Color.FromArgb(245, 246, 250);
        static readonly Color BorderCol = Color.FromArgb(208, 214, 228);
        static readonly string ConfigFile = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "dbconfig.ini");

        TextBox txtServer, txtDatabase, txtUser, txtPass;
        TextBox txtApiKey, txtAiModel;
        RadioButton rdoWindows, rdoSQL;
        Label lblStatus;
        Button btnTest, btnLuu;

        public CauHinhHeThong()
        {
            BuildUI();
            LoadConfig();
        }

        void BuildUI()
        {
            this.Text = "Cấu Hình Hệ Thống";
            this.ClientSize = new Size(500, 660);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BgGray;

            // ── Header ──────────────────────────────────────
            var pnlHeader = new Panel { BackColor = NavBlue, Dock = DockStyle.Top, Height = 52 };
            pnlHeader.Controls.Add(new Label
            {
                Dock = DockStyle.Fill, Text = "CẤU HÌNH HỆ THỐNG",
                Font = new Font("Segoe UI Semibold", 13.5F, FontStyle.Bold),
                ForeColor = Color.White, TextAlign = ContentAlignment.MiddleCenter
            });

            // ── Footer ──────────────────────────────────────
            var pnlFooter = new Panel
            {
                BackColor = Color.FromArgb(232, 236, 244),
                Dock = DockStyle.Bottom, Height = 58
            };
            pnlFooter.Paint += (s, e) =>
                e.Graphics.DrawLine(new Pen(BorderCol), 0, 0, pnlFooter.Width, 0);

            btnTest = MakeBtn("Test kết nối", Color.FromArgb(34, 120, 80));
            btnTest.Location = new Point(14, 12);
            btnLuu  = MakeBtn("Lưu cấu hình", NavBlue);
            btnLuu.Location  = new Point(154, 12);
            var btnTroVe = MakeBtn("Trở Về", Color.White);
            btnTroVe.ForeColor = Color.FromArgb(48, 62, 90);
            btnTroVe.FlatAppearance.BorderColor = BorderCol;
            btnTroVe.FlatAppearance.BorderSize = 1;
            btnTroVe.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            pnlFooter.Resize += (s, e) => btnTroVe.Location = new Point(pnlFooter.Width - 144, 12);
            btnTroVe.Location = new Point(356, 12);

            btnTest.Click   += BtnTest_Click;
            btnLuu.Click    += BtnLuu_Click;
            btnTroVe.Click  += (s, e) => this.Close();

            pnlFooter.Controls.AddRange(new Control[] { btnTest, btnLuu, btnTroVe });

            // ── Body ────────────────────────────────────────
            var pnlBody = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = BgGray,
                Padding = new Padding(20, 14, 20, 8)
            };

            // Section label
            var lblSec = new Label
            {
                Text = "KẾT NỐI CƠ SỞ DỮ LIỆU",
                Dock = DockStyle.Top, Height = 32,
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                ForeColor = Color.White, BackColor = Color.FromArgb(50, 80, 130),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // TableLayoutPanel chứa tất cả fields
            var tbl = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 2,
                RowCount = 10,
                BackColor = Color.White,
                Padding = new Padding(12, 8, 12, 12),
                AutoSize = true
            };
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 145));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 52)); // 0: Server
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 52)); // 1: Database
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 62)); // 2: Auth radios
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 52)); // 3: User
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 52)); // 4: Pass
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 32)); // 5: Status
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 36)); // 6: AI header
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 52)); // 7: API Key
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 52)); // 8: Model
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 32)); // 9: AI hint

            // Row 0 — Server
            tbl.Controls.Add(TblLbl("Tên Server:"), 0, 0);
            txtServer = TblTxt(); tbl.Controls.Add(txtServer, 1, 0);

            // Row 1 — Database
            tbl.Controls.Add(TblLbl("Tên Database:"), 0, 1);
            txtDatabase = TblTxt(); tbl.Controls.Add(txtDatabase, 1, 1);

            // Row 2 — Auth
            tbl.Controls.Add(TblLbl("Kiểu xác thực:"), 0, 2);
            var pnlRadio = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            rdoWindows = new RadioButton
            {
                Text = "Windows Authentication",
                Location = new Point(0, 6), AutoSize = true,
                Font = new Font("Segoe UI", 9.5F), Checked = true
            };
            rdoSQL = new RadioButton
            {
                Text = "SQL Server Authentication",
                Location = new Point(0, 30), AutoSize = true,
                Font = new Font("Segoe UI", 9.5F)
            };
            rdoWindows.CheckedChanged += (s, e) => ToggleSQLAuth();
            rdoSQL.CheckedChanged     += (s, e) => ToggleSQLAuth();
            pnlRadio.Controls.Add(rdoWindows);
            pnlRadio.Controls.Add(rdoSQL);
            tbl.Controls.Add(pnlRadio, 1, 2);

            // Row 3 — SQL User
            tbl.Controls.Add(TblLbl("Username SQL:"), 0, 3);
            txtUser = TblTxt(); tbl.Controls.Add(txtUser, 1, 3);

            // Row 4 — SQL Pass
            tbl.Controls.Add(TblLbl("Mật khẩu SQL:"), 0, 4);
            txtPass = TblTxt(); txtPass.UseSystemPasswordChar = true;
            tbl.Controls.Add(txtPass, 1, 4);

            // Row 5 — Status
            tbl.Controls.Add(new Label(), 0, 5);
            lblStatus = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 120, 160),
                Text = "Chưa kiểm tra kết nối.",
                TextAlign = ContentAlignment.MiddleLeft
            };
            tbl.Controls.Add(lblStatus, 1, 5);

            // Row 6 — AI section header
            var lblAiSec = new Label
            {
                Text = "  🤖  CẤU HÌNH AI (OPENAI)",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(40, 90, 155),
                TextAlign = ContentAlignment.MiddleLeft
            };
            tbl.Controls.Add(lblAiSec, 0, 6);
            tbl.SetColumnSpan(lblAiSec, 2);

            // Row 7 — OpenAI API Key
            tbl.Controls.Add(TblLbl("OpenAI API Key:"), 0, 7);
            txtApiKey = TblTxt();
            txtApiKey.UseSystemPasswordChar = true;
            tbl.Controls.Add(txtApiKey, 1, 7);

            // Row 8 — Model
            tbl.Controls.Add(TblLbl("Model AI:"), 0, 8);
            txtAiModel = TblTxt();
            tbl.Controls.Add(txtAiModel, 1, 8);

            // Row 9 — Hint
            tbl.Controls.Add(new Label(), 0, 9);
            var lblAiHint = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 120, 160),
                Text = "Lấy API Key tại: platform.openai.com  |  Model mặc định: gpt-4o-mini",
                TextAlign = ContentAlignment.MiddleLeft
            };
            tbl.Controls.Add(lblAiHint, 1, 9);

            // Bọc section + table vào wrapper panel
            var pnlCard = new Panel { Dock = DockStyle.Top, BackColor = Color.Transparent };
            pnlCard.Controls.Add(tbl);
            pnlCard.Controls.Add(lblSec);
            pnlCard.AutoSize = true;

            pnlBody.Controls.Add(pnlCard);

            // ── Assemble theo đúng thứ tự Dock ──────────────
            this.Controls.Add(pnlBody);
            this.Controls.Add(pnlFooter);
            this.Controls.Add(pnlHeader);

            ToggleSQLAuth();
        }

        // ── Helpers ───────────────────────────────────────────
        Label TblLbl(string text) => new Label
        {
            Text = text, Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 9F), ForeColor = Color.FromArgb(68, 82, 110),
            TextAlign = ContentAlignment.MiddleLeft
        };

        TextBox TblTxt() => new TextBox
        {
            Dock = DockStyle.Fill, Margin = new Padding(0, 12, 0, 12),
            Font = new Font("Segoe UI", 9.5F), BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.FromArgb(250, 251, 255)
        };

        Button MakeBtn(string text, Color bg)
        {
            var b = new Button
            {
                Text = text, Size = new Size(128, 34),
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

        void ToggleSQLAuth()
        {
            if (txtUser == null || txtPass == null) return;
            bool sql = rdoSQL.Checked;
            txtUser.Enabled   = sql;
            txtPass.Enabled   = sql;
            txtUser.BackColor = sql ? Color.FromArgb(250, 251, 255) : Color.FromArgb(235, 238, 248);
            txtPass.BackColor = sql ? Color.FromArgb(250, 251, 255) : Color.FromArgb(235, 238, 248);
        }

        // ── Config I/O ───────────────────────────────────────
        void LoadConfig()
        {
            if (!File.Exists(ConfigFile))
            {
                txtServer.Text     = "localhost";
                txtDatabase.Text   = "quanlybanhang1";
                rdoWindows.Checked = true;
                return;
            }
            foreach (var line in File.ReadAllLines(ConfigFile))
            {
                var p = line.Split('=');
                if (p.Length < 2) continue;
                string key = p[0].Trim();
                string val = string.Join("=", p, 1, p.Length - 1).Trim();
                switch (key)
                {
                    case "Server":   txtServer.Text   = val; break;
                    case "Database": txtDatabase.Text = val; break;
                    case "AuthType":
                        rdoSQL.Checked     = val == "SQL";
                        rdoWindows.Checked = val != "SQL";
                        break;
                    case "User":        txtUser.Text   = val; break;
                    case "Pass":        txtPass.Text   = val; break;
                    case "OpenAI_Key":  txtApiKey.Text = val; break;
                    case "OpenAI_Model": txtAiModel.Text = val; break;
                }
            }
        }

        void SaveConfig()
        {
            File.WriteAllLines(ConfigFile, new[]
            {
                "Server="        + txtServer.Text.Trim(),
                "Database="      + txtDatabase.Text.Trim(),
                "AuthType="      + (rdoSQL.Checked ? "SQL" : "Windows"),
                "User="          + txtUser.Text.Trim(),
                "Pass="          + txtPass.Text.Trim(),
                "OpenAI_Key="    + txtApiKey.Text.Trim(),
                "OpenAI_Model="  + txtAiModel.Text.Trim()
            });
        }

        string BuildConnStr()
        {
            string srv = txtServer.Text.Trim();
            string db  = txtDatabase.Text.Trim();
            if (rdoWindows.Checked)
                return "Data Source=" + srv + ";Database=" + db +
                       ";Integrated Security=True;TrustServerCertificate=True;";
            return "Data Source=" + srv + ";Database=" + db +
                   ";User Id=" + txtUser.Text.Trim() +
                   ";Password=" + txtPass.Text.Trim() +
                   ";TrustServerCertificate=True;";
        }

        void BtnTest_Click(object s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtServer.Text) || string.IsNullOrWhiteSpace(txtDatabase.Text))
            { SetStatus("Vui lòng nhập đủ Server và Database.", false); return; }

            btnTest.Enabled = false;
            SetStatus("Đang kết nối...", null);
            Application.DoEvents();
            try
            {
                using (var conn = new SqlConnection(BuildConnStr()))
                {
                    conn.Open();
                    SetStatus("✔  Kết nối thành công! (SQL Server " + conn.ServerVersion + ")", true);
                }
            }
            catch (Exception ex) { SetStatus("✘  Lỗi: " + ex.Message, false); }
            finally { btnTest.Enabled = true; }
        }

        void BtnLuu_Click(object s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtServer.Text) || string.IsNullOrWhiteSpace(txtDatabase.Text))
            { FormHelper.ShowWarn("Vui lòng nhập đủ Server và Database."); return; }
            try
            {
                SaveConfig();
                FormHelper.ShowOK("Đã lưu cấu hình.\nKhởi động lại ứng dụng để áp dụng thay đổi.");
            }
            catch (Exception ex) { FormHelper.ShowError("Không lưu được: " + ex.Message); }
        }

        void SetStatus(string msg, bool? ok)
        {
            lblStatus.Text = msg;
            if (ok == null)       lblStatus.ForeColor = Color.FromArgb(100, 120, 160);
            else if (ok == true)  lblStatus.ForeColor = Color.FromArgb(20, 130, 60);
            else                  lblStatus.ForeColor = Color.FromArgb(180, 40, 40);
        }
    }
}
