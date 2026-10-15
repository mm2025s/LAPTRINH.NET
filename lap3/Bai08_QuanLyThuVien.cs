using System;
using System.Collections.Generic;

namespace Lap3
{
    /// <summary>
    /// BÀI 8: QUẢN LÝ TÀI LIỆU THƯ VIỆN (tự thực hiện)
    /// Tìm kiếm và in thông tin theo kiểu tài liệu.
    /// </summary>
    public static class Bai08_QuanLyThuVien
    {
        public static void Run()
        {
            Console.WriteLine("--- Bài 8: Quản lý tài liệu thư viện ---");
            Console.WriteLine();

            List<TaiLieu> ds = new List<TaiLieu>
            {
                new Sach("S001", "Lập trình C# căn bản", 2023, "Nguyễn Văn A", "NXB Giáo Dục"),
                new Sach("S002", "Cấu trúc dữ liệu và giải thuật", 2022, "Trần Thị B", "NXB Khoa Học"),
                new TapChi("T001", "Tạp chí CNTT Việt Nam", 2025, 128, "Tháng 3/2025"),
                new TapChi("T002", "Tạp chí Khoa học trẻ", 2024, 96, "Tháng 11/2024"),
                new LuanVan("L001", "Ứng dụng AI trong giáo dục", 2025, "Lê Văn C", "Thạc sĩ"),
                new LuanVan("L002", "Tối ưu hóa truy vấn CSDL", 2023, "Phạm Thị D", "Tiến sĩ")
            };

            Console.WriteLine("Toàn bộ tài liệu trong thư viện:");
            foreach (TaiLieu tl in ds)
            {
                Console.WriteLine("  " + tl.MoTa());
            }

            Console.WriteLine();
            Console.WriteLine("--- Tìm kiếm theo từ khóa tiêu đề ---");
            string tuKhoa = "C#";
            Console.WriteLine("Từ khóa: \"" + tuKhoa + "\"");
            List<TaiLieu> timThay = TimTheoTieuDe(ds, tuKhoa);
            if (timThay.Count == 0) Console.WriteLine("  Không tìm thấy tài liệu nào.");
            foreach (TaiLieu tl in timThay) Console.WriteLine("  " + tl.ToDisplayText());

            Console.WriteLine();
            Console.WriteLine("--- Lọc theo kiểu tài liệu ---");
            foreach (string loai in new string[] { "Sách", "Tạp chí", "Luận văn" })
            {
                List<TaiLieu> loc = TimTheoLoai(ds, loai);
                Console.WriteLine("{0} ({1} tài liệu):", loai, loc.Count);
                foreach (TaiLieu tl in loc) Console.WriteLine("  " + tl.ToDisplayText());
            }

            Console.WriteLine();
            Console.WriteLine("--- Thống kê theo loại và thời hạn mượn ---");
            Dictionary<string, int> thongKe = new Dictionary<string, int>();
            foreach (TaiLieu tl in ds)
            {
                if (!thongKe.ContainsKey(tl.LoaiTaiLieu)) thongKe[tl.LoaiTaiLieu] = 0;
                thongKe[tl.LoaiTaiLieu]++;
            }
            foreach (KeyValuePair<string, int> kv in thongKe)
            {
                Console.WriteLine("  {0,-10}: {1} tài liệu", kv.Key, kv.Value);
            }

            Console.WriteLine();
            Console.WriteLine("Thời hạn cho mượn tối đa theo loại (đa hình):");
            foreach (TaiLieu tl in ds)
            {
                string han = tl.SoNgayMuonToiDa == 0 ? "Chỉ đọc tại chỗ" : tl.SoNgayMuonToiDa + " ngày";
                Console.WriteLine("  {0,-8} {1,-10} -> {2}", tl.MaTaiLieu, tl.LoaiTaiLieu, han);
            }
        }

        private static List<TaiLieu> TimTheoTieuDe(List<TaiLieu> ds, string tuKhoa)
        {
            List<TaiLieu> ketQua = new List<TaiLieu>();
            foreach (TaiLieu tl in ds)
            {
                if (tl.TieuDe.IndexOf(tuKhoa, StringComparison.OrdinalIgnoreCase) >= 0)
                    ketQua.Add(tl);
            }
            return ketQua;
        }

        private static List<TaiLieu> TimTheoLoai(List<TaiLieu> ds, string loai)
        {
            List<TaiLieu> ketQua = new List<TaiLieu>();
            foreach (TaiLieu tl in ds)
            {
                if (tl.LoaiTaiLieu.Equals(loai, StringComparison.OrdinalIgnoreCase))
                    ketQua.Add(tl);
            }
            return ketQua;
        }
    }
}
