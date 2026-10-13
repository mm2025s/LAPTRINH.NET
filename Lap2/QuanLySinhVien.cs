using System;
using System.Collections.Generic;
using System.Linq;

namespace Lap2
{
    public class QuanLySinhVien
    {
        private List<SinhVien> _ds = new List<SinhVien>();

        public IReadOnlyList<SinhVien> DanhSach { get { return _ds; } }

        public bool Them(SinhVien sv)
        {
            if (sv == null) return false;
            if (TimTheoMa(sv.MaSV) != null)
            {
                Console.WriteLine("Mã sinh viên đã tồn tại: " + sv.MaSV);
                return false;
            }
            _ds.Add(sv);
            return true;
        }

        public SinhVien? TimTheoMa(string ma)
        {
            if (string.IsNullOrWhiteSpace(ma)) return null;
            return _ds.FirstOrDefault(x => x.MaSV.Equals(ma.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        public List<SinhVien> TimTheoTen(string tuKhoa)
        {
            if (string.IsNullOrWhiteSpace(tuKhoa)) return new List<SinhVien>();
            return _ds.Where(x => x.HoTen.IndexOf(tuKhoa.Trim(), StringComparison.OrdinalIgnoreCase) >= 0).ToList();
        }

        public bool Sua(string ma, SinhVien moi)
        {
            SinhVien? cu = TimTheoMa(ma);
            if (cu == null || moi == null) return false;

            cu.HoTen = moi.HoTen;
            cu.NgaySinh = moi.NgaySinh;
            cu.DiemTB = moi.DiemTB;
            return true;
        }

        public bool Xoa(string ma)
        {
            SinhVien? sv = TimTheoMa(ma);
            if (sv == null) return false;
            _ds.Remove(sv);
            return true;
        }

        public void SapXepTheoDiemGiamDan()
        {
            _ds = _ds.OrderByDescending(x => x.DiemTB).ToList();
        }

        public void SapXepTheoMa()
        {
            _ds = _ds.OrderBy(x => x.MaSV, StringComparer.OrdinalIgnoreCase).ToList();
        }

        public void HienThiDanhSach()
        {
            if (_ds.Count == 0)
            {
                Console.WriteLine("Danh sách rỗng.");
                return;
            }

            Console.WriteLine("{0,-8} {1,-20} {2,-12} {3,8} {4,10} {5,12}",
                "Mã", "Họ tên", "Ngày sinh", "Tuổi", "Điểm TB", "Xếp loại");
            Console.WriteLine(new string('-', 76));
            foreach (SinhVien sv in _ds)
            {
                Console.WriteLine("{0,-8} {1,-20} {2,-12:dd/MM/yyyy} {3,8} {4,10:N2} {5,12}",
                    sv.MaSV, sv.HoTen, sv.NgaySinh, sv.TinhTuoi(), sv.DiemTB, sv.XepLoai());
            }
        }

        public void ThongKe()
        {
            if (_ds.Count == 0)
            {
                Console.WriteLine("Danh sách rỗng, không có gì để thống kê.");
                return;
            }

            Console.WriteLine("Tổng số sinh viên: {0}", _ds.Count);
            Console.WriteLine("Điểm TB cao nhất : {0:N2} ({1})",
                _ds.Max(x => x.DiemTB), _ds.OrderByDescending(x => x.DiemTB).First().HoTen);
            Console.WriteLine("Điểm TB thấp nhất: {0:N2} ({1})",
                _ds.Min(x => x.DiemTB), _ds.OrderBy(x => x.DiemTB).First().HoTen);
            Console.WriteLine("Điểm TB toàn bộ  : {0:N2}", _ds.Average(x => x.DiemTB));

            Console.WriteLine("\nSố lượng theo xếp loại:");
            var nhom = _ds.GroupBy(x => x.XepLoai()).OrderByDescending(g => g.Key);
            foreach (var g in nhom)
            {
                Console.WriteLine("  {0,-10}: {1}", g.Key, g.Count());
            }
        }

        public void NapDuLieuMau()
        {
            Them(new SinhVien("SV01", "Nguyễn An", new DateTime(2005, 5, 12), 8.2));
            Them(new SinhVien("SV02", "Trần Bình", new DateTime(2005, 9, 3), 6.8));
            Them(new SinhVien("SV03", "Lê Cường", new DateTime(2004, 3, 15), 9.3));
            Them(new SinhVien("SV04", "Phạm Dung", new DateTime(2006, 1, 22), 4.5));
            Them(new SinhVien("SV05", "Hoàng Em", new DateTime(2005, 7, 8), 7.0));
        }
    }
}
