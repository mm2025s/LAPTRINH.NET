using System;
using System.Drawing;
using System.Windows.Forms;
using Lab05_MultiForm.Helpers;
using Lab05_MultiForm.Models;

namespace Lab05_MultiForm.Forms
{
    /// <summary>
    /// Bài 7: Hộp thoại tìm kiếm.
    /// Form nhận DANH SÁCH sinh viên qua constructor (không phụ thuộc frmMain)
    /// và trả KẾT QUẢ về Form gọi qua thuộc tính + DialogResult.
    ///   - KetQua : sinh viên được chọn (null nếu hủy)
    ///   - TuKhoa : từ khóa người dùng đã tìm
    /// </summary>
    public partial class frmTimKiem : FormBase
    {
        private readonly System.Collections.Generic.List<SinhVien> _nguon;

        public SinhVien KetQua { get; private set; }
        public string TuKhoa { get; private set; }

        public frmTimKiem(System.Collections.Generic.IEnumerable<SinhVien> nguon)
        {
            InitializeComponent();

            _nguon = new System.Collections.Generic.List<SinhVien>();
            if (nguon != null) _nguon.AddRange(nguon);
            TuKhoa = string.Empty;
        }

        private void frmTimKiem_Load(object sender, EventArgs e)
        {
            radMa.Checked = true;
            lblTong.Text = "Đang tìm trong " + _nguon.Count + " sinh viên.";
            ThucHienTim(string.Empty);
            txtTuKhoa.Focus();
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            ThucHienTim(UiFactory.Val(txtTuKhoa));
        }

        private void txtTuKhoa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { ThucHienTim(UiFactory.Val(txtTuKhoa)); e.SuppressKeyPress = true; }
        }

        private void ThucHienTim(string tuKhoa)
        {
            TuKhoa = tuKhoa;
            lstKetQua.Items.Clear();
            string k = tuKhoa.Trim().ToLowerInvariant();

            int dem = 0;
            foreach (SinhVien sv in _nguon)
            {
                string nguon;
                if (radMa.Checked) nguon = sv.MaSV;
                else if (radTen.Checked) nguon = sv.HoTen;
                else nguon = sv.Khoa;

                if (k.Length > 0 && !nguon.ToLowerInvariant().Contains(k)) continue;

                string[] cot = {
                    sv.MaSV,
                    sv.HoTen,
                    sv.Khoa,
                    sv.DiemTB.ToString("0.0"),
                    sv.XepLoai()
                };
                var item = new ListViewItem(cot);
                item.Tag = sv;
                item.ForeColor = KiemTra.MauXepLoai(sv.DiemTB);
                lstKetQua.Items.Add(item);
                dem++;
            }

            if (dem == 0)
            {
                lblTong.Text = k.Length == 0
                    ? "Chưa có dữ liệu."
                    : "Không tìm thấy sinh viên nào khớp \"" + tuKhoa + "\".";
                lblTong.ForeColor = UiTheme.Red;
                btnChon.Enabled = false;
            }
            else
            {
                lblTong.Text = string.Format("Tìm thấy {0} kết quả cho \"{1}\" - chọn một dòng rồi bấm Chọn.",
                    dem, k.Length == 0 ? "tất cả" : tuKhoa);
                lblTong.ForeColor = UiTheme.Green;
                btnChon.Enabled = true;
                if (lstKetQua.Items.Count > 0) lstKetQua.Items[0].Selected = true;
            }
        }

        private void lstKetQua_DoubleClick(object sender, EventArgs e)
        {
            btnChon_Click(sender, e);
        }

        private void btnChon_Click(object sender, EventArgs e)
        {
            if (lstKetQua.SelectedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một sinh viên trong danh sách kết quả.",
                    "Chưa chọn", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Trả kết quả qua THUỘC TÍNH + DialogResult
            KetQua = lstKetQua.SelectedItems[0].Tag as SinhVien;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            KetQua = null;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void chkKieuTim_CheckedChanged(object sender, EventArgs e)
        {
            ThucHienTim(UiFactory.Val(txtTuKhoa));
        }
    }
}
