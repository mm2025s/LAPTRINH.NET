using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Lab06_ThreeLayer.GUI.Helpers;

namespace Lab06_ThreeLayer.GUI
{
    partial class frmDanhMuc
    {
        private IContainer components = null;

        private InfoStrip infoStrip;
        private TreeView tvDanhMuc;
        private ContextMenuStrip menuNgang;
        private ToolStripMenuItem mnuThemDanhMuc, mnuDoiTen, mnuXoa, mnuNapLai;
        private Label lblDem, lblHuongDan;

        private CardPanel cardChiTiet;
        private Label lblChonTieuDe, lblChonTen, lblChonLoai, lblChonChiTiet, lblKetQua;

        private AccentButton btnThem, btnXoa;

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
            infoStrip.Accent = UiTheme.Indigo;
            infoStrip.ChipText = "7";
            infoStrip.Title = "Danh mục sản phẩm";
            infoStrip.Subtitle = "TreeView: danh mục (cấp 1) - sản phẩm (cấp 2) · chuột phải để thêm / đổi tên / xóa";
            infoStrip.RightTop = "TreeView + ContextMenuStrip";
            infoStrip.RightBottom = "Quy tắc xóa do DanhMucBUS kiểm tra";

            // ============================ CÂY ============================
            tvDanhMuc = new TreeView();
            tvDanhMuc.Name = "tvDanhMuc";
            tvDanhMuc.SetBounds(22, 106, 400, 400);
            tvDanhMuc.Font = UiTheme.FontBody(10f);
            tvDanhMuc.ForeColor = UiTheme.Ink;
            tvDanhMuc.BackColor = Color.White;
            tvDanhMuc.BorderStyle = BorderStyle.FixedSingle;
            tvDanhMuc.HideSelection = false;
            tvDanhMuc.ItemHeight = 26;
            tvDanhMuc.ShowLines = true;
            tvDanhMuc.ShowPlusMinus = true;
            tvDanhMuc.ShowRootLines = true;
            tvDanhMuc.FullRowSelect = true;
            tvDanhMuc.AfterSelect += tvDanhMuc_AfterSelect;

            // ============================ CONTEXT MENU ============================
            menuNgang = new ContextMenuStrip();
            menuNgang.Font = UiTheme.FontBody(9.5f);
            menuNgang.Name = "menuNgang";

            mnuThemDanhMuc = new ToolStripMenuItem("Thêm danh mục...");
            mnuThemDanhMuc.Name = "mnuThemDanhMuc";
            mnuThemDanhMuc.Click += mnuThemDanhMuc_Click;

            mnuDoiTen = new ToolStripMenuItem("Đổi tên danh mục...");
            mnuDoiTen.Name = "mnuDoiTen";
            mnuDoiTen.Click += mnuDoiTen_Click;

            mnuXoa = new ToolStripMenuItem("Xóa danh mục");
            mnuXoa.Name = "mnuXoa";
            mnuXoa.Click += mnuXoa_Click;

            mnuNapLai = new ToolStripMenuItem("Nạp lại cây");
            mnuNapLai.Name = "mnuNapLai";
            mnuNapLai.Click += mnuNapLai_Click;

            menuNgang.Items.AddRange(new ToolStripItem[] {
                mnuThemDanhMuc, mnuDoiTen, new ToolStripSeparator(), mnuXoa, new ToolStripSeparator(), mnuNapLai });
            tvDanhMuc.ContextMenuStrip = menuNgang;

            lblDem = new Label();
            lblDem.Name = "lblDem";
            lblDem.Text = "0 danh mục";
            lblDem.Font = UiTheme.FontBody(8.75f);
            lblDem.ForeColor = UiTheme.Muted;
            lblDem.AutoSize = true;
            lblDem.BackColor = Color.Transparent;
            lblDem.Location = new Point(22, 512);

            btnThem = new AccentButton();
            btnThem.Name = "btnThem";
            btnThem.Text = "Thêm danh mục";
            btnThem.BaseColor = UiTheme.Indigo;
            btnThem.SetBounds(22, 462, 160, 36);
            btnThem.Click += btnThem_Click;

            btnXoa = new AccentButton();
            btnXoa.Name = "btnXoa";
            btnXoa.Text = "Xóa danh mục";
            btnXoa.BaseColor = UiTheme.Red;
            btnXoa.OutlineOnly = true;
            btnXoa.SetBounds(192, 462, 160, 36);
            btnXoa.Click += btnXoa_Click;

