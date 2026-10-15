using System;
using System.Collections.Generic;

namespace Lap3
{
    /// <summary>
    /// BÀI 7: QUẢN LÝ PHƯƠNG TIỆN (tự thực hiện)
    /// Mô phỏng tốc độ và chi phí vận hành của các phương tiện.
    /// </summary>
    public static class Bai07_QuanLyPhuongTien
    {
        public static void Run()
        {
            Console.WriteLine("--- Bài 7: Quản lý phương tiện (interface IMove) ---");
            Console.WriteLine();

            // Khai báo theo kiểu interface => gọi qua hợp đồng IMove
            List<IMove> ds = new List<IMove>
            {
                new XeMay("Honda Wave"),
                new OTo("Toyota Vios"),
                new XeDap("Đạp thể thao")
            };

            Console.WriteLine("Danh sách phương tiện:");
            foreach (IMove pt in ds)
            {
                Console.WriteLine("  " + pt.MoTa());
            }

            double quangDuong = 150;
            Console.WriteLine();
            Console.WriteLine("Mô phỏng hành trình {0:N0} km:", quangDuong);
            Console.WriteLine("{0,-14} {1,12} {2,14} {3,16}",
                "Phương tiện", "Tốc độ", "Thời gian", "Chi phí");
            Console.WriteLine(new string('-', 60));

            double tongChiPhi = 0;
            foreach (IMove pt in ds)
            {
                double thoiGian = pt.TinhThoiGian(quangDuong);
                double chiPhi = pt.TinhChiPhi(quangDuong);
                tongChiPhi += chiPhi;

                string ten = pt is PhuongTien p ? p.Ten : "Không rõ";
                Console.WriteLine("{0,-14} {1,9:0.#} km/h {2,11:0.0} giờ {3,16:N0}",
                    ten, pt.TocDoToiDa, thoiGian, chiPhi);
            }
            Console.WriteLine(new string('-', 60));
            Console.WriteLine("Tổng chi phí vận hành: {0:N0} đ", tongChiPhi);

            Console.WriteLine();
            Console.WriteLine("Kiểm tra chặn quãng đường âm:");
            try
            {
                ds[0].TinhChiPhi(-10);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine("  Bắt lỗi: " + ex.Message);
            }
        }
    }
}
