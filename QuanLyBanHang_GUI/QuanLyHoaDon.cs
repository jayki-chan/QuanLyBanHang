using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBanHang_BUS;
using QuanLyBanHang_DTO;

namespace QuanLyBanHang_GUI
{
    public partial class QuanLyHoaDon : Form
    {
        // ── BUS ───────────────────────────────────────────────
        private readonly HoaDonBUS   _bus    = new HoaDonBUS();
        private readonly KhachHangBUS _busKH = new KhachHangBUS();
        private readonly NhanVienBUS  _busNV = new NhanVienBUS();

        // ── Controls ─────────────────────────────────────────
        Panel pnlInput;
        DataGridView dgv;
        Button btnReload, btnThem, btnSua, btnLuu, btnHuybo, btnXoa, btnXuat;
        TextBox txtMaHD, txtTimKiem;
        ComboBox cboKH, cboNV, cboLoaiHD, cboFilter;
        DateTimePicker dtpLap, dtpNhan;
        bool _adding;
        DataTable _dtData;

        public QuanLyHoaDon()
        {
            BuildUI();
            LoadCombos();
            FormHelper.SetEditMode(false, pnlInput, btnLuu, btnHuybo, btnThem, btnSua, btnXoa, btnReload);
            Load_();
        }

        void BuildUI()
        {
            this.Text = "Quản Lý Danh Mục Hóa Đơn";
            this.ClientSize = new Size(980, 560);
            this.MinimumSize = new Size(820, 440);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = FormHelper.BgGray;
            this.FormBorderStyle = FormBorderStyle.Sizable;

            var (hdr, _) = FormHelper.BuildHeader("QUẢN LÝ DANH MỤC HÓA ĐƠN");
            pnlInput = FormHelper.BuildInputPanel(120);
            var (pnlGrid, grid) = FormHelper.BuildGridPanel();
            dgv = grid;
            var footer = FormHelper.BuildFooter(out btnReload, out btnThem, out btnSua,
                out btnLuu, out btnHuybo, out btnXoa, (s, e) => this.Close());

            // Thêm nút Xuất vào cuối thanh Footer
            btnXuat = FormHelper.MakeBtn("📤  Xuất", Color.FromArgb(70, 130, 180));
            btnXuat.Click += (s, e) => {
                if (dgv.CurrentRow == null) { FormHelper.ShowWarn("Chọn hóa đơn cần xuất."); return; }
                string ma = dgv.CurrentRow.Cells[0].Value.ToString();
                new PhieuHoaDon(ma).ShowDialog();
            };
            if (footer.Controls[0] is FlowLayoutPanel flow) flow.Controls.Add(btnXuat);
            // Tự động khóa nút Xuất khi đang ở chế độ Thêm/Sửa
            pnlInput.EnabledChanged += (s, e) => { if (btnXuat != null) btnXuat.Enabled = !pnlInput.Enabled; };

            (_, txtMaHD) = FormHelper.MakeField(pnlInput,     "Mã Hóa Đơn",       14,  110);
            (_, cboKH)   = FormHelper.MakeCombo(pnlInput,     "Khách Hàng",       138,  240);
            (_, cboNV)   = FormHelper.MakeCombo(pnlInput,     "Nhân Viên",        392,  220);
            (_, cboLoaiHD) = FormHelper.MakeCombo(pnlInput,   "Loại HĐ",          630,  120);
            (_, dtpLap)  = FormHelper.MakeDatePicker(pnlInput,"Ngày Lập Hóa Đơn", 14,  160, 80);
            (_, dtpNhan) = FormHelper.MakeDatePicker(pnlInput,"Ngày Nhận Hàng",   188,  160, 80);

            Panel pnlSearch = new Panel { Dock = DockStyle.Top, Height = 46, BackColor = FormHelper.BgGray };
            Label lblTimKiem = new Label { Text = "🔍 Tìm kiếm (Mã HĐ/Khách Hàng/NV):", Location = new Point(14, 14), AutoSize = true, Font = new Font("Segoe UI", 8.5F), ForeColor = Color.FromArgb(68, 82, 110) };
            txtTimKiem = new TextBox { Location = new Point(230, 10), Size = new Size(300, 26), Font = new Font("Segoe UI", 9.5F) };
            
            cboFilter = new ComboBox { Location = new Point(540, 10), Size = new Size(120, 26), Font = new Font("Segoe UI", 9.5F), DropDownStyle = ComboBoxStyle.DropDownList };
            cboFilter.Items.AddRange(new string[] { "Tất cả", "N - Nhập", "X - Xuất" });
            cboFilter.SelectedIndex = 0;

            Action applyFilter = () => {
                if (_dtData == null) return;
                string kw = txtTimKiem.Text.Trim().Replace("'", "''");
                string fLoai = cboFilter.SelectedIndex == 1 ? "N" : (cboFilter.SelectedIndex == 2 ? "X" : "");
                string rowFilter = $"([Mã HĐ] LIKE '%{kw}%' OR [_MaKH] LIKE '%{kw}%' OR [Khách Hàng] LIKE '%{kw}%' OR [Nhân Viên] LIKE '%{kw}%')";
                if (!string.IsNullOrEmpty(fLoai)) rowFilter += $" AND [Loại] = '{fLoai}'";
                _dtData.DefaultView.RowFilter = rowFilter;
            };
            txtTimKiem.TextChanged += (s, e) => applyFilter();
            cboFilter.SelectedIndexChanged += (s, e) => applyFilter();

            pnlSearch.Controls.Add(lblTimKiem);
            pnlSearch.Controls.Add(txtTimKiem);
            pnlSearch.Controls.Add(cboFilter);

            this.Controls.Add(pnlGrid);
            this.Controls.Add(pnlSearch);
            this.Controls.Add(footer);
            this.Controls.Add(pnlInput);
            this.Controls.Add(hdr);

            btnReload.Click += (s, e) => Load_();
            btnThem.Click   += (s, e) => { _adding = true; ClearFields(); FormHelper.SetEditMode(true, pnlInput, btnLuu, btnHuybo, btnThem, btnSua, btnXoa, btnReload); txtMaHD.ReadOnly = false; txtMaHD.Focus(); };
            btnSua.Click    += (s, e) => StartEdit();
            btnLuu.Click    += (s, e) => Save();
            btnHuybo.Click  += (s, e) => { ClearFields(); FormHelper.SetEditMode(false, pnlInput, btnLuu, btnHuybo, btnThem, btnSua, btnXoa, btnReload); };
            btnXoa.Click    += (s, e) => Delete();
            dgv.CellClick   += (s, e) => { if (e.RowIndex >= 0) FillRow(e.RowIndex); };

            // Sự kiện đúp chuột mở Form Biên lai Hóa Đơn
            dgv.CellDoubleClick += (s, e) => {
                if (e.RowIndex < 0) return;
                string maHD = dgv.Rows[e.RowIndex].Cells[0].Value?.ToString();
                if (!string.IsNullOrEmpty(maHD))
                {
                    try {
                        new QuanLyChiTietHoaDon(maHD).ShowDialog(); // Mở form Chi tiết HĐ
                    } catch (Exception ex) {
                        FormHelper.ShowError("Không thể mở Form Chi Tiết: " + ex.Message);
                    }
                }
            };
        }

