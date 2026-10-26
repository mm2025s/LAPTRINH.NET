namespace Lab04_WinFormsBasic.Forms
{
    partial class frmBMI
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing) { if (disposing && (components != null)) components.Dispose(); base.Dispose(disposing); }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.lblHeader = new System.Windows.Forms.Label();
            this.grpNhap = new System.Windows.Forms.GroupBox();
            this.lblChieuCao = new System.Windows.Forms.Label();
            this.txtChieuCao = new System.Windows.Forms.TextBox();
            this.lblDVCaoCao = new System.Windows.Forms.Label();
            this.lblCanNang = new System.Windows.Forms.Label();
            this.txtCanNang = new System.Windows.Forms.TextBox();
            this.lblDVCanNang = new System.Windows.Forms.Label();
            this.grpKetQua = new System.Windows.Forms.GroupBox();
            this.lblBMILabel = new System.Windows.Forms.Label();
            this.txtBMI = new System.Windows.Forms.TextBox();
            this.lblPhanLoai = new System.Windows.Forms.Label();
            this.progressBMI = new System.Windows.Forms.ProgressBar();
            this.lblPhanLoaiBang = new System.Windows.Forms.Label();
            this.lblNhanXet = new System.Windows.Forms.Label();
            this.pnlBtns = new System.Windows.Forms.Panel();
            this.btnTinhBMI = new System.Windows.Forms.Button();
            this.btnLamMoi = new System.Windows.Forms.Button();
            this.btnDong = new System.Windows.Forms.Button();

            this.grpNhap.SuspendLayout();
            this.grpKetQua.SuspendLayout();
            this.pnlBtns.SuspendLayout();
            this.SuspendLayout();

            // lblHeader
            this.lblHeader.AutoSize = false; this.lblHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblHeader.BackColor = System.Drawing.Color.FromArgb(233, 30, 99);
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblHeader.ForeColor = System.Drawing.Color.White; this.lblHeader.Height = 50;
            this.lblHeader.Text = "âš–ï¸ BÃ i 8: TÃ­nh Chá»‰ Sá»‘ BMI"; this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // grpNhap
            this.grpNhap.Location = new System.Drawing.Point(12, 62); this.grpNhap.Size = new System.Drawing.Size(320, 100);
            this.grpNhap.Text = "Nháº­p thÃ´ng sá»‘"; this.grpNhap.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpNhap.ForeColor = System.Drawing.Color.FromArgb(136, 14, 79);

            this.lblChieuCao.Location = new System.Drawing.Point(10, 28); this.lblChieuCao.Size = new System.Drawing.Size(90, 23); this.lblChieuCao.Text = "Chiá»u cao:"; this.lblChieuCao.Font = new System.Drawing.Font("Segoe UI", 9F); this.lblChieuCao.ForeColor = System.Drawing.Color.FromArgb(50,50,50);
            this.txtChieuCao.Location = new System.Drawing.Point(105, 25); this.txtChieuCao.Size = new System.Drawing.Size(140, 23); this.txtChieuCao.Font = new System.Drawing.Font("Segoe UI", 9F); Lab04_WinFormsBasic.Helpers.TextBoxExtensions.SetPlaceholderText(this.txtChieuCao, "VD: 170");
            this.lblDVCaoCao.Location = new System.Drawing.Point(252, 28); this.lblDVCaoCao.Size = new System.Drawing.Size(50, 23); this.lblDVCaoCao.Text = "cm"; this.lblDVCaoCao.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold); this.lblDVCaoCao.ForeColor = System.Drawing.Color.Gray;

            this.lblCanNang.Location = new System.Drawing.Point(10, 62); this.lblCanNang.Size = new System.Drawing.Size(90, 23); this.lblCanNang.Text = "CÃ¢n náº·ng:"; this.lblCanNang.Font = new System.Drawing.Font("Segoe UI", 9F); this.lblCanNang.ForeColor = System.Drawing.Color.FromArgb(50,50,50);
            this.txtCanNang.Location = new System.Drawing.Point(105, 59); this.txtCanNang.Size = new System.Drawing.Size(140, 23); this.txtCanNang.Font = new System.Drawing.Font("Segoe UI", 9F); Lab04_WinFormsBasic.Helpers.TextBoxExtensions.SetPlaceholderText(this.txtCanNang, "VD: 60");
            this.lblDVCanNang.Location = new System.Drawing.Point(252, 62); this.lblDVCanNang.Size = new System.Drawing.Size(50, 23); this.lblDVCanNang.Text = "kg"; this.lblDVCanNang.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold); this.lblDVCanNang.ForeColor = System.Drawing.Color.Gray;

            this.grpNhap.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblChieuCao, this.txtChieuCao, this.lblDVCaoCao,
                this.lblCanNang, this.txtCanNang, this.lblDVCanNang });

            // grpKetQua
            this.grpKetQua.Location = new System.Drawing.Point(12, 175); this.grpKetQua.Size = new System.Drawing.Size(320, 190);
            this.grpKetQua.Text = "Káº¿t quáº£"; this.grpKetQua.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpKetQua.ForeColor = System.Drawing.Color.FromArgb(136, 14, 79);

            this.lblBMILabel.Location = new System.Drawing.Point(10, 28); this.lblBMILabel.Size = new System.Drawing.Size(80, 35); this.lblBMILabel.Text = "Chá»‰ sá»‘ BMI:"; this.lblBMILabel.Font = new System.Drawing.Font("Segoe UI", 9F); this.lblBMILabel.ForeColor = System.Drawing.Color.FromArgb(50,50,50);
            this.txtBMI.Location = new System.Drawing.Point(100, 22); this.txtBMI.Size = new System.Drawing.Size(200, 38);
            this.txtBMI.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.txtBMI.ReadOnly = true; this.txtBMI.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtBMI.BackColor = System.Drawing.Color.FromArgb(252, 228, 236); this.txtBMI.ForeColor = System.Drawing.Color.FromArgb(136, 14, 79);

            this.lblPhanLoai.Location = new System.Drawing.Point(10, 72); this.lblPhanLoai.Size = new System.Drawing.Size(300, 28);
            this.lblPhanLoai.Text = "PhÃ¢n loáº¡i: ---"; this.lblPhanLoai.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblPhanLoai.ForeColor = System.Drawing.Color.Gray;

            this.progressBMI.Location = new System.Drawing.Point(10, 105); this.progressBMI.Size = new System.Drawing.Size(300, 18);
            this.progressBMI.Minimum = 10; this.progressBMI.Maximum = 40; this.progressBMI.Value = 10;
            this.progressBMI.Style = System.Windows.Forms.ProgressBarStyle.Continuous;

            this.lblPhanLoaiBang.Location = new System.Drawing.Point(10, 128); this.lblPhanLoaiBang.Size = new System.Drawing.Size(300, 18);
            this.lblPhanLoaiBang.Text = "â—€ Gáº§y(18.5) | BÃ¬nh thÆ°á»ng(25) | Thá»«a cÃ¢n(30) | BÃ©o phÃ¬ â–¶";
            this.lblPhanLoaiBang.Font = new System.Drawing.Font("Segoe UI", 7F); this.lblPhanLoaiBang.ForeColor = System.Drawing.Color.DimGray;

            this.lblNhanXet.Location = new System.Drawing.Point(10, 152); this.lblNhanXet.Size = new System.Drawing.Size(300, 32);
            this.lblNhanXet.Text = "Nháº­p chiá»u cao vÃ  cÃ¢n náº·ng, sau Ä‘Ã³ nháº¥n TÃ­nh BMI.";
            this.lblNhanXet.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Italic);
            this.lblNhanXet.ForeColor = System.Drawing.Color.Gray;

            this.grpKetQua.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblBMILabel, this.txtBMI, this.lblPhanLoai, this.progressBMI,
                this.lblPhanLoaiBang, this.lblNhanXet });

            // pnlBtns
            this.pnlBtns.Dock = System.Windows.Forms.DockStyle.Bottom; this.pnlBtns.Height = 55;
            this.pnlBtns.BackColor = System.Drawing.Color.FromArgb(252, 228, 236);

            this.btnTinhBMI.Size = new System.Drawing.Size(120, 36); this.btnTinhBMI.Location = new System.Drawing.Point(15, 9);
            this.btnTinhBMI.Text = "âš–ï¸ TÃ­nh BMI"; this.btnTinhBMI.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnTinhBMI.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnTinhBMI.FlatAppearance.BorderSize = 0;
            this.btnTinhBMI.BackColor = System.Drawing.Color.FromArgb(233, 30, 99); this.btnTinhBMI.ForeColor = System.Drawing.Color.White;
            this.btnTinhBMI.Cursor = System.Windows.Forms.Cursors.Hand;

            this.btnLamMoi.Size = new System.Drawing.Size(100, 36); this.btnLamMoi.Location = new System.Drawing.Point(145, 9);
            this.btnLamMoi.Text = "ðŸ”„ LÃ m má»›i"; this.btnLamMoi.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnLamMoi.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnLamMoi.FlatAppearance.BorderSize = 0;
            this.btnLamMoi.BackColor = System.Drawing.Color.FromArgb(245, 124, 0); this.btnLamMoi.ForeColor = System.Drawing.Color.White;
            this.btnLamMoi.Cursor = System.Windows.Forms.Cursors.Hand;

            this.btnDong.Size = new System.Drawing.Size(90, 36); this.btnDong.Location = new System.Drawing.Point(255, 9);
            this.btnDong.Text = "âŒ ÄÃ³ng"; this.btnDong.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnDong.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnDong.FlatAppearance.BorderSize = 0;
            this.btnDong.BackColor = System.Drawing.Color.FromArgb(97, 97, 97); this.btnDong.ForeColor = System.Drawing.Color.White;
            this.btnDong.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDong.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            this.pnlBtns.Controls.AddRange(new System.Windows.Forms.Control[] { this.btnTinhBMI, this.btnLamMoi, this.btnDong });

            // frmBMI
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(255, 248, 251);
            this.ClientSize = new System.Drawing.Size(344, 435);
            this.Controls.Add(this.grpNhap);
            this.Controls.Add(this.grpKetQua);
            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.pnlBtns);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmBMI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BÃ i 8: TÃ­nh Chá»‰ Sá»‘ BMI";
            this.CancelButton = this.btnDong;

            this.btnTinhBMI.Click += new System.EventHandler(this.btnTinhBMI_Click);
            this.btnLamMoi.Click += new System.EventHandler(this.btnLamMoi_Click);

            this.grpNhap.ResumeLayout(false);
            this.grpKetQua.ResumeLayout(false);
            this.pnlBtns.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.GroupBox grpNhap;
        private System.Windows.Forms.Label lblChieuCao;
        private System.Windows.Forms.TextBox txtChieuCao;
        private System.Windows.Forms.Label lblDVCaoCao;
        private System.Windows.Forms.Label lblCanNang;
        private System.Windows.Forms.TextBox txtCanNang;
        private System.Windows.Forms.Label lblDVCanNang;
        private System.Windows.Forms.GroupBox grpKetQua;
        private System.Windows.Forms.Label lblBMILabel;
        private System.Windows.Forms.TextBox txtBMI;
        private System.Windows.Forms.Label lblPhanLoai;
        private System.Windows.Forms.ProgressBar progressBMI;
        private System.Windows.Forms.Label lblPhanLoaiBang;
        private System.Windows.Forms.Label lblNhanXet;
        private System.Windows.Forms.Panel pnlBtns;
        private System.Windows.Forms.Button btnTinhBMI;
        private System.Windows.Forms.Button btnLamMoi;
        private System.Windows.Forms.Button btnDong;
    }
}
