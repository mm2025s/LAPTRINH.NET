using System;
using System.Collections.Generic;

namespace Lap3
{
    /// <summary>
    /// BÀI 10: NÂNG CẤP QUẢN LÝ SINH VIÊN (tự thực hiện)
    /// Nhiều loại sinh viên, interface xuất dữ liệu, tìm kiếm và thống kê theo loại.
    /// </summary>
    public static class Bai10_NangCapQuanLySinhVien
    {
        public static void Run()
        {
            Console.WriteLine("--- Bài 10: Nâng cấp quản lý sinh viên ---");
            Console.WriteLine();

            QuanLySinhVienNangCao ql = QuanLySinhVienNangCao.TaoDuLieuMau();

            Console.WriteLine("1) DANH SÁCH SINH VIÊN (đa hình theo loại):");
            ql.InDanhSach();

            Console.WriteLine();
            Console.WriteLine("2) TÌM KIẾM THEO MÃ:");
            string ma = "LT01";
            SinhVienMoRong? timMa = ql.TimTheoMa(ma);
            Console.WriteLine("  Mã \"{0}\": {1}", ma, timMa == null ? "Không tìm thấy" : timMa.MoTa());

            Console.WriteLine();
            Console.WriteLine("3) TÌM KIẾM THEO TÊN:");
            string tuKhoa = "u";
            List<SinhVienMoRong> timTen = ql.TimTheoTen(tuKhoa);
            Console.WriteLine("  Từ khóa chứa \"{0}\": {1} kết quả", tuKhoa, timTen.Count);
            foreach (SinhVienMoRong sv in timTen)
            {
                Console.WriteLine("    " + sv.ToDisplayText() + " (" + sv.LoaiSinhVien + ")");
            }

            Console.WriteLine();
            Console.WriteLine("4) LỌC THEO LOẠI SINH VIÊN:");
            foreach (string loai in new string[] { "Chính quy", "Liên thông", "Vừa làm vừa học" })
            {
                List<SinhVienMoRong> loc = ql.LocTheoLoai(loai);
                Console.WriteLine("  {0,-18}: {1} sinh viên", loai, loc.Count);
            }

            Console.WriteLine();
            Console.WriteLine("5) THỐNG KÊ THEO LOẠI:");
            Dictionary<string, int> thongKe = ql.ThongKeSoLuongTheoLoai();
            int tongSo = 0;
            foreach (KeyValuePair<string, int> kv in thongKe)
            {
                Console.WriteLine("  {0,-18}: {1} sinh viên ({2:0.0}%)",
                    kv.Key, kv.Value, kv.Value * 100.0 / ql.SoLuong);
                tongSo += kv.Value;
            }
            Console.WriteLine("  {0,-18}: {1} sinh viên", "TỔNG CỘNG", tongSo);

            Console.WriteLine();
            Console.WriteLine("6) TỔNG HỌC PHÍ (tính đa hình): {0:N0} đ", ql.TongHocPhi());

            Console.WriteLine();
            Console.WriteLine("7) SẮP XẾP THEO ĐIỂM GIẢM DẦN:");
            ql.SapXepTheoDiem();
            ql.InDanhSach();

            Console.WriteLine();
            Console.WriteLine("8) XUẤT DỮ LIỆU RA CSV (qua interface IXuatDuLieu):");
            Console.Write(ql.XuatCsv());

            Console.WriteLine();
            Console.WriteLine("9) XUẤT DỮ LIỆU RA JSON (qua interface IXuatDuLieu):");
            Console.Write(ql.XuatJson());
        }
    }
}
