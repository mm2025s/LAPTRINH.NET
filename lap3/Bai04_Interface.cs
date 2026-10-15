using System;

namespace Lap3
{
    /// <summary>
    /// BÀI 4: INTERFACE KIỂM TRA VÀ IN DỮ LIỆU
    /// Mục tiêu: Tạo IValidatable và IPrintable; cho SinhVien thực thi cả hai.
    /// Interface chỉ KHAI BÁO hợp đồng, lớp cài đặt nội dung chi tiết.
    ///
    /// BÀI HỌC QUAN TRỌNG: có HAI tầng kiểm tra dữ liệu, bổ sung cho nhau:
    ///   (1) Setter của thuộc tính  -> chặn NGAY từng giá trị sai (ném ngoại lệ).
    ///   (2) IsValid() của interface -> kiểm tra QUY TẮC NGHIỆP VỤ trên toàn
    ///       đối tượng (nhiều trường kết hợp) và trả về thông báo thay vì crash.
    /// </summary>
    public static class Bai04_Interface
    {
        public static void Run()
        {
            Console.WriteLine("--- Bài 4: Interface IValidatable & IPrintable ---");
            Console.WriteLine("SinhVien thực thi 3 interface: IValidatable, IPrintable, IXuatDuLieu");
            Console.WriteLine();

            Console.WriteLine(">>> TẦNG 1: Setter chặn từng giá trị sai ngay khi gán");
            ThuGanDuLieuSai("", "Nguyễn An", new DateTime(2005, 5, 12), 8.2);
            ThuGanDuLieuSai("SV01", "Nguyễn An", new DateTime(2005, 5, 12), 11);

            Console.WriteLine();
            Console.WriteLine(">>> TẦNG 2: IsValid() kiểm tra quy tắc nghiệp vụ trên toàn đối tượng");
            Console.WriteLine();

            SinhVien[] ds =
            {
                new SinhVien("SV01", "Nguyễn An", new DateTime(2005, 5, 12), 8.2),
                new SinhVien("1SV02", "Trần Bình", new DateTime(2005, 9, 3), 6.8),
                new SinhVien("SV03", "Lê Cường", new DateTime(2020, 3, 15), 9.3),
                new SinhVien("SV04", "Phạm Dung", new DateTime(2006, 1, 22), 4.5)
            };

            foreach (SinhVien sv in ds)
            {
                string thongBao;
                bool hopLe = sv.IsValid(out thongBao);

                Console.WriteLine("Kiểm tra {0,-8} -> {1}",
                    sv.MaSV,
                    hopLe ? "HỢP LỆ" : "KHÔNG HỢP LỆ: " + thongBao);

                if (hopLe)
                {
                    Console.WriteLine("  ToDisplayText(): " + sv.ToDisplayText());
                    Console.WriteLine("  MoTa()         : " + sv.MoTa());
                }
                Console.WriteLine();
            }

            Console.WriteLine(">>> Gọi thành viên qua THAM CHIẾU INTERFACE");
            Console.WriteLine("(không cần biết lớp cụ thể, chỉ cần biết hợp đồng)");

            IPrintable printable = new SinhVien("SV05", "Hoàng Em", new DateTime(2005, 7, 8), 7.0);
            Console.WriteLine("  IPrintable.ToDisplayText() -> " + printable.ToDisplayText());

            IValidatable validatable = new SinhVien("SV06", "Vũ Phúc", new DateTime(2005, 7, 8), 7.5);
            string msg;
            Console.WriteLine("  IValidatable.IsValid()     -> " + validatable.IsValid(out msg));

            IXuatDuLieu xuat = new SinhVien("SV07", "Đỗ Quý", new DateTime(2005, 7, 8), 8.8);
            Console.WriteLine("  IXuatDuLieu.XuatCsv()      -> " + xuat.XuatCsv());
            Console.WriteLine("  IXuatDuLieu.XuatJson()     -> " + xuat.XuatJson());
        }

        /// <summary>
        /// Thử gán dữ liệu sai để chứng minh setter chặn ngay và ném ngoại lệ.
        /// </summary>
        private static void ThuGanDuLieuSai(string ma, string ten, DateTime ngaySinh, double diem)
        {
            try
            {
                SinhVien sv = new SinhVien(ma, ten, ngaySinh, diem);
                Console.WriteLine("  Không báo lỗi (ngoài mong đợi): " + sv.MoTa());
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("  Bắt lỗi: " + ex.Message);
            }
        }
    }
}
