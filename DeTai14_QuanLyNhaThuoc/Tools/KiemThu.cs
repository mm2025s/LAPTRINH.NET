using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using QuanLyNhaThuoc.BUS;
using QuanLyNhaThuoc.DAL;
using QuanLyNhaThuoc.DTO;

namespace QuanLyNhaThuoc.Tools
{
    /// <summary>
    /// Kiem thu tu dong tren CSDL that, qua dung tang BUS ma giao dien dung.
    /// Du lieu thu co tien to KT (thuoc KT01, KT02, nhom KTN...) va duoc xoa sach
    /// khi ket thuc, ke ca khi co phep thu that bai.
    /// Rieng buoc chuan bi/don dep dung SQL truc tiep - CHI trong cong cu nay.
    /// </summary>
    public static class KiemThu
    {
        private static readonly StringBuilder Log = new StringBuilder();
        private static int _dat, _tong;

        public static int Chay()
        {
            Log.AppendLine("KIEM THU PHAN MEM QUAN LY NHA THUOC - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            Log.AppendLine(new string('=', 72));
            DonDep();
            try { ChayCacPhepThu(); }
            catch (Exception ex) { Ghi("Bo kiem thu chay tron ven", false, ex.ToString()); }
            finally { DonDep(); }

            Ghi("Du lieu goc: ton kho = tong nhap - tong ban (sau don dep)", TonKhoKhopChungTu(), null);
            Log.AppendLine(new string('=', 72));
            Log.AppendLine(string.Format("KET QUA: {0}/{1} phep thu dat.", _dat, _tong));
            string tep = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "BaoCao", "kiem_thu.txt"));
            Directory.CreateDirectory(Path.GetDirectoryName(tep));
            File.WriteAllText(tep, Log.ToString(), new UTF8Encoding(true));
            return _tong - _dat;
        }

