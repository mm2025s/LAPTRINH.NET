using System;

namespace Lap1
{
    public static class Bai06_GiaiPhuongTrinhBacHai
    {
        public static void Run()
        {
            Console.Write("a = ");
            double a = double.Parse(Console.ReadLine() ?? "0");
            Console.Write("b = ");
            double b = double.Parse(Console.ReadLine() ?? "0");
            Console.Write("c = ");
            double c = double.Parse(Console.ReadLine() ?? "0");

            if (a == 0)
            {
                if (b == 0)
                {
                    if (c == 0)
                        Console.WriteLine("Phương trình vô số nghiệm.");
                    else
                        Console.WriteLine("Phương trình vô nghiệm.");
                }
                else
                {
                    double x = -c / b;
                    Console.WriteLine("Nghiệm x = {0:N2}", x);
                }
            }
            else
            {
                double delta = b * b - 4 * a * c;
                if (delta < 0)
                {
                    Console.WriteLine("Phương trình vô nghiệm.");
                }
                else if (delta == 0)
                {
                    double x = -b / (2 * a);
                    Console.WriteLine("Nghiệm kép x1 = x2 = {0:N2}", x);
                }
                else
                {
                    double x1 = (-b + Math.Sqrt(delta)) / (2 * a);
                    double x2 = (-b - Math.Sqrt(delta)) / (2 * a);
                    Console.WriteLine("x1 = {0:N2}; x2 = {1:N2}", x1, x2);
                }
            }
        }
    }
}
