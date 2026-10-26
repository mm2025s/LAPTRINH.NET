using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Lab05_MultiForm.Helpers;

namespace Lab05_MultiForm.Forms
{
    partial class frmDangNhap
    {
        private IContainer components = null;

        private InfoStrip infoStrip;
        private Label lblTieuDe, lblMoTa;
        private Label lblNhanTen, lblNhanMatKhau;
        private TextBox txtTenDangNhap, txtMatKhau;
        private AccentButton btnToggle;
        private CheckBox chkHienMatKhau;
        private ErrorProvider errorProvider1;
        private Label lblTrangThai, lblGoiY;
        private AccentButton btnDangNhap, btnHuy;

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

            infoStrip = new InfoStrip();
            infoStrip.Dock = DockStyle.Top;
            infoStrip.Height = 86;
            infoStrip.Accent = UiTheme.Blue;
            infoStrip.ChipText = "6";
            infoStrip.Title = "Đăng nhập";
            infoStrip.Subtitle = "Sai 3 lần => khóa nút đăng nhập trong 30 giây (đếm ngược từng giây trên Form)";
            infoStrip.RightTop = "Tài khoản mẫu: admin / 123456";
            infoStrip.RightBottom = "Enter ở ô tài khoản => chuyển sang ô mật khẩu";

            lblTieuDe = new Label();
            lblTieuDe.Text = "Chào mừng trở lại";
            lblTieuDe.Font = UiTheme.FontTitle(15f);
            lblTieuDe.ForeColor = UiTheme.Ink;
            lblTieuDe.AutoSize = true;
            lblTieuDe.BackColor = Color.Transparent;
            lblTieuDe.Location = new Point(24, 102);

            lblMoTa = new Label();
            lblMoTa.Text = "Đăng nhập bằng tài khoản mẫu để xem luồng giới hạn lần thử.";
            lblMoTa.Font = UiTheme.FontBody(8.75f);
            lblMoTa.ForeColor = UiTheme.Muted;
            lblMoTa.AutoSize = true;
            lblMoTa.BackColor = Color.Transparent;
            lblMoTa.Location = new Point(26, 132);

            lblNhanTen = UiFactory.FieldLabel("Tên đăng nhập");
            lblNhanTen.SetBounds(24, 168, 180, 18);
            txtTenDangNhap = UiFactory.TextBox("txtTenDangNhap", "admin", false);
            txtTenDangNhap.SetBounds(24, 186, 366, 28);
            txtTenDangNhap.KeyDown += txtTenDangNhap_KeyDown;

            lblNhanMatKhau = UiFactory.FieldLabel("Mật khẩu");
            lblNhanMatKhau.SetBounds(24, 222, 180, 18);
            txtMatKhau = UiFactory.TextBox("txtMatKhau", "", false);
            txtMatKhau.SetBounds(24, 240, 252, 28);
            txtMatKhau.UseSystemPasswordChar = true;
            txtMatKhau.KeyDown += txtMatKhau_KeyDown;

            btnToggle = new AccentButton();
            btnToggle.Text = "Hiện";
            btnToggle.BaseColor = UiTheme.Blue;
            btnToggle.OutlineOnly = true;
            btnToggle.SetBounds(284, 240, 70, 28);
            btnToggle.Click += btnToggle_Click;

            chkHienMatKhau = new CheckBox();
            chkHienMatKhau.Text = "Hiển thị mật khẩu";
            chkHienMatKhau.Font = UiTheme.FontBody(8.75f);
            chkHienMatKhau.ForeColor = UiTheme.InkSoft;
            chkHienMatKhau.AutoSize = true;
            chkHienMatKhau.BackColor = Color.Transparent;
            chkHienMatKhau.Location = new Point(26, 278);
            chkHienMatKhau.CheckedChanged += chkHienMatKhau_CheckedChanged;

            lblTrangThai = new Label();
            lblTrangThai.Text = "Nhập tên đăng nhập và mật khẩu để tiếp tục.";
            lblTrangThai.Font = UiTheme.FontBold(8.75f);
            lblTrangThai.ForeColor = UiTheme.Muted;
            lblTrangThai.AutoSize = false;
            lblTrangThai.AutoEllipsis = true;
            lblTrangThai.BackColor = Color.Transparent;
            lblTrangThai.SetBounds(24, 306, 366, 20);

            lblGoiY = new Label();
            lblGoiY.Text = "Tài khoản mẫu: admin / 123456.";
            lblGoiY.Font = UiTheme.FontBody(8.25f);
            lblGoiY.ForeColor = UiTheme.Muted;
            lblGoiY.AutoSize = false;
            lblGoiY.BackColor = Color.Transparent;
            lblGoiY.SetBounds(24, 328, 366, 32);

            btnDangNhap = new AccentButton();
            btnDangNhap.Name = "btnDangNhap";
            btnDangNhap.Text = "Đăng nhập";
            btnDangNhap.BaseColor = UiTheme.Blue;
            btnDangNhap.SetBounds(24, 370, 246, 42);
            btnDangNhap.Click += btnDangNhap_Click;

            btnHuy = new AccentButton();
            btnHuy.Name = "btnHuy";
            btnHuy.Text = "Hủy";
            btnHuy.BaseColor = UiTheme.Slate;
            btnHuy.OutlineOnly = true;
            btnHuy.SetBounds(290, 370, 100, 42);
            btnHuy.Click += btnHuy_Click;

            AutoScaleMode = AutoScaleMode.None;
            Text = "Đăng nhập";
            ClientSize = new Size(416, 434);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            CancelButton = btnHuy;

            Controls.Add(infoStrip);
            Controls.AddRange(new Control[] {
                lblTieuDe, lblMoTa, lblNhanTen, txtTenDangNhap, lblNhanMatKhau, txtMatKhau,
                btnToggle, chkHienMatKhau, lblTrangThai, lblGoiY, btnDangNhap, btnHuy });

            Load += frmDangNhap_Load;
            // Cháº¿ Ä‘á»™ chá»¥p áº£nh bÃ¡o cÃ¡o (--screenshots): tá»± Ä‘Ã³ng Form Ä‘á»ƒ bá»™ chá»¥p cháº¡y tiáº¿p ká»‹ch báº£n sau.
            if (CheDoChupAnh.DangChup) { Shown += delegate { BeginInvoke((MethodInvoker)Close); }; }
            FormClosing += frmDangNhap_FormClosing;
        }
    }
}
