using System;
using System.Drawing;
using System.Windows.Forms;
using Lab06_ThreeLayer.GUI.Helpers;

namespace Lab06_ThreeLayer.GUI
{
    /// <summary>
    /// frmKienTruc - Bài 1: giải thích mô hình 3 lớp bằng hình khối + ví dụ luồng gọi thật.
    ///
    /// Form này cũng minh hoạ luồng: người dùng bấm nút -> GUI gọi BUS -> BUS gọi DAL,
    /// và in ra kết quả để thấy rõ tầng nào làm việc gì.
    /// </summary>
    public partial class frmKienTruc : FormBase
    {
        public frmKienTruc()
        {
            InitializeComponent();
        }

        private void frmKienTruc_Load(object sender, EventArgs e)
        {
            lstLop.Items.Clear();
            lstLop.Items.Add("DTO  - SinhVien, Khoa, SanPham, DanhMuc, LoiNghiepVu");
            lstLop.Items.Add("       chỉ chứa dữ liệu, không logic, không giao diện");
            lstLop.Items.Add("");
            lstLop.Items.Add("DAL  - SinhVienDAL, SanPhamDAL, DanhMucDAL, KhoaDAL");
            lstLop.Items.Add("       chỉ lưu trữ/đọc (List trong bộ nhớ + CSV)");
            lstLop.Items.Add("       Lab 7: thay ruột bằng SqlConnection, giữ nguyên chữ ký");
            lstLop.Items.Add("");
            lstLop.Items.Add("BUS  - SinhVienBUS, SanPhamBUS, DanhMucBUS, KhoaBUS");
            lstLop.Items.Add("       kiểm tra dữ liệu, quy tắc nghiệp vụ, điều phối DAL");
            lstLop.Items.Add("       ném LoiNghiepVu(truong, thongBao) cho GUI");
            lstLop.Items.Add("");
            lstLop.Items.Add("GUI  - frmMain, frmSinhVien, frmSanPham, frmDanhMuc...");
            lstLop.Items.Add("       chỉ gọi BUS, không biết dữ liệu nằm ở đâu");

            lstLoi.Items.Clear();
            lstLoi.Items.Add("BUS ném: LoiNghiepVu(\"HoTen\", \"Họ tên không được chứa chữ số.\")");
            lstLoi.Items.Add("GUI bắt: catch (LoiNghiepVu ex)");
            lstLoi.Items.Add("     -> ErrorProvider.SetError(txtHoTen, ex.Message)");
            lstLoi.Items.Add("     -> focus đúng ô nhập, dữ liệu KHÔNG được lưu");
            lstLoi.Items.Add("");
            lstLoi.Items.Add("Nhờ vậy quy tắc chỉ viết MỘT lần ở BUS,");
            lstLoi.Items.Add("mọi Form (nhập liệu, nhập CSV, Lab 7...) dùng lại.");
        }

        /// <summary>Demo luồng gọi 3 lớp: BUS kiểm tra dữ liệu rồi mới ghi qua DAL.</summary>
        private void btnChayThu_Click(object sender, EventArgs e)
        {
            lblKetQua.Text = string.Empty;
            try
            {
                var dal = new DAL.SinhVienDAL();
                var bus = new BUS.SinhVienBUS(dal);

                var sv = new DTO.SinhVien();
                sv.MaSV = "SV99";
                sv.HoTen = "Nguyễn Văn Thử";
                sv.NgaySinh = new DateTime(2005, 6, 1);
                sv.Khoa = "Công nghệ thông tin";
                sv.DiemTB = 8.2;

                bus.Add(sv);

                // Ghi thêm một bản ghi SAI (họ tên có chữ số) để thấy BUS chặn lại
                var sai = sv.Clone();
                sai.MaSV = "SV98";
                sai.HoTen = "Nguyễn Văn 2";
                bus.Add(sai);

                lblKetQua.Text = "Không mong đợi: dữ liệu sai đã được ghi.";
                lblKetQua.ForeColor = UiTheme.Red;
            }
            catch (DTO.LoiNghiepVu ex)
            {
                lblKetQua.Text = string.Format(
                    "Luồng: GUI -> BUS.KiemTraDuLieu -> báo lỗi ở trường \"{0}\".\r\n" +
                    "Thông báo của BUS: {1}\r\n" +
                    "DAL không hề nhận bản ghi sai (không có lệnh ghi nào chạy).",
                    ex.Truong, ex.Message);
                lblKetQua.ForeColor = UiTheme.Green;
            }
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnLab7_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this,
                "Sang Lab 7 chỉ cần đổi phần ruột của tầng DAL:\r\n\r\n" +
                "  SqlConnection -> SqlCommand -> SqlDataAdapter\r\n\r\n" +
                "BUS và GUI giữ nguyên vì không lớp nào biết dữ liệu\r\n" +
                "đang nằm trong bộ nhớ hay trong SQL Server.",
                "Chuẩn bị cho Lab 7", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
