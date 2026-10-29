using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Lab05_MultiForm.Helpers;
using Lab05_MultiForm.Models;

namespace Lab05_MultiForm.Forms
{
    /// <summary>
    /// frmSinhVien - kế thừa giao diện Lab 4, bổ sung toàn bộ nội dung Lab 5:
    ///   Bài 1: ValidateInput() + ErrorProvider báo lỗi ngay cạnh control.
    ///   Bài 2: txtDiemTB chỉ cho nhập chữ số và MỘT dấu thập phân (KeyPress).
    ///   Bài 3: TextChanged chung cập nhật xem trước + trạng thái hợp lệ theo thời gian thực.
    ///   Bài 4: Mở frmChiTietSinhVien bằng constructor, nhận kết quả qua thuộc tính,
    ///           và phát sự kiện SinhVienDaLuu để Form khác (frmDanhSach) tự làm mới.
    ///
    /// Việc nhập/sửa dữ liệu chỉ hoạt động khi Form đang ở chế độ ChoPhépNhap (mở từ menu
    /// "Nhập thông tin sinh viên"). Khi xem lại một sinh viên từ danh sách, Form ở chế độ chỉ đọc.
    /// </summary>
    public partial class frmSinhVien : FormBase
    {
        private const string TrangThaiDiemHopLe = "Diem hop le";

        // ---------------------------------------------------------------
        // Sự kiện public: Form cha / frmDanhSach đăng ký để biết khi có dữ liệu mới.
        // Đây là cách truyền dữ liệu giữa Form bằng delegate/event (không dùng biến toàn cục).
        // ---------------------------------------------------------------
        public event EventHandler<SinhVien> SinhVienDaLuu;

        /// <summary>Chế độ cho phép nhập/sửa.</summary>
        public bool ChoPhépNhap { get; set; }

        /// <summary>Sinh viên đang hiển thị (null nếu là nhập mới).</summary>
        public SinhVien SinhVienHienTai { get; private set; }

        public frmSinhVien()
            : this(null, true)
        {
        }

        /// <summary>Mở Form ở chế độ chỉ xem một sinh viên đã có (từ frmDanhSach).</summary>
        public frmSinhVien(SinhVien sv)
            : this(sv, false)
        {
        }

        public frmSinhVien(SinhVien sv, bool choPhepNhap)
        {
            InitializeComponent();

            ChoPhépNhap = choPhepNhap;
            SinhVienHienTai = sv;
            if (sv != null)
            {
                Text = "Chi tiết sinh viên: " + sv.MaSV + " - " + sv.HoTen;
            }
        }

        // =====================================================================
        // KHỞI TẠO + LOAD DỮ LIỆU
        // =====================================================================
        private void frmSinhVien_Load(object sender, EventArgs e)
        {
            NapDanhSachKhoa();
            NapSoThich();

            if (SinhVienHienTai != null) HienThiSinhVien(SinhVienHienTai);

            ChuyenCheDo(ChoPhépNhap);
            CapNhatXemTruoc();

            // Đặt con trỏ vào ô đầu tiên
            if (ChoPhépNhap) txtMaSV.Focus();
        }

        private void NapDanhSachKhoa()
        {
            cboKhoa.Items.Clear();
            foreach (Khoa k in DuLieuMau.DanhSachKhoa()) cboKhoa.Items.Add(k);
            if (cboKhoa.Items.Count > 0) cboKhoa.SelectedIndex = 0;
        }

        private void NapSoThich()
        {
            // Sở thích: mỗi mục là một CheckBox xếp thành 2 cột
            string[] ds = { "Đọc sách", "Lập trình", "Âm nhạc", "Thể thao", "Du lịch", "Hiếu học" };
            for (int i = 0; i < ds.Length; i++)
            {
                var chk = new CheckBox();
                chk.Text = ds[i];
                chk.Tag = ds[i];
                chk.Font = UiTheme.FontBody(9f);
                chk.ForeColor = UiTheme.InkSoft;
                chk.BackColor = Color.Transparent;
                chk.AutoSize = true;
                chk.CheckedChanged += BatBuoc_CheckedChanged;
                int row = i / 2;
                int col = i % 2;
                chk.Location = new Point(4 + col * 108, 2 + row * 26);
                pnlSoThich.Controls.Add(chk);
            }
        }

