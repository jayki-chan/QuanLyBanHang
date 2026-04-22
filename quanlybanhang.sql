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

INSERT INTO NHANVIEN VALUES ('NV001',N'Trần',       N'Long Giang', 0,'2020-01-01',N'Hà Nội','0901111111',NULL,'tranlonggiang',      '123456');
INSERT INTO NHANVIEN VALUES ('NV002',N'Nguyễn Thị', N'Khánh Linh', 1,'2021-06-15',N'HCM',   '0912222222',NULL,'nguyenthikhanhlinh', '123456');
INSERT INTO NHANVIEN VALUES ('NV003',N'Trịnh',      N'Thái Sơn',   1,'2021-06-15',N'HCM',   '0912222222',NULL,'trinhthaison',       '123456');
INSERT INTO NHANVIEN VALUES ('NV004',N'Cao',        N'Văn Vũ',     1,'2021-06-15',N'HCM',   '0912222222',NULL,'caovanvu',           '123456');
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
    PRINT N'Đã thêm cột Role vào bảng NHANVIEN.';
END
ELSE
BEGIN
    PRINT N'Cột Role đã tồn tại, không cần thêm.';
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
    PRINT N'Đã mở rộng cột Matkhau thành VARCHAR(64).';
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

PRINT N'Cập nhật Role và hash mật khẩu hoàn tất.';
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

-- ============================================================
-- 1. CONSTRAINT: CHECK / UNIQUE 
-- ============================================================

-- UNIQUE: Username nhân viên không được trùng
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='UQ_NHANVIEN_Username')
    ALTER TABLE NHANVIEN ADD CONSTRAINT UQ_NHANVIEN_Username UNIQUE(Username);
GO

-- CHECK: Role chỉ chấp nhận 3 giá trị
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name='CK_NHANVIEN_Role')
    ALTER TABLE NHANVIEN ADD CONSTRAINT CK_NHANVIEN_Role
        CHECK (Role IN ('admin','sales','warehouse'));
GO

-- CHECK: Đơn giá sản phẩm phải > 0
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name='CK_SANPHAM_DonGia')
    ALTER TABLE SANPHAM ADD CONSTRAINT CK_SANPHAM_DonGia CHECK (DonGia > 0);
GO

-- CHECK: Số lượng trong chi tiết HĐ phải > 0
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name='CK_CTHD_SoLuong')
    ALTER TABLE CHITIETHOADON ADD CONSTRAINT CK_CTHD_SoLuong CHECK (SoLuong > 0);
GO

-- CHECK: Ngày nhận hàng >= Ngày lập
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name='CK_HOADON_NgayNhan')
    ALTER TABLE HOADON ADD CONSTRAINT CK_HOADON_NgayNhan
        CHECK (NgayNhanHang IS NULL OR NgayNhanHang >= NgayLapHD);
GO

-- CHECK: Số điện thoại phải bắt đầu bằng số
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name='CK_NHANVIEN_DienThoai')
    ALTER TABLE NHANVIEN ADD CONSTRAINT CK_NHANVIEN_DienThoai
        CHECK (DienThoai IS NULL OR DienThoai LIKE '[0-9]%');
GO


-- ============================================================
-- 2. AUDITING — bảng lưu vết truy cập / thao tác
-- ============================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='AUDIT_LOG' AND xtype='U')
CREATE TABLE AUDIT_LOG (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    ThoiGian    DATETIME      NOT NULL DEFAULT GETDATE(),
    Username    NVARCHAR(100) NOT NULL DEFAULT SUSER_SNAME(),
    HostName    NVARCHAR(100) NOT NULL DEFAULT HOST_NAME(),
    BangTacDong NVARCHAR(50)  NOT NULL,
    HanhDong    NVARCHAR(20)  NOT NULL,
    KhoaChinh   NVARCHAR(100),
    NoiDung     NVARCHAR(MAX)
);
GO


-- ============================================================
-- 3. PHÂN QUYỀN — 3 roles cấp database + SQL login + gán user
-- ============================================================
BEGIN TRY IF DATABASE_PRINCIPAL_ID('role_admin')     IS NULL EXEC('CREATE ROLE role_admin');     END TRY BEGIN CATCH END CATCH
BEGIN TRY IF DATABASE_PRINCIPAL_ID('role_sales')     IS NULL EXEC('CREATE ROLE role_sales');     END TRY BEGIN CATCH END CATCH
BEGIN TRY IF DATABASE_PRINCIPAL_ID('role_warehouse') IS NULL EXEC('CREATE ROLE role_warehouse'); END TRY BEGIN CATCH END CATCH
GO

