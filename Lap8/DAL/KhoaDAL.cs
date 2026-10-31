using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Lab08_SqlCommand_Parameters.DTO;

namespace Lab08_SqlCommand_Parameters.DAL
{
    /// <summary>
    /// Truy van bang dbo.Khoa - dung cho ComboBox chon khoa.
    /// Chi dung SqlParameter khi can loc; o day la SELECT don gian.
    /// </summary>
    public class KhoaDAL
    {
        public List<Khoa> GetAll()
        {
            var ds = new List<Khoa>();
            const string sql = "SELECT MaKhoa, TenKhoa FROM dbo.Khoa ORDER BY TenKhoa";

            using (SqlConnection conn = DbConnectionFactory.Create())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        ds.Add(new Khoa
                        {
                            MaKhoa = rd.GetString(rd.GetOrdinal("MaKhoa")),
                            TenKhoa = rd.GetString(rd.GetOrdinal("TenKhoa"))
                        });
                    }
                }
            }
            return ds;
        }
    }
}
