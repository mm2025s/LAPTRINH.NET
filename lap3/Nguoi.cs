using System;

namespace Lap3
{
    /// <summary>
    /// Lớp cơ sở (base class) mô tả một con người nói chung.
    /// Chứa các thuộc tính chung nhất: Họ tên, Ngày sinh, Giới tính.
    /// Được kế thừa bởi SinhVien, NguoiHoc, NhanVien...
    /// </summary>
    public class Nguoi
    {
        private string _hoTen = string.Empty;
        private DateTime _ngaySinh;

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

        public DateTime NgaySinh
        {
            get { return _ngaySinh; }
            set
            {
                if (value > DateTime.Today)
                    throw new ArgumentException("Ngày sinh không hợp lệ.");
                _ngaySinh = value;
            }
        }

        public string GioiTinh { get; set; }

        /// <summary>
        /// Constructor mặc định: tạo trạng thái hợp lệ tối thiểu.
        /// </summary>
        public Nguoi()
        {
            _hoTen = "Chưa có";
            _ngaySinh = DateTime.Today;
            GioiTinh = "Khác";
        }

        public Nguoi(string hoTen, DateTime ngaySinh)
        {
            HoTen = hoTen;
            NgaySinh = ngaySinh;
            GioiTinh = "Khác";
        }

        public Nguoi(string hoTen, DateTime ngaySinh, string gioiTinh)
        {
            HoTen = hoTen;
            NgaySinh = ngaySinh;
            GioiTinh = gioiTinh;
        }

        public int TinhTuoi()
        {
            int tuoi = DateTime.Today.Year - NgaySinh.Year;
            if (NgaySinh.Date > DateTime.Today.AddYears(-tuoi)) tuoi--;
            return tuoi;
        }

        /// <summary>
        /// Phương thức ảo (virtual): lớp con CÓ THỂ ghi đè để mô tả chi tiết hơn.
        /// Đây là nền tảng của tính đa hình (polymorphism).
        /// </summary>
        public virtual string MoTa()
        {
            return string.Format("{0} - {1:dd/MM/yyyy} - {2} tuổi", HoTen, NgaySinh, TinhTuoi());
        }
    }
}
