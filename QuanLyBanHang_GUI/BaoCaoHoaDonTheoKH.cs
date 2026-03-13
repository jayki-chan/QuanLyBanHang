using QuanLyBanHang_DAL;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyBanHang_GUI
{
    /// <summary>
    /// Báo cáo: Danh sách Hóa Đơn theo từng Khách Hàng.
    /// Có ComboBox lọc KH, lọc ngày, tìm kiếm nhanh, hiện tổng số HĐ và tổng tiền.
    /// </summary>
    public partial class BaoCaoHoaDonTheoKH : Form
    {
        ComboBox cboKH;
        DateTimePicker dtpTu, dtpDen;
        CheckBox chkLocNgay;
        TextBox txtSearch;
        DataGridView dgv;
        Label lblTong;
        DataTable _dt;  // lưu dữ liệu gốc để tìm kiếm không load lại DB

        static readonly Color NavBlue   = Color.FromArgb(30, 55, 100);
        static readonly Color BgGray    = Color.FromArgb(245, 246, 250);
        static readonly Color InputBg   = Color.FromArgb(242, 246, 255);
        static readonly Color BorderCol = Color.FromArgb(208, 214, 228);

        public BaoCaoHoaDonTheoKH()
        {
            BuildUI();
            LoadComboKH();
            Load_();
        }

        void BuildUI()
        {
            this.Text = "Hóa Đơn theo Khách Hàng";
            this.ClientSize = new Size(980, 560);
            this.MinimumSize = new Size(780, 440);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BgGray;
            this.FormBorderStyle = FormBorderStyle.Sizable;

            var pnlHeader = new Panel { BackColor = NavBlue, Dock = DockStyle.Top, Height = 52 };
            pnlHeader.Controls.Add(new Label
            {
                Dock = DockStyle.Fill, Text = "HÓA ĐƠN THEO KHÁCH HÀNG",
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

            // Row 1 — Khách hàng + Tìm kiếm
            Lbl(pnlFilter, "Khách Hàng:", 14, 14);
            cboKH = new ComboBox
            {
                Location = new Point(105, 11), Size = new Size(240, 26),
                Font = new Font("Segoe UI", 9.5F), DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboKH.SelectedIndexChanged += (s, e) => Load_();
            pnlFilter.Controls.Add(cboKH);

            Lbl(pnlFilter, "Tìm:", 358, 14);
            txtSearch = new TextBox
            {
                Location = new Point(388, 11), Size = new Size(200, 26),
                Font = new Font("Segoe UI", 9.5F)
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;
            pnlFilter.Controls.Add(txtSearch);

            var btnReload = MakeBtn("Tải lại", Color.FromArgb(85, 110, 155));
            btnReload.Location = new Point(602, 10);
            btnReload.Click += (s, e) => { LoadComboKH(); Load_(); };
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

        void LoadComboKH()
        {
            using (var c = DBConnection.GetConnection())
            {
                var dt = new DataTable();
                new SqlDataAdapter(
                    "SELECT '' AS MaKH, N'-- Tất cả --' AS HT " +
                    "UNION SELECT MaKH, MaKH+' — '+TenCty AS HT FROM KHACHHANG ORDER BY HT", c).Fill(dt);
                cboKH.DataSource = dt;
                cboKH.DisplayMember = "HT";
                cboKH.ValueMember = "MaKH";
            }
        }

        void Load_()
        {
            try
            {
                string ma = cboKH.SelectedValue?.ToString() ?? "";
                var wheres = new System.Collections.Generic.List<string>();
                if (!string.IsNullOrEmpty(ma))   wheres.Add("hd.MaKH = @ma");
                if (chkLocNgay.Checked)
                {
                    wheres.Add("hd.NgayLapHD >= @tu");
                    wheres.Add("hd.NgayLapHD <= @den");
                }
                string where = wheres.Count > 0 ? "WHERE " + string.Join(" AND ", wheres) : "";

                using (var c = DBConnection.GetConnection())
                {
                    var sql = $@"
                        SELECT kh.TenCty                               AS [Khách Hàng],
                               hd.MaHD                                 AS [Mã HĐ],
                               nv.Ho + ' ' + nv.Ten                   AS [Nhân Viên],
                               CONVERT(VARCHAR, hd.NgayLapHD,   103)  AS [Ngày Lập],
                               CONVERT(VARCHAR, hd.NgayNhanHang,103)  AS [Ngày Nhận],
                               ISNULL(
                                   (SELECT SUM(ct.SoLuong * sp.DonGia)
                                    FROM CHITIETHOADON ct
                                    JOIN SANPHAM sp ON ct.MaSP = sp.MaSP
                                    WHERE ct.MaHD = hd.MaHD), 0) AS [_TongTien],
                               FORMAT(ISNULL(
                                   (SELECT SUM(ct.SoLuong * sp.DonGia)
                                    FROM CHITIETHOADON ct
                                    JOIN SANPHAM sp ON ct.MaSP = sp.MaSP
                                    WHERE ct.MaHD = hd.MaHD), 0), N'N0') + N' đ'  AS [Tổng Tiền]
                        FROM HOADON hd
                        LEFT JOIN KHACHHANG kh ON hd.MaKH = kh.MaKH
                        LEFT JOIN NHANVIEN  nv ON hd.MaNV = nv.MaNV
                        {where}
                        ORDER BY kh.TenCty, hd.MaHD";

                    var cmd = new SqlCommand(sql, c);
                    if (!string.IsNullOrEmpty(ma))  cmd.Parameters.AddWithValue("@ma",  ma);
                    if (chkLocNgay.Checked)
                    {
                        cmd.Parameters.AddWithValue("@tu",  dtpTu.Value.Date);
                        cmd.Parameters.AddWithValue("@den", dtpDen.Value.Date);
                    }

                    _dt = new DataTable();
                    new SqlDataAdapter(cmd).Fill(_dt);
                    AddStt(_dt);
                }

                // Áp filter tìm kiếm nếu có
                ApplySearch();
            }
            catch (Exception ex) { FormHelper.ShowError(ex.Message); }
        }

        void TxtSearch_TextChanged(object s, EventArgs e) => ApplySearch();

        void ApplySearch()
        {
            if (_dt == null) return;

            DataTable source;
            string keyword = txtSearch?.Text?.Trim() ?? "";
            if (string.IsNullOrEmpty(keyword))
            {
                source = _dt;
            }
            else
            {
                var dv = _dt.DefaultView;
                dv.RowFilter = $"[Khách Hàng] LIKE '%{keyword.Replace("'", "''")}%'";
                source = dv.ToTable();
            }

            dgv.DataSource = source;

            // Ẩn cột số nội bộ
            if (dgv.Columns["_TongTien"] != null)
                dgv.Columns["_TongTien"].Visible = false;

            // Tính tổng tiền từ source hiện tại
            decimal tongTien = 0;
            foreach (DataRow r in source.Rows)
                if (decimal.TryParse(r["_TongTien"]?.ToString(), out decimal v)) tongTien += v;

            lblTong.Text = $"  Tổng số: {source.Rows.Count} hóa đơn  |  Tổng tiền: {tongTien:N0} đ";
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

        static void AddStt(DataTable dt)
        {
            var col = new DataColumn("STT", typeof(int));
            dt.Columns.Add(col);
            dt.Columns["STT"].SetOrdinal(0);
            for (int i = 0; i < dt.Rows.Count; i++)
                dt.Rows[i]["STT"] = i + 1;
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
                    b.Image        = AppIcons.Get(kv.Value, 16, b.ForeColor == Color.White ? Color.White : Color.FromArgb(48, 62, 90));
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
