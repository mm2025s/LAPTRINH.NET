using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Lab06_ThreeLayer.BUS;
using Lab06_ThreeLayer.GUI.Helpers;

namespace Lab06_ThreeLayer.GUI
{
    partial class frmSanPham
    {
        private IContainer components = null;

        private InfoStrip infoStrip;

        private TextBox txtTim;
        private AccentButton btnSapXepTon, btnSapXepGiaTri, btnKiemTraTong;
        private Label lblDem;
        private DataGridView dgvSanPham;

        private CardPanel cardTong;
        private Label lblTongTieuDe, lblTongChiTiet, lblTongGiaTri, lblKetQua;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new Container();

            // ============================ HEADER ============================
            infoStrip = new InfoStrip();
            infoStrip.Dock = DockStyle.Top;
            infoStrip.Height = 90;
            infoStrip.Accent = UiTheme.Orange;
            infoStrip.ChipText = "6";
            infoStrip.Title = "Danh sách sản phẩm";
            infoStrip.Subtitle = "Tổng giá trị hàng tồn = Σ(đơn giá × số lượng) - tính ở SanPhamBUS, không tính ở Form";
            infoStrip.RightTop = "DataGridView";
            infoStrip.RightBottom = "Hàng tồn dưới 10 đơn vị được tô đỏ";

            // ============================ THANH CÔNG CỤ ============================
            txtTim = UiFactory.TextBox("txtTim", "Tìm theo mã hoặc tên sản phẩm...", false);
            txtTim.SetBounds(22, 106, 268, 26);
            txtTim.TextChanged += txtTim_TextChanged;

            btnSapXepTon = new AccentButton();
            btnSapXepTon.Name = "btnSapXepTon";
            btnSapXepTon.Text = "Tồn nhiều → ít";
            btnSapXepTon.BaseColor = UiTheme.Orange;
            btnSapXepTon.SetBounds(300, 104, 146, 30);
            btnSapXepTon.Click += btnSapXepTon_Click;

            btnSapXepGiaTri = new AccentButton();
            btnSapXepGiaTri.Name = "btnSapXepGiaTri";
            btnSapXepGiaTri.Text = "Giá trị tồn ↓";
            btnSapXepGiaTri.BaseColor = UiTheme.Orange;
            btnSapXepGiaTri.OutlineOnly = true;
            btnSapXepGiaTri.SetBounds(456, 104, 146, 30);
            btnSapXepGiaTri.Click += btnSapXepGiaTri_Click;

            lblDem = new Label();
            lblDem.Name = "lblDem";
            lblDem.Text = "0 sản phẩm";
            lblDem.Font = UiTheme.FontBody(8.75f);
            lblDem.ForeColor = UiTheme.Muted;
            lblDem.AutoSize = true;
            lblDem.BackColor = Color.Transparent;
            lblDem.Location = new Point(618, 112);

