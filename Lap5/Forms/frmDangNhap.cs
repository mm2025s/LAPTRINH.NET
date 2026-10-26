using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Lab05_MultiForm.Helpers;

namespace Lab05_MultiForm.Forms
{
    /// <summary>
    /// Bài 6: Form đăng nhập có giới hạn lần thử.
    ///   - Sai 3 lần  => khóa nút đăng nhập trong 30 giây (đếm ngược hiển thị từng giây).
    ///   - Hết 30 giây => cho phép thử lại và đặt lại bộ đếm.
    /// Truyền kết quả về Form gọi qua thuộc tính TenDangNhap / VaiTro + DialogResult.OK.
    /// </summary>
    public partial class frmDangNhap : FormBase
    {
        private const int MaxLanSai = 3;
        private const int ThoiGianKhoa = 30;   // giây

        private sealed class TaiKhoan
        {
            public string Ten;
            public string MatKhau;
            public string VaiTro;
        }

        private readonly List<TaiKhoan> _taiKhoan = new List<TaiKhoan>
        {
            new TaiKhoan { Ten = "admin",    MatKhau = "123456",   VaiTro = "Quản trị viên" },
            new TaiKhoan { Ten = "user",     MatKhau = "password", VaiTro = "Giảng viên" },
            new TaiKhoan { Ten = "sinhvien", MatKhau = "sv2026",   VaiTro = "Sinh viên" }
        };

        private readonly System.Windows.Forms.Timer _demNguoc = new System.Windows.Forms.Timer();
        private int _soLanSai;
        private int _giayConLai;

        public string TenDangNhap { get; private set; }
        public string VaiTro { get; private set; }

        public frmDangNhap()
        {
            InitializeComponent();

            _demNguoc.Interval = 1000;
            _demNguoc.Tick += DemNguoc_Tick;
            TenDangNhap = string.Empty;
            VaiTro = string.Empty;
        }

        private void frmDangNhap_Load(object sender, EventArgs e)
        {
            CapNhatTrangThai();
            txtTenDangNhap.Focus();
        }

        // =====================================================================
        // ĐĂNG NHẬP
        // =====================================================================
        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            string loi;

            if (!KiemTra.HopLeDangNhap(UiFactory.Val(txtTenDangNhap), out loi))
            {
                errorProvider1.SetError(txtTenDangNhap, loi);
                txtTenDangNhap.Focus();
                return;
            }

            if (!KiemTra.HopLeMatKhau(UiFactory.Val(txtMatKhau), out loi))
            {
                errorProvider1.SetError(txtMatKhau, loi);
                txtMatKhau.Focus();
                return;
            }

            string ten = UiFactory.Val(txtTenDangNhap);
            string mk = UiFactory.Val(txtMatKhau);

            TaiKhoan tk = _taiKhoan.Find(x =>
                string.Equals(x.Ten, ten, StringComparison.OrdinalIgnoreCase) && x.MatKhau == mk);

            if (tk != null)
            {
                TenDangNhap = tk.Ten;
                VaiTro = tk.VaiTro;
                _soLanSai = 0;
                lblTrangThai.Text = "Đăng nhập thành công - xin chào " + tk.Ten + "!";
                lblTrangThai.ForeColor = UiTheme.Green;

                DialogResult = DialogResult.OK;
                Close();
                return;
            }

            // Sai mật khẩu
            _soLanSai++;
            UiFactory.SetVal(txtMatKhau, string.Empty);

            if (_soLanSai >= MaxLanSai)
            {
                KhoaDangNhap();
            }
            else
            {
                // Dữ liệu thử nhanh: dùng thông báo qua ErrorProvider thay vì MessageBox để không chặn luồng
                errorProvider1.SetError(txtMatKhau, string.Format("Sai tài khoản/mật khẩu - còn {0} lần thử.", MaxLanSai - _soLanSai));
                CapNhatTrangThai();
            }

            txtMatKhau.Focus();
        }