        private void HienThiSinhVien(SinhVien sv)
        {
            UiFactory.SetVal(txtMaSV, sv.MaSV);
            UiFactory.SetVal(txtHoTen, sv.HoTen);
            dtpNgaySinh.Value = BatGioiHanNgay(sv.NgaySinh);

            radNam.Checked = sv.GioiTinh != "Nữ";
            radNu.Checked = sv.GioiTinh == "Nữ";

            foreach (object o in cboKhoa.Items)
            {
                if (o.ToString() == sv.Khoa) cboKhoa.SelectedItem = o;
            }
            if (cboKhoa.SelectedIndex < 0) cboKhoa.SelectedIndex = 0;

            UiFactory.SetVal(txtDiemTB, sv.DiemTB.ToString("0.0"));

            List<string> soThich = UiTheme.SplitList(sv.SoThich);
            foreach (Control c in pnlSoThich.Controls)
            {
                CheckBox chk = c as CheckBox;
                if (chk == null) continue;
                chk.Checked = soThich.Contains((string)chk.Tag);
            }

            HienThiAnh(sv.AnhDaiDien, sv.HoTen);
        }

        private DateTime BatGioiHanNgay(DateTime d)
        {
            if (d < dtpNgaySinh.MinDate) return dtpNgaySinh.MinDate;
            if (d > dtpNgaySinh.MaxDate) return dtpNgaySinh.MaxDate;
            return d;
        }

        private void HienThiAnh(string duongDan, string hoTen)
        {
            if (!string.IsNullOrEmpty(duongDan) && System.IO.File.Exists(duongDan))
            {
                try
                {
                    picAnh.Image = Image.FromFile(duongDan);
                    lblTenFileAnh.Text = System.IO.Path.GetFileName(duongDan);
                    return;
                }
                catch (Exception) { }
            }
            picAnh.Image = UiTheme.CreateAvatarImage(hoTen, 150);
            lblTenFileAnh.Text = "Ảnh đại diện mặc định (tự vẽ)";
        }

        /// <summary>Chuyển giữa chế độ nhập dữ liệu và chế độ chỉ xem.</summary>
        private void ChuyenCheDo(bool choPhep)
        {
            txtMaSV.ReadOnly = !choPhep;
            txtHoTen.ReadOnly = !choPhep;
            txtDiemTB.ReadOnly = !choPhep;
            dtpNgaySinh.Enabled = choPhep;
            cboKhoa.Enabled = choPhep;
            radNam.Enabled = choPhep;
            radNu.Enabled = choPhep;
            btnChonAnh.Enabled = choPhep;
            btnXoaAnh.Enabled = choPhep;
            btnLamMoi.Enabled = choPhep;
            pnlSoThich.Enabled = choPhep;

            txtMaSV.BackColor = choPhep ? Color.White : Color.FromArgb(246, 247, 251);
            txtHoTen.BackColor = txtMaSV.BackColor;
            txtDiemTB.BackColor = txtMaSV.BackColor;

            btnLuu.Enabled = choPhep;
            lblCheDo.Text = choPhep
                ? "CHẾ ĐỘ: NHẬP DỮ LIỆU"
                : "CHẾ ĐỘ: CHỈ XEM  ·  mở menu Nghiệp vụ » Nhập thông tin sinh viên để tạo mới";
            lblCheDo.ForeColor = choPhep ? UiTheme.Teal : UiTheme.Muted;
        }

        // =====================================================================
        // BÀI 1: ValidateInput() - kiểm tra bắt buộc bằng ErrorProvider
        // =====================================================================
        private bool ValidateInput()
        {
            errorProvider1.Clear();
            bool valid = true;
            string loi;

            // ---- Mã sinh viên ----
            if (!KiemTra.HopLeMaSV(UiFactory.Val(txtMaSV), out loi))
            {
                errorProvider1.SetError(txtMaSV, loi);
                valid = false;
            }

            // ---- Họ tên ----
            if (!KiemTra.HopLeHoTen(UiFactory.Val(txtHoTen), out loi))
            {
                errorProvider1.SetError(txtHoTen, loi);
                valid = false;
            }

            // ---- Ngày sinh ----
            if (!KiemTra.HopLeNgaySinh(dtpNgaySinh.Value, out loi))
            {
                errorProvider1.SetError(dtpNgaySinh, loi);
                valid = false;
            }

            // ---- Điểm trung bình ----
            double diem;
            if (!KiemTra.HopLeDiem(UiFactory.Val(txtDiemTB), out diem, out loi))
            {
                errorProvider1.SetError(txtDiemTB, loi);
                valid = false;
            }

            // ---- Khoa ----
            if (cboKhoa.SelectedItem == null)
            {
                errorProvider1.SetError(cboKhoa, "Vui lòng chọn khoa.");
                valid = false;
            }

            return valid;
        }

