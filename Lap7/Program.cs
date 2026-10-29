using System;
using System.Threading;
using System.Windows.Forms;
using Lab07_ADONET_Connection.GUI;

namespace Lab07_ADONET_Connection
{
    /// <summary>
    /// Diem vao cua ung dung Lab 07.
    ///
    /// Che do thuong  : mo frmMain.
    /// Che do chup anh: Lab07_ADONET_Connection.exe --screenshots [kichBan]
    ///                  -> chay tren mot luong STA rieng, tu dong mo cac Form
    ///                     va luu anh PNG vao thu muc BaoCao (xem BaoCaoTools).
    /// </summary>
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool cheDoScreenshot = false;
            string kichBan = null;
            foreach (string a in args)
            {
                if (string.Equals(a, "--screenshots", StringComparison.OrdinalIgnoreCase))
                    cheDoScreenshot = true;
                else if (a.StartsWith("--kichban=", StringComparison.OrdinalIgnoreCase))
                    kichBan = a.Substring("--kichban=".Length);
                else if (kichBan == null && !a.StartsWith("--", StringComparison.Ordinal))
                    kichBan = a;
            }

            if (cheDoScreenshot)
            {
                // Chay tren luong rieng co message loop de co the mo Form that
                Thread t = new Thread(delegate ()
                {
                    try
                    {
                        var trinh = new BaoCaoTools.TrinhChupAnh(kichBan);
                        trinh.Chay();
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
