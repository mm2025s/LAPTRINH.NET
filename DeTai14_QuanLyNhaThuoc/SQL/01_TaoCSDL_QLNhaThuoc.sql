/* =====================================================================
   DE TAI 14 - PHAN MEM QUAN LY NHA THUOC
   Tep 1/2: tao CSDL QLNhaThuoc, bang, rang buoc, kieu bang, thu tuc.

   Chay: sqlcmd -S (localdb)\MSSQLLocalDB -E -i 01_TaoCSDL_QLNhaThuoc.sql
         sau do chay 02_DuLieuMau.sql.
   CANH BAO: tep XOA va tao lai toan bo bang cua CSDL QLNhaThuoc.

   Ma loi nghiep vu (THROW):
     51001 Ten dang nhap da ton tai        51006 Thuoc dang co giao dich
     51002 Khong tim thay ban ghi           51007 Hoa don rong
     51003 Ma thuoc da ton tai              51008 So dien thoai khach da ton tai
     51004 Khong du ton kho                 51009 Danh muc dang duoc su dung
     51005 Thuoc da het han                 51010 Ma danh muc da ton tai
   ===================================================================== */
IF DB_ID(N'QLNhaThuoc') IS NULL CREATE DATABASE QLNhaThuoc;
GO
USE QLNhaThuoc;
GO
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

/* ---------- Xoa doi tuong cu (theo thu tu khoa ngoai) ---------- */
DECLARE @sql NVARCHAR(MAX) = N'';
SELECT @sql += N'DROP PROCEDURE dbo.' + QUOTENAME(name) + N';' FROM sys.procedures;
EXEC (@sql);
GO
DROP TABLE IF EXISTS dbo.NhatKy, dbo.ChiTietPhieuNhap, dbo.PhieuNhap, dbo.ChiTietHoaDon, dbo.HoaDon,
    dbo.KhachHang, dbo.Thuoc, dbo.NhaCungCap, dbo.NhomThuoc, dbo.NhanVien;
DROP TYPE IF EXISTS dbo.DongHoaDonType;
DROP TYPE IF EXISTS dbo.DongPhieuNhapType;
GO

/* ===================================================================== BANG */
CREATE TABLE dbo.NhanVien
(
    MaNV        INT IDENTITY(1,1) PRIMARY KEY,
    HoTen       NVARCHAR(100) NOT NULL,
    TenDangNhap VARCHAR(30)   NOT NULL CONSTRAINT UQ_NhanVien_TenDangNhap UNIQUE,
    MatKhauHash VARBINARY(32) NOT NULL,          -- SHA2-256(Salt + MatKhau)
    Salt        NVARCHAR(32)  NOT NULL,
    VaiTro      VARCHAR(10)   NOT NULL CONSTRAINT CK_NhanVien_VaiTro CHECK (VaiTro IN ('QuanLy','DuocSi')),
    IsActive    BIT           NOT NULL CONSTRAINT DF_NhanVien_IsActive DEFAULT 1
);

CREATE TABLE dbo.NhomThuoc
(
    MaNhom  VARCHAR(10)   NOT NULL PRIMARY KEY,
    TenNhom NVARCHAR(100) NOT NULL CONSTRAINT UQ_NhomThuoc_Ten UNIQUE
);

CREATE TABLE dbo.NhaCungCap
(
    MaNCC     VARCHAR(10)   NOT NULL PRIMARY KEY,
    TenNCC    NVARCHAR(150) NOT NULL,
    DienThoai VARCHAR(15)   NULL,
    DiaChi    NVARCHAR(200) NULL
);

CREATE TABLE dbo.Thuoc
(
    MaThuoc   VARCHAR(15)   NOT NULL PRIMARY KEY,
    TenThuoc  NVARCHAR(150) NOT NULL,
    MaNhom    VARCHAR(10)   NOT NULL CONSTRAINT FK_Thuoc_Nhom REFERENCES dbo.NhomThuoc(MaNhom),
    DonViTinh NVARCHAR(20)  NOT NULL,
    GiaNhap   DECIMAL(12,0) NOT NULL CONSTRAINT DF_Thuoc_GiaNhap DEFAULT 0,
    GiaBan    DECIMAL(12,0) NOT NULL,
    TonKho    INT           NOT NULL CONSTRAINT DF_Thuoc_TonKho DEFAULT 0,
    HanSuDung DATE          NULL,                -- han cua lo gan het han nhat dang ton
    CanKeDon  BIT           NOT NULL CONSTRAINT DF_Thuoc_CanKeDon DEFAULT 0,
    IsActive  BIT           NOT NULL CONSTRAINT DF_Thuoc_IsActive DEFAULT 1,
    CONSTRAINT CK_Thuoc_Gia    CHECK (GiaBan > 0 AND GiaNhap >= 0),
    CONSTRAINT CK_Thuoc_TonKho CHECK (TonKho >= 0)   -- chot chan cuoi: khong bao gio ban am kho
);

