using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QuanLyNhaThuoc.DTO;

namespace QuanLyNhaThuoc.DAL
{
    public class NhanVienDAL
    {
        /// <summary>Lay thong tin dang nhap; out hash + salt de BUS so khop.</summary>
        public NhanVienDTO LayDeDangNhap(string tenDangNhap, out byte[] hash, out string salt)
        {
            NhanVienDTO nv = null;
            byte[] h = null;
            string s = null;
            Db.Doc("sp_NhanVien_LayDeDangNhap", rd =>
            {
                nv = new NhanVienDTO
                {
                    MaNV = rd.GetInt32(0), HoTen = rd.GetString(1), TenDangNhap = rd.GetString(2),
                    VaiTro = rd.GetString(5), IsActive = rd.GetBoolean(6)
                };
                h = (byte[])rd[3];
                s = rd.GetString(4);
            }, Db.P("@TenDangNhap", SqlDbType.VarChar, tenDangNhap, 30));
            hash = h;
            salt = s;
            return nv;
        }

        public DataTable GetAll() { return Db.Bang("sp_NhanVien_GetAll"); }

        public int Insert(NhanVienDTO nv, byte[] hash, string salt)
        {
            return Db.ChayLenh("sp_NhanVien_Insert",
                Db.P("@HoTen", SqlDbType.NVarChar, nv.HoTen, 100),
                Db.P("@TenDangNhap", SqlDbType.VarChar, nv.TenDangNhap, 30),
                Db.P("@MatKhauHash", SqlDbType.VarBinary, hash, 32),
                Db.P("@Salt", SqlDbType.NVarChar, salt, 32),
                Db.P("@VaiTro", SqlDbType.VarChar, nv.VaiTro, 10));
        }

        public int Update(NhanVienDTO nv)
        {
            return Db.ChayLenh("sp_NhanVien_Update",
                Db.P("@MaNV", SqlDbType.Int, nv.MaNV),
                Db.P("@HoTen", SqlDbType.NVarChar, nv.HoTen, 100),
                Db.P("@VaiTro", SqlDbType.VarChar, nv.VaiTro, 10),
                Db.P("@IsActive", SqlDbType.Bit, nv.IsActive));
        }

        public int DoiMatKhau(int maNV, byte[] hash, string salt)
        {
            return Db.ChayLenh("sp_NhanVien_DoiMatKhau",
                Db.P("@MaNV", SqlDbType.Int, maNV),
                Db.P("@MatKhauHash", SqlDbType.VarBinary, hash, 32),
                Db.P("@Salt", SqlDbType.NVarChar, salt, 32));
        }
    }

    public class DanhMucDAL
    {
        public DataTable GetNhomThuoc() { return Db.Bang("sp_NhomThuoc_GetAll"); }

        public int SaveNhomThuoc(NhomThuocDTO n, bool laThemMoi)
        {
            return Db.ChayLenh("sp_NhomThuoc_Save",
                Db.P("@MaNhom", SqlDbType.VarChar, n.MaNhom, 10),
                Db.P("@TenNhom", SqlDbType.NVarChar, n.TenNhom, 100),
                Db.P("@LaThemMoi", SqlDbType.Bit, laThemMoi));
        }

        public int DeleteNhomThuoc(string ma)
        {
            return Db.ChayLenh("sp_NhomThuoc_Delete", Db.P("@MaNhom", SqlDbType.VarChar, ma, 10));
        }

        public DataTable GetNhaCungCap() { return Db.Bang("sp_NhaCungCap_GetAll"); }

        public int SaveNhaCungCap(NhaCungCapDTO n, bool laThemMoi)
        {
            return Db.ChayLenh("sp_NhaCungCap_Save",
                Db.P("@MaNCC", SqlDbType.VarChar, n.MaNCC, 10),
                Db.P("@TenNCC", SqlDbType.NVarChar, n.TenNCC, 150),
                Db.P("@DienThoai", SqlDbType.VarChar, n.DienThoai, 15),
                Db.P("@DiaChi", SqlDbType.NVarChar, n.DiaChi, 200),
                Db.P("@LaThemMoi", SqlDbType.Bit, laThemMoi));
        }

        public int DeleteNhaCungCap(string ma)
        {
            return Db.ChayLenh("sp_NhaCungCap_Delete", Db.P("@MaNCC", SqlDbType.VarChar, ma, 10));
        }
    }

    /// <summary>
    /// Lop co so truu tuong cho DAL co CRUD day du: ke thua ICrudDAL, lop con chi cai dat
    /// 4 thao tac bang Stored Procedure cua minh.
    /// </summary>
    public abstract class DalCoSo<T> : ICrudDAL<T> where T : IDoiTuong
    {
        public abstract DataTable TimKiem(string tuKhoa);
        public abstract int Them(T doiTuong);
        public abstract int Sua(T doiTuong);
        public abstract int Xoa(string khoaChinh);
    }

