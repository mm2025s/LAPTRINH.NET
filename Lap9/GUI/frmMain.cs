using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Lab09_DataReaderSearch.BUS;
using Lab09_DataReaderSearch.DTO;
using Lab09_DataReaderSearch.GUI.Helpers;

namespace Lab09_DataReaderSearch.GUI
{
    /// <summary>
    /// frmMain - cua so chinh cua Lab 9 (MDI).
    ///
    /// Menu "Bài tập" mo 10 Form tuong ung 10 bai trong de cuong Chuong 4:
    ///   1. Nạp SqlDataReader vào DataTable
    ///   2. Tìm kiếm gần đúng theo họ tên
    ///   3. Lọc theo khoa và khoảng điểm
    ///   4. Phân trang bằng OFFSET - FETCH
    ///   5. Hiển thị chi tiết dòng được chọn
    ///   6. Tìm kiếm đa tiêu chí            (tự thực hiện)
    ///   7. Sắp xếp theo lựa chọn           (tự thực hiện)
    ///   8. Lọc theo tuổi                   (tự thực hiện)
    ///   9. Định dạng DataGridView          (tự thực hiện)
    ///  10. Xuất kết quả tìm kiếm ra CSV    (tự thực hiện)
    /// </summary>
    public class frmMain : FormBase
    {
        private readonly SinhVienBUS _bus = new SinhVienBUS();

        private MenuStrip _menu;
        private StatusStrip _status;
        private ToolStripStatusLabel _lblTrangThai;
        private ToolStripStatusLabel _lblDongHo;
        private InfoStrip _hero;
        private Label _lblTongSV, _lblSoKhoa, _lblSoTrang, _lblKetNoi;
        private System.Windows.Forms.Timer _timer;

        public frmMain()
        {
            DungGiaoDien();
            Load += frmMain_Load;
            FormClosing += frmMain_FormClosing;
        }

