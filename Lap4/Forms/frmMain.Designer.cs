namespace Lab04_WinFormsBasic.Forms
{
    partial class frmMain
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
            this.lblTieuDe = new System.Windows.Forms.Label();
            this.lblMoTa = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSinhVien = new System.Windows.Forms.Button();
            this.btnMayTinh = new System.Windows.Forms.Button();
            this.btnDangNhap = new System.Windows.Forms.Button();
            this.btnBMI = new System.Windows.Forms.Button();
            this.btnDangKyHocPhan = new System.Windows.Forms.Button();
            this.btnSanPham = new System.Windows.Forms.Button();
            this.btnThoat = new System.Windows.Forms.Button();
            this.lblFooter = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();

            // lblTieuDe
            this.lblTieuDe.AutoSize = false;
            this.lblTieuDe.BackColor = System.Drawing.Color.FromArgb(30, 30, 60);
            this.lblTieuDe.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTieuDe.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTieuDe.ForeColor = System.Drawing.Color.White;
            this.lblTieuDe.Height = 70;
            this.lblTieuDe.Text = "LAB 4 - WINFORMS CƠ BẢN";
            this.lblTieuDe.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // lblMoTa
            this.lblMoTa.AutoSize = false;
            this.lblMoTa.BackColor = System.Drawing.Color.FromArgb(45, 45, 80);
            this.lblMoTa.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblMoTa.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblMoTa.ForeColor = System.Drawing.Color.FromArgb(200, 200, 230);
            this.lblMoTa.Height = 35;
            this.lblMoTa.Text = "IT3212 - Lập trình .NET | Khoa CNTT - ĐH Công nghệ Đông Á";
            this.lblMoTa.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // panel1 - Button grid
            this.panel1.BackColor = System.Drawing.Color.FromArgb(240, 242, 250);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Padding = new System.Windows.Forms.Padding(30, 20, 30, 20);

            // Helper: create styled button
            System.Action<System.Windows.Forms.Button, string, string, int, int> setBtn =
                (btn, text, tag, row, col) =>
                {
                    btn.Text = text;
                    btn.Tag = tag;
                    btn.Size = new System.Drawing.Size(200, 60);
                    btn.Location = new System.Drawing.Point(30 + col * 220, 20 + row * 80);
                    btn.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
                    btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.BackColor = System.Drawing.Color.FromArgb(63, 81, 181);
                    btn.ForeColor = System.Drawing.Color.White;
                    btn.Cursor = System.Windows.Forms.Cursors.Hand;
                };

            setBtn(this.btnSinhVien, "📋 Bài 1-5: Thông tin Sinh Viên", "sv", 0, 0);
            setBtn(this.btnMayTinh, "🔢 Bài 6: Máy Tính 4 Phép Toán", "mt", 0, 1);
            setBtn(this.btnDangNhap, "🔒 Bài 7: Form Đăng Nhập", "dn", 1, 0);
            setBtn(this.btnBMI, "⚖️ Bài 8: Tính Chỉ Số BMI", "bmi", 1, 1);
            setBtn(this.btnDangKyHocPhan, "📚 Bài 9: Đăng Ký Học Phần", "hp", 2, 0);
            setBtn(this.btnSanPham, "📦 Bài 10: Phiếu Nhập Sản Phẩm", "sp", 2, 1);

            this.btnMayTinh.BackColor = System.Drawing.Color.FromArgb(0, 150, 136);
            this.btnDangNhap.BackColor = System.Drawing.Color.FromArgb(156, 39, 176);
            this.btnBMI.BackColor = System.Drawing.Color.FromArgb(233, 30, 99);
            this.btnDangKyHocPhan.BackColor = System.Drawing.Color.FromArgb(255, 87, 34);
            this.btnSanPham.BackColor = System.Drawing.Color.FromArgb(76, 175, 80);

            this.btnThoat.Text = "❌  Thoát";
            this.btnThoat.Size = new System.Drawing.Size(200, 45);
            this.btnThoat.Location = new System.Drawing.Point(110, 270);
            this.btnThoat.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnThoat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnThoat.FlatAppearance.BorderSize = 0;
            this.btnThoat.BackColor = System.Drawing.Color.FromArgb(120, 120, 130);
            this.btnThoat.ForeColor = System.Drawing.Color.White;
            this.btnThoat.Cursor = System.Windows.Forms.Cursors.Hand;

            this.panel1.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.btnSinhVien, this.btnMayTinh, this.btnDangNhap,
                this.btnBMI, this.btnDangKyHocPhan, this.btnSanPham, this.btnThoat });

            // lblFooter
            this.lblFooter.AutoSize = false;
            this.lblFooter.BackColor = System.Drawing.Color.FromArgb(30, 30, 60);
            this.lblFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblFooter.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblFooter.ForeColor = System.Drawing.Color.FromArgb(150, 150, 180);
            this.lblFooter.Height = 30;
            this.lblFooter.Text = "© 2026 Lab 4 WinForms Basic | Visual Studio 2022 | .NET Framework 4.8";
            this.lblFooter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // frmMain
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(240, 242, 250);
            this.ClientSize = new System.Drawing.Size(480, 460);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblMoTa);
            this.Controls.Add(this.lblTieuDe);
            this.Controls.Add(this.lblFooter);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Lab 4 - WinForms Cơ Bản";

            // Wire events
            this.btnSinhVien.Click += new System.EventHandler(this.btnSinhVien_Click);
            this.btnMayTinh.Click += new System.EventHandler(this.btnMayTinh_Click);
            this.btnDangNhap.Click += new System.EventHandler(this.btnDangNhap_Click);
            this.btnBMI.Click += new System.EventHandler(this.btnBMI_Click);
            this.btnDangKyHocPhan.Click += new System.EventHandler(this.btnDangKyHocPhan_Click);
            this.btnSanPham.Click += new System.EventHandler(this.btnSanPham_Click);
            this.btnThoat.Click += new System.EventHandler(this.btnThoat_Click);

            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

        private System.Windows.Forms.Label lblTieuDe;
        private System.Windows.Forms.Label lblMoTa;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnSinhVien;
        private System.Windows.Forms.Button btnMayTinh;
        private System.Windows.Forms.Button btnDangNhap;
        private System.Windows.Forms.Button btnBMI;
        private System.Windows.Forms.Button btnDangKyHocPhan;
        private System.Windows.Forms.Button btnSanPham;
        private System.Windows.Forms.Button btnThoat;
        private System.Windows.Forms.Label lblFooter;
    }
}
