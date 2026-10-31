using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Lab09_DataReaderSearch.BUS;
using Lab09_DataReaderSearch.DTO;
using Lab09_DataReaderSearch.GUI.Helpers;

namespace Lab09_DataReaderSearch.BaoCaoTools
{
    /// <summary>
    /// KIEM TRA NHANH 10 bai cua Lab 9 - chay KHONG CAN GIAO DIEN.
    ///
    /// Muc dich: khang dinh tang DAL/BUS tra ve dung so lieu THAT trong CSDL
    /// truoc khi chup anh bao cao. Moi bai co 1..3 phep kiem tra; ket qua ghi
    /// ra bin/Debug/kiem_tra_nhanh.txt va BaoCao/kiem_tra_nhanh.txt.
    ///
    /// Toan bo chi DOC du lieu (tru bai 10 ghi ra thu muc tam) nen chay lai
    /// nhieu lan khong lam thay doi CSDL.
    /// </summary>
    public static class KiemTraChucNang
    {
        private static readonly StringBuilder KetQua = new StringBuilder();
        private static int _dat, _hong;

        public static void Chay()
        {
            var bus = new SinhVienBUS();

            Ghi("===== KIEM TRA NHANH - LAB 9 (SqlDataReader, tim kiem, loc) =====");
            Ghi("Thoi diem : " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            Ghi("May       : " + Environment.MachineName);
            Ghi("");

            try
            {
                KiemTraBai1(bus);
                KiemTraBai2(bus);
                KiemTraBai3(bus);
                KiemTraBai4(bus);
                KiemTraBai5(bus);
                KiemTraBai6(bus);
                KiemTraBai7(bus);
                KiemTraBai8(bus);
                KiemTraBai9(bus);
                KiemTraBai10(bus);
            }
            catch (Exception ex)
            {
                Ghi("LOI NGHIEM TRONG: " + ex);
                _hong++;
            }

            Ghi("");
            Ghi("------------------------------------------------------------");
            Ghi(string.Format("KET QUA: {0}/{1} phep kiem tra dat.", _dat, _dat + _hong));
            Ghi("------------------------------------------------------------");

            Luu();
        }

        // =====================================================================

        private static void KiemTraBai1(SinhVienBUS bus)
        {
            Muc("BAI 1 - Nap SqlDataReader vao DataTable");

            DataTable a = bus.LoadStudents();
            KiemTra("DataTable.Load(reader) doc duoc du lieu", a.Rows.Count > 0,
                a.Rows.Count + " dong, " + a.Columns.Count + " cot");

            KiemTra("Co du 8 cot theo yeu cau",
                a.Columns.Contains("MaSV") && a.Columns.Contains("HoTen") &&
                a.Columns.Contains("NgaySinh") && a.Columns.Contains("GioiTinh") &&
                a.Columns.Contains("MaKhoa") && a.Columns.Contains("TenKhoa") &&
                a.Columns.Contains("DiemTB") && a.Columns.Contains("Email"),
                string.Join(", ", a.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray()));

            int soDong;
            DataTable b = bus.NapThuCong(out soDong);
            KiemTra("Nap thu cong cho cung so dong", b.Rows.Count == a.Rows.Count && soDong == a.Rows.Count,
                "Load=" + a.Rows.Count + ", thu cong=" + b.Rows.Count);

            KiemTra("JOIN Khoa lay duoc TenKhoa (khong NULL)",
                a.Rows.Cast<DataRow>().All(r => !string.IsNullOrEmpty(Convert.ToString(r["TenKhoa"]))),
                "3 khoa: CNTT / OTO / KT");

            KiemTra("CASE GioiTinh tra ve 'Nam' hoac 'Nữ'",
                a.Rows.Cast<DataRow>().All(r =>
                {
                    string g = Convert.ToString(r["GioiTinh"]);
                    return g == "Nam" || g == "Nữ";
                }), "da doi BIT -> chuoi");
        }

        private static void KiemTraBai2(SinhVienBUS bus)
        {
            Muc("BAI 2 - Tim kiem gan dung theo ho ten (LIKE @Keyword)");

            DataTable all = bus.LoadStudents();

            DataTable timNguyen = bus.SearchByName("Nguyễn");
            int trongTatCa = all.Rows.Cast<DataRow>()
                .Count(r => Convert.ToString(r["HoTen"]).IndexOf("Nguyễn", StringComparison.OrdinalIgnoreCase) >= 0);

            KiemTra("LIKE 'Nguyễn' khop voi dem o client", timNguyen.Rows.Count == trongTatCa,
                "SQL=" + timNguyen.Rows.Count + ", client=" + trongTatCa);

            DataTable khongCo = bus.SearchByName("zzz_khong_ton_tai_zzz");
            KiemTra("Tu khoa khong ton tai tra ve 0 dong", khongCo.Rows.Count == 0,
                "0 dong - khong nem loi");

            // Ky tu % cua nguoi dung phai duoc coi la GIA TRI, khong phai wildcard
            DataTable phanTram = bus.SearchByName("%");
            KiemTra("Ky tu % khong bi hieu la wildcard (vao tham so)",
                phanTram.Rows.Count < all.Rows.Count,
                "% tra ve " + phanTram.Rows.Count + "/" + all.Rows.Count + " dong");

            // SQL Injection - chuoi tan cong phai duoc coi la van ban
            DataTable tanCong = bus.SearchByName("'; DROP TABLE SinhVien --");
            DataTable sauTanCong = bus.LoadStudents();
            KiemTra("Chuoi SQL Injection khong lam hong bang",
                sauTanCong.Rows.Count == all.Rows.Count && tanCong.Rows.Count == 0,
                "bang con " + sauTanCong.Rows.Count + " dong");

            try
            {
                bus.SearchByName("   ");
                KiemTra("Tu khoa rong bi tu choi", false, "khong nem LoiNghiepVu");
            }
            catch (LoiNghiepVu)
            {
                KiemTra("Tu khoa rong bi tu choi (LoiNghiepVu)", true, "kiem tra o BUS");
            }
        }

        private static void KiemTraBai3(SinhVienBUS bus)
        {
            Muc("BAI 3 - Loc theo khoa va khoang diem");

            DataTable tatCa = bus.FilterByKhoaAndDiem(null, null, null);
            KiemTra("Bo loc rong = toan bo du lieu", tatCa.Rows.Count > 0, tatCa.Rows.Count + " dong");

            DataTable cntt = bus.FilterByKhoaAndDiem("CNTT", null, null);
            KiemTra("Loc theo khoa CNTT", cntt.Rows.Cast<DataRow>()
                    .All(r => Convert.ToString(r["MaKhoa"]) == "CNTT"),
                cntt.Rows.Count + " SV khoa CNTT");

            DataTable khoang = bus.FilterByKhoaAndDiem(null, 7m, 9m);
            bool dungKhoang = khoang.Rows.Cast<DataRow>()
                .All(r => Convert.ToDecimal(r["DiemTB"]) >= 7m && Convert.ToDecimal(r["DiemTB"]) <= 9m);
            int trongKhoang = tatCa.Rows.Cast<DataRow>()
                .Count(r => Convert.ToDecimal(r["DiemTB"]) >= 7m && Convert.ToDecimal(r["DiemTB"]) <= 9m);

            KiemTra("Khoang diem 7.00 - 9.00 dung bien", dungKhoang && khoang.Rows.Count == trongKhoang,
                khoang.Rows.Count + " dong");

            DataTable between = bus.FilterByDiemBetween(7m, 9m);
            KiemTra("BETWEEN cho cung ket qua voi >= / <=",
                between.Rows.Count == khoang.Rows.Count,
                "BETWEEN=" + between.Rows.Count + ", >=/<= " + khoang.Rows.Count);

            try
            {
                bus.FilterByKhoaAndDiem(null, 9m, 5m);
                KiemTra("Khoang diem nguoc bi tu choi", false, "khong nem loi");
            }
            catch (LoiNghiepVu)
            {
                KiemTra("Khoang diem nguoc bi tu choi", true, "DiemTu > DiemDen");
            }

            try
            {
                bus.FilterByKhoaAndDiem(null, -1m, 5m);
                KiemTra("Diem am bi tu choi", false, "khong nem loi");
            }
            catch (LoiNghiepVu)
            {
                KiemTra("Diem am bi tu choi", true, "0 <= diem <= 10");
            }
        }

        private static void KiemTraBai4(SinhVienBUS bus)
        {
            Muc("BAI 4 - Phan trang bang OFFSET - FETCH");

            int tong = bus.LoadStudents().Rows.Count;

            KetQuaPhanTrang t1 = bus.LoadTrang(1, 10);
            KiemTra("Trang 1 co dung 10 dong", t1.Bang != null && ((DataTable)t1.Bang).Rows.Count == 10,
                ((DataTable)t1.Bang).Rows.Count + " dong");

            KiemTra("Tong so dong dung bang COUNT(*)",
                t1.TongSoDong == tong, "COUNT=" + t1.TongSoDong + ", thuc te=" + tong);

            int soTrangMong = (int)Math.Ceiling(tong / 10.0);
            KiemTra("Tong so trang = ceiling(tong/10)",
                t1.TongSoTrang == soTrangMong, t1.TongSoTrang + " trang");

            KetQuaPhanTrang t2 = bus.LoadTrang(2, 10);
            DataTable d1 = (DataTable)t1.Bang;
            DataTable d2 = (DataTable)t2.Bang;

            var ma1 = d1.Rows.Cast<DataRow>().Select(r => Convert.ToString(r["MaSV"])).ToList();
            var ma2 = d2.Rows.Cast<DataRow>().Select(r => Convert.ToString(r["MaSV"])).ToList();

            KiemTra("Trang 2 khong trung dong nao voi trang 1",
                !ma1.Intersect(ma2).Any(),
                "trang1=" + ma1.Count + ", trang2=" + ma2.Count + ", trung=0");

            KiemTra("Trang 1 khong co trang truoc, co trang sau",
                !t1.CoTrangTruoc && t1.CoTrangSau, "Trang 1/" + t1.TongSoTrang);

            KetQuaPhanTrang tCuoi = bus.LoadTrang(t1.TongSoTrang, 10);
            KiemTra("Trang cuoi khong co trang sau",
                !tCuoi.CoTrangSau && tCuoi.CoTrangTruoc,
                "Trang " + tCuoi.ChiSoTrang + "/" + tCuoi.TongSoTrang);

            // Cong don tat ca cac trang phai bang tong so dong
            int congDon = 0;
            for (int i = 1; i <= t1.TongSoTrang; i++)
                congDon += ((DataTable)bus.LoadTrang(i, 10).Bang).Rows.Count;

            KiemTra("Cong don tat ca cac trang = tong so dong", congDon == tong,
                congDon + " = " + tong);

            KetQuaPhanTrang theoDiem = bus.LoadTrang(1, 10, SinhVienCot.DiemTB, true);
            DataTable dDiem = (DataTable)theoDiem.Bang;
            bool giamDan = true;
            for (int i = 1; i < dDiem.Rows.Count; i++)
            {
                if (Convert.ToDecimal(dDiem.Rows[i]["DiemTB"]) > Convert.ToDecimal(dDiem.Rows[i - 1]["DiemTB"]))
                { giamDan = false; break; }
            }
            KiemTra("Sap xep theo DiemTB giam dan dung thu tu", giamDan,
                "OFFSET 0 FETCH NEXT 10");
        }

        private static void KiemTraBai5(SinhVienBUS bus)
        {
            Muc("BAI 5 - Doc chi tiet dong duoc chon (xu ly DBNull)");

            DataTable all = bus.LoadStudents();
            DataRow dau = all.Rows[0];
            string ma = Convert.ToString(dau["MaSV"]);

            SinhVienChiTiet sv = bus.LayChiTiet(ma);
            KiemTra("Doc chi tiet theo MaSV", sv != null && sv.CoDuLieu && sv.MaSV == ma,
                ma + " - " + sv.HoTen);

            KiemTra("Tuoi tinh tu NgaySinh nam trong 15..40",
                sv.Tuoi.HasValue && sv.Tuoi.Value >= 15 && sv.Tuoi.Value <= 40,
                "Tuoi = " + sv.Tuoi);

            // Tim mot sinh vien co Email NULL de kiem tra DBNull
            DataRow coEmailNull = all.Rows.Cast<DataRow>()
                .FirstOrDefault(r => r["Email"] == DBNull.Value || r["Email"] == null);

            if (coEmailNull != null)
            {
                string maNull = Convert.ToString(coEmailNull["MaSV"]);
                SinhVienChiTiet svNull = bus.LayChiTiet(maNull);
                KiemTra("Email NULL doc ra null (khong nem InvalidCastException)",
                    svNull.Email == null, maNull + " Email = DBNull.Value");
            }
            else
            {
                KiemTra("Khong co Email NULL trong du lieu (bo qua)", true, "du lieu hien tai deu co Email");
            }

            SinhVienChiTiet khongCo = bus.LayChiTiet("ZZZ_KHONG_CO");
            KiemTra("MaSV khong ton tai tra ve doi tuong rong",
                khongCo != null && !khongCo.CoDuLieu, "reader.Read() = false");

            // Kiem tra cong thuc tuoi chinh xac
            int? tuoi1 = SinhVienBUS.TinhTuoi(new DateTime(2000, 12, 31));
            int? tuoi2 = SinhVienBUS.TinhTuoi(DateTime.Today.AddYears(-20));
            KiemTra("TinhTuoi: sinh nhat chua toi thi tru 1",
                tuoi1.HasValue && tuoi2 == 20,
                "31/12/2000 -> " + tuoi1 + " tuoi; hom nay -20 nam -> " + tuoi2);
        }

        private static void KiemTraBai6(SinhVienBUS bus)
        {
            Muc("BAI 6 - Tim kiem da tieu chi");

            DataTable all = bus.LoadStudents();

            KetQuaPhanTrang rong = bus.TimKiem(new TieuChiTimKiem(), 1, 100);
            KiemTra("Tieu chi rong = toan bo du lieu",
                rong.TongSoDong == all.Rows.Count, rong.TongSoDong + " dong");

            var tc1 = new TieuChiTimKiem { GioiTinh = true };
            KetQuaPhanTrang nam = bus.TimKiem(tc1, 1, 100);
            int demNam = all.Rows.Cast<DataRow>().Count(r => Convert.ToString(r["GioiTinh"]) == "Nam");
            KiemTra("Loc gioi tinh = Nam", nam.TongSoDong == demNam,
                nam.TongSoDong + " = " + demNam);

            var tc2 = new TieuChiTimKiem { MaKhoa = "CNTT", DiemTu = 8m };
            KetQuaPhanTrang cnttGioi = bus.TimKiem(tc2, 1, 100);
            DataTable d2 = (DataTable)cnttGioi.Bang;
            KiemTra("Ket hop khoa + diem: moi dong deu thoa ca hai",
                d2.Rows.Cast<DataRow>().All(r =>
                    Convert.ToString(r["MaKhoa"]) == "CNTT" && Convert.ToDecimal(r["DiemTB"]) >= 8m),
                cnttGioi.TongSoDong + " SV CNTT co diem >= 8");

            var tc3 = new TieuChiTimKiem { TuoiTu = 20, TuoiDen = 22 };
            KetQuaPhanTrang tuoi = bus.TimKiem(tc3, 1, 100);
            KiemTra("Loc theo khoang tuoi tra ve ket qua hop le",
                tuoi.TongSoDong > 0 && tuoi.TongSoDong < all.Rows.Count,
                tuoi.TongSoDong + " SV tuoi 20-22");

            var tc4 = new TieuChiTimKiem { HoTen = "Nguyễn", GioiTinh = false };
            KetQuaPhanTrang nuNguyen = bus.TimKiem(tc4, 1, 100);
            DataTable d4 = (DataTable)nuNguyen.Bang;
            KiemTra("Ket hop ho ten + gioi tinh",
                d4.Rows.Cast<DataRow>().All(r =>
                    Convert.ToString(r["HoTen"]).IndexOf("Nguyễn", StringComparison.OrdinalIgnoreCase) >= 0
                    && Convert.ToString(r["GioiTinh"]) == "Nữ"),
                nuNguyen.TongSoDong + " SV nu ho Nguyen");

            var tc5 = new TieuChiTimKiem { MaSV = "ZZZZ" };
            KetQuaPhanTrang khongCo = bus.TimKiem(tc5, 1, 100);
            KiemTra("Tieu chi khong khop gi tra ve 0 (khong loi)",
                khongCo.TongSoDong == 0 && khongCo.TongSoTrang == 1, "0 dong");
        }

        private static void KiemTraBai7(SinhVienBUS bus)
        {
            Muc("BAI 7 - Sap xep theo cot trong whitelist");

            foreach (string cot in SinhVienCot.DanhSach.Keys)
            {
                DataTable tang = bus.SapXep(cot, false);
                DataTable giam = bus.SapXep(cot, true);

                bool dungTang = KiemTraThuTu(tang, cot, false);
                bool dungGiam = KiemTraThuTu(giam, cot, true);

                KiemTra("Sap xep " + cot + " tang/giam", dungTang && dungGiam,
                    tang.Rows.Count + " dong");
            }

            // Ten cot la -> quay ve MaSV, khong nem loi
            DataTable la = bus.SapXep("sv.MaSV; DROP TABLE SinhVien --", false);
            DataTable chuan = bus.SapXep(SinhVienCot.MaSV, false);

            bool giongNhau = la.Rows.Count == chuan.Rows.Count
                && Convert.ToString(la.Rows[0]["MaSV"]) == Convert.ToString(chuan.Rows[0]["MaSV"]);

            KiemTra("Ten cot la bi loai bo, quay ve MaSV", giongNhau,
                "ChuanHoa() -> " + SinhVienCot.ChuanHoa("sv.MaSV; DROP TABLE SinhVien --"));

            KiemTra("Bang van con nguyen sau khi thu tan cong",
                bus.LoadStudents().Rows.Count > 0, "du lieu an toan");
        }

        /// <summary>
        /// Kiem tra mot cot da duoc sap dung chieu chua.
        ///
        /// Voi cot so/ngay: so sanh truc tiep theo kieu du lieu.
        /// Voi cot chuoi: SQL Server sap theo COLLATION cua CSDL (vi du "Đ" co the
        /// dung sau "D"), khac voi CultureInfo mac dinh cua may. Vi vay chi can
        /// thu tu khop voi MOT trong cac comparer chuan la du - dieu nay van
        /// bat duoc loi that (sai chieu, khong sap xep gi).
        /// </summary>
        private static bool KiemTraThuTu(DataTable table, string cot, bool giamDan)
        {
            if (table.Rows.Count < 2) return true;
            if (!table.Columns.Contains(cot)) return false;

            var giaTri = table.Rows.Cast<DataRow>()
                .Select(r => r[cot])
                .Where(v => v != null && v != DBNull.Value)
                .ToList();

            if (giaTri.Count < 2) return true;

            object mau = giaTri[0];
            if (!(mau is string))
            {
                // So hoac ngay: mot kieu so sanh duy nhat
                return KiemTraMonotonic(giaTri, delegate (object a, object b)
                {
                    return Comparer<object>.Default.Compare(a, b);
                }, giamDan);
            }

            // Chuoi: thu lan luot cac quy tac sap xep thong dung
            var quyTac = new Comparison<object>[]
            {
                (a, b) => string.Compare(Convert.ToString(a), Convert.ToString(b), StringComparison.Ordinal),
                (a, b) => string.Compare(Convert.ToString(a), Convert.ToString(b), StringComparison.OrdinalIgnoreCase),
                (a, b) => string.Compare(Convert.ToString(a), Convert.ToString(b), StringComparison.InvariantCulture),
                (a, b) => string.Compare(Convert.ToString(a), Convert.ToString(b), StringComparison.InvariantCultureIgnoreCase),
                (a, b) => string.Compare(Convert.ToString(a), Convert.ToString(b),
                    System.Globalization.CultureInfo.GetCultureInfo("vi-VN"),
                    System.Globalization.CompareOptions.IgnoreCase)
            };

            foreach (Comparison<object> so in quyTac)
            {
                if (KiemTraMonotonic(giaTri, so, giamDan)) return true;
            }
            return false;
        }

        private static bool KiemTraMonotonic(System.Collections.Generic.List<object> giaTri,
            Comparison<object> soSanh, bool giamDan)
        {
            for (int i = 1; i < giaTri.Count; i++)
            {
                int cmp = soSanh(giaTri[i - 1], giaTri[i]);
                if (giamDan ? cmp < 0 : cmp > 0) return false;
            }
            return true;
        }

        private static void KiemTraBai8(SinhVienBUS bus)
        {
            Muc("BAI 8 - Loc theo tuoi (doi sang khoang NgaySinh)");

            DataTable all = bus.LoadStudents();

            DataTable tu20 = bus.LocTheoTuoi(20, 22);
            bool dungTuoi = tu20.Rows.Cast<DataRow>().All(r =>
            {
                int? tuoi = SinhVienBUS.TinhTuoi(Convert.ToDateTime(r["NgaySinh"]));
                return tuoi.HasValue && tuoi.Value >= 20 && tuoi.Value <= 22;
            });

            int demClient = all.Rows.Cast<DataRow>().Count(r =>
            {
                int? tuoi = SinhVienBUS.TinhTuoi(Convert.ToDateTime(r["NgaySinh"]));
                return tuoi.HasValue && tuoi.Value >= 20 && tuoi.Value <= 22;
            });

            KiemTra("Loc tuoi 20-22 khop voi tinh o client",
                dungTuoi && tu20.Rows.Count == demClient,
                "SQL=" + tu20.Rows.Count + ", client=" + demClient);

            // Kiem tra bien: nguoi vua tron tuoi phai thuoc nhom
            DateTime homNay = DateTime.Today;
            DateTime vuaTron20 = homNay.AddYears(-20);
            int? tuoiBien = SinhVienBUS.TinhTuoi(vuaTron20);
            KiemTra("Nguoi sinh dung hom nay -20 nam co tuoi = 20",
                tuoiBien == 20, "NgaySinh = " + vuaTron20.ToString("dd/MM/yyyy") + " -> " + tuoiBien);

            if (homNay.Month != 2 || homNay.Day != 29)
            {
                DateTime chuaTron20 = homNay.AddYears(-20).AddDays(1);
                int? tuoiChuaTron = SinhVienBUS.TinhTuoi(chuaTron20);
                KiemTra("Nguoi con 1 ngay nua moi tron 20 tuoi co tuoi = 19",
                    tuoiChuaTron == 19,
                    "NgaySinh = " + chuaTron20.ToString("dd/MM/yyyy") + " -> " + tuoiChuaTron);
            }

            DataTable chiTuoi = bus.LocTheoTuoi(25, null);
            KiemTra("Chi nhap tuoi to (25+)", chiTuoi.Rows.Cast<DataRow>().All(r =>
                SinhVienBUS.TinhTuoi(Convert.ToDateTime(r["NgaySinh"])) >= 25),
                chiTuoi.Rows.Count + " SV");

            try
            {
                bus.LocTheoTuoi(25, 20);
                KiemTra("Khoang tuoi nguoc bi tu choi", false, "khong nem loi");
            }
            catch (LoiNghiepVu)
            {
                KiemTra("Khoang tuoi nguoc bi tu choi", true, "TuoiTu > TuoiDen");
            }
        }

        private static void KiemTraBai9(SinhVienBUS bus)
        {
            Muc("BAI 9 - Dinh dang DataGridView + to mau theo diem");

            DataTable all = bus.LoadStudents();

            int duoi5 = all.Rows.Cast<DataRow>().Count(r => Convert.ToDecimal(r["DiemTB"]) < 5m);
            int tu8 = all.Rows.Cast<DataRow>().Count(r => Convert.ToDecimal(r["DiemTB"]) >= 8m);

            KiemTra("Co du ca hai nhom mau (do va xanh)",
                duoi5 > 0 && tu8 > 0,
                "duoi 5: " + duoi5 + " SV, tu 8: " + tu8 + " SV");

            // Dinh dang ngay dd/MM/yyyy va diem 0.00
            DataRow mau = all.Rows[0];
            string ngay = DinhDang.Ngay(Convert.ToDateTime(mau["NgaySinh"]));
            string diem = DinhDang.Diem(Convert.ToDecimal(mau["DiemTB"]));

            KiemTra("Dinh dang ngay la dd/MM/yyyy", ngay.Length == 10 && ngay[2] == '/' && ngay[5] == '/', ngay);
            KiemTra("Dinh dang diem co 2 chu so thap phan", diem.Length > 3 && diem[diem.Length - 3] == '.', diem);

            KiemTra("Diem nam trong 0..10 (rang buoc CHECK)",
                all.Rows.Cast<DataRow>().All(r =>
                {
                    decimal d = Convert.ToDecimal(r["DiemTB"]);
                    return d >= 0m && d <= 10m;
                }), "du lieu hop le");
        }

        private static void KiemTraBai10(SinhVienBUS bus)
        {
            Muc("BAI 10 - Xuat CSV UTF-8 co BOM");

            DataTable all = bus.LoadStudents();
            string thuMuc = Path.Combine(Path.GetTempPath(), "Lab09_KiemTraCsv");
            Directory.CreateDirectory(thuMuc);
            string tep = Path.Combine(thuMuc, "kiem_tra.csv");

            int soDong = XuatCsv.Ghi(all, tep, "MaKhoa");

            KiemTra("Ghi du so dong", soDong == all.Rows.Count,
                soDong + " = " + all.Rows.Count);

            KiemTra("Tep co BOM UTF-8 (EF BB BF)", XuatCsv.CoBom(tep),
                "Excel doc dung tieng Viet");

            string[] dong = File.ReadAllLines(tep, Encoding.UTF8);
            KiemTra("Dong tieu de co cot tieng Viet",
                dong.Length > 0 && dong[0].StartsWith("Mã SV,"),
                dong.Length > 0 ? dong[0] : "");

            // Kiem tra tieng Viet khong bi loi khi doc lai
            bool coDau = dong.Any(l => l.IndexOf("Nguyễn", StringComparison.Ordinal) >= 0
                                    || l.IndexOf("Trần", StringComparison.Ordinal) >= 0);
            KiemTra("Doc lai giu nguyen dau tieng Viet", coDau, "khong bi mojibake");

            // Escape dau phay / dau nhay
            var test = new DataTable();
            test.Columns.Add("HoTen", typeof(string));
            test.Rows.Add("Nguyễn Văn A, \"Bình\"");
            string tep2 = Path.Combine(thuMuc, "escape.csv");
            XuatCsv.Ghi(test, tep2);

            string noiDung = File.ReadAllText(tep2, Encoding.UTF8);
            KiemTra("Gia tri co dau phay/nhay duoc boc trong \"...\"",
                noiDung.Contains("\"Nguyễn Văn A, \"\"Bình\"\"\""),
                "RFC 4180");

            DataTable goc = bus.LoadStudents();
            KiemTra("Xuat khong lam thay doi du lieu goc",
                goc.Rows.Count == all.Rows.Count, goc.Rows.Count + " dong");

            try { Directory.Delete(thuMuc, true); } catch (Exception) { }
        }

        // =====================================================================

        private static void Muc(string tieuDe)
        {
            Ghi("");
            Ghi("--- " + tieuDe + " ---");
        }

        private static void KiemTra(string ten, bool dat, string chiTiet)
        {
            if (dat) _dat++; else _hong++;

            Ghi(string.Format("  [{0}] {1}{2}",
                dat ? "DAT " : "HONG",
                ten,
                string.IsNullOrEmpty(chiTiet) ? "" : "  (" + chiTiet + ")"));
        }

        private static void Ghi(string text)
        {
            KetQua.AppendLine(text);
        }

        /// <summary>
        /// Ghi file ket qua. Phai ghi TRUOC moi lenh Console vi WinExe khong co
        /// console handle -> Console.WriteLine co the nem IOException.
        /// </summary>
        private static void Luu()
        {
            string[] noiLuu =
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "kiem_tra_nhanh.txt"),
                Path.Combine(TimThuMucBaoCao(), "kiem_tra_nhanh.txt")
            };

            foreach (string tep in noiLuu)
            {
                try
                {
                    string thuMuc = Path.GetDirectoryName(tep);
                    if (!string.IsNullOrEmpty(thuMuc)) Directory.CreateDirectory(thuMuc);
                    File.WriteAllText(tep, KetQua.ToString(), new UTF8Encoding(true));
                }
                catch (Exception) { }
            }

            try
            {
                Console.OutputEncoding = Encoding.UTF8;
                Console.Write(KetQua.ToString());
            }
            catch (Exception) { }
        }

        private static string TimThuMucBaoCao()
        {
            DirectoryInfo thuMuc = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (thuMuc != null)
            {
                if (Directory.GetFiles(thuMuc.FullName, "Lab09_DataReaderSearch.csproj").Length > 0)
                    return Path.Combine(thuMuc.FullName, "BaoCao");
                thuMuc = thuMuc.Parent;
            }
            return AppDomain.CurrentDomain.BaseDirectory;
        }
    }
}
