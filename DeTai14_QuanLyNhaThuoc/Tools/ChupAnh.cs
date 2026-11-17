using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using QuanLyNhaThuoc.BUS;
using QuanLyNhaThuoc.GUI;
using QuanLyNhaThuoc.GUI.Helpers;

namespace QuanLyNhaThuoc.Tools
{
    /// <summary>
    /// Chup cac man hinh cho bao cao bang DrawToBitmap (khong phu thuoc man hinh that,
    /// cua so dat ngoai vung nhin). Khong ghi du lieu: chi gio hang trong bo nho.
    /// </summary>
    public static class ChupAnh
    {
        public static void Chay()
        {
            Ui.ChayTuDong = true;
            string thuMuc = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "BaoCao", "HinhAnh"));
            Directory.CreateDirectory(thuMuc);

            Chup(new frmDangNhap(), thuMuc, "MH01_DangNhap.png", null);
            new NhanVienBUS().DangNhap("admin", "admin123");

            Chup(new frmMain(), thuMuc, "MH02_ManHinhChinh.png", null);
            Chup(new frmBanHang(), thuMuc, "MH03_BanHang.png", f =>
            {
                var bh = (frmBanHang)f;
                bh.ThemGio("T002", 10);
                bh.ThemGio("T009", 20);
                bh.ThemGio("T017", 1);
            });
            Chup(new frmThuoc(), thuMuc, "MH04_DanhMucThuoc.png", null);
            Chup(new frmNhapHang(), thuMuc, "MH05_NhapHang.png", f =>
            {
                ((frmNhapHang)f).ThemDong("T008", 100, 11000);
                ((frmNhapHang)f).ThemDong("T018", 200, 1000);
            });
            Chup(new frmHoaDon(), thuMuc, "MH06_HoaDon.png", null);
            Chup(new frmThongKe(), thuMuc, "MH07_ThongKe.png", null);
            Chup(new frmKhachHang(), thuMuc, "MH08_KhachHang.png", null);
            Chup(new frmDanhMuc(), thuMuc, "MH09_DanhMuc.png", null);
            Chup(new frmNhanVien(), thuMuc, "MH10_NhanVien.png", null);
            Chup(new frmNhatKy(), thuMuc, "MH12_NhatKy.png", null);

            // Ban in bao cao (cung ham ve voi PrintDocument), kho A4 o 100 dpi.
            var bh = new BanHangBUS();
            System.Data.DataRow hd = bh.TimHoaDon(DateTime.Today.AddDays(-30), DateTime.Today).Rows[0];
            InA4(new BaoCaoHoaDon(hd, bh.ChiTietHoaDon((int)hd["MaHD"])), thuMuc, "MH13_InHoaDon.png");
            var tk = new ThongKeBUS();
            InA4(new BaoCaoDoanhThu(DateTime.Today.AddDays(-13), DateTime.Today, tk.DoanhThuTheoNgay(DateTime.Today.AddDays(-13), DateTime.Today)),
                thuMuc, "MH14_InBaoCaoDoanhThu.png");

