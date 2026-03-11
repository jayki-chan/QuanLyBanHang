namespace QuanLyBanHang_DTO
{
    public class ChiTietHoaDonDTO
    {
        public string MaHD { get; set; }
        public string MaSP { get; set; }
        public int SoLuong { get; set; }

        // Join display
        public string TenSP { get; set; }
        public string DonViTinh { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;
    }
}