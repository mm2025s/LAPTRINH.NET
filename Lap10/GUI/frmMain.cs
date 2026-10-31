using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Lab10_DisconnectedData.BUS;
using Lab10_DisconnectedData.DTO;
using Lab10_DisconnectedData.GUI.Helpers;

namespace Lab10_DisconnectedData.GUI
{
    /// <summary>
    /// frmMain - cua so chinh cua Lab 10 (MDI).
    ///
    /// Menu "Bài tập" mo 10 Form tuong ung 10 bai trong de cuong Chuong 4:
    ///   1. Fill DataTable bằng SqlDataAdapter
    ///   2. DataSet chứa nhiều bảng
    ///   3. Thiết lập DataRelation Khoa - SinhVien
    ///   4. Lọc và sắp xếp bằng DataView
    ///   5. Cập nhật bằng SqlCommandBuilder
    ///   6. Tạo DataTable thủ công          (tự thực hiện)
    ///   7. Theo dõi RowState                (tự thực hiện)
    ///   8. AcceptChanges và RejectChanges   (tự thực hiện)
    ///   9. Lưu và đọc DataSet XML           (tự thực hiện)
    ///  10. Hợp nhất dữ liệu bằng Merge      (tự thực hiện)
    /// </summary>
    public class frmMain : FormBase
    {
        private readonly SinhVienBUS _bus = new SinhVienBUS();

        private MenuStrip _menu;
        private StatusStrip _status;
        private ToolStripStatusLabel _lblTrangThai;
        private ToolStripStatusLabel _lblDongHo;
        private InfoStrip _hero;
        private Label _lblTongSV, _lblThongTin, _lblCheDo;
        private System.Windows.Forms.Timer _timer;

        /// <summary>
        /// Bat/tat hop thoai xac nhan khi dong Form.
        /// Che do chup anh bao cao dat = false de tien trinh con thoat ngay,
        /// khong bi treo 45 giay vi hop thoai khong ai bam.
        /// </summary>
        public bool HoiXacNhanKhiDong { get; set; }

        public frmMain()
        {
            HoiXacNhanKhiDong = true;
            DungGiaoDien();
            Load += frmMain_Load;
            FormClosing += frmMain_FormClosing;
        }

