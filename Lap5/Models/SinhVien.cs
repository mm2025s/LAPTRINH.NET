using System;

namespace Lab05_MultiForm.Models
{
    /// <summary>
    /// Mô hình SinhVien kế thừa Lab 4, bổ sung tầng kiểm tra ở setter
    /// để đối tượng không bao giờ rơi vào trạng thái không hợp lệ.
    /// Giữ nguyên các thành viên của Lab 4 (TinhTuoi, XepLoai, ToString) để không phá vỡ tương thích.
    /// </summary>
    public class SinhVien
    {
        private string _maSV = string.Empty;
        private string _hoTen = string.Empty;
        private DateTime _ngaySinh;
        private double _diemTB;
        private string _khoa = string.Empty;

        public string MaSV
        {
            get { return _maSV; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Mã sinh viên không được rỗng.", "MaSV");
                _maSV = value.Trim();
            }
        }

        public string HoTen
        {
            get { return _hoTen; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Họ tên không được rỗng.", "HoTen");
                _hoTen = value.Trim();
            }
        }

        public DateTime NgaySinh
        {
            get { return _ngaySinh; }
            set
            {
                if (value.Date > DateTime.Today)
                    throw new ArgumentException("Ngày sinh không hợp lệ (ở tương lai).", "NgaySinh");
                _ngaySinh = value;
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

        public string GioiTinh { get; set; }
        public string Khoa { get; set; }
        public string SoThich { get; set; }
        public string AnhDaiDien { get; set; }

        public SinhVien()
        {
            _maSV = string.Empty;
            _hoTen = string.Empty;
            _ngaySinh = DateTime.Today.AddYears(-19);
            _diemTB = 0;
            GioiTinh = "Nam";
            Khoa = string.Empty;
            SoThich = string.Empty;
            AnhDaiDien = string.Empty;
        }

        public SinhVien(string maSV, string hoTen, DateTime ngaySinh, double diemTB)
        {
            GioiTinh = "Nam";
            Khoa = string.Empty;
            SoThich = string.Empty;
            AnhDaiDien = string.Empty;
            MaSV = maSV;
            HoTen = hoTen;
            NgaySinh = ngaySinh;
            DiemTB = diemTB;
        }

        public int TinhTuoi()
        {
            return Helpers.KiemTra.TinhTuoi(NgaySinh);
        }

        public string XepLoai()
        {
            return Helpers.KiemTra.XepLoai(DiemTB);
        }

        /// <summary>Bản sao độc lập - tránh Form chi tiết sửa nhầm dữ liệu gốc của Form nhập.</summary>
        public SinhVien Clone()
        {
            return new SinhVien
            {
                MaSV = MaSV,
                HoTen = HoTen,
                NgaySinh = NgaySinh,
                DiemTB = DiemTB,
                GioiTinh = GioiTinh,
                Khoa = Khoa,
                SoThich = SoThich,
                AnhDaiDien = AnhDaiDien
            };
        }

        public override string ToString()
        {
            return string.Format("{0} - {1} - {2:dd/MM/yyyy} - {3} - {4}",
                MaSV, HoTen, NgaySinh, Khoa, GioiTinh);
        }
    }
}
