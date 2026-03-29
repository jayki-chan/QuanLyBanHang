using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBanHang_BUS;
using QuanLyBanHang_DTO;

namespace QuanLyBanHang_GUI
{
    public partial class QuanLyThanhPho : Form
    {
        // ── BUS ───────────────────────────────────────────────
        private readonly ThanhPhoBUS _bus = new ThanhPhoBUS();

        // ── Controls ─────────────────────────────────────────
        Panel pnlInput;
        DataGridView dgv;
        Button btnReload, btnThem, btnSua, btnLuu, btnHuybo, btnXoa;
        TextBox txtMa, txtTen, txtTimKiem;
        bool _adding;
        DataTable _dtData;

        public QuanLyThanhPho()
        {
            BuildUI();
            FormHelper.SetEditMode(false, pnlInput, btnLuu, btnHuybo, btnThem, btnSua, btnXoa, btnReload);
            Load_();
        }

        void BuildUI()
        {
            this.Text = "Quản Lý Danh Mục Thành Phố";
            this.ClientSize = new Size(860, 520);
            this.MinimumSize = new Size(700, 420);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = FormHelper.BgGray;
            this.FormBorderStyle = FormBorderStyle.Sizable;

            var (hdr, _) = FormHelper.BuildHeader("QUẢN LÝ DANH MỤC THÀNH PHỐ");
            pnlInput = FormHelper.BuildInputPanel(68);
            var (pnlGrid, grid) = FormHelper.BuildGridPanel();
            dgv = grid;
            var footer = FormHelper.BuildFooter(out btnReload, out btnThem, out btnSua,
                out btnLuu, out btnHuybo, out btnXoa, (s, e) => this.Close());

            (_, txtMa)  = FormHelper.MakeField(pnlInput, "Mã Thành Phố",  14,  120);
            (_, txtTen) = FormHelper.MakeField(pnlInput, "Tên Thành Phố", 148, 300);

            Panel pnlSearch = new Panel { Dock = DockStyle.Top, Height = 46, BackColor = FormHelper.BgGray };
            Label lblTimKiem = new Label { Text = "🔍 Tìm kiếm:", Location = new Point(14, 14), AutoSize = true, Font = new Font("Segoe UI", 8.5F), ForeColor = Color.FromArgb(68, 82, 110) };
            txtTimKiem = new TextBox { Location = new Point(90, 10), Size = new Size(300, 26), Font = new Font("Segoe UI", 9.5F) };

            txtTimKiem.TextChanged += (s, e) => {
                if (_dtData == null) return;
                string kw = txtTimKiem.Text.Trim().Replace("'", "''");
                _dtData.DefaultView.RowFilter = $"[Mã TP] LIKE '%{kw}%' OR [Tên Thành Phố] LIKE '%{kw}%'";
            };
            pnlSearch.Controls.Add(lblTimKiem);
            pnlSearch.Controls.Add(txtTimKiem);

            this.Controls.Add(pnlGrid);
            this.Controls.Add(pnlSearch);
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
                var list = _bus.GetAll();          // ← gọi BUS, không gọi SQL trực tiếp
                _dtData = new DataTable();
                _dtData.Columns.Add("Mã TP");
                _dtData.Columns.Add("Tên Thành Phố");
                foreach (var tp in list)
                    _dtData.Rows.Add(tp.ThanhPho, tp.TenThanhPho);
                dgv.DataSource = _dtData;
                ClearFields();
            }
            catch (Exception ex) { FormHelper.ShowError(ex.Message); }
        }

        // ── StartEdit ────────────────────────────────────────
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
            var dto = new ThanhPhoDTO
            {
                ThanhPho    = txtMa.Text.Trim(),
                TenThanhPho = txtTen.Text.Trim()
            };

            var (ok, msg) = _adding ? _bus.Insert(dto) : _bus.Update(dto);  // ← BUS validate + lưu

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
            if (!FormHelper.Confirm($"Xóa Thành Phố '{ma}'?")) return;

            var (ok, msg) = _bus.Delete(ma);   // ← BUS xử lý
            if (ok) { FormHelper.ShowOK(msg); Load_(); }
            else FormHelper.ShowError(msg);
        }

        void FillRow(int r)
        {
            txtMa.Text  = dgv.Rows[r].Cells[0].Value?.ToString();
            txtTen.Text = dgv.Rows[r].Cells[1].Value?.ToString();
        }
        void ClearFields() { txtMa.Clear(); txtTen.Clear(); }
    }
}
