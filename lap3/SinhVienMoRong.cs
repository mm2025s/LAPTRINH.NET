using System;
using System.Collections.Generic;

namespace Lap3
{
    /// <summary>
    /// BÀI 10 (tự thực hiện): NÂNG CẤP QUẢN LÝ SINH VIÊN
    /// Lớp trừu tượng mở rộng SinhVien, bổ sung loại sinh viên và cách tính học phí.
    /// Kế thừa SinhVien => tái sử dụng toàn bộ kiểm tra dữ liệu và interface đã có.
    /// </summary>
    public abstract class SinhVienMoRong : SinhVien
    {
        protected SinhVienMoRong() : base() { }

        protected SinhVienMoRong(string ma, string ten, DateTime ngaySinh, double diem, string nganh)
            : base(ma, ten, ngaySinh, diem, nganh) { }

        /// <summary>Loại sinh viên — dùng cho tìm kiếm và thống kê theo loại.</summary>
        public abstract string LoaiSinhVien { get; }

        /// <summary>Học phí mỗi tín chỉ của từng loại sinh viên.</summary>
        public abstract decimal DonGiaTinChi { get; }

        public int SoTinChi { get; set; }

        public decimal TinhHocPhi()
        {
            return SoTinChi * DonGiaTinChi;
        }

        /// <summary>Ghi đè MoTa() để bổ sung thông tin loại sinh viên.</summary>
        public override string MoTa()
        {
            return base.MoTa() + " - " + LoaiSinhVien;
        }
    }

    /// <summary>Sinh viên chính quy: 450.000 đ/tín chỉ.</summary>
    public class SvChinhQuy : SinhVienMoRong
    {
        public SvChinhQuy() : base() { }
        public SvChinhQuy(string ma, string ten, DateTime ngaySinh, double diem, string nganh, int soTinChi)
            : base(ma, ten, ngaySinh, diem, nganh) { SoTinChi = soTinChi; }

        public override string LoaiSinhVien { get { return "Chính quy"; } }
        public override decimal DonGiaTinChi { get { return 450000m; } }
    }

    /// <summary>Sinh viên liên thông: 520.000 đ/tín chỉ.</summary>
    public class SvLienThong : SinhVienMoRong
    {
        public SvLienThong() : base() { }
        public SvLienThong(string ma, string ten, DateTime ngaySinh, double diem, string nganh, int soTinChi)
            : base(ma, ten, ngaySinh, diem, nganh) { SoTinChi = soTinChi; }

        public override string LoaiSinhVien { get { return "Liên thông"; } }
        public override decimal DonGiaTinChi { get { return 520000m; } }
    }

    /// <summary>Sinh viên vừa làm vừa học: 380.000 đ/tín chỉ.</summary>
    public class SvVuaLamVuaHoc : SinhVienMoRong
    {
        public SvVuaLamVuaHoc() : base() { }
        public SvVuaLamVuaHoc(string ma, string ten, DateTime ngaySinh, double diem, string nganh, int soTinChi)
            : base(ma, ten, ngaySinh, diem, nganh) { SoTinChi = soTinChi; }

        public override string LoaiSinhVien { get { return "Vừa làm vừa học"; } }
        public override decimal DonGiaTinChi { get { return 380000m; } }
    }

    /// <summary>
    /// Tầng nghiệp vụ nâng cao: quản lý danh sách đa hình, tìm kiếm,
    /// thống kê theo loại và xuất dữ liệu qua interface IXuatDuLieu.
    /// </summary>
    public class QuanLySinhVienNangCao
    {
        private readonly List<SinhVienMoRong> _ds = new List<SinhVienMoRong>();

        public int SoLuong { get { return _ds.Count; } }

        public void Them(SinhVienMoRong sv)
        {
            if (sv == null) throw new ArgumentNullException("sv");
            _ds.Add(sv);
        }

        public SinhVienMoRong? TimTheoMa(string ma)
        {
            foreach (SinhVienMoRong sv in _ds)
            {
                if (sv.MaSV.Equals(ma, StringComparison.OrdinalIgnoreCase)) return sv;
            }
            return null;
        }

