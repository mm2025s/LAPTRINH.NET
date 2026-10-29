/* =====================================================================
   Lab 07 - IT3212 (Lap trinh .NET) - Chuong 4
   TAO CSDL SQL SERVER QLSinhVien + DU LIEU MAU
   ---------------------------------------------------------------------
   Co so du lieu QLSinhVien duoc dung lien tuc tu Lab 7 den Lab 16.

   Cach chay:
     - SSMS  : mo tep nay, nhan Execute (F5).
     - sqlcmd: sqlcmd -S .\SQLEXPRESS -E -i Lab07_QLSinhVien.sql
               sqlcmd -S (localdb)\MSSQLLocalDB -E -i Lab07_QLSinhVien.sql

   Luu y: tep co the chay lai nhieu lan tren database moi hoac da co.
          Tep se XOA cac bang cu (neu ton tai) truoc khi tao lai.
   ===================================================================== */

/* ---------------------------------------------------------------------
   BAI 1: Tao co so du lieu QLSinhVien va chuyen ngu canh su dung
   --------------------------------------------------------------------- */
IF DB_ID(N'QLSinhVien') IS NULL
    CREATE DATABASE QLSinhVien;
GO

USE QLSinhVien;
GO

/* (Tuy chon) Don sach neu chay lai - xoa theo thu tu khoa ngoai */
IF OBJECT_ID(N'dbo.DangKy',  N'U') IS NOT NULL DROP TABLE dbo.DangKy;
IF OBJECT_ID(N'dbo.HocPhan', N'U') IS NOT NULL DROP TABLE dbo.HocPhan;
IF OBJECT_ID(N'dbo.SinhVien',N'U') IS NOT NULL DROP TABLE dbo.SinhVien;
IF OBJECT_ID(N'dbo.Khoa',    N'U') IS NOT NULL DROP TABLE dbo.Khoa;
GO

/* ---------------------------------------------------------------------
   BAI 2: Tao bang Khoa va SinhVien
          - Khoa chinh, khoa ngoai
          - CHECK diem thuoc [0, 10]
          - Du lieu tieng Viet dung NVARCHAR
   --------------------------------------------------------------------- */
CREATE TABLE Khoa
(
    MaKhoa  VARCHAR(10)   NOT NULL PRIMARY KEY,
    TenKhoa NVARCHAR(100) NOT NULL UNIQUE
);
GO

CREATE TABLE SinhVien
(
    MaSV     VARCHAR(15)  NOT NULL PRIMARY KEY,
    HoTen    NVARCHAR(100) NOT NULL,
    NgaySinh DATE         NOT NULL,
    GioiTinh BIT          NOT NULL             -- 1: Nam, 0: Nu
        CONSTRAINT DF_SinhVien_GioiTinh DEFAULT 1,
    MaKhoa   VARCHAR(10)  NOT NULL,
    DiemTB   DECIMAL(4,2) NOT NULL CONSTRAINT DF_SinhVien_DiemTB DEFAULT 0,
    Email    VARCHAR(100) NULL,
    CONSTRAINT CK_SinhVien_Diem   CHECK (DiemTB BETWEEN 0 AND 10),
    CONSTRAINT FK_SinhVien_Khoa   FOREIGN KEY (MaKhoa) REFERENCES Khoa(MaKhoa)
);
GO

/* ---------------------------------------------------------------------
   BAI 2 (Bo sung - rang buoc du lieu):
          - UNIQUE cho Email (co the NULL; SQL Server cho phep nhieu NULL)
          - DEFAULT ngay dang ky (dung cho bang DangKy ben duoi)
   --------------------------------------------------------------------- */
ALTER TABLE SinhVien
    ADD CONSTRAINT UQ_SinhVien_Email UNIQUE (Email);
GO

/* ---------------------------------------------------------------------
   BAI 3: Tao bang HocPhan va DangKy
          - Mo hinh quan he nhieu - nhieu giua SinhVien va HocPhan
          - Khoa chinh cua DangKy gom (MaSV, MaHP)
   --------------------------------------------------------------------- */
