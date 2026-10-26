using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Lab05_MultiForm.Helpers;
using Lab05_MultiForm.Models;

namespace Lab05_MultiForm.Forms
{
    partial class frmDanhSach
    {
        private IContainer components = null;

        private InfoStrip infoStrip;
        private Label lblTim, lblKhoa;
        private TextBox txtTim;
        private ComboBox cboKhoa;
        private Label lblDem;
        private DataGridView dgvDanhSach;
        private AccentButton btnXemChiTiet;

        private CardPanel cardChon;
        private RoundedPictureBox picChon;
        private Label lblChonMa, lblChonTen, lblChonKhoa, lblChonDiem, lblChon;

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
            infoStrip.ChipText = "5";
            infoStrip.Title = "Danh sách sinh viên";
            infoStrip.Subtitle = "Form con MDI · chọn dòng để xem nhanh · mở Form chi tiết qua sự kiện MoChiTiet (frmMain mở giúp)";
            infoStrip.RightTop = "Dữ liệu mẫu trong bộ nhớ";
            infoStrip.RightBottom = "Đúp chuột vào một dòng để mở chi tiết";

            // ============================ THANH LỌC ============================
            lblTim = UiFactory.FieldLabel("Tìm theo mã hoặc họ tên");
            lblTim.SetBounds(22, 106, 260, 18);
            txtTim = UiFactory.TextBox("txtTim", "Gõ để lọc nhanh...", false);
            txtTim.SetBounds(22, 124, 260, 26);
            txtTim.TextChanged += txtTim_TextChanged;

            lblKhoa = UiFactory.FieldLabel("Khoa");
            lblKhoa.SetBounds(300, 106, 200, 18);
            cboKhoa = UiFactory.ComboBox("cboKhoa");
            cboKhoa.SetBounds(300, 126, 200, 22);
            cboKhoa.Items.Add("Tất cả khoa");
            foreach (Khoa k in DuLieuMau.DanhSachKhoa()) cboKhoa.Items.Add(k);
            cboKhoa.SelectedIndex = 0;
            cboKhoa.SelectedIndexChanged += cboKhoa_SelectedIndexChanged;

            lblDem = new Label();
            lblDem.Text = "Hiển thị 0 sinh viên";
            lblDem.Font = UiTheme.FontBody(8.75f);
            lblDem.ForeColor = UiTheme.Muted;
            lblDem.AutoSize = true;
            lblDem.BackColor = Color.Transparent;
            lblDem.Location = new Point(520, 130);

