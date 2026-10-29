using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Lab07_ADONET_Connection.DAL;
using Lab07_ADONET_Connection.DTO;

namespace Lab07_ADONET_Connection.BUS
{
    /// <summary>
    /// Nghiep vu ket noi (bai 5, 7, 9).
    ///
    /// KiemTraKetNoi mo ket noi that, do thoi gian, doc cac thuoc tinh
    /// cua SqlConnection (DataSource, Database, ServerVersion, State) va
    /// tra ve DTO.TrangThaiKetNoi. GUI chi hien thi, khong tu mo ket noi.
    /// </summary>
    public class KetNoiBUS
    {
        /// <summary>Kiem tra bang chuoi ket noi dang cau hinh (App.config / mac dinh).</summary>
        public TrangThaiKetNoi KiemTraKetNoi()
        {
            return KiemTraVoiChuoi(DbConnectionFactory.ChuoiKetNoi, DbConnectionFactory.NguonChuoiKetNoi);
        }

        /// <summary>Kiem tra voi chuoi ket noi tuong minh (nguoi dung nhap tren Form).</summary>
        public TrangThaiKetNoi KiemTraVoiChuoi(string chuoiKetNoi, string nguon)
        {
            var tt = new TrangThaiKetNoi();
            tt.ChuoiKetNoi = chuoiKetNoi;
            tt.NguonChuoiKetNoi = nguon;

            DateTime batDau = DateTime.Now;
            try
            {
                using (SqlConnection conn = DbConnectionFactory.Create(chuoiKetNoi))
                {
                    conn.Open();
                    tt.ThoiGianKetNoiMs = (long)(DateTime.Now - batDau).TotalMilliseconds;

                    tt.ThanhCong = true;
                    tt.DataSource = conn.DataSource;
                    tt.Database = conn.Database;
                    tt.ServerVersion = conn.ServerVersion;
                    tt.State = conn.State.ToString();
                    tt.ThoiDiem = DateTime.Now;
                    tt.ThongBao = "Ket noi thanh cong: " + conn.Database;
                }
            }
            catch (SqlException ex)
            {
                tt.ThoiGianKetNoiMs = (long)(DateTime.Now - batDau).TotalMilliseconds;
                tt.ThanhCong = false;
                tt.State = "Closed (loi)";
                tt.ThoiDiem = DateTime.Now;
                tt.ThongBao = "Loi ket noi (SqlException so " + ex.Number + "): " + ex.Message;
            }
            catch (Exception ex)
            {
                tt.ThoiGianKetNoiMs = (long)(DateTime.Now - batDau).TotalMilliseconds;
                tt.ThanhCong = false;
                tt.State = "Closed (loi)";
                tt.ThoiDiem = DateTime.Now;
                tt.ThongBao = "Loi ket noi: " + ex.Message;
            }

            return tt;
        }

        /// <summary>Kiem tra bang SQL Authentication (bai 7).</summary>
        public TrangThaiKetNoi KiemTraSqlAuth(string mayChu, string csdl, string taiKhoan, string matKhau)
        {
            string chuoi = DbConnectionFactory.TaoChuoiSqlAuth(mayChu, csdl, taiKhoan, matKhau);
            return KiemTraVoiChuoi(chuoi, "SQL Authentication (" + taiKhoan + "@" + mayChu + ")");
        }
    }

    /// <summary>
    /// Nghiep vu truy van du lieu mau (bai 5).
    /// Form chi goi BUS; BUS goi DAL. Quy tac (loc ten bang, sap xep) dat o day.
    /// </summary>
    public class TruyVanBUS
    {
        private readonly TruyVanDAL _dal;

        public TruyVanBUS() : this(new TruyVanDAL()) { }

        public TruyVanBUS(TruyVanDAL dal)
        {
            _dal = dal;
        }

        public List<Khoa> GetKhoa() { return _dal.GetKhoa(); }
        public List<SinhVien> GetSinhVien() { return _dal.GetSinhVien(); }
        public List<DangKy> GetDangKy() { return _dal.GetDangKy(); }

        /// <summary>Cau SELECT cho tung bang - dat o BUS de Form chi chon bang.</summary>
        public DataTable GetBang(string tenBang)
        {
            string sql = SqlCuaBang(tenBang);
            return _dal.GetBang(sql);
        }

        private static string SqlCuaBang(string tenBang)
        {
            switch (tenBang)
            {
                case "Khoa":
                    return "SELECT MaKhoa, TenKhoa FROM dbo.Khoa ORDER BY MaKhoa";
                case "SinhVien":
                    return "SELECT sv.MaSV, sv.HoTen, sv.NgaySinh, " +
                           "CASE sv.GioiTinh WHEN 1 THEN N'Nam' ELSE N'Nữ' END AS GioiTinh, " +
                           "sv.MaKhoa, k.TenKhoa, sv.DiemTB, sv.Email " +
                           "FROM dbo.SinhVien sv JOIN dbo.Khoa k ON k.MaKhoa = sv.MaKhoa " +
                           "ORDER BY sv.MaSV";
                case "HocPhan":
                    return "SELECT MaHP, TenHP, SoTinChi FROM dbo.HocPhan ORDER BY MaHP";
                case "DangKy":
                    return "SELECT dk.MaSV, sv.HoTen, dk.MaHP, hp.TenHP, dk.HocKy, dk.Diem, dk.NgayDangKy " +
                           "FROM dbo.DangKy dk JOIN dbo.SinhVien sv ON sv.MaSV = dk.MaSV " +
                           "JOIN dbo.HocPhan hp ON hp.MaHP = dk.MaHP ORDER BY dk.MaSV, dk.MaHP";
                default:
                    throw new LoiNghiepVu("tenBang", "Bang khong hop le: " + tenBang);
            }
        }

        public int DemBanGhi(string tenBang) { return _dal.DemBanGhi(tenBang); }

        /// <summary>Doi chieu so lieu cho Dashboard frmMain.</summary>
        public Dictionary<string, int> ThongKeNhieuBang(string[] dsBang)
        {
            var kq = new Dictionary<string, int>();
            foreach (string b in dsBang) kq[b] = _dal.DemBanGhi(b);
            return kq;
        }

        public DataTable GetCotCuaBang(string tenBang) { return _dal.GetCotCuaBang(tenBang); }
        public DataTable GetQuanHe() { return _dal.GetQuanHe(); }
    }
}
