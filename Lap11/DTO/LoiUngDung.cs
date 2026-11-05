using System;

namespace Lab11_CRUD_StoredProcedure.DTO
{
    /// <summary>Loi nhap lieu: phat hien duoc ma khong can toi CSDL.</summary>
    public class LoiNhapLieu : ArgumentException
    {
        public LoiNhapLieu(string thongBao) : base(thongBao) { }
    }

    /// <summary>Loi nghiep vu: vi pham quy tac (THROW 5000x, trung UNIQUE, FK...).</summary>
    public class LoiNghiepVu : Exception
    {
        public int MaLoi { get; private set; }

        public LoiNghiepVu(string thongBao, int maLoi) : base(thongBao)
        {
            MaLoi = maLoi;
        }
    }

    /// <summary>Loi SQL: su co ky thuat. Message la cau than thien, chi tiet nam trong nhat ky.</summary>
    public class LoiCsdl : Exception
    {
        public int SoLoi { get; private set; }
        public string MaThamChieu { get; private set; }

        public LoiCsdl(string thongBao, int soLoi, string maThamChieu, Exception goc)
            : base(thongBao, goc)
        {
            SoLoi = soLoi;
            MaThamChieu = maThamChieu;
        }
    }
}