    public class ThuocDAL : DalCoSo<ThuocDTO>
    {
        public override DataTable TimKiem(string tuKhoa) { return Search(tuKhoa, null); }
        public override int Them(ThuocDTO t) { return Insert(t); }
        public override int Sua(ThuocDTO t) { return Update(t); }
        public override int Xoa(string ma) { return Delete(ma); }

        public DataTable Search(string tuKhoa, string maNhom)
        {
            return Db.Bang("sp_Thuoc_Search",
                Db.P("@TuKhoa", SqlDbType.NVarChar, tuKhoa, 100),
                Db.P("@MaNhom", SqlDbType.VarChar, maNhom, 10));
        }

        public int Insert(ThuocDTO t) { return Db.ChayLenh("sp_Thuoc_Insert", ThamSo(t)); }
        public int Update(ThuocDTO t) { return Db.ChayLenh("sp_Thuoc_Update", ThamSo(t)); }

        public int Delete(string maThuoc)
        {
            return Db.ChayLenh("sp_Thuoc_Delete", Db.P("@MaThuoc", SqlDbType.VarChar, maThuoc, 15));
        }

        private static SqlParameter[] ThamSo(ThuocDTO t)
        {
            return new[]
            {
                Db.P("@MaThuoc", SqlDbType.VarChar, t.MaThuoc, 15),
                Db.P("@TenThuoc", SqlDbType.NVarChar, t.TenThuoc, 150),
                Db.P("@MaNhom", SqlDbType.VarChar, t.MaNhom, 10),
                Db.P("@DonViTinh", SqlDbType.NVarChar, t.DonViTinh, 20),
                Db.P("@GiaBan", SqlDbType.Decimal, t.GiaBan),
                Db.P("@CanKeDon", SqlDbType.Bit, t.CanKeDon)
            };
        }
    }

    public class KhachHangDAL : DalCoSo<KhachHangDTO>
    {
        public override DataTable TimKiem(string tuKhoa) { return Search(tuKhoa); }
        public override int Them(KhachHangDTO k) { k.MaKH = null; return Save(k); }
        public override int Sua(KhachHangDTO k) { return Save(k); }
        public override int Xoa(string ma) { return Delete(int.Parse(ma)); }

        public DataTable Search(string tuKhoa)
        {
            return Db.Bang("sp_KhachHang_Search", Db.P("@TuKhoa", SqlDbType.NVarChar, tuKhoa, 100));
        }

        /// <summary>Them (MaKH null) hoac sua; tra ve MaKH qua tham so OUTPUT.</summary>
        public int Save(KhachHangDTO k)
        {
            SqlParameter ra = Db.Output("@MaKHMoi", SqlDbType.Int);
            Db.ChayLenh("sp_KhachHang_Save",
                Db.P("@MaKH", SqlDbType.Int, k.MaKH),
                Db.P("@HoTen", SqlDbType.NVarChar, k.HoTen, 100),
                Db.P("@DienThoai", SqlDbType.VarChar, k.DienThoai, 15),
                Db.P("@DiaChi", SqlDbType.NVarChar, k.DiaChi, 200),
                ra);
            return (int)ra.Value;
        }

        public int Delete(int maKH)
        {
            return Db.ChayLenh("sp_KhachHang_Delete", Db.P("@MaKH", SqlDbType.Int, maKH));
        }
    }

    public class GiaoDichDAL
    {
        /// <summary>
        /// Lap hoa don: ca gio hang di trong MOT tham so kieu bang, thu tuc
        /// xu ly trong mot transaction nen khong bao gio co hoa don nua voi.
        /// </summary>
        public int TaoHoaDon(int maNV, int? maKH, string ghiChu, IEnumerable<DongGioHang> gio)
        {
            var bang = new DataTable();
            bang.Columns.Add("MaThuoc", typeof(string));
            bang.Columns.Add("SoLuong", typeof(int));
            foreach (DongGioHang d in gio) bang.Rows.Add(d.MaThuoc, d.SoLuong);

            SqlParameter ra = Db.Output("@MaHD", SqlDbType.Int);
            Db.ChayLenh("sp_HoaDon_Tao",
                Db.P("@MaNV", SqlDbType.Int, maNV),
                Db.P("@MaKH", SqlDbType.Int, maKH),
                Db.P("@GhiChu", SqlDbType.NVarChar, ghiChu, 200),
                Db.Bang("@ChiTiet", "dbo.DongHoaDonType", bang),
                ra);
            return (int)ra.Value;
        }

