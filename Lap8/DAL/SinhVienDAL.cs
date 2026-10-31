using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Lab08_SqlCommand_Parameters.DTO;
using Lab08_SqlCommand_Parameters.GUI.Helpers;

namespace Lab08_SqlCommand_Parameters.DAL
{
    /// <summary>
    /// TANG TRUY CAP DU LIEU cho Lab 8 - the hien dung 3 phuong thuc thuc thi:
    ///
    ///   ExecuteScalar    (bai 1, 7)  : tra ve 1 gia tri (COUNT, MAX, AVG).
    ///   ExecuteReader    (bai 2, 5, 6): doc tuan tu, forward-only, read-only.
    ///   ExecuteNonQuery  (bai 3, 4, 8): INSERT/UPDATE/DELETE -> so dong anh huong.
    ///
    /// MOI cau lenh deu dung SqlParameter, KHONG noi chuoi SQL -> chong SQL Injection.
    /// Day la tang duy nhat biet ten bang/cot cua CSDL.
    /// </summary>
    public class SinhVienDAL
    {
        // =================================================================
        // BAI 1: Dem so sinh vien bang ExecuteScalar
        // =================================================================
        /// <summary>
        /// Lay so luong sinh vien. Gia tri ExecuteScalar tra ve la object:
        /// co the la int, co the la NULL -> phai chuyen doi an toan.
        /// </summary>
        public int CountStudents()
        {
            const string sql = "SELECT COUNT(*) FROM dbo.SinhVien";

            using (SqlConnection conn = DbConnectionFactory.Create())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                object kq = cmd.ExecuteScalar();
                // Convert.ToInt32(null) = 0, khong nem ngoai le nhu (int)null
                return Convert.ToInt32(kq);
            }
        }

