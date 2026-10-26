using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Lab06_ThreeLayer.BUS;
using Lab06_ThreeLayer.DAL;
using Lab06_ThreeLayer.DTO;
using Lab06_ThreeLayer.GUI.Helpers;

namespace Lab06_ThreeLayer.GUI
{
    /// <summary>
    /// frmMain - Bài 1 (giới thiệu mô hình 3 lớp) và Bài 6 (Dashboard tổng hợp).
    ///
    /// Ba tầng được khởi tạo DUY NHẤT MỘT LẦN ở đây rồi truyền xuống các Form con:
    ///   Form (GUI) -> BUS -> DAL -> dữ liệu trong bộ nhớ
    /// Nhờ vậy mọi Form con cùng nhìn thấy một kho dữ liệu chung mà không cần biến static.
    /// Form con KHÔNG BAO GIỜ gọi DAL trực tiếp.
    /// </summary>
    public partial class frmMain : FormBase
    {
        // Giữ tham chiếu tới DAL để các BUS dùng chung một kho dữ liệu
        private readonly SinhVienDAL _dalSV = new SinhVienDAL();
        private readonly SanPhamDAL _dalSP = new SanPhamDAL();
        private readonly DanhMucDAL _dalDM = new DanhMucDAL();

        private readonly SinhVienBUS _busSV;
        private readonly SanPhamBUS _busSP;
        private readonly DanhMucBUS _busDanhMuc;
        private readonly KhoaBUS _busKhoa = new KhoaBUS();

        private readonly System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer();
        private string _trangThai = "Sẵn sàng";
        private int _soLanMoForm;

        public frmMain()
        {
            InitializeComponent();

            // Tiêm DAL vào BUS: cùng một DAL => cùng một kho dữ liệu
            _busSV = new SinhVienBUS(_dalSV);
            _busSP = new SanPhamBUS(_dalSP, _dalDM);
            _busDanhMuc = new DanhMucBUS(_dalDM, _dalSP);

            // Nạp dữ liệu mẫu qua BUS (dữ liệu mẫu cũng phải qua kiểm tra nghiệp vụ)
            DuLieuMau.NapSinhVien(_busSV);
            DuLieuMau.NapSanPham(_busSP, _busDanhMuc);
        }

        /// <summary>BUS dùng chung cho các Form con (mở từ menu).</summary>
        public SinhVienBUS BusSinhVien { get { return _busSV; } }
        public KhoaBUS BusKhoa { get { return _busKhoa; } }

        // =====================================================================
        // KHỞI TẠO
        // =====================================================================
        private void frmMain_Load(object sender, EventArgs e)
        {
            foreach (Control c in Controls)
            {
                MdiClient client = c as MdiClient;
                if (client != null)
                {
                    client.BackColor = Color.FromArgb(236, 239, 247);
                    client.Resize += MdiClient_Resize;
                    break;
                }
            }

            pnlDashboard.Parent = this;
            pnlDashboard.Bounds = ClientRectangle;
            pnlDashboard.BringToFront();

            SapXepDashboard();
            CapNhatDashboard();
            DongHo_Tick(null, EventArgs.Empty);

            _timer.Interval = 1000;
            _timer.Tick += DongHo_Tick;
            _timer.Start();

            Log("Khởi động Lab 6 - ứng dụng 3 lớp với DataGridView");
        }

        private void MdiClient_Resize(object sender, EventArgs e)
        {
            SapXepDashboard();
        }

        // =====================================================================
        // BÀI 6: DASHBOARD - tổng hợp số liệu từ BUS
        // =====================================================================
        private void SapXepDashboard()
        {
            int width = Math.Max(ClientSize.Width, 900);
            int left = 20;
            int top = 20;
            int cardW = 216;
            int gap = 14;

            CardPanel[] cards = { card1, card2, card3, card4 };
            for (int i = 0; i < cards.Length; i++)
                cards[i].SetBounds(left + i * (cardW + gap), top, cardW, 96);

            cardTomTat.SetBounds(left, top + 116, Math.Max(560, width - left * 2 - 20), 236);
        }