-- role_admin: toàn quyền CRUD trên toàn bộ schema dbo
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO role_admin;
GO

-- role_sales: chỉ thao tác trên KH, HĐ, CT HĐ; được xem SP & NV
GRANT SELECT                         ON SANPHAM       TO role_sales;
GRANT SELECT                         ON NHANVIEN      TO role_sales;
GRANT SELECT, INSERT, UPDATE, DELETE ON KHACHHANG     TO role_sales;
GRANT SELECT, INSERT, UPDATE, DELETE ON HOADON        TO role_sales;
GRANT SELECT, INSERT, UPDATE, DELETE ON CHITIETHOADON TO role_sales;
DENY  DELETE                         ON NHANVIEN      TO role_sales;
GO

-- role_warehouse: quản lý SP, chỉ xem HĐ/CT HĐ/KH (không sửa)
GRANT SELECT, INSERT, UPDATE, DELETE ON SANPHAM       TO role_warehouse;
GRANT SELECT                         ON HOADON        TO role_warehouse;
GRANT SELECT                         ON CHITIETHOADON TO role_warehouse;
GRANT SELECT                         ON KHACHHANG     TO role_warehouse;
DENY  INSERT, UPDATE, DELETE         ON HOADON        TO role_warehouse;
GO

-- Mọi role đều được ghi vào AUDIT_LOG (để lưu vết)
GRANT INSERT         ON AUDIT_LOG TO role_sales, role_warehouse;
GRANT SELECT, INSERT ON AUDIT_LOG TO role_admin;
GO

-- Tạo 3 SQL Login (chạy ở context master)
USE master;
GO
BEGIN TRY IF NOT EXISTS (SELECT 1 FROM sys.sql_logins WHERE name='qlbh_admin')     EXEC('CREATE LOGIN qlbh_admin     WITH PASSWORD=''Admin@123'',     CHECK_POLICY=OFF'); END TRY BEGIN CATCH END CATCH
BEGIN TRY IF NOT EXISTS (SELECT 1 FROM sys.sql_logins WHERE name='qlbh_sales')     EXEC('CREATE LOGIN qlbh_sales     WITH PASSWORD=''Sales@123'',     CHECK_POLICY=OFF'); END TRY BEGIN CATCH END CATCH
BEGIN TRY IF NOT EXISTS (SELECT 1 FROM sys.sql_logins WHERE name='qlbh_warehouse') EXEC('CREATE LOGIN qlbh_warehouse WITH PASSWORD=''Warehouse@123'', CHECK_POLICY=OFF'); END TRY BEGIN CATCH END CATCH
GO
USE quanlybanhang1;
GO

-- Tạo user DB cho 3 login
BEGIN TRY IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name='qlbh_admin')     EXEC('CREATE USER qlbh_admin     FOR LOGIN qlbh_admin');     END TRY BEGIN CATCH END CATCH
BEGIN TRY IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name='qlbh_sales')     EXEC('CREATE USER qlbh_sales     FOR LOGIN qlbh_sales');     END TRY BEGIN CATCH END CATCH
BEGIN TRY IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name='qlbh_warehouse') EXEC('CREATE USER qlbh_warehouse FOR LOGIN qlbh_warehouse'); END TRY BEGIN CATCH END CATCH
GO

-- Gán user vào role tương ứng (idempotent)
BEGIN TRY ALTER ROLE role_admin     ADD MEMBER qlbh_admin;     END TRY BEGIN CATCH END CATCH
BEGIN TRY ALTER ROLE role_sales     ADD MEMBER qlbh_sales;     END TRY BEGIN CATCH END CATCH
BEGIN TRY ALTER ROLE role_warehouse ADD MEMBER qlbh_warehouse; END TRY BEGIN CATCH END CATCH
GO


-- ============================================================
-- 4. TRIGGERS (7 trigger — nghiệp vụ + auditing + báo lỗi)
-- ============================================================

