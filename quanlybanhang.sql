-- ============================================
-- Script tạo CSDL Quản Lý Bán Hàng
-- Chạy trong SQL Server Management Studio
-- ============================================

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'quanlybanhang1')
BEGIN
    CREATE DATABASE quanlybanhang1;
END
GO

USE quanlybanhang1;
GO

-- ===== Bảng THANHPHO =====
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='THANHPHO' AND xtype='U')
CREATE TABLE THANHPHO (
    ThanhPho     VARCHAR(10)   PRIMARY KEY,
    TenThanhPho  NVARCHAR(100) NOT NULL
);
GO

-- ===== Bảng KHACHHANG =====
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='KHACHHANG' AND xtype='U')
CREATE TABLE KHACHHANG (
    MaKH     VARCHAR(10)   PRIMARY KEY,
    TenCty   NVARCHAR(150) NOT NULL,
    DiaChi   NVARCHAR(200),
    ThanhPho VARCHAR(10)   REFERENCES THANHPHO(ThanhPho),
    DienThoai VARCHAR(20)
);
GO

-- ===== Bảng NHANVIEN =====
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='NHANVIEN' AND xtype='U')
CREATE TABLE NHANVIEN (
    MaNV     VARCHAR(10)   PRIMARY KEY,
    Ho       NVARCHAR(50),
    Ten      NVARCHAR(50),
    Nu       BIT DEFAULT 0,
    NgayNV   DATE,
    DiaChi   NVARCHAR(200),
    DienThoai VARCHAR(20),
    Hình     NVARCHAR(200),
    Username VARCHAR(50),
    Matkhau  VARCHAR(50)
);
GO

-- ===== Bảng SANPHAM =====
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SANPHAM' AND xtype='U')
CREATE TABLE SANPHAM (
    MaSP     VARCHAR(10)   PRIMARY KEY,
    TenSP    NVARCHAR(150) NOT NULL,
    DonViTinh NVARCHAR(50),
    DonGia   DECIMAL(18,2),
    Hinh     NVARCHAR(200)
);
GO

-- ===== Bảng HOADON =====
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='HOADON' AND xtype='U')
CREATE TABLE HOADON (
    MaHD         VARCHAR(10)  PRIMARY KEY,
    MaKH         VARCHAR(10)  REFERENCES KHACHHANG(MaKH),
    MaNV         VARCHAR(10)  REFERENCES NHANVIEN(MaNV),
    NgayLapHD    DATE,
    NgayNhanHang DATE
);
GO

-- ===== Bảng CHITIETHOADON =====
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='CHITIETHOADON' AND xtype='U')
CREATE TABLE CHITIETHOADON (
    MaHD    VARCHAR(10) REFERENCES HOADON(MaHD),
    MaSP    VARCHAR(10) REFERENCES SANPHAM(MaSP),
    SoLuong INT,
    PRIMARY KEY (MaHD, MaSP)
);
GO

-- ===== Dữ liệu mẫu =====
INSERT INTO THANHPHO VALUES ('HCM', N'Hồ Chí Minh');
INSERT INTO THANHPHO VALUES ('HN',  N'Hà Nội');
INSERT INTO THANHPHO VALUES ('DN',  N'Đà Nẵng');
GO

INSERT INTO KHACHHANG VALUES ('AGROMAS', N'Cơ Điện Nông Nghiệp Q.3', N'Quận 3', 'HCM', '0281234567');
INSERT INTO KHACHHANG VALUES ('ALSIMES', N'Giày An Lạc',              N'Bình Chánh', 'HCM', '0289876543');
INSERT INTO KHACHHANG VALUES ('ASC',     N'Du Lịch An Phú',           N'Quận 1', 'HCM', '0281112222');
INSERT INTO KHACHHANG VALUES ('ASECO',   N'Giày May 3/2',             N'Quận 10','HCM', '0283334444');
INSERT INTO KHACHHANG VALUES ('ATC',     N'Sản Xuất Hàng Mỹ Thuật',  N'Tân Bình','HCM', '0285556666');
GO

INSERT INTO NHANVIEN VALUES ('NV001','Trần','Long Giang',0,'2020-01-01',N'Hà Nội','0901111111',NULL,'tranlonggiang','123456');
INSERT INTO NHANVIEN VALUES ('NV002','Nguyễn Thị','Khánh Linh',1,'2021-06-15',N'HCM','0912222222',NULL,'nguyenthikhanhlinh','123456');
INSERT INTO NHANVIEN VALUES ('NV003','Trịnh','Thái Sơn',1,'2021-06-15',N'HCM','0912222222',NULL,'trinhthaison','123456');
INSERT INTO NHANVIEN VALUES ('NV004','Cao','Văn Vũ',1,'2021-06-15',N'HCM','0912222222',NULL,'caovanvu','123456');
GO

