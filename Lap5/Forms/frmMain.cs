using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Lab05_MultiForm.Helpers;
using Lab05_MultiForm.Models;

namespace Lab05_MultiForm.Forms
{
    /// <summary>
    /// frmMain - Bài 5 (MenuStrip, mở Form con chống trùng) + Bài 9 (FormClosing)
    ///           + Bài 10 (Dashboard: menu, StatusStrip, đồng hồ).
    ///
    /// Ứng dụng dùng MDI: menu "Nghiệp vụ" mở Form con dạng MDI (xem chi tiết - danh sách),
    /// các tính năng hỗ trợ (tìm kiếm, chọn khoa, đăng nhập, giới thiệu) mở dạng hộp thoại.
    /// </summary>
    public partial class frmMain : FormBase
    {
        // ---------------------------------------------------------------
        // Trạng thái Dashboard
        // ---------------------------------------------------------------
        private readonly System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer();
        private readonly BindingList<SinhVien> _ds = new BindingList<SinhVien>();
        private string _trangThai = "Sẵn sàng";
        private int _soLanMoForm;
        private bool _dangChinhSua;

        public frmMain()
        {
            InitializeComponent();
        }

        // =====================================================================
        // KHỞI TẠO
        // =====================================================================
        private void frmMain_Load(object sender, EventArgs e)
        {

            foreach (Control c in Controls)
            {
                mdiClient = c as MdiClient;
                if (mdiClient != null) break;
            }

            // Vùng MDI tô nền xám nhạt như Visual Studio
            if (mdiClient != null)
            {

                mdiClient.BackColor = Color.FromArgb(236, 239, 247);
                mdiClient.Resize += MdiClient_Resize;
            }

            // Dashboard (4 phiếu chỉ số + thẻ kiểm tra nhanh) nằm trên vùng MDI như một "trang chủ".
            // Khi mở form con sẽ tạm ẩn; đóng hết form con thì hiện lại.
            pnlDashboard.Parent = this;

            pnlDashboard.Bounds = ClientRectangle;
            pnlDashboard.BringToFront();

            // Nạp dữ liệu mẫu để các chỉ số trên Dashboard có ý nghĩa
            foreach (SinhVien sv in DuLieuMau.DanhSachSinhVien())
                _ds.Add(sv);
            _ds.ListChanged += Ds_ListChanged;

            CapNhatDashboard();

            SapXepDashboard();
            DongHo_Tick(null, EventArgs.Empty);

            _timer.Interval = 1000;
            _timer.Tick += DongHo_Tick;
            _timer.Start();

            Log("Ứng dụng khởi động - chào mừng bạn đến với Lab 5!");
        }

        /// <summary>Sắp xếp 4 phiếu chỉ số + thẻ kiểm tra nhanh trong Dashboard.</summary>
        private void SapXepDashboard()
        {
            int width = Math.Max(ClientSize.Width, 980);
            int cardW = 240;
            int gap = 16;
            int left = 20;
            int top = 20;

            CardPanel[] cards = { card1, card2, card3, card4 };
            for (int i = 0; i < cards.Length; i++)
            {
                cards[i].SetBounds(left + i * (cardW + gap), top, cardW, 96);
            }

            cardNhanh.SetBounds(left, top + 116, Math.Max(600, width - left * 2 - 20), 228);
        }

        /// <summary>Ẩn Dashboard khi có form con, hiện lại khi không còn form con nào.</summary>
        private void CapNhatTrangChu()
        {
            bool conFormCon = MdiChildren.Length > 0;
            pnlDashboard.Visible = !conFormCon;
            if (!conFormCon) pnlDashboard.BringToFront();
        }

        private void DongHo_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            tsslDongHo.Text = " " + now.ToString("HH:mm:ss") + "  " + now.ToString("dddd, dd/MM/yyyy");
            tsslSoLanMo.Text = string.Format("Đã mở {0} form", _soLanMoForm);
            tsslTrangThai.Text = " " + _trangThai;
            tsslSoSV.Text = " " + _ds.Count + " sinh viên";
            menuPhienBan.Text = "Phiên bản 5.0 - " + now.ToString("dd/MM/yyyy");
        }

