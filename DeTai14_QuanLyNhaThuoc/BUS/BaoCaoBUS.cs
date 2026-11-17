using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using QuanLyNhaThuoc.DTO;

namespace QuanLyNhaThuoc.BUS
{
    /// <summary>Lop co so cho cac bao cao in: luu tieu de, cot va du lieu (cai dat IBaoCao).</summary>
    public abstract class BaoCaoCoSo : IBaoCao
    {
        public string TieuDe { get; protected set; }
        public string PhuDe { get; protected set; }
        public IList<KeyValuePair<string, string>> Cot { get; protected set; }
        public DataTable DuLieu { get; protected set; }
        public abstract string DongTong { get; }

        protected static List<KeyValuePair<string, string>> DanhSachCot(params string[] tenVaTieuDe)
        {
            var ds = new List<KeyValuePair<string, string>>();
            for (int i = 0; i + 1 < tenVaTieuDe.Length; i += 2)
                ds.Add(new KeyValuePair<string, string>(tenVaTieuDe[i], tenVaTieuDe[i + 1]));
            return ds;
        }
    }

    /// <summary>Hoa don ban le de in cho khach.</summary>
    public class BaoCaoHoaDon : BaoCaoCoSo
    {
        private readonly decimal _tong;

        public BaoCaoHoaDon(DataRow hoaDon, DataTable chiTiet)
        {
            TieuDe = "HÓA ĐƠN BÁN LẺ SỐ " + hoaDon["MaHD"];
            PhuDe = "Ngày " + Convert.ToDateTime(hoaDon["NgayLap"]).ToString("dd/MM/yyyy HH:mm")
                    + " · Thu ngân: " + LayChuoi(hoaDon, "NhanVien") + " · Khách: " + (LayChuoi(hoaDon, "KhachHang") ?? "Khách lẻ");
            Cot = DanhSachCot("TenThuoc", "Tên thuốc", "DonViTinh", "ĐVT", "SoLuong", "SL", "DonGia", "Đơn giá", "ThanhTien", "Thành tiền");
            DuLieu = chiTiet;
            _tong = chiTiet.AsEnumerable().Sum(r => Convert.ToDecimal(r["ThanhTien"]));
        }

        public override string DongTong { get { return "Tổng cộng: " + _tong.ToString("N0") + " đ"; } }

        private static string LayChuoi(DataRow r, string cot)
        {
            return r.Table.Columns.Contains(cot) && r[cot] != DBNull.Value ? Convert.ToString(r[cot]) : null;
        }
    }

    /// <summary>Bao cao doanh thu theo ngay trong mot khoang thoi gian.</summary>
    public class BaoCaoDoanhThu : BaoCaoCoSo
    {
        private readonly decimal _tong;
        private readonly int _soHD;

        public BaoCaoDoanhThu(DateTime tu, DateTime den, DataTable doanhThu)
        {
            TieuDe = "BÁO CÁO DOANH THU";
            PhuDe = "Từ " + tu.ToString("dd/MM/yyyy") + " đến " + den.ToString("dd/MM/yyyy");
            Cot = DanhSachCot("Ngay", "Ngày", "SoHoaDon", "Số hóa đơn", "DoanhThu", "Doanh thu (đ)");
            DuLieu = doanhThu;
            _tong = doanhThu.AsEnumerable().Sum(r => r.Field<decimal>("DoanhThu"));
            _soHD = doanhThu.AsEnumerable().Sum(r => r.Field<int>("SoHoaDon"));
        }

        public override string DongTong { get { return "Tổng: " + _soHD + " hóa đơn · " + _tong.ToString("N0") + " đ"; } }
    }
}
