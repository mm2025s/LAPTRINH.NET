using System;

namespace Lap3
{
    /// <summary>
    /// BÀI 1: KẾ THỪA Nguoi - SinhVien
    /// Mục tiêu: Tạo lớp cơ sở Nguoi, lớp SinhVien kế thừa và gọi constructor
    /// lớp cha bằng từ khóa base.
    /// </summary>
    public static class Bai01_KeThua
    {
        public static void Run()
        {
            Console.WriteLine("--- Bài 1: Kế thừa Nguoi - SinhVien ---");
            Console.WriteLine("Minh họa quan hệ kế thừa: SinhVien LÀ MỘT Nguoi (is-a).");
            Console.WriteLine();

            // Khởi tạo trực tiếp lớp cha
            Nguoi nguoi = new Nguoi("Nguyễn Văn A", new DateTime(1990, 4, 8));
            Console.WriteLine("Đối tượng Nguoi  : " + nguoi.MoTa());

            // Lớp con gọi base(ten, ngaySinh) để khởi tạo phần dữ liệu lớp cha
            SinhVien sv = new SinhVien("SV01", "Nguyễn An", new DateTime(2005, 5, 12), 8.2, "Công nghệ thông tin");
            Console.WriteLine("Đối tượng SinhVien: " + sv.MoTa());

            // Chứng minh quan hệ is-a: gán lớp con cho biến kiểu lớp cha
            Nguoi thamChieuCha = sv;
            Console.WriteLine();
            Console.WriteLine("Gán SinhVien cho biến kiểu Nguoi (is-a) và gọi MoTa():");
            Console.WriteLine("  " + thamChieuCha.MoTa());

            // Kiểm tra kiểu để truy cập thành viên riêng của lớp con
            if (thamChieuCha is SinhVien sv2)
            {
                Console.WriteLine("Ép kiểu thành công -> truy cập MaSV: " + sv2.MaSV + ", Ngành: " + sv2.Nganh);
            }

            Console.WriteLine();
            Console.WriteLine("Thứ tự gọi constructor: Nguoi(...) chạy TRƯỚC, sau đó mới đến thân SinhVien(...).");
        }
    }
}
