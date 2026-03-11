using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBanHang_BUS;
using QuanLyBanHang_DTO;

namespace QuanLyBanHang_GUI
{
    public partial class Form1 : Form
    {
        private string      _tenNhanVien;
        private string      _username;
        private NhanVienBUS _busNV = new NhanVienBUS();
        private DashboardPanel _dashboard;

        public Form1(string tenNhanVien = "", string username = "")
        {
            InitializeComponent();
            SetMenuIcons();
            _tenNhanVien = tenNhanVien;
            _username    = username;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;

            if (!string.IsNullOrEmpty(_tenNhanVien))
            {
                lblStatus.Text = $"  ✔  Đã đăng nhập:  {_tenNhanVien}";
                đăngNhậpToolStripMenuItem.Visible  = false;
                đăngXuấtToolStripMenuItem.Visible  = true;
            }

            // Tạo và nhúng Dashboard vào vùng chính
            _dashboard = DashboardPanel.Create();
            this.Controls.Add(_dashboard);
            _dashboard.BringToFront();
            _dashboard.LoadData();
        }

        // ── Xem Danh mục ──────────────────────────────────────
        private void danhMụcThànhPhốToolStripMenuItem_Click(object sender, EventArgs e)
            => new QuanLyThanhPho().ShowDialog();

        private void danhMụcKháchhàngToolStripMenuItem_Click(object sender, EventArgs e)
            => new QuanLyKhachHang().ShowDialog();

        private void danhMụcNhânViênToolStripMenuItem_Click(object sender, EventArgs e)
            => new QuanLyNhanVien().ShowDialog();

        private void danhMụcSảnPhẩmToolStripMenuItem_Click(object sender, EventArgs e)
            => new QuanLySanPham().ShowDialog();

        private void danhMụcHóaĐơnToolStripMenuItem_Click(object sender, EventArgs e)
            => new QuanLyHoaDon().ShowDialog();

        private void danhMụcChiTiếtHóaĐơnToolStripMenuItem_Click(object sender, EventArgs e)
            => new QuanLyChiTietHoaDon().ShowDialog();

        // ── Quản lý Danh mục đơn ──────────────────────────────
        private void danhMụcThànhPhốToolStripMenuItem1_Click(object sender, EventArgs e)
        { new QuanLyThanhPho().ShowDialog(); _dashboard?.LoadData(); }

        private void danhMụcKháchhàngToolStripMenuItem1_Click(object sender, EventArgs e)
        { new QuanLyKhachHang().ShowDialog(); _dashboard?.LoadData(); }

        private void danhMụcNhânViênToolStripMenuItem1_Click(object sender, EventArgs e)
        { new QuanLyNhanVien().ShowDialog(); _dashboard?.LoadData(); }

        private void danhMụcSảnPhẩmToolStripMenuItem1_Click(object sender, EventArgs e)
        { new QuanLySanPham().ShowDialog(); _dashboard?.LoadData(); }

        private void danhMụcHóaĐơnToolStripMenuItem1_Click(object sender, EventArgs e)
        { new QuanLyHoaDon().ShowDialog(); _dashboard?.LoadData(); }

        private void danhMụcChiTiếtHóaĐơnToolStripMenuItem1_Click(object sender, EventArgs e)
        { new QuanLyChiTietHoaDon().ShowDialog(); _dashboard?.LoadData(); }

        // ── Quản lý theo nhóm (Báo cáo) ──────────────────────
        private void kháchhàngTheoThànhPhốToolStripMenuItem_Click(object sender, EventArgs e)
            => new BaoCaoKhachHangTheoTP().ShowDialog();

        private void hóaĐơnTheoKháchhàngToolStripMenuItem_Click(object sender, EventArgs e)
            => new BaoCaoHoaDonTheoKH().ShowDialog();

        private void hóaĐơnTheoSảnPhẩmToolStripMenuItem_Click(object sender, EventArgs e)
            => new BaoCaoHoaDonTheoSP().ShowDialog();

        private void hóaĐơnTheoNhânViênToolStripMenuItem_Click(object sender, EventArgs e)
            => new BaoCaoHoaDonTheoNV().ShowDialog();

        private void chiTiếtHóaĐơnTheoNhânViênToolStripMenuItem_Click(object sender, EventArgs e)
            => new BaoCaoChiTietHoaDonTheoNV().ShowDialog();

        // ── Hệ thống ──────────────────────────────────────────
        private void câuHìnhHệThốngToolStripMenuItem_Click(object sender, EventArgs e)
            => new CauHinhHeThong().ShowDialog();

        private void quảnLýNgườiDùngToolStripMenuItem_Click(object sender, EventArgs e)
            => new QuanLyNguoiDung().ShowDialog();

        private void đổiMậtKhẩuToolStripMenuItem_Click(object sender, EventArgs e)
            => new DoiMatKhau(_username).ShowDialog();

        private void đăngNhậpToolStripMenuItem_Click(object sender, EventArgs e) { }

        private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có muốn đăng xuất không?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                Application.Restart();
        }

        private void thoátToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn thoát không?", "Xác nhận",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                Application.Exit();
        }

        // ── Giúp đỡ ───────────────────────────────────────────
        private void hướngDẫnSửDụngToolStripMenuItem_Click(object sender, EventArgs e)
            => MessageBox.Show("Hướng dẫn sử dụng phần mềm Quản Lý Bán Hàng", "Giúp đỡ", MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void tácGiảToolStripMenuItem_Click(object sender, EventArgs e)
            => MessageBox.Show("Tác giả: Sinh viên - ĐH Công Nghiệp Việt Hung", "Giúp đỡ", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
