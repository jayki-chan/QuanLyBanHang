using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace baitaplon
{
    public partial class frmMain : Form
    {
       public void xemDanhMuc (int intDanhMuc)
        {
            XemDM tp = new XemDM ();
            tp.Text = intDanhMuc.ToString ();
            tp.ShowDialog();
        }
        public frmMain()
        {
            InitializeComponent();
        }
        private void danhMụcThànhPhốToolStripMenuItem_Click(object sender, EventArgs e)
        {
            xemDanhMuc(1);
        }
        private void danhMụcKháchHàngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            xemDanhMuc(2);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Dangnhap Dangnhap = new Dangnhap();
            Dangnhap.FormClosed += Dangnhap_FormClosed;
            Dangnhap.ShowDialog();
            this.WindowState = FormWindowState.Normal;
        }
        private void danhMụcThànhPhoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Danhmucthanhpho dmtp = new Danhmucthanhpho();
            dmtp.Text = "Quản Lý Danh Mục Thành Phố";
            dmtp.ShowDialog();
        }
        private void danhMụcSảnPhẩmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            xemDanhMuc(4);
        }
         private void danhMụcHóaĐơnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            xemDanhMuc(5);
        }

        private void danhMụcChiTiếtHóaĐơnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            xemDanhMuc(6);
        }
        private void xemDanhMụcToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void mậtKhẩuToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }



        private void quanToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void đăngNhậpToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void Dangnhap_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.menuStrip1.Enabled = true;
        }

        private void danhMụcNhânViênToolStripMenuItem_Click(object sender, EventArgs e)
        {
            xemDanhMuc(3);
        }
    }
}
