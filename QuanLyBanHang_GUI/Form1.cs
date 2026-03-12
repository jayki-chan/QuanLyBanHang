using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBanHang_BUS;
using QuanLyBanHang_DTO;

namespace QuanLyBanHang_GUI
{
    public partial class Form1 : Form
    {
        private string         _tenNhanVien;
        private string         _username;
        private NhanVienDTO    _loggedUser;
        private NhanVienBUS    _busNV = new NhanVienBUS();
        private DashboardPanel _dashboard;

        public Form1(NhanVienDTO user = null)
        {
            InitializeComponent();
            SetMenuIcons();
            _loggedUser  = user;
            _tenNhanVien = user?.HoTen    ?? "";
            _username    = user?.Username ?? "";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;

            if (!string.IsNullOrEmpty(_tenNhanVien))
            {
                đăngNhậpToolStripMenuItem.Visible  = false;
                đăngXuấtToolStripMenuItem.Visible  = true;
            }

            ApplyRoleBasedUI();

            // Tạo và nhúng Dashboard vào vùng chính (truyền role để hiển thị đúng cấp bậc)
            _dashboard = DashboardPanel.Create(_loggedUser?.Role);
            this.Controls.Add(_dashboard);
            _dashboard.BringToFront();
            _dashboard.LoadData();
        }

        /// <summary>
        /// Phân luồng hiển thị menu theo 3 cấp bậc:
        ///   admin     -> toàn quyền
        ///   sales     -> khách hàng, hóa đơn, chi tiết hóa đơn
        ///   warehouse -> chỉ sản phẩm
        /// </summary>
        private void ApplyRoleBasedUI()
        {
            string role      = (_loggedUser?.Role ?? "").ToLower();
            bool isAdmin     = role == "admin";
            bool isSales     = role == "sales";
            bool isWarehouse = role == "warehouse";

            // ── Hệ thống ────────────────────────────────────────────────────
            câuHìnhHệThốngToolStripMenuItem.Visible  = isAdmin;
            quảnLýNgườiDùngToolStripMenuItem.Visible = isAdmin;

            // ── Xem Danh mục ────────────────────────────────────────────────
            danhMụcThànhPhốToolStripMenuItem.Visible          = isAdmin || isSales;
            danhMụcKháchhàngToolStripMenuItem.Visible         = isAdmin || isSales;
            danhMụcNhânViênToolStripMenuItem.Visible          = isAdmin;
            danhMụcSảnPhẩmToolStripMenuItem.Visible          = isAdmin || isWarehouse;
            danhMụcHóaĐơnToolStripMenuItem.Visible           = isAdmin || isSales;
            danhMụcChiTiếtHóaĐơnToolStripMenuItem.Visible   = isAdmin || isSales;

            // ── Quản lý Danh mục đơn ────────────────────────────────────────
            danhMụcThànhPhốToolStripMenuItem1.Visible         = isAdmin || isSales;
            danhMụcKháchhàngToolStripMenuItem1.Visible        = isAdmin || isSales;
            danhMụcNhânViênToolStripMenuItem1.Visible         = isAdmin;
            danhMụcSảnPhẩmToolStripMenuItem1.Visible         = isAdmin || isWarehouse;
            danhMụcHóaĐơnToolStripMenuItem1.Visible          = isAdmin || isSales;
            danhMụcChiTiếtHóaĐơnToolStripMenuItem1.Visible  = isAdmin || isSales;

            // ── Báo cáo / Nhóm ──────────────────────────────────────────────
            kháchhàngTheoThànhPhốToolStripMenuItem.Visible      = isAdmin || isSales;
            hóaĐơnTheoKháchhàngToolStripMenuItem.Visible        = isAdmin || isSales;
            hóaĐơnTheoSảnPhẩmToolStripMenuItem.Visible         = isAdmin || isSales;
            hóaĐơnTheoNhânViênToolStripMenuItem.Visible         = isAdmin;
            chiTiếtHóaĐơnTheoNhânViênToolStripMenuItem.Visible  = isAdmin;
            // Ẩn toàn bộ menu Báo cáo đối với warehouse (không có báo cáo liên quan)
            quảnLýDanhMụcTheoNhómToolStripMenuItem.Visible     = isAdmin || isSales;

            // ── Status bar: hiển thị cấp bậc ────────────────────────────────
            string roleName = isAdmin     ? "Quản trị viên"
                            : isSales     ? "Nhân viên bán hàng"
                            : isWarehouse ? "Nhân viên kho hàng"
                            : "Người dùng";
            if (!string.IsNullOrEmpty(_tenNhanVien))
                lblStatus.Text = $"  ✔  Đã đăng nhập: {_tenNhanVien}  |  Cấp bậc: {roleName}";
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

        // ── Hệ thống ─────────────────────────────────────────
        private void câuHìnhHệThốngToolStripMenuItem_Click(object sender, EventArgs e)
            => new CauHinhHeThong().ShowDialog();

        private void quảnLýNgườiDùngToolStripMenuItem_Click(object sender, EventArgs e)
            => new QuanLyNguoiDung().ShowDialog();

        private void đổiMậtKhẩuToolStripMenuItem_Click(object sender, EventArgs e)
            => new DoiMatKhau(_username).ShowDialog();

        private void đăngNhậpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var f = new dangnhap())
            {
                if (f.ShowDialog() == DialogResult.OK && f.LoggedInUser != null)
                {
                    _loggedUser  = f.LoggedInUser;
                    _tenNhanVien = _loggedUser.HoTen;
                    _username    = _loggedUser.Username;
                    đăngNhậpToolStripMenuItem.Visible = false;
                    đăngXuấtToolStripMenuItem.Visible = true;
                    ApplyRoleBasedUI();

                    // Tái tạo Dashboard đúng role mới
                    if (_dashboard != null)
                    {
                        this.Controls.Remove(_dashboard);
                        _dashboard.Dispose();
                    }
                    _dashboard = DashboardPanel.Create(_loggedUser?.Role);
                    this.Controls.Add(_dashboard);
                    _dashboard.BringToFront();
                    _dashboard.LoadData();
                }
            }
        }

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
