-- =====================================================================
-- FILE TEST BÀI TẬP LỚN — CSDL QUẢN LÝ BÁN HÀNG
-- Mở SSMS → chạy từng SECTION (bôi đen + F5) để kiểm tra tiêu chí.
-- Mỗi lỗi được RAISERROR/CHECK/UNIQUE ra là ĐÚNG NGHIỆP VỤ (không phải bug).
-- =====================================================================

USE quanlybanhang1;
GO

PRINT N'================ BẮT ĐẦU TEST ================';
GO


-- =====================================================================
-- SECTION A — PHÂN QUYỀN & AUDITING (3 điểm)
-- =====================================================================

-- A.1  Tạo Role (0.5) — phải thấy 3 role
PRINT N'--- A.1 Danh sách 3 Role ---';
SELECT name AS RoleName, type_desc
FROM sys.database_principals
WHERE type='R' AND name LIKE 'role_%';

-- A.2  Gán quyền (1.0) — phải thấy quyền GRANT/DENY từng bảng cho từng role
PRINT N'--- A.2 Quyền đã gán cho mỗi role ---';
SELECT dp.name AS Role,
       pm.state_desc AS Action,
       pm.permission_name AS Quyen,
       OBJECT_NAME(pm.major_id) AS ObjectName
FROM sys.database_permissions pm
JOIN sys.database_principals dp ON pm.grantee_principal_id = dp.principal_id
WHERE dp.name LIKE 'role_%'
ORDER BY dp.name, ObjectName, pm.permission_name;

-- A.3  Gán user vào role (0.5) — phải thấy 3 user đúng role
PRINT N'--- A.3 User thuộc role ---';
SELECT dp.name AS Role, u.name AS UserName
FROM sys.database_role_members rm
JOIN sys.database_principals dp ON rm.role_principal_id  = dp.principal_id
JOIN sys.database_principals u  ON rm.member_principal_id = u.principal_id
WHERE dp.name LIKE 'role_%';

-- A.4  Demo phân quyền — giả lập làm NV bán hàng, thử xóa NV → bị chặn
PRINT N'--- A.4.1 Giả lập qlbh_sales: xóa NV (phải bị DENY) ---';
EXECUTE AS USER = 'qlbh_sales';
    SELECT SUSER_NAME() AS LoginHienTai, USER_NAME() AS DbUser;
    BEGIN TRY
        DELETE FROM NHANVIEN WHERE MaNV='NV999_KHONG_TON_TAI';
    END TRY
    BEGIN CATCH
        PRINT N'>> BỊ CHẶN ĐÚNG: ' + ERROR_MESSAGE();
    END CATCH
REVERT;

PRINT N'--- A.4.2 Giả lập qlbh_warehouse: insert HĐ (phải bị DENY) ---';
EXECUTE AS USER = 'qlbh_warehouse';
    BEGIN TRY
        INSERT INTO HOADON VALUES('HD_DENY','AGROMAS','NV001','2024-08-01',NULL);
    END TRY
    BEGIN CATCH
        PRINT N'>> BỊ CHẶN ĐÚNG: ' + ERROR_MESSAGE();
    END CATCH
REVERT;

-- A.5  Auditing (1.0) — thực hiện 1 vài thao tác rồi xem log
PRINT N'--- A.5 Auditing: thao tác HĐ → xem AUDIT_LOG ---';
INSERT INTO HOADON VALUES('HD_AUDIT','ASC','NV002','2024-06-01','2024-06-05');
UPDATE HOADON SET NgayNhanHang='2024-06-10' WHERE MaHD='HD_AUDIT';
DELETE FROM HOADON WHERE MaHD='HD_AUDIT';

SELECT TOP 10 Id, ThoiGian, Username, BangTacDong, HanhDong, KhoaChinh, NoiDung
FROM AUDIT_LOG
ORDER BY Id DESC;
GO


-- =====================================================================
-- SECTION B — CONSTRAINT (1 điểm)
-- =====================================================================

