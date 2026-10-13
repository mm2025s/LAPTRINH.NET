using System;

namespace Lap2
{
    public class TaiKhoanNganHang
    {
        private string _soTaiKhoan = string.Empty;
        private string _chuTaiKhoan = string.Empty;
        private double _soDu;

        public string SoTaiKhoan
        {
            get { return _soTaiKhoan; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Số tài khoản không được rỗng.");
                _soTaiKhoan = value.Trim();
            }
        }

        public string ChuTaiKhoan
        {
            get { return _chuTaiKhoan; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Chủ tài khoản không được rỗng.");
                _chuTaiKhoan = value.Trim();
            }
        }

        public double SoDu
        {
            get { return _soDu; }
            private set
            {
                if (value < 0)
                    throw new InvalidOperationException("Số dư không được âm.");
                _soDu = value;
            }
        }

        public TaiKhoanNganHang()
        {
            _soTaiKhoan = "Chưa có";
            _chuTaiKhoan = "Chưa có";
            _soDu = 0;
        }

        public TaiKhoanNganHang(string soTaiKhoan, string chuTaiKhoan, double soDuBanDau)
        {
            SoTaiKhoan = soTaiKhoan;
            ChuTaiKhoan = chuTaiKhoan;
            SoDu = soDuBanDau;
        }

        public void NapTien(double soTien)
        {
            if (soTien <= 0)
            {
                Console.WriteLine("Số tiền nạp phải > 0.");
                return;
            }
            SoDu += soTien;
            Console.WriteLine("Nạp {0:N0} thành công. Số dư: {1:N0}", soTien, SoDu);
        }

        public void RutTien(double soTien)
        {
            if (soTien <= 0)
            {
                Console.WriteLine("Số tiền rút phải > 0.");
                return;
            }
            if (soTien > SoDu)
            {
                Console.WriteLine("Số dư không đủ. Số dư hiện tại: {0:N0}", SoDu);
                return;
            }
            SoDu -= soTien;
            Console.WriteLine("Rút {0:N0} thành công. Số dư: {1:N0}", soTien, SoDu);
        }

        public void ChuyenTien(TaiKhoanNganHang nhan, double soTien)
        {
            if (nhan == null)
            {
                Console.WriteLine("Tài khoản nhận không hợp lệ.");
                return;
            }
            if (soTien <= 0)
            {
                Console.WriteLine("Số tiền chuyển phải > 0.");
                return;
            }
            if (soTien > SoDu)
            {
                Console.WriteLine("Số dư không đủ để chuyển. Số dư hiện tại: {0:N0}", SoDu);
                return;
            }
            SoDu -= soTien;
            nhan.SoDu += soTien;
            Console.WriteLine("Chuyển {0:N0} từ {1} sang {2} thành công.",
                soTien, SoTaiKhoan, nhan.SoTaiKhoan);
        }

        public void HienThi()
        {
            Console.WriteLine("TK: {0} - {1} - Số dư: {2:N0}", SoTaiKhoan, ChuTaiKhoan, SoDu);
        }
    }
}
