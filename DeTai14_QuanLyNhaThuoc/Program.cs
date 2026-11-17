using System;
using System.Windows.Forms;
using QuanLyNhaThuoc.BUS;
using QuanLyNhaThuoc.DTO;
using QuanLyNhaThuoc.GUI;
using QuanLyNhaThuoc.Tools;

namespace QuanLyNhaThuoc
{
    /// <summary>
    /// Diem vao:
    ///   (khong tham so) dang nhap -> man hinh chinh; dang xuat thi quay lai dang nhap.
    ///   --kiemtra       chay bo kiem thu nghiep vu, ghi BaoCao\kiem_thu.txt.
    ///   --chupanh       chup cac man hinh vao BaoCao\HinhAnh.
    /// </summary>
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length > 0 && args[0] == "--kiemtra") { Environment.ExitCode = KiemThu.Chay(); return; }
            if (args.Length > 0 && args[0] == "--chupanh") { ChupAnh.Chay(); return; }
            if (args.Length > 0 && args[0] == "--chupthucte") { ChupAnh.ChayThucTe(); return; }
            if (args.Length > 0 && args[0] == "--caidat") { DAL.CaiDatCsdl.TaoMoi(); return; }   // tao lai CSDL + du lieu mau

            // Loi khong bat duoc o dau ca cung di qua bo xu ly loi chung, khong dong ung dung dot ngot.
            Application.ThreadException += (s, e) =>
                MessageBox.Show(XuLyLoi.PhanLoai(e.Exception, "lỗi không bắt được").ThongBao, "Lỗi hệ thống",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

            if (!KiemTraCsdl()) return;

            while (true)
            {
                using (var dn = new frmDangNhap())
                {
                    if (dn.ShowDialog() != DialogResult.OK) return;
                }
                var main = new frmMain();
                Application.Run(main);
                new NhanVienBUS().DangXuat();
                if (!main.DangXuat) return;
            }
        }

        /// <summary>Lan chay dau tren may moi: CSDL chua co thi hoi va tu tao bang script trong thu muc SQL.</summary>
        private static bool KiemTraCsdl()
        {
            string loi;
            if (DAL.CaiDatCsdl.DaSanSang(out loi)) return true;

            if (loi != null)
            {
                MessageBox.Show("Không kết nối được SQL Server:\n" + loi +
                    "\n\nHãy cài SQL Server Express / LocalDB, hoặc sửa chuỗi kết nối \"QLNhaThuoc\" trong App.config.",
                    "Chưa kết nối được CSDL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (MessageBox.Show("Chưa có CSDL QLNhaThuoc trên máy này.\nTạo CSDL và dữ liệu mẫu ngay bây giờ?",
                    "Cài đặt lần đầu", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return false;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                DAL.CaiDatCsdl.TaoMoi();
                MessageBox.Show("Đã tạo CSDL. Đăng nhập: admin / admin123 hoặc duocsi / 123456.", "Cài đặt lần đầu",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Tạo CSDL thất bại:\n" + ex.Message, "Cài đặt lần đầu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
