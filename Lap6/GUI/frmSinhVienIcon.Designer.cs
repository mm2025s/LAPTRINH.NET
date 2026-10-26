using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Lab06_ThreeLayer.GUI.Helpers;

namespace Lab06_ThreeLayer.GUI
{
    partial class frmSinhVienIcon
    {
        private IContainer components = null;

        private InfoStrip infoStrip;
        private ListView lvSinhVien;
        private Label lblDem;
        private AccentButton btnChiTiet, btnBieuTuong, btnDanhSachNho, btnSapXepDiem;

        private CardPanel cardChon;
        private RoundedPictureBox picAnh;
        private Label lblTen, lblPhu, lblDiem;

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
            infoStrip.Accent = UiTheme.Rose;
            infoStrip.ChipText = "8";
            infoStrip.Title = "Sinh viên có biểu tượng";
            infoStrip.Subtitle = "ListView (LargeIcon / SmallIcon / Details) + ImageList tự vẽ theo học lực";
            infoStrip.RightTop = "ListView + ImageList";
            infoStrip.RightBottom = "★ giỏi · ✓ trung bình · ! yếu";

            // ============================ LISTVIEW ============================
            lvSinhVien = new ListView();
            lvSinhVien.Name = "lvSinhVien";
            lvSinhVien.SetBounds(22, 106, 560, 396);
            lvSinhVien.View = View.LargeIcon;
            lvSinhVien.Font = UiTheme.FontBody(9.5f);
            lvSinhVien.ForeColor = UiTheme.Ink;
            lvSinhVien.BackColor = Color.White;
            lvSinhVien.BorderStyle = BorderStyle.FixedSingle;
            lvSinhVien.HideSelection = false;
            lvSinhVien.MultiSelect = false;
            lvSinhVien.FullRowSelect = true;
            lvSinhVien.GridLines = true;
            lvSinhVien.SelectedIndexChanged += lvSinhVien_SelectedIndexChanged;

            // Cột cho chế độ Details
            lvSinhVien.Columns.Add("Mã - Họ tên", 210);
            lvSinhVien.Columns.Add("Khoa", 140);
            lvSinhVien.Columns.Add("Ngày sinh", 90);
            lvSinhVien.Columns.Add("Điểm", 50, HorizontalAlignment.Center);
            lvSinhVien.Columns.Add("Xếp loại", 70);

            lblDem = new Label();
            lblDem.Name = "lblDem";
            lblDem.Text = "0 sinh viên";
            lblDem.Font = UiTheme.FontBody(8.75f);
            lblDem.ForeColor = UiTheme.Muted;
            lblDem.AutoSize = true;
            lblDem.BackColor = Color.Transparent;
            lblDem.Location = new Point(22, 510);

            btnChiTiet = new AccentButton();
            btnChiTiet.Name = "btnChiTiet";
            btnChiTiet.Text = "Dạng chi tiết";
            btnChiTiet.BaseColor = UiTheme.Rose;
            btnChiTiet.OutlineOnly = true;
            btnChiTiet.SetBounds(22, 532, 124, 32);
            btnChiTiet.Click += btnChiTiet_Click;

            btnBieuTuong = new AccentButton();
            btnBieuTuong.Name = "btnBieuTuong";
            btnBieuTuong.Text = "Dạng biểu tượng";
            btnBieuTuong.BaseColor = UiTheme.Rose;
            btnBieuTuong.SetBounds(156, 532, 140, 32);
            btnBieuTuong.Click += btnBieuTuong_Click;

            btnDanhSachNho = new AccentButton();
            btnDanhSachNho.Name = "btnDanhSachNho";
            btnDanhSachNho.Text = "Biểu tượng nhỏ";
            btnDanhSachNho.BaseColor = UiTheme.Slate;
            btnDanhSachNho.OutlineOnly = true;
            btnDanhSachNho.SetBounds(306, 532, 140, 32);
            btnDanhSachNho.Click += btnDanhSachNho_Click;

