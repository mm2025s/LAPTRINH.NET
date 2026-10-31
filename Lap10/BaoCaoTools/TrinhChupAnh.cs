using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Lab10_DisconnectedData.GUI;
using Lab10_DisconnectedData.GUI.Helpers;

namespace Lab10_DisconnectedData.BaoCaoTools
{
    /// <summary>
    /// Trinh chup anh bao cao cho Lab 10 (che do --screenshots).
    ///
    /// Moi kich ban chay trong MOT TIEN TRINH RIENG: mo Form, cho du lieu nap
    /// xong, chup, roi thoat. Lam vay de mot Form loi khong keo ca 11 anh
    /// xuong, va de tranh ro ri ket noi giua cac bai.
    ///
    /// Anh duoc luu vao Lap10/BaoCao voi ten Bai10_01_... den Bai10_11_...
    /// </summary>
    public class TrinhChupAnh
    {
        private readonly string _thuMucAnh;
        private readonly string _kichBan;
        private readonly string _nhatKy;

        /// <summary>Danh sach 11 kich ban: ma, ten tep, mo ta.</summary>
        private static readonly string[,] KichBan =
        {
            { "Chup01_Main",       "Bai10_01_frmMain.png",            "Cua so chinh MDI" },
            { "Chup02_Bai1",       "Bai10_02_Bai1_FillDataTable.png",  "Bai 1 - Fill DataTable" },
            { "Chup03_Bai2",       "Bai10_03_Bai2_DataSet.png",        "Bai 2 - DataSet nhieu bang" },
            { "Chup04_Bai3",       "Bai10_04_Bai3_DataRelation.png",   "Bai 3 - DataRelation" },
            { "Chup05_Bai4",       "Bai10_05_Bai4_DataView.png",       "Bai 4 - DataView loc/sap xep" },
            { "Chup06_Bai5",       "Bai10_06_Bai5_CapNhat.png",        "Bai 5 - SqlCommandBuilder" },
            { "Chup07_Bai6",       "Bai10_07_Bai6_DataTableThuCong.png","Bai 6 - DataTable thu cong" },
            { "Chup08_Bai7",       "Bai10_08_Bai7_RowState.png",       "Bai 7 - Theo doi RowState" },
            { "Chup09_Bai8",       "Bai10_09_Bai8_AcceptReject.png",   "Bai 8 - Accept/RejectChanges" },
            { "Chup10_Bai9",       "Bai10_10_Bai9_Xml.png",            "Bai 9 - DataSet XML" },
            { "Chup11_Bai10",      "Bai10_11_Bai10_Merge.png",         "Bai 10 - Merge" }
        };

        public TrinhChupAnh(string kichBan)
        {
            _kichBan = kichBan;
            _thuMucAnh = TimThuMucBaoCao();
            _nhatKy = Path.Combine(_thuMucAnh, "nhat_ky_chup_anh.txt");
        }

