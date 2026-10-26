using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Lab06_ThreeLayer.GUI.Helpers;

namespace Lab06_ThreeLayer.GUI
{
    partial class frmDanhMucSinhVien
    {
        private IContainer components = null;

        private InfoStrip infoStrip;
        private TreeView tvKhoa;
        private Label lblDem;
        private AccentButton btnMoRong, btnThuGon, btnXemSinhVienGioi, btnNapLai;

        private CardPanel cardChiTiet;
        private Label lblNhanTieuDe, lblTieuDe, lblPhu, lblDiem, lblChiTiet;

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
            infoStrip.Accent = UiTheme.Violet;
            infoStrip.ChipText = "4";
            infoStrip.Title = "Danh mục khoa - sinh viên";
            infoStrip.Subtitle = "TreeView nhóm dữ liệu theo khoa (BUS.NhomTheoKhoa) · chọn một sinh viên để xem chi tiết";
            infoStrip.RightTop = "TreeView";
            infoStrip.RightBottom = "Điểm dưới 5.0 hiển thị màu đỏ";

            // ============================ CÂY ============================
            tvKhoa = new TreeView();
            tvKhoa.Name = "tvKhoa";
            tvKhoa.SetBounds(22, 106, 420, 388);
            tvKhoa.Font = UiTheme.FontBody(9.75f);
            tvKhoa.ForeColor = UiTheme.Ink;
            tvKhoa.BackColor = Color.White;
            tvKhoa.BorderStyle = BorderStyle.FixedSingle;
            tvKhoa.HideSelection = false;
            tvKhoa.ItemHeight = 24;
            tvKhoa.AfterSelect += tvKhoa_AfterSelect;

            lblDem = new Label();
            lblDem.Name = "lblDem";
            lblDem.Text = "0 khoa";
            lblDem.Font = UiTheme.FontBody(8.75f);
            lblDem.ForeColor = UiTheme.Muted;
            lblDem.AutoSize = true;
            lblDem.BackColor = Color.Transparent;
            lblDem.Location = new Point(22, 502);

            btnMoRong = new AccentButton();
            btnMoRong.Name = "btnMoRong";
            btnMoRong.Text = "Mở rộng";
            btnMoRong.BaseColor = UiTheme.Violet;
            btnMoRong.OutlineOnly = true;
            btnMoRong.SetBounds(22, 524, 100, 32);
            btnMoRong.Click += btnMoRong_Click;

            btnThuGon = new AccentButton();
            btnThuGon.Name = "btnThuGon";
            btnThuGon.Text = "Thu gọn";
            btnThuGon.BaseColor = UiTheme.Slate;
            btnThuGon.OutlineOnly = true;
            btnThuGon.SetBounds(132, 524, 100, 32);
            btnThuGon.Click += btnThuGon_Click;

            btnXemSinhVienGioi = new AccentButton();
            btnXemSinhVienGioi.Name = "btnXemSinhVienGioi";
            btnXemSinhVienGioi.Text = "Lọc điểm ≥ 8.0";
            btnXemSinhVienGioi.BaseColor = UiTheme.Green;
            btnXemSinhVienGioi.SetBounds(242, 524, 130, 32);
            btnXemSinhVienGioi.Click += btnXemSinhVienGioi_Click;

            btnNapLai = new AccentButton();
            btnNapLai.Name = "btnNapLai";
            btnNapLai.Text = "Nạp lại cây";
            btnNapLai.BaseColor = UiTheme.Violet;
            btnNapLai.SetBounds(382, 524, 110, 32);
            btnNapLai.Click += btnNapLai_Click;

            // ============================ CARD CHI TIẾT ============================
            cardChiTiet = new CardPanel();
            cardChiTiet.SetBounds(458, 106, 360, 388);

            lblNhanTieuDe = new Label();
            lblNhanTieuDe.Text = "CHI TIẾT MỤC ĐANG CHỌN";
            lblNhanTieuDe.Font = UiTheme.FontBold(8f);
            lblNhanTieuDe.ForeColor = UiTheme.Muted;
            lblNhanTieuDe.AutoSize = true;
            lblNhanTieuDe.BackColor = Color.Transparent;
            lblNhanTieuDe.Location = new Point(20, 16);

            lblTieuDe = new Label();
            lblTieuDe.Name = "lblTieuDe";
            lblTieuDe.Text = "-";
            lblTieuDe.Font = UiTheme.FontTitle(14f);
            lblTieuDe.ForeColor = UiTheme.Ink;
            lblTieuDe.AutoSize = false;
            lblTieuDe.AutoEllipsis = true;
            lblTieuDe.BackColor = Color.Transparent;
            lblTieuDe.SetBounds(20, 38, 320, 30);

            lblPhu = new Label();
            lblPhu.Name = "lblPhu";
            lblPhu.Text = "";
            lblPhu.Font = UiTheme.FontBody(9f);
            lblPhu.ForeColor = UiTheme.Muted;
            lblPhu.AutoSize = false;
            lblPhu.BackColor = Color.Transparent;
            lblPhu.SetBounds(20, 72, 320, 22);

            lblDiem = new Label();
            lblDiem.Name = "lblDiem";
            lblDiem.Text = "";
            lblDiem.Font = UiTheme.FontBold(13f);
            lblDiem.ForeColor = UiTheme.Teal;
            lblDiem.AutoSize = true;
            lblDiem.BackColor = Color.Transparent;
            lblDiem.Location = new Point(20, 102);

            lblChiTiet = new Label();
            lblChiTiet.Name = "lblChiTiet";
            lblChiTiet.Text = "";
            lblChiTiet.Font = UiTheme.FontBody(9.25f);
            lblChiTiet.ForeColor = UiTheme.InkSoft;
            lblChiTiet.AutoSize = false;
            lblChiTiet.BackColor = Color.Transparent;
            lblChiTiet.SetBounds(20, 144, 320, 90);

            Label lblChuThich = new Label();
            lblChuThich.Text =
                "Khoa cấp 1 do BUS nhóm từ danh sách sinh viên.\r\n" +
                "Sinh viên cấp 2 sắp xếp theo họ tên (BUS.SapXep).";
            lblChuThich.Font = new Font("Consolas", 8.25f);
            lblChuThich.ForeColor = UiTheme.InkSoft;
            lblChuThich.AutoSize = false;
            lblChuThich.BackColor = Color.FromArgb(247, 247, 251);
            lblChuThich.Padding = new Padding(8);
            lblChuThich.SetBounds(20, 252, 320, 70);

            cardChiTiet.Controls.AddRange(new Control[] { lblNhanTieuDe, lblTieuDe, lblPhu, lblDiem, lblChiTiet, lblChuThich });

            // ============================ FORM ============================
            AutoScaleMode = AutoScaleMode.None;
            Text = "Danh mục khoa - sinh viên (TreeView)";
            ClientSize = new Size(840, 570);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;

            Controls.Add(infoStrip);
            Controls.AddRange(new Control[] { tvKhoa, lblDem, btnMoRong, btnThuGon, btnXemSinhVienGioi, btnNapLai, cardChiTiet });

            Load += frmDanhMucSinhVien_Load;
        }
    }
}
