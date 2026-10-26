using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Lab05_MultiForm.Helpers;

namespace Lab05_MultiForm.Forms
{
    partial class frmChiTietSinhVien
    {
        private IContainer components = null;

        private GradientPanel pnlLeft;
        private Label lblLeftTitle;
        private RoundedPictureBox picAnh;
        private Label lblHoTen;
        private Label lblBadgeMoTa;
        private BadgeLabel badgeXepLoai;

        private Label lblMa, lblDiem, lblNgaySinh, lblTuoi, lblGioiTinh, lblKhoa, lblSoThich;
        private Label lblNhanMa, lblNhanDiem, lblNhanNgay, lblNhanTuoi, lblNhanGioiTinh, lblNhanKhoa, lblNhanSoThich;
        private Label lblDiemThang;
        private Panel pnlDiemBar, pnlDiemBarFill;
        private CheckBox chkChinhSua;
        private Panel pnlSua;
        private Label lblSuaHoTen, lblSuaDiem, lblSuaNgay, lblSuaKhoa, lblSuaSoThich;
        private TextBox txtHoTen, txtDiem, txtSoThich;
        private DateTimePicker dtpNgaySinh;
        private ComboBox cboKhoa;
        private RadioButton radNam, radNu;
        private ErrorProvider errorProvider1;
        private AccentButton btnLuu, btnHuy, btnDong;
        private Label lblGoiY, lblNguon;

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

            // =========================== CỘT TRÁI (gradient) ===========================
            pnlLeft = new GradientPanel();
            pnlLeft.SetBounds(0, 0, 236, 452);
            pnlLeft.ColorFrom = Color.FromArgb(28, 24, 84);
            pnlLeft.ColorTo = Color.FromArgb(98, 72, 206);
            pnlLeft.Angle = 110f;

            lblLeftTitle = new Label();
            lblLeftTitle.Text = "CHI TIẾT SINH VIÊN";
            lblLeftTitle.Font = UiTheme.FontBold(9f);
            lblLeftTitle.ForeColor = Color.FromArgb(186, 192, 244);
            lblLeftTitle.AutoSize = true;
            lblLeftTitle.BackColor = pnlLeft.ColorFrom;
            lblLeftTitle.Location = new Point(24, 22);

            picAnh = new RoundedPictureBox();
            picAnh.SetBounds(48, 50, 140, 140);
            picAnh.CornerRadius = 70;
            picAnh.BorderColor = Color.FromArgb(140, 255, 255, 255);
            picAnh.BackFill = Color.FromArgb(36, 255, 255, 255);
            picAnh.Image = UiTheme.CreateAvatarImage(string.Empty, 140);

            lblHoTen = new Label();
            lblHoTen.Text = "—";
            lblHoTen.Font = UiTheme.FontTitle(13f);
            lblHoTen.ForeColor = Color.White;
            lblHoTen.TextAlign = ContentAlignment.MiddleCenter;
            lblHoTen.AutoEllipsis = true;
            lblHoTen.BackColor = pnlLeft.ColorFrom;
            lblHoTen.SetBounds(10, 200, 216, 30);

            lblBadgeMoTa = new Label();
            lblBadgeMoTa.Text = "Xếp loại học tập";
            lblBadgeMoTa.Font = UiTheme.FontBody(8.25f);
            lblBadgeMoTa.ForeColor = Color.FromArgb(198, 204, 242);
            lblBadgeMoTa.TextAlign = ContentAlignment.MiddleCenter;
            lblBadgeMoTa.BackColor = pnlLeft.ColorFrom;
            lblBadgeMoTa.SetBounds(10, 232, 216, 18);

            badgeXepLoai = new BadgeLabel();
            badgeXepLoai.Value = "—";
            badgeXepLoai.SetColor(UiTheme.Violet);
            badgeXepLoai.SetBounds(70, 252, 96, 24);

            pnlDiemBar = new Panel();
            pnlDiemBar.SetBounds(32, 296, 172, 8);
            pnlDiemBar.BackColor = Color.FromArgb(60, 255, 255, 255);

            pnlDiemBarFill = new Panel();
            pnlDiemBarFill.SetBounds(0, 0, 172, 8);
            pnlDiemBarFill.BackColor = Color.FromArgb(124, 231, 211);
            pnlDiemBar.Controls.Add(pnlDiemBarFill);

            lblDiemThang = new Label();
            lblDiemThang.Text = "0.0 / 10";
            lblDiemThang.Font = UiTheme.FontBold(8.5f);
            lblDiemThang.ForeColor = Color.FromArgb(124, 231, 211);
            lblDiemThang.TextAlign = ContentAlignment.MiddleCenter;
            lblDiemThang.BackColor = Color.Transparent;
            lblDiemThang.SetBounds(32, 308, 172, 18);

