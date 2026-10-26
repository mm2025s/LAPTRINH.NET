using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Lab06_ThreeLayer.GUI.Helpers;

namespace Lab06_ThreeLayer.GUI
{
    partial class frmKienTruc
    {
        private IContainer components = null;

        private GradientPanel pnlTop;
        private Label lblTieuDe, lblPhuDe;

        private Panel pnlSoDo;
        private Label lblGui, lblBus, lblDal, lblDto;
        private Label lblMuiTen1, lblMuiTen2;

        private Label lblLopTitle, lblLoiTitle, lblLoi;
        private ListBox lstLop, lstLoi;
        private Label lblKetQuaTitle;
        private Label lblKetQua;

        private AccentButton btnChayThu, btnLab7, btnDong;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new Container();

            // ============================ HEADER ============================
            pnlTop = new GradientPanel();
            pnlTop.SetBounds(0, 0, 840, 96);
            pnlTop.ColorFrom = Color.FromArgb(14, 46, 44);
            pnlTop.ColorTo = Color.FromArgb(30, 110, 102);
            pnlTop.Angle = 12f;

            lblTieuDe = new Label();
            lblTieuDe.Text = "Mô hình 3 lớp (GUI - BUS - DAL)";
            lblTieuDe.Font = UiTheme.FontTitle(16f);
            lblTieuDe.ForeColor = Color.White;
            lblTieuDe.AutoSize = false;
            lblTieuDe.BackColor = Color.Transparent;
            lblTieuDe.SetBounds(26, 18, 700, 30);

            lblPhuDe = new Label();
            lblPhuDe.Text = "Lab 6 · IT3212 · Khoa Công nghệ Thông tin - ĐH Công nghệ Đông Á";
            lblPhuDe.Font = UiTheme.FontBody(8.75f);
            lblPhuDe.ForeColor = Color.FromArgb(214, 242, 238);
            lblPhuDe.AutoSize = false;
            lblPhuDe.BackColor = Color.Transparent;
            lblPhuDe.SetBounds(28, 52, 700, 20);

            pnlTop.Controls.AddRange(new Control[] { lblTieuDe, lblPhuDe });

            // ============================ SƠ ĐỒ KHỐI ============================
            pnlSoDo = new Panel();
            pnlSoDo.SetBounds(22, 112, 796, 96);
            pnlSoDo.BackColor = Color.FromArgb(238, 246, 244);

            lblGui = Khoi("GUI", "Form / DataGridView", 18, 20, 180, UiTheme.Teal);
            lblMuiTen1 = MuiTen(206, 20);
            lblBus = Khoi("BUS", "Nghiệp vụ + kiểm tra", 246, 20, 220, UiTheme.Indigo);
            lblMuiTen2 = MuiTen(474, 20);
            lblDal = Khoi("DAL", "Lưu trữ (List / CSV)", 514, 20, 180, UiTheme.Orange);
            lblDto = Khoi("DTO", "Đối tượng dữ liệu dùng chung cho cả 3 lớp", 18, 60, 676, UiTheme.Slate);

            pnlSoDo.Controls.AddRange(new Control[] { lblGui, lblMuiTen1, lblBus, lblMuiTen2, lblDal, lblDto });

            // ============================ DANH SÁCH LỚP ============================
            lblLopTitle = new Label();
            lblLopTitle.Text = "Nhiệm vụ từng lớp";
            lblLopTitle.Font = UiTheme.FontBold(9.5f);
            lblLopTitle.ForeColor = UiTheme.Ink;
            lblLopTitle.AutoSize = true;
            lblLopTitle.BackColor = Color.Transparent;
            lblLopTitle.Location = new Point(22, 220);

            lstLop = new ListBox();
            lstLop.Name = "lstLop";
            lstLop.Font = new Font("Consolas", 8.75f);
            lstLop.ForeColor = UiTheme.InkSoft;
            lstLop.BorderStyle = BorderStyle.FixedSingle;
            lstLop.IntegralHeight = false;
            lstLop.ItemHeight = 17;
            lstLop.SetBounds(22, 242, 420, 236);

            lblLoiTitle = new Label();
            lblLoiTitle.Text = "Cách BUS báo lỗi cho GUI";
            lblLoiTitle.Font = UiTheme.FontBold(9.5f);
            lblLoiTitle.ForeColor = UiTheme.Ink;
            lblLoiTitle.AutoSize = true;
            lblLoiTitle.BackColor = Color.Transparent;
            lblLoiTitle.Location = new Point(458, 220);

