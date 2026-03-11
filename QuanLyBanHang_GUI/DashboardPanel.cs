using QuanLyBanHang_DAL;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace QuanLyBanHang_GUI
{
    public class DashboardPanel : Panel
    {
        static readonly Color NavBlue = Color.FromArgb(30, 55, 100);
        static readonly Color BgGray  = Color.FromArgb(245, 246, 250);
        static readonly Color White   = Color.White;

        static readonly Color[] StatColors = {
            Color.FromArgb(34, 120, 86),
            Color.FromArgb(30, 90, 160),
            Color.FromArgb(160, 90, 20),
            Color.FromArgb(140, 30, 80),
        };

        Label        _lblKH, _lblNV, _lblSP, _lblHD;
        Label        _lblChartTitle;
        ChartPanel   _chart;
        DataGridView _dgv;
        ComboBox     _cboNam;

        public static DashboardPanel Create()
        {
            var d = new DashboardPanel();
            d.Dock      = DockStyle.Fill;
            d.BackColor = BgGray;
            d.BuildUI();
            return d;
        }

        void BuildUI()
        {
            var outer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 6,
                BackColor = BgGray, Padding = new Padding(16, 10, 16, 10)
            };
            outer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));   // 0: tiêu đề stat
            outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));  // 1: stat cards
            outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));   // 2: tiêu đề shortcut
            outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 90));   // 3: shortcuts
            outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));   // 4: tiêu đề chart + chọn năm
            outer.RowStyles.Add(new RowStyle(SizeType.Percent, 100));   // 5: chart + grid

            // ── Row 0 ─────────────────────────────────────────
            outer.Controls.Add(SectionLbl("Tổng quan hệ thống"), 0, 0);

            // ── Row 1: Stat cards ─────────────────────────────
            var rowStats = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 1,
                BackColor = Color.Transparent
            };
            for (int i = 0; i < 4; i++)
                rowStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

            string[]   titles = { "Khách Hàng", "Nhân Viên", "Sản Phẩm", "Hóa Đơn" };
            IconType[] icons  = { IconType.User, IconType.Users, IconType.Product, IconType.Invoice };
            Label[]    lbls   = new Label[4];
            for (int i = 0; i < 4; i++)
            {
                var card = BuildStatCard(titles[i], icons[i], StatColors[i], out lbls[i]);
                card.Margin = new Padding(0, 0, i < 3 ? 10 : 0, 0);
                rowStats.Controls.Add(card, i, 0);
            }
            _lblKH = lbls[0]; _lblNV = lbls[1]; _lblSP = lbls[2]; _lblHD = lbls[3];
            outer.Controls.Add(rowStats, 0, 1);

            // ── Row 2: Tiêu đề shortcut ───────────────────────
            outer.Controls.Add(SectionLbl("Truy cập nhanh"), 0, 2);

            // ── Row 3: Shortcuts ──────────────────────────────
            var rowShort = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false, BackColor = Color.Transparent
            };
            var shortcuts = new (string text, IconType icon, Color bg, EventHandler click)[]
            {
                ("Khách Hàng", IconType.User,    Color.FromArgb(30, 90, 160),  (s,e)=>{ new QuanLyKhachHang().ShowDialog(); LoadData(); }),
                ("Nhân Viên",  IconType.Users,   Color.FromArgb(34, 120, 86),  (s,e)=>{ new QuanLyNhanVien().ShowDialog();  LoadData(); }),
                ("Sản Phẩm",   IconType.Product, Color.FromArgb(160, 90, 20),  (s,e)=>{ new QuanLySanPham().ShowDialog();   LoadData(); }),
                ("Hóa Đơn",    IconType.Invoice, Color.FromArgb(140, 30, 80),  (s,e)=>{ new QuanLyHoaDon().ShowDialog();    LoadData(); }),
                ("BC KH/TP",   IconType.Chart,   Color.FromArgb(70, 50, 140),  (s,e)=>  new BaoCaoKhachHangTheoTP().ShowDialog()),
                ("BC HĐ/NV",   IconType.Chart,   Color.FromArgb(20, 110, 130), (s,e)=>  new BaoCaoHoaDonTheoNV().ShowDialog()),
            };
            foreach (var sc in shortcuts)
            {
                var btn = new Button
                {
                    Text = sc.text, Size = new Size(130, 78),
                    BackColor = sc.bg, ForeColor = White, FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
                    Cursor = Cursors.Hand, Margin = new Padding(0, 0, 10, 0),
                    Image = AppIcons.Get(sc.icon, 26, White),
                    ImageAlign = ContentAlignment.TopCenter, TextAlign = ContentAlignment.BottomCenter,
                    TextImageRelation = TextImageRelation.ImageAboveText,
                    Padding = new Padding(0, 10, 0, 8)
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.Click += sc.click;
                rowShort.Controls.Add(btn);
            }
            outer.Controls.Add(rowShort, 0, 3);

            // ── Row 4: Tiêu đề chart + ComboBox năm + nút làm mới ──
            var pnlChartHdr = new Panel
            {
                Dock = DockStyle.Fill, BackColor = Color.Transparent
            };

            _lblChartTitle = new Label
            {
                Text = "Doanh thu theo tháng",
                Location = new Point(2, 8), AutoSize = true,
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                ForeColor = NavBlue
            };

            var lblNam = new Label
            {
                Text = "Năm:", Location = new Point(200, 10), AutoSize = true,
                Font = new Font("Segoe UI", 9F), ForeColor = NavBlue
            };

            _cboNam = new ComboBox
            {
                Location = new Point(238, 6), Size = new Size(80, 24),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            // Thêm 5 năm gần nhất
            int curYear = DateTime.Now.Year;
            for (int y = curYear; y >= curYear - 4; y--)
                _cboNam.Items.Add(y);
            _cboNam.SelectedIndex = 0;
            _cboNam.SelectedIndexChanged += (s, e) => LoadChart();

            var lblSep = new Label
            {
                Text = "|   Hóa đơn gần đây",
                Location = new Point(330, 10), AutoSize = true,
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                ForeColor = NavBlue
            };

            var btnRefresh = new Button
            {
                Text = "  Làm mới", Size = new Size(110, 26),
                Location = new Point(540, 4),
                BackColor = NavBlue, ForeColor = White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 8.5F),
                Cursor = Cursors.Hand,
                Image = AppIcons.Get(IconType.Reload, 14, White),
                ImageAlign = ContentAlignment.MiddleLeft,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                Padding = new Padding(2, 0, 0, 0)
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += (s, e) => LoadData();

            pnlChartHdr.Controls.Add(_lblChartTitle);
            pnlChartHdr.Controls.Add(lblNam);
            pnlChartHdr.Controls.Add(_cboNam);
            pnlChartHdr.Controls.Add(lblSep);
            pnlChartHdr.Controls.Add(btnRefresh);
            outer.Controls.Add(pnlChartHdr, 0, 4);

            // ── Row 5: Chart + Grid ───────────────────────────
            var rowBottom = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1,
                BackColor = Color.Transparent
            };
            rowBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 57));
            rowBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 43));

            _chart = new ChartPanel { Dock = DockStyle.Fill, BackColor = White, Margin = new Padding(0, 0, 10, 0) };

            _dgv = new DataGridView();
            StyleGrid(_dgv);
            var dgvCard = new Panel { Dock = DockStyle.Fill, BackColor = White };
            _dgv.Dock = DockStyle.Fill;
            dgvCard.Controls.Add(_dgv);

            rowBottom.Controls.Add(_chart, 0, 0);
            rowBottom.Controls.Add(dgvCard, 1, 0);
            outer.Controls.Add(rowBottom, 0, 5);

            this.Controls.Add(outer);
            this.VisibleChanged += (s, e) => { if (Visible) LoadData(); };
        }

        Panel BuildStatCard(string title, IconType icon, Color bg, out Label valLbl)
        {
            var card = new Panel { Dock = DockStyle.Fill, BackColor = bg };
            card.Paint += (s, e) =>
            {
                var ico = AppIcons.Get(icon, 50, Color.FromArgb(35, 255, 255, 255));
                e.Graphics.DrawImage(ico, card.Width - 58, card.Height - 56);
            };
            var lblTitle = new Label
            {
                Text = title, AutoSize = false, Location = new Point(14, 12), Size = new Size(150, 20),
                Font = new Font("Segoe UI", 9.5F), ForeColor = Color.FromArgb(200, 230, 255), BackColor = Color.Transparent
            };
            valLbl = new Label
            {
                Text = "...", AutoSize = false, Location = new Point(14, 36), Size = new Size(150, 40),
                Font = new Font("Segoe UI Black", 24F, FontStyle.Bold), ForeColor = White, BackColor = Color.Transparent
            };
            card.Controls.Add(lblTitle);
            card.Controls.Add(valLbl);
            return card;
        }

        Label SectionLbl(string text) => new Label
        {
            Text = "  " + text, Dock = DockStyle.Fill,
            Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
            ForeColor = NavBlue, BackColor = Color.Transparent,
            TextAlign = ContentAlignment.BottomLeft
        };

        void StyleGrid(DataGridView g)
        {
            g.AllowUserToAddRows  = false; g.ReadOnly = true;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.RowHeadersVisible = false; g.BorderStyle = BorderStyle.None;
            g.BackgroundColor = White; g.GridColor = Color.FromArgb(220, 226, 238);
            g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            g.ColumnHeadersDefaultCellStyle.BackColor = NavBlue;
            g.ColumnHeadersDefaultCellStyle.ForeColor = White;
            g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold);
            g.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 0, 0, 0);
            g.ColumnHeadersHeight = 30; g.EnableHeadersVisualStyles = false;
            g.DefaultCellStyle.Font = new Font("Segoe UI", 8.5F);
            g.DefaultCellStyle.Padding = new Padding(4, 0, 0, 0);
            g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(205, 222, 252);
            g.DefaultCellStyle.SelectionForeColor = Color.FromArgb(15, 38, 80);
            g.RowTemplate.Height = 26;
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(246, 249, 255);
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        // ── Load tất cả ───────────────────────────────────────
        public void LoadData()
        {
            try
            {
                using (var conn = DBConnection.GetConnection())
                {
                    conn.Open();

                    // Thẻ thống kê
                    string[] tbls = { "KHACHHANG", "NHANVIEN", "SANPHAM", "HOADON" };
                    Label[]  lbls = { _lblKH, _lblNV, _lblSP, _lblHD };
                    for (int i = 0; i < 4; i++)
                    {
                        var cmd = new SqlCommand("SELECT COUNT(*) FROM " + tbls[i], conn);
                        if (lbls[i] != null) lbls[i].Text = cmd.ExecuteScalar().ToString();
                    }

                    // Hóa đơn gần đây
                    var dtRecent = new DataTable();
                    new SqlDataAdapter(@"
                        SELECT TOP 8
                            h.MaHD   AS [Mã HĐ],
                            k.TenCty AS [Khách Hàng],
                            CONVERT(varchar, h.NgayLapHD, 103) AS [Ngày lập],
                            FORMAT(
                                ISNULL((
                                    SELECT SUM(ct.SoLuong * sp.DonGia)
                                    FROM CHITIETHOADON ct
                                    JOIN SANPHAM sp ON sp.MaSP = ct.MaSP
                                    WHERE ct.MaHD = h.MaHD
                                ), 0), N'#,##0'
                            ) + N' đ' AS [Tổng tiền]
                        FROM HOADON h
                        LEFT JOIN KHACHHANG k ON h.MaKH = k.MaKH
                        ORDER BY h.NgayLapHD DESC, h.MaHD DESC",
                        conn).Fill(dtRecent);
                    _dgv.DataSource = dtRecent;

                    // Cập nhật ComboBox năm với các năm có dữ liệu
                    RefreshYearCombo(conn);
                }
                // Load biểu đồ theo năm đang chọn
                LoadChart();
            }
            catch (Exception ex)
            {
                if (_lblKH != null) _lblKH.Text = "—";
                if (_lblNV != null) _lblNV.Text = "—";
                if (_lblSP != null) _lblSP.Text = "—";
                if (_lblHD != null) _lblHD.Text = "—";
                _chart?.SetData(null);
            }
        }

        void RefreshYearCombo(SqlConnection conn)
        {
            if (_cboNam == null) return;
            try
            {
                var cmd = new SqlCommand(
                    "SELECT DISTINCT YEAR(NgayLapHD) AS Nam FROM HOADON ORDER BY Nam DESC", conn);
                var reader = cmd.ExecuteReader();
                var years = new System.Collections.Generic.List<int>();
                while (reader.Read()) years.Add(Convert.ToInt32(reader["Nam"]));
                reader.Close();

                if (years.Count == 0) return;

                // Thêm năm chưa có trong combo
                int selected = _cboNam.SelectedItem != null ? (int)_cboNam.SelectedItem : years[0];
                _cboNam.Items.Clear();
                foreach (var y in years) _cboNam.Items.Add(y);
                // Chọn lại năm cũ nếu còn, không thì chọn năm mới nhất
                _cboNam.SelectedItem = years.Contains(selected) ? (object)selected : years[0];
            }
            catch { }
        }

        void LoadChart()
        {
            if (_cboNam == null || _cboNam.SelectedItem == null) return;
            int year = (int)_cboNam.SelectedItem;

            // Cập nhật tiêu đề
            if (_lblChartTitle != null)
                _lblChartTitle.Text = "Doanh thu theo tháng — " + year;

            try
            {
                using (var conn = DBConnection.GetConnection())
                {
                    conn.Open();
                    var dtChart = new DataTable();
                    var cmd = new SqlCommand(@"
                        SELECT t.thang,
                               ISNULL(SUM(ct.SoLuong * sp.DonGia), 0) AS doanhthu
                        FROM (
                            SELECT 1 thang UNION SELECT 2 UNION SELECT 3 UNION SELECT 4
                            UNION SELECT 5 UNION SELECT 6 UNION SELECT 7 UNION SELECT 8
                            UNION SELECT 9 UNION SELECT 10 UNION SELECT 11 UNION SELECT 12
                        ) t
                        LEFT JOIN HOADON h
                            ON MONTH(h.NgayLapHD) = t.thang
                            AND YEAR(h.NgayLapHD) = @year
                        LEFT JOIN CHITIETHOADON ct ON ct.MaHD = h.MaHD
                        LEFT JOIN SANPHAM sp       ON sp.MaSP = ct.MaSP
                        GROUP BY t.thang
                        ORDER BY t.thang", conn);
                    cmd.Parameters.AddWithValue("@year", year);
                    new SqlDataAdapter(cmd).Fill(dtChart);
                    _chart.SetData(dtChart, year);
                }
            }
            catch { _chart?.SetData(null, year); }
        }
    }

    // ════════════════════════════════════════════════════════
    // ChartPanel
    // ════════════════════════════════════════════════════════
    public class ChartPanel : Panel
    {
        static readonly Color NavBlue = Color.FromArgb(30, 55, 100);
        double[] _vals = new double[12];
        int      _year = DateTime.Now.Year;
        int      _hover = -1;

        public ChartPanel()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            MouseMove  += (s, e) => { _hover = HitBar(e.X); Invalidate(); };
            MouseLeave += (s, e) => { _hover = -1; Invalidate(); };
        }

        public void SetData(DataTable dt, int year = 0)
        {
            _vals = new double[12];
            _year = year > 0 ? year : DateTime.Now.Year;
            if (dt != null)
                foreach (DataRow r in dt.Rows)
                {
                    int m = Convert.ToInt32(r["thang"]) - 1;
                    if (m >= 0 && m < 12) _vals[m] = Convert.ToDouble(r["doanhthu"]);
                }
            Invalidate();
        }

        int HitBar(int mx)
        {
            if (Width <= 0) return -1;
            int pad = 46, w = Width - pad * 2;
            float bw = w / 12f;
            int i = (int)((mx - pad) / bw);
            return (i >= 0 && i < 12) ? i : -1;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode     = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int padL = 50, padR = 12, padT = 22, padB = 32;
            int chartW = Width - padL - padR, chartH = Height - padT - padB;
            if (chartW <= 0 || chartH <= 0) return;

            double maxV = 1;
            foreach (var v in _vals) if (v > maxV) maxV = v;
            double step = NiceStep(maxV, 4);
            double maxY = Math.Ceiling(maxV / step) * step;

            var penGrid  = new Pen(Color.FromArgb(218, 224, 236));
            var penAxis  = new Pen(Color.FromArgb(180, 195, 215));
            var fntSm    = new Font("Segoe UI", 7.5F);
            var fntTip   = new Font("Segoe UI Semibold", 7.5F, FontStyle.Bold);
            var brushLbl = new SolidBrush(Color.FromArgb(100, 120, 160));
            var sfC      = new StringFormat { Alignment = StringAlignment.Center };
            var sfR      = new StringFormat { Alignment = StringAlignment.Far };

            for (int i = 0; i <= 4; i++)
            {
                double yv = maxY * i / 4;
                int yp = padT + chartH - (int)(chartH * yv / maxY);
                g.DrawLine(penGrid, padL, yp, padL + chartW, yp);
                string lbl = yv >= 1_000_000 ? (yv/1_000_000).ToString("0.#")+"M"
                           : yv >= 1_000     ? (yv/1_000).ToString("0")+"K"
                           : yv.ToString("0");
                g.DrawString(lbl, fntSm, brushLbl, new RectangleF(0, yp-9, padL-4, 18), sfR);
            }
            g.DrawLine(penAxis, padL, padT+chartH, padL+chartW, padT+chartH);
            g.DrawLine(penAxis, padL, padT, padL, padT+chartH);

            float barW = chartW / 12f, gap = barW * 0.22f;
            string[] months = {"T1","T2","T3","T4","T5","T6","T7","T8","T9","T10","T11","T12"};

            for (int i = 0; i < 12; i++)
            {
                float x = padL + i * barW + gap / 2, w = barW - gap;
                int bh = (int)(chartH * _vals[i] / maxY);
                int yb = padT + chartH - bh;

                // Tháng hiện tại chỉ highlight nếu đúng năm
                bool isCurMonth = (i == DateTime.Now.Month - 1) && (_year == DateTime.Now.Year);
                Color bc = i == _hover  ? Color.FromArgb(20, 60, 140)
                         : isCurMonth   ? Color.FromArgb(34, 120, 86)
                         : Color.FromArgb(60, 110, 200);

                if (bh > 0)
                {
                    using (var br = new SolidBrush(bc))
                    using (var path = RoundTop(new RectangleF(x, yb, w, bh), 3))
                        g.FillPath(br, path);
                }

                if (i == _hover && _vals[i] > 0)
                {
                    string tip = _vals[i] >= 1_000_000 ? (_vals[i]/1_000_000).ToString("0.##")+"M đ"
                               : _vals[i] >= 1_000     ? (_vals[i]/1_000).ToString("0")+"K đ"
                               : _vals[i].ToString("0")+" đ";
                    var tr = new RectangleF(x-8, yb-18, w+16, 16);
                    g.FillRectangle(new SolidBrush(Color.FromArgb(230, NavBlue)), tr);
                    g.DrawString(tip, fntTip, new SolidBrush(Color.White), tr, sfC);
                }

                g.DrawString(months[i], fntSm, brushLbl,
                    new RectangleF(x, padT+chartH+4, w, 16), sfC);
            }

            // Tổng năm
            double total = 0; foreach (var v in _vals) total += v;
            string totalStr = total >= 1_000_000 ? "Tổng: " + (total/1_000_000).ToString("0.##") + "M đ"
                            : total >= 1_000     ? "Tổng: " + (total/1_000).ToString("0") + "K đ"
                            : total > 0          ? "Tổng: " + total.ToString("0") + " đ"
                            : "Chưa có dữ liệu";
            g.DrawString(totalStr, fntTip, new SolidBrush(Color.FromArgb(34,120,86)), padL+4, 4);
        }

        static double NiceStep(double max, int parts)
        {
            double raw = max / parts;
            double mag = Math.Pow(10, Math.Floor(Math.Log10(raw < 1 ? 1 : raw)));
            foreach (double n in new[] {1.0,2.0,5.0,10.0})
                if (n * mag >= raw) return n * mag;
            return 10 * mag;
        }

        static GraphicsPath RoundTop(RectangleF r, float rad)
        {
            float d = rad * 2;
            var p = new GraphicsPath();
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right-d, r.Y, d, d, 270, 90);
            p.AddLine(r.Right, r.Y+rad, r.Right, r.Bottom);
            p.AddLine(r.Right, r.Bottom, r.X, r.Bottom);
            p.AddLine(r.X, r.Bottom, r.X, r.Y+rad);
            p.CloseFigure();
            return p;
        }
    }
}
