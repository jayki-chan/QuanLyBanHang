using QuanLyBanHang_DAL;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyBanHang_GUI
{
    /// <summary>
    /// Báo cáo: Danh sách Hóa Đơn do từng Nhân Viên lập.
    /// Có thể lọc theo nhân viên, lọc thêm theo khoảng ngày lập.
    /// </summary>
    public partial class BaoCaoHoaDonTheoNV : Form
    {
        ComboBox cboNV;
        DateTimePicker dtpTu, dtpDen;
        CheckBox chkLocNgay;
        DataGridView dgv;
        Label lblTong;

        static readonly Color NavBlue   = Color.FromArgb(30, 55, 100);
        static readonly Color BgGray    = Color.FromArgb(245, 246, 250);
        static readonly Color InputBg   = Color.FromArgb(242, 246, 255);
        static readonly Color BorderCol = Color.FromArgb(208, 214, 228);

        public BaoCaoHoaDonTheoNV()
        {
            BuildUI();
            LoadComboNV();
            Load_();
        }

        void BuildUI()
        {
            this.Text = "Hóa Đơn theo Nhân Viên";
            this.ClientSize = new Size(1020, 580);
            this.MinimumSize = new Size(800, 460);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BgGray;
            this.FormBorderStyle = FormBorderStyle.Sizable;

            var pnlHeader = new Panel { BackColor = NavBlue, Dock = DockStyle.Top, Height = 52 };
            pnlHeader.Controls.Add(new Label
            {
                Dock = DockStyle.Fill, Text = "HÓA ĐƠN THEO NHÂN VIÊN",
                Font = new Font("Segoe UI Semibold", 13.5F, FontStyle.Bold),
                ForeColor = Color.White, TextAlign = ContentAlignment.MiddleCenter
            });

            // Filter: 2 hàng
            var pnlFilter = new Panel
            {
                BackColor = InputBg, Dock = DockStyle.Top, Height = 92,
                Padding = new Padding(14, 8, 14, 8)
            };
            pnlFilter.Paint += (s, e) =>
                e.Graphics.DrawLine(new Pen(BorderCol), 0, pnlFilter.Height - 1, pnlFilter.Width, pnlFilter.Height - 1);

            // Row 1 — Nhân viên
            Lbl(pnlFilter, "Nhân Viên:", 14, 14);
            cboNV = new ComboBox
            {
                Location = new Point(100, 11), Size = new Size(280, 26),
                Font = new Font("Segoe UI", 9.5F), DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboNV.SelectedIndexChanged += (s, e) => Load_();
            pnlFilter.Controls.Add(cboNV);

            var btnReload = MakeBtn("Tải lại", Color.FromArgb(85, 110, 155));
            btnReload.Location = new Point(394, 10);
            btnReload.Click += (s, e) => { LoadComboNV(); Load_(); };
            pnlFilter.Controls.Add(btnReload);

            // Row 2 — Lọc ngày
            chkLocNgay = new CheckBox
            {
                Text = "Lọc theo ngày lập:", Location = new Point(14, 50),
                AutoSize = true, Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(50, 70, 110)
            };
            chkLocNgay.CheckedChanged += (s, e) =>
            {
                dtpTu.Enabled = dtpDen.Enabled = chkLocNgay.Checked;
                Load_();
            };
            pnlFilter.Controls.Add(chkLocNgay);

            Lbl(pnlFilter, "Từ:", 195, 52);
            dtpTu = new DateTimePicker
            {
                Location = new Point(215, 48), Size = new Size(130, 24),
                Font = new Font("Segoe UI", 9.5F), Format = DateTimePickerFormat.Short,
                Value = DateTime.Today.AddMonths(-1), Enabled = false
            };
            dtpTu.ValueChanged += (s, e) => { if (chkLocNgay.Checked) Load_(); };
            pnlFilter.Controls.Add(dtpTu);

            Lbl(pnlFilter, "Đến:", 358, 52);
            dtpDen = new DateTimePicker
            {
                Location = new Point(383, 48), Size = new Size(130, 24),
                Font = new Font("Segoe UI", 9.5F), Format = DateTimePickerFormat.Short,
                Value = DateTime.Today, Enabled = false
            };
            dtpDen.ValueChanged += (s, e) => { if (chkLocNgay.Checked) Load_(); };
            pnlFilter.Controls.Add(dtpDen);

            dgv = BuildGrid();
            var pnlGrid = new Panel { Dock = DockStyle.Fill, Padding = new Padding(14, 10, 14, 0), BackColor = BgGray };
            pnlGrid.Controls.Add(dgv);

            var pnlFooter = new Panel { BackColor = Color.FromArgb(232, 236, 244), Dock = DockStyle.Bottom, Height = 42 };
            pnlFooter.Paint += (s, e) =>
                e.Graphics.DrawLine(new Pen(BorderCol), 0, 0, pnlFooter.Width, 0);

            lblTong = new Label
            {
                Dock = DockStyle.Left, Width = 600,
                Font = new Font("Segoe UI", 9F), ForeColor = Color.FromArgb(40, 60, 100),
                TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(14, 0, 0, 0)
            };
            var btnTroVe = MakeBtn("Trở Về", Color.White);
            btnTroVe.ForeColor = Color.FromArgb(48, 62, 90);
            btnTroVe.FlatAppearance.BorderColor = Color.FromArgb(175, 188, 212);
            btnTroVe.FlatAppearance.BorderSize = 1;
            btnTroVe.Dock = DockStyle.Right; btnTroVe.Width = 96;
            btnTroVe.Click += (s, e) => this.Close();

            pnlFooter.Controls.Add(lblTong);
            pnlFooter.Controls.Add(btnTroVe);

            this.Controls.Add(pnlGrid);
            this.Controls.Add(pnlFooter);
            this.Controls.Add(pnlFilter);
            this.Controls.Add(pnlHeader);
        }

        void LoadComboNV()
        {
            using (var c = DBConnection.GetConnection())
            {
                var dt = new DataTable();
                new SqlDataAdapter(
                    "SELECT '' AS MaNV, N'-- Tất cả --' AS HT " +
                    "UNION SELECT MaNV, MaNV+' — '+Ho+' '+Ten AS HT FROM NHANVIEN ORDER BY HT", c).Fill(dt);
                cboNV.DataSource = dt;
                cboNV.DisplayMember = "HT";
                cboNV.ValueMember = "MaNV";
            }
        }

        void Load_()
        {
            try
            {
                string ma = cboNV.SelectedValue?.ToString() ?? "";
                var wheres = new System.Collections.Generic.List<string>();
                if (!string.IsNullOrEmpty(ma))        wheres.Add("hd.MaNV = @ma");
                if (chkLocNgay.Checked)
                {
                    wheres.Add("hd.NgayLapHD >= @tu");
                    wheres.Add("hd.NgayLapHD <= @den");
                }
                string where = wheres.Count > 0 ? "WHERE " + string.Join(" AND ", wheres) : "";

                using (var c = DBConnection.GetConnection())
                {
                    var sql = $@"
                        SELECT nv.Ho + ' ' + nv.Ten                   AS [Nhân Viên],
                               hd.MaHD                                 AS [Mã HĐ],
                               kh.TenCty                               AS [Khách Hàng],
                               CONVERT(VARCHAR, hd.NgayLapHD,   103)  AS [Ngày Lập],
                               CONVERT(VARCHAR, hd.NgayNhanHang,103)  AS [Ngày Nhận],
                               FORMAT(ISNULL(
                                   (SELECT SUM(ct.SoLuong * sp.DonGia)
                                    FROM CHITIETHOADON ct
                                    JOIN SANPHAM sp ON ct.MaSP = sp.MaSP
                                    WHERE ct.MaHD = hd.MaHD), 0), N'N0') + N' đ'  AS [Tổng Tiền]
                        FROM HOADON hd
                        LEFT JOIN NHANVIEN  nv ON hd.MaNV = nv.MaNV
                        LEFT JOIN KHACHHANG kh ON hd.MaKH = kh.MaKH
                        {where}
                        ORDER BY nv.Ho, nv.Ten, hd.MaHD";

                    var cmd = new SqlCommand(sql, c);
                    if (!string.IsNullOrEmpty(ma))   cmd.Parameters.AddWithValue("@ma",  ma);
                    if (chkLocNgay.Checked)
                    {
                        cmd.Parameters.AddWithValue("@tu",  dtpTu.Value.Date);
                        cmd.Parameters.AddWithValue("@den", dtpDen.Value.Date);
                    }

                    var dt = new DataTable();
                    new SqlDataAdapter(cmd).Fill(dt);
                    dgv.DataSource = dt;
                    lblTong.Text = $"  Tổng số: {dt.Rows.Count} hóa đơn";
                }
            }
            catch (Exception ex) { FormHelper.ShowError(ex.Message); }
        }

        void Lbl(Panel p, string text, int x, int y) =>
            p.Controls.Add(new Label
            {
                Text = text, Location = new Point(x, y), AutoSize = true,
                Font = new Font("Segoe UI", 9F), ForeColor = Color.FromArgb(50, 70, 110)
            });

        DataGridView BuildGrid()
        {
            var g = new DataGridView();
            FormHelper.StyleGrid(g);
            g.Dock = DockStyle.Fill;
            return g;
        }

        Button MakeBtn(string text, Color bg)
        {
            var b = new Button
            {
                Text = text, Size = new Size(96, 34),
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
    }
}
