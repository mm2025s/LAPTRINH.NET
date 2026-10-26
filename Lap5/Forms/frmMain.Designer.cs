using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Lab05_MultiForm.Helpers;

namespace Lab05_MultiForm.Forms
{
    partial class frmMain
    {
        private IContainer components = null;

        private MenuStrip menuChinh;
        private ToolStripMenuItem menuHeThong;
        private ToolStripMenuItem menuDangNhap;
        private ToolStripMenuItem menuThoat;
        private ToolStripMenuItem menuDanhMuc;
        private ToolStripMenuItem menuChonKhoa;
        private ToolStripMenuItem menuNghiepVu;
        private ToolStripMenuItem menuSinhVien;
        private ToolStripMenuItem menuDanhSach;
        private ToolStripMenuItem menuTimKiem;
        private ToolStripMenuItem menuDongTatCa;
        private ToolStripMenuItem menuTroGiup;
        private ToolStripMenuItem menuKiemTraNhanh;
        private ToolStripMenuItem menuGioiThieu;
        private ToolStripMenuItem menuPhienBan;

        private StatusStrip statusChinh;
        private ToolStripStatusLabel tsslTrangThai;
        private ToolStripStatusLabel tsslSpring;
        private ToolStripStatusLabel tsslSoSV;
        private ToolStripStatusLabel tsslNguoiDung;
        private ToolStripStatusLabel tsslSoLanMo;
        private ToolStripStatusLabel tsslDongHo;

        private Panel pnlHero;
        private Label lblChao;
        private Label lblMoTa;
        private Label lblBadge;

        private CardPanel card1, card2, card3, card4;
        private Label lblCap1, lblCap2, lblCap3, lblCap4;
        private Label lblStatTong, lblStatDiemTB, lblStatCaoNhat, lblStatGioi;

        private CardPanel cardNhanh;
        private Label lblTieuDeNhanh, lblPhuNhanh, lblKetQuaNhanh;
        private TextBox txtMaNhanh, txtTenNhanh, txtDiemNhanh;
        private ComboBox cboKhoaNhanh;
        private AccentButton btnKiemTraNhanh;

        private MdiClient mdiClient;
        private Panel pnlDashboard;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new Container();

            // ============================ MENU ============================
            menuChinh = new MenuStrip();
            menuHeThong = new ToolStripMenuItem("Hệ thống");
            menuDangNhap = new ToolStripMenuItem("Đăng nhập...");
            menuThoat = new ToolStripMenuItem("Thoát ứng dụng");
            menuDanhMuc = new ToolStripMenuItem("Danh mục");
            menuChonKhoa = new ToolStripMenuItem("Chọn khoa...");
            menuNghiepVu = new ToolStripMenuItem("Nghiệp vụ");
            menuSinhVien = new ToolStripMenuItem("Nhập thông tin sinh viên");
            menuDanhSach = new ToolStripMenuItem("Danh sách sinh viên");
            menuTimKiem = new ToolStripMenuItem("Tìm kiếm sinh viên...");
            menuDongTatCa = new ToolStripMenuItem("Đóng tất cả form con");
            menuTroGiup = new ToolStripMenuItem("Trợ giúp");
            menuKiemTraNhanh = new ToolStripMenuItem("Kiểm tra dữ liệu (hàm dùng chung)");
            menuGioiThieu = new ToolStripMenuItem("Giới thiệu chương trình");
            menuPhienBan = new ToolStripMenuItem("Phiên bản 5.0");

            menuChinh.Font = UiTheme.FontBody(9.75f);
            menuChinh.BackColor = Color.White;
            menuChinh.ForeColor = UiTheme.Ink;
            menuChinh.Padding = new Padding(14, 5, 0, 5);
            menuChinh.Renderer = new ToolStripProfessionalRenderer(new MenuColorTable());
            menuChinh.Items.AddRange(new ToolStripItem[] {
                menuHeThong, menuDanhMuc, menuNghiepVu, menuTroGiup, menuPhienBan });

            menuPhienBan.Enabled = false;
            menuPhienBan.ForeColor = UiTheme.Muted;

            menuHeThong.DropDownItems.AddRange(new ToolStripItem[] { menuDangNhap, new ToolStripSeparator(), menuThoat });
            menuDanhMuc.DropDownItems.AddRange(new ToolStripItem[] { menuChonKhoa });
            menuNghiepVu.DropDownItems.AddRange(new ToolStripItem[] {
                menuSinhVien, menuDanhSach, menuTimKiem, new ToolStripSeparator(), menuDongTatCa });
            menuTroGiup.DropDownItems.AddRange(new ToolStripItem[] { menuKiemTraNhanh, menuGioiThieu });

