using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Lab06_ThreeLayer.BUS;
using Lab06_ThreeLayer.DAL;
using Lab06_ThreeLayer.DTO;
using Lab06_ThreeLayer.GUI;
using Lab06_ThreeLayer.GUI.Helpers;

namespace Lab06_ThreeLayer.BaoCaoTools
{
    /// <summary>
    /// TRÌNH TỰ CHỤP ẢNH BÁO CÁO cho Lab 6 (không cần thao tác tay).
    ///
    /// Chạy: Lab06_ThreeLayer.exe --screenshots
    ///   - Mỗi kịch bản chạy trong một TIẾN TRÌNH RIÊNG (--screenshots &lt;kịch bản&gt;) + watchdog 40 giây.
    ///   - Ảnh chụp là giao diện thật: CopyFromScreen nếu có màn hình, ngược lại Form.DrawToBitmap.
    ///   - Kết quả lưu vào Lap6/BaoCao/*.png, nhật ký ở BaoCao/nhat_ky_chup_anh.txt.
    /// </summary>
    public static class TrinhChupAnh
    {
        private const int Le = 26;
        private const int NguongWatchdog = 40000;

        private static readonly StringBuilder NhatKy = new StringBuilder();
        private static string _thuMucAnh;
        private static bool _coManHinh;
        private static int _idAnh;

        /// <summary>Tiến trình cha: chạy lần lượt từng kịch bản trong tiến trình con.</summary>
        public static void Chay()
        {
            ChayTrenLuongUi(null);
        }

        /// <summary>Tiến trình con: chạy đúng một kịch bản.</summary>
        public static void ChayMotKichBan(string tenKichBan)
        {
            ChayTrenLuongUi(tenKichBan);
        }

        private static void ChayTrenLuongUi(string riengMotKichBan)
        {
            Application.ThreadException += delegate (object s, ThreadExceptionEventArgs e) { GhiLog("LỖI: " + e.Exception); };
            AppDomain.CurrentDomain.UnhandledException += delegate (object s, UnhandledExceptionEventArgs e) { GhiLog("LỖI: " + e.ExceptionObject); };

            _thuMucAnh = TimThuMucBaoCao();
            Directory.CreateDirectory(_thuMucAnh);

            if (riengMotKichBan == null)
            {
                NhatKy.Length = 0;
                GhiLog("===== TỰ ĐỘNG CHỤP ẢNH BÁO CÁO - LAB 6 =====");
                GhiLog("Thời điểm    : " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                GhiLog("Thư mục ảnh  : " + _thuMucAnh);
                _coManHinh = ThuChupManHinh();
                GhiLog("Chế độ chụp  : " + (_coManHinh ? "CopyFromScreen" : "DrawToBitmap (không có màn hình)"));
                GhiLog("");
            }

            string[] kichBan = new string[]
            {
                "Chup01_Main", "Chup02_DataGridView", "Chup03_KiemTraLoi", "Chup04_TimSapXep",
                "Chup05_TreeKhoa", "Chup06_SanPham", "Chup07_TreeDanhMuc", "Chup08_ListView",
                "Chup09_CSV", "Chup10_KienTruc", "Chup11_GioiThieu", "Chup12_XacNhanThoat"
            };

            if (riengMotKichBan != null) kichBan = new string[] { riengMotKichBan };

            string exe = Application.ExecutablePath;

            for (int i = 0; i < kichBan.Length; i++)
            {
                if (riengMotKichBan != null)
                {
                    try
                    {
                        MethodInfo m = typeof(TrinhChupAnh).GetMethod(kichBan[i], BindingFlags.Static | BindingFlags.NonPublic);
                        if (m == null) GhiLog("Không có kịch bản " + kichBan[i]);
                        else m.Invoke(null, null);
                    }
                    catch (Exception ex)
                    {
                        GhiLoiNghiemTrong(kichBan[i], ex);
                    }
                    continue;
                }

                using (Process p = new Process())
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
                            GhiLog("[watchdog] " + kichBan[i] + " quá 40 giây -> kết thúc tiến trình con");
                            try { p.Kill(); } catch (Exception) { }
                        }
                    }
                    catch (Exception ex)
                    {
                        GhiLog("LỖI chạy " + kichBan[i] + ": " + ex.Message);
                    }
                }
            }

