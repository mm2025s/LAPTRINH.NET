/* =====================================================================
   Lab 08 - IT3212 (Lap trinh .NET) - Chuong 4
   SQLCOMMAND, SQLPARAMETER VA CAC PHUONG THUC THUC THI
   ---------------------------------------------------------------------
   Chay SAU khi da co CSDL QLSinhVien cua Lab 7.

   Cach chay:
     - SSMS  : mo tep nay, nhan Execute (F5).
     - sqlcmd: sqlcmd -S (localdb)\MSSQLLocalDB -E -i Lab08_StoredProcedures.sql

   Noi dung:
     1. Bo sung >= 10 sinh vien de thu loc / thong ke (bai 4 chuan bi).
     2. sp_SinhVien_TheoKhoa  - bai 5: goi Stored Procedure co tham so.
     3. sp_SinhVien_TimTheoMa - bai 6: tim theo ma bang tham so.
     4. sp_SinhVien_Them      - bai 9: them sinh vien + OUTPUT so luong hien tai.

   Luu y: tep chay lai nhieu lan duoc (idempotent).
   ===================================================================== */

USE QLSinhVien;
GO

-- Filtered index (unique index co dieu kien) doi hoi QUOTED_IDENTIFIER ON.
-- sqlcmd mac dinh la OFF, nen phai bat truoc khi tao index.
SET QUOTED_IDENTIFIER ON;
GO

SET NOCOUNT ON;
GO

/* ---------------------------------------------------------------------
   BUOC 0: Kiem tra CSDL Lab 7 da ton tai chua
   --------------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.SinhVien', N'U') IS NULL
BEGIN
    RAISERROR(N'Chua co bang dbo.SinhVien. Hay chay Lap7/SQL/Lab07_QLSinhVien.sql truoc.', 16, 1);
    RETURN;
END
GO

/* ---------------------------------------------------------------------
   BUOC 1: Cho phep NHIEU sinh vien de trong email
   ---------------------------------------------------------------------
   Rang buoc UNIQUE cua Lab 7 chi cho phep MOT gia tri NULL trong cot Email.
   Trong khi do bai 3 (them sinh vien) truyen DBNull.Value khi bo trong email
   -> sinh viên thu hai se bi loi "Cannot insert duplicate key ... (<NULL>)".

   Giai phap dung: thay UNIQUE constraint bang UNIQUE INDEX CO DIEU KIEN
       WHERE Email IS NOT NULL
   -> email van khong duoc trung, nhung bao nhieu ban ghi NULL cung duoc.
   --------------------------------------------------------------------- */
IF EXISTS (SELECT 1 FROM sys.key_constraints
           WHERE name = 'UQ_SinhVien_Email' AND parent_object_id = OBJECT_ID('dbo.SinhVien'))
BEGIN
    ALTER TABLE dbo.SinhVien DROP CONSTRAINT UQ_SinhVien_Email;
    PRINT N'Da bo rang buoc UNIQUE UQ_SinhVien_Email.';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = 'UX_SinhVien_Email' AND object_id = OBJECT_ID('dbo.SinhVien'))
BEGIN
    CREATE UNIQUE INDEX UX_SinhVien_Email ON dbo.SinhVien (Email) WHERE Email IS NOT NULL;
    PRINT N'Da tao UNIQUE INDEX co dieu kien UX_SinhVien_Email (WHERE Email IS NOT NULL).';
END
GO

/* ---------------------------------------------------------------------
   BUOC 2: Bo sung 10 sinh vien nua de thu loc va thong ke
   ---------------------------------------------------------------------
   12 SV cua Lab 7 + 10 SV duoi day = 22 sinh vien.
   Cac ma SV013..SV022 chi duoc them khi chua ton tai.
   --------------------------------------------------------------------- */
IF NOT EXISTS (SELECT 1 FROM dbo.SinhVien WHERE MaSV = 'SV013')
BEGIN
    INSERT INTO dbo.SinhVien (MaSV, HoTen, NgaySinh, GioiTinh, MaKhoa, DiemTB, Email) VALUES
    ('SV013', N'Phan Thanh Mai',   '2005-02-11', 0, 'CNTT', 9.35, 'mai@eaut.edu.vn'),
    ('SV014', N'Đinh Công Minh',   '2005-03-22', 1, 'CNTT', 6.45, 'minh@eaut.edu.vn'),
    ('SV015', N'Trương Mỹ Duyên',  '2005-05-17', 0, 'KT',   8.05, 'duyen@eaut.edu.vn'),
    ('SV016', N'Hồ Nhật Nam',      '2004-09-09', 1, 'OTO',  7.65, 'nam@eaut.edu.vn'),
    ('SV017', N'Lâm Thu Trang',    '2005-07-04', 0, 'CNTT', 9.55, 'trang@eaut.edu.vn'),
    ('SV018', N'Võ Đức Thịnh',     '2005-08-25', 1, 'OTO',  5.85, 'thinh@eaut.edu.vn'),
    ('SV019', N'Nguyễn Kim Oanh',  '2005-10-13', 0, 'KT',   7.25, 'oanh@eaut.edu.vn'),
    ('SV020', N'Trần Hải Đăng',    '2004-12-28', 1, 'CNTT', 8.60, 'dang@eaut.edu.vn'),
    ('SV021', N'Bùi Phương Thảo',  '2005-01-06', 0, 'KT',   6.95, 'thao@eaut.edu.vn'),
    ('SV022', N'Cao Xuân Trường',  '2005-11-19', 1, 'OTO',  8.15, 'truong@eaut.edu.vn');

    PRINT N'Da bo sung 10 sinh vien (SV013 - SV022).';
