using System;
using System.Windows.Forms;

namespace Lab04_WinFormsBasic.Forms
{
    /// <summary>
    /// Form chính - Menu điều hướng tới 7 bài tập Lab 4.
    /// </summary>
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void btnSinhVien_Click(object sender, EventArgs e)
        {
            new frmSinhVien().ShowDialog();
        }

        private void btnMayTinh_Click(object sender, EventArgs e)
        {
            new frmMayTinh().ShowDialog();
        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            new frmDangNhap().ShowDialog();
        }

        private void btnBMI_Click(object sender, EventArgs e)
        {
            new frmBMI().ShowDialog();
        }

        private void btnDangKyHocPhan_Click(object sender, EventArgs e)
        {
            new frmDangKyHocPhan().ShowDialog();
        }

        private void btnSanPham_Click(object sender, EventArgs e)
        {
            new frmSanPham().ShowDialog();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
