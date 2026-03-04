using System;
using System.Data;
using System.Data.SqlClient;
using QuanLyNhanVien.Models;

namespace QuanLyNhanVien.DAL
{
    public class UserDAL
    {
        // ── A1: Xác thực tài khoản ────────────────────────────────────
        public UserModel AuthLogin(string username, string passwordHash)
        {
            using (var conn = DatabaseConnection.GetConnection())
            using (var cmd = new SqlCommand("sp_AuthLogin", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username",     username);
                cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return null;
                    return MapUser(reader);
                }
            }
        }

        // ── A3: Tăng retry, kiểm tra lock ─────────────────────────────
        public (int retryCount, bool isLocked) IncrementRetry(string username, int maxRetry = 3)
        {
            using (var conn = DatabaseConnection.GetConnection())
            using (var cmd = new SqlCommand("sp_IncrementRetry", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@MaxRetry", maxRetry);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return (0, false);
                    return (
                        reader.GetInt32(reader.GetOrdinal("RetryCount")),
                        reader.GetBoolean(reader.GetOrdinal("IsLocked"))
                    );
                }
            }
        }

        // ── A2: Reset retry sau đăng nhập thành công ──────────────────
        public void ResetRetry(int userID)
        {
            using (var conn = DatabaseConnection.GetConnection())
            using (var cmd = new SqlCommand("sp_ResetRetry", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID", userID);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // ── Ghi log đăng nhập ─────────────────────────────────────────
        public int LogLogin(int userID, string ipAddress, bool isSuccess, string note = null)
        {
            using (var conn = DatabaseConnection.GetConnection())
            using (var cmd = new SqlCommand("sp_LogLogin", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID",    userID);
                cmd.Parameters.AddWithValue("@IPAddress", (object)ipAddress ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsSuccess", isSuccess);
                cmd.Parameters.AddWithValue("@Note",      (object)note ?? DBNull.Value);

                conn.Open();
                var result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }

        // ── Ghi logout ────────────────────────────────────────────────
        public void LogLogout(int logID)
        {
            using (var conn = DatabaseConnection.GetConnection())
            using (var cmd = new SqlCommand("sp_LogLogout", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@LogID", logID);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // ── A6: Đổi mật khẩu lần đầu ─────────────────────────────────
        public bool ChangePasswordFirstLogin(int userID, string newPwHash)
        {
            using (var conn = DatabaseConnection.GetConnection())
            using (var cmd = new SqlCommand("sp_ChangePasswordFirstLogin", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID",    userID);
                cmd.Parameters.AddWithValue("@NewPwHash", newPwHash);

                conn.Open();
                var affected = cmd.ExecuteScalar();
                return affected != null && Convert.ToInt32(affected) > 0;
            }
        }

        // ── A5: Tìm user theo email ────────────────────────────────────
        public UserModel FindUserByEmail(string email)
        {
            using (var conn = DatabaseConnection.GetConnection())
            using (var cmd = new SqlCommand("sp_FindUserByEmail", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Email", email);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return null;
                    return new UserModel
                    {
                        UserID    = reader.GetInt32(reader.GetOrdinal("UserID")),
                        Username  = reader["Username"].ToString(),
                        HoTen     = reader["HoTen"].ToString(),
                        Email     = reader["Email"].ToString(),
                        IsActive  = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                        IsLocked  = reader.GetBoolean(reader.GetOrdinal("IsLocked"))
                    };
                }
            }
        }

        // ── A7: Tạo OTP ───────────────────────────────────────────────
        public int CreateOTP(int userID, string otpCode, int minutes = 10)
        {
            using (var conn = DatabaseConnection.GetConnection())
            using (var cmd = new SqlCommand("sp_CreateOTP", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID",  userID);
                cmd.Parameters.AddWithValue("@OtpCode", otpCode);
                cmd.Parameters.AddWithValue("@Minutes", minutes);

                conn.Open();
                var result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }

        // ── A7: Xác thực OTP ──────────────────────────────────────────
        public bool VerifyOTP(int userID, string otpCode)
        {
            using (var conn = DatabaseConnection.GetConnection())
            using (var cmd = new SqlCommand("sp_VerifyOTP", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID",  userID);
                cmd.Parameters.AddWithValue("@OtpCode", otpCode);

                conn.Open();
                var result = cmd.ExecuteScalar();
                return result != null && Convert.ToInt32(result) == 1;
            }
        }

        // ── Reset mật khẩu sau OTP ────────────────────────────────────
        public bool ResetPassword(int userID, string newPwHash)
        {
            using (var conn = DatabaseConnection.GetConnection())
            using (var cmd = new SqlCommand("sp_ResetPassword", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID",    userID);
                cmd.Parameters.AddWithValue("@NewPwHash", newPwHash);

                conn.Open();
                var affected = cmd.ExecuteScalar();
                return affected != null && Convert.ToInt32(affected) > 0;
            }
        }

        // ── Helper: Map DataReader → UserModel ────────────────────────
        private UserModel MapUser(SqlDataReader r)
        {
            return new UserModel
            {
                UserID     = r.GetInt32(r.GetOrdinal("UserID")),
                Username   = r["Username"].ToString(),
                HoTen      = r["HoTen"].ToString(),
                Role       = r["Role"].ToString(),
                Email      = r["Email"].ToString(),
                MaNhanVien = r["MaNhanVien"] == DBNull.Value ? null : r["MaNhanVien"].ToString(),
                IsActive   = r.GetBoolean(r.GetOrdinal("IsActive")),
                IsLocked   = r.GetBoolean(r.GetOrdinal("IsLocked")),
                FirstLogin = r.GetBoolean(r.GetOrdinal("FirstLogin")),
                RetryCount = r.GetInt32(r.GetOrdinal("RetryCount"))
            };
        }
    }
}
