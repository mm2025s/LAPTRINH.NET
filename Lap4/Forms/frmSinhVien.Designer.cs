namespace Lab04_WinFormsBasic.Forms
{
    partial class frmSinhVien
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.grpThongTin = new System.Windows.Forms.GroupBox();
            this.lblMaSV = new System.Windows.Forms.Label();
            this.txtMaSV = new System.Windows.Forms.TextBox();
            this.lblHoTen = new System.Windows.Forms.Label();
            this.txtHoTen = new System.Windows.Forms.TextBox();
            this.lblNgaySinh = new System.Windows.Forms.Label();
            this.dtpNgaySinh = new System.Windows.Forms.DateTimePicker();
            this.lblTuoi = new System.Windows.Forms.Label();
            this.lblGioiTinh = new System.Windows.Forms.Label();
            this.radNam = new System.Windows.Forms.RadioButton();
            this.radNu = new System.Windows.Forms.RadioButton();
            this.lblKhoa = new System.Windows.Forms.Label();
            this.cboKhoa = new System.Windows.Forms.ComboBox();
            this.grpSoThich = new System.Windows.Forms.GroupBox();
            this.chkDocSach = new System.Windows.Forms.CheckBox();
            this.chkTheThao = new System.Windows.Forms.CheckBox();
            this.chkLapTrinh = new System.Windows.Forms.CheckBox();
            this.chkAmNhac = new System.Windows.Forms.CheckBox();
            this.chkDuLich = new System.Windows.Forms.CheckBox();
            this.lblSoThich = new System.Windows.Forms.Label();
            this.grpAnh = new System.Windows.Forms.GroupBox();
            this.picAnh = new System.Windows.Forms.PictureBox();
            this.txtDuongDanAnh = new System.Windows.Forms.TextBox();
            this.btnChonAnh = new System.Windows.Forms.Button();
            this.btnXoaAnh = new System.Windows.Forms.Button();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnHienThi = new System.Windows.Forms.Button();
            this.btnLamMoi = new System.Windows.Forms.Button();
            this.btnDong = new System.Windows.Forms.Button();
            this.lblHeader = new System.Windows.Forms.Label();

            this.grpThongTin.SuspendLayout();
            this.grpSoThich.SuspendLayout();
            this.grpAnh.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAnh)).BeginInit();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();

            // ---- lblHeader ----
            this.lblHeader.AutoSize = false;
            this.lblHeader.BackColor = System.Drawing.Color.FromArgb(63, 81, 181);
            this.lblHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblHeader.ForeColor = System.Drawing.Color.White;
            this.lblHeader.Height = 50;
            this.lblHeader.Text = "📋 Bài 1-5: Thông Tin Sinh Viên";
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // ---- grpThongTin ----
            this.grpThongTin.Location = new System.Drawing.Point(12, 62);
            this.grpThongTin.Size = new System.Drawing.Size(380, 240);
            this.grpThongTin.Text = "Thông tin cơ bản";
            this.grpThongTin.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpThongTin.ForeColor = System.Drawing.Color.FromArgb(63, 81, 181);

            // Mã SV
            this.lblMaSV.Location = new System.Drawing.Point(10, 28); this.lblMaSV.Size = new System.Drawing.Size(90, 23);
            this.lblMaSV.Text = "Mã SV (*):"; this.lblMaSV.Font = new System.Drawing.Font("Segoe UI", 9F); this.lblMaSV.ForeColor = System.Drawing.Color.FromArgb(50,50,50);
            this.txtMaSV.Location = new System.Drawing.Point(110, 25); this.txtMaSV.Size = new System.Drawing.Size(255, 23);
            this.txtMaSV.Font = new System.Drawing.Font("Segoe UI", 9F); this.txtMaSV.PlaceholderText = "VD: SV001";

            // Họ tên
            this.lblHoTen.Location = new System.Drawing.Point(10, 60); this.lblHoTen.Size = new System.Drawing.Size(90, 23);
            this.lblHoTen.Text = "Họ tên (*):"; this.lblHoTen.Font = new System.Drawing.Font("Segoe UI", 9F); this.lblHoTen.ForeColor = System.Drawing.Color.FromArgb(50,50,50);
            this.txtHoTen.Location = new System.Drawing.Point(110, 57); this.txtHoTen.Size = new System.Drawing.Size(255, 23);
            this.txtHoTen.Font = new System.Drawing.Font("Segoe UI", 9F); this.txtHoTen.PlaceholderText = "VD: Nguyễn Văn An";

            // Ngày sinh
            this.lblNgaySinh.Location = new System.Drawing.Point(10, 94); this.lblNgaySinh.Size = new System.Drawing.Size(90, 23);
            this.lblNgaySinh.Text = "Ngày sinh:"; this.lblNgaySinh.Font = new System.Drawing.Font("Segoe UI", 9F); this.lblNgaySinh.ForeColor = System.Drawing.Color.FromArgb(50,50,50);
            this.dtpNgaySinh.Location = new System.Drawing.Point(110, 91); this.dtpNgaySinh.Size = new System.Drawing.Size(170, 23);
            this.dtpNgaySinh.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpNgaySinh.Value = System.DateTime.Today.AddYears(-20);
            this.dtpNgaySinh.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblTuoi.Location = new System.Drawing.Point(290, 94); this.lblTuoi.Size = new System.Drawing.Size(80, 23);
            this.lblTuoi.Text = "Tuổi: 20"; this.lblTuoi.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblTuoi.ForeColor = System.Drawing.Color.FromArgb(0, 120, 60);

            // Giới tính
            this.lblGioiTinh.Location = new System.Drawing.Point(10, 128); this.lblGioiTinh.Size = new System.Drawing.Size(90, 23);
            this.lblGioiTinh.Text = "Giới tính:"; this.lblGioiTinh.Font = new System.Drawing.Font("Segoe UI", 9F); this.lblGioiTinh.ForeColor = System.Drawing.Color.FromArgb(50,50,50);
            this.radNam.Location = new System.Drawing.Point(110, 126); this.radNam.Size = new System.Drawing.Size(60, 23);
            this.radNam.Text = "Nam"; this.radNam.Font = new System.Drawing.Font("Segoe UI", 9F); this.radNam.Checked = true;
            this.radNu.Location = new System.Drawing.Point(180, 126); this.radNu.Size = new System.Drawing.Size(50, 23);
            this.radNu.Text = "Nữ"; this.radNu.Font = new System.Drawing.Font("Segoe UI", 9F);

            // Khoa
            this.lblKhoa.Location = new System.Drawing.Point(10, 162); this.lblKhoa.Size = new System.Drawing.Size(90, 23);
            this.lblKhoa.Text = "Khoa:"; this.lblKhoa.Font = new System.Drawing.Font("Segoe UI", 9F); this.lblKhoa.ForeColor = System.Drawing.Color.FromArgb(50,50,50);
            this.cboKhoa.Location = new System.Drawing.Point(110, 159); this.cboKhoa.Size = new System.Drawing.Size(255, 23);
            this.cboKhoa.Font = new System.Drawing.Font("Segoe UI", 9F);

            this.grpThongTin.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblMaSV, this.txtMaSV, this.lblHoTen, this.txtHoTen,
                this.lblNgaySinh, this.dtpNgaySinh, this.lblTuoi,
                this.lblGioiTinh, this.radNam, this.radNu,
                this.lblKhoa, this.cboKhoa });

            // ---- grpSoThich ----
            this.grpSoThich.Location = new System.Drawing.Point(12, 315);
            this.grpSoThich.Size = new System.Drawing.Size(380, 110);
            this.grpSoThich.Text = "Sở thích (Bài 4)";
            this.grpSoThich.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpSoThich.ForeColor = System.Drawing.Color.FromArgb(63, 81, 181);

            this.chkDocSach.Location = new System.Drawing.Point(10, 25); this.chkDocSach.Size = new System.Drawing.Size(90, 20); this.chkDocSach.Text = "Đọc sách"; this.chkDocSach.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkTheThao.Location = new System.Drawing.Point(110, 25); this.chkTheThao.Size = new System.Drawing.Size(80, 20); this.chkTheThao.Text = "Thể thao"; this.chkTheThao.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkLapTrinh.Location = new System.Drawing.Point(200, 25); this.chkLapTrinh.Size = new System.Drawing.Size(90, 20); this.chkLapTrinh.Text = "Lập trình"; this.chkLapTrinh.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkAmNhac.Location = new System.Drawing.Point(10, 52); this.chkAmNhac.Size = new System.Drawing.Size(90, 20); this.chkAmNhac.Text = "Âm nhạc"; this.chkAmNhac.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkDuLich.Location = new System.Drawing.Point(110, 52); this.chkDuLich.Size = new System.Drawing.Size(80, 20); this.chkDuLich.Text = "Du lịch"; this.chkDuLich.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSoThich.Location = new System.Drawing.Point(10, 78); this.lblSoThich.Size = new System.Drawing.Size(360, 20);
            this.lblSoThich.Text = "Chưa chọn sở thích"; this.lblSoThich.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Italic);
            this.lblSoThich.ForeColor = System.Drawing.Color.FromArgb(100, 100, 100);

            this.grpSoThich.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.chkDocSach, this.chkTheThao, this.chkLapTrinh,
                this.chkAmNhac, this.chkDuLich, this.lblSoThich });

            // ---- grpAnh ----
            this.grpAnh.Location = new System.Drawing.Point(404, 62);
            this.grpAnh.Size = new System.Drawing.Size(220, 363);
            this.grpAnh.Text = "Ảnh đại diện (Bài 5)";
            this.grpAnh.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpAnh.ForeColor = System.Drawing.Color.FromArgb(63, 81, 181);

            this.picAnh.Location = new System.Drawing.Point(10, 25); this.picAnh.Size = new System.Drawing.Size(200, 200);
            this.picAnh.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picAnh.BackColor = System.Drawing.Color.FromArgb(230, 232, 240);
            this.picAnh.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;

            this.txtDuongDanAnh.Location = new System.Drawing.Point(10, 235); this.txtDuongDanAnh.Size = new System.Drawing.Size(200, 23);
            this.txtDuongDanAnh.Font = new System.Drawing.Font("Segoe UI", 8F); this.txtDuongDanAnh.ReadOnly = true;
            this.txtDuongDanAnh.PlaceholderText = "Chưa chọn ảnh...";

            this.btnChonAnh.Location = new System.Drawing.Point(10, 265); this.btnChonAnh.Size = new System.Drawing.Size(200, 32);
            this.btnChonAnh.Text = "📂 Chọn ảnh..."; this.btnChonAnh.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnChonAnh.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnChonAnh.FlatAppearance.BorderSize = 0;
            this.btnChonAnh.BackColor = System.Drawing.Color.FromArgb(63, 81, 181); this.btnChonAnh.ForeColor = System.Drawing.Color.White;
            this.btnChonAnh.Cursor = System.Windows.Forms.Cursors.Hand;

            this.btnXoaAnh.Location = new System.Drawing.Point(10, 305); this.btnXoaAnh.Size = new System.Drawing.Size(200, 32);
            this.btnXoaAnh.Text = "🗑️ Xóa ảnh"; this.btnXoaAnh.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnXoaAnh.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnXoaAnh.FlatAppearance.BorderSize = 0;
            this.btnXoaAnh.BackColor = System.Drawing.Color.FromArgb(198, 40, 40); this.btnXoaAnh.ForeColor = System.Drawing.Color.White;
            this.btnXoaAnh.Cursor = System.Windows.Forms.Cursors.Hand;

            this.grpAnh.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.picAnh, this.txtDuongDanAnh, this.btnChonAnh, this.btnXoaAnh });

            // ---- pnlButtons ----
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlButtons.Height = 55;
            this.pnlButtons.BackColor = System.Drawing.Color.FromArgb(230, 232, 245);
            this.pnlButtons.Padding = new System.Windows.Forms.Padding(10, 8, 10, 8);

            this.btnHienThi.Size = new System.Drawing.Size(150, 36); this.btnHienThi.Location = new System.Drawing.Point(70, 9);
            this.btnHienThi.Text = "✅ Hiển thị"; this.btnHienThi.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnHienThi.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnHienThi.FlatAppearance.BorderSize = 0;
            this.btnHienThi.BackColor = System.Drawing.Color.FromArgb(56, 142, 60); this.btnHienThi.ForeColor = System.Drawing.Color.White;
            this.btnHienThi.Cursor = System.Windows.Forms.Cursors.Hand;

            this.btnLamMoi.Size = new System.Drawing.Size(130, 36); this.btnLamMoi.Location = new System.Drawing.Point(240, 9);
            this.btnLamMoi.Text = "🔄 Làm mới"; this.btnLamMoi.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnLamMoi.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnLamMoi.FlatAppearance.BorderSize = 0;
            this.btnLamMoi.BackColor = System.Drawing.Color.FromArgb(245, 124, 0); this.btnLamMoi.ForeColor = System.Drawing.Color.White;
            this.btnLamMoi.Cursor = System.Windows.Forms.Cursors.Hand;

            this.btnDong.Size = new System.Drawing.Size(120, 36); this.btnDong.Location = new System.Drawing.Point(390, 9);
            this.btnDong.Text = "❌ Đóng"; this.btnDong.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnDong.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnDong.FlatAppearance.BorderSize = 0;
            this.btnDong.BackColor = System.Drawing.Color.FromArgb(97, 97, 97); this.btnDong.ForeColor = System.Drawing.Color.White;
            this.btnDong.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDong.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            this.pnlButtons.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.btnHienThi, this.btnLamMoi, this.btnDong });

            // ---- frmSinhVien ----
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(245, 246, 252);
            this.ClientSize = new System.Drawing.Size(638, 500);
            this.Controls.Add(this.grpThongTin);
            this.Controls.Add(this.grpSoThich);
            this.Controls.Add(this.grpAnh);
            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.pnlButtons);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmSinhVien";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bài 1-5: Thông Tin Sinh Viên";
            this.CancelButton = this.btnDong;

            // Events
            this.Load += new System.EventHandler(this.frmSinhVien_Load);
            this.btnHienThi.Click += new System.EventHandler(this.btnHienThi_Click);
            this.btnLamMoi.Click += new System.EventHandler(this.btnLamMoi_Click);
            this.btnChonAnh.Click += new System.EventHandler(this.btnChonAnh_Click);
            this.btnXoaAnh.Click += new System.EventHandler(this.btnXoaAnh_Click);
            this.dtpNgaySinh.ValueChanged += new System.EventHandler(this.dtpNgaySinh_ValueChanged);
            this.chkDocSach.CheckedChanged += new System.EventHandler(this.AnySoThich_CheckedChanged);
            this.chkTheThao.CheckedChanged += new System.EventHandler(this.AnySoThich_CheckedChanged);
            this.chkLapTrinh.CheckedChanged += new System.EventHandler(this.AnySoThich_CheckedChanged);
            this.chkAmNhac.CheckedChanged += new System.EventHandler(this.AnySoThich_CheckedChanged);
            this.chkDuLich.CheckedChanged += new System.EventHandler(this.AnySoThich_CheckedChanged);

            this.grpThongTin.ResumeLayout(false);
            this.grpSoThich.ResumeLayout(false);
            this.grpAnh.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picAnh)).EndInit();
            this.pnlButtons.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.GroupBox grpThongTin;
        private System.Windows.Forms.Label lblMaSV;
        private System.Windows.Forms.TextBox txtMaSV;
        private System.Windows.Forms.Label lblHoTen;
        private System.Windows.Forms.TextBox txtHoTen;
        private System.Windows.Forms.Label lblNgaySinh;
        private System.Windows.Forms.DateTimePicker dtpNgaySinh;
        private System.Windows.Forms.Label lblTuoi;
        private System.Windows.Forms.Label lblGioiTinh;
        private System.Windows.Forms.RadioButton radNam;
        private System.Windows.Forms.RadioButton radNu;
        private System.Windows.Forms.Label lblKhoa;
        private System.Windows.Forms.ComboBox cboKhoa;
        private System.Windows.Forms.GroupBox grpSoThich;
        private System.Windows.Forms.CheckBox chkDocSach;
        private System.Windows.Forms.CheckBox chkTheThao;
        private System.Windows.Forms.CheckBox chkLapTrinh;
        private System.Windows.Forms.CheckBox chkAmNhac;
        private System.Windows.Forms.CheckBox chkDuLich;
        private System.Windows.Forms.Label lblSoThich;
        private System.Windows.Forms.GroupBox grpAnh;
        private System.Windows.Forms.PictureBox picAnh;
        private System.Windows.Forms.TextBox txtDuongDanAnh;
        private System.Windows.Forms.Button btnChonAnh;
        private System.Windows.Forms.Button btnXoaAnh;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Button btnHienThi;
        private System.Windows.Forms.Button btnLamMoi;
        private System.Windows.Forms.Button btnDong;
    }
}
