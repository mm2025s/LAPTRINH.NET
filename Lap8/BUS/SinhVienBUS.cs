using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using Lab08_SqlCommand_Parameters.DAL;
using Lab08_SqlCommand_Parameters.DTO;

namespace Lab08_SqlCommand_Parameters.BUS
{
    /// <summary>
    /// TANG NGHIEP VU cua Lab 8.
    ///
    /// Nhiem vu:
    ///   - Kiem tra du lieu nhap TRUOC khi xuong DAL (khong de CSDL bao loi tho).
    ///   - Chuyen loi SqlException thanh thong bao tieng Viet de GUI hien thi.
    ///   - GUI chi goi BUS, khong biet SqlCommand/SqlParameter.
    /// </summary>
    public class SinhVienBUS
    {
        private readonly SinhVienDAL _dal;

        public SinhVienBUS() : this(new SinhVienDAL()) { }

        public SinhVienBUS(SinhVienDAL dal)
        {
            if (dal == null) throw new ArgumentNullException("dal");
            _dal = dal;
        }

        // =================================================================
        // BAI 1, 2
        // =================================================================
        public int CountStudents()
        {
            try
            {
                return _dal.CountStudents();
            }
            catch (Exception ex)
            {
                throw new LoiNghiepVu("", "Không đếm được số sinh viên: " + MoTa(ex));
            }
        }

        public List<SinhVien> DocDanhSach()
        {
            try
            {
                return _dal.DocDanhSach();
            }
            catch (Exception ex)
            {
                throw new LoiNghiepVu("", "Không đọc được danh sách sinh viên: " + MoTa(ex));
            }
        }

        // =================================================================
        // BAI 3: Them sinh vien
        // =================================================================
        public KetQuaThaoTac ThemSinhVien(SinhVien sv)
        {
            KiemTraHopLe(sv, false);

            try
            {
                int rows = _dal.ThemSinhVien(sv);
                return KetQuaThaoTac.TuSoDong(rows,
                    string.Format("Đã thêm {0} bản ghi sinh viên \"{1}\".", rows, sv.MaSV),
                    "Không thêm được bản ghi nào.");
            }
            catch (LoiNghiepVu) { throw; }
            catch (Exception ex)
            {
                throw new LoiNghiepVu("MaSV", MoTaThem(ex, sv.MaSV));
            }
        }

        // =================================================================
        // BAI 4: Cap nhat diem
        // =================================================================
        public KetQuaThaoTac CapNhatDiem(string maSV, string diemText)
        {
            maSV = (maSV ?? string.Empty).Trim();
            if (maSV.Length == 0)
                throw new LoiNghiepVu("MaSV", "Vui lòng nhập mã sinh viên cần cập nhật.");

            decimal diem = DocDiem(diemText, "DiemTB");

            try
            {
                int rows = _dal.CapNhatDiem(maSV, diem);
                if (rows == 0)
                    return new KetQuaThaoTac { SoDong = 0, ThongBao = "Không tìm thấy mã sinh viên \"" + maSV + "\"." };

                return new KetQuaThaoTac
                {
                    SoDong = rows,
                    ThongBao = string.Format("Cập nhật thành công điểm {0:0.00} cho {1}.", diem, maSV)
                };
            }
            catch (LoiNghiepVu) { throw; }
            catch (Exception ex)
            {
                throw new LoiNghiepVu("DiemTB", "Không cập nhật được điểm: " + MoTa(ex));
            }
        }

        // =================================================================
        // BAI 5, 6
        // =================================================================
        public List<SinhVien> SinhVienTheoKhoa(string maKhoa)
        {
            maKhoa = (maKhoa ?? string.Empty).Trim();
            if (maKhoa.Length == 0)
                throw new LoiNghiepVu("MaKhoa", "Vui lòng chọn khoa cần lọc.");

            try
            {
                return _dal.SinhVienTheoKhoa(maKhoa);
            }
            catch (Exception ex)
            {
                throw new LoiNghiepVu("MaKhoa", "Không gọi được thủ tục sp_SinhVien_TheoKhoa: " + MoTa(ex));
            }
        }

        public SinhVien TimTheoMa(string maSV)
        {
            maSV = (maSV ?? string.Empty).Trim();
            if (maSV.Length == 0)
                throw new LoiNghiepVu("MaSV", "Vui lòng nhập mã sinh viên cần tìm.");

            try
            {
                return _dal.TimTheoMa(maSV);
            }
            catch (Exception ex)
            {
                throw new LoiNghiepVu("MaSV", "Không tìm kiếm được: " + MoTa(ex));
            }
        }

