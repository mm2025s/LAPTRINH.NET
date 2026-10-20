using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Lab04_WinFormsBasic.Forms
{
    /// <summary>
    /// Bài 9: Đăng ký học phần.
    /// Dùng CheckedListBox để chọn môn; tính tổng số tín chỉ.
    /// </summary>
    public partial class frmDangKyHocPhan : Form
    {
        // Danh sách môn học: (Tên môn, Số tín chỉ)
        private readonly (string Ten, int TinChi)[] _danhSachMon = {
            ("Lập trình .NET (IT3212)", 3),
            ("Cơ sở dữ liệu (IT3201)", 3),
            ("Lập trình Web (IT3301)", 3),
            ("Mạng máy tính (IT3401)", 2),
            ("Trí tuệ nhân tạo (IT3501)", 3),
            ("An toàn thông tin (IT3601)", 2),
            ("Phân tích thiết kế (IT3701)", 3),
            ("Đồ án nhóm (IT3999)", 4),
            ("Tiếng Anh chuyên ngành (NN301)", 2),
            ("Toán rời rạc (TN201)", 3),
        };

        private const int MAX_TIN_CHI = 24;
        private const int MIN_TIN_CHI = 12;

        public frmDangKyHocPhan()
        {
            InitializeComponent();
        }

        private void frmDangKyHocPhan_Load(object sender, EventArgs e)
        {
            foreach (var mon in _danhSachMon)
                clbMonHoc.Items.Add(string.Format("{0} [{1} TC]", mon.Ten, mon.TinChi));

            CapNhatTongTinChi();
        }

        private void clbMonHoc_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Cần delay 1 tick để CheckedItems cập nhật với e.NewValue
            this.BeginInvoke(new Action(CapNhatTongTinChi));
        }

        private void CapNhatTongTinChi()
        {
            int tong = 0;
            List<string> dsMon = new List<string>();

            for (int i = 0; i < clbMonHoc.CheckedIndices.Count; i++)
            {
                int idx = clbMonHoc.CheckedIndices[i];
                tong += _danhSachMon[idx].TinChi;
                dsMon.Add(_danhSachMon[idx].Ten);
            }

            lblTongTinChi.Text = string.Format("Tổng số tín chỉ đã chọn: {0} / {1}", tong, MAX_TIN_CHI);

            if (tong > MAX_TIN_CHI)
            {
                lblTongTinChi.ForeColor = System.Drawing.Color.Red;
                lblTrangThai.Text = string.Format("⛔ Vượt quá {0} tín chỉ! Hãy bỏ bớt môn.", MAX_TIN_CHI);
                lblTrangThai.ForeColor = System.Drawing.Color.Red;
            }
            else if (tong < MIN_TIN_CHI && dsMon.Count > 0)
            {
                lblTongTinChi.ForeColor = System.Drawing.Color.OrangeRed;
                lblTrangThai.Text = string.Format("⚠️ Tối thiểu {0} tín chỉ/kỳ. Hãy chọn thêm môn.", MIN_TIN_CHI);
                lblTrangThai.ForeColor = System.Drawing.Color.OrangeRed;
            }
            else if (tong >= MIN_TIN_CHI)
            {
                lblTongTinChi.ForeColor = System.Drawing.Color.FromArgb(27, 94, 32);
                lblTrangThai.Text = "✅ Số tín chỉ hợp lệ. Có thể xác nhận đăng ký.";
                lblTrangThai.ForeColor = System.Drawing.Color.FromArgb(27, 94, 32);
            }
            else
            {
                lblTongTinChi.ForeColor = System.Drawing.Color.Gray;
                lblTrangThai.Text = "Chưa chọn môn học nào.";
                lblTrangThai.ForeColor = System.Drawing.Color.Gray;
            }

            // Hiển thị chi tiết các môn đã chọn
            txtDanhSachChon.Text = dsMon.Count > 0
                ? string.Join("\r\n", dsMon)
                : "(Chưa chọn môn nào)";
        }

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            int tong = 0;
            List<string> dsMon = new List<string>();

            for (int i = 0; i < clbMonHoc.CheckedIndices.Count; i++)
            {
                int idx = clbMonHoc.CheckedIndices[i];
                tong += _danhSachMon[idx].TinChi;
                dsMon.Add(string.Format("  • {0} ({1} TC)", _danhSachMon[idx].Ten, _danhSachMon[idx].TinChi));
            }

            if (dsMon.Count == 0)
            {
                MessageBox.Show("Chưa chọn môn học nào!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (tong > MAX_TIN_CHI)
            {
                MessageBox.Show(string.Format("Vượt quá {0} tín chỉ! Vui lòng bỏ bớt môn.", MAX_TIN_CHI),
                    "Lỗi đăng ký", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (tong < MIN_TIN_CHI)
            {
                MessageBox.Show(string.Format("Tối thiểu {0} tín chỉ/kỳ! Vui lòng chọn thêm môn.", MIN_TIN_CHI),
                    "Lỗi đăng ký", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string thongBao = string.Format(
                "📋 XÁC NHẬN ĐĂNG KÝ HỌC PHẦN\n" +
                "━━━━━━━━━━━━━━━━━━━━━━━━\n" +
                "Danh sách môn học đã đăng ký:\n{0}\n\n" +
                "📊 Tổng số tín chỉ: {1} tín chỉ\n" +
                "━━━━━━━━━━━━━━━━━━━━━━━━\n" +
                "Đăng ký thành công!",
                string.Join("\n", dsMon), tong);

            MessageBox.Show(thongBao, "Đăng ký thành công",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnBoChon_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbMonHoc.Items.Count; i++)
                clbMonHoc.SetItemChecked(i, false);
            CapNhatTongTinChi();
        }

        private void btnChonTatCa_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbMonHoc.Items.Count; i++)
                clbMonHoc.SetItemChecked(i, true);
            CapNhatTongTinChi();
        }
    }
}
