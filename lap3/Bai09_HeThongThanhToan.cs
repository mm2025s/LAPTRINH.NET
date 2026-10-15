using System;
using System.Collections.Generic;

namespace Lap3
{
    /// <summary>
    /// BÀI 9: HỆ THỐNG THANH TOÁN (tự thực hiện)
    /// Kiểm tra và ghi nhận giao dịch qua interface IPayment.
    /// </summary>
    public static class Bai09_HeThongThanhToan
    {
        public static void Run()
        {
            Console.WriteLine("--- Bài 9: Hệ thống thanh toán (interface IPayment) ---");
            Console.WriteLine();

            List<IPayment> ds = new List<IPayment>
            {
                new ThanhToanTienMat(),
                new ChuyenKhoan("1234567890"),
                new ViDienTu("MoMo")
            };

            Console.WriteLine("Các phương thức thanh toán khả dụng:");
            foreach (IPayment pt in ds)
            {
                Console.WriteLine("  - " + pt.TenPhuongThuc);
            }

            decimal[] soTienThu = { 500000m, 15000000m, 25000000m };

            Console.WriteLine();
            Console.WriteLine("Mô phỏng giao dịch:");
            Console.WriteLine("{0,-24} {1,16} {2,12} {3,14} {4,10}",
                "Phương thức", "Số tiền", "Phí", "Tổng trừ", "Kết quả");
            Console.WriteLine(new string('-', 82));

            decimal tongThu = 0;
            foreach (decimal soTien in soTienThu)
            {
                foreach (IPayment pt in ds)
                {
                    string thongBao;
                    bool hopLe = pt.KiemTraHopLe(soTien, out thongBao);

                    if (hopLe)
                    {
                        decimal phi = pt.TinhPhi(soTien);
                        string maGd = pt.GhiNhanGiaoDich(soTien);
                        tongThu += soTien + phi;
                        Console.WriteLine("{0,-24} {1,16:N0} {2,12:N0} {3,14:N0} {4,10}",
                            pt.TenPhuongThuc, soTien, phi, soTien + phi, maGd);
                    }
                    else
                    {
                        Console.WriteLine("{0,-24} {1,16:N0} {2,12} {3,14} {4,10}",
                            pt.TenPhuongThuc, soTien, "-", "-", "Từ chối");
                        Console.WriteLine("{0,58}{1}", "", "Lý do: " + thongBao);
                    }
                }
                Console.WriteLine();
            }

            Console.WriteLine(new string('-', 82));
            Console.WriteLine("Tổng tiền đã ghi nhận (gồm phí): {0:N0} đ", tongThu);
        }
    }
}
