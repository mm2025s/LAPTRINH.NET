using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Lab05_MultiForm.Helpers;
using Lab05_MultiForm.Models;

namespace Lab05_MultiForm.Forms
{
    /// <summary>
    /// frmDanhSach - Form con MDI.
    ///
    /// Hai cách truyền dữ liệu giữa Form được dùng ở đây (không dùng biến toàn cục):
    ///   1. CONSTRUCTOR : new frmSinhVien(sinhVien) - truyền đối tượng vào Form xem chi tiết.
    ///   2. DELEGATE/EVENT : sự kiện MoChiTiet được frmMain đăng ký,
    ///      nhờ vậy frmDanhSach không cần biết frmMain tồn tại (giảm phụ thuộc).
    ///
    /// Form cũng đăng ký nhận sự kiện SinhVienDaLuu từ frmSinhVien để tự làm mới dữ liệu.
    /// </summary>
    public partial class frmDanhSach : FormBase
    {
        /// <summary>Yêu cầu mở Form chi tiết cho một sinh viên (frmMain xử lý).</summary>
        public event EventHandler<SinhVien> MoChiTiet;

        private readonly List<SinhVien> _ds = new List<SinhVien>();
        private List<SinhVien> _hienThi = new List<SinhVien>();

        /// <summary>Dữ liệu đang hiển thị (chỉ đọc - dùng cho thống kê / kiểm thử).</summary>
        public IList<SinhVien> DuLieuDangHienThi { get { return _hienThi.AsReadOnly(); } }

        /// <summary>Chọn một đối tượng trong danh sách (dùng khi mở từ frmMain và cho trình chụp ảnh).</summary>
        public void ChonSinhVien(string maSV)
        {
            foreach (DataGridViewRow row in dgvDanhSach.Rows)
            {
                SinhVien sv = row.Tag as SinhVien;
                if (sv != null && sv.MaSV == maSV)
                {
                    row.Selected = true;
                    dgvDanhSach.CurrentCell = row.Cells["HoTen"];
                    return;
                }
            }
        }

        public frmDanhSach()
        {
            InitializeComponent();

            foreach (SinhVien sv in DuLieuMau.DanhSachSinhVien()) _ds.Add(sv);
        }

        private void frmDanhSach_Load(object sender, EventArgs e)
        {
            // Đăng ký nhận dữ liệu mới từ form nhập (nếu đang mở)
            frmMain main = MdiParent as frmMain;
            if (main != null)
            {
                foreach (Form f in main.MdiChildren)
                {
                    var nhap = f as frmSinhVien;
                    if (nhap != null)
                    {
                        nhap.SinhVienDaLuu -= Nhap_SinhVienDaLuu;
                        nhap.SinhVienDaLuu += Nhap_SinhVienDaLuu;
                    }
                }
            }

            txtTim.Focus();
            LocDuLieu();
        }

        /// <summary>Nhận dữ liệu qua sự kiện: thêm mới hoặc cập nhật danh sách rồi làm mới lưới.</summary>
        private void Nhap_SinhVienDaLuu(object sender, SinhVien sv)
        {
            if (sv == null) return;

            SinhVien cu = _ds.Find(x => x.MaSV == sv.MaSV);
            if (cu != null)
            {
                _ds.Remove(cu);
                _ds.Add(sv.Clone());
                Log("Đã cập nhật " + sv.MaSV + " từ frmSinhVien (qua sự kiện)");
            }
            else
            {
                _ds.Add(sv.Clone());
                Log("Đã thêm " + sv.MaSV + " từ frmSinhVien (qua sự kiện)");
            }

            LocDuLieu();
        }

        // =====================================================================
        // LỌC + NẠP LƯỚI
        // =====================================================================
        private void txtTim_TextChanged(object sender, EventArgs e)
        {
            LocDuLieu();
        }

        private void cboKhoa_SelectedIndexChanged(object sender, EventArgs e)
        {
            LocDuLieu();
        }

        private void LocDuLieu()
        {
            string tuKhoa = UiFactory.Val(txtTim).ToLowerInvariant();
            string khoa = cboKhoa.SelectedIndex <= 0 ? string.Empty : cboKhoa.Text;

            _hienThi = new List<SinhVien>();
            foreach (SinhVien sv in _ds)
            {
                bool khopChu = tuKhoa.Length == 0
                    || sv.MaSV.ToLowerInvariant().Contains(tuKhoa)
                    || sv.HoTen.ToLowerInvariant().Contains(tuKhoa);
                bool khopKhoa = khoa.Length == 0 || sv.Khoa == khoa;
                if (khopChu && khopKhoa) _hienThi.Add(sv);
            }

            NapLuoi();
        }

