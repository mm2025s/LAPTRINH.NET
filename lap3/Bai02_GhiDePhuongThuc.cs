using System;
using System.Collections.Generic;

namespace Lap3
{
    /// <summary>
    /// BÀI 2: GHI ĐÈ PHƯƠNG THỨC HIỂN THỊ
    /// Mục tiêu: Khai báo MoTa() ảo (virtual) ở Nguoi và ghi đè (override)
    /// trong SinhVien; dùng base.MoTa() để tái sử dụng hành vi lớp cha.
    /// </summary>
    public static class Bai02_GhiDePhuongThuc
    {
        public static void Run()
        {
            Console.WriteLine("--- Bài 2: Ghi đè phương thức MoTa() ---");
            Console.WriteLine("Cùng một lời gọi MoTa() nhưng kết quả khác nhau tùy kiểu đối tượng.");
            Console.WriteLine();

            Nguoi nguoi = new Nguoi("Trần Văn B", new DateTime(1988, 2, 20), "Nam");
            SinhVien sv = new SinhVien("SV02", "Trần Bình", new DateTime(2005, 9, 3), 6.8);

            Console.WriteLine("Nguoi.MoTa()   : " + nguoi.MoTa());
            Console.WriteLine("SinhVien.MoTa(): " + sv.MoTa());
            Console.WriteLine();
            Console.WriteLine("=> SinhVien.MoTa() = base.MoTa() + phần riêng (MaSV, DiemTB).");
            Console.WriteLine();

            Console.WriteLine("Đa hình qua danh sách Nguoi (không cần ép kiểu):");
            List<Nguoi> ds = new List<Nguoi>
            {
                new Nguoi("Phạm Văn C", new DateTime(1975, 11, 30), "Nam"),
                new SinhVien("SV03", "Lê Cường", new DateTime(2004, 3, 15), 9.3),
                new SinhVien("SV04", "Phạm Dung", new DateTime(2006, 1, 22), 4.5),
                new Nguoi("Hoàng Thị D", new DateTime(1995, 7, 8), "Nữ")
            };

            foreach (Nguoi item in ds)
            {
                Console.WriteLine("  [{0,-12}] {1}", item.GetType().Name, item.MoTa());
            }

            Console.WriteLine();
            Console.WriteLine("Lưu ý: GetType().Name cho biết kiểu THỰC TẾ tại thời điểm chạy,");
            Console.WriteLine("quyết định phiên bản MoTa() nào được thực thi.");
        }
    }
}
