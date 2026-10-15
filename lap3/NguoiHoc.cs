using System;

namespace Lap3
{
    /// <summary>
    /// Lớp TRỪU TƯỢNG (abstract) cho mọi đối tượng người học.
    /// - Không thể khởi tạo trực tiếp: new NguoiHoc() là lỗi biên dịch.
    /// - TinhHocPhi() là phương thức trừu tượng: BẮT BUỘC lớp con cài đặt,
    ///   vì mỗi loại người học có công thức học phí khác nhau.
    /// </summary>
    public abstract class NguoiHoc : IValidatable, IPrintable
    {
        private string _maNguoiHoc = string.Empty;
        private string _hoTen = string.Empty;
        private int _soTinChi;

        public string MaNguoiHoc
        {
            get { return _maNguoiHoc; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Mã người học không được rỗng.");
                _maNguoiHoc = value.Trim();
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

        public int SoTinChi
        {
            get { return _soTinChi; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("SoTinChi", "Số tín chỉ phải >= 0.");
                _soTinChi = value;
            }
        }

        /// <summary>Tên loại người học — lớp con ghi đè để phân biệt trong báo cáo.</summary>
        public abstract string LoaiNguoiHoc { get; }

        protected NguoiHoc()
        {
            _maNguoiHoc = "Chưa có";
            _hoTen = "Chưa có";
            _soTinChi = 0;
        }

        protected NguoiHoc(string ma, string hoTen, int soTinChi)
        {
            MaNguoiHoc = ma;
            HoTen = hoTen;
            SoTinChi = soTinChi;
        }

        /// <summary>
        /// Phương thức trừu tượng: mỗi loại người học tự quyết định cách tính học phí.
        /// Không có thân hàm (không có { }) — chỉ có dấu chấm phẩy.
        /// </summary>
        public abstract decimal TinhHocPhi();

        /// <summary>
        /// Phương thức ảo: dùng chung được, lớp con có thể ghi đè để in chi tiết hơn.
        /// </summary>
        public virtual string MoTa()
        {
            return string.Format("{0} | {1} | {2} tín chỉ | Học phí: {3:N0} đ",
                MaNguoiHoc, HoTen, SoTinChi, TinhHocPhi());
        }

        public bool IsValid(out string message)
        {
            if (string.IsNullOrWhiteSpace(MaNguoiHoc)) { message = "Thiếu mã người học"; return false; }
            if (string.IsNullOrWhiteSpace(HoTen)) { message = "Thiếu họ tên"; return false; }
            if (SoTinChi < 0) { message = "Số tín chỉ không hợp lệ"; return false; }
            message = string.Empty;
            return true;
        }

        public string ToDisplayText()
        {
            return string.Format("{0} - {1} ({2})", MaNguoiHoc, HoTen, LoaiNguoiHoc);
        }
    }

    /// <summary>Hệ chính quy: 450.000 đ / tín chỉ.</summary>
    public class SinhVienChinhQuy : NguoiHoc
    {
        public const decimal DonGiaTinChi = 450000m;

        public SinhVienChinhQuy() { }

        public SinhVienChinhQuy(string ma, string hoTen, int soTinChi)
            : base(ma, hoTen, soTinChi) { }

        public override string LoaiNguoiHoc { get { return "Chính quy"; } }

        public override decimal TinhHocPhi()
        {
            return SoTinChi * DonGiaTinChi;
        }
    }

    /// <summary>Hệ liên thông: 520.000 đ / tín chỉ.</summary>
    public class SinhVienLienThong : NguoiHoc
    {
        public const decimal DonGiaTinChi = 520000m;

        public SinhVienLienThong() { }

        public SinhVienLienThong(string ma, string hoTen, int soTinChi)
            : base(ma, hoTen, soTinChi) { }

        public override string LoaiNguoiHoc { get { return "Liên thông"; } }

        public override decimal TinhHocPhi()
        {
            return SoTinChi * DonGiaTinChi;
        }
    }

    /// <summary>
    /// Hệ cao đẳng nghề: 380.000 đ / tín chỉ + 300.000 đ phí thực hành.
    /// Minh họa công thức học phí phức tạp hơn ở lớp con.
    /// </summary>
    public class SinhVienCaoDang : NguoiHoc
    {
        public const decimal DonGiaTinChi = 380000m;
        public const decimal PhiThucHanh = 300000m;

        public SinhVienCaoDang() { }

        public SinhVienCaoDang(string ma, string hoTen, int soTinChi)
            : base(ma, hoTen, soTinChi) { }

        public override string LoaiNguoiHoc { get { return "Cao đẳng nghề"; } }

        public override decimal TinhHocPhi()
        {
            return SoTinChi * DonGiaTinChi + PhiThucHanh;
        }
    }
}