            // ============================ LƯỚI ============================
            dgvDanhSach = new DataGridView();
            dgvDanhSach.Name = "dgvDanhSach";
            dgvDanhSach.SetBounds(22, 162, 712, 250);
            dgvDanhSach.AllowUserToAddRows = false;
            dgvDanhSach.AllowUserToDeleteRows = false;
            dgvDanhSach.AllowUserToResizeRows = false;
            dgvDanhSach.ReadOnly = true;
            dgvDanhSach.RowHeadersVisible = false;
            dgvDanhSach.MultiSelect = false;
            dgvDanhSach.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDanhSach.BackgroundColor = Color.White;
            dgvDanhSach.BorderStyle = BorderStyle.None;
            dgvDanhSach.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvDanhSach.GridColor = UiTheme.Border;
            dgvDanhSach.EnableHeadersVisualStyles = false;
            dgvDanhSach.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvDanhSach.Font = UiTheme.FontBody(9.25f);
            dgvDanhSach.RowTemplate.Height = 30;
            dgvDanhSach.ColumnHeadersHeight = 36;
            dgvDanhSach.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvDanhSach.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 238, 252);
            dgvDanhSach.ColumnHeadersDefaultCellStyle.ForeColor = UiTheme.Ink;
            dgvDanhSach.ColumnHeadersDefaultCellStyle.Font = UiTheme.FontBold(8.5f);
            dgvDanhSach.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(240, 238, 252);
            dgvDanhSach.DefaultCellStyle.SelectionBackColor = Color.FromArgb(236, 232, 255);
            dgvDanhSach.DefaultCellStyle.SelectionForeColor = UiTheme.Ink;
            dgvDanhSach.DefaultCellStyle.Padding = new Padding(4, 0, 4, 0);
            dgvDanhSach.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 253);

            dgvDanhSach.Columns.Add("STT", "#");
            dgvDanhSach.Columns.Add("MaSV", "Mã SV");
            dgvDanhSach.Columns.Add("HoTen", "Họ và tên");
            dgvDanhSach.Columns.Add("NgaySinh", "Ngày sinh");
            dgvDanhSach.Columns.Add("Tuoi", "Tuổi");
            dgvDanhSach.Columns.Add("Khoa", "Khoa");
            dgvDanhSach.Columns.Add("Diem", "Điểm");
            dgvDanhSach.Columns.Add("XepLoai", "Xếp loại");

            dgvDanhSach.Columns["STT"].FillWeight = 22;
            dgvDanhSach.Columns["MaSV"].FillWeight = 50;
            dgvDanhSach.Columns["HoTen"].FillWeight = 100;
            dgvDanhSach.Columns["NgaySinh"].FillWeight = 62;
            dgvDanhSach.Columns["Tuoi"].FillWeight = 32;
            dgvDanhSach.Columns["Khoa"].FillWeight = 95;
            dgvDanhSach.Columns["Diem"].FillWeight = 40;
            dgvDanhSach.Columns["XepLoai"].FillWeight = 62;

            dgvDanhSach.Columns["STT"].DefaultCellStyle.ForeColor = UiTheme.Muted;
            dgvDanhSach.Columns["MaSV"].DefaultCellStyle.Font = UiTheme.FontBold(9.25f);
            dgvDanhSach.Columns["MaSV"].DefaultCellStyle.ForeColor = UiTheme.Indigo;
            dgvDanhSach.Columns["HoTen"].DefaultCellStyle.Font = UiTheme.FontBold(9.25f);
            dgvDanhSach.Columns["Diem"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvDanhSach.SelectionChanged += dgvDanhSach_SelectionChanged;
            dgvDanhSach.CellDoubleClick += dgvDanhSach_CellDoubleClick;

            // ============================ THẺ CHỌN ============================
            cardChon = new CardPanel();
            cardChon.SetBounds(22, 424, 490, 96);

            picChon = new RoundedPictureBox();
            picChon.SetBounds(16, 12, 72, 72);
            picChon.CornerRadius = 36;
            picChon.BackFill = Color.FromArgb(240, 238, 252);
            picChon.Image = UiTheme.CreateAvatarImage(string.Empty, 72);

            lblChonMa = new Label();
            lblChonMa.Text = "—";
            lblChonMa.Font = UiTheme.FontBold(9f);
            lblChonMa.ForeColor = UiTheme.Indigo;
            lblChonMa.AutoSize = true;
            lblChonMa.BackColor = Color.Transparent;
            lblChonMa.Location = new Point(102, 12);

            lblChonTen = new Label();
            lblChonTen.Text = "Chưa chọn sinh viên";
            lblChonTen.Font = UiTheme.FontTitle(11f);
            lblChonTen.ForeColor = UiTheme.Ink;
            lblChonTen.AutoSize = false;
            lblChonTen.AutoEllipsis = true;
            lblChonTen.BackColor = Color.Transparent;
            lblChonTen.SetBounds(102, 32, 372, 24);

            lblChonKhoa = new Label();
            lblChonKhoa.Text = string.Empty;
            lblChonKhoa.Font = UiTheme.FontBody(8.5f);
            lblChonKhoa.ForeColor = UiTheme.Muted;
            lblChonKhoa.AutoSize = false;
            lblChonKhoa.AutoEllipsis = true;
            lblChonKhoa.BackColor = Color.Transparent;
            lblChonKhoa.SetBounds(102, 56, 372, 18);

            lblChonDiem = new Label();
            lblChonDiem.Text = string.Empty;
            lblChonDiem.Font = UiTheme.FontBold(8.75f);
            lblChonDiem.ForeColor = UiTheme.Teal;
            lblChonDiem.AutoSize = true;
            lblChonDiem.BackColor = Color.Transparent;
            lblChonDiem.Location = new Point(102, 72);

            cardChon.Controls.AddRange(new Control[] { picChon, lblChonMa, lblChonTen, lblChonKhoa, lblChonDiem });

            // ============================ NÚT ============================
            btnXemChiTiet = new AccentButton();
            btnXemChiTiet.Name = "btnXemChiTiet";
            btnXemChiTiet.Text = "Xem chi tiết";
            btnXemChiTiet.BaseColor = UiTheme.Violet;
            btnXemChiTiet.SetBounds(532, 440, 202, 40);
            btnXemChiTiet.Click += btnXemChiTiet_Click;

            lblChon = new Label();
            lblChon.Text = "Chọn dòng để xem chi tiết";
            lblChon.Font = UiTheme.FontBody(8.25f);
            lblChon.ForeColor = UiTheme.Muted;
            lblChon.AutoSize = false;
            lblChon.TextAlign = ContentAlignment.MiddleRight;
            lblChon.BackColor = Color.Transparent;
            lblChon.SetBounds(452, 480, 282, 20);

            // ============================ FORM ============================
            AutoScaleMode = AutoScaleMode.None;
            Text = "Danh sách sinh viên";
            ClientSize = new Size(756, 544);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;

            Controls.Add(infoStrip);
            Controls.AddRange(new Control[] {
                lblTim, txtTim, lblKhoa, cboKhoa, lblDem, dgvDanhSach, cardChon, btnXemChiTiet, lblChon });

            Load += frmDanhSach_Load;
            // Cháº¿ Ä‘á»™ chá»¥p áº£nh bÃ¡o cÃ¡o (--screenshots): tá»± Ä‘Ã³ng Form Ä‘á»ƒ bá»™ chá»¥p cháº¡y tiáº¿p ká»‹ch báº£n sau.
            if (CheDoChupAnh.DangChup) { Shown += delegate { BeginInvoke((MethodInvoker)Close); }; }
        }
    }
}
