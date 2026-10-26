using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Lab06_ThreeLayer.BUS;
using Lab06_ThreeLayer.DTO;
using Lab06_ThreeLayer.GUI.Helpers;

namespace Lab06_ThreeLayer.GUI
{
    /// <summary>
    /// frmSinhVien - Bài 2, 3, 5, 9.
    ///
    /// Vì sao có thể thoát khỏi Form này bằng phím Esc, và một vài trạng thái (chế độ sửa, xếp)
    /// được kết hợp ở code-behind: ở đây CHỈ là khai báo/phối trí giao diện, mọi câu hỏi
    /// "dữ liệu hợp lệ chưa?" đều do BUS trả lời.
    /// </summary>
    public partial class frmSinhVien : FormBase
    {
        private System.ComponentModel.IContainer components = null;

        // Trạng thái sắp xếp lưới: Form chỉ nhớ tiêu chí, việc sắp xếp do BUS làm
        private string luoiXepTheo;
        private bool luoiGiam;

        private InfoStrip infoStrip;

        // Cột bên trái: nhập liệu
        private CardPanel cardNhap;
        private Label lblNhapTieuDe, lblTrangThai, lblLoi;
        private Label lbMaSV, lbHoTen, lbNgaySinh, lbGioiTinh, lbDiemTB, lbEmail, lbKhoa;
        private TextBox txtMaSV, txtHoTen, txtDiemTB, txtEmail;
        private DateTimePicker dtpNgaySinh;
        private RadioButton radNam, radNu;
        private ComboBox cboKhoa;
        private AccentButton btnThem, btnLuu, btnSua, btnXoa;

        // Cột bên phải: tìm kiếm + lưới + CSV
        private CardPanel cardLuoi;
        private Label lblLuoiTieuDe, lblDem, lblTapTin;
        private TextBox txtTimKiem;
        private ComboBox cboKhoaFilter;
        private AccentButton btnXoaLoc, btnXuatCSV, btnNhapCSV;
        private DataGridView dgvSinhVien;
        private BindingSource bindingSource;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new Container();
            bindingSource = new BindingSource(components);

            // ============================ HEADER ============================
            infoStrip = new InfoStrip();
            infoStrip.Dock = DockStyle.Top;
            infoStrip.Height = 90;
            infoStrip.Accent = UiTheme.Teal;
            infoStrip.ChipText = "6";
            infoStrip.Title = "Quản lý sinh viên";
            infoStrip.Subtitle = "DataGridView + BindingSource · Form gọi BUS, BUS gọi DAL · ErrorProvider nhận lỗi từ BUS";
            infoStrip.RightTop = "Mô hình 3 lớp";
            infoStrip.RightBottom = "Nhấn tiêu đề cột để sắp xếp · Ctrl+S để lưu";

            // ============================ CỘT NHẬP LIỆU ============================
            cardNhap = new CardPanel();
            cardNhap.SetBounds(22, 104, 330, 430);

            lblNhapTieuDe = new Label();
            lblNhapTieuDe.Text = "Thông tin sinh viên";
            lblNhapTieuDe.Font = UiTheme.FontBold(11f);
            lblNhapTieuDe.ForeColor = UiTheme.Ink;
            lblNhapTieuDe.AutoSize = true;
            lblNhapTieuDe.BackColor = Color.Transparent;
            lblNhapTieuDe.Location = new Point(18, 14);

            lblTrangThai = new Label();
            lblTrangThai.Text = "Đang thêm mới";
            lblTrangThai.Font = UiTheme.FontBold(8.75f);
            lblTrangThai.ForeColor = UiTheme.Green;
            lblTrangThai.AutoSize = true;
            lblTrangThai.BackColor = Color.Transparent;
            lblTrangThai.Location = new Point(18, 40);

            lbMaSV = UiFactory.FieldLabel("Mã sinh viên");
            lbMaSV.SetBounds(18, 66, 290, 18);
            txtMaSV = UiFactory.TextBox("txtMaSV", "VD: SV10", false);
            txtMaSV.SetBounds(18, 84, 292, 26);
            txtMaSV.TextChanged += txtMaSV_TextChanged;

