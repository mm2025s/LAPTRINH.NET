using System;

namespace Lab07_ADONET_Connection.DTO
{
    /// <summary>
    /// Khoa - anh xa bang dbo.Khoa.
    /// DTO chi chua du lieu, khong co logic nghiep vu, khong biet CSDL.
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

    /// <summary>SinhVien - anh xa bang dbo.SinhVien.</summary>
    public class SinhVien
    {
        public string MaSV { get; set; }
        public string HoTen { get; set; }
        public DateTime NgaySinh { get; set; }
        public bool GioiTinh { get; set; }          // true: Nam, false: Nu
        public string MaKhoa { get; set; }
        public decimal DiemTB { get; set; }
        public string Email { get; set; }

        public string GioiTinhText { get { return GioiTinh ? "Nam" : "Nữ"; } }
    }

    /// <summary>HocPhan - anh xa bang dbo.HocPhan.</summary>
    public class HocPhan
    {
        public string MaHP { get; set; }
        public string TenHP { get; set; }
        public int SoTinChi { get; set; }
    }

    /// <summary>DangKy - anh xa bang dbo.DangKy (bao gom ten SV/HP de hien thi).</summary>
    public class DangKy
    {
        public string MaSV { get; set; }
        public string MaHP { get; set; }
        public int HocKy { get; set; }
        public decimal? Diem { get; set; }
        public DateTime NgayDangKy { get; set; }

        // Tien ich hien thi (lay tu JOIN, khong luu trong bang DangKy)
        public string HoTen { get; set; }
        public string TenHP { get; set; }
    }

    /// <summary>
    /// Ket qua mot lan kiem tra ket noi - dung cho trang trang thai ket noi (bai 9).
    /// </summary>
    public class TrangThaiKetNoi
    {
        public bool ThanhCong { get; set; }
        public string ThongBao { get; set; }
        public string DataSource { get; set; }
        public string Database { get; set; }
        public string ServerVersion { get; set; }
        public string State { get; set; }
        public long ThoiGianKetNoiMs { get; set; }
        public string ChuoiKetNoi { get; set; }
        public string NguonChuoiKetNoi { get; set; }   // App.config hoac mac dinh
        public DateTime ThoiDiem { get; set; }

        public TrangThaiKetNoi()
        {
            DataSource = string.Empty;
            Database = string.Empty;
            ServerVersion = string.Empty;
            State = "Closed";
            ThongBao = string.Empty;
            ChuoiKetNoi = string.Empty;
            NguonChuoiKetNoi = string.Empty;
            ThoiDiem = DateTime.Now;
        }
    }

    /// <summary>
    /// Ngoai le nghiep vu mang ten truong bi sai - GUI bat va hien thi.
    /// (Giu dung quy uoc cua Lab 6 de dong bo chuoi bai lab.)
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
