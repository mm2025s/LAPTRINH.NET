/* =====================================================================
   Lab 11 - IT3212 (Lap trinh .NET) - Chuong 4
   CRUD THAM SO HOA VA STORED PROCEDURE
   ---------------------------------------------------------------------
   Chay SAU cac script Lab 7, Lab 8, Lab 9 (CSDL QLSinhVien).

   TRUOC KHI CHAY - sao luu CSDL (muc 4 cua de bai), vi du:
     BACKUP DATABASE QLSinhVien
       TO DISK = N'D:\LAPTRINH.NET\Lap11\SQL\Backup\QLSinhVien_TruocLab11.bak'
       WITH INIT, CHECKSUM;

   Cach chay:
     - SSMS  : mo tep nay, nhan Execute (F5).
     - sqlcmd: sqlcmd -S (localdb)\MSSQLLocalDB -E -f 65001 -i Lab11_StoredProcedures.sql

   Tep luu UTF-8 co BOM de chuoi N'...' tieng Viet di dung vao CSDL.
   Chay lai nhieu lan duoc (idempotent), khong xoa du lieu.

   Bang ma loi nghiep vu (THROW):
     50001  Ma sinh vien da ton tai
     50002  Khong tim thay sinh vien (dang hoat dong)
     50003  Khong tim thay sinh vien da xoa de khoi phuc
     50004  Khong tim thay khoa
     50005  Ma khoa da ton tai
     50006  Ten khoa da ton tai
     50007  Khoa dang co sinh vien, khong duoc xoa
   ===================================================================== */

USE QLSinhVien;
GO

-- Thu tuc ghi len bang co FILTERED INDEX (UX_SinhVien_Email) phai duoc tao
-- voi QUOTED_IDENTIFIER ON, neu khong se gap loi 1934 luc chay. sqlcmd mac
-- dinh la OFF, nen phai bat tuong minh o day.
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

/* ---------------------------------------------------------------------
   BUOC 0: Kiem tra CSDL Lab 7 da ton tai chua
   --------------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.SinhVien', N'U') IS NULL OR OBJECT_ID(N'dbo.Khoa', N'U') IS NULL
BEGIN
    RAISERROR(N'Chua co bang SinhVien/Khoa. Hay chay Lap7/SQL/Lab07_QLSinhVien.sql truoc.', 16, 1);
    SET NOEXEC ON;
END
GO

/* =====================================================================
   PHAN A - THAY DOI CAU TRUC (dat truoc vi cac thu tuc ben duoi dung toi)
   ===================================================================== */

/* BAI 9: cot IsActive phuc vu xoa mem. Du lieu cu nhan DEFAULT 1. */
IF COL_LENGTH('dbo.SinhVien', 'IsActive') IS NULL
BEGIN
    ALTER TABLE dbo.SinhVien
        ADD IsActive BIT NOT NULL CONSTRAINT DF_SinhVien_IsActive DEFAULT 1;
    PRINT N'Da them cot SinhVien.IsActive.';
END
GO

/* BAI 8: rang buoc CSDL bao dam email khong trung.
   Lab 7 dung UNIQUE constraint (chi cho phep MOT dong Email = NULL); Lab 8
   da doi sang UNIQUE INDEX co dieu kien. Doan nay bao dam ket qua dung ca khi
   CSDL moi chi chay script Lab 7.

   Index tinh tren MOI dong, ke ca dong da xoa mem: sinh vien xoa mem co the
   duoc khoi phuc, nen email cua ho van phai duoc giu cho. */
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
    PRINT N'Da tao UNIQUE INDEX UX_SinhVien_Email (WHERE Email IS NOT NULL).';
END
GO

/* =====================================================================
   PHAN B - STORED PROCEDURE CRUD SINH VIEN
   ===================================================================== */

/* ---------------------------------------------------------------------
   BAI 1: Them sinh vien. Kiem tra trung ma va dung THROW bao loi nghiep vu.

   KHAC CODE MAU: khong co SET NOCOUNT ON. Bai 3 yeu cau DAL tra ve so dong
   anh huong qua ExecuteNonQuery(); neu bat NOCOUNT thi ham nay luon tra -1.
   --------------------------------------------------------------------- */