            menuSinhVien.ShortcutKeys = Keys.Control | Keys.N;
            menuDanhSach.ShortcutKeys = Keys.Control | Keys.L;
            menuTimKiem.ShortcutKeys = Keys.Control | Keys.F;
            menuKiemTraNhanh.ShortcutKeys = Keys.Control | Keys.K;
            menuGioiThieu.ShortcutKeys = Keys.F1;
            menuDangNhap.ShortcutKeys = Keys.Control | Keys.O;

            menuSinhVien.Click += mnuSinhVien_Click;
            menuDanhSach.Click += mnuDanhSach_Click;
            menuDongTatCa.Click += mnuDongTatCa_Click;
            menuTimKiem.Click += mnuTimKiem_Click;
            menuChonKhoa.Click += mnuChonKhoa_Click;
            menuDangNhap.Click += mnuDangNhap_Click;
            menuGioiThieu.Click += mnuGioiThieu_Click;
            menuKiemTraNhanh.Click += mnuKiemTraNhanh_Click;
            menuThoat.Click += mnuThoat_Click;

            // ============================ STATUS STRIP ============================
            statusChinh = new StatusStrip();
            statusChinh.BackColor = Color.FromArgb(24, 26, 40);
            statusChinh.ForeColor = Color.White;
            statusChinh.Font = UiTheme.FontBody(8.75f);
            statusChinh.SizingGrip = false;
            statusChinh.Padding = new Padding(10, 3, 10, 3);

            tsslTrangThai = new ToolStripStatusLabel(" Sẵn sàng");
            tsslTrangThai.ForeColor = Color.FromArgb(214, 218, 238);
            tsslSpring = new ToolStripStatusLabel();
            tsslSpring.Spring = true;
            tsslSoSV = new ToolStripStatusLabel(" 0 sinh viên");
            tsslNguoiDung = new ToolStripStatusLabel(" Khách");
            tsslSoLanMo = new ToolStripStatusLabel(" Đã mở 0 form");
            tsslDongHo = new ToolStripStatusLabel();

            ToolStripStatusLabel[] info = { tsslSoSV, tsslNguoiDung, tsslSoLanMo };
            foreach (ToolStripStatusLabel l in info)
            {
                l.ForeColor = Color.FromArgb(158, 166, 200);
                l.BorderSides = ToolStripStatusLabelBorderSides.Left;
                l.BorderStyle = Border3DStyle.Etched;
            }
            tsslDongHo.ForeColor = Color.FromArgb(124, 231, 211);
            tsslDongHo.BorderSides = ToolStripStatusLabelBorderSides.Left;

            statusChinh.Items.AddRange(new ToolStripItem[] {
                tsslTrangThai, tsslSpring, tsslSoSV, tsslNguoiDung, tsslSoLanMo, tsslDongHo });

            // ============================ HERO ============================
            pnlHero = new Panel();
            pnlHero.Dock = DockStyle.Top;
            pnlHero.Height = 120;
            pnlHero.BackColor = Color.Transparent;
            pnlHero.Paint += pnlHero_Paint;

            lblChao = new Label();
            lblChao.Text = "Lab 5 - Xử lý sự kiện & ứng dụng đa Form";
            lblChao.Font = UiTheme.FontTitle(19f);
            lblChao.ForeColor = Color.White;
            lblChao.AutoSize = true;
            lblChao.BackColor = Color.Transparent;
            lblChao.Location = new Point(30, 22);

            lblMoTa = new Label();
            lblMoTa.Text = "IT3212 - Lập trình .NET  |  Khoa Công nghệ Thông tin - ĐH Công nghệ Đông Á";
            lblMoTa.Font = UiTheme.FontBody(9.75f);
            lblMoTa.ForeColor = Color.FromArgb(208, 212, 238);
            lblMoTa.AutoSize = true;
            lblMoTa.BackColor = Color.Transparent;
            lblMoTa.Location = new Point(32, 58);

            lblBadge = new Label();
            lblBadge.Text = "ErrorProvider    MenuStrip    StatusStrip    FormClosing    Hộp thoại tìm kiếm    Chọn khoa";
            lblBadge.Font = UiTheme.FontBody(8.5f);
            lblBadge.ForeColor = Color.FromArgb(146, 255, 243);
            lblBadge.AutoSize = true;
            lblBadge.BackColor = Color.Transparent;
            lblBadge.Location = new Point(32, 86);

            pnlHero.Controls.AddRange(new Control[] { lblChao, lblMoTa, lblBadge });

