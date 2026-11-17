/* =====================================================================
   DE TAI 14 - Tep 2/2: du lieu mau.
   Tai khoan: admin / admin123 (Quan ly)  -  duocsi / 123456 (Duoc si)
   Hoa don va phieu nhap mau duoc tao QUA chinh sp_PhieuNhap_Tao va
   sp_HoaDon_Tao, nen ton kho luon khop voi chung tu.
   ===================================================================== */
USE QLNhaThuoc;
GO
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
GO

/* Mat khau: SHA2-256 cua (Salt + MatKhau) dang NVARCHAR (UTF-16LE),
   khop voi Encoding.Unicode ben C#. */
DECLARE @s1 NVARCHAR(32) = N'5a1t-admin-2026', @s2 NVARCHAR(32) = N'5a1t-duocsi-2026';
INSERT INTO dbo.NhanVien (HoTen, TenDangNhap, MatKhauHash, Salt, VaiTro) VALUES
 (N'Nguyễn Trường Giang', 'admin',  HASHBYTES('SHA2_256', @s1 + N'admin123'), @s1, 'QuanLy'),
 (N'Trần Thị Mai',        'duocsi', HASHBYTES('SHA2_256', @s2 + N'123456'),   @s2, 'DuocSi');

INSERT INTO dbo.NhomThuoc (MaNhom, TenNhom) VALUES
 ('KS',  N'Kháng sinh'), ('GD', N'Giảm đau - Hạ sốt'), ('VTM', N'Vitamin - Khoáng chất'),
 ('TH',  N'Tiêu hóa'),   ('HH', N'Hô hấp'),            ('DL',  N'Da liễu');

INSERT INTO dbo.NhaCungCap (MaNCC, TenNCC, DienThoai, DiaChi) VALUES
 ('DHG',  N'Công ty CP Dược Hậu Giang', '02923891433', N'288 Nguyễn Văn Cừ, Cần Thơ'),
 ('TRAPH',N'Công ty CP Traphaco',       '02436815555', N'75 Yên Ninh, Hà Nội'),
 ('IMEX', N'Công ty CP Imexpharm',      '02773851941', N'Số 4, đường 30/4, Đồng Tháp'),
 ('PYME', N'Công ty CP Pymepharco',     '02573829165', N'166 - 170 Nguyễn Huệ, Phú Yên');

INSERT INTO dbo.Thuoc (MaThuoc, TenThuoc, MaNhom, DonViTinh, GiaBan, CanKeDon) VALUES
 ('T001', N'Paracetamol 500mg',          'GD',  N'Viên', 1000, 0),
 ('T002', N'Panadol Extra',              'GD',  N'Viên', 2000, 0),
 ('T003', N'Ibuprofen 400mg',            'GD',  N'Viên', 1500, 0),
 ('T004', N'Efferalgan 500mg sủi',       'GD',  N'Viên', 3500, 0),
 ('T005', N'Amoxicillin 500mg',          'KS',  N'Viên', 2500, 1),
 ('T006', N'Cefuroxim 500mg',            'KS',  N'Viên', 9000, 1),
 ('T007', N'Azithromycin 250mg',         'KS',  N'Viên', 8000, 1),
 ('T008', N'Augmentin 625mg',            'KS',  N'Viên', 15000, 1),
 ('T009', N'Vitamin C 500mg',            'VTM', N'Viên', 800, 0),
 ('T010', N'Berocca Performance',        'VTM', N'Tuýp', 95000, 0),
 ('T011', N'Calcium Corbiere 10ml',      'VTM', N'Ống', 6000, 0),
 ('T012', N'Vitamin 3B',                 'VTM', N'Viên', 1200, 0),
 ('T013', N'Smecta',                     'TH',  N'Gói', 4000, 0),
 ('T014', N'Berberin 100mg',             'TH',  N'Lọ', 12000, 0),
 ('T015', N'Omeprazol 20mg',             'TH',  N'Viên', 2000, 1),
 ('T016', N'Men vi sinh Enterogermina',  'TH',  N'Ống', 9500, 0),
 ('T017', N'Siro ho Prospan 100ml',      'HH',  N'Chai', 85000, 0),
 ('T018', N'Eugica đỏ',                  'HH',  N'Viên', 1500, 0),
 ('T019', N'Loratadin 10mg',             'HH',  N'Viên', 1000, 0),
 ('T020', N'Kem Fucicort 15g',           'DL',  N'Tuýp', 98000, 1),
 ('T021', N'Povidine 10% 90ml',          'DL',  N'Chai', 32000, 0),
 ('T022', N'Gel Silkron 10g',            'DL',  N'Tuýp', 18000, 0);

INSERT INTO dbo.KhachHang (HoTen, DienThoai, DiaChi) VALUES
 (N'Lê Văn Hùng',   '0912345678', N'Hà Đông, Hà Nội'),
 (N'Phạm Thu Hà',   '0987654321', N'Thanh Xuân, Hà Nội'),
 (N'Đỗ Minh Quân',  '0901122334', N'Cầu Giấy, Hà Nội'),
 (N'Vũ Thị Lan',    '0933445566', N'Hoàng Mai, Hà Nội'),
 (N'Bùi Đức Thắng', '0977889900', N'Long Biên, Hà Nội');
GO