-- B.1  Khóa chính / khóa ngoại (0.5)
PRINT N'--- B.1 Danh sách khóa chính & khóa ngoại ---';
SELECT OBJECT_NAME(parent_object_id) AS TableName, name AS PK
FROM sys.key_constraints WHERE type='PK';

SELECT fk.name AS FK,
       OBJECT_NAME(fk.parent_object_id)    AS FromTable,
       OBJECT_NAME(fk.referenced_object_id) AS ReferTo
FROM sys.foreign_keys fk;

-- B.2  CHECK / UNIQUE (0.5) — mỗi câu dưới PHẢI báo lỗi
PRINT N'--- B.2.1 CK_SANPHAM_DonGia: chặn đơn giá âm ---';
BEGIN TRY
    INSERT INTO SANPHAM VALUES ('SP_ERR', N'Test', N'Cái', -100, NULL);
END TRY BEGIN CATCH PRINT N'>> OK: ' + ERROR_MESSAGE(); END CATCH

PRINT N'--- B.2.2 CK_NHANVIEN_Role: chặn role lạ ---';
BEGIN TRY
    UPDATE NHANVIEN SET Role='hacker' WHERE MaNV='NV001';
END TRY BEGIN CATCH PRINT N'>> OK: ' + ERROR_MESSAGE(); END CATCH

PRINT N'--- B.2.3 CK_HOADON_NgayNhan: chặn ngày nhận < ngày lập ---';
BEGIN TRY
    INSERT INTO HOADON VALUES ('HD_ERR','AGROMAS','NV001','2024-05-10','2024-05-01');
END TRY BEGIN CATCH PRINT N'>> OK: ' + ERROR_MESSAGE(); END CATCH

PRINT N'--- B.2.4 CK_CTHD_SoLuong: chặn số lượng <= 0 ---';
BEGIN TRY
    INSERT INTO CHITIETHOADON VALUES ('HD001','SP003', 0);
END TRY BEGIN CATCH PRINT N'>> OK: ' + ERROR_MESSAGE(); END CATCH

PRINT N'--- B.2.5 UQ_NHANVIEN_Username: chặn username trùng ---';
BEGIN TRY
    INSERT INTO NHANVIEN(MaNV,Ho,Ten,Username,Matkhau,Role)
    VALUES('NV_DUP',N'Test',N'A','tranlonggiang','x','sales');
END TRY BEGIN CATCH PRINT N'>> OK: ' + ERROR_MESSAGE(); END CATCH
GO


-- =====================================================================
-- SECTION C — TRIGGER (3 điểm, 5-7 trigger, nghiệp vụ + lỗi + sáng tạo)
-- =====================================================================

-- C.1  Số lượng (1.0) — phải thấy 7 trigger
PRINT N'--- C.1 Danh sách 7 trigger ---';
SELECT name, OBJECT_NAME(parent_id) AS TableName, create_date
FROM sys.triggers
WHERE is_ms_shipped = 0
ORDER BY name;

-- C.2  TRG_CTHD_ValidateSoLuong — số lượng > 1000 bị chặn
PRINT N'--- C.2 TRG_CTHD_ValidateSoLuong: SoLuong > 1000 ---';
BEGIN TRY
    INSERT INTO CHITIETHOADON VALUES ('HD001','SP003', 9999);
END TRY BEGIN CATCH PRINT N'>> OK: ' + ERROR_MESSAGE(); END CATCH

-- C.3  TRG_NHANVIEN_PreventDelete — xóa NV có HĐ bị chặn (INSTEAD OF)
PRINT N'--- C.3 TRG_NHANVIEN_PreventDelete: xóa NV001 đã có HĐ ---';
BEGIN TRY
    DELETE FROM NHANVIEN WHERE MaNV='NV001';
END TRY BEGIN CATCH PRINT N'>> OK: ' + ERROR_MESSAGE(); END CATCH

