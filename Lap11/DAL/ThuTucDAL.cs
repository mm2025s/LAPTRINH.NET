using System;
using System.Data;
using System.Data.SqlClient;

namespace Lab11_CRUD_StoredProcedure.DAL
{
    /// <summary>
    /// Goi Stored Procedure bang tham so. SqlException duoc nem nguyen ven
    /// de tang BUS dich sang thong bao.
    /// </summary>
    public class ThuTucDAL
    {
        public int XoaKhoa(string maKhoa)
        {
            return Goi("sp_Khoa_Delete", P("@MaKhoa", SqlDbType.VarChar, 10, maKhoa));
        }

        public int ThemSinhVien(string maSV, string hoTen, string maKhoa, string email)
        {
            return Goi("sp_SinhVien_Insert",
                P("@MaSV", SqlDbType.VarChar, 15, maSV),
                P("@HoTen", SqlDbType.NVarChar, 100, hoTen),
                P("@NgaySinh", SqlDbType.Date, 0, new DateTime(2005, 1, 1)),
                P("@GioiTinh", SqlDbType.Bit, 0, true),
                P("@MaKhoa", SqlDbType.VarChar, 10, maKhoa),
                P("@DiemTB", SqlDbType.Decimal, 0, 7.5m),
                P("@Email", SqlDbType.VarChar, 100, email));
        }

        /// <summary>SQL Server bao loi 2812.</summary>
        public int GoiThuTucKhongTonTai()
        {
            return Goi("sp_KhongTonTai_Lab11");
        }

        /// <summary>SQL Server bao loi 4060.</summary>
        public void MoKetNoiSaiCsdl()
        {
            var builder = new SqlConnectionStringBuilder(DbConnectionFactory.ChuoiKetNoi);
            builder.InitialCatalog = "CSDL_KhongTonTai_Lab11";
            builder.ConnectTimeout = 5;
            using (var conn = new SqlConnection(builder.ConnectionString))
            {
                conn.Open();
            }
        }

        private static int Goi(string thuTuc, params SqlParameter[] thamSo)
        {
            using (SqlConnection conn = DbConnectionFactory.Create())
            using (var cmd = new SqlCommand(thuTuc, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(thamSo);
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        private static SqlParameter P(string ten, SqlDbType kieu, int doDai, object giaTri)
        {
            var p = new SqlParameter(ten, kieu, doDai);
            p.Value = giaTri ?? DBNull.Value;
            return p;
        }
    }
}
