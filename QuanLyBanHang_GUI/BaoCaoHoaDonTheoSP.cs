using QuanLyBanHang_DAL;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyBanHang_GUI
{
    /// <summary>
    /// Báo cáo: Danh sách Hóa Đơn có chứa từng Sản Phẩm.
    /// Hiển thị số lượng và thành tiền theo từng sản phẩm.
    /// </summary>
    public partial class BaoCaoHoaDonTheoSP : Form
    {
        ComboBox cboSP;
        DataGridView dgv;
        Label lblTong;

        static readonly Color NavBlue   = Color.FromArgb(30, 55, 100);
        static readonly Color BgGray    = Color.FromArgb(245, 246, 250);
        static readonly Color InputBg   = Color.FromArgb(242, 246, 255);
        static readonly Color BorderCol = Color.FromArgb(208, 214, 228);

        public BaoCaoHoaDonTheoSP()
        {
            BuildUI();
            LoadComboSP();
            Load_();
        }

        void BuildUI()
        {
            this.Text = "Hóa Đơn theo Sản Phẩm";
            this.ClientSize = new Size(1000, 560);
            this.MinimumSize = new Size(800, 440);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BgGray;
            this.FormBorderStyle = FormBorderStyle.Sizable;

            var pnlHeader = new Panel { BackColor = NavBlue, Dock = DockStyle.Top, Height = 52 };
            pnlHeader.Controls.Add(new Label
            {
                Dock = DockStyle.Fill, Text = "HÓA ĐƠN THEO SẢN PHẨM",
                Font = new Font("Segoe UI Semibold", 13.5F, FontStyle.Bold),
                ForeColor = Color.White, TextAlign = ContentAlignment.MiddleCenter
            });

            var pnlFilter = new Panel
            {
                BackColor = InputBg, Dock = DockStyle.Top, Height = 56,
                Padding = new Padding(14, 10, 14, 8)
            };
            pnlFilter.Paint += (s, e) =>
                e.Graphics.DrawLine(new Pen(BorderCol), 0, pnlFilter.Height - 1, pnlFilter.Width, pnlFilter.Height - 1);

            pnlFilter.Controls.Add(new Label
            {
                Text = "Sản Phẩm:", Location = new Point(14, 14),
                AutoSize = true, Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(50, 70, 110)
            });
            cboSP = new ComboBox
            {
                Location = new Point(100, 11), Size = new Size(320, 26),
                Font = new Font("Segoe UI", 9.5F), DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboSP.SelectedIndexChanged += (s, e) => Load_();
            pnlFilter.Controls.Add(cboSP);

            var btnReload = MakeBtn("Tải lại", Color.FromArgb(85, 110, 155));
            btnReload.Location = new Point(434, 10);
            btnReload.Click += (s, e) => { LoadComboSP(); Load_(); };
            pnlFilter.Controls.Add(btnReload);

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

        void LoadComboSP()
        {
            using (var c = DBConnection.GetConnection())
            {
                var dt = new DataTable();
                new SqlDataAdapter(
                    "SELECT '' AS MaSP, N'-- Tất cả sản phẩm --' AS HT " +
                    "UNION SELECT MaSP, MaSP+' — '+TenSP AS HT FROM SANPHAM ORDER BY HT", c).Fill(dt);
                cboSP.DataSource = dt;
                cboSP.DisplayMember = "HT";
                cboSP.ValueMember = "MaSP";
            }
        }

        void Load_()
        {
            try
            {
                string ma = cboSP.SelectedValue?.ToString() ?? "";
                string where = string.IsNullOrEmpty(ma) ? "" : "WHERE ct.MaSP = @ma";

                using (var c = DBConnection.GetConnection())
                {
                    var sql = $@"
                        SELECT sp.TenSP                                 AS [Sản Phẩm],
                               ct.MaHD                                  AS [Mã HĐ],
                               kh.TenCty                                AS [Khách Hàng],
                               nv.Ho + ' ' + nv.Ten                    AS [Nhân Viên],
                               CONVERT(VARCHAR, hd.NgayLapHD, 103)     AS [Ngày Lập],
                               ct.SoLuong                              AS [Số Lượng],
                               FORMAT(sp.DonGia, N'N0') + N' đ'        AS [Đơn Giá],
                               FORMAT(ct.SoLuong * sp.DonGia, N'N0') + N' đ'  AS [Thành Tiền]
                        FROM CHITIETHOADON ct
                        JOIN SANPHAM   sp ON ct.MaSP = sp.MaSP
                        JOIN HOADON    hd ON ct.MaHD = hd.MaHD
                        LEFT JOIN KHACHHANG kh ON hd.MaKH = kh.MaKH
                        LEFT JOIN NHANVIEN  nv ON hd.MaNV = nv.MaNV
                        {where}
                        ORDER BY sp.TenSP, ct.MaHD";

                    var cmd = new SqlCommand(sql, c);
                    if (!string.IsNullOrEmpty(ma))
                        cmd.Parameters.AddWithValue("@ma", ma);

                    var dt = new DataTable();
                    new SqlDataAdapter(cmd).Fill(dt);
                    dgv.DataSource = dt;

                    // Tính tổng số lượng
                    long tongSL = 0;
                    foreach (DataRow r in dt.Rows)
                        if (long.TryParse(r["Số Lượng"]?.ToString(), out long sl)) tongSL += sl;

                    lblTong.Text = $"  Tổng: {dt.Rows.Count} dòng  |  Tổng số lượng: {tongSL:N0}";
                }
            }
            catch (Exception ex) { FormHelper.ShowError(ex.Message); }
        }

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
