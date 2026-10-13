using System;

namespace Lap2
{
    public static class Bai01_KhaiBaoLop
    {
        public static void Run()
        {
            SinhVien sv = new SinhVien();
            sv.MaSV = "SV01";
            sv.HoTen = "Nguyễn An";
            sv.NgaySinh = new DateTime(2005, 5, 12);
            sv.DiemTB = 8.2;

            Console.WriteLine("--- Bài 1: Khai báo lớp SinhVien ---");
            sv.HienThi();
        }
    }
}