CREATE TABLE dbo.KhachHang
(
    MaKH      INT IDENTITY(1,1) PRIMARY KEY,
    HoTen     NVARCHAR(100) NOT NULL,
    DienThoai VARCHAR(15)   NOT NULL CONSTRAINT UQ_KhachHang_DienThoai UNIQUE,
    DiaChi    NVARCHAR(200) NULL
);

CREATE TABLE dbo.HoaDon
(
    MaHD     INT IDENTITY(1,1) PRIMARY KEY,
    NgayLap  DATETIME2(0)  NOT NULL CONSTRAINT DF_HoaDon_NgayLap DEFAULT SYSDATETIME(),
    MaNV     INT           NOT NULL CONSTRAINT FK_HoaDon_NV REFERENCES dbo.NhanVien(MaNV),
    MaKH     INT           NULL     CONSTRAINT FK_HoaDon_KH REFERENCES dbo.KhachHang(MaKH),
    TongTien DECIMAL(14,0) NOT NULL,
    GhiChu   NVARCHAR(200) NULL
);

CREATE TABLE dbo.ChiTietHoaDon
(
    MaHD    INT           NOT NULL CONSTRAINT FK_CTHD_HD REFERENCES dbo.HoaDon(MaHD),
    MaThuoc VARCHAR(15)   NOT NULL CONSTRAINT FK_CTHD_Thuoc REFERENCES dbo.Thuoc(MaThuoc),
    SoLuong INT           NOT NULL CONSTRAINT CK_CTHD_SoLuong CHECK (SoLuong > 0),
    DonGia  DECIMAL(12,0) NOT NULL,
    CONSTRAINT PK_ChiTietHoaDon PRIMARY KEY (MaHD, MaThuoc)
);

CREATE TABLE dbo.PhieuNhap
(
    MaPN     INT IDENTITY(1,1) PRIMARY KEY,
    NgayNhap DATETIME2(0)  NOT NULL CONSTRAINT DF_PhieuNhap_Ngay DEFAULT SYSDATETIME(),
    MaNV     INT           NOT NULL CONSTRAINT FK_PN_NV REFERENCES dbo.NhanVien(MaNV),
    MaNCC    VARCHAR(10)   NOT NULL CONSTRAINT FK_PN_NCC REFERENCES dbo.NhaCungCap(MaNCC),
    TongTien DECIMAL(14,0) NOT NULL
);

CREATE TABLE dbo.ChiTietPhieuNhap
(
    MaPN       INT           NOT NULL CONSTRAINT FK_CTPN_PN REFERENCES dbo.PhieuNhap(MaPN),
    MaThuoc    VARCHAR(15)   NOT NULL CONSTRAINT FK_CTPN_Thuoc REFERENCES dbo.Thuoc(MaThuoc),
    SoLuong    INT           NOT NULL CONSTRAINT CK_CTPN_SoLuong CHECK (SoLuong > 0),
    DonGiaNhap DECIMAL(12,0) NOT NULL CONSTRAINT CK_CTPN_Gia CHECK (DonGiaNhap >= 0),
    HanSuDung  DATE          NOT NULL,
    CONSTRAINT PK_ChiTietPhieuNhap PRIMARY KEY (MaPN, MaThuoc)
);
GO

/* Kieu bang de truyen ca gio hang / phieu nhap trong MOT lan goi thu tuc. */
CREATE TYPE dbo.DongHoaDonType AS TABLE
(
    MaThuoc VARCHAR(15) NOT NULL PRIMARY KEY,
    SoLuong INT         NOT NULL
);
CREATE TYPE dbo.DongPhieuNhapType AS TABLE
(
    MaThuoc    VARCHAR(15)   NOT NULL PRIMARY KEY,
    SoLuong    INT           NOT NULL,
    DonGiaNhap DECIMAL(12,0) NOT NULL,
    HanSuDung  DATE          NOT NULL
);
GO

/* ===================================================================== NHAN VIEN */
CREATE PROCEDURE dbo.sp_NhanVien_LayDeDangNhap @TenDangNhap VARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT MaNV, HoTen, TenDangNhap, MatKhauHash, Salt, VaiTro, IsActive
    FROM dbo.NhanVien WHERE TenDangNhap = @TenDangNhap;
