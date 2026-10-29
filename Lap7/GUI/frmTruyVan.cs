using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Lab07_ADONET_Connection.BUS;
using Lab07_ADONET_Connection.GUI.Helpers;

namespace Lab07_ADONET_Connection.GUI
{
    /// <summary>
    /// frmTruyVan - Bai 5: minh hoa doc du lieu mau tu QLSinhVien.
    /// Cho phep chon bang (Khoa / SinhVien / HocPhan / DangKy) va do du lieu vao
    /// DataGridView qua SqlDataAdapter. Form chi goi BUS, khong tu viet SQL.
    /// </summary>
    public class frmTruyVan : FormBase
    {
        private readonly TruyVanBUS _bus = new TruyVanBUS();

        private InfoStrip _hero;
        private ComboBox _cboBang;
        private DataGridView _luoi;
        private Label _lblTong, _lblThongBao;
        private BadgeLabel _badge;

        public frmTruyVan()
        {
            InitializeComponent();
            Load += (s, e) => NapBang();
        }

        private void InitializeComponent()
        {
            Text = "Truy vấn dữ liệu mẫu - QLSinhVien";
            ClientSize = new Size(940, 620);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(820, 560);

            _hero = new InfoStrip();
            _hero.Dock = DockStyle.Top;
            _hero.Accent = UiTheme.Violet;
            _hero.ChipText = "5";
            _hero.Title = "Truy vấn dữ liệu mẫu QLSinhVien";
            _hero.Subtitle = "SqlDataAdapter · DataTable · DataGridView";
            _hero.RightTop = "TruyVanBUS";
            _hero.RightBottom = "dbo.Khoa / SinhVien / HocPhan / DangKy";

            var root = new Panel();
            root.Dock = DockStyle.Fill;
            root.BackColor = UiTheme.Canvas;
            root.Padding = new Padding(18);

            var lblChon = UiFactory.Label("Chọn bảng:", 9.5f, UiTheme.Ink, true);
            lblChon.SetBounds(18, 18, 90, 28);

            _cboBang = UiFactory.ComboBox("cboBang");
            _cboBang.SetBounds(110, 18, 220, 28);
            _cboBang.Items.AddRange(new object[] { "Khoa", "SinhVien", "HocPhan", "DangKy" });
            _cboBang.SelectedIndex = 0;
            _cboBang.SelectedIndexChanged += (s, e) => NapBang();

            var btnNap = new AccentButton();
            btnNap.Name = "btnNap";
            btnNap.Text = "Nạp dữ liệu";
            btnNap.BaseColor = UiTheme.Violet;
            btnNap.SetBounds(342, 16, 140, 32);
            btnNap.Click += (s, e) => NapBang();

            _badge = new BadgeLabel();
            _badge.BaseColor = UiTheme.Violet;
            _badge.Value = "0 dòng";
            _badge.SetBounds(500, 18, 110, 28);

            _lblTong = new Label();
            _lblTong.Text = "";
            _lblTong.Font = UiTheme.FontBody(9.5f);
            _lblTong.ForeColor = UiTheme.InkSoft;
            _lblTong.BackColor = Color.Transparent;
            _lblTong.SetBounds(624, 20, 300, 24);
            _lblTong.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            _luoi = new DataGridView();
            _luoi.Name = "luoiDuLieu";
            _luoi.SetBounds(18, 60, 904, 500);
            _luoi.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _luoi.BackgroundColor = Color.White;
            _luoi.BorderStyle = BorderStyle.FixedSingle;
            _luoi.RowHeadersVisible = false;
            _luoi.AllowUserToAddRows = false;
            _luoi.AllowUserToDeleteRows = false;
            _luoi.ReadOnly = true;
            _luoi.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _luoi.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _luoi.Font = UiTheme.FontBody(9.5f);
            _luoi.ColumnHeadersDefaultCellStyle.Font = UiTheme.FontBold(9.5f);
            _luoi.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(238, 234, 252);
            _luoi.EnableHeadersVisualStyles = false;
            _luoi.RowTemplate.Height = 26;
            _luoi.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 248, 254);
            _luoi.DataError += (s, e) => { e.ThrowException = false; };

            _lblThongBao = new Label();
            _lblThongBao.Name = "lblThongBao";
            _lblThongBao.Text = "";
            _lblThongBao.Font = UiTheme.FontBody(9f);
            _lblThongBao.ForeColor = UiTheme.Muted;
            _lblThongBao.BackColor = Color.Transparent;
            _lblThongBao.SetBounds(18, 566, 904, 40);
            _lblThongBao.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            root.Controls.AddRange(new Control[] { lblChon, _cboBang, btnNap, _badge, _lblTong, _luoi, _lblThongBao });

            Controls.Add(root);
            Controls.Add(_hero);
            _hero.BringToFront();
        }

        public void NapBang()
        {
            string tenBang = _cboBang.SelectedItem as string;
            try
            {
                DataTable dt = _bus.GetBang(tenBang);
                _luoi.DataSource = dt;

                _badge.Value = dt.Rows.Count + " dòng";
                _lblTong.Text = string.Format("Bảng dbo.{0}: {1} dòng · {2} cột", tenBang, dt.Rows.Count, dt.Columns.Count);
                _lblThongBao.ForeColor = UiTheme.Green;
                _lblThongBao.Text = string.Format("Đã nạp dữ liệu bảng {0} lúc {1}.", tenBang, DateTime.Now.ToString("HH:mm:ss"));
            }
            catch (Exception ex)
            {
                _luoi.DataSource = null;
                _badge.Value = "Lỗi";
                _badge.BaseColor = UiTheme.Red;
                _lblThongBao.ForeColor = UiTheme.Red;
                _lblThongBao.Text = "Không nạp được dữ liệu: " + ex.Message;
            }
        }
    }
}
