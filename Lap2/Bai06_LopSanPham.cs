using System;
using System.Collections.Generic;

namespace Lap2
{
    public static class Bai06_LopSanPham
    {
        public static void Run()
        {
            Console.WriteLine("--- Bài 6: Lớp SanPham ---");

            List<SanPham> ds = new List<SanPham>();
            ds.Add(new SanPham("SP01", "Bàn phím", 250000, 3));
            ds.Add(new SanPham("SP02", "Chuột máy tính", 150000, 5));
            ds.Add(new SanPham("SP03", "Màn hình", 3200000, 2));

            double tongTien = 0;
            Console.WriteLine("{0,-6} {1,-18} {2,12} {3,6} {4,14}", "Mã", "Tên", "Đơn giá", "SL", "Thành tiền");
            foreach (SanPham sp in ds)
            {
                Console.WriteLine("{0,-6} {1,-18} {2,12:N0} {3,6} {4,14:N0}",
                    sp.MaSP, sp.TenSP, sp.DonGia, sp.SoLuong, sp.ThanhTien());
                tongTien += sp.ThanhTien();
            }
            Console.WriteLine("Tổng giá trị kho: {0:N0}", tongTien);

            Console.WriteLine("\nKiểm tra dữ liệu không hợp lệ:");
            try
            {
                SanPham loi = new SanPham("SP99", "Lỗi", -1000, 1);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine("Bắt lỗi: " + ex.Message);
            }
        }
    }
}
