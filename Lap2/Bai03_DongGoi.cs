using System;

namespace Lap2
{
    public static class Bai03_DongGoi
    {
        public static void Run()
        {
            Console.WriteLine("--- Bài 3: Đóng gói và kiểm tra dữ liệu ---");

            try
            {
                SinhVien sv = new SinhVien();
                sv.HoTen = "   ";
                Console.WriteLine("Không xảy ra lỗi (không mong đợi).");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Bắt lỗi họ tên rỗng: " + ex.Message);
            }

            try
            {
                SinhVien sv = new SinhVien();
                sv.DiemTB = 11;
                Console.WriteLine("Không xảy ra lỗi (không mong đợi).");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine("Bắt lỗi điểm ngoài [0,10]: " + ex.Message);
            }

            SinhVien hopLe = new SinhVien("SV03", "  Lê Cường  ", new DateTime(2004, 3, 15), 7.5);
            Console.WriteLine("Dữ liệu hợp lệ (họ tên đã Trim):");
            hopLe.HienThi();
        }
    }
}
