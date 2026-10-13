using System;

namespace Lap1
{
    public static class Bai05_XuLyNgoaiLePhapChia
    {
        public static void Run()
        {
            try
            {
                Console.Write("a = ");
                int a = int.Parse(Console.ReadLine() ?? "");
                Console.Write("b = ");
                int b = int.Parse(Console.ReadLine() ?? "");
                Console.WriteLine("a / b = " + (a / b));
            }
            catch (FormatException)
            {
                Console.WriteLine("Dữ liệu phải là số nguyên.");
            }
            catch (DivideByZeroException)
            {
                Console.WriteLine("Không thể chia cho 0.");
            }
            finally
            {
                Console.WriteLine("Kết thúc phép tính.");
            }
        }
    }
}
