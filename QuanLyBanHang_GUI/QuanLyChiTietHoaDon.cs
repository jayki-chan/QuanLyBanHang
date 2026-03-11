using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBanHang_BUS;
using QuanLyBanHang_DTO;

namespace QuanLyBanHang_GUI
{
    public partial class QuanLyChiTietHoaDon : Form
    {
        // ── BUS ───────────────────────────────────────────────
        private readonly ChiTietHoaDonBUS _bus   = new ChiTietHoaDonBUS();
        private readonly HoaDonBUS        _busHD = new HoaDonBUS();
        private readonly SanPhamBUS       _busSP = new SanPhamBUS();

        // ── Controls ─────────────────────────────────────────
        Panel pnlInput;
        DataGridView dgv;
        Button btnReload, btnThem, btnSua, btnLuu, btnHuybo, btnXoa;
        ComboBox cboHD, cboSP;
        TextBox txtSoLuong;
        Label lblTong;
        bool _adding;

        public QuanLyChiTietHoaDon()
        {
            BuildUI();
            LoadCombos();
            FormHelper.SetEditMode(false, pnlInput, btnLuu, btnHuybo, btnThem, btnSua, btnXoa, btnReload);
            Load_();
        }

        void BuildUI()
        {
            this.Text = "Quản Lý Chi Tiết Hóa Đơn";
            this.ClientSize = new Size(980, 560);
            this.MinimumSize = new Size(820, 440);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = FormHelper.BgGray;
            this.FormBorderStyle = FormBorderStyle.Sizable;

            var (hdr, _) = FormHelper.BuildHeader("QUẢN LÝ CHI TIẾT HÓA ĐƠN");
            pnlInput = FormHelper.BuildInputPanel(72);
            var (pnlGrid, grid) = FormHelper.BuildGridPanel();
            dgv = grid;
            var footer = FormHelper.BuildFooter(out btnReload, out btnThem, out btnSua,
                out btnLuu, out btnHuybo, out btnXoa, (s, e) => this.Close());

            (_, cboHD)      = FormHelper.MakeCombo(pnlInput, "Hóa Đơn",    14,  200);
            (_, cboSP)      = FormHelper.MakeCombo(pnlInput, "Sản Phẩm",  228,  250);
            (_, txtSoLuong) = FormHelper.MakeField(pnlInput, "Số Lượng",  492,   80);

            // Label tổng tiền
            lblTong = new Label
            {
                Text = "", AutoSize = false,
                Location = new Point(590, 10), Size = new Size(250, 46),
                Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 120, 86),
                TextAlign = ContentAlignment.MiddleLeft
            };
            pnlInput.Controls.Add(lblTong);

