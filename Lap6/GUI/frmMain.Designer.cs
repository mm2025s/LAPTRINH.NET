using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Lab06_ThreeLayer.GUI.Helpers;

namespace Lab06_ThreeLayer.GUI
{
    partial class frmMain
    {
        private IContainer components = null;

        private MenuStrip menuChinh;
        private ToolStripMenuItem menuHeThong, menuThoat;
        private ToolStripMenuItem menuNghiepVu;
        private ToolStripMenuItem menuSinhVien, menuSanPham, menuDanhMuc, menuDanhMucSinhVien, menuBieuTuong;
        private ToolStripMenuItem menuCuaSo, menuTrangChu;
        private ToolStripMenuItem menuTroGiup, menuKienTruc, menuGioiThieu;
        private ToolStripMenuItem menuPhienBan;

        private StatusStrip statusChinh;
        private ToolStripStatusLabel tsslTrangThai, tsslSpring, tsslSoLanMo, tsslDongHo;

        private Panel pnlHero;
        private Label lblChao, lblMoTa, lblBadge;

        private CardPanel card1, card2, card3, card4;
        private Label lblCap1, lblCap2, lblCap3, lblCap4;
        private Label lblStatSV, lblStatDiem, lblStatSP, lblStatTon;

        private Panel pnlDashboard;
        private CardPanel cardTomTat;
        private Label lblTomTatTieuDe, lblTomTatPhu;
        private ListBox lstTheoKhoa;
        private AccentButton btnMoSinhVien, btnMoSanPham, btnMoDanhMuc, btnMoBieuTuong;

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
            menuThoat = new ToolStripMenuItem("Thoát ứng dụng");
            menuNghiepVu = new ToolStripMenuItem("Nghiệp vụ");
            menuSinhVien = new ToolStripMenuItem("Quản lý sinh viên");
            menuSanPham = new ToolStripMenuItem("Quản lý sản phẩm");
            menuDanhMuc = new ToolStripMenuItem("Danh mục sản phẩm");
            menuDanhMucSinhVien = new ToolStripMenuItem("Danh mục khoa - sinh viên");
            menuBieuTuong = new ToolStripMenuItem("Sinh viên có biểu tượng");
            menuCuaSo = new ToolStripMenuItem("Cửa sổ");
            menuTrangChu = new ToolStripMenuItem("Về trang chủ");
            menuTroGiup = new ToolStripMenuItem("Trợ giúp");
            menuKienTruc = new ToolStripMenuItem("Kiến trúc 3 lớp");
            menuGioiThieu = new ToolStripMenuItem("Giới thiệu chương trình");
            menuPhienBan = new ToolStripMenuItem("Phiên bản 6.0");

            menuChinh.Name = "menuChinh";
            menuChinh.Font = UiTheme.FontBody(9.75f);
            menuChinh.BackColor = Color.White;
            menuChinh.ForeColor = UiTheme.Ink;
            menuChinh.Padding = new Padding(14, 5, 0, 5);
            menuChinh.Renderer = new ToolStripProfessionalRenderer(new MenuColorTable());
            menuChinh.Items.AddRange(new ToolStripItem[] {
                menuHeThong, menuNghiepVu, menuCuaSo, menuTroGiup, menuPhienBan });

            menuPhienBan.Enabled = false;
            menuPhienBan.ForeColor = UiTheme.Muted;

            menuHeThong.DropDownItems.AddRange(new ToolStripItem[] { menuThoat });
            menuNghiepVu.DropDownItems.AddRange(new ToolStripItem[] {
                menuSinhVien, menuSanPham, new ToolStripSeparator(),
                menuDanhMuc, menuDanhMucSinhVien, menuBieuTuong });
            menuCuaSo.DropDownItems.AddRange(new ToolStripItem[] { menuTrangChu });
            menuTroGiup.DropDownItems.AddRange(new ToolStripItem[] { menuKienTruc, menuGioiThieu });

            menuSinhVien.ShortcutKeys = Keys.Control | Keys.N;
            menuSanPham.ShortcutKeys = Keys.Control | Keys.D;
            menuDanhMuc.ShortcutKeys = Keys.Control | Keys.M;
            menuBieuTuong.ShortcutKeys = Keys.Control | Keys.I;
            menuGioiThieu.ShortcutKeys = Keys.F1;
            menuKienTruc.ShortcutKeys = Keys.Control | Keys.K;

            menuSinhVien.Click += mnuSinhVien_Click;
            menuSanPham.Click += mnuSanPham_Click;
            menuDanhMuc.Click += mnuDanhMuc_Click;
            menuDanhMucSinhVien.Click += mnuDanhMucSinhVien_Click;
            menuBieuTuong.Click += mnuBieuTuong_Click;
            menuTrangChu.Click += mnuTrangChu_Click;
            menuKienTruc.Click += mnuKienTruc_Click;
            menuGioiThieu.Click += mnuGioiThieu_Click;
            menuThoat.Click += mnuThoat_Click;

