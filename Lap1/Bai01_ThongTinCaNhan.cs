using System;

namespace Lap1
{
    public static class Bai01_ThongTinCaNhan
    {
        public static void Run()
        {
            Console.Write("Họ và tên: ");
            string hoTen = Console.ReadLine() ?? "";

            Console.Write("Năm sinh: ");
            int namSinh;
            if (!int.TryParse(Console.ReadLine(), out namSinh))
            {
                Console.WriteLine("Năm sinh không hợp lệ.");
                return;
            }

            int tuoi = DateTime.Now.Year - namSinh;
            Console.WriteLine("Xin chào {0}, tuổi gần đúng: {1}", hoTen, tuoi);
        }
    }
}
