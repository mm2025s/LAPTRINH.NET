using System;
using System.Drawing;
using System.Windows.Forms;
using Lab06_ThreeLayer.GUI.Helpers;

namespace Lab06_ThreeLayer.GUI
{
    /// <summary>
    /// frmGioiThieu - Form trợ giúp: mục tiêu Lab 6, bản đồ tính năng, phím tắt, cấu trúc thư mục.
    /// </summary>
    public partial class frmGioiThieu : FormBase
    {
        public frmGioiThieu()
        {
            InitializeComponent();
        }

        private void frmGioiThieu_Load(object sender, EventArgs e)
        {
            lstMucTieu.Items.Clear();
            lstMucTieu.Items.Add("Bài 1: tổ chức project 3 lớp DTO - DAL - BUS - GUI");
            lstMucTieu.Items.Add("Bài 2: hiển thị dữ liệu bằng DataGridView + BindingSource");
            lstMucTieu.Items.Add("Bài 3: thêm / sửa / xóa, kiểm tra dữ liệu ở tầng BUS");
            lstMucTieu.Items.Add("Bài 4: TreeView khoa - sinh viên (nhóm dữ liệu)");
            lstMucTieu.Items.Add("Bài 5: tìm kiếm và sắp xếp không đụng dữ liệu gốc");
            lstMucTieu.Items.Add("Bài 6: Dashboard tổng hợp, tổng giá trị hàng tồn");
            lstMucTieu.Items.Add("Bài 7: TreeView danh mục - sản phẩm + ContextMenuStrip");
            lstMucTieu.Items.Add("Bài 8: ListView + ImageList hiển thị biểu tượng");
            lstMucTieu.Items.Add("Bài 9: nhập / xuất CSV qua BUS -> DAL");
            lstMucTieu.Items.Add("Bài 10: giữ xác nhận thoát + thống kê từ tầng BUS");

            lstTinhNang.Items.Clear();
            lstTinhNang.Items.Add("frmMain              MDI + MenuStrip + StatusStrip + Dashboard");
            lstTinhNang.Items.Add("frmSinhVien          DataGridView, CRUD, lọc, sắp xếp, CSV");
            lstTinhNang.Items.Add("                     ErrorProvider nhận LoiNghiepVu từ BUS");
            lstTinhNang.Items.Add("frmSanPham           tổng giá trị tồn = Σ(đơn giá × số lượng)");
            lstTinhNang.Items.Add("frmDanhMuc           TreeView + ContextMenuStrip (thêm/xóa)");
            lstTinhNang.Items.Add("frmDanhMucSinhVien   TreeView khoa - sinh viên, xem chi tiết");
            lstTinhNang.Items.Add("frmSinhVienIcon      ListView 3 chế độ + ImageList tự vẽ");
            lstTinhNang.Items.Add("frmKienTruc          sơ đồ 3 lớp + chạy thử luồng gọi");

            lstPhimTat.Items.Clear();
            lstPhimTat.Items.Add("Ctrl + N   Quản lý sinh viên");
            lstPhimTat.Items.Add("Ctrl + D   Quản lý sản phẩm");
            lstPhimTat.Items.Add("Ctrl + M   Danh mục sản phẩm");
            lstPhimTat.Items.Add("Ctrl + I   Sinh viên có biểu tượng");
            lstPhimTat.Items.Add("Ctrl + K   Kiến trúc 3 lớp");
            lstPhimTat.Items.Add("Ctrl + S   Lưu (trong form sinh viên)");
            lstPhimTat.Items.Add("Esc        Xóa trắng ô nhập (form sinh viên)");
            lstPhimTat.Items.Add("F1         Giới thiệu chương trình");

            lblKienTruc.Text =
                "Cấu trúc thư mục (Lab6)\r\n" +
                "  DTO/     DoiTuong.cs        SinhVien, Khoa, SanPham, DanhMuc, LoiNghiepVu\r\n" +
                "  DAL/     DuLieuGiaLap.cs    lưu/đọc dữ liệu (List + CSV)\r\n" +
                "  BUS/     NghiepVu.cs        kiểm tra + quy tắc nghiệp vụ\r\n" +
                "  GUI/     frmMain, frmSinhVien, frmSanPham, frmDanhMuc,\r\n" +
                "           frmDanhMucSinhVien, frmSinhVienIcon, frmKienTruc, frmGioiThieu\r\n" +
                "  GUI/Helpers/  UiTheme, UiControls, FormBase, KiemTraChung, NhapLieu, DinhDang\r\n\r\n" +
                "Chiều phụ thuộc: GUI -> BUS -> DAL -> dữ liệu\r\n" +
                "GUI KHÔNG được dùng DAL; DAL không biết BUS/GUI.";
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
