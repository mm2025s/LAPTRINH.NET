using System;
using System.Collections.Generic;
using System.Globalization;
using Lab11_CRUD_StoredProcedure.DAL;
using Lab11_CRUD_StoredProcedure.DTO;

namespace Lab11_CRUD_StoredProcedure.BUS
{
    /// <summary>Mot tinh huong gay loi THAT, dung chung cho Form va --kiemtra.</summary>
    public class TinhHuongLoi
    {
        public string Ten { get; set; }
        public LoaiLoi Mong { get; set; }
        public Action Lenh { get; set; }

        /// <summary>
        /// Khong tinh huong nao lam doi du lieu: moi lenh ghi deu bi CSDL tu choi.
        /// </summary>
        public static List<TinhHuongLoi> DanhSach()
        {
            var dal = new ThuTucDAL();
            return new List<TinhHuongLoi>
            {
                Tao("Điểm nhập chữ", LoaiLoi.NhapLieu, () => DocDiem("abc")),
                Tao("Điểm ngoài [0,10]", LoaiLoi.NhapLieu, () => DocDiem("11")),
                Tao("Thêm mã SV trùng (THROW 50001)", LoaiLoi.NghiepVu,
                    () => XuLyLoi.Chay(() => dal.ThemSinhVien("SV001", "Thử", "CNTT", null), "thêm sinh viên")),
                Tao("Xóa khoa đang có SV (THROW 50007)", LoaiLoi.NghiepVu,
                    () => XuLyLoi.Chay(() => dal.XoaKhoa("CNTT"), "xóa khoa")),
                Tao("Email trùng, UNIQUE INDEX chặn (2601)", LoaiLoi.NghiepVu,
                    () => XuLyLoi.Chay(() => dal.ThemSinhVien("TS1100", "Thử", "CNTT", "an@eaut.edu.vn"), "thêm sinh viên")),
                Tao("Thủ tục không tồn tại (2812)", LoaiLoi.Sql,
                    () => XuLyLoi.Chay(() => dal.GoiThuTucKhongTonTai(), "gọi thủ tục")),
                Tao("Kết nối sai CSDL (4060)", LoaiLoi.Sql,
                    () => XuLyLoi.Chay(() => { dal.MoKetNoiSaiCsdl(); return 0; }, "mở kết nối")),
                Tao("Lỗi ngoài dự kiến", LoaiLoi.HeThong,
                    () => { throw new InvalidOperationException("Lỗi mô phỏng cho bài 10"); })
            };
        }

        private static TinhHuongLoi Tao(string ten, LoaiLoi mong, Action lenh)
        {
            return new TinhHuongLoi { Ten = ten, Mong = mong, Lenh = lenh };
        }

        private static void DocDiem(string chuoi)
        {
            decimal diem;
            if (!decimal.TryParse(chuoi, NumberStyles.Number, CultureInfo.InvariantCulture, out diem))
                throw new LoiNhapLieu("Điểm phải là số, ví dụ 8.25.");
            if (diem < 0 || diem > 10)
                throw new LoiNhapLieu("Điểm phải thuộc [0,10].");
        }
    }
}