            lstLoi = new ListBox();
            lstLoi.Name = "lstLoi";
            lstLoi.Font = new Font("Consolas", 8.75f);
            lstLoi.ForeColor = UiTheme.InkSoft;
            lstLoi.BorderStyle = BorderStyle.FixedSingle;
            lstLoi.IntegralHeight = false;
            lstLoi.ItemHeight = 17;
            lstLoi.SetBounds(458, 242, 360, 140);

            lblKetQuaTitle = new Label();
            lblKetQuaTitle.Text = "Chạy thử luồng gọi";
            lblKetQuaTitle.Font = UiTheme.FontBold(9.5f);
            lblKetQuaTitle.ForeColor = UiTheme.Ink;
            lblKetQuaTitle.AutoSize = true;
            lblKetQuaTitle.BackColor = Color.Transparent;
            lblKetQuaTitle.Location = new Point(458, 392);

            lblKetQua = new Label();
            lblKetQua.Name = "lblKetQua";
            lblKetQua.Text = "Bấm \"Chạy thử luồng\" để xem BUS chặn dữ liệu sai trước khi DAL nhận.";
            lblKetQua.Font = UiTheme.FontBody(8.5f);
            lblKetQua.ForeColor = UiTheme.Muted;
            lblKetQua.AutoSize = false;
            lblKetQua.BackColor = Color.FromArgb(247, 247, 251);
            lblKetQua.Padding = new Padding(8);
            lblKetQua.SetBounds(458, 414, 360, 64);

            // ============================ NÚT ============================
            btnChayThu = new AccentButton();
            btnChayThu.Name = "btnChayThu";
            btnChayThu.Text = "Chạy thử luồng";
            btnChayThu.BaseColor = UiTheme.Teal;
            btnChayThu.SetBounds(22, 492, 150, 36);
            btnChayThu.Click += btnChayThu_Click;

            btnLab7 = new AccentButton();
            btnLab7.Name = "btnLab7";
            btnLab7.Text = "Chuẩn bị Lab 7";
            btnLab7.BaseColor = UiTheme.Indigo;
            btnLab7.OutlineOnly = true;
            btnLab7.SetBounds(182, 492, 150, 36);
            btnLab7.Click += btnLab7_Click;

            btnDong = new AccentButton();
            btnDong.Name = "btnDong";
            btnDong.Text = "Đóng";
            btnDong.BaseColor = UiTheme.Slate;
            btnDong.SetBounds(702, 492, 116, 36);
            btnDong.Click += btnDong_Click;

            // ============================ FORM ============================
            AutoScaleMode = AutoScaleMode.None;
            Text = "Kiến trúc 3 lớp - Lab 6";
            ClientSize = new Size(840, 542);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;

            Controls.Add(pnlTop);
            Controls.AddRange(new Control[] {
                pnlSoDo, lblLopTitle, lstLop, lblLoiTitle, lstLoi,
                lblKetQuaTitle, lblKetQua, btnChayThu, btnLab7, btnDong });

            Load += frmKienTruc_Load;
        }

        /// <summary>Khối chữ nhật có tiêu đề + mô tả, dùng vẽ sơ đồ 3 lớp.</summary>
        private Label Khoi(string ten, string moTa, int left, int top, int width, Color mau)
        {
            var l = new Label();
            l.Name = "lblKhoi" + ten;
            l.Text = ten + "\r\n" + moTa;
            l.Font = UiTheme.FontBold(9.25f);
            l.ForeColor = Color.White;
            l.BackColor = mau;
            l.TextAlign = ContentAlignment.MiddleCenter;
            l.AutoSize = false;
            l.SetBounds(left, top, width, 40);
            return l;
        }

        private Label MuiTen(int left, int top)
        {
            var l = new Label();
            l.Name = "lblMuiTen" + left;
            l.Text = "→";
            l.Font = UiTheme.FontBold(16f);
            l.ForeColor = UiTheme.Teal;
            l.BackColor = Color.Transparent;
            l.TextAlign = ContentAlignment.MiddleCenter;
            l.AutoSize = false;
            l.SetBounds(left, top, 40, 40);
            return l;
        }
    }
}