CREATE OR ALTER PROCEDURE dbo.sp_SinhVien_Insert
    @MaSV VARCHAR(15), @HoTen NVARCHAR(100), @NgaySinh DATE,
    @GioiTinh BIT, @MaKhoa VARCHAR(10), @DiemTB DECIMAL(4,2),
    @Email VARCHAR(100) = NULL
AS
BEGIN
    -- Ma cua sinh vien da xoa mem van nam trong bang, can bao ro de nguoi dung
    -- khoi phuc thay vi thu lai ma do mai.
    IF EXISTS (SELECT 1 FROM dbo.SinhVien WHERE MaSV = @MaSV AND IsActive = 0)
        THROW 50001, N'Mã sinh viên đã tồn tại (sinh viên này đã bị xóa, hãy khôi phục thay vì thêm mới).', 1;

    IF EXISTS (SELECT 1 FROM dbo.SinhVien WHERE MaSV = @MaSV)
        THROW 50001, N'Mã sinh viên đã tồn tại.', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.Khoa WHERE MaKhoa = @MaKhoa)
        THROW 50004, N'Khoa không tồn tại.', 1;

    INSERT INTO dbo.SinhVien (MaSV, HoTen, NgaySinh, GioiTinh, MaKhoa, DiemTB, Email)
    VALUES (@MaSV, @HoTen, @NgaySinh, @GioiTinh, @MaKhoa, @DiemTB, @Email);
END;
GO

/* ---------------------------------------------------------------------
   BAI 2: Cap nhat sinh vien. MaSV chi nam trong WHERE: khong doi khoa chinh.
   Bai 9: them dieu kien IsActive = 1, sinh vien da xoa khong sua duoc.
   --------------------------------------------------------------------- */
CREATE OR ALTER PROCEDURE dbo.sp_SinhVien_Update
    @MaSV VARCHAR(15), @HoTen NVARCHAR(100), @NgaySinh DATE,
    @GioiTinh BIT, @MaKhoa VARCHAR(10), @DiemTB DECIMAL(4,2),
    @Email VARCHAR(100) = NULL
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.Khoa WHERE MaKhoa = @MaKhoa)
        THROW 50004, N'Khoa không tồn tại.', 1;

    UPDATE dbo.SinhVien
    SET HoTen = @HoTen, NgaySinh = @NgaySinh, GioiTinh = @GioiTinh,
        MaKhoa = @MaKhoa, DiemTB = @DiemTB, Email = @Email
    WHERE MaSV = @MaSV AND IsActive = 1;

    IF @@ROWCOUNT = 0 THROW 50002, N'Không tìm thấy sinh viên.', 1;
END;
GO

/* ---------------------------------------------------------------------
   BAI 2 + BAI 9: Xoa sinh vien.

   Ban cua Bai 2 la xoa vat ly:
       DELETE FROM SinhVien WHERE MaSV = @MaSV;
   Bai 9 yeu cau THAY bang cap nhat trang thai, nen than thu tuc doi thanh
   UPDATE nhung giu nguyen ten, tham so va ma loi 50002. Nho do DAL/BUS/GUI
   khong phai sua mot dong nao: day chinh la loi ich cua Stored Procedure.
   --------------------------------------------------------------------- */
CREATE OR ALTER PROCEDURE dbo.sp_SinhVien_Delete
    @MaSV VARCHAR(15)
AS
BEGIN
    UPDATE dbo.SinhVien SET IsActive = 0
    WHERE MaSV = @MaSV AND IsActive = 1;

    IF @@ROWCOUNT = 0 THROW 50002, N'Không tìm thấy sinh viên.', 1;
END;
GO

/* BAI 9: Khoi phuc sinh vien da xoa mem. */
CREATE OR ALTER PROCEDURE dbo.sp_SinhVien_Restore
    @MaSV VARCHAR(15)
