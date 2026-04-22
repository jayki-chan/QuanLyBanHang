using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBanHang_BUS;
using QuanLyBanHang_DTO;


namespace QuanLyBanHang_GUI
{
    public class PhieuHoaDon : Form
    {
        private string _maHD;
        private HoaDonBUS _busHD = new HoaDonBUS();
        private ChiTietHoaDonBUS _busCT = new ChiTietHoaDonBUS();

        private Label lblInfo, lblTotal;
        private DataGridView dgvItems;

        public PhieuHoaDon(string maHD)
        {
            _maHD = maHD;
            BuildUI();
            LoadData();
        }

        void BuildUI()
        {
            this.Text = "Phiếu Hóa Đơn";
            this.ClientSize = new Size(380, 620);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 246, 250);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Dải nút bấm ở dưới cùng (nằm ngoài tờ giấy)
            var pnlExport = new TableLayoutPanel {
                Dock = DockStyle.Bottom, Height = 66,
                ColumnCount = 3, RowCount = 1,
                Padding = new Padding(8, 10, 8, 16), // Căn lề trái phải bằng với lề tờ giấy (8)
                BackColor = Color.FromArgb(245, 246, 250)
            };
            pnlExport.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            pnlExport.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            pnlExport.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));

            var btnIn = new Button { Text = "🖨️ In", Dock = DockStyle.Fill, BackColor = Color.FromArgb(30, 55, 100), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F), Cursor = Cursors.Hand, Margin = new Padding(0, 0, 6, 0) };
            btnIn.FlatAppearance.BorderSize = 0;
            btnIn.Click += (s, e) => MessageBox.Show("Đang kết nối máy in...", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

            var btnExcel = new Button { Text = "📊 Excel", Dock = DockStyle.Fill, BackColor = Color.FromArgb(34, 139, 86), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F), Cursor = Cursors.Hand, Margin = new Padding(3, 0, 3, 0) };
            btnExcel.FlatAppearance.BorderSize = 0;
            btnExcel.Click += (s, e) => ExportToExcel();

            var btnEInv = new Button { Text = "📧 E-Invoice", Dock = DockStyle.Fill, BackColor = Color.FromArgb(175, 118, 18), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F), Cursor = Cursors.Hand, Margin = new Padding(6, 0, 0, 0) };
            btnEInv.FlatAppearance.BorderSize = 0;
            btnEInv.Click += (s, e) => MessageBox.Show("Đang phát hành hóa đơn điện tử...", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

            pnlExport.Controls.Add(btnIn, 0, 0);
            pnlExport.Controls.Add(btnExcel, 1, 0);
            pnlExport.Controls.Add(btnEInv, 2, 0);

            // Container nền tạo khoảng trắng viền xung quanh tờ giấy
            var pnlPaperBg = new Panel {
                Dock = DockStyle.Fill,
                Padding = new Padding(8, 8, 8, 6),
                BackColor = Color.FromArgb(245, 246, 250)
            };

            // Bảng trắng (Tờ giấy hóa đơn)
            var pnlPaper = new Panel {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            pnlPaper.Paint += (s, e) => e.Graphics.DrawRectangle(new Pen(Color.FromArgb(210, 215, 225)), 0, 0, pnlPaper.Width - 1, pnlPaper.Height - 1);

            var lblHeader = new Label {
                Text = "-- HÓA ĐƠN THANH TOÁN --",
                Dock = DockStyle.Top, Height = 60,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                Padding = new Padding(0, 15, 0, 0)
            };

            lblInfo = new Label {
                Dock = DockStyle.Top, Height = 90,
                Font = new Font("Segoe UI", 9.5F), 
                Padding = new Padding(15, 5, 15, 5)
            };

            var pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 90 };
            lblTotal = new Label {
                Dock = DockStyle.Top, Height = 40,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Padding = new Padding(0, 10, 15, 0)
            };
            var lblThanks = new Label {
                Dock = DockStyle.Top, Height = 30,
                Text = "Cảm ơn Quý Khách & Hẹn Gặp Lại!",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Italic),
                UseMnemonic = false
            };
            pnlBottom.Controls.Add(lblThanks);
            pnlBottom.Controls.Add(lblTotal);

            dgvItems = new DataGridView {
                Dock = DockStyle.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                RowHeadersVisible = false, AllowUserToAddRows = false, ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                GridColor = Color.FromArgb(230, 230, 230), Font = new Font("Segoe UI", 9F)
            };
            dgvItems.DefaultCellStyle.SelectionBackColor = Color.White; dgvItems.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvItems.ColumnHeadersDefaultCellStyle.BackColor = Color.White; dgvItems.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvItems.EnableHeadersVisualStyles = false;

            pnlPaper.Controls.Add(dgvItems);
            pnlPaper.Controls.Add(pnlBottom);
            pnlPaper.Controls.Add(lblInfo);
            pnlPaper.Controls.Add(lblHeader);

            pnlPaperBg.Controls.Add(pnlPaper);
            this.Controls.Add(pnlPaperBg);
            this.Controls.Add(pnlExport);
        }

        private void ExportToExcel()
        {
            var hd = _busHD.GetByMa(_maHD);
            if (hd == null) return;

            try
            {
                // Sử dụng Kỹ thuật Late Binding (dynamic) để gọi Excel App
                // Yêu cầu máy tính phải cài đặt sẵn Microsoft Excel. Không cần cài thêm thư viện (như EPPlus).
                Type excelType = Type.GetTypeFromProgID("Excel.Application");
                if (excelType == null)
                {
                    MessageBox.Show("Không tìm thấy phần mềm Microsoft Excel trên máy tính của bạn.\nTính năng này yêu cầu phải cài đặt Excel.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                dynamic excelApp = Activator.CreateInstance(excelType);
                excelApp.Visible = true; // Mở giao diện Excel lên cho người dùng xem quá trình vẽ dữ liệu
                
                dynamic wb = excelApp.Workbooks.Add();
                dynamic ws = wb.ActiveSheet;
                ws.Name = "HoaDon";

                // --- Tiêu đề ---
                ws.Cells[1, 1] = "HÓA ĐƠN THANH TOÁN";
                ws.Range("A1", "D1").Merge();
                ws.Range("A1", "D1").Font.Bold = true;
                ws.Range("A1", "D1").Font.Size = 14;
                ws.Range("A1", "D1").HorizontalAlignment = -4108; // xlCenter

                // --- Thông tin chung ---
                ws.Cells[3, 1] = "Số HĐ:";      ws.Cells[3, 2] = hd.MaHD;
                ws.Cells[4, 1] = "Ngày lập:";   ws.Cells[4, 2] = hd.NgayLapHD.ToString("dd/MM/yyyy HH:mm");
                ws.Cells[5, 1] = "Thu ngân:";   ws.Cells[5, 2] = hd.HoTenNV;
                ws.Cells[6, 1] = "Khách hàng:"; ws.Cells[6, 2] = hd.TenCty;

                // --- Header Bảng ---
                ws.Cells[8, 1] = "Tên Sản Phẩm";
                ws.Cells[8, 2] = "Số Lượng";
                ws.Cells[8, 3] = "Đơn Giá";
                ws.Cells[8, 4] = "Thành Tiền";
                ws.Range("A8", "D8").Font.Bold = true;
                ws.Range("A8", "D8").Interior.Color = ColorTranslator.ToOle(Color.FromArgb(230, 230, 230));

                // --- Dữ liệu chi tiết ---
                var cts = _busCT.GetByHoaDon(_maHD);
                decimal tong = 0;
                int row = 9;
                foreach (var ct in cts)
                {
                    ws.Cells[row, 1] = ct.TenSP;
                    ws.Cells[row, 2] = ct.SoLuong;
                    ws.Cells[row, 3] = ct.DonGia;
                    ws.Cells[row, 4] = ct.ThanhTien;
                    
                    ws.Cells[row, 3].NumberFormat = "#,##0";
                    ws.Cells[row, 4].NumberFormat = "#,##0";

                    //ws.Cells[row, 2].HorizontalAlignment = -4131; // xlLeft (Căn trái Số Lượng)
                    //ws.Cells[row, 3].HorizontalAlignment = -4131; // xlLeft (Căn trái Đơn Giá)
                    //ws.Cells[row, 4].HorizontalAlignment = -4131; // xlLeft (Căn trái Thành Tiền)

                    tong += ct.ThanhTien;
                    row++;
                }

                // --- Tổng cộng ---
                ws.Cells[row, 3] = "TỔNG:";
                ws.Cells[row, 3].Font.Bold = true;
                ws.Cells[row, 4] = tong;
                ws.Cells[row, 4].Font.Bold = true;
                ws.Cells[row, 4].NumberFormat = "#,##0";

                // Kẻ khung (Borders)
                dynamic range = ws.Range("A8", "D" + row);
                range.Borders.LineStyle = 1; // xlContinuous

                // Căn chỉnh độ rộng cột CHỈ dựa trên phần bảng chi tiết (từ dòng 8 trở xuống)
                // Thông tin dài ở trên (như Tên KH) sẽ tự động tràn sang các ô trống bên phải.
                ws.Range("A8", "D" + row).Columns.AutoFit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi điều khiển Excel: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void LoadData()
        {
            // 1. Gắn thông tin Hóa Đơn
            var hd = _busHD.GetByMa(_maHD);
            if (hd != null)
            {
                lblInfo.Text = $"Số HĐ: {hd.MaHD}\n" +
                               $"Ngày lập: {hd.NgayLapHD:dd/MM/yyyy HH:mm}\n" +
                               $"Thu ngân: {hd.HoTenNV}\n" +
                               $"Khách hàng: {hd.TenCty}";
            }

            // 2. Gắn Danh sách chi tiết Sản Phẩm
            var cts = _busCT.GetByHoaDon(_maHD);
            var dt = new DataTable();
            dt.Columns.Add("Tên SP"); dt.Columns.Add("SL"); dt.Columns.Add("Đ.Giá"); dt.Columns.Add("T.Tiền");

            decimal tong = 0;
            foreach (var ct in cts)
            {
                dt.Rows.Add(ct.TenSP, ct.SoLuong, ct.DonGia.ToString("N0"), ct.ThanhTien.ToString("N0"));
                tong += ct.ThanhTien;
            }
            dgvItems.DataSource = dt;

            // 3. Căn chỉnh thẩm mỹ các cột
            if (dgvItems.Columns.Count == 4)
            {
                dgvItems.Columns[0].FillWeight = 47; // Tên SP rộng nhất
                
                dgvItems.Columns[1].FillWeight = 12; 
                dgvItems.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                
                dgvItems.Columns[2].FillWeight = 21; 
                dgvItems.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                
                dgvItems.Columns[3].FillWeight = 20; 
                dgvItems.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }

            lblTotal.Text = $"TỔNG: {tong:N0} đ";
        }
    }
}