        private void DungGiaoDien()
        {
            Text = "Lab 09 - Đọc, tìm kiếm và lọc dữ liệu bằng SqlDataReader";
            ClientSize = new Size(1080, 680);
            MinimumSize = new Size(940, 620);
            StartPosition = FormStartPosition.CenterScreen;
            IsMdiContainer = true;

            // ---- MenuStrip ----
            _menu = new MenuStrip();
            _menu.Font = UiTheme.FontBody(9.5f);
            _menu.BackColor = Color.White;

            ToolStripMenuItem mnuBai = new ToolStripMenuItem("Bài tập");
            mnuBai.DropDownItems.Add(TaoMenu("Bài 1 - Nạp SqlDataReader vào DataTable", (s, e) => Mo(new frmBai1()), "DataTable.Load(reader) + nạp thủ công"));
            mnuBai.DropDownItems.Add(TaoMenu("Bài 2 - Tìm kiếm gần đúng theo họ tên", (s, e) => Mo(new frmBai2()), "LIKE @Keyword"));
            mnuBai.DropDownItems.Add(TaoMenu("Bài 3 - Lọc theo khoa và khoảng điểm", (s, e) => Mo(new frmBai3()), "WHERE động + BETWEEN"));
            mnuBai.DropDownItems.Add(TaoMenu("Bài 4 - Phân trang bằng OFFSET-FETCH", (s, e) => Mo(new frmBai4()), "OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY"));
            mnuBai.DropDownItems.Add(TaoMenu("Bài 5 - Chi tiết dòng được chọn", (s, e) => Mo(new frmBai5()), "SelectionChanged + DBNull"));
            mnuBai.DropDownItems.Add(new ToolStripSeparator());
            mnuBai.DropDownItems.Add(TaoMenu("Bài 6 - Tìm kiếm đa tiêu chí", (s, e) => Mo(new frmBai6()), "6 tiêu chí, bỏ trống = không lọc"));
            mnuBai.DropDownItems.Add(TaoMenu("Bài 7 - Sắp xếp theo lựa chọn", (s, e) => Mo(new frmBai7()), "whitelist cột - không dùng tham số cho tên cột"));
            mnuBai.DropDownItems.Add(TaoMenu("Bài 8 - Lọc theo tuổi", (s, e) => Mo(new frmBai8()), "đổi tuổi thành khoảng NgàySinh"));
            mnuBai.DropDownItems.Add(TaoMenu("Bài 9 - Định dạng DataGridView", (s, e) => Mo(new frmBai9()), "CellFormatting + tô màu theo điểm"));
            mnuBai.DropDownItems.Add(TaoMenu("Bài 10 - Xuất kết quả ra CSV", (s, e) => Mo(new frmBai10()), "UTF-8 có BOM cho Excel"));
            _menu.Items.Add(mnuBai);

            ToolStripMenuItem mnuCuaSo = new ToolStripMenuItem("Cửa sổ");
            mnuCuaSo.DropDownItems.Add(TaoMenu("Sắp xếp (Cascade)", (s, e) => { LayoutMdi(MdiLayout.Cascade); Log("Sắp xếp cửa sổ con"); }, ""));
            mnuCuaSo.DropDownItems.Add(TaoMenu("Đóng tất cả", (s, e) => { foreach (Form f in MdiChildren) f.Close(); }, ""));
            _menu.Items.Add(mnuCuaSo);

            _menu.Items.Add(TaoMenu("Thoát", (s, e) => Close(), "Thoát ứng dụng"));

            // ---- StatusStrip ----
            _status = new StatusStrip();
            _status.BackColor = Color.White;
            _lblTrangThai = new ToolStripStatusLabel("Sẵn sàng");
            _lblTrangThai.Spring = true;
            _lblTrangThai.TextAlign = ContentAlignment.MiddleLeft;
            _lblDongHo = new ToolStripStatusLabel(string.Empty);
            _status.Items.AddRange(new ToolStripItem[] { _lblTrangThai, _lblDongHo });

            // ---- Hero ----
            _hero = new InfoStrip();
            _hero.Dock = DockStyle.Top;
            _hero.Accent = UiTheme.Blue;
            _hero.ChipText = "09";
            _hero.Title = "SqlDataReader · Tìm kiếm · Lọc · Sắp xếp · Phân trang";
            _hero.Subtitle = "IT3212 - Lập trình .NET · Chương 4 · CSDL QLSinhVien";
            _hero.RightTop = "OFFSET / FETCH";
            _hero.RightBottom = ".NET Framework 4.8";

            // ---- Dashboard ----
            var panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.BackColor = UiTheme.Canvas;
            panel.Padding = new Padding(18);

            var card = new CardPanel();
            card.SetBounds(18, 18, 1036, 132);
            card.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            Label lblTd = UiFactory.Label("TỔNG QUAN DỮ LIỆU", 8.5f, UiTheme.Blue, true);
            lblTd.SetBounds(24, 14, 400, 22);

            _lblTongSV = new Label();
            _lblTongSV.Text = "—";
            _lblTongSV.Font = UiTheme.FontTitle(34f);
            _lblTongSV.ForeColor = UiTheme.Ink;
            _lblTongSV.BackColor = Color.Transparent;
            _lblTongSV.SetBounds(22, 36, 200, 56);

            Label lblDv = UiFactory.Label("sinh viên trong dbo.SinhVien", 9.5f, UiTheme.Muted, false);
            lblDv.SetBounds(24, 96, 300, 24);

            _lblSoKhoa = UiFactory.Label("", 9.5f, UiTheme.InkSoft, false);
            _lblSoKhoa.SetBounds(230, 44, 560, 24);
            _lblSoKhoa.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            _lblSoTrang = UiFactory.Label("", 9.5f, UiTheme.InkSoft, false);
            _lblSoTrang.SetBounds(230, 74, 560, 24);
            _lblSoTrang.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            _lblKetNoi = UiFactory.Label("", 8.75f, UiTheme.Muted, false);
            _lblKetNoi.SetBounds(24, 108, 760, 20);
            _lblKetNoi.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            var btnLamMoi = new AccentButton();
            btnLamMoi.Name = "btnLamMoi";
            btnLamMoi.Text = "Làm mới số liệu";
            btnLamMoi.BaseColor = UiTheme.Blue;
            btnLamMoi.SetBounds(866, 40, 148, 38);
            btnLamMoi.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnLamMoi.Click += (s, e) => CapNhatSoLieu();

            var btnBai4 = new AccentButton();
            btnBai4.Name = "btnBai4";
            btnBai4.Text = "Mở Bài 4 (phân trang)";
            btnBai4.BaseColor = UiTheme.Amber;
            btnBai4.OutlineOnly = true;
            btnBai4.SetBounds(866, 84, 148, 32);
            btnBai4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBai4.Click += (s, e) => Mo(new frmBai4());

            card.Controls.AddRange(new Control[] { lblTd, _lblTongSV, lblDv, _lblSoKhoa, _lblSoTrang, _lblKetNoi, btnLamMoi, btnBai4 });

            Label lblHuongDan = UiFactory.Label(
                "Mở menu “Bài tập” để chạy từng bài. Mỗi Form chỉ gọi tầng BUS; BUS gọi DAL — " +
                "nơi duy nhất chứa SqlCommand và SqlDataReader.",
                9.5f, UiTheme.InkSoft, false);
            lblHuongDan.SetBounds(18, 168, 1036, 30);
            lblHuongDan.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            panel.Controls.AddRange(new Control[] { card, lblHuongDan });

            Controls.Add(panel);
            Controls.Add(_hero);
            Controls.Add(_status);
            Controls.Add(_menu);
            MainMenuStrip = _menu;
            _hero.BringToFront();

            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 1000;
            _timer.Tick += (s, e) => _lblDongHo.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            _timer.Start();
        }