            // ============================ STAT CARDS ============================
            card1 = TaoCard("Tổng sinh viên", "0", "trong danh sách mẫu", UiTheme.Violet, out lblCap1, out lblStatTong);
            card2 = TaoCard("Điểm trung bình", "0.00", "toàn bộ sinh viên", UiTheme.Teal, out lblCap2, out lblStatDiemTB);
            card3 = TaoCard("Điểm cao nhất", "0.00", "thang điểm 10", UiTheme.Orange, out lblCap3, out lblStatCaoNhat);
            card4 = TaoCard("Từ 8.0 trở lên", "0", "xếp loại Giỏi / Xuất sắc", UiTheme.Rose, out lblCap4, out lblStatGioi);

            // ============================ QUICK CHECK ============================
            cardNhanh = new CardPanel();
            cardNhanh.CornerRadius = 12;

            lblTieuDeNhanh = new Label();
            lblTieuDeNhanh.Text = "Kiểm tra nhanh bằng bộ hàm dùng chung";
            lblTieuDeNhanh.Font = UiTheme.FontBold(11f);
            lblTieuDeNhanh.ForeColor = UiTheme.Ink;
            lblTieuDeNhanh.AutoSize = true;
            lblTieuDeNhanh.BackColor = Color.Transparent;
            lblTieuDeNhanh.Location = new Point(22, 16);

            lblPhuNhanh = new Label();
            lblPhuNhanh.Text = "Các hàm trong Helpers/KiemTra.cs được dùng chung cho mọi Form. Nhập thử dữ liệu để xem từng quy tắc.";
            lblPhuNhanh.Font = UiTheme.FontBody(8.5f);
            lblPhuNhanh.ForeColor = UiTheme.Muted;
            lblPhuNhanh.AutoSize = true;
            lblPhuNhanh.BackColor = Color.Transparent;
            lblPhuNhanh.Location = new Point(22, 42);

            txtMaNhanh = UiFactory.TextBox("txtMaNhanh", "Mã SV: SV10", false);
            txtTenNhanh = UiFactory.TextBox("txtTenNhanh", "Họ và tên", false);
            txtDiemNhanh = UiFactory.TextBox("txtDiemNhanh", "Điểm 0 - 10", false);
            cboKhoaNhanh = UiFactory.ComboBox("cboKhoaNhanh");
            foreach (Khoa k in DuLieuMau.DanhSachKhoa()) cboKhoaNhanh.Items.Add(k);

            txtMaNhanh.SetBounds(22, 76, 140, 26);
            txtTenNhanh.SetBounds(174, 76, 210, 26);
            txtDiemNhanh.SetBounds(396, 76, 110, 26);
            cboKhoaNhanh.SetBounds(518, 76, 200, 26);
            cboKhoaNhanh.SelectedIndex = 0;
            txtDiemNhanh.KeyPress += KiemTra.ChanNhapDiem;

            btnKiemTraNhanh = new AccentButton();
            btnKiemTraNhanh.Name = "btnKiemTraNhanh";
            btnKiemTraNhanh.Text = "Kiểm tra";
            btnKiemTraNhanh.BaseColor = UiTheme.Violet;
            btnKiemTraNhanh.SetBounds(22, 116, 132, 38);
            btnKiemTraNhanh.Click += btnKiemTraNhanh_Click;

            lblKetQuaNhanh = new Label();
            lblKetQuaNhanh.Text = "Nhập dữ liệu rồi bấm Kiểm tra để xem kết quả của từng quy tắc.";
            lblKetQuaNhanh.Font = UiTheme.FontBody(9f);
            lblKetQuaNhanh.ForeColor = UiTheme.InkSoft;
            lblKetQuaNhanh.BackColor = Color.Transparent;
            lblKetQuaNhanh.SetBounds(172, 112, 546, 88);

            cardNhanh.Controls.AddRange(new Control[] {
                lblTieuDeNhanh, lblPhuNhanh, txtMaNhanh, txtTenNhanh, txtDiemNhanh,
                cboKhoaNhanh, btnKiemTraNhanh, lblKetQuaNhanh });

            // ============================ DASHBOARD ============================
            pnlDashboard = new Panel();
            pnlDashboard.Name = "pnlDashboard";
            pnlDashboard.BackColor = Color.FromArgb(236, 239, 247);
            pnlDashboard.Visible = false;
            pnlDashboard.Controls.AddRange(new Control[] { card1, card2, card3, card4, cardNhanh });
            pnlDashboard.Resize += (s, e) => SapXepDashboard();

            // ============================ MDI ============================
            IsMdiContainer = true;