        private void NapLuoi()
        {
            dgvDanhSach.Rows.Clear();
            int i = 1;
            foreach (SinhVien sv in _hienThi)
            {
                int idx = dgvDanhSach.Rows.Add(
                    i++.ToString("00"),
                    sv.MaSV,
                    sv.HoTen,
                    sv.NgaySinh.ToString("dd/MM/yyyy"),
                    KiemTra.TinhTuoi(sv.NgaySinh),
                    sv.Khoa,
                    sv.DiemTB.ToString("0.0"),
                    sv.XepLoai());

                DataGridViewRow row = dgvDanhSach.Rows[idx];
                row.Tag = sv;

                // Tô màu theo xếp loại (dùng cột "Xếp loại" như một badge nhỏ)
                row.Cells[7].Style.ForeColor = KiemTra.MauXepLoai(sv.DiemTB);
                row.Cells[7].Style.Font = UiTheme.FontBold(9f);
                if (sv.DiemTB < 5)
                {
                    row.Cells[6].Style.ForeColor = UiTheme.Red;
                    row.Cells[6].Style.Font = UiTheme.FontBold(9.5f);
                }
            }

            lblDem.Text = string.Format("Hiển thị {0} / {1} sinh viên", _hienThi.Count, _ds.Count);
            lblDem.ForeColor = _hienThi.Count == _ds.Count ? UiTheme.Muted : UiTheme.Violet;

            lblChon.Text = dgvDanhSach.Rows.Count == 0
                ? "Không có dòng nào được chọn"
                : "Chọn dòng để xem chi tiết (bấm Đúp hoặc nút Xem chi tiết)";

            ChonDong(null);
        }

        private void dgvDanhSach_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDanhSach.CurrentRow == null) { ChonDong(null); return; }
            ChonDong(dgvDanhSach.CurrentRow.Tag as SinhVien);
        }

        private void ChonDong(SinhVien sv)
        {
            if (sv == null)
            {
                lblChonMa.Text = "—";
                lblChonTen.Text = "Chưa chọn sinh viên";
                lblChonKhoa.Text = string.Empty;
                lblChonDiem.Text = string.Empty;
                picChon.Image = UiTheme.CreateAvatarImage(string.Empty, 72);
                return;
            }

            lblChonMa.Text = sv.MaSV;
            lblChonTen.Text = sv.HoTen;
            lblChonKhoa.Text = sv.Khoa + "   ·   " + sv.GioiTinh + "   ·   " + KiemTra.TinhTuoi(sv.NgaySinh) + " tuổi";
            lblChonDiem.Text = "Điểm " + sv.DiemTB.ToString("0.0") + " - " + sv.XepLoai();
            lblChonDiem.ForeColor = KiemTra.MauXepLoai(sv.DiemTB);
            picChon.Image = UiTheme.CreateAvatarImage(sv.HoTen, 72);
        }

        private void dgvDanhSach_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) YeuCauMoChiTiet();
        }

        private void btnXemChiTiet_Click(object sender, EventArgs e)
        {
            YeuCauMoChiTiet();
        }

        private void YeuCauMoChiTiet()
        {
            if (dgvDanhSach.CurrentRow == null) return;
            SinhVien sv = dgvDanhSach.CurrentRow.Tag as SinhVien;
            if (sv == null) return;

            EventHandler<SinhVien> handler = MoChiTiet;
            if (handler != null)
            {
                handler(this, sv);
                return;
            }

            // Dự phòng khi Form được mở đứng riêng (không qua frmMain):
            // truyền dữ liệu bằng CONSTRUCTOR.
            using (var chiTiet = new frmChiTietSinhVien(sv.Clone(), "từ frmDanhSach"))
            {
                if (chiTiet.ShowDialog(this) == DialogResult.OK)
                {
                    int idx = _ds.IndexOf(sv);
                    _ds[idx] = chiTiet.SinhVien.Clone();
                    LocDuLieu();
                }
            }
        }

        private void Log(string text)
        {
            frmMain main = MdiParent as frmMain;
            if (main != null) main.Log("frmDanhSach: " + text);
        }
    }
}