        // ── Load Combos từ BUS ────────────────────────────────
        void LoadCombos()
        {
            var dtKH = new DataTable();
            dtKH.Columns.Add("MaKH"); dtKH.Columns.Add("HT");
            foreach (var kh in _busKH.GetAll())    // ← BUS
                dtKH.Rows.Add(kh.MaKH, kh.MaKH + " — " + kh.TenCty);
            cboKH.DataSource = dtKH; cboKH.DisplayMember = "HT"; cboKH.ValueMember = "MaKH";

            var dtNV = new DataTable();
            dtNV.Columns.Add("MaNV"); dtNV.Columns.Add("HT");
            foreach (var nv in _busNV.GetAll())    // ← BUS
                dtNV.Rows.Add(nv.MaNV, nv.MaNV + " — " + nv.HoTen);
            cboNV.DataSource = dtNV; cboNV.DisplayMember = "HT"; cboNV.ValueMember = "MaNV";

            var dtLoai = new DataTable();
            dtLoai.Columns.Add("Ma"); dtLoai.Columns.Add("Ten");
            dtLoai.Rows.Add("X", "Xuất");
            dtLoai.Rows.Add("N", "Nhập");
            cboLoaiHD.DataSource = dtLoai;
            cboLoaiHD.DisplayMember = "Ten"; cboLoaiHD.ValueMember = "Ma";
        }

