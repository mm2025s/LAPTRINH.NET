using System;
using System.Threading;
using System.Windows.Forms;
using Lab10_DisconnectedData.BaoCaoTools;
using Lab10_DisconnectedData.GUI;

namespace Lab10_DisconnectedData
{
    /// <summary>
    /// Diem vao cua ung dung Lab 10.
    ///
    /// Che do thuong   : mo frmMain.
    /// Che do kiem tra : Lab10_DisconnectedData.exe --kiemtra
    ///                   -> chay 10 nhom kiem tra tren tang DAL/BUS that, ghi
    ///                      kiem_tra_nhanh.txt (khong can giao dien).
    /// Che do chup anh : Lab10_DisconnectedData.exe --screenshots [kichBan]
    ///                   -> mo lan luot 11 Form, chup PNG vao Lap10/BaoCao.
    ///
    /// Tham so --chup-1=... la tham so NOI BO: tien trinh cha dung no de goi
    /// tien trinh con chup dung mot Form.
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
            string chupMot = null;

            foreach (string a in args)
            {
                if (string.Equals(a, "--screenshots", StringComparison.OrdinalIgnoreCase))
                    cheDoScreenshot = true;
                else if (string.Equals(a, "--kiemtra", StringComparison.OrdinalIgnoreCase))
                    cheDoKiemTra = true;
                else if (a.StartsWith("--kichban=", StringComparison.OrdinalIgnoreCase))
                    kichBan = a.Substring("--kichban=".Length);
                else if (a.StartsWith("--chup-1=", StringComparison.OrdinalIgnoreCase))
                    chupMot = a.Substring("--chup-1=".Length);
                else if (kichBan == null && !a.StartsWith("--", StringComparison.Ordinal))
                    kichBan = a;
            }

            // --- Tien trinh con: chup dung mot Form roi thoat ---
            if (!string.IsNullOrEmpty(chupMot))
            {
                TrinhChupAnh.ChupMotForm(chupMot);
                return;
            }

            // --- Che do kiem tra headless ---
            if (cheDoKiemTra)
            {
                KiemTraChucNang.Chay();
                return;
            }

            // --- Che do chup anh bao cao ---
            if (cheDoScreenshot)
            {
                var t = new Thread(delegate ()
                {
                    try
                    {
                        new TrinhChupAnh(kichBan).Chay();
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
