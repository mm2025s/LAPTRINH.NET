using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Lab10_DisconnectedData.DAL
{
    /// <summary>
    /// BAI 10: Lop SqlHelper toi gian - dong goi 3 phuong thuc thuc thi cua
    /// SqlCommand de tang tren khong phai lap lai using(...) / Open() / Close().
    ///
    /// Nguyen tac:
    ///   - Moi phuong thuc tu mo va tu dong ket noi (using 2 lop: connection + command).
    ///   - Tham so luon duoc truyen qua SqlParameter (khong noi chuoi SQL).
    ///   - ExecuteReader nhan mot delegate de doc du lieu NGAY TRONG khi ket noi
    ///     con mo (SqlDataReader chi doc duoc khi connection dang mo).
    ///   - Loi SqlException duoc nem ra nguyen ven de tang BUS/GUI xu ly.
    /// </summary>
    public static class SqlHelper
    {
        /// <summary>
        /// Thuc thi cau lenh tra ve MOT gia tri duy nhat (COUNT, MAX, SUM...).
        /// Tra ve null khi ket qua la NULL (vi du MAX tren bang rong).
        /// </summary>
        public static object ExecuteScalar(string sql, IEnumerable<SqlParameter> thamSo = null,
            CommandType kieuLenh = CommandType.Text, SqlConnection connNgoai = null)
        {
            return Chay(sql, thamSo, kieuLenh, connNgoai, cmd => cmd.ExecuteScalar());
        }

        /// <summary>
        /// Thuc thi INSERT / UPDATE / DELETE, tra ve so dong bi anh huong.
        /// </summary>
        public static int ExecuteNonQuery(string sql, IEnumerable<SqlParameter> thamSo = null,
            CommandType kieuLenh = CommandType.Text, SqlConnection connNgoai = null)
        {
            object kq = Chay(sql, thamSo, kieuLenh, connNgoai, cmd => cmd.ExecuteNonQuery());
            return kq == null ? 0 : Convert.ToInt32(kq);
        }

        /// <summary>
        /// Thuc thi SELECT va doc tung dong bang delegate.
        /// Connect van mo trong suot thoi gian delegate chay -> an toan.
        /// </summary>
        public static int ExecuteReader(string sql, Action<SqlDataReader> docDong,
            IEnumerable<SqlParameter> thamSo = null, CommandType kieuLenh = CommandType.Text,
            SqlConnection connNgoai = null)
        {
            int soDong = 0;
            object kq = Chay(sql, thamSo, kieuLenh, connNgoai, cmd =>
            {
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        soDong++;
                        if (docDong != null) docDong(rd);
                    }
                }
                return soDong;
            });
            return kq == null ? 0 : Convert.ToInt32(kq);
        }

        /// <summary>Do ket qua vao DataTable (tien cho DataGridView).</summary>
        public static DataTable ExecuteDataTable(string sql, IEnumerable<SqlParameter> thamSo = null,
            CommandType kieuLenh = CommandType.Text)
        {
            var table = new DataTable();
            using (SqlConnection conn = DbConnectionFactory.Create())
            using (SqlCommand cmd = TaoLenh(sql, thamSo, kieuLenh, conn))
            using (var adapter = new SqlDataAdapter(cmd))
            {
                adapter.Fill(table);
            }
            return table;
        }

        /// <summary>
        /// Ham loi chung: tao command, gan tham so, mo ket noi, chay va LUON dong.
        /// Neu truyen connNgoai (dang trong mot transaction) thi khong tu mo/dong.
        /// </summary>
        private static object Chay(string sql, IEnumerable<SqlParameter> thamSo, CommandType kieuLenh,
            SqlConnection connNgoai, Func<SqlCommand, object> thucThi)
        {
            if (connNgoai != null)
            {
                using (SqlCommand cmd = TaoLenh(sql, thamSo, kieuLenh, connNgoai))
                {
                    return thucThi(cmd);
                }
            }

            using (SqlConnection conn = DbConnectionFactory.Create())
            using (SqlCommand cmd = TaoLenh(sql, thamSo, kieuLenh, conn))
            {
                conn.Open();
                return thucThi(cmd);
            }
        }

        private static SqlCommand TaoLenh(string sql, IEnumerable<SqlParameter> thamSo,
            CommandType kieuLenh, SqlConnection conn)
        {
            var cmd = new SqlCommand(sql, conn);
            cmd.CommandType = kieuLenh;
            cmd.CommandTimeout = 30;
            if (thamSo != null)
            {
                foreach (SqlParameter p in thamSo.Where(p => p != null)) cmd.Parameters.Add(p);
            }
            return cmd;
        }

        /// <summary>
        /// Tien ich tao SqlParameter dung kieu va do dai - tranh AddWithValue
        /// (AddWithValue doan kieu theo gia tri C#, de sai voi DECIMAL/DATE/NVARCHAR).
        /// </summary>
        public static SqlParameter P(string ten, SqlDbType kieu, object giaTri, int doDai = 0)
        {
            var p = new SqlParameter(ten, kieu);
            if (doDai > 0) p.Size = doDai;
            p.Value = giaTri ?? DBNull.Value;
            return p;
        }

        /// <summary>Tham so OUTPUT (bai 9).</summary>
        public static SqlParameter POutput(string ten, SqlDbType kieu, int doDai = 0)
        {
            var p = new SqlParameter(ten, kieu);
            if (doDai > 0) p.Size = doDai;
            p.Direction = ParameterDirection.Output;
            return p;
        }
    }
}
