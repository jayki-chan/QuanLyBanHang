using System;
namespace QuanLyBanHang_DTO
{
    public class NhanVienDTO
    {
        public string MaNV { get; set; }
        public string Ho { get; set; }
        public string Ten { get; set; }
        public bool Nu { get; set; }
        public DateTime NgayNV { get; set; }
        public string DiaChi { get; set; }
        public string DienThoai { get; set; }
        public string Hinh { get; set; }
        public string Username { get; set; }
        public string Matkhau { get; set; }
        public string Role { get; set; } = "user";

        public string HoTen => Ho + " " + Ten;
    }
}