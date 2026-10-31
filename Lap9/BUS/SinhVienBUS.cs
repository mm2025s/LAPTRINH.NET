using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Lab09_DataReaderSearch.DAL;
using Lab09_DataReaderSearch.DTO;

namespace Lab09_DataReaderSearch.BUS
{
    /// <summary>
    /// Tang nghiep vu cua Lab 9.
    ///
    /// Vai tro o bai nay:
    ///   - Kiem tra du lieu nguoi dung nhap TRUOC khi xuong DAL (diem 0..10,
    ///     khoang diem khong bi nguoc, khoang tuoi hop le...).
    ///   - Chuan hoa bo loc: bo trong = khong loc.
    ///   - Doi loi SQL Server thanh thong bao tieng Viet de GUI chi viec hien thi.
    ///
    /// GUI khong bao gio goi DAL truc tiep, cung khong chua mot dong SQL nao.
    /// </summary>
    public class SinhVienBUS
    {
        private readonly SinhVienDAL _dal = new SinhVienDAL();

        public const int KichThuocTrangMacDinh = 10;

        // =====================================================================
        // BAI 1 - Nap danh sach
        // =====================================================================

        public DataTable LoadStudents()
        {
            return XuLy(() => _dal.LoadStudents(), "nạp danh sách sinh viên");
        }

        public DataTable NapThuCong(out int soDong)
        {
            int dem = 0;
            DataTable t = XuLy(() => _dal.NapThuCong(out dem), "nạp danh sách sinh viên (thủ công)");
            soDong = dem;
            return t;
        }

        // =====================================================================
        // BAI 2 - Tim theo ho ten
        // =====================================================================

        public DataTable SearchByName(string keyword)
        {
            if (string.IsNullOrEmpty((keyword ?? string.Empty).Trim()))
                throw new LoiNghiepVu("HoTen", "Hãy nhập từ khóa cần tìm (tối thiểu 1 ký tự).");

            return XuLy(() => _dal.SearchByName(keyword), "tìm theo họ tên");
        }

        // =====================================================================
        // BAI 3 - Loc theo khoa va khoang diem
        // =====================================================================

        public DataTable FilterByKhoaAndDiem(string maKhoa, decimal? diemTu, decimal? diemDen)
        {
            KiemTraKhoangDiem(diemTu, diemDen);

            return XuLy(() => _dal.FilterByKhoaAndDiem(maKhoa, diemTu, diemDen), "lọc theo khoa và điểm");
        }

        public DataTable FilterByDiemBetween(decimal diemTu, decimal diemDen)
        {
            KiemTraKhoangDiem(diemTu, diemDen);

            return XuLy(() => _dal.FilterByDiemBetween(diemTu, diemDen), "lọc theo khoảng điểm");
        }

        // =====================================================================
        // BAI 4 - Phan trang
        // =====================================================================

        public KetQuaPhanTrang LoadTrang(int chiSoTrang, int kichThuocTrang = KichThuocTrangMacDinh,
            string cotSapXep = null, bool giamDan = false)
        {
            if (kichThuocTrang < 1)
                throw new LoiNghiepVu(null, "Kích thước trang phải lớn hơn 0.");

            // Chi cho phep cot nam trong whitelist - chan tu day, khong doi DAL tu bao ve.
            string cot = SinhVienCot.ChuanHoa(cotSapXep);

            return XuLy(() => _dal.LoadTrang(chiSoTrang, kichThuocTrang, cot, giamDan), "phân trang");
        }

        // =====================================================================
        // BAI 5 - Chi tiet mot dong
        // =====================================================================

        public SinhVienChiTiet LayChiTiet(string maSV)
        {
            if (string.IsNullOrEmpty((maSV ?? string.Empty).Trim()))
                return new SinhVienChiTiet();          // chua chon dong nao - khong phai loi

            return XuLy(() => _dal.LayChiTiet(maSV), "đọc chi tiết sinh viên");
        }

        // =====================================================================
        // BAI 6 - Tim kiem da tieu chi
        // =====================================================================

        public KetQuaPhanTrang TimKiem(TieuChiTimKiem tc,
            int chiSoTrang, int kichThuocTrang = KichThuocTrangMacDinh)
        {
            if (tc == null) tc = new TieuChiTimKiem();

            KiemTraKhoangDiem(tc.DiemTu, tc.DiemDen);

            if (tc.TuoiTu.HasValue && tc.TuoiDen.HasValue && tc.TuoiTu > tc.TuoiDen)
                throw new LoiNghiepVu("TuoiTu", "Tuổi từ không được lớn hơn tuổi đến.");

            if (tc.TuoiTu.HasValue && tc.TuoiTu.Value < 0)
                throw new LoiNghiepVu("TuoiTu", "Tuổi không được âm.");

            tc.CotSapXep = SinhVienCot.ChuanHoa(tc.CotSapXep);

            return XuLy(() => _dal.TimKiemPhanTrang(tc, chiSoTrang, kichThuocTrang), "tìm kiếm đa tiêu chí");
        }

