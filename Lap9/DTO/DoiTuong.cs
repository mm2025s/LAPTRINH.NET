using System;
using System.Collections.Generic;

namespace Lab09_DataReaderSearch.DTO
{
    /// <summary>
    /// Khoa - anh xa bang dbo.Khoa, dung cho ComboBox loc.
    /// </summary>
    public class Khoa
    {
        public string MaKhoa { get; set; }
        public string TenKhoa { get; set; }

        public override string ToString()
        {
            return MaKhoa + " - " + TenKhoa;
        }
    }

    /// <summary>
    /// Tieu chi tim kiem da tieu chi (bai 6).
    /// Moi thuoc tinh deu co the bo trong (null / chuoi rong) - khi do dieu kien
    /// WHERE tuong ung KHONG duoc sinh ra.
    /// </summary>
    public class TieuChiTimKiem
    {
        public string MaSV { get; set; }          // so khop gan dung
        public string HoTen { get; set; }         // so khop gan dung
        public string MaKhoa { get; set; }        // null/rong = Tat ca
        public bool? GioiTinh { get; set; }       // null = Tat ca, true = Nam, false = Nu
        public decimal? DiemTu { get; set; }
        public decimal? DiemDen { get; set; }
        public int? TuoiTu { get; set; }
        public int? TuoiDen { get; set; }

        /// <summary>Cot sap xep da duoc kiem tra thuoc danh sach cho phep (bai 7).</summary>
        public string CotSapXep { get; set; }
        public bool GiamDan { get; set; }

        public TieuChiTimKiem()
        {
            MaSV = string.Empty;
            HoTen = string.Empty;
            CotSapXep = SinhVienCot.MaSV;
        }
    }

    /// <summary>
    /// Danh sach cot hop le de sap xep (bai 7).
    ///
    /// Vi sao phai whitelist: ten cot KHONG the truyen qua SqlParameter
    /// (tham so chi thay cho GIA TRI, khong thay cho ten cot), nen buoc phai
    /// noi chuoi. Cach an toan la doi chieu voi danh sach trang duoi day roi
    /// moi ghep vao ORDER BY - nguoi dung khong bao gio gui duoc chuoi SQL.
    /// </summary>
    public static class SinhVienCot
    {
        public const string MaSV = "MaSV";
        public const string HoTen = "HoTen";
        public const string NgaySinh = "NgaySinh";
        public const string DiemTB = "DiemTB";
        public const string TenKhoa = "TenKhoa";

        /// <summary>Ten cot (trong CSDL) -> nhan hien thi cho nguoi dung.</summary>
        public static readonly Dictionary<string, string> DanhSach =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { MaSV,     "Mã sinh viên" },
                { HoTen,    "Họ tên" },
                { NgaySinh, "Ngày sinh" },
                { DiemTB,   "Điểm trung bình" },
                { TenKhoa,  "Tên khoa" }
            };

        /// <summary>
        /// Tra ve ten cot an toan: neu khong nam trong danh sach cho phep thi
        /// quay ve MaSV. Nho vay khong the chen chuoi la vao ORDER BY.
        /// </summary>
        public static string ChuanHoa(string cot)
        {
            if (string.IsNullOrEmpty(cot)) return MaSV;
            return DanhSach.ContainsKey(cot.Trim()) ? cot.Trim() : MaSV;
        }

        /// <summary>
        /// Bieu thuc ORDER BY dung trong SQL - chi ghep tu whitelist.
        ///
        /// Luon kem khoa phu sv.MaSV: neu cot chinh bi TRUNG gia tri (nhieu sinh
        /// vien cung diem, cung khoa) thi thu tu giua cac dong la khong xac dinh,
        /// va OFFSET-FETCH co the tra ve cung mot dong o 2 trang lien tiep.
        /// </summary>
        public static string BieuThucOrderBy(string cot, bool giamDan)
        {
            string c = ChuanHoa(cot);
            // TenKhoa nam o bang Khoa (alias k), cac cot con lai o SinhVien (alias sv).
            string cotSql = string.Equals(c, TenKhoa, StringComparison.OrdinalIgnoreCase)
                ? "k.TenKhoa"
                : "sv." + c;

            string chieu = giamDan ? "DESC" : "ASC";

            // Chi them khoa phu khi cot chinh KHONG phai MaSV (neu khong se lap cot
            // va SQL Server bao loi "column specified more than once in the order by list").
            if (string.Equals(c, MaSV, StringComparison.OrdinalIgnoreCase))
                return cotSql + " " + chieu;

            return cotSql + " " + chieu + ", sv.MaSV ASC";
        }
    }

    /// <summary>
    /// Mot dong du lieu doc tu SqlDataReader (bai 5: vung thong tin chi tiet).
    /// Cac truong nullable duoc giu dung kieu de phan biet "khong co du lieu"
    /// voi "gia tri 0" / "chuoi rong".
    /// </summary>
    public class SinhVienChiTiet
    {
        public string MaSV { get; set; }
        public string HoTen { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string GioiTinhText { get; set; }
        public string MaKhoa { get; set; }
        public string TenKhoa { get; set; }
        public decimal? DiemTB { get; set; }
        public string Email { get; set; }

        /// <summary>Tuoi tinh chinh xac (da tru neu chua toi sinh nhat nam nay).</summary>
        public int? Tuoi { get; set; }

        public bool CoDuLieu { get { return !string.IsNullOrEmpty(MaSV); } }
    }

    /// <summary>
    /// Ket qua mot lan truy van co phan trang (bai 4).
    ///
    /// Vi sao can ca hai: OFFSET-FETCH chi tra ve MOT trang, nhung giao dien
    /// van phai biet tong so dong de tinh tong so trang va bat/tat nut Truoc/Sau.
    /// </summary>
    public class KetQuaPhanTrang
    {
        public object Bang { get; set; }        // DataTable (dung object de DTO khong phu thuoc System.Data)
        public int TongSoDong { get; set; }
        public int ChiSoTrang { get; set; }     // bat dau tu 1
        public int KichThuocTrang { get; set; }

        public int TongSoTrang
        {
            get
            {
                if (KichThuocTrang <= 0) return 1;
                return Math.Max(1, (int)Math.Ceiling(TongSoDong / (double)KichThuocTrang));
            }
        }

        public bool CoTrangTruoc { get { return ChiSoTrang > 1; } }
        public bool CoTrangSau { get { return ChiSoTrang < TongSoTrang; } }

        /// <summary>"Trang 2/5 · 46 sinh viên" - hien thi o thanh trang thai.</summary>
        public string MoTa
        {
            get
            {
                return string.Format("Trang {0}/{1} · {2} sinh viên", ChiSoTrang, TongSoTrang, TongSoDong);
            }
        }
    }

    /// <summary>
    /// Ngoai le nghiep vu mang ten truong bi sai - GUI bat va hien thi
    /// ngay duoi o nhap tuong ung (giu quy uoc cua Lab 6, 7, 8).
    /// </summary>
    public class LoiNghiepVu : Exception
    {
        public string Truong { get; private set; }

        public LoiNghiepVu(string truong, string thongBao)
            : base(thongBao)
        {
            Truong = truong;
        }
    }
}
