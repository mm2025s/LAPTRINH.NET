using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Lab05_MultiForm.Helpers;

namespace Lab05_MultiForm.Forms
{
    partial class frmSinhVien
    {
        private IContainer components = null;

        private InfoStrip infoStrip;
        private GradientPanel pnlLeft;
        private Label lblLeftTieuDe;
        private Label lblTenFileAnh;
        private RoundedPictureBox picAnh;
        private AccentButton btnChonAnh, btnXoaAnh;
        private Label lblLeftGhiChu;

        private Label lblMathieuDe1, lblMoTa1, lblMathieuDe2, lblMoTa2;
        private Label lblMathieuDe3, lblMathieuDe4, lblMoTa4;
        private Label lblNhanMa, lblNhanTen, lblNhanGioiTinh, lblNhanKhoa, lblNhanNgay, lblNhanDiem, lblNhanSoThich;
        private TextBox txtMaSV, txtHoTen, txtDiemTB, txtDuongDanAnh;
        private DateTimePicker dtpNgaySinh;
        private ComboBox cboKhoa;
        private RadioButton radNam, radNu;
        private Panel pnlSoThich;
        private ErrorProvider errorProvider1;

        private Label lblXemTruoc, lblTrangThai, lblTuoi, lblSoThich, lblThongBao, lblCheDo;
        private BadgeLabel badgeDiem, badgeTong;
        private AccentButton btnLuu, btnLamMoi, btnDong;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new Container();
            errorProvider1 = new ErrorProvider(components);
            errorProvider1.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            errorProvider1.ContainerControl = this;

            // ============================ HEADER ============================
            infoStrip = new InfoStrip();
            infoStrip.Dock = DockStyle.Top;
            infoStrip.Height = 90;
            infoStrip.Accent = UiTheme.Violet;
            infoStrip.ChipText = "1-4";
            infoStrip.Title = "Nhập thông tin sinh viên";
            infoStrip.Subtitle = "ErrorProvider · KeyPress giới hạn điểm · TextChanged cập nhật tức thời · Truyền dữ liệu sang Form chi tiết";
            infoStrip.RightTop = "IT3212 · LAB 5";
            infoStrip.RightBottom = "Mã, Họ tên và Điểm là bắt buộc";

            // ============================ PANEL TRÁI ============================
            pnlLeft = new GradientPanel();
            pnlLeft.Dock = DockStyle.Left;
            pnlLeft.Width = 244;
            pnlLeft.ColorFrom = Color.FromArgb(30, 26, 88);
            pnlLeft.ColorTo = Color.FromArgb(102, 70, 208);
            pnlLeft.Angle = 100f;

            lblLeftTieuDe = new Label();
            lblLeftTieuDe.Text = "ẢNH ĐẠI DIỆN";
            lblLeftTieuDe.Font = UiTheme.FontBold(8.5f);
            lblLeftTieuDe.ForeColor = Color.FromArgb(184, 190, 244);
            lblLeftTieuDe.AutoSize = true;
            lblLeftTieuDe.BackColor = Color.Transparent;
            lblLeftTieuDe.Location = new Point(26, 28);

            picAnh = new RoundedPictureBox();
            picAnh.SetBounds(26, 52, 152, 152);
            picAnh.CornerRadius = 76;
            picAnh.BorderColor = Color.FromArgb(130, 255, 255, 255);
            picAnh.BackFill = Color.FromArgb(36, 255, 255, 255);
            picAnh.Image = UiTheme.CreateAvatarImage(string.Empty, 152);

            lblTenFileAnh = new Label();
            lblTenFileAnh.Text = "Ảnh mặc định (tự vẽ)";
            lblTenFileAnh.Font = UiTheme.FontBody(8f);
            lblTenFileAnh.ForeColor = Color.FromArgb(204, 210, 244);
            lblTenFileAnh.TextAlign = ContentAlignment.MiddleCenter;
            lblTenFileAnh.AutoEllipsis = true;
            lblTenFileAnh.BackColor = Color.Transparent;
            lblTenFileAnh.SetBounds(6, 208, 192, 20);

            btnChonAnh = new AccentButton();
            btnChonAnh.Name = "btnChonAnh";
            btnChonAnh.Text = "Chọn ảnh...";
            btnChonAnh.BaseColor = Color.FromArgb(146, 124, 255);
            btnChonAnh.SetBounds(26, 236, 152, 34);
            btnChonAnh.Click += btnChonAnh_Click;

