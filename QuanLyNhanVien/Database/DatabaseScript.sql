-- ============================================================
-- QUẢN LÝ NHÂN VIÊN - DATABASE SCRIPT
-- SQL Server 2019+
-- Phase 1: Authentication Tables
-- ============================================================

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'QuanLyNhanVien')
    CREATE DATABASE QuanLyNhanVien;
GO

USE QuanLyNhanVien;
GO

-- ── BẢNG PHÒNG BAN ─────────────────────────────────────────
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PhongBan]'))
CREATE TABLE PhongBan (
    MaPhongBan  INT           IDENTITY(1,1) PRIMARY KEY,
    TenPhongBan NVARCHAR(100) NOT NULL,
    MoTa        NVARCHAR(255) NULL,
    IsActive    BIT           NOT NULL DEFAULT 1,
    NgayTao     DATETIME      NOT NULL DEFAULT GETDATE()
);
GO

-- ── BẢNG NHÂN VIÊN ─────────────────────────────────────────
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NhanVien]'))
CREATE TABLE NhanVien (
    MaNhanVien  NVARCHAR(20)  PRIMARY KEY,
    HoTen       NVARCHAR(100) NOT NULL,
    NgaySinh    DATE          NULL,
    GioiTinh    NVARCHAR(10)  NULL,
    DiaChi      NVARCHAR(255) NULL,
    SoDienThoai NVARCHAR(15)  NULL,
    Email       NVARCHAR(100) NULL,
    MaPhongBan  INT           NULL REFERENCES PhongBan(MaPhongBan),
    ChucVu      NVARCHAR(100) NULL,
    LuongCoBan  DECIMAL(15,2) NOT NULL DEFAULT 0,
    NgayVaoLam  DATE          NULL,
    NgayNghiViec DATE         NULL,
    IsActive    BIT           NOT NULL DEFAULT 1,
    NgayTao     DATETIME      NOT NULL DEFAULT GETDATE(),
    NgayCapNhat DATETIME      NULL
);
GO

-- ── BẢNG NGƯỜI DÙNG ────────────────────────────────────────
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]'))
CREATE TABLE Users (
    UserID      INT           IDENTITY(1,1) PRIMARY KEY,
    Username    NVARCHAR(50)  NOT NULL UNIQUE,
    PasswordHash NVARCHAR(256) NOT NULL,         -- BCrypt / SHA256
    Email       NVARCHAR(100) NOT NULL,
    HoTen       NVARCHAR(100) NOT NULL,
    Role        NVARCHAR(20)  NOT NULL            -- ADMIN | MANAGER | HR_STAFF | EMPLOYEE
                    CHECK (Role IN ('ADMIN','MANAGER','HR_STAFF','EMPLOYEE')),
    MaNhanVien  NVARCHAR(20)  NULL REFERENCES NhanVien(MaNhanVien),
    IsActive    BIT           NOT NULL DEFAULT 1,
    IsLocked    BIT           NOT NULL DEFAULT 0,
    FirstLogin  BIT           NOT NULL DEFAULT 1, -- Bắt buộc đổi MK lần đầu
    RetryCount  INT           NOT NULL DEFAULT 0,
    LastLogin   DATETIME      NULL,
    NgayTao     DATETIME      NOT NULL DEFAULT GETDATE(),
    NgayCapNhat DATETIME      NULL
);
GO

-- ── BẢNG LỊCH SỬ ĐĂNG NHẬP ────────────────────────────────
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LoginHistory]'))
CREATE TABLE LoginHistory (
    LogID       INT           IDENTITY(1,1) PRIMARY KEY,
    UserID      INT           NOT NULL REFERENCES Users(UserID),
    LoginTime   DATETIME      NOT NULL DEFAULT GETDATE(),
    LogoutTime  DATETIME      NULL,
    IPAddress   NVARCHAR(50)  NULL,
    IsSuccess   BIT           NOT NULL DEFAULT 1,
    Note        NVARCHAR(255) NULL
);
GO

-- ── BẢNG OTP RESET MẬT KHẨU ───────────────────────────────
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OtpTokens]'))
CREATE TABLE OtpTokens (
    TokenID     INT           IDENTITY(1,1) PRIMARY KEY,
    UserID      INT           NOT NULL REFERENCES Users(UserID),
    OtpCode     NVARCHAR(10)  NOT NULL,
    ExpireTime  DATETIME      NOT NULL,
    IsUsed      BIT           NOT NULL DEFAULT 0,
    CreatedAt   DATETIME      NOT NULL DEFAULT GETDATE()
);
GO

-- ============================================================
-- DỮ LIỆU MẪU
-- ============================================================

-- Phòng ban
INSERT INTO PhongBan (TenPhongBan, MoTa) VALUES
    (N'Ban Giám Đốc',        N'Lãnh đạo công ty'),
    (N'Phòng Nhân Sự',       N'Quản lý nhân sự'),
    (N'Phòng Kế Toán',       N'Tài chính kế toán'),
    (N'Phòng Công Nghệ',     N'Phát triển phần mềm'),
    (N'Phòng Kinh Doanh',    N'Bán hàng & Marketing');
GO

-- Nhân viên mẫu
INSERT INTO NhanVien (MaNhanVien, HoTen, NgaySinh, GioiTinh, Email, SoDienThoai, MaPhongBan, ChucVu, LuongCoBan, NgayVaoLam) VALUES
    ('NV001', N'Nguyễn Văn An',   '1990-01-15', N'Nam', 'an.nv@company.com',   '0901234567', 1, N'Giám Đốc',       50000000, '2020-01-01'),
    ('NV002', N'Trần Thị Bình',   '1992-03-20', N'Nữ',  'binh.tt@company.com', '0912345678', 2, N'Trưởng Phòng NS', 20000000, '2020-02-01'),
    ('NV003', N'Lê Văn Cường',    '1995-07-10', N'Nam', 'cuong.lv@company.com','0923456789', 4, N'Lập Trình Viên', 15000000, '2021-03-01');
