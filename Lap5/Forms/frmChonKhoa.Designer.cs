using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Lab05_MultiForm.Helpers;

namespace Lab05_MultiForm.Forms
{
    partial class frmChonKhoa
    {
        private IContainer components = null;

        private InfoStrip infoStrip;
        private Label lblDem;
        private ListView lstKhoa;
        private Label lblChon;
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
            infoStrip.Accent = UiTheme.Rose;
            infoStrip.ChipText = "8";
            infoStrip.Title = "Chọn khoa";
            infoStrip.Subtitle = "Danh sách khoa nhận qua constructor - trả về khoa được chọn qua thuộc tính KhoaDuocChon";
            infoStrip.RightTop = "Không dùng biến toàn cục";

            lblDem = new Label();
            lblDem.Text = "0 khoa";
            lblDem.Font = UiTheme.FontBody(8.75f);
            lblDem.ForeColor = UiTheme.Muted;
            lblDem.AutoSize = false;
            lblDem.BackColor = Color.Transparent;
            lblDem.SetBounds(20, 92, 420, 20);

            lstKhoa = new ListView();
            lstKhoa.View = View.Details;
            lstKhoa.FullRowSelect = true;
            lstKhoa.MultiSelect = false;
            lstKhoa.HideSelection = false;
            lstKhoa.Font = UiTheme.FontBody(9.5f);
            lstKhoa.BackColor = Color.White;
            lstKhoa.SetBounds(20, 116, 420, 200);
            lstKhoa.Columns.Add("Mã khoa", 90);
            lstKhoa.Columns.Add("Tên khoa", 310);
            lstKhoa.SelectedIndexChanged += lstKhoa_SelectedIndexChanged;
            lstKhoa.DoubleClick += lstKhoa_DoubleClick;

            lblChon = new Label();
            lblChon.Text = "Chưa chọn khoa";
            lblChon.Font = UiTheme.FontBold(9f);
            lblChon.ForeColor = UiTheme.Muted;
            lblChon.AutoSize = false;
            lblChon.AutoEllipsis = true;
            lblChon.BackColor = Color.Transparent;
            lblChon.SetBounds(20, 326, 420, 24);

            btnChon = new AccentButton();
            btnChon.Name = "btnChon";
            btnChon.Text = "Chọn";
            btnChon.BaseColor = UiTheme.Rose;
            btnChon.SetBounds(226, 356, 104, 38);
            btnChon.Click += btnChon_Click;

            btnHuy = new AccentButton();
            btnHuy.Name = "btnHuy";
            btnHuy.Text = "Hủy";
            btnHuy.BaseColor = UiTheme.Slate;
            btnHuy.OutlineOnly = true;
            btnHuy.SetBounds(344, 356, 96, 38);
            btnHuy.Click += btnHuy_Click;

            AutoScaleMode = AutoScaleMode.None;
            Text = "Chọn khoa";
            ClientSize = new Size(460, 410);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            CancelButton = btnHuy;

            Controls.Add(infoStrip);
            Controls.AddRange(new Control[] { lblDem, lstKhoa, lblChon, btnChon, btnHuy });

            Load += frmChonKhoa_Load;
            // Cháº¿ Ä‘á»™ chá»¥p áº£nh bÃ¡o cÃ¡o (--screenshots): tá»± Ä‘Ã³ng Form Ä‘á»ƒ bá»™ chá»¥p cháº¡y tiáº¿p ká»‹ch báº£n sau.
            if (CheDoChupAnh.DangChup) { Shown += delegate { BeginInvoke((MethodInvoker)Close); }; }
        }
    }
}