            btnXoaAnh = new AccentButton();
            btnXoaAnh.Name = "btnXoaAnh";
            btnXoaAnh.Text = "Xóa ảnh";
            btnXoaAnh.BaseColor = Color.FromArgb(108, 116, 196);
            btnXoaAnh.OutlineOnly = true;
            btnXoaAnh.SetBounds(26, 276, 152, 32);
            btnXoaAnh.Click += btnXoaAnh_Click;

            lblLeftGhiChu = new Label();
            lblLeftGhiChu.Text = "Ảnh và sở thích là phần tùy chọn.\nChỉ cần Mã, Họ tên và Điểm hợp lệ là có thể bấm Lưu.";
            lblLeftGhiChu.Font = UiTheme.FontBody(8.25f);
            lblLeftGhiChu.ForeColor = Color.FromArgb(196, 202, 240);
            lblLeftGhiChu.BackColor = Color.Transparent;
            lblLeftGhiChu.SetBounds(26, 322, 180, 56);

            txtDuongDanAnh = new TextBox();
            txtDuongDanAnh.Visible = false;
            txtDuongDanAnh.Name = "txtDuongDanAnh";

            pnlLeft.Controls.AddRange(new Control[] {
                lblLeftTieuDe, picAnh, lblTenFileAnh, btnChonAnh, btnXoaAnh, lblLeftGhiChu, txtDuongDanAnh });

            // ============================ MỤC 1 ============================
            lblMathieuDe1 = Muc("Bài 1 · Kiểm tra bắt buộc (ErrorProvider)", 14, UiTheme.Violet);
            lblMoTa1 = Phu("Lỗi hiển thị ngay cạnh control thông qua phương thức ValidateInput().", 36);

            lblNhanMa = UiFactory.FieldLabel("Mã sinh viên");
            lblNhanMa.SetBounds(14, 62, 140, 18);
            txtMaSV = UiFactory.TextBox("txtMaSV", "VD: SV10", false);
            txtMaSV.SetBounds(14, 80, 138, 26);

            lblNhanTen = UiFactory.FieldLabel("Họ và tên");
            lblNhanTen.SetBounds(166, 62, 290, 18);
            txtHoTen = UiFactory.TextBox("txtHoTen", "VD: Nguyễn Văn An", false);
            txtHoTen.SetBounds(166, 80, 304, 26);

            lblNhanGioiTinh = UiFactory.FieldLabel("Giới tính");
            lblNhanGioiTinh.SetBounds(14, 114, 100, 18);
            radNam = new RadioButton();
            radNam.Text = "Nam";
            radNam.Font = UiTheme.FontBody(9f);
            radNam.AutoSize = true;
            radNam.Checked = true;
            radNam.BackColor = Color.Transparent;
            radNam.Location = new Point(16, 136);
            radNam.CheckedChanged += BatBuoc_CheckedChanged;
            radNu = new RadioButton();
            radNu.Text = "Nữ";
            radNu.Font = UiTheme.FontBody(9f);
            radNu.AutoSize = true;
            radNu.BackColor = Color.Transparent;
            radNu.Location = new Point(84, 136);
            radNu.CheckedChanged += BatBuoc_CheckedChanged;

            lblNhanNgay = UiFactory.FieldLabel("Ngày sinh");
            lblNhanNgay.SetBounds(166, 114, 120, 18);
            dtpNgaySinh = UiFactory.DatePicker("dtpNgaySinh");
            dtpNgaySinh.SetBounds(166, 136, 128, 24);
            dtpNgaySinh.Value = new DateTime(2005, 1, 1);
            dtpNgaySinh.ValueChanged += dtpNgaySinh_ValueChanged;

            lblNhanDiem = UiFactory.FieldLabel("Điểm trung bình");
            lblNhanDiem.SetBounds(306, 114, 100, 18);
            txtDiemTB = UiFactory.TextBox("txtDiemTB", "0.0 - 10", false);
            txtDiemTB.SetBounds(306, 136, 80, 26);
            txtDiemTB.KeyPress += txtDiemTB_KeyPress;

            lblNhanKhoa = UiFactory.FieldLabel("Khoa");
            lblNhanKhoa.SetBounds(400, 114, 80, 18);
            cboKhoa = UiFactory.ComboBox("cboKhoa");
            cboKhoa.SetBounds(398, 138, 92, 22);
            cboKhoa.SelectedIndexChanged += cboKhoa_SelectedIndexChanged;

