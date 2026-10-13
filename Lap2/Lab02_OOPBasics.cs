using System;

namespace Lap2
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
                Console.WriteLine("\n--- LAB 02: LẬP TRÌNH HƯỚNG ĐỐI TƯỢNG (IT3212) ---");
                Console.WriteLine("-- Bài tập có hướng dẫn --");
                Console.WriteLine("1. Khai báo lớp SinhVien");
                Console.WriteLine("2. Constructor và nạp chồng constructor");
                Console.WriteLine("3. Đóng gói và kiểm tra dữ liệu");
                Console.WriteLine("4. Phương thức nghiệp vụ TinhTuoi() & XepLoai()");
                Console.WriteLine("5. Quản lý danh sách SinhVien");
                Console.WriteLine("-- Bài tập tự thực hiện --");
                Console.WriteLine("6. Lớp SanPham");
                Console.WriteLine("7. Lớp TaiKhoanNganHang");
                Console.WriteLine("8. Lớp NhanVien (bảng lương)");
                Console.WriteLine("9. Lớp HinhChuNhat");
                Console.WriteLine("10. Ứng dụng quản lý sinh viên");
                Console.WriteLine("0. Thoát");
                Console.Write("Chọn bài (0-10): ");

                if (!int.TryParse(Console.ReadLine(), out chon))
                {
                    Console.WriteLine("Lựa chọn không hợp lệ!");
                    continue;
                }

                Console.WriteLine();
                switch (chon)
                {
                    case 1: Bai01_KhaiBaoLop.Run(); break;
                    case 2: Bai02_Constructor.Run(); break;
                    case 3: Bai03_DongGoi.Run(); break;
                    case 4: Bai04_PhuongThucNghiepVu.Run(); break;
                    case 5: Bai05_QuanLyDanhSach.Run(); break;
                    case 6: Bai06_LopSanPham.Run(); break;
                    case 7: Bai07_TaiKhoanNganHang.Run(); break;
                    case 8: Bai08_LopNhanVien.Run(); break;
                    case 9: Bai09_HinhChuNhat.Run(); break;
                    case 10: Bai10_UngDungQuanLySinhVien.Run(); break;
                    case 0: Console.WriteLine("Tạm biệt!"); break;
                    default: Console.WriteLine("Lựa chọn không hợp lệ!"); break;
                }
            } while (chon != 0);
        }
    }
}
