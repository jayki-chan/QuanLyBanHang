namespace QuanLyNhanVien.Models
{
    public class UserModel
    {
        public int    UserID      { get; set; }
        public string Username    { get; set; }
        public string HoTen       { get; set; }
        public string Role        { get; set; }   // ADMIN | MANAGER | HR_STAFF | EMPLOYEE
        public string Email       { get; set; }
        public string MaNhanVien  { get; set; }
        public bool   IsActive    { get; set; }
        public bool   IsLocked    { get; set; }
        public bool   FirstLogin  { get; set; }
        public int    RetryCount  { get; set; }
    }
}
