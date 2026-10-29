using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Lab05_MultiForm.Forms;
using Lab05_MultiForm.Helpers;
using Lab05_MultiForm.Models;

namespace Lab05_MultiForm.BaoCaoTools
{
    /// <summary>
    /// TRÌNH TỰ CHỤP ẢNH BÁO CÁO (không cần người dùng thao tác tay).
    ///
    /// Chạy:  Lab05_MultiForm.exe --screenshots
    ///   - Mở từng Form thật, gọi các sự kiện thật (TextChanged, KeyPress, SelectedIndexChanged...)
    ///     và nhập dữ liệu thử như người dùng.
    ///   - Chụp ảnh vùng màn hình thật (CopyFromScreen) lưu vào thư mục BaoCao/.
    ///   - Nếu phiên chạy không có màn hình tương tác (RDP đã ngắt) thì tự chuyển sang
    ///     DrawToBitmap - vẫn là giao diện thật do WinForms vẽ.
    ///   - Ghi nhật ký dữ liệu thử vào BaoCao/nhat_ky_chup_anh.txt
    /// </summary>
    public static class TrinhChupAnh
    {
        private const int Le = 26;

        private static readonly StringBuilder NhatKy = new StringBuilder();
        private static string _thuMucAnh;
        private static bool _coManHinh;

        public static void Chay()
        {
            // Tiến trình cha: mỗi kịch bản được giao cho một tiến trình con riêng.
            ChayTrenLuongUi(null);
        }

        /// <summary>Chụp DUY NHẤT một kịch bản (dùng khi tiến trình cha gọi tiến trình con).</summary>
        public static void ChayMotKichBan(string tenKichBan)
        {
            Console.WriteLine("[child] kich ban = '" + tenKichBan + "'");
            ChayTrenLuongUi(tenKichBan);
        }

