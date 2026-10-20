namespace Lab04_WinFormsBasic.Forms
{
    partial class frmDangNhap
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing) { if (disposing && (components != null)) components.Dispose(); base.Dispose(disposing); }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.lblHeader = new System.Windows.Forms.Label();
            this.lblSubHeader = new System.Windows.Forms.Label();
            this.grpDangNhap = new System.Windows.Forms.GroupBox();
            this.lblTenDN = new System.Windows.Forms.Label();
            this.txtTenDangNhap = new System.Windows.Forms.TextBox();
            this.lblMatKhau = new System.Windows.Forms.Label();
            this.txtMatKhau = new System.Windows.Forms.TextBox();
            this.btnToggleMatKhau = new System.Windows.Forms.Button();
            this.chkHienMatKhau = new System.Windows.Forms.CheckBox();
            this.chkGhiNho = new System.Windows.Forms.CheckBox();
            this.lblTrangThai = new System.Windows.Forms.Label();
            this.pnlBtns = new System.Windows.Forms.Panel();
            this.btnDangNhap = new System.Windows.Forms.Button();
            this.btnDangKy = new System.Windows.Forms.Button();
            this.btnHuy = new System.Windows.Forms.Button();

            this.grpDangNhap.SuspendLayout();
            this.pnlBtns.SuspendLayout();
            this.SuspendLayout();

            // lblHeader
            this.lblHeader.AutoSize = false;
            this.lblHeader.BackColor = System.Drawing.Color.FromArgb(49, 27, 146);
            this.lblHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblHeader.ForeColor = System.Drawing.Color.White;
            this.lblHeader.Height = 60;
            this.lblHeader.Text = "🔒 Đăng Nhập Hệ Thống";
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // lblSubHeader
            this.lblSubHeader.AutoSize = false;
            this.lblSubHeader.BackColor = System.Drawing.Color.FromArgb(69, 39, 160);
            this.lblSubHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSubHeader.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSubHeader.ForeColor = System.Drawing.Color.FromArgb(200, 190, 240);
            this.lblSubHeader.Height = 28;
            this.lblSubHeader.Text = "Bài 7: Form Đăng Nhập | IT3212 Lab 4";
            this.lblSubHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // grpDangNhap
            this.grpDangNhap.Location = new System.Drawing.Point(12, 102);
            this.grpDangNhap.Size = new System.Drawing.Size(330, 205);
            this.grpDangNhap.Text = "Thông tin đăng nhập";
            this.grpDangNhap.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpDangNhap.ForeColor = System.Drawing.Color.FromArgb(49, 27, 146);

            this.lblTenDN.Location = new System.Drawing.Point(10, 28); this.lblTenDN.Size = new System.Drawing.Size(110, 23); this.lblTenDN.Text = "Tên đăng nhập:"; this.lblTenDN.Font = new System.Drawing.Font("Segoe UI", 9F); this.lblTenDN.ForeColor = System.Drawing.Color.FromArgb(50,50,50);
            this.txtTenDangNhap.Location = new System.Drawing.Point(130, 25); this.txtTenDangNhap.Size = new System.Drawing.Size(185, 23); this.txtTenDangNhap.Font = new System.Drawing.Font("Segoe UI", 9F); this.txtTenDangNhap.PlaceholderText = "Nhập tên đăng nhập...";

            this.lblMatKhau.Location = new System.Drawing.Point(10, 62); this.lblMatKhau.Size = new System.Drawing.Size(110, 23); this.lblMatKhau.Text = "Mật khẩu:"; this.lblMatKhau.Font = new System.Drawing.Font("Segoe UI", 9F); this.lblMatKhau.ForeColor = System.Drawing.Color.FromArgb(50,50,50);
            this.txtMatKhau.Location = new System.Drawing.Point(130, 59); this.txtMatKhau.Size = new System.Drawing.Size(145, 23); this.txtMatKhau.Font = new System.Drawing.Font("Segoe UI", 9F); this.txtMatKhau.PasswordChar = '●'; this.txtMatKhau.PlaceholderText = "Mật khẩu...";

            this.btnToggleMatKhau.Location = new System.Drawing.Point(282, 59); this.btnToggleMatKhau.Size = new System.Drawing.Size(38, 23);
            this.btnToggleMatKhau.Text = "👁️"; this.btnToggleMatKhau.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnToggleMatKhau.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnToggleMatKhau.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(150, 150, 180);
            this.btnToggleMatKhau.BackColor = System.Drawing.Color.White; this.btnToggleMatKhau.Cursor = System.Windows.Forms.Cursors.Hand;

            this.chkHienMatKhau.Location = new System.Drawing.Point(130, 92); this.chkHienMatKhau.Size = new System.Drawing.Size(180, 20); this.chkHienMatKhau.Text = "Hiển thị mật khẩu"; this.chkHienMatKhau.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.chkGhiNho.Location = new System.Drawing.Point(130, 118); this.chkGhiNho.Size = new System.Drawing.Size(180, 20); this.chkGhiNho.Text = "Ghi nhớ đăng nhập"; this.chkGhiNho.Font = new System.Drawing.Font("Segoe UI", 8.5F); this.chkGhiNho.ForeColor = System.Drawing.Color.FromArgb(49, 27, 146);

            this.lblTrangThai.Location = new System.Drawing.Point(10, 155); this.lblTrangThai.Size = new System.Drawing.Size(310, 40);
            this.lblTrangThai.Text = "💡 Gợi ý: admin/123456"; this.lblTrangThai.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Italic); this.lblTrangThai.ForeColor = System.Drawing.Color.FromArgb(100, 100, 150);

            this.grpDangNhap.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblTenDN, this.txtTenDangNhap, this.lblMatKhau, this.txtMatKhau,
                this.btnToggleMatKhau, this.chkHienMatKhau, this.chkGhiNho, this.lblTrangThai });

            // pnlBtns
            this.pnlBtns.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBtns.Height = 55;
            this.pnlBtns.BackColor = System.Drawing.Color.FromArgb(237, 231, 246);

            this.btnDangNhap.Size = new System.Drawing.Size(120, 36); this.btnDangNhap.Location = new System.Drawing.Point(15, 9);
            this.btnDangNhap.Text = "🔓 Đăng nhập"; this.btnDangNhap.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnDangNhap.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnDangNhap.FlatAppearance.BorderSize = 0;
            this.btnDangNhap.BackColor = System.Drawing.Color.FromArgb(49, 27, 146); this.btnDangNhap.ForeColor = System.Drawing.Color.White;
            this.btnDangNhap.Cursor = System.Windows.Forms.Cursors.Hand;

            this.btnDangKy.Size = new System.Drawing.Size(110, 36); this.btnDangKy.Location = new System.Drawing.Point(145, 9);
            this.btnDangKy.Text = "📝 Đăng ký"; this.btnDangKy.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnDangKy.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnDangKy.FlatAppearance.BorderSize = 0;
            this.btnDangKy.BackColor = System.Drawing.Color.FromArgb(56, 142, 60); this.btnDangKy.ForeColor = System.Drawing.Color.White;
            this.btnDangKy.Cursor = System.Windows.Forms.Cursors.Hand;

            this.btnHuy.Size = new System.Drawing.Size(90, 36); this.btnHuy.Location = new System.Drawing.Point(265, 9);
            this.btnHuy.Text = "❌ Hủy"; this.btnHuy.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnHuy.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnHuy.FlatAppearance.BorderSize = 0;
            this.btnHuy.BackColor = System.Drawing.Color.FromArgb(97, 97, 97); this.btnHuy.ForeColor = System.Drawing.Color.White;
            this.btnHuy.Cursor = System.Windows.Forms.Cursors.Hand;

            this.pnlBtns.Controls.AddRange(new System.Windows.Forms.Control[] { this.btnDangNhap, this.btnDangKy, this.btnHuy });

            // frmDangNhap
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(248, 246, 255);
            this.ClientSize = new System.Drawing.Size(354, 390);
            this.Controls.Add(this.grpDangNhap);
            this.Controls.Add(this.lblSubHeader);
            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.pnlBtns);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmDangNhap";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bài 7: Form Đăng Nhập";

            this.Load += new System.EventHandler(this.frmDangNhap_Load);
            this.btnDangNhap.Click += new System.EventHandler(this.btnDangNhap_Click);
            this.btnDangKy.Click += new System.EventHandler(this.btnDangKy_Click);
            this.btnHuy.Click += new System.EventHandler(this.btnHuy_Click);
            this.chkHienMatKhau.CheckedChanged += new System.EventHandler(this.chkHienMatKhau_CheckedChanged);
            this.btnToggleMatKhau.Click += new System.EventHandler(this.btnToggleMatKhau_Click);
            this.txtTenDangNhap.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTenDangNhap_KeyDown);
            this.txtMatKhau.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMatKhau_KeyDown);

            this.grpDangNhap.ResumeLayout(false);
            this.pnlBtns.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Label lblSubHeader;
        private System.Windows.Forms.GroupBox grpDangNhap;
        private System.Windows.Forms.Label lblTenDN;
        private System.Windows.Forms.TextBox txtTenDangNhap;
        private System.Windows.Forms.Label lblMatKhau;
        private System.Windows.Forms.TextBox txtMatKhau;
        private System.Windows.Forms.Button btnToggleMatKhau;
        private System.Windows.Forms.CheckBox chkHienMatKhau;
        private System.Windows.Forms.CheckBox chkGhiNho;
        private System.Windows.Forms.Label lblTrangThai;
        private System.Windows.Forms.Panel pnlBtns;
        private System.Windows.Forms.Button btnDangNhap;
        private System.Windows.Forms.Button btnDangKy;
        private System.Windows.Forms.Button btnHuy;
    }
}
