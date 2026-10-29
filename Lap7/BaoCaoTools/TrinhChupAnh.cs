using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Lab07_ADONET_Connection.BUS;
using Lab07_ADONET_Connection.GUI;
using Lab07_ADONET_Connection.GUI.Helpers;

namespace Lab07_ADONET_Connection.BaoCaoTools
{
    /// <summary>
    /// TRINH TU CHUP ANH BAO CAO cho Lab 7 (khong can thao tac tay).
    ///
    /// Chay: Lab07_ADONET_Connection.exe --screenshots
    ///   - Moi kich ban chay trong mot TIEN TRINH RIENG (--screenshots &lt;kich ban&gt;) + watchdog 45 giay.
    ///   - Anh la giao dien THAT: CopyFromScreen neu co man hinh, nguoc lai Form.DrawToBitmap.
    ///   - Ket qua luu vao Lap7/BaoCao/*.png, nhat ky o BaoCao/nhat_ky_chup_anh.txt.
    ///
    /// Luu y: cac kich ban TRUY VAN can CSDL QLSinhVien dang ton tai.
    ///        Neu chua co, xem huong dan trong README (chay file SQL truoc).
    /// </summary>
    public class TrinhChupAnh
    {
        private const int Le = 26;
        private const int NguongWatchdog = 45000;

        private readonly StringBuilder _nhatKy = new StringBuilder();
        private string _thuMucAnh;
        private bool _coManHinh;
        private int _idAnh;
        private readonly string _kichBanRieng;

        public TrinhChupAnh(string kichBanRieng)
        {
            _kichBanRieng = kichBanRieng;
        }

        /// <summary>Tien trinh cha: chay lan luot tung kich ban trong tien trinh con.</summary>
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
                "Chup01_Main", "Chup02_KiemTraKetNoiWindows", "Chup03_TrangThaiKetNoi",
                "Chup04_TruyVanKhoa", "Chup05_TruyVanSinhVien", "Chup06_TruyVanHocPhan",
                "Chup07_TruyVanDangKy", "Chup08_SoDoQuanHe", "Chup09_KetQuaSQL", "Chup10_LoiKetNoi"
            };