        // =================================================================
        // BAI 7: Thong ke
        // =================================================================
        public List<ThongKeKhoa> ThongKeDiemCaoNhatTheoKhoa()
        {
            try
            {
                return _dal.ThongKeDiemCaoNhatTheoKhoa();
            }
            catch (Exception ex)
            {
                throw new LoiNghiepVu("", "Không thống kê được điểm: " + MoTa(ex));
            }
        }

        public decimal? DiemCaoNhatCuaKhoa(string maKhoa)
        {
            maKhoa = (maKhoa ?? string.Empty).Trim();
            if (maKhoa.Length == 0)
                throw new LoiNghiepVu("MaKhoa", "Vui lòng chọn khoa cần lấy điểm cao nhất.");

            try
            {
                return _dal.DiemCaoNhatCuaKhoa(maKhoa);
            }
            catch (Exception ex)
            {
                throw new LoiNghiepVu("MaKhoa", "Không lấy được điểm cao nhất: " + MoTa(ex));
            }
        }

        // =================================================================
        // BAI 8: Xoa sinh vien
        // =================================================================
        /// <summary>
        /// Xoa sinh vien. Neu con dang ky, nem LoiNghiepVu de GUI hoi nguoi dung
        /// co muon xoa ca dang ky hay khong (xem CoDangKy / XoaKemDangKy).
        /// </summary>
        public int XoaSinhVien(string maSV)
        {
            maSV = (maSV ?? string.Empty).Trim();
            if (maSV.Length == 0)
                throw new LoiNghiepVu("MaSV", "Vui lòng nhập mã sinh viên cần xóa.");

            try
            {
                return _dal.XoaSinhVien(maSV);
            }
            catch (LoiNghiepVu) { throw; }
            catch (Exception ex)
            {
                throw new LoiNghiepVu("MaSV", "Không xóa được sinh viên: " + MoTa(ex));
            }
        }

