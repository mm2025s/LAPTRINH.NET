using System;
using System.Windows.Forms;

namespace Lab04_WinFormsBasic.Forms
{
    /// <summary>
    /// Bài 8: Tính chỉ số BMI.
    /// Công thức: BMI = cân nặng (kg) / (chiều cao (m))^2
    /// Phân loại: Gầy < 18.5 | Bình thường 18.5–24.9 | Thừa cân 25–29.9 | Béo phì >= 30
    /// </summary>
    public partial class frmBMI : Form
    {
        public frmBMI()
        {
            InitializeComponent();
        }

        private void btnTinhBMI_Click(object sender, EventArgs e)
        {
            // Validate chiều cao
            if (!double.TryParse(txtChieuCao.Text, out double chieuCao) || chieuCao <= 0)
            {
                MessageBox.Show("Chiều cao không hợp lệ! (ví dụ: 170)", "Lỗi nhập liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtChieuCao.Focus(); txtChieuCao.SelectAll();
                return;
            }

            // Validate cân nặng
            if (!double.TryParse(txtCanNang.Text, out double canNang) || canNang <= 0)
            {
                MessageBox.Show("Cân nặng không hợp lệ! (ví dụ: 60)", "Lỗi nhập liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCanNang.Focus(); txtCanNang.SelectAll();
                return;
            }

            // Đổi chiều cao từ cm sang m
            double chieuCaoMet = chieuCao / 100.0;
            double bmi = canNang / (chieuCaoMet * chieuCaoMet);

            string phanLoai = XepLoaiBMI(bmi);
            System.Drawing.Color mauPhanLoai = LayMauPhanLoai(bmi);

            // Hiển thị kết quả
            txtBMI.Text = bmi.ToString("F2");
            lblPhanLoai.Text = "Phân loại: " + phanLoai;
            lblPhanLoai.ForeColor = mauPhanLoai;
            lblNhanXet.Text = LayNhanXet(bmi);

            // Cập nhật progress bar trực quan
            int bmiInt = Math.Min(40, Math.Max(10, (int)bmi));
            progressBMI.Value = bmiInt;
            progressBMI.ForeColor = mauPhanLoai;
        }

        private string XepLoaiBMI(double bmi)
        {
            if (bmi < 16) return "🚨 Thiếu cân nặng (độ III)";
            if (bmi < 17) return "⚠️ Thiếu cân nặng (độ II)";
            if (bmi < 18.5) return "⚠️ Thiếu cân (Gầy)";
            if (bmi < 25) return "✅ Bình thường";
            if (bmi < 30) return "⚠️ Thừa cân";
            if (bmi < 35) return "🚨 Béo phì (độ I)";
            if (bmi < 40) return "🚨 Béo phì (độ II)";
            return "🚨 Béo phì (độ III)";
        }

        private System.Drawing.Color LayMauPhanLoai(double bmi)
        {
            if (bmi < 18.5) return System.Drawing.Color.FromArgb(25, 118, 210);
            if (bmi < 25) return System.Drawing.Color.FromArgb(56, 142, 60);
            if (bmi < 30) return System.Drawing.Color.FromArgb(245, 124, 0);
            return System.Drawing.Color.FromArgb(198, 40, 40);
        }

        private string LayNhanXet(double bmi)
        {
            if (bmi < 18.5) return "Bạn đang thiếu cân. Nên ăn uống đầy đủ dinh dưỡng hơn.";
            if (bmi < 25) return "Chúc mừng! Bạn đang có cân nặng lý tưởng. Hãy duy trì!";
            if (bmi < 30) return "Bạn đang thừa cân. Nên tập thể dục và ăn uống lành mạnh hơn.";
            return "Bạn đang trong tình trạng béo phì. Cần có chế độ ăn và tập luyện khoa học.";
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            txtChieuCao.Clear();
            txtCanNang.Clear();
            txtBMI.Clear();
            lblPhanLoai.Text = "Phân loại: ---";
            lblPhanLoai.ForeColor = System.Drawing.Color.Gray;
            lblNhanXet.Text = "Nhập chiều cao và cân nặng, sau đó nhấn Tính BMI.";
            lblNhanXet.ForeColor = System.Drawing.Color.Gray;
            progressBMI.Value = 0;
            txtChieuCao.Focus();
        }
    }
}
