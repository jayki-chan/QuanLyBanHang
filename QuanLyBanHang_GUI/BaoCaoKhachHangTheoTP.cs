using QuanLyBanHang_DAL;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyBanHang_GUI
{
    /// <summary>
    /// Báo cáo: Danh sách Khách Hàng theo từng Thành Phố.
    /// Lọc TP, tìm kiếm nhanh theo tên công ty, hiện số HĐ và tổng doanh thu từng KH.
    /// </summary>
    public partial class BaoCaoKhachHangTheoTP : Form
    {
        ComboBox cboTP;
        TextBox txtSearch;
        DataGridView dgv;
        Label lblTong;
        DataTable _dt;  // lưu dữ liệu gốc để tìm kiếm không load lại DB

        static readonly Color NavBlue   = Color.FromArgb(30, 55, 100);
        static readonly Color BgGray    = Color.FromArgb(245, 246, 250);
        static readonly Color InputBg   = Color.FromArgb(242, 246, 255);
        static readonly Color BorderCol = Color.FromArgb(208, 214, 228);

        public BaoCaoKhachHangTheoTP()
        {
            BuildUI();
            LoadComboTP();
            Load_();
        }

        void BuildUI()
        {
            this.Text = "Khách Hàng theo Thành Phố";
            this.ClientSize = new Size(1020, 560);
            this.MinimumSize = new Size(820, 440);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BgGray;
            this.FormBorderStyle = FormBorderStyle.Sizable;

            // ── Header ──────────────────────────────────────
            var pnlHeader = new Panel { BackColor = NavBlue, Dock = DockStyle.Top, Height = 52 };
            pnlHeader.Controls.Add(new Label
            {
                Dock = DockStyle.Fill, Text = "KHÁCH HÀNG THEO THÀNH PHỐ",
                Font = new Font("Segoe UI Semibold", 13.5F, FontStyle.Bold),
                ForeColor = Color.White, TextAlign = ContentAlignment.MiddleCenter
            });

            // ── Filter Panel ────────────────────────────────
            var pnlFilter = new Panel
            {
                BackColor = InputBg, Dock = DockStyle.Top, Height = 56,
                Padding = new Padding(14, 10, 14, 8)
            };
            pnlFilter.Paint += (s, e) =>
                e.Graphics.DrawLine(new Pen(BorderCol), 0, pnlFilter.Height - 1, pnlFilter.Width, pnlFilter.Height - 1);

            pnlFilter.Controls.Add(new Label
            {
                Text = "Thành Phố:", Location = new Point(14, 14),
                AutoSize = true, Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(50, 70, 110)
            });
            cboTP = new ComboBox
            {
                Location = new Point(100, 11), Size = new Size(200, 26),
                Font = new Font("Segoe UI", 9.5F), DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboTP.SelectedIndexChanged += (s, e) => Load_();
            pnlFilter.Controls.Add(cboTP);

            Lbl(pnlFilter, "Tìm:", 314, 14);
            txtSearch = new TextBox
            {
                Location = new Point(344, 11), Size = new Size(220, 26),
                Font = new Font("Segoe UI", 9.5F)
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;
            pnlFilter.Controls.Add(txtSearch);

            var btnReload = MakeBtn("Tải lại", Color.FromArgb(85, 110, 155));
            btnReload.Location = new Point(578, 10);
            btnReload.Click += (s, e) => { LoadComboTP(); Load_(); };
            pnlFilter.Controls.Add(btnReload);

            // ── Grid ────────────────────────────────────────
            dgv = BuildGrid();
            var pnlGrid = new Panel { Dock = DockStyle.Fill, Padding = new Padding(14, 10, 14, 0), BackColor = BgGray };
            pnlGrid.Controls.Add(dgv);

            // ── Footer ──────────────────────────────────────
            var pnlFooter = new Panel { BackColor = Color.FromArgb(232, 236, 244), Dock = DockStyle.Bottom, Height = 42 };
            pnlFooter.Paint += (s, e) =>
                e.Graphics.DrawLine(new Pen(BorderCol), 0, 0, pnlFooter.Width, 0);

            lblTong = new Label
            {
                Dock = DockStyle.Left, Width = 500,
                Font = new Font("Segoe UI", 9F), ForeColor = Color.FromArgb(40, 60, 100),
                TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(14, 0, 0, 0)
            };
            var btnTroVe = MakeBtn("Trở Về", Color.White);
            btnTroVe.ForeColor = Color.FromArgb(48, 62, 90);
            btnTroVe.FlatAppearance.BorderColor = Color.FromArgb(175, 188, 212);
            btnTroVe.FlatAppearance.BorderSize = 1;
            btnTroVe.Dock = DockStyle.Right;
            btnTroVe.Width = 96;
            btnTroVe.Click += (s, e) => this.Close();

            pnlFooter.Controls.Add(lblTong);
            pnlFooter.Controls.Add(btnTroVe);

            this.Controls.Add(pnlGrid);
            this.Controls.Add(pnlFooter);
            this.Controls.Add(pnlFilter);
            this.Controls.Add(pnlHeader);
        }

        void LoadComboTP()
        {
            using (var c = DBConnection.GetConnection())
            {
                var dt = new DataTable();
                new SqlDataAdapter("SELECT '' AS ThanhPho, N'-- Tất cả --' AS TenThanhPho " +
                    "UNION SELECT ThanhPho, TenThanhPho FROM THANHPHO ORDER BY TenThanhPho", c).Fill(dt);
                cboTP.DataSource = dt;
                cboTP.DisplayMember = "TenThanhPho";
                cboTP.ValueMember = "ThanhPho";
            }
        }

        void Load_()
        {
            try
            {
                string tp = cboTP.SelectedValue?.ToString() ?? "";
                string where = string.IsNullOrEmpty(tp) ? "" : "WHERE kh.ThanhPho = @tp";

                using (var c = DBConnection.GetConnection())
                {
                    var sql = $@"
                        SELECT tp.TenThanhPho AS [Thành Phố],
                               kh.MaKH        AS [Mã KH],
                               kh.TenCty      AS [Tên Công Ty],
                               kh.DiaChi      AS [Địa Chỉ],
                               kh.DienThoai   AS [Điện Thoại],
                               (SELECT COUNT(*) FROM HOADON WHERE MaKH = kh.MaKH) AS [Số HĐ],
                               FORMAT(ISNULL(
                                   (SELECT SUM(ct.SoLuong * sp.DonGia)
                                    FROM HOADON hd2
                                    JOIN CHITIETHOADON ct ON hd2.MaHD = ct.MaHD
                                    JOIN SANPHAM sp ON ct.MaSP = sp.MaSP
                                    WHERE hd2.MaKH = kh.MaKH), 0), N'N0') + N' đ' AS [Tổng Doanh Thu]
                        FROM KHACHHANG kh
                        LEFT JOIN THANHPHO tp ON kh.ThanhPho = tp.ThanhPho
                        {where}
                        ORDER BY tp.TenThanhPho, kh.MaKH";

                    var cmd = new SqlCommand(sql, c);
                    if (!string.IsNullOrEmpty(tp))
                        cmd.Parameters.AddWithValue("@tp", tp);

                    _dt = new DataTable();
                    new SqlDataAdapter(cmd).Fill(_dt);
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
                dv.RowFilter = $"[Tên Công Ty] LIKE '%{keyword.Replace("'", "''")}%'";
                source = dv.ToTable();
            }

            dgv.DataSource = source;
            lblTong.Text = $"  Tổng số: {source.Rows.Count} khách hàng";
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
