using System;
using System.Windows.Forms;

namespace Lab04_WinFormsBasic.Forms
{
    /// <summary>
    /// Bài 7: Form đăng nhập.
    /// - Tên đăng nhập, mật khẩu (PasswordChar).
    /// - CheckBox "Ghi nhớ đăng nhập".
    /// - Button "Hiển thị/Ẩn mật khẩu" toggle.
    /// - Tài khoản mẫu: admin/123456 và user/password.
    /// </summary>
    public partial class frmDangNhap : Form
    {
        // Tài khoản mẫu (trong thực tế nên dùng database)
        private readonly string[,] _taiKhoanMau = {
            { "admin", "123456" },
            { "user", "password" },
            { "sinhvien", "sv2026" }
        };

        private int _soLanSai = 0;
        private const int MAX_SAI = 3;

        public frmDangNhap()
        {
            InitializeComponent();
        }

        private void frmDangNhap_Load(object sender, EventArgs e)
        {
            txtTenDangNhap.Focus();
            UpdateDangNhapStatus();
        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            if (_soLanSai >= MAX_SAI)
            {
                MessageBox.Show("Tài khoản bị khóa do nhập sai quá nhiều lần!\nVui lòng liên hệ quản trị viên.",
                    "Tài khoản bị khóa", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            string tenDN = txtTenDangNhap.Text.Trim();
            string matKhau = txtMatKhau.Text;

            if (string.IsNullOrEmpty(tenDN))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenDangNhap.Focus();
                return;
            }

            if (string.IsNullOrEmpty(matKhau))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMatKhau.Focus();
                return;
            }

            // Kiểm tra tài khoản
            bool thanhCong = false;
            for (int i = 0; i < _taiKhoanMau.GetLength(0); i++)
            {
                if (_taiKhoanMau[i, 0] == tenDN && _taiKhoanMau[i, 1] == matKhau)
                {
                    thanhCong = true;
                    break;
                }
            }

            if (thanhCong)
            {
                string ghiNho = chkGhiNho.Checked ? "\n✅ Đã ghi nhớ đăng nhập" : "";
                MessageBox.Show(
                    string.Format("🎉 Đăng nhập thành công!\nXin chào: {0}{1}", tenDN, ghiNho),
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _soLanSai = 0;
            }
            else
            {
                _soLanSai++;
                int conLai = MAX_SAI - _soLanSai;
                string thongBao = conLai > 0
                    ? string.Format("❌ Tên đăng nhập hoặc mật khẩu không đúng!\nCòn {0} lần thử.", conLai)
                    : "❌ Nhập sai quá nhiều lần. Tài khoản bị khóa!";

                MessageBox.Show(thongBao, "Đăng nhập thất bại",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtMatKhau.Clear();
                txtMatKhau.Focus();
                UpdateDangNhapStatus();
            }
        }

        private void chkHienMatKhau_CheckedChanged(object sender, EventArgs e)
        {
            // Toggle hiển thị/ẩn mật khẩu
            txtMatKhau.PasswordChar = chkHienMatKhau.Checked ? '\0' : '●';
            btnToggleMatKhau.Text = chkHienMatKhau.Checked ? "🙈 Ẩn" : "👁️ Hiện";
        }

        private void btnToggleMatKhau_Click(object sender, EventArgs e)
        {
            chkHienMatKhau.Checked = !chkHienMatKhau.Checked;
        }

        private void btnDangKy_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng đăng ký sẽ được triển khai trong Lab 5!",
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void UpdateDangNhapStatus()
        {
            if (_soLanSai >= MAX_SAI)
            {
                btnDangNhap.Enabled = false;
                lblTrangThai.Text = "🔒 Tài khoản bị khóa";
                lblTrangThai.ForeColor = System.Drawing.Color.Red;
            }
            else if (_soLanSai > 0)
            {
                lblTrangThai.Text = string.Format("⚠️ Đã sai {0}/{1} lần", _soLanSai, MAX_SAI);
                lblTrangThai.ForeColor = System.Drawing.Color.OrangeRed;
            }
            else
            {
                lblTrangThai.Text = "💡 Gợi ý: admin/123456";
                lblTrangThai.ForeColor = System.Drawing.Color.FromArgb(100, 100, 150);
            }
        }

        // Enter trên txtTenDangNhap chuyển sang txtMatKhau
        private void txtTenDangNhap_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) txtMatKhau.Focus();
        }

        // Enter trên txtMatKhau thực hiện đăng nhập
        private void txtMatKhau_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) btnDangNhap_Click(sender, e);
        }
    }
}
