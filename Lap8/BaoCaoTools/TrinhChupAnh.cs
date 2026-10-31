using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Lab08_SqlCommand_Parameters.BUS;
using Lab08_SqlCommand_Parameters.GUI;
using Lab08_SqlCommand_Parameters.GUI.Helpers;

namespace Lab08_SqlCommand_Parameters.BaoCaoTools
{
    /// <summary>
    /// TRINH TU CHUP ANH BAO CAO cho Lab 8.
    ///
    /// Chay: Lab08_SqlCommand_Parameters.exe --screenshots
    ///   - Moi kich ban chay trong MOT TIEN TRINH RIENG (--screenshots <kichBan>)
    ///     de tranh ro ri trang thai giua cac Form.
    ///   - Watchdog 45 giay/kich ban.
    ///   - Anh luu vao Lap8/BaoCao/*.png, nhat ky o nhat_ky_chup_anh.txt.
    ///
    /// Chi chup cac bai KHONG lam thay doi du lieu mau (1, 2, 5, 6, 7, 10).
    /// Bai 3, 4, 8, 9 can thao tac nhap lieu nen de nguoi hoc tu chay va chup tay.
    /// </summary>
    public class TrinhChupAnh
    {
        private const int Le = 26;
        private const int NguongWatchdog = 45000;

        private readonly StringBuilder _nhatKy = new StringBuilder();
        private string _thuMucAnh;
        private bool _coManHinh;
        private readonly string _kichBanRieng;

        public TrinhChupAnh(string kichBanRieng)
        {
            _kichBanRieng = kichBanRieng;
        }

        public void Chay()
        {
            ChayTrenLuongUi(_kichBanRieng);
        }

