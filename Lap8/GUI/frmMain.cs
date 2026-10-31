using System;
using System.Drawing;
using System.Windows.Forms;
using Lab08_SqlCommand_Parameters.BUS;
using Lab08_SqlCommand_Parameters.DTO;
using Lab08_SqlCommand_Parameters.GUI.Helpers;

namespace Lab08_SqlCommand_Parameters.GUI
{
    /// <summary>
    /// frmMain - cua so chinh cua Lab 8 (MDI).
    ///
    /// Menu "Bài tập" mo 10 Form tuong ung 10 bai trong de cuong:
    ///   1. Đếm sinh viên (ExecuteScalar)
    ///   2. Đọc danh sách (ExecuteReader)
    ///   3. Thêm sinh viên (ExecuteNonQuery + tham số)
    ///   4. Cập nhật điểm (tham số)
    ///   5. Stored Procedure theo khoa
    ///   6. Tìm theo mã
    ///   7. Thống kê điểm cao nhất (NULL)
    ///   8. Xóa có xác nhận (khóa ngoại)
    ///   9. Stored Procedure + OUTPUT parameter
    ///  10. Lớp SqlHelper tối giản
    /// </summary>
    public class frmMain : FormBase
    {
        private readonly SinhVienBUS _bus = new SinhVienBUS();

        private MenuStrip _menu;
        private StatusStrip _status;
        private ToolStripStatusLabel _lblTrangThai;
        private ToolStripStatusLabel _lblDongHo;
        private InfoStrip _hero;
        private Label _lblTongSV, _lblSoBang, _lblKetNoi;
        private System.Windows.Forms.Timer _timer;

        public frmMain()
        {
            DungGiaoDien();
            Load += frmMain_Load;
            FormClosing += frmMain_FormClosing;
        }

