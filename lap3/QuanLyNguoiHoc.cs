using System;
using System.Collections.Generic;

namespace Lap3
{
    /// <summary>
    /// Lớp nghiệp vụ quản lý danh sách người học một cách ĐA HÌNH.
    /// Danh sách khai báo theo kiểu lớp cha List&lt;NguoiHoc&gt; nhưng có thể
    /// chứa mọi lớp con (SinhVienChinhQuy, SinhVienLienThong, SinhVienCaoDang).
    /// </summary>
    public class QuanLyNguoiHoc
    {
        private readonly List<NguoiHoc> _ds = new List<NguoiHoc>();

        public int SoLuong { get { return _ds.Count; } }

        public void Them(NguoiHoc nh)
        {
            if (nh == null) throw new ArgumentNullException("nh");
            _ds.Add(nh);
        }

        public void ThemDanhSach(IEnumerable<NguoiHoc> ds)
        {
            foreach (NguoiHoc nh in ds) Them(nh);
        }

        /// <summary>
        /// Tổng học phí tính qua tham chiếu lớp cha => thể hiện đa hình tại thời điểm chạy.
        /// </summary>
        public decimal TongHocPhi()
        {
            decimal tong = 0;
            foreach (NguoiHoc nh in _ds) tong += nh.TinhHocPhi();
            return tong;
        }

        public void InDanhSach()
        {
            Console.WriteLine("{0,-8} {1,-20} {2,-16} {3,10} {4,16}",
                "Mã", "Họ tên", "Loại", "Tín chỉ", "Học phí");
            Console.WriteLine(new string('-', 76));
            foreach (NguoiHoc nh in _ds)
            {
                Console.WriteLine("{0,-8} {1,-20} {2,-16} {3,10} {4,16:N0}",
                    nh.MaNguoiHoc, nh.HoTen, nh.LoaiNguoiHoc, nh.SoTinChi, nh.TinhHocPhi());
            }
        }

        /// <summary>
        /// Thống kê số lượng người học theo từng loại — dùng so khớp kiểu động (runtime type).
        /// </summary>
        public Dictionary<string, int> ThongKeTheoLoai()
        {
            Dictionary<string, int> ketQua = new Dictionary<string, int>();
            foreach (NguoiHoc nh in _ds)
            {
                string loai = nh.LoaiNguoiHoc;
                if (ketQua.ContainsKey(loai)) ketQua[loai]++;
                else ketQua[loai] = 1;
            }
            return ketQua;
        }

        public List<NguoiHoc> TimTheoLoai(string loai)
        {
            List<NguoiHoc> ketQua = new List<NguoiHoc>();
            foreach (NguoiHoc nh in _ds)
            {
                if (nh.LoaiNguoiHoc.Equals(loai, StringComparison.OrdinalIgnoreCase))
                    ketQua.Add(nh);
            }
            return ketQua;
        }

        public List<NguoiHoc> TimTheoHoTen(string tuKhoa)
        {
            List<NguoiHoc> ketQua = new List<NguoiHoc>();
            foreach (NguoiHoc nh in _ds)
            {
                if (nh.HoTen.IndexOf(tuKhoa, StringComparison.OrdinalIgnoreCase) >= 0)
                    ketQua.Add(nh);
            }
            return ketQua;
        }

        /// <summary>Dữ liệu mẫu dùng chung cho nhiều bài tập.</summary>
        public static QuanLyNguoiHoc TaoDuLieuMau()
        {
            QuanLyNguoiHoc ql = new QuanLyNguoiHoc();
            ql.Them(new SinhVienChinhQuy("NQ01", "Nguyễn An", 18));
            ql.Them(new SinhVienChinhQuy("NQ02", "Trần Bình", 21));
            ql.Them(new SinhVienLienThong("LT01", "Lê Cường", 15));
            ql.Them(new SinhVienLienThong("LT02", "Phạm Dung", 12));
            ql.Them(new SinhVienCaoDang("CD01", "Hoàng Em", 20));
            return ql;
        }
    }
}
