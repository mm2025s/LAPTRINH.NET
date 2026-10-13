using System;
using System.Collections.Generic;

namespace Lap2
{
    public static class Bai08_LopNhanVien
    {
        public static void Run()
        {
            Console.WriteLine("--- Bài 8: Lớp NhanVien - Bảng lương ---");

            List<NhanVien> ds = new List<NhanVien>();
            ds.Add(new NhanVien("NV01", "Nguyễn An", 5000000, 2.5, 800000));
            ds.Add(new NhanVien("NV02", "Trần Bình", 5000000, 1.8, 500000));
            ds.Add(new NhanVien("NV03", "Lê Cường", 6000000, 3.0, 1200000));

            Console.WriteLine("{0,-6} {1,-15} {2,12} {3,6} {4,12} {5,15}",
                "Mã", "Họ tên", "Lương CB", "Hệ số", "Phụ cấp", "Tổng lương");
            Console.WriteLine(new string('-', 72));

            double tongQuyLuong = 0;
            foreach (NhanVien nv in ds)
            {
                Console.WriteLine("{0,-6} {1,-15} {2,12:N0} {3,6} {4,12:N0} {5,15:N0}",
                    nv.MaNV, nv.HoTen, nv.LuongCoBan, nv.HeSo, nv.PhuCap, nv.TinhLuong());
                tongQuyLuong += nv.TinhLuong();
            }
            Console.WriteLine(new string('-', 72));
            Console.WriteLine("Tổng quỹ lương: {0:N0}", tongQuyLuong);
        }
    }
}