            lbHoTen = UiFactory.FieldLabel("Họ và tên");
            lbHoTen.SetBounds(18, 116, 290, 18);
            txtHoTen = UiFactory.TextBox("txtHoTen", "VD: Nguyễn Văn An", false);
            txtHoTen.SetBounds(18, 134, 292, 26);
            txtHoTen.TextChanged += txtHoTen_TextChanged;

            lbNgaySinh = UiFactory.FieldLabel("Ngày sinh");
            lbNgaySinh.SetBounds(18, 166, 130, 18);
            dtpNgaySinh = UiFactory.DatePicker("dtpNgaySinh");
            dtpNgaySinh.SetBounds(18, 184, 140, 26);

            lbGioiTinh = UiFactory.FieldLabel("Giới tính");
            lbGioiTinh.SetBounds(172, 166, 138, 18);
            radNam = new RadioButton();
            radNam.Text = "Nam";
            radNam.Name = "radNam";
            radNam.Checked = true;
            radNam.Font = UiTheme.FontBody(9.5f);
            radNam.ForeColor = UiTheme.InkSoft;
            radNam.AutoSize = true;
            radNam.BackColor = Color.Transparent;
            radNam.Location = new Point(172, 190);
            radNu = new RadioButton();
            radNu.Text = "Nữ";
            radNu.Name = "radNu";
            radNu.Font = UiTheme.FontBody(9.5f);
            radNu.ForeColor = UiTheme.InkSoft;
            radNu.AutoSize = true;
            radNu.BackColor = Color.Transparent;
            radNu.Location = new Point(238, 190);

            lbDiemTB = UiFactory.FieldLabel("Điểm trung bình (0 - 10)");
            lbDiemTB.SetBounds(18, 216, 190, 18);
            txtDiemTB = UiFactory.TextBox("txtDiemTB", "VD: 8.5", false);
            txtDiemTB.SetBounds(18, 234, 140, 26);
            txtDiemTB.KeyPress += txtDiemTB_KeyPress;

            lbKhoa = UiFactory.FieldLabel("Khoa");
            lbKhoa.SetBounds(172, 216, 138, 18);
            cboKhoa = UiFactory.ComboBox("cboKhoa");
            cboKhoa.SetBounds(172, 236, 138, 24);

            lbEmail = UiFactory.FieldLabel("Email");
            lbEmail.SetBounds(18, 266, 290, 18);
            txtEmail = UiFactory.TextBox("txtEmail", "VD: sv10@sv.dongang.edu.vn", false);
            txtEmail.SetBounds(18, 284, 292, 26);

            btnThem = new AccentButton();
            btnThem.Name = "btnThem";
            btnThem.Text = "Thêm";
            btnThem.BaseColor = UiTheme.Teal;
            btnThem.SetBounds(18, 322, 140, 38);
            btnThem.Click += btnThem_Click;

            btnLuu = new AccentButton();
            btnLuu.Name = "btnLuu";
            btnLuu.Text = "Lưu";
            btnLuu.BaseColor = UiTheme.Green;
            btnLuu.SetBounds(170, 322, 140, 38);
            btnLuu.Click += btnLuu_Click;

            btnSua = new AccentButton();
            btnSua.Name = "btnSua";
            btnSua.Text = "Sửa";
            btnSua.BaseColor = UiTheme.Amber;
            btnSua.SetBounds(18, 370, 140, 38);
            btnSua.Click += btnSua_Click;

            btnXoa = new AccentButton();
            btnXoa.Name = "btnXoa";
            btnXoa.Text = "Xóa";
            btnXoa.BaseColor = UiTheme.Red;
            btnXoa.OutlineOnly = true;
            btnXoa.SetBounds(170, 370, 140, 38);
            btnXoa.Click += btnXoa_Click;

            lblLoi = new Label();
            lblLoi.Name = "lblLoi";
            lblLoi.Text = "";
            lblLoi.Font = UiTheme.FontBody(8.25f);
            lblLoi.ForeColor = UiTheme.Red;
            lblLoi.AutoSize = false;
            lblLoi.BackColor = Color.Transparent;
            lblLoi.SetBounds(118, 14, 196, 40);

