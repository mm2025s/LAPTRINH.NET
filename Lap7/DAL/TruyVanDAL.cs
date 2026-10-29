using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Lab07_ADONET_Connection.DTO;

namespace Lab07_ADONET_Connection.DAL
{
    /// <summary>
    /// Truy van du lieu mau tu QLSinhVien (bai 5 - tong quan ADO.NET).
    ///
    /// Minh hoa 2 cach doc du lieu pho bien:
    ///   - SqlDataReader : doc tuan tu, nhanh, chi doc (GetKhoa, GetSinhVien...).
    ///   - SqlDataAdapter + DataTable : do vao DataGridView (GetBang).
    /// Moi ket noi deu duoc dong bang using(...) - khong ro ri ket noi.
    /// </summary>
    public class TruyVanDAL
    {
        // =================================================================
        // DOC BANG SqlDataReader
        // =================================================================
        public List<Khoa> GetKhoa()
        {
            var ds = new List<Khoa>();
            const string sql = "SELECT MaKhoa, TenKhoa FROM dbo.Khoa ORDER BY MaKhoa";

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

        public List<SinhVien> GetSinhVien()
        {
            var ds = new List<SinhVien>();
            const string sql =
                "SELECT MaSV, HoTen, NgaySinh, GioiTinh, MaKhoa, DiemTB, Email " +
                "FROM dbo.SinhVien ORDER BY MaSV";

            using (SqlConnection conn = DbConnectionFactory.Create())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        ds.Add(new SinhVien
                        {
                            MaSV = rd.GetString(rd.GetOrdinal("MaSV")),
                            HoTen = rd.GetString(rd.GetOrdinal("HoTen")),
                            NgaySinh = rd.GetDateTime(rd.GetOrdinal("NgaySinh")),
                            GioiTinh = rd.GetBoolean(rd.GetOrdinal("GioiTinh")),
                            MaKhoa = rd.GetString(rd.GetOrdinal("MaKhoa")),
                            DiemTB = rd.GetDecimal(rd.GetOrdinal("DiemTB")),
                            Email = rd.IsDBNull(rd.GetOrdinal("Email")) ? null : rd.GetString(rd.GetOrdinal("Email"))
                        });
                    }
                }
            }
            return ds;
        }

        /// <summary>Lay danh sach dang ky kem ten sinh vien / ten hoc phan (JOIN).</summary>
        public List<DangKy> GetDangKy()
        {
            var ds = new List<DangKy>();
            const string sql =
                "SELECT dk.MaSV, sv.HoTen, dk.MaHP, hp.TenHP, dk.HocKy, dk.Diem, dk.NgayDangKy " +
                "FROM dbo.DangKy dk " +
                "JOIN dbo.SinhVien sv ON sv.MaSV = dk.MaSV " +
                "JOIN dbo.HocPhan hp  ON hp.MaHP = dk.MaHP " +
                "ORDER BY dk.MaSV, dk.MaHP";

            using (SqlConnection conn = DbConnectionFactory.Create())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        ds.Add(new DangKy
                        {
                            MaSV = rd.GetString(rd.GetOrdinal("MaSV")),
                            HoTen = rd.GetString(rd.GetOrdinal("HoTen")),
                            MaHP = rd.GetString(rd.GetOrdinal("MaHP")),
                            TenHP = rd.GetString(rd.GetOrdinal("TenHP")),
                            HocKy = rd.GetInt32(rd.GetOrdinal("HocKy")),
                            Diem = rd.IsDBNull(rd.GetOrdinal("Diem")) ? (decimal?)null : rd.GetDecimal(rd.GetOrdinal("Diem")),
                            NgayDangKy = rd.GetDateTime(rd.GetOrdinal("NgayDangKy"))
                        });
                    }
                }
            }
            return ds;
        }

        // =================================================================
        // DOC BANG SqlDataAdapter -> DataTable (de do vao DataGridView)
        // =================================================================
        /// <summary>
        /// Chay mot cau SELECT tuong minh va tra ve DataTable.
        /// Dung cho DataGridView: Adapter tu mo/dong ket noi khi Fill.
        /// </summary>
        public DataTable GetBang(string sql)
        {
            var table = new DataTable();
            using (SqlConnection conn = DbConnectionFactory.Create())
            using (SqlDataAdapter adapter = new SqlDataAdapter(sql, conn))
            {
                adapter.Fill(table);
            }
            return table;
        }

        /// <summary>Doc 1 gia tri duy nhat (vd: SELECT COUNT(*) FROM ...).</summary>
        public int DemBanGhi(string tenBang)
        {
            // ten bang duoc loc theo danh sach cho phep -> tranh SQL injection
            string bang = LocTenBang(tenBang);
            using (SqlConnection conn = DbConnectionFactory.Create())
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM dbo." + bang, conn))
            {
                conn.Open();
                object kq = cmd.ExecuteScalar();
                return kq == null ? 0 : Convert.ToInt32(kq);
            }
        }

        private static string LocTenBang(string tenBang)
        {
            string[] choPhep = { "Khoa", "SinhVien", "HocPhan", "DangKy" };
            foreach (string b in choPhep)
            {
                if (string.Equals(b, tenBang, StringComparison.OrdinalIgnoreCase)) return b;
            }
            throw new ArgumentException("Ten bang khong hop le: " + tenBang);
        }

        // =================================================================
        // METADATA - phuc vu ve so do quan he (frmSoDoQuanHe)
        // =================================================================
        /// <summary>Lay danh sach cot cua mot bang (ten, kieu, cho phep NULL, co PK).</summary>
        public DataTable GetCotCuaBang(string tenBang)
        {
            const string sql =
                "SELECT c.COLUMN_NAME, c.DATA_TYPE, c.CHARACTER_MAXIMUM_LENGTH, c.IS_NULLABLE, " +
                "       CASE WHEN pk.COLUMN_NAME IS NULL THEN 0 ELSE 1 END AS IS_PK " +
                "FROM INFORMATION_SCHEMA.COLUMNS c " +
                "LEFT JOIN (" +
                "   SELECT ku.TABLE_NAME, ku.COLUMN_NAME " +
                "   FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc " +
                "   JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE ku " +
                "     ON ku.CONSTRAINT_NAME = tc.CONSTRAINT_NAME " +
                "   WHERE tc.CONSTRAINT_TYPE = 'PRIMARY KEY') pk " +
                "  ON pk.TABLE_NAME = c.TABLE_NAME AND pk.COLUMN_NAME = c.COLUMN_NAME " +
                "WHERE c.TABLE_NAME = @bang " +
                "ORDER BY c.ORDINAL_POSITION";

            var table = new DataTable();
            using (SqlConnection conn = DbConnectionFactory.Create())
            using (SqlDataAdapter adapter = new SqlDataAdapter(sql, conn))
            {
                adapter.SelectCommand.Parameters.AddWithValue("@bang", tenBang);
                adapter.Fill(table);
            }
            return table;
        }

        /// <summary>Lay danh sach quan he khoa ngoai (bang con -> bang cha).</summary>
        public DataTable GetQuanHe()
        {
            const string sql =
                "SELECT " +
                "   OBJECT_NAME(fk.parent_object_id) AS BangCon, " +
                "   COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS CotCon, " +
                "   OBJECT_NAME(fk.referenced_object_id) AS BangCha, " +
                "   COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS CotCha, " +
                "   fk.name AS TenFK " +
                "FROM sys.foreign_keys fk " +
                "JOIN sys.foreign_key_columns fkc ON fkc.constraint_object_id = fk.object_id " +
                "ORDER BY BangCon, TenFK";

            var table = new DataTable();
            using (SqlConnection conn = DbConnectionFactory.Create())
            using (SqlDataAdapter adapter = new SqlDataAdapter(sql, conn))
            {
                adapter.Fill(table);
            }
            return table;
        }
    }
}