            // ============================ STATUS STRIP ============================
            statusChinh = new StatusStrip();
            statusChinh.BackColor = Color.FromArgb(20, 34, 33);
            statusChinh.ForeColor = Color.White;
            statusChinh.Font = UiTheme.FontBody(8.75f);
            statusChinh.SizingGrip = false;
            statusChinh.Padding = new Padding(10, 3, 10, 3);

            tsslTrangThai = new ToolStripStatusLabel(" Sẵn sàng");
            tsslTrangThai.ForeColor = Color.FromArgb(206, 224, 220);
            tsslSpring = new ToolStripStatusLabel();
            tsslSpring.Spring = true;
            tsslSoLanMo = new ToolStripStatusLabel(" Đã mở 0 form");
            tsslSoLanMo.ForeColor = Color.FromArgb(150, 176, 170);
            tsslSoLanMo.BorderSides = ToolStripStatusLabelBorderSides.Left;
            tsslDongHo = new ToolStripStatusLabel();
            tsslDongHo.ForeColor = Color.FromArgb(124, 231, 211);
            tsslDongHo.BorderSides = ToolStripStatusLabelBorderSides.Left;

            statusChinh.Items.AddRange(new ToolStripItem[] { tsslTrangThai, tsslSpring, tsslSoLanMo, tsslDongHo });

            // ============================ HERO ============================
            pnlHero = new Panel();
            pnlHero.Name = "pnlHero";
            pnlHero.Dock = DockStyle.Top;
            pnlHero.Height = 120;
            pnlHero.BackColor = Color.Transparent;
            pnlHero.Paint += pnlHero_Paint;

            lblChao = new Label();
            lblChao.Text = "Lab 6 - Dữ liệu với DataGridView & mô hình 3 lớp";
            lblChao.Font = UiTheme.FontTitle(19f);
            lblChao.ForeColor = Color.White;
            lblChao.AutoSize = true;
            lblChao.BackColor = Color.Transparent;
            lblChao.Location = new Point(30, 22);

            lblMoTa = new Label();
            lblMoTa.Text = "IT3212 - Lập trình .NET  |  Khoa Công nghệ Thông tin - ĐH Công nghệ Đông Á";
            lblMoTa.Font = UiTheme.FontBody(9.75f);
            lblMoTa.ForeColor = Color.FromArgb(208, 232, 228);
            lblMoTa.AutoSize = true;
            lblMoTa.BackColor = Color.Transparent;
            lblMoTa.Location = new Point(32, 58);

            lblBadge = new Label();
            lblBadge.Text = "DataGridView    BindingSource    TreeView    ListView + ImageList    BUS / DAL    CSV";
            lblBadge.Font = UiTheme.FontBody(8.5f);
            lblBadge.ForeColor = Color.FromArgb(146, 255, 243);
            lblBadge.AutoSize = true;
            lblBadge.BackColor = Color.Transparent;
            lblBadge.Location = new Point(32, 86);

            pnlHero.Controls.AddRange(new Control[] { lblChao, lblMoTa, lblBadge });

            // ============================ STAT CARDS ============================
            card1 = TaoCard("Sinh viên", "0", "BUS.SinhVienBUS", UiTheme.Teal, out lblCap1, out lblStatSV);
            card2 = TaoCard("Điểm trung bình", "0.00", "BUS tính, không phải GUI", UiTheme.Indigo, out lblCap2, out lblStatDiem);
            card3 = TaoCard("Sản phẩm", "0", "BUS.SanPhamBUS", UiTheme.Orange, out lblCap3, out lblStatSP);
            card4 = TaoCard("Giá trị hàng tồn", "0 đ", "tổng đơn giá × số lượng", UiTheme.Rose, out lblCap4, out lblStatTon);

            // ============================ DASHBOARD ============================
            pnlDashboard = new Panel();
            pnlDashboard.Name = "pnlDashboard";
            pnlDashboard.BackColor = Color.FromArgb(236, 239, 247);
            pnlDashboard.Visible = false;

            cardTomTat = new CardPanel();
            cardTomTat.CornerRadius = 12;

            lblTomTatTieuDe = new Label();
            lblTomTatTieuDe.Text = "Số sinh viên theo khoa (BUS.NhomTheoKhoa)";
            lblTomTatTieuDe.Font = UiTheme.FontBold(11f);
            lblTomTatTieuDe.ForeColor = UiTheme.Ink;
            lblTomTatTieuDe.AutoSize = true;
            lblTomTatTieuDe.BackColor = Color.Transparent;
            lblTomTatTieuDe.Location = new Point(22, 16);