        /// <summary>Kiem tra dieu kien loc co sinh ra dieu kien WHERE nao khong.</summary>
        public static bool CoTieuChi(TieuChiTimKiem tc)
        {
            if (tc == null) return false;
            return !string.IsNullOrEmpty(tc.MaSV)
                || !string.IsNullOrEmpty(tc.HoTen)
                || !string.IsNullOrEmpty(tc.MaKhoa)
                || tc.GioiTinh.HasValue
                || tc.DiemTu.HasValue
                || tc.DiemDen.HasValue
                || tc.TuoiTu.HasValue
                || tc.TuoiDen.HasValue;
        }

        // =====================================================================
        // BAI 7 - Sap xep
        // =====================================================================

        public DataTable SapXep(string cot, bool giamDan)
        {
            string cotAnToan = SinhVienCot.ChuanHoa(cot);
            return XuLy(() => _dal.SapXep(cotAnToan, giamDan), "sắp xếp");
        }

        // =====================================================================
        // BAI 8 - Loc theo tuoi
        // =====================================================================

        public DataTable LocTheoTuoi(int? tuoiTu, int? tuoiDen)
        {
            if (!tuoiTu.HasValue && !tuoiDen.HasValue)
                throw new LoiNghiepVu("TuoiTu", "Hãy nhập ít nhất một đầu của khoảng tuổi.");

            if (tuoiTu.HasValue && tuoiTu.Value < 0)
                throw new LoiNghiepVu("TuoiTu", "Tuổi không được âm.");

            if (tuoiTu.HasValue && tuoiDen.HasValue && tuoiTu > tuoiDen)
                throw new LoiNghiepVu("TuoiTu", "Tuổi từ không được lớn hơn tuổi đến.");

            return XuLy(() => _dal.LocTheoTuoi(tuoiTu, tuoiDen), "lọc theo tuổi");
        }

        public int DemTheoTuoi(int tuoi)
        {
            return XuLy(() => _dal.DemTheoTuoi(tuoi), "đếm theo tuổi");
        }

        // =====================================================================
        // Tien ich
        // =====================================================================

        public List<Khoa> DanhSachKhoa()
        {
            return XuLy(() => _dal.DanhSachKhoa(), "nạp danh sách khoa");
        }

        public static int? TinhTuoi(DateTime? ngaySinh)
        {
            return SinhVienDAL.TinhTuoi(ngaySinh);
        }

        // =====================================================================
        // Kiem tra du lieu
        // =====================================================================

        private static void KiemTraKhoangDiem(decimal? tu, decimal? den)
        {
            if (tu.HasValue && (tu.Value < 0 || tu.Value > 10))
                throw new LoiNghiepVu("DiemTu", "Điểm từ phải nằm trong khoảng 0 đến 10.");

            if (den.HasValue && (den.Value < 0 || den.Value > 10))
                throw new LoiNghiepVu("DiemDen", "Điểm đến phải nằm trong khoảng 0 đến 10.");

            if (tu.HasValue && den.HasValue && tu > den)
                throw new LoiNghiepVu("DiemTu", "Điểm từ không được lớn hơn điểm đến.");
        }

        /// <summary>
        /// Boc moi loi SQL Server thanh thong bao tieng Viet.
        /// Ly do: nguoi dung cuoi khong nen doc thong bao goc cua SQL Server
        /// (vua kho hieu, vua lo cau truc CSDL).
        /// </summary>
        private static T XuLy<T>(Func<T> ham, string thaoTac)
        {
            try
            {
                return ham();
            }
            catch (LoiNghiepVu)
            {
                throw;
            }
            catch (SqlException ex)
            {
                throw new Exception("Không thể " + thaoTac + ": " + MoTaLoiSql(ex));
            }
            catch (InvalidOperationException)
            {
                throw new Exception("Không thể " + thaoTac + ": kết nối CSDL chưa được mở hoặc đã đóng.");
            }
        }

        /// <summary>Chuyen ma loi SQL Server thanh cau tieng Viet de hieu.</summary>
        public static string MoTaLoiSql(SqlException ex)
        {
            switch (ex.Number)
            {
                case -2:    return "hết thời gian chờ (timeout). CSDL có thể đang bận.";
                case 53:    return "không tìm thấy máy chủ SQL Server. Kiểm tra Instance trong App.config.";
                case -1:    return "không kết nối được tới SQL Server.";
                case 4060:  return "không mở được CSDL QLSinhVien. Hãy chạy script tạo CSDL của Lab 7.";
                case 18456: return "đăng nhập SQL Server thất bại. Kiểm tra tài khoản/mật khẩu.";
                case 208:   return "không tìm thấy bảng hoặc view. Hãy chạy script tạo CSDL.";
                case 8155:  return "cột trong câu lệnh không có tên định danh (lỗi câu SQL).";
                default:    return ex.Message;
            }
        }
    }
}