            lblNguon = new Label();
            lblNguon.Text = "Dữ liệu nhận qua constructor";
            lblNguon.Font = UiTheme.FontBody(8f);
            lblNguon.ForeColor = Color.FromArgb(186, 192, 240);
            lblNguon.TextAlign = ContentAlignment.MiddleCenter;
            lblNguon.AutoEllipsis = true;
            lblNguon.BackColor = pnlLeft.ColorFrom;
            lblNguon.SetBounds(8, 350, 220, 34);

            pnlLeft.Controls.AddRange(new Control[] {
                lblLeftTitle, picAnh, lblHoTen, lblBadgeMoTa, badgeXepLoai, pnlDiemBar, lblDiemThang, lblNguon });

            // =========================== CỘT PHẢI (thông tin) ===========================
            int x1 = 256, x2 = 476, y = 22;

            lblNhanMa = UiFactory.FieldLabel("Mã sinh viên");
            lblNhanMa.SetBounds(x1, y, 200, 18);
            lblMa = UiFactory.Value("—");
            lblMa.Font = UiTheme.FontTitle(13f);
            lblMa.ForeColor = UiTheme.Violet;
            lblMa.SetBounds(x1, y + 18, 200, 28);

            lblNhanDiem = UiFactory.FieldLabel("Điểm trung bình");
            lblNhanDiem.SetBounds(x2, y, 200, 18);
            lblDiem = UiFactory.Value("0.00");
            lblDiem.Font = UiTheme.FontTitle(13f);
            lblDiem.ForeColor = UiTheme.Orange;
            lblDiem.SetBounds(x2, y + 18, 200, 28);

            y = 80;
            lblNhanNgay = UiFactory.FieldLabel("Ngày sinh");
            lblNhanNgay.SetBounds(x1, y, 200, 18);
            lblNgaySinh = UiFactory.Value("—");
            lblNgaySinh.SetBounds(x1, y + 18, 130, 24);

            lblNhanTuoi = UiFactory.FieldLabel("Tuổi");
            lblNhanTuoi.SetBounds(x1 + 150, y, 60, 18);
            lblTuoi = UiFactory.Value("—");
            lblTuoi.SetBounds(x1 + 150, y + 18, 60, 24);

            lblNhanGioiTinh = UiFactory.FieldLabel("Giới tính");
            lblNhanGioiTinh.SetBounds(x2, y, 200, 18);
            lblGioiTinh = UiFactory.Value("—");
            lblGioiTinh.SetBounds(x2, y + 18, 200, 24);

            y = 134;
            lblNhanKhoa = UiFactory.FieldLabel("Khoa");
            lblNhanKhoa.SetBounds(x1, y, 420, 18);
            lblKhoa = UiFactory.Value("—");
            lblKhoa.SetBounds(x1, y + 18, 420, 24);

            y = 188;
            lblNhanSoThich = UiFactory.FieldLabel("Sở thích");
            lblNhanSoThich.SetBounds(x1, y, 420, 18);
            lblSoThich = UiFactory.Value("—");
            lblSoThich.Font = UiTheme.FontBody(9.5f);
            lblSoThich.SetBounds(x1, y + 18, 420, 24);

            // =========================== KHU CHỈNH SỬA ===========================
            chkChinhSua = new CheckBox();
            chkChinhSua.Text = "Chỉnh sửa thông tin";
            chkChinhSua.Font = UiTheme.FontBold(9f);
            chkChinhSua.ForeColor = UiTheme.Ink;
            chkChinhSua.BackColor = UiTheme.Card;
            chkChinhSua.AutoSize = true;
            chkChinhSua.Location = new Point(256, 232);
            chkChinhSua.CheckedChanged += chkChinhSua_CheckedChanged;

            pnlSua = new Panel();
            pnlSua.SetBounds(248, 258, 440, 124);
            pnlSua.BackColor = Color.FromArgb(247, 245, 255);
            pnlSua.Visible = false;

            lblSuaHoTen = UiFactory.FieldLabel("Họ và tên");
            lblSuaHoTen.SetBounds(10, 6, 260, 18);
            txtHoTen = UiFactory.TextBox("txtHoTen", "Họ và tên", false);
            txtHoTen.SetBounds(10, 24, 260, 24);

            lblSuaDiem = UiFactory.FieldLabel("Điểm");
            lblSuaDiem.SetBounds(284, 6, 70, 18);
            txtDiem = UiFactory.TextBox("txtDiem", "0 - 10", false);
            txtDiem.SetBounds(284, 24, 70, 24);
            txtDiem.KeyPress += txtDiem_KeyPress;

            lblSuaNgay = UiFactory.FieldLabel("Ngày sinh");
            lblSuaNgay.SetBounds(10, 56, 120, 18);
            dtpNgaySinh = UiFactory.DatePicker("dtpNgaySinh");
            dtpNgaySinh.SetBounds(10, 74, 120, 24);

            lblSuaKhoa = UiFactory.FieldLabel("Khoa");
            lblSuaKhoa.SetBounds(150, 56, 140, 18);
            cboKhoa = UiFactory.ComboBox("cboKhoa");
            cboKhoa.SetBounds(150, 76, 140, 22);