            lblTomTatPhu = new Label();
            lblTomTatPhu.Text = "";
            lblTomTatPhu.Font = UiTheme.FontBody(8.5f);
            lblTomTatPhu.ForeColor = UiTheme.Muted;
            lblTomTatPhu.AutoSize = true;
            lblTomTatPhu.BackColor = Color.Transparent;
            lblTomTatPhu.Location = new Point(22, 40);

            lstTheoKhoa = new ListBox();
            lstTheoKhoa.Name = "lstTheoKhoa";
            lstTheoKhoa.Font = new Font("Consolas", 9f);
            lstTheoKhoa.ForeColor = UiTheme.InkSoft;
            lstTheoKhoa.BorderStyle = BorderStyle.FixedSingle;
            lstTheoKhoa.IntegralHeight = false;
            lstTheoKhoa.ItemHeight = 18;
            lstTheoKhoa.SetBounds(24, 68, 430, 126);

            btnMoSinhVien = new AccentButton();
            btnMoSinhVien.Name = "btnMoSinhVien";
            btnMoSinhVien.Text = "Mở Quản lý sinh viên";
            btnMoSinhVien.BaseColor = UiTheme.Teal;
            btnMoSinhVien.SetBounds(476, 70, 210, 40);
            btnMoSinhVien.Click += btnMoSinhVien_Click;

            btnMoSanPham = new AccentButton();
            btnMoSanPham.Name = "btnMoSanPham";
            btnMoSanPham.Text = "Mở Quản lý sản phẩm";
            btnMoSanPham.BaseColor = UiTheme.Orange;
            btnMoSanPham.SetBounds(476, 120, 210, 40);
            btnMoSanPham.Click += btnMoSanPham_Click;

            btnMoDanhMuc = new AccentButton();
            btnMoDanhMuc.Name = "btnMoDanhMuc";
            btnMoDanhMuc.Text = "Mở Danh mục sản phẩm";
            btnMoDanhMuc.BaseColor = UiTheme.Indigo;
            btnMoDanhMuc.SetBounds(476, 170, 210, 40);
            btnMoDanhMuc.Click += btnMoDanhMuc_Click;

            btnMoBieuTuong = new AccentButton();
            btnMoBieuTuong.Name = "btnMoBieuTuong";
            btnMoBieuTuong.Text = "Mở Danh sách có biểu tượng";
            btnMoBieuTuong.BaseColor = UiTheme.Rose;
            btnMoBieuTuong.SetBounds(700, 70, 220, 40);
            btnMoBieuTuong.Click += btnMoBieuTuong_Click;

            cardTomTat.Controls.AddRange(new Control[] {
                lblTomTatTieuDe, lblTomTatPhu, lstTheoKhoa,
                btnMoSinhVien, btnMoSanPham, btnMoDanhMuc, btnMoBieuTuong });

            pnlDashboard.Controls.AddRange(new Control[] { card1, card2, card3, card4, cardTomTat });

            // ============================ FORM ============================
            IsMdiContainer = true;

            AutoScaleMode = AutoScaleMode.None;
            Text = "Lab 6 - Dữ liệu với DataGridView và mô hình 3 lớp (IT3212)";
            ClientSize = new Size(1140, 668);
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1000, 620);
            MainMenuStrip = menuChinh;
            KeyPreview = true;

            Controls.Add(pnlHero);
            Controls.Add(statusChinh);
            Controls.Add(menuChinh);

            pnlHero.BringToFront();

            Load += frmMain_Load;
            FormClosing += frmMain_FormClosing;
        }

        /// <summary>Card chỉ số: thanh màu + tiêu đề + số lớn + ghi chú (lấy từ BUS).</summary>
        private CardPanel TaoCard(string caption, string value, string note, Color accent, out Label lblCaption, out Label lblValue)
        {
            CardPanel card = new CardPanel();
            card.CornerRadius = 12;

            Panel bar = new Panel();
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
            lblValue.Font = UiTheme.FontTitle(20f);
            lblValue.ForeColor = accent;
            lblValue.AutoSize = true;
            lblValue.BackColor = Color.Transparent;
            lblValue.Location = new Point(15, 28);

            Label lblNote = new Label();
            lblNote.Text = note;
            lblNote.Font = UiTheme.FontBody(8f);
            lblNote.ForeColor = UiTheme.Muted;
            lblNote.AutoSize = true;
            lblNote.BackColor = Color.Transparent;
            lblNote.Location = new Point(18, 70);

            card.Controls.AddRange(new Control[] { bar, lblCaption, lblValue, lblNote });
            return card;
        }
    }
}
