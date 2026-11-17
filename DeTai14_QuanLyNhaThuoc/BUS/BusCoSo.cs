using System;
using System.Data;
using System.Diagnostics;
using QuanLyNhaThuoc.DAL;
using QuanLyNhaThuoc.DTO;

namespace QuanLyNhaThuoc.BUS
{
    /// <summary>
    /// Nhat ky nguoi dung. Viec ghi nhat ky KHONG duoc lam hong nghiep vu:
    /// neu bang NhatKy loi thi chi ghi vao tep logs, giao dich van thanh cong.
    /// </summary>
    public static class NhatKy
    {
        /// <summary>Noi ghi (mac dinh: bang NhatKy qua DAL). Kiem thu co the thay bang lop khac.</summary>
        public static IGhiNhatKy Noi { get; set; } = new NhatKyDAL();

        public static void Ghi(string hanhDong, string doiTuong, string chiTiet, bool thanhCong, long ms, string tenDangNhap = null)
        {
            try { Noi.Ghi(PhienDangNhap.HienTai, tenDangNhap, hanhDong, doiTuong, chiTiet, thanhCong, (int)Math.Min(ms, int.MaxValue)); }
            catch (Exception ex) { XuLyLoi.GhiNhatKy("ghi nhật ký", ex); }
        }
    }

    /// <summary>
    /// Lop co so cua moi lop nghiep vu: chay lenh qua bo xu ly loi chung, do thoi gian
    /// va ghi nhat ky cho cac thao tac thay doi du lieu.
    /// </summary>
    public abstract class BusCoSo
    {
        /// <summary>Thao tac doc: chi dich loi SQL, khong ghi nhat ky.</summary>
        protected static T Doc<T>(Func<T> lenh, string thaoTac)
        {
            return XuLyLoi.Chay(lenh, thaoTac);
        }

        /// <summary>Thao tac ghi: do thoi gian, ghi nhat ky ca khi thanh cong lan that bai.</summary>
        protected static T Ghi<T>(Func<T> lenh, string thaoTac, string doiTuong, string chiTiet)
        {
            var dongHo = Stopwatch.StartNew();
            try
            {
                T kq = XuLyLoi.Chay(lenh, thaoTac);
                NhatKy.Ghi(thaoTac, doiTuong, chiTiet, true, dongHo.ElapsedMilliseconds);
                return kq;
            }
            catch (Exception ex)
            {
                NhatKy.Ghi(thaoTac, doiTuong, chiTiet + " | " + ex.Message, false, dongHo.ElapsedMilliseconds);
                throw;
            }
        }
    }

    /// <summary>Tra cuu nhat ky va hieu nang nghiep vu (chi Quan ly).</summary>
    public class NhatKyBUS : BusCoSo
    {
        private readonly NhatKyDAL _dal = new NhatKyDAL();
        private readonly ThongKeDAL _tk = new ThongKeDAL();

        public DataTable TimKiem(DateTime tu, DateTime den, string tuKhoa)
        {
            KiemTra.YeuCauQuanLy();
            if (tu.Date > den.Date) throw new LoiNhapLieu("TuNgay", "Từ ngày phải trước đến ngày.");
            return Doc(() => _dal.Search(tu, den, tuKhoa), "tra cứu nhật ký");
        }

        public DataTable HieuNangNhanVien(DateTime tu, DateTime den)
        {
            KiemTra.YeuCauQuanLy();
            return Doc(() => _tk.HieuNangNhanVien(tu, den), "hiệu năng nhân viên");
        }

        public DataTable HieuNangThaoTac(DateTime tu, DateTime den)
        {
            KiemTra.YeuCauQuanLy();
            return Doc(() => _tk.HieuNangThaoTac(tu, den), "hiệu năng thao tác");
        }
    }
}
