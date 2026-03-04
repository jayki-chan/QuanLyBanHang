using System.Configuration;
using System.Data.SqlClient;

namespace QuanLyNhanVien.DAL
{
    public static class DatabaseConnection
    {
        private static string _connectionString;

        public static string ConnectionString
        {
            get
            {
                if (_connectionString == null)
                    _connectionString = ConfigurationManager.ConnectionStrings["QuanLyNhanVienDB"]?.ConnectionString
                        ?? "Server=.;Database=QuanLyNhanVien;Trusted_Connection=True;";
                return _connectionString;
            }
        }

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
