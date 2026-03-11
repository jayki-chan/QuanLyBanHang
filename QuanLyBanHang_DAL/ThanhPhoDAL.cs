using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QuanLyBanHang_DTO;

namespace QuanLyBanHang_DAL
{
    public class ThanhPhoDAL
    {
        public List<ThanhPhoDTO> GetAll()
        {
            var list = new List<ThanhPhoDTO>();
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT ThanhPho, TenThanhPho FROM THANHPHO ORDER BY TenThanhPho", conn);
                var rd = cmd.ExecuteReader();
                while (rd.Read())
                    list.Add(new ThanhPhoDTO
                    {
                        ThanhPho = rd["ThanhPho"].ToString(),
                        TenThanhPho = rd["TenThanhPho"].ToString()
                    });
            }
            return list;
        }

        public ThanhPhoDTO GetByMa(string ma)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT ThanhPho, TenThanhPho FROM THANHPHO WHERE ThanhPho=@ma", conn);
                cmd.Parameters.AddWithValue("@ma", ma);
                var rd = cmd.ExecuteReader();
                if (rd.Read())
                    return new ThanhPhoDTO { ThanhPho = rd["ThanhPho"].ToString(), TenThanhPho = rd["TenThanhPho"].ToString() };
            }
            return null;
        }

        public bool Insert(ThanhPhoDTO dto)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("INSERT INTO THANHPHO(ThanhPho,TenThanhPho) VALUES(@ma,@ten)", conn);
                cmd.Parameters.AddWithValue("@ma", dto.ThanhPho);
                cmd.Parameters.AddWithValue("@ten", dto.TenThanhPho);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Update(ThanhPhoDTO dto)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE THANHPHO SET TenThanhPho=@ten WHERE ThanhPho=@ma", conn);
                cmd.Parameters.AddWithValue("@ten", dto.TenThanhPho);
                cmd.Parameters.AddWithValue("@ma", dto.ThanhPho);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(string ma)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM THANHPHO WHERE ThanhPho=@ma", conn);
                cmd.Parameters.AddWithValue("@ma", ma);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Exists(string ma)
        {
            using (var conn = DBConnection.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT COUNT(*) FROM THANHPHO WHERE ThanhPho=@ma", conn);
                cmd.Parameters.AddWithValue("@ma", ma);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }
    }
}