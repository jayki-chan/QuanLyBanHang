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

        /// <summary>Hash mật khẩu bằng SHA-256 (lowercase hex).</summary>
        public static string HashPassword(string password)
        {
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password ?? "");
                var hash  = sha.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        public NhanVienDTO Login(string username, string matkhau)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                string hashed = HashPassword(matkhau);
                var cmd = new SqlCommand(
                    "SELECT * FROM NHANVIEN WHERE Username=@u AND Matkhau=@p", conn);
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", hashed);
                var rd = cmd.ExecuteReader();
                return rd.Read() ? Map(rd) : null;
            }
        }

        public bool Insert(NhanVienDTO dto)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                // Hash mật khẩu trước khi lưu
                if (!string.IsNullOrEmpty(dto.Matkhau))
                    dto.Matkhau = HashPassword(dto.Matkhau);
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
                // Hash mật khẩu nếu có cập nhật
                if (updatePass && !string.IsNullOrEmpty(dto.Matkhau))
                    dto.Matkhau = HashPassword(dto.Matkhau);
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

        public bool UpdateUsername(string maNV, string username)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE NHANVIEN SET Username=@u WHERE MaNV=@ma", conn);
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@ma", maNV);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool SetRole(string maNV, string role)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE NHANVIEN SET Role=@r WHERE MaNV=@ma", conn);
                cmd.Parameters.AddWithValue("@r", role);
                cmd.Parameters.AddWithValue("@ma", maNV);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool ResetPassword(string maNV, string newPass)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE NHANVIEN SET Matkhau=@p WHERE MaNV=@ma", conn);
                cmd.Parameters.AddWithValue("@p", HashPassword(newPass));
                cmd.Parameters.AddWithValue("@ma", maNV);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool ChangePasswordByUsername(string username, string newPass)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE NHANVIEN SET Matkhau=@p WHERE Username=@u", conn);
                cmd.Parameters.AddWithValue("@p", HashPassword(newPass));
                cmd.Parameters.AddWithValue("@u", username);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool DeleteAccount(string maNV)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE NHANVIEN SET Username=NULL, Matkhau=NULL WHERE MaNV=@ma", conn);
                cmd.Parameters.AddWithValue("@ma", maNV);
                return cmd.ExecuteNonQuery() > 0;
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
            Role = HasColumn(rd, "Role") ? rd["Role"].ToString() : "user"
        };

        static bool HasColumn(SqlDataReader rd, string col)
        {
            for (int i = 0; i < rd.FieldCount; i++)
                if (string.Equals(rd.GetName(i), col, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }
    }
}