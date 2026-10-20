using System;

namespace Lab04_WinFormsBasic.Models
{
    /// <summary>
    /// Lớp SinhVien - mô hình dữ liệu sinh viên (kế thừa từ Lab 2).
    /// Được mở rộng thêm thuộc tính GioiTinh, Khoa, SoThich, AnhDaiDien
    /// để phục vụ giao diện WinForms trong Lab 4.
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
                    throw new ArgumentException("Mã sinh viên không được rỗng.");
                _maSV = value.Trim();
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

        // Thuộc tính mở rộng cho Lab 4 (WinForms)
        public string GioiTinh { get; set; } = "Nam";
        public string Khoa
        {
            get { return _khoa; }
            set { _khoa = value ?? string.Empty; }
        }
        public string SoThich { get; set; } = string.Empty;
        public string AnhDaiDien { get; set; } = string.Empty;

        public SinhVien()
        {
            _maSV = "Chưa có";
            _hoTen = "Chưa có";
            _ngaySinh = DateTime.Today.AddYears(-20);
            _diemTB = 0;
        }

        public SinhVien(string maSV, string hoTen, DateTime ngaySinh, double diemTB)
        {
            MaSV = maSV;
            HoTen = hoTen;
            NgaySinh = ngaySinh;
            DiemTB = diemTB;
        }

        public int TinhTuoi()
        {
            int tuoi = DateTime.Today.Year - NgaySinh.Year;
            if (NgaySinh.Date > DateTime.Today.AddYears(-tuoi)) tuoi--;
            return tuoi;
        }

        public string XepLoai()
        {
            if (DiemTB >= 9) return "Xuất sắc";
            if (DiemTB >= 8) return "Giỏi";
            if (DiemTB >= 6.5) return "Khá";
            if (DiemTB >= 5) return "Trung bình";
            return "Yếu";
        }

        public override string ToString()
        {
            return string.Format("{0} - {1} - {2:dd/MM/yyyy} - {3} - {4}",
                MaSV, HoTen, NgaySinh, Khoa, GioiTinh);
        }
    }
}
