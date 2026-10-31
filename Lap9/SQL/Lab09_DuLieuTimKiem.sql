/* ============================================================================
   LAB 9 - IT3212 - LAP TRINH .NET
   Doc, tim kiem va loc du lieu bang SqlDataReader (Chuong 4)

   Muc dich: bo sung du lieu du LON cho CSDL QLSinhVien de bai phan trang
   (10 sinh vien / trang) co it nhat 3 trang.

   Sau khi chay script nay: QLSinhVien co 46 sinh vien -> 5 trang.
   Script chay lai nhieu lan deu an toan (kiem tra NOT EXISTS theo MaSV).

   Cach chay:
       sqlcmd -S "(localdb)\MSSQLLocalDB" -E -i Lap9\SQL\Lab09_DuLieuTimKiem.sql -f 65001
   ============================================================================ */

USE QLSinhVien;
GO

-- Bang dbo.SinhVien co UNIQUE INDEX co dieu kien (UX_SinhVien_Email, tao o Lab 8)
-- nen MOI thao tac ghi (INSERT/UPDATE) deu phai chay voi QUOTED_IDENTIFIER ON.
-- sqlcmd mac dinh la OFF -> thieu dong nay se bao loi 1934.
SET QUOTED_IDENTIFIER ON;
GO

SET NOCOUNT ON;
GO

PRINT N'--- BO SUNG DU LIEU CHO BAI PHAN TRANG / TIM KIEM / SAP XEP ---';
GO

/* ---------------------------------------------------------------------------
   Sinh vien SV023 -> SV046.
   Du lieu duoc chon co chu dich:
     - NgaySinh rai deu 2000..2007  -> bai 8 (loc theo tuoi) co nhieu nhom tuoi.
     - DiemTB co ca duoi 5.0 (to do) va tu 8.0 tro len (to xanh) -> bai 9.
     - GioiTinh co ca Nam va Nu        -> bai 6 (tim kiem da tieu chi).
     - MaKhoa phu ca 3 khoa            -> bai 3 (loc theo khoa).
     - Mot so Email de trong (NULL)    -> kiem tra hien thi DBNull.
   --------------------------------------------------------------------------- */
DECLARE @Sv TABLE
(
    MaSV     VARCHAR(15)  PRIMARY KEY,
    HoTen    NVARCHAR(100) NOT NULL,
    NgaySinh DATE          NOT NULL,
    GioiTinh BIT           NOT NULL,
    MaKhoa   VARCHAR(10)   NOT NULL,
    DiemTB   DECIMAL(4,2)  NOT NULL,
    Email    VARCHAR(100)  NULL
);

INSERT INTO @Sv (MaSV, HoTen, NgaySinh, GioiTinh, MaKhoa, DiemTB, Email) VALUES
 ('SV023', N'Nguyễn Hoàng Bảo',    '2004-03-12', 1, 'CNTT', 7.85, 'bao.nh@sv.edu.vn'),
 ('SV024', N'Trần Khánh Linh',     '2005-07-21', 0, 'KT',   8.60, 'linh.tk@sv.edu.vn'),
 ('SV025', N'Lê Minh Quân',        '2003-11-02', 1, 'OTO',  6.40, NULL),
 ('SV026', N'Phạm Thu Hà',         '2006-01-15', 0, 'CNTT', 9.15, 'ha.pt@sv.edu.vn'),
 ('SV027', N'Vũ Đức Thắng',        '2002-09-30', 1, 'CNTT', 4.75, 'thang.vd@sv.edu.vn'),
 ('SV028', N'Hoàng Mai Phương',    '2005-05-08', 0, 'KT',   7.20, NULL),
 ('SV029', N'Đặng Quốc Huy',       '2004-12-19', 1, 'OTO',  8.05, 'huy.dq@sv.edu.vn'),
 ('SV030', N'Bùi Thị Ngọc',        '2007-02-27', 0, 'CNTT', 3.90, 'ngoc.bt@sv.edu.vn'),
 ('SV031', N'Ngô Văn Khánh',       '2003-06-14', 1, 'KT',   5.60, NULL),
 ('SV032', N'Đỗ Hồng Nhung',       '2006-08-03', 0, 'OTO',  8.85, 'nhung.dh@sv.edu.vn'),
 ('SV033', N'Trịnh Công Sơn',      '2001-04-25', 1, 'CNTT', 6.95, 'son.tc@sv.edu.vn'),
 ('SV034', N'Lý Thị Kim Chi',      '2005-10-11', 0, 'KT',   9.05, 'chi.ltk@sv.edu.vn'),
 ('SV035', N'Phan Tuấn Anh',       '2004-02-06', 1, 'OTO',  4.20, NULL),
 ('SV036', N'Đinh Thu Trang',      '2006-11-23', 0, 'CNTT', 7.60, 'trang.dt@sv.edu.vn'),
 ('SV037', N'Cao Xuân Trường',     '2002-07-17', 1, 'KT',   8.30, 'truong.cx@sv.edu.vn'),
 ('SV038', N'Trương Mỹ Duyên',     '2007-05-04', 0, 'OTO',  6.10, NULL),
 ('SV039', N'Lương Văn Hùng',      '2003-01-29', 1, 'CNTT', 5.35, 'hung.lv@sv.edu.vn'),
 ('SV040', N'Tạ Khánh Vy',         '2005-09-09', 0, 'KT',   9.40, 'vy.tk@sv.edu.vn'),
 ('SV041', N'Hồ Nhật Nam',         '2004-08-22', 1, 'OTO',  7.05, NULL),
 ('SV042', N'Võ Thị Ánh Tuyết',    '2006-03-16', 0, 'CNTT', 8.15, 'tuyet.vt@sv.edu.vn'),
 ('SV043', N'Dương Chí Cường',     '2001-12-08', 1, 'KT',   4.65, 'cuong.dc@sv.edu.vn'),
 ('SV044', N'Nguyễn Bảo Châu',     '2007-07-30', 0, 'OTO',  9.60, NULL),
 ('SV045', N'Thái Minh Triết',     '2002-10-05', 1, 'CNTT', 5.90, 'triet.tm@sv.edu.vn'),
 ('SV046', N'Lâm Diệu Linh',       '2005-04-18', 0, 'KT',   8.75, 'linh.ld@sv.edu.vn');

