using System;

namespace Lap1
{
    /// <summary>
    /// Lab 1 - IT3212: Lập trình .NET
    /// Câu 1: Tạo ứng dụng Console và nhập thông tin cá nhân.
    /// Nhập họ tên, năm sinh; tính tuổi gần đúng và hiển thị thông tin.
    /// </summary>
    class Program
    {
        static void Main()
        {
            // Thiết lập bảng mã UTF-8 để hiển thị đúng tiếng Việt có dấu.
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.Write("Họ và tên: ");
            string hoTen = Console.ReadLine();

            Console.Write("Năm sinh: ");
            int namSinh;

            // int.TryParse trả về false nếu dữ liệu nhập không phải số nguyên,
            // nhờ đó chương trình không bị lỗi mà chỉ thông báo và kết thúc.
            if (!int.TryParse(Console.ReadLine(), out namSinh))
            {
                Console.WriteLine("Năm sinh không hợp lệ.");
                return;
            }

            // Kiểm tra năm sinh có nằm trong khoảng hợp lý hay không.
            int namHienTai = DateTime.Now.Year;
            if (namSinh < namHienTai - 120 || namSinh > namHienTai)
            {
                Console.WriteLine("Năm sinh không hợp lệ.");
                return;
            }

            // Tuổi gần đúng = năm hiện tại - năm sinh.
            int tuoi = namHienTai - namSinh;

            // Chuỗi nội suy giúp ghép biến vào chuỗi dễ đọc hơn.
            Console.WriteLine($"Xin chào {hoTen}, tuổi gần đúng: {tuoi}");
        }
    }
}
