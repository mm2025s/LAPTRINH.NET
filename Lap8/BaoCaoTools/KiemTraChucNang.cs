using System;
using System.Collections.Generic;
using System.Text;
using Lab08_SqlCommand_Parameters.BUS;
using Lab08_SqlCommand_Parameters.DAL;
using Lab08_SqlCommand_Parameters.DTO;

namespace Lab08_SqlCommand_Parameters.BaoCaoTools
{
    /// <summary>
    /// KIEM TRA NHANH (khong co giao dien) - chay:
    ///     Lab08_SqlCommand_Parameters.exe --kiemtra
    ///
    /// Muc dich: chung minh 10 bai chay dung tren CSDL that ma khong can mo Form.
    /// Bai 3, 4, 8, 9 CO thay doi du lieu, nen ham nay tu don dep:
    ///   - Them SV900 (bai 3) roi xoa.
    ///   - Them SV901 qua thu tuc (bai 9) roi xoa.
    ///   - Cap nhat diem SV001 roi tra lai gia tri cu (bai 4).
    ///   - Thu xoa SV005 (con dang ky) de thay loi 547, KHONG xoa that (bai 8).
    /// Ket thuc: so ban ghi tro ve dung nhu truoc khi kiem tra.
    /// </summary>
    public static class KiemTraChucNang
    {
        private static readonly StringBuilder _kq = new StringBuilder();
        private static int _dat, _khongDat;

