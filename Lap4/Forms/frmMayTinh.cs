using System;
using System.Windows.Forms;

namespace Lab04_WinFormsBasic.Forms
{
    /// <summary>
    /// Bài 6: Máy tính 4 phép toán (cộng, trừ, nhân, chia).
    /// Có nút Làm mới và Thoát.
    /// </summary>
    public partial class frmMayTinh : Form
    {
        public frmMayTinh()
        {
            InitializeComponent();
        }

        private bool LayHaiSo(out double so1, out double so2)
        {
            so1 = so2 = 0;
            if (!double.TryParse(txtSo1.Text, out so1))
            {
                MessageBox.Show("Số thứ nhất không hợp lệ!", "Lỗi nhập liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSo1.Focus(); txtSo1.SelectAll();
                return false;
            }
            if (!double.TryParse(txtSo2.Text, out so2))
            {
                MessageBox.Show("Số thứ hai không hợp lệ!", "Lỗi nhập liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSo2.Focus(); txtSo2.SelectAll();
                return false;
            }
            return true;
        }

        private void HienThiKetQua(double so1, string phepToan, double so2, double ketQua)
        {
            txtKetQua.Text = ketQua.ToString("G10");
            lblBieuThuc.Text = string.Format("{0} {1} {2} = {3}",
                so1, phepToan, so2, ketQua.ToString("G10"));
        }

        private void btnCong_Click(object sender, EventArgs e)
        {
            if (LayHaiSo(out double a, out double b))
                HienThiKetQua(a, "+", b, a + b);
        }

        private void btnTru_Click(object sender, EventArgs e)
        {
            if (LayHaiSo(out double a, out double b))
                HienThiKetQua(a, "-", b, a - b);
        }

        private void btnNhan_Click(object sender, EventArgs e)
        {
            if (LayHaiSo(out double a, out double b))
                HienThiKetQua(a, "×", b, a * b);
        }

        private void btnChia_Click(object sender, EventArgs e)
        {
            if (LayHaiSo(out double a, out double b))
            {
                if (b == 0)
                {
                    MessageBox.Show("Không thể chia cho 0!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtSo2.Focus(); txtSo2.SelectAll();
                    return;
                }
                HienThiKetQua(a, "÷", b, a / b);
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            txtSo1.Clear();
            txtSo2.Clear();
            txtKetQua.Clear();
            lblBieuThuc.Text = "Nhập hai số và chọn phép tính...";
            txtSo1.Focus();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Cho phép Enter trên TextBox để tính nhanh
        private void txtSo_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
                btnCong_Click(sender, e);
        }
    }
}
