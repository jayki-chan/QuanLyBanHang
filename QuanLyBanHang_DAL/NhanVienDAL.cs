using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using QuanLyBanHang_DTO;

namespace QuanLyBanHang_DAL
{
    public class NhanVienDAL
    {
        public List<NhanVienDTO> GetAll()
        {
            var list = new List<NhanVienDTO>();
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM NHANVIEN ORDER BY MaNV", conn);
                var rd = cmd.ExecuteReader();
                while (rd.Read()) list.Add(Map(rd));
            }
            return list;
        }

        public NhanVienDTO GetByMa(string ma)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM NHANVIEN WHERE MaNV=@ma", conn);
                cmd.Parameters.AddWithValue("@ma", ma);
                var rd = cmd.ExecuteReader();
                return rd.Read() ? Map(rd) : null;
            }
        }

        public NhanVienDTO Login(string username, string matkhau)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "SELECT * FROM NHANVIEN WHERE Username=@u AND Matkhau=@p", conn);
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", matkhau);
                var rd = cmd.ExecuteReader();
                return rd.Read() ? Map(rd) : null;
            }
        }

        public bool Insert(NhanVienDTO dto)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    INSERT INTO NHANVIEN(MaNV,Ho,Ten,Nu,NgayNV,DiaChi,DienThoai,Hình,Username,Matkhau,Role)
                    VALUES(@ma,@ho,@ten,@nu,@ngay,@dc,@dt,@hinh,@user,@pass,@role)", conn);
                AddParams(cmd, dto, true);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Update(NhanVienDTO dto, bool updatePass = false)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                string sql = updatePass
                    ? "UPDATE NHANVIEN SET Ho=@ho,Ten=@ten,Nu=@nu,NgayNV=@ngay,DiaChi=@dc,DienThoai=@dt,Hình=@hinh,Username=@user,Matkhau=@pass,Role=@role WHERE MaNV=@ma"
                    : "UPDATE NHANVIEN SET Ho=@ho,Ten=@ten,Nu=@nu,NgayNV=@ngay,DiaChi=@dc,DienThoai=@dt,Hình=@hinh,Username=@user,Role=@role WHERE MaNV=@ma";
                var cmd = new SqlCommand(sql, conn);
                AddParams(cmd, dto, updatePass);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(string ma)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM NHANVIEN WHERE MaNV=@ma", conn);
                cmd.Parameters.AddWithValue("@ma", ma);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Exists(string ma)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT COUNT(*) FROM NHANVIEN WHERE MaNV=@ma", conn);
                cmd.Parameters.AddWithValue("@ma", ma);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        public bool UsernameExists(string username, string excludeMaNV = null)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                string sql = excludeMaNV == null
                    ? "SELECT COUNT(*) FROM NHANVIEN WHERE Username=@u"
                    : "SELECT COUNT(*) FROM NHANVIEN WHERE Username=@u AND MaNV<>@ma";
                var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@u", username);
                if (excludeMaNV != null) cmd.Parameters.AddWithValue("@ma", excludeMaNV);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        static void AddParams(SqlCommand cmd, NhanVienDTO dto, bool includePass)
        {
            cmd.Parameters.AddWithValue("@ma", dto.MaNV);
            cmd.Parameters.AddWithValue("@ho", dto.Ho ?? "");
            cmd.Parameters.AddWithValue("@ten", dto.Ten ?? "");
            cmd.Parameters.AddWithValue("@nu", dto.Nu ? 1 : 0);
            cmd.Parameters.AddWithValue("@ngay", dto.NgayNV);
            cmd.Parameters.AddWithValue("@dc", dto.DiaChi ?? "");
            cmd.Parameters.AddWithValue("@dt", dto.DienThoai ?? "");
            cmd.Parameters.AddWithValue("@hinh", string.IsNullOrEmpty(dto.Hinh) ? (object)System.DBNull.Value : dto.Hinh);
            cmd.Parameters.AddWithValue("@user", dto.Username ?? "");
            cmd.Parameters.AddWithValue("@role", dto.Role ?? "user");
            if (includePass) cmd.Parameters.AddWithValue("@pass", dto.Matkhau ?? "");
        }

        static NhanVienDTO Map(SqlDataReader rd) => new NhanVienDTO
        {
            MaNV = rd["MaNV"].ToString(),
            Ho = rd["Ho"].ToString(),
            Ten = rd["Ten"].ToString(),
            Nu = Convert.ToBoolean(rd["Nu"]),
            NgayNV = rd["NgayNV"] == DBNull.Value ? DateTime.Today : Convert.ToDateTime(rd["NgayNV"]),
            DiaChi = rd["DiaChi"].ToString(),
            DienThoai = rd["DienThoai"].ToString(),
            Hinh = rd["Hình"].ToString(),
            Username = rd["Username"].ToString(),
            Matkhau = rd["Matkhau"].ToString(),
            Role = rd.GetOrdinal("Role") >= 0 ? rd["Role"].ToString() : "user"
        };
    }
}