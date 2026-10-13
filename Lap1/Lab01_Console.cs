using System;

namespace Lap1
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            int chon;
            do
            {
                Console.WriteLine("\n--- LAB 01: IT3212 ---");
                Console.WriteLine("1. Thông tin cá nhân");
                Console.WriteLine("2. Tính hình trụ");
                Console.WriteLine("3. Xếp loại học tập");
                Console.WriteLine("4. Menu tổng 1..n & cửu chương");
                Console.WriteLine("5. Ngoại lệ phép chia");
                Console.WriteLine("6. Giải phương trình bậc 2");
                Console.WriteLine("7. Thống kê dãy số");
                Console.WriteLine("8. Số nguyên tố <= n");
                Console.WriteLine("9. Đọc số 0..99");
                Console.WriteLine("10. Vẽ hình ký tự");
                Console.WriteLine("0. Thoát");
                Console.Write("Chọn bài (0-10): ");

                int.TryParse(Console.ReadLine(), out chon);
                Console.WriteLine();

                switch (chon)
                {
                    case 1: Bai01_ThongTinCaNhan.Run(); break;
                    case 2: Bai02_TinhHinhTru.Run(); break;
                    case 3: Bai03_XepLoaiHocTap.Run(); break;
                    case 4: Bai04_MenuTongVaBangCuuChuong.Run(); break;
                    case 5: Bai05_XuLyNgoaiLePhapChia.Run(); break;
                    case 6: Bai06_GiaiPhuongTrinhBacHai.Run(); break;
                    case 7: Bai07_ThongKeDaySo.Run(); break;
                    case 8: Bai08_KiemTraSoNguyenTo.Run(); break;
                    case 9: Bai09_DocSo0Den99.Run(); break;
                    case 10: Bai10_VeHinhKyTu.Run(); break;
                }
            } while (chon != 0);
        }
    }
}
