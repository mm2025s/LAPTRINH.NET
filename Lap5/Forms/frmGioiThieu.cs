using System;
using System.Drawing;
using System.Windows.Forms;
using Lab05_MultiForm.Helpers;

namespace Lab05_MultiForm.Forms
{
    /// <summary>
    /// frmGioiThieu - Bài 10 (Dashboard): Form giới thiệu chương trình.
    /// Liệt kê mục tiêu Lab 5, kỹ thuật đã dùng và bản đồ phím tắt.
    /// </summary>
    public partial class frmGioiThieu : FormBase
    {
        public frmGioiThieu()
        {
            InitializeComponent();
        }

        private void frmGioiThieu_Load(object sender, EventArgs e)
        {
            KhoiTaoNoiDung();
        }

        private void KhoiTaoNoiDung()
        {
            lblTieuDe.Text = "Lab 5 · Xử lý sự kiện, kiểm tra dữ liệu và ứng dụng đa Form";
            lblPhuDe.Text = "Lập trình .NET (IT3212) · Chương 3 · Khoa Công nghệ Thông tin - ĐH Công nghệ Đông Á";

            lstMucTieu.Items.Clear();
            lstMucTieu.Items.Add("Sự kiện Click, TextChanged, KeyPress, SelectedIndexChanged, FormClosing");
            lstMucTieu.Items.Add("Kiểm tra dữ liệu bằng ErrorProvider + bộ hàm dùng chung KiemTra");
            lstMucTieu.Items.Add("Truyền dữ liệu giữa Form: constructor, thuộc tính, delegate/event");
            lstMucTieu.Items.Add("Ứng dụng nhiều Form với MenuStrip, StatusStrip, MDI");
            lstMucTieu.Items.Add("Hộp thoại tìm kiếm, chọn khoa, đăng nhập giới hạn lần thử");
            lstMucTieu.Items.Add("Xác nhận đóng ứng dụng (FormClosing) - chọn No thì không đóng");

            lstKyThuat.Items.Clear();
            lstKyThuat.Items.Add("frmMain      : MDI, menu 4 nhóm, đồng hồ, log hành động");
            lstKyThuat.Items.Add("frmSinhVien  : ValidateInput() + ErrorProvider");
            lstKyThuat.Items.Add("             + KeyPress lọc ký tự nhập điểm");
            lstKyThuat.Items.Add("             + TextChanged dùng chung 3 TextBox");
            lstKyThuat.Items.Add("frmChiTiet   : nhận SinhVien qua constructor, trả qua thuộc tính");
            lstKyThuat.Items.Add("frmTimKiem   : lọc mã/họ tên/khoa, trả KetQua + DialogResult");
            lstKyThuat.Items.Add("frmChonKhoa  : nhận List<Khoa> qua constructor");
            lstKyThuat.Items.Add("frmDangNhap  : 3 lần sai => khóa 30 giây có đếm ngược");
            lstKyThuat.Items.Add("frmDanhSach  : Form con MDI + sự kiện MoChiTiet (delegate)");

            lstPhimTat.Items.Clear();
            lstPhimTat.Items.Add("Ctrl + N   Nhập thông tin sinh viên");
            lstPhimTat.Items.Add("Ctrl + L   Danh sách sinh viên");
            lstPhimTat.Items.Add("Ctrl + F   Tìm kiếm sinh viên");
            lstPhimTat.Items.Add("Ctrl + O   Đăng nhập");
            lstPhimTat.Items.Add("Ctrl + K   Kiểm tra dữ liệu nhanh");
            lstPhimTat.Items.Add("F1         Giới thiệu chương trình");
            lstPhimTat.Items.Add("Esc        Đóng hộp thoại đang mở");

            lblKienTruc.Text =
                "Kiến trúc thư mục\r\n" +
                "  Forms/    : frmMain, frmSinhVien, frmChiTietSinhVien, frmTimKiem, frmChonKhoa,\r\n" +
                "              frmDangNhap, frmDanhSach, frmGioiThieu\r\n" +
                "  Models/   : SinhVien, Khoa (+ DuLieuMau)\r\n" +
                "  Helpers/  : KiemTra (hàm dùng chung), UiTheme, UiControls, FormBase, RoundedPictureBox\r\n\r\n" +
                "Chuỗi bài lab: Lab 4 (giao diện) → Lab 5 (sự kiện + đa Form) → Lab 6 (3 lớp + DataGridView).";
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void openLink_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://daucongnghe.edu.vn");
        }
    }
}
