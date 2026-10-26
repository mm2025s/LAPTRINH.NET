using System;
using System.Collections.Generic;

namespace Lab06_ThreeLayer.DTO
{
    /// <summary>
    /// Đối tượng truyền dữ liệu SinhVien - dùng chung cho cả 3 lớp.
    /// (Trong mô hình 3 lớp cổ điển, DTO thuộc lớp DTO được GUI/BUS/DAL cùng tham chiếu.)
    /// </summary>
    public class SinhVien
    {
        public string MaSV { get; set; }
        public string HoTen { get; set; }
        public DateTime NgaySinh { get; set; }
        public string GioiTinh { get; set; }
        public string Khoa { get; set; }
        public double DiemTB { get; set; }
        public string Email { get; set; }

        /// <summary>Loại sinh viên ảnh hưởng biểu tượng ListView (Bài 8).</summary>
        public string LoaiSinhVien { get; set; }

        public SinhVien()
        {
            MaSV = string.Empty;
            HoTen = string.Empty;
            NgaySinh = DateTime.Today.AddYears(-19);
            GioiTinh = "Nam";
            Khoa = string.Empty;
            Email = string.Empty;
            LoaiSinhVien = "Thường";
        }

        public SinhVien(string maSV, string hoTen, DateTime ngaySinh, double diemTB, string khoa)
        {
            MaSV = maSV;
            HoTen = hoTen;
            NgaySinh = ngaySinh;
            DiemTB = diemTB;
            Khoa = khoa;
            GioiTinh = "Nam";
            Email = string.Empty;
            LoaiSinhVien = "Thường";
        }

        public SinhVien Clone()
        {
            return new SinhVien
            {
                MaSV = MaSV,
                HoTen = HoTen,
                NgaySinh = NgaySinh,
                GioiTinh = GioiTinh,
                Khoa = Khoa,
                DiemTB = DiemTB,
                Email = Email,
                LoaiSinhVien = LoaiSinhVien
            };
        }

        public override string ToString()
        {
            return string.Format("{0} - {1} - {2} - {3:0.0}", MaSV, HoTen, Khoa, DiemTB);
        }
    }

    /// <summary>Khoa (dùng cho ComboBox, TreeView bài 4, TreeView bài 7).</summary>
    public class Khoa
    {
        public string MaKhoa { get; set; }
        public string TenKhoa { get; set; }

        public Khoa() { MaKhoa = string.Empty; TenKhoa = string.Empty; }

        public Khoa(string maKhoa, string tenKhoa)
        {
            MaKhoa = maKhoa;
            TenKhoa = tenKhoa;
        }

        public override string ToString()
        {
            return TenKhoa;
        }
    }

    /// <summary>Danh mục sản phẩm (Bài 6, Bài 7).</summary>
    public class SanPham
    {
        public string MaSP { get; set; }
        public string TenSP { get; set; }
        public string MaDanhMuc { get; set; }
        public double DonGia { get; set; }
        public int SoLuongTon { get; set; }

        public SanPham()
        {
            MaSP = string.Empty;
            TenSP = string.Empty;
            MaDanhMuc = string.Empty;
        }

        public SanPham(string maSP, string tenSP, string maDanhMuc, double donGia, int soLuongTon)
        {
            MaSP = maSP;
            TenSP = tenSP;
            MaDanhMuc = maDanhMuc;
            DonGia = donGia;
            SoLuongTon = soLuongTon;
        }

        /// <summary>Giá trị hàng tồn = đơn giá × số lượng tồn.</summary>
        public double GiaTriTon
        {
            get { return DonGia * SoLuongTon; }
        }

        public SanPham Clone()
        {
            return new SanPham(MaSP, TenSP, MaDanhMuc, DonGia, SoLuongTon);
        }

        public override string ToString()
        {
            return MaSP + " - " + TenSP;
        }
    }

    /// <summary>Danh mục (cha của sản phẩm trong TreeView).</summary>
    public class DanhMuc
    {
        public string MaDanhMuc { get; set; }
        public string TenDanhMuc { get; set; }

        public DanhMuc() { MaDanhMuc = string.Empty; TenDanhMuc = string.Empty; }

        public DanhMuc(string ma, string ten)
        {
            MaDanhMuc = ma;
            TenDanhMuc = ten;
        }

        public override string ToString()
        {
            return TenDanhMuc;
        }
    }

    /// <summary>Kết quả nhập/xuất CSV để báo cáo lên giao diện.</summary>
    public class KetQuaCSV
    {
        public int SoDong { get; set; }
        public string DuongDan { get; set; }
        public DateTime ThoiDiem { get; set; }
        public List<object> ChiTietLoi { get; set; }

        public KetQuaCSV()
        {
            DuongDan = string.Empty;
            ThoiDiem = DateTime.Now;
            ChiTietLoi = new List<object>();
        }
    }

    /// <summary>Lỗi nghiệp vụ do tầng BUS phát ra (DAL không tự kiểm tra nghiệp vụ).</summary>
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