        private static void ChayCacPhepThu()
        {
            var nvBus = new NhanVienBUS();
            var thuocBus = new ThuocBUS();

            Muc("TC01-03 Dang nhap va phan quyen");
            Loai<LoiNghiepVu>("TC01 Sai mat khau bi tu choi", () => nvBus.DangNhap("admin", "sai-mat-khau"));
            Ghi("TC02 Dang nhap duoc bang duocsi / 123456 (hash C# khop HASHBYTES cua SQL)",
                nvBus.DangNhap("duocsi", "123456").VaiTro == VaiTroNV.DuocSi, null);
            Loai<LoiNghiepVu>("TC03 Duoc si khong duoc them thuoc (chan o BUS)",
                () => thuocBus.Them(new ThuocDTO { MaThuoc = "KT00", TenThuoc = "x", MaNhom = "GD", DonViTinh = "Viên", GiaBan = 1000 }));
            nvBus.DangNhap("admin", "admin123");

            Muc("TC04-07 Danh muc thuoc");
            Loai<LoiNhapLieu>("TC04 Gia ban <= 0 bi tu choi",
                () => thuocBus.Them(new ThuocDTO { MaThuoc = "KT01", TenThuoc = "Thuốc thử", MaNhom = "GD", DonViTinh = "Viên", GiaBan = 0 }));
            thuocBus.Them(new ThuocDTO { MaThuoc = "kt01", TenThuoc = "Thuốc thử KT01", MaNhom = "GD", DonViTinh = "Viên", GiaBan = 5000 });
            thuocBus.Them(new ThuocDTO { MaThuoc = "KT02", TenThuoc = "Kháng sinh thử KT02", MaNhom = "KS", DonViTinh = "Viên", GiaBan = 8000, CanKeDon = true });
            DataRow kt01 = Thuoc("KT01");
            Ghi("TC05 Them thuoc moi, ma duoc viet hoa, ton kho = 0", kt01 != null && (int)kt01["TonKho"] == 0, null);
            Loai<LoiNghiepVu>("TC06 Trung ma thuoc (THROW 51003)",
                () => thuocBus.Them(new ThuocDTO { MaThuoc = "KT01", TenThuoc = "Trùng", MaNhom = "GD", DonViTinh = "Viên", GiaBan = 1000 }));
            DataTable kq = thuocBus.TimKiem("10%", null);
            Ghi("TC07 Tim \"10%\": % la ky tu thuong (chi Povidine 10%), khong phai ky tu dai dien",
                kq.Rows.Count == 1 && (string)kq.Rows[0]["MaThuoc"] == "T021", "So dong = " + kq.Rows.Count);

            Muc("TC08-10 Nhap hang");
            var nhap = new NhapHangBUS();
            Loai<LoiNhapLieu>("TC08 Khong nhap lo da het han", () => nhap.ThemDong("KT01", "KT01", 10, 3000, DateTime.Today));
            nhap.ThemDong("KT01", "KT01", 50, 3000, DateTime.Today.AddYears(1));
            nhap.ThemDong("KT02", "KT02", 20, 6000, DateTime.Today.AddMonths(6));
            int maPN = nhap.LuuPhieu("DHG");
            Ghi("TC09 Luu phieu nhap: ton KT01 = 50, KT02 = 20", (int)Thuoc("KT01")["TonKho"] == 50 && (int)Thuoc("KT02")["TonKho"] == 20, "MaPN=" + maPN);
            Ghi("TC10 Gia nhap va han dung duoc cap nhat theo lo",
                (decimal)Thuoc("KT01")["GiaNhap"] == 3000 && ((DateTime)Thuoc("KT01")["HanSuDung"]).Date == DateTime.Today.AddYears(1), null);

            Muc("TC11-17 Ban hang");
            var ban = new BanHangBUS();
            ban.ThemVaoGio(Thuoc("KT01"), 4);
            ban.ThemVaoGio(Thuoc("KT01"), 6);
            Ghi("TC11 Them trung thuoc thi cong don so luong", ban.Gio.Count == 1 && ban.Gio[0].SoLuong == 10, null);
            Loai<LoiNghiepVu>("TC12 Them vuot ton kho bi chan ngay o gio hang", () => ban.ThemVaoGio(Thuoc("KT01"), 41));
            int maHD = ban.ThanhToan(null, null);
            Ghi("TC13 Thanh toan: ton KT01 50 -> 40, tong tien = 10 x 5.000",
                (int)Thuoc("KT01")["TonKho"] == 40 && TongTienHoaDon(maHD) == 50000m, "MaHD=" + maHD);

            ban.ThemVaoGio(Thuoc("KT02"), 1);
            Loai<LoiNhapLieu>("TC14 Thuoc ke don bat buoc nhap so don thuoc", () => ban.ThanhToan(null, ""));
            ban.Gio.Clear();

            // Mo phong quay khac vua ban: gio hang da kiem tra luc them, nhung ton kho that giam sau do.
            ban.ThemVaoGio(Thuoc("KT01"), 30);
            Sql("UPDATE Thuoc SET TonKho = 5 WHERE MaThuoc = 'KT01'");
            int soHD = SoHoaDon();
            Loai<LoiNghiepVu>("TC15 Thu tuc kiem tra lai ton kho (THROW 51004)", () => ban.ThanhToan(null, null));
            Ghi("TC16 Rollback: khong sinh hoa don, ton kho giu nguyen 5", SoHoaDon() == soHD && (int)Thuoc("KT01")["TonKho"] == 5, null);
            ban.Gio.Clear();

            Sql("UPDATE Thuoc SET HanSuDung = DATEADD(DAY,-1,GETDATE()) WHERE MaThuoc = 'KT01'");
            Loai<LoiNghiepVu>("TC17 Thuoc het han khong duoc them vao gio", () => ban.ThemVaoGio(Thuoc("KT01"), 1));

            Muc("TC18-21 Rang buoc du lieu va xu ly loi");
            thuocBus.Xoa("KT01");
            Ghi("TC18 Xoa thuoc da co giao dich -> ngung kinh doanh, lich su van con",
                Thuoc("KT01") == null && Dem("SELECT COUNT(*) FROM ChiTietHoaDon WHERE MaThuoc='KT01'") == 1, null);
            Loai<LoiNghiepVu>("TC19 Khong xoa nhom thuoc dang co thuoc (THROW 51009)", () => new DanhMucBUS().XoaNhomThuoc("GD"));
            Loai<LoiNghiepVu>("TC20 Khong trung so dien thoai khach (THROW 51008)",
                () => new KhachHangBUS().Luu(new KhachHangDTO { HoTen = "Khách thử", DienThoai = "0912345678" }));
            ThongTinLoi tt = null;
            try { Db.ChayLenh("sp_KhongTonTai"); }
            catch (Exception ex) { tt = XuLyLoi.PhanLoai(ex, "kiem thu"); }
            Ghi("TC21 Loi SQL ky thuat (2812) -> thong bao than thien + ma tham chieu",
                tt != null && tt.Loai == LoaiLoi.Sql && tt.ThongBao.Contains("Mã tham chiếu") && !tt.ThongBao.Contains("sp_KhongTonTai"), tt == null ? null : tt.ThongBao);

            Muc("TC23-26 Huong doi tuong, nhat ky va bao cao");
            ICrudDAL<ThuocDTO> crud = new ThuocDAL();
            Ghi("TC23 ThuocDAL dung qua interface ICrudDAL<ThuocDTO> (ke thua DalCoSo)",
                crud.TimKiem("Paracetamol").Rows.Count > 0 && crud is DalCoSo<ThuocDTO>, null);
            Ghi("TC24 Thao tac ghi duoc ghi nhat ky kem thoi gian xu ly (them thuoc KT01, thanh toan)",
                Dem("SELECT COUNT(*) FROM NhatKy WHERE HanhDong = N'thêm thuốc' AND ChiTiet LIKE 'KT01%' AND ThanhCong = 1") == 1
                && Dem("SELECT COUNT(*) FROM NhatKy WHERE HanhDong = N'lập hóa đơn' AND ThanhCong = 1 AND ThoiGianMs >= 0") >= 1, null);
            Ghi("TC25 Thao tac that bai cung duoc ghi (ban vuot ton kho, THROW 51004)",
                Dem("SELECT COUNT(*) FROM NhatKy WHERE HanhDong = N'lập hóa đơn' AND ThanhCong = 0") >= 1, null);
            var bc = new BaoCaoDoanhThu(DateTime.Today.AddDays(-29), DateTime.Today,
                new ThongKeBUS().DoanhThuTheoNgay(DateTime.Today.AddDays(-29), DateTime.Today));
            Ghi("TC26 Bao cao doanh thu (IBaoCao) co du lieu va dong tong", ((IBaoCao)bc).DuLieu.Rows.Count > 0 && bc.DongTong.StartsWith("Tổng"), bc.DongTong);
        }

