using System;
using System.Collections.Generic;

namespace Lap2
{
    public static class Bai10_UngDungQuanLySinhVien
    {
        static QuanLySinhVien ql = new QuanLySinhVien();

        public static void Run()
        {
            Console.WriteLine("--- Bài 10: Ứng dụng quản lý sinh viên ---");
            if (ql.DanhSach.Count == 0) ql.NapDuLieuMau();

            int chon;
            do
            {
                Console.WriteLine("\n===== QUẢN LÝ SINH VIÊN =====");
                Console.WriteLine("1. Thêm sinh viên");
                Console.WriteLine("2. Sửa sinh viên theo mã");
                Console.WriteLine("3. Xóa sinh viên theo mã");
                Console.WriteLine("4. Tìm theo mã");
                Console.WriteLine("5. Tìm theo tên");
                Console.WriteLine("6. Hiển thị danh sách");
                Console.WriteLine("7. Sắp xếp theo điểm giảm dần");
                Console.WriteLine("8. Sắp xếp theo mã");
                Console.WriteLine("9. Thống kê");
                Console.WriteLine("0. Thoát");
                Console.Write("Chọn: ");

                if (!int.TryParse(Console.ReadLine(), out chon))
                {
                    Console.WriteLine("Lựa chọn không hợp lệ!");
                    continue;
                }

                try
                {
                    switch (chon)
                    {
                        case 1: ThemSinhVien(); break;
                        case 2: SuaSinhVien(); break;
                        case 3: XoaSinhVien(); break;
                        case 4: TimTheoMa(); break;
                        case 5: TimTheoTen(); break;
                        case 6: ql.HienThiDanhSach(); break;
                        case 7:
                            ql.SapXepTheoDiemGiamDan();
                            Console.WriteLine("Đã sắp xếp theo điểm giảm dần.");
                            ql.HienThiDanhSach();
                            break;
                        case 8:
                            ql.SapXepTheoMa();
                            Console.WriteLine("Đã sắp xếp theo mã.");
                            ql.HienThiDanhSach();
                            break;
                        case 9: ql.ThongKe(); break;
                        case 0: Console.WriteLine("Thoát chương trình quản lý."); break;
                        default: Console.WriteLine("Lựa chọn không hợp lệ!"); break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi: " + ex.Message);
                }
            } while (chon != 0);
        }

        static string NhapChuoi(string thongBao)
        {
            Console.Write(thongBao);
            return Console.ReadLine() ?? "";
        }

        static double NhapDiem(string thongBao)
        {
            double diem;
            while (true)
            {
                Console.Write(thongBao);
                if (double.TryParse(Console.ReadLine(), out diem) && diem >= 0 && diem <= 10)
                    return diem;
                Console.WriteLine("Điểm phải là số trong [0, 10]. Nhập lại!");
            }
        }

        static DateTime NhapNgaySinh(string thongBao)
        {
            DateTime ngay;
            while (true)
            {
                Console.Write(thongBao);
                if (DateTime.TryParse(Console.ReadLine(), out ngay) && ngay <= DateTime.Today)
                    return ngay;
                Console.WriteLine("Ngày sinh không hợp lệ (định dạng dd/MM/yyyy). Nhập lại!");
            }
        }

        static void ThemSinhVien()
        {
            string ma = NhapChuoi("Mã SV: ");
            string ten = NhapChuoi("Họ tên: ");
            DateTime ngaySinh = NhapNgaySinh("Ngày sinh (dd/MM/yyyy): ");
            double diem = NhapDiem("Điểm TB: ");

            SinhVien sv = new SinhVien(ma, ten, ngaySinh, diem);
            if (ql.Them(sv)) Console.WriteLine("Thêm thành công!");
        }

        static void SuaSinhVien()
        {
            string ma = NhapChuoi("Mã SV cần sửa: ");
            SinhVien? cu = ql.TimTheoMa(ma);
            if (cu == null)
            {
                Console.WriteLine("Không tìm thấy sinh viên có mã " + ma);
                return;
            }

            Console.WriteLine("Thông tin hiện tại:");
            cu.HienThi();

            string ten = NhapChuoi("Họ tên mới: ");
            DateTime ngaySinh = NhapNgaySinh("Ngày sinh mới (dd/MM/yyyy): ");
            double diem = NhapDiem("Điểm TB mới: ");

            SinhVien moi = new SinhVien(cu.MaSV, ten, ngaySinh, diem);
            if (ql.Sua(ma, moi)) Console.WriteLine("Sửa thành công!");
        }

        static void XoaSinhVien()
        {
            string ma = NhapChuoi("Mã SV cần xóa: ");
            SinhVien? sv = ql.TimTheoMa(ma);
            if (sv == null)
            {
                Console.WriteLine("Không tìm thấy sinh viên có mã " + ma);
                return;
            }

            Console.WriteLine("Sinh viên sẽ bị xóa:");
            sv.HienThi();

            if (ql.Xoa(ma)) Console.WriteLine("Xóa thành công!");
        }

        static void TimTheoMa()
        {
            string ma = NhapChuoi("Mã cần tìm: ");
            SinhVien? sv = ql.TimTheoMa(ma);
            if (sv == null) Console.WriteLine("Không tìm thấy");
            else sv.HienThi();
        }

        static void TimTheoTen()
        {
            string ten = NhapChuoi("Tên cần tìm: ");
            List<SinhVien> ketQua = ql.TimTheoTen(ten);
            if (ketQua.Count == 0)
            {
                Console.WriteLine("Không tìm thấy");
                return;
            }

            Console.WriteLine("Tìm thấy {0} kết quả:", ketQua.Count);
            foreach (SinhVien sv in ketQua) sv.HienThi();
        }
    }
}
