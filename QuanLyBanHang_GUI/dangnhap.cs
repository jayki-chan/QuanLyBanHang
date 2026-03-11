using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using QuanLyBanHang_DAL;

namespace QuanLyBanHang_GUI
{
    public partial class dangnhap : Form
    {
        // Property để Form1 lấy tên nhân viên sau khi đăng nhập
        public string TenNhanVien { get; private set; } = "";

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

            // Dùng DBConnection chung
            using (SqlConnection conn = DBConnection.GetConnection())
            {
                try
                {
                    conn.Open();
                    string sql = "SELECT Ho + ' ' + Ten AS TenNV FROM NHANVIEN " +
                                 "WHERE Username = @user AND Matkhau = @pass";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@user", txtUser.Text.Trim());
                    cmd.Parameters.AddWithValue("@pass", txtPassword.Text.Trim());

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        TenNhanVien = result.ToString();
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