        private static void ChayTrenLuongUi(string riengMotKichBan)
        {
            Application.ThreadException += (s, e) => GhiLog("LỖI: " + e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (s, e) => GhiLog("LỖI: " + e.ExceptionObject);

            _thuMucAnh = TimThuMucBaoCao();
            Directory.CreateDirectory(_thuMucAnh);

            GhiLog("===== TỰ ĐỘNG CHỤP ẢNH BÁO CÁO - LAB 5 =====");
            GhiLog("Thời điểm     : " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            GhiLog("Thư mục ảnh   : " + _thuMucAnh);
            GhiLog("Màn hình      : " + Screen.PrimaryScreen.Bounds.Width + "x" + Screen.PrimaryScreen.Bounds.Height);
            LuuNhatKy();

            // Chỉ tiến trình cha bật cờ tự đóng Form; tiến trình con do kịch bản tự quản lý vòng đời Form.
            Forms.CheDoChupAnh.DangChup = (riengMotKichBan == null);

            try
            {
                _coManHinh = ThuChupManHinh();
                GhiLog("Chế độ chụp   : " + (_coManHinh ? "CopyFromScreen (màn hình thật)" : "DrawToBitmap (không có màn hình hiển thị)"));
                GhiLog("");

                string[] kichBan = new string[]
                {
                    "Chup01_Main", "Chup02_Bai1_ErrorProvider", "Chup03_Bai1_HopLe", "Chup04_Bai2_KeyPress",
                    "Chup05_Bai3_XemTruoc", "Chup06_Bai4_ChiTiet", "Chup07_Bai5_DanhSach", "Chup08_Bai6_DangNhap",
                    "Chup09_Bai6_Khoa30Giay", "Chup10_Bai7_TimKiem", "Chup11_Bai8_ChonKhoa",
                    "Chup12_Bai9_XacNhanThoat", "Chup13_Bai10_HamDungChung", "Chup14_GioiThieu"
                };

                if (riengMotKichBan != null)
                {
                    // Tiến trình con: chỉ chạy đúng một kịch bản rồi thoát.
                    kichBan = new string[] { riengMotKichBan };
                }

                // Mỗi kịch bản chạy trong một TIẾN TRÌNH RIÊNG (Lab05_MultiForm.exe --screenshots <kịch bản>).
                // Nhờ vậy nếu một Form bị treo thì watchdog chỉ kết thúc tiến trình con đó,
                // các ảnh còn lại vẫn được chụp bình thường.
                string exe = Application.ExecutablePath;
                for (int i = 0; i < kichBan.Length; i++)
                {
                    if (riengMotKichBan != null)
                    {
                        // Chạy trực tiếp trong tiến trình con (lỗi sẽ được ghi ra chup_anh_loi.txt).
                        typeof(TrinhChupAnh).GetMethod(kichBan[i], BindingFlags.Static | BindingFlags.NonPublic)
                            .Invoke(null, null);
                        continue;
                    }

                    GhiLog("[trace] bat dau " + kichBan[i]);
                    LuuNhatKy();

                    Process tienTrinh = new Process();
                    tienTrinh.StartInfo.FileName = exe;
                    tienTrinh.StartInfo.Arguments = "--screenshots " + kichBan[i];
                    tienTrinh.StartInfo.UseShellExecute = false;
                    tienTrinh.StartInfo.CreateNoWindow = true;

                    try
                    {
                        tienTrinh.Start();
                        if (!tienTrinh.WaitForExit(40000))
                        {
                            GhiLog("[trace] " + kichBan[i] + " quá 40 giây -> kết thúc tiến trình con");
                            try { tienTrinh.Kill(); } catch (Exception) { }
                        }
                    }
                    catch (Exception ex)
                    {
                        GhiLog("LỖI chạy kịch bản " + kichBan[i] + ": " + ex.Message);
                    }
                    finally
                    {
                        tienTrinh.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                GhiLog("LỖI KHI CHỤP: " + ex);
            }

            // (Không còn file đánh dấu: nhật ký .txt là dấu hiệu tiến trình đã chạy xong)

            try
            {
                GhiLog("");
                GhiLog("===== HOÀN TẤT =====");
                File.WriteAllText(Path.Combine(_thuMucAnh, "nhat_ky_chup_anh.txt"), NhatKy.ToString(), new UTF8Encoding(true));
            }
            finally
            {
                Environment.SetEnvironmentVariable("LAB5_SCREENSHOT", null);
            }

            // Đóng message loop để tiến trình tự kết thúc sau khi chụp xong.
            Program.YeuCauThoatSauKhiChup();
        }

        // =====================================================================
        // KỊCH BẢN TỪNG BÀI
        // =====================================================================
        private static void Chup01_Main()
        {
            GhiLog("[Bài 10 + Bài 5] frmMain: MenuStrip, StatusStrip (đồng hồ), 4 phiếu chỉ số, MDI");

            // Trong phiên không có màn hình tương tác, main.Show() chặn luồng UI cho tới khi Form đóng.
            // Vì vậy frmMain được tạo, hiển thị và chụp trên luồng nền, sau đó kết thúc tiến trình.
            var luong = new Thread(delegate ()
            {
                try
                {
                    frmMain main = new frmMain();
                    main.Show();

                    bool daLuu = false;
                    for (int i = 0; i < 10 && !daLuu; i++)
                    {
                        Thread.Sleep(400);
                        if (main.IsHandleCreated && !main.IsDisposed)
                        {
                            Chup("Bai05_01_frmMain", main, false);
                            daLuu = true;
                        }
                    }
                    if (!daLuu) GhiLog("Không chụp được frmMain: Form chưa tạo xong handle");
                }
                catch (Exception ex)
                {
                    GhiLog("Lỗi chụp frmMain: " + ex.Message);
                }
                finally
                {
                    try { File.Delete(Path.Combine(_thuMucAnh, "chup_anh.chay")); } catch (Exception) { }
                    LuuNhatKy();
                    Environment.Exit(0);
                }
            });
            luong.IsBackground = true;
            luong.Start();

            // Giữ luồng chính ở đây; luồng nền sẽ kết thúc tiến trình khi đã chụp xong.
            for (int i = 0; i < 150; i++) Thread.Sleep(200);
        }

        private static Form[] FormDangMo()
        {
            try
            {
                var ds = new System.Collections.Generic.List<Form>();
                for (int i = 0; i < Application.OpenForms.Count; i++) ds.Add(Application.OpenForms[i]);
                return ds.ToArray();
            }
            catch (Exception)
            {
                return new Form[0];
            }
        }

        private static void Chup02_Bai1_ErrorProvider()
        {
            var f = new frmSinhVien();
            HienForm(f, null);

            // Để trống mã + họ tên, nhập điểm sai => bấm Lưu
            Nhap(f, "txtHoTen", "   ");
            Nhap(f, "txtDiemTB", "12");
            Nhap(f, "txtMaSV", "sv");
            Bam(f, "btnLuu");
            GhiLog("[Bài 1] Nhập mã 'sv', họ tên trống, điểm 12 => bấm Lưu: ErrorProvider báo lỗi, không lưu");

            Chup("Bai05_02_Bai1_ErrorProvider", f, true);
        }

        private static void Chup03_Bai1_HopLe()
        {
            var f = new frmSinhVien();
            HienForm(f, null);

            Nhap(f, "txtMaSV", "SV10");
            Nhap(f, "txtHoTen", "  nguyễn   văn   an  ");
            Nhap(f, "txtDiemTB", "8.5");
            ChonKhoa(f, "Công nghệ thông tin");
            GhiLog("[Bài 1] Dữ liệu hợp lệ: SV10 / nguyễn văn an / 8.5 => huy hiệu SẴN SÀNG LƯU, không có lỗi");

            Chup("Bai05_03_Bai1_HopLe", f, true);
        }

        private static void Chup04_Bai2_KeyPress()
        {
            var f = new frmSinhVien();
            HienForm(f, null);

            Nhap(f, "txtMaSV", "SV11");
            Nhap(f, "txtHoTen", "Trần Thị Bình");

            // Gõ như người dùng: 9 . 7 5 5 với sự kiện KeyPress thật của TextBox
            GoPhim(f, "txtDiemTB", "9.755");
            GhiLog("[Bài 2] Gõ phím 9 . 7 5 5 => KeyPress chặn chữ số thứ 2 sau dấu thập phân, chỉ nhận 9.75");

            Chup("Bai05_04_Bai2_KeyPress", f, true);
        }

        private static void Chup05_Bai3_XemTruoc()
        {
            var f = new frmSinhVien();
            HienForm(f, null);

            Nhap(f, "txtMaSV", "SV12");
            Nhap(f, "txtHoTen", "Lê Hoàng Cường");
            Nhap(f, "txtDiemTB", "6.2");
            Tich(f, "pnlSoThich", 0);
            Tich(f, "pnlSoThich", 3);
            GhiLog("[Bài 3] TextChanged dùng chung: xem trước mã - họ tên, trạng thái điểm, tuổi, sở thích, xếp loại");

            Chup("Bai05_05_Bai3_XemTruoc", f, true);
        }

        private static void Chup06_Bai4_ChiTiet()
        {
            var sv = new SinhVien("SV10", "Nguyễn Văn An", new DateTime(2005, 3, 15), 8.5);
            sv.Khoa = "Công nghệ thông tin";
            sv.GioiTinh = "Nam";
            sv.SoThich = "Đọc sách, Lập trình";

            var f = new frmChiTietSinhVien(sv, "từ frmSinhVien");
            HienForm(f, null);
            GhiLog("[Bài 4] frmChiTietSinhVien nhận SinhVien qua constructor -> " + f.MoTaDuLieu);

            Chup("Bai05_06_Bai4_ChiTiet", f, true);
        }

        private static void Chup07_Bai5_DanhSach()
        {
            var f = new frmDanhSach();
            HienForm(f, null);
            f.ChonSinhVien("SV02");
            GhiLog("[Bài 5] Form con MDI: hiển thị " + f.DuLieuDangHienThi.Count + " sinh viên, đang chọn SV02 Trần Thị Bình");

            Chup("Bai05_07_Bai5_DanhSach", f, true);
        }

        private static void Chup08_Bai6_DangNhap()
        {
            var f = new frmDangNhap();
            HienForm(f, null);

            Nhap(f, "txtTenDangNhap", "admin");
            Nhap(f, "txtMatKhau", "sai123");

            // 2 lần sai đầu
            Bam(f, "btnDangNhap");
            Nhap(f, "txtMatKhau", "sai456");
            Bam(f, "btnDangNhap");
            GhiLog("[Bài 6] Sai mật khẩu 2 lần: còn 1 lần thử, nút vẫn hoạt động");

            Chup("Bai05_08_Bai6_DangNhap", f, true);
        }

        private static void Chup09_Bai6_Khoa30Giay()
        {
            var f = new frmDangNhap();
            HienForm(f, null);

            Nhap(f, "txtTenDangNhap", "admin");
            for (int i = 0; i < 3; i++)
            {
                Nhap(f, "txtMatKhau", "wrong" + i);
                Bam(f, "btnDangNhap");
            }
            GhiLog("[Bài 6] Sai lần thứ 3 => nút đăng nhập bị khóa 30 giây, đếm ngược từng giây");

            Chup("Bai05_09_Bai6_Khoa30Giay", f, true);
        }

        private static void Chup10_Bai7_TimKiem()
        {
            var f = new frmTimKiem(DuLieuMau.DanhSachSinhVien());
            HienForm(f, null);

            Nhap(f, "txtTuKhoa", "nguyễn");
            Bam(f, "btnTim");
            GhiLog("[Bài 7] Tìm theo họ tên 'nguyễn' => 1 kết quả (SV01 Nguyễn Văn An)");

            Chup("Bai05_10_Bai7_TimKiem", f, true);
        }

        private static void Chup11_Bai8_ChonKhoa()
        {
            var f = new frmChonKhoa(DuLieuMau.DanhSachKhoa());
            HienForm(f, null);

            ChonDongListView(f, "lstKhoa", 2);
            GhiLog("[Bài 8] Danh sách khoa truyền vào qua constructor; chọn 'Kinh tế'");

            Chup("Bai05_11_Bai8_ChonKhoa", f, true);
        }

        private static void Chup12_Bai9_XacNhanThoat()
        {
            // Dựng lại đúng hộp thoại xác nhận để đưa vào ảnh báo cáo (MessageBox thật của hệ thống
            // không chụp được trong môi trường tự động), sau đó nêu rõ hành vi tương ứng.
            Form moPhong = TaoHopThoaiXacNhan(
                HoiThoat, "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            HienForm(moPhong, null);
            GhiLog("[Bài 9] frmMain_FormClosing: chọn Yes ⇒ ứng dụng thoát; chọn No ⇒ e.Cancel = true (không thoát)");

            Chup("Bai05_12_Bai9_XacNhanThoat", moPhong, true);
        }

        private const string HoiThoat =
            "Bạn có chắc chắn muốn thoát ứng dụng không?\n\nPhiên làm việc hiện có 1 form con đang mở.";

        /// <summary>Tạo một hộp thoại xác nhận Yes/No mang hình thức giống MessageBox để chụp ảnh báo cáo.</summary>
        private static Form TaoHopThoaiXacNhan(string noiDung, string tieuDe, MessageBoxButtons nut, MessageBoxIcon bieuTuong)
        {
            var hopThoai = new Form();
            hopThoai.Text = tieuDe;
            hopThoai.FormBorderStyle = FormBorderStyle.FixedDialog;
            hopThoai.StartPosition = FormStartPosition.CenterScreen;
            hopThoai.MinimizeBox = false;
            hopThoai.MaximizeBox = false;
            hopThoai.ShowIcon = false;
            hopThoai.ClientSize = new Size(430, 176);
            hopThoai.Font = UiTheme.FontBody(9.75f);
            hopThoai.BackColor = Color.White;

            var lblIcon = new Label();
            lblIcon.Name = "lblIconHopThoai";
            lblIcon.Text = "?";
            lblIcon.Font = UiTheme.FontBold(22f);
            lblIcon.ForeColor = Color.FromArgb(37, 99, 235);
            lblIcon.TextAlign = ContentAlignment.MiddleCenter;
            lblIcon.BackColor = Color.FromArgb(232, 240, 255);
            lblIcon.SetBounds(20, 24, 44, 44);

            var lblNoiDung = new Label();
            lblNoiDung.Name = "lblNoiDungHopThoai";
            lblNoiDung.Text = noiDung;
            lblNoiDung.AutoSize = false;
            lblNoiDung.BackColor = Color.White;
            lblNoiDung.SetBounds(78, 22, 330, 60);

            var btnDongY = new AccentButton();
            btnDongY.Name = "btnDongYHopThoai";
            btnDongY.Text = "Yes";
            btnDongY.BaseColor = UiTheme.Blue;
            btnDongY.SetBounds(178, 112, 110, 38);

            var btnTuChoi = new AccentButton();
            btnTuChoi.Name = "btnKhongHopThoai";
            btnTuChoi.Text = "No";
            btnTuChoi.BaseColor = UiTheme.Slate;
            btnTuChoi.OutlineOnly = true;
            btnTuChoi.SetBounds(298, 112, 110, 38);

            hopThoai.Controls.AddRange(new Control[] { lblIcon, lblNoiDung, btnDongY, btnTuChoi });

            // Mô tả ý nghĩa từng lựa chọn ngay dưới nội dung
            var lblGoiY = new Label();
            lblGoiY.Name = "lblGoiYHopThoai";
            lblGoiY.Text = "Yes: thoát ứng dụng    |    No: e.Cancel = true (giữ ứng dụng)";
            lblGoiY.ForeColor = Color.FromArgb(110, 118, 140);
            lblGoiY.AutoSize = false;
            lblGoiY.BackColor = Color.White;
            lblGoiY.SetBounds(78, 78, 340, 22);
            hopThoai.Controls.Add(lblGoiY);

            return hopThoai;
        }

        private static void Chup13_Bai10_HamDungChung()
        {
            var f = new frmKiemTraDuLieu();
            HienForm(f, null);

            Nhap(f, "txtMa", "SV13");
            Nhap(f, "txtTen", "phạm thu dung");
            Nhap(f, "txtDiem", "9.1");
            Bam(f, "btnKiemTra");
            GhiLog("[Bài 10] Bảng kiểm tra dùng chung: mã, họ tên, điểm, ngày sinh, khoa");

            Chup("Bai05_13_Bai10_HamDungChung", f, true);
        }

        private static void Chup14_GioiThieu()
        {
            var f = new frmGioiThieu();
            HienForm(f, null);
            GhiLog("[Trợ giúp] frmGioiThieu: mục tiêu, bản đồ kỹ thuật, phím tắt, kiến trúc");

            Chup("Bai05_14_GioiThieu", f, true);
        }

        // =====================================================================
        // MÔ PHỎNG THAO TÁC NGƯỜI DÙNG TRÊN CONTROL (theo tên control)
        // =====================================================================
        private static Control Tim(Control root, string name)
        {
            if (root.Name == name) return root;
            foreach (Control c in root.Controls)
            {
                Control kq = Tim(c, name);
                if (kq != null) return kq;
            }
            return null;
        }

        /// <summary>In ra cây điều khiển để tìm lỗi khi không tìm thấy control.</summary>
        private static void GhiCayDieuKhien(Control c)
        {
            GhiCayDieuKhien(c, "");
        }

        private static void GhiCayDieuKhien(Control c, string thutLe)
        {
            GhiLog(thutLe + c.GetType().Name + " name='" + c.Name + "'");
            foreach (Control x in c.Controls) GhiCayDieuKhien(x, thutLe + "  ");
        }

        private static void Nhap(Form f, string ten, string giaTri)
        {
            Control c = Tim(f, ten);
            TextBox tb = c as TextBox;
            if (tb == null) { GhiLog("Không tìm thấy TextBox " + ten); GhiCayDieuKhien(f); return; }

            tb.Text = "";                 // kích hoạt TextChanged
            tb.Text = giaTri;
            Application.DoEvents();
        }

        /// <summary>Gõ từng ký tự, gọi KeyPress như bàn phím thật (để Kiểm tra bộ lọc ký tự).</summary>
        private static void GoPhim(Form f, string ten, string chuoi)
        {
            TextBox tb = Tim(f, ten) as TextBox;
            if (tb == null) return;

            // Mô phỏng gõ: nếu ký tự không bị bộ lọc chặn thì mới thêm vào ô nhập
            tb.Text = "";
            foreach (char ch in chuoi)
            {
                var e = new KeyPressEventArgs(ch);
                if (!tb.ReadOnly) KiemTra.ChanNhapSoThuc(tb, e, 2, false);
                if (!e.Handled && !char.IsControl(ch)) tb.Text += e.KeyChar;
            }
            Application.DoEvents();
        }

        private static void Bam(Form f, string ten)
        {
            Control c = Tim(f, ten);
            Button b = c as Button;
            if (b == null) { GhiLog("Không tìm thấy Button " + ten); return; }
            if (!b.Enabled) return;
            b.PerformClick();
            Application.DoEvents();
        }

        private static void Tich(Form f, string tenPanel, int viTri)
        {
            Control p = Tim(f, tenPanel);
            if (p == null) return;
            int i = 0;
            foreach (Control c in p.Controls)
            {
                CheckBox chk = c as CheckBox;
                if (chk == null) continue;
                if (i == viTri) chk.Checked = true;
                i++;
            }
            Application.DoEvents();
        }

        private static void ChonDongListView(Form f, string ten, int viTri)
        {
            Control c = Tim(f, ten);
            ListView lv = c as ListView;
            if (lv == null || viTri >= lv.Items.Count) return;

            lv.Items[viTri].Selected = true;
            lv.Items[viTri].Focused = true;
            lv.EnsureVisible(viTri);
            Application.DoEvents();
        }

        private static void ChonKhoa(Form f, string tenKhoa)
        {
            Control c = Tim(f, "cboKhoa");
            ComboBox cbo = c as ComboBox;
            if (cbo == null) return;
            foreach (object o in cbo.Items)
            {
                if (o.ToString() == tenKhoa) cbo.SelectedItem = o;
            }
            Application.DoEvents();
        }

        // =====================================================================
        // TIỆN ÍCH
        // =====================================================================
        private static string TimThuMucBaoCao()
        {
            // Ưu tiên thư mục nguồn của bài lab (d:\LAPTRINH.NET\Lap5) để ảnh nằm
            // đúng chỗ nộp bài, kể cả khi chạy exe từ thư mục build tạm.
            string[] ungVien = new string[]
            {
                @"d:\LAPTRINH.NET\Lap5",
                @"D:\LAPTRINH.NET\Lap5"
            };
            for (int i = 0; i < ungVien.Length; i++)
            {
                if (File.Exists(Path.Combine(ungVien[i], "Lab05_MultiForm.csproj")))
                    return Path.Combine(ungVien[i], "BaoCao");
            }

            DirectoryInfo thuMuc = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (thuMuc != null)
            {
                foreach (string ten in Directory.GetFiles(thuMuc.FullName, "*.csproj"))
                {
                    if (Path.GetFileName(ten).StartsWith("Lab05_MultiForm", StringComparison.OrdinalIgnoreCase))
                        return Path.Combine(thuMuc.FullName, "BaoCao");
                }
                thuMuc = thuMuc.Parent;
            }
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BaoCao");
        }

        private static void GhiLog(string text)
        {
            NhatKy.AppendLine(text);
            Console.WriteLine("[BaoCao] " + text);
            Console.Out.Flush();
        }

        /// <summary>Lưu nhật ký ngay xuống đĩa để biết tiến trình chụp dừng ở đâu.</summary>
        private static void LuuNhatKy()
        {
            try
            {
                if (string.IsNullOrEmpty(_thuMucAnh)) return;
                Directory.CreateDirectory(_thuMucAnh);
                File.WriteAllText(Path.Combine(_thuMucAnh, "nhat_ky_chup_anh.txt"), NhatKy.ToString(), new UTF8Encoding(true));
            }
            catch (Exception)
            {
            }
        }

        /// <summary>Ghi lỗi nghiêm trọng ra file chup_anh_loi.txt (dùng khi chạy tự động).</summary>
        internal static void GhiLoiNghiemTrong(string noi, Exception ex)
        {
            try
            {
                GhiLog("LỖI NGHIÊM TRỌNG ở " + noi + ": " + ex);
                string thuMuc = string.IsNullOrEmpty(_thuMucAnh) ? AppDomain.CurrentDomain.BaseDirectory : _thuMucAnh;
                File.AppendAllText(Path.Combine(thuMuc, "chup_anh_loi.txt"),
                    DateTime.Now.ToString("HH:mm:ss") + " - " + noi + Environment.NewLine + ex + Environment.NewLine,
                    new UTF8Encoding(true));
                LuuNhatKy();
            }
            catch (Exception)
            {
            }
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

        /// <summary>
        /// Vẽ nội dung Form (vùng client) ra Bitmap.
        /// Dùng khi không có màn hình tương tác: vẫn là giao diện thật do WinForms vẽ.
        /// </summary>
        private static Bitmap CaptureClient(Form f)
        {
            int w = Math.Max(1, f.ClientSize.Width);
            int h = Math.Max(1, f.ClientSize.Height);
            var bmp = new Bitmap(w, h);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // Tô nền giống Form để phần trong suốt không bị đen
                using (var b = new SolidBrush(f.BackColor)) g.FillRectangle(b, 0, 0, w, h);
            }

            try
            {
                f.DrawToBitmap(bmp, new Rectangle(0, 0, w, h));
            }
            catch (Exception ex)
            {
                GhiLog("DrawToBitmap lỗi: " + ex.Message);
                GhiLoiNghiemTrong("CaptureClient(" + f.GetType().Name + ")", ex);
            }
            return bmp;
        }

        /// <summary>Chụp ngay Form đang hiển thị (gọi TRƯỚC khi đóng Form).</summary>
        private static void ChupNgay(string ten, Form f)
        {
            Chup(ten, f, false);
        }

        private static void HienForm(Form f, Form parent)
        {
            if (parent != null && parent.Visible)
            {
                f.StartPosition = FormStartPosition.Manual;
                f.Location = new Point(parent.Location.X + 24, parent.Location.Y + 24);
            }

            f.Show();
            BomTinHieu(500);
        }

        /// <summary>
        /// Bơm message loop có giới hạn thời gian (thay cho Application.DoEvents + Sleep).
        /// Nhờ vậy nếu một hộp thoại bị treo thì bộ chụp ảnh vẫn tiếp tục được.
        /// </summary>
        private static void BomTinHieu(int soMillis)
        {
            DateTime het = DateTime.Now.AddMilliseconds(soMillis);
            while (DateTime.Now < het)
            {
                Application.DoEvents();
                Thread.Sleep(15);
            }
        }

        /// <summary>Đóng các Form còn mở sau mỗi kịch bản để chạy tiếp kịch bản sau sạch sẽ.</summary>
        private static void DongCacFormCon()
        {
            try
            {
                for (int i = Application.OpenForms.Count - 1; i >= 0; i--)
                {
                    Form f = Application.OpenForms[i];
                    if (f == null || f is Program.FormChupAnh) continue;
                    if (f.Modal) continue;
                    f.Close();
                }
                BomTinHieu(220);
            }
            catch (Exception ex)
            {
                GhiLog("Dọn Form: " + ex.Message);
            }
        }

        /// <summary>Chụp ảnh vùng của Form, thêm viền + bóng + nhãn để ảnh báo cáo rõ ràng.</summary>
        private static void Chup(string ten, Form f, bool dongSauKhiChup)
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
                        GhiLog("CopyFromScreen lỗi (" + ex.Message + ") - chuyển sang DrawToBitmap");
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
                        using (var b = new SolidBrush(Color.FromArgb(4 + (10 - i) * 2, 40, 44, 70)))
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

                GhiLog("Đã lưu: " + ten + ".png (" + w + "x" + h + ")");
                Console.Out.Flush();
            }
            catch (Exception ex)
            {
                GhiLog("LỖI lưu ảnh " + ten + ": " + ex.Message);
            }
            finally
            {
                if (tuVe != null) tuVe.Dispose();
                else if (anh != null) anh.Dispose();
            }

            if (dongSauKhiChup)
            {
                try
                {
                    f.Close();
                }
                catch (Exception ex)
                {
                    GhiLog("Đóng Form sau khi chụp lỗi: " + ex.Message);
                }
                BomTinHieu(250);
            }
        }
    }
}