INSERT INTO dbo.SinhVien (MaSV, HoTen, NgaySinh, GioiTinh, MaKhoa, DiemTB, Email)
SELECT s.MaSV, s.HoTen, s.NgaySinh, s.GioiTinh, s.MaKhoa, s.DiemTB, s.Email
FROM @Sv s
WHERE NOT EXISTS (SELECT 1 FROM dbo.SinhVien x WHERE x.MaSV = s.MaSV);

PRINT N'  => Da bo sung xong sinh vien SV023 - SV046.';
GO

/* ---------------------------------------------------------------------------
   Kiem tra ket qua
   --------------------------------------------------------------------------- */
SELECT
    COUNT(*)                                        AS TongSoSinhVien,
    SUM(CASE WHEN DiemTB < 5 THEN 1 ELSE 0 END)     AS DuoiTrungBinh,
    SUM(CASE WHEN DiemTB >= 8 THEN 1 ELSE 0 END)    AS KhaGioi,
    SUM(CASE WHEN Email IS NULL THEN 1 ELSE 0 END)  AS ChuaCoEmail,
    COUNT(DISTINCT MaKhoa)                          AS SoKhoa,
    MIN(YEAR(NgaySinh))                             AS NamSinhNhoNhat,
    MAX(YEAR(NgaySinh))                             AS NamSinhLonNhat
FROM dbo.SinhVien;
GO

PRINT N'--- SO DONG MOI TRANG KHI KICH THUOC TRANG = 10 ---';
SELECT CEILING(COUNT(*) / 10.0) AS SoTrang FROM dbo.SinhVien;
GO

/* ---------------------------------------------------------------------------
   Minh hoa nhanh 2 ky thuat cot loi cua Lab 9 (chay duoc ngay trong sqlcmd)
   --------------------------------------------------------------------------- */

-- 1) Phan trang bang OFFSET - FETCH (ORDER BY la bat buoc)
PRINT N'--- TRANG 2 (OFFSET 10 ROWS FETCH NEXT 10 ROWS ONLY) ---';
SELECT sv.MaSV, sv.HoTen,
       CASE sv.GioiTinh WHEN 1 THEN N'Nam' ELSE N'Nữ' END AS GioiTinh,
       k.TenKhoa, sv.DiemTB
FROM dbo.SinhVien sv
JOIN dbo.Khoa k ON sv.MaKhoa = k.MaKhoa
ORDER BY sv.MaSV
OFFSET 10 ROWS FETCH NEXT 10 ROWS ONLY;
GO

-- 2) Tuoi chinh xac (da qua sinh nhat trong nam nay hay chua)
PRINT N'--- TUOI CHINH XAC (khong dung DATEDIFF nam don thuan) ---';
SELECT TOP (5) MaSV, HoTen, NgaySinh,
       DATEDIFF(YEAR, NgaySinh, GETDATE())
         - CASE WHEN DATEADD(YEAR, DATEDIFF(YEAR, NgaySinh, GETDATE()), NgaySinh) > CAST(GETDATE() AS DATE)
                THEN 1 ELSE 0 END AS TuoiChinhXac
FROM dbo.SinhVien
ORDER BY NgaySinh;
GO

PRINT N'--- HOAN TAT SCRIPT DU LIEU LAB 9 ---';
GO
