using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Lab08_SqlCommand_Parameters.DAL
{
    /// <summary>
    /// Lop tao ket noi SQL Server (bai 5, 7, 8).
    ///
    /// - Uu tien doc chuoi ket noi tu App.config qua ConfigurationManager
    ///   (bai 8: khong hard-code chuoi ket noi trong ma nguon).
    /// - Neu App.config khong co, dung chuoi mac dinh (co ghi chu ro de doi).
    /// - Ho tro ca Windows Authentication va SQL Authentication (bai 7).
    /// - Luon goi using(...) o noi goi de dong ket noi.
    /// </summary>
    public static class DbConnectionFactory
    {
        /// <summary>Ten khoa trong App.config &lt;connectionStrings&gt;.</summary>
        public const string TenChuoiKetNoi = "QLSinhVien";

        /// <summary>
        /// Chuoi ket noi mac dinh (chi dung khi App.config thieu).
        ///
        /// SUA TAI DAY theo moi truong thuc te:
        ///   - SQL Server Express : Data Source=.\SQLEXPRESS
        ///   - SQL Server mac dinh : Data Source=.
        ///   - LocalDB            : Data Source=(localdb)\MSSQLLocalDB
        /// </summary>
        private const string ChuoiMacDinh =
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=QLSinhVien;Integrated Security=True";

        /// <summary>Chuoi ket noi dang dung + nguon goc (App.config / mac dinh).</summary>
        public static string ChuoiKetNoi
        {
            get
            {
                string c = LayChuoiTuCauHinh();
                if (!string.IsNullOrEmpty(c)) return c;
                return ChuoiMacDinh;
            }
        }

        /// <summary>Nguon cua chuoi ket noi dang dung - hien thi o trang trang thai.</summary>
        public static string NguonChuoiKetNoi
        {
            get
            {
                return string.IsNullOrEmpty(LayChuoiTuCauHinh())
                    ? "Mac dinh trong ma nguon (DbConnectionFactory.ChuoiMacDinh)"
                    : "App.config -> connectionStrings[name=\"" + TenChuoiKetNoi + "\"]";
            }
        }

        private static string LayChuoiTuCauHinh()
        {
            try
            {
                ConnectionStringSettings css = ConfigurationManager.ConnectionStrings[TenChuoiKetNoi];
                if (css != null && !string.IsNullOrEmpty(css.ConnectionString))
                    return css.ConnectionString;
            }
            catch (Exception)
            {
                // Thieu System.Configuration hoac App.config hong -> dung mac dinh.
            }
            return null;
        }

        /// <summary>Tao SqlConnection voi chuoi ket noi dang dung (bai 5 - code mau).</summary>
        public static SqlConnection Create()
        {
            return new SqlConnection(ChuoiKetNoi);
        }

        /// <summary>
        /// Tao SqlConnection voi chuoi ket noi tuong minh (dung khi nguoi dung
        /// sua chuoi tren Form ma khong muon ghi vao App.config).
        /// </summary>
        public static SqlConnection Create(string chuoiKetNoi)
        {
            return new SqlConnection(chuoiKetNoi);
        }

        /// <summary>
        /// Tao SqlConnection bang SQL Authentication (bai 7).
        /// Vi du: CreateSqlAuth(@".\SQLEXPRESS", "QLSinhVien", "app_qlsv", "MatKhau@123");
        /// </summary>
        public static SqlConnection CreateSqlAuth(string mayChu, string csdl, string taiKhoan, string matKhau)
        {
            var builder = new SqlConnectionStringBuilder();
            builder.DataSource = mayChu;
            builder.InitialCatalog = csdl;
            builder.UserID = taiKhoan;
            builder.Password = matKhau;
            builder.IntegratedSecurity = false;
            builder.ConnectTimeout = 15;
            return new SqlConnection(builder.ConnectionString);
        }

        /// <summary>
        /// Tao chuoi ket noi SQL Authentication (khong chua doi tuong connection).
        /// </summary>
        public static string TaoChuoiSqlAuth(string mayChu, string csdl, string taiKhoan, string matKhau)
        {
            var builder = new SqlConnectionStringBuilder();
            builder.DataSource = mayChu;
            builder.InitialCatalog = csdl;
            builder.UserID = taiKhoan;
            builder.Password = matKhau;
            builder.IntegratedSecurity = false;
            builder.ConnectTimeout = 15;
            return builder.ConnectionString;
        }
    }
}
