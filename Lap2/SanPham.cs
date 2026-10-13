using System;

namespace Lap2
{
    public class SanPham
    {
        private string _maSP = string.Empty;
        private string _tenSP = string.Empty;
        private double _donGia;
        private int _soLuong;

        public string MaSP
        {
            get { return _maSP; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Mã sản phẩm không được rỗng.");
                _maSP = value.Trim();
            }
        }

        public string TenSP
        {
            get { return _tenSP; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Tên sản phẩm không được rỗng.");
                _tenSP = value.Trim();
            }
        }

        public double DonGia
        {
            get { return _donGia; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("DonGia", "Đơn giá phải >= 0.");
                _donGia = value;
            }
        }

        public int SoLuong
        {
            get { return _soLuong; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("SoLuong", "Số lượng phải >= 0.");
                _soLuong = value;
            }
        }

        public SanPham()
        {
            _maSP = "Chưa có";
            _tenSP = "Chưa có";
            _donGia = 0;
            _soLuong = 0;
        }

        public SanPham(string maSP, string tenSP, double donGia, int soLuong)
        {
            MaSP = maSP;
            TenSP = tenSP;
            DonGia = donGia;
            SoLuong = soLuong;
        }

        public double ThanhTien()
        {
            return DonGia * SoLuong;
        }

        public void HienThi()
        {
            Console.WriteLine("{0} - {1} - Đơn giá: {2:N0} - SL: {3} - Thành tiền: {4:N0}",
                MaSP, TenSP, DonGia, SoLuong, ThanhTien());
        }
    }
}
