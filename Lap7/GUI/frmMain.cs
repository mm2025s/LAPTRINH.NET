using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Lab07_ADONET_Connection.BUS;
using Lab07_ADONET_Connection.DTO;
using Lab07_ADONET_Connection.GUI.Helpers;

namespace Lab07_ADONET_Connection.GUI
{
    /// <summary>
    /// frmMain - cua so chinh (MDI): MenuStrip + StatusStrip + Dashboard.
    /// Cho phep mo frmKetNoi, frmTrangThai, frmTruyVan, frmSoDoQuanHe.
    /// Dashboard lay so ban ghi tung bang tu BUS (khong dem tren luoi).
    /// </summary>
    public class frmMain : FormBase
    {
        private readonly TruyVanBUS _busTruyVan = new TruyVanBUS();
        private readonly KetNoiBUS _busKetNoi = new KetNoiBUS();

        private MenuStrip _menu;
        private StatusStrip _status;
        private ToolStripStatusLabel _lblTrangThai;
        private ToolStripStatusLabel _lblKetNoi;
        private ToolStripStatusLabel _lblDongHo;
        private System.Windows.Forms.Timer _timer;
        private InfoStrip _hero;
        private Label _lblBangKhoa, _lblBangSV, _lblBangHP, _lblBangDK;
        private Label _lblKetNoiTomTat;
        private BadgeLabel _badgeKetNoi;

        public frmMain()
        {
            InitializeComponent();
            this.Load += frmMain_Load;
            this.FormClosing += frmMain_FormClosing;
        }

