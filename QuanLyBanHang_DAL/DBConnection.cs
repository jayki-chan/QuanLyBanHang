using System.Data.SqlClient;

namespace QuanLyBanHang_DAL
{
    public class DBConnection
    {
        private static string _connStr =
            "Data Source=localhost; Database=quanlybanhang1; Integrated Security=True; TrustServerCertificate=True;";

        public static SqlConnection GetConnection()
            => new SqlConnection(_connStr);

        public static void SetConnectionString(string connStr)
            => _connStr = connStr;
    }
}