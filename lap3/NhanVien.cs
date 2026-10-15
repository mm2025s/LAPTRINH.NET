using System;

namespace Lap3
{
    /// <summary>
    /// BÀI 6 (tự thực hiện): HỆ THỐNG NHÂN VIÊN
    /// Lớp trừu tượng NhanVien với phương thức trừu tượng TinhLuong().
    /// Mỗi loại nhân viên có cách tính lương khác nhau => tính lương đa hình.
    /// </summary>
    public abstract class NhanVien
    {
        private string _maNV = string.Empty;
        private string _hoTen = string.Empty;
        private double _luongCoBan;

        public string MaNV
        {
            get { return _maNV; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Mã nhân viên không được rỗng.");
                _maNV = value.Trim();
            }
        }

        public string HoTen
        {
            get { return _hoTen; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Họ tên không được rỗng.");
                _hoTen = value.Trim();
            }
        }

        public double LuongCoBan
        {
            get { return _luongCoBan; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("LuongCoBan", "Lương cơ bản phải >= 0.");
                _luongCoBan = value;
            }
        }

        public abstract string ChucDanh { get; }

        protected NhanVien()
        {
            _maNV = "Chưa có";
            _hoTen = "Chưa có";
            _luongCoBan = 0;
        }

        protected NhanVien(string maNV, string hoTen, double luongCoBan)
        {
            MaNV = maNV;
            HoTen = hoTen;
            LuongCoBan = luongCoBan;
        }

        /// <summary>Mỗi loại nhân viên tự cài đặt công thức lương riêng.</summary>
        public abstract double TinhLuong();

        public virtual string MoTa()
        {
            return string.Format("{0,-6} {1,-16} {2,-14} {3,14:N0}",
                MaNV, HoTen, ChucDanh, TinhLuong());
        }
    }

    /// <summary>Nhân viên văn phòng: Lương = LươngCoBan * HeSo + PhuCap.</summary>
    public class NhanVienVanPhong : NhanVien
    {
        public double HeSo { get; set; }
        public double PhuCap { get; set; }

        public NhanVienVanPhong()
        {
            HeSo = 1;
        }

        public NhanVienVanPhong(string maNV, string hoTen, double luongCoBan, double heSo, double phuCap)
            : base(maNV, hoTen, luongCoBan)
        {
            HeSo = heSo;
            PhuCap = phuCap;
        }

        public override string ChucDanh { get { return "Văn phòng"; } }

        public override double TinhLuong()
        {
            return LuongCoBan * HeSo + PhuCap;
        }
    }

    /// <summary>
    /// Nhân viên kinh doanh: Lương = LươngCoBan + DoanhSo * TyLeHoaHong.
    /// </summary>
    public class NhanVienKinhDoanh : NhanVien
    {
        private double _doanhSo;

        public double DoanhSo
        {
            get { return _doanhSo; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("DoanhSo", "Doanh số phải >= 0.");
                _doanhSo = value;
            }
        }

        public double TyLeHoaHong { get; set; }

        public NhanVienKinhDoanh()
        {
            TyLeHoaHong = 0.05;
        }

        public NhanVienKinhDoanh(string maNV, string hoTen, double luongCoBan, double doanhSo, double tyLe)
            : base(maNV, hoTen, luongCoBan)
        {
            DoanhSo = doanhSo;
            TyLeHoaHong = tyLe;
        }

        public override string ChucDanh { get { return "Kinh doanh"; } }

        public override double TinhLuong()
        {
            return LuongCoBan + DoanhSo * TyLeHoaHong;
        }
    }

    /// <summary>
    /// Giảng viên: Lương = LươngCoBan * HeSo + SoTiet * ThuLaoTiet.
    /// </summary>
    public class GiangVien : NhanVien
    {
        public double HeSo { get; set; }
        public int SoTiet { get; set; }
        public double ThuLaoTiet { get; set; }

        public GiangVien()
        {
            HeSo = 1;
            ThuLaoTiet = 120000;
        }

        public GiangVien(string maNV, string hoTen, double luongCoBan, double heSo, int soTiet, double thuLaoTiet)
            : base(maNV, hoTen, luongCoBan)
        {
            HeSo = heSo;
            SoTiet = soTiet;
            ThuLaoTiet = thuLaoTiet;
        }

        public override string ChucDanh { get { return "Giảng viên"; } }

        public override double TinhLuong()
        {
            return LuongCoBan * HeSo + SoTiet * ThuLaoTiet;
        }
    }
}
