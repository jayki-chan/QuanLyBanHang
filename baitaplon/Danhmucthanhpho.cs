using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace baitaplon
{
    public partial class Danhmucthanhpho : Form
    {

        string ketnoi = "Data Source=LAPTOP-V3HRC9O8\\MSSQLSERVER01; Database=quanlybanhang; User Id=sa; Password=123456 ";
        SqlConnection conn = null;
        SqlDataAdapter daThanhpho = null;
        DataTable dtThanhpho = null;
        bool Them;
        public Danhmucthanhpho()
        {
            InitializeComponent();
        }

        void LoadData()
        {
            try
            {
                conn = new SqlConnection(ketnoi);
               daThanhpho = new SqlDataAdapter("SELECT *  FROM thanhpho", conn);
                dtThanhpho = new DataTable();
              dtThanhpho.Clear();
              daThanhpho.Fill(dtThanhpho);

                dgvThanhpho.DataSource = dtThanhpho;
                dgvThanhpho.AutoResizeColumns();

                this.txtThanhpho.ResetText();
                this.txtTenthanhpho.ResetText();

                this.btnLuu.Enabled = false;
               this.btnHuybo.Enabled = false;
               this.panel1.Enabled = false;

                this.btnThem.Enabled = true;
               this.btnSua.Enabled = true;
                this.btnXoa.Enabled = true;
                this.btnThoat.Enabled = true;
            }
            catch (SqlException)
            {
             MessageBox.Show("Không lấy đưuọc nội dung trong table thanhpho");
            }
        }
        private void Danhmucthanhpho_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void Danhmucthanhpho_FormClosing(object sender, FormClosingEventArgs e)
        {
            dtThanhpho.Dispose();
           dtThanhpho = null;
            conn = null;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            Them = true;
            this.txtThanhpho.ResetText();
           this.txtTenthanhpho.ResetText();

            this.btnLuu.Enabled = true;
           this.btnHuybo.Enabled = true;
            this.panel1.Enabled = true;

            this.btnThem.Enabled = false;
            this.btnSua.Enabled = false;
            this.btnXoa.Enabled = false;
            this.btnThoat.Enabled = false;

            this.txtThanhpho.Focus();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            Them = false;

            this.panel1.Enabled = true;
            int r = dgvThanhpho.CurrentCell.RowIndex;

            this.txtThanhpho.Text = dgvThanhpho.Rows[r].Cells[0].Value.ToString();
            this.txtTenthanhpho.Text = dgvThanhpho.Rows[r].Cells[1].Value.ToString();

            this.btnLuu.Enabled = true;
             this.btnHuybo.Enabled = true;
            this.panel1.Enabled = true;

            this.btnThem.Enabled = false;
            this.btnSua.Enabled = false;
            this.btnXoa.Enabled = false;
            this.btnThoat.Enabled = false;

            this.txtThanhpho.Focus();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            conn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;

                int r = dgvThanhpho.CurrentCell.RowIndex;
                string strthanhpho =
                dgvThanhpho.Rows[r].Cells[0].Value.ToString();

                cmd.CommandText = System.String.Concat("Delete From Thanhpho where Thanhpho = '" + strthanhpho + "'");
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                LoadData();
                MessageBox.Show("Đã xóa xong!");
            }
            catch (SqlException)
            {
                MessageBox.Show("Không xóa được. Lỗi rồi!");
            }
        }

        private void btnHuybo_Click(object sender, EventArgs e)
        {
            this.txtThanhpho.ResetText();
            this.txtTenthanhpho.ResetText();

            this.btnThem.Enabled = true;
            this.btnSua.Enabled = true;
            this.btnXoa.Enabled = true;
            this.btnThoat.Enabled = true;

            this.btnLuu.Enabled = false;
            this.btnHuybo.Enabled = false;
            this.panel1.Enabled = false;
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            conn.Open();
            if (Them)
            {
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = System.String.Concat("Insert Into Thanhpho Values(" + "'" +
                        this.txtThanhpho.Text.ToString() + "',N'" +
                        this.txtTenthanhpho.Text.ToString() + "')");
                    cmd.CommandType |= CommandType.Text;
                    cmd.ExecuteNonQuery();
                    LoadData();
                    MessageBox.Show("Đã thêm xong!");
                }
                catch (SqlException)
                {
                    MessageBox.Show("KHông thêm được. Lỗi rồi!!!");
                }
            }

            if (!Them)
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;

                int r = dgvThanhpho.CurrentCell.RowIndex;
                string strthanhpho = dgvThanhpho.Rows[r].Cells[0].Value.ToString();
                cmd.CommandText = System.String.Concat("Update thanhpho Set Tenthanhpho = '" +
                    this.txtTenthanhpho.Text.ToString() + "'Where  Thanhpho = '" + strthanhpho + "'");
                cmd.CommandType |= CommandType.Text;
                cmd.ExecuteNonQuery();
                LoadData();
                MessageBox.Show("Đã sửa xong!");

            }
            conn.Close();

        }
        
    }
}
