using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Lab06_ThreeLayer.GUI.Helpers;

namespace Lab06_ThreeLayer.GUI
{
    partial class frmGioiThieu
    {
        private IContainer components = null;

        private GradientPanel pnlTop;
        private Label lblTieuDe, lblPhuDe, lblTag;
        private Label lblMucTieuTitle, lblTinhNangTitle, lblPhimTitle, lblKienTrucTitle;
        private ListBox lstMucTieu, lstTinhNang, lstPhimTat;
        private Label lblKienTruc;
        private AccentButton btnDong;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new Container();

            pnlTop = new GradientPanel();
            pnlTop.SetBounds(0, 0, 820, 104);
            pnlTop.ColorFrom = Color.FromArgb(12, 52, 50);
            pnlTop.ColorTo = Color.FromArgb(38, 132, 122);
            pnlTop.Angle = 15f;

            lblTieuDe = new Label();
            lblTieuDe.Text = "Lab 6 · Dữ liệu với DataGridView và mô hình 3 lớp";
            lblTieuDe.Font = UiTheme.FontTitle(16f);
            lblTieuDe.ForeColor = Color.White;
            lblTieuDe.AutoSize = false;
            lblTieuDe.BackColor = Color.Transparent;
            lblTieuDe.AutoEllipsis = true;
            lblTieuDe.SetBounds(26, 18, 760, 30);

            lblPhuDe = new Label();
            lblPhuDe.Text = "Lập trình .NET (IT3212) · Khoa Công nghệ Thông tin - ĐH Công nghệ Đông Á";
            lblPhuDe.Font = UiTheme.FontBody(8.75f);
            lblPhuDe.ForeColor = Color.FromArgb(206, 240, 236);
            lblPhuDe.AutoSize = false;
            lblPhuDe.BackColor = Color.Transparent;
            lblPhuDe.SetBounds(28, 50, 760, 20);

            lblTag = new Label();
            lblTag.Text = "DataGridView   ·   BindingSource   ·   TreeView   ·   ListView + ImageList   ·   BUS/DAL   ·   CSV";
            lblTag.Font = UiTheme.FontBody(8.25f);
            lblTag.ForeColor = Color.FromArgb(148, 255, 242);
            lblTag.AutoSize = true;
            lblTag.BackColor = Color.Transparent;
            lblTag.Location = new Point(28, 74);

            pnlTop.Controls.AddRange(new Control[] { lblTieuDe, lblPhuDe, lblTag });

            lblMucTieuTitle = TieuDe("Mục tiêu cần đạt được (10 bài)", 30, 122);
            lstMucTieu = DanhSach("lstMucTieu", 30, 144, 370, 158);

            lblTinhNangTitle = TieuDe("Bản đồ Form - tính năng", 30, 314);
            lstTinhNang = DanhSach("lstTinhNang", 30, 336, 370, 150);

            lblPhimTitle = TieuDe("Phím tắt", 418, 122);
            lstPhimTat = DanhSach("lstPhimTat", 418, 144, 370, 150);

            lblKienTrucTitle = TieuDe("Cấu trúc thư mục & chiều phụ thuộc", 418, 306);

            lblKienTruc = new Label();
            lblKienTruc.Text = "";
            lblKienTruc.Font = new Font("Consolas", 8.25f);
            lblKienTruc.ForeColor = UiTheme.InkSoft;
            lblKienTruc.AutoSize = false;
            lblKienTruc.BackColor = Color.FromArgb(247, 247, 251);
            lblKienTruc.SetBounds(418, 328, 370, 158);
            lblKienTruc.Padding = new Padding(8);

            btnDong = new AccentButton();
            btnDong.Name = "btnDong";
            btnDong.Text = "Đóng";
            btnDong.BaseColor = UiTheme.Teal;
            btnDong.SetBounds(672, 500, 116, 38);
            btnDong.Click += btnDong_Click;

            AutoScaleMode = AutoScaleMode.None;
            Text = "Giới thiệu chương trình - Lab 6";
            ClientSize = new Size(820, 554);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;

            Controls.Add(pnlTop);
            Controls.AddRange(new Control[] {
                lblMucTieuTitle, lstMucTieu, lblTinhNangTitle, lstTinhNang,
                lblPhimTitle, lstPhimTat, lblKienTrucTitle, lblKienTruc, btnDong });

            Load += frmGioiThieu_Load;
        }

        private Label TieuDe(string text, int left, int top)
        {
            var l = new Label();
            l.Text = text;
            l.Font = UiTheme.FontBold(9.5f);
            l.ForeColor = UiTheme.Ink;
            l.AutoSize = true;
            l.BackColor = Color.Transparent;
            l.Location = new Point(left, top);
            return l;
        }

        private ListBox DanhSach(string name, int left, int top, int width, int height)
        {
            var l = new ListBox();
            l.Name = name;
            l.Font = new Font("Consolas", 8.5f);
            l.ForeColor = UiTheme.InkSoft;
            l.BorderStyle = BorderStyle.FixedSingle;
            l.IntegralHeight = false;
            l.ItemHeight = 17;
            l.SetBounds(left, top, width, height);
            return l;
        }
    }
}