-- Trigger 1: Ghi log mọi thao tác trên HOADON
GO
CREATE OR ALTER TRIGGER TRG_HOADON_Audit
ON HOADON
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM inserted) AND EXISTS (SELECT 1 FROM deleted)
        INSERT INTO AUDIT_LOG(BangTacDong,HanhDong,KhoaChinh,NoiDung)
        SELECT 'HOADON','UPDATE',i.MaHD,
               N'Cập nhật HĐ '+i.MaHD+N' (KH='+ISNULL(i.MaKH,'')+N', NV='+ISNULL(i.MaNV,'')+N')'
        FROM inserted i;
    ELSE IF EXISTS (SELECT 1 FROM inserted)
        INSERT INTO AUDIT_LOG(BangTacDong,HanhDong,KhoaChinh,NoiDung)
        SELECT 'HOADON','INSERT',i.MaHD, N'Thêm HĐ '+i.MaHD FROM inserted i;
    ELSE IF EXISTS (SELECT 1 FROM deleted)
        INSERT INTO AUDIT_LOG(BangTacDong,HanhDong,KhoaChinh,NoiDung)
        SELECT 'HOADON','DELETE',d.MaHD, N'Xóa HĐ '+d.MaHD FROM deleted d;
END
GO

-- Trigger 2: Giới hạn số lượng 1 dòng chi tiết HĐ ≤ 1000
GO
CREATE OR ALTER TRIGGER TRG_CTHD_ValidateSoLuong
ON CHITIETHOADON
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM inserted WHERE SoLuong > 1000)
    BEGIN
        RAISERROR(N'Số lượng mỗi mặt hàng trong 1 hóa đơn không được vượt quá 1000.',16,1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END
GO

-- Trigger 3: Không cho phép xóa nhân viên đã có hóa đơn
GO
CREATE OR ALTER TRIGGER TRG_NHANVIEN_PreventDelete
ON NHANVIEN
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM deleted d JOIN HOADON h ON h.MaNV=d.MaNV)
    BEGIN
        RAISERROR(N'Không thể xóa nhân viên đã lập hóa đơn. Hãy vô hiệu hóa tài khoản thay vì xóa.',16,1);
        RETURN;
    END
    DELETE FROM NHANVIEN WHERE MaNV IN (SELECT MaNV FROM deleted);
END
GO

-- Trigger 4: 1 khách hàng không được lập > 5 hóa đơn trong cùng 1 ngày
GO
CREATE OR ALTER TRIGGER TRG_HOADON_LimitPerDay
ON HOADON
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1
        FROM HOADON h JOIN inserted i ON h.MaKH=i.MaKH AND h.NgayLapHD=i.NgayLapHD
        GROUP BY h.MaKH, h.NgayLapHD
        HAVING COUNT(*) > 5
    )
    BEGIN
        RAISERROR(N'Khách hàng không được có quá 5 hóa đơn trong cùng một ngày.',16,1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END
GO

-- Trigger 5: Tự gán NgayLapHD = hôm nay nếu bị NULL
GO
CREATE OR ALTER TRIGGER TRG_HOADON_DefaultDate
ON HOADON
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE HOADON SET NgayLapHD = CAST(GETDATE() AS DATE)
    WHERE MaHD IN (SELECT MaHD FROM inserted WHERE NgayLapHD IS NULL);
END
GO

-- Trigger 6: Chống gian lận — không cho giảm đơn giá SP quá 50%
GO
CREATE OR ALTER TRIGGER TRG_SANPHAM_GiaKhongGiamQua
ON SANPHAM
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF UPDATE(DonGia) AND EXISTS (
        SELECT 1 FROM inserted i JOIN deleted d ON i.MaSP=d.MaSP
        WHERE d.DonGia > 0 AND i.DonGia < d.DonGia * 0.5
    )
    BEGIN
        RAISERROR(N'Không thể giảm đơn giá sản phẩm quá 50%% so với giá cũ.',16,1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END
GO

-- Trigger 7: Tên công ty khách hàng không được trùng (case-insensitive)
GO
CREATE OR ALTER TRIGGER TRG_KHACHHANG_TrungTen
ON KHACHHANG
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1 FROM KHACHHANG k JOIN inserted i
               ON k.TenCty=i.TenCty AND k.MaKH<>i.MaKH
    )
    BEGIN
        RAISERROR(N'Tên công ty khách hàng đã tồn tại. Vui lòng dùng tên khác.',16,1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END
GO


-- ============================================================
-- 5. STORED PROCEDURE — CRUD + Tìm kiếm (cho từng bảng chính)
-- ============================================================

-- ----- SANPHAM -----
GO
CREATE OR ALTER PROCEDURE sp_SANPHAM_Insert
    @MaSP VARCHAR(10), @TenSP NVARCHAR(150),
    @DonViTinh NVARCHAR(50), @DonGia DECIMAL(18,2), @Hinh NVARCHAR(200)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM SANPHAM WHERE MaSP=@MaSP)
    BEGIN
        RAISERROR(N'Mã sản phẩm đã tồn tại.',16,1); RETURN;
    END
    INSERT INTO SANPHAM(MaSP,TenSP,DonViTinh,DonGia,Hinh)
    VALUES(@MaSP,@TenSP,@DonViTinh,@DonGia,@Hinh);
END
GO

GO
CREATE OR ALTER PROCEDURE sp_SANPHAM_Update
    @MaSP VARCHAR(10), @TenSP NVARCHAR(150),
    @DonViTinh NVARCHAR(50), @DonGia DECIMAL(18,2), @Hinh NVARCHAR(200)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE SANPHAM SET TenSP=@TenSP, DonViTinh=@DonViTinh, DonGia=@DonGia, Hinh=@Hinh
    WHERE MaSP=@MaSP;
    IF @@ROWCOUNT=0 RAISERROR(N'Không tìm thấy sản phẩm để cập nhật.',16,1);
END
GO

GO
CREATE OR ALTER PROCEDURE sp_SANPHAM_Delete @MaSP VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM CHITIETHOADON WHERE MaSP=@MaSP)
    BEGIN
        RAISERROR(N'Sản phẩm đã có trong hóa đơn, không thể xóa.',16,1); RETURN;
    END
    DELETE FROM SANPHAM WHERE MaSP=@MaSP;
