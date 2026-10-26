using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Lab06_ThreeLayer.GUI.Helpers
{
    /// <summary>
    /// Các hàm kiểm tra dữ liệu dùng chung.
    /// Mục tiêu của Lab 6: KHÔNG viết lại logic kiểm tra ở nhiều Form,
    /// mọi Form đều gọi những hàm thuần (không phụ thuộc giao diện) dưới đây.
    /// Nhờ vậy Lab 6 (mô hình 3 lớp) có thể dùng lại ở tầng BUS.
    /// </summary>
    public static class KiemTra
    {
        // Tháng luôn có 31 ngày để DateTimePicker không kẹp giá trị khi chuyển tháng.
        public static readonly DateTime NgayToiThieu = new DateTime(1940, 1, 1);
        public static readonly DateTime NgayToiDa = new DateTime(2035, 12, 31);

        public const double DiemToiThieu = 0.0;
        public const double DiemToiDa = 10.0;
        public const int TuoiToiThieu = 16;
        public const int TuoiToiDa = 80;

        /// <summary>Mã sinh viên: bắt đầu bằng chữ, chỉ gồm chữ/số, 2..10 ký tự (dùng Regex).</summary>
        public static bool HopLeMaSV(string ma, out string thongBao)
        {
            thongBao = null;
            string m = (ma ?? string.Empty).Trim();
            if (m.Length == 0) { thongBao = "Vui lòng nhập mã sinh viên."; return false; }
            if (!Regex.IsMatch(m, "^[A-Za-z][A-Za-z0-9]{1,9}$"))
            {
                thongBao = "Mã sinh viên 2-10 ký tự, bắt đầu bằng chữ cái (VD: SV01).";
                return false;
            }
            return true;
        }

        public static bool HopLeHoTen(string hoTen, out string thongBao)
        {
            string t = (hoTen ?? string.Empty).Trim();
            if (t.Length == 0) { thongBao = "Vui lòng nhập họ tên sinh viên."; return false; }
            if (t.Length < 2) { thongBao = "Họ tên phải có ít nhất 2 ký tự."; return false; }
            if (Regex.IsMatch(t, "[0-9]")) { thongBao = "Họ tên không được chứa chữ số."; return false; }
            thongBao = null;
            return true;
        }

        /// <summary>Chuẩn hóa khoảng trắng + viết hoa chữ đầu mỗi từ (VD: "  nguyễn   văn an " -> "Nguyễn Văn An").</summary>
        public static string ChuanHoaHoTen(string hoTen)
        {
            if (string.IsNullOrWhiteSpace(hoTen)) return string.Empty;
            string[] parts = hoTen.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var result = new List<string>();
            foreach (string p in parts)
            {
                string lower = p.ToLower(CultureInfo.CurrentCulture);
                result.Add(char.ToUpper(lower[0], CultureInfo.CurrentCulture) + lower.Substring(1));
            }
            return string.Join(" ", result.ToArray());
        }

        /// <summary>Điểm: đọc được, thuộc [0, 10], tối đa 1 chữ số thập phân.</summary>
        public static bool HopLeDiem(string text, out double diem, out string thongBao)
        {
            diem = 0;
            string t = (text ?? string.Empty).Trim();
            if (t.Length == 0) { thongBao = "Vui lòng nhập điểm trung bình."; return false; }
            if (!double.TryParse(t, NumberStyles.Float, CultureInfo.CurrentCulture, out diem))
            {
                thongBao = "Điểm trung bình phải là con số (VD: 8.5).";
                return false;
            }
            if (diem < DiemToiThieu || diem > DiemToiDa)
            {
                thongBao = "Điểm trung bình phải thuộc khoảng [0, 10].";
                return false;
            }
            int decimals = t.IndexOf('.') >= 0 ? t.Length - t.IndexOf('.') - 1 : 0;
            if (t.IndexOf(',') >= 0) decimals = t.Length - t.IndexOf(',') - 1;
            if (decimals > 1)
            {
                thongBao = "Điểm trung bình chỉ cho phép 1 chữ số thập phân (VD: 8.5).";
                return false;
            }
            thongBao = null;
            return true;
        }

        public static bool DuyNhatDiem(decimal diem)
        {
            return diem >= 0 && diem <= 10;
        }

        /// <summary>Tính tuổi chính xác (đã trừ trường hợp chưa tới sinh nhật trong năm).</summary>
        public static int TinhTuoi(DateTime ngaySinh)
        {
            DateTime hn = DateTime.Today;
            int tuoi = hn.Year - ngaySinh.Year;
            if (ngaySinh.Date > hn.AddYears(-tuoi)) tuoi--;
            return tuoi;
        }

        public static bool HopLeNgaySinh(DateTime ngaySinh, out string thongBao)
        {
            if (ngaySinh.Date > DateTime.Today) { thongBao = "Ngày sinh không được ở tương lai."; return false; }
            int tuoi = TinhTuoi(ngaySinh);
            if (tuoi < TuoiToiThieu || tuoi > TuoiToiDa)
            {
                thongBao = string.Format("Tuổi ({0}) phải nằm trong khoảng {1}-{2} - kiểm tra lại ngày sinh.",
                    tuoi, TuoiToiThieu, TuoiToiDa);
                return false;
            }
            thongBao = null;
            return true;
        }

        public static bool HopLeDangNhap(string tenDangNhap, out string thongBao)
        {
            string t = (tenDangNhap ?? string.Empty).Trim();
            if (t.Length == 0) { thongBao = "Vui lòng nhập tên đăng nhập."; return false; }
            if (t.Length < 3) { thongBao = "Tên đăng nhập phải có ít nhất 3 ký tự."; return false; }
            if (!Regex.IsMatch(t, "^[A-Za-z0-9._]+$"))
            {
                thongBao = "Tên đăng nhập chỉ gồm chữ, số, dấu chấm và gạch dưới.";
                return false;
            }
            thongBao = null;
            return true;
        }

        public static bool HopLeMatKhau(string matKhau, out string thongBao)
        {
            if (string.IsNullOrEmpty(matKhau)) { thongBao = "Vui lòng nhập mật khẩu."; return false; }
            if (matKhau.Length < 6) { thongBao = "Mật khẩu phải có ít nhất 6 ký tự."; return false; }
            thongBao = null;
            return true;
        }

        // ----------------------------------------------------------------
        // "Hàm dùng chung" cho sự kiện: chỉ cho phép số / số thực trong TextBox
        // ----------------------------------------------------------------
        /// <summary>Chặn mọi ký tự không phải chữ số (dùng cho các ô số nguyên: số lượng, ...).</summary>
        public static void ChanNhapSoNguyen(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;
            if (char.IsDigit(e.KeyChar)) return;
            e.Handled = true;
        }

        /// <summary>
        /// Chặn ký tự không hợp lệ của một số thực: chỉ chữ số, đúng MỘT dấu thập phân,
        /// không cho dấu âm; đồng thời chặn phần nguyên vượt quá số chữ số cho phép.
        /// Trả về true nếu ký tự hợp lệ.
        /// </summary>
        public static bool ChanNhapSoThuc(TextBox t, KeyPressEventArgs e, int soNguyenToiDa, bool choPhepAm)
        {
            if (char.IsControl(e.KeyChar)) return true;

            char dec = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.Length > 0
                ? CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0]
                : '.';

            if (char.IsDigit(e.KeyChar))
            {
                int decPos = t.Text.IndexOf(dec);
                if (decPos < 0)
                {
                    // Đang nhập phần nguyên
                    string nguyen = t.Text.TrimStart('-');
                    int sel = t.SelectionLength;
                    if (sel == 0 && nguyen.Length >= soNguyenToiDa)
                    {
                        e.Handled = true;
                        return false;
                    }
                }
                return true;
            }

            if ((e.KeyChar == dec || e.KeyChar == '.' || e.KeyChar == ',') && t.Text.IndexOf(dec) < 0)
            {
                // Chưa có dấu thập phân => đổi sang ký tự thập phân của hệ thống và cho phép
                e.KeyChar = dec;
                return true;
            }

            if (e.KeyChar == '-' && choPhepAm && t.SelectionStart == 0 && t.Text.IndexOf('-') < 0)
                return true;

            e.Handled = true;
            return false;
        }

        public static void ChanNhapDiem(object sender, KeyPressEventArgs e)
        {
            TextBox t = sender as TextBox;
            if (t == null) { e.Handled = true; return; }
            ChanNhapSoThuc(t, e, 2, false);
        }

        // ----------------------------------------------------------------
        // Gợi ý nghiệp vụ
        // ----------------------------------------------------------------
        public static string XepLoai(double diem)
        {
            if (diem >= 9) return "Xuất sắc";
            if (diem >= 8) return "Giỏi";
            if (diem >= 6.5) return "Khá";
            if (diem >= 5) return "Trung bình";
            return "Yếu";
        }

        /// <summary>Lấy màu tương ứng xếp loại để tô badge.</summary>
        public static System.Drawing.Color MauXepLoai(double diem)
        {
            if (diem >= 9) return UiTheme.Green;
            if (diem >= 8) return UiTheme.Teal;
            if (diem >= 6.5) return UiTheme.Blue;
            if (diem >= 5) return UiTheme.Amber;
            return UiTheme.Red;
        }

        public static string ChamDiemNhanh(string maSV, string hoTen, string diemText, string khoa)
        {
            // Tổng hợp kết quả kiểm tra cho tính năng "Kiểm tra nhanh" ở frmMain
            var sb = new System.Text.StringBuilder();
            string loi;

            sb.AppendLine("1) Mã sinh viên");
            sb.AppendLine("   " + (HopLeMaSV(maSV, out loi) ? "Hợp lệ" : "LỖI: " + loi));
            sb.AppendLine("2) Họ tên");
            sb.AppendLine("   " + (HopLeHoTen(hoTen, out loi) ? "Hợp lệ" : "LỖI: " + loi));
            double d;
            sb.AppendLine("3) Điểm trung bình");
            sb.AppendLine("   " + (HopLeDiem(diemText, out d, out loi)
                ? string.Format("Hợp lệ - xếp loại {0}", XepLoai(d))
                : "LỖI: " + loi));
            sb.AppendLine("4) Khoa");
            sb.AppendLine("   " + (string.IsNullOrWhiteSpace(khoa) ? "LỖI: Chưa chọn khoa" : "Hợp lệ: " + khoa));
            return sb.ToString();
        }
    }
}