        /// <summary>
        /// Chay che do chup anh.
        ///
        /// Neu co tham so kich ban (--kichban=Chup03_Bai2) thi chi chup kich ban
        /// do; con lai chup du 11 anh.
        /// </summary>
        public void Chay()
        {
            Directory.CreateDirectory(_thuMucAnh);
            GhiLog("===== TU DONG CHUP ANH BAO CAO - LAB 10 =====");
            GhiLog("Thoi diem    : " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            GhiLog("Thu muc anh  : " + _thuMucAnh);
            GhiLog("Chan doan CSDL: " + KiemTraCsdl());
            GhiLog("Che do chup  : " + (CoManHinh() ? "CopyFromScreen" : "DrawToBitmap (khong co man hinh)"));
            GhiLog("");

            int soAnh = 0;
            for (int i = 0; i < KichBan.GetLength(0); i++)
            {
                string ma = KichBan[i, 0];
                if (!string.IsNullOrEmpty(_kichBan) &&
                    !string.Equals(_kichBan, ma, StringComparison.OrdinalIgnoreCase)) continue;

                ChayMotKichBan(i);
                soAnh++;
            }

            GhiLog("===== HOAN TAT: " + soAnh + " anh =====");
        }

        // ------------------------------------------------------------------
        // Chay 1 kich ban trong tien trinh con
        // ------------------------------------------------------------------

        private void ChayMotKichBan(int chiSo)
        {
            string ma = KichBan[chiSo, 0];
            string tenTep = KichBan[chiSo, 1];
            string moTa = KichBan[chiSo, 2];

            GhiLog("===== KICH BAN " + ma + " =====");
            GhiLog("Mo ta: " + moTa);

            string exe = Application.ExecutablePath;
            string tienTrinh = "--chup-1=" + ma;

            var psi = new ProcessStartInfo(exe, tienTrinh);
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;

            try
            {
                using (Process p = Process.Start(psi))
                {
                    // Cho toi da 45 giay: Form nao treo thi bo qua, khong treo ca bo anh
                    if (!p.WaitForExit(45000))
                    {
                        try { p.Kill(); } catch (Exception) { }
                        GhiLog("LOI: qua 45 giay, da dung tien trinh con.");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                GhiLog("LOI: khong chay duoc tien trinh con - " + ex.Message);
                return;
            }

            string duongDan = Path.Combine(_thuMucAnh, tenTep);
            if (File.Exists(duongDan))
                GhiLog("Da luu: " + tenTep + " (" + new FileInfo(duongDan).Length + " byte)");
            else
                GhiLog("LOI: khong thay tep " + tenTep + " sau khi tien trinh con ket thuc.");
        }

        // ------------------------------------------------------------------
        // Chup 1 Form (chay trong tien trinh con)
        // ------------------------------------------------------------------

        /// <summary>
        /// Mo Form tuong ung voi ma kich ban, cho du lieu nap xong, chup anh, thoat.
        /// Ham nay duoc goi tu Program.cs khi tham so la --chup-1=...
        /// </summary>
        public static void ChupMotForm(string ma)
        {
            int chiSo = -1;
            for (int i = 0; i < KichBan.GetLength(0); i++)
                if (string.Equals(KichBan[i, 0], ma, StringComparison.OrdinalIgnoreCase)) { chiSo = i; break; }

            if (chiSo < 0) { Environment.Exit(2); return; }

            Form form = TaoForm(ma);
            if (form == null) { Environment.Exit(3); return; }

            // Che do chup anh: khong hoi xac nhan khi dong, neu khong tien trinh
            // con se treo o hop thoai MessageBox khong ai bam.
            var chinh = form as frmMain;
            if (chinh != null) chinh.HoiXacNhanKhiDong = false;

            string thuMuc = TimThuMucBaoCao();
            string tep = Path.Combine(thuMuc, KichBan[chiSo, 1]);

            form.StartPosition = FormStartPosition.Manual;
            form.Location = new Point(0, 0);
            form.ShowInTaskbar = false;

            var dongHo = Stopwatch.StartNew();
            form.Shown += delegate
            {
                // Cho Form xu ly xong Load/OnShown roi moi chup
                var cho = new System.Windows.Forms.Timer();
                cho.Interval = 900;
                cho.Tick += delegate
                {
                    cho.Stop();
                    cho.Dispose();

                    try
                    {
                        ChuanBiTruocKhiChup(form);
                        Chup(form, tep);
                    }
                    catch (Exception ex)
                    {
                        GhiLoi(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "loi_chup_anh.txt"), ex);
                    }

                    form.Close();
                };
                cho.Start();
            };

            form.FormClosed += delegate { Application.ExitThread(); };

            try
            {
                Application.Run(form);
            }
            catch (Exception ex)
            {
                GhiLoi(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "loi_chup_anh.txt"), ex);
            }

            GhiLogTuTienTrinhCon(Path.Combine(thuMuc, "nhat_ky_chup_anh.txt"), ma, tep, dongHo.ElapsedMilliseconds);
        }

        private static Form TaoForm(string ma)
        {
            switch (ma)
            {
                case "Chup01_Main": return new frmMain();
                case "Chup02_Bai1": return new frmBai1();
                case "Chup03_Bai2": return new frmBai2();
                case "Chup04_Bai3": return new frmBai3();
                case "Chup05_Bai4": return new frmBai4();
                case "Chup06_Bai5": return new frmBai5();
                case "Chup07_Bai6": return new frmBai6();
                case "Chup08_Bai7": return new frmBai7();
                case "Chup09_Bai8": return new frmBai8();
                case "Chup10_Bai9": return new frmBai9();
                case "Chup11_Bai10": return new frmBai10();
                default: return null;
            }
        }

        /// <summary>
        /// Tao ra mot trang thai "co du lieu de nhin" truoc khi chup, de anh bao
        /// cao khong phai la man hinh trang.
        /// </summary>
        private static void ChuanBiTruocKhiChup(Form form)
        {
            string ten = form.GetType().Name;

            try
            {
                switch (ten)
                {
                    case "frmMain":
                        // mo san Bai 4 de thay khung MDI co noi dung
                        break;

                    case "frmBai3":
                        Bam(form, "btnTaoQuanHe");
                        break;

                    case "frmBai7":
                        Bam(form, "btnSuaDiem");
                        break;

                    case "frmBai10":
                        Bam(form, "btnMerge");
                        break;
                }
            }
            catch (Exception) { }
        }

        private static void Bam(Form form, string tenNut)
        {
            AccentButton b = TimNut(form, tenNut) as AccentButton;
            if (b != null && b.Enabled) b.PerformClick();
        }

        private static Control TimNut(Control goc, string ten)
        {
            if (goc == null) return null;
            if (goc.Name == ten) return goc;
            foreach (Control c in goc.Controls)
            {
                Control kq = TimNut(c, ten);
                if (kq != null) return kq;
            }
            return null;
        }

        private static void Chup(Form form, string duongDan)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(duongDan));

