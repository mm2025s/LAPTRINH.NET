using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Lab04_WinFormsBasic.Models;

namespace Lab04_WinFormsBasic.Forms
{
    /// <summary>
    /// Bài 1-5: Form nhập thông tin sinh viên.
    /// - Bài 1: Hiển thị thông tin qua MessageBox.
    /// - Bài 2: Nạp ComboBox khi Form_Load.
    /// - Bài 3: Tính tuổi từ DateTimePicker.
    /// - Bài 4: Hiển thị sở thích từ CheckBox.
    /// - Bài 5: Chọn ảnh đại diện qua OpenFileDialog.
    /// </summary>
    public partial class frmSinhVien : Form
    {
        public frmSinhVien()
        {
            InitializeComponent();
        }

        // =====================================================================
        // Bài 2: Khởi tạo ComboBox và trạng thái Form khi Load
        // =====================================================================
        private void frmSinhVien_Load(object sender, EventArgs e)
        {
            // Nạp danh sách khoa
            cboKhoa.Items.Add("Công nghệ thông tin");
            cboKhoa.Items.Add("Công nghệ kỹ thuật ô tô");
            cboKhoa.Items.Add("Kinh tế");
            cboKhoa.Items.Add("Điện - Điện tử");
            cboKhoa.Items.Add("Xây dựng");
            cboKhoa.DropDownStyle = ComboBoxStyle.DropDownList;
            if (cboKhoa.Items.Count > 0) cboKhoa.SelectedIndex = 0;

            // Mặc định giới tính Nam
            radNam.Checked = true;

            // Tính tuổi mặc định
            lblTuoi.Text = "Tuổi: " + TinhTuoi(dtpNgaySinh.Value);
        }

        // =====================================================================
        // Bài 1: Hiển thị thông tin sinh viên
        // =====================================================================
        private void btnHienThi_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaSV.Text))
            {
                MessageBox.Show("Vui lòng nhập mã sinh viên!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaSV.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtHoTen.Text))
            {
                MessageBox.Show("Vui lòng nhập họ tên sinh viên!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtHoTen.Focus();
                return;
            }

            string gioiTinh = radNam.Checked ? "Nam" : "Nữ";
            string soThich = LayDanhSachSoThich();

            string thongTin = string.Format(
                "Mã SV   : {0}\n" +
                "Họ tên  : {1}\n" +
                "Ngày sinh: {2:dd/MM/yyyy}\n" +
                "Tuổi    : {3}\n" +
                "Giới tính: {4}\n" +
                "Khoa    : {5}\n" +
                "Sở thích: {6}",
                txtMaSV.Text,
                txtHoTen.Text,
                dtpNgaySinh.Value,
                TinhTuoi(dtpNgaySinh.Value),
                gioiTinh,
                cboKhoa.Text,
                string.IsNullOrEmpty(soThich) ? "Chưa chọn" : soThich);

            MessageBox.Show(thongTin, "Thông tin sinh viên",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // =====================================================================
        // Bài 3: Tính tuổi từ DateTimePicker
        // =====================================================================
        private int TinhTuoi(DateTime ngaySinh)
        {
            int tuoi = DateTime.Today.Year - ngaySinh.Year;
            if (ngaySinh.Date > DateTime.Today.AddYears(-tuoi)) tuoi--;
            return tuoi;
        }

        private void dtpNgaySinh_ValueChanged(object sender, EventArgs e)
        {
            int tuoi = TinhTuoi(dtpNgaySinh.Value);
            lblTuoi.Text = "Tuổi: " + tuoi;

            // Đổi màu cảnh báo nếu tuổi nhỏ
            lblTuoi.ForeColor = tuoi < 16
                ? System.Drawing.Color.OrangeRed
                : System.Drawing.Color.FromArgb(0, 120, 60);
        }

        // =====================================================================
        // Bài 4: Hiển thị danh sách sở thích từ CheckBox
        // =====================================================================
        private string LayDanhSachSoThich()
        {
            List<string> ds = new List<string>();
            if (chkDocSach.Checked) ds.Add("Đọc sách");
            if (chkTheThao.Checked) ds.Add("Thể thao");
            if (chkLapTrinh.Checked) ds.Add("Lập trình");
            if (chkAmNhac.Checked) ds.Add("Âm nhạc");
            if (chkDuLich.Checked) ds.Add("Du lịch");
            return string.Join(", ", ds);
        }

        private void AnySoThich_CheckedChanged(object sender, EventArgs e)
        {
            string ds = LayDanhSachSoThich();
            lblSoThich.Text = string.IsNullOrEmpty(ds) ? "Chưa chọn sở thích" : ds;
        }

        // =====================================================================
        // Bài 5: Chọn ảnh đại diện qua OpenFileDialog
        // =====================================================================
        private void btnChonAnh_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Chọn ảnh đại diện";
                dlg.Filter = "Image files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        picAnh.SizeMode = PictureBoxSizeMode.Zoom;
                        picAnh.ImageLocation = dlg.FileName;
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

        private void btnXoaAnh_Click(object sender, EventArgs e)
        {
            picAnh.Image = null;
            picAnh.ImageLocation = null;
            txtDuongDanAnh.Clear();
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            txtMaSV.Clear();
            txtHoTen.Clear();
            dtpNgaySinh.Value = DateTime.Today.AddYears(-20);
            cboKhoa.SelectedIndex = 0;
            radNam.Checked = true;
            chkDocSach.Checked = false;
            chkTheThao.Checked = false;
            chkLapTrinh.Checked = false;
            chkAmNhac.Checked = false;
            chkDuLich.Checked = false;
            lblSoThich.Text = "Chưa chọn sở thích";
            picAnh.Image = null;
            picAnh.ImageLocation = null;
            txtDuongDanAnh.Clear();
            txtMaSV.Focus();
        }
    }
}
