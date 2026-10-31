using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Lab09_DataReaderSearch.BUS;
using Lab09_DataReaderSearch.GUI;
using Lab09_DataReaderSearch.GUI.Helpers;

namespace Lab09_DataReaderSearch.BaoCaoTools
{
    /// <summary>
    /// TRINH TU CHUP ANH BAO CAO cho Lab 8.
    ///
    /// Chay: Lab09_DataReaderSearch.exe --screenshots
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
                "Chup01_Main", "Chup02_Bai1_DataTable", "Chup03_Bai2_TimKiem",
                "Chup04_Bai3_Loc", "Chup05_Bai4_PhanTrang", "Chup06_Bai5_ChiTiet",
                "Chup07_Bai6_DaTieuChi", "Chup08_Bai7_SapXep", "Chup09_Bai8_Tuoi",
                "Chup10_Bai9_DinhDang", "Chup11_Bai10_CSV"
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

                GhiLog("===== TU DONG CHUP ANH BAO CAO - LAB 9 =====");
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
                var table = new SinhVienBUS().LoadStudents();
                return "Ket noi duoc, SinhVien co " + table.Rows.Count + " ban ghi.";
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
                BomTinHieu(1600);
                Chup("Bai09_01_frmMain", main, false);
            }
            catch (Exception ex) { GhiLog("Loi chup frmMain: " + ex.Message); }
            finally { Environment.Exit(0); }
        }

        private void Chup02_Bai1_DataTable()
        {
            ChupForm(new frmBai1(), "Bai09_02_Bai1_DataTableLoad", 1600);
        }

        private void Chup03_Bai2_TimKiem()
        {
            try
            {
                var f = new frmBai2();
                f.Show();
                BomTinHieu(900);

                Dien(f, "HoTen", "Nguyễn");
                BomTinHieu(300);
                BamNut(f, "btnTim");
                BomTinHieu(800);

                Chup("Bai09_03_Bai2_TimKiem", f, true);
            }
            catch (Exception ex) { GhiLog("Loi chup bai 2: " + ex.Message); }
            finally { Environment.Exit(0); }
        }

        private void Chup04_Bai3_Loc()
        {
            try
            {
                var f = new frmBai3();
                f.Show();
                BomTinHieu(1200);

                Dien(f, "DiemTu", "7.00");
                Dien(f, "DiemDen", "9.00");
                BomTinHieu(300);
                BamNut(f, "btnLoc");
                BomTinHieu(800);

                Chup("Bai09_04_Bai3_LocKhoaDiem", f, true);
            }
            catch (Exception ex) { GhiLog("Loi chup bai 3: " + ex.Message); }
            finally { Environment.Exit(0); }
        }

        private void Chup05_Bai4_PhanTrang()
        {
            try
            {
                var f = new frmBai4();
                f.Show();
                BomTinHieu(1400);

                // Nhảy tới trang 2 để ảnh thể hiện rõ OFFSET 10
                BamNut(f, "btnTrangSau");
                BomTinHieu(900);

                Chup("Bai09_05_Bai4_PhanTrang", f, true);
            }
            catch (Exception ex) { GhiLog("Loi chup bai 4: " + ex.Message); }
            finally { Environment.Exit(0); }
        }

        private void Chup06_Bai5_ChiTiet()
        {
            try
            {
                var f = new frmBai5();
                f.Show();
                BomTinHieu(1500);

                // Chon dong thu 3 tren luoi de vung chi tiet co du lieu
                ChonDongLuoi(f, 3);
                BomTinHieu(900);

                Chup("Bai09_06_Bai5_ChiTiet", f, true);
            }
            catch (Exception ex) { GhiLog("Loi chup bai 5: " + ex.Message); }
            finally { Environment.Exit(0); }
        }

        private void Chup07_Bai6_DaTieuChi()
        {
            try
            {
                var f = new frmBai6();
                f.Show();
                BomTinHieu(1300);

                Dien(f, "DiemTu", "7.00");
                Dien(f, "TuoiTu", "19");
                BomTinHieu(300);
                BamNut(f, "btnTim");
                BomTinHieu(900);

                Chup("Bai09_07_Bai6_DaTieuChi", f, true);
            }
            catch (Exception ex) { GhiLog("Loi chup bai 6: " + ex.Message); }
            finally { Environment.Exit(0); }
        }

        private void Chup08_Bai7_SapXep()
        {
            try
            {
                var f = new frmBai7();
                f.Show();
                BomTinHieu(1200);

                // Chon cot "Diem trung binh" (index 3) va giam dan
                ChonCombo(f, "Cot", 3);
                ChonRadio(f, "radGiam");
                BomTinHieu(300);
                BamNut(f, "btnSapXep");
                BomTinHieu(900);

                Chup("Bai09_08_Bai7_SapXep", f, true);
            }
            catch (Exception ex) { GhiLog("Loi chup bai 7: " + ex.Message); }
            finally { Environment.Exit(0); }
        }

        private void Chup09_Bai8_Tuoi()
        {
            try
            {
                var f = new frmBai8();
                f.Show();
                BomTinHieu(1200);

                Dien(f, "TuoiTu", "20");
                Dien(f, "TuoiDen", "22");
                BomTinHieu(200);
                BamNut(f, "btnLoc");
                BomTinHieu(700);
                BamNut(f, "btnDoiChieu");
                BomTinHieu(900);

                Chup("Bai09_09_Bai8_LocTuoi", f, true);
            }
            catch (Exception ex) { GhiLog("Loi chup bai 8: " + ex.Message); }
            finally { Environment.Exit(0); }
        }

        private void Chup10_Bai9_DinhDang()
        {
            ChupForm(new frmBai9(), "Bai09_10_Bai9_DinhDang", 1800);
        }

        private void Chup11_Bai10_CSV()
        {
            try
            {
                var f = new frmBai10();
                f.Show();
                BomTinHieu(1600);

                // Goi thang ham xuat voi duong dan co dinh (khong hien SaveFileDialog
                // vi hop thoai se chan tien trinh chup anh).
                XuatCsvDeChup(f);
                BomTinHieu(900);

                Chup("Bai09_11_Bai10_XuatCsv", f, true);
            }
            catch (Exception ex) { GhiLog("Loi chup bai 10: " + ex.Message); }
            finally { Environment.Exit(0); }
        }

        /// <summary>
        /// Goi XuatCsv truc tiep tren du lieu dang hien thi (bo qua SaveFileDialog)
        /// va cap nhat lai o xem truoc de anh chup the hien ket qua that.
        /// </summary>
        private void XuatCsvDeChup(Form f)
        {
            try
            {
                var luoi = Tim(f, "luoiSinhVien") as DataGridView;
                if (luoi == null || luoi.DataSource == null) { GhiLog("Khong tim thay luoi cua bai 10"); return; }

                var table = luoi.DataSource as System.Data.DataTable;
                if (table == null) { GhiLog("DataSource cua bai 10 khong phai DataTable"); return; }

                string thuMuc = Path.Combine(_thuMucAnh, "..", "BaoCao_Csv");
                Directory.CreateDirectory(thuMuc);
                string duongDan = Path.GetFullPath(Path.Combine(thuMuc, "KetQua_SinhVien.csv"));

                int soDong = XuatCsv.Ghi(table, duongDan, "MaKhoa");

                _nhatKy.AppendLine("Xuat CSV: " + soDong + " dong -> " + duongDan);
                GhiLog("Xuat CSV: " + soDong + " dong -> " + duongDan);

                // Day noi dung vao o xem truoc tren Form
                TextBox xem = Tim(f, "txtXemTruoc") as TextBox;
                if (xem != null && File.Exists(duongDan))
                {
                    var dong = File.ReadAllLines(duongDan, Encoding.UTF8);
                    var sb = new StringBuilder();
                    for (int i = 0; i < Math.Min(10, dong.Length); i++) sb.AppendLine(dong[i]);
                    xem.Text = sb.ToString();
                }
            }
            catch (Exception ex)
            {
                GhiLog("Loi xuat CSV de chup: " + ex.Message);
            }
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

        /// <summary>Chon mot dong tren DataGridView theo chi so (0-based).</summary>
        private void ChonDongLuoi(Control goc, int chiSo)
        {
            DataGridView luoi = Tim(goc, "luoiSinhVien") as DataGridView;
            if (luoi == null || luoi.Rows.Count == 0) return;

            if (chiSo < 0) chiSo = 0;
            if (chiSo >= luoi.Rows.Count) chiSo = luoi.Rows.Count - 1;
            if (luoi.Columns.Count == 0) return;

            luoi.CurrentCell = luoi.Rows[chiSo].Cells[0];
            luoi.Rows[chiSo].Selected = true;
            luoi.Focus();
            Application.DoEvents();
        }

        private void ChonCombo(Control goc, string ten, int chiSo)
        {
            ComboBox cbo = Tim(goc, ten) as ComboBox;
            if (cbo == null || cbo.Items.Count == 0) return;

            if (chiSo < 0) chiSo = 0;
            if (chiSo >= cbo.Items.Count) chiSo = cbo.Items.Count - 1;
            cbo.SelectedIndex = chiSo;
            Application.DoEvents();
        }

        private void ChonRadio(Control goc, string ten)
        {
            RadioButton rad = Tim(goc, ten) as RadioButton;
            if (rad != null) { rad.Checked = true; Application.DoEvents(); }
        }

        private string TimThuMucBaoCao()
        {
            string[] ungVien = { @"d:\LAPTRINH.NET\Lap9", @"D:\LAPTRINH.NET\Lap9" };
            for (int i = 0; i < ungVien.Length; i++)
            {
                if (File.Exists(Path.Combine(ungVien[i], "Lab09_DataReaderSearch.csproj")))
                    return Path.Combine(ungVien[i], "BaoCao");
            }

            DirectoryInfo thuMuc = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (thuMuc != null)
            {
                if (Directory.GetFiles(thuMuc.FullName, "Lab09_DataReaderSearch.csproj").Length > 0)
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
