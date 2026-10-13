using System;

namespace Lap2
{
    public static class Bai04_PhuongThucNghiepVu
    {
        public static void Run()
        {
            Console.WriteLine("--- Bài 4: Phương thức nghiệp vụ TinhTuoi() và XepLoai() ---");

            SinhVien sv = new SinhVien("SV04", "Phạm Dung", new DateTime(2005, 12, 20), 9.1);

            Console.WriteLine("Họ tên: {0}", sv.HoTen);
            Console.WriteLine("Ngày sinh: {0:dd/MM/yyyy}", sv.NgaySinh);
            Console.WriteLine("Tuổi: {0}", sv.TinhTuoi());
            Console.WriteLine("Điểm TB: {0:N2}", sv.DiemTB);
            Console.WriteLine("Xếp loại: {0}", sv.XepLoai());
        }
    }
}
