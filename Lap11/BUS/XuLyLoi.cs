using System;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using Lab11_CRUD_StoredProcedure.DTO;

namespace Lab11_CRUD_StoredProcedure.BUS
{
    public enum LoaiLoi { NhapLieu, NghiepVu, Sql, HeThong }

    /// <summary>Ket qua phan loai mot exception, san sang de giao dien hien thi.</summary>
    public class ThongTinLoi
    {
        public LoaiLoi Loai { get; set; }
        public string TenLoai { get; set; }
        public string ThongBao { get; set; }
        public string MaThamChieu { get; set; }
    }

    /// <summary>
    /// BAI 10: chuan hoa xu ly loi tai mot noi duy nhat.
    ///   Nhap lieu : LoiNhapLieu - nguoi dung sua lai o nhap la xong.
    ///   Nghiep vu : LoiNghiepVu - THROW 5000x, trung UNIQUE, vi pham FK/CHECK.
    ///   SQL       : LoiCsdl     - cau ngan + ma tham chieu, chi tiet ghi nhat ky.
    ///   He thong  : loi ngoai du kien, xu ly nhu loi SQL.
    /// </summary>
    public static class XuLyLoi
    {
        private static readonly object KhoaGhi = new object();

        public static string TepNhatKyHomNay
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs",
                    "loi_" + DateTime.Now.ToString("yyyyMMdd") + ".log");
            }
        }

        /// <summary>Chay mot lenh DAL, dich SqlException sang loi cua ung dung.</summary>
        public static T Chay<T>(Func<T> lenh, string thaoTac)
        {
            try
            {
                return lenh();
            }
            catch (SqlException ex)
            {
                throw DichLoiSql(ex, thaoTac);
            }
        }

        public static Exception DichLoiSql(SqlException ex, string thaoTac)
        {
            // 50000+ do chinh Stored Procedure THROW: thong bao da viet san cho nguoi dung.
            if (ex.Number >= 50000 && ex.Number < 60000)
                return new LoiNghiepVu(ex.Message, ex.Number);

            if (ex.Number == 2601 || ex.Number == 2627)
                return new LoiNghiepVu(ex.Message.Contains("UX_SinhVien_Email")
                    ? "Email đã được dùng bởi sinh viên khác (kể cả sinh viên đã xóa)."
                    : "Dữ liệu bị trùng với bản ghi đã có.", ex.Number);

            if (ex.Number == 547)
                return new LoiNghiepVu("Dữ liệu vi phạm ràng buộc của cơ sở dữ liệu.", ex.Number);

            string ma = GhiNhatKy(LoaiLoi.Sql, thaoTac, ex);
            return new LoiCsdl(ThongBaoLoiSql(ex.Number) + " (Mã tham chiếu: " + ma + ")", ex.Number, ma, ex);
        }

        private static string ThongBaoLoiSql(int so)
        {
            switch (so)
            {
                case -2:
                    return "Máy chủ CSDL phản hồi quá lâu, vui lòng thử lại.";
                case -1:
                case 2:
                case 53:
                case 4060:
                    return "Không kết nối được cơ sở dữ liệu. Hãy kiểm tra SQL Server và chuỗi kết nối trong App.config.";
                case 18456:
                    return "Tài khoản không đăng nhập được vào cơ sở dữ liệu.";
                case 1205:
                    return "Cơ sở dữ liệu đang bận, vui lòng thử lại.";
                case 2812:
                    return "Thiếu Stored Procedure trong CSDL. Hãy chạy script SQL\\Lab11_StoredProcedures.sql.";
                default:
                    return "Thao tác không thực hiện được do lỗi cơ sở dữ liệu.";
            }
        }

        /// <summary>Phan loai moi exception; loi ngoai du kien duoc ghi nhat ky tai day.</summary>
        public static ThongTinLoi PhanLoai(Exception ex, string thaoTac)
        {
            var sql = ex as SqlException;
            if (sql != null) ex = DichLoiSql(sql, thaoTac);

            if (ex is LoiNhapLieu)
                return new ThongTinLoi { Loai = LoaiLoi.NhapLieu, TenLoai = "Nhập liệu", ThongBao = ex.Message };

            if (ex is LoiNghiepVu)
                return new ThongTinLoi { Loai = LoaiLoi.NghiepVu, TenLoai = "Nghiệp vụ", ThongBao = ex.Message };

            var csdl = ex as LoiCsdl;
            if (csdl != null)
                return new ThongTinLoi { Loai = LoaiLoi.Sql, TenLoai = "SQL", ThongBao = csdl.Message, MaThamChieu = csdl.MaThamChieu };

            string ma = GhiNhatKy(LoaiLoi.HeThong, thaoTac, ex);
            return new ThongTinLoi
            {
                Loai = LoaiLoi.HeThong,
                TenLoai = "Hệ thống",
                ThongBao = "Đã xảy ra lỗi ngoài dự kiến. (Mã tham chiếu: " + ma + ")",
                MaThamChieu = ma
            };
        }

        /// <summary>Ghi chi tiet cho lap trinh vien. Loi ghi file khong duoc lam hong luong chinh.</summary>
        private static string GhiNhatKy(LoaiLoi loai, string thaoTac, Exception ex)
        {
            string ma = "E" + DateTime.Now.ToString("yyMMddHHmmss") + "-"
                + Guid.NewGuid().ToString("N").Substring(0, 4).ToUpperInvariant();

            var sb = new StringBuilder();
            sb.AppendLine(string.Format("[{0:yyyy-MM-dd HH:mm:ss}] {1} | {2} | {3}", DateTime.Now, ma, loai, thaoTac));

            var sql = ex as SqlException;
            if (sql != null)
                sb.AppendLine(string.Format("  Number={0} Class={1} State={2} Procedure={3} Line={4}",
                    sql.Number, sql.Class, sql.State, sql.Procedure, sql.LineNumber));

            sb.AppendLine("  Type=" + ex.GetType().FullName);
            sb.AppendLine("  Message=" + ex.Message);
            sb.AppendLine("  StackTrace=" + ex.StackTrace);
            sb.AppendLine();

            try
            {
                lock (KhoaGhi)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(TepNhatKyHomNay));
                    File.AppendAllText(TepNhatKyHomNay, sb.ToString(), new UTF8Encoding(true));
                }
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }

            return ma;
        }

        public static string DocNhatKy()
        {
            try
            {
                return File.Exists(TepNhatKyHomNay) ? File.ReadAllText(TepNhatKyHomNay, Encoding.UTF8) : string.Empty;
            }
            catch (IOException) { return string.Empty; }
        }
    }
}
