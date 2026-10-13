using System;

namespace Lap2
{
    public class SinhVien
    {
        private string _maSV = string.Empty;
        private string _hoTen = string.Empty;
        private DateTime _ngaySinh;
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

        public SinhVien()
        {
            _maSV = "Chưa có";
            _hoTen = "Chưa có";
            _ngaySinh = DateTime.Today;
            _diemTB = 0;
        }

        public SinhVien(string maSV, string hoTen, DateTime ngaySinh, double diemTB)
        {
            MaSV = maSV;
            HoTen = hoTen;
            NgaySinh = ngaySinh;
            DiemTB = diemTB;
        }

        public void HienThi()
        {
            Console.WriteLine("{0} - {1} - {2:dd/MM/yyyy} - {3:N2} - {4} tuổi - {5}",
                MaSV, HoTen, NgaySinh, DiemTB, TinhTuoi(), XepLoai());
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
    }
}
