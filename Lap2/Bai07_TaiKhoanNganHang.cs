using System;

namespace Lap2
{
    public static class Bai07_TaiKhoanNganHang
    {
        public static void Run()
        {
            Console.WriteLine("--- Bài 7: Lớp TaiKhoanNganHang ---");

            TaiKhoanNganHang tkA = new TaiKhoanNganHang("TK001", "Nguyễn An", 1000000);
            TaiKhoanNganHang tkB = new TaiKhoanNganHang("TK002", "Trần Bình", 500000);

            Console.WriteLine("Số dư ban đầu:");
            tkA.HienThi();
            tkB.HienThi();

            Console.WriteLine("\nNạp tiền vào TK001:");
            tkA.NapTien(2000000);

            Console.WriteLine("\nRút tiền từ TK001:");
            tkA.RutTien(500000);

            Console.WriteLine("\nRút quá số dư (kiểm tra chặn số dư âm):");
            tkA.RutTien(99999999);

            Console.WriteLine("\nChuyển tiền TK001 -> TK002:");
            tkA.ChuyenTien(tkB, 300000);

            Console.WriteLine("\nSố dư sau cùng:");
            tkA.HienThi();
            tkB.HienThi();
        }
    }
}