        private void CapNhatDashboard()
        {
            DateTime batDau = DateTime.Now;
            List<SinhVien> dsSV = _busSV.GetAll();
            List<SanPham> dsSP = _busSP.GetAll();

            double diemTB = 0;
            foreach (SinhVien sv in dsSV) diemTB += sv.DiemTB;
            if (dsSV.Count > 0) diemTB = diemTB / dsSV.Count;

            lblStatSV.Text = dsSV.Count.ToString();
            lblStatDiem.Text = diemTB.ToString("0.00");
            lblStatSP.Text = dsSP.Count.ToString();
            lblStatTon.Text = DinhDang.Tien(_busSP.TongGiaTriTon(dsSP));

            // Bảng đếm theo khoa - dữ liệu lấy qua BUS.NhomTheoKhoa()
            Dictionary<string, List<SinhVien>> theoKhoa = _busSV.NhomTheoKhoa();
            lstTheoKhoa.Items.Clear();
            foreach (KeyValuePair<string, List<SinhVien>> cap in theoKhoa)
            {
                lstTheoKhoa.Items.Add(string.Format("{0,-30}|  {1,3} sinh viên", cap.Key, cap.Value.Count));
            }

            long ms = (long)(DateTime.Now - batDau).TotalMilliseconds;
            lblTomTatPhu.Text = string.Format(
                "Toàn bộ số liệu được tính ở tầng BUS trong {0} ms - GUI không tự tính nghiệp vụ.", ms);
        }

        private void DongHo_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            tsslDongHo.Text = " " + now.ToString("HH:mm:ss") + "  " + now.ToString("dddd, dd/MM/yyyy");
            tsslTrangThai.Text = " " + _trangThai;
            tsslSoLanMo.Text = string.Format("Đã mở {0} form", _soLanMoForm);
        }

        // =====================================================================
        // MỞ FORM CON (MDI - không mở trùng)
        // =====================================================================
        private void OpenChildForm(Form child)
        {
            foreach (Form f in MdiChildren)
            {
                if (f.GetType() == child.GetType())
                {
                    f.Activate();
                    child.Dispose();
                    _trangThai = "Form đã mở: " + f.Text;
                    Log("Không mở trùng Form: " + f.Text);
                    return;
                }
            }

            child.MdiParent = this;
            child.Show();
            _soLanMoForm++;
            Log("Mở form con: " + child.Text);

            MenuCuaSoItem(child);
            child.FormClosed += FormCon_FormClosed;
            pnlDashboard.Visible = false;
        }

        private void FormCon_FormClosed(object sender, FormClosedEventArgs e)
        {
            bool conFormCon = MdiChildren.Length > 0;
            if (!conFormCon)
            {
                pnlDashboard.Visible = true;
                pnlDashboard.BringToFront();
            }
            CapNhatDashboard();
        }