        private void ChayTrenLuongUi(string riengMotKichBan)
        {
            Application.ThreadException += delegate (object s, ThreadExceptionEventArgs e) { GhiLog("LOI: " + e.Exception); };
            AppDomain.CurrentDomain.UnhandledException += delegate (object s, UnhandledExceptionEventArgs e) { GhiLog("LOI: " + e.ExceptionObject); };

            _thuMucAnh = TimThuMucBaoCao();
            Directory.CreateDirectory(_thuMucAnh);

            string[] kichBan =
            {
                "Chup01_Main", "Chup02_Bai1_ExecuteScalar", "Chup03_Bai2_ExecuteReader",
                "Chup04_Bai5_StoredProcedure", "Chup05_Bai6_TimTheoMa", "Chup06_Bai7_ThongKe",
                "Chup07_Bai10_SqlHelper", "Chup08_Bai3_ThemSinhVien", "Chup09_Bai8_XoaSinhVien",
                "Chup10_Bai9_OutputParameter"
            };

            if (riengMotKichBan == null)
            {
                _nhatKy.Length = 0;

                // Xoa nhat ky cu de lan chay nay bat dau tu trang
                try
                {
                    string tepCu = Path.Combine(_thuMucAnh, "nhat_ky_chup_anh.txt");
                    if (File.Exists(tepCu)) File.Delete(tepCu);
                }
                catch (Exception) { }

                GhiLog("===== TU DONG CHUP ANH BAO CAO - LAB 8 =====");
                GhiLog("Thoi diem    : " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                GhiLog("Thu muc anh  : " + _thuMucAnh);
                GhiLog("Chan doan CSDL: " + KiemTraCsdl());
                GhiLog("");

                _coManHinh = ThuChupManHinh();
                GhiLog("Che do chup  : " + (_coManHinh ? "CopyFromScreen" : "DrawToBitmap (khong co man hinh)"));
                GhiLog("");
            }
            else
            {
                kichBan = new string[] { riengMotKichBan };
                _coManHinh = ThuChupManHinh();
                GhiLog("===== KICH BAN " + riengMotKichBan + " =====");
                GhiLog("Thu muc anh : " + _thuMucAnh);
                GhiLog("Chan doan    : " + KiemTraCsdl());
                GhiLog("Che do chup : " + (_coManHinh ? "CopyFromScreen" : "DrawToBitmap"));
            }

            string exe = Application.ExecutablePath;

            for (int i = 0; i < kichBan.Length; i++)
            {
                if (riengMotKichBan != null)
                {
                    try
                    {
                        MethodInfo m = typeof(TrinhChupAnh).GetMethod(kichBan[i],
                            BindingFlags.Instance | BindingFlags.NonPublic);
                        if (m == null) GhiLog("Khong co kich ban " + kichBan[i]);
                        else m.Invoke(this, null);
                    }
                    catch (Exception ex)
                    {
                        GhiLoiNghiemTrong(kichBan[i], ex);
                    }
                    continue;
                }

                using (var p = new System.Diagnostics.Process())
                {
                    p.StartInfo.FileName = exe;
                    p.StartInfo.Arguments = "--screenshots " + kichBan[i];
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.CreateNoWindow = true;

                    try
                    {
                        p.Start();
                        if (!p.WaitForExit(NguongWatchdog))
                        {
                            GhiLog("[watchdog] " + kichBan[i] + " qua 45 giay -> ket thuc tien trinh con");
                            try { p.Kill(); } catch (Exception) { }
                        }
                    }
                    catch (Exception ex)
                    {
                        GhiLog("LOI chay " + kichBan[i] + ": " + ex.Message);
                    }
                }
            }

            if (riengMotKichBan == null)
            {
                GhiLog("");
                GhiLog("===== HOAN TAT =====");
                LuuNhatKy();
            }
        }

        private static string KiemTraCsdl()
        {
            try
            {
                int soSV = new SinhVienBUS().CountStudents();
                return "Ket noi duoc, SinhVien co " + soSV + " ban ghi.";
            }
            catch (Exception ex)
            {
                return "CHUA KET NOI DUOC CSDL: " + ex.Message;
            }
        }

        // =====================================================================
        // KICH BAN
        // =====================================================================
        private void Chup01_Main()
        {
            frmMain main = null;
            try
            {
                main = new frmMain();
                main.Show();
                BomTinHieu(1500);
                Chup("Bai08_01_frmMain", main, false);
            }
            catch (Exception ex) { GhiLog("Loi chup frmMain: " + ex.Message); }
            finally { Environment.Exit(0); }
        }

        private void Chup02_Bai1_ExecuteScalar()
        {
            ChupForm(new frmBai1(), "Bai08_02_Bai1_ExecuteScalar", 1200);
        }

        private void Chup03_Bai2_ExecuteReader()
        {
            ChupForm(new frmBai2(), "Bai08_03_Bai2_ExecuteReader", 1200);
        }

        private void Chup04_Bai5_StoredProcedure()
        {
            ChupForm(new frmBai5(), "Bai08_04_Bai5_StoredProcedure", 1500);
        }

        private void Chup05_Bai6_TimTheoMa()
        {
            try
            {
                var f = new frmBai6();
                f.Show();
                BomTinHieu(600);

                // Nhap ma SV001 va bam Tim kiem de anh co du lieu
                TextBox txt = Tim(f, "txtMaSV") as TextBox;
                if (txt != null) txt.Text = "SV001";
                BamNut(f, "btnTim");
                BomTinHieu(700);

                Chup("Bai08_05_Bai6_TimTheoMa", f, true);
            }
            catch (Exception ex) { GhiLog("Loi chup bai 6: " + ex.Message); }
            finally { Environment.Exit(0); }
        }

        private void Chup06_Bai7_ThongKe()
        {
            try
            {
                var f = new frmBai7();
                f.Show();
                BomTinHieu(1400);   // Load -> Lay tat ca khoa
                Chup("Bai08_06_Bai7_ThongKe", f, true);
            }
            catch (Exception ex) { GhiLog("Loi chup bai 7: " + ex.Message); }
            finally { Environment.Exit(0); }
        }

        private void Chup07_Bai10_SqlHelper()
        {
            try
            {
                var f = new frmBai10();
                f.Show();
                BomTinHieu(1800);
                Chup("Bai08_07_Bai10_SqlHelper", f, true);
            }
            catch (Exception ex) { GhiLog("Loi chup bai 10: " + ex.Message); }
            finally { Environment.Exit(0); }
        }

        private void Chup08_Bai3_ThemSinhVien()
        {
            try
            {
                var f = new frmBai3();
                f.Show();
                BomTinHieu(900);

                // Dien du lieu mau (khong bam Them de tranh thay doi CSDL)
                Dien(f, "MaSV", "SV023");
                Dien(f, "HoTen", "Nguyễn Văn Minh");
                Dien(f, "DiemTB", "8.25");
                Dien(f, "Email", "minh@eaut.edu.vn");
                BomTinHieu(600);

                Chup("Bai08_08_Bai3_ThemSinhVien", f, true);
            }
            catch (Exception ex) { GhiLog("Loi chup bai 3: " + ex.Message); }
            finally { Environment.Exit(0); }
        }

        private void Chup09_Bai8_XoaSinhVien()
        {
            try
            {
                var f = new frmBai8();
                f.Show();
                BomTinHieu(700);

                // Nhap ma SV001 (co dang ky) de thay canh bao khoa ngoai
                TextBox txt = Tim(f, "txtMaSV") as TextBox;
                if (txt != null) txt.Text = "SV022";
                BomTinHieu(900);

                Chup("Bai08_09_Bai8_XoaSinhVien", f, true);
            }
            catch (Exception ex) { GhiLog("Loi chup bai 8: " + ex.Message); }
            finally { Environment.Exit(0); }
        }

        private void Chup10_Bai9_OutputParameter()
        {
            try
            {
                var f = new frmBai9();
                f.Show();
                BomTinHieu(900);

                Dien(f, "MaSV", "SV023");
                Dien(f, "HoTen", "Nguyễn Văn Minh");
                Dien(f, "DiemTB", "8.25");
                BomTinHieu(500);

                Chup("Bai08_10_Bai9_OutputParameter", f, true);
            }
            catch (Exception ex) { GhiLog("Loi chup bai 9: " + ex.Message); }
            finally { Environment.Exit(0); }
        }

        // =====================================================================
        // PHU TRO
        // =====================================================================
        private void ChupForm(Form f, string tenAnh, int choMs)
        {
            try
            {
                f.Show();
                BomTinHieu(choMs);
                Chup(tenAnh, f, true);
            }
            catch (Exception ex) { GhiLog("Loi chup " + tenAnh + ": " + ex.Message); }
            finally { Environment.Exit(0); }
        }

        private void Dien(Control goc, string ten, string giaTri)
        {
            TextBox txt = Tim(goc, ten) as TextBox;
            if (txt != null) UiFactory.SetVal(txt, giaTri);
        }

        private static Control Tim(Control goc, string ten)
        {
            if (goc.Name == ten) return goc;
            foreach (Control c in goc.Controls)
            {
                Control kq = Tim(c, ten);
                if (kq != null) return kq;
            }
            return null;
        }

        private void BamNut(Control goc, string ten)
        {
            Button b = Tim(goc, ten) as Button;
            if (b != null && b.Visible && b.Enabled)
            {
                b.PerformClick();
                BomTinHieu(250);
            }
        }

        private string TimThuMucBaoCao()
        {
            string[] ungVien = { @"d:\LAPTRINH.NET\Lap8", @"D:\LAPTRINH.NET\Lap8" };
            for (int i = 0; i < ungVien.Length; i++)
            {
                if (File.Exists(Path.Combine(ungVien[i], "Lab08_SqlCommand_Parameters.csproj")))
                    return Path.Combine(ungVien[i], "BaoCao");
            }

            DirectoryInfo thuMuc = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (thuMuc != null)
            {
                if (Directory.GetFiles(thuMuc.FullName, "Lab08_SqlCommand_Parameters.csproj").Length > 0)
                    return Path.Combine(thuMuc.FullName, "BaoCao");
                thuMuc = thuMuc.Parent;
            }
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BaoCao");
        }

        private void GhiLog(string text)
        {
            _nhatKy.AppendLine(text);
            Console.WriteLine("[BaoCao] " + text);

            // Ghi ngay ra file de khong bi mat khi tien trinh goi Environment.Exit(0)
            try
            {
                if (!string.IsNullOrEmpty(_thuMucAnh))
                {
                    Directory.CreateDirectory(_thuMucAnh);
                    File.AppendAllText(Path.Combine(_thuMucAnh, "nhat_ky_chup_anh.txt"),
                        text + Environment.NewLine, new UTF8Encoding(true));
                }
            }
            catch (Exception) { }
        }

        private void LuuNhatKy()
        {
            // Khong lam gi: GhiLog() da append tung dong ra file ngay khi phat sinh,
            // va moi kich ban chay trong tien trinh rieng cung append vao cung file.
            // Ghi lai toan bo _nhatKy o day se lam noi dung bi lap.
        }

        private void GhiLoiNghiemTrong(string noi, Exception ex)
        {
            try
            {
                GhiLog("LOI o " + noi + ": " + ex);
                string thuMuc = string.IsNullOrEmpty(_thuMucAnh) ? AppDomain.CurrentDomain.BaseDirectory : _thuMucAnh;
                File.AppendAllText(Path.Combine(thuMuc, "chup_anh_loi.txt"),
                    DateTime.Now.ToString("HH:mm:ss") + " - " + noi + Environment.NewLine + ex + Environment.NewLine,
                    new UTF8Encoding(true));
            }
            catch (Exception) { }
        }

        private static bool ThuChupManHinh()
        {
            try
            {
                using (var bmp = new Bitmap(16, 16))
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(0, 0, 0, 0, new Size(16, 16));
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private Bitmap CaptureClient(Form f)
        {
            int w = Math.Max(1, f.ClientSize.Width);
            int h = Math.Max(1, f.ClientSize.Height);
            var bmp = new Bitmap(w, h);
            using (Graphics g = Graphics.FromImage(bmp))
            using (var b = new SolidBrush(f.BackColor))
            {
                g.FillRectangle(b, 0, 0, w, h);
            }

            try
            {
                f.DrawToBitmap(bmp, new Rectangle(0, 0, w, h));
            }
            catch (Exception ex)
            {
                GhiLog("DrawToBitmap loi (" + f.GetType().Name + "): " + ex.Message);
            }
            return bmp;
        }

        private static void BomTinHieu(int soMillis)
        {
            DateTime het = DateTime.Now.AddMilliseconds(soMillis);
            while (DateTime.Now < het)
            {
                Application.DoEvents();
                Thread.Sleep(15);
            }
        }

        private void Chup(string ten, Form f, bool dongSauKhiChup)
        {
            Image anh = null;
            Bitmap tuVe = null;

            try
            {
                if (_coManHinh)
                {
                    try
                    {
                        anh = new Bitmap(f.Width, f.Height);
                        using (Graphics g = Graphics.FromImage(anh))
                            g.CopyFromScreen(f.Bounds.Location, Point.Empty, f.Bounds.Size);
                    }
                    catch (Exception ex)
                    {
                        GhiLog("CopyFromScreen loi (" + ex.Message + ") - chuyen sang DrawToBitmap");
                        _coManHinh = false;
                        if (anh != null) { anh.Dispose(); anh = null; }
                    }
                }

                if (anh == null)
                {
                    tuVe = CaptureClient(f);
                    anh = tuVe;
                }

                int w = anh.Width + Le * 2;
                int h = anh.Height + Le * 2;

                using (var ketQua = new Bitmap(w, h, PixelFormat.Format32bppArgb))
                using (Graphics g = Graphics.FromImage(ketQua))
                {
                    UiTheme.SmoothingOn(g);
                    g.Clear(Color.FromArgb(232, 235, 243));

                    for (int i = 10; i >= 1; i--)
                    {
                        using (var b = new SolidBrush(Color.FromArgb(4 + (10 - i) * 2, 20, 44, 42)))
                        {
                            g.FillRectangle(b, Le - i + 3, Le - i + 5, anh.Width + i * 2 - 6, anh.Height + i * 2 - 4);
                        }
                    }

                    g.DrawImage(anh, Le, Le);
                    using (var pen = new Pen(Color.FromArgb(192, 198, 214)))
                        g.DrawRectangle(pen, Le - 1, Le - 1, anh.Width, anh.Height);
                    using (var font = UiTheme.FontBold(8f))
                    using (var brush = new SolidBrush(Color.FromArgb(108, 116, 140)))
                        g.DrawString(ten, font, brush, Le, 6);

                    ketQua.Save(Path.Combine(_thuMucAnh, ten + ".png"), ImageFormat.Png);
                }

                GhiLog("Da luu: " + ten + ".png (" + w + "x" + h + ")");
            }
            catch (Exception ex)
            {
                GhiLog("LOI luu anh " + ten + ": " + ex.Message);
            }
            finally
            {
                if (tuVe != null) tuVe.Dispose();
                else if (anh != null) anh.Dispose();
            }

            if (dongSauKhiChup)
            {
                try { f.Close(); }
                catch (Exception ex) { GhiLog("Dong Form sau khi chup loi: " + ex.Message); }
                BomTinHieu(200);
            }
        }
    }
}
