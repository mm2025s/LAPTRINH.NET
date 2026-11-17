using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using QuanLyNhaThuoc.DAL;
using QuanLyNhaThuoc.DTO;

namespace QuanLyNhaThuoc.BUS
{
    /// <summary>Kiem tra du lieu dung chung cho cac BUS.</summary>
    internal static class KiemTra
    {
        private static readonly Regex MauMa = new Regex("^[A-Za-z0-9_-]+$");
        private static readonly Regex MauDienThoai = new Regex("^0[0-9]{9,10}$");

        public static string Ma(string giaTri, string truong, string ten, int toiDa)
        {
            string v = (giaTri ?? "").Trim().ToUpperInvariant();
            if (v.Length == 0) throw new LoiNhapLieu(truong, ten + " không được rỗng.");
            if (v.Length > toiDa) throw new LoiNhapLieu(truong, ten + " tối đa " + toiDa + " ký tự.");
            if (!MauMa.IsMatch(v)) throw new LoiNhapLieu(truong, ten + " chỉ gồm chữ không dấu, số, '_' và '-'.");
            return v;
        }

        public static string Chu(string giaTri, string truong, string ten, int toiDa, bool batBuoc = true)
        {
            string v = (giaTri ?? "").Trim();
            if (batBuoc && v.Length == 0) throw new LoiNhapLieu(truong, ten + " không được rỗng.");
            if (v.Length > toiDa) throw new LoiNhapLieu(truong, ten + " tối đa " + toiDa + " ký tự.");
            return v.Length == 0 ? null : v;
        }

        public static string DienThoai(string giaTri, bool batBuoc)
        {
            string v = Regex.Replace(giaTri ?? "", @"[\s.]", "");
            if (v.Length == 0 && !batBuoc) return null;
            if (!MauDienThoai.IsMatch(v)) throw new LoiNhapLieu("DienThoai", "Số điện thoại gồm 10-11 chữ số, bắt đầu bằng 0.");
            return v;
        }

        /// <summary>Chi Quan ly moi duoc thuc hien thao tac (kiem tra o BUS, khong chi an nut tren GUI).</summary>
        public static void YeuCauQuanLy()
        {
            if (PhienDangNhap.HienTai == null || !PhienDangNhap.HienTai.LaQuanLy)
                throw new LoiNghiepVu("Chức năng này chỉ dành cho Quản lý.");
        }

        public static NhanVienDTO NhanVienHienTai()
        {
            if (PhienDangNhap.HienTai == null) throw new LoiNghiepVu("Phiên làm việc đã hết, vui lòng đăng nhập lại.");
            return PhienDangNhap.HienTai;
        }
    }

    // =====================================================================
    public class NhanVienBUS : BusCoSo
    {
        private readonly NhanVienDAL _dal = new NhanVienDAL();

        /// <summary>SHA-256(salt + mat khau) theo UTF-16LE, khop HASHBYTES('SHA2_256', NVARCHAR) cua SQL Server.</summary>
        public static byte[] BamMatKhau(string salt, string matKhau)
        {
            using (var sha = SHA256.Create())
                return sha.ComputeHash(Encoding.Unicode.GetBytes(salt + matKhau));
        }

        private static string TaoSalt()
        {
            var b = new byte[12];
            using (var rng = RandomNumberGenerator.Create()) rng.GetBytes(b);
            return Convert.ToBase64String(b);   // 16 ky tu
        }

