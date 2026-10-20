namespace Lab04_WinFormsBasic.Forms
{
    partial class frmMayTinh
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
            this.lblHeader = new System.Windows.Forms.Label();
            this.grpNhap = new System.Windows.Forms.GroupBox();
            this.lblSo1 = new System.Windows.Forms.Label();
            this.txtSo1 = new System.Windows.Forms.TextBox();
            this.lblSo2 = new System.Windows.Forms.Label();
            this.txtSo2 = new System.Windows.Forms.TextBox();
            this.grpPhepTinh = new System.Windows.Forms.GroupBox();
            this.btnCong = new System.Windows.Forms.Button();
            this.btnTru = new System.Windows.Forms.Button();
            this.btnNhan = new System.Windows.Forms.Button();
            this.btnChia = new System.Windows.Forms.Button();
            this.grpKetQua = new System.Windows.Forms.GroupBox();
            this.lblBieuThuc = new System.Windows.Forms.Label();
            this.txtKetQua = new System.Windows.Forms.TextBox();
            this.pnlBtns = new System.Windows.Forms.Panel();
            this.btnLamMoi = new System.Windows.Forms.Button();
            this.btnThoat = new System.Windows.Forms.Button();

            this.grpNhap.SuspendLayout();
            this.grpPhepTinh.SuspendLayout();
            this.grpKetQua.SuspendLayout();
            this.pnlBtns.SuspendLayout();
            this.SuspendLayout();

            // lblHeader
            this.lblHeader.AutoSize = false;
            this.lblHeader.BackColor = System.Drawing.Color.FromArgb(0, 150, 136);
            this.lblHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblHeader.ForeColor = System.Drawing.Color.White;
            this.lblHeader.Height = 50;
            this.lblHeader.Text = "🔢 Bài 6: Máy Tính 4 Phép Toán";
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // grpNhap
            this.grpNhap.Location = new System.Drawing.Point(12, 62);
            this.grpNhap.Size = new System.Drawing.Size(340, 100);
            this.grpNhap.Text = "Nhập số";
            this.grpNhap.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpNhap.ForeColor = System.Drawing.Color.FromArgb(0, 100, 90);

            this.lblSo1.Location = new System.Drawing.Point(10, 28); this.lblSo1.Size = new System.Drawing.Size(80, 23); this.lblSo1.Text = "Số thứ nhất:"; this.lblSo1.Font = new System.Drawing.Font("Segoe UI", 9F); this.lblSo1.ForeColor = System.Drawing.Color.FromArgb(50,50,50);
            this.txtSo1.Location = new System.Drawing.Point(100, 25); this.txtSo1.Size = new System.Drawing.Size(220, 23); this.txtSo1.Font = new System.Drawing.Font("Segoe UI", 9F); this.txtSo1.PlaceholderText = "Nhập số 1...";
            this.lblSo2.Location = new System.Drawing.Point(10, 62); this.lblSo2.Size = new System.Drawing.Size(80, 23); this.lblSo2.Text = "Số thứ hai:"; this.lblSo2.Font = new System.Drawing.Font("Segoe UI", 9F); this.lblSo2.ForeColor = System.Drawing.Color.FromArgb(50,50,50);
            this.txtSo2.Location = new System.Drawing.Point(100, 59); this.txtSo2.Size = new System.Drawing.Size(220, 23); this.txtSo2.Font = new System.Drawing.Font("Segoe UI", 9F); this.txtSo2.PlaceholderText = "Nhập số 2...";

            this.grpNhap.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblSo1, this.txtSo1, this.lblSo2, this.txtSo2 });

            // grpPhepTinh
            this.grpPhepTinh.Location = new System.Drawing.Point(12, 175);
            this.grpPhepTinh.Size = new System.Drawing.Size(340, 70);
            this.grpPhepTinh.Text = "Chọn phép tính";
            this.grpPhepTinh.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpPhepTinh.ForeColor = System.Drawing.Color.FromArgb(0, 100, 90);

            System.Action<System.Windows.Forms.Button, string, System.Drawing.Color, int> setOpBtn =
                (btn, text, color, x) => {
                    btn.Text = text; btn.Size = new System.Drawing.Size(70, 35);
                    btn.Location = new System.Drawing.Point(x, 20);
                    btn.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
                    btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat; btn.FlatAppearance.BorderSize = 0;
                    btn.BackColor = color; btn.ForeColor = System.Drawing.Color.White;
                    btn.Cursor = System.Windows.Forms.Cursors.Hand;
                };

            setOpBtn(this.btnCong, "+", System.Drawing.Color.FromArgb(63, 81, 181), 10);
            setOpBtn(this.btnTru, "−", System.Drawing.Color.FromArgb(233, 30, 99), 90);
            setOpBtn(this.btnNhan, "×", System.Drawing.Color.FromArgb(76, 175, 80), 170);
            setOpBtn(this.btnChia, "÷", System.Drawing.Color.FromArgb(255, 87, 34), 250);

            this.grpPhepTinh.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.btnCong, this.btnTru, this.btnNhan, this.btnChia });

            // grpKetQua
            this.grpKetQua.Location = new System.Drawing.Point(12, 258);
            this.grpKetQua.Size = new System.Drawing.Size(340, 95);
            this.grpKetQua.Text = "Kết quả";
            this.grpKetQua.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpKetQua.ForeColor = System.Drawing.Color.FromArgb(0, 100, 90);

            this.lblBieuThuc.Location = new System.Drawing.Point(10, 22); this.lblBieuThuc.Size = new System.Drawing.Size(315, 23);
            this.lblBieuThuc.Text = "Nhập hai số và chọn phép tính...";
            this.lblBieuThuc.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Italic);
            this.lblBieuThuc.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);

            this.txtKetQua.Location = new System.Drawing.Point(10, 50); this.txtKetQua.Size = new System.Drawing.Size(315, 33);
            this.txtKetQua.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.txtKetQua.ReadOnly = true; this.txtKetQua.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtKetQua.BackColor = System.Drawing.Color.FromArgb(232, 245, 233);
            this.txtKetQua.ForeColor = System.Drawing.Color.FromArgb(27, 94, 32);

            this.grpKetQua.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblBieuThuc, this.txtKetQua });

            // pnlBtns
            this.pnlBtns.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBtns.Height = 55;
            this.pnlBtns.BackColor = System.Drawing.Color.FromArgb(230, 245, 243);

            this.btnLamMoi.Size = new System.Drawing.Size(150, 36); this.btnLamMoi.Location = new System.Drawing.Point(30, 9);
            this.btnLamMoi.Text = "🔄 Làm mới"; this.btnLamMoi.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnLamMoi.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnLamMoi.FlatAppearance.BorderSize = 0;
            this.btnLamMoi.BackColor = System.Drawing.Color.FromArgb(245, 124, 0); this.btnLamMoi.ForeColor = System.Drawing.Color.White;
            this.btnLamMoi.Cursor = System.Windows.Forms.Cursors.Hand;

            this.btnThoat.Size = new System.Drawing.Size(150, 36); this.btnThoat.Location = new System.Drawing.Point(190, 9);
            this.btnThoat.Text = "❌ Thoát"; this.btnThoat.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnThoat.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnThoat.FlatAppearance.BorderSize = 0;
            this.btnThoat.BackColor = System.Drawing.Color.FromArgb(97, 97, 97); this.btnThoat.ForeColor = System.Drawing.Color.White;
            this.btnThoat.Cursor = System.Windows.Forms.Cursors.Hand;

            this.pnlBtns.Controls.AddRange(new System.Windows.Forms.Control[] { this.btnLamMoi, this.btnThoat });

            // frmMayTinh
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(245, 252, 251);
            this.ClientSize = new System.Drawing.Size(364, 435);
            this.Controls.Add(this.grpNhap);
            this.Controls.Add(this.grpPhepTinh);
            this.Controls.Add(this.grpKetQua);
            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.pnlBtns);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmMayTinh";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bài 6: Máy Tính 4 Phép Toán";

            this.btnCong.Click += new System.EventHandler(this.btnCong_Click);
            this.btnTru.Click += new System.EventHandler(this.btnTru_Click);
            this.btnNhan.Click += new System.EventHandler(this.btnNhan_Click);
            this.btnChia.Click += new System.EventHandler(this.btnChia_Click);
            this.btnLamMoi.Click += new System.EventHandler(this.btnLamMoi_Click);
            this.btnThoat.Click += new System.EventHandler(this.btnThoat_Click);
            this.txtSo1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSo_KeyDown);
            this.txtSo2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSo_KeyDown);

            this.grpNhap.ResumeLayout(false);
            this.grpPhepTinh.ResumeLayout(false);
            this.grpKetQua.ResumeLayout(false);
            this.pnlBtns.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.GroupBox grpNhap;
        private System.Windows.Forms.Label lblSo1;
        private System.Windows.Forms.TextBox txtSo1;
        private System.Windows.Forms.Label lblSo2;
        private System.Windows.Forms.TextBox txtSo2;
        private System.Windows.Forms.GroupBox grpPhepTinh;
        private System.Windows.Forms.Button btnCong;
        private System.Windows.Forms.Button btnTru;
        private System.Windows.Forms.Button btnNhan;
        private System.Windows.Forms.Button btnChia;
        private System.Windows.Forms.GroupBox grpKetQua;
        private System.Windows.Forms.Label lblBieuThuc;
        private System.Windows.Forms.TextBox txtKetQua;
        private System.Windows.Forms.Panel pnlBtns;
        private System.Windows.Forms.Button btnLamMoi;
        private System.Windows.Forms.Button btnThoat;
    }
}
