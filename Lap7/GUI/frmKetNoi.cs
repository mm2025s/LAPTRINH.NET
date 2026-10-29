using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Lab07_ADONET_Connection.BUS;
using Lab07_ADONET_Connection.DAL;
using Lab07_ADONET_Connection.DTO;
using Lab07_ADONET_Connection.GUI.Helpers;

namespace Lab07_ADONET_Connection.GUI
{
    /// <summary>
    /// frmKetNoi - Bai 5 (nut btnKiemTra), bai 7 (SQL Authentication), bai 8 (doc chuoi ket noi).
    ///
    /// Nut "btnKiemTra" giu dung tinh than code mau:
    ///     conn.Open();
    ///     MessageBox.Show("Ket noi thanh cong: " + conn.Database);
    ///   catch (SqlException ex) -> "Loi ket noi: " + ex.Message
    ///
    /// Ngoai ra cho phep xem/sua chuoi ket noi dang dung va thu SQL Authentication.
    /// </summary>
    public class frmKetNoi : FormBase
    {
        private readonly KetNoiBUS _bus = new KetNoiBUS();
        private readonly frmMain _main;

        private InfoStrip _hero;
        private TextBox _txtChuoiKetNoi;
        private Label _lblNguon;
        private TextBox _txtMayChu, _txtTaiKhoan, _txtMatKhau, _txtCsdl;
        private Label _lblKetQua;
        private bool _cheDoSqlAuth;

        public frmKetNoi() : this(null) { }

        public frmKetNoi(frmMain main)
        {
            _main = main;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Kiểm tra kết nối SQL Server";
            ClientSize = new Size(880, 600);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(760, 560);

            _hero = new InfoStrip();
            _hero.Dock = DockStyle.Top;
            _hero.Accent = UiTheme.Blue;
            _hero.ChipText = "5";
            _hero.Title = "Kiểm tra kết nối SQL Server";
            _hero.Subtitle = "Windows Authentication và SQL Authentication · đọc chuỗi kết nối từ App.config";
            _hero.RightTop = "DbConnectionFactory";
            _hero.RightBottom = "SqlConnection";

            var root = new Panel();
            root.Dock = DockStyle.Fill;
            root.BackColor = UiTheme.Canvas;
            root.Padding = new Padding(18);

            // ---- Card: chuoi ket noi dang dung ----
            var card1 = new CardPanel();
            card1.SetBounds(18, 14, 844, 168);
            card1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            card1.Controls.Add(TieuDe("Chuỗi kết nối đang sử dụng", 24, 14));

            _lblNguon = new Label();
            _lblNguon.Text = "Nguồn: " + DbConnectionFactory.NguonChuoiKetNoi;
            _lblNguon.Font = UiTheme.FontItalic(9f);
            _lblNguon.ForeColor = UiTheme.Muted;
            _lblNguon.BackColor = Color.Transparent;
            _lblNguon.SetBounds(24, 40, 800, 20);

            _txtChuoiKetNoi = new TextBox();
            _txtChuoiKetNoi.Name = "txtChuoiKetNoi";
            _txtChuoiKetNoi.Font = UiTheme.FontMono(9.5f);
            _txtChuoiKetNoi.BorderStyle = BorderStyle.FixedSingle;
            _txtChuoiKetNoi.Multiline = true;
            _txtChuoiKetNoi.ScrollBars = ScrollBars.Vertical;
            _txtChuoiKetNoi.Text = DbConnectionFactory.ChuoiKetNoi;
            _txtChuoiKetNoi.SetBounds(24, 64, 796, 52);
            _txtChuoiKetNoi.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            var btnNapLai = new AccentButton();
            btnNapLai.Name = "btnNapLai";
            btnNapLai.Text = "Nạp lại từ App.config";
            btnNapLai.BaseColor = UiTheme.Slate;
            btnNapLai.OutlineOnly = true;
            btnNapLai.SetBounds(24, 124, 180, 32);
            btnNapLai.Click += (s, e) =>
            {
                _txtChuoiKetNoi.Text = DbConnectionFactory.ChuoiKetNoi;
                _lblNguon.Text = "Nguồn: " + DbConnectionFactory.NguonChuoiKetNoi;
            };

            card1.Controls.AddRange(new Control[] { _lblNguon, _txtChuoiKetNoi, btnNapLai });

            // ---- Card: kiem tra ----
            var card2 = new CardPanel();
            card2.SetBounds(18, 194, 844, 200);
            card2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            card2.Controls.Add(TieuDe("Kiểm tra kết nối", 24, 14));

            var lblCheDo = new Label();
            lblCheDo.Text = "Chế độ: Windows Authentication (Integrated Security)";
            lblCheDo.Font = UiTheme.FontBold(9.5f);
            lblCheDo.ForeColor = UiTheme.Teal;
            lblCheDo.BackColor = Color.Transparent;
            lblCheDo.SetBounds(24, 42, 500, 22);

            // Vung SQL auth (an theo mac dinh)
            _txtMayChu = TaoONhap("txtMayChu", ".\\SQLEXPRESS", 24, 74, 380);
            _txtCsdl = TaoONhap("txtCsdl", "QLSinhVien", 424, 74, 396);
            _txtTaiKhoan = TaoONhap("txtTaiKhoan", "app_qlsv", 24, 116, 380);
            _txtMatKhau = TaoONhap("txtMatKhau", "MatKhau@123", 424, 116, 396);
            _txtMatKhau.UseSystemPasswordChar = true;
            AnSqlauth(true);

            var btnBamWindows = new AccentButton();
            btnBamWindows.Name = "btnWindowsMode";
            btnBamWindows.Text = "Windows Auth";
            btnBamWindows.BaseColor = UiTheme.Teal;
            btnBamWindows.SetBounds(24, 156, 150, 34);
            btnBamWindows.Click += (s, e) =>
            {
                _cheDoSqlAuth = false;
                AnSqlauth(true);
                lblCheDo.Text = "Chế độ: Windows Authentication (Integrated Security)";
                lblCheDo.ForeColor = UiTheme.Teal;
            };

            var btnBamSql = new AccentButton();
            btnBamSql.Name = "btnSqlMode";
            btnBamSql.Text = "SQL Auth";
            btnBamSql.BaseColor = UiTheme.Indigo;
            btnBamSql.OutlineOnly = true;
            btnBamSql.SetBounds(184, 156, 150, 34);
            btnBamSql.Click += (s, e) =>
            {
                _cheDoSqlAuth = true;
                AnSqlauth(false);
                lblCheDo.Text = "Chế độ: SQL Server Authentication (tài khoản ứng dụng)";
                lblCheDo.ForeColor = UiTheme.Indigo;
            };

            card2.Controls.AddRange(new Control[] { lblCheDo, _txtMayChu, _txtCsdl, _txtTaiKhoan, _txtMatKhau, btnBamWindows, btnBamSql });

            // ---- Card: ket qua ----
            var card3 = new CardPanel();
            card3.SetBounds(18, 404, 844, 160);
            card3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            card3.Controls.Add(TieuDe("Kết quả", 24, 14));

            _lblKetQua = new Label();
            _lblKetQua.Name = "lblKetQua";
            _lblKetQua.Text = "Chưa kiểm tra. Nhấn “Kiểm tra kết nối”.";
            _lblKetQua.Font = UiTheme.FontBody(10f);
            _lblKetQua.ForeColor = UiTheme.InkSoft;
            _lblKetQua.BackColor = Color.Transparent;
            _lblKetQua.SetBounds(24, 44, 796, 100);
            _lblKetQua.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            card3.Controls.Add(_lblKetQua);

            // ---- Nut chinh ----
            var btnKiemTra = new AccentButton();
            btnKiemTra.Name = "btnKiemTra";
            btnKiemTra.Text = "Kiểm tra kết nối";
            btnKiemTra.BaseColor = UiTheme.Blue;
            btnKiemTra.SetBounds(18, 574, 200, 42);
            btnKiemTra.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnKiemTra.Click += btnKiemTra_Click;

            var btnDong = new AccentButton();
            btnDong.Name = "btnDong";
            btnDong.Text = "Đóng";
            btnDong.BaseColor = UiTheme.Slate;
            btnDong.OutlineOnly = true;
            btnDong.SetBounds(228, 574, 120, 42);
            btnDong.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnDong.Click += (s, e) => Close();

            root.Controls.AddRange(new Control[] { card1, card2, card3, btnKiemTra, btnDong });

            Controls.Add(root);
            Controls.Add(_hero);
            _hero.BringToFront();
        }