        public int TaoPhieuNhap(int maNV, string maNCC, IEnumerable<DongNhapHang> dong)
        {
            var bang = new DataTable();
            bang.Columns.Add("MaThuoc", typeof(string));
            bang.Columns.Add("SoLuong", typeof(int));
            bang.Columns.Add("DonGiaNhap", typeof(decimal));
            bang.Columns.Add("HanSuDung", typeof(DateTime));
            foreach (DongNhapHang d in dong) bang.Rows.Add(d.MaThuoc, d.SoLuong, d.DonGiaNhap, d.HanSuDung.Date);

            SqlParameter ra = Db.Output("@MaPN", SqlDbType.Int);
            Db.ChayLenh("sp_PhieuNhap_Tao",
                Db.P("@MaNV", SqlDbType.Int, maNV),
                Db.P("@MaNCC", SqlDbType.VarChar, maNCC, 10),
                Db.Bang("@ChiTiet", "dbo.DongPhieuNhapType", bang),
                ra);
            return (int)ra.Value;
        }

        public DataTable SearchHoaDon(DateTime tu, DateTime den)
        {
            return Db.Bang("sp_HoaDon_Search",
                Db.P("@TuNgay", SqlDbType.Date, tu.Date), Db.P("@DenNgay", SqlDbType.Date, den.Date));
        }

        public DataTable ChiTietHoaDon(int maHD)
        {
            return Db.Bang("sp_HoaDon_ChiTiet", Db.P("@MaHD", SqlDbType.Int, maHD));
        }
    }

    /// <summary>Ghi nhat ky nguoi dung vao bang NhatKy (cai dat IGhiNhatKy).</summary>
    public class NhatKyDAL : IGhiNhatKy
    {
        public void Ghi(NhanVienDTO nv, string tenDangNhap, string hanhDong, string doiTuong, string chiTiet, bool thanhCong, int ms)
        {
            Db.ChayLenh("sp_NhatKy_Ghi",
                Db.P("@MaNV", SqlDbType.Int, nv == null ? (object)null : nv.MaNV),
                Db.P("@TenDangNhap", SqlDbType.VarChar, nv != null ? nv.TenDangNhap : tenDangNhap, 30),
                Db.P("@HanhDong", SqlDbType.NVarChar, hanhDong, 60),
                Db.P("@DoiTuong", SqlDbType.NVarChar, doiTuong, 40),
                Db.P("@ChiTiet", SqlDbType.NVarChar, chiTiet, 300),
                Db.P("@ThanhCong", SqlDbType.Bit, thanhCong),
                Db.P("@ThoiGianMs", SqlDbType.Int, ms));
        }

        public DataTable Search(DateTime tu, DateTime den, string tuKhoa)
        {
            return Db.Bang("sp_NhatKy_Search",
                Db.P("@TuNgay", SqlDbType.Date, tu.Date), Db.P("@DenNgay", SqlDbType.Date, den.Date),
                Db.P("@TuKhoa", SqlDbType.NVarChar, string.IsNullOrWhiteSpace(tuKhoa) ? null : tuKhoa.Trim(), 100));
        }
    }

    public class ThongKeDAL
    {
        public DataTable HieuNangNhanVien(DateTime tu, DateTime den)
        {
            return Db.Bang("sp_ThongKe_HieuNangNhanVien",
                Db.P("@TuNgay", SqlDbType.Date, tu.Date), Db.P("@DenNgay", SqlDbType.Date, den.Date));
        }

        public DataTable HieuNangThaoTac(DateTime tu, DateTime den)
        {
            return Db.Bang("sp_ThongKe_HieuNangThaoTac",
                Db.P("@TuNgay", SqlDbType.Date, tu.Date), Db.P("@DenNgay", SqlDbType.Date, den.Date));
        }

        public ThongKeTongQuan TongQuan()
        {
            var tk = new ThongKeTongQuan();
            Db.Doc("sp_ThongKe_TongQuan", rd =>
            {
                tk.DoanhThuHomNay = rd.GetDecimal(0);
                tk.SoHoaDonHomNay = rd.GetInt32(1);
                tk.DoanhThuThang = rd.GetDecimal(2);
                tk.SoThuoc = rd.GetInt32(3);
                tk.SoSapHetHang = rd.GetInt32(4);
                tk.SoSapHetHan = rd.GetInt32(5);
            });
            return tk;
        }

        public DataTable DoanhThuTheoNgay(DateTime tu, DateTime den)
        {
            return Db.Bang("sp_ThongKe_DoanhThuTheoNgay",
                Db.P("@TuNgay", SqlDbType.Date, tu.Date), Db.P("@DenNgay", SqlDbType.Date, den.Date));
        }

        public DataTable TopThuoc(DateTime tu, DateTime den)
        {
            return Db.Bang("sp_ThongKe_TopThuoc",
                Db.P("@TuNgay", SqlDbType.Date, tu.Date), Db.P("@DenNgay", SqlDbType.Date, den.Date),
                Db.P("@SoLuong", SqlDbType.Int, 10));
        }

        public DataTable CanhBao(int nguongTon, int soNgay)
        {
            return Db.Bang("sp_ThongKe_CanhBao",
                Db.P("@NguongTon", SqlDbType.Int, nguongTon), Db.P("@SoNgay", SqlDbType.Int, soNgay));
        }
    }
}