        public int DemDangKy(string maSV)
        {
            try
            {
                return _dal.DemDangKy((maSV ?? string.Empty).Trim());
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>Xoa dang ky roi xoa sinh vien (2 thao tac ExecuteNonQuery lien tiep).</summary>
        public KetQuaThaoTac XoaKemDangKy(string maSV)
        {
            maSV = (maSV ?? string.Empty).Trim();
            if (maSV.Length == 0)
                throw new LoiNghiepVu("MaSV", "Vui lòng nhập mã sinh viên cần xóa.");

            try
            {
                int soDangKy = _dal.XoaDangKyCuaSinhVien(maSV);
                int rows = _dal.XoaSinhVien(maSV);

                return new KetQuaThaoTac
                {
                    SoDong = rows,
                    ThongBao = string.Format("Đã xóa {0} đăng ký và sinh viên {1}.", soDangKy, maSV)
                };
            }
            catch (LoiNghiepVu) { throw; }
            catch (Exception ex)
            {
                throw new LoiNghiepVu("MaSV", "Không xóa được: " + MoTa(ex));
            }
        }

        // =================================================================
        // BAI 9: Them qua Stored Procedure co OUTPUT
        // =================================================================
        public KetQuaThaoTac ThemQuaThuTuc(SinhVien sv)
        {
            KiemTraHopLe(sv, false);

            try
            {
                int tongSo = _dal.ThemSinhVienQuaThuTuc(sv);
                return new KetQuaThaoTac
                {
                    SoDong = 1,
                    GiaTriOutput = tongSo,
                    ThongBao = string.Format(
                        "Đã thêm sinh viên {0}. Tham số OUTPUT @SoLuongHienTai = {1} (tổng số sinh viên).",
                        sv.MaSV, tongSo)
                };
            }
            catch (LoiNghiepVu) { throw; }
            catch (Exception ex)
            {
                throw new LoiNghiepVu("MaSV", MoTaThem(ex, sv.MaSV));
            }
        }

        // =================================================================
        // KIEM TRA DU LIEU
        // =================================================================
        public List<Khoa> DanhSachKhoa()
        {
            try
            {
                return _dal.DanhSachKhoa();
            }
            catch (Exception ex)
            {
                throw new LoiNghiepVu("", "Không đọc được danh sách khoa: " + MoTa(ex));
            }
        }

        /// <summary>
        /// Kiem tra ma sinh vien: bat buoc, toi da 15 ky tu, chi chu + so + gach noi.
        /// Tra ve null neu hop le, nguoc lai la thong bao loi.
        /// </summary>
        public static string KiemTraMaSV(string maSV)
        {
            maSV = (maSV ?? string.Empty).Trim();
            if (maSV.Length == 0) return "Vui lòng nhập mã sinh viên.";
            if (maSV.Length > 15) return "Mã sinh viên tối đa 15 ký tự.";
            if (!Regex.IsMatch(maSV, @"^[A-Za-z0-9_-]+$"))
                return "Mã sinh viên chỉ gồm chữ, số, gạch nối hoặc gạch dưới.";
            return null;
        }

        /// <summary>Kiem tra diem: bat buoc, so thuc trong khoang 0 - 10.</summary>
        public static string KiemTraDiem(string diemText)
        {
            decimal diem;
            if (!decimal.TryParse((diemText ?? string.Empty).Trim().Replace(',', '.'),
                    System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.InvariantCulture, out diem))
                return "Điểm phải là một số (ví dụ 8.25).";

            if (diem < 0 || diem > 10) return "Điểm phải nằm trong khoảng 0 đến 10.";
            return null;
        }

        private static decimal DocDiem(string diemText, string truong)
        {
            string loi = KiemTraDiem(diemText);
            if (loi != null) throw new LoiNghiepVu(truong, loi);

            return decimal.Parse((diemText ?? string.Empty).Trim().Replace(',', '.'),
                System.Globalization.NumberStyles.Number,
                System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>Kiem tra toan bo DTO truoc khi ghi CSDL.</summary>
        private void KiemTraHopLe(SinhVien sv, bool khiCapNhat)
        {
            if (sv == null) throw new LoiNghiepVu("", "Dữ liệu sinh viên rỗng.");

            string loi = KiemTraMaSV(sv.MaSV);
            if (loi != null) throw new LoiNghiepVu("MaSV", loi);

            if (string.IsNullOrWhiteSpace(sv.HoTen))
                throw new LoiNghiepVu("HoTen", "Vui lòng nhập họ tên sinh viên.");
            if (sv.HoTen.Trim().Length > 100)
                throw new LoiNghiepVu("HoTen", "Họ tên tối đa 100 ký tự.");

            if (sv.NgaySinh == DateTime.MinValue)
                throw new LoiNghiepVu("NgaySinh", "Vui lòng chọn ngày sinh.");
            if (sv.NgaySinh.Date > DateTime.Today)
                throw new LoiNghiepVu("NgaySinh", "Ngày sinh không được lớn hơn ngày hôm nay.");

            if (string.IsNullOrWhiteSpace(sv.MaKhoa))
                throw new LoiNghiepVu("MaKhoa", "Vui lòng chọn khoa.");

            string loiDiem = KiemTraDiem(sv.DiemTB.ToString(System.Globalization.CultureInfo.InvariantCulture));
            if (loiDiem != null) throw new LoiNghiepVu("DiemTB", loiDiem);

            if (!string.IsNullOrWhiteSpace(sv.Email))
            {
                string email = sv.Email.Trim();
                if (email.Length > 100)
                    throw new LoiNghiepVu("Email", "Email tối đa 100 ký tự.");
                if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    throw new LoiNghiepVu("Email", "Email không đúng định dạng.");
            }
        }

        // ---------------------------------------------------------------
        // CHUYEN LOI KY THUAT -> THONG BAO TIENG VIET
        // ---------------------------------------------------------------
        private static string MoTa(Exception ex)
        {
            var sql = ex as System.Data.SqlClient.SqlException;
            if (sql == null) return ex.Message;

            switch (sql.Number)
            {
                case 2627:
                case 2601: return "Dữ liệu bị trùng khóa (mã sinh viên hoặc email đã tồn tại).";
                case 547: return "Vi phạm ràng buộc khóa ngoại (mã khoa không tồn tại hoặc còn dữ liệu liên quan).";
                case 515: return "Có trường bắt buộc đang bị bỏ trống.";
                case 8114: return "Dữ liệu không đúng kiểu (kiểm tra lại điểm số / ngày tháng).";
                case -2: return "Hết thời gian chờ CSDL (timeout).";
                case 4060: return "Không mở được cơ sở dữ liệu QLSinhVien.";
                case 18456: return "Đăng nhập SQL Server thất bại (sai tài khoản / mật khẩu).";
                case 53:
                case -1: return "Không kết nối được tới SQL Server (kiểm tra tên instance trong App.config).";
                default: return "Lỗi SQL (" + sql.Number + "): " + sql.Message;
            }
        }

        private string MoTaThem(Exception ex, string maSV)
        {
            string moTa = MoTa(ex);
            if (_dal.DemDangKy(maSV) > 0 || moTa.IndexOf("trùng", StringComparison.OrdinalIgnoreCase) >= 0)
                return moTa + " Mã \"" + maSV + "\" có thể đã được dùng, hãy chọn mã khác.";
            return moTa;
        }
    }
}
