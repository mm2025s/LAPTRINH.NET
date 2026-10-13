using System;

namespace Lap1
{
    public static class Bai07_ThongKeDaySo
    {
        public static void Run()
        {
            Console.Write("n = ");
            int n = int.Parse(Console.ReadLine() ?? "0");
            if (n <= 0) return;

            int[] a = new int[n];
            for (int i = 0; i < n; i++)
            {
                Console.Write("a[{0}] = ", i);
                a[i] = int.Parse(Console.ReadLine() ?? "0");
            }

            int tong = 0;
            int max = a[0];
            int min = a[0];
            int demChan = 0;

            for (int i = 0; i < n; i++)
            {
                tong += a[i];
                if (a[i] > max) max = a[i];
                if (a[i] < min) min = a[i];
                if (a[i] % 2 == 0) demChan++;
            }

            double tb = (double)tong / n;
            Console.WriteLine("Tổng = {0}; TB = {1:N2}; Max = {2}; Min = {3}; Số chẵn = {4}", tong, tb, max, min, demChan);
        }
    }
}