            txtSoLuong.KeyPress += (s, e) => { if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar)) e.Handled = true; };
            cboHD.SelectedIndexChanged += (s, e) => { Load_(); UpdateTong(); };
            cboSP.SelectedIndexChanged += (s, e) => UpdateTong();
            txtSoLuong.TextChanged     += (s, e) => UpdateTong();

            this.Controls.Add(pnlGrid);
            this.Controls.Add(footer);
            this.Controls.Add(pnlInput);
            this.Controls.Add(hdr);

            btnReload.Click += (s, e) => Load_();
            btnThem.Click   += (s, e) => { _adding = true; ClearFields(); FormHelper.SetEditMode(true, pnlInput, btnLuu, btnHuybo, btnThem, btnSua, btnXoa, btnReload); txtSoLuong.Focus(); };
            btnSua.Click    += (s, e) => StartEdit();
            btnLuu.Click    += (s, e) => Save();
            btnHuybo.Click  += (s, e) => { ClearFields(); FormHelper.SetEditMode(false, pnlInput, btnLuu, btnHuybo, btnThem, btnSua, btnXoa, btnReload); };
            btnXoa.Click    += (s, e) => Delete();
            dgv.CellClick   += (s, e) => { if (e.RowIndex >= 0) FillRow(e.RowIndex); };
        }

        void LoadCombos()
        {
            // ComboBox HoaDon
            var dtHD = new DataTable();
            dtHD.Columns.Add("MaHD"); dtHD.Columns.Add("HT");
            foreach (var hd in _busHD.GetAll())    // ← BUS
                dtHD.Rows.Add(hd.MaHD, hd.MaHD + " — " + hd.TenCty + " (" + hd.NgayLapHD.ToString("dd/MM/yy") + ")");
            cboHD.DataSource = dtHD; cboHD.DisplayMember = "HT"; cboHD.ValueMember = "MaHD";

            // ComboBox SanPham
            var dtSP = new DataTable();
            dtSP.Columns.Add("MaSP"); dtSP.Columns.Add("HT");
            foreach (var sp in _busSP.GetAll())    // ← BUS
                dtSP.Rows.Add(sp.MaSP, sp.MaSP + " — " + sp.TenSP + " (" + sp.DonGia.ToString("N0") + " đ)");
            cboSP.DataSource = dtSP; cboSP.DisplayMember = "HT"; cboSP.ValueMember = "MaSP";
        }

        // ── Load theo HĐ đang chọn ───────────────────────────
        void Load_()
        {
            try
            {
                string maHD = cboHD.SelectedValue?.ToString() ?? "";
                if (string.IsNullOrEmpty(maHD)) return;

                var list = _bus.GetByHoaDon(maHD);  // ← BUS
                var dt = new DataTable();
                dt.Columns.Add("Mã SP"); dt.Columns.Add("Tên Sản Phẩm");
                dt.Columns.Add("ĐVT"); dt.Columns.Add("Đơn Giá"); dt.Columns.Add("Số Lượng"); dt.Columns.Add("Thành Tiền");
                decimal tongHD = 0;
                foreach (var ct in list)
                {
                    dt.Rows.Add(ct.MaSP, ct.TenSP, ct.DonViTinh,
                        ct.DonGia.ToString("N0") + " đ",
                        ct.SoLuong,
                        ct.ThanhTien.ToString("N0") + " đ");
                    tongHD += ct.ThanhTien;
                }
                dgv.DataSource = dt;
                lblTong.Text = $"Tổng HĐ: {tongHD:N0} đ";
            }
            catch (Exception ex) { FormHelper.ShowError(ex.Message); }
        }

        void UpdateTong()
        {
            if (int.TryParse(txtSoLuong.Text, out int sl) && sl > 0)
            {
                var maSP = cboSP.SelectedValue?.ToString();
                if (maSP != null)
                {
                    var sp = _busSP.GetByMa(maSP);  // ← BUS
                    if (sp != null)
                        lblTong.Text = $"Thành tiền: {(sp.DonGia * sl):N0} đ";
                }
            }
        }

        void StartEdit()
        {
            if (dgv.CurrentRow == null) { FormHelper.ShowWarn("Chọn dòng cần sửa."); return; }
            _adding = false;
            FillRow(dgv.CurrentRow.Index);
            cboSP.Enabled = false;   // không đổi mã SP khi sửa
            FormHelper.SetEditMode(true, pnlInput, btnLuu, btnHuybo, btnThem, btnSua, btnXoa, btnReload);
            txtSoLuong.Focus();
        }

        // ── Save ─────────────────────────────────────────────
        void Save()
        {
            if (!int.TryParse(txtSoLuong.Text, out int sl) || sl <= 0)
            { FormHelper.ShowWarn("Số lượng phải là số nguyên dương."); return; }

            var dto = new ChiTietHoaDonDTO
            {
                MaHD    = cboHD.SelectedValue?.ToString() ?? "",
                MaSP    = cboSP.SelectedValue?.ToString() ?? "",
                SoLuong = sl
            };

            var (ok, msg) = _adding
                ? _bus.Insert(dto)   // ← BUS
                : _bus.Update(dto);  // ← BUS

            if (ok)
            {
                FormHelper.ShowOK(msg);
                Load_();
                FormHelper.SetEditMode(false, pnlInput, btnLuu, btnHuybo, btnThem, btnSua, btnXoa, btnReload);
                cboSP.Enabled = true;
            }
            else FormHelper.ShowWarn(msg);
        }

        // ── Delete ───────────────────────────────────────────
        void Delete()
        {
            if (dgv.CurrentRow == null) { FormHelper.ShowWarn("Chọn dòng cần xóa."); return; }
            string maSP = dgv.CurrentRow.Cells[0].Value.ToString();
            string maHD = cboHD.SelectedValue?.ToString() ?? "";
            if (!FormHelper.Confirm($"Xóa sản phẩm '{maSP}' khỏi hóa đơn '{maHD}'?")) return;

            var (ok, msg) = _bus.Delete(maHD, maSP);  // ← BUS
            if (ok) { FormHelper.ShowOK(msg); Load_(); }
            else FormHelper.ShowError(msg);
        }

        void FillRow(int r)
        {
            var row = dgv.Rows[r];
            cboSP.SelectedValue = row.Cells[0].Value?.ToString();
            txtSoLuong.Text     = row.Cells[4].Value?.ToString();
        }
        void ClearFields() { txtSoLuong.Clear(); if (cboSP.Items.Count > 0) cboSP.SelectedIndex = 0; }
    }
}
