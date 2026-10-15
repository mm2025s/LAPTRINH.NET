using System;

namespace Lap3
{
    /// <summary>
    /// Chương trình chính LAB 03 - KẾ THỪA, ĐA HÌNH, LỚP TRỪU TƯỢNG VÀ INTERFACE.
    /// Học phần: Lập trình .NET (IT3212).
    /// </summary>
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
                Console.WriteLine("\n--- LAB 03: KẾ THỪA, ĐA HÌNH, LỚP TRỪU TƯỢNG VÀ INTERFACE (IT3212) ---");
                Console.WriteLine("-- Bài tập có hướng dẫn --");
                Console.WriteLine("1. Kế thừa Nguoi - SinhVien (base)");
                Console.WriteLine("2. Ghi đè phương thức MoTa() (virtual/override)");
                Console.WriteLine("3. Lớp trừu tượng NguoiHoc & học phí");
                Console.WriteLine("4. Interface IValidatable & IPrintable");
                Console.WriteLine("5. Danh sách đa hình List<NguoiHoc>");
                Console.WriteLine("-- Bài tập tự thực hiện --");
                Console.WriteLine("6. Hệ thống nhân viên (tính lương đa hình)");
                Console.WriteLine("7. Quản lý phương tiện (interface IMove)");
                Console.WriteLine("8. Quản lý tài liệu thư viện");
                Console.WriteLine("9. Hệ thống thanh toán (interface IPayment)");
                Console.WriteLine("10. Nâng cấp quản lý sinh viên");
                Console.WriteLine("0. Thoát");
                Console.Write("Chọn bài (0-10): ");

                if (!int.TryParse(Console.ReadLine(), out chon))
                {
                    Console.WriteLine("Lựa chọn không hợp lệ!");
                    continue;
                }

                Console.WriteLine();
                try
                {
                    switch (chon)
                    {
                        case 1: Bai01_KeThua.Run(); break;
                        case 2: Bai02_GhiDePhuongThuc.Run(); break;
                        case 3: Bai03_LopTruuTuong.Run(); break;
                        case 4: Bai04_Interface.Run(); break;
                        case 5: Bai05_DanhSachDaHinh.Run(); break;
                        case 6: Bai06_HeThongNhanVien.Run(); break;
                        case 7: Bai07_QuanLyPhuongTien.Run(); break;
                        case 8: Bai08_QuanLyThuVien.Run(); break;
                        case 9: Bai09_HeThongThanhToan.Run(); break;
                        case 10: Bai10_NangCapQuanLySinhVien.Run(); break;
                        case 0: Console.WriteLine("Tạm biệt!"); break;
                        default: Console.WriteLine("Lựa chọn không hợp lệ!"); break;
                    }
                }
                catch (Exception ex)
                {
                    // Giữ chương trình không bị crash khi dữ liệu không hợp lệ
                    Console.WriteLine("Đã xảy ra lỗi: " + ex.Message);
                }
            } while (chon != 0);
        }
    }
}