        private void Ds_ListChanged(object sender, ListChangedEventArgs e)
        {
            CapNhatDashboard();
        }

        // =====================================================================
        // BÀI 10: THỐNG KÊ PHIÊU CHỈ SỐ
        // =====================================================================
        private void CapNhatDashboard()
        {
            TinhLaiThongKe();
        }

        private void TinhLaiThongKe()
        {
            // Trong lúc form chi tiết đang sửa (dữ liệu nháp chưa lưu) thì không tính lại,
            // tránh trường hợp đóng form mà chọn "Không lưu" nhưng Dashboard đã đổi số.
            if (_dangChinhSua) return;

            double tong = 0;
            double cao = 0;
            int gioiTroLen = 0;
            foreach (SinhVien sv in _ds)
            {
                tong += sv.DiemTB;
                if (sv.DiemTB > cao) cao = sv.DiemTB;
                if (sv.DiemTB >= 8) gioiTroLen++;
            }

            lblStatTong.Text = _ds.Count.ToString();
            lblStatDiemTB.Text = _ds.Count == 0 ? "0.00" : (tong / _ds.Count).ToString("0.00");
            lblStatCaoNhat.Text = _ds.Count == 0 ? "0.00" : cao.ToString("0.0");
            lblStatGioi.Text = gioiTroLen.ToString();
        }

        // =====================================================================
        // BÀI 5: MDI - mở Form con, tránh mở trùng
        // =====================================================================
        private void OpenChildForm(Form child)
        {
            foreach (Form f in this.MdiChildren)
            {
                if (f.GetType() == child.GetType())
                {
                    f.Activate();
                    child.Dispose();
                    Log("Form " + f.Text + " đã mở - chuyển sang tab đang chạy (không mở trùng)");
                    return;
                }
            }

            child.MdiParent = this;
            child.Show();
            _soLanMoForm++;
            Log("Mở form con: " + child.Text);

            if (child is frmDanhSach)
            {
                var ds = (frmDanhSach)child;
                ds.MoChiTiet -= DanhSach_MoChiTiet;
                ds.MoChiTiet += DanhSach_MoChiTiet;
            }

            DuaVaoMenuCon(child);
            CapNhatTrangChu();
        }

        private void frmCon_FormClosed(object sender, FormClosedEventArgs e)
        {
            CapNhatTrangChu();
        }

        /// <summary>Hiển thị form con trong menu "Nghiệp vụ" (mdiWindowListItem) giống Visual Studio.</summary>
        private void DuaVaoMenuCon(Form child)
        {
            var item = new ToolStripMenuItem(child.Text);
            child.Tag = item;
            item.Tag = child;
            item.Click += (s, e) =>
            {
                if (!child.IsDisposed)
                {
                    child.Activate();
                    if (child.WindowState == FormWindowState.Minimized) child.WindowState = FormWindowState.Normal;
                }
            };
            menuNghiepVu.DropDownItems.Add(item);
            child.FormClosed += frmCon_FormClosed;
            child.FormClosed += (s, e) =>
            {
                if (menuNghiepVu.DropDownItems.Contains(item)) menuNghiepVu.DropDownItems.Remove(item);
                item.Dispose();
            };
        }

        /// <summary>Form danh sách yêu cầu mở chi tiết (Xem chi tiết từng sinh viên đang chọn).</summary>
        private void DanhSach_MoChiTiet(object sender, SinhVien sv)
        {
            MoChiTietSinhVien(sv);
        }

        /// <summary>Tìm sinh viên theo mã trong danh sách gốc (BindingList không có Find như List).</summary>
        public SinhVien TimTheoMa(string maSV)
        {
            foreach (SinhVien sv in _ds)
            {
                if (sv.MaSV == maSV) return sv;
            }
            return null;
        }