        // =====================================================================
        // BÀI 2: Giới hạn ký tự nhập điểm (KeyPress)
        // =====================================================================
        private void txtDiemTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            KiemTra.ChanNhapDiem(sender, e);
        }

        // =====================================================================
        // BÀI 3: Cập nhật trạng thái theo thời gian thực (TextChanged dùng chung)
        // =====================================================================
        private void Input_TextChanged(object sender, EventArgs e)
        {
            CapNhatXemTruoc();

            // Khi đã hiện lỗi thì kiểm tra lại ngay, lỗi hết thì tự biến mất
            TextBox tb = sender as TextBox;
            if (tb != null && !string.IsNullOrEmpty(errorProvider1.GetError(tb)))
            {
                KiemTraLaiMotO(tb);
            }
        }

        private void KiemTraLaiMotO(TextBox tb)
        {
            string loi = null;
            string val = UiFactory.Val(tb);
            if (tb == txtMaSV) KiemTra.HopLeMaSV(val, out loi);
            else if (tb == txtHoTen) KiemTra.HopLeHoTen(val, out loi);
            else if (tb == txtDiemTB)
            {
                double d;
                KiemTra.HopLeDiem(val, out d, out loi);
            }
            errorProvider1.SetError(tb, loi ?? string.Empty);
        }

        private void CapNhatXemTruoc()
        {
            string ma = UiFactory.Val(txtMaSV);
            string ten = UiFactory.Val(txtHoTen);
            string diemText = UiFactory.Val(txtDiemTB);

            // --- Xem trước mã - họ tên ---
            lblXemTruoc.Text = (ma.Length == 0 ? "…" : ma) + "  -  " + (ten.Length == 0 ? "…" : ten);

            // --- Trạng thái điểm ---
            double diem;
            string loi;
            bool hopLe = KiemTra.HopLeDiem(diemText, out diem, out loi);

            if (diemText.Length == 0)
            {
                lblTrangThai.Text = "Chưa nhập điểm trung bình";
                lblTrangThai.ForeColor = UiTheme.Muted;
                badgeDiem.Value = "CHƯA NHẬP";
                badgeDiem.SetColor(UiTheme.Slate);
            }
            else if (hopLe)
            {
                lblTrangThai.Text = "Dữ liệu điểm hợp lệ - xếp loại " + KiemTra.XepLoai(diem);
                lblTrangThai.ForeColor = KiemTra.MauXepLoai(diem);
                badgeDiem.Value = KiemTra.XepLoai(diem).ToUpperInvariant();
                badgeDiem.SetColor(KiemTra.MauXepLoai(diem));
            }
            else
            {
                lblTrangThai.Text = "Kiểm tra điểm: " + loi;
                lblTrangThai.ForeColor = UiTheme.Red;
                badgeDiem.Value = "KHÔNG HỢP LỆ";
                badgeDiem.SetColor(UiTheme.Red);
            }

            // --- Trạng thái tổng hợp (mã + họ tên) ---
            bool maOk = KiemTra.HopLeMaSV(ma, out loi);
            bool tenOk = KiemTra.HopLeHoTen(ten, out loi);
            if (ma.Length == 0 && ten.Length == 0)
            {
                badgeTong.Value = "CHƯA NHẬP";
                badgeTong.SetColor(UiTheme.Slate);
            }
            else if (maOk && tenOk && hopLe)
            {
                badgeTong.Value = "SẴN SÀNG LƯU";
                badgeTong.SetColor(UiTheme.Green);
            }
            else
            {
                badgeTong.Value = "CẦN KIỂM TRA";
                badgeTong.SetColor(UiTheme.Amber);
            }

            // --- Tuổi + giới tính + khoa ---
            DateTime ns = dtpNgaySinh.Value;
            int tuoi = KiemTra.TinhTuoi(ns);
            lblTuoi.Text = string.Format("Tuổi: {0}   ·   {1:dd/MM/yyyy}", tuoi, ns);
            lblTuoi.ForeColor = (tuoi < KiemTra.TuoiToiThieu || tuoi > KiemTra.TuoiToiDa)
                ? UiTheme.Red
                : UiTheme.InkSoft;

            // --- Số thích ---
            string st = LayDanhSachSoThich();
            lblSoThich.Text = st.Length == 0 ? "Chưa chọn sở thích" : st;

            // --- Gợi ý vùng ErrorProvider (tính cả ngày sinh) ---
            string loiNgay;
            bool ngayOk = KiemTra.HopLeNgaySinh(ns, out loiNgay);
            lblThongBao.Text = (maOk && tenOk && hopLe && ngayOk)
                ? "Tất cả dữ liệu bắt buộc đã hợp lệ - bạn có thể bấm Lưu."
                : "Nhập đủ Mã sinh viên, Họ tên và Điểm trong khoảng 0 - 10 (xem lỗi cạnh từng ô).";
            lblThongBao.ForeColor = (maOk && tenOk && hopLe && ngayOk) ? UiTheme.Green : UiTheme.Muted;
        }