-- C.4  TRG_HOADON_DefaultDate — tự điền NgayLapHD khi NULL
PRINT N'--- C.4 TRG_HOADON_DefaultDate: insert NgayLapHD=NULL ---';
INSERT INTO HOADON(MaHD,MaKH,MaNV,NgayLapHD,NgayNhanHang)
VALUES('HD_AUTO','AGROMAS','NV002',NULL,NULL);
SELECT MaHD, NgayLapHD FROM HOADON WHERE MaHD='HD_AUTO';
DELETE FROM HOADON WHERE MaHD='HD_AUTO';

-- C.5  TRG_SANPHAM_GiaKhongGiamQua — giảm >50% bị chặn (sáng tạo)
PRINT N'--- C.5 TRG_SANPHAM_GiaKhongGiamQua: giảm 60% ---';
BEGIN TRY
    UPDATE SANPHAM SET DonGia=100000 WHERE MaSP='SP001';  -- 250k → 100k
END TRY BEGIN CATCH PRINT N'>> OK: ' + ERROR_MESSAGE(); END CATCH

-- C.6  TRG_KHACHHANG_TrungTen — trùng tên công ty bị chặn
PRINT N'--- C.6 TRG_KHACHHANG_TrungTen: trùng tên ---';
BEGIN TRY
    INSERT INTO KHACHHANG VALUES ('KH_DUP', N'Giày An Lạc', N'x','HCM','0900');
END TRY BEGIN CATCH PRINT N'>> OK: ' + ERROR_MESSAGE(); END CATCH

-- C.7  TRG_HOADON_LimitPerDay — tạo 6 HĐ cùng 1 KH/ngày → trigger chặn cái thứ 6
PRINT N'--- C.7 TRG_HOADON_LimitPerDay: >5 HĐ/ngày ---';
BEGIN TRY
    INSERT INTO HOADON VALUES
        ('HD_L1','ASC','NV002','2025-01-01',NULL),
        ('HD_L2','ASC','NV002','2025-01-01',NULL),
        ('HD_L3','ASC','NV002','2025-01-01',NULL),
        ('HD_L4','ASC','NV002','2025-01-01',NULL),
        ('HD_L5','ASC','NV002','2025-01-01',NULL),
        ('HD_L6','ASC','NV002','2025-01-01',NULL);
END TRY BEGIN CATCH PRINT N'>> OK: ' + ERROR_MESSAGE(); END CATCH
DELETE FROM HOADON WHERE MaHD LIKE 'HD_L_';
GO


-- =====================================================================
-- SECTION D — DATABASE & DEMO (3 điểm)
-- =====================================================================

-- D.1  Thiết kế DB (0.5) — xem list bảng + quan hệ
PRINT N'--- D.1 Danh sách bảng + số dòng ---';
SELECT t.name AS TableName,
       p.rows AS RowCounts
FROM sys.tables t
JOIN sys.partitions p ON p.object_id = t.object_id AND p.index_id IN (0,1)
ORDER BY t.name;

-- D.2  Dữ liệu test (0.5)
PRINT N'--- D.2 Dữ liệu mẫu ---';
SELECT 'THANHPHO' AS BangDuLieu, COUNT(*) AS SoDong FROM THANHPHO
UNION ALL SELECT 'KHACHHANG',     COUNT(*) FROM KHACHHANG
UNION ALL SELECT 'NHANVIEN',      COUNT(*) FROM NHANVIEN
UNION ALL SELECT 'SANPHAM',       COUNT(*) FROM SANPHAM
UNION ALL SELECT 'HOADON',        COUNT(*) FROM HOADON
UNION ALL SELECT 'CHITIETHOADON', COUNT(*) FROM CHITIETHOADON;

-- D.3  PROCEDURE (1.0) — demo CRUD bằng SP
PRINT N'--- D.3.1 sp_SANPHAM_Insert ---';
EXEC sp_SANPHAM_Insert 'SP_DEMO', N'Túi xách demo', N'Cái', 500000, NULL;