END
GO

GO
CREATE OR ALTER PROCEDURE sp_SANPHAM_Search @Keyword NVARCHAR(150)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM SANPHAM
    WHERE @Keyword IS NULL OR @Keyword=N''
       OR TenSP LIKE N'%'+@Keyword+N'%'
       OR MaSP  LIKE '%'+CAST(@Keyword AS VARCHAR(150))+'%';
END
GO

-- ----- KHACHHANG -----
GO
CREATE OR ALTER PROCEDURE sp_KHACHHANG_Insert
    @MaKH VARCHAR(10), @TenCty NVARCHAR(150), @DiaChi NVARCHAR(200),
    @ThanhPho VARCHAR(10), @DienThoai VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO KHACHHANG VALUES(@MaKH,@TenCty,@DiaChi,@ThanhPho,@DienThoai);
END
GO

GO
CREATE OR ALTER PROCEDURE sp_KHACHHANG_Update
    @MaKH VARCHAR(10), @TenCty NVARCHAR(150), @DiaChi NVARCHAR(200),
    @ThanhPho VARCHAR(10), @DienThoai VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE KHACHHANG SET TenCty=@TenCty,DiaChi=@DiaChi,ThanhPho=@ThanhPho,DienThoai=@DienThoai
    WHERE MaKH=@MaKH;
    IF @@ROWCOUNT=0 RAISERROR(N'Không tìm thấy khách hàng.',16,1);
END
GO

GO
CREATE OR ALTER PROCEDURE sp_KHACHHANG_Delete @MaKH VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM HOADON WHERE MaKH=@MaKH)
    BEGIN
        RAISERROR(N'Khách hàng đã có hóa đơn, không thể xóa.',16,1); RETURN;
    END
    DELETE FROM KHACHHANG WHERE MaKH=@MaKH;
END
GO

-- ----- HOADON — lập hóa đơn header -----
GO
CREATE OR ALTER PROCEDURE sp_HOADON_LapHoaDon
    @MaHD VARCHAR(10), @MaKH VARCHAR(10), @MaNV VARCHAR(10),
    @NgayLap DATE=NULL, @NgayNhan DATE=NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN
            INSERT INTO HOADON VALUES(@MaHD,@MaKH,@MaNV,@NgayLap,@NgayNhan);
        COMMIT TRAN
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT>0 ROLLBACK TRAN;
        DECLARE @msg NVARCHAR(2000)=ERROR_MESSAGE();
        RAISERROR(@msg,16,1);
    END CATCH
END
GO

-- ----- HOADON — xóa hóa đơn kèm chi tiết -----
GO
CREATE OR ALTER PROCEDURE sp_HOADON_Delete @MaHD VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN
            DELETE FROM CHITIETHOADON WHERE MaHD=@MaHD;
            DELETE FROM HOADON WHERE MaHD=@MaHD;
        COMMIT TRAN
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT>0 ROLLBACK TRAN;
        DECLARE @msg NVARCHAR(2000)=ERROR_MESSAGE();
        RAISERROR(@msg,16,1);
    END CATCH
