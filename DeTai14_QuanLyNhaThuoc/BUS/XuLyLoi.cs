using System;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using QuanLyNhaThuoc.DTO;

namespace QuanLyNhaThuoc.BUS
{
    public enum LoaiLoi { NhapLieu, NghiepVu, Sql, HeThong }

    public class ThongTinLoi
    {
        public LoaiLoi Loai { get; set; }
        public string ThongBao { get; set; }
        public string Truong { get; set; }
    }

    /// <summary>
    /// Xu ly loi tap trung (ke thua Lab 11 - Bai 10):
    ///   nhap lieu / nghiep vu -> thong bao ro rang, khong ghi nhat ky;
    ///   loi SQL / he thong     -> cau ngan + ma tham chieu, chi tiet ghi logs\loi_yyyyMMdd.log.
    /// </summary>
    public static class XuLyLoi
    {
        private static readonly object KhoaGhi = new object();

        public static T Chay<T>(Func<T> lenh, string thaoTac)
        {
            try { return lenh(); }
            catch (SqlException ex) { throw Dich(ex, thaoTac); }
        }

        public static Exception Dich(SqlException ex, string thaoTac)
        {
            if (ex.Number >= 50000 && ex.Number < 60000)
                return new LoiNghiepVu(ex.Message, ex.Number);
            if (ex.Number == 2601 || ex.Number == 2627)
                return new LoiNghiepVu("Dữ liệu bị trùng với bản ghi đã có.", ex.Number);
            if (ex.Number == 547)
                return new LoiNghiepVu(ex.Message.Contains("CK_Thuoc_TonKho")
                    ? "Không đủ tồn kho để thực hiện giao dịch."
                    : "Dữ liệu đang được sử dụng hoặc vi phạm ràng buộc.", ex.Number);

            string ma = GhiNhatKy(thaoTac, ex);
            string cau;
            switch (ex.Number)
            {
                case -2: cau = "Máy chủ CSDL phản hồi quá lâu, vui lòng thử lại."; break;
                case -1: case 2: case 53: case 4060: case 18456:
                    cau = "Không kết nối được CSDL QLNhaThuoc. Kiểm tra SQL Server và App.config."; break;
                case 1205: cau = "CSDL đang bận, vui lòng thử lại."; break;
                case 2812: cau = "CSDL thiếu thủ tục. Hãy chạy lại SQL\\01_TaoCSDL_QLNhaThuoc.sql."; break;
                default: cau = "Thao tác không thực hiện được do lỗi cơ sở dữ liệu."; break;
            }
            return new LoiCsdl(cau + " (Mã tham chiếu: " + ma + ")", ma, ex);
        }

        public static ThongTinLoi PhanLoai(Exception ex, string thaoTac)
        {
            var sql = ex as SqlException;
            if (sql != null) ex = Dich(sql, thaoTac);

            var nhap = ex as LoiNhapLieu;
            if (nhap != null) return new ThongTinLoi { Loai = LoaiLoi.NhapLieu, ThongBao = nhap.Message, Truong = nhap.Truong };
            if (ex is LoiNghiepVu) return new ThongTinLoi { Loai = LoaiLoi.NghiepVu, ThongBao = ex.Message };
            if (ex is LoiCsdl) return new ThongTinLoi { Loai = LoaiLoi.Sql, ThongBao = ex.Message };

            string ma = GhiNhatKy(thaoTac, ex);
            return new ThongTinLoi { Loai = LoaiLoi.HeThong, ThongBao = "Đã xảy ra lỗi ngoài dự kiến. (Mã tham chiếu: " + ma + ")" };
        }

        internal static string GhiNhatKy(string thaoTac, Exception ex)
        {
            string ma = "E" + DateTime.Now.ToString("yyMMddHHmmss") + "-" + Guid.NewGuid().ToString("N").Substring(0, 4).ToUpperInvariant();
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("[{0:yyyy-MM-dd HH:mm:ss}] {1} | {2} | NV={3}", DateTime.Now, ma, thaoTac,
                PhienDangNhap.HienTai == null ? "-" : PhienDangNhap.HienTai.TenDangNhap));
            var sql = ex as SqlException;
            if (sql != null)
                sb.AppendLine(string.Format("  Number={0} Procedure={1} Line={2}", sql.Number, sql.Procedure, sql.LineNumber));
            sb.AppendLine("  " + ex.GetType().FullName + ": " + ex.Message);
            sb.AppendLine("  " + ex.StackTrace);
            try
            {
                lock (KhoaGhi)
                {
                    string thuMuc = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
                    Directory.CreateDirectory(thuMuc);
                    File.AppendAllText(Path.Combine(thuMuc, "loi_" + DateTime.Now.ToString("yyyyMMdd") + ".log"),
                        sb.ToString(), new UTF8Encoding(true));
                }
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
            return ma;
        }
    }
}