PRINT N'--- D.3.2 sp_SANPHAM_Search (keyword Giày) ---';
EXEC sp_SANPHAM_Search N'Giày';

PRINT N'--- D.3.3 sp_SANPHAM_Update ---';
EXEC sp_SANPHAM_Update 'SP_DEMO', N'Túi xách Gucci', N'Cái', 600000, NULL;
SELECT * FROM SANPHAM WHERE MaSP='SP_DEMO';

PRINT N'--- D.3.4 sp_SANPHAM_Delete (SP đã có HĐ → báo lỗi) ---';
BEGIN TRY EXEC sp_SANPHAM_Delete 'SP001';
END TRY BEGIN CATCH PRINT N'>> OK: ' + ERROR_MESSAGE(); END CATCH

PRINT N'--- D.3.5 sp_SANPHAM_Delete SP_DEMO (chưa có HĐ → OK) ---';
EXEC sp_SANPHAM_Delete 'SP_DEMO';

PRINT N'--- D.3.6 sp_HOADON_LapHoaDon + sp_HOADON_Delete ---';
EXEC sp_HOADON_LapHoaDon 'HD_DEMO','ASC','NV003','2024-07-01','2024-07-05';
SELECT * FROM HOADON WHERE MaHD='HD_DEMO';
EXEC sp_HOADON_Delete 'HD_DEMO';

-- D.4  SELECT nâng cao — 4 VIEW báo cáo (JOIN + GROUP BY + TOP)
PRINT N'--- D.4.1 V_DoanhThu_KhachHang ---';
SELECT * FROM V_DoanhThu_KhachHang ORDER BY TongDoanhThu DESC;

PRINT N'--- D.4.2 V_Top3_SanPham ---';
SELECT * FROM V_Top3_SanPham;

PRINT N'--- D.4.3 V_DoanhThu_NhanVien ---';
SELECT * FROM V_DoanhThu_NhanVien;

PRINT N'--- D.4.4 V_DoanhThu_ThanhPho ---';
SELECT * FROM V_DoanhThu_ThanhPho;

-- D.4.5  Truy vấn SELECT nâng cao ad-hoc: Khách hàng nào chưa mua gì?
PRINT N'--- D.4.5 Truy vấn nâng cao: KH chưa mua hàng ---';
SELECT k.MaKH, k.TenCty
FROM KHACHHANG k
WHERE NOT EXISTS (SELECT 1 FROM HOADON h WHERE h.MaKH = k.MaKH);

-- D.4.6  Nhân viên bán được nhiều SP khác nhau nhất
PRINT N'--- D.4.6 Truy vấn nâng cao: NV bán nhiều mã SP nhất ---';
SELECT nv.MaNV, nv.Ho+N' '+nv.Ten AS HoTen,
       COUNT(DISTINCT ct.MaSP) AS SoMaSP
FROM NHANVIEN nv
LEFT JOIN HOADON h  ON h.MaNV = nv.MaNV
LEFT JOIN CHITIETHOADON ct ON ct.MaHD = h.MaHD
GROUP BY nv.MaNV, nv.Ho, nv.Ten
ORDER BY SoMaSP DESC;
GO


-- =====================================================================
-- SECTION E — DỌN DẸP DỮ LIỆU TEST
-- =====================================================================
PRINT N'--- E. Dọn dẹp ---';
DELETE FROM CHITIETHOADON WHERE MaHD LIKE 'HD\_%' ESCAPE '\';
DELETE FROM HOADON        WHERE MaHD LIKE 'HD\_%' ESCAPE '\';
DELETE FROM SANPHAM       WHERE MaSP='SP_DEMO';
DELETE FROM KHACHHANG     WHERE MaKH='KH_DUP';
DELETE FROM NHANVIEN      WHERE MaNV='NV_DUP';
GO

PRINT N'================ HOÀN TẤT TEST ================';
PRINT N'Nếu tất cả section đều in >> OK: ... (hoặc bảng dữ liệu phù hợp) là đạt yêu cầu.';
GO
