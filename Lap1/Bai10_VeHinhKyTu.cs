using System;

namespace Lap1
{
    public static class Bai10_VeHinhKyTu
    {
        public static void Run()
        {
            int chon;
            do
            {
                Console.WriteLine("1. Tam giác vuông đặc");
                Console.WriteLine("2. Tam giác vuông rỗng");
                Console.WriteLine("3. Hình vuông rỗng");
                Console.WriteLine("0. Thoát");
                int.TryParse(Console.ReadLine(), out chon);

                switch (chon)
                {
                    case 1:
                        Console.Write("h = ");
                        int h1 = int.Parse(Console.ReadLine() ?? "0");
                        for (int i = 1; i <= h1; i++)
                        {
                            for (int j = 1; j <= i; j++) Console.Write("* ");
                            Console.WriteLine();
                        }
                        break;

                    case 2:
                        Console.Write("h = ");
                        int h2 = int.Parse(Console.ReadLine() ?? "0");
                        for (int i = 1; i <= h2; i++)
                        {
                            for (int j = 1; j <= i; j++)
                            {
                                if (j == 1 || j == i || i == h2) Console.Write("* ");
                                else Console.Write("  ");
                            }
                            Console.WriteLine();
                        }
                        break;

                    case 3:
                        Console.Write("a = ");
                        int a = int.Parse(Console.ReadLine() ?? "0");
                        for (int i = 1; i <= a; i++)
                        {
                            for (int j = 1; j <= a; j++)
                            {
                                if (i == 1 || i == a || j == 1 || j == a) Console.Write("* ");
                                else Console.Write("  ");
                            }
                            Console.WriteLine();
                        }
                        break;
                }
            } while (chon != 0);
        }
    }
}