            // ============================ MỤC 2 + 3 ============================
            lblMathieuDe2 = Muc("Bài 2 · Giới hạn ký tự nhập điểm (KeyPress)", 172, UiTheme.Teal);
            lblMoTa2 = Phu("Chỉ nhận chữ số và MỘT dấu thập phân; miền [0, 10] kiểm tra tiếp khi lưu. Thử gõ 12.345 hoặc 8..5.", 194);

            lblNhanSoThich = UiFactory.FieldLabel("Sở thích");
            lblNhanSoThich.SetBounds(14, 216, 140, 18);
            pnlSoThich = new Panel();
            pnlSoThich.BackColor = Color.Transparent;
            pnlSoThich.SetBounds(14, 236, 476, 56);

            lblCheDo = new Label();
            lblCheDo.Text = "CHẾ ĐỘ: NHẬP DỮ LIỆU";
            lblCheDo.Font = UiTheme.FontBody(8f);
            lblCheDo.ForeColor = UiTheme.Teal;
            lblCheDo.AutoSize = true;
            lblCheDo.BackColor = Color.Transparent;
            lblCheDo.Location = new Point(14, 298);

            lblMathieuDe3 = Muc("Bài 3 · Cập nhật trạng thái theo thời gian thực (TextChanged)", 322, UiTheme.Blue);

            CardPanel cardXem = new CardPanel();
            cardXem.SetBounds(14, 344, 210, 172);

            Label lblXemTitle = new Label();
            lblXemTitle.Text = "Xem trước dữ liệu";
            lblXemTitle.Font = UiTheme.FontBold(9f);
            lblXemTitle.ForeColor = UiTheme.Ink;
            lblXemTitle.AutoSize = true;
            lblXemTitle.BackColor = Color.Transparent;
            lblXemTitle.Location = new Point(14, 10);

            badgeTong = new BadgeLabel();
            badgeTong.Value = "CHƯA NHẬP";
            badgeTong.SetColor(UiTheme.Slate);
            badgeTong.SetBounds(96, 8, 100, 22);

            lblXemTruoc = new Label();
            lblXemTruoc.Text = "…  -  …";
            lblXemTruoc.Font = UiTheme.FontBold(10f);
            lblXemTruoc.ForeColor = UiTheme.Ink;
            lblXemTruoc.AutoSize = false;
            lblXemTruoc.AutoEllipsis = true;
            lblXemTruoc.BackColor = Color.Transparent;
            lblXemTruoc.SetBounds(14, 38, 182, 22);

            lblTrangThai = new Label();
            lblTrangThai.Text = "Chưa nhập điểm trung bình";
            lblTrangThai.Font = UiTheme.FontBody(8.25f);
            lblTrangThai.ForeColor = UiTheme.Muted;
            lblTrangThai.AutoSize = false;
            lblTrangThai.BackColor = Color.Transparent;
            lblTrangThai.SetBounds(14, 64, 186, 34);

            badgeDiem = new BadgeLabel();
            badgeDiem.Value = "CHƯA NHẬP";
            badgeDiem.SetColor(UiTheme.Slate);
            badgeDiem.SetBounds(14, 104, 104, 22);

            lblTuoi = new Label();
            lblTuoi.Text = "Tuổi: …";
            lblTuoi.Font = UiTheme.FontBody(8.5f);
            lblTuoi.ForeColor = UiTheme.InkSoft;
            lblTuoi.AutoSize = true;
            lblTuoi.BackColor = Color.Transparent;
            lblTuoi.Location = new Point(14, 136);

            cardXem.Controls.AddRange(new Control[] { lblXemTitle, badgeTong, lblXemTruoc, lblTrangThai, badgeDiem, lblTuoi });

            CardPanel cardSoThich = new CardPanel();
            cardSoThich.SetBounds(234, 344, 256, 78);
            Label lblSTTitle = new Label();
            lblSTTitle.Text = "Sở thích đã chọn";
            lblSTTitle.Font = UiTheme.FontBold(9f);
            lblSTTitle.ForeColor = UiTheme.Ink;
            lblSTTitle.AutoSize = true;
            lblSTTitle.BackColor = Color.Transparent;
            lblSTTitle.Location = new Point(14, 8);
            lblSoThich = new Label();
            lblSoThich.Text = "Chưa chọn sở thích";
            lblSoThich.Font = UiTheme.FontBody(8.5f);
            lblSoThich.ForeColor = UiTheme.InkSoft;
            lblSoThich.AutoSize = false;
            lblSoThich.BackColor = Color.Transparent;
            lblSoThich.SetBounds(14, 30, 228, 40);
            cardSoThich.Controls.AddRange(new Control[] { lblSTTitle, lblSoThich });

