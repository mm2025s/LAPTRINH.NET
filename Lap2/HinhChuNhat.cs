using System;

namespace Lap2
{
    public class HinhChuNhat
    {
        private double _chieuDai;
        private double _chieuRong;

        public double ChieuDai
        {
            get { return _chieuDai; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("ChieuDai", "Chiều dài phải > 0.");
                _chieuDai = value;
            }
        }

        public double ChieuRong
        {
            get { return _chieuRong; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("ChieuRong", "Chiều rộng phải > 0.");
                _chieuRong = value;
            }
        }

        public HinhChuNhat()
        {
            _chieuDai = 1;
            _chieuRong = 1;
        }

        public HinhChuNhat(double chieuDai, double chieuRong)
        {
            ChieuDai = chieuDai;
            ChieuRong = chieuRong;
        }

        public double ChuVi()
        {
            return (_chieuDai + _chieuRong) * 2;
        }

        public double DienTich()
        {
            return _chieuDai * _chieuRong;
        }

        public void HienThi()
        {
            Console.WriteLine("HCN {0} x {1} -> Chu vi: {2:N2} - Diện tích: {3:N2}",
                ChieuDai, ChieuRong, ChuVi(), DienTich());
        }
    }
}