            // ============================ LƯỚI ============================
            dgvSanPham = new DataGridView();
            dgvSanPham.Name = "dgvSanPham";
            dgvSanPham.SetBounds(22, 144, 830, 300);
            dgvSanPham.AllowUserToAddRows = false;
            dgvSanPham.AllowUserToDeleteRows = false;
            dgvSanPham.AllowUserToResizeRows = false;
            dgvSanPham.ReadOnly = true;
            dgvSanPham.RowHeadersVisible = false;
            dgvSanPham.MultiSelect = false;
            dgvSanPham.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSanPham.BackgroundColor = Color.White;
            dgvSanPham.BorderStyle = BorderStyle.None;
            dgvSanPham.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvSanPham.GridColor = UiTheme.Border;
            dgvSanPham.EnableHeadersVisualStyles = false;
            dgvSanPham.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSanPham.Font = UiTheme.FontBody(9.5f);
            dgvSanPham.RowTemplate.Height = 30;
            dgvSanPham.ColumnHeadersHeight = 36;
            dgvSanPham.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvSanPham.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 242, 231);
            dgvSanPham.ColumnHeadersDefaultCellStyle.ForeColor = UiTheme.Ink;
            dgvSanPham.ColumnHeadersDefaultCellStyle.Font = UiTheme.FontBold(8.75f);
            dgvSanPham.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 242, 231);
            dgvSanPham.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 236, 222);
            dgvSanPham.DefaultCellStyle.SelectionForeColor = UiTheme.Ink;
            dgvSanPham.DefaultCellStyle.Padding = new Padding(4, 0, 4, 0);
            dgvSanPham.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(252, 252, 253);

            dgvSanPham.Columns.Add("STT", "#");
            dgvSanPham.Columns.Add("MaSP", "Mã SP");
            dgvSanPham.Columns.Add("TenSP", "Tên sản phẩm");
            dgvSanPham.Columns.Add("MaDanhMuc", "Danh mục");
            dgvSanPham.Columns.Add("DonGia", "Đơn giá");
            dgvSanPham.Columns.Add("SoLuongTon", "SL tồn");
            dgvSanPham.Columns.Add("GiaTriTon", "Giá trị tồn");

            dgvSanPham.Columns["STT"].FillWeight = 25;
            dgvSanPham.Columns["MaSP"].FillWeight = 50;
            dgvSanPham.Columns["TenSP"].FillWeight = 150;
            dgvSanPham.Columns["MaDanhMuc"].FillWeight = 55;
            dgvSanPham.Columns["DonGia"].FillWeight = 75;
            dgvSanPham.Columns["SoLuongTon"].FillWeight = 45;
            dgvSanPham.Columns["GiaTriTon"].FillWeight = 90;

            dgvSanPham.Columns["STT"].DefaultCellStyle.ForeColor = UiTheme.Muted;
            dgvSanPham.Columns["MaSP"].DefaultCellStyle.Font = UiTheme.FontBold(9.5f);
            dgvSanPham.Columns["MaSP"].DefaultCellStyle.ForeColor = UiTheme.Orange;
            dgvSanPham.Columns["DonGia"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvSanPham.Columns["SoLuongTon"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvSanPham.Columns["GiaTriTon"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            // ============================ CARD TỔNG ============================
            cardTong = new CardPanel();
            cardTong.SetBounds(22, 456, 830, 96);

            lblTongTieuDe = new Label();
            lblTongTieuDe.Text = "TỔNG GIÁ TRỊ HÀNG TỒN (BUS.TongGiaTriTon)";
            lblTongTieuDe.Font = UiTheme.FontBold(8.75f);
            lblTongTieuDe.ForeColor = UiTheme.Muted;
            lblTongTieuDe.AutoSize = true;
            lblTongTieuDe.BackColor = Color.Transparent;
            lblTongTieuDe.Location = new Point(22, 14);

            lblTongGiaTri = new Label();
            lblTongGiaTri.Name = "lblTongGiaTri";
            lblTongGiaTri.Text = "0 đ";
            lblTongGiaTri.Font = UiTheme.FontTitle(20f);
            lblTongGiaTri.ForeColor = UiTheme.Orange;
            lblTongGiaTri.AutoSize = true;
            lblTongGiaTri.BackColor = Color.Transparent;
            lblTongGiaTri.Location = new Point(20, 34);

            lblTongChiTiet = new Label();
            lblTongChiTiet.Name = "lblTongChiTiet";
            lblTongChiTiet.Text = "";
            lblTongChiTiet.Font = UiTheme.FontBody(8.75f);
            lblTongChiTiet.ForeColor = UiTheme.InkSoft;
            lblTongChiTiet.AutoSize = true;
            lblTongChiTiet.BackColor = Color.Transparent;
            lblTongChiTiet.Location = new Point(22, 70);

            btnKiemTraTong = new AccentButton();
            btnKiemTraTong.Name = "btnKiemTraTong";
            btnKiemTraTong.Text = "Kiểm tra lại tổng";
            btnKiemTraTong.BaseColor = UiTheme.Orange;
            btnKiemTraTong.SetBounds(400, 16, 170, 34);
            btnKiemTraTong.Click += btnKiemTraTong_Click;

            lblKetQua = new Label();
            lblKetQua.Name = "lblKetQua";
            lblKetQua.Text = "So sánh kết quả tính trên lưới với kết quả của BUS";
            lblKetQua.Font = UiTheme.FontBody(8.5f);
            lblKetQua.ForeColor = UiTheme.Muted;
            lblKetQua.AutoSize = false;
            lblKetQua.BackColor = Color.Transparent;
            lblKetQua.SetBounds(400, 56, 412, 32);

            cardTong.Controls.AddRange(new Control[] { lblTongTieuDe, lblTongGiaTri, lblTongChiTiet, btnKiemTraTong, lblKetQua });

            // ============================ FORM ============================
            AutoScaleMode = AutoScaleMode.None;
            Text = "Sản phẩm - tổng giá trị hàng tồn";
            ClientSize = new Size(874, 570);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;

            Controls.Add(infoStrip);
            Controls.AddRange(new Control[] { txtTim, btnSapXepTon, btnSapXepGiaTri, lblDem, dgvSanPham, cardTong });

            Load += frmSanPham_Load;
        }
    }
}