            btnSapXepDiem = new AccentButton();
            btnSapXepDiem.Name = "btnSapXepDiem";
            btnSapXepDiem.Text = "Điểm cao trước";
            btnSapXepDiem.BaseColor = UiTheme.Green;
            btnSapXepDiem.SetBounds(456, 532, 126, 32);
            btnSapXepDiem.Click += btnSapXepDiem_Click;

            // ============================ CARD CHỌN ============================
            cardChon = new CardPanel();
            cardChon.SetBounds(598, 106, 300, 396);

            Label lblNhan = new Label();
            lblNhan.Text = "SINH VIÊN ĐANG CHỌN";
            lblNhan.Font = UiTheme.FontBold(8f);
            lblNhan.ForeColor = UiTheme.Muted;
            lblNhan.AutoSize = true;
            lblNhan.BackColor = Color.Transparent;
            lblNhan.Location = new Point(20, 16);

            picAnh = new RoundedPictureBox();
            picAnh.Name = "picAnh";
            picAnh.SetBounds(96, 44, 96, 96);
            picAnh.CornerRadius = 48;

            lblTen = new Label();
            lblTen.Name = "lblTen";
            lblTen.Text = "Chọn một sinh viên";
            lblTen.Font = UiTheme.FontTitle(12f);
            lblTen.ForeColor = UiTheme.Ink;
            lblTen.AutoSize = false;
            lblTen.TextAlign = ContentAlignment.MiddleCenter;
            lblTen.AutoEllipsis = true;
            lblTen.BackColor = Color.Transparent;
            lblTen.SetBounds(16, 152, 268, 28);

            lblPhu = new Label();
            lblPhu.Name = "lblPhu";
            lblPhu.Text = "";
            lblPhu.Font = UiTheme.FontBody(8.75f);
            lblPhu.ForeColor = UiTheme.Muted;
            lblPhu.AutoSize = false;
            lblPhu.TextAlign = ContentAlignment.MiddleCenter;
            lblPhu.BackColor = Color.Transparent;
            lblPhu.SetBounds(16, 182, 268, 40);

            lblDiem = new Label();
            lblDiem.Name = "lblDiem";
            lblDiem.Text = "";
            lblDiem.Font = UiTheme.FontBold(13f);
            lblDiem.ForeColor = UiTheme.Teal;
            lblDiem.AutoSize = false;
            lblDiem.TextAlign = ContentAlignment.MiddleCenter;
            lblDiem.BackColor = Color.Transparent;
            lblDiem.SetBounds(16, 226, 268, 30);

            Label lblChuThich = new Label();
            lblChuThich.Text =
                "ImageList có 3 ảnh 40x40\r\nđược vẽ bằng GDI+:\r\n\r\n" +
                "  ★  điểm ≥ 8.0\r\n  ✓  5.0 - 7.9\r\n  !  điểm < 5.0";
            lblChuThich.Font = new Font("Consolas", 8.5f);
            lblChuThich.ForeColor = UiTheme.InkSoft;
            lblChuThich.AutoSize = false;
            lblChuThich.BackColor = Color.FromArgb(252, 244, 248);
            lblChuThich.Padding = new Padding(8);
            lblChuThich.SetBounds(20, 268, 260, 108);

            cardChon.Controls.AddRange(new Control[] { lblNhan, picAnh, lblTen, lblPhu, lblDiem, lblChuThich });

            // ============================ FORM ============================
            AutoScaleMode = AutoScaleMode.None;
            Text = "Sinh viên có biểu tượng (ListView + ImageList)";
            ClientSize = new Size(920, 578);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;

            Controls.Add(infoStrip);
            Controls.AddRange(new Control[] { lvSinhVien, lblDem, btnChiTiet, btnBieuTuong, btnDanhSachNho, btnSapXepDiem, cardChon });

            Load += frmSinhVienIcon_Load;
        }
    }
}
