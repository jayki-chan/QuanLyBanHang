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
        Button btnReload, btnThem, btnSua, btnLuu, btnHuybo, btnXoa;
        TextBox txtMaHD;
        ComboBox cboKH, cboNV;
        DateTimePicker dtpLap, dtpNhan;
        bool _adding;

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

            (_, txtMaHD) = FormHelper.MakeField(pnlInput,     "Mã Hóa Đơn",       14,  110);
            (_, cboKH)   = FormHelper.MakeCombo(pnlInput,     "Khách Hàng",       138,  240);
            (_, cboNV)   = FormHelper.MakeCombo(pnlInput,     "Nhân Viên",        392,  220);
            (_, dtpLap)  = FormHelper.MakeDatePicker(pnlInput,"Ngày Lập Hóa Đơn", 14,  160, 80);
            (_, dtpNhan) = FormHelper.MakeDatePicker(pnlInput,"Ngày Nhận Hàng",   188,  160, 80);

            this.Controls.Add(pnlGrid);
            this.Controls.Add(footer);
            this.Controls.Add(pnlInput);
            this.Controls.Add(hdr);

            btnReload.Click += (s, e) => Load_();
            btnThem.Click   += (s, e) => { _adding = true; ClearFields(); FormHelper.SetEditMode(true, pnlInput, btnLuu, btnHuybo, btnThem, btnSua, btnXoa, btnReload); txtMaHD.ReadOnly = false; txtMaHD.Focus(); };
            btnSua.Click    += (s, e) => StartEdit();
            btnLuu.Click    += (s, e) => Save();
            btnHuybo.Click  += (s, e) => { ClearFields(); FormHelper.SetEditMode(false, pnlInput, btnLuu, btnHuybo, btnThem, btnSua, btnXoa, btnReload); };
            btnXoa.Click    += (s, e) => Delete();
            dgv.CellClick   += (s, e) => { if (e.RowIndex >= 0) txtMaHD.Text = dgv.Rows[e.RowIndex].Cells[0].Value?.ToString(); };
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
        }

        // ── Load danh sách ───────────────────────────────────
        void Load_()
        {
            try
            {
                var list = _bus.GetAll();            // ← BUS
                var dt = new DataTable();
                dt.Columns.Add("Mã HĐ"); dt.Columns.Add("Khách Hàng");
                dt.Columns.Add("Nhân Viên"); dt.Columns.Add("Ngày Lập"); dt.Columns.Add("Ngày Nhận");
                foreach (var hd in list)
                    dt.Rows.Add(hd.MaHD, hd.TenCty, hd.HoTenNV,
                        hd.NgayLapHD.ToString("dd/MM/yyyy"),
                        hd.NgayNhanHang.ToString("dd/MM/yyyy"));
                dgv.DataSource = dt;
                ClearFields();
            }
            catch (Exception ex) { FormHelper.ShowError(ex.Message); }
        }

        void StartEdit()
        {
            if (dgv.CurrentRow == null) { FormHelper.ShowWarn("Chọn dòng cần sửa."); return; }
            _adding = false;
            string ma = dgv.CurrentRow.Cells[0].Value.ToString();
            txtMaHD.Text = ma;

            var hd = _bus.GetByMa(ma);               // ← BUS lấy đối tượng đầy đủ
            if (hd != null)
            {
                try { cboKH.SelectedValue = hd.MaKH; } catch { }
                try { cboNV.SelectedValue = hd.MaNV; } catch { }
                dtpLap.Value  = hd.NgayLapHD;
                dtpNhan.Value = hd.NgayNhanHang;
            }
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
            if (!FormHelper.Confirm($"Xóa Hóa Đơn '{ma}'? (Sẽ xóa cả chi tiết hóa đơn)")) return;

            var (ok, msg) = _bus.Delete(ma);          // ← BUS (tự xóa chi tiết trước)
            if (ok) { FormHelper.ShowOK(msg); Load_(); }
            else FormHelper.ShowError(msg);
        }

        void ClearFields()
        {
            txtMaHD.Clear();
            dtpLap.Value = dtpNhan.Value = DateTime.Today;
            if (cboKH.Items.Count > 0) cboKH.SelectedIndex = 0;
            if (cboNV.Items.Count > 0) cboNV.SelectedIndex = 0;
        }
    }
}
