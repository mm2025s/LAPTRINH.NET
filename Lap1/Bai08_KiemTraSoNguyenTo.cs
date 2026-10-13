using System;

namespace Lap1
{
    public static class Bai08_KiemTraSoNguyenTo
    {
        static bool LaSoNguyenTo(int x)
        {
            if (x < 2) return false;
            for (int i = 2; i <= Math.Sqrt(x); i++)
            {
                if (x % i == 0) return false;
            }
            return true;
        }

        public static void Run()
        {
            Console.Write("n = ");
            int n = int.Parse(Console.ReadLine() ?? "0");

            Console.Write("Các số nguyên tố <= {0}: ", n);
            for (int i = 2; i <= n; i++)
            {
                if (LaSoNguyenTo(i))
                {
                    Console.Write(i + " ");
                }
            }
            Console.WriteLine();
        }
    }
}
