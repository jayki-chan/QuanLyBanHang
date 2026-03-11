namespace QuanLyBanHang_DTO
{
    public class KhachHangDTO
    {
        public string MaKH { get; set; }
        public string TenCty { get; set; }
        public string DiaChi { get; set; }
        public string ThanhPho { get; set; }
        public string DienThoai { get; set; }

        // Join display
        public string TenThanhPho { get; set; }
    }
}