using System;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace QuanLyNhaThuoc.DAL
{
    /// <summary>
    /// Tu tao CSDL lan chay dau: mo Visual Studio, nhan F5 la chay duoc, khong can
    /// chay tay script SQL. Script duoc chep vao thu muc chay (bin\Debug\SQL).
    /// </summary>
    public static class CaiDatCsdl
    {
        /// <summary>true neu ket noi duoc va CSDL QLNhaThuoc da co bang Thuoc.</summary>
        public static bool DaSanSang(out string loi)
        {
            loi = null;
            try
            {
                using (var conn = new SqlConnection(ChuoiMaster()))
                using (var cmd = new SqlCommand("SELECT CASE WHEN DB_ID(@ten) IS NOT NULL AND OBJECT_ID(@bang) IS NOT NULL THEN 1 ELSE 0 END", conn))
                {
                    string ten = new SqlConnectionStringBuilder(Db.ChuoiKetNoi).InitialCatalog;
                    cmd.Parameters.AddWithValue("@ten", ten);
                    cmd.Parameters.AddWithValue("@bang", ten + ".dbo.Thuoc");
                    conn.Open();
                    return (int)cmd.ExecuteScalar() == 1;
                }
            }
            catch (SqlException ex)
            {
                loi = ex.Message;
                return false;
            }
        }

        /// <summary>Chay 01_TaoCSDL va 02_DuLieuMau, tach theo dong GO giong sqlcmd/SSMS.</summary>
        public static void TaoMoi()
        {
            string thuMuc = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SQL");
            using (var conn = new SqlConnection(ChuoiMaster()))
            {
                conn.Open();
                foreach (string tep in new[] { "01_TaoCSDL_QLNhaThuoc.sql", "02_DuLieuMau.sql" })
                {
                    string noiDung = File.ReadAllText(Path.Combine(thuMuc, tep));
                    foreach (string lo in Regex.Split(noiDung, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase))
                    {
                        if (lo.Trim().Length == 0) continue;
                        using (var cmd = new SqlCommand(lo, conn) { CommandTimeout = 120 })
                            cmd.ExecuteNonQuery();
                    }
                }
            }
            SqlConnection.ClearAllPools();
        }

        private static string ChuoiMaster()
        {
            var b = new SqlConnectionStringBuilder(Db.ChuoiKetNoi) { InitialCatalog = "master", ConnectTimeout = 15 };
            return b.ConnectionString;
        }
    }
}
