using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBanHang_BUS;
using QuanLyBanHang_DTO;

namespace QuanLyBanHang_GUI
{
    public partial class QuanLySanPham : Form
    {
        // ── BUS ───────────────────────────────────────────────
        private readonly SanPhamBUS _bus = new SanPhamBUS();

        // ── Controls ─────────────────────────────────────────
        Panel pnlInput;
        DataGridView dgv;
        Button btnReload, btnThem, btnSua, btnLuu, btnHuybo, btnXoa;
        TextBox txtMa, txtTen, txtDVT, txtGia;
        bool _adding;

        public QuanLySanPham()
        {
            BuildUI();
            FormHelper.SetEditMode(false, pnlInput, btnLuu, btnHuybo, btnThem, btnSua, btnXoa, btnReload);
            Load_();
        }

        void BuildUI()
        {
            this.Text = "Quản Lý Danh Mục Sản Phẩm";
            this.ClientSize = new Size(920, 520);
            this.MinimumSize = new Size(760, 420);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = FormHelper.BgGray;
            this.FormBorderStyle = FormBorderStyle.Sizable;

            var (hdr, _) = FormHelper.BuildHeader("QUẢN LÝ DANH MỤC SẢN PHẨM");
            pnlInput = FormHelper.BuildInputPanel(68);
            var (pnlGrid, grid) = FormHelper.BuildGridPanel();
            dgv = grid;
            var footer = FormHelper.BuildFooter(out btnReload, out btnThem, out btnSua,
                out btnLuu, out btnHuybo, out btnXoa, (s, e) => this.Close());

            (_, txtMa)  = FormHelper.MakeField(pnlInput, "Mã SP",          14,  100);
            (_, txtTen) = FormHelper.MakeField(pnlInput, "Tên Sản Phẩm",  128,  260);
            (_, txtDVT) = FormHelper.MakeField(pnlInput, "Đơn Vị Tính",   402,  110);
            (_, txtGia) = FormHelper.MakeField(pnlInput, "Đơn Giá (VNĐ)", 526,  150);

            // Chỉ nhập số
            txtGia.KeyPress += (s, e) => { if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar)) e.Handled = true; };
            txtGia.Leave    += (s, e) => { if (decimal.TryParse(txtGia.Text.Replace(",", ""), out decimal v)) txtGia.Text = v.ToString("N0"); };

            this.Controls.Add(pnlGrid);
            this.Controls.Add(footer);
            this.Controls.Add(pnlInput);
            this.Controls.Add(hdr);

            btnReload.Click += (s, e) => Load_();
            btnThem.Click   += (s, e) => { _adding = true; ClearFields(); FormHelper.SetEditMode(true, pnlInput, btnLuu, btnHuybo, btnThem, btnSua, btnXoa, btnReload); txtMa.ReadOnly = false; txtMa.Focus(); };
            btnSua.Click    += (s, e) => StartEdit();
            btnLuu.Click    += (s, e) => Save();
            btnHuybo.Click  += (s, e) => { ClearFields(); FormHelper.SetEditMode(false, pnlInput, btnLuu, btnHuybo, btnThem, btnSua, btnXoa, btnReload); };
            btnXoa.Click    += (s, e) => Delete();
            dgv.CellClick   += (s, e) => { if (e.RowIndex >= 0) FillRow(e.RowIndex); };
        }

        // ── Load ─────────────────────────────────────────────
        void Load_()
        {
            try
            {
                var list = _bus.GetAll();        // ← BUS
                var dt = new DataTable();
                dt.Columns.Add("Mã SP");
                dt.Columns.Add("Tên Sản Phẩm");
                dt.Columns.Add("ĐVT");
                dt.Columns.Add("Đơn Giá");
                foreach (var sp in list)
                    dt.Rows.Add(sp.MaSP, sp.TenSP, sp.DonViTinh, sp.DonGia.ToString("N0") + " đ");
                dgv.DataSource = dt;
                ClearFields();
            }
            catch (Exception ex) { FormHelper.ShowError(ex.Message); }
        }

        void StartEdit()
        {
            if (dgv.CurrentRow == null) { FormHelper.ShowWarn("Chọn dòng cần sửa."); return; }
            _adding = false;
            FillRow(dgv.CurrentRow.Index);
            // Load đơn giá dạng số từ BUS
            var sp = _bus.GetByMa(txtMa.Text);
            if (sp != null) txtGia.Text = sp.DonGia.ToString("N0");
            txtMa.ReadOnly = true;
            FormHelper.SetEditMode(true, pnlInput, btnLuu, btnHuybo, btnThem, btnSua, btnXoa, btnReload);
            txtTen.Focus();
        }

        // ── Save ─────────────────────────────────────────────
        void Save()
        {
            string giaStr = txtGia.Text.Replace(",", "").Replace(".", "").Trim();
            if (!decimal.TryParse(giaStr, out decimal gia))
            { FormHelper.ShowWarn("Đơn giá phải là số (VD: 350000)."); return; }

            var dto = new SanPhamDTO
            {
                MaSP      = txtMa.Text.Trim(),
                TenSP     = txtTen.Text.Trim(),
                DonViTinh = txtDVT.Text.Trim(),
                DonGia    = gia
            };

            var (ok, msg) = _adding ? _bus.Insert(dto) : _bus.Update(dto);  // ← BUS

            if (ok)
            {
                FormHelper.ShowOK(msg);
                Load_();
                FormHelper.SetEditMode(false, pnlInput, btnLuu, btnHuybo, btnThem, btnSua, btnXoa, btnReload);
            }
            else FormHelper.ShowWarn(msg);
        }

        // ── Delete ───────────────────────────────────────────
        void Delete()
        {
            if (dgv.CurrentRow == null) { FormHelper.ShowWarn("Chọn dòng cần xóa."); return; }
            string ma = dgv.CurrentRow.Cells[0].Value.ToString();
            if (!FormHelper.Confirm($"Xóa Sản Phẩm '{ma}'?")) return;

            var (ok, msg) = _bus.Delete(ma);     // ← BUS
            if (ok) { FormHelper.ShowOK(msg); Load_(); }
            else FormHelper.ShowError(msg);
        }

        void FillRow(int r)
        {
            txtMa.Text  = dgv.Rows[r].Cells[0].Value?.ToString();
            txtTen.Text = dgv.Rows[r].Cells[1].Value?.ToString();
            txtDVT.Text = dgv.Rows[r].Cells[2].Value?.ToString();
        }
        void ClearFields() { txtMa.Clear(); txtTen.Clear(); txtDVT.Clear(); txtGia.Clear(); }
    }
}
