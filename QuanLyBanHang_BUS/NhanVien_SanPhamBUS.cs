using System;
using System.Collections.Generic;
using QuanLyBanHang_DAL;
using QuanLyBanHang_DTO;

namespace QuanLyBanHang_BUS
{
    // ══════════════════════════════════════════════════════════
    // NhanVienBUS
    // ══════════════════════════════════════════════════════════
    public class NhanVienBUS
    {
        private readonly NhanVienDAL _dal = new NhanVienDAL();

        public List<NhanVienDTO> GetAll() => _dal.GetAll();

        public NhanVienDTO GetByMa(string ma) => _dal.GetByMa(ma);

        public NhanVienDTO Login(string username, string matkhau)
            => _dal.Login(username, matkhau);

        public (bool ok, string msg) Insert(NhanVienDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.MaNV))
                return (false, "Mã nhân viên không được để trống.");
            if (string.IsNullOrWhiteSpace(dto.Ho) || string.IsNullOrWhiteSpace(dto.Ten))
                return (false, "Họ và Tên không được để trống.");
            if (string.IsNullOrWhiteSpace(dto.Username))
                return (false, "Username không được để trống.");
            if (string.IsNullOrWhiteSpace(dto.Matkhau))
                return (false, "Mật khẩu không được để trống khi thêm mới.");
            if (_dal.Exists(dto.MaNV))
                return (false, "Mã nhân viên đã tồn tại.");
            if (_dal.UsernameExists(dto.Username))
                return (false, "Username đã được sử dụng.");
            return _dal.Insert(dto)
                ? (true, "Thêm nhân viên thành công!")
                : (false, "Thêm thất bại.");
        }

        public (bool ok, string msg) Update(NhanVienDTO dto, bool updatePass = false)
        {
            if (string.IsNullOrWhiteSpace(dto.Ho) || string.IsNullOrWhiteSpace(dto.Ten))
                return (false, "Họ và Tên không được để trống.");
            if (string.IsNullOrWhiteSpace(dto.Username))
                return (false, "Username không được để trống.");
            if (_dal.UsernameExists(dto.Username, dto.MaNV))
                return (false, "Username đã được sử dụng bởi nhân viên khác.");
            if (updatePass && string.IsNullOrWhiteSpace(dto.Matkhau))
                return (false, "Mật khẩu mới không được để trống.");
            return _dal.Update(dto, updatePass)
                ? (true, "Cập nhật thành công!")
                : (false, "Cập nhật thất bại.");
        }

        public (bool ok, string msg) Delete(string ma)
        {
            if (!_dal.Exists(ma))
                return (false, "Không tìm thấy nhân viên.");
            return _dal.Delete(ma)
                ? (true, "Xóa thành công!")
                : (false, "Không thể xóa. Nhân viên có thể đang có hóa đơn.");
        }

        public (bool ok, string msg) UpdateUsername(string maNV, string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return (false, "Username không được để trống.");
            if (_dal.UsernameExists(username, maNV))
                return (false, "Username đã được sử dụng bởi nhân viên khác.");
            return _dal.UpdateUsername(maNV, username)
                ? (true, "Đã cập nhật Username thành công!")
                : (false, "Cập nhật thất bại.");
        }

        public (bool ok, string msg) SetRole(string maNV, string role)
        {
            if (role != "admin" && role != "user")
                return (false, "Giá trị quyền không hợp lệ (admin/user).");
            return _dal.SetRole(maNV, role)
                ? (true, "Đã cập nhật quyền thành công!")
                : (false, "Cập nhật quyền thất bại.");
        }

        public (bool ok, string msg) ResetPassword(string maNV, string newPass)
        {
            if (string.IsNullOrWhiteSpace(newPass) || newPass.Length < 6)
                return (false, "Mật khẩu mới phải từ 6 ký tự trở lên.");
            return _dal.ResetPassword(maNV, newPass)
                ? (true, "Đã đặt lại mật khẩu thành công!")
                : (false, "Đặt lại mật khẩu thất bại.");
        }

        public (bool ok, string msg) ChangePassword(string username, string oldPass, string newPass)
        {
            if (_dal.Login(username, oldPass) == null)
                return (false, "Mật khẩu hiện tại không đúng.");
            if (string.IsNullOrWhiteSpace(newPass) || newPass.Length < 6)
                return (false, "Mật khẩu mới phải từ 6 ký tự trở lên.");
            if (oldPass == newPass)
                return (false, "Mật khẩu mới phải khác mật khẩu cũ.");
            return _dal.ChangePasswordByUsername(username, newPass)
                ? (true, "Đổi mật khẩu thành công!\nVui lòng dùng mật khẩu mới cho lần đăng nhập sau.")
                : (false, "Đổi mật khẩu thất bại.");
        }

        public (bool ok, string msg) DeleteAccount(string maNV)
        {
            if (!_dal.Exists(maNV))
                return (false, "Không tìm thấy nhân viên.");
            return _dal.DeleteAccount(maNV)
                ? (true, "Đã xóa tài khoản thành công!")
                : (false, "Xóa tài khoản thất bại.");
        }

        /// <summary>Đánh giá độ mạnh mật khẩu: 0=Yếu, 1=Trung bình, 2=Mạnh</summary>
        public int PasswordStrength(string pass)
        {
            if (string.IsNullOrEmpty(pass) || pass.Length < 6) return 0;
            bool hasUpper = System.Text.RegularExpressions.Regex.IsMatch(pass, "[A-Z]");
            bool hasLower = System.Text.RegularExpressions.Regex.IsMatch(pass, "[a-z]");
            bool hasDigit = System.Text.RegularExpressions.Regex.IsMatch(pass, "[0-9]");
            bool hasSymbol = System.Text.RegularExpressions.Regex.IsMatch(pass, "[^a-zA-Z0-9]");
            int score = (hasUpper ? 1 : 0) + (hasLower ? 1 : 0) + (hasDigit ? 1 : 0) + (hasSymbol ? 1 : 0);
            if (score <= 1) return 0;
            if (score == 2) return 1;
            return 2;
        }
    }

    // ══════════════════════════════════════════════════════════
    // SanPhamBUS
    // ══════════════════════════════════════════════════════════
    public class SanPhamBUS
    {
        private readonly SanPhamDAL _dal = new SanPhamDAL();

        public List<SanPhamDTO> GetAll() => _dal.GetAll();

        public SanPhamDTO GetByMa(string ma) => _dal.GetByMa(ma);

        public (bool ok, string msg) Insert(SanPhamDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.MaSP))
                return (false, "Mã sản phẩm không được để trống.");
            if (string.IsNullOrWhiteSpace(dto.TenSP))
                return (false, "Tên sản phẩm không được để trống.");
            if (dto.DonGia < 0)
                return (false, "Đơn giá không được âm.");
            if (_dal.Exists(dto.MaSP))
                return (false, "Mã sản phẩm đã tồn tại.");
            return _dal.Insert(dto)
                ? (true, "Thêm sản phẩm thành công!")
                : (false, "Thêm thất bại.");
        }

        public (bool ok, string msg) Update(SanPhamDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TenSP))
                return (false, "Tên sản phẩm không được để trống.");
            if (dto.DonGia < 0)
                return (false, "Đơn giá không được âm.");
            return _dal.Update(dto)
                ? (true, "Cập nhật thành công!")
                : (false, "Cập nhật thất bại.");
        }

        public (bool ok, string msg) Delete(string ma)
        {
            if (!_dal.Exists(ma))
                return (false, "Không tìm thấy sản phẩm.");
            return _dal.Delete(ma)
                ? (true, "Xóa thành công!")
                : (false, "Không thể xóa. Sản phẩm đang có trong hóa đơn.");
        }
    }
}