using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Lab05_MultiForm.Helpers;

namespace Lab05_MultiForm.Forms
{
    partial class frmTimKiem
    {
        private IContainer components = null;

        private InfoStrip infoStrip;
        private Label lblTuKhoa;
        private TextBox txtTuKhoa;
        private RadioButton radMa, radTen, radKhoa;
        private AccentButton btnTim;
        private ListView lstKetQua;
        private Label lblTong;
        private AccentButton btnChon, btnHuy;

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
            infoStrip.Accent = UiTheme.Teal;
            infoStrip.ChipText = "7";
            infoStrip.Title = "Hộp thoại tìm kiếm";
            infoStrip.Subtitle = "Nhận dữ liệu qua constructor - trả kết quả về Form gọi qua thuộc tính KetQua + DialogResult";
            infoStrip.RightTop = "Tìm theo";

            lblTuKhoa = UiFactory.FieldLabel("Từ khóa");
            lblTuKhoa.SetBounds(20, 92, 300, 18);
            txtTuKhoa = UiFactory.TextBox("txtTuKhoa", "VD: SV01, Nguyễn, thông tin", false);
            txtTuKhoa.SetBounds(20, 110, 300, 26);
            txtTuKhoa.KeyDown += txtTuKhoa_KeyDown;

            radMa = new RadioButton();
            radMa.Text = "Mã SV";
            radMa.Font = UiTheme.FontBody(8.75f);
            radMa.AutoSize = true;
            radMa.Checked = true;
            radMa.BackColor = Color.Transparent;
            radMa.Location = new Point(340, 114);
            radMa.CheckedChanged += chkKieuTim_CheckedChanged;

            radTen = new RadioButton();
            radTen.Text = "Họ tên";
            radTen.Font = UiTheme.FontBody(8.75f);
            radTen.AutoSize = true;
            radTen.BackColor = Color.Transparent;
            radTen.Location = new Point(414, 114);
            radTen.CheckedChanged += chkKieuTim_CheckedChanged;

            radKhoa = new RadioButton();
            radKhoa.Text = "Khoa";
            radKhoa.Font = UiTheme.FontBody(8.75f);
            radKhoa.AutoSize = true;
            radKhoa.BackColor = Color.Transparent;
            radKhoa.Location = new Point(488, 114);
            radKhoa.CheckedChanged += chkKieuTim_CheckedChanged;

            btnTim = new AccentButton();
            btnTim.Name = "btnTim";
            btnTim.Text = "Tìm";
            btnTim.BaseColor = UiTheme.Teal;
            btnTim.SetBounds(578, 108, 90, 30);
            btnTim.Click += btnTim_Click;

            lstKetQua = new ListView();
            lstKetQua.View = View.Details;
            lstKetQua.FullRowSelect = true;
            lstKetQua.MultiSelect = false;
            lstKetQua.HideSelection = false;
            lstKetQua.GridLines = false;
            lstKetQua.Font = UiTheme.FontBody(9.25f);
            lstKetQua.BackColor = Color.White;
            lstKetQua.BorderStyle = BorderStyle.FixedSingle;
            lstKetQua.SetBounds(20, 154, 648, 200);
            lstKetQua.Columns.Add("Mã SV", 76);
            lstKetQua.Columns.Add("Họ và tên", 196);
            lstKetQua.Columns.Add("Khoa", 196);
            lstKetQua.Columns.Add("Điểm", 60, HorizontalAlignment.Center);
            lstKetQua.Columns.Add("Xếp loại", 104);
            lstKetQua.DoubleClick += lstKetQua_DoubleClick;
            lstKetQua.SelectedIndexChanged += (s, e) => btnChon.Enabled = lstKetQua.SelectedItems.Count > 0;

            lblTong = new Label();
            lblTong.Text = "Đang tìm trong 0 sinh viên.";
            lblTong.Font = UiTheme.FontBody(8.75f);
            lblTong.ForeColor = UiTheme.Muted;
            lblTong.AutoSize = false;
            lblTong.AutoEllipsis = true;
            lblTong.BackColor = Color.Transparent;
            lblTong.SetBounds(20, 360, 430, 20);

            btnChon = new AccentButton();
            btnChon.Name = "btnChon";
            btnChon.Text = "Chọn sinh viên này";
            btnChon.BaseColor = UiTheme.Green;
            btnChon.SetBounds(430, 386, 140, 38);
            btnChon.Click += btnChon_Click;

            btnHuy = new AccentButton();
            btnHuy.Name = "btnHuy";
            btnHuy.Text = "Hủy";
            btnHuy.BaseColor = UiTheme.Slate;
            btnHuy.OutlineOnly = true;
            btnHuy.SetBounds(578, 386, 90, 38);
            btnHuy.Click += btnHuy_Click;

            AutoScaleMode = AutoScaleMode.None;
            Text = "Tìm kiếm sinh viên";
            ClientSize = new Size(688, 440);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            CancelButton = btnHuy;

            Controls.Add(infoStrip);
            Controls.AddRange(new Control[] {
                lblTuKhoa, txtTuKhoa, radMa, radTen, radKhoa, btnTim, lstKetQua, lblTong, btnChon, btnHuy });

            Load += frmTimKiem_Load;
            // Cháº¿ Ä‘á»™ chá»¥p áº£nh bÃ¡o cÃ¡o (--screenshots): tá»± Ä‘Ã³ng Form Ä‘á»ƒ bá»™ chá»¥p cháº¡y tiáº¿p ká»‹ch báº£n sau.
            if (CheDoChupAnh.DangChup) { Shown += delegate { BeginInvoke((MethodInvoker)Close); }; }
        }
    }
}