            lblThongBao = new Label();
            lblThongBao.Text = "Nhập đủ Mã sinh viên, Họ tên và Điểm trong khoảng 0 - 10.";
            lblThongBao.Font = UiTheme.FontBody(8.25f);
            lblThongBao.ForeColor = UiTheme.Muted;
            lblThongBao.AutoSize = false;
            lblThongBao.BackColor = Color.Transparent;
            lblThongBao.SetBounds(234, 426, 256, 32);

            // ============================ MỤC 4 ============================
            lblMathieuDe4 = Muc("Bài 4 · Truyền dữ liệu sang Form chi tiết", 524, UiTheme.Orange);
            lblMoTa4 = Phu("frmChiTietSinhVien nhận đối tượng qua constructor; Form chi tiết trả kết quả qua thuộc tính SinhVien.", 546);

            btnLuu = new AccentButton();
            btnLuu.Name = "btnLuu";
            btnLuu.Text = "Lưu sinh viên";
            btnLuu.BaseColor = UiTheme.Green;
            btnLuu.SetBounds(14, 572, 140, 42);
            btnLuu.Click += btnLuu_Click;

            btnLamMoi = new AccentButton();
            btnLamMoi.Name = "btnLamMoi";
            btnLamMoi.Text = "Làm mới";
            btnLamMoi.BaseColor = UiTheme.Slate;
            btnLamMoi.OutlineOnly = true;
            btnLamMoi.SetBounds(164, 572, 120, 42);
            btnLamMoi.Click += btnLamMoi_Click;

            btnDong = new AccentButton();
            btnDong.Name = "btnDong";
            btnDong.Text = "Đóng";
            btnDong.BaseColor = UiTheme.Slate;
            btnDong.SetBounds(294, 572, 196, 42);
            btnDong.Click += btnDong_Click;

            // ============================ FORM ============================
            AutoScaleMode = AutoScaleMode.None;
            Text = "Nhập thông tin sinh viên";
            ClientSize = new Size(740, 700);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;

            Controls.Add(infoStrip);
            Controls.Add(pnlLeft);
            Controls.AddRange(new Control[] {
                lblMathieuDe1, lblMoTa1, lblNhanMa, txtMaSV, lblNhanTen, txtHoTen,
                lblNhanGioiTinh, radNam, radNu, lblNhanNgay, dtpNgaySinh, lblNhanDiem, txtDiemTB,
                lblNhanKhoa, cboKhoa,
                lblMathieuDe2, lblMoTa2, lblNhanSoThich, pnlSoThich, lblCheDo,
                lblMathieuDe3, cardXem, cardSoThich, lblThongBao,
                lblMathieuDe4, lblMoTa4, btnLuu, btnLamMoi, btnDong });

            txtMaSV.TextChanged += Input_TextChanged;
            txtHoTen.TextChanged += Input_TextChanged;
            txtDiemTB.TextChanged += Input_TextChanged;

            Load += frmSinhVien_Load;
            // Cháº¿ Ä‘á»™ chá»¥p áº£nh bÃ¡o cÃ¡o (--screenshots): tá»± Ä‘Ã³ng Form Ä‘á»ƒ bá»™ chá»¥p cháº¡y tiáº¿p ká»‹ch báº£n sau.
            if (CheDoChupAnh.DangChup) { Shown += delegate { BeginInvoke((MethodInvoker)Close); }; }
            FormClosing += frmSinhVien_FormClosing;
        }

        private Label Muc(string text, int y, Color accent)
        {
            var l = new Label();
            l.Text = text;
            l.Font = UiTheme.FontBold(9f);
            l.ForeColor = accent;
            l.AutoSize = true;
            l.BackColor = Color.Transparent;
            l.Location = new Point(12, y);
            return l;
        }

        private Label Phu(string text, int y)
        {
            var l = new Label();
            l.Text = text;
            l.Font = UiTheme.FontBody(8f);
            l.ForeColor = UiTheme.Muted;
            l.AutoSize = true;
            l.BackColor = Color.Transparent;
            l.Location = new Point(12, y);
            return l;
        }
    }
}
