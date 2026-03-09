using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace baitaplon
{
    public partial class Dangnhap : Form
    {
        string ketnoi = "Data Source=LAPTOP-V3HRC9O8\\MSSQLSERVER01; Database=quanlybanhang; User Id=sa; Password=123456 ";
        public Dangnhap()
        {
            InitializeComponent();
        }
        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult traloi;
            traloi = MessageBox.Show("Bạn có chắc muốn thoát không?", "trả lời", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (traloi == DialogResult.OK)
                this.Close();
        }
        private void Dangnhap_Load(object sender, EventArgs e)
        {
        }
        private void btnDangnhap_Click(object sender, EventArgs e)
        {
            string tk = txtUser.Text;
            string mk = txtPass.Text;

            try
            {
                SqlConnection conn = new SqlConnection(ketnoi);
                conn.Open();
                string sql = "Select * from thanhvien where Username='" + tk + "' and Pass='" + mk + "'";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader da = cmd.ExecuteReader();
                if (da.Read() == true)
                {
                    MessageBox.Show("Đăng nhập thành công");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Bạn nhập sai username hoặc password");
                }
            }
            catch (Exception) 
            {
                MessageBox.Show("Lỗi kết nối");
            }

        }
    }
}