            new NhanVienBUS().DangNhap("duocsi", "123456");
            Chup(new frmMain(), thuMuc, "MH11_ManHinhChinh_DuocSi.png", null);
        }

        // ------------------------------------------------------------------
        // Chup thuc te tren man hinh (bat duoc ca MessageBox)
        // ------------------------------------------------------------------

        [DllImport("user32.dll")] private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")] private static extern bool PostMessage(IntPtr h, uint msg, IntPtr w, IntPtr l);
        [DllImport("user32.dll")] private static extern bool GetWindowRect(IntPtr h, out RECT r);
        [DllImport("user32.dll")] private static extern bool MoveWindow(IntPtr h, int x, int y, int w, int ht, bool repaint);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT { public int Left, Top, Right, Bottom; }

        /// <summary>
        /// Chay ung dung that: hien Form tren man hinh, thuc hien thao tac (thao tac
        /// co the bat MessageBox), chup vung Form tu man hinh, roi dong hop thoai.
        /// </summary>
        public static void ChayThucTe()
        {
            Ui.TuXacNhan = true;
            string thuMuc = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "BaoCao", "HinhAnh"));

            ChupManHinh(new frmDangNhap(), thuMuc, "TH01_DangNhapSai.png", f =>
            {
                f.Controls.Find("TenDangNhap", true)[0].Text = "duocsi";
                f.Controls.Find("MatKhau", true)[0].Text = "sai-mat-khau";
                ((Button)f.Controls.Find("btnDangNhap", true)[0]).PerformClick();
            });

            new NhanVienBUS().DangNhap("duocsi", "123456");
            ChupManHinh(new frmBanHang(), thuMuc, "TH02_VuotTonKho.png", f => ((frmBanHang)f).ThemGio("T008", 500));
            ChupManHinh(new frmBanHang(), thuMuc, "TH03_ThieuSoDonThuoc.png", f =>
            {
                var bh = (frmBanHang)f;
                bh.ThemGio("T005", 10);
                bh.ThemGio("T001", 20);
                bh.ThanhToan();
            });
            ChupManHinh(new frmBanHang(), thuMuc, "TH04_ThanhToanThanhCong.png", f =>
            {
                var bh = (frmBanHang)f;
                bh.ThemGio("T002", 10);
                bh.ThemGio("T009", 30);
                f.Controls.Find("DienThoai", true)[0].Text = "0912345678";
                bh.GanKhach();
                bh.ThanhToan();
            });
            ChupManHinh(new frmHoaDon(), thuMuc, "TH05_HoaDonVuaLap.png", null);
            new NhanVienBUS().DangNhap("admin", "admin123");
            ChupManHinh(new frmMain(), thuMuc, "TH06_ManHinhChinhSauBan.png", null);
        }

        private static void ChupManHinh(Form trang, string thuMuc, string tep, Action<Form> thaoTac)
        {
            Form f = TrongKhung(trang);
            using (f)
            {
                f.StartPosition = FormStartPosition.Manual;
                f.Location = new Point(0, 0);
                // Vua man hinh hien tai (ke ca man hinh xoay doc).
                Rectangle manHinh = Screen.PrimaryScreen.WorkingArea;
                f.MinimumSize = Size.Empty;
                f.Size = new Size(Math.Min(f.Width, manHinh.Width), Math.Min(f.Height, manHinh.Height));
                f.TopMost = true;
                f.Show();
                f.Activate();
                Application.DoEvents();
                Thread.Sleep(400);
                Application.DoEvents();

                // Chi chup vung trong cua so: vien trong suot cua Windows de lo man hinh nen phia sau.
                Rectangle vung = f.RectangleToScreen(f.ClientRectangle);
                IntPtr cuaSoForm = f.Handle;
                // Luong phu: cho hop thoai hien len, chup man hinh, dong hop thoai.
                var luong = new Thread(() =>
                {
                    Thread.Sleep(1200);
                    // Hop thoai MessageBox canh giua man hinh: dua no vao giua Form roi moi chup.
                    IntPtr hop = GetForegroundWindow();
                    RECT r;
                    if (hop != cuaSoForm && GetWindowRect(hop, out r))
                    {
                        int w = r.Right - r.Left, h = r.Bottom - r.Top;
                        MoveWindow(hop, vung.Left + (vung.Width - w) / 2, vung.Top + (vung.Height - h) / 2, w, h, true);
                        Thread.Sleep(500);
                    }
                    using (var anh = new Bitmap(vung.Width, vung.Height))
                    {
                        using (Graphics g = Graphics.FromImage(anh)) g.CopyFromScreen(vung.Location, Point.Empty, vung.Size);
                        anh.Save(Path.Combine(thuMuc, tep));
                    }
                    if (hop != cuaSoForm) PostMessage(hop, 0x0010, IntPtr.Zero, IntPtr.Zero);   // WM_CLOSE
                });
                luong.Start();
                if (thaoTac != null) f.BeginInvoke(new Action(() => thaoTac(trang)));
                while (luong.IsAlive) { Application.DoEvents(); Thread.Sleep(30); }
                f.Close();
            }
        }

        /// <summary>Boc trang vao cua so chinh de anh co sidebar + header nhu khi dung that.</summary>
        private static void InA4(QuanLyNhaThuoc.DTO.IBaoCao bc, string thuMuc, string tep)
        {
            using (var anh = new Bitmap(827, 1169))
            using (Graphics g = Graphics.FromImage(anh))
            {
                g.Clear(Color.White);
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                InBaoCao.VeTrang(g, new Rectangle(60, 60, 707, 1049), bc, 0);
                g.DrawRectangle(Pens.Silver, 0, 0, 826, 1168);
                anh.Save(Path.Combine(thuMuc, tep));
            }
        }

        private static Form TrongKhung(Form trang)
        {
            if (!(trang is ManHinh) || trang is frmTongQuan) return trang;
            var chinh = new frmMain();
            chinh.Load += (s, e) => chinh.Mo(trang);
            return chinh;
        }

        private static void Chup(Form trang, string thuMuc, string tep, Action<Form> kichBan)
        {
            Form f = TrongKhung(trang);
            using (f)
            {
                f.StartPosition = FormStartPosition.Manual;
                f.Location = new Point(-4000, 0);
                f.ShowInTaskbar = false;
                f.Show();
                Application.DoEvents();
                if (kichBan != null) kichBan(trang);
                Application.DoEvents();
                using (var anh = new Bitmap(f.Width, f.Height))
                {
                    f.DrawToBitmap(anh, new Rectangle(0, 0, f.Width, f.Height));
                    anh.Save(Path.Combine(thuMuc, tep));
                }
                f.Close();
            }
        }
    }
}
