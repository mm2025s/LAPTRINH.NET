using System;

namespace QuanLyNhaThuoc.DTO
{
    /// <summary>Loi nhap lieu: phat hien ma khong can toi CSDL.</summary>
    public class LoiNhapLieu : ArgumentException
    {
        public string Truong { get; private set; }

        public LoiNhapLieu(string truong, string thongBao) : base(thongBao)
        {
            Truong = truong;
        }
    }

    /// <summary>Loi nghiep vu: vi pham quy tac (THROW 51xxx, trung khoa, phan quyen...).</summary>
    public class LoiNghiepVu : Exception
    {
        public int MaLoi { get; private set; }

        public LoiNghiepVu(string thongBao, int maLoi = 0) : base(thongBao)
        {
            MaLoi = maLoi;
        }
    }

    /// <summary>Loi ky thuat: cau than thien + ma tham chieu; chi tiet nam trong nhat ky.</summary>
    public class LoiCsdl : Exception
    {
        public string MaThamChieu { get; private set; }

        public LoiCsdl(string thongBao, string maThamChieu, Exception goc) : base(thongBao, goc)
        {
            MaThamChieu = maThamChieu;
        }
    }
}