            cardNhap.Controls.AddRange(new Control[] {
                lblNhapTieuDe, lblTrangThai, lblLoi,
                lbMaSV, txtMaSV, lbHoTen, txtHoTen, lbNgaySinh, dtpNgaySinh,
                lbGioiTinh, radNam, radNu, lbDiemTB, txtDiemTB, lbKhoa, cboKhoa,
                lbEmail, txtEmail, btnThem, btnLuu, btnSua, btnXoa });

            // ============================ CỘT LƯỚI ============================
            cardLuoi = new CardPanel();
            cardLuoi.SetBounds(366, 104, 636, 430);

            lblLuoiTieuDe = new Label();
            lblLuoiTieuDe.Text = "Danh sách sinh viên";
            lblLuoiTieuDe.Font = UiTheme.FontBold(11f);
            lblLuoiTieuDe.ForeColor = UiTheme.Ink;
            lblLuoiTieuDe.AutoSize = true;
            lblLuoiTieuDe.BackColor = Color.Transparent;
            lblLuoiTieuDe.Location = new Point(18, 12);

            lblDem = new Label();
            lblDem.Name = "lblDem";
            lblDem.Text = "0 / 0 sinh viên";
            lblDem.Font = UiTheme.FontBody(8.5f);
            lblDem.ForeColor = UiTheme.Muted;
            lblDem.AutoSize = true;
            lblDem.BackColor = Color.Transparent;
            lblDem.Location = new Point(490, 16);

            txtTimKiem = UiFactory.TextBox("txtTimKiem", "Tìm theo mã / họ tên / khoa...", false);
            txtTimKiem.SetBounds(18, 40, 250, 26);
            txtTimKiem.TextChanged += txtTimKiem_TextChanged;

            cboKhoaFilter = UiFactory.ComboBox("cboKhoaFilter");
            cboKhoaFilter.SetBounds(278, 42, 180, 22);
            cboKhoaFilter.SelectedIndexChanged += cboKhoaFilter_SelectedIndexChanged;

            btnXoaLoc = new AccentButton();
            btnXoaLoc.Name = "btnXoaLoc";
            btnXoaLoc.Text = "Xóa lọc";
            btnXoaLoc.BaseColor = UiTheme.Slate;
            btnXoaLoc.OutlineOnly = true;
            btnXoaLoc.SetBounds(468, 38, 88, 30);
            btnXoaLoc.Click += btnXoaLoc_Click;