        // ------------------------------------------------------------------
        private static DataRow Thuoc(string ma)
        {
            return new ThuocBUS().TimKiem(ma, null).AsEnumerable().FirstOrDefault(r => (string)r["MaThuoc"] == ma);
        }

        private static decimal TongTienHoaDon(int maHD)
        {
            return new BanHangBUS().TimHoaDon(DateTime.Today, DateTime.Today).AsEnumerable()
                .Where(r => (int)r["MaHD"] == maHD).Select(r => (decimal)r["TongTien"]).FirstOrDefault();
        }

        private static int SoHoaDon() { return Dem("SELECT COUNT(*) FROM HoaDon"); }

        private static bool TonKhoKhopChungTu()
        {
            return Dem("SELECT (SELECT ISNULL(SUM(SoLuong),0) FROM ChiTietPhieuNhap) - (SELECT ISNULL(SUM(SoLuong),0) FROM ChiTietHoaDon) - (SELECT SUM(TonKho) FROM Thuoc)") == 0;
        }

        private static void DonDep()
        {
            Sql(@"DELETE c FROM ChiTietHoaDon c WHERE c.MaHD IN (SELECT MaHD FROM ChiTietHoaDon WHERE MaThuoc LIKE 'KT%');
                  DELETE h FROM HoaDon h WHERE NOT EXISTS (SELECT 1 FROM ChiTietHoaDon c WHERE c.MaHD = h.MaHD);
                  DELETE c FROM ChiTietPhieuNhap c WHERE c.MaPN IN (SELECT MaPN FROM ChiTietPhieuNhap WHERE MaThuoc LIKE 'KT%');
                  DELETE p FROM PhieuNhap p WHERE NOT EXISTS (SELECT 1 FROM ChiTietPhieuNhap c WHERE c.MaPN = p.MaPN);
                  DELETE FROM Thuoc WHERE MaThuoc LIKE 'KT%';
                  DELETE FROM NhatKy WHERE ChiTiet LIKE '%KT0%' OR ChiTiet LIKE N'%Khách thử%';");
        }

        private static void Sql(string lenh)
        {
            using (SqlConnection c = Db.TaoKetNoi())
            using (var cmd = new SqlCommand(lenh, c))
            {
                c.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private static int Dem(string lenh)
        {
            using (SqlConnection c = Db.TaoKetNoi())
            using (var cmd = new SqlCommand(lenh, c))
            {
                c.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private static void Muc(string ten) { Log.AppendLine(); Log.AppendLine("--- " + ten + " ---"); }

        private static void Loai<T>(string ten, Action lenh) where T : Exception
        {
            try
            {
                lenh();
                Ghi(ten, false, "khong phat sinh loi");
            }
            catch (Exception ex)
            {
                Ghi(ten, ex is T, ex.GetType().Name + ": " + ex.Message);
            }
        }

        private static void Ghi(string ten, bool dat, string chiTiet)
        {
            _tong++;
            if (dat) _dat++;
            Log.AppendLine(string.Format("  [{0}] {1}", dat ? "DAT" : "SAI", ten));
            if (!string.IsNullOrEmpty(chiTiet)) Log.AppendLine("        -> " + chiTiet);
        }
    }
}