        private void DungGiaoDien()
        {
            Text = "Lab 10 - DataAdapter, DataTable, DataSet và dữ liệu ngắt kết nối";
            ClientSize = new Size(1080, 680);
            MinimumSize = new Size(940, 620);
            StartPosition = FormStartPosition.CenterScreen;
            IsMdiContainer = true;

            // ---- MenuStrip ----
            _menu = new MenuStrip();
            _menu.Font = UiTheme.FontBody(9.5f);
            _menu.BackColor = Color.White;

            ToolStripMenuItem mnuBai = new ToolStripMenuItem("Bài tập");
            mnuBai.DropDownItems.Add(TaoMenu("Bài 1 - Fill DataTable bằng SqlDataAdapter", (s, e) => Mo(new frmBai1()), "adapter.Fill(table) - tự mở/đóng kết nối"));
            mnuBai.DropDownItems.Add(TaoMenu("Bài 2 - DataSet chứa nhiều bảng", (s, e) => Mo(new frmBai2()), "ds.Tables[\"Khoa\"] và ds.Tables[\"SinhVien\"]"));
            mnuBai.DropDownItems.Add(TaoMenu("Bài 3 - Thiết lập DataRelation", (s, e) => Mo(new frmBai3()), "GetChildRows + ForeignKeyConstraint"));
            mnuBai.DropDownItems.Add(TaoMenu("Bài 4 - Lọc và sắp xếp bằng DataView", (s, e) => Mo(new frmBai4()), "RowFilter + Sort, không truy vấn lại CSDL"));
            mnuBai.DropDownItems.Add(TaoMenu("Bài 5 - Cập nhật bằng SqlCommandBuilder", (s, e) => Mo(new frmBai5()), "adapter.Update + transaction"));
            mnuBai.DropDownItems.Add(new ToolStripSeparator());
            mnuBai.DropDownItems.Add(TaoMenu("Bài 6 - Tạo DataTable thủ công", (s, e) => Mo(new frmBai6()), "DataColumn.Expression + Constraint"));
            mnuBai.DropDownItems.Add(TaoMenu("Bài 7 - Theo dõi RowState", (s, e) => Mo(new frmBai7()), "Đếm Added/Modified/Deleted + GetChanges()"));
            mnuBai.DropDownItems.Add(TaoMenu("Bài 8 - AcceptChanges và RejectChanges", (s, e) => Mo(new frmBai8()), "chốt / hủy thay đổi trong bộ nhớ"));
            mnuBai.DropDownItems.Add(TaoMenu("Bài 9 - Lưu và đọc DataSet XML", (s, e) => Mo(new frmBai9()), "WriteXml + WriteSchema"));
            mnuBai.DropDownItems.Add(TaoMenu("Bài 10 - Hợp nhất dữ liệu (Merge)", (s, e) => Mo(new frmBai10()), "Merge theo PrimaryKey"));
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
            _hero.Accent = UiTheme.Violet;
            _hero.ChipText = "10";
            _hero.Title = "DataAdapter · DataTable · DataSet · DataRelation · DataView";
            _hero.Subtitle = "IT3212 - Lập trình .NET · Chương 4 · CSDL QLSinhVien";
            _hero.RightTop = "Ngắt kết nối";
            _hero.RightBottom = ".NET Framework 4.8";

            // ---- Dashboard ----
            var panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.BackColor = UiTheme.Canvas;
            panel.Padding = new Padding(18);

            var card = new CardPanel();
            card.SetBounds(18, 18, 1036, 148);
            card.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            Label lblTd = UiFactory.Label("TỔNG QUAN DỮ LIỆU", 8.5f, UiTheme.Violet, true);
            lblTd.SetBounds(24, 14, 400, 22);

            _lblTongSV = new Label();
            _lblTongSV.Text = "—";
            _lblTongSV.Font = UiTheme.FontTitle(34f);
            _lblTongSV.ForeColor = UiTheme.Ink;
            _lblTongSV.BackColor = Color.Transparent;
            _lblTongSV.SetBounds(22, 40, 200, 56);

            Label lblDv = UiFactory.Label("sinh viên trong dbo.SinhVien", 9.5f, UiTheme.Muted, false);
            lblDv.SetBounds(24, 100, 300, 24);

            _lblThongTin = UiFactory.Label("", 9.5f, UiTheme.InkSoft, false);
            _lblThongTin.SetBounds(230, 46, 560, 44);
            _lblThongTin.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            _lblCheDo = UiFactory.Label("", 8.75f, UiTheme.Muted, false);
            _lblCheDo.SetBounds(24, 124, 760, 20);
            _lblCheDo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            var btnLamMoi = new AccentButton();
            btnLamMoi.Name = "btnLamMoi";
            btnLamMoi.Text = "Làm mới số liệu";
            btnLamMoi.BaseColor = UiTheme.Violet;
            btnLamMoi.SetBounds(866, 40, 148, 38);
            btnLamMoi.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnLamMoi.Click += (s, e) => CapNhatSoLieu();

            var btnBai4 = new AccentButton();
            btnBai4.Name = "btnBai4";
            btnBai4.Text = "Mở Bài 4 (DataView)";
            btnBai4.BaseColor = UiTheme.Amber;
            btnBai4.OutlineOnly = true;
            btnBai4.SetBounds(866, 84, 148, 32);
            btnBai4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBai4.Click += (s, e) => Mo(new frmBai4());

            card.Controls.AddRange(new Control[] { lblTd, _lblTongSV, lblDv, _lblThongTin, _lblCheDo, btnLamMoi, btnBai4 });

            Label lblHuongDan = UiFactory.Label(
                "Mở menu “Bài tập” để chạy từng bài. Dữ liệu được Fill một lần từ CSDL rồi xử lý hoàn toàn " +
                "trong bộ nhớ; chỉ Bài 5 mới ghi ngược lại CSDL.",
                9.5f, UiTheme.InkSoft, false);
            lblHuongDan.SetBounds(18, 180, 1036, 30);
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
            Log("Mở ứng dụng Lab 10 - DataAdapter, DataTable, DataSet");
        }

        public void CapNhatSoLieu()
        {
            try
            {
                DataSet ds = _bus.GetDataSet();

                DataTable sv = ds.Tables["SinhVien"];
                DataTable khoa = ds.Tables["Khoa"];

                _lblTongSV.Text = sv.Rows.Count.ToString();

                _lblThongTin.Text = string.Format(
                    "DataSet \"{0}\" · {1} bảng (Khoa {2} dòng, SinhVien {3} dòng)\n" +
                    "Kiểu cột DiemTB = {4} · PrimaryKey = {5}",
                    ds.DataSetName, ds.Tables.Count, khoa.Rows.Count, sv.Rows.Count,
                    sv.Columns["DiemTB"].DataType.Name,
                    sv.PrimaryKey.Length > 0 ? sv.PrimaryKey[0].ColumnName : "(chưa đặt)");

                _lblCheDo.ForeColor = UiTheme.Muted;
                _lblCheDo.Text = "Chuỗi kết nối đọc từ App.config (khoá \"QLSinhVien\") — " +
                    "dữ liệu nằm trong bộ nhớ sau khi Fill, không cần kết nối để lọc/sắp xếp.";
            }
            catch (Exception ex)
            {
                _lblTongSV.Text = "?";
                _lblThongTin.Text = "Không đọc được dữ liệu.";
                _lblCheDo.ForeColor = UiTheme.Red;
                _lblCheDo.Text = ex.Message;
            }
        }

        public void Log(string text)
        {
            _lblTrangThai.Text = DateTime.Now.ToString("HH:mm:ss") + "  ·  " + text;
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing) return;
            if (!HoiXacNhanKhiDong) return;

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