        // ── Load danh sách ───────────────────────────────────
        void Load_()
        {
            try
            {
                var list = _bus.GetAll();            // ← BUS
                _dtData = new DataTable();
                _dtData.Columns.Add("Mã HĐ"); _dtData.Columns.Add("Khách Hàng");
                _dtData.Columns.Add("Nhân Viên"); _dtData.Columns.Add("Ngày Lập"); _dtData.Columns.Add("Ngày Nhận");
                _dtData.Columns.Add("Loại");
                _dtData.Columns.Add("_MaKH");
                foreach (var hd in list)
                {
                    string loai = hd.GetType().GetProperty("LoaiHD")?.GetValue(hd, null)?.ToString() ?? "X";
                    _dtData.Rows.Add(hd.MaHD, hd.TenCty, hd.HoTenNV,
                        hd.NgayLapHD.ToString("dd/MM/yyyy"),
                        hd.NgayNhanHang.ToString("dd/MM/yyyy"), loai, hd.MaKH);
                }
                dgv.DataSource = _dtData;
                if (dgv.Columns.Contains("_MaKH")) dgv.Columns["_MaKH"].Visible = false;
                ClearFields();
            }
            catch (Exception ex) { FormHelper.ShowError(ex.Message); }
        }

        void StartEdit()
        {
            if (dgv.CurrentRow == null) { FormHelper.ShowWarn("Chọn dòng cần sửa."); return; }
            _adding = false;
            FillRow(dgv.CurrentRow.Index);
            txtMaHD.ReadOnly = true;
            FormHelper.SetEditMode(true, pnlInput, btnLuu, btnHuybo, btnThem, btnSua, btnXoa, btnReload);
        }

        // ── Save ─────────────────────────────────────────────
        void Save()
        {
            var dto = new HoaDonDTO
            {
                MaHD         = txtMaHD.Text.Trim(),
                MaKH         = cboKH.SelectedValue?.ToString() ?? "",
                MaNV         = cboNV.SelectedValue?.ToString() ?? "",
                NgayLapHD    = dtpLap.Value.Date,
                NgayNhanHang = dtpNhan.Value.Date
            };

            var prop = dto.GetType().GetProperty("LoaiHD");
            if (prop != null) prop.SetValue(dto, cboLoaiHD.SelectedValue?.ToString() ?? "X", null);

            var (ok, msg) = _adding ? _bus.Insert(dto) : _bus.Update(dto);  // ← BUS
            
            bool wasAdding = _adding; // Lưu lại trạng thái để mở form chi tiết

            if (ok)
            {
                FormHelper.ShowOK(msg);
                Load_();
                FormHelper.SetEditMode(false, pnlInput, btnLuu, btnHuybo, btnThem, btnSua, btnXoa, btnReload);

                // Tự động mở form chi tiết cho hóa đơn vừa tạo
                if (wasAdding) new QuanLyChiTietHoaDon(dto.MaHD).ShowDialog();
            }
            else FormHelper.ShowWarn(msg);
        }

        // ── Delete ───────────────────────────────────────────
        void Delete()
        {
            if (dgv.CurrentRow == null) { FormHelper.ShowWarn("Chọn dòng cần xóa."); return; }
            string ma = dgv.CurrentRow.Cells[0].Value.ToString();
            if (!FormHelper.Confirm($"Xóa Hóa Đơn '{ma}'? (Sẽ xóa cả chi tiết hóa đơn)")) return;

            var (ok, msg) = _bus.Delete(ma);          // ← BUS (tự xóa chi tiết trước)
            if (ok) { FormHelper.ShowOK(msg); Load_(); }
            else FormHelper.ShowError(msg);
        }

        void FillRow(int r)
        {
            string ma = dgv.Rows[r].Cells[0].Value?.ToString();
            txtMaHD.Text = ma;

            var hd = _bus.GetByMa(ma);
            if (hd != null)
            {
                try { cboKH.SelectedValue = hd.MaKH; } catch { }
                try { cboNV.SelectedValue = hd.MaNV; } catch { }
                dtpLap.Value  = hd.NgayLapHD;
                dtpNhan.Value = hd.NgayNhanHang;
                var prop = hd.GetType().GetProperty("LoaiHD");
                if (prop != null) cboLoaiHD.SelectedValue = prop.GetValue(hd)?.ToString() ?? "X";
            }
        }

        void ClearFields()
        {
            txtMaHD.Clear();
            dtpLap.Value = dtpNhan.Value = DateTime.Today;
            if (cboKH.Items.Count > 0) cboKH.SelectedIndex = 0;
            if (cboNV.Items.Count > 0) cboNV.SelectedIndex = 0;
            if (cboLoaiHD.Items.Count > 0) cboLoaiHD.SelectedIndex = 0;
        }
    }
}