        // =================================================================
        // DUNG GIAO DIEN
        // =================================================================
        private void InitializeComponent()
        {
            Text = "Lab 07 - ADO.NET va ket noi CSDL SQL Server";
            ClientSize = new Size(1040, 660);
            MinimumSize = new Size(900, 600);
            StartPosition = FormStartPosition.CenterScreen;
            IsMdiContainer = true;

            // ---- MenuStrip ----
            _menu = new MenuStrip();
            _menu.Font = UiTheme.FontBody(9.5f);
            _menu.BackColor = Color.White;

            _menu.Items.Add(TaoMenu("Hệ thống", mnuThoat_Click, "Thoát ứng dụng (Alt+F4)"));
            _menu.Items.Add(TaoMenu("Kết nối", mnuKetNoi_Click, "Kiểm tra kết nối SQL Server (bài 5, 7)"));
            _menu.Items.Add(TaoMenu("Trạng thái", mnuTrangThai_Click, "Trang trạng thái kết nối (bài 9)"));
            _menu.Items.Add(TaoMenu("Truy vấn", mnuTruyVan_Click, "Xem dữ liệu mẫu QLSinhVien (bài 5)"));
            _menu.Items.Add(TaoMenu("Sơ đồ quan hệ", mnuSoDo_Click, "Sơ đồ quan hệ các bảng"));
            _menu.Items.Add(TaoMenu("Cửa sổ", mnuSapXep_Click, "Sắp xếp các cửa sổ con"));

            // ---- StatusStrip ----
            _status = new StatusStrip();
            _status.BackColor = Color.White;
            _lblTrangThai = new ToolStripStatusLabel("Sẵn sàng");
            _lblTrangThai.Spring = true;
            _lblTrangThai.TextAlign = ContentAlignment.MiddleLeft;
            _lblKetNoi = new ToolStripStatusLabel("Chưa kiểm tra kết nối");
            _lblDongHo = new ToolStripStatusLabel(string.Empty);
            _status.Items.AddRange(new ToolStripItem[] { _lblTrangThai, _lblKetNoi, _lblDongHo });

            // ---- Hero + Dashboard ----
            _hero = new InfoStrip();
            _hero.Dock = DockStyle.Top;
            _hero.Accent = UiTheme.Blue;
            _hero.ChipText = "07";
            _hero.Title = "Tạo CSDL SQL Server · Tổng quan ADO.NET và kết nối CSDL";
            _hero.Subtitle = "IT3212 - Lập trình .NET · Chương 4 · Cơ sở dữ liệu QLSinhVien";
            _hero.RightTop = "System.Data.SqlClient";
            _hero.RightBottom = ".NET Framework 4.8";

            var panelDash = new Panel();
            panelDash.Dock = DockStyle.Fill;
            panelDash.BackColor = UiTheme.Canvas;
            panelDash.Padding = new Padding(18);

            var cardKetNoi = new CardPanel();
            cardKetNoi.SetBounds(18, 18, 996, 120);
            cardKetNoi.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            var lblKT = UiTheme.FontBold(12f);
            var lbKetNoiTitle = new Label();
            lbKetNoiTitle.Text = "Trạng thái kết nối";
            lbKetNoiTitle.Font = lblKT;
            lbKetNoiTitle.ForeColor = UiTheme.Ink;
            lbKetNoiTitle.BackColor = Color.Transparent;
            lbKetNoiTitle.SetBounds(24, 16, 300, 26);

            _badgeKetNoi = new BadgeLabel();
            _badgeKetNoi.BaseColor = UiTheme.Slate;
            _badgeKetNoi.Value = "Chưa kiểm tra";
            _badgeKetNoi.SetBounds(24, 52, 150, 26);

            _lblKetNoiTomTat = new Label();
            _lblKetNoiTomTat.Text = "Nhấn “Kết nối → Kiểm tra kết nối” hoặc nút bên phải để mở kết nối tới QLSinhVien.";
            _lblKetNoiTomTat.Font = UiTheme.FontBody(9.5f);
            _lblKetNoiTomTat.ForeColor = UiTheme.InkSoft;
            _lblKetNoiTomTat.BackColor = Color.Transparent;
            _lblKetNoiTomTat.SetBounds(188, 50, 620, 40);
            _lblKetNoiTomTat.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            var btnMoKetNoi = new AccentButton();
            btnMoKetNoi.Name = "btnMoKetNoi";
            btnMoKetNoi.Text = "Kiểm tra kết nối";
            btnMoKetNoi.BaseColor = UiTheme.Blue;
            btnMoKetNoi.SetBounds(828, 24, 148, 40);
            btnMoKetNoi.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMoKetNoi.Click += mnuKetNoi_Click;

            var btnMoTrangThai = new AccentButton();
            btnMoTrangThai.Name = "btnMoTrangThai";
            btnMoTrangThai.Text = "Trang trạng thái";
            btnMoTrangThai.BaseColor = UiTheme.Teal;
            btnMoTrangThai.OutlineOnly = true;
            btnMoTrangThai.SetBounds(828, 72, 148, 34);
            btnMoTrangThai.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMoTrangThai.Click += mnuTrangThai_Click;

            cardKetNoi.Controls.AddRange(new Control[] { lbKetNoiTitle, _badgeKetNoi, _lblKetNoiTomTat, btnMoKetNoi, btnMoTrangThai });

            // ---- 4 the so lieu ----
            int y = 158;
            int cardW = 234;
            int gap = 20;
            CardPanel the01, the02, the03, the04;
            _lblBangKhoa = TaoThe(panelDash, "KHOA", "Khoa", 0, y, cardW, UiTheme.Violet, out the01);
            _lblBangSV = TaoThe(panelDash, "SINH VIÊN", "SinhVien", cardW + gap, y, cardW, UiTheme.Blue, out the02);
            _lblBangHP = TaoThe(panelDash, "HỌC PHẦN", "HocPhan", (cardW + gap) * 2, y, cardW, UiTheme.Teal, out the03);
            _lblBangDK = TaoThe(panelDash, "ĐĂNG KÝ", "DangKy", (cardW + gap) * 3, y, cardW, UiTheme.Orange, out the04);

            var lblHuongDan = new Label();
            lblHuongDan.Text = "Hướng dẫn: dùng menu để mở các màn hình. “Truy vấn” hiển thị dữ liệu mẫu bằng DataGridView; " +
                               "“Sơ đồ quan hệ” vẽ quan hệ khoá ngoại giữa các bảng.";
            lblHuongDan.Font = UiTheme.FontBody(9.5f);
            lblHuongDan.ForeColor = UiTheme.InkSoft;
            lblHuongDan.BackColor = Color.Transparent;
            lblHuongDan.SetBounds(18, 300, 996, 50);
            lblHuongDan.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            panelDash.Controls.AddRange(new Control[] { cardKetNoi, lblHuongDan });

            Controls.Add(panelDash);
            Controls.Add(_hero);
            Controls.Add(_status);
            Controls.Add(_menu);
            MainMenuStrip = _menu;
            _hero.BringToFront();

            // ---- Dong ho ----
            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 1000;
            _timer.Tick += (s, e) => _lblDongHo.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            _timer.Start();
        }

        private ToolStripMenuItem TaoMenu(string text, EventHandler handler, string tooltip)
        {
            var item = new ToolStripMenuItem(text);
            item.ToolTipText = tooltip;
            item.Click += handler;
            return item;
        }