        /// <summary>Thêm Form con vào menu "Cửa sổ" để chuyển nhanh giống Visual Studio.</summary>
        private void MenuCuaSoItem(Form child)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(child.Text);
            item.Tag = child;
            item.Click += delegate
            {
                if (!child.IsDisposed)
                {
                    child.Activate();
                    if (child.WindowState == FormWindowState.Minimized) child.WindowState = FormWindowState.Normal;
                }
            };
            menuCuaSo.DropDownItems.Add(item);
        }

        // =====================================================================
        // SỰ KIỆN MENU
        // =====================================================================
        private void mnuSinhVien_Click(object sender, EventArgs e)
        {
            OpenChildForm(new frmSinhVien(_busSV, _busKhoa));
        }

        private void mnuSanPham_Click(object sender, EventArgs e)
        {
            OpenChildForm(new frmSanPham(_busSP));
        }

        private void mnuDanhMuc_Click(object sender, EventArgs e)
        {
            OpenChildForm(new frmDanhMuc(_busDanhMuc));
        }

        private void mnuDanhMucSinhVien_Click(object sender, EventArgs e)
        {
            OpenChildForm(new frmDanhMucSinhVien(_busSV));
        }

        private void mnuBieuTuong_Click(object sender, EventArgs e)
        {
            OpenChildForm(new frmSinhVienIcon(_busSV));
        }

        private void mnuTrangChu_Click(object sender, EventArgs e)
        {
            for (int i = MdiChildren.Length - 1; i >= 0; i--) MdiChildren[i].Close();
            pnlDashboard.Visible = true;
            pnlDashboard.BringToFront();
            Log("Về trang chủ - đã đóng các form con");
        }

        private void mnuThoat_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void mnuKienTruc_Click(object sender, EventArgs e)
        {
            using (var f = new frmKienTruc())
            {
                f.ShowDialog(this);
            }
        }

        private void mnuGioiThieu_Click(object sender, EventArgs e)
        {
            using (var f = new frmGioiThieu())
            {
                f.ShowDialog(this);
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Giữ cơ chế xác nhận thoát của Lab 5: chọn No => e.Cancel = true
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult r = MessageBox.Show(this,
                    "Bạn có chắc chắn muốn thoát ứng dụng không?",
                    "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);

                if (r != DialogResult.Yes)
                {
                    e.Cancel = true;
                    _trangThai = "Hủy lệnh thoát (e.Cancel = true)";
                    Log("Người dùng chọn No ở hộp thoại thoát -> e.Cancel = true");
                    return;
                }
            }
            Log("Thoát ứng dụng");
        }

        // =====================================================================
        // NÚT NHANH TRÊN DASHBOARD
        // =====================================================================
        private void btnMoSinhVien_Click(object sender, EventArgs e)
        {
            mnuSinhVien_Click(sender, e);
        }

        private void btnMoSanPham_Click(object sender, EventArgs e)
        {
            mnuSanPham_Click(sender, e);
        }

        private void btnMoDanhMuc_Click(object sender, EventArgs e)
        {
            mnuDanhMuc_Click(sender, e);
        }

        private void btnMoBieuTuong_Click(object sender, EventArgs e)
        {
            mnuBieuTuong_Click(sender, e);
        }

        private void pnlHero_Paint(object sender, PaintEventArgs e)
        {
            PanelPaint.GradientAndBubbles(sender, e);
        }

        // =====================================================================
        // TIỆN ÍCH
        // =====================================================================
        /// <summary>Hiện thông báo ngắn trên StatusStrip và ghi Console.</summary>
        public void Log(string text)
        {
            _trangThai = text;
            if (tsslTrangThai != null) tsslTrangThai.Text = " " + text;
            Console.WriteLine("[frmMain] " + text);
        }

        /// <summary>Form con gọi để Dashboard cập nhật lại số liệu.</summary>
        public void YeuCauCapNhat()
        {
            CapNhatDashboard();
        }

        private class MenuColorTable : ProfessionalColorTable
        {
            public override Color MenuItemSelected { get { return UiTheme.Tint(UiTheme.Teal, 0.16); } }
            public override Color MenuItemSelectedGradientBegin { get { return UiTheme.Tint(UiTheme.Teal, 0.16); } }
            public override Color MenuItemSelectedGradientEnd { get { return UiTheme.Tint(UiTheme.Teal, 0.16); } }
            public override Color MenuItemBorder { get { return UiTheme.Tint(UiTheme.Teal, 0.4); } }
            public override Color MenuBorder { get { return UiTheme.Border; } }
            public override Color ToolStripDropDownBackground { get { return Color.White; } }
            public override Color ImageMarginGradientBegin { get { return Color.White; } }
            public override Color ImageMarginGradientMiddle { get { return Color.White; } }
            public override Color ImageMarginGradientEnd { get { return Color.White; } }
        }
    }
}