        private static ToolStripMenuItem TaoMenu(string text, EventHandler handler, string tooltip)
        {
            var item = new ToolStripMenuItem(text);
            item.ToolTipText = tooltip;
            item.Click += handler;
            return item;
        }

        /// <summary>Mo Form con trong MDI, tranh mo trung 2 lan cung 1 Form.</summary>
        public void Mo(Form f)
        {
            foreach (Form con in MdiChildren)
            {
                if (con.GetType() == f.GetType())
                {
                    f.Dispose();
                    con.Activate();
                    con.WindowState = FormWindowState.Maximized;
                    Log("Chuyển tới cửa sổ đang mở: " + con.Text);
                    return;
                }
            }

            f.MdiParent = this;
            f.WindowState = FormWindowState.Maximized;
            f.Show();
            Log("Mở: " + f.Text);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            CapNhatSoLieu();
            Log("Mở ứng dụng Lab 09 - SqlDataReader, tìm kiếm và lọc dữ liệu");
        }

        public void CapNhatSoLieu()
        {
            try
            {
                DataTable table = _bus.LoadStudents();
                _lblTongSV.Text = table.Rows.Count.ToString();

                int soKhoa = 0;
                var daDem = new System.Collections.Generic.HashSet<string>();
                foreach (DataRow row in table.Rows)
                {
                    string ma = Convert.ToString(row["MaKhoa"]);
                    if (daDem.Add(ma)) soKhoa++;
                }

                int soTrang = (int)Math.Ceiling(table.Rows.Count / (double)SinhVienBUS.KichThuocTrangMacDinh);

                _lblSoKhoa.Text = string.Format(
                    "ExecuteReader → DataTable.Load(reader): {0} dòng · {1} cột · {2} khoa",
                    table.Rows.Count, table.Columns.Count, soKhoa);

                _lblSoTrang.Text = string.Format(
                    "Phân trang 10 dòng/trang → {0} trang (OFFSET 0 / 10 / 20 … FETCH NEXT 10 ROWS ONLY)",
                    soTrang);

                _lblKetNoi.ForeColor = UiTheme.Muted;
                _lblKetNoi.Text = "Chuỗi kết nối đọc từ App.config (khoá \"QLSinhVien\") — " +
                                  "10 bài của Lab 9 chỉ ĐỌC dữ liệu, không sửa CSDL.";
            }
            catch (Exception ex)
            {
                _lblTongSV.Text = "?";
                _lblSoKhoa.Text = "Không đọc được dữ liệu.";
                _lblSoTrang.Text = "";
                _lblKetNoi.ForeColor = UiTheme.Red;
                _lblKetNoi.Text = ex.Message;
            }
        }

        public void Log(string text)
        {
            _lblTrangThai.Text = DateTime.Now.ToString("HH:mm:ss") + "  ·  " + text;
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing) return;

            DialogResult r = MessageBox.Show(this,
                "Bạn có chắc chắn muốn thoát ứng dụng không?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (r != DialogResult.Yes)
            {
                e.Cancel = true;
                Log("Người dùng chọn No → e.Cancel = true");
            }
        }
    }
}