CREATE TABLE HocPhan
(
    MaHP     VARCHAR(10)   NOT NULL PRIMARY KEY,
    TenHP    NVARCHAR(120) NOT NULL,
    SoTinChi INT           NOT NULL
        CONSTRAINT CK_HocPhan_SoTinChi CHECK (SoTinChi BETWEEN 1 AND 6)
);
GO

CREATE TABLE DangKy
(
    MaSV       VARCHAR(15)  NOT NULL,
    MaHP       VARCHAR(10)  NOT NULL,
    HocKy      INT          NOT NULL,
    Diem       DECIMAL(4,2) NULL,
    NgayDangKy DATE         NOT NULL
        CONSTRAINT DF_DangKy_NgayDangKy DEFAULT GETDATE(),   -- Bo sung bai 6
    CONSTRAINT PK_DangKy        PRIMARY KEY (MaSV, MaHP),
    CONSTRAINT CK_DangKy_HocKy  CHECK (HocKy BETWEEN 1 AND 3),  -- Bo sung bai 6
    CONSTRAINT CK_DangKy_Diem   CHECK (Diem IS NULL OR Diem BETWEEN 0 AND 10),
    CONSTRAINT FK_DangKy_SV     FOREIGN KEY (MaSV) REFERENCES SinhVien(MaSV),
    CONSTRAINT FK_DangKy_HP     FOREIGN KEY (MaHP) REFERENCES HocPhan(MaHP)
);
GO

/* ---------------------------------------------------------------------
   BAI 4: Them du lieu mau
          Yeu cau toi thieu: 3 khoa, 10 sinh vien, 5 hoc phan + dang ky.
          Duoi day them 3 khoa, 12 sinh vien, 6 hoc phan, 24 dang ky.
   --------------------------------------------------------------------- */
INSERT INTO Khoa(MaKhoa, TenKhoa) VALUES
('CNTT', N'Công nghệ thông tin'),
('OTO',  N'Công nghệ kỹ thuật ô tô'),
('KT',   N'Kinh tế');
GO

INSERT INTO SinhVien(MaSV, HoTen, NgaySinh, GioiTinh, MaKhoa, DiemTB, Email) VALUES
('SV001', N'Nguyễn Văn An',    '2005-02-15', 1, 'CNTT', 8.20, 'an@eaut.edu.vn'),
('SV002', N'Trần Thị Bình',    '2005-07-21', 0, 'CNTT', 7.40, 'binh@eaut.edu.vn'),
('SV003', N'Lê Văn Cường',     '2004-12-03', 1, 'OTO',  6.80, NULL),
('SV004', N'Phạm Thu Dung',    '2005-03-09', 0, 'KT',   9.10, 'dung@eaut.edu.vn'),
('SV005', N'Hoàng Minh Đức',   '2005-01-27', 1, 'CNTT', 4.80, 'duc@eaut.edu.vn'),
('SV006', N'Vũ Thị Hằng',      '2004-10-11', 0, 'KT',   7.90, 'hang@eaut.edu.vn'),
('SV007', N'Đặng Quốc Khánh',  '2005-05-30', 1, 'OTO',  6.80, 'khanh@eaut.edu.vn'),
('SV008', N'Bùi Lan Anh',      '2005-08-18', 0, 'CNTT', 8.20, 'lananh@eaut.edu.vn'),
('SV009', N'Ngô Tuấn Kiệt',    '2004-11-25', 1, 'OTO',  5.50, 'kiet@eaut.edu.vn'),
('SV010', N'Đỗ Thùy Linh',     '2005-04-02', 0, 'KT',   8.70, 'linh@eaut.edu.vn'),
('SV011', N'Trịnh Bảo Long',   '2005-06-14', 1, 'CNTT', 7.10, 'long@eaut.edu.vn'),
('SV012', N'Lý Minh Ngọc',     '2005-09-08', 0, 'OTO',  6.30, 'ngoc@eaut.edu.vn');
GO

