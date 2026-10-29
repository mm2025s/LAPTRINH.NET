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
    /// frmTrangThai - Bai 9: man hinh trang thai ket noi.
    /// Hien thi day du: DataSource, Database, ServerVersion, State, thoi gian ket noi
    /// va nguon chuoi ket noi. Co nut lam moi va bang chi tiet.
    /// </summary>
    public class frmTrangThai : FormBase
    {
        private readonly KetNoiBUS _bus = new KetNoiBUS();

        private InfoStrip _hero;
        private BadgeLabel _badge;
        private Label _lblDataSource, _lblDatabase, _lblServerVersion, _lblState, _lblThoiGian, _lblNguon, _lblChuoi;
        private DataGridView _luoi;
        private Label _lblThongBao;

        public frmTrangThai()
        {
            InitializeComponent();
            Load += (s, e) => LamMoi();
        }

        private void InitializeComponent()
        {
            Text = "Trang trạng thái kết nối";
            ClientSize = new Size(860, 600);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(760, 560);

            _hero = new InfoStrip();
            _hero.Dock = DockStyle.Top;
            _hero.Accent = UiTheme.Teal;
            _hero.ChipText = "9";
            _hero.Title = "Trạng thái kết nối CSDL";
            _hero.Subtitle = "DataSource · Database · ServerVersion · State · thời gian kết nối";
            _hero.RightTop = "SqlConnection";
            _hero.RightBottom = "QLSinhVien";

            var root = new Panel();
            root.Dock = DockStyle.Fill;
            root.BackColor = UiTheme.Canvas;
            root.Padding = new Padding(18);

            _badge = new BadgeLabel();
            _badge.BaseColor = UiTheme.Slate;
            _badge.Value = "Chưa kiểm tra";
            _badge.SetBounds(18, 12, 160, 28);

            _lblThongBao = new Label();
            _lblThongBao.Text = "Nhấn “Làm mới” để mở lại kết nối.";
            _lblThongBao.Font = UiTheme.FontBody(9.5f);
            _lblThongBao.ForeColor = UiTheme.InkSoft;
            _lblThongBao.BackColor = Color.Transparent;
            _lblThongBao.SetBounds(190, 16, 520, 24);

            var btnLamMoi = new AccentButton();
            btnLamMoi.Name = "btnLamMoi";
            btnLamMoi.Text = "Làm mới";
            btnLamMoi.BaseColor = UiTheme.Teal;
            btnLamMoi.SetBounds(720, 10, 120, 32);
            btnLamMoi.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnLamMoi.Click += (s, e) => LamMoi();

            // ---- Luoi chi tiet ----
            _luoi = new DataGridView();
            _luoi.Name = "luoiTrangThai";
            _luoi.SetBounds(18, 52, 824, 300);
            _luoi.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _luoi.BackgroundColor = Color.White;
            _luoi.BorderStyle = BorderStyle.FixedSingle;
            _luoi.RowHeadersVisible = false;
            _luoi.AllowUserToAddRows = false;
            _luoi.AllowUserToResizeRows = false;
            _luoi.ReadOnly = true;
            _luoi.SelectionMode = DataGridViewSelectionMode.CellSelect;
            _luoi.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _luoi.Font = UiTheme.FontBody(9.5f);
            _luoi.ColumnHeadersDefaultCellStyle.Font = UiTheme.FontBold(9.5f);
            _luoi.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(236, 242, 252);
            _luoi.EnableHeadersVisualStyles = false;
            _luoi.RowTemplate.Height = 26;
            _luoi.ColumnCount = 2;
            _luoi.Columns[0].Name = "ThuocTinh";
            _luoi.Columns[0].HeaderText = "Thuộc tính";
            _luoi.Columns[0].FillWeight = 32;
            _luoi.Columns[1].Name = "GiaTri";
            _luoi.Columns[1].HeaderText = "Giá trị";
            _luoi.Columns[1].FillWeight = 68;

            // ---- Chuoi ket noi ----
            var lblChuoiTd = new Label();
            lblChuoiTd.Text = "Chuỗi kết nối đang dùng";
            lblChuoiTd.Font = UiTheme.FontBold(10f);
            lblChuoiTd.ForeColor = UiTheme.Ink;
            lblChuoiTd.BackColor = Color.Transparent;
            lblChuoiTd.SetBounds(18, 360, 400, 22);
            lblChuoiTd.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

            _lblChuoi = new Label();
            _lblChuoi.Name = "lblChuoiKetNoi";
            _lblChuoi.Text = "—";
            _lblChuoi.Font = UiTheme.FontMono(9f);
            _lblChuoi.ForeColor = UiTheme.InkSoft;
            _lblChuoi.BackColor = Color.White;
            _lblChuoi.BorderStyle = BorderStyle.FixedSingle;
            _lblChuoi.SetBounds(18, 384, 824, 44);
            _lblChuoi.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            var lblNguonTd = new Label();
            lblNguonTd.Text = "Nguồn chuỗi:";
            lblNguonTd.Font = UiTheme.FontBold(9f);
            lblNguonTd.ForeColor = UiTheme.Muted;
            lblNguonTd.BackColor = Color.Transparent;
            lblNguonTd.SetBounds(18, 434, 110, 20);
            lblNguonTd.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

            _lblNguon = new Label();
            _lblNguon.Text = "—";
            _lblNguon.Font = UiTheme.FontBody(9f);
            _lblNguon.ForeColor = UiTheme.InkSoft;
            _lblNguon.BackColor = Color.Transparent;
            _lblNguon.SetBounds(120, 434, 722, 20);
            _lblNguon.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            // Cac label chi tiet (dung de trinh chup anh doc truc tiep)
            _lblDataSource = An("lblDataSource", 500, 500);
            _lblDatabase = An("lblDatabase", 500, 500);
            _lblServerVersion = An("lblServerVersion", 500, 500);
            _lblState = An("lblState", 500, 500);
            _lblThoiGian = An("lblThoiGian", 500, 500);

            root.Controls.AddRange(new Control[] { _badge, _lblThongBao, btnLamMoi, _luoi, lblChuoiTd, _lblChuoi, lblNguonTd, _lblNguon,
                                                   _lblDataSource, _lblDatabase, _lblServerVersion, _lblState, _lblThoiGian });

            Controls.Add(root);
            Controls.Add(_hero);
            _hero.BringToFront();
        }

        /// <summary>Label an (chi de luu gia tri cho trinh chup anh) - khong hien thi.</summary>
        private static Label An(string name, int x, int y)
        {
            var l = new Label();
            l.Name = name;
            l.Text = "";
            l.Visible = false;
            l.SetBounds(x, y, 10, 10);
            return l;
        }

        public void LamMoi()
        {
            TrangThaiKetNoi tt = _bus.KiemTraKetNoi();

            _lblDataSource.Text = tt.DataSource;
            _lblDatabase.Text = tt.Database;
            _lblServerVersion.Text = tt.ServerVersion;
            _lblState.Text = tt.State;
            _lblThoiGian.Text = tt.ThoiGianKetNoiMs + " ms";
            _lblNguon.Text = tt.NguonChuoiKetNoi;
            _lblChuoi.Text = tt.ChuoiKetNoi;

            _luoi.Rows.Clear();
            _luoi.Rows.Add("DataSource", tt.DataSource);
            _luoi.Rows.Add("Database", tt.Database);
            _luoi.Rows.Add("ServerVersion", tt.ServerVersion);
            _luoi.Rows.Add("State", tt.State);
            _luoi.Rows.Add("Thời gian kết nối", tt.ThoiGianKetNoiMs + " ms");
            _luoi.Rows.Add("Thời điểm kiểm tra", tt.ThoiDiem.ToString("dd/MM/yyyy HH:mm:ss"));
            _luoi.Rows.Add("Nguồn chuỗi kết nối", tt.NguonChuoiKetNoi);
            _luoi.Rows.Add("Kết quả", tt.ThanhCong ? "Thành công" : "Thất bại");

            if (tt.ThanhCong)
            {
                _badge.Value = "Đã kết nối";
                _badge.BaseColor = UiTheme.Green;
                _lblThongBao.ForeColor = UiTheme.Green;
                _lblThongBao.Text = "Kết nối đang mở, trạng thái: " + tt.State;
            }
            else
            {
                _badge.Value = "Lỗi kết nối";
                _badge.BaseColor = UiTheme.Red;
                _lblThongBao.ForeColor = UiTheme.Red;
                _lblThongBao.Text = tt.ThongBao;
            }
        }
    }
}