            // ============================ FORM ============================
            AutoScaleMode = AutoScaleMode.None;
            Text = "Lab 5 - Xử lý sự kiện và ứng dụng đa Form (IT3212)";
            ClientSize = new Size(1140, 668);
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1020, 620);
            MainMenuStrip = menuChinh;
            KeyPreview = true;

            Controls.Add(pnlHero);
            Controls.Add(statusChinh);
            Controls.Add(menuChinh);

            pnlHero.BringToFront();
            statusChinh.SendToBack();

            Load += frmMain_Load;
            // Cháº¿ Ä‘á»™ chá»¥p áº£nh bÃ¡o cÃ¡o (--screenshots): tá»± Ä‘Ã³ng Form Ä‘á»ƒ bá»™ chá»¥p cháº¡y tiáº¿p ká»‹ch báº£n sau.
            if (CheDoChupAnh.DangChup) { Shown += delegate { BeginInvoke((MethodInvoker)Close); }; }
            FormClosing += frmMain_FormClosing;
            FormClosed += frmMain_FormClosed;
        }

        /// <summary>Card phiếu chỉ số: thanh màu + tiêu đề + số lớn + ghi chú.</summary>
        private CardPanel TaoCard(string caption, string value, string note, Color accent, out Label lblCaption, out Label lblValue)
        {
            var card = new CardPanel();
            card.CornerRadius = 12;
            card.Padding = new Padding(0);

            var bar = new Panel();
            bar.BackColor = accent;
            bar.SetBounds(0, 0, 5, 96);
            bar.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;

            lblCaption = new Label();
            lblCaption.Text = caption;
            lblCaption.Font = UiTheme.FontBody(8.75f);
            lblCaption.ForeColor = UiTheme.Muted;
            lblCaption.AutoSize = true;
            lblCaption.BackColor = Color.Transparent;
            lblCaption.Location = new Point(18, 10);

            lblValue = new Label();
            lblValue.Text = value;
            lblValue.Font = UiTheme.FontTitle(21f);
            lblValue.ForeColor = accent;
            lblValue.AutoSize = true;
            lblValue.BackColor = Color.Transparent;
            lblValue.Location = new Point(15, 28);

            var lblNote = new Label();
            lblNote.Text = note;
            lblNote.Font = UiTheme.FontBody(8f);
            lblNote.ForeColor = UiTheme.Muted;
            lblNote.AutoSize = true;
            lblNote.BackColor = Color.Transparent;
            lblNote.Location = new Point(18, 70);

            card.Controls.AddRange(new Control[] { bar, lblCaption, lblValue, lblNote });
            return card;
        }

        private void btnKiemTraNhanh_Click(object sender, EventArgs e)
        {
            string kq = KiemTra.ChamDiemNhanh(
                UiFactory.Val(txtMaNhanh),
                UiFactory.Val(txtTenNhanh),
                UiFactory.Val(txtDiemNhanh),
                cboKhoaNhanh.Text);

            lblKetQuaNhanh.Text = kq;
            lblKetQuaNhanh.ForeColor = kq.Contains("LỖI") ? UiTheme.Red : UiTheme.Green;
            Log("Kiểm tra nhanh dữ liệu nhập trên Dashboard");
        }

        private void pnlHero_Paint(object sender, PaintEventArgs e)
        {
            PanelPaint.GradientAndBubbles(sender, e);
        }

        /// <summary>Tô màu menu theo bảng màu của ứng dụng.</summary>
        private class MenuColorTable : ProfessionalColorTable
        {
            public override Color MenuItemSelected { get { return UiTheme.Tint(UiTheme.Violet, 0.16); } }
            public override Color MenuItemSelectedGradientBegin { get { return UiTheme.Tint(UiTheme.Violet, 0.16); } }
            public override Color MenuItemSelectedGradientEnd { get { return UiTheme.Tint(UiTheme.Violet, 0.16); } }
            public override Color MenuItemBorder { get { return UiTheme.Tint(UiTheme.Violet, 0.4); } }
            public override Color MenuBorder { get { return UiTheme.Border; } }
            public override Color ToolStripDropDownBackground { get { return Color.White; } }
            public override Color ImageMarginGradientBegin { get { return Color.White; } }
            public override Color ImageMarginGradientMiddle { get { return Color.White; } }
            public override Color ImageMarginGradientEnd { get { return Color.White; } }
            public override Color MenuItemPressedGradientBegin { get { return UiTheme.Tint(UiTheme.Violet, 0.22); } }
            public override Color MenuItemPressedGradientEnd { get { return UiTheme.Tint(UiTheme.Violet, 0.22); } }
        }
    }
}
