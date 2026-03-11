using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBanHang_BUS;
using QuanLyBanHang_DTO;

namespace QuanLyBanHang_GUI
{
    public partial class QuanLyKhachHang : Form
    {
        // ── BUS ───────────────────────────────────────────────
        private readonly KhachHangBUS _bus    = new KhachHangBUS();
        private readonly ThanhPhoBUS  _busTP  = new ThanhPhoBUS();

        // ── Controls ─────────────────────────────────────────
        Panel pnlInput;
        DataGridView dgv;
        Button btnReload, btnThem, btnSua, btnLuu, btnHuybo, btnXoa;
        TextBox txtMa, txtTen, txtDiaChi, txtDT;
        ComboBox cboTP;
        bool _adding;

        public QuanLyKhachHang()
        {
            BuildUI();
            LoadComboTP();
            FormHelper.SetEditMode(false, pnlInput, btnLuu, btnHuybo, btnThem, btnSua, btnXoa, btnReload);
            Load_();
        }

        void BuildUI()
        {
            this.Text = "Quản Lý Danh Mục Khách Hàng";
            this.ClientSize = new Size(960, 540);
            this.MinimumSize = new Size(800, 420);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = FormHelper.BgGray;
            this.FormBorderStyle = FormBorderStyle.Sizable;

            var (hdr, _) = FormHelper.BuildHeader("QUẢN LÝ DANH MỤC KHÁCH HÀNG");
            pnlInput = FormHelper.BuildInputPanel(72);
            var (pnlGrid, grid) = FormHelper.BuildGridPanel();
            dgv = grid;
            var footer = FormHelper.BuildFooter(out btnReload, out btnThem, out btnSua,
                out btnLuu, out btnHuybo, out btnXoa, (s, e) => this.Close());

            (_, txtMa)     = FormHelper.MakeField(pnlInput, "Mã KH",       14,  90);
            (_, txtTen)    = FormHelper.MakeField(pnlInput, "Tên Công Ty", 118, 230);
            (_, txtDiaChi) = FormHelper.MakeField(pnlInput, "Địa Chỉ",    362, 190);
            (_, cboTP)     = FormHelper.MakeCombo(pnlInput, "Thành Phố",   566, 160);
            (_, txtDT)     = FormHelper.MakeField(pnlInput, "Điện Thoại",  740, 130);

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

        // ── Load ComboBox Thành Phố từ BUS ───────────────────
        void LoadComboTP()
        {
            var list = _busTP.GetAll();          // ← BUS
            var dt = new DataTable();
            dt.Columns.Add("ThanhPho");
            dt.Columns.Add("TenThanhPho");
            foreach (var tp in list)
                dt.Rows.Add(tp.ThanhPho, tp.TenThanhPho);
            cboTP.DataSource    = dt;
            cboTP.DisplayMember = "TenThanhPho";
            cboTP.ValueMember   = "ThanhPho";
        }

        // ── Load danh sách ───────────────────────────────────
        void Load_()
        {
            try
            {
                var list = _bus.GetAll();        // ← BUS
                var dt = new DataTable();
                dt.Columns.Add("Mã KH");
                dt.Columns.Add("Tên Công Ty");
                dt.Columns.Add("Địa Chỉ");
                dt.Columns.Add("Thành Phố");
                dt.Columns.Add("Điện Thoại");
                dt.Columns.Add("_MaTP");         // cột ẩn để FillRow

                foreach (var kh in list)
                    dt.Rows.Add(kh.MaKH, kh.TenCty, kh.DiaChi, kh.TenThanhPho, kh.DienThoai, kh.ThanhPho);

                dgv.DataSource = dt;
                if (dgv.Columns.Contains("_MaTP")) dgv.Columns["_MaTP"].Visible = false;
                ClearFields();
            }
            catch (Exception ex) { FormHelper.ShowError(ex.Message); }
        }

        void StartEdit()
        {
            if (dgv.CurrentRow == null) { FormHelper.ShowWarn("Chọn dòng cần sửa."); return; }
            _adding = false;
            FillRow(dgv.CurrentRow.Index);
            txtMa.ReadOnly = true;
            FormHelper.SetEditMode(true, pnlInput, btnLuu, btnHuybo, btnThem, btnSua, btnXoa, btnReload);
            txtTen.Focus();
        }

        // ── Save ─────────────────────────────────────────────
        void Save()
        {
            var dto = new KhachHangDTO
            {
                MaKH      = txtMa.Text.Trim(),
                TenCty    = txtTen.Text.Trim(),
                DiaChi    = txtDiaChi.Text.Trim(),
                ThanhPho  = cboTP.SelectedValue?.ToString() ?? "",
                DienThoai = txtDT.Text.Trim()
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
            if (!FormHelper.Confirm($"Xóa Khách Hàng '{ma}'?")) return;

            var (ok, msg) = _bus.Delete(ma);     // ← BUS
            if (ok) { FormHelper.ShowOK(msg); Load_(); }
            else FormHelper.ShowError(msg);
        }

        void FillRow(int r)
        {
            var row = dgv.Rows[r];
            txtMa.Text     = row.Cells[0].Value?.ToString();
            txtTen.Text    = row.Cells[1].Value?.ToString();
            txtDiaChi.Text = row.Cells[2].Value?.ToString();
            txtDT.Text     = row.Cells[4].Value?.ToString();
            // Chọn đúng thành phố từ cột ẩn _MaTP
            var maTP = row.Cells["_MaTP"].Value?.ToString();
            if (maTP != null) cboTP.SelectedValue = maTP;
        }

        void ClearFields()
        {
            txtMa.Clear(); txtTen.Clear(); txtDiaChi.Clear(); txtDT.Clear();
            if (cboTP.Items.Count > 0) cboTP.SelectedIndex = 0;
        }
    }
}