        private void DungGiaoDien()
        {
            Text = "Lab 08 - SqlCommand, SqlParameter và các phương thức thực thi";
            ClientSize = new Size(1060, 660);
            MinimumSize = new Size(920, 600);
            StartPosition = FormStartPosition.CenterScreen;
            IsMdiContainer = true;

            // ---- MenuStrip ----
            _menu = new MenuStrip();
            _menu.Font = UiTheme.FontBody(9.5f);
            _menu.BackColor = Color.White;

            ToolStripMenuItem mnuBai = new ToolStripMenuItem("Bài tập");
            mnuBai.DropDownItems.Add(TaoMenu("Bài 1 - Đếm sinh viên (ExecuteScalar)", (s, e) => Mo(new frmBai1()), "SELECT COUNT(*)"));
            mnuBai.DropDownItems.Add(TaoMenu("Bài 2 - Đọc danh sách (ExecuteReader)", (s, e) => Mo(new frmBai2()), "SqlDataReader forward-only"));
            mnuBai.DropDownItems.Add(TaoMenu("Bài 3 - Thêm sinh viên (ExecuteNonQuery)", (s, e) => Mo(new frmBai3()), "INSERT tham số hóa"));
            mnuBai.DropDownItems.Add(TaoMenu("Bài 4 - Cập nhật điểm bằng tham số", (s, e) => Mo(new frmBai4()), "UPDATE ... WHERE MaSV = @MaSV"));
            mnuBai.DropDownItems.Add(new ToolStripSeparator());
            mnuBai.DropDownItems.Add(TaoMenu("Bài 5 - Stored Procedure theo khoa", (s, e) => Mo(new frmBai5()), "CommandType.StoredProcedure"));
            mnuBai.DropDownItems.Add(TaoMenu("Bài 6 - Tìm sinh viên theo mã", (s, e) => Mo(new frmBai6()), "ExecuteReader + SqlParameter"));
            mnuBai.DropDownItems.Add(TaoMenu("Bài 7 - Thống kê điểm cao nhất", (s, e) => Mo(new frmBai7()), "MAX(DiemTB) + DBNull"));
            mnuBai.DropDownItems.Add(TaoMenu("Bài 8 - Xóa sinh viên có xác nhận", (s, e) => Mo(new frmBai8()), "DELETE + FOREIGN KEY 547"));
            mnuBai.DropDownItems.Add(TaoMenu("Bài 9 - Tham số đầu ra (OUTPUT)", (s, e) => Mo(new frmBai9()), "ParameterDirection.Output"));
            mnuBai.DropDownItems.Add(TaoMenu("Bài 10 - Lớp SqlHelper tối giản", (s, e) => Mo(new frmBai10()), "Đóng gói 3 phương thức thực thi"));
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
            _hero.Accent = UiTheme.Teal;
            _hero.ChipText = "08";
            _hero.Title = "SqlCommand · SqlParameter · ExecuteReader / Scalar / NonQuery";
            _hero.Subtitle = "IT3212 - Lập trình .NET · Chương 4 · CSDL QLSinhVien (Lab 7)";
            _hero.RightTop = "System.Data.SqlClient";
            _hero.RightBottom = ".NET Framework 4.8";

            // ---- Dashboard ----
            var panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.BackColor = UiTheme.Canvas;
            panel.Padding = new Padding(18);

            var card = new CardPanel();
            card.SetBounds(18, 18, 1016, 132);
            card.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            Label lblTd = UiFactory.Label("TỔNG QUAN DỮ LIỆU", 8.5f, UiTheme.Teal, true);
            lblTd.SetBounds(24, 14, 400, 22);

            _lblTongSV = new Label();
            _lblTongSV.Text = "—";
            _lblTongSV.Font = UiTheme.FontTitle(34f);
            _lblTongSV.ForeColor = UiTheme.Ink;
            _lblTongSV.BackColor = Color.Transparent;
            _lblTongSV.SetBounds(22, 36, 240, 56);

            Label lblDv = UiFactory.Label("sinh viên trong dbo.SinhVien", 9.5f, UiTheme.Muted, false);
            lblDv.SetBounds(24, 96, 300, 24);

            _lblSoBang = UiFactory.Label("", 9.5f, UiTheme.InkSoft, false);
            _lblSoBang.SetBounds(300, 44, 480, 24);
            _lblSoBang.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            _lblKetNoi = UiFactory.Label("", 9.5f, UiTheme.InkSoft, false);
            _lblKetNoi.SetBounds(300, 74, 480, 40);
            _lblKetNoi.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            var btnLamMoi = new AccentButton();
            btnLamMoi.Name = "btnLamMoi";
            btnLamMoi.Text = "Làm mới số liệu";
            btnLamMoi.BaseColor = UiTheme.Teal;
            btnLamMoi.SetBounds(846, 40, 148, 38);
            btnLamMoi.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnLamMoi.Click += (s, e) => CapNhatSoLieu();

            var btnBai1 = new AccentButton();
            btnBai1.Name = "btnBai1";
            btnBai1.Text = "Mở Bài 1 (ExecuteScalar)";
            btnBai1.BaseColor = UiTheme.Violet;
            btnBai1.OutlineOnly = true;
            btnBai1.SetBounds(846, 84, 148, 32);
            btnBai1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBai1.Click += (s, e) => Mo(new frmBai1());

            card.Controls.AddRange(new Control[] { lblTd, _lblTongSV, lblDv, _lblSoBang, _lblKetNoi, btnLamMoi, btnBai1 });

            Label lblHuongDan = UiFactory.Label(
                "Mở menu “Bài tập” để chạy từng bài. Mỗi Form chỉ gọi tầng BUS; " +
                "BUS gọi DAL — nơi duy nhất chứa SqlCommand và SqlParameter.",
                9.5f, UiTheme.InkSoft, false);
            lblHuongDan.SetBounds(18, 168, 1016, 30);
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
            Log("Mở ứng dụng Lab 08 - SqlCommand & SqlParameter");
        }

        public void CapNhatSoLieu()
        {
            try
            {
                int soSV = _bus.CountStudents();
                _lblTongSV.Text = soSV.ToString();
                _lblSoBang.Text = "ExecuteScalar: SELECT COUNT(*) FROM dbo.SinhVien  →  " + soSV;
                _lblKetNoi.ForeColor = UiTheme.InkSoft;
                _lblKetNoi.Text = "Kết nối đọc từ App.config (khoá \"QLSinhVien\"). " +
                                  "Các bài 3, 4, 8, 9 sẽ thay đổi dữ liệu thật trong CSDL.";
            }
            catch (LoiNghiepVu ex)
            {
                _lblTongSV.Text = "?";
                _lblSoBang.Text = "Không đọc được dữ liệu.";
                _lblKetNoi.ForeColor = UiTheme.Red;
                _lblKetNoi.Text = ex.Message;
            }
            catch (Exception ex)
            {
                _lblTongSV.Text = "?";
                _lblSoBang.Text = "Lỗi không xác định.";
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
