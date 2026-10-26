using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Lab05_MultiForm.Helpers;
using Lab05_MultiForm.Models;

namespace Lab05_MultiForm.Forms
{
    /// <summary>
    /// Form phụ "Kiểm tra dữ liệu" (menu Trợ giúp) - minh họa việc DÙNG LẠI
    /// bộ hàm dùng chung KiemTra ở một Form khác mà không viết lại logic.
    ///
    /// Nhập dữ liệu -> bấm Kiểm tra -> xem bảng kết quả từng quy tắc.
    /// </summary>
    public partial class frmKiemTraDuLieu : FormBase
    {
        public frmKiemTraDuLieu()
        {
            InitializeComponent();
        }

        private void frmKiemTraDuLieu_Load(object sender, EventArgs e)
        {
            cboKhoa.Items.Clear();
            foreach (Khoa k in DuLieuMau.DanhSachKhoa()) cboKhoa.Items.Add(k);
            if (cboKhoa.Items.Count > 0) cboKhoa.SelectedIndex = 0;

            KiemTraDuLieu();
        }

        private void txtDiem_KeyPress(object sender, KeyPressEventArgs e)
        {
            KiemTra.ChanNhapDiem(sender, e);
        }

        private void btnKiemTra_Click(object sender, EventArgs e)
        {
            KiemTraDuLieu();
        }

        private void KiemTraDuLieu()
        {
            lstKetQua.Items.Clear();
            string loi;
            bool tatCaDung = true;

            // 1. Mã sinh viên
            bool maDung = KiemTra.HopLeMaSV(UiFactory.Val(txtMa), out loi);
            ThemDong("1. Mã sinh viên", maDung, maDung ? "Hợp lệ" : loi, maDung);

            // 2. Họ tên
            bool tenDung = KiemTra.HopLeHoTen(UiFactory.Val(txtTen), out loi);
            ThemDong("2. Họ tên", tenDung, tenDung ? "Hợp lệ - chuẩn hóa: " + KiemTra.ChuanHoaHoTen(UiFactory.Val(txtTen)) : loi, tenDung);

            // 3. Điểm trung bình
            double diem;
            bool diemDung = KiemTra.HopLeDiem(UiFactory.Val(txtDiem), out diem, out loi);
            ThemDong("3. Điểm trung bình [0, 10]", diemDung,
                diemDung ? string.Format("Hợp lệ - xếp loại {0}", KiemTra.XepLoai(diem)) : loi, diemDung);

            // 4. Ngày sinh
            bool ngayDung = KiemTra.HopLeNgaySinh(dtpNgaySinh.Value, out loi);
            ThemDong("4. Ngày sinh (tuổi 16 - 80)", ngayDung,
                ngayDung ? string.Format("Hợp lệ - {0} tuổi", KiemTra.TinhTuoi(dtpNgaySinh.Value)) : loi, ngayDung);

            // 5. Khoa
            bool khoaDung = cboKhoa.SelectedItem != null;
            ThemDong("5. Khoa", khoaDung, khoaDung ? "Đã chọn: " + cboKhoa.Text : "Chưa chọn khoa", khoaDung);

            tatCaDung = maDung && tenDung && diemDung && ngayDung && khoaDung;

            lblKetLuan.Text = tatCaDung
                ? "KẾT LUẬN: Dữ liệu hợp lệ - có thể lưu."
                : "KẾT LUẬN: Dữ liệu CHƯA hợp lệ - xem chi tiết các dòng đỏ.";
            lblKetLuan.ForeColor = tatCaDung ? UiTheme.Green : UiTheme.Red;
        }

        private void ThemDong(string quyTac, bool ok, string ghiChu, bool emphasis)
        {
            var item = new ListViewItem(ok ? "ĐẠT" : "LỖI");
            item.SubItems.Add(quyTac);
            item.SubItems.Add(ghiChu);
            item.ForeColor = ok ? UiTheme.Green : UiTheme.Red;
            if (emphasis) item.Font = UiTheme.FontBody(9f);
            lstKetQua.Items.Add(item);
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