INSERT INTO SANPHAM VALUES ('SP001', N'Giày Da Nam',   N'Đôi',  250000, NULL);
INSERT INTO SANPHAM VALUES ('SP002', N'Áo Thun Nữ',    N'Cái',  150000, NULL);
INSERT INTO SANPHAM VALUES ('SP003', N'Quần Jean Nam', N'Cái',  350000, NULL);
GO

INSERT INTO HOADON VALUES ('HD001','AGROMAS','NV001','2024-01-10','2024-01-15');
INSERT INTO HOADON VALUES ('HD002','ALSIMES','NV002','2024-02-05','2024-02-10');
GO

INSERT INTO CHITIETHOADON VALUES ('HD001','SP001',10);
INSERT INTO CHITIETHOADON VALUES ('HD001','SP002',5);
INSERT INTO CHITIETHOADON VALUES ('HD002','SP003',8);
GO

SELECT * FROM THANHPHO;
SELECT * FROM KHACHHANG;
SELECT * FROM NHANVIEN;
GO

IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'NHANVIEN' AND COLUMN_NAME = 'Role'
)
BEGIN
    ALTER TABLE NHANVIEN
    ADD Role NVARCHAR(20) NOT NULL DEFAULT 'sales';
    PRINT 'Đã thêm cột Role vào bảng NHANVIEN.';
END
ELSE
BEGIN
    PRINT 'Cột Role đã tồn tại, không cần thêm.';
END
GO

-- ============================================
-- UPDATE Role và Mật khẩu 
-- ============================================
IF EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'NHANVIEN' AND COLUMN_NAME = 'Matkhau'
      AND CHARACTER_MAXIMUM_LENGTH < 64
)
BEGIN
    ALTER TABLE NHANVIEN ALTER COLUMN Matkhau VARCHAR(64);
    PRINT 'Đã mở rộng cột Matkhau thành VARCHAR(64).';
END
GO

-- Đổi role cũ 'user' thành 'sales' (tương thích ngược)
UPDATE NHANVIEN SET Role = 'sales' WHERE Role = 'user';
GO

-- Gán role cụ thể cho từng nhân viên mẫu
UPDATE NHANVIEN SET Role = 'admin'     WHERE MaNV = 'NV001'; 
UPDATE NHANVIEN SET Role = 'sales'     WHERE MaNV = 'NV002'; 
UPDATE NHANVIEN SET Role = 'sales'     WHERE MaNV = 'NV003';
UPDATE NHANVIEN SET Role = 'warehouse' WHERE MaNV = 'NV004';
GO

-- ============================================
-- Hash mật khẩu bằng SHA-256
-- ============================================
UPDATE NHANVIEN
SET Matkhau = '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92'
WHERE Matkhau = '123456';
GO

PRINT 'Cập nhật Role và hash mật khẩu hoàn tất.';
GO

-- ============================================================
-- Lịch sử hội thoại AI
-- ============================================================

-- Bảng lưu các phiên hội thoại (mỗi cuộc hội thoại là 1 dòng)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='CHAT_HISTORY_SESSION' AND xtype='U')
CREATE TABLE CHAT_HISTORY_SESSION (
    Id        INT IDENTITY(1,1) PRIMARY KEY,
    Username  VARCHAR(50)   NOT NULL DEFAULT '',
    Title     NVARCHAR(200) NOT NULL DEFAULT N'Hội thoại mới',
    CreatedAt DATETIME      NOT NULL DEFAULT GETDATE()
);
GO

-- Bảng lưu từng tin nhắn trong một phiên hội thoại
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='CHAT_HISTORY_MESSAGE' AND xtype='U')
CREATE TABLE CHAT_HISTORY_MESSAGE (
    Id        INT IDENTITY(1,1) PRIMARY KEY,
    SessionId INT           NOT NULL REFERENCES CHAT_HISTORY_SESSION(Id) ON DELETE CASCADE,
    Role      VARCHAR(20)   NOT NULL,            -- 'user' hoặc 'assistant'
    Content   NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME      NOT NULL DEFAULT GETDATE()
);
GO

PRINT N'Tạo bảng CHAT_HISTORY_SESSION và CHAT_HISTORY_MESSAGE hoàn tất.';
GO