AS
BEGIN
    UPDATE dbo.SinhVien SET IsActive = 1
    WHERE MaSV = @MaSV AND IsActive = 0;

    IF @@ROWCOUNT = 0 THROW 50003, N'Không tìm thấy sinh viên đã xóa để khôi phục.', 1;
END;
GO

/* BAI 5 + BAI 9: Danh sach sinh vien. Mac dinh chi lay sinh vien dang hoat dong;
   @IsActive = 0 lay danh sach da xoa mem. */
CREATE OR ALTER PROCEDURE dbo.sp_SinhVien_GetAll
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    SELECT sv.MaSV, sv.HoTen, sv.NgaySinh, sv.GioiTinh, sv.MaKhoa, k.TenKhoa,
           sv.DiemTB, sv.Email, sv.IsActive
    FROM dbo.SinhVien sv
    JOIN dbo.Khoa k ON k.MaKhoa = sv.MaKhoa
    WHERE sv.IsActive = @IsActive
    ORDER BY sv.MaSV;
END;
GO

/* BAI 2: Tai mot sinh vien theo ma (ke ca da xoa mem, de hien thi dung trang thai). */
CREATE OR ALTER PROCEDURE dbo.sp_SinhVien_GetById
    @MaSV VARCHAR(15)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT sv.MaSV, sv.HoTen, sv.NgaySinh, sv.GioiTinh, sv.MaKhoa, k.TenKhoa,
           sv.DiemTB, sv.Email, sv.IsActive
    FROM dbo.SinhVien sv
    JOIN dbo.Khoa k ON k.MaKhoa = sv.MaKhoa
    WHERE sv.MaSV = @MaSV;
END;
GO

/* ---------------------------------------------------------------------
   BAI 7: Tim kiem theo tu khoa va khoa; tham so rong tra ve tat ca.

   - NULL, chuoi rong va chuoi toan khoang trang deu duoc coi la "khong loc".
   - Ky tu dai dien cua LIKE trong tu khoa duoc vo hieu hoa (ESCAPE), nen go
     % hay _ se tim dung ky tu do thay vi tra ve toan bo bang.
   - Khong dung SQL dong nen khong co nguy co SQL Injection.
   --------------------------------------------------------------------- */
