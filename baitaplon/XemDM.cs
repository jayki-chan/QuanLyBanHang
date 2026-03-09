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
    public partial class XemDM : Form
    {           
        string ketnoi = "Data Source=LAPTOP-V3HRC9O8\\MSSQLSERVER01; Database=quanlybanhang; User Id=sa; Password=123456 ";
        SqlConnection conn = null;
        SqlDataAdapter daTable = null;
        DataTable dtTable = null;
        public XemDM()
        {
            InitializeComponent();
        }
        private void XemDM_Load(object sender, EventArgs e)
        {
            try
            {
                conn = new SqlConnection(ketnoi);
                int intDM = Convert.ToInt32(this.Text);
                switch (intDM)
                {
                    case 1:
                        this.Text = "Danh Mục Thành Phố";
                        lblDanhmuc.Text = "Danh Mục Thành Phố";
                        daTable = new SqlDataAdapter("SELECT * FROM thanhpho", conn);
                        break;
                    case 2:
                        this.Text = "Danh Mục Khách Hàng";
                        lblDanhmuc.Text = "Danh Mục Khách Hàng";
                        daTable = new SqlDataAdapter("SELECT * FROM khachhang", conn);
                        break;
                    case 3:
                        this.Text = "Danh Mục Nhân Viên";
                        lblDanhmuc.Text = "Danh Mục Nhân Viên";
                        daTable = new SqlDataAdapter("SELECT * FROM nhanvien", conn);
                        break;
                    case 4:
                        this.Text = "Danh Mục Sản Phẩm";
                        lblDanhmuc.Text = "Danh Mục Sản Phẩm";
                        daTable = new SqlDataAdapter("SELECT * FROM SanP", conn);
                        break;
                    case 5:
                        this.Text = "Danh Mục Hóa Đơn";
                        lblDanhmuc.Text = "Danh Mục Hóa Đơn";
                        daTable = new SqlDataAdapter("SELECT * FROM hoadon", conn);
                        break;
                    case 6:
                        this.Text = "Danh Mục Chi Tiết Hóa Đơn";
                        lblDanhmuc.Text = "Danh Mục Hóa Đơn";
                        daTable = new SqlDataAdapter("SELECT * FROM chitiethoadon", conn);
                        break;
                    default:
                        break;
                }
                dtTable = new DataTable();
                dtTable.Clear();
                daTable.Fill(dtTable);
                DataGridView1.DataSource = dtTable;
                DataGridView1.AutoResizeColumns();
            }
            catch (SqlException)
            {
                MessageBox.Show("Không lấy được nội dung trong table. Lỗi rồi!!!");
            }
        }
        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
