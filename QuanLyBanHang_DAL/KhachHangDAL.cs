using System.Collections.Generic;
using System.Data.SqlClient;
using QuanLyBanHang_DTO;

namespace QuanLyBanHang_DAL
{
    public class KhachHangDAL
    {
        public List<KhachHangDTO> GetAll()
        {
            var list = new List<KhachHangDTO>();
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT k.MaKH, k.TenCty, k.DiaChi, k.ThanhPho, k.DienThoai,
                           t.TenThanhPho
                    FROM KHACHHANG k
                    LEFT JOIN THANHPHO t ON t.ThanhPho = k.ThanhPho
                    ORDER BY k.MaKH", conn);
                var rd = cmd.ExecuteReader();
                while (rd.Read())
                    list.Add(Map(rd));
            }
            return list;
        }

        public KhachHangDTO GetByMa(string ma)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT k.MaKH, k.TenCty, k.DiaChi, k.ThanhPho, k.DienThoai,
                           t.TenThanhPho
                    FROM KHACHHANG k
                    LEFT JOIN THANHPHO t ON t.ThanhPho = k.ThanhPho
                    WHERE k.MaKH=@ma", conn);
                cmd.Parameters.AddWithValue("@ma", ma);
                var rd = cmd.ExecuteReader();
                return rd.Read() ? Map(rd) : null;
            }
        }

        public bool Insert(KhachHangDTO dto)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    INSERT INTO KHACHHANG(MaKH,TenCty,DiaChi,ThanhPho,DienThoai)
                    VALUES(@ma,@ten,@dc,@tp,@dt)", conn);
                AddParams(cmd, dto);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Update(KhachHangDTO dto)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    UPDATE KHACHHANG
                    SET TenCty=@ten, DiaChi=@dc, ThanhPho=@tp, DienThoai=@dt
                    WHERE MaKH=@ma", conn);
                AddParams(cmd, dto);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(string ma)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM KHACHHANG WHERE MaKH=@ma", conn);
                cmd.Parameters.AddWithValue("@ma", ma);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Exists(string ma)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT COUNT(*) FROM KHACHHANG WHERE MaKH=@ma", conn);
                cmd.Parameters.AddWithValue("@ma", ma);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        static void AddParams(SqlCommand cmd, KhachHangDTO dto)
        {
            cmd.Parameters.AddWithValue("@ma", dto.MaKH);
            cmd.Parameters.AddWithValue("@ten", dto.TenCty);
            cmd.Parameters.AddWithValue("@dc", dto.DiaChi ?? "");
            cmd.Parameters.AddWithValue("@tp", dto.ThanhPho ?? "");
            cmd.Parameters.AddWithValue("@dt", dto.DienThoai ?? "");
        }

        static KhachHangDTO Map(SqlDataReader rd) => new KhachHangDTO
        {
            MaKH = rd["MaKH"].ToString(),
            TenCty = rd["TenCty"].ToString(),
            DiaChi = rd["DiaChi"].ToString(),
            ThanhPho = rd["ThanhPho"].ToString(),
            DienThoai = rd["DienThoai"].ToString(),
            TenThanhPho = rd["TenThanhPho"].ToString()
        };
    }
}