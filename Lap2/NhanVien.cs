using System;

namespace Lap2
{
    public class NhanVien
    {
        private string _maNV = string.Empty;
        private string _hoTen = string.Empty;
        private double _luongCoBan;
        private double _heSo;
        private double _phuCap;

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

        public double HeSo
        {
            get { return _heSo; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("HeSo", "Hệ số phải > 0.");
                _heSo = value;
            }
        }

        public double PhuCap
        {
            get { return _phuCap; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("PhuCap", "Phụ cấp phải >= 0.");
                _phuCap = value;
            }
        }

        public NhanVien()
        {
            _maNV = "Chưa có";
            _hoTen = "Chưa có";
            _luongCoBan = 0;
            _heSo = 1;
            _phuCap = 0;
        }

        public NhanVien(string maNV, string hoTen, double luongCoBan, double heSo, double phuCap)
        {
            MaNV = maNV;
            HoTen = hoTen;
            LuongCoBan = luongCoBan;
            HeSo = heSo;
            PhuCap = phuCap;
        }

        public double TinhLuong()
        {
            return LuongCoBan * HeSo + PhuCap;
        }

        public void HienThi()
        {
            Console.WriteLine("{0} - {1} - LCB: {2:N0} - HS: {3} - PC: {4:N0} - Lương: {5:N0}",
                MaNV, HoTen, LuongCoBan, HeSo, PhuCap, TinhLuong());
        }
    }
}