        /// <summary>Mở frmChiTietSinhVien, truyền đối tượng SinhVien qua constructor.
        /// Form chi tiết sửa trên BẢN SAO (sv.Clone()) và chỉ trả về khi người dùng bấm Lưu.
        /// </summary>
        public void MoChiTietSinhVien(SinhVien sv)
        {
            if (sv == null) return;

            SinhVien goc = TimTheoMa(sv.MaSV);
            if (goc == null) return;

            // Tạm hoãn thống kê để "Không lưu" không làm sai chỉ số trên Dashboard
            BatDauChinhSua();

            using (var chiTiet = new frmChiTietSinhVien(goc.Clone()))
            {
                DialogResult r = MoHopThoai(chiTiet);

                if (r == DialogResult.OK)
                {
                    SinhVien daSua = chiTiet.SinhVien;
                    goc.HoTen = daSua.HoTen;
                    goc.NgaySinh = daSua.NgaySinh;
                    goc.DiemTB = daSua.DiemTB;
                    goc.Khoa = daSua.Khoa;
                    goc.GioiTinh = daSua.GioiTinh;
                    goc.SoThich = daSua.SoThich;
                    goc.AnhDaiDien = daSua.AnhDaiDien;
                }

                KetThucChinhSua(r == DialogResult.OK);
            }
        }

        /// <summary>Mở hộp thoại con (modal) và ghi log khi đóng.</summary>
        private DialogResult MoHopThoai(Form dialog)
        {
            _soLanMoForm++;
            Log("Mở hộp thoại: " + dialog.Text);
            DialogResult r = dialog.ShowDialog(this);
            Log("Đóng hộp thoại: " + dialog.Text + " (" + r + ")");
            return r;
        }

        private void mnuSinhVien_Click(object sender, EventArgs e)
        {
            OpenChildForm(new frmSinhVien());
        }

        private void mnuDanhSach_Click(object sender, EventArgs e)
        {
            OpenChildForm(new frmDanhSach());
        }

        private void mnuDongTatCa_Click(object sender, EventArgs e)
        {
            foreach (Form f in MdiChildren)
                f.Close();
            Log("Đã đóng tất cả form con");
        }

        // =====================================================================
        // BÀI 7: Hộp thoại tìm kiếm - nhận kết quả qua thuộc tính
        // =====================================================================
        private void mnuTimKiem_Click(object sender, EventArgs e)
        {
            using (var dlg = new frmTimKiem(_ds))
            {
                if (MoHopThoai(dlg) == DialogResult.OK && dlg.KetQua != null)
                {
                    Log("Tìm kiếm \"" + dlg.TuKhoa + "\" => " + dlg.KetQua.MaSV + " - " + dlg.KetQua.HoTen);

                    using (var chiTiet = new frmChiTietSinhVien(dlg.KetQua))
                    {
                        MoChiTietSinhVien(dlg.KetQua);
                    }
                }
            }
        }

