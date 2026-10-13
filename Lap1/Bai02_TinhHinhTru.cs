using System;

namespace Lap1
{
    public static class Bai02_TinhHinhTru
    {
        public static void Run()
        {
            double r, h;
            Console.Write("Bán kính r = ");
            if (!double.TryParse(Console.ReadLine(), out r) || r <= 0) return;

            Console.Write("Chiều cao h = ");
            if (!double.TryParse(Console.ReadLine(), out h) || h <= 0) return;

            double dienTichXQ = 2 * Math.PI * r * h;
            double theTich = Math.PI * r * r * h;
            Console.WriteLine("Sxq = {0:N2}; V = {1:N2}", dienTichXQ, theTich);
        }
    }
}