        /// <summary>Sai đủ 3 lần: khóa nút đăng nhập 30 giây.</summary>
        private void KhoaDangNhap()
        {
            _giayConLai = ThoiGianKhoa;

            btnDangNhap.Enabled = false;
            txtTenDangNhap.Enabled = false;
            txtMatKhau.Enabled = false;

            // Khi chạy trình tự chụp ảnh báo cáo: không hiện MessageBox (sẽ chặn luồng)
            if (Environment.GetEnvironmentVariable("LAB5_SCREENSHOT") != "1")
            {
                MessageBox.Show(
                    string.Format("Bạn đã nhập sai {0} lần.\nNút đăng nhập bị khóa trong {1} giây.", MaxLanSai, ThoiGianKhoa),
                    "Tài khoản tạm khóa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            _demNguoc.Start();
            CapNhatTrangThai();
        }

        private void DemNguoc_Tick(object sender, EventArgs e)
        {
            _giayConLai--;

            if (_giayConLai > 0)
            {
                CapNhatTrangThai();
                return;
            }

            // Hết thời gian khóa: mở lại và đặt lại số lần sai
            _demNguoc.Stop();
            _soLanSai = 0;
            btnDangNhap.Enabled = true;
            txtTenDangNhap.Enabled = true;
            txtMatKhau.Enabled = true;
            lblTrangThai.Text = "Đã hết thời gian khóa - bạn có thể đăng nhập lại.";
            lblTrangThai.ForeColor = UiTheme.Green;
            txtTenDangNhap.Focus();
        }

        private void CapNhatTrangThai()
        {
            if (!btnDangNhap.Enabled)
            {
                lblTrangThai.Text = string.Format("Tài khoản tạm khóa - thử lại sau {0} giây.", _giayConLai);
                lblTrangThai.ForeColor = UiTheme.Red;
                lblGoiY.Text = string.Format("Đã sai {0}/{1} lần. Sau {2} giây hệ thống tự mở khóa.", MaxLanSai, MaxLanSai, _giayConLai);
                return;
            }

            if (_soLanSai == 0)
            {
                lblTrangThai.Text = "Nhập tên đăng nhập và mật khẩu để tiếp tục.";
                lblTrangThai.ForeColor = UiTheme.Muted;
                lblGoiY.Text = "Tài khoản mẫu: admin / 123456. Chỉ cần bấm nút Hiện để xem mật khẩu đã nhập.";
            }
            else
            {
                lblTrangThai.Text = string.Format("Đăng nhập thất bại - đã sai {0}/{1} lần.", _soLanSai, MaxLanSai);
                lblTrangThai.ForeColor = UiTheme.Amber;
                lblGoiY.Text = string.Format("Còn {0} lần thử trước khi nút đăng nhập bị khóa 30 giây.", MaxLanSai - _soLanSai);
            }
        }

        // =====================================================================
        // HIỂN THỊ MẬT KHẨU
        // =====================================================================
        private void chkHienMatKhau_CheckedChanged(object sender, EventArgs e)
        {
            txtMatKhau.UseSystemPasswordChar = !chkHienMatKhau.Checked;
            btnToggle.Text = chkHienMatKhau.Checked ? "Ẩn" : "Hiện";
        }

        private void btnToggle_Click(object sender, EventArgs e)
        {
            chkHienMatKhau.Checked = !chkHienMatKhau.Checked;
        }

        // =====================================================================
        // PHÍM TẮT
        // =====================================================================
        private void txtTenDangNhap_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtMatKhau.Focus();
                e.SuppressKeyPress = true;
            }
        }

        private void txtMatKhau_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && btnDangNhap.Enabled)
            {
                btnDangNhap_Click(sender, e);
                e.SuppressKeyPress = true;
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void frmDangNhap_FormClosing(object sender, FormClosingEventArgs e)
        {
            _demNguoc.Stop();
            if (DialogResult == DialogResult.None) DialogResult = DialogResult.Cancel;
        }
    }
}