INSERT INTO HocPhan(MaHP, TenHP, SoTinChi) VALUES
('HP01', N'Lập trình .NET',        4),
('HP02', N'Cơ sở dữ liệu',         3),
('HP03', N'Lập trình hướng đối tượng', 3),
('HP04', N'Mạng máy tính',         3),
('HP05', N'Trí tuệ nhân tạo',      3),
('HP06', N'An toàn thông tin',     2);
GO

INSERT INTO DangKy(MaSV, MaHP, HocKy, Diem) VALUES
('SV001', 'HP01', 1, 8.50), ('SV001', 'HP02', 1, 7.80), ('SV001', 'HP03', 2, 9.00),
('SV002', 'HP01', 1, 7.00), ('SV002', 'HP02', 1, 7.50), ('SV002', 'HP04', 2, 6.90),
('SV003', 'HP01', 1, 6.50), ('SV003', 'HP05', 2, 7.20),
('SV004', 'HP02', 1, 9.30), ('SV004', 'HP03', 1, 8.80), ('SV004', 'HP06', 2, 9.10),
('SV005', 'HP01', 1, 4.50), ('SV005', 'HP04', 2, 5.10),
('SV006', 'HP02', 1, 8.00), ('SV006', 'HP03', 2, 7.80),
('SV007', 'HP01', 1, 6.90), ('SV007', 'HP05', 2, 6.70),
('SV008', 'HP01', 1, 8.40), ('SV008', 'HP03', 1, 8.00), ('SV008', 'HP06', 3, 8.30),
('SV009', 'HP05', 2, 5.40),
('SV010', 'HP02', 1, 8.90), ('SV010', 'HP06', 2, 8.60),
('SV011', 'HP01', 1, 7.30), ('SV011', 'HP04', 2, 7.00),
('SV012', 'HP05', 2, 6.20);
GO

/* ---------------------------------------------------------------------
   BAI 6 (tu thuc hien): THU DU LIEU VI PHAM RANG BUOC
   Cac cau lenh duoi day CO Y vi pham. Chung duoc boc trong TRY/CATCH
   de chay thanh cong va in thong bao loi, chung minh rang buoc hoat dong.
   --------------------------------------------------------------------- */
PRINT N'--- Thu 1: DiemTB = 12 (vi pham CK_SinhVien_Diem) ---';
BEGIN TRY
    INSERT INTO SinhVien(MaSV, HoTen, NgaySinh, GioiTinh, MaKhoa, DiemTB, Email)
    VALUES ('SV999', N'Nguyễn Văn Sai', '2005-01-01', 1, 'CNTT', 12, NULL);
    PRINT N'  => KHONG chan (sai)!';
END TRY
BEGIN CATCH
    PRINT N'  => Da chan dung: ' + ERROR_MESSAGE();
END CATCH;
GO

PRINT N'--- Thu 2: Email trung (vi pham UQ_SinhVien_Email) ---';
BEGIN TRY
    INSERT INTO SinhVien(MaSV, HoTen, NgaySinh, GioiTinh, MaKhoa, DiemTB, Email)
    VALUES ('SV998', N'Trần Email Trùng', '2005-01-01', 1, 'CNTT', 7, 'an@eaut.edu.vn');
    PRINT N'  => KHONG chan (sai)!';
END TRY
BEGIN CATCH
    PRINT N'  => Da chan dung: ' + ERROR_MESSAGE();
END CATCH;
GO

PRINT N'--- Thu 3: MaKhoa khong ton tai (vi pham FK_SinhVien_Khoa) ---';
BEGIN TRY
    INSERT INTO SinhVien(MaSV, HoTen, NgaySinh, GioiTinh, MaKhoa, DiemTB, Email)
    VALUES ('SV997', N'Lê Khoa Lạ', '2005-01-01', 1, 'XYZ', 7, NULL);
    PRINT N'  => KHONG chan (sai)!';
