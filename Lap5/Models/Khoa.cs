using System;
using System.Collections.Generic;

namespace Lab05_MultiForm.Models
{
    /// <summary>
    /// Khoa đào tạo - dùng cho ComboBox ở frmSinhVien và frmChonKhoa (Bài 8).
    /// </summary>
    public class Khoa
    {
        public string MaKhoa { get; set; }
        public string TenKhoa { get; set; }

        public Khoa()
        {
            MaKhoa = string.Empty;
            TenKhoa = string.Empty;
        }

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

    /// <summary>Dữ liệu mẫu tĩnh dùng chung (Lab 6 sẽ thay thế bằng tầng DAL).</summary>
    public static class DuLieuMau
    {
        public static List<Khoa> DanhSachKhoa()
        {
            return new List<Khoa>
            {
                new Khoa("CNTT", "Công nghệ thông tin"),
                new Khoa("CK", "Cơ khí"),
                new Khoa("KD", "Kinh tế"),
                new Khoa("DT", "Điện - Điện tử"),
                new Khoa("OTO", "Công nghệ kỹ thuật ô tô"),
                new Khoa("NN", "Ngoại ngữ"),
                new Khoa("XD", "Xây dựng")
            };
        }

        public static List<SinhVien> DanhSachSinhVien()
        {
            var list = new List<SinhVien>();
            list.Add(Tao("SV01", "Nguyễn Văn An", new DateTime(2004, 3, 15), 8.7, "Công nghệ thông tin", "Nam", "Đọc sách, Lập trình"));
            list.Add(Tao("SV02", "Trần Thị Bình", new DateTime(2005, 7, 22), 7.4, "Kinh tế", "Nữ", "Âm nhạc, Du lịch"));
            list.Add(Tao("SV03", "Lê Hoàng Cường", new DateTime(2004, 11, 2), 6.2, "Điện - Điện tử", "Nam", "Thể thao"));
            list.Add(Tao("SV04", "Phạm Thu Dung", new DateTime(2005, 1, 9), 9.1, "Công nghệ thông tin", "Nữ", "Đọc sách, Âm nhạc"));
            list.Add(Tao("SV05", "Hoàng Minh Đức", new DateTime(2003, 9, 18), 4.8, "Cơ khí", "Nam", "Thể thao, Lập trình"));
            return list;
        }

        private static SinhVien Tao(string ma, string ten, DateTime ns, double diem, string khoa, string gioiTinh, string soThich)
        {
            var sv = new SinhVien(ma, ten, ns, diem);
            sv.Khoa = khoa;
            sv.GioiTinh = gioiTinh;
            sv.SoThich = soThich;
            return sv;
        }
    }
}