        private static Label TieuDe(string text, int x, int y)
        {
            var l = new Label();
            l.Text = text;
            l.Font = UiTheme.FontBold(12f);
            l.ForeColor = UiTheme.Ink;
            l.BackColor = Color.Transparent;
            l.SetBounds(x, y, 600, 24);
            return l;
        }

        private TextBox TaoONhap(string name, string value, int x, int y, int w)
        {
            var tb = new TextBox();
            tb.Name = name;
            tb.Font = UiTheme.FontBody(9.5f);
            tb.BorderStyle = BorderStyle.FixedSingle;
            tb.Text = value;
            tb.SetBounds(x, y, w, 26);
            return tb;
        }

        private void AnSqlauth(bool an)
        {
            _txtMayChu.Visible = !an;
            _txtCsdl.Visible = !an;
            _txtTaiKhoan.Visible = !an;
            _txtMatKhau.Visible = !an;
        }

        // =================================================================
        // BAI 5 & 7: XU LY NUT btnKiemTra
        // =================================================================
        private void btnKiemTra_Click(object sender, EventArgs e)
        {
            TrangThaiKetNoi tt;
            if (_cheDoSqlAuth)
            {
                tt = _bus.KiemTraSqlAuth(_txtMayChu.Text.Trim(), _txtCsdl.Text.Trim(),
                                         _txtTaiKhoan.Text.Trim(), _txtMatKhau.Text);
            }
            else
            {
                // Kiem tra bang chuoi ket noi hien tren Form (co the da sua)
                string chuoi = _txtChuoiKetNoi.Text.Trim();
                tt = _bus.KiemTraVoiChuoi(chuoi, "Chuỗi trên Form (Windows Authentication)");
            }

            HienKetQua(tt);
        }

        private void HienKetQua(TrangThaiKetNoi tt)
        {
            if (tt.ThanhCong)
            {
                _lblKetQua.ForeColor = UiTheme.Green;
                _lblKetQua.Text =
                    "✔ Kết nối thành công.\n" +
                    "   • Database       : " + tt.Database + "\n" +
                    "   • DataSource     : " + tt.DataSource + "\n" +
                    "   • ServerVersion  : " + tt.ServerVersion + "\n" +
                    "   • Thời gian (ms) : " + tt.ThoiGianKetNoiMs;
            }
            else
            {
                _lblKetQua.ForeColor = UiTheme.Red;
                _lblKetQua.Text = "✘ Lỗi kết nối.\n\n" + tt.ThongBao;
            }

            if (_main != null) _main.CapNhatKetNoi(tt);
        }
    }
}
