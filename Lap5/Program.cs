using System;
using System.Threading;
using System.Windows.Forms;

namespace Lab05_MultiForm
{
    static class Program
    {
        /// <summary>
        /// Entry point. Ứng dụng chạy bình thường ở chế độ tương tác;
        /// nếu có tham số "--screenshots" thì chuyển sang chế độ tự chụp ảnh báo cáo
        /// (xem BaoCaoTools/TrinhChupAnh.cs).
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args != null && args.Length > 0 && args[0] == "--screenshots")
            {
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                // Chạy bộ chụp ảnh trên một luồng STA riêng để có message loop
                // cho các hộp thoại/Form hiển thị trong lúc chụp.
                Thread luongChup = new Thread((ThreadStart)delegate { ChayCheDoChupAnh(args.Length > 1 ? args[1] : null); });
                luongChup.SetApartmentState(ApartmentState.STA);
                luongChup.Start();
                luongChup.Join();
                return;
            }

            Application.Run(new Forms.frmMain());
        }

        private static void ChayCheDoChupAnh(string kichBan)
        {
            Application.Run(new FormChupAnh(kichBan));
        }

        /// <summary>
        /// Gọi từ BaoCaoTools.TrinhChupAnh khi đã chụp xong: đóng message loop
        /// để tiến trình kết thúc thay vì treo chờ thao tác người dùng.
        /// </summary>
        internal static void YeuCauThoatSauKhiChup()
        {
            Application.ExitThread();
        }

        /// <summary>Form ẩn chỉ để tạo message loop cho luồng chụp ảnh.</summary>
        internal sealed class FormChupAnh : Form
        {
            private readonly string _kichBan;

            public FormChupAnh(string kichBan)
            {
                _kichBan = kichBan;
                ShowInTaskbar = false;
                WindowState = FormWindowState.Minimized;
                Opacity = 0;
                Size = new System.Drawing.Size(1, 1);
            }

            protected override void OnLoad(EventArgs e)
            {
                base.OnLoad(e);
                BeginInvoke((MethodInvoker)Chay);
            }

            private void Chay()
            {
                try
                {
                    if (string.IsNullOrEmpty(_kichBan)) BaoCaoTools.TrinhChupAnh.Chay();
                    else BaoCaoTools.TrinhChupAnh.ChayMotKichBan(_kichBan);
                }
                catch (Exception ex)
                {
                    BaoCaoTools.TrinhChupAnh.GhiLoiNghiemTrong("FormChupAnh", ex);
                }
                finally
                {
                    Close();
                }
            }

            protected override void OnFormClosed(FormClosedEventArgs e)
            {
                base.OnFormClosed(e);
                Application.ExitThread();
            }
        }
    }
}