END
ELSE
    PRINT N'10 sinh vien bo sung da co, bo qua buoc nay.';
GO

/* ---------------------------------------------------------------------
   BAI 5: sp_SinhVien_TheoKhoa
   Tim sinh vien theo khoa. Ten tham so trong C# phai khop @MaKhoa.
   --------------------------------------------------------------------- */
CREATE OR ALTER PROCEDURE dbo.sp_SinhVien_TheoKhoa
    @MaKhoa VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT sv.MaSV, sv.HoTen, sv.NgaySinh, sv.GioiTinh, sv.MaKhoa, sv.DiemTB, sv.Email
    FROM dbo.SinhVien sv
    WHERE sv.MaKhoa = @MaKhoa
    ORDER BY sv.HoTen;
END;
GO

/* ---------------------------------------------------------------------
   BAI 6: sp_SinhVien_TimTheoMa
   Tra ve toi da 1 dong. Khong tim thay -> 0 dong (GUI bao "khong tim thay").
   --------------------------------------------------------------------- */
CREATE OR ALTER PROCEDURE dbo.sp_SinhVien_TimTheoMa
    @MaSV VARCHAR(15)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT sv.MaSV, sv.HoTen, sv.NgaySinh, sv.GioiTinh,
           sv.MaKhoa, k.TenKhoa, sv.DiemTB, sv.Email
    FROM dbo.SinhVien sv
    JOIN dbo.Khoa k ON k.MaKhoa = sv.MaKhoa
    WHERE sv.MaSV = @MaSV;
END;
GO

/* ---------------------------------------------------------------------
   BAI 9: sp_SinhVien_Them
   Them 1 sinh vien bang tham so hoa va tra ve so luong sinh vien hien co
   qua tham so OUTPUT @SoLuongHienTai.

   - @Email co the NULL (GUI truyen DBNull.Value).
   - Loi rang buoc (trung khoa chinh, trung email, sai khoa) duoc nem ra
     de tang C# bat SqlException va hien thi than thien.
   --------------------------------------------------------------------- */
CREATE OR ALTER PROCEDURE dbo.sp_SinhVien_Them
    @MaSV       VARCHAR(15),
    @HoTen      NVARCHAR(100),
    @NgaySinh   DATE,
    @GioiTinh   BIT,
    @MaKhoa     VARCHAR(10),
    @DiemTB     DECIMAL(4, 2),
    @Email      VARCHAR(100) = NULL,
    @SoLuongHienTai INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.SinhVien (MaSV, HoTen, NgaySinh, GioiTinh, MaKhoa, DiemTB, Email)
    VALUES (@MaSV, @HoTen, @NgaySinh, @GioiTinh, @MaKhoa, @DiemTB, @Email);

    SELECT @SoLuongHienTai = COUNT(*) FROM dbo.SinhVien;
END;
GO

/* ---------------------------------------------------------------------
   KIEM TRA: in danh sach doi tuong vua tao + so lieu
   --------------------------------------------------------------------- */
PRINT N'';
PRINT N'==================== KIEM TRA LAB 8 ====================';
SELECT name AS [Doi tuong], type_desc AS [Loai]
FROM sys.objects
WHERE name IN ('sp_SinhVien_TheoKhoa', 'sp_SinhVien_TimTheoMa', 'sp_SinhVien_Them')
ORDER BY name;

SELECT COUNT(*) AS TongSinhVien FROM dbo.SinhVien;

PRINT N'--- Thu goi sp_SinhVien_TheoKhoa voi MaKhoa = CNTT ---';
EXEC dbo.sp_SinhVien_TheoKhoa @MaKhoa = 'CNTT';
GO

/* ---------------------------------------------------------------------
   Thu nghiem tham so OUTPUT bang T-SQL (truong hop GUI lam tuong tu)
   --------------------------------------------------------------------- */
DECLARE @SoLuong INT;
BEGIN TRY
    EXEC dbo.sp_SinhVien_Them
        @MaSV = 'SV900', @HoTen = N'Nguyễn Thử Nghiệm', @NgaySinh = '2005-01-01',
        @GioiTinh = 1, @MaKhoa = 'CNTT', @DiemTB = 7.50, @Email = NULL,
        @SoLuongHienTai = @SoLuong OUTPUT;

    PRINT N'  => Them thanh cong. So sinh vien hien tai: ' + CAST(@SoLuong AS VARCHAR(10));

    -- Don ban ghi thu nghiem de khong lam thay doi du lieu mau
    DELETE FROM dbo.SinhVien WHERE MaSV = 'SV900';
    PRINT N'  => Da xoa ban ghi thu nghiem SV900.';
END TRY
BEGIN CATCH
    PRINT N'  => Loi: ' + ERROR_MESSAGE();
END CATCH;
GO

PRINT N'';
PRINT N'===== HOAN TAT LAB 8 (Stored Procedures + du lieu thu) =====';
GO