            dgvSinhVien = new DataGridView();
            dgvSinhVien.Name = "dgvSinhVien";
            dgvSinhVien.SetBounds(18, 76, 600, 296);
            dgvSinhVien.AllowUserToAddRows = false;
            dgvSinhVien.AllowUserToDeleteRows = false;
            dgvSinhVien.AllowUserToResizeRows = false;
            dgvSinhVien.ReadOnly = true;
            dgvSinhVien.RowHeadersVisible = false;
            dgvSinhVien.MultiSelect = false;
            dgvSinhVien.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSinhVien.BackgroundColor = Color.White;
            dgvSinhVien.BorderStyle = BorderStyle.None;
            dgvSinhVien.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvSinhVien.GridColor = UiTheme.Border;
            dgvSinhVien.EnableHeadersVisualStyles = false;
            dgvSinhVien.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSinhVien.Font = UiTheme.FontBody(9.25f);
            dgvSinhVien.RowTemplate.Height = 28;
            dgvSinhVien.ColumnHeadersHeight = 34;
            dgvSinhVien.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvSinhVien.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 244, 242);
            dgvSinhVien.ColumnHeadersDefaultCellStyle.ForeColor = UiTheme.Ink;
            dgvSinhVien.ColumnHeadersDefaultCellStyle.Font = UiTheme.FontBold(8.5f);
            dgvSinhVien.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 244, 242);
            dgvSinhVien.DefaultCellStyle.SelectionBackColor = Color.FromArgb(224, 242, 239);
            dgvSinhVien.DefaultCellStyle.SelectionForeColor = UiTheme.Ink;
            dgvSinhVien.DefaultCellStyle.Padding = new Padding(4, 0, 4, 0);
            dgvSinhVien.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 251, 253);
            dgvSinhVien.SelectionChanged += dgvSinhVien_SelectionChanged;
            dgvSinhVien.ColumnHeaderMouseClick += dgvSinhVien_ColumnHeaderMouseClick;

            // Cột gắn với thuộc tính của DTO (AutoGenerateColumns = false)
            ThemCot("MaSV", "Mã SV", 55);
            ThemCot("HoTen", "Họ và tên", 130);
            ThemCot("NgaySinh", "Ngày sinh", 78);
            ThemCot("GioiTinh", "Giới tính", 55);
            ThemCot("Khoa", "Khoa", 130);
            ThemCot("DiemTB", "Điểm", 45);
            ThemCot("Email", "Email", 130);

            dgvSinhVien.DataSource = bindingSource;

            btnXuatCSV = new AccentButton();
            btnXuatCSV.Name = "btnXuatCSV";
            btnXuatCSV.Text = "Xuất CSV";
            btnXuatCSV.BaseColor = UiTheme.Indigo;
            btnXuatCSV.SetBounds(18, 384, 130, 34);
            btnXuatCSV.Click += btnXuatCSV_Click;

            btnNhapCSV = new AccentButton();
            btnNhapCSV.Name = "btnNhapCSV";
            btnNhapCSV.Text = "Nhập CSV";
            btnNhapCSV.BaseColor = UiTheme.Indigo;
            btnNhapCSV.OutlineOnly = true;
            btnNhapCSV.SetBounds(158, 384, 130, 34);
            btnNhapCSV.Click += btnNhapCSV_Click;

            lblTapTin = new Label();
            lblTapTin.Name = "lblTapTin";
            lblTapTin.Text = "CSV: Bus luu tru, DAL ghi/doc tep";
            lblTapTin.Font = UiTheme.FontBody(8.25f);
            lblTapTin.ForeColor = UiTheme.Muted;
            lblTapTin.AutoSize = false;
            lblTapTin.AutoEllipsis = true;
            lblTapTin.TextAlign = ContentAlignment.MiddleLeft;
            lblTapTin.BackColor = Color.Transparent;
            lblTapTin.SetBounds(300, 388, 318, 26);

            cardLuoi.Controls.AddRange(new Control[] {
                lblLuoiTieuDe, lblDem, txtTimKiem, cboKhoaFilter, btnXoaLoc,
                dgvSinhVien, btnXuatCSV, btnNhapCSV, lblTapTin });

            // ============================ FORM ============================
            AutoScaleMode = AutoScaleMode.None;
            Text = "Sinh viên";
            ClientSize = new Size(1024, 552);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            KeyPreview = true;

            Controls.Add(infoStrip);
            Controls.AddRange(new Control[] { cardNhap, cardLuoi });

            Load += frmSinhVien_Load;
            KeyDown += frmSinhVien_KeyDown;
        }

        private void ThemCot(string thuocTinh, string tieuDe, int weight)
        {
            var cot = new DataGridViewTextBoxColumn();
            cot.Name = thuocTinh;
            cot.DataPropertyName = thuocTinh;
            cot.HeaderText = tieuDe;
            cot.FillWeight = weight;
            cot.SortMode = DataGridViewColumnSortMode.Programmatic;
            dgvSinhVien.Columns.Add(cot);
        }

        // =====================================================================
        // SỰ KIỆN PHỤ
        // =====================================================================
        private void frmSinhVien_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                Luu(_dangSua);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                XoaTrang();
                e.Handled = true;
            }
        }

        /// <summary>
        /// TextChanged trên ô mã: chỉ cập nhật trạng thái "đang thêm mới", KHÔNG kiểm tra nghiệp vụ
        /// (việc đó thuộc BUS và chỉ chạy khi bấm Lưu).
        /// </summary>
        private void txtMaSV_TextChanged(object sender, EventArgs e)
        {
            _loi.SetError(txtMaSV, string.Empty);
        }

        private void txtHoTen_TextChanged(object sender, EventArgs e)
        {
            _loi.SetError(txtHoTen, string.Empty);
        }
    }
}
