using System;
using System.Windows.Forms;

namespace Lab04_WinFormsBasic.Forms
{
    /// <summary>
    /// Bài 10: Phiếu nhập sản phẩm.
    /// Giao diện gồm: mã, tên, loại (ComboBox), số lượng, đơn giá, ngày nhập, ảnh.
    /// Tự động tính thành tiền = số lượng × đơn giá.
    /// </summary>
    public partial class frmSanPham : Form
    {
        public frmSanPham()
        {
            InitializeComponent();
        }

        private void frmSanPham_Load(object sender, EventArgs e)
        {
            // Nạp danh sách loại sản phẩm
            cboLoai.Items.AddRange(new object[] {
                "Điện tử", "Thực phẩm", "Đồ dùng gia đình",
                "Thời trang", "Văn phòng phẩm", "Sách & Giáo trình",
                "Đồ chơi", "Thiết bị y tế", "Khác"
            });
            cboLoai.DropDownStyle = ComboBoxStyle.DropDownList;
            cboLoai.SelectedIndex = 0;

            // Ngày nhập mặc định là hôm nay
            dtpNgayNhap.Value = DateTime.Today;
            dtpNgayNhap.MaxDate = DateTime.Today;

            TinhThanhTien();
        }

        private void TinhThanhTien()
        {
            if (int.TryParse(txtSoLuong.Text, out int sl) &&
                decimal.TryParse(txtDonGia.Text, out decimal dg) &&
                sl > 0 && dg >= 0)
            {
                decimal thanhTien = sl * dg;
                txtThanhTien.Text = thanhTien.ToString("N0");
                txtThanhTien.ForeColor = System.Drawing.Color.FromArgb(27, 94, 32);
            }
            else
            {
                txtThanhTien.Text = "0";
                txtThanhTien.ForeColor = System.Drawing.Color.Gray;
            }
        }

        private void txtSoLuong_TextChanged(object sender, EventArgs e) => TinhThanhTien();
        private void txtDonGia_TextChanged(object sender, EventArgs e) => TinhThanhTien();

        private void btnChonAnh_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Chọn ảnh sản phẩm";
                dlg.Filter = "Image files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        picSanPham.SizeMode = PictureBoxSizeMode.Zoom;
                        picSanPham.ImageLocation = dlg.FileName;
                        txtDuongDanAnh.Text = dlg.FileName;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Không thể tải ảnh: " + ex.Message, "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(txtMaSP.Text))
            {
                MessageBox.Show("Vui lòng nhập mã sản phẩm!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaSP.Focus(); return;
            }
            if (string.IsNullOrWhiteSpace(txtTenSP.Text))
            {
                MessageBox.Show("Vui lòng nhập tên sản phẩm!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenSP.Focus(); return;
            }
            if (!int.TryParse(txtSoLuong.Text, out int sl) || sl <= 0)
            {
                MessageBox.Show("Số lượng phải là số nguyên dương!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSoLuong.Focus(); txtSoLuong.SelectAll(); return;
            }
            if (!decimal.TryParse(txtDonGia.Text, out decimal dg) || dg < 0)
            {
                MessageBox.Show("Đơn giá không hợp lệ!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDonGia.Focus(); txtDonGia.SelectAll(); return;
            }

            string thongBao = string.Format(
                "✅ PHIẾU NHẬP SẢN PHẨM\n" +
                "━━━━━━━━━━━━━━━━━━━━━━━━\n" +
                "Mã SP      : {0}\n" +
                "Tên SP     : {1}\n" +
                "Loại       : {2}\n" +
                "Số lượng   : {3:N0}\n" +
                "Đơn giá    : {4:N0} VNĐ\n" +
                "Thành tiền : {5:N0} VNĐ\n" +
                "Ngày nhập  : {6:dd/MM/yyyy}\n" +
                "━━━━━━━━━━━━━━━━━━━━━━━━\n" +
                "Đã lưu phiếu nhập thành công!",
                txtMaSP.Text, txtTenSP.Text, cboLoai.Text,
                sl, dg, sl * dg, dtpNgayNhap.Value);

            MessageBox.Show(thongBao, "Lưu thành công",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            txtMaSP.Clear();
            txtTenSP.Clear();
            cboLoai.SelectedIndex = 0;
            txtSoLuong.Text = "1";
            txtDonGia.Clear();
            txtThanhTien.Text = "0";
            dtpNgayNhap.Value = DateTime.Today;
            picSanPham.Image = null;
            picSanPham.ImageLocation = null;
            txtDuongDanAnh.Clear();
            txtMoTa.Clear();
            txtMaSP.Focus();
        }
    }
}
