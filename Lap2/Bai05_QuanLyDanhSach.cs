using System;
using System.Collections.Generic;

namespace Lap2
{
    public static class Bai05_QuanLyDanhSach
    {
        public static void Run()
        {
            Console.WriteLine("--- Bài 5: Quản lý danh sách SinhVien ---");

            List<SinhVien> ds = new List<SinhVien>();
            ds.Add(new SinhVien("SV01", "Nguyễn An", new DateTime(2005, 5, 12), 8.2));
            ds.Add(new SinhVien("SV02", "Trần Bình", new DateTime(2005, 9, 3), 6.8));

            Console.WriteLine("Danh sách sinh viên:");
            foreach (SinhVien sv in ds) sv.HienThi();

            Console.Write("Mã cần tìm: ");
            string ma = Console.ReadLine() ?? "";
            SinhVien? ketQua = ds.Find(x => x.MaSV.Equals(ma, StringComparison.OrdinalIgnoreCase));
            Console.WriteLine(ketQua == null ? "Không tìm thấy" : ketQua.HoTen);
        }
    }
}
