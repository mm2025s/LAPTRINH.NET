using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Lab11_CRUD_StoredProcedure.BUS;
using Lab11_CRUD_StoredProcedure.GUI;

namespace Lab11_CRUD_StoredProcedure
{
    /// <summary>
    /// Lab 11 - Bai 10. Chay thuong: mo frmBai10.
    /// --kiemtra : doi chieu tung tinh huong, ghi BaoCao\kiem_tra_nhanh.txt.
    /// --chupanh : chup Form vao BaoCao\Bai11_10_XuLyLoi.png.
    /// </summary>
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length > 0 && args[0] == "--kiemtra") { KiemTra(); return; }
            if (args.Length > 0 && args[0] == "--chupanh") { ChupAnh(); return; }

            Application.Run(new frmBai10());
        }

        private static void KiemTra()
        {
            var sb = new StringBuilder();
            int dat = 0, tong = 0;

            foreach (TinhHuongLoi th in TinhHuongLoi.DanhSach())
            {
                ThongTinLoi tt = null;
                try { th.Lenh(); }
                catch (Exception ex) { tt = XuLyLoi.PhanLoai(ex, th.Ten); }

                bool canMa = th.Mong == LoaiLoi.Sql || th.Mong == LoaiLoi.HeThong;
                bool ok = tt != null && tt.Loai == th.Mong
                    && canMa == !string.IsNullOrEmpty(tt.MaThamChieu)
                    && (!canMa || XuLyLoi.DocNhatKy().Contains(tt.MaThamChieu))
                    && !tt.ThongBao.Contains("Exception");

                tong++;
                if (ok) dat++;
                sb.AppendLine(string.Format("[{0}] {1} -> {2}", ok ? "DAT" : "SAI", th.Ten,
                    tt == null ? "(khong co loi)" : tt.TenLoai + ": " + tt.ThongBao));
            }

            sb.AppendLine("KET QUA: " + dat + "/" + tong + " phep kiem tra dat.");
            File.WriteAllText(Path.Combine(ThuMucBaoCao(), "kiem_tra_nhanh.txt"), sb.ToString(), new UTF8Encoding(true));
            Environment.ExitCode = tong - dat;
        }

        private static void ChupAnh()
        {
            using (var form = new frmBai10())
            {
                form.Show();
                form.ChayTatCa();
                Application.DoEvents();
                using (var anh = new Bitmap(form.Width, form.Height))
                {
                    form.DrawToBitmap(anh, new Rectangle(0, 0, anh.Width, anh.Height));
                    anh.Save(Path.Combine(ThuMucBaoCao(), "Bai11_10_XuLyLoi.png"));
                }
            }
        }

        /// <summary>Di nguoc tu bin\Debug len thu muc project.</summary>
        private static string ThuMucBaoCao()
        {
            string thuMuc = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "BaoCao"));
            Directory.CreateDirectory(thuMuc);
            return thuMuc;
        }
    }
}