        private static Label TaoThe(Control parent, string tieuDe, string bang, int x, int y, int w, Color accent, out CardPanel card)
        {
            card = new CardPanel();
            card.SetBounds(x, y, w, 120);

            var lblTd = new Label();
            lblTd.Text = tieuDe;
            lblTd.Font = UiTheme.FontBold(8.5f);
            lblTd.ForeColor = accent;
            lblTd.BackColor = Color.Transparent;
            lblTd.SetBounds(18, 16, w - 36, 22);

            var lblSo = new Label();
            lblSo.Text = "—";
            lblSo.Font = UiTheme.FontTitle(30f);
            lblSo.ForeColor = UiTheme.Ink;
            lblSo.BackColor = Color.Transparent;
            lblSo.SetBounds(16, 40, w - 32, 48);

            var lblChu = new Label();
            lblChu.Text = "bản ghi trong dbo." + bang;
            lblChu.Font = UiTheme.FontBody(8.75f);
            lblChu.ForeColor = UiTheme.Muted;
            lblChu.BackColor = Color.Transparent;
            lblChu.SetBounds(18, 90, w - 36, 22);

            card.Controls.AddRange(new Control[] { lblTd, lblSo, lblChu });
            parent.Controls.Add(card);
            return lblSo;
        }

        // =================================================================
        // SU KIEN
        // =================================================================
        private void frmMain_Load(object sender, EventArgs e)
        {
            CapNhatDashboard();
            Log("Mo ung dung Lab 07 - ADO.NET & ket noi CSDL");
        }

        public void CapNhatDashboard()
        {
            try
            {
                Dictionary<string, int> tk = _busTruyVan.ThongKeNhieuBang(new[] { "Khoa", "SinhVien", "HocPhan", "DangKy" });
                _lblBangKhoa.Text = tk["Khoa"].ToString();
                _lblBangSV.Text = tk["SinhVien"].ToString();
                _lblBangHP.Text = tk["HocPhan"].ToString();
                _lblBangDK.Text = tk["DangKy"].ToString();
            }
            catch (Exception ex)
            {
                _lblBangKhoa.Text = _lblBangSV.Text = _lblBangHP.Text = _lblBangDK.Text = "?";
                Log("Chưa đọc được dữ liệu (kiểm tra kết nối): " + ex.Message);
            }
        }

        /// <summary>Cap nhat dai trang thai ket noi tren Dashboard.</summary>
        public void CapNhatKetNoi(TrangThaiKetNoi tt)
        {
            if (tt.ThanhCong)
            {
                _badgeKetNoi.Value = "Đã kết nối";
                _badgeKetNoi.BaseColor = UiTheme.Green;
                _lblKetNoiTomTat.Text = string.Format(
                    "Nguồn: {0}\nDatabase “{1}” · {2} · {3} ms",
                    tt.NguonChuoiKetNoi, tt.Database, tt.ServerVersion, tt.ThoiGianKetNoiMs);
                _lblKetNoi.Text = "Kết nối: " + tt.Database + " @ " + tt.DataSource;
                Log("Kết nối thành công tới " + tt.Database + " (" + tt.ThoiGianKetNoiMs + " ms)");
                CapNhatDashboard();
            }
            else
            {
                _badgeKetNoi.Value = "Lỗi kết nối";
                _badgeKetNoi.BaseColor = UiTheme.Red;
                _lblKetNoiTomTat.Text = tt.ThongBao;
                _lblKetNoi.Text = "Mất kết nối";
                Log("Lỗi kết nối: " + tt.ThongBao);
            }
        }

        public void Log(string text)
        {
            _lblTrangThai.Text = DateTime.Now.ToString("HH:mm:ss") + "  ·  " + text;
        }

        // ---------------------------------------------------------------
        // MENU
        // ---------------------------------------------------------------
        private void mnuKetNoi_Click(object sender, EventArgs e)
        {
            var f = new frmKetNoi(this);
            f.MdiParent = this;
            f.Show();
        }

        private void mnuTrangThai_Click(object sender, EventArgs e)
        {
            var f = new frmTrangThai();
            f.MdiParent = this;
            f.Show();
        }

        private void mnuTruyVan_Click(object sender, EventArgs e)
        {
            var f = new frmTruyVan();
            f.MdiParent = this;
            f.Show();
        }

        private void mnuSoDo_Click(object sender, EventArgs e)
        {
            var f = new frmSoDoQuanHe();
            f.MdiParent = this;
            f.Show();
        }

        private void mnuSapXep_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
            Log("Sắp xếp cửa sổ con (Cascade)");
        }

        private void mnuThoat_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult r = MessageBox.Show(this,
                    "Bạn có chắc chắn muốn thoát ứng dụng không?",
                    "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);

                if (r != DialogResult.Yes)
                {
                    e.Cancel = true;
                    Log("Người dùng chọn No -> e.Cancel = true");
                    return;
                }
            }
            Log("Thoát ứng dụng");
        }
    }
}
