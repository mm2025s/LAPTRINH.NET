namespace Lab04_WinFormsBasic.Forms
{
    partial class frmDangKyHocPhan
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing) { if (disposing && (components != null)) components.Dispose(); base.Dispose(disposing); }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.lblHeader = new System.Windows.Forms.Label();
            this.grpChonMon = new System.Windows.Forms.GroupBox();
            this.clbMonHoc = new System.Windows.Forms.CheckedListBox();
            this.pnlChonNhanh = new System.Windows.Forms.Panel();
            this.btnChonTatCa = new System.Windows.Forms.Button();
            this.btnBoChon = new System.Windows.Forms.Button();
            this.grpKetQua = new System.Windows.Forms.GroupBox();
            this.lblTongTinChi = new System.Windows.Forms.Label();
            this.lblTrangThai = new System.Windows.Forms.Label();
            this.txtDanhSachChon = new System.Windows.Forms.TextBox();
            this.pnlBtns = new System.Windows.Forms.Panel();
            this.btnXacNhan = new System.Windows.Forms.Button();
            this.btnDong = new System.Windows.Forms.Button();

            this.grpChonMon.SuspendLayout();
            this.pnlChonNhanh.SuspendLayout();
            this.grpKetQua.SuspendLayout();
            this.pnlBtns.SuspendLayout();
            this.SuspendLayout();

            // lblHeader
            this.lblHeader.AutoSize = false; this.lblHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblHeader.BackColor = System.Drawing.Color.FromArgb(230, 74, 25);
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblHeader.ForeColor = System.Drawing.Color.White; this.lblHeader.Height = 50;
            this.lblHeader.Text = "📚 Bài 9: Đăng Ký Học Phần"; this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // grpChonMon
            this.grpChonMon.Location = new System.Drawing.Point(12, 62); this.grpChonMon.Size = new System.Drawing.Size(310, 300);
            this.grpChonMon.Text = "Danh sách học phần"; this.grpChonMon.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpChonMon.ForeColor = System.Drawing.Color.FromArgb(191, 54, 12);

            this.clbMonHoc.Location = new System.Drawing.Point(10, 22); this.clbMonHoc.Size = new System.Drawing.Size(288, 230);
            this.clbMonHoc.Font = new System.Drawing.Font("Segoe UI", 9F); this.clbMonHoc.CheckOnClick = true;
            this.clbMonHoc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.clbMonHoc.BackColor = System.Drawing.Color.FromArgb(255, 252, 248);

            // pnlChonNhanh
            this.pnlChonNhanh.Location = new System.Drawing.Point(10, 260); this.pnlChonNhanh.Size = new System.Drawing.Size(288, 32);

            this.btnChonTatCa.Size = new System.Drawing.Size(135, 28); this.btnChonTatCa.Location = new System.Drawing.Point(0, 2);
            this.btnChonTatCa.Text = "☑ Chọn tất cả"; this.btnChonTatCa.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.btnChonTatCa.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnChonTatCa.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(230,74,25);
            this.btnChonTatCa.BackColor = System.Drawing.Color.FromArgb(255, 235, 220); this.btnChonTatCa.ForeColor = System.Drawing.Color.FromArgb(191,54,12);
            this.btnChonTatCa.Cursor = System.Windows.Forms.Cursors.Hand;

            this.btnBoChon.Size = new System.Drawing.Size(135, 28); this.btnBoChon.Location = new System.Drawing.Point(143, 2);
            this.btnBoChon.Text = "☐ Bỏ chọn tất cả"; this.btnBoChon.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.btnBoChon.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnBoChon.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnBoChon.BackColor = System.Drawing.Color.FromArgb(245, 245, 245); this.btnBoChon.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this.btnBoChon.Cursor = System.Windows.Forms.Cursors.Hand;

            this.pnlChonNhanh.Controls.AddRange(new System.Windows.Forms.Control[] { this.btnChonTatCa, this.btnBoChon });
            this.grpChonMon.Controls.AddRange(new System.Windows.Forms.Control[] { this.clbMonHoc, this.pnlChonNhanh });

            // grpKetQua
            this.grpKetQua.Location = new System.Drawing.Point(334, 62); this.grpKetQua.Size = new System.Drawing.Size(270, 300);
            this.grpKetQua.Text = "Kết quả đăng ký"; this.grpKetQua.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpKetQua.ForeColor = System.Drawing.Color.FromArgb(191, 54, 12);

            this.lblTongTinChi.Location = new System.Drawing.Point(10, 25); this.lblTongTinChi.Size = new System.Drawing.Size(245, 25);
            this.lblTongTinChi.Text = "Tổng số tín chỉ đã chọn: 0 / 24"; this.lblTongTinChi.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblTongTinChi.ForeColor = System.Drawing.Color.Gray;

            this.lblTrangThai.Location = new System.Drawing.Point(10, 55); this.lblTrangThai.Size = new System.Drawing.Size(245, 40);
            this.lblTrangThai.Text = "Chưa chọn môn học nào."; this.lblTrangThai.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Italic);
            this.lblTrangThai.ForeColor = System.Drawing.Color.Gray;

            this.txtDanhSachChon.Location = new System.Drawing.Point(10, 100); this.txtDanhSachChon.Size = new System.Drawing.Size(245, 185);
            this.txtDanhSachChon.Multiline = true; this.txtDanhSachChon.ReadOnly = true; this.txtDanhSachChon.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDanhSachChon.Font = new System.Drawing.Font("Segoe UI", 8.5F); this.txtDanhSachChon.BackColor = System.Drawing.Color.FromArgb(255, 252, 248);
            this.txtDanhSachChon.Text = "(Chưa chọn môn nào)"; this.txtDanhSachChon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.grpKetQua.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblTongTinChi, this.lblTrangThai, this.txtDanhSachChon });

            // pnlBtns
            this.pnlBtns.Dock = System.Windows.Forms.DockStyle.Bottom; this.pnlBtns.Height = 55;
            this.pnlBtns.BackColor = System.Drawing.Color.FromArgb(255, 240, 230);

            this.btnXacNhan.Size = new System.Drawing.Size(200, 36); this.btnXacNhan.Location = new System.Drawing.Point(100, 9);
            this.btnXacNhan.Text = "✅ Xác nhận đăng ký"; this.btnXacNhan.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnXacNhan.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnXacNhan.FlatAppearance.BorderSize = 0;
            this.btnXacNhan.BackColor = System.Drawing.Color.FromArgb(56, 142, 60); this.btnXacNhan.ForeColor = System.Drawing.Color.White;
            this.btnXacNhan.Cursor = System.Windows.Forms.Cursors.Hand;

            this.btnDong.Size = new System.Drawing.Size(120, 36); this.btnDong.Location = new System.Drawing.Point(314, 9);
            this.btnDong.Text = "❌ Đóng"; this.btnDong.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnDong.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnDong.FlatAppearance.BorderSize = 0;
            this.btnDong.BackColor = System.Drawing.Color.FromArgb(97, 97, 97); this.btnDong.ForeColor = System.Drawing.Color.White;
            this.btnDong.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDong.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            this.pnlBtns.Controls.AddRange(new System.Windows.Forms.Control[] { this.btnXacNhan, this.btnDong });

            // frmDangKyHocPhan
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(255, 250, 245);
            this.ClientSize = new System.Drawing.Size(616, 440);
            this.Controls.Add(this.grpChonMon);
            this.Controls.Add(this.grpKetQua);
            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.pnlBtns);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmDangKyHocPhan";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bài 9: Đăng Ký Học Phần";
            this.CancelButton = this.btnDong;

            this.Load += new System.EventHandler(this.frmDangKyHocPhan_Load);
            this.clbMonHoc.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbMonHoc_ItemCheck);
            this.btnXacNhan.Click += new System.EventHandler(this.btnXacNhan_Click);
            this.btnBoChon.Click += new System.EventHandler(this.btnBoChon_Click);
            this.btnChonTatCa.Click += new System.EventHandler(this.btnChonTatCa_Click);

            this.grpChonMon.ResumeLayout(false);
            this.pnlChonNhanh.ResumeLayout(false);
            this.grpKetQua.ResumeLayout(false);
            this.pnlBtns.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.GroupBox grpChonMon;
        private System.Windows.Forms.CheckedListBox clbMonHoc;
        private System.Windows.Forms.Panel pnlChonNhanh;
        private System.Windows.Forms.Button btnChonTatCa;
        private System.Windows.Forms.Button btnBoChon;
        private System.Windows.Forms.GroupBox grpKetQua;
        private System.Windows.Forms.Label lblTongTinChi;
        private System.Windows.Forms.Label lblTrangThai;
        private System.Windows.Forms.TextBox txtDanhSachChon;
        private System.Windows.Forms.Panel pnlBtns;
        private System.Windows.Forms.Button btnXacNhan;
        private System.Windows.Forms.Button btnDong;
    }
}