        // =================================================================
        // BAI 2: Doc danh sach bang ExecuteReader
        // =================================================================
        public List<SinhVien> DocDanhSach()
        {
            var ds = new List<SinhVien>();
            const string sql =
                "SELECT MaSV, HoTen, NgaySinh, GioiTinh, MaKhoa, DiemTB, Email " +
                "FROM dbo.SinhVien ORDER BY HoTen";

            using (SqlConnection conn = DbConnectionFactory.Create())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                // SqlDataReader: forward-only, read-only -> doc 1 lan tu tren xuong
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ds.Add(DocMotDong(reader));
                    }
                }
            }
            return ds;
        }

        /// <summary>Doc 1 dong SqlDataReader thanh DTO (dung chung cho nhieu bai).</summary>
        private static SinhVien DocMotDong(SqlDataReader reader)
        {
            return new SinhVien
            {
                MaSV = reader.GetString(reader.GetOrdinal("MaSV")),
                HoTen = reader.GetString(reader.GetOrdinal("HoTen")),
                NgaySinh = reader.GetDateTime(reader.GetOrdinal("NgaySinh")),
                GioiTinh = reader.GetBoolean(reader.GetOrdinal("GioiTinh")),
                MaKhoa = reader.GetString(reader.GetOrdinal("MaKhoa")),
                DiemTB = reader.GetDecimal(reader.GetOrdinal("DiemTB")),
                // Email co the NULL trong CSDL -> kiem tra IsDBNull truoc khi GetString
                Email = reader.IsDBNull(reader.GetOrdinal("Email"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("Email"))
            };
        }

        // =================================================================
        // BAI 3: Them sinh vien bang ExecuteNonQuery + SqlParameter
        // =================================================================
        /// <summary>
        /// Them mot sinh vien. Tra ve so dong anh huong (1 = thanh cong).
        /// Gia tri NULL cua Email duoc thay bang DBNull.Value.
        /// </summary>
        public int ThemSinhVien(SinhVien sv)
        {
            const string sql = @"INSERT INTO dbo.SinhVien
                                    (MaSV, HoTen, NgaySinh, GioiTinh, MaKhoa, DiemTB, Email)
                                 VALUES
                                    (@MaSV, @HoTen, @NgaySinh, @GioiTinh, @MaKhoa, @DiemTB, @Email)";

            using (SqlConnection conn = DbConnectionFactory.Create())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                // Khai bao DUNG kieu + DO DAI cua cot trong CSDL.
                // Khong dung AddWithValue vi no tu doan kieu theo gia tri C#.
                cmd.Parameters.Add("@MaSV", SqlDbType.VarChar, 15).Value = sv.MaSV;
                cmd.Parameters.Add("@HoTen", SqlDbType.NVarChar, 100).Value = sv.HoTen;
                cmd.Parameters.Add("@NgaySinh", SqlDbType.Date).Value = sv.NgaySinh.Date;
                cmd.Parameters.Add("@GioiTinh", SqlDbType.Bit).Value = sv.GioiTinh;
                cmd.Parameters.Add("@MaKhoa", SqlDbType.VarChar, 10).Value = sv.MaKhoa;

                SqlParameter pDiem = cmd.Parameters.Add("@DiemTB", SqlDbType.Decimal);
                pDiem.Precision = 4;
                pDiem.Scale = 2;
                pDiem.Value = sv.DiemTB;

                cmd.Parameters.Add("@Email", SqlDbType.VarChar, 100).Value =
                    string.IsNullOrWhiteSpace(sv.Email) ? (object)DBNull.Value : sv.Email.Trim();

                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        // =================================================================
        // BAI 4: Cap nhat diem theo MaSV
        // =================================================================
        /// <summary>
        /// Cap nhat DiemTB theo MaSV. Tra ve 0 khi khong tim thay ma.
        /// Ma sinh viên la THAM SO, khong ghep vao chuoi SQL.
        /// </summary>
        public int CapNhatDiem(string maSV, decimal diemTB)
        {
            const string sql = "UPDATE dbo.SinhVien SET DiemTB = @DiemTB WHERE MaSV = @MaSV";

            using (SqlConnection conn = DbConnectionFactory.Create())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                SqlParameter pDiem = cmd.Parameters.Add("@DiemTB", SqlDbType.Decimal);
                pDiem.Precision = 4;
                pDiem.Scale = 2;
                pDiem.Value = diemTB;

                cmd.Parameters.Add("@MaSV", SqlDbType.VarChar, 15).Value = maSV;

                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        // =================================================================
        // BAI 5: Goi Stored Procedure sp_SinhVien_TheoKhoa
        // =================================================================
        public List<SinhVien> SinhVienTheoKhoa(string maKhoa)
        {
            var ds = new List<SinhVien>();

            using (SqlConnection conn = DbConnectionFactory.Create())
            using (SqlCommand cmd = new SqlCommand("dbo.sp_SinhVien_TheoKhoa", conn))
            {
                // Bat buoc: CommandType.StoredProcedure (mac dinh la Text)
                cmd.CommandType = CommandType.StoredProcedure;
                // Ten tham so trong C# phai KHOP ten tham so cua thu tuc
                cmd.Parameters.Add("@MaKhoa", SqlDbType.VarChar, 10).Value = maKhoa;

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ds.Add(DocMotDong(reader));
                    }
                }
            }
            return ds;
        }

        // =================================================================
        // BAI 6: Tim sinh vien theo ma (ExecuteReader + SqlParameter)
        // =================================================================
        /// <summary>Tra ve null neu khong tim thay (0 dong).</summary>
        public SinhVien TimTheoMa(string maSV)
        {
            const string sql =
                "SELECT sv.MaSV, sv.HoTen, sv.NgaySinh, sv.GioiTinh, sv.MaKhoa, " +
                "       k.TenKhoa, sv.DiemTB, sv.Email " +
                "FROM dbo.SinhVien sv " +
                "JOIN dbo.Khoa k ON k.MaKhoa = sv.MaKhoa " +
                "WHERE sv.MaSV = @MaSV";

            using (SqlConnection conn = DbConnectionFactory.Create())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@MaSV", SqlDbType.VarChar, 15).Value = maSV;

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return null;   // khong co dong nao

                    SinhVien sv = DocMotDong(reader);
                    sv.TenKhoa = reader.GetString(reader.GetOrdinal("TenKhoa"));
                    return sv;
                }
            }
        }

        // =================================================================
        // BAI 7: Thong ke diem cao nhat theo khoa (ExecuteScalar + NULL)
        // =================================================================
        /// <summary>
        /// Voi moi khoa: lay MAX(DiemTB) bang ExecuteScalar.
        /// Khoa chua co sinh vien -> MAX tra NULL -> phai kiem tra DBNull.
        /// </summary>
        public List<ThongKeKhoa> ThongKeDiemCaoNhatTheoKhoa()
        {
            var kq = new List<ThongKeKhoa>();
            List<Khoa> dsKhoa = new KhoaDAL().GetAll();

            const string sql =
                "SELECT COUNT(*) AS SoSV, MAX(DiemTB) AS CaoNhat, AVG(DiemTB) AS TrungBinh " +
                "FROM dbo.SinhVien WHERE MaKhoa = @MaKhoa";

            using (SqlConnection conn = DbConnectionFactory.Create())
            {
                conn.Open();

                foreach (Khoa khoa in dsKhoa)
                {
                    var tk = new ThongKeKhoa { MaKhoa = khoa.MaKhoa, TenKhoa = khoa.TenKhoa };

                    // Dung cung 1 ket noi cho ca vong lap -> it chi phi mo ket noi.
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@MaKhoa", SqlDbType.VarChar, 10).Value = khoa.MaKhoa;

                        using (SqlDataReader rd = cmd.ExecuteReader())
                        {
                            if (rd.Read())
                            {
                                tk.SoSinhVien = rd.GetInt32(rd.GetOrdinal("SoSV"));

                                // NULL-safe: MAX/AVG co the tra ve DBNull
                                object caoNhat = rd["CaoNhat"];
                                object trungBinh = rd["TrungBinh"];
                                tk.DiemCaoNhat = caoNhat == DBNull.Value ? (decimal?)null : Convert.ToDecimal(caoNhat);
                                tk.DiemTrungBinh = trungBinh == DBNull.Value ? (decimal?)null : Convert.ToDecimal(trungBinh);
                            }
                        }
                    }
                    kq.Add(tk);
                }
            }
            return kq;
        }

        /// <summary>
        /// Bien the dung ExecuteScalar thuan tuy cho 1 khoa - de doi chieu voi cach
        /// doc SqlDataReader o tren. Tra ve null khi khoa rong (MAX = NULL).
        /// </summary>
        public decimal? DiemCaoNhatCuaKhoa(string maKhoa)
        {
            const string sql = "SELECT MAX(DiemTB) FROM dbo.SinhVien WHERE MaKhoa = @MaKhoa";

            using (SqlConnection conn = DbConnectionFactory.Create())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@MaKhoa", SqlDbType.VarChar, 10).Value = maKhoa;
                conn.Open();

                object kq = cmd.ExecuteScalar();
                // ExecuteScalar tra ve NULL khi khong co dong / MAX khong co gia tri
                if (kq == null || kq == DBNull.Value) return null;
                return Convert.ToDecimal(kq);
            }
        }

        // =================================================================
        // BAI 8: Xoa sinh vien theo ma (ExecuteNonQuery + xu ly khoa ngoai)
        // =================================================================
        /// <summary>
        /// Xoa sinh vien. Neu sinh vien con dang ky hoc phan, SQL Server bao loi
        /// khoa ngoai (FK_DangKy_SV, so 547) -> DAL nem LoiNghiepVu de GUI hien thi.
        /// </summary>
        public int XoaSinhVien(string maSV)
        {
            const string sql = "DELETE FROM dbo.SinhVien WHERE MaSV = @MaSV";

            using (SqlConnection conn = DbConnectionFactory.Create())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@MaSV", SqlDbType.VarChar, 15).Value = maSV;

                conn.Open();
                try
                {
                    return cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 547)
                    {
                        throw new LoiNghiepVu("MaSV",
                            "Không xóa được: sinh viên \"" + maSV + "\" vẫn còn đăng ký học phần " +
                            "(ràng buộc khóa ngoại FK_DangKy_SV). Hãy xóa các đăng ký trước.");
                    }
                    throw;
                }
            }
        }

        // =================================================================
        // BAI 9: Stored Procedure co tham so OUTPUT
        // =================================================================
        /// <summary>
        /// Goi sp_SinhVien_Them: them sinh vien va nhan lai tong so sinh vien hien tai
        /// qua tham so OUTPUT @SoLuongHienTai.
        /// </summary>
        public int ThemSinhVienQuaThuTuc(SinhVien sv)
        {
            using (SqlConnection conn = DbConnectionFactory.Create())
            using (SqlCommand cmd = new SqlCommand("dbo.sp_SinhVien_Them", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@MaSV", SqlDbType.VarChar, 15).Value = sv.MaSV;
                cmd.Parameters.Add("@HoTen", SqlDbType.NVarChar, 100).Value = sv.HoTen;
                cmd.Parameters.Add("@NgaySinh", SqlDbType.Date).Value = sv.NgaySinh.Date;
                cmd.Parameters.Add("@GioiTinh", SqlDbType.Bit).Value = sv.GioiTinh;
                cmd.Parameters.Add("@MaKhoa", SqlDbType.VarChar, 10).Value = sv.MaKhoa;

                SqlParameter pDiem = cmd.Parameters.Add("@DiemTB", SqlDbType.Decimal);
                pDiem.Precision = 4;
                pDiem.Scale = 2;
                pDiem.Value = sv.DiemTB;

                cmd.Parameters.Add("@Email", SqlDbType.VarChar, 100).Value =
                    string.IsNullOrWhiteSpace(sv.Email) ? (object)DBNull.Value : sv.Email.Trim();

                // Tham so OUTPUT: phai khai bao Direction truoc khi ExecuteNonQuery
                SqlParameter pOut = new SqlParameter("@SoLuongHienTai", SqlDbType.Int);
                pOut.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(pOut);

                conn.Open();
                cmd.ExecuteNonQuery();

                // Doc gia tri OUTPUT sau khi lenh chay xong
                object giaTri = pOut.Value;
                return (giaTri == null || giaTri == DBNull.Value) ? 0 : Convert.ToInt32(giaTri);
            }
        }

        // =================================================================
        // TIEN ICH HIEN THI
        // =================================================================
        /// <summary>Lay danh sach ten khoa de do vao ComboBox.</summary>
        public List<Khoa> DanhSachKhoa()
        {
            return new KhoaDAL().GetAll();
        }

        /// <summary>Dem nhanh so ban ghi dang ky cua mot sinh vien (phuc vu bai 8).</summary>
        public int DemDangKy(string maSV)
        {
            const string sql = "SELECT COUNT(*) FROM dbo.DangKy WHERE MaSV = @MaSV";

            using (SqlConnection conn = DbConnectionFactory.Create())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@MaSV", SqlDbType.VarChar, 15).Value = maSV;
                conn.Open();
                object kq = cmd.ExecuteScalar();
                return Convert.ToInt32(kq);
            }
        }

        /// <summary>
        /// Xoa toan bo dang ky cua sinh vien (dung truoc khi xoa sinh viên o bai 8
        /// khi nguoi dung dong y "xoa ca dang ky").
        /// </summary>
        public int XoaDangKyCuaSinhVien(string maSV)
        {
            const string sql = "DELETE FROM dbo.DangKy WHERE MaSV = @MaSV";

            using (SqlConnection conn = DbConnectionFactory.Create())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@MaSV", SqlDbType.VarChar, 15).Value = maSV;
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }
    }
}
