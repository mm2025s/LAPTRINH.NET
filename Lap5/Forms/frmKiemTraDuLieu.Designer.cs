using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Lab05_MultiForm.Helpers;

namespace Lab05_MultiForm.Forms
{
    partial class frmKiemTraDuLieu
    {
        private IContainer components = null;

        private InfoStrip infoStrip;
        private Label lblMa, lblTen, lblDiem, lblNgaySinh, lblKhoa;
        private TextBox txtMa, txtTen, txtDiem;
        private DateTimePicker dtpNgaySinh;
        private ComboBox cboKhoa;
        private AccentButton btnKiemTra, btnDong;
        private ListView lstKetQua;
        private Label lblKetLuan;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new Container();

            infoStrip = new InfoStrip();
            infoStrip.Dock = DockStyle.Top;
            infoStrip.Height = 78;
            infoStrip.Accent = UiTheme.Orange;
            infoStrip.ChipText = "K";
            infoStrip.Title = "Kiểm tra dữ liệu bằng hàm dùng chung";
            infoStrip.Subtitle = "Cùng một bộ hàm KiemTra.* được dùng ở frmSinhVien, frmChiTietSinhVien và Form này";
            infoStrip.RightTop = "Helpers/KiemTra.cs";

            lblMa = UiFactory.FieldLabel("Mã sinh viên");
            lblMa.SetBounds(20, 94, 140, 18);
            txtMa = UiFactory.TextBox("txtMa", "SV10", false);
            txtMa.SetBounds(20, 112, 140, 26);

            lblTen = UiFactory.FieldLabel("Họ và tên");
            lblTen.SetBounds(172, 94, 200, 18);
            txtTen = UiFactory.TextBox("txtTen", "Nguyễn Văn An", false);
            txtTen.SetBounds(172, 112, 200, 26);

            lblDiem = UiFactory.FieldLabel("Điểm");
            lblDiem.SetBounds(384, 94, 80, 18);
            txtDiem = UiFactory.TextBox("txtDiem", "8.5", false);
            txtDiem.SetBounds(384, 112, 80, 26);
            txtDiem.KeyPress += txtDiem_KeyPress;

            lblNgaySinh = UiFactory.FieldLabel("Ngày sinh");
            lblNgaySinh.SetBounds(476, 94, 130, 18);
            dtpNgaySinh = UiFactory.DatePicker("dtpNgaySinh");
            dtpNgaySinh.SetBounds(476, 114, 130, 24);
            dtpNgaySinh.Value = new DateTime(2005, 3, 15);

            lblKhoa = UiFactory.FieldLabel("Khoa");
            lblKhoa.SetBounds(20, 148, 200, 18);
            cboKhoa = UiFactory.ComboBox("cboKhoa");
            cboKhoa.SetBounds(20, 166, 200, 22);

            btnKiemTra = new AccentButton();
            btnKiemTra.Name = "btnKiemTra";
            btnKiemTra.Text = "Kiểm tra";
            btnKiemTra.BaseColor = UiTheme.Orange;
            btnKiemTra.SetBounds(236, 160, 120, 34);
            btnKiemTra.Click += btnKiemTra_Click;

            btnDong = new AccentButton();
            btnDong.Name = "btnDong";
            btnDong.Text = "Đóng";
            btnDong.BaseColor = UiTheme.Slate;
            btnDong.OutlineOnly = true;
            btnDong.SetBounds(370, 160, 100, 34);
            btnDong.Click += btnDong_Click;

            lstKetQua = new ListView();
            lstKetQua.View = View.Details;
            lstKetQua.FullRowSelect = true;
            lstKetQua.Font = UiTheme.FontBody(9f);
            lstKetQua.BorderStyle = BorderStyle.FixedSingle;
            lstKetQua.SetBounds(20, 206, 586, 152);
            lstKetQua.Columns.Add("Kết quả", 70);
            lstKetQua.Columns.Add("Quy tắc kiểm tra", 180);
            lstKetQua.Columns.Add("Chi tiết", 330);

            lblKetLuan = new Label();
            lblKetLuan.Text = "KẾT LUẬN:";
            lblKetLuan.Font = UiTheme.FontBold(9f);
            lblKetLuan.ForeColor = UiTheme.Muted;
            lblKetLuan.AutoSize = false;
            lblKetLuan.BackColor = Color.Transparent;
            lblKetLuan.SetBounds(20, 364, 586, 22);

            AutoScaleMode = AutoScaleMode.None;
            Text = "Kiểm tra dữ liệu";
            ClientSize = new Size(626, 396);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;

            Controls.Add(infoStrip);
            Controls.AddRange(new Control[] {
                lblMa, txtMa, lblTen, txtTen, lblDiem, txtDiem, lblNgaySinh, dtpNgaySinh,
                lblKhoa, cboKhoa, btnKiemTra, btnDong, lstKetQua, lblKetLuan });

            Load += frmKiemTraDuLieu_Load;
            // Cháº¿ Ä‘á»™ chá»¥p áº£nh bÃ¡o cÃ¡o (--screenshots): tá»± Ä‘Ã³ng Form Ä‘á»ƒ bá»™ chá»¥p cháº¡y tiáº¿p ká»‹ch báº£n sau.
            if (CheDoChupAnh.DangChup) { Shown += delegate { BeginInvoke((MethodInvoker)Close); }; }
        }
    }
}