END
GO

-- Cấp quyền EXECUTE cho role tương ứng
GRANT EXECUTE ON sp_SANPHAM_Insert   TO role_admin, role_warehouse;
GRANT EXECUTE ON sp_SANPHAM_Update   TO role_admin, role_warehouse;
GRANT EXECUTE ON sp_SANPHAM_Delete   TO role_admin, role_warehouse;
GRANT EXECUTE ON sp_SANPHAM_Search   TO role_admin, role_sales, role_warehouse;
GRANT EXECUTE ON sp_KHACHHANG_Insert TO role_admin, role_sales;
GRANT EXECUTE ON sp_KHACHHANG_Update TO role_admin, role_sales;
GRANT EXECUTE ON sp_KHACHHANG_Delete TO role_admin, role_sales;
GRANT EXECUTE ON sp_HOADON_LapHoaDon TO role_admin, role_sales;
GRANT EXECUTE ON sp_HOADON_Delete    TO role_admin, role_sales;
GO


-- ============================================================
-- 6. SELECT NÂNG CAO (VIEW báo cáo — JOIN, GROUP, TOP)
-- ============================================================

-- View 1: Doanh thu theo khách hàng
GO
CREATE OR ALTER VIEW V_DoanhThu_KhachHang AS
SELECT k.MaKH, k.TenCty,
       COUNT(DISTINCT h.MaHD)                    AS SoHoaDon,
       ISNULL(SUM(ct.SoLuong*sp.DonGia),0)       AS TongDoanhThu
FROM KHACHHANG k
LEFT JOIN HOADON        h  ON h.MaKH = k.MaKH
LEFT JOIN CHITIETHOADON ct ON ct.MaHD = h.MaHD
LEFT JOIN SANPHAM       sp ON sp.MaSP = ct.MaSP
GROUP BY k.MaKH, k.TenCty;
GO

-- View 2: Top 3 sản phẩm bán chạy
GO
CREATE OR ALTER VIEW V_Top3_SanPham AS
SELECT TOP 3 sp.MaSP, sp.TenSP, SUM(ct.SoLuong) AS TongBan
FROM SANPHAM sp JOIN CHITIETHOADON ct ON sp.MaSP = ct.MaSP
GROUP BY sp.MaSP, sp.TenSP
ORDER BY SUM(ct.SoLuong) DESC;
GO

-- View 3: Doanh thu theo nhân viên
GO
CREATE OR ALTER VIEW V_DoanhThu_NhanVien AS
SELECT nv.MaNV,
       (nv.Ho + N' ' + nv.Ten)                AS HoTen,
       nv.Role,
       ISNULL(SUM(ct.SoLuong*sp.DonGia),0)    AS TongDoanhThu
FROM NHANVIEN nv
LEFT JOIN HOADON        h  ON h.MaNV = nv.MaNV
LEFT JOIN CHITIETHOADON ct ON ct.MaHD = h.MaHD
LEFT JOIN SANPHAM       sp ON sp.MaSP = ct.MaSP
GROUP BY nv.MaNV, nv.Ho, nv.Ten, nv.Role;
GO

-- View 4: Doanh thu theo thành phố
GO
CREATE OR ALTER VIEW V_DoanhThu_ThanhPho AS
SELECT tp.ThanhPho, tp.TenThanhPho,
       ISNULL(SUM(ct.SoLuong*sp.DonGia),0) AS TongDoanhThu
FROM THANHPHO tp
LEFT JOIN KHACHHANG     k  ON k.ThanhPho = tp.ThanhPho
LEFT JOIN HOADON        h  ON h.MaKH = k.MaKH
LEFT JOIN CHITIETHOADON ct ON ct.MaHD = h.MaHD
LEFT JOIN SANPHAM       sp ON sp.MaSP = ct.MaSP
GROUP BY tp.ThanhPho, tp.TenThanhPho;
GO

GRANT SELECT ON V_DoanhThu_KhachHang TO role_admin, role_sales;
GRANT SELECT ON V_Top3_SanPham       TO role_admin, role_sales, role_warehouse;
GRANT SELECT ON V_DoanhThu_NhanVien  TO role_admin;
GRANT SELECT ON V_DoanhThu_ThanhPho  TO role_admin, role_sales;
GO

PRINT N'Hoàn tất: Role/Phân quyền + Auditing + Constraint + 7 Trigger + Procedure CRUD + View báo cáo.';
GO