            if (riengMotKichBan == null)
            {
                GhiLog("");
                GhiLog("===== HOÀN TẤT =====");
                LuuNhatKy();
                Program.YeuCauThoatSauKhiChup();
            }
        }

        // =====================================================================
        // KỊCH BẢN
        // =====================================================================
        private static void Chup01_Main()
        {
            // frmMain của Lab 6 dùng MDI + timer: tạo/hiện/chụp trên luồng nền rồi kết thúc tiến trình.
            Thread luong = new Thread(delegate ()
            {
                try
                {
                    frmMain main = new frmMain();
                    main.Show();

                    bool daLuu = false;
                    for (int i = 0; i < 12 && !daLuu; i++)
                    {
                        Thread.Sleep(350);
                        if (main.IsHandleCreated && !main.IsDisposed)
                        {
                            Chup("Bai06_01_frmMain", main, false);
                            daLuu = true;
                        }
                    }
                    if (!daLuu) GhiLog("Không chụp được frmMain (Form chưa tạo xong handle)");
                }
                catch (Exception ex)
                {
                    GhiLog("Lỗi chụp frmMain: " + ex.Message);
                }
                finally
                {
                    Environment.Exit(0);
                }
            });
            luong.IsBackground = true;
            luong.Start();

            for (int i = 0; i < 150; i++) Thread.Sleep(200);
        }

        /// <summary>Tạo bộ BUS/DAL có cùng dữ liệu mẫu như frmMain dùng ở chế độ bình thường.</summary>
        private static SinhVienBUS TaoBusSinhVien()
        {
            var bus = new SinhVienBUS(new SinhVienDAL());
            DuLieuMau.NapSinhVien(bus);
            return bus;
        }

        private static void Chup02_DataGridView()
        {
            SinhVienBUS bus = TaoBusSinhVien();
            var f = new frmSinhVien(bus, new KhoaBUS());
            HienForm(f);
            ChonDong(f, "dgvSinhVien", 2);
            GhiLog("[Bài 2] DataGridView liên kết BindingSource hiển thị 9 sinh viên mẫu; chọn dòng SV03");
            Chup("Bai06_02_DataGridView", f, true);
        }

        private static void Chup03_KiemTraLoi()
        {
            SinhVienBUS bus = TaoBusSinhVien();
            var f = new frmSinhVien(bus, new KhoaBUS());
            HienForm(f);

            Nhap(f, "txtMaSV", "SV01");     // trùng mã với dữ liệu mẫu
            Nhap(f, "txtHoTen", "Nguyễn 2"); // có chữ số -> BUS báo lỗi
            Nhap(f, "txtDiemTB", "12");      // ngoài [0,10]
            Bam(f, "btnLuu");
            GhiLog("[Bài 3] Lưu với dữ liệu sai: BUS trả LoiNghiepVu, ErrorProvider báo đúng ô, không ghi dữ liệu");
            Chup("Bai06_03_KiemTraLoi", f, true);
        }

        private static void Chup04_TimSapXep()
        {
            SinhVienBUS bus = TaoBusSinhVien();
            var f = new frmSinhVien(bus, new KhoaBUS());
            HienForm(f);

            Nhap(f, "txtTimKiem", "khoa: kinh tế");
            // Lọc theo khoa bằng ComboBox để thấy cả hai bộ lọc chạy qua BUS
            ChonMuc(f, "cboKhoaFilter", 2);
            Nhap(f, "txtTimKiem", "");
            SapXepCot(f, "dgvSinhVien", "DiemTB");
            GhiLog("[Bài 5] Lọc khoa 'Kinh tế' + sắp xếp theo điểm: BUS.TimKiem + BUS.SapXep");
            Chup("Bai06_04_TimSapXep", f, true);
        }

        private static void Chup05_TreeKhoa()
        {
            SinhVienBUS bus = TaoBusSinhVien();
            var f = new frmDanhMucSinhVien(bus);
            HienForm(f);

            // Mở rộng khoa thứ hai và chọn sinh viên đầu tiên
            TreeView tv = Tim(f, "tvKhoa") as TreeView;
            if (tv != null && tv.Nodes.Count > 1)
            {
                tv.Nodes[1].Expand();
                if (tv.Nodes[1].Nodes.Count > 0) tv.SelectedNode = tv.Nodes[1].Nodes[0];
            }
            BomTinHieu(300);
            GhiLog("[Bài 4] TreeView khoa - sinh viên: chọn khoa thứ hai, xem chi tiết sinh viên");
            Chup("Bai06_05_TreeKhoa", f, true);
        }

        private static void Chup06_SanPham()
        {
            var dalDM = new DanhMucDAL();
            var dalSP = new SanPhamDAL();
            var busDM = new DanhMucBUS(dalDM, dalSP);
            var busSP = new SanPhamBUS(dalSP, dalDM);
            DuLieuMau.NapSanPham(busSP, busDM);

            var f = new frmSanPham(busSP);
            HienForm(f);
            Bam(f, "btnKiemTraTong");
            GhiLog("[Bài 6] Tổng giá trị hàng tồn do SanPhamBUS tính, Form chỉ hiển thị");
            Chup("Bai06_06_SanPham", f, true);
        }

        private static void Chup07_TreeDanhMuc()
        {
            var dalDM = new DanhMucDAL();
            var dalSP = new SanPhamDAL();
            var busDM = new DanhMucBUS(dalDM, dalSP);
            var busSP = new SanPhamBUS(dalSP, dalDM);
            DuLieuMau.NapSanPham(busSP, busDM);

            var f = new frmDanhMuc(busDM);
            HienForm(f);

            // Cố xóa danh mục còn sản phẩm -> BUS từ chối (hiện ngay thông điệp lên Form)
            TreeView tv = Tim(f, "tvDanhMuc") as TreeView;
            if (tv != null && tv.Nodes.Count > 0) tv.SelectedNode = tv.Nodes[0];
            Bam(f, "btnXoa");
            GhiLog("[Bài 7] Xóa 'Điện tử' khi còn 3 sản phẩm: DanhMucBUS ném LoiNghiepVu, cây không đổi");
            Chup("Bai06_07_TreeDanhMuc", f, true);
        }

        private static void Chup08_ListView()
        {
            SinhVienBUS bus = TaoBusSinhVien();
            var f = new frmSinhVienIcon(bus);
            HienForm(f);
            Bam(f, "btnBieuTuong");
            ChonDong(f, "lvSinhVien", 0);
            GhiLog("[Bài 8] ListView dạng LargeIcon: biểu tượng ★/✓/! do ImageList tự vẽ theo học lực");
            Chup("Bai06_08_ListView", f, true);
        }

        private static void Chup09_CSV()
        {
            SinhVienBUS bus = TaoBusSinhVien();
            var f = new frmSinhVien(bus, new KhoaBUS());
            HienForm(f);

            string duongDan = Path.Combine(_thuMucAnh, "sinhvien_lab6.csv");
            f.XuatCSV(duongDan);
            GhiLog("[Bài 9] Xuất CSV qua BUS.LuuCSV -> DAL ghi tệp: " + Path.GetFileName(duongDan));
            Chup("Bai06_09_CSV", f, true);
        }

        private static void Chup10_KienTruc()
        {
            var f = new frmKienTruc();
            HienForm(f);
            Bam(f, "btnChayThu");
            GhiLog("[Bài 1] Sơ đồ 3 lớp + chạy thử: BUS chặn bản ghi có chữ số trong họ tên trước khi tới DAL");
            Chup("Bai06_10_KienTruc", f, true);
        }

        private static void Chup11_GioiThieu()
        {
            var f = new frmGioiThieu();
            HienForm(f);
            GhiLog("[Trợ giúp] frmGioiThieu: mục tiêu 10 bài, bản đồ form, phím tắt, cấu trúc thư mục");
            Chup("Bai06_11_GioiThieu", f, true);
        }

        private static void Chup12_XacNhanThoat()
        {
            // Hộp thoại MessageBox thật của hệ thống không chụp được trong chế độ tự động,
            // nên dựng một hộp thoại cùng nội dung (đúng câu hỏi trong frmMain_FormClosing).
            Form hopThoai = TaoHopThoaiXacNhan(
                "Bạn có chắc chắn muốn thoát ứng dụng không?", "Xác nhận");
            HienForm(hopThoai);
            GhiLog("[Bài 10] frmMain_FormClosing: Yes => thoát; No => e.Cancel = true (giữ ứng dụng)");
            Chup("Bai06_12_XacNhanThoat", hopThoai, true);
        }

        private static Form TaoHopThoaiXacNhan(string noiDung, string tieuDe)
        {
            var hopThoai = new Form();
            hopThoai.Text = tieuDe;
            hopThoai.FormBorderStyle = FormBorderStyle.FixedDialog;
            hopThoai.StartPosition = FormStartPosition.CenterScreen;
            hopThoai.MinimizeBox = false;
            hopThoai.MaximizeBox = false;
            hopThoai.ShowIcon = false;
            hopThoai.ClientSize = new Size(440, 186);
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
            lblNoiDung.BackColor = Color.White;
            lblNoiDung.SetBounds(78, 22, 340, 44);

            var lblGoiY = new Label();
            lblGoiY.Name = "lblGoiYHopThoai";
            lblGoiY.Text = "Yes: thoát ứng dụng    |    No: e.Cancel = true (giữ ứng dụng)";
            lblGoiY.ForeColor = Color.FromArgb(110, 118, 140);
            lblGoiY.BackColor = Color.White;
            lblGoiY.SetBounds(78, 66, 350, 22);

            var btnYes = new AccentButton();
            btnYes.Name = "btnDongYHopThoai";
            btnYes.Text = "Yes";
            btnYes.BaseColor = UiTheme.Teal;
            btnYes.SetBounds(188, 120, 110, 38);

            var btnNo = new AccentButton();
            btnNo.Name = "btnKhongHopThoai";
            btnNo.Text = "No";
            btnNo.BaseColor = UiTheme.Slate;
            btnNo.OutlineOnly = true;
            btnNo.SetBounds(308, 120, 110, 38);

            hopThoai.Controls.AddRange(new Control[] { lblIcon, lblNoiDung, lblGoiY, btnYes, btnNo });
            return hopThoai;
        }

        // =====================================================================
        // MÔ PHỎNG THAO TÁC NGƯỜI DÙNG
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

        private static void GhiCay(Control c, string thutLe)
        {
            GhiLog(thutLe + c.GetType().Name + " name='" + c.Name + "'");
            foreach (Control x in c.Controls) GhiCay(x, thutLe + "  ");
        }

        private static void Nhap(Form f, string ten, string giaTri)
        {
            Control c = Tim(f, ten);
            TextBox tb = c as TextBox;
            if (tb == null) { GhiLog("Không tìm thấy TextBox " + ten); GhiCay(f, ""); return; }

            tb.Text = string.Empty;
            tb.Text = giaTri;
            BomTinHieu(120);
        }

        private static void Bam(Form f, string ten)
        {
            Control c = Tim(f, ten);
            Button b = c as Button;
            if (b == null) { GhiLog("Không tìm thấy Button " + ten); GhiCay(f, ""); return; }
            if (!b.Enabled) { GhiLog("Nút " + ten + " đang bị vô hiệu hoá"); return; }

            b.PerformClick();
            BomTinHieu(200);
        }

        private static void ChonMuc(Form f, string ten, int viTri)
        {
            ComboBox cbo = Tim(f, ten) as ComboBox;
            if (cbo == null || viTri >= cbo.Items.Count) return;
            cbo.SelectedIndex = viTri;
            BomTinHieu(200);
        }

        private static void ChonDong(Form f, string ten, int viTri)
        {
            Control c = Tim(f, ten);

            DataGridView dgv = c as DataGridView;
            if (dgv != null)
            {
                if (viTri < dgv.Rows.Count)
                {
                    dgv.CurrentCell = dgv.Rows[viTri].Cells[1];
                    dgv.Rows[viTri].Selected = true;
                }
                return;
            }

            ListView lv = c as ListView;
            if (lv != null && viTri < lv.Items.Count)
            {
                lv.Items[viTri].Selected = true;
                lv.Items[viTri].Focused = true;
                lv.EnsureVisible(viTri);
            }
        }

        /// <summary>Giả lập bấm vào tiêu đề cột để sắp xếp.</summary>
        private static void SapXepCot(Form f, string ten, string tenCot)
        {
            DataGridView dgv = Tim(f, ten) as DataGridView;
            if (dgv == null || !dgv.Columns.Contains(tenCot)) return;

            MethodInfo m = typeof(frmSinhVien).GetMethod("dgvSinhVien_ColumnHeaderMouseClick",
                BindingFlags.Instance | BindingFlags.NonPublic);
            if (m != null)
                m.Invoke(f, new object[] { dgv, new DataGridViewCellMouseEventArgs(dgv.Columns[tenCot].Index, -1, 0, 0, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0)) });
            BomTinHieu(200);
        }

        // =====================================================================
        // TIỆN ÍCH
        // =====================================================================
        private static string TimThuMucBaoCao()
        {
            string[] ungVien = new string[]
            {
                @"d:\LAPTRINH.NET\Lap6",
                @"D:\LAPTRINH.NET\Lap6"
            };
            for (int i = 0; i < ungVien.Length; i++)
            {
                if (File.Exists(Path.Combine(ungVien[i], "Lab06_ThreeLayer.csproj")))
                    return Path.Combine(ungVien[i], "BaoCao");
            }

            DirectoryInfo thuMuc = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (thuMuc != null)
            {
                string[] csproj = Directory.GetFiles(thuMuc.FullName, "Lab06_ThreeLayer.csproj");
                if (csproj.Length > 0) return Path.Combine(thuMuc.FullName, "BaoCao");
                thuMuc = thuMuc.Parent;
            }
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BaoCao");
        }

        private static void GhiLog(string text)
        {
            if (text == NhatKy.ToString()) return;
            NhatKy.AppendLine(text);
            Console.WriteLine("[BaoCao] " + text);
        }

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

        internal static void GhiLoiNghiemTrong(string noi, Exception ex)
        {
            try
            {
                GhiLog("LỖI ở " + noi + ": " + ex);
                string thuMuc = string.IsNullOrEmpty(_thuMucAnh) ? AppDomain.CurrentDomain.BaseDirectory : _thuMucAnh;
                File.AppendAllText(Path.Combine(thuMuc, "chup_anh_loi.txt"),
                    DateTime.Now.ToString("HH:mm:ss") + " - " + noi + Environment.NewLine + ex + Environment.NewLine,
                    new UTF8Encoding(true));
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

        private static Bitmap CaptureClient(Form f)
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
                GhiLog("DrawToBitmap lỗi (" + f.GetType().Name + "): " + ex.Message);
            }
            return bmp;
        }

        private static void HienForm(Form f)
        {
            f.Show();
            BomTinHieu(500);
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
                GhiLog("Đã lưu: " + ten + ".png (" + w + "x" + h + ")");
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
                BomTinHieu(200);
            }
        }
    }
}
