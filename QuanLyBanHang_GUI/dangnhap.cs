using System;
using System.Windows.Forms;
using QuanLyBanHang_BUS;
using QuanLyBanHang_DTO;

namespace QuanLyBanHang_GUI
{
    public partial class dangnhap : Form
    {
        private readonly NhanVienBUS _bus = new NhanVienBUS();

        /// <summary>Thông tin nhân viên đã đăng nhập thành công.</summary>
        public NhanVienDTO LoggedInUser { get; private set; }

        /// <summary>Giữ lại để tương thích ngược với code cũ.</summary>
        public string TenNhanVien => LoggedInUser?.HoTen ?? "";

        public dangnhap()
        {
            InitializeComponent();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnDangnhap_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUser.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblError.Text = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.";
                lblError.Visible = true;
                return;
            }

            try
            {
                var user = _bus.Login(txtUser.Text.Trim(), txtPassword.Text.Trim());
                if (user != null)
                {
                    LoggedInUser = user;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    lblError.Text = "Tên đăng nhập hoặc mật khẩu không đúng.";
                    lblError.Visible = true;
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Lỗi kết nối CSDL: " + ex.Message;
                lblError.Visible = true;
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnDangnhap_Click(sender, e);
        }

        private void txtUser_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                txtPassword.Focus();
        }
    }
}