        // =====================================================================
        // BÀI 8: Form chọn khoa - dữ liệu truyền qua constructor, trả về qua property
        // =====================================================================
        private void mnuChonKhoa_Click(object sender, EventArgs e)
        {
            using (var dlg = new frmChonKhoa(DuLieuMau.DanhSachKhoa()))
            {
                if (MoHopThoai(dlg) == DialogResult.OK && dlg.KhoaDuocChon != null)
                {
                    Log("Chọn khoa: " + dlg.KhoaDuocChon.TenKhoa + " (" + dlg.KhoaDuocChon.MaKhoa + ")");
                    MessageBox.Show(
                        "Đã chọn khoa:\n\n" + dlg.KhoaDuocChon.TenKhoa + "\nMã khoa: " + dlg.KhoaDuocChon.MaKhoa,
                        "Khoa được chọn", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        // =====================================================================
        // BÀI 6: Form đăng nhập giới hạn 3 lần thử / khóa 30 giây
        // =====================================================================
        private void mnuDangNhap_Click(object sender, EventArgs e)
        {
            using (var dlg = new frmDangNhap())
            {
                if (MoHopThoai(dlg) == DialogResult.OK)
                {
                    Log("Đăng nhập thành công: " + dlg.TenDangNhap + " (" + dlg.VaiTro + ")");
                    tsslNguoiDung.Text = "  " + dlg.TenDangNhap + " - " + dlg.VaiTro;
                    MessageBox.Show("Đăng nhập thành công!\nXin chào " + dlg.TenDangNhap + " (" + dlg.VaiTro + ").",
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        // =====================================================================
        // Trợ giúp
        // =====================================================================
        private void mnuGioiThieu_Click(object sender, EventArgs e)
        {
            MoHopThoai(new frmGioiThieu());
        }

        private void mnuKiemTraNhanh_Click(object sender, EventArgs e)
        {
            using (var dlg = new frmKiemTraDuLieu())
            {
                MoHopThoai(dlg);
            }
        }

        private void mnuThoat_Click(object sender, EventArgs e)
        {
            Close();
        }

        // =====================================================================
        // BÀI 9: Xác nhận khi đóng ứng dụng (FormClosing)
        // =====================================================================
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Lưu ý: kiểm tra e.CloseReason để không hỏi lại khi Windows tắt máy
            if (e.CloseReason != CloseReason.UserClosing) return;

            DialogResult kq = MessageBox.Show(
                "Bạn có chắc chắn muốn thoát ứng dụng không?\n\nPhiên làm việc hiện có "
                + MdiChildren.Length + " form con đang mở.",
                "Xác nhận thoát",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (kq != DialogResult.Yes)
            {
                e.Cancel = true;   // KHÔNG đóng ứng dụng
                Log("Người dùng chọn Không - hủy đóng ứng dụng");
                tsslTrangThai.Text = " Đã hủy thoát";
            }
            else
            {
                _timer.Stop();
            }
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (mdiClient != null) mdiClient.Resize -= MdiClient_Resize;
            _ds.ListChanged -= Ds_ListChanged;
            _timer.Tick -= DongHo_Tick;
            _timer.Stop();
            _timer.Dispose();
        }

        private void MdiClient_Resize(object sender, EventArgs e)
        {
            // Form con tự do co giãn theo MdiClient, Dashboard chỉ cần giữ kích thước đầy đủ
            if (pnlDashboard.Visible) pnlDashboard.Bounds = ClientRectangle;
        }

        // =====================================================================
        // DÀNH CHO FORM CHI TIẾT: tạm hoãn cập nhật chỉ số khi đang sửa dữ liệu
        // =====================================================================
        /// <summary>Gọi khi form chi tiết bắt đầu sửa (Giữ lại = false theo Quy tắc 6).</summary>
        public void BatDauChinhSua()
        {
            _dangChinhSua = true;
            Log("Đang xem chi tiết - chỉ số tạm hoãn cập nhật");
        }

        /// <summary>Gọi khi form chi tiết đóng. onDaLuu = true nếu người dùng đã lưu thay đổi.</summary>
        public void KetThucChinhSua(bool daLuu)
        {
            _dangChinhSua = false;
            CapNhatDashboard();
            Log(daLuu ? "Đã cập nhật dữ liệu từ form chi tiết" : "Đã hủy thay đổi từ form chi tiết");
        }

        // =====================================================================
        // LOG LÊN STATUSSTRIP
        // =====================================================================
        public void Log(string thongDiep)
        {
            _trangThai = thongDiep;
            tsslTrangThai.Text = " " + thongDiep;
            if (Environment.GetEnvironmentVariable("LAB5_SCREENSHOT") == "1")
                Console.WriteLine("[frmMain] " + thongDiep);
        }

        // Cho các Form con gọi ngược vào hàm chung (không dùng biến toàn cục)
        public BindingList<SinhVien> DanhSach { get { return _ds; } }
    }
}
