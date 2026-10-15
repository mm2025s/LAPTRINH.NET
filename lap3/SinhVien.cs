using System;

namespace Lap3
{
    /// <summary>
    /// Lớp SinhVien kế thừa Nguoi (quan hệ "is-a": sinh viên LÀ một người).
    /// Đồng thời thực thi hai interface IValidatable và IPrintable.
    /// </summary>
    public class SinhVien : Nguoi, IValidatable, IPrintable, IXuatDuLieu
    {
        private string _maSV = string.Empty;
        private double _diemTB;

        public string MaSV
        {
            get { return _maSV; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Mã sinh viên không được rỗng.");
                _maSV = value.Trim();
            }
        }

        public double DiemTB
        {
            get { return _diemTB; }
            set
            {
                if (value < 0 || value > 10)
                    throw new ArgumentOutOfRangeException("DiemTB", "Điểm trung bình phải thuộc [0, 10].");
                _diemTB = value;
            }
        }

        public string Nganh { get; set; }

        /// <summary>
        /// Constructor mặc định của lớp con.
        /// Lưu ý: constructor mặc định của lớp cha luôn được gọi ngầm trước.
        /// </summary>
        public SinhVien()
        {
            MaSV = "Chưa có";
            DiemTB = 0;
            Nganh = "Chưa có";
        }

        /// <summary>
        /// Constructor đầy đủ tham số.
        /// Từ khóa base(ten, ngaySinh) gọi constructor của lớp Nguoi để
        /// khởi tạo phần dữ liệu thuộc lớp cha — tránh lặp lại code.
        /// </summary>
        public SinhVien(string ma, string ten, DateTime ngaySinh, double diem)
            : base(ten, ngaySinh)
        {
            MaSV = ma;
            DiemTB = diem;
            Nganh = "Chưa có";
        }

        public SinhVien(string ma, string ten, DateTime ngaySinh, double diem, string nganh)
            : base(ten, ngaySinh)
        {
            MaSV = ma;
            DiemTB = diem;
            Nganh = nganh;
        }

        /// <summary>
        /// Ghi đè (override) MoTa() của lớp Nguoi.
        /// Gọi base.MoTa() để TÁI SỬ DỤNG hành vi lớp cha rồi bổ sung phần riêng.
        /// </summary>
        public override string MoTa()
        {
            return base.MoTa() + " - " + MaSV + " - " + DiemTB.ToString("N2");
        }

        public string XepLoai()
        {
            if (DiemTB >= 9) return "Xuất sắc";
            if (DiemTB >= 8) return "Giỏi";
            if (DiemTB >= 6.5) return "Khá";
            if (DiemTB >= 5) return "Trung bình";
            return "Yếu";
        }

        public void HienThi()
        {
            Console.WriteLine(MoTa() + " - " + XepLoai());
        }

        /// <summary>
        /// Cài đặt hợp đồng IValidatable.
        /// Lưu ý: setter chỉ kiểm tra TỪNG trường riêng lẻ, còn IsValid() kiểm tra
        /// các quy tắc nghiệp vụ trên TOÀN BỘ đối tượng (ví dụ: tuổi nhập học,
        /// định dạng mã) — đây chính là phần mà setter không thể đảm nhiệm.
        /// </summary>
        public bool IsValid(out string message)
        {
            if (string.IsNullOrWhiteSpace(MaSV)) { message = "Thiếu mã"; return false; }
            if (string.IsNullOrWhiteSpace(HoTen)) { message = "Thiếu họ tên"; return false; }
            if (DiemTB < 0 || DiemTB > 10) { message = "Điểm sai"; return false; }

            // Quy tắc nghiệp vụ 1: mã sinh viên phải bắt đầu bằng chữ cái
            if (!char.IsLetter(MaSV[0]))
            {
                message = "Mã sinh viên phải bắt đầu bằng chữ cái";
                return false;
            }

            // Quy tắc nghiệp vụ 2: tuổi nhập học tối thiểu 15
            if (TinhTuoi() < 15)
            {
                message = "Chưa đủ tuổi nhập học (tối thiểu 15)";
                return false;
            }

            message = string.Empty;
            return true;
        }

        /// <summary>Cài đặt hợp đồng IPrintable.</summary>
        public string ToDisplayText()
        {
            return MaSV + " - " + HoTen;
        }

        /// <summary>Cài đặt hợp đồng IXuatDuLieu.</summary>
        public string XuatCsv()
        {
            return string.Join(",", MaSV, HoTen, NgaySinh.ToString("dd/MM/yyyy"), DiemTB.ToString("0.00"), XepLoai());
        }

        public string XuatJson()
        {
            return string.Format(
                "{{\"maSV\":\"{0}\",\"hoTen\":\"{1}\",\"ngaySinh\":\"{2:dd/MM/yyyy}\",\"diemTB\":{3},\"xepLoai\":\"{4}\"}}",
                MaSV, HoTen, NgaySinh, DiemTB.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture), XepLoai());
        }
    }
}
