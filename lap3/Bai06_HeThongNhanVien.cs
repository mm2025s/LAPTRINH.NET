using System;
using System.Collections.Generic;

namespace Lap3
{
    /// <summary>
    /// BÀI 6: HỆ THỐNG NHÂN VIÊN (tự thực hiện)
    /// Tính lương đa hình cho 3 loại nhân viên và in bảng lương.
    /// </summary>
    public static class Bai06_HeThongNhanVien
    {
        public static void Run()
        {
            Console.WriteLine("--- Bài 6: Hệ thống nhân viên (tính lương đa hình) ---");
            Console.WriteLine();

            List<NhanVien> ds = new List<NhanVien>();
            ds.Add(new NhanVienVanPhong("VP01", "Nguyễn An", 5000000, 2.5, 800000));
            ds.Add(new NhanVienVanPhong("VP02", "Trần Bình", 5000000, 1.8, 500000));
            ds.Add(new NhanVienKinhDoanh("KD01", "Lê Cường", 4500000, 120000000, 0.05));
            ds.Add(new NhanVienKinhDoanh("KD02", "Phạm Dung", 4500000, 80000000, 0.07));
            ds.Add(new GiangVien("GV01", "Hoàng Em", 6000000, 2.2, 60, 120000));
            ds.Add(new GiangVien("GV02", "Vũ Phúc", 6500000, 2.6, 45, 150000));

            Console.WriteLine("{0,-6} {1,-16} {2,-14} {3,14}", "Mã", "Họ tên", "Chức danh", "Lương");
            Console.WriteLine(new string('-', 56));

            double tongQuyLuong = 0;
            foreach (NhanVien nv in ds)
            {
                // Gọi TinhLuong() qua tham chiếu lớp trừu tượng => đa hình
                Console.WriteLine(nv.MoTa());
                tongQuyLuong += nv.TinhLuong();
            }

            Console.WriteLine(new string('-', 56));
            Console.WriteLine("Tổng quỹ lương: {0:N0} đ", tongQuyLuong);

            Console.WriteLine();
            Console.WriteLine("Thống kê theo chức danh:");
            Dictionary<string, double> theLoai = new Dictionary<string, double>();
            foreach (NhanVien nv in ds)
            {
                if (!theLoai.ContainsKey(nv.ChucDanh)) theLoai[nv.ChucDanh] = 0;
                theLoai[nv.ChucDanh] += nv.TinhLuong();
            }
            foreach (KeyValuePair<string, double> kv in theLoai)
            {
                Console.WriteLine("  {0,-14}: {1,14:N0} đ", kv.Key, kv.Value);
            }
        }
    }
}
