using System;

namespace Lap2
{
    public static class Bai02_Constructor
    {
        public static void Run()
        {
            Console.WriteLine("--- Bài 2: Constructor và nạp chồng constructor ---");

            SinhVien svMacDinh = new SinhVien();
            Console.WriteLine("Constructor không tham số:");
            svMacDinh.HienThi();

            SinhVien svDayDu = new SinhVien("SV02", "Trần Bình", new DateTime(2005, 9, 3), 6.8);
            Console.WriteLine("Constructor đầy đủ tham số:");
            svDayDu.HienThi();
        }
    }
}
