using System.Collections.Generic;
using QuanLyBanHang_DAL;
using QuanLyBanHang_DTO;

namespace QuanLyBanHang_BUS
{
    // ══════════════════════════════════════════════════════════
    // ThanhPhoBUS
    // ══════════════════════════════════════════════════════════
    public class ThanhPhoBUS
    {
        private readonly ThanhPhoDAL _dal = new ThanhPhoDAL();

        public List<ThanhPhoDTO> GetAll() => _dal.GetAll();

        public ThanhPhoDTO GetByMa(string ma) => _dal.GetByMa(ma);

        public (bool ok, string msg) Insert(ThanhPhoDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ThanhPho))
                return (false, "Mã thành phố không được để trống.");
            if (string.IsNullOrWhiteSpace(dto.TenThanhPho))
                return (false, "Tên thành phố không được để trống.");
            if (_dal.Exists(dto.ThanhPho))
                return (false, "Mã thành phố đã tồn tại.");
            return _dal.Insert(dto)
                ? (true, "Thêm thành công!")
                : (false, "Thêm thất bại.");
        }

        public (bool ok, string msg) Update(ThanhPhoDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TenThanhPho))
                return (false, "Tên thành phố không được để trống.");
            return _dal.Update(dto)
                ? (true, "Cập nhật thành công!")
                : (false, "Cập nhật thất bại.");
        }

        public (bool ok, string msg) Delete(string ma)
        {
            if (!_dal.Exists(ma))
                return (false, "Không tìm thấy mã thành phố.");
            return _dal.Delete(ma)
                ? (true, "Xóa thành công!")
                : (false, "Không thể xóa. Có thể có khách hàng thuộc thành phố này.");
        }
    }

    // ══════════════════════════════════════════════════════════
    // KhachHangBUS
    // ══════════════════════════════════════════════════════════
    public class KhachHangBUS
    {
        private readonly KhachHangDAL _dal = new KhachHangDAL();

        public List<KhachHangDTO> GetAll() => _dal.GetAll();

        public KhachHangDTO GetByMa(string ma) => _dal.GetByMa(ma);

        public (bool ok, string msg) Insert(KhachHangDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.MaKH))
                return (false, "Mã khách hàng không được để trống.");
            if (string.IsNullOrWhiteSpace(dto.TenCty))
                return (false, "Tên công ty không được để trống.");
            if (_dal.Exists(dto.MaKH))
                return (false, "Mã khách hàng đã tồn tại.");
            return _dal.Insert(dto)
                ? (true, "Thêm khách hàng thành công!")
                : (false, "Thêm thất bại.");
        }

        public (bool ok, string msg) Update(KhachHangDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TenCty))
                return (false, "Tên công ty không được để trống.");
            return _dal.Update(dto)
                ? (true, "Cập nhật thành công!")
                : (false, "Cập nhật thất bại.");
        }

        public (bool ok, string msg) Delete(string ma)
        {
            if (!_dal.Exists(ma))
                return (false, "Không tìm thấy khách hàng.");
            return _dal.Delete(ma)
                ? (true, "Xóa thành công!")
                : (false, "Không thể xóa. Khách hàng có thể đang có hóa đơn.");
        }
    }
}