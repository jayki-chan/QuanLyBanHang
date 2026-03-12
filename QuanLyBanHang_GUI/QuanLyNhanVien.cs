using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using QuanLyBanHang_BUS;
using QuanLyBanHang_DTO;

namespace QuanLyBanHang_GUI
{
    public partial class QuanLyNhanVien : Form
    {
        // ── BUS ───────────────────────────────────────────────
        private readonly NhanVienBUS _bus = new NhanVienBUS();

        // ── Controls ─────────────────────────────────────────
        Panel pnlInput, pnlAvatar;
        PictureBox picAvatar;
        Button btnChonAnh;
        DataGridView dgv;
        Button btnReload, btnThem, btnSua, btnLuu, btnHuybo, btnXoa;
        TextBox txtMa, txtHo, txtTen, txtDiaChi, txtDT, txtUser, txtPass, txtSearch;
        CheckBox chkGiuPass;
        RadioButton rdoNam, rdoNu;
        DateTimePicker dtpNgay;
        ComboBox cboQuyen;

        bool _adding;
        string _anhPath = "";
        DataTable _masterDt;
        EventHandler _chkGiuPassHandler;

        static readonly Color NavBlue     = Color.FromArgb(30, 55, 100);
        static readonly Color NavBlueLite = Color.FromArgb(242, 246, 255);
        static readonly Color BgGray      = Color.FromArgb(245, 246, 250);
        static readonly Color BorderCol   = Color.FromArgb(208, 214, 228);

        public QuanLyNhanVien()
        {
            BuildUI();
            SetEditMode(false);
            Load_();
        }

        void BuildUI()
        {
            this.Text = "Quản Lý Danh Mục Nhân Viên";
            this.ClientSize = new Size(1060, 620);
            this.MinimumSize = new Size(900, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BgGray;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.Font = new Font("Segoe UI", 9F);

            // ── Header ──────────────────────────────────────
            var pnlHeader = new Panel { BackColor = NavBlue, Dock = DockStyle.Top, Height = 52 };
            pnlHeader.Controls.Add(new Label
            {
                Dock = DockStyle.Fill, Text = "QUẢN LÝ DANH MỤC NHÂN VIÊN",
                Font = new Font("Segoe UI Semibold", 13.5F, FontStyle.Bold),
                ForeColor = Color.White, TextAlign = ContentAlignment.MiddleCenter
            });

            // ── Avatar Panel ─────────────────────────────────
            pnlAvatar = new Panel { BackColor = NavBlueLite, Dock = DockStyle.Right, Width = 130 };
            pnlAvatar.Paint += (s, e) => e.Graphics.DrawLine(new Pen(BorderCol), 0, 0, 0, pnlAvatar.Height);

            picAvatar = new PictureBox
            {
                Size = new Size(106, 100), Location = new Point(12, 8),
                SizeMode = PictureBoxSizeMode.Zoom, BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White, Image = MakeDefaultAvatar()
            };
            btnChonAnh = new Button
            {
                Text = "📷 Chọn ảnh", Size = new Size(106, 26), Location = new Point(12, 114),
                FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(50, 80, 130),
                ForeColor = Color.White, Cursor = Cursors.Hand, Font = new Font("Segoe UI", 8.5F)
            };
            btnChonAnh.FlatAppearance.BorderSize = 0;
            btnChonAnh.Click += (s, e) => ChonAnh();
            pnlAvatar.Controls.AddRange(new Control[] { picAvatar, btnChonAnh,
                new Label { Text="Ảnh đại diện", Location=new Point(12,142), Size=new Size(106,18),
                    ForeColor=Color.FromArgb(100,120,160), Font=new Font("Segoe UI",7.5F), TextAlign=ContentAlignment.MiddleCenter }
            });

            // ── Input Panel ─────────────────────────────────
            pnlInput = new Panel
            {
                BackColor = NavBlueLite, Dock = DockStyle.Fill,
                Padding = new Padding(14, 8, 14, 6), Height = 140
            };
            pnlInput.Paint += (s, e) => e.Graphics.DrawLine(new Pen(BorderCol), 0, pnlInput.Height - 1, pnlInput.Width, pnlInput.Height - 1);

            // Row 1
            Field(pnlInput, "Mã NV",         14, 10, 90,  out txtMa);
            Field(pnlInput, "Họ",            118, 10, 140, out txtHo);
            Field(pnlInput, "Tên",           272, 10, 130, out txtTen);
            AddLbl(pnlInput, "Giới Tính",    416, 10);
            rdoNam = new RadioButton { Text = "Nam", Location = new Point(416, 28), AutoSize = true, Font = new Font("Segoe UI", 9.5F), Checked = true };
            rdoNu  = new RadioButton { Text = "Nữ",  Location = new Point(475, 28), AutoSize = true, Font = new Font("Segoe UI", 9.5F) };
            pnlInput.Controls.AddRange(new Control[] { rdoNam, rdoNu });
            AddLbl(pnlInput, "Ngày Vào Làm", 530, 10);
            dtpNgay = new DateTimePicker { Location = new Point(530, 28), Size = new Size(155, 24), Font = new Font("Segoe UI", 9.5F), Format = DateTimePickerFormat.Short };
            pnlInput.Controls.Add(dtpNgay);

            // Row 2
            Field(pnlInput, "Địa Chỉ",    14, 68, 230, out txtDiaChi);
            Field(pnlInput, "Điện Thoại", 258, 68, 145, out txtDT);
            Field(pnlInput, "Username",   417, 68, 140, out txtUser);
            Field(pnlInput, "Mật Khẩu",   571, 68, 130, out txtPass);
            txtPass.PasswordChar = '●';

            // CheckBox giữ mật khẩu cũ (chỉ hiện khi sửa)
            chkGiuPass = new CheckBox
            {
                Text = "Giữ mật khẩu cũ (không đổi)",
                Location = new Point(571, 112), AutoSize = true,
                Font = new Font("Segoe UI", 8.5F), ForeColor = Color.FromArgb(60, 80, 130),
                Visible = false
            };
            pnlInput.Controls.Add(chkGiuPass);

            // ComboBox Quyền
            AddLbl(pnlInput, "Quyền", 715, 68);
            cboQuyen = new ComboBox
            {
                Location = new Point(715, 86), Size = new Size(110, 24),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold), FlatStyle = FlatStyle.Flat
            };
            cboQuyen.Items.AddRange(new object[] { "user", "admin" });
            cboQuyen.SelectedIndex = 0;
            pnlInput.Controls.Add(cboQuyen);

            // ── Grid ─────────────────────────────────────────
            dgv = new DataGridView();
            FormHelper.StyleGrid(dgv);
            dgv.CellClick += (s, e) => { if (e.RowIndex >= 0) FillRow(e.RowIndex); };
            var pnlGrid = new Panel { Dock = DockStyle.Fill, Padding = new Padding(14, 10, 14, 0), BackColor = BgGray };
            pnlGrid.Controls.Add(dgv);
            dgv.Dock = DockStyle.Fill;

            // ── Footer ───────────────────────────────────────
            var pnlFooter = new Panel { BackColor = Color.FromArgb(232, 236, 244), Dock = DockStyle.Bottom, Height = 58 };
            pnlFooter.Paint += (s, e) => e.Graphics.DrawLine(new Pen(BorderCol), 0, 0, pnlFooter.Width, 0);
            var flow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false, Padding = new Padding(10, 12, 0, 0), BackColor = Color.Transparent };
            btnReload = Btn("↺  Reload",  Color.FromArgb(85, 110, 155));
            btnThem   = Btn("+  Thêm",    Color.FromArgb(34, 139, 86));
            btnSua    = Btn("✎  Sửa",    Color.FromArgb(175, 118, 18));
            btnLuu    = Btn("💾  Lưu",   NavBlue);
            btnHuybo  = Btn("✕  Hủy Bỏ", Color.FromArgb(130, 48, 48));
            btnXoa    = Btn("🗑  Xóa",   Color.FromArgb(188, 40, 40));
            flow.Controls.AddRange(new Control[] { btnReload, btnThem, btnSua, btnLuu, btnHuybo, btnXoa });
            var rightWrap = new Panel { Dock = DockStyle.Right, Width = 110, BackColor = Color.Transparent };
            var btnTroVe = new Button { Text = "Trở Về", Size = new Size(90, 34), Location = new Point(10, 12), Font = new Font("Segoe UI", 9.5F), BackColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnTroVe.FlatAppearance.BorderColor = Color.FromArgb(175, 188, 212);
            btnTroVe.Click += (s, e) => this.Close();
            rightWrap.Controls.Add(btnTroVe);
            pnlFooter.Controls.Add(flow);
            pnlFooter.Controls.Add(rightWrap);

            // ── Events ───────────────────────────────────────
            btnReload.Click += (s, e) => Load_();
            btnThem.Click   += (s, e) => StartAdd();
            btnSua.Click    += (s, e) => StartEdit();
            btnLuu.Click    += (s, e) => Save();
            btnHuybo.Click  += (s, e) => { ClearFields(); SetEditMode(false); };
            btnXoa.Click    += (s, e) => Delete();

            // ── Search bar ───────────────────────────────────────────
            var pnlSearch = new Panel { BackColor = Color.FromArgb(235, 239, 250), Dock = DockStyle.Top, Height = 38 };
            pnlSearch.Paint += (s, e) => e.Graphics.DrawLine(new Pen(BorderCol), 0, pnlSearch.Height - 1, pnlSearch.Width, pnlSearch.Height - 1);
            pnlSearch.Controls.Add(new Label
            {
                Text = "🔍  Tìm kiếm:", Location = new Point(14, 10), AutoSize = true,
                Font = new Font("Segoe UI", 9F), ForeColor = Color.FromArgb(40, 60, 110)
            });
            txtSearch = new TextBox
            {
                Location = new Point(118, 7), Size = new Size(300, 24),
                Font = new Font("Segoe UI", 9.5F), BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            txtSearch.TextChanged += (s, e) => FilterGrid();
            pnlSearch.Controls.Add(txtSearch);
            pnlSearch.Controls.Add(new Label
            {
                Text = "(Tên, Mã NV, Điện thoại, Tài khoản)",
                Location = new Point(424, 10), AutoSize = true,
                Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                ForeColor = Color.FromArgb(120, 140, 180)
            });

            // ── Assemble ─────────────────────────────────────────────────
            this.Controls.Add(pnlGrid);
            this.Controls.Add(pnlFooter);
            this.Controls.Add(pnlSearch);
            var pnlInputRow = new Panel { Dock = DockStyle.Top, Height = 140, BackColor = NavBlueLite };
            pnlInputRow.Controls.Add(pnlInput);
            pnlInput.Dock = DockStyle.Fill;
            pnlInputRow.Controls.Add(pnlAvatar);
            this.Controls.Add(pnlInputRow);
            this.Controls.Add(pnlHeader);
        }

        // ── Load ─────────────────────────────────────────────
        void Load_()
        {
            try
            {
                var list = _bus.GetAll();        // ← BUS
                var dt = new DataTable();
                dt.Columns.Add("Ảnh", typeof(Image));
                dt.Columns.Add("Mã NV"); dt.Columns.Add("Họ"); dt.Columns.Add("Tên");
                dt.Columns.Add("Giới Tính"); dt.Columns.Add("Ngày Vào");
                dt.Columns.Add("Địa Chỉ"); dt.Columns.Add("Điện Thoại");
                dt.Columns.Add("Tài Khoản"); dt.Columns.Add("Quyền");
                dt.Columns.Add("_Hinh");     // ẩn

                foreach (var nv in list)
                    dt.Rows.Add(LoadAvatarImage(nv.Hinh),
                        nv.MaNV, nv.Ho, nv.Ten,
                        nv.Nu ? "Nữ" : "Nam",
                        nv.NgayNV.ToString("dd/MM/yyyy"),
                        nv.DiaChi, nv.DienThoai, nv.Username, nv.Role, nv.Hinh);

                _masterDt = dt;
                dgv.DataSource = _masterDt;
                if (dgv.Columns.Contains("_Hinh")) dgv.Columns["_Hinh"].Visible = false;
                if (dgv.Columns.Contains("Ảnh"))
                {
                    var imgCol = (DataGridViewImageColumn)dgv.Columns["Ảnh"];
                    imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
                    imgCol.Width = 58;
                    imgCol.ReadOnly = true;
                }
                dgv.RowTemplate.Height = 56;

                // Tô màu cột Quyền
                dgv.CellFormatting += (s, e) =>
                {
                    if (e.RowIndex < 0 || dgv.Columns[e.ColumnIndex].Name != "Quyền") return;
                    e.CellStyle.ForeColor  = e.Value?.ToString() == "admin"
                        ? Color.FromArgb(140, 30, 80) : Color.FromArgb(34, 120, 86);
                    e.CellStyle.Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold);
                };

                ClearFields();
            }
            catch (Exception ex) { FormHelper.ShowError(ex.Message); }
        }

        void StartAdd()
        {
            _adding = true;
            ClearFields();
            chkGiuPass.Visible = false;
            txtPass.Enabled = true;
            SetEditMode(true);
            txtMa.ReadOnly = false;
            txtMa.Focus();
        }

        void StartEdit()
        {
            if (dgv.CurrentRow == null) { FormHelper.ShowWarn("Chọn dòng cần sửa."); return; }
            _adding = false;
            FillRow(dgv.CurrentRow.Index);
            txtMa.ReadOnly = true;
            chkGiuPass.Visible = true;
            chkGiuPass.Checked = true;
            txtPass.Enabled = false;
            // Huỷ handler cũ trước khi đăng ký mới, tránh chồng chất
            if (_chkGiuPassHandler != null)
                chkGiuPass.CheckedChanged -= _chkGiuPassHandler;
            _chkGiuPassHandler = (s, e) => txtPass.Enabled = !chkGiuPass.Checked;
            chkGiuPass.CheckedChanged += _chkGiuPassHandler;
            SetEditMode(true);
            txtHo.Focus();
        }

        // ── Save ─────────────────────────────────────────────
        void Save()
        {
            bool doiPass = !_adding && !chkGiuPass.Checked;

            var dto = new NhanVienDTO
            {
                MaNV      = txtMa.Text.Trim(),
                Ho        = txtHo.Text.Trim(),
                Ten       = txtTen.Text.Trim(),
                Nu        = rdoNu.Checked,
                NgayNV    = dtpNgay.Value.Date,
                DiaChi    = txtDiaChi.Text.Trim(),
                DienThoai = txtDT.Text.Trim(),
                Hinh      = _anhPath,
                Username  = txtUser.Text.Trim(),
                Matkhau   = txtPass.Text.Trim(),
                Role      = cboQuyen.SelectedItem?.ToString() ?? "user"
            };

            var (ok, msg) = _adding
                ? _bus.Insert(dto)                    // ← BUS validate + Insert
                : _bus.Update(dto, doiPass);           // ← BUS validate + Update

            if (ok)
            {
                FormHelper.ShowOK(msg);
                Load_();
                SetEditMode(false);
            }
            else FormHelper.ShowWarn(msg);
        }

        // ── Delete ───────────────────────────────────────────
        void Delete()
        {
            if (dgv.CurrentRow == null) { FormHelper.ShowWarn("Chọn dòng cần xóa."); return; }
            string ma = dgv.CurrentRow.Cells["Mã NV"].Value.ToString();
            if (!FormHelper.Confirm($"Xóa Nhân Viên '{ma}'?")) return;

            var (ok, msg) = _bus.Delete(ma);         // ← BUS
            if (ok) { FormHelper.ShowOK(msg); Load_(); }
            else FormHelper.ShowError(msg);
        }

        void FillRow(int r)
        {
            var row = dgv.Rows[r];
            txtMa.Text     = row.Cells["Mã NV"].Value?.ToString();
            txtHo.Text     = row.Cells["Họ"].Value?.ToString();
            txtTen.Text    = row.Cells["Tên"].Value?.ToString();
            rdoNu.Checked  = row.Cells["Giới Tính"].Value?.ToString() == "Nữ";
            rdoNam.Checked = !rdoNu.Checked;
            if (DateTime.TryParse(row.Cells["Ngày Vào"].Value?.ToString(), out DateTime d)) dtpNgay.Value = d;
            txtDiaChi.Text = row.Cells["Địa Chỉ"].Value?.ToString();
            txtDT.Text     = row.Cells["Điện Thoại"].Value?.ToString();
            txtUser.Text   = row.Cells["Tài Khoản"].Value?.ToString();
            cboQuyen.SelectedItem = row.Cells["Quyền"].Value?.ToString() ?? "user";
            _anhPath = row.Cells["_Hinh"].Value?.ToString() ?? "";
            LoadAvatar(_anhPath);
        }

        void ChonAnh()
        {
            using (var dlg = new OpenFileDialog { Title = "Chọn ảnh", Filter = "Ảnh (*.jpg;*.png;*.bmp)|*.jpg;*.png;*.bmp" })
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    _anhPath = dlg.FileName;
                    try { picAvatar.Image = Image.FromFile(_anhPath); }
                    catch { FormHelper.ShowWarn("Không đọc được ảnh."); }
                }
        }

        void LoadAvatar(string path)
        {
            picAvatar.Image = LoadAvatarImage(path);
        }

        Image LoadAvatarImage(string path)
        {
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
                try { return Image.FromFile(path); } catch { }
            return MakeDefaultAvatar();
        }

        void SetEditMode(bool editing)
        {
            pnlInput.Enabled = editing; pnlAvatar.Enabled = editing;
            btnLuu.Enabled = editing;   btnHuybo.Enabled  = editing;
            btnThem.Enabled = !editing; btnSua.Enabled    = !editing;
            btnXoa.Enabled  = !editing; btnReload.Enabled = !editing;
        }

        void ClearFields()
        {
            txtMa.Clear(); txtHo.Clear(); txtTen.Clear();
            txtDiaChi.Clear(); txtDT.Clear(); txtUser.Clear(); txtPass.Clear();
            rdoNam.Checked = true; dtpNgay.Value = DateTime.Today;
            cboQuyen.SelectedIndex = 0;
            _anhPath = ""; picAvatar.Image = MakeDefaultAvatar();
        }

        void FilterGrid()
        {
            if (_masterDt == null) return;
            string q = txtSearch.Text.Trim();
            _masterDt.DefaultView.RowFilter = string.IsNullOrEmpty(q) ? "" :
                $"[Mã NV] LIKE '%{q.Replace("'", "''")}%' OR " +
                $"[Họ] LIKE '%{q.Replace("'", "''")}%' OR " +
                $"[Tên] LIKE '%{q.Replace("'", "''")}%' OR " +
                $"[Điện Thoại] LIKE '%{q.Replace("'", "''")}%' OR " +
                $"[Tài Khoản] LIKE '%{q.Replace("'", "''")}%'";
        }

        // ── UI helpers ───────────────────────────────────────
        void Field(Panel p, string lbl, int x, int y, int w, out TextBox txt)
        {
            p.Controls.Add(new Label { Text = lbl, Location = new Point(x, y), AutoSize = true, Font = new Font("Segoe UI", 8.5F), ForeColor = Color.FromArgb(68, 82, 110) });
            txt = new TextBox { Location = new Point(x, y + 18), Size = new Size(w, 24), Font = new Font("Segoe UI", 9.5F), BorderStyle = BorderStyle.FixedSingle, BackColor = Color.White };
            p.Controls.Add(txt);
        }
        void AddLbl(Panel p, string text, int x, int y) =>
            p.Controls.Add(new Label { Text = text, Location = new Point(x, y), AutoSize = true, Font = new Font("Segoe UI", 8.5F), ForeColor = Color.FromArgb(68, 82, 110) });
        Button Btn(string text, Color bg)
        {
            var b = new Button { Text = text, Size = new Size(96, 34), Font = new Font("Segoe UI", 9F), BackColor = bg, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Margin = new Padding(0, 0, 5, 0) };
            b.FlatAppearance.BorderSize = 0; return b;
        }
        Image MakeDefaultAvatar()
        {
            var bmp = new Bitmap(106, 100);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.FromArgb(225, 232, 248));
                var br = new SolidBrush(Color.FromArgb(150, 170, 210));
                g.FillEllipse(br, 33, 12, 40, 40);
                g.FillEllipse(br, 15, 58, 76, 50);
            }
            return bmp;
        }
    }
}