END;
GO
CREATE PROCEDURE dbo.sp_NhanVien_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT MaNV, HoTen, TenDangNhap, VaiTro, IsActive FROM dbo.NhanVien ORDER BY MaNV;
END;
GO
CREATE PROCEDURE dbo.sp_NhanVien_Insert
    @HoTen NVARCHAR(100), @TenDangNhap VARCHAR(30), @MatKhauHash VARBINARY(32),
    @Salt NVARCHAR(32), @VaiTro VARCHAR(10)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM dbo.NhanVien WHERE TenDangNhap = @TenDangNhap)
        THROW 51001, N'Tên đăng nhập đã tồn tại.', 1;
    INSERT INTO dbo.NhanVien (HoTen, TenDangNhap, MatKhauHash, Salt, VaiTro)
    VALUES (@HoTen, @TenDangNhap, @MatKhauHash, @Salt, @VaiTro);
END;
GO
CREATE PROCEDURE dbo.sp_NhanVien_Update
    @MaNV INT, @HoTen NVARCHAR(100), @VaiTro VARCHAR(10), @IsActive BIT
AS
BEGIN
    UPDATE dbo.NhanVien SET HoTen = @HoTen, VaiTro = @VaiTro, IsActive = @IsActive WHERE MaNV = @MaNV;
    IF @@ROWCOUNT = 0 THROW 51002, N'Không tìm thấy nhân viên.', 1;
END;
GO
CREATE PROCEDURE dbo.sp_NhanVien_DoiMatKhau
    @MaNV INT, @MatKhauHash VARBINARY(32), @Salt NVARCHAR(32)
AS
BEGIN
    UPDATE dbo.NhanVien SET MatKhauHash = @MatKhauHash, Salt = @Salt WHERE MaNV = @MaNV;
    IF @@ROWCOUNT = 0 THROW 51002, N'Không tìm thấy nhân viên.', 1;
END;
GO

/* ===================================================================== DANH MUC */
CREATE PROCEDURE dbo.sp_NhomThuoc_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT n.MaNhom, n.TenNhom, COUNT(t.MaThuoc) AS SoThuoc
    FROM dbo.NhomThuoc n LEFT JOIN dbo.Thuoc t ON t.MaNhom = n.MaNhom
    GROUP BY n.MaNhom, n.TenNhom ORDER BY n.MaNhom;
END;
GO
CREATE PROCEDURE dbo.sp_NhomThuoc_Save @MaNhom VARCHAR(10), @TenNhom NVARCHAR(100), @LaThemMoi BIT
AS
BEGIN
    IF @LaThemMoi = 1
    BEGIN
        IF EXISTS (SELECT 1 FROM dbo.NhomThuoc WHERE MaNhom = @MaNhom)
            THROW 51010, N'Mã nhóm thuốc đã tồn tại.', 1;
        INSERT INTO dbo.NhomThuoc (MaNhom, TenNhom) VALUES (@MaNhom, @TenNhom);
    END
    ELSE
    BEGIN
        UPDATE dbo.NhomThuoc SET TenNhom = @TenNhom WHERE MaNhom = @MaNhom;
        IF @@ROWCOUNT = 0 THROW 51002, N'Không tìm thấy nhóm thuốc.', 1;
    END
END;
GO
CREATE PROCEDURE dbo.sp_NhomThuoc_Delete @MaNhom VARCHAR(10)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM dbo.Thuoc WHERE MaNhom = @MaNhom)
        THROW 51009, N'Nhóm đang có thuốc, không thể xóa.', 1;
    DELETE FROM dbo.NhomThuoc WHERE MaNhom = @MaNhom;
    IF @@ROWCOUNT = 0 THROW 51002, N'Không tìm thấy nhóm thuốc.', 1;
END;
GO
CREATE PROCEDURE dbo.sp_NhaCungCap_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT MaNCC, TenNCC, DienThoai, DiaChi FROM dbo.NhaCungCap ORDER BY MaNCC;
END;
GO
CREATE PROCEDURE dbo.sp_NhaCungCap_Save
    @MaNCC VARCHAR(10), @TenNCC NVARCHAR(150), @DienThoai VARCHAR(15), @DiaChi NVARCHAR(200), @LaThemMoi BIT
AS
BEGIN
    IF @LaThemMoi = 1
    BEGIN
        IF EXISTS (SELECT 1 FROM dbo.NhaCungCap WHERE MaNCC = @MaNCC)
            THROW 51010, N'Mã nhà cung cấp đã tồn tại.', 1;
        INSERT INTO dbo.NhaCungCap (MaNCC, TenNCC, DienThoai, DiaChi) VALUES (@MaNCC, @TenNCC, @DienThoai, @DiaChi);
    END
    ELSE
    BEGIN
        UPDATE dbo.NhaCungCap SET TenNCC = @TenNCC, DienThoai = @DienThoai, DiaChi = @DiaChi WHERE MaNCC = @MaNCC;
        IF @@ROWCOUNT = 0 THROW 51002, N'Không tìm thấy nhà cung cấp.', 1;
    END