            if (riengMotKichBan == null)
            {
                _nhatKy.Length = 0;
                GhiLog("===== TU DONG CHUP ANH BAO CAO - LAB 7 =====");
                GhiLog("Thoi diem    : " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                GhiLog("Thu muc anh  : " + _thuMucAnh);

                // Kiem tra CSDL truoc khi chup
                string chanDoan = KiemTraCsdl();
                GhiLog("Chan doan CSDL: " + chanDoan);
                GhiLog("");

                _coManHinh = ThuChupManHinh();
                GhiLog("Che do chup  : " + (_coManHinh ? "CopyFromScreen" : "DrawToBitmap (khong co man hinh)"));
                GhiLog("");
            }

            if (riengMotKichBan != null) kichBan = new string[] { riengMotKichBan };

            string exe = Application.ExecutablePath;

            for (int i = 0; i < kichBan.Length; i++)
            {
                if (riengMotKichBan != null)
                {
                    try
                    {
                        MethodInfo m = typeof(TrinhChupAnh).GetMethod(kichBan[i], BindingFlags.Instance | BindingFlags.NonPublic);
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

        private string KiemTraCsdl()
        {
            try
            {
                int soSV = new TruyVanBUS().DemBanGhi("SinhVien");
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
                BomTinHieu(1200);
                Chup("Bai07_01_frmMain", main, false);
            }
            catch (Exception ex) { GhiLog("Loi chup frmMain: " + ex.Message); }
            finally { Environment.Exit(0); }
        }

        private void Chup02_KiemTraKetNoiWindows()
        {
            try
            {
                var f = new frmKetNoi();
                f.Show();
                BomTinHieu(400);
                // Bam nut "Kiem tra ket noi" (Windows Authentication)
                BamNut(f, "btnKiemTra");
                BomTinHieu(900);
                Chup("Bai07_02_KetNoiWindows", f, true);
            }
            catch (Exception ex) { GhiLog("Loi chup ket noi: " + ex.Message); }
            finally { Environment.Exit(0); }
        }

        private void Chup03_TrangThaiKetNoi()
        {
            try
            {
                var f = new frmTrangThai();
                f.Show();
                BomTinHieu(1200);   // Load -> LamMoi
                Chup("Bai07_03_TrangThaiKetNoi", f, true);
            }
            catch (Exception ex) { GhiLog("Loi chup trang thai: " + ex.Message); }
            finally { Environment.Exit(0); }
        }

        private void Chup04_TruyVanKhoa()
        {
            ChupBangTruyVan("Khoa", "Bai07_04_TruyVanKhoa");
        }

        private void Chup05_TruyVanSinhVien()
        {
            ChupBangTruyVan("SinhVien", "Bai07_05_TruyVanSinhVien");
        }

        private void Chup06_TruyVanHocPhan()
        {
            ChupBangTruyVan("HocPhan", "Bai07_06_TruyVanHocPhan");
        }

        private void Chup07_TruyVanDangKy()
        {
            ChupBangTruyVan("DangKy", "Bai07_07_TruyVanDangKy");
        }

        private void Chup08_SoDoQuanHe()
        {
            try
            {
                var f = new frmSoDoQuanHe();
                f.Show();
                BomTinHieu(1500);
                Chup("Bai07_08_SoDoQuanHe", f, true);
            }
            catch (Exception ex) { GhiLog("Loi chup so do: " + ex.Message); }
            finally { Environment.Exit(0); }
        }

        private void Chup09_KetQuaSQL()
        {
            try
            {
                // Ve ket qua truy van SQL (JOIN + GROUP BY) ra bitmap - minh hoa ket qua chay script.
                DataTable dt1 = new TruyVanBUS().GetBang("SinhVien");
                Bitmap bmp = VeBangKetQua("Ket qua truy van: danh sach sinh vien kem ten khoa (JOIN)",
                    dt1, new[] { "MaSV", "HoTen", "NgaySinh", "GioiTinh", "TenKhoa", "DiemTB", "Email" });
                LuuAnhCoVien("Bai07_09_KetQuaSinhVien", bmp);
                bmp.Dispose();
            }
            catch (Exception ex) { GhiLog("Loi chup ket qua SQL: " + ex.Message); }
            finally { Environment.Exit(0); }
        }

        private void Chup10_LoiKetNoi()
        {
            try
            {
                var f = new frmKetNoi();
                f.Show();
                BomTinHieu(400);
                // Sua chuoi ket noi thanh sai de minh hoa bat loi SqlException
                TextBox txt = Tim(f, "txtChuoiKetNoi") as TextBox;
                if (txt != null) txt.Text = "Data Source=(localdb)\\KHONG_TON_TAI;Initial Catalog=QLSinhVien;Integrated Security=True";
                BamNut(f, "btnKiemTra");
                BomTinHieu(900);
                Chup("Bai07_10_LoiKetNoi", f, true);
            }
            catch (Exception ex) { GhiLog("Loi chup loi ket noi: " + ex.Message); }
            finally { Environment.Exit(0); }
        }

        // =====================================================================
        // PHU TRO KICH BAN
        // =====================================================================
        private void ChupBangTruyVan(string tenBang, string tenAnh)
        {
            try
            {
                var f = new frmTruyVan();
                f.Show();
                BomTinHieu(500);
                ComboBox cbo = Tim(f, "cboBang") as ComboBox;
                if (cbo != null && cbo.Items.Contains(tenBang))
                {
                    cbo.SelectedItem = tenBang;
                    BomTinHieu(700);
                }
                Chup(tenAnh, f, true);
            }
            catch (Exception ex) { GhiLog("Loi chup bang " + tenBang + ": " + ex.Message); }
            finally { Environment.Exit(0); }
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
                BomTinHieu(200);
            }
        }

        /// <summary>Ve mot DataTable ra bitmap dep (tieu de + bang ke + so dong).</summary>
        private Bitmap VeBangKetQua(string tieuDe, DataTable dt, string[] cot)
        {
            int[] rong = { 70, 180, 90, 70, 130, 70, 190 };
            int le = 20, rowH = 30, headerH = 40, titleH = 52;
            int w = le * 2;
            for (int i = 0; i < cot.Length; i++) w += rong[i];
            int soDong = Math.Min(dt.Rows.Count, 14);
            int h = titleH + headerH + soDong * rowH + 50;

            var bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                UiTheme.SmoothingOn(g);
                g.Clear(Color.White);

                // Tieu de
                using (var brush = new SolidBrush(UiTheme.Indigo))
                    g.FillRectangle(brush, 0, 0, w, titleH);
                using (var font = UiTheme.FontBold(12f))
                using (var brush = new SolidBrush(Color.White))
                    g.DrawString(tieuDe, font, brush, le, 14);

                int y = titleH;
                // Header
                using (var brush = new SolidBrush(Color.FromArgb(232, 234, 248)))
                    g.FillRectangle(brush, 0, y, w, headerH);
                int x = le;
                using (var font = UiTheme.FontBold(9f))
                using (var brush = new SolidBrush(UiTheme.Ink))
                {
                    for (int i = 0; i < cot.Length; i++)
                    {
                        g.DrawString(cot[i], font, brush, x + 6, y + 12);
                        x += rong[i];
                    }
                }
                y += headerH;

                // Dong
                using (var font = UiTheme.FontBody(8.75f))
                using (var brush = new SolidBrush(UiTheme.Ink))
                using (var pen = new Pen(UiTheme.Border))
                {
                    for (int r = 0; r < soDong; r++)
                    {
                        if (r % 2 == 1)
                        {
                            using (var bg = new SolidBrush(Color.FromArgb(249, 250, 253)))
                                g.FillRectangle(bg, 0, y, w, rowH);
                        }
                        DataRow row = dt.Rows[r];
                        x = le;
                        for (int c = 0; c < cot.Length; c++)
                        {
                            string val = "";
                            if (dt.Columns.Contains(cot[c]))
                            {
                                object o = row[cot[c]];
                                if (o is DateTime) val = ((DateTime)o).ToString("dd/MM/yyyy");
                                else if (o is decimal) val = ((decimal)o).ToString("0.00");
                                else val = Convert.ToString(o);
                            }
                            g.DrawString(UiTheme.Fit(val, 26), font, brush, x + 6, y + 8);
                            x += rong[c];
                        }
                        g.DrawLine(pen, 0, y, w, y);
                        y += rowH;
                    }
                    g.DrawLine(pen, 0, y, w, y);
                }

                // Footer
                using (var font = UiTheme.FontItalic(8.75f))
                using (var brush = new SolidBrush(UiTheme.Muted))
                    g.DrawString("Tong cong " + dt.Rows.Count + " dong (hien thi " + soDong + " dong dau).",
                        font, brush, le, y + 12);
            }
            return bmp;
        }

        private void LuuAnhCoVien(string ten, Bitmap anh)
        {
            int w = anh.Width + Le * 2;
            int h = anh.Height + Le * 2;
            using (var ketQua = new Bitmap(w, h, PixelFormat.Format32bppArgb))
            using (Graphics g = Graphics.FromImage(ketQua))
            {
                UiTheme.SmoothingOn(g);
                g.Clear(Color.FromArgb(232, 235, 243));
                g.DrawImage(anh, Le, Le);
                using (var pen = new Pen(Color.FromArgb(192, 198, 214)))
                    g.DrawRectangle(pen, Le - 1, Le - 1, anh.Width, anh.Height);
                using (var font = UiTheme.FontBold(8f))
                using (var brush = new SolidBrush(Color.FromArgb(108, 116, 140)))
                    g.DrawString(ten, font, brush, Le, 6);

                ketQua.Save(Path.Combine(_thuMucAnh, ten + ".png"), ImageFormat.Png);
            }
            _idAnh++;
            GhiLog("Da luu: " + ten + ".png");
        }

        // =====================================================================
        // TIEN ICH
        // =====================================================================
        private string TimThuMucBaoCao()
        {
            string[] ungVien = { @"d:\LAPTRINH.NET\Lap7", @"D:\LAPTRINH.NET\Lap7" };
            for (int i = 0; i < ungVien.Length; i++)
            {
                if (File.Exists(Path.Combine(ungVien[i], "Lab07_ADONET_Connection.csproj")))
                    return Path.Combine(ungVien[i], "BaoCao");
            }

            DirectoryInfo thuMuc = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (thuMuc != null)
            {
                string[] csproj = Directory.GetFiles(thuMuc.FullName, "Lab07_ADONET_Connection.csproj");
                if (csproj.Length > 0) return Path.Combine(thuMuc.FullName, "BaoCao");
                thuMuc = thuMuc.Parent;
            }
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BaoCao");
        }

        private void GhiLog(string text)
        {
            if (text == _nhatKy.ToString()) return;
            _nhatKy.AppendLine(text);
            Console.WriteLine("[BaoCao] " + text);
        }

        private void LuuNhatKy()
        {
            try
            {
                if (string.IsNullOrEmpty(_thuMucAnh)) return;
                Directory.CreateDirectory(_thuMucAnh);
                File.WriteAllText(Path.Combine(_thuMucAnh, "nhat_ky_chup_anh.txt"), _nhatKy.ToString(), new UTF8Encoding(true));
            }
            catch (Exception) { }
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
                    {
                        g.DrawRectangle(pen, Le - 1, Le - 1, anh.Width, anh.Height);
                    }
                    using (var font = UiTheme.FontBold(8f))
                    using (var brush = new SolidBrush(Color.FromArgb(108, 116, 140)))
                    {
                        g.DrawString(ten, font, brush, Le, 6);
                    }

                    ketQua.Save(Path.Combine(_thuMucAnh, ten + ".png"), ImageFormat.Png);
                }

                _idAnh++;
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