GO

-- Tài khoản (Mật khẩu: Admin@123 → hash SHA256)
-- SHA256("Admin@123") = a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3
-- Dùng plaintext hash demo; thực tế dùng BCrypt
INSERT INTO Users (Username, PasswordHash, Email, HoTen, Role, MaNhanVien, FirstLogin) VALUES
    ('admin',   'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3',
                'admin@company.com',   N'Nguyễn Văn An',   'ADMIN',    'NV001', 0),
    ('manager', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3',
                'manager@company.com', N'Trần Thị Bình',   'MANAGER',  'NV002', 0),
    ('hr001',   'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3',
                'hr@company.com',      N'Lê Văn Cường',    'HR_STAFF', 'NV003', 1);
GO

-- ============================================================
-- STORED PROCEDURES
-- ============================================================

-- SP: Xác thực đăng nhập
CREATE OR ALTER PROCEDURE sp_AuthLogin
    @Username NVARCHAR(50),
    @PasswordHash NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        UserID, Username, HoTen, Role, IsActive, IsLocked,
        FirstLogin, RetryCount, Email, MaNhanVien
    FROM Users
    WHERE Username = @Username
      AND PasswordHash = @PasswordHash;
END
GO

-- SP: Tăng retry và kiểm tra lock
CREATE OR ALTER PROCEDURE sp_IncrementRetry
    @Username NVARCHAR(50),
    @MaxRetry INT = 3
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Users
    SET RetryCount  = RetryCount + 1,
        IsLocked    = CASE WHEN RetryCount + 1 >= @MaxRetry THEN 1 ELSE 0 END,
        NgayCapNhat = GETDATE()
    WHERE Username = @Username;

    SELECT RetryCount, IsLocked FROM Users WHERE Username = @Username;
END
GO

-- SP: Reset retry sau đăng nhập thành công
CREATE OR ALTER PROCEDURE sp_ResetRetry
    @UserID INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Users
    SET RetryCount  = 0,
        LastLogin   = GETDATE(),
        NgayCapNhat = GETDATE()
    WHERE UserID = @UserID;
END
GO

-- SP: Ghi log đăng nhập
CREATE OR ALTER PROCEDURE sp_LogLogin
    @UserID    INT,
    @IPAddress NVARCHAR(50),
    @IsSuccess BIT,
    @Note      NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO LoginHistory (UserID, IPAddress, IsSuccess, Note)
    VALUES (@UserID, @IPAddress, @IsSuccess, @Note);
    SELECT SCOPE_IDENTITY() AS LogID;
END
GO

-- SP: Ghi logout
CREATE OR ALTER PROCEDURE sp_LogLogout
    @LogID INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE LoginHistory SET LogoutTime = GETDATE() WHERE LogID = @LogID;
END
GO

-- SP: Đổi mật khẩu lần đầu
CREATE OR ALTER PROCEDURE sp_ChangePasswordFirstLogin
    @UserID      INT,
    @NewPwHash   NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Users
    SET PasswordHash = @NewPwHash,
        FirstLogin   = 0,
        NgayCapNhat  = GETDATE()
    WHERE UserID = @UserID;
    SELECT @@ROWCOUNT AS Affected;
END
GO

-- SP: Tạo OTP reset mật khẩu
CREATE OR ALTER PROCEDURE sp_CreateOTP
    @UserID  INT,
    @OtpCode NVARCHAR(10),
    @Minutes INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    -- Vô hiệu hoá OTP cũ chưa dùng
    UPDATE OtpTokens SET IsUsed = 1
    WHERE UserID = @UserID AND IsUsed = 0;

    INSERT INTO OtpTokens (UserID, OtpCode, ExpireTime)
    VALUES (@UserID, @OtpCode, DATEADD(MINUTE, @Minutes, GETDATE()));

    SELECT SCOPE_IDENTITY() AS TokenID;
END
GO

-- SP: Xác thực OTP
CREATE OR ALTER PROCEDURE sp_VerifyOTP
    @UserID  INT,
    @OtpCode NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Valid INT = 0;

    IF EXISTS (
        SELECT 1 FROM OtpTokens
        WHERE UserID    = @UserID
          AND OtpCode   = @OtpCode
          AND IsUsed    = 0
          AND ExpireTime > GETDATE()
    )
    BEGIN
        UPDATE OtpTokens SET IsUsed = 1
        WHERE UserID = @UserID AND OtpCode = @OtpCode;
        SET @Valid = 1;
    END

    SELECT @Valid AS IsValid;
END
GO

-- SP: Đặt lại mật khẩu sau OTP
CREATE OR ALTER PROCEDURE sp_ResetPassword
    @UserID    INT,
    @NewPwHash NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Users
    SET PasswordHash = @NewPwHash,
        IsLocked     = 0,
        RetryCount   = 0,
        NgayCapNhat  = GETDATE()
    WHERE UserID = @UserID;
    SELECT @@ROWCOUNT AS Affected;
END
GO

-- SP: Tìm user theo email (cho forgot password)
CREATE OR ALTER PROCEDURE sp_FindUserByEmail
    @Email NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT UserID, Username, HoTen, Email, IsActive, IsLocked
    FROM Users
    WHERE Email = @Email AND IsActive = 1;
END
GO

PRINT N'Database QuanLyNhanVien created successfully!';
GO
