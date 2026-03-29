﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using QuanLyBanHang_DTO;

namespace QuanLyBanHang_DAL
{
    // ══════════════════════════════════════════════════════════
    // SanPhamDAL
    // ══════════════════════════════════════════════════════════
    public class SanPhamDAL
    {
        public List<SanPhamDTO> GetAll()
        {
            var list = new List<SanPhamDTO>();
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var rd = new SqlCommand("SELECT * FROM SANPHAM ORDER BY MaSP", conn).ExecuteReader();
                while (rd.Read()) list.Add(MapSP(rd));
            }
            return list;
        }

        public SanPhamDTO GetByMa(string ma)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM SANPHAM WHERE MaSP=@ma", conn);
                cmd.Parameters.AddWithValue("@ma", ma);
                var rd = cmd.ExecuteReader();
                return rd.Read() ? MapSP(rd) : null;
            }
        }

        public bool Insert(SanPhamDTO dto)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "INSERT INTO SANPHAM(MaSP,TenSP,DonViTinh,DonGia,Hinh) VALUES(@ma,@ten,@dvt,@gia,@hinh)", conn);
                AddParamsSP(cmd, dto);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Update(SanPhamDTO dto)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "UPDATE SANPHAM SET TenSP=@ten,DonViTinh=@dvt,DonGia=@gia,Hinh=@hinh WHERE MaSP=@ma", conn);
                AddParamsSP(cmd, dto);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(string ma)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM SANPHAM WHERE MaSP=@ma", conn);
                cmd.Parameters.AddWithValue("@ma", ma);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Exists(string ma)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT COUNT(*) FROM SANPHAM WHERE MaSP=@ma", conn);
                cmd.Parameters.AddWithValue("@ma", ma);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        static void AddParamsSP(SqlCommand cmd, SanPhamDTO dto)
        {
            cmd.Parameters.AddWithValue("@ma", dto.MaSP);
            cmd.Parameters.AddWithValue("@ten", dto.TenSP);
            cmd.Parameters.AddWithValue("@dvt", dto.DonViTinh ?? "");
            cmd.Parameters.AddWithValue("@gia", dto.DonGia);
            cmd.Parameters.AddWithValue("@hinh", string.IsNullOrEmpty(dto.Hinh) ? (object)DBNull.Value : dto.Hinh);
        }

        static SanPhamDTO MapSP(SqlDataReader rd) => new SanPhamDTO
        {
            MaSP = rd["MaSP"].ToString(),
            TenSP = rd["TenSP"].ToString(),
            DonViTinh = rd["DonViTinh"].ToString(),
            DonGia = Convert.ToDecimal(rd["DonGia"]),
            Hinh = rd["Hinh"].ToString()
        };
    }

    // ══════════════════════════════════════════════════════════
    // HoaDonDAL
    // ══════════════════════════════════════════════════════════
    public class HoaDonDAL
    {
        public List<HoaDonDTO> GetAll()
        {
            var list = new List<HoaDonDTO>();
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT h.*, k.TenCty,
                           nv.Ho + ' ' + nv.Ten AS HoTenNV
                    FROM HOADON h
                    LEFT JOIN KHACHHANG k  ON k.MaKH = h.MaKH
                    LEFT JOIN NHANVIEN  nv ON nv.MaNV = h.MaNV
                    ORDER BY h.NgayLapHD DESC", conn);
                var rd = cmd.ExecuteReader();
                while (rd.Read()) list.Add(MapHD(rd));
            }
            return list;
        }

        public HoaDonDTO GetByMa(string ma)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT h.*, k.TenCty,
                           nv.Ho + ' ' + nv.Ten AS HoTenNV
                    FROM HOADON h
                    LEFT JOIN KHACHHANG k  ON k.MaKH = h.MaKH
                    LEFT JOIN NHANVIEN  nv ON nv.MaNV = h.MaNV
                    WHERE h.MaHD=@ma", conn);
                cmd.Parameters.AddWithValue("@ma", ma);
                var rd = cmd.ExecuteReader();
                return rd.Read() ? MapHD(rd) : null;
            }
        }

        public bool Insert(HoaDonDTO dto)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                "INSERT INTO HOADON(MaHD,MaKH,MaNV,NgayLapHD,NgayNhanHang,LoaiHD) VALUES(@ma,@kh,@nv,@ngay,@nhan,@loai)", conn);
                AddParamsHD(cmd, dto);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Update(HoaDonDTO dto)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                "UPDATE HOADON SET MaKH=@kh,MaNV=@nv,NgayLapHD=@ngay,NgayNhanHang=@nhan,LoaiHD=@loai WHERE MaHD=@ma", conn);
                AddParamsHD(cmd, dto);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(string ma)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM HOADON WHERE MaHD=@ma", conn);
                cmd.Parameters.AddWithValue("@ma", ma);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Exists(string ma)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT COUNT(*) FROM HOADON WHERE MaHD=@ma", conn);
                cmd.Parameters.AddWithValue("@ma", ma);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        static void AddParamsHD(SqlCommand cmd, HoaDonDTO dto)
        {
            cmd.Parameters.AddWithValue("@ma", dto.MaHD);
            cmd.Parameters.AddWithValue("@kh", dto.MaKH);
            cmd.Parameters.AddWithValue("@nv", dto.MaNV);
            cmd.Parameters.AddWithValue("@ngay", dto.NgayLapHD);
            cmd.Parameters.AddWithValue("@nhan", dto.NgayNhanHang);
            
            var prop = dto.GetType().GetProperty("LoaiHD");
            cmd.Parameters.AddWithValue("@loai", prop != null ? prop.GetValue(dto) ?? "X" : "X");
        }

        static HoaDonDTO MapHD(SqlDataReader rd)
        {
            var dto = new HoaDonDTO
            {
                MaHD = rd["MaHD"].ToString(),
                MaKH = rd["MaKH"].ToString(),
                MaNV = rd["MaNV"].ToString(),
                NgayLapHD = Convert.ToDateTime(rd["NgayLapHD"]),
                NgayNhanHang = Convert.ToDateTime(rd["NgayNhanHang"]),
                TenCty = rd["TenCty"].ToString(),
                HoTenNV = rd["HoTenNV"].ToString()
            };
            var prop = dto.GetType().GetProperty("LoaiHD");
            if (prop != null) prop.SetValue(dto, rd["LoaiHD"].ToString());
            return dto;
        }
    }

    // ══════════════════════════════════════════════════════════
    // ChiTietHoaDonDAL
    // ══════════════════════════════════════════════════════════
    public class ChiTietHoaDonDAL
    {
        public List<ChiTietHoaDonDTO> GetByHoaDon(string maHD)
        {
            var list = new List<ChiTietHoaDonDTO>();
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT ct.MaHD, ct.MaSP, ct.SoLuong,
                           sp.TenSP, sp.DonViTinh, sp.DonGia
                    FROM CHITIETHOADON ct
                    JOIN SANPHAM sp ON sp.MaSP = ct.MaSP
                    WHERE ct.MaHD=@ma", conn);
                cmd.Parameters.AddWithValue("@ma", maHD);
                var rd = cmd.ExecuteReader();
                while (rd.Read()) list.Add(MapCT(rd));
            }
            return list;
        }

        public bool Insert(ChiTietHoaDonDTO dto)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "INSERT INTO CHITIETHOADON(MaHD,MaSP,SoLuong) VALUES(@hd,@sp,@sl)", conn);
                AddParamsCT(cmd, dto);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Update(ChiTietHoaDonDTO dto)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "UPDATE CHITIETHOADON SET SoLuong=@sl WHERE MaHD=@hd AND MaSP=@sp", conn);
                AddParamsCT(cmd, dto);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(string maHD, string maSP)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM CHITIETHOADON WHERE MaHD=@hd AND MaSP=@sp", conn);
                cmd.Parameters.AddWithValue("@hd", maHD);
                cmd.Parameters.AddWithValue("@sp", maSP);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool DeleteByHoaDon(string maHD)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM CHITIETHOADON WHERE MaHD=@hd", conn);
                cmd.Parameters.AddWithValue("@hd", maHD);
                return cmd.ExecuteNonQuery() >= 0;
            }
        }

        static void AddParamsCT(SqlCommand cmd, ChiTietHoaDonDTO dto)
        {
            cmd.Parameters.AddWithValue("@hd", dto.MaHD);
            cmd.Parameters.AddWithValue("@sp", dto.MaSP);
            cmd.Parameters.AddWithValue("@sl", dto.SoLuong);
        }

        static ChiTietHoaDonDTO MapCT(SqlDataReader rd) => new ChiTietHoaDonDTO
        {
            MaHD = rd["MaHD"].ToString(),
            MaSP = rd["MaSP"].ToString(),
            SoLuong = Convert.ToInt32(rd["SoLuong"]),
            TenSP = rd["TenSP"].ToString(),
            DonViTinh = rd["DonViTinh"].ToString(),
            DonGia = Convert.ToDecimal(rd["DonGia"])
        };
    }
}