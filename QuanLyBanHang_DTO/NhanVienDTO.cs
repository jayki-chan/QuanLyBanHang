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
        /// <summary>
        /// Cấp bậc: "admin" | "sales" | "warehouse"
        /// </summary>
        public string Role { get; set; } = "sales";

        public string HoTen => Ho + " " + Ten;

        /// <summary>Tên hiển thị theo cấp bậc.</summary>
        public string RoleDisplay
        {
            get
            {
                switch ((Role ?? "").ToLower())
                {
                    case "admin":     return "Quản trị viên";
                    case "sales":     return "Nhân viên bán hàng";
                    case "warehouse": return "Nhân viên kho hàng";
                    default:          return Role ?? "—";
                }
            }
        }
    }
}