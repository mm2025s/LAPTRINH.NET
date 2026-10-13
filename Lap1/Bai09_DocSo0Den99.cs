using System;

namespace Lap1
{
    public static class Bai09_DocSo0Den99
    {
        public static void Run()
        {
            Console.Write("Nhập số (0-99): ");
            int n = int.Parse(Console.ReadLine() ?? "0");
            if (n < 0 || n > 99)
            {
                Console.WriteLine("Số không hợp lệ.");
                return;
            }

            if (n < 10)
            {
                switch (n)
                {
                    case 0: Console.WriteLine("không"); break;
                    case 1: Console.WriteLine("một"); break;
                    case 2: Console.WriteLine("hai"); break;
                    case 3: Console.WriteLine("ba"); break;
                    case 4: Console.WriteLine("bốn"); break;
                    case 5: Console.WriteLine("năm"); break;
                    case 6: Console.WriteLine("sáu"); break;
                    case 7: Console.WriteLine("bảy"); break;
                    case 8: Console.WriteLine("tám"); break;
                    case 9: Console.WriteLine("chín"); break;
                }
                return;
            }

            int chuc = n / 10;
            int donVi = n % 10;
            string docChuc = "";
            string docDonVi = "";

            switch (chuc)
            {
                case 1: docChuc = "mười"; break;
                case 2: docChuc = "hai mươi"; break;
                case 3: docChuc = "ba mươi"; break;
                case 4: docChuc = "bốn mươi"; break;
                case 5: docChuc = "năm mươi"; break;
                case 6: docChuc = "sáu mươi"; break;
                case 7: docChuc = "bảy mươi"; break;
                case 8: docChuc = "tám mươi"; break;
                case 9: docChuc = "chín mươi"; break;
            }

            switch (donVi)
            {
                case 0: docDonVi = ""; break;
                case 1: docDonVi = (chuc == 1) ? "một" : "mốt"; break;
                case 2: docDonVi = "hai"; break;
                case 3: docDonVi = "ba"; break;
                case 4: docDonVi = (chuc == 1) ? "bốn" : "tư"; break;
                case 5: docDonVi = "lăm"; break;
                case 6: docDonVi = "sáu"; break;
                case 7: docDonVi = "bảy"; break;
                case 8: docDonVi = "tám"; break;
                case 9: docDonVi = "chín"; break;
            }

            if (donVi == 0)
                Console.WriteLine(docChuc);
            else
                Console.WriteLine("{0} {1}", docChuc, docDonVi);
        }
    }
}
