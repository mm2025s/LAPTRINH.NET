using System;

namespace QuanLyNhaThuoc.DTO
{
    /// <summary>Nhan vien dang nhap he thong. Khong chua mat khau dang ro.</summary>
    public class NhanVienDTO : DoiTuongCoSo
    {
        public override string KhoaChinh { get { return MaNV.ToString(); } }
        public override string MoTa() { return TenDangNhap + " - " + HoTen; }

        public int MaNV { get; set; }
        public string HoTen { get; set; }
        public string TenDangNhap { get; set; }
        public string VaiTro { get; set; }          // "QuanLy" | "DuocSi"
        public bool IsActive { get; set; }

        public bool LaQuanLy { get { return VaiTro == VaiTroNV.QuanLy; } }
        public string TenVaiTro { get { return LaQuanLy ? "Quản lý" : "Dược sĩ"; } }
    }

    public static class VaiTroNV
    {
        public const string QuanLy = "QuanLy";
        public const string DuocSi = "DuocSi";
    }

    public class ThuocDTO : DoiTuongCoSo
    {
        public override string KhoaChinh { get { return MaThuoc; } }
        public override string MoTa() { return MaThuoc + " - " + TenThuoc; }

        public string MaThuoc { get; set; }
        public string TenThuoc { get; set; }
        public string MaNhom { get; set; }
        public string DonViTinh { get; set; }
        public decimal GiaBan { get; set; }
        public int TonKho { get; set; }
        public DateTime? HanSuDung { get; set; }
        public bool CanKeDon { get; set; }
    }

    public class NhomThuocDTO : DoiTuongCoSo
    {
        public override string KhoaChinh { get { return MaNhom; } }
        public override string MoTa() { return MaNhom + " - " + TenNhom; }

        public string MaNhom { get; set; }
        public string TenNhom { get; set; }
    }

    public class NhaCungCapDTO : DoiTuongCoSo
    {
        public override string KhoaChinh { get { return MaNCC; } }
        public override string MoTa() { return MaNCC + " - " + TenNCC; }

        public string MaNCC { get; set; }
        public string TenNCC { get; set; }
        public string DienThoai { get; set; }
        public string DiaChi { get; set; }
    }

    public class KhachHangDTO : DoiTuongCoSo
    {
        public override string KhoaChinh { get { return MaKH.HasValue ? MaKH.Value.ToString() : null; } }
        public override string MoTa() { return HoTen + " (" + DienThoai + ")"; }

        public int? MaKH { get; set; }
        public string HoTen { get; set; }
        public string DienThoai { get; set; }
        public string DiaChi { get; set; }
    }

    /// <summary>Mot dong trong gio hang cua man hinh ban hang.</summary>
    public class DongGioHang
    {
        public string MaThuoc { get; set; }
        public string TenThuoc { get; set; }
        public string DonViTinh { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public bool CanKeDon { get; set; }
        public decimal ThanhTien { get { return SoLuong * DonGia; } }
    }

    /// <summary>Mot dong cua phieu nhap hang.</summary>
    public class DongNhapHang
    {
        public string MaThuoc { get; set; }
        public string TenThuoc { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGiaNhap { get; set; }
        public DateTime HanSuDung { get; set; }
        public decimal ThanhTien { get { return SoLuong * DonGiaNhap; } }
    }

    public class ThongKeTongQuan
    {
        public decimal DoanhThuHomNay { get; set; }
        public int SoHoaDonHomNay { get; set; }
        public decimal DoanhThuThang { get; set; }
        public int SoThuoc { get; set; }
        public int SoSapHetHang { get; set; }
        public int SoSapHetHan { get; set; }
    }

    /// <summary>Phien lam viec cua nhan vien dang dang nhap.</summary>
    public static class PhienDangNhap
    {
        public static NhanVienDTO HienTai { get; set; }
    }
}