CREATE OR ALTER PROCEDURE dbo.sp_SinhVien_Search
    @TuKhoa NVARCHAR(100) = NULL,
    @MaKhoa VARCHAR(10)   = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SET @TuKhoa = NULLIF(LTRIM(RTRIM(@TuKhoa)), N'');
    SET @MaKhoa = NULLIF(LTRIM(RTRIM(@MaKhoa)), '');

    DECLARE @Mau NVARCHAR(310) =
        N'%' + REPLACE(REPLACE(REPLACE(REPLACE(@TuKhoa,
            N'\', N'\\'), N'%', N'\%'), N'_', N'\_'), N'[', N'\[') + N'%';

    SELECT sv.MaSV, sv.HoTen, sv.NgaySinh, sv.GioiTinh, sv.MaKhoa, k.TenKhoa,
           sv.DiemTB, sv.Email, sv.IsActive
    FROM dbo.SinhVien sv
    JOIN dbo.Khoa k ON k.MaKhoa = sv.MaKhoa
    WHERE sv.IsActive = 1
      AND (@MaKhoa IS NULL OR sv.MaKhoa = @MaKhoa)
      AND (@Mau IS NULL
           OR sv.MaSV  LIKE @Mau ESCAPE N'\'
           OR sv.HoTen LIKE @Mau ESCAPE N'\'
           OR sv.Email LIKE @Mau ESCAPE N'\')
    ORDER BY sv.MaSV;
END;
GO

/* BAI 8: Kiem tra email da duoc dung chua (ke ca boi sinh vien da xoa mem).
   @MaSVBoQua: khi cap nhat, bo qua chinh sinh vien dang sua. */
CREATE OR ALTER PROCEDURE dbo.sp_SinhVien_EmailExists
    @Email     VARCHAR(100),
    @MaSVBoQua VARCHAR(15) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CASE WHEN EXISTS (SELECT 1 FROM dbo.SinhVien
                             WHERE Email = @Email
                               AND (@MaSVBoQua IS NULL OR MaSV <> @MaSVBoQua))
                THEN 1 ELSE 0 END AS DaTonTai;
END;
GO

/* BAI 8: Thong tin rang buoc email tren CSDL, hien thi o Form Bai 8. */
CREATE OR ALTER PROCEDURE dbo.sp_SinhVien_EmailIndexInfo
AS
BEGIN
    SET NOCOUNT ON;

    SELECT i.name AS TenIndex, i.is_unique AS LaDuyNhat, i.filter_definition AS DieuKien
    FROM sys.indexes i
    WHERE i.object_id = OBJECT_ID('dbo.SinhVien') AND i.name = 'UX_SinhVien_Email';
END;
GO

/* =====================================================================
   PHAN C - STORED PROCEDURE CRUD KHOA (BAI 6)
   ===================================================================== */

/* SoDaXoa duoc tra ve rieng vi khoa co sinh vien da xoa mem van khong xoa duoc. */
CREATE OR ALTER PROCEDURE dbo.sp_Khoa_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT k.MaKhoa, k.TenKhoa,
           SUM(CASE WHEN sv.IsActive = 1 THEN 1 ELSE 0 END) AS SoSinhVien,
           SUM(CASE WHEN sv.IsActive = 0 THEN 1 ELSE 0 END) AS SoDaXoa
    FROM dbo.Khoa k
    LEFT JOIN dbo.SinhVien sv ON sv.MaKhoa = k.MaKhoa
    GROUP BY k.MaKhoa, k.TenKhoa
    ORDER BY k.MaKhoa;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Khoa_Insert
    @MaKhoa VARCHAR(10), @TenKhoa NVARCHAR(100)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM dbo.Khoa WHERE MaKhoa = @MaKhoa)
        THROW 50005, N'Mã khoa đã tồn tại.', 1;

    IF EXISTS (SELECT 1 FROM dbo.Khoa WHERE TenKhoa = @TenKhoa)
        THROW 50006, N'Tên khoa đã tồn tại.', 1;

    INSERT INTO dbo.Khoa (MaKhoa, TenKhoa) VALUES (@MaKhoa, @TenKhoa);
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Khoa_Update
    @MaKhoa VARCHAR(10), @TenKhoa NVARCHAR(100)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM dbo.Khoa WHERE TenKhoa = @TenKhoa AND MaKhoa <> @MaKhoa)
        THROW 50006, N'Tên khoa đã tồn tại.', 1;

    UPDATE dbo.Khoa SET TenKhoa = @TenKhoa WHERE MaKhoa = @MaKhoa;

    IF @@ROWCOUNT = 0 THROW 50004, N'Không tìm thấy khoa.', 1;
END;
GO

/* Khong cho xoa khoa dang co sinh vien. Dem CA sinh vien da xoa mem: cac dong
   do van tham chieu toi Khoa qua FK_SinhVien_Khoa nen DELETE se bi tu choi. */
CREATE OR ALTER PROCEDURE dbo.sp_Khoa_Delete
    @MaKhoa VARCHAR(10)
AS
BEGIN
    DECLARE @SoSinhVien INT = (SELECT COUNT(*) FROM dbo.SinhVien WHERE MaKhoa = @MaKhoa);

    IF @SoSinhVien > 0
    BEGIN
        DECLARE @ThongBao NVARCHAR(200) =
            CONCAT(N'Không thể xóa khoa: còn ', @SoSinhVien, N' sinh viên thuộc khoa này (tính cả sinh viên đã xóa).');
        THROW 50007, @ThongBao, 1;
    END

    DELETE FROM dbo.Khoa WHERE MaKhoa = @MaKhoa;

    IF @@ROWCOUNT = 0 THROW 50004, N'Không tìm thấy khoa.', 1;
END;
GO

SET NOEXEC OFF;
GO

PRINT N'Hoan tat script Lab 11.';
GO
