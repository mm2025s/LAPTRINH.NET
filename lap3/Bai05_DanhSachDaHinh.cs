using System;
using System.Collections.Generic;

namespace Lap3
{
    /// <summary>
    /// BÀI 5: DANH SÁCH ĐA HÌNH
    /// Mục tiêu: Tạo List&lt;NguoiHoc&gt; chứa nhiều kiểu sinh viên và tính tổng học phí.
    /// Gọi phương thức qua tham chiếu lớp cha để thể hiện tính đa hình.
    /// </summary>
    public static class Bai05_DanhSachDaHinh
    {
        public static void Run()
        {
            Console.WriteLine("--- Bài 5: Danh sách đa hình List<NguoiHoc> ---");
            Console.WriteLine();

            List<NguoiHoc> ds = new List<NguoiHoc>();
            ds.Add(new SinhVienChinhQuy { MaNguoiHoc = "NQ01", HoTen = "An", SoTinChi = 18 });
            ds.Add(new SinhVienLienThong { MaNguoiHoc = "LT01", HoTen = "Bình", SoTinChi = 15 });
            ds.Add(new SinhVienCaoDang { MaNguoiHoc = "CD01", HoTen = "Cường", SoTinChi = 20 });

            decimal tong = 0;
            Console.WriteLine("Chi tiết học phí từng người học:");
            foreach (NguoiHoc item in ds)
            {
                // Lời gọi phương thức ảo/trừu tượng qua tham chiếu lớp cha
                decimal hocPhi = item.TinhHocPhi();
                tong += hocPhi;
                Console.WriteLine("{0,-6} {1,-8} {2,-16} {3,16:N0} đ",
                    item.MaNguoiHoc, item.HoTen, item.LoaiNguoiHoc, hocPhi);
            }
            Console.WriteLine(new string('-', 52));
            Console.WriteLine("Tổng học phí: {0:N0} đ", tong);

            Console.WriteLine();
            Console.WriteLine("Cùng lời gọi TinhHocPhi() nhưng mỗi lớp con cho kết quả khác nhau:");
            foreach (NguoiHoc item in ds)
            {
                Console.WriteLine("  {0,-16} ({1}) -> {2:N0} đ",
                    item.HoTen, item.GetType().Name, item.TinhHocPhi());
            }

            Console.WriteLine();
            Console.WriteLine("Sắp xếp danh sách theo học phí giảm dần (vẫn qua kiểu NguoiHoc):");
            ds.Sort(delegate (NguoiHoc a, NguoiHoc b) { return b.TinhHocPhi().CompareTo(a.TinhHocPhi()); });
            foreach (NguoiHoc item in ds)
            {
                Console.WriteLine("  {0,-8} {1,16:N0} đ", item.HoTen, item.TinhHocPhi());
            }
        }
    }
}
