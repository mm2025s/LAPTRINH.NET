using System;
using System.Threading;
using System.Windows.Forms;
using Lab09_DataReaderSearch.GUI;

namespace Lab09_DataReaderSearch
{
    /// <summary>
    /// Diem vao cua ung dung Lab 09.
    ///
    /// Che do thuong   : mo frmMain.
    /// Che do kiem tra : Lab09_DataReaderSearch.exe --kiemtra
    ///                   -> chay 10 nhom kiem tra tren tang DAL/BUS that, ghi
    ///                      kiem_tra_nhanh.txt (khong can giao dien).
    /// Che do chup anh : Lab09_DataReaderSearch.exe --screenshots [kichBan]
    ///                   -> mo lan luot cac Form bai tap, chup PNG vao Lap9/BaoCao.
    ///
    /// Ca 10 bai cua Lab 9 chi DOC du lieu nen trinh chup anh khong lam thay
    /// doi CSDL (bai 10 ghi ra thu muc BaoCao_Csv rieng).
    /// </summary>
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool cheDoScreenshot = false;
            bool cheDoKiemTra = false;
            string kichBan = null;

            foreach (string a in args)
            {
                if (string.Equals(a, "--screenshots", StringComparison.OrdinalIgnoreCase))
                    cheDoScreenshot = true;
                else if (string.Equals(a, "--kiemtra", StringComparison.OrdinalIgnoreCase))
                    cheDoKiemTra = true;
                else if (a.StartsWith("--kichban=", StringComparison.OrdinalIgnoreCase))
                    kichBan = a.Substring("--kichban=".Length);
                else if (kichBan == null && !a.StartsWith("--", StringComparison.Ordinal))
                    kichBan = a;
            }

            if (cheDoKiemTra)
            {
                BaoCaoTools.KiemTraChucNang.Chay();
                return;
            }

            if (cheDoScreenshot)
            {
                var t = new Thread(delegate ()
                {
                    try
                    {
                        new BaoCaoTools.TrinhChupAnh(kichBan).Chay();
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            System.IO.File.AppendAllText(
                                System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "loi_chup_anh.txt"),
                                DateTime.Now + " : " + ex + Environment.NewLine);
                        }
                        catch (Exception) { }
                    }
                    finally
                    {
                        Application.ExitThread();
                    }
                });
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
                Application.Run();
                return;
            }

            Application.Run(new frmMain());
        }
    }
}
