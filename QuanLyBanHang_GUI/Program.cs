using System;
using System.Windows.Forms;

namespace QuanLyBanHang_GUI
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            dangnhap loginForm = new dangnhap();
            DialogResult result = loginForm.ShowDialog();

            if (result == DialogResult.OK)
            {
                Application.Run(new Form1(loginForm.TenNhanVien));
            }
        }
    }
}