            lblSuaSoThich = UiFactory.FieldLabel("Sở thích");
            lblSuaSoThich.SetBounds(304, 56, 120, 18);
            txtSoThich = UiFactory.TextBox("txtSoThich", "Cách nhau bằng dấu phẩy", false);
            txtSoThich.SetBounds(304, 74, 120, 24);

            radNam = new RadioButton();
            radNam.Text = "Nam";
            radNam.Font = UiTheme.FontBody(8.5f);
            radNam.AutoSize = true;
            radNam.Checked = true;
            radNam.BackColor = Color.Transparent;
            radNam.Location = new Point(362, 6);
            radNu = new RadioButton();
            radNu.Text = "Nữ";
            radNu.Font = UiTheme.FontBody(8.5f);
            radNu.AutoSize = true;
            radNu.BackColor = Color.Transparent;
            radNu.Location = new Point(362, 26);

            pnlSua.Controls.AddRange(new Control[] {
                lblSuaHoTen, txtHoTen, lblSuaDiem, txtDiem, radNam, radNu,
                lblSuaNgay, dtpNgaySinh, lblSuaKhoa, cboKhoa, lblSuaSoThich, txtSoThich });

            lblGoiY = new Label();
            lblGoiY.Text = "Bật \"Chỉnh sửa\" để sửa họ tên, điểm, khoa rồi bấm Lưu để trả dữ liệu về Form gọi.";
            lblGoiY.Font = UiTheme.FontBody(8.25f);
            lblGoiY.ForeColor = UiTheme.Muted;
            lblGoiY.AutoSize = false;
            lblGoiY.BackColor = Color.Transparent;
            lblGoiY.SetBounds(256, 300, 442, 40);

            btnLuu = new AccentButton();
            btnLuu.Name = "btnLuu";
            btnLuu.Text = "Lưu và trả kết quả";
            btnLuu.BaseColor = UiTheme.Green;
            btnLuu.Visible = false;
            btnLuu.SetBounds(256, 402, 190, 40);
            btnLuu.Click += btnLuu_Click;

            btnHuy = new AccentButton();
            btnHuy.Name = "btnHuy";
            btnHuy.Text = "Hủy";
            btnHuy.BaseColor = UiTheme.Slate;
            btnHuy.OutlineOnly = true;
            btnHuy.Visible = false;
            btnHuy.SetBounds(456, 402, 100, 40);
            btnHuy.Click += btnHuy_Click;

            btnDong = new AccentButton();
            btnDong.Name = "btnDong";
            btnDong.Text = "Đóng";
            btnDong.BaseColor = UiTheme.Indigo;
            btnDong.SetBounds(566, 402, 130, 40);
            btnDong.Click += btnDong_Click;

            // Ba nút Lưu/Hủy/Đóng nằm cùng panel với khu chỉnh sửa nên không cần thêm lại

            // =========================== FORM ===========================
            AutoScaleMode = AutoScaleMode.None;
            Text = "Chi tiết sinh viên";
            ClientSize = new Size(720, 452);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;

            Panel pnlRight = new Panel();
            pnlRight.SetBounds(236, 0, 484, 452);
            pnlRight.BackColor = UiTheme.Card;
            pnlRight.Controls.AddRange(new Control[] {
                lblNhanMa, lblMa, lblNhanDiem, lblDiem,
                lblNhanNgay, lblNgaySinh, lblNhanTuoi, lblTuoi, lblNhanGioiTinh, lblGioiTinh,
                lblNhanKhoa, lblKhoa, lblNhanSoThich, lblSoThich,
                chkChinhSua, pnlSua, lblGoiY, btnLuu, btnHuy, btnDong });
            pnlRight.Paint += PnlRight_Paint;

            Controls.Add(pnlRight);
            Controls.Add(pnlLeft);

            Load += frmChiTietSinhVien_Load;
            // Cháº¿ Ä‘á»™ chá»¥p áº£nh bÃ¡o cÃ¡o (--screenshots): tá»± Ä‘Ã³ng Form Ä‘á»ƒ bá»™ chá»¥p cháº¡y tiáº¿p ká»‹ch báº£n sau.
            if (CheDoChupAnh.DangChup) { Shown += delegate { BeginInvoke((MethodInvoker)Close); }; }
            FormClosing += frmChiTietSinhVien_FormClosing;
        }

        private void PnlRight_Paint(object sender, PaintEventArgs e)
        {
            using (var pen = new Pen(UiTheme.Border))
            {
                e.Graphics.DrawLine(pen, 0, 0, 0, ((Control)sender).Height);
            }
            using (var brush = new SolidBrush(UiTheme.Tint(UiTheme.Violet, 0.08)))
            {
                e.Graphics.FillRectangle(brush, 0, 224, ((Control)sender).Width, 1);
            }
        }
    }
}
