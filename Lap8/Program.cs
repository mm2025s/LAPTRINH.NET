using System;
using System.Threading;
using System.Windows.Forms;
using Lab08_SqlCommand_Parameters.GUI;

namespace Lab08_SqlCommand_Parameters
{
    /// <summary>
    /// Diem vao cua ung dung Lab 08.
    ///
    /// Che do thuong   : mo frmMain.
    /// Che do chup anh : Lab08_SqlCommand_Parameters.exe --screenshots [kichBan]
    ///                   -> mo lan luot 10 Form bai tap, chup PNG vao Lap8/BaoCao.
    ///
    /// Luu y: cac bai 3, 4, 8, 9 THAY DOI du lieu that trong CSDL. Trinh chup anh
    /// chi chay cac thao tac DOC (bai 1, 2, 5, 6, 7, 10) hoac thao tac khong doi du lieu.
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
