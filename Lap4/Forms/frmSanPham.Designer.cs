namespace Lab04_WinFormsBasic.Forms
{
    partial class frmSanPham
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing) { if (disposing && (components != null)) components.Dispose(); base.Dispose(disposing); }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.lblHeader = new System.Windows.Forms.Label();
            this.grpThongTin = new System.Windows.Forms.GroupBox();
            this.lblMaSP = new System.Windows.Forms.Label(); this.txtMaSP = new System.Windows.Forms.TextBox();
            this.lblTenSP = new System.Windows.Forms.Label(); this.txtTenSP = new System.Windows.Forms.TextBox();
            this.lblLoai = new System.Windows.Forms.Label(); this.cboLoai = new System.Windows.Forms.ComboBox();
            this.lblSoLuong = new System.Windows.Forms.Label(); this.txtSoLuong = new System.Windows.Forms.TextBox();
            this.lblDonGia = new System.Windows.Forms.Label(); this.txtDonGia = new System.Windows.Forms.TextBox(); this.lblDVDonGia = new System.Windows.Forms.Label();
            this.lblThanhTien = new System.Windows.Forms.Label(); this.txtThanhTien = new System.Windows.Forms.TextBox(); this.lblDVThanhTien = new System.Windows.Forms.Label();
            this.lblNgayNhap = new System.Windows.Forms.Label(); this.dtpNgayNhap = new System.Windows.Forms.DateTimePicker();
            this.lblMoTa = new System.Windows.Forms.Label(); this.txtMoTa = new System.Windows.Forms.TextBox();
            this.grpAnh = new System.Windows.Forms.GroupBox();
            this.picSanPham = new System.Windows.Forms.PictureBox();
            this.txtDuongDanAnh = new System.Windows.Forms.TextBox();
            this.btnChonAnh = new System.Windows.Forms.Button();
            this.pnlBtns = new System.Windows.Forms.Panel();
            this.btnLuu = new System.Windows.Forms.Button();
            this.btnLamMoi = new System.Windows.Forms.Button();
            this.btnDong = new System.Windows.Forms.Button();

            this.grpThongTin.SuspendLayout();
            this.grpAnh.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picSanPham)).BeginInit();
            this.pnlBtns.SuspendLayout();
            this.SuspendLayout();

            // lblHeader
            this.lblHeader.AutoSize = false; this.lblHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblHeader.BackColor = System.Drawing.Color.FromArgb(27, 94, 32);
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblHeader.ForeColor = System.Drawing.Color.White; this.lblHeader.Height = 50;
            this.lblHeader.Text = "ðŸ“¦ BÃ i 10: Phiáº¿u Nháº­p Sáº£n Pháº©m"; this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // grpThongTin
            this.grpThongTin.Location = new System.Drawing.Point(12, 62); this.grpThongTin.Size = new System.Drawing.Size(400, 350);
            this.grpThongTin.Text = "ThÃ´ng tin sáº£n pháº©m"; this.grpThongTin.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpThongTin.ForeColor = System.Drawing.Color.FromArgb(27, 94, 32);

            System.Action<System.Windows.Forms.Label, System.Windows.Forms.Control, string, int> addRow =
                (lbl, ctrl, text, y) => {
                    lbl.Location = new System.Drawing.Point(10, y + 4); lbl.Size = new System.Drawing.Size(90, 23);
                    lbl.Text = text; lbl.Font = new System.Drawing.Font("Segoe UI", 9F); lbl.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
                    ctrl.Location = new System.Drawing.Point(110, y); ctrl.Size = new System.Drawing.Size(275, 23);
                    if (ctrl is System.Windows.Forms.TextBox tb) tb.Font = new System.Drawing.Font("Segoe UI", 9F);
                    else if (ctrl is System.Windows.Forms.ComboBox cb) cb.Font = new System.Drawing.Font("Segoe UI", 9F);
                    else if (ctrl is System.Windows.Forms.DateTimePicker dtp) dtp.Font = new System.Drawing.Font("Segoe UI", 9F);
                };

            addRow(this.lblMaSP, this.txtMaSP, "MÃ£ SP (*):", 25); Lab04_WinFormsBasic.Helpers.TextBoxExtensions.SetPlaceholderText(this.txtMaSP, "VD: SP001");
            addRow(this.lblTenSP, this.txtTenSP, "TÃªn SP (*):", 58); Lab04_WinFormsBasic.Helpers.TextBoxExtensions.SetPlaceholderText(this.txtTenSP, "VD: Laptop Dell XPS 15");
            addRow(this.lblLoai, this.cboLoai, "Loáº¡i SP:", 91);

            // Sá»‘ lÆ°á»£ng (short)
            this.lblSoLuong.Location = new System.Drawing.Point(10, 128); this.lblSoLuong.Size = new System.Drawing.Size(90, 23); this.lblSoLuong.Text = "Sá»‘ lÆ°á»£ng (*):"; this.lblSoLuong.Font = new System.Drawing.Font("Segoe UI", 9F); this.lblSoLuong.ForeColor = System.Drawing.Color.FromArgb(50,50,50);
            this.txtSoLuong.Location = new System.Drawing.Point(110, 125); this.txtSoLuong.Size = new System.Drawing.Size(100, 23); this.txtSoLuong.Font = new System.Drawing.Font("Segoe UI", 9F); this.txtSoLuong.Text = "1";

            // ÄÆ¡n giÃ¡
            this.lblDonGia.Location = new System.Drawing.Point(10, 162); this.lblDonGia.Size = new System.Drawing.Size(90, 23); this.lblDonGia.Text = "ÄÆ¡n giÃ¡ (*):"; this.lblDonGia.Font = new System.Drawing.Font("Segoe UI", 9F); this.lblDonGia.ForeColor = System.Drawing.Color.FromArgb(50,50,50);
            this.txtDonGia.Location = new System.Drawing.Point(110, 159); this.txtDonGia.Size = new System.Drawing.Size(200, 23); this.txtDonGia.Font = new System.Drawing.Font("Segoe UI", 9F); Lab04_WinFormsBasic.Helpers.TextBoxExtensions.SetPlaceholderText(this.txtDonGia, "VD: 25000000");
            this.lblDVDonGia.Location = new System.Drawing.Point(318, 162); this.lblDVDonGia.Size = new System.Drawing.Size(60, 23); this.lblDVDonGia.Text = "VNÄ"; this.lblDVDonGia.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold); this.lblDVDonGia.ForeColor = System.Drawing.Color.Gray;

            // ThÃ nh tiá»n (readonly)
            this.lblThanhTien.Location = new System.Drawing.Point(10, 196); this.lblThanhTien.Size = new System.Drawing.Size(90, 23); this.lblThanhTien.Text = "ThÃ nh tiá»n:"; this.lblThanhTien.Font = new System.Drawing.Font("Segoe UI", 9F); this.lblThanhTien.ForeColor = System.Drawing.Color.FromArgb(50,50,50);
            this.txtThanhTien.Location = new System.Drawing.Point(110, 193); this.txtThanhTien.Size = new System.Drawing.Size(200, 26);
            this.txtThanhTien.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.txtThanhTien.ReadOnly = true; this.txtThanhTien.Text = "0";
            this.txtThanhTien.BackColor = System.Drawing.Color.FromArgb(232, 245, 233); this.txtThanhTien.ForeColor = System.Drawing.Color.FromArgb(27, 94, 32);
            this.lblDVThanhTien.Location = new System.Drawing.Point(318, 196); this.lblDVThanhTien.Size = new System.Drawing.Size(60, 23); this.lblDVThanhTien.Text = "VNÄ"; this.lblDVThanhTien.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold); this.lblDVThanhTien.ForeColor = System.Drawing.Color.FromArgb(27,94,32);

            // NgÃ y nháº­p
            this.lblNgayNhap.Location = new System.Drawing.Point(10, 232); this.lblNgayNhap.Size = new System.Drawing.Size(90, 23); this.lblNgayNhap.Text = "NgÃ y nháº­p:"; this.lblNgayNhap.Font = new System.Drawing.Font("Segoe UI", 9F); this.lblNgayNhap.ForeColor = System.Drawing.Color.FromArgb(50,50,50);
            this.dtpNgayNhap.Location = new System.Drawing.Point(110, 229); this.dtpNgayNhap.Size = new System.Drawing.Size(180, 23); this.dtpNgayNhap.Format = System.Windows.Forms.DateTimePickerFormat.Short; this.dtpNgayNhap.Font = new System.Drawing.Font("Segoe UI", 9F);

            // MÃ´ táº£
            this.lblMoTa.Location = new System.Drawing.Point(10, 266); this.lblMoTa.Size = new System.Drawing.Size(90, 23); this.lblMoTa.Text = "MÃ´ táº£:"; this.lblMoTa.Font = new System.Drawing.Font("Segoe UI", 9F); this.lblMoTa.ForeColor = System.Drawing.Color.FromArgb(50,50,50);
            this.txtMoTa.Location = new System.Drawing.Point(110, 263); this.txtMoTa.Size = new System.Drawing.Size(275, 75);
            this.txtMoTa.Multiline = true; this.txtMoTa.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMoTa.Font = new System.Drawing.Font("Segoe UI", 9F); Lab04_WinFormsBasic.Helpers.TextBoxExtensions.SetPlaceholderText(this.txtMoTa, "MÃ´ táº£ sáº£n pháº©m (tÃ¹y chá»n)...");

            this.grpThongTin.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblMaSP, this.txtMaSP, this.lblTenSP, this.txtTenSP,
                this.lblLoai, this.cboLoai, this.lblSoLuong, this.txtSoLuong,
                this.lblDonGia, this.txtDonGia, this.lblDVDonGia,
                this.lblThanhTien, this.txtThanhTien, this.lblDVThanhTien,
                this.lblNgayNhap, this.dtpNgayNhap, this.lblMoTa, this.txtMoTa });

            // grpAnh
            this.grpAnh.Location = new System.Drawing.Point(424, 62); this.grpAnh.Size = new System.Drawing.Size(200, 350);
            this.grpAnh.Text = "áº¢nh sáº£n pháº©m"; this.grpAnh.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpAnh.ForeColor = System.Drawing.Color.FromArgb(27, 94, 32);

            this.picSanPham.Location = new System.Drawing.Point(10, 25); this.picSanPham.Size = new System.Drawing.Size(180, 220);
            this.picSanPham.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picSanPham.BackColor = System.Drawing.Color.FromArgb(230, 245, 230);
            this.picSanPham.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;

            this.txtDuongDanAnh.Location = new System.Drawing.Point(10, 255); this.txtDuongDanAnh.Size = new System.Drawing.Size(180, 23);
            this.txtDuongDanAnh.Font = new System.Drawing.Font("Segoe UI", 8F); this.txtDuongDanAnh.ReadOnly = true;
            Lab04_WinFormsBasic.Helpers.TextBoxExtensions.SetPlaceholderText(this.txtDuongDanAnh, "ChÆ°a chá»n áº£nh...");

            this.btnChonAnh.Location = new System.Drawing.Point(10, 285); this.btnChonAnh.Size = new System.Drawing.Size(180, 35);
            this.btnChonAnh.Text = "ðŸ“‚ Chá»n áº£nh sáº£n pháº©m"; this.btnChonAnh.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnChonAnh.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnChonAnh.FlatAppearance.BorderSize = 0;
            this.btnChonAnh.BackColor = System.Drawing.Color.FromArgb(27, 94, 32); this.btnChonAnh.ForeColor = System.Drawing.Color.White;
            this.btnChonAnh.Cursor = System.Windows.Forms.Cursors.Hand;

            this.grpAnh.Controls.AddRange(new System.Windows.Forms.Control[] { this.picSanPham, this.txtDuongDanAnh, this.btnChonAnh });

            // pnlBtns
            this.pnlBtns.Dock = System.Windows.Forms.DockStyle.Bottom; this.pnlBtns.Height = 55;
            this.pnlBtns.BackColor = System.Drawing.Color.FromArgb(225, 245, 225);

            this.btnLuu.Size = new System.Drawing.Size(150, 36); this.btnLuu.Location = new System.Drawing.Point(80, 9);
            this.btnLuu.Text = "ðŸ’¾ LÆ°u phiáº¿u nháº­p"; this.btnLuu.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnLuu.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnLuu.FlatAppearance.BorderSize = 0;
            this.btnLuu.BackColor = System.Drawing.Color.FromArgb(27, 94, 32); this.btnLuu.ForeColor = System.Drawing.Color.White;
            this.btnLuu.Cursor = System.Windows.Forms.Cursors.Hand;

            this.btnLamMoi.Size = new System.Drawing.Size(120, 36); this.btnLamMoi.Location = new System.Drawing.Point(245, 9);
            this.btnLamMoi.Text = "ðŸ”„ LÃ m má»›i"; this.btnLamMoi.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnLamMoi.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnLamMoi.FlatAppearance.BorderSize = 0;
            this.btnLamMoi.BackColor = System.Drawing.Color.FromArgb(245, 124, 0); this.btnLamMoi.ForeColor = System.Drawing.Color.White;
            this.btnLamMoi.Cursor = System.Windows.Forms.Cursors.Hand;

            this.btnDong.Size = new System.Drawing.Size(100, 36); this.btnDong.Location = new System.Drawing.Point(380, 9);
            this.btnDong.Text = "âŒ ÄÃ³ng"; this.btnDong.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnDong.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnDong.FlatAppearance.BorderSize = 0;
            this.btnDong.BackColor = System.Drawing.Color.FromArgb(97, 97, 97); this.btnDong.ForeColor = System.Drawing.Color.White;
            this.btnDong.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDong.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            this.pnlBtns.Controls.AddRange(new System.Windows.Forms.Control[] { this.btnLuu, this.btnLamMoi, this.btnDong });

            // frmSanPham
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(245, 252, 245);
            this.ClientSize = new System.Drawing.Size(636, 480);
            this.Controls.Add(this.grpThongTin);
            this.Controls.Add(this.grpAnh);
            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.pnlBtns);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmSanPham";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BÃ i 10: Phiáº¿u Nháº­p Sáº£n Pháº©m";
            this.CancelButton = this.btnDong;

            this.Load += new System.EventHandler(this.frmSanPham_Load);
            this.btnLuu.Click += new System.EventHandler(this.btnLuu_Click);
            this.btnLamMoi.Click += new System.EventHandler(this.btnLamMoi_Click);
            this.btnChonAnh.Click += new System.EventHandler(this.btnChonAnh_Click);
            this.txtSoLuong.TextChanged += new System.EventHandler(this.txtSoLuong_TextChanged);
            this.txtDonGia.TextChanged += new System.EventHandler(this.txtDonGia_TextChanged);

            this.grpThongTin.ResumeLayout(false);
            this.grpAnh.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picSanPham)).EndInit();
            this.pnlBtns.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.GroupBox grpThongTin;
        private System.Windows.Forms.Label lblMaSP; private System.Windows.Forms.TextBox txtMaSP;
        private System.Windows.Forms.Label lblTenSP; private System.Windows.Forms.TextBox txtTenSP;
        private System.Windows.Forms.Label lblLoai; private System.Windows.Forms.ComboBox cboLoai;
        private System.Windows.Forms.Label lblSoLuong; private System.Windows.Forms.TextBox txtSoLuong;
        private System.Windows.Forms.Label lblDonGia; private System.Windows.Forms.TextBox txtDonGia; private System.Windows.Forms.Label lblDVDonGia;
        private System.Windows.Forms.Label lblThanhTien; private System.Windows.Forms.TextBox txtThanhTien; private System.Windows.Forms.Label lblDVThanhTien;
        private System.Windows.Forms.Label lblNgayNhap; private System.Windows.Forms.DateTimePicker dtpNgayNhap;
        private System.Windows.Forms.Label lblMoTa; private System.Windows.Forms.TextBox txtMoTa;
        private System.Windows.Forms.GroupBox grpAnh;
        private System.Windows.Forms.PictureBox picSanPham;
        private System.Windows.Forms.TextBox txtDuongDanAnh;
        private System.Windows.Forms.Button btnChonAnh;
        private System.Windows.Forms.Panel pnlBtns;
        private System.Windows.Forms.Button btnLuu;
        private System.Windows.Forms.Button btnLamMoi;
        private System.Windows.Forms.Button btnDong;
    }
}