        public NhanVienDTO DangNhap(string tenDangNhap, string matKhau)
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap)) throw new LoiNhapLieu("TenDangNhap", "Chưa nhập tên đăng nhập.");
            if (string.IsNullOrEmpty(matKhau)) throw new LoiNhapLieu("MatKhau", "Chưa nhập mật khẩu.");

            byte[] hash = null;
            string salt = null;
            NhanVienDTO nv = Doc(() =>
            {
                byte[] h; string s;
                NhanVienDTO kq = _dal.LayDeDangNhap(tenDangNhap.Trim(), out h, out s);
                hash = h; salt = s;
                return kq;
            }, "đăng nhập");

            // Cung mot thong bao cho "sai ten" va "sai mat khau" de khong lo ten tai khoan ton tai.
            if (nv == null || !BamMatKhau(salt, matKhau).SequenceEqual(hash))
            {
                NhatKy.Ghi("đăng nhập", "NhanVien", "Sai tên đăng nhập hoặc mật khẩu", false, 0, Cat(tenDangNhap.Trim(), 30));
                throw new LoiNghiepVu("Tên đăng nhập hoặc mật khẩu không đúng.");
            }
            if (!nv.IsActive)
            {
                NhatKy.Ghi("đăng nhập", "NhanVien", "Tài khoản bị khóa", false, 0, nv.TenDangNhap);
                throw new LoiNghiepVu("Tài khoản đã bị khóa. Liên hệ Quản lý.");
            }

            PhienDangNhap.HienTai = nv;
            NhatKy.Ghi("đăng nhập", "NhanVien", nv.MoTa(), true, 0);
            return nv;
        }

        public void DangXuat()
        {
            if (PhienDangNhap.HienTai == null) return;
            NhatKy.Ghi("đăng xuất", "NhanVien", PhienDangNhap.HienTai.MoTa(), true, 0);
            PhienDangNhap.HienTai = null;
        }

        private static string Cat(string s, int n) { return s.Length <= n ? s : s.Substring(0, n);
        }

        public DataTable GetAll()
        {
            KiemTra.YeuCauQuanLy();
            return Doc(() => _dal.GetAll(), "tải nhân viên");
        }

        public void Them(NhanVienDTO nv, string matKhau)
        {
            KiemTra.YeuCauQuanLy();
            nv.HoTen = KiemTra.Chu(nv.HoTen, "HoTen", "Họ tên", 100);
            nv.TenDangNhap = KiemTra.Chu(nv.TenDangNhap, "TenDangNhap", "Tên đăng nhập", 30).ToLowerInvariant();
            KiemTraMatKhau(matKhau);
            string salt = TaoSalt();
            Ghi(() => _dal.Insert(nv, BamMatKhau(salt, matKhau), salt), "thêm nhân viên", "NhanVien", nv.TenDangNhap);
        }

        public void CapNhat(NhanVienDTO nv)
        {
            KiemTra.YeuCauQuanLy();
            nv.HoTen = KiemTra.Chu(nv.HoTen, "HoTen", "Họ tên", 100);
            if (nv.MaNV == PhienDangNhap.HienTai.MaNV && (!nv.IsActive || !nv.LaQuanLy))
                throw new LoiNghiepVu("Không thể tự khóa hoặc tự hạ quyền tài khoản đang đăng nhập.");
            Ghi(() => _dal.Update(nv), "cập nhật nhân viên", "NhanVien", nv.MoTa() + (nv.IsActive ? "" : " (khóa)"));
        }

        /// <summary>Quan ly dat lai mat khau cho nhan vien khac.</summary>
        public void DatLaiMatKhau(int maNV, string matKhauMoi)
        {
            KiemTra.YeuCauQuanLy();
            KiemTraMatKhau(matKhauMoi);
            string salt = TaoSalt();
            Ghi(() => _dal.DoiMatKhau(maNV, BamMatKhau(salt, matKhauMoi), salt), "đặt lại mật khẩu", "NhanVien", "MaNV=" + maNV);
        }

        /// <summary>Nhan vien tu doi mat khau: phai nhap dung mat khau cu.</summary>
        public void DoiMatKhau(string matKhauCu, string matKhauMoi, string xacNhan)
        {
            NhanVienDTO nv = KiemTra.NhanVienHienTai();
            byte[] hash = null;
            string salt = null;
            Doc(() =>
            {
                byte[] h; string s;
                NhanVienDTO kq = _dal.LayDeDangNhap(nv.TenDangNhap, out h, out s);
                hash = h; salt = s;
                return kq;
            }, "đổi mật khẩu");
            if (!BamMatKhau(salt, matKhauCu ?? "").SequenceEqual(hash))
                throw new LoiNhapLieu("MatKhauCu", "Mật khẩu hiện tại không đúng.");
            KiemTraMatKhau(matKhauMoi);
            if (matKhauMoi != xacNhan) throw new LoiNhapLieu("XacNhan", "Xác nhận mật khẩu không khớp.");
            string saltMoi = TaoSalt();
            Ghi(() => _dal.DoiMatKhau(nv.MaNV, BamMatKhau(saltMoi, matKhauMoi), saltMoi), "đổi mật khẩu", "NhanVien", nv.TenDangNhap);
        }

        private static void KiemTraMatKhau(string mk)
        {
            if (mk == null || mk.Length < 6) throw new LoiNhapLieu("MatKhau", "Mật khẩu tối thiểu 6 ký tự.");
        }
    }

    // =====================================================================
    public class DanhMucBUS : BusCoSo
    {
        private readonly DanhMucDAL _dal = new DanhMucDAL();

        public DataTable GetNhomThuoc() { return Doc(() => _dal.GetNhomThuoc(), "tải nhóm thuốc"); }
        public DataTable GetNhaCungCap() { return Doc(() => _dal.GetNhaCungCap(), "tải nhà cung cấp"); }

        public void LuuNhomThuoc(NhomThuocDTO n, bool laThemMoi)
        {
            KiemTra.YeuCauQuanLy();
            n.MaNhom = KiemTra.Ma(n.MaNhom, "MaNhom", "Mã nhóm", 10);
            n.TenNhom = KiemTra.Chu(n.TenNhom, "TenNhom", "Tên nhóm", 100);
            Ghi(() => _dal.SaveNhomThuoc(n, laThemMoi), laThemMoi ? "thêm nhóm thuốc" : "sửa nhóm thuốc", "NhomThuoc", n.MoTa());
        }

        public void XoaNhomThuoc(string ma)
        {
            KiemTra.YeuCauQuanLy();
            Ghi(() => _dal.DeleteNhomThuoc(KiemTra.Ma(ma, "MaNhom", "Mã nhóm", 10)), "xóa nhóm thuốc", "NhomThuoc", ma);
        }

        public void LuuNhaCungCap(NhaCungCapDTO n, bool laThemMoi)
        {
            KiemTra.YeuCauQuanLy();
            n.MaNCC = KiemTra.Ma(n.MaNCC, "MaNCC", "Mã nhà cung cấp", 10);
            n.TenNCC = KiemTra.Chu(n.TenNCC, "TenNCC", "Tên nhà cung cấp", 150);
            n.DienThoai = KiemTra.DienThoai(n.DienThoai, false);
            n.DiaChi = KiemTra.Chu(n.DiaChi, "DiaChi", "Địa chỉ", 200, false);
            Ghi(() => _dal.SaveNhaCungCap(n, laThemMoi), laThemMoi ? "thêm nhà cung cấp" : "sửa nhà cung cấp", "NhaCungCap", n.MoTa());
        }

        public void XoaNhaCungCap(string ma)
        {
            KiemTra.YeuCauQuanLy();
            Ghi(() => _dal.DeleteNhaCungCap(KiemTra.Ma(ma, "MaNCC", "Mã nhà cung cấp", 10)), "xóa nhà cung cấp", "NhaCungCap", ma);
        }
    }

    // =====================================================================
    public class ThuocBUS : BusCoSo
    {
        private readonly ThuocDAL _dal = new ThuocDAL();

        public DataTable TimKiem(string tuKhoa, string maNhom)
        {
            return Doc(() => _dal.Search(tuKhoa, maNhom), "tìm thuốc");
        }

        public void Them(ThuocDTO t) { KiemTra.YeuCauQuanLy(); ChuanHoa(t); Ghi(() => _dal.Them(t), "thêm thuốc", "Thuoc", t.MoTa()); }
        public void CapNhat(ThuocDTO t) { KiemTra.YeuCauQuanLy(); ChuanHoa(t); Ghi(() => _dal.Sua(t), "cập nhật thuốc", "Thuoc", t.MoTa()); }

        /// <summary>Thuoc da co giao dich chi bi ngung kinh doanh (xoa mem) - xu ly trong sp_Thuoc_Delete.</summary>
        public void Xoa(string maThuoc)
        {
            KiemTra.YeuCauQuanLy();
            Ghi(() => _dal.Xoa(KiemTra.Ma(maThuoc, "MaThuoc", "Mã thuốc", 15)), "xóa thuốc", "Thuoc", maThuoc);
        }

        private static void ChuanHoa(ThuocDTO t)
        {
            if (t == null) throw new LoiNhapLieu(null, "Chưa có dữ liệu thuốc.");
            t.MaThuoc = KiemTra.Ma(t.MaThuoc, "MaThuoc", "Mã thuốc", 15);
            t.TenThuoc = KiemTra.Chu(t.TenThuoc, "TenThuoc", "Tên thuốc", 150);
            t.DonViTinh = KiemTra.Chu(t.DonViTinh, "DonViTinh", "Đơn vị tính", 20);
            if (string.IsNullOrEmpty(t.MaNhom)) throw new LoiNhapLieu("MaNhom", "Chưa chọn nhóm thuốc.");
            if (t.GiaBan <= 0 || t.GiaBan > 100000000m) throw new LoiNhapLieu("GiaBan", "Giá bán phải lớn hơn 0 và không quá 100 triệu.");
            if (decimal.Round(t.GiaBan, 0) != t.GiaBan) throw new LoiNhapLieu("GiaBan", "Giá bán tính theo đồng, không có phần lẻ.");
        }
    }

    // =====================================================================
    public class KhachHangBUS : BusCoSo
    {
        private readonly KhachHangDAL _dal = new KhachHangDAL();

        public DataTable TimKiem(string tuKhoa) { return Doc(() => _dal.Search(tuKhoa), "tìm khách hàng"); }

        /// <summary>Tim dung mot khach theo so dien thoai (man hinh ban hang); khong co thi null.</summary>
        public KhachHangDTO TimTheoDienThoai(string dienThoai)
        {
            string sdt = KiemTra.DienThoai(dienThoai, true);
            foreach (DataRow r in TimKiem(sdt).Rows)
                if ((string)r["DienThoai"] == sdt)
                    return new KhachHangDTO { MaKH = (int)r["MaKH"], HoTen = (string)r["HoTen"], DienThoai = sdt };
            return null;
        }

        public int Luu(KhachHangDTO k)
        {
            if (k == null) throw new LoiNhapLieu(null, "Chưa có dữ liệu khách hàng.");
            k.HoTen = KiemTra.Chu(k.HoTen, "HoTen", "Họ tên khách hàng", 100);
            k.DienThoai = KiemTra.DienThoai(k.DienThoai, true);
            k.DiaChi = KiemTra.Chu(k.DiaChi, "DiaChi", "Địa chỉ", 200, false);
            return Ghi(() => k.MaKH == null ? _dal.Them(k) : _dal.Sua(k), k.MaKH == null ? "thêm khách hàng" : "sửa khách hàng", "KhachHang", k.MoTa());
        }

        public void Xoa(int maKH) { KiemTra.YeuCauQuanLy(); Ghi(() => _dal.Xoa(maKH.ToString()), "xóa khách hàng", "KhachHang", "MaKH=" + maKH); }
    }

    // =====================================================================
    /// <summary>Nghiep vu ban hang: quan ly gio hang trong bo nho, lap hoa don qua mot transaction.</summary>
    public class BanHangBUS : BusCoSo, IGioHang<DongGioHang>
    {
        private readonly GiaoDichDAL _dal = new GiaoDichDAL();

        public List<DongGioHang> Gio { get; private set; }

        public BanHangBUS() { Gio = new List<DongGioHang>(); }

        public decimal TongTien { get { return Gio.Sum(d => d.ThanhTien); } }
        public bool CoThuocKeDon { get { return Gio.Any(d => d.CanKeDon); } }

        /// <summary>Them thuoc vao gio tu mot dong ket qua tim kiem. Trung thuoc thi cong don so luong.</summary>
        public void ThemVaoGio(DataRow thuoc, int soLuong)
        {
            if (thuoc == null) throw new LoiNhapLieu("Thuoc", "Chưa chọn thuốc.");
            if (soLuong <= 0) throw new LoiNhapLieu("SoLuong", "Số lượng phải lớn hơn 0.");

            string ma = (string)thuoc["MaThuoc"];
            string ten = (string)thuoc["TenThuoc"];
            int ton = (int)thuoc["TonKho"];
            if (thuoc["HanSuDung"] != DBNull.Value && (DateTime)thuoc["HanSuDung"] < DateTime.Today)
                throw new LoiNghiepVu("Thuốc \"" + ten + "\" đã hết hạn sử dụng, không được bán.");

            DongGioHang dong = Gio.FirstOrDefault(d => d.MaThuoc == ma);
            int tong = soLuong + (dong == null ? 0 : dong.SoLuong);
            if (tong > ton)
                throw new LoiNghiepVu("Không đủ tồn kho: \"" + ten + "\" chỉ còn " + ton + " " + thuoc["DonViTinh"] + ".");

            if (dong != null) { dong.SoLuong = tong; return; }
            Gio.Add(new DongGioHang
            {
                MaThuoc = ma, TenThuoc = ten, DonViTinh = (string)thuoc["DonViTinh"],
                SoLuong = soLuong, DonGia = (decimal)thuoc["GiaBan"], CanKeDon = (bool)thuoc["CanKeDon"]
            });
        }

        public void XoaKhoiGio(string maThuoc) { Gio.RemoveAll(d => d.MaThuoc == maThuoc); }

        // ---- IGioHang ----
        IList<DongGioHang> IGioHang<DongGioHang>.DanhSach { get { return Gio; } }
        public void XoaDong(string maThuoc) { XoaKhoiGio(maThuoc); }
        public void LamTrong() { Gio.Clear(); }

        /// <summary>
        /// Lap hoa don. Thuoc ke don bat buoc ghi so don thuoc. Ton kho duoc
        /// kiem tra LAI trong thu tuc (co the quay khac vua ban mat).
        /// </summary>
        public int ThanhToan(int? maKH, string soDonThuoc)
        {
            NhanVienDTO nv = KiemTra.NhanVienHienTai();
            if (Gio.Count == 0) throw new LoiNghiepVu("Giỏ hàng đang trống.");
            string ghiChu = KiemTra.Chu(soDonThuoc, "SoDonThuoc", "Số đơn thuốc", 200, false);
            if (CoThuocKeDon && ghiChu == null)
                throw new LoiNhapLieu("SoDonThuoc", "Giỏ hàng có thuốc kê đơn: bắt buộc nhập số đơn thuốc của bác sĩ.");

            int maHD = Ghi(() => _dal.TaoHoaDon(nv.MaNV, maKH, ghiChu, Gio), "lập hóa đơn", "HoaDon",
                Gio.Count + " dòng, " + TongTien.ToString("N0") + " đ" + (maKH == null ? "" : ", MaKH=" + maKH));
            Gio.Clear();
            return maHD;
        }

        public DataTable TimHoaDon(DateTime tu, DateTime den)
        {
            if (tu.Date > den.Date) throw new LoiNhapLieu("TuNgay", "Từ ngày phải trước đến ngày.");
            return Doc(() => _dal.SearchHoaDon(tu, den), "tìm hóa đơn");
        }

        public DataTable ChiTietHoaDon(int maHD) { return Doc(() => _dal.ChiTietHoaDon(maHD), "xem chi tiết hóa đơn"); }
    }

    // =====================================================================
    public class NhapHangBUS : BusCoSo, IGioHang<DongNhapHang>
    {
        private readonly GiaoDichDAL _dal = new GiaoDichDAL();

        public List<DongNhapHang> Dong { get; private set; }

        public NhapHangBUS() { Dong = new List<DongNhapHang>(); }

        public decimal TongTien { get { return Dong.Sum(d => d.ThanhTien); } }

        // ---- IGioHang ----
        IList<DongNhapHang> IGioHang<DongNhapHang>.DanhSach { get { return Dong; } }
        public void XoaDong(string maThuoc) { Dong.RemoveAll(d => d.MaThuoc == maThuoc); }
        public void LamTrong() { Dong.Clear(); }

        public void ThemDong(string maThuoc, string tenThuoc, int soLuong, decimal donGia, DateTime hanSuDung)
        {
            if (string.IsNullOrEmpty(maThuoc)) throw new LoiNhapLieu("Thuoc", "Chưa chọn thuốc.");
            if (soLuong <= 0) throw new LoiNhapLieu("SoLuong", "Số lượng phải lớn hơn 0.");
            if (donGia < 0) throw new LoiNhapLieu("DonGia", "Đơn giá nhập không được âm.");
            if (hanSuDung.Date <= DateTime.Today) throw new LoiNhapLieu("HanSuDung", "Không nhập lô thuốc đã hết hạn.");
            if (Dong.Any(d => d.MaThuoc == maThuoc)) throw new LoiNghiepVu("Thuốc này đã có trong phiếu, hãy xóa dòng cũ nếu muốn sửa.");
            Dong.Add(new DongNhapHang { MaThuoc = maThuoc, TenThuoc = tenThuoc, SoLuong = soLuong, DonGiaNhap = donGia, HanSuDung = hanSuDung.Date });
        }

        public int LuuPhieu(string maNCC)
        {
            KiemTra.YeuCauQuanLy();
            if (string.IsNullOrEmpty(maNCC)) throw new LoiNhapLieu("NCC", "Chưa chọn nhà cung cấp.");
            if (Dong.Count == 0) throw new LoiNghiepVu("Phiếu nhập chưa có thuốc nào.");
            int maPN = Ghi(() => _dal.TaoPhieuNhap(PhienDangNhap.HienTai.MaNV, maNCC, Dong), "lập phiếu nhập", "PhieuNhap",
                maNCC + ", " + Dong.Count + " dòng, " + TongTien.ToString("N0") + " đ");
            Dong.Clear();
            return maPN;
        }
    }

    // =====================================================================
    public class ThongKeBUS : BusCoSo
    {
        public const int NguongTon = 20;
        public const int SoNgayCanhBao = 90;

        private readonly ThongKeDAL _dal = new ThongKeDAL();

        public ThongKeTongQuan TongQuan() { return Doc(() => _dal.TongQuan(), "thống kê tổng quan"); }
        public DataTable CanhBao() { return Doc(() => _dal.CanhBao(NguongTon, SoNgayCanhBao), "cảnh báo kho"); }

        public DataTable DoanhThuTheoNgay(DateTime tu, DateTime den)
        {
            KiemTra.YeuCauQuanLy();
            if (tu.Date > den.Date) throw new LoiNhapLieu("TuNgay", "Từ ngày phải trước đến ngày.");
            return Doc(() => _dal.DoanhThuTheoNgay(tu, den), "thống kê doanh thu");
        }

        public DataTable TopThuoc(DateTime tu, DateTime den)
        {
            KiemTra.YeuCauQuanLy();
            return Doc(() => _dal.TopThuoc(tu, den), "thống kê thuốc bán chạy");
        }
    }
}
