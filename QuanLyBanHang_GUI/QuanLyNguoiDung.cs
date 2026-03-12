using QuanLyBanHang_BUS;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyBanHang_GUI
{
    /// <summary>
    /// Quản lý tài khoản đăng nhập của Nhân Viên (Username / Mật khẩu).
    /// Hiển thị danh sách NV, cho phép reset mật khẩu và kích hoạt/khóa tài khoản.
    /// </summary>
    public partial class QuanLyNguoiDung : Form
    {
        static readonly Color NavBlue   = Color.FromArgb(30, 55, 100);
        static readonly Color BgGray    = Color.FromArgb(245, 246, 250);
        static readonly Color InputBg   = Color.FromArgb(242, 246, 255);
        static readonly Color BorderCol = Color.FromArgb(208, 214, 228);

        private readonly NhanVienBUS _busNV = new NhanVienBUS();

        DataGridView dgv;
        TextBox txtMaNV, txtHoTen, txtUser, txtPassMoi, txtPassXacNhan;
        ComboBox cboQuyen;
        Button btnReload, btnLuu, btnResetPass, btnXoa, btnTroVe;
        Label lblPassStrength;
        Panel pnlEdit;
        bool _editing = false;

        public QuanLyNguoiDung()
        {
            BuildUI();
            Load_();
        }

        void BuildUI()
        {
            this.Text = "Quản Lý Người Dùng";
            this.ClientSize = new Size(1020, 600);
            this.MinimumSize = new Size(860, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BgGray;
            this.FormBorderStyle = FormBorderStyle.Sizable;

            // ── Header ──────────────────────────────────────
            var pnlHeader = new Panel { BackColor = NavBlue, Dock = DockStyle.Top, Height = 52 };
            pnlHeader.Controls.Add(new Label
            {
                Dock = DockStyle.Fill, Text = "QUẢN LÝ NGƯỜI DÙNG",
                Font = new Font("Segoe UI Semibold", 13.5F, FontStyle.Bold),
                ForeColor = Color.White, TextAlign = ContentAlignment.MiddleCenter
            });

            // ── Edit Panel (right side) ──────────────────────
            pnlEdit = new Panel
            {
                BackColor = Color.White, Dock = DockStyle.Right, Width = 310,
                Padding = new Padding(16)
            };
            pnlEdit.Paint += (s, e) =>
                e.Graphics.DrawLine(new Pen(BorderCol), 0, 0, 0, pnlEdit.Height);

            // Section header
            var lblEditHdr = new Label
            {
                Text = "THÔNG TIN TÀI KHOẢN",
                Dock = DockStyle.Top, Height = 34,
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                ForeColor = Color.White, BackColor = Color.FromArgb(50, 80, 130),
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlEdit.Controls.Add(lblEditHdr);

            int y = 50;
            EditField(pnlEdit, "Mã Nhân Viên:", ref y, out txtMaNV, readOnly: true);
            EditField(pnlEdit, "Họ và Tên:", ref y, out txtHoTen, readOnly: true);
            EditField(pnlEdit, "Username:", ref y, out txtUser, readOnly: false);

            // Quyền (Role)
            pnlEdit.Controls.Add(new Label
            {
                Text = "Quyền (Role):", Location = new Point(16, y), AutoSize = true,
                Font = new Font("Segoe UI", 8.5F), ForeColor = Color.FromArgb(68, 82, 110)
            });
            y += 18;
            cboQuyen = new ComboBox
            {
                Location = new Point(16, y), Size = new Size(270, 26),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold), FlatStyle = FlatStyle.Flat
            };
            cboQuyen.Items.AddRange(new object[] { "Quản trị viên", "Nhân viên bán hàng", "Nhân viên kho hàng" });
            cboQuyen.SelectedIndex = 1; // mặc định: Nhân viên bán hàng
            pnlEdit.Controls.Add(cboQuyen);
            y += 38;

            // Divider
            var div = new Label
            {
                Text = "──── Đặt lại mật khẩu ────",
                Location = new Point(16, y), Size = new Size(270, 22),
                Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 130, 180),
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlEdit.Controls.Add(div);
            y += 28;

            EditField(pnlEdit, "Mật khẩu mới:", ref y, out txtPassMoi, readOnly: false, isPass: true);
            EditField(pnlEdit, "Xác nhận mật khẩu:", ref y, out txtPassXacNhan, readOnly: false, isPass: true);

            // Strength indicator
            lblPassStrength = new Label
            {
                Location = new Point(16, y), Size = new Size(270, 18),
                Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                ForeColor = Color.Gray, Text = ""
            };
            pnlEdit.Controls.Add(lblPassStrength);
            txtPassMoi.TextChanged += (s, e) => UpdatePassStrength();
            y += 26;

            // Buttons
            btnLuu = EditBtn("Lưu Username", NavBlue, y); y += 44;
            btnResetPass = EditBtn("Đặt lại MK", Color.FromArgb(34, 120, 80), y); y += 44;

            btnLuu.Click      += BtnLuu_Click;
            btnResetPass.Click += BtnResetPass_Click;

            pnlEdit.Controls.Add(btnLuu);
            pnlEdit.Controls.Add(btnResetPass);

            // ── Grid ────────────────────────────────────────
            dgv = new DataGridView();
            FormHelper.StyleGrid(dgv);
            dgv.RowTemplate.Height = 36;
            dgv.CellClick += DgvCellClick;
            var pnlGrid = new Panel { Dock = DockStyle.Fill, Padding = new Padding(14, 10, 14, 0), BackColor = BgGray };
            pnlGrid.Controls.Add(dgv);

            // ── Footer ──────────────────────────────────────
            var pnlFooter = new Panel { BackColor = Color.FromArgb(232, 236, 244), Dock = DockStyle.Bottom, Height = 52 };
            pnlFooter.Paint += (s, e) =>
                e.Graphics.DrawLine(new Pen(BorderCol), 0, 0, pnlFooter.Width, 0);

            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false, Padding = new Padding(10, 10, 0, 0), BackColor = Color.Transparent
            };

            btnReload = FooterBtn("Reload", Color.FromArgb(85, 110, 155));
            btnXoa    = FooterBtn("Xóa TK", Color.FromArgb(188, 40, 40));
            btnTroVe  = FooterBtn("Trở Về",    Color.White);
            btnTroVe.ForeColor = Color.FromArgb(48, 62, 90);
            btnTroVe.FlatAppearance.BorderColor = BorderCol;
            btnTroVe.FlatAppearance.BorderSize = 1;

            btnReload.Click += (s, e) => Load_();
            btnXoa.Click    += BtnXoa_Click;
            btnTroVe.Click  += (s, e) => this.Close();
            btnTroVe.Dock = DockStyle.Right; btnTroVe.Width = 100;

            flow.Controls.AddRange(new Control[] { btnReload, btnXoa });
            pnlFooter.Controls.Add(flow);
            pnlFooter.Controls.Add(btnTroVe);

            this.Controls.Add(pnlGrid);
            this.Controls.Add(pnlEdit);
            this.Controls.Add(pnlFooter);
            this.Controls.Add(pnlHeader);
        }

        // ── Field builders ────────────────────────────────────
        void EditField(Panel p, string label, ref int y, out TextBox txt, bool readOnly, bool isPass = false)
        {
            p.Controls.Add(new Label
            {
                Text = label, Location = new Point(16, y), AutoSize = true,
                Font = new Font("Segoe UI", 8.5F), ForeColor = Color.FromArgb(68, 82, 110)
            });
            y += 18;
            txt = new TextBox
            {
                Location = new Point(16, y), Size = new Size(270, 26),
                Font = new Font("Segoe UI", 9.5F), BorderStyle = BorderStyle.FixedSingle,
                BackColor = readOnly ? Color.FromArgb(235, 238, 248) : Color.White,
                ReadOnly = readOnly
            };
            if (isPass) txt.UseSystemPasswordChar = true;
            p.Controls.Add(txt);
            y += 34;
        }

        Button EditBtn(string text, Color bg, int y)
        {
            var b = new Button
            {
                Text = text, Location = new Point(16, y), Size = new Size(270, 34),
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

        Button FooterBtn(string text, Color bg)
        {
            var b = new Button
            {
                Text = text, Size = new Size(110, 32),
                Font = new Font("Segoe UI", 9F), BackColor = bg,
                ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand, Margin = new Padding(0, 0, 6, 0)
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

        // ── Load data ─────────────────────────────────────────
        void Load_()
        {
            try
            {
                var list = _busNV.GetAll();
                var dt = new DataTable();
                dt.Columns.Add("Mã NV");
                dt.Columns.Add("Họ và Tên");
                dt.Columns.Add("Username");
                dt.Columns.Add("Quyền");
                dt.Columns.Add("Trạng Thái");
                foreach (var nv in list)
                {
                    bool hasAcc = !string.IsNullOrEmpty(nv.Username);
                    dt.Rows.Add(nv.MaNV, nv.HoTen, nv.Username, nv.RoleDisplay,
                        hasAcc ? "Có tài khoản" : "Chưa có tài khoản");
                }
                dgv.DataSource = dt;
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    bool active = row.Cells["Trạng Thái"].Value?.ToString() == "Có tài khoản";
                    row.Cells["Trạng Thái"].Style.ForeColor = active ? Color.FromArgb(20, 130, 60) : Color.FromArgb(160, 60, 60);
                    row.Cells["Trạng Thái"].Style.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
                }
                ClearEdit();
            }
            catch (Exception ex) { FormHelper.ShowError(ex.Message); }
        }

        void DgvCellClick(object s, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgv.Rows[e.RowIndex];
            txtMaNV.Text  = row.Cells["Mã NV"].Value?.ToString();
            txtHoTen.Text = row.Cells["Họ và Tên"].Value?.ToString();
            txtUser.Text  = row.Cells["Username"].Value?.ToString();
            // "Quyền" cột hiển thị tên tiếng Việt, cần map về item có trong combo
            string roleDisplay = row.Cells["Quyền"].Value?.ToString() ?? "Nhân viên bán hàng";
            cboQuyen.SelectedItem = cboQuyen.Items.Contains(roleDisplay) ? roleDisplay : "Nhân viên bán hàng";
            txtPassMoi.Clear();
            txtPassXacNhan.Clear();
            lblPassStrength.Text = "";
        }

        // ── Đánh giá độ mạnh mật khẩu ────────────────────────
        void UpdatePassStrength()
        {
            string p = txtPassMoi.Text;
            if (p.Length == 0) { lblPassStrength.Text = ""; return; }
            int score = 0;
            if (p.Length >= 8) score++;
            if (System.Text.RegularExpressions.Regex.IsMatch(p, @"[A-Z]")) score++;
            if (System.Text.RegularExpressions.Regex.IsMatch(p, @"[0-9]")) score++;
            if (System.Text.RegularExpressions.Regex.IsMatch(p, @"[^a-zA-Z0-9]")) score++;

            switch (score)
            {
                case 0: case 1:
                    lblPassStrength.Text = "Độ mạnh: Yếu"; lblPassStrength.ForeColor = Color.FromArgb(180, 40, 40); break;
                case 2:
                    lblPassStrength.Text = "Độ mạnh: Trung bình"; lblPassStrength.ForeColor = Color.FromArgb(180, 120, 0); break;
                case 3:
                    lblPassStrength.Text = "Độ mạnh: Khá"; lblPassStrength.ForeColor = Color.FromArgb(0, 140, 80); break;
                default:
                    lblPassStrength.Text = "Độ mạnh: Mạnh 💪"; lblPassStrength.ForeColor = Color.FromArgb(0, 100, 50); break;
            }
        }

        // ── Lưu username ──────────────────────────────────────
        void BtnLuu_Click(object s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaNV.Text)) { FormHelper.ShowWarn("Chọn nhân viên trong danh sách."); return; }
            if (string.IsNullOrWhiteSpace(txtUser.Text)) { FormHelper.ShowWarn("Username không được để trống."); return; }

            string maNV     = txtMaNV.Text.Trim();
            string username = txtUser.Text.Trim();
            // Ánh xạ tên hiển thị → giá trị raw
            string roleDisplay2 = cboQuyen.SelectedItem?.ToString() ?? "Nhân viên bán hàng";
            string role = roleDisplay2 == "Quản trị viên" ? "admin"
                        : roleDisplay2 == "Nhân viên kho hàng" ? "warehouse"
                        : "sales";

            var (okU, msgU) = _busNV.UpdateUsername(maNV, username);
            if (!okU) { FormHelper.ShowWarn(msgU); return; }

            var (okR, msgR) = _busNV.SetRole(maNV, role);
            if (!okR) { FormHelper.ShowWarn(msgR); return; }

            FormHelper.ShowOK("Đã cập nhật Username và Quyền thành công!");
            Load_();
        }

        // ── Reset mật khẩu ───────────────────────────────────
        void BtnResetPass_Click(object s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaNV.Text)) { FormHelper.ShowWarn("Chọn nhân viên trong danh sách."); return; }
            if (string.IsNullOrWhiteSpace(txtPassMoi.Text)) { FormHelper.ShowWarn("Nhập mật khẩu mới."); return; }
            if (txtPassMoi.Text != txtPassXacNhan.Text) { FormHelper.ShowWarn("Mật khẩu xác nhận không khớp."); return; }
            if (!FormHelper.Confirm($"Đặt lại mật khẩu cho '{txtHoTen.Text}' ({txtMaNV.Text})?")) return;

            var (ok, msg) = _busNV.ResetPassword(txtMaNV.Text.Trim(), txtPassMoi.Text.Trim());
            if (ok)
            {
                FormHelper.ShowOK(msg);
                txtPassMoi.Clear(); txtPassXacNhan.Clear(); lblPassStrength.Text = "";
            }
            else FormHelper.ShowWarn(msg);
        }

        // ── Xóa tài khoản (xóa username + pass) ─────────────
        void BtnXoa_Click(object s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaNV.Text)) { FormHelper.ShowWarn("Chọn nhân viên trong danh sách."); return; }
            if (!FormHelper.Confirm($"Xóa tài khoản của '{txtHoTen.Text}'?\n(Username và mật khẩu sẽ bị xóa, nhân viên vẫn còn trong hệ thống)")) return;

            var (ok, msg) = _busNV.DeleteAccount(txtMaNV.Text.Trim());
            if (ok) { FormHelper.ShowOK(msg); Load_(); }
            else FormHelper.ShowError(msg);
        }

        void ClearEdit()
        {
            txtMaNV.Clear(); txtHoTen.Clear(); txtUser.Clear();
            txtPassMoi.Clear(); txtPassXacNhan.Clear();
            lblPassStrength.Text = "";
            if (cboQuyen != null) cboQuyen.SelectedIndex = 1; // mặc định: Nhân viên bán hàng
        }
    }
}
