using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab06_ThreeLayer.BUS
{
    /// <summary>
    /// ============================================================
    /// TẦNG BUS - NGHIỆP VỤ (Business Logic Layer)
    /// ============================================================
    /// Nhiệm vụ: kiểm tra dữ liệu, áp quy tắc nghiệp vụ, điều phối gọi DAL.
    ///   - Form KHÔNG được gọi DAL trực tiếp; Form chỉ gọi BUS.
    ///   - BUS không biết gì về giao diện (không tham chiếu Windows.Forms).
    ///
    /// Ngoại lệ: LoiNghiepVu (kèm tên trường) để GUI hiển thị lỗi đúng ô nhập.
    /// </summary>
    public class SinhVienBUS
    {
        private readonly DAL.SinhVienDAL _dal;

        public SinhVienBUS()
            : this(new DAL.SinhVienDAL())
        {
        }

        // Cho phép tiêm DAL vào (dependency injection) - tiện cho kiểm thử và Lab 7
        public SinhVienBUS(DAL.SinhVienDAL dal)
        {
            if (dal == null) throw new ArgumentNullException("dal");
            _dal = dal;
        }

        // =================================================================
        // ĐỌC
        // =================================================================
        public List<DTO.SinhVien> GetAll()
        {
            return _dal.GetAll();
        }

        public DTO.SinhVien GetByMa(string maSV)
        {
            return _dal.GetByMa(maSV);
        }

        public int SoLuong
        {
            get { return _dal.Count; }
        }

        /// <summary>Tìm kiếm theo từ khóa (mã, họ tên, khoa) và lọc theo khoa - không đụng dữ liệu gốc.</summary>
        public List<DTO.SinhVien> TimKiem(string tuKhoa, string khoa)
        {
            string k = (tuKhoa ?? string.Empty).Trim().ToLowerInvariant();
            List<DTO.SinhVien> ds = _dal.GetAll();

            return ds.Where(sv =>
                    (k.Length == 0
                     || sv.MaSV.ToLowerInvariant().Contains(k)
                     || sv.HoTen.ToLowerInvariant().Contains(k)
                     || sv.Khoa.ToLowerInvariant().Contains(k))
                    && (string.IsNullOrEmpty(khoa) || sv.Khoa == khoa))
                .ToList();
        }

        /// <summary>Sắp xếp theo tiêu chí: ma | ten | diem | khoa (giảm dần nếu descending = true).</summary>
        public List<DTO.SinhVien> SapXep(IEnumerable<DTO.SinhVien> nguon, string tieuChi, bool giamDan)
        {
            if (nguon == null) return new List<DTO.SinhVien>();
            IOrderedEnumerable<DTO.SinhVien> q;
            switch ((tieuChi ?? string.Empty).ToLowerInvariant())
            {
                case "ten": q = giamDan ? nguon.OrderByDescending(s => s.HoTen) : nguon.OrderBy(s => s.HoTen); break;
                case "diem": q = giamDan ? nguon.OrderByDescending(s => s.DiemTB) : nguon.OrderBy(s => s.DiemTB); break;
                case "khoa": q = giamDan ? nguon.OrderByDescending(s => s.Khoa).ThenBy(s => s.HoTen) : nguon.OrderBy(s => s.Khoa).ThenBy(s => s.HoTen); break;
                default: q = giamDan ? nguon.OrderByDescending(s => s.MaSV) : nguon.OrderBy(s => s.MaSV); break;
            }
            return q.ToList();
        }

        /// <summary>Nhóm theo khoa (Bài 4: TreeView khoa - sinh viên).</summary>
        public Dictionary<string, List<DTO.SinhVien>> NhomTheoKhoa()
        {
            var kq = new Dictionary<string, List<DTO.SinhVien>>();
            foreach (DTO.SinhVien sv in _dal.GetAll())
            {
                string khoa = string.IsNullOrEmpty(sv.Khoa) ? "(Chưa xếp khoa)" : sv.Khoa;
                if (!kq.ContainsKey(khoa)) kq[khoa] = new List<DTO.SinhVien>();
                kq[khoa].Add(sv);
            }
            return kq;
        }

        // =================================================================
        // GHI (có kiểm tra nghiệp vụ)
        // =================================================================
        public void Add(DTO.SinhVien sv)
        {
            KiemTraDuLieu(sv, false);
            _dal.Add(sv);
        }

        public void Update(DTO.SinhVien sv)
        {
            KiemTraDuLieu(sv, true);
            if (!_dal.Update(sv))
                throw new LoiNghiepVu("MaSV", "Không tìm thấy sinh viên " + sv.MaSV + " để cập nhật.");
        }

        public void Delete(string maSV)
        {
            if (string.IsNullOrWhiteSpace(maSV))
                throw new LoiNghiepVu("MaSV", "Thiếu mã sinh viên cần xóa.");
            if (!_dal.Delete(maSV))
                throw new LoiNghiepVu("MaSV", "Không tìm thấy sinh viên " + maSV + " để xóa.");
        }

        // =================================================================
        // QUY TẮC NGHIỆP VỤ
        // =================================================================
        /// <summary>
        /// Kiểm tra toàn bộ dữ liệu theo quy tắc Lab 6:
        ///   - Mã không rỗng, không trùng (khi thêm)
        ///   - Họ tên >= 2 ký tự, không chứa chữ số
        ///   - Điểm thuộc [0, 10]
        ///   - Tuổi 16 - 80, ngày sinh không ở tương lai
        ///   - Khoa phải thuộc danh mục khoa
        /// </summary>
        public void KiemTraDuLieu(DTO.SinhVien sv, bool laCapNhat)
        {
            if (sv == null) throw new LoiNghiepVu("", "Dữ liệu sinh viên rỗng.");

            if (string.IsNullOrWhiteSpace(sv.MaSV))
                throw new LoiNghiepVu("MaSV", "Thiếu mã sinh viên.");
            if (sv.MaSV.Trim().Length < 2 || sv.MaSV.Trim().Length > 10)
                throw new LoiNghiepVu("MaSV", "Mã sinh viên phải có 2 - 10 ký tự.");

            if (!laCapNhat && _dal.GetByMa(sv.MaSV) != null)
                throw new LoiNghiepVu("MaSV", "Trùng mã sinh viên: " + sv.MaSV + " đã tồn tại.");

            if (string.IsNullOrWhiteSpace(sv.HoTen))
                throw new LoiNghiepVu("HoTen", "Thiếu họ tên sinh viên.");
            if (sv.HoTen.Trim().Length < 2)
                throw new LoiNghiepVu("HoTen", "Họ tên phải có ít nhất 2 ký tự.");
            if (sv.HoTen.Any(char.IsDigit))
                throw new LoiNghiepVu("HoTen", "Họ tên không được chứa chữ số.");

            if (sv.DiemTB < 0 || sv.DiemTB > 10)
                throw new LoiNghiepVu("DiemTB", "Điểm trung bình phải thuộc [0, 10].");

            if (sv.NgaySinh.Date > DateTime.Today)
                throw new LoiNghiepVu("NgaySinh", "Ngày sinh không được ở tương lai.");
            int tuoi = TinhTuoi(sv.NgaySinh);
            if (tuoi < 16 || tuoi > 80)
                throw new LoiNghiepVu("NgaySinh", string.Format("Tuổi {0} không hợp lệ (yêu cầu 16 - 80).", tuoi));

            if (!string.IsNullOrEmpty(sv.Email) && !HopLeEmail(sv.Email))
                throw new LoiNghiepVu("Email", "Email không đúng định dạng.");

            if (string.IsNullOrWhiteSpace(sv.Khoa))
                throw new LoiNghiepVu("Khoa", "Chưa chọn khoa.");
            if (!new KhoaBUS().GetAll().Any(k => k.TenKhoa == sv.Khoa))
                throw new LoiNghiepVu("Khoa", "Khoa \"" + sv.Khoa + "\" không có trong danh mục.");
        }

        public static int TinhTuoi(DateTime ngaySinh)
        {
            int tuoi = DateTime.Today.Year - ngaySinh.Year;
            if (ngaySinh.Date > DateTime.Today.AddYears(-tuoi)) tuoi--;
            return tuoi;
        }

        public static bool HopLeEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return true;
            int at = email.IndexOf('@');
            if (at <= 0 || at != email.LastIndexOf('@')) return false;
            string domain = email.Substring(at + 1);
            return domain.Contains('.') && !domain.EndsWith(".") && !email.Contains(" ");
        }

        // =================================================================
        // BÀI 9: CSV - Form gọi BUS, BUS gọi DAL
        // =================================================================
        public int LuuCSV(string duongDan)
        {
            if (string.IsNullOrWhiteSpace(duongDan))
                throw new LoiNghiepVu("", "Chưa có đường dẫn tệp CSV.");
            return _dal.LuuCSV(duongDan);
        }

        public int DocCSV(string duongDan, out int soDongLoi)
        {
            return _dal.DocCSV(duongDan, out soDongLoi);
        }
    }

    /// <summary>Nghiệp vụ Khoa + dữ liệu mẫu 3 khoa.</summary>
    public class KhoaBUS
    {
        private readonly static DAL.KhoaDAL _dal = TaoDuLieuMau();

        private static DAL.KhoaDAL TaoDuLieuMau()
        {
            var dal = new DAL.KhoaDAL();
            dal.Add(new DTO.Khoa("CNTT", "Công nghệ thông tin"));
            dal.Add(new DTO.Khoa("KD", "Kinh tế"));
            dal.Add(new DTO.Khoa("DT", "Điện - Điện tử"));
            return dal;
        }

        public List<DTO.Khoa> GetAll()
        {
            return _dal.GetAll();
        }

        public void Them(string maKhoa, string tenKhoa)
        {
            if (string.IsNullOrWhiteSpace(maKhoa) || string.IsNullOrWhiteSpace(tenKhoa))
                throw new LoiNghiepVu("Khoa", "Mã khoa và tên khoa không được rỗng.");
            if (_dal.Exists(tenKhoa))
                throw new LoiNghiepVu("Khoa", "Khoa \"" + tenKhoa + "\" đã tồn tại.");
            _dal.Add(new DTO.Khoa(maKhoa.Trim().ToUpperInvariant(), tenKhoa.Trim()));
        }
    }

    /// <summary>Nghiệp vụ Sản phẩm (Bài 6: thêm/sửa/xóa/tìm/sắp xếp/tổng hàng tồn).</summary>
    public class SanPhamBUS
    {
        private readonly DAL.SanPhamDAL _dal;
        private readonly DAL.DanhMucDAL _danhMuc;

        public SanPhamBUS()
            : this(new DAL.SanPhamDAL(), new DAL.DanhMucDAL())
        {
        }

        public SanPhamBUS(DAL.SanPhamDAL dal, DAL.DanhMucDAL danhMuc)
        {
            _dal = dal;
            _danhMuc = danhMuc;
        }

        public List<DTO.SanPham> GetAll()
        {
            return _dal.GetAll();
        }

        public void Add(DTO.SanPham sp)
        {
            KiemTra(sp);
            if (_dal.Exists(sp.MaSP))
                throw new LoiNghiepVu("MaSP", "Trùng mã sản phẩm.");
            _dal.Add(sp);
        }

        public void Update(DTO.SanPham sp)
        {
            KiemTra(sp);
            if (!_dal.Update(sp))
                throw new LoiNghiepVu("MaSP", "Không tìm thấy sản phẩm " + sp.MaSP + ".");
        }

        public void Delete(string maSP)
        {
            if (!_dal.Delete(maSP))
                throw new LoiNghiepVu("MaSP", "Không tìm thấy sản phẩm " + maSP + ".");
        }

        private void KiemTra(DTO.SanPham sp)
        {
            if (sp == null) throw new LoiNghiepVu("", "Dữ liệu sản phẩm rỗng.");
            if (string.IsNullOrWhiteSpace(sp.MaSP)) throw new LoiNghiepVu("MaSP", "Thiếu mã sản phẩm.");
            if (string.IsNullOrWhiteSpace(sp.TenSP)) throw new LoiNghiepVu("TenSP", "Thiếu tên sản phẩm.");
            if (sp.DonGia < 0) throw new LoiNghiepVu("DonGia", "Đơn giá không được âm.");
            if (sp.SoLuongTon < 0) throw new LoiNghiepVu("SoLuongTon", "Số lượng tồn không được âm.");
        }

        /// <summary>Tổng giá trị hàng tồn (đơn giá × số lượng) - tính ở BUS, không tính ở Form.</summary>
        public double TongGiaTriTon(IEnumerable<DTO.SanPham> ds)
        {
            if (ds == null) return 0;
            return ds.Sum(x => x.DonGia * x.SoLuongTon);
        }

        public List<DTO.SanPham> TimKiem(string tuKhoa)
        {
            string k = (tuKhoa ?? string.Empty).Trim().ToLowerInvariant();
            return _dal.GetAll().Where(x =>
                k.Length == 0 || x.MaSP.ToLowerInvariant().Contains(k) || x.TenSP.ToLowerInvariant().Contains(k)).ToList();
        }

        public List<DTO.SanPham> SapXep(IEnumerable<DTO.SanPham> nguon, string tieuChi, bool giamDan)
        {
            IEnumerable<DTO.SanPham> s = nguon ?? new List<DTO.SanPham>();
            switch ((tieuChi ?? "").ToLowerInvariant())
            {
                case "ten": return giamDan ? s.OrderByDescending(x => x.TenSP).ToList() : s.OrderBy(x => x.TenSP).ToList();
                case "gia": return giamDan ? s.OrderByDescending(x => x.DonGia).ToList() : s.OrderBy(x => x.DonGia).ToList();
                case "ton": return giamDan ? s.OrderByDescending(x => x.SoLuongTon).ToList() : s.OrderBy(x => x.SoLuongTon).ToList();
                case "giatri": return giamDan ? s.OrderByDescending(x => x.GiaTriTon).ToList() : s.OrderBy(x => x.GiaTriTon).ToList();
                default: return giamDan ? s.OrderByDescending(x => x.MaSP).ToList() : s.OrderBy(x => x.MaSP).ToList();
            }
        }
    }

    /// <summary>Nghiệp vụ Danh mục sản phẩm (Bài 7: TreeView danh mục - sản phẩm).</summary>
    public class DanhMucBUS
    {
        private readonly DAL.DanhMucDAL _dal;
        private readonly DAL.SanPhamDAL _sanPham;

        public DanhMucBUS()
            : this(new DAL.DanhMucDAL(), new DAL.SanPhamDAL())
        {
        }

        public DanhMucBUS(DAL.DanhMucDAL dal, DAL.SanPhamDAL sanPham)
        {
            _dal = dal;
            _sanPham = sanPham;
        }

        public List<DTO.DanhMuc> GetAll()
        {
            return _dal.GetAll();
        }

        public List<DTO.SanPham> SanPhamCua(string maDanhMuc)
        {
            return _sanPham.GetAll().Where(x => x.MaDanhMuc == maDanhMuc).ToList();
        }

        public void ThemDanhMuc(string ma, string ten)
        {
            if (string.IsNullOrWhiteSpace(ma) || string.IsNullOrWhiteSpace(ten))
                throw new LoiNghiepVu("DanhMuc", "Mã và tên danh mục không được rỗng.");
            if (_dal.Exists(ma))
                throw new LoiNghiepVu("DanhMuc", "Danh mục \"" + ma + "\" đã tồn tại.");
            _dal.Add(new DTO.DanhMuc(ma.Trim(), ten.Trim()));
        }

        public void DoiTenDanhMuc(string ma, string tenMoi)
        {
            if (string.IsNullOrWhiteSpace(tenMoi))
                throw new LoiNghiepVu("DanhMuc", "Tên danh mục không được rỗng.");
            if (!_dal.DoiTen(ma, tenMoi.Trim()))
                throw new LoiNghiepVu("DanhMuc", "Không tìm thấy danh mục " + ma + ".");
        }

        /// <summary>Không cho xóa danh mục còn sản phẩm (quy tắc nghiệp vụ).</summary>
        public void XoaDanhMuc(string ma)
        {
            if (SanPhamCua(ma).Count > 0)
                throw new LoiNghiepVu("DanhMuc", "Danh mục còn sản phẩm, không thể xóa.");
            if (!_dal.Delete(ma))
                throw new LoiNghiepVu("DanhMuc", "Không tìm thấy danh mục " + ma + ".");
        }
    }

    /// <summary>
    /// Bộ dữ liệu mẫu: 3 khoa, mỗi khoa 3 sinh viên (đúng yêu cầu chuẩn bị của Lab 6).
    /// Được nạp qua BUS nên cũng phải đi qua kiểm tra nghiệp vụ.
    /// </summary>
    public static class DuLieuMau
    {
        public static void NapSinhVien(SinhVienBUS bus)
        {
            if (bus == null || bus.SoLuong > 0) return;

            var ds = new List<DTO.SinhVien>
            {
                new DTO.SinhVien("SV01", "Nguyễn Văn An",   new DateTime(2005, 3, 15), 8.7, "Công nghệ thông tin"),
                new DTO.SinhVien("SV02", "Trần Thị Bình",   new DateTime(2005, 7, 22), 7.4, "Công nghệ thông tin"),
                new DTO.SinhVien("SV03", "Lê Hoàng Cường",  new DateTime(2004, 11, 2), 6.2, "Công nghệ thông tin"),
                new DTO.SinhVien("SV04", "Phạm Thu Dung",   new DateTime(2005, 1, 9),  9.1, "Kinh tế"),
                new DTO.SinhVien("SV05", "Hoàng Minh Đức",  new DateTime(2004, 9, 18), 4.8, "Kinh tế"),
                new DTO.SinhVien("SV06", "Vũ Thị Hằng",     new DateTime(2005, 5, 5),  7.9, "Kinh tế"),
                new DTO.SinhVien("SV07", "Đặng Quốc Khánh", new DateTime(2004, 12, 12), 6.8, "Điện - Điện tử"),
                new DTO.SinhVien("SV08", "Bùi Lan Anh",     new DateTime(2005, 2, 28), 8.2, "Điện - Điện tử"),
                new DTO.SinhVien("SV09", "Ngô Tuấn Kiệt",   new DateTime(2003, 8, 30), 5.5, "Điện - Điện tử")
            };

            foreach (DTO.SinhVien sv in ds)
            {
                sv.GioiTinh = sv.HoTen.StartsWith("Nguyễn Văn") || sv.HoTen.StartsWith("Lê ") || sv.HoTen.StartsWith("Hoàng ")
                    || sv.HoTen.StartsWith("Đặng ") || sv.HoTen.StartsWith("Ngô ") ? "Nam" : "Nữ";
                sv.Email = sv.MaSV.ToLowerInvariant() + "@sv.dongang.edu.vn";
                bus.Add(sv);
            }
        }

        public static void NapSanPham(SanPhamBUS bus, DanhMucBUS danhMuc)
        {
            if (bus == null) return;
            if (danhMuc.GetAll().Count == 0)
            {
                danhMuc.ThemDanhMuc("DM01", "Điện tử");
                danhMuc.ThemDanhMuc("DM02", "Văn phòng phẩm");
                danhMuc.ThemDanhMuc("DM03", "Gia dụng");
            }

            if (bus.GetAll().Count > 0) return;

            var ds = new List<DTO.SanPham>
            {
                new DTO.SanPham("SP01", "Laptop Dell XPS 15", "DM01", 25000000, 5),
                new DTO.SanPham("SP02", "Chuột không dây Logitech", "DM01", 350000, 42),
                new DTO.SanPham("SP03", "Bàn phím cơ Keychron K2", "DM01", 1850000, 12),
                new DTO.SanPham("SP04", "Bút bi Thiên Long TL-027", "DM02", 5000, 300),
                new DTO.SanPham("SP05", "Vở Campus 200 trang", "DM02", 18000, 150),
                new DTO.SanPham("SP06", "Nồi cơm điện Sharp 1.8L", "DM03", 1450000, 8)
            };

            foreach (DTO.SanPham sp in ds) bus.Add(sp);
        }
    }
}
