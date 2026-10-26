using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Lab05_MultiForm.Helpers;

namespace Lab05_MultiForm.Forms
{
    partial class frmGioiThieu
    {
        private IContainer components = null;

        private GradientPanel pnlTop;
        private Label lblTieuDe, lblPhuDe, lblTag;
        private Label lblMucTieuTitle, lblKyThuatTitle, lblPhimTitle, lblKienTrucTitle;
        private ListBox lstMucTieu, lstKyThuat, lstPhimTat;
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
            pnlTop.SetBounds(0, 0, 760, 104);
            pnlTop.ColorFrom = Color.FromArgb(26, 22, 70);
            pnlTop.ColorTo = Color.FromArgb(96, 66, 210);
            pnlTop.Angle = 15f;

            lblTieuDe = new Label();
            lblTieuDe.Text = "Lab 5";
            lblTieuDe.Font = UiTheme.FontTitle(16f);
            lblTieuDe.ForeColor = Color.White;
            lblTieuDe.AutoSize = false;
            lblTieuDe.BackColor = Color.Transparent;
            lblTieuDe.AutoEllipsis = true;
            lblTieuDe.SetBounds(26, 18, 700, 30);

            lblPhuDe = new Label();
            lblPhuDe.Text = "";
            lblPhuDe.Font = UiTheme.FontBody(8.75f);
            lblPhuDe.ForeColor = Color.FromArgb(206, 211, 240);
            lblPhuDe.AutoSize = false;
            lblPhuDe.BackColor = Color.Transparent;
            lblPhuDe.SetBounds(28, 50, 700, 20);

            lblTag = new Label();
            lblTag.Text = "ErrorProvider   ·   KeyPress   ·   TextChanged   ·   FormClosing   ·   MenuStrip   ·   StatusStrip   ·   MDI";
            lblTag.Font = UiTheme.FontBody(8.25f);
            lblTag.ForeColor = Color.FromArgb(148, 255, 242);
            lblTag.AutoSize = true;
            lblTag.BackColor = Color.Transparent;
            lblTag.Location = new Point(28, 74);

            pnlTop.Controls.AddRange(new Control[] { lblTieuDe, lblPhuDe, lblTag });

            lblMucTieuTitle = TieuDe("Mục tiêu cần đạt được", 30, 122);
            lstMucTieu = DanhSach("lstMucTieu", 30, 144, 360, 128);

            lblKyThuatTitle = TieuDe("Bản đồ kỹ thuật theo Form", 30, 286);
            lstKyThuat = DanhSach("lstKyThuat", 30, 308, 360, 148);

            lblPhimTitle = TieuDe("Phím tắt", 388, 122);
            lstPhimTat = DanhSach("lstPhimTat", 388, 144, 342, 148);

            lblKienTrucTitle = TieuDe("Kiến trúc thư mục & chuỗi bài lab", 388, 306);

            lblKienTruc = new Label();
            lblKienTruc.Text = "";
            lblKienTruc.Font = new Font("Consolas", 8.25f);
            lblKienTruc.ForeColor = UiTheme.InkSoft;
            lblKienTruc.AutoSize = false;
            lblKienTruc.BackColor = Color.FromArgb(247, 247, 251);
            lblKienTruc.SetBounds(388, 328, 342, 128);
            lblKienTruc.Padding = new Padding(8);

            btnDong = new AccentButton();
            btnDong.Name = "btnDong";
            btnDong.Text = "Đóng";
            btnDong.BaseColor = UiTheme.Violet;
            btnDong.SetBounds(614, 474, 116, 38);
            btnDong.Click += btnDong_Click;

            AutoScaleMode = AutoScaleMode.None;
            Text = "Giới thiệu chương trình";
            ClientSize = new Size(760, 528);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;

            Controls.Add(pnlTop);
            Controls.AddRange(new Control[] {
                lblMucTieuTitle, lstMucTieu, lblKyThuatTitle, lstKyThuat,
                lblPhimTitle, lstPhimTat, lblKienTrucTitle, lblKienTruc, btnDong });

            Load += frmGioiThieu_Load;
            // Cháº¿ Ä‘á»™ chá»¥p áº£nh bÃ¡o cÃ¡o (--screenshots): tá»± Ä‘Ã³ng Form Ä‘á»ƒ bá»™ chá»¥p cháº¡y tiáº¿p ká»‹ch báº£n sau.
            if (CheDoChupAnh.DangChup) { Shown += delegate { BeginInvoke((MethodInvoker)Close); }; }
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
            l.Font = UiTheme.FontBody(8.75f);
            l.ForeColor = UiTheme.InkSoft;
            l.BorderStyle = BorderStyle.FixedSingle;
            l.IntegralHeight = false;
            l.ItemHeight = 18;
            l.SetBounds(left, top, width, height);
            return l;
        }
    }
}
