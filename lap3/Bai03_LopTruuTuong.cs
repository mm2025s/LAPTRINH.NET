using System;

namespace Lap3
{
    /// <summary>
    /// BÀI 3: LỚP TRỪU TƯỢNG NguoiHoc
    /// Mục tiêu: Tạo lớp trừu tượng có phương thức TinhHocPhi(); cài đặt
    /// SinhVienChinhQuy và SinhVienLienThong với công thức học phí khác nhau.
    /// </summary>
    public static class Bai03_LopTruuTuong
    {
        public static void Run()
        {
            Console.WriteLine("--- Bài 3: Lớp trừu tượng NguoiHoc ---");
            Console.WriteLine("Không thể khởi tạo lớp trừu tượng: new NguoiHoc() => lỗi biên dịch.");
            Console.WriteLine();

            SinhVienChinhQuy cq = new SinhVienChinhQuy("NQ01", "Nguyễn An", 18);
            SinhVienLienThong lt = new SinhVienLienThong("LT01", "Lê Cường", 15);

            Console.WriteLine("Bảng học phí:");
            Console.WriteLine("{0,-8} {1,-16} {2,-16} {3,10} {4,16}",
                "Mã", "Họ tên", "Loại", "Tín chỉ", "Học phí");
            Console.WriteLine(new string('-', 72));
            Console.WriteLine("{0,-8} {1,-16} {2,-16} {3,10} {4,16:N0}",
                cq.MaNguoiHoc, cq.HoTen, cq.LoaiNguoiHoc, cq.SoTinChi, cq.TinhHocPhi());
            Console.WriteLine("{0,-8} {1,-16} {2,-16} {3,10} {4,16:N0}",
                lt.MaNguoiHoc, lt.HoTen, lt.LoaiNguoiHoc, lt.SoTinChi, lt.TinhHocPhi());
            Console.WriteLine(new string('-', 72));
            Console.WriteLine("Tổng học phí: {0:N0} đ", cq.TinhHocPhi() + lt.TinhHocPhi());

            Console.WriteLine();
            Console.WriteLine("So sánh đơn giá tín chỉ:");
            Console.WriteLine("  Chính quy  : {0:N0} đ/tín chỉ", SinhVienChinhQuy.DonGiaTinChi);
            Console.WriteLine("  Liên thông : {0:N0} đ/tín chỉ", SinhVienLienThong.DonGiaTinChi);

            Console.WriteLine();
            Console.WriteLine("Kiểm tra ràng buộc dữ liệu của lớp trừu tượng:");
            try
            {
                SinhVienChinhQuy loi = new SinhVienChinhQuy("NQ99", "Người Lạ", -5);
                Console.WriteLine(loi.MoTa());
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine("  Bắt lỗi: " + ex.Message);
            }
        }
    }
}