END;
GO
CREATE PROCEDURE dbo.sp_NhaCungCap_Delete @MaNCC VARCHAR(10)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM dbo.PhieuNhap WHERE MaNCC = @MaNCC)
        THROW 51009, N'Nhà cung cấp đã có phiếu nhập, không thể xóa.', 1;
    DELETE FROM dbo.NhaCungCap WHERE MaNCC = @MaNCC;
    IF @@ROWCOUNT = 0 THROW 51002, N'Không tìm thấy nhà cung cấp.', 1;
END;
GO

/* ===================================================================== THUOC */
CREATE PROCEDURE dbo.sp_Thuoc_Search
    @TuKhoa NVARCHAR(100) = NULL, @MaNhom VARCHAR(10) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET @TuKhoa = NULLIF(LTRIM(RTRIM(@TuKhoa)), N'');
    SET @MaNhom = NULLIF(LTRIM(RTRIM(@MaNhom)), '');
    DECLARE @Mau NVARCHAR(310) = N'%' + REPLACE(REPLACE(REPLACE(REPLACE(@TuKhoa,
        N'\', N'\\'), N'%', N'\%'), N'_', N'\_'), N'[', N'\[') + N'%';

    SELECT t.MaThuoc, t.TenThuoc, t.MaNhom, n.TenNhom, t.DonViTinh, t.GiaNhap, t.GiaBan,
           t.TonKho, t.HanSuDung, t.CanKeDon
    FROM dbo.Thuoc t JOIN dbo.NhomThuoc n ON n.MaNhom = t.MaNhom
    WHERE t.IsActive = 1
      AND (@MaNhom IS NULL OR t.MaNhom = @MaNhom)
      AND (@Mau IS NULL OR t.MaThuoc LIKE @Mau ESCAPE N'\' OR t.TenThuoc LIKE @Mau ESCAPE N'\')
    ORDER BY t.TenThuoc;
END;
GO
CREATE PROCEDURE dbo.sp_Thuoc_Insert
    @MaThuoc VARCHAR(15), @TenThuoc NVARCHAR(150), @MaNhom VARCHAR(10), @DonViTinh NVARCHAR(20),
    @GiaBan DECIMAL(12,0), @CanKeDon BIT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM dbo.Thuoc WHERE MaThuoc = @MaThuoc)
        THROW 51003, N'Mã thuốc đã tồn tại.', 1;
    -- Thuoc moi chua co hang: ton kho, gia nhap va han dung duoc cap nhat khi nhap hang.
    INSERT INTO dbo.Thuoc (MaThuoc, TenThuoc, MaNhom, DonViTinh, GiaBan, CanKeDon)
    VALUES (@MaThuoc, @TenThuoc, @MaNhom, @DonViTinh, @GiaBan, @CanKeDon);
END;
GO
CREATE PROCEDURE dbo.sp_Thuoc_Update
    @MaThuoc VARCHAR(15), @TenThuoc NVARCHAR(150), @MaNhom VARCHAR(10), @DonViTinh NVARCHAR(20),
    @GiaBan DECIMAL(12,0), @CanKeDon BIT
AS
BEGIN
    -- Ton kho KHONG sua tay: chi thay doi qua ban hang va nhap hang de so lieu luon khop chung tu.
    UPDATE dbo.Thuoc SET TenThuoc = @TenThuoc, MaNhom = @MaNhom, DonViTinh = @DonViTinh,
           GiaBan = @GiaBan, CanKeDon = @CanKeDon
    WHERE MaThuoc = @MaThuoc AND IsActive = 1;
    IF @@ROWCOUNT = 0 THROW 51002, N'Không tìm thấy thuốc.', 1;
END;
GO
/* Thuoc da tung ban/nhap thi chi ngung kinh doanh (xoa mem) de giu lich su chung tu. */
CREATE PROCEDURE dbo.sp_Thuoc_Delete @MaThuoc VARCHAR(15)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM dbo.ChiTietHoaDon WHERE MaThuoc = @MaThuoc)
       OR EXISTS (SELECT 1 FROM dbo.ChiTietPhieuNhap WHERE MaThuoc = @MaThuoc)
    BEGIN
        UPDATE dbo.Thuoc SET IsActive = 0 WHERE MaThuoc = @MaThuoc AND IsActive = 1;
        IF @@ROWCOUNT = 0 THROW 51002, N'Không tìm thấy thuốc.', 1;
        RETURN;
    END
    DELETE FROM dbo.Thuoc WHERE MaThuoc = @MaThuoc;
    IF @@ROWCOUNT = 0 THROW 51002, N'Không tìm thấy thuốc.', 1;
END;
GO

/* ===================================================================== KHACH HANG */
CREATE PROCEDURE dbo.sp_KhachHang_Search @TuKhoa NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET @TuKhoa = NULLIF(LTRIM(RTRIM(@TuKhoa)), N'');
    SELECT k.MaKH, k.HoTen, k.DienThoai, k.DiaChi,
           COUNT(h.MaHD) AS SoHoaDon, ISNULL(SUM(h.TongTien), 0) AS TongMua
    FROM dbo.KhachHang k LEFT JOIN dbo.HoaDon h ON h.MaKH = k.MaKH
    WHERE @TuKhoa IS NULL OR k.HoTen LIKE N'%' + @TuKhoa + N'%' OR k.DienThoai LIKE @TuKhoa + '%'
    GROUP BY k.MaKH, k.HoTen, k.DienThoai, k.DiaChi
    ORDER BY k.HoTen;
END;
GO
CREATE PROCEDURE dbo.sp_KhachHang_Save
    @MaKH INT = NULL, @HoTen NVARCHAR(100), @DienThoai VARCHAR(15), @DiaChi NVARCHAR(200) = NULL,
    @MaKHMoi INT = NULL OUTPUT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM dbo.KhachHang WHERE DienThoai = @DienThoai AND (@MaKH IS NULL OR MaKH <> @MaKH))
        THROW 51008, N'Số điện thoại đã thuộc về khách hàng khác.', 1;
    IF @MaKH IS NULL
    BEGIN
        INSERT INTO dbo.KhachHang (HoTen, DienThoai, DiaChi) VALUES (@HoTen, @DienThoai, @DiaChi);
        SET @MaKHMoi = SCOPE_IDENTITY();
    END
    ELSE
    BEGIN
        UPDATE dbo.KhachHang SET HoTen = @HoTen, DienThoai = @DienThoai, DiaChi = @DiaChi WHERE MaKH = @MaKH;
        IF @@ROWCOUNT = 0 THROW 51002, N'Không tìm thấy khách hàng.', 1;
        SET @MaKHMoi = @MaKH;
    END
END;
GO
CREATE PROCEDURE dbo.sp_KhachHang_Delete @MaKH INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM dbo.HoaDon WHERE MaKH = @MaKH)
        THROW 51009, N'Khách hàng đã có hóa đơn, không thể xóa.', 1;
    DELETE FROM dbo.KhachHang WHERE MaKH = @MaKH;
    IF @@ROWCOUNT = 0 THROW 51002, N'Không tìm thấy khách hàng.', 1;
END;
GO

/* ===================================================================== BAN HANG
   Lap hoa don trong MOT transaction:
     1. Kiem tra tung dong: thuoc ton tai, chua het han, du ton kho.
     2. Tru ton kho co dieu kien TonKho >= SoLuong (chong ban am khi 2 quay
        cung ban mot luc) - so dong cap nhat phai bang so dong gio hang.
     3. Ghi HoaDon + ChiTietHoaDon voi gia ban tai thoi diem lap.
   XACT_ABORT ON: bat ky loi nao cung rollback toan bo, khong de hoa don nua voi. */
CREATE PROCEDURE dbo.sp_HoaDon_Tao
    @MaNV INT, @MaKH INT = NULL, @GhiChu NVARCHAR(200) = NULL,
    @ChiTiet dbo.DongHoaDonType READONLY,
    @MaHD INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS (SELECT 1 FROM @ChiTiet)
        THROW 51007, N'Hóa đơn chưa có thuốc nào.', 1;

    DECLARE @Ten NVARCHAR(150), @ThongBao NVARCHAR(300);

    SELECT TOP 1 @Ten = c.MaThuoc FROM @ChiTiet c
    LEFT JOIN dbo.Thuoc t ON t.MaThuoc = c.MaThuoc AND t.IsActive = 1
    WHERE t.MaThuoc IS NULL OR c.SoLuong <= 0;
    IF @Ten IS NOT NULL
    BEGIN
        SET @ThongBao = CONCAT(N'Thuốc ', @Ten, N' không tồn tại hoặc số lượng không hợp lệ.');
        THROW 51002, @ThongBao, 1;
    END

    SELECT TOP 1 @Ten = t.TenThuoc FROM @ChiTiet c JOIN dbo.Thuoc t ON t.MaThuoc = c.MaThuoc
    WHERE t.HanSuDung IS NOT NULL AND t.HanSuDung < CAST(GETDATE() AS DATE);
    IF @Ten IS NOT NULL
    BEGIN
        SET @ThongBao = CONCAT(N'Thuốc "', @Ten, N'" đã hết hạn sử dụng, không được bán.');
        THROW 51005, @ThongBao, 1;
    END

    BEGIN TRANSACTION;

        UPDATE t SET t.TonKho = t.TonKho - c.SoLuong
        FROM dbo.Thuoc t WITH (UPDLOCK, ROWLOCK)
        JOIN @ChiTiet c ON c.MaThuoc = t.MaThuoc
        WHERE t.TonKho >= c.SoLuong;

        IF @@ROWCOUNT <> (SELECT COUNT(*) FROM @ChiTiet)
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT TOP 1 @ThongBao = CONCAT(N'Không đủ tồn kho: "', t.TenThuoc, N'" chỉ còn ', t.TonKho, N' ', t.DonViTinh, N'.')
            FROM @ChiTiet c JOIN dbo.Thuoc t ON t.MaThuoc = c.MaThuoc
            WHERE t.TonKho < c.SoLuong;
            THROW 51004, @ThongBao, 1;
        END

        INSERT INTO dbo.HoaDon (MaNV, MaKH, TongTien, GhiChu)
        SELECT @MaNV, @MaKH, SUM(c.SoLuong * t.GiaBan), @GhiChu
        FROM @ChiTiet c JOIN dbo.Thuoc t ON t.MaThuoc = c.MaThuoc;

        SET @MaHD = SCOPE_IDENTITY();

        INSERT INTO dbo.ChiTietHoaDon (MaHD, MaThuoc, SoLuong, DonGia)
        SELECT @MaHD, c.MaThuoc, c.SoLuong, t.GiaBan
        FROM @ChiTiet c JOIN dbo.Thuoc t ON t.MaThuoc = c.MaThuoc;

    COMMIT TRANSACTION;
END;
GO
CREATE PROCEDURE dbo.sp_HoaDon_Search @TuNgay DATE, @DenNgay DATE
AS
BEGIN
    SET NOCOUNT ON;
    SELECT h.MaHD, h.NgayLap, nv.HoTen AS NhanVien, ISNULL(k.HoTen, N'Khách lẻ') AS KhachHang,
           h.TongTien, h.GhiChu
    FROM dbo.HoaDon h
    JOIN dbo.NhanVien nv ON nv.MaNV = h.MaNV
    LEFT JOIN dbo.KhachHang k ON k.MaKH = h.MaKH
    WHERE h.NgayLap >= @TuNgay AND h.NgayLap < DATEADD(DAY, 1, @DenNgay)
    ORDER BY h.NgayLap DESC;
END;
GO
CREATE PROCEDURE dbo.sp_HoaDon_ChiTiet @MaHD INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT c.MaThuoc, t.TenThuoc, t.DonViTinh, c.SoLuong, c.DonGia, c.SoLuong * c.DonGia AS ThanhTien
    FROM dbo.ChiTietHoaDon c JOIN dbo.Thuoc t ON t.MaThuoc = c.MaThuoc
    WHERE c.MaHD = @MaHD ORDER BY t.TenThuoc;
END;
GO

/* ===================================================================== NHAP HANG
   Mot transaction: ghi phieu, cong ton kho, cap nhat gia nhap va han dung gan nhat. */
CREATE PROCEDURE dbo.sp_PhieuNhap_Tao
    @MaNV INT, @MaNCC VARCHAR(10), @ChiTiet dbo.DongPhieuNhapType READONLY, @MaPN INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS (SELECT 1 FROM @ChiTiet)
        THROW 51007, N'Phiếu nhập chưa có thuốc nào.', 1;
    IF EXISTS (SELECT 1 FROM @ChiTiet WHERE SoLuong <= 0 OR DonGiaNhap < 0)
        THROW 51002, N'Số lượng hoặc đơn giá nhập không hợp lệ.', 1;
    IF EXISTS (SELECT 1 FROM @ChiTiet WHERE HanSuDung <= CAST(GETDATE() AS DATE))
        THROW 51005, N'Không nhập lô thuốc đã hết hạn.', 1;
    IF EXISTS (SELECT 1 FROM @ChiTiet c LEFT JOIN dbo.Thuoc t ON t.MaThuoc = c.MaThuoc AND t.IsActive = 1 WHERE t.MaThuoc IS NULL)
        THROW 51002, N'Có thuốc không tồn tại hoặc đã ngừng kinh doanh.', 1;

    BEGIN TRANSACTION;
        INSERT INTO dbo.PhieuNhap (MaNV, MaNCC, TongTien)
        SELECT @MaNV, @MaNCC, SUM(SoLuong * DonGiaNhap) FROM @ChiTiet;
        SET @MaPN = SCOPE_IDENTITY();

        INSERT INTO dbo.ChiTietPhieuNhap (MaPN, MaThuoc, SoLuong, DonGiaNhap, HanSuDung)
        SELECT @MaPN, MaThuoc, SoLuong, DonGiaNhap, HanSuDung FROM @ChiTiet;

        UPDATE t SET
            t.HanSuDung = CASE WHEN t.TonKho = 0 OR t.HanSuDung IS NULL OR c.HanSuDung < t.HanSuDung
                               THEN c.HanSuDung ELSE t.HanSuDung END,
            t.TonKho  = t.TonKho + c.SoLuong,
            t.GiaNhap = c.DonGiaNhap
        FROM dbo.Thuoc t JOIN @ChiTiet c ON c.MaThuoc = t.MaThuoc;
    COMMIT TRANSACTION;
END;
GO

/* ===================================================================== THONG KE */
CREATE PROCEDURE dbo.sp_ThongKe_TongQuan
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        (SELECT ISNULL(SUM(TongTien),0) FROM dbo.HoaDon WHERE CAST(NgayLap AS DATE) = CAST(GETDATE() AS DATE)) AS DoanhThuHomNay,
        (SELECT COUNT(*) FROM dbo.HoaDon WHERE CAST(NgayLap AS DATE) = CAST(GETDATE() AS DATE)) AS SoHoaDonHomNay,
        (SELECT ISNULL(SUM(TongTien),0) FROM dbo.HoaDon WHERE NgayLap >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) AS DoanhThuThang,
        (SELECT COUNT(*) FROM dbo.Thuoc WHERE IsActive = 1) AS SoThuoc,
        (SELECT COUNT(*) FROM dbo.Thuoc WHERE IsActive = 1 AND TonKho <= 20) AS SoSapHetHang,
        (SELECT COUNT(*) FROM dbo.Thuoc WHERE IsActive = 1 AND TonKho > 0 AND HanSuDung <= DATEADD(DAY, 90, GETDATE())) AS SoSapHetHan;
END;
GO
CREATE PROCEDURE dbo.sp_ThongKe_DoanhThuTheoNgay @TuNgay DATE, @DenNgay DATE
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CAST(NgayLap AS DATE) AS Ngay, COUNT(*) AS SoHoaDon, SUM(TongTien) AS DoanhThu
    FROM dbo.HoaDon
    WHERE NgayLap >= @TuNgay AND NgayLap < DATEADD(DAY, 1, @DenNgay)
    GROUP BY CAST(NgayLap AS DATE) ORDER BY Ngay;
END;
GO
CREATE PROCEDURE dbo.sp_ThongKe_TopThuoc @TuNgay DATE, @DenNgay DATE, @SoLuong INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP (@SoLuong) t.MaThuoc, t.TenThuoc, SUM(c.SoLuong) AS SoLuongBan, SUM(c.SoLuong * c.DonGia) AS DoanhThu
    FROM dbo.ChiTietHoaDon c
    JOIN dbo.HoaDon h ON h.MaHD = c.MaHD
    JOIN dbo.Thuoc t ON t.MaThuoc = c.MaThuoc
    WHERE h.NgayLap >= @TuNgay AND h.NgayLap < DATEADD(DAY, 1, @DenNgay)
    GROUP BY t.MaThuoc, t.TenThuoc ORDER BY DoanhThu DESC;
END;
GO
/* Canh bao kho: sap het hang (TonKho <= @NguongTon) hoac sap het han (trong @SoNgay ngay). */
CREATE PROCEDURE dbo.sp_ThongKe_CanhBao @NguongTon INT = 20, @SoNgay INT = 90
AS
BEGIN
    SET NOCOUNT ON;
    SELECT t.MaThuoc, t.TenThuoc, t.DonViTinh, t.TonKho, t.HanSuDung,
           CASE WHEN t.HanSuDung < CAST(GETDATE() AS DATE) AND t.TonKho > 0 THEN N'Đã hết hạn'
                WHEN t.TonKho > 0 AND t.HanSuDung <= DATEADD(DAY, @SoNgay, GETDATE()) THEN N'Sắp hết hạn'
                ELSE N'Sắp hết hàng' END AS CanhBao
    FROM dbo.Thuoc t
    WHERE t.IsActive = 1
      AND (t.TonKho <= @NguongTon OR (t.TonKho > 0 AND t.HanSuDung <= DATEADD(DAY, @SoNgay, GETDATE())))
    ORDER BY CASE WHEN t.HanSuDung < CAST(GETDATE() AS DATE) THEN 0 ELSE 1 END, t.HanSuDung, t.TonKho;
END;
GO
/* ===================================================================== NHAT KY & HIEU NANG */
/* Nhat ky nguoi dung: moi thao tac ghi du lieu, dang nhap/dang xuat deu ghi lai
   cung ket qua (thanh cong / that bai) va thoi gian xu ly (ms) de do hieu nang. */
CREATE TABLE dbo.NhatKy
(
    MaNK        BIGINT IDENTITY PRIMARY KEY,
    ThoiGian    DATETIME2(0)  NOT NULL CONSTRAINT DF_NhatKy_ThoiGian DEFAULT SYSDATETIME(),
    MaNV        INT           NULL REFERENCES dbo.NhanVien(MaNV),
    TenDangNhap VARCHAR(30)   NULL,
    HanhDong    NVARCHAR(60)  NOT NULL,
    DoiTuong    NVARCHAR(40)  NULL,
    ChiTiet     NVARCHAR(300) NULL,
    ThanhCong   BIT           NOT NULL,
    ThoiGianMs  INT           NOT NULL CONSTRAINT CK_NhatKy_Ms CHECK (ThoiGianMs >= 0)
);
CREATE INDEX IX_NhatKy_ThoiGian ON dbo.NhatKy(ThoiGian DESC);
GO
CREATE PROCEDURE dbo.sp_NhatKy_Ghi
    @MaNV INT = NULL, @TenDangNhap VARCHAR(30) = NULL, @HanhDong NVARCHAR(60), @DoiTuong NVARCHAR(40) = NULL,
    @ChiTiet NVARCHAR(300) = NULL, @ThanhCong BIT, @ThoiGianMs INT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT dbo.NhatKy(MaNV, TenDangNhap, HanhDong, DoiTuong, ChiTiet, ThanhCong, ThoiGianMs)
    VALUES (@MaNV, @TenDangNhap, @HanhDong, @DoiTuong, LEFT(@ChiTiet, 300), @ThanhCong, @ThoiGianMs);
END;
GO
CREATE PROCEDURE dbo.sp_NhatKy_Search @TuNgay DATE, @DenNgay DATE, @TuKhoa NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @mau NVARCHAR(110) = N'%' + REPLACE(REPLACE(REPLACE(ISNULL(@TuKhoa, N''), N'[', N'[[]'), N'%', N'[%]'), N'_', N'[_]') + N'%';
    SELECT TOP (1000) n.MaNK, n.ThoiGian, ISNULL(n.TenDangNhap, N'(khách)') AS TenDangNhap, n.HanhDong, n.DoiTuong, n.ChiTiet,
           CASE WHEN n.ThanhCong = 1 THEN N'Thành công' ELSE N'Thất bại' END AS KetQua, n.ThoiGianMs
    FROM dbo.NhatKy n
    WHERE n.ThoiGian >= @TuNgay AND n.ThoiGian < DATEADD(DAY, 1, @DenNgay)
      AND (@TuKhoa IS NULL OR n.HanhDong LIKE @mau OR n.ChiTiet LIKE @mau OR n.TenDangNhap LIKE @mau)
    ORDER BY n.MaNK DESC;
END;
GO
/* Hieu nang nghiep vu theo nhan vien: so hoa don, doanh thu, gia tri trung binh. */
CREATE PROCEDURE dbo.sp_ThongKe_HieuNangNhanVien @TuNgay DATE, @DenNgay DATE
AS
BEGIN
    SET NOCOUNT ON;
    SELECT nv.HoTen, COUNT(h.MaHD) AS SoHoaDon, ISNULL(SUM(h.TongTien), 0) AS DoanhThu,
           CAST(ISNULL(AVG(h.TongTien), 0) AS DECIMAL(18,0)) AS TrungBinhHD
    FROM dbo.NhanVien nv
    LEFT JOIN dbo.HoaDon h ON h.MaNV = nv.MaNV AND h.NgayLap >= @TuNgay AND h.NgayLap < DATEADD(DAY, 1, @DenNgay)
    GROUP BY nv.MaNV, nv.HoTen ORDER BY DoanhThu DESC;
END;
GO
/* Hieu nang xu ly theo thao tac (lay tu nhat ky): so lan, ti le loi, thoi gian TB / lon nhat. */
CREATE PROCEDURE dbo.sp_ThongKe_HieuNangThaoTac @TuNgay DATE, @DenNgay DATE
AS
BEGIN
    SET NOCOUNT ON;
    SELECT HanhDong, COUNT(*) AS SoLan, SUM(CASE WHEN ThanhCong = 0 THEN 1 ELSE 0 END) AS SoLanLoi,
           AVG(ThoiGianMs) AS TrungBinhMs, MAX(ThoiGianMs) AS LonNhatMs
    FROM dbo.NhatKy
    WHERE ThoiGian >= @TuNgay AND ThoiGian < DATEADD(DAY, 1, @DenNgay)
    GROUP BY HanhDong ORDER BY SoLan DESC;
END;
GO
PRINT N'Da tao CSDL QLNhaThuoc. Tiep theo: chay 02_DuLieuMau.sql';
GO
