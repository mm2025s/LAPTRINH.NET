using System;
using System.Collections.Generic;

namespace Lab08_SqlCommand_Parameters.DTO
{
    /// <summary>
    /// Khoa - anh xa bang dbo.Khoa.
    /// DTO chi giu du lieu, khong co logic nghiep vu, khong biet CSDL.
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
    /// SinhVien - anh xa bang dbo.SinhVien (co them TenKhoa khi JOIN).
    /// </summary>
    public class SinhVien
    {
        public string MaSV { get; set; }
        public string HoTen { get; set; }
        public DateTime NgaySinh { get; set; }
        public bool GioiTinh { get; set; }          // true: Nam, false: Nu
        public string MaKhoa { get; set; }
        public decimal DiemTB { get; set; }
        public string Email { get; set; }

        // Tien ich hien thi (lay tu JOIN, khong luu trong bang)
        public string TenKhoa { get; set; }

        public string GioiTinhText { get { return GioiTinh ? "Nam" : "Nữ"; } }

        /// <summary>Dong hien thi cho ListBox cua bai 2: "MaSV - HoTen - DiemTB".</summary>
        public override string ToString()
        {
            return string.Format("{0}  -  {1}  -  {2:0.00}", MaSV, HoTen, DiemTB);
        }
    }

    /// <summary>
    /// Ket qua thong ke diem cua mot khoa (bai 7).
    /// DiemCaoNhat / DiemTrungBinh de kieu nullable vi ham MAX/AVG tra ve NULL
    /// khi khoa chua co sinh vien nao.
    /// </summary>
    public class ThongKeKhoa
    {
        public string MaKhoa { get; set; }
        public string TenKhoa { get; set; }
        public int SoSinhVien { get; set; }
        public decimal? DiemCaoNhat { get; set; }
        public decimal? DiemTrungBinh { get; set; }

        public string DiemCaoNhatText
        {
            get { return DiemCaoNhat.HasValue ? DiemCaoNhat.Value.ToString("0.00") : "— (NULL)"; }
        }

        public string DiemTrungBinhText
        {
            get { return DiemTrungBinh.HasValue ? DiemTrungBinh.Value.ToString("0.00") : "— (NULL)"; }
        }
    }

    /// <summary>
    /// Ket qua mot thao tac ghi du lieu (INSERT / UPDATE / DELETE),
    /// de GUI chi hien thi ma khong phai tu ghep thong bao.
    /// </summary>
    public class KetQuaThaoTac
    {
        public bool ThanhCong { get { return SoDong > 0; } }
        public int SoDong { get; set; }
        public string ThongBao { get; set; }

        /// <summary>Gia tri tham so OUTPUT (bai 9: so luong sinh vien sau khi them).</summary>
        public int? GiaTriOutput { get; set; }

        public KetQuaThaoTac()
        {
            ThongBao = string.Empty;
        }

        public static KetQuaThaoTac TuSoDong(int soDong, string thanhCong, string thatBai)
        {
            return new KetQuaThaoTac
            {
                SoDong = soDong,
                ThongBao = soDong > 0 ? thanhCong : thatBai
            };
        }
    }

    /// <summary>
    /// Ngoai le nghiep vu mang ten truong bi sai - GUI bat va hien thi
    /// ngay duoi o nhap tuong ung (giu quy uoc cua Lab 6, 7).
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