            lblHuongDan = new Label();
            lblHuongDan.Name = "lblHuongDan";
            lblHuongDan.Text = "Chuột phải vào cây để mở menu ngữ cảnh";
            lblHuongDan.Font = UiTheme.FontBody(8.25f);
            lblHuongDan.ForeColor = UiTheme.Muted;
            lblHuongDan.AutoSize = true;
            lblHuongDan.BackColor = Color.Transparent;
            lblHuongDan.Location = new Point(366, 512);

            // ============================ CARD CHI TIẾT ============================
            cardChiTiet = new CardPanel();
            cardChiTiet.SetBounds(438, 106, 380, 400);

            lblChonTieuDe = new Label();
            lblChonTieuDe.Text = "Chi tiết mục đang chọn";
            lblChonTieuDe.Font = UiTheme.FontBold(8.75f);
            lblChonTieuDe.ForeColor = UiTheme.Muted;
            lblChonTieuDe.AutoSize = true;
            lblChonTieuDe.BackColor = Color.Transparent;
            lblChonTieuDe.Location = new Point(20, 16);

            lblChonTen = new Label();
            lblChonTen.Name = "lblChonTen";
            lblChonTen.Text = "Chưa chọn mục nào";
            lblChonTen.Font = UiTheme.FontTitle(13f);
            lblChonTen.ForeColor = UiTheme.Ink;
            lblChonTen.AutoSize = false;
            lblChonTen.AutoEllipsis = true;
            lblChonTen.BackColor = Color.Transparent;
            lblChonTen.SetBounds(20, 42, 340, 28);

            lblChonLoai = new Label();
            lblChonLoai.Name = "lblChonLoai";
            lblChonLoai.Text = "";
            lblChonLoai.Font = UiTheme.FontBold(9f);
            lblChonLoai.ForeColor = UiTheme.Indigo;
            lblChonLoai.AutoSize = true;
            lblChonLoai.BackColor = Color.Transparent;
            lblChonLoai.Location = new Point(20, 76);

            lblChonChiTiet = new Label();
            lblChonChiTiet.Name = "lblChonChiTiet";
            lblChonChiTiet.Text = "";
            lblChonChiTiet.Font = UiTheme.FontBody(9f);
            lblChonChiTiet.ForeColor = UiTheme.InkSoft;
            lblChonChiTiet.AutoSize = false;
            lblChonChiTiet.BackColor = Color.Transparent;
            lblChonChiTiet.SetBounds(20, 102, 340, 26);

            lblKetQua = new Label();
            lblKetQua.Name = "lblKetQua";
            lblKetQua.Text = "";
            lblKetQua.Font = UiTheme.FontBody(8.75f);
            lblKetQua.ForeColor = UiTheme.Red;
            lblKetQua.AutoSize = false;
            lblKetQua.BackColor = Color.Transparent;
            lblKetQua.SetBounds(20, 140, 340, 60);

            Label lblGoiY = new Label();
            lblGoiY.Name = "lblGoiY";
            lblGoiY.Text =
                "Thử xóa \"Điện tử\" khi danh mục còn 3 sản phẩm:\r\n\r\n" +
                "DanhMucBUS.XoaDanhMuc() ném LoiNghiepVu\r\n" +
                "\"Danh mục còn sản phẩm, không thể xóa.\"";
            lblGoiY.Font = new Font("Consolas", 8.5f);
            lblGoiY.ForeColor = UiTheme.InkSoft;
            lblGoiY.AutoSize = false;
            lblGoiY.BackColor = Color.FromArgb(247, 247, 251);
            lblGoiY.Padding = new Padding(8);
            lblGoiY.SetBounds(20, 300, 340, 84);

            cardChiTiet.Controls.AddRange(new Control[] { lblChonTieuDe, lblChonTen, lblChonLoai, lblChonChiTiet, lblKetQua, lblGoiY });

            // ============================ FORM ============================
            AutoScaleMode = AutoScaleMode.None;
            Text = "Danh mục sản phẩm (TreeView)";
            ClientSize = new Size(840, 542);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;

            Controls.Add(infoStrip);
            Controls.AddRange(new Control[] { tvDanhMuc, btnThem, btnXoa, lblDem, lblHuongDan, cardChiTiet });

            Load += frmDanhMuc_Load;
        }
    }
}
