using System;
using System.Collections.Generic;
using QuanLyBanHang_DAL;
using QuanLyBanHang_DTO;

namespace QuanLyBanHang_BUS
{
    // ══════════════════════════════════════════════════════════
    // HoaDonBUS
    // ══════════════════════════════════════════════════════════
    public class HoaDonBUS
    {
        private readonly HoaDonDAL _dal = new HoaDonDAL();
        private readonly ChiTietHoaDonDAL _ctDal = new ChiTietHoaDonDAL();

        public List<HoaDonDTO> GetAll() => _dal.GetAll();

        public HoaDonDTO GetByMa(string ma) => _dal.GetByMa(ma);

        public List<ChiTietHoaDonDTO> GetChiTiet(string maHD)
            => _ctDal.GetByHoaDon(maHD);

        public (bool ok, string msg) Insert(HoaDonDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.MaHD))
                return (false, "Mã hóa đơn không được để trống.");
            if (string.IsNullOrWhiteSpace(dto.MaKH))
                return (false, "Phải chọn khách hàng.");
            if (string.IsNullOrWhiteSpace(dto.MaNV))
                return (false, "Phải chọn nhân viên.");
            if (dto.NgayNhanHang < dto.NgayLapHD)
                return (false, "Ngày nhận hàng không được trước ngày lập hóa đơn.");
            if (_dal.Exists(dto.MaHD))
                return (false, "Mã hóa đơn đã tồn tại.");
            return _dal.Insert(dto)
                ? (true, "Thêm hóa đơn thành công!")
                : (false, "Thêm thất bại.");
        }

        public (bool ok, string msg) Update(HoaDonDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.MaKH))
                return (false, "Phải chọn khách hàng.");
            if (dto.NgayNhanHang < dto.NgayLapHD)
                return (false, "Ngày nhận hàng không được trước ngày lập hóa đơn.");
            return _dal.Update(dto)
                ? (true, "Cập nhật thành công!")
                : (false, "Cập nhật thất bại.");
        }

        public (bool ok, string msg) Delete(string ma)
        {
            if (!_dal.Exists(ma))
                return (false, "Không tìm thấy hóa đơn.");
            // Xóa chi tiết trước
            _ctDal.DeleteByHoaDon(ma);
            return _dal.Delete(ma)
                ? (true, "Xóa hóa đơn thành công!")
                : (false, "Xóa thất bại.");
        }

        /// <summary>Tính tổng tiền 1 hóa đơn</summary>
        public decimal TinhTong(string maHD)
        {
            decimal total = 0;
            foreach (var ct in _ctDal.GetByHoaDon(maHD))
                total += ct.ThanhTien;
            return total;
        }
    }

    // ══════════════════════════════════════════════════════════
    // ChiTietHoaDonBUS
    // ══════════════════════════════════════════════════════════
    public class ChiTietHoaDonBUS
    {
        private readonly ChiTietHoaDonDAL _dal = new ChiTietHoaDonDAL();

        public List<ChiTietHoaDonDTO> GetByHoaDon(string maHD)
            => _dal.GetByHoaDon(maHD);

        public (bool ok, string msg) Insert(ChiTietHoaDonDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.MaHD))
                return (false, "Mã hóa đơn không hợp lệ.");
            if (string.IsNullOrWhiteSpace(dto.MaSP))
                return (false, "Phải chọn sản phẩm.");
            if (dto.SoLuong <= 0)
                return (false, "Số lượng phải lớn hơn 0.");
            return _dal.Insert(dto)
                ? (true, "Thêm chi tiết thành công!")
                : (false, "Thêm thất bại.");
        }

        public (bool ok, string msg) Update(ChiTietHoaDonDTO dto)
        {
            if (dto.SoLuong <= 0)
                return (false, "Số lượng phải lớn hơn 0.");
            return _dal.Update(dto)
                ? (true, "Cập nhật thành công!")
                : (false, "Cập nhật thất bại.");
        }

        public (bool ok, string msg) Delete(string maHD, string maSP)
            => _dal.Delete(maHD, maSP)
                ? (true, "Xóa thành công!")
                : (false, "Xóa thất bại.");
    }
}