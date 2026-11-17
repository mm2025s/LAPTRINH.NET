using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace QuanLyNhaThuoc.DAL
{
    /// <summary>
    /// Lop truy cap CSDL dung chung: moi lenh deu la Stored Procedure voi
    /// SqlParameter (khong noi chuoi SQL), ket noi mo/dong trong using.
    /// </summary>
    public static class Db
    {
        public static string ChuoiKetNoi
        {
            get
            {
                ConnectionStringSettings css = ConfigurationManager.ConnectionStrings["QLNhaThuoc"];
                return css != null
                    ? css.ConnectionString
                    : @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=QLNhaThuoc;Integrated Security=True";
            }
        }

        public static SqlConnection TaoKetNoi()
        {
            return new SqlConnection(ChuoiKetNoi);
        }

        /// <summary>Thu tuc SELECT -> DataTable (mo hinh ngat ket noi, Lab 10).</summary>
        public static DataTable Bang(string thuTuc, params SqlParameter[] thamSo)
        {
            var bang = new DataTable();
            using (SqlConnection conn = TaoKetNoi())
            using (SqlCommand cmd = Lenh(thuTuc, conn, thamSo))
            using (var adapter = new SqlDataAdapter(cmd))
            {
                adapter.Fill(bang);
            }
            return bang;
        }

        /// <summary>Thu tuc ghi du lieu, tra ve so dong anh huong.</summary>
        public static int ChayLenh(string thuTuc, params SqlParameter[] thamSo)
        {
            using (SqlConnection conn = TaoKetNoi())
            using (SqlCommand cmd = Lenh(thuTuc, conn, thamSo))
            {
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public static void Doc(string thuTuc, Action<SqlDataReader> docDong, params SqlParameter[] thamSo)
        {
            using (SqlConnection conn = TaoKetNoi())
            using (SqlCommand cmd = Lenh(thuTuc, conn, thamSo))
            {
                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read()) docDong(rd);
                }
            }
        }

        private static SqlCommand Lenh(string thuTuc, SqlConnection conn, SqlParameter[] thamSo)
        {
            var cmd = new SqlCommand(thuTuc, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddRange(thamSo);
            return cmd;
        }

        /// <summary>Tao tham so dung kieu/do dai; null -> DBNull.Value.</summary>
        public static SqlParameter P(string ten, SqlDbType kieu, object giaTri, int doDai = 0)
        {
            var p = new SqlParameter(ten, kieu);
            if (doDai > 0) p.Size = doDai;
            p.Value = giaTri ?? DBNull.Value;
            return p;
        }

        public static SqlParameter Output(string ten, SqlDbType kieu)
        {
            return new SqlParameter(ten, kieu) { Direction = ParameterDirection.Output };
        }

        /// <summary>Tham so kieu bang (Table-Valued Parameter) cho gio hang / phieu nhap.</summary>
        public static SqlParameter Bang(string ten, string tenKieu, DataTable duLieu)
        {
            return new SqlParameter(ten, SqlDbType.Structured) { TypeName = tenKieu, Value = duLieu };
        }
    }
}
