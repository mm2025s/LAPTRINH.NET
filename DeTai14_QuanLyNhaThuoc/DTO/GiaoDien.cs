using System.Collections.Generic;
using System.Data;

namespace QuanLyNhaThuoc.DTO
{
    /// <summary>
    /// Cac interface cua he thong (huong doi tuong):
    ///   IDoiTuong       - moi DTO co khoa chinh va mo ta ngan (dung cho nhat ky)
    ///   ICrudDAL&lt;T&gt;   - hop dong CRUD cua lop truy cap du lieu
    ///   IGhiNhatKy      - noi ghi nhat ky nguoi dung (DAL ghi vao bang NhatKy)
    ///   IGioHang&lt;T&gt;   - danh sach dong tam truoc khi luu (gio ban hang, phieu nhap)
    ///   IBaoCao         - nguon du lieu cho mot bao cao in duoc
    ///   ILamMoi         - trang giao dien co the tai lai du lieu
    /// </summary>
    public interface IDoiTuong
    {
        string KhoaChinh { get; }
        string MoTa();
    }

    public interface ICrudDAL<T> where T : IDoiTuong
    {
        DataTable TimKiem(string tuKhoa);
        int Them(T doiTuong);
        int Sua(T doiTuong);
        int Xoa(string khoaChinh);
    }

    public interface IGhiNhatKy
    {
        void Ghi(NhanVienDTO nhanVien, string tenDangNhap, string hanhDong, string doiTuong,
                 string chiTiet, bool thanhCong, int thoiGianMs);
    }

    public interface IGioHang<TDong>
    {
        IList<TDong> DanhSach { get; }
        decimal TongTien { get; }
        void XoaDong(string maThuoc);
        void LamTrong();
    }

    public interface IBaoCao
    {
        string TieuDe { get; }
        string PhuDe { get; }
        /// <summary>Cac cot can in: ten cot trong DuLieu -&gt; tieu de hien thi.</summary>
        IList<KeyValuePair<string, string>> Cot { get; }
        DataTable DuLieu { get; }
        string DongTong { get; }
    }

    public interface ILamMoi
    {
        void LamMoi();
    }

    /// <summary>Lop co so truu tuong cua moi DTO danh muc: ke thua bat buoc cai dat khoa chinh.</summary>
    public abstract class DoiTuongCoSo : IDoiTuong
    {
        public abstract string KhoaChinh { get; }
        public virtual string MoTa() { return KhoaChinh; }
        public override string ToString() { return MoTa(); }
    }
}