        public List<SinhVienMoRong> TimTheoTen(string tuKhoa)
        {
            List<SinhVienMoRong> kq = new List<SinhVienMoRong>();
            foreach (SinhVienMoRong sv in _ds)
            {
                if (sv.HoTen.IndexOf(tuKhoa, StringComparison.OrdinalIgnoreCase) >= 0) kq.Add(sv);
            }
            return kq;
        }

        public List<SinhVienMoRong> LocTheoLoai(string loai)
        {
            List<SinhVienMoRong> kq = new List<SinhVienMoRong>();
            foreach (SinhVienMoRong sv in _ds)
            {
                if (sv.LoaiSinhVien.Equals(loai, StringComparison.OrdinalIgnoreCase)) kq.Add(sv);
            }
            return kq;
        }

        public void SapXepTheoDiem()
        {
            _ds.Sort(delegate (SinhVienMoRong a, SinhVienMoRong b)
            {
                return b.DiemTB.CompareTo(a.DiemTB);
            });
        }

        /// <summary>Thống kê số lượng và học phí theo từng loại sinh viên.</summary>
        public Dictionary<string, int> ThongKeSoLuongTheoLoai()
        {
            Dictionary<string, int> kq = new Dictionary<string, int>();
            foreach (SinhVienMoRong sv in _ds)
            {
                if (!kq.ContainsKey(sv.LoaiSinhVien)) kq[sv.LoaiSinhVien] = 0;
                kq[sv.LoaiSinhVien]++;
            }
            return kq;
        }

        public decimal TongHocPhi()
        {
            decimal tong = 0;
            foreach (SinhVienMoRong sv in _ds) tong += sv.TinhHocPhi();
            return tong;
        }

        public void InDanhSach()
        {
            Console.WriteLine("{0,-7} {1,-14} {2,-14} {3,6} {4,8} {5,16} {6,-10}",
                "Mã", "Họ tên", "Loại", "Tuổi", "Điểm", "Học phí", "Xếp loại");
            Console.WriteLine(new string('-', 84));
            foreach (SinhVienMoRong sv in _ds)
            {
                Console.WriteLine("{0,-7} {1,-14} {2,-14} {3,6} {4,8:N2} {5,16:N0} {6,-10}",
                    sv.MaSV, sv.HoTen, sv.LoaiSinhVien, sv.TinhTuoi(), sv.DiemTB, sv.TinhHocPhi(), sv.XepLoai());
            }
        }

        /// <summary>Xuất toàn bộ danh sách ra CSV — gọi qua hợp đồng IXuatDuLieu.</summary>
        public string XuatCsv()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("MaSV,HoTen,NgaySinh,DiemTB,XepLoai,LoaiSinhVien,SoTinChi,HocPhi");
            foreach (SinhVienMoRong sv in _ds)
            {
                IXuatDuLieu x = sv;
                sb.AppendLine(x.XuatCsv() + "," + sv.LoaiSinhVien + "," + sv.SoTinChi + "," + sv.TinhHocPhi());
            }
            return sb.ToString();
        }

        /// <summary>Xuất toàn bộ danh sách ra JSON — gọi qua hợp đồng IXuatDuLieu.</summary>
        public string XuatJson()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("[");
            for (int i = 0; i < _ds.Count; i++)
            {
                IXuatDuLieu x = _ds[i];
                sb.Append("  " + x.XuatJson());
                if (i < _ds.Count - 1) sb.Append(",");
                sb.AppendLine();
            }
            sb.AppendLine("]");
            return sb.ToString();
        }

        public static QuanLySinhVienNangCao TaoDuLieuMau()
        {
            QuanLySinhVienNangCao ql = new QuanLySinhVienNangCao();
            ql.Them(new SvChinhQuy("SV01", "Nguyễn An", new DateTime(2005, 5, 12), 8.2, "CNTT", 18));
            ql.Them(new SvChinhQuy("SV02", "Trần Bình", new DateTime(2005, 9, 3), 6.8, "CNTT", 21));
            ql.Them(new SvLienThong("LT01", "Lê Cường", new DateTime(2004, 3, 15), 9.3, "KTPM", 15));
            ql.Them(new SvLienThong("LT02", "Phạm Dung", new DateTime(2006, 1, 22), 4.5, "KTPM", 12));
            ql.Them(new SvVuaLamVuaHoc("VL01", "Hoàng Em", new DateTime(2003, 7, 8), 7.0, "HTTT", 20));
            return ql;
        }
    }
}