/* ---------- Nhap hang qua thu tuc (2 phieu) ---------- */
DECLARE @pn dbo.DongPhieuNhapType, @MaPN INT;
INSERT INTO @pn VALUES
 ('T001', 1000, 600,   DATEADD(MONTH, 20, GETDATE())), ('T002', 500, 1300,  DATEADD(MONTH, 18, GETDATE())),
 ('T003', 300,  900,   DATEADD(MONTH, 14, GETDATE())), ('T004', 200, 2400,  DATEADD(DAY, 45, GETDATE())),
 ('T005', 400,  1600,  DATEADD(MONTH, 12, GETDATE())), ('T006', 150, 6200,  DATEADD(MONTH, 16, GETDATE())),
 ('T007', 100,  5500,  DATEADD(MONTH, 10, GETDATE())), ('T008', 15,  11000, DATEADD(MONTH, 11, GETDATE())),
 ('T009', 800,  450,   DATEADD(MONTH, 24, GETDATE())), ('T010', 40,  72000, DATEADD(MONTH, 9, GETDATE()));
EXEC dbo.sp_PhieuNhap_Tao @MaNV = 1, @MaNCC = 'DHG', @ChiTiet = @pn, @MaPN = @MaPN OUTPUT;

DELETE FROM @pn;
INSERT INTO @pn VALUES
 ('T011', 200, 4200,  DATEADD(MONTH, 15, GETDATE())), ('T012', 300, 800,   DATEADD(DAY, 60, GETDATE())),
 ('T013', 250, 2800,  DATEADD(MONTH, 20, GETDATE())), ('T014', 120, 8000,  DATEADD(MONTH, 13, GETDATE())),
 ('T015', 400, 1200,  DATEADD(MONTH, 17, GETDATE())), ('T016', 180, 7000,  DATEADD(MONTH, 8, GETDATE())),
 ('T017', 60,  65000, DATEADD(MONTH, 12, GETDATE())), ('T018', 18,  1000,  DATEADD(MONTH, 6, GETDATE())),
 ('T019', 300, 600,   DATEADD(MONTH, 22, GETDATE())), ('T020', 25,  78000, DATEADD(MONTH, 19, GETDATE())),
 ('T021', 80,  24000, DATEADD(MONTH, 30, GETDATE()));
EXEC dbo.sp_PhieuNhap_Tao @MaNV = 1, @MaNCC = 'TRAPH', @ChiTiet = @pn, @MaPN = @MaPN OUTPUT;
-- T022 co trong danh muc nhung chua nhap hang (ton kho 0) de thu canh bao het hang.
GO

/* ---------- Ban hang qua thu tuc: ~60 hoa don trong 30 ngay qua ---------- */
DECLARE @i INT = 0, @hd dbo.DongHoaDonType, @MaHD INT, @MaKH INT, @MaNV INT;
WHILE @i < 60
BEGIN
    DELETE FROM @hd;
    INSERT INTO @hd (MaThuoc, SoLuong)
    SELECT TOP (1 + @i % 3) MaThuoc, 1 + ABS(CHECKSUM(NEWID())) % 6
    FROM dbo.Thuoc WHERE TonKho > 30 ORDER BY NEWID();

    SET @MaKH = CASE WHEN @i % 3 = 0 THEN NULL ELSE 1 + @i % 5 END;
    SET @MaNV = 1 + @i % 2;
    EXEC dbo.sp_HoaDon_Tao @MaNV = @MaNV, @MaKH = @MaKH, @ChiTiet = @hd, @MaHD = @MaHD OUTPUT;

    -- Dan hoa don ra 30 ngay gan nhat de thong ke co du lieu.
    UPDATE dbo.HoaDon SET NgayLap = DATEADD(MINUTE, -(@i * 715), SYSDATETIME()) WHERE MaHD = @MaHD;
    SET @i += 1;
END
GO

/* ---------- Nhat ky mau: dang nhap + lap hoa don tuong ung cac hoa don mau ---------- */
INSERT dbo.NhatKy(ThoiGian, MaNV, TenDangNhap, HanhDong, DoiTuong, ChiTiet, ThanhCong, ThoiGianMs)
SELECT DATEADD(SECOND, -40, h.NgayLap), nv.MaNV, nv.TenDangNhap, N'đăng nhập', N'NhanVien', nv.TenDangNhap + N' - ' + nv.HoTen, 1, 0
FROM dbo.HoaDon h JOIN dbo.NhanVien nv ON nv.MaNV = h.MaNV WHERE h.MaHD % 4 = 1;
INSERT dbo.NhatKy(ThoiGian, MaNV, TenDangNhap, HanhDong, DoiTuong, ChiTiet, ThanhCong, ThoiGianMs)
SELECT h.NgayLap, nv.MaNV, nv.TenDangNhap, N'lập hóa đơn', N'HoaDon',
       CAST((SELECT COUNT(*) FROM dbo.ChiTietHoaDon c WHERE c.MaHD = h.MaHD) AS NVARCHAR) + N' dòng, ' + FORMAT(h.TongTien, 'N0') + N' đ',
       1, 8 + ABS(CHECKSUM(NEWID())) % 40
FROM dbo.HoaDon h JOIN dbo.NhanVien nv ON nv.MaNV = h.MaNV;
INSERT dbo.NhatKy(ThoiGian, MaNV, TenDangNhap, HanhDong, DoiTuong, ChiTiet, ThanhCong, ThoiGianMs)
VALUES (DATEADD(HOUR, -26, SYSDATETIME()), NULL, 'admin', N'đăng nhập', N'NhanVien', N'Sai tên đăng nhập hoặc mật khẩu', 0, 0),
       (DATEADD(HOUR, -5, SYSDATETIME()), 2, 'duocsi', N'lập hóa đơn', N'HoaDon', N'1 dòng | Không đủ tồn kho', 0, 11);
GO
PRINT N'Da nap du lieu mau.';
SELECT (SELECT COUNT(*) FROM dbo.Thuoc) AS Thuoc, (SELECT COUNT(*) FROM dbo.HoaDon) AS HoaDon,
       (SELECT COUNT(*) FROM dbo.PhieuNhap) AS PhieuNhap;
GO
