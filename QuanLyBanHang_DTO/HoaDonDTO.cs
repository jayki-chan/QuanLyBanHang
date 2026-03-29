using System;
namespace QuanLyBanHang_DTO
{
    public class HoaDonDTO
    {
        public string MaHD { get; set; }
        public string MaKH { get; set; }
        public string MaNV { get; set; }
        public DateTime NgayLapHD { get; set; }
        public DateTime NgayNhanHang { get; set; }

        // Join display
        public string TenCty { get; set; }
        public string HoTenNV { get; set; }
        public string LoaiHD { get; set; }
    }
}