using System;

namespace Lap2
{
    public static class Bai09_HinhChuNhat
    {
        public static void Run()
        {
            Console.WriteLine("--- Bài 9: Lớp HinhChuNhat ---");

            HinhChuNhat h1 = new HinhChuNhat(5, 3);
            h1.HienThi();

            HinhChuNhat h2 = new HinhChuNhat(4.5, 2.5);
            h2.HienThi();

            Console.WriteLine("\nKiểm tra cạnh không dương:");
            try
            {
                HinhChuNhat loi = new HinhChuNhat(-5, 3);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine("Bắt lỗi: " + ex.Message);
            }
        }
    }
}