            Bitmap anh = null;

            if (CoManHinh())
            {
                try
                {
                    var r = form.RectangleToScreen(form.ClientRectangle);
                    anh = new Bitmap(Math.Max(1, r.Width), Math.Max(1, r.Height));
                    using (Graphics g = Graphics.FromImage(anh))
                        g.CopyFromScreen(r.Location, Point.Empty, r.Size);
                }
                catch (Exception) { anh = null; }
            }

            if (anh == null)
            {
                // Khong co man hinh (hoac CopyFromScreen that bai): ve truc tiep
                anh = new Bitmap(Math.Max(1, form.Width), Math.Max(1, form.Height));
                form.DrawToBitmap(anh, new Rectangle(0, 0, anh.Width, anh.Height));
            }

            using (anh) anh.Save(duongDan, System.Drawing.Imaging.ImageFormat.Png);
        }

        private static bool CoManHinh()
        {
            try
            {
                // Session 0 / khong co desktop that -> khong chup man hinh duoc
                if (SystemInformation.TerminalServerSession) return false;
                return Screen.PrimaryScreen != null && Screen.PrimaryScreen.Bounds.Width > 0;
            }
            catch (Exception) { return false; }
        }

        // ------------------------------------------------------------------
        // Chan doan + log
        // ------------------------------------------------------------------

        private static string KiemTraCsdl()
        {
            try
            {
                var dal = new DAL.SinhVienDAL();
                System.Data.DataTable t = dal.GetStudentTable();
                return "Ket noi duoc, SinhVien co " + t.Rows.Count + " ban ghi.";
            }
            catch (Exception ex)
            {
                return "KHONG ket noi duoc: " + ex.Message;
            }
        }

        private static string TimThuMucBaoCao()
        {
            string thuMuc = AppDomain.CurrentDomain.BaseDirectory;

            for (int i = 0; i < 8 && !string.IsNullOrEmpty(thuMuc); i++)
            {
                if (File.Exists(Path.Combine(thuMuc, "Lab10_DisconnectedData.csproj")))
                    return Path.Combine(thuMuc, "BaoCao");

                DirectoryInfo cha = Directory.GetParent(thuMuc);
                thuMuc = cha == null ? null : cha.FullName;
            }

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BaoCao");
        }

        private void GhiLog(string dong)
        {
            try { File.AppendAllText(_nhatKy, dong + Environment.NewLine, new System.Text.UTF8Encoding(true)); }
            catch (Exception) { }
        }

        private static void GhiLogTuTienTrinhCon(string tep, string ma, string tepAnh, long ms)
        {
            try
            {
                if (!File.Exists(tepAnh)) return;   // tien trinh cha se ghi LOI
                File.AppendAllText(tep,
                    "  -> " + ma + ": " + Path.GetFileName(tepAnh) + " (" + ms + " ms)" + Environment.NewLine,
                    new System.Text.UTF8Encoding(true));
            }
            catch (Exception) { }
        }

        private static void GhiLoi(string tep, Exception ex)
        {
            try { File.AppendAllText(tep, DateTime.Now + " : " + ex + Environment.NewLine); }
            catch (Exception) { }
        }
    }
}