END TRY
BEGIN CATCH
    PRINT N'  => Da chan dung: ' + ERROR_MESSAGE();
END CATCH;
GO

PRINT N'--- Thu 4: HocKy = 5 (vi pham CK_DangKy_HocKy) ---';
BEGIN TRY
    INSERT INTO DangKy(MaSV, MaHP, HocKy, Diem) VALUES ('SV001', 'HP05', 5, 8);
    PRINT N'  => KHONG chan (sai)!';
END TRY
BEGIN CATCH
    PRINT N'  => Da chan dung: ' + ERROR_MESSAGE();
END CATCH;
GO

PRINT N'--- Thu 5: SoTinChi = 10 (vi pham CK_HocPhan_SoTinChi) ---';
BEGIN TRY
    INSERT INTO HocPhan(MaHP, TenHP, SoTinChi) VALUES ('HP99', N'Học phần lạ', 10);
    PRINT N'  => KHONG chan (sai)!';
END TRY
BEGIN CATCH
    PRINT N'  => Da chan dung: ' + ERROR_MESSAGE();
END CATCH;
GO

/* ---------------------------------------------------------------------
   KIEM TRA: In so luong ban ghi va vai truy van minh hoa
   --------------------------------------------------------------------- */
PRINT N'';
PRINT N'==================== KIEM TRA DU LIEU MAU ====================';
SELECT N'Khoa'    AS Bang, COUNT(*) AS SoBanGhi FROM Khoa
UNION ALL SELECT N'SinhVien', COUNT(*) FROM SinhVien
UNION ALL SELECT N'HocPhan',  COUNT(*) FROM HocPhan
UNION ALL SELECT N'DangKy',   COUNT(*) FROM DangKy;
GO

PRINT N'';
PRINT N'--- Danh sach sinh vien kem ten khoa (JOIN) ---';
SELECT sv.MaSV, sv.HoTen, k.TenKhoa, sv.DiemTB, sv.Email
FROM SinhVien sv
JOIN Khoa k ON k.MaKhoa = sv.MaKhoa
ORDER BY sv.DiemTB DESC;
GO

PRINT N'';
PRINT N'--- Diem trung binh tung khoa ---';
SELECT k.MaKhoa, k.TenKhoa,
       COUNT(sv.MaSV)      AS SoSV,
       AVG(sv.DiemTB)      AS DiemTrungBinh
FROM Khoa k
LEFT JOIN SinhVien sv ON sv.MaKhoa = k.MaKhoa
GROUP BY k.MaKhoa, k.TenKhoa
ORDER BY k.MaKhoa;
GO

PRINT N'';
PRINT N'--- Bang diem theo sinh vien ---';
SELECT dk.MaSV, sv.HoTen, dk.MaHP, hp.TenHP, dk.HocKy, dk.Diem, dk.NgayDangKy
FROM DangKy dk
JOIN SinhVien sv ON sv.MaSV = dk.MaSV
JOIN HocPhan hp  ON hp.MaHP = dk.MaHP
ORDER BY dk.MaSV, dk.MaHP;
GO

/* ---------------------------------------------------------------------
   BAI 10 (tu thuc hien): SAO LUU CO SO DU LIEU (BACKUP)
   Chay phan nay tren SQL Server that (khong chay tren LocalDB neu khong
   co quyen ghi thu muc). Doi duong dan theo may cua ban.
   --------------------------------------------------------------------- */
-- BACKUP DATABASE QLSinhVien
-- TO DISK = N'C:\Backup\QLSinhVien.bak'
-- WITH INIT, NAME = N'QLSinhVien-Full Backup';
-- GO
--
-- -- Sau khi xoa thu mot ban ghi, phuc hoi lai:
-- -- RESTORE DATABASE QLSinhVien
-- -- FROM DISK = N'C:\Backup\QLSinhVien.bak'
-- -- WITH REPLACE, RECOVERY;
-- -- GO

PRINT N'';
PRINT N'===== HOAN TAT TAO CSDL QLSinhVien =====';
GO