        public static void Chay()
        {
            try { Console.OutputEncoding = Encoding.UTF8; } catch (Exception) { }
            var bus = new SinhVienBUS();

            GhiDau("KIEM TRA NHANH - LAB 8");

            int truoc = 0;
            try { truoc = bus.CountStudents(); }
            catch (Exception ex) { Ghi("Không kết nối được CSDL: " + ex.Message); }

            // ---------------- Bài 1 ----------------
            Thu("Bài 1 - ExecuteScalar COUNT(*)", () =>
            {
                int n = bus.CountStudents();
                return "SinhVien = " + n + " bản ghi";
            });

            // ---------------- Bài 2 ----------------
            Thu("Bài 2 - ExecuteReader danh sách", () =>
            {
                List<SinhVien> ds = bus.DocDanhSach();
                return ds.Count + " dòng; dòng đầu: " + (ds.Count > 0 ? ds[0].ToString() : "(rỗng)");
            });

            // ---------------- Bài 3 ----------------
            Thu("Bài 3 - ExecuteNonQuery INSERT (Email = DBNull)", () =>
            {
                var sv = new SinhVien
                {
                    MaSV = "SV900", HoTen = "Sinh Viên Kiểm Tra", NgaySinh = new DateTime(2005, 5, 5),
                    GioiTinh = true, MaKhoa = "CNTT", DiemTB = 7.50m, Email = string.Empty
                };
                KetQuaThaoTac kq = bus.ThemSinhVien(sv);
                int sau = bus.CountStudents();
                return kq.SoDong + " dòng; tổng sau khi thêm = " + sau;
            });

            // ---------------- Bài 3b: trùng mã phải bị chặn ----------------
            Thu("Bài 3b - Thêm trùng mã phải báo lỗi", () =>
            {
                var sv = new SinhVien
                {
                    MaSV = "SV900", HoTen = "Trùng Mã", NgaySinh = new DateTime(2005, 5, 5),
                    GioiTinh = false, MaKhoa = "CNTT", DiemTB = 6m, Email = null
                };
                try
                {
                    bus.ThemSinhVien(sv);
                    return "KHÔNG chặn được trùng khóa (sai)";
                }
                catch (LoiNghiepVu ex)
                {
                    return "Đã chặn: " + ex.Message;
                }
            });

            // ---------------- Bài 4 ----------------
            Thu("Bài 4 - ExecuteNonQuery UPDATE điểm", () =>
            {
                SinhVien goc = bus.TimTheoMa("SV001");
                if (goc == null) return "không có SV001 để thử";

                KetQuaThaoTac kq = bus.CapNhatDiem("SV001", "9.99");
                SinhVien sau = bus.TimTheoMa("SV001");
                // Trả lại giá trị cũ
                bus.CapNhatDiem("SV001", goc.DiemTB.ToString(
                    "0.00", System.Globalization.CultureInfo.InvariantCulture));

                return string.Format("{0} dòng; {1:0.00} → {2:0.00} → đã trả lại {3:0.00}",
                    kq.SoDong, goc.DiemTB, sau.DiemTB, goc.DiemTB);
            });

            Thu("Bài 4b - Cập nhật mã không tồn tại (rows = 0)", () =>
            {
                KetQuaThaoTac kq = bus.CapNhatDiem("SV999", "8.00");
                return kq.SoDong + " dòng → \"" + kq.ThongBao + "\"";
            });

            Thu("Bài 4c - Chặn SQL Injection (chuỗi chỉ là dữ liệu)", () =>
            {
                int truocDo = bus.CountStudents();
                KetQuaThaoTac kq = null;

                try
                {
                    kq = bus.CapNhatDiem("' OR '1'='1", "10.00");
                }
                catch (LoiNghiepVu ex)
                {
                    return "An toàn: BUS từ chối ngay ở bước kiểm tra mã → " + ex.Message;
                }

                int sauDo = bus.CountStudents();

                if (kq.SoDong > 1)
                    return "NGUY HIỂM: câu lệnh bị chèn, cập nhật " + kq.SoDong + " dòng";

                return "An toàn: tham số hóa, cập nhật " + kq.SoDong + " dòng (không mã nào khớp), " +
                       "số sinh viên " + truocDo + " → " + sauDo;
            });

            // ---------------- Bài 5 ----------------
            Thu("Bài 5 - sp_SinhVien_TheoKhoa (StoredProcedure)", () =>
            {
                List<SinhVien> ds = bus.SinhVienTheoKhoa("CNTT");
                return ds.Count + " sinh viên khoa CNTT";
            });

            // ---------------- Bài 6 ----------------
            Thu("Bài 6 - Tìm theo mã", () =>
            {
                SinhVien sv = bus.TimTheoMa("SV017");
                return sv == null ? "không tìm thấy SV017" : sv.MaSV + " · " + sv.HoTen + " · " + sv.TenKhoa;
            });

            Thu("Bài 6b - Tìm mã không tồn tại", () =>
            {
                SinhVien sv = bus.TimTheoMa("SV999");
                return sv == null ? "null → hiển thị \"Không tìm thấy\"" : "trả về dữ liệu (sai)";
            });

            // ---------------- Bài 7 ----------------
            Thu("Bài 7 - ExecuteScalar MAX(DiemTB) xử lý NULL", () =>
            {
                decimal? cn = bus.DiemCaoNhatCuaKhoa("CNTT");
                List<ThongKeKhoa> ds = bus.ThongKeDiemCaoNhatTheoKhoa();

                string chiTiet = "";
                foreach (ThongKeKhoa t in ds)
                    chiTiet += string.Format(" [{0}: {1}, {2} SV]", t.MaKhoa, t.DiemCaoNhatText, t.SoSinhVien);

                return "CNTT = " + (cn.HasValue ? cn.Value.ToString("0.00") : "NULL") + ";" + chiTiet;
            });

            // ---------------- Bài 8 ----------------
            Thu("Bài 8 - Xóa sinh viên còn đăng ký → lỗi 547", () =>
            {
                int soDangKy = bus.DemDangKy("SV005");
                try
                {
                    bus.XoaSinhVien("SV005");
                    return "xóa được (sai, SV005 còn " + soDangKy + " đăng ký)";
                }
                catch (LoiNghiepVu ex)
                {
                    return "Đã chặn đúng (còn " + soDangKy + " đăng ký): " + ex.Message;
                }
            });

            Thu("Bài 8b - Xóa mã không tồn tại → 0 dòng", () =>
            {
                int rows = bus.XoaSinhVien("SV999");
                return rows + " dòng → \"" + (rows == 0 ? "Không tìm thấy mã" : "có xóa") + "\"";
            });

            // ---------------- Bài 9 ----------------
            Thu("Bài 9 - StoredProcedure + tham số OUTPUT", () =>
            {
                var sv = new SinhVien
                {
                    MaSV = "SV901", HoTen = "Sinh Viên Output", NgaySinh = new DateTime(2005, 6, 6),
                    GioiTinh = false, MaKhoa = "KT", DiemTB = 8.75m, Email = null
                };
                KetQuaThaoTac kq = bus.ThemQuaThuTuc(sv);
                return "@SoLuongHienTai = " + kq.GiaTriOutput + " (trước khi kiểm tra: " + truoc + ")";
            });

            // ---------------- Bài 10 ----------------
            Thu("Bài 10 - SqlHelper.ExecuteScalar/Reader/NonQuery", () =>
            {
                int n = Convert.ToInt32(SqlHelper.ExecuteScalar("SELECT COUNT(*) FROM dbo.SinhVien"));

                int soDong = SqlHelper.ExecuteReader("SELECT HoTen FROM dbo.SinhVien", rd => { });

                int rows = SqlHelper.ExecuteNonQuery(
                    "UPDATE dbo.SinhVien SET DiemTB = DiemTB WHERE MaSV = @MaSV",
                    new[] { SqlHelper.P("@MaSV", System.Data.SqlDbType.VarChar, "SV001", 15) });

                return string.Format("Scalar = {0}, Reader = {1} dòng, NonQuery = {2} dòng", n, soDong, rows);
            });

            // ---------------- Dọn dẹp ----------------
            GhiDau("DON DEP DU LIEU KIEM TRA");
            foreach (string ma in new[] { "SV900", "SV901" })
            {
                try
                {
                    int rows = bus.XoaSinhVien(ma);
                    Ghi("  Xóa " + ma + ": " + rows + " dòng");
                }
                catch (Exception ex)
                {
                    Ghi("  Không xóa được " + ma + ": " + ex.Message);
                }
            }

            try
            {
                int sau = bus.CountStudents();
                Ghi("Số bản ghi trước kiểm tra = " + truoc + ", sau khi dọn = " + sau +
                    (truoc == sau ? "  ✔ khớp" : "  ✖ KHÁC NHAU"));
            }
            catch (Exception) { }

            GhiDau("KET QUA: " + _dat + " đạt / " + _khongDat + " không đạt");

            try
            {
                System.IO.File.WriteAllText(
                    System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "kiem_tra_nhanh.txt"),
                    _kq.ToString(), new UTF8Encoding(true));
            }
            catch (Exception) { }

            try { Console.WriteLine(_kq.ToString()); } catch (Exception) { }
        }

        private static void Thu(string ten, Func<string> ham)
        {
            try
            {
                string kq = ham();
                bool dat = kq != null
                    && kq.IndexOf("KHÔNG", StringComparison.Ordinal) < 0
                    && kq.IndexOf("(sai)", StringComparison.Ordinal) < 0;

                if (dat) _dat++; else _khongDat++;
                Ghi((dat ? "[ĐẠT]     " : "[KHÔNG ĐẠT] ") + ten);
                Ghi("           → " + kq);
            }
            catch (Exception ex)
            {
                _khongDat++;
                Ghi("[LỖI]     " + ten);
                Ghi("           → " + ex.Message);
            }
        }

        private static void Ghi(string text)
        {
            _kq.AppendLine(text);
        }

        private static void GhiDau(string text)
        {
            _kq.AppendLine();
            _kq.AppendLine("========== " + text + " ==========");
        }
    }
}