        private string LayDanhSachSoThich()
        {
            var ds = new List<string>();
            foreach (Control c in pnlSoThich.Controls)
            {
                CheckBox chk = c as CheckBox;
                if (chk != null && chk.Checked) ds.Add(chk.Text);
            }
            return string.Join(", ", ds.ToArray());
        }

        private string ChonGioiTinh()
        {
            return radNu.Checked ? "Nữ" : "Nam";
        }

        private void BatBuoc_CheckedChanged(object sender, EventArgs e)
        {
            CapNhatXemTruoc();
        }

        private void dtpNgaySinh_ValueChanged(object sender, EventArgs e)
        {
            CapNhatXemTruoc();
        }

        private void cboKhoa_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Bài 5 (Lab 5): SelectedIndexChanged - đổi màu viền để người dùng biết đã chọn
            cboKhoa.FlatStyle = cboKhoa.SelectedIndex >= 0 ? FlatStyle.Flat : FlatStyle.Standard;
            CapNhatXemTruoc();
        }

        // =====================================================================
        // BÀI 4: Lưu dữ liệu + mở Form chi tiết
        // =====================================================================
        private bool TaoSinhVienTuGiaoDien(out SinhVien sv, out double diem)
        {
            sv = null;
            diem = 0;
            string loi;
            if (!KiemTra.HopLeDiem(UiFactory.Val(txtDiemTB), out diem, out loi)) return false;

            sv = new SinhVien();
            sv.MaSV = UiFactory.Val(txtMaSV);
            sv.HoTen = KiemTra.ChuanHoaHoTen(UiFactory.Val(txtHoTen));
            sv.NgaySinh = dtpNgaySinh.Value;
            sv.DiemTB = diem;
            sv.Khoa = cboKhoa.Text;
            sv.GioiTinh = ChonGioiTinh();
            sv.SoThich = LayDanhSachSoThich();
            sv.AnhDaiDien = txtDuongDanAnh.Text;
            return true;
        }

        /// <summary>Dùng cho các Form khác (hoặc trình chụp ảnh báo cáo) gọi xử lý Lưu.</summary>
        public void LuuSinhVien()
        {
            btnLuu_Click(btnLuu, EventArgs.Empty);
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
            {
                lblThongBao.Text = "Không thể lưu: " + DemLoi() + " ô dữ liệu chưa hợp lệ (xem biểu tượng lỗi cạnh ô nhập).";
                lblThongBao.ForeColor = UiTheme.Red;
                System.Media.SystemSounds.Exclamation.Play();
                return;
            }

            SinhVien sv;
            double diem;
            if (!TaoSinhVienTuGiaoDien(out sv, out diem)) return;

            // Thông báo (dùng MessageBox chuẩn của WinForms).
            // Khi chạy ở chế độ tự chụp ảnh báo cáo (LAB5_SCREENSHOT=1) thì bỏ qua hộp thoại
            // để báo cáo không bị chặn, thay vào đó ghi lại nội dung.
            bool tuDong = Environment.GetEnvironmentVariable("LAB5_SCREENSHOT") == "1";
            bool duLieuMoi = SinhVienHienTai == null || !ChoPhépNhap;

            if (tuDong)
            {
                Console.WriteLine("[frmSinhVien] Lưu " + sv.MaSV + " - " + sv.HoTen + string.Format(" - {0:0.0} ({1})", sv.DiemTB, sv.XepLoai()));
            }
            else
            {
                DialogResult tt = MessageBox.Show(
                    string.Format("Lưu thành công sinh viên:\n\n{0} - {1}\nĐiểm: {2:0.0} ({3})\nKhoa: {4}\n\nBạn có muốn mở Form chi tiết không?",
                        sv.MaSV, sv.HoTen, sv.DiemTB, sv.XepLoai(), sv.Khoa),
                    "Lưu thành công", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (tt == DialogResult.Yes)
                {
                    // Truyền dữ liệu sang Form chi tiết qua CONSTRUCTOR
                    using (var chiTiet = new frmChiTietSinhVien(sv.Clone(), "vừa lưu từ frmSinhVien"))
                    {
                        chiTiet.ShowDialog(this);

                        // Nhận dữ liệu trả về qua THUỘC TÍNH
                        if (chiTiet.DialogResult == DialogResult.OK)
                        {
                            HienThiSinhVien(chiTiet.SinhVien);
                        }
                    }
                }
            }

            SinhVienHienTai = sv;

            // Phát sự kiện cho Form khác (frmDanhSach) - không dùng biến toàn cục
            PhátSuKienDaLuu(sv);

            if (duLieuMoi)
            {
                lblThongBao.Text = "Đã lưu " + sv.MaSV + ". Bạn có thể nhập tiếp sinh viên khác hoặc bấm Làm mới.";
                lblThongBao.ForeColor = UiTheme.Green;
            }
        }

        private void PhátSuKienDaLuu(SinhVien sv)
        {
            EventHandler<SinhVien> handler = SinhVienDaLuu;
            if (handler != null) handler(this, sv);
        }

        private int DemLoi()
        {
            int n = 0;
            if (!string.IsNullOrEmpty(errorProvider1.GetError(txtMaSV))) n++;
            if (!string.IsNullOrEmpty(errorProvider1.GetError(txtHoTen))) n++;
            if (!string.IsNullOrEmpty(errorProvider1.GetError(txtDiemTB))) n++;
            if (!string.IsNullOrEmpty(errorProvider1.GetError(dtpNgaySinh))) n++;
            if (!string.IsNullOrEmpty(errorProvider1.GetError(cboKhoa))) n++;
            return n;
        }

        // =====================================================================
        // BÀI 5 (Lab 4) - Ảnh đại diện, làm mới, đóng
        // =====================================================================
        private void btnChonAnh_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Title = "Chọn ảnh đại diện";
                dlg.Filter = "Tệp ảnh|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        picAnh.Image = Image.FromFile(dlg.FileName);
                        txtDuongDanAnh.Text = dlg.FileName;
                        lblTenFileAnh.Text = System.IO.Path.GetFileName(dlg.FileName);
                    }
                    catch (Exception ex)
                    {
                        errorProvider1.SetError(btnChonAnh, "Không đọc được ảnh: " + ex.Message);
                    }
                }
            }
        }

        private void btnXoaAnh_Click(object sender, EventArgs e)
        {
            txtDuongDanAnh.Clear();
            HienThiAnh(string.Empty, UiFactory.Val(txtHoTen));
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            UiFactory.SetVal(txtMaSV, string.Empty);
            UiFactory.SetVal(txtHoTen, string.Empty);
            UiFactory.SetVal(txtDiemTB, string.Empty);
            dtpNgaySinh.Value = new DateTime(2005, 1, 1);
            cboKhoa.SelectedIndex = 0;
            radNam.Checked = true;
            foreach (Control c in pnlSoThich.Controls)
            {
                CheckBox chk = c as CheckBox;
                if (chk != null) chk.Checked = false;
            }
            txtDuongDanAnh.Clear();
            HienThiAnh(string.Empty, string.Empty);
            SinhVienHienTai = null;
            CapNhatXemTruoc();
            lblThongBao.Text = "Đã làm mới Form - nhập sinh viên mới.";
            lblThongBao.ForeColor = UiTheme.Muted;
            txtMaSV.Focus();
        }

        // =====================================================================
        // ĐÓNG FORM
        // =====================================================================
        private void btnDong_Click(object sender, EventArgs e)
        {
            Close();
        }

        // =====================================================================
        // FORM CLOSING - cảnh báo khi đóng Form giữa chừng
        // =====================================================================
        private void frmSinhVien_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ChoPhépNhap) return;
            if (MdiParent == null) return;   // Chỉ hỏi khi là form con của frmMain

            bool coDuLieu = UiFactory.Val(txtMaSV).Length > 0 || UiFactory.Val(txtHoTen).Length > 0;
            if (!coDuLieu) return;

            DialogResult r = MessageBox.Show(
                "Dữ liệu đang nhập có thể chưa được lưu.\nBạn vẫn muốn đóng Form?",
                "Xác nhận đóng", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (r == DialogResult.No)
            {
                e.Cancel = true;
            }
        }
    }
}
