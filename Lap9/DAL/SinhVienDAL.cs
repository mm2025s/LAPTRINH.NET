using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Lab09_DataReaderSearch.DTO;

namespace Lab09_DataReaderSearch.DAL
{
    /// <summary>
    /// Tang truy cap du lieu cho Lab 9 - TOAN BO nghiep vu doc/tim kiem/loc
    /// deu nam o day va deu dung SqlDataReader hoac DataTable.
    ///
    /// Nguyen tac chung cua ca file:
    ///   1. Khong noi chuoi gia tri vao SQL - moi gia tri deu la SqlParameter.
    ///   2. Chi noi chuoi voi MENH DE CO DINH (dieu kien WHERE tuy chon,
    ///      bieu thuc ORDER BY lay tu whitelist SinhVienCot).
    ///   3. SqlDataReader chi doc duoc khi connection con mo -> moi lan doc deu
    ///      nam trong khoi using(...) cua connection.
    ///   4. Truy van co phan trang phai kem 1 lan COUNT(*) de biet tong so trang.
    /// </summary>
    public class SinhVienDAL
    {
        /// <summary>Bieu thuc SELECT dung chung: JOIN Khoa + doi GioiTinh sang chuoi.</summary>
        private const string CotChon =
            @"SELECT sv.MaSV, sv.HoTen, sv.NgaySinh,
                     CASE sv.GioiTinh WHEN 1 THEN N'Nam' ELSE N'Nữ' END AS GioiTinh,
                     sv.MaKhoa, k.TenKhoa, sv.DiemTB, sv.Email
              FROM dbo.SinhVien sv
              JOIN dbo.Khoa k ON sv.MaKhoa = k.MaKhoa";

        // =====================================================================
        // BAI 1 - Nap SqlDataReader vao DataTable
        // =====================================================================

        /// <summary>
        /// Doc toan bo sinh vien kem ten khoa vao DataTable de day len DataGridView.
        ///
        /// Vi sao DataTable.Load(reader): SqlDataReader la con tro chi-tien
        /// (forward-only) va rang buoc vong doi connection. Load() doc het mot
        /// lan roi nap vao bang bo nho -> DataGridView co the sap xep / loc /
        /// duyet lui ma khong can ket noi nua.
        /// </summary>
        public DataTable LoadStudents()
        {
            string sql = CotChon + " ORDER BY sv.MaSV";
            var table = new DataTable();

            using (SqlConnection conn = DbConnectionFactory.Create())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    table.Load(reader);
                }
            }
            return table;
        }

        /// <summary>
        /// Cach thu hai: nap THU CONG tung dong bang reader.Read() + GetXxx.
        /// Dung de doi chieu voi DataTable.Load() (bai 1 yeu cau ca hai cach).
        /// Tra ve ca so dong da doc de giao dien hien thi.
        /// </summary>
        public DataTable NapThuCong(out int soDong)
        {
            string sql = CotChon + " ORDER BY sv.MaSV";
            var table = new DataTable();
            table.Columns.Add("MaSV", typeof(string));
            table.Columns.Add("HoTen", typeof(string));
            table.Columns.Add("NgaySinh", typeof(DateTime));
            table.Columns.Add("GioiTinh", typeof(string));
            table.Columns.Add("MaKhoa", typeof(string));
            table.Columns.Add("TenKhoa", typeof(string));
            table.Columns.Add("DiemTB", typeof(decimal));
            table.Columns.Add("Email", typeof(string));

            soDong = 0;
            using (SqlConnection conn = DbConnectionFactory.Create())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        soDong++;
                        object email = reader["Email"];   // co the la DBNull.Value

                        table.Rows.Add(
                            reader["MaSV"],
                            reader["HoTen"],
                            reader["NgaySinh"],
                            reader["GioiTinh"],
                            reader["MaKhoa"],
                            reader["TenKhoa"],
                            reader["DiemTB"],
                            email == DBNull.Value ? (object)null : email);
                    }
                }
            }
            return table;
        }

        // =====================================================================
        // BAI 2 - Tim kiem gan dung theo ho ten (LIKE + tham so)
        // =====================================================================

        /// <summary>
        /// Tim sinh vien co ho ten CHUA tu khoa.
        ///
        /// Diem quan trong: ky tu % duoc them vao GIA TRI THAM SO, khong phai
        /// ghep vao cau SQL. Nguoi dung go "'; DROP TABLE" chi duoc hieu la
        /// mot chuoi can tim, khong phai lenh.
        /// </summary>
        public DataTable SearchByName(string keyword)
        {
            string sql = CotChon + " WHERE sv.HoTen LIKE @Keyword ESCAPE '\\' ORDER BY sv.HoTen";

            var table = new DataTable();
            using (SqlConnection conn = DbConnectionFactory.Create())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                // % do NGUOI DUNG go phai la ky tu van ban, khong phai wildcard
                cmd.Parameters.Add("@Keyword", SqlDbType.NVarChar, 200).Value =
                    "%" + EscapeLike((keyword ?? string.Empty).Trim()) + "%";

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    table.Load(reader);
                }
            }
            return table;
        }

        // =====================================================================
        // BAI 3 - Loc theo khoa va khoang diem (WHERE dung dan)
        // =====================================================================

        /// <summary>
        /// Loc theo khoa (null = Tat ca) va khoang diem.
        ///
        /// Cau SQL duoc dung dan theo bo loc da chon, nhung phan duoc noi them
        /// chi la menh de co dinh " AND MaKhoa = @MaKhoa" - con gia tri khoa
        /// van di qua tham so.
        /// </summary>
        public DataTable FilterByKhoaAndDiem(string maKhoa, decimal? diemTu, decimal? diemDen)
        {
            string sql = CotChon + " WHERE 1 = 1";
            if (!string.IsNullOrEmpty(maKhoa)) sql += " AND sv.MaKhoa = @MaKhoa";
            if (diemTu.HasValue) sql += " AND sv.DiemTB >= @DiemTu";
            if (diemDen.HasValue) sql += " AND sv.DiemTB <= @DiemDen";
            sql += " ORDER BY sv.DiemTB DESC, sv.MaSV";

            var table = new DataTable();
            using (SqlConnection conn = DbConnectionFactory.Create())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                if (!string.IsNullOrEmpty(maKhoa))
                    cmd.Parameters.Add("@MaKhoa", SqlDbType.VarChar, 10).Value = maKhoa;

                if (diemTu.HasValue)
                    cmd.Parameters.Add(TaoThamSoDiem("@DiemTu", diemTu.Value));

                if (diemDen.HasValue)
                    cmd.Parameters.Add(TaoThamSoDiem("@DiemDen", diemDen.Value));

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    table.Load(reader);
                }
            }
            return table;
        }

        /// <summary>
        /// Loc bang BETWEEN khi CO DU ca hai dau (bai 3 - code mau).
        /// Giữ riêng để đối chiếu với cách dùng &gt;= / &lt;= ở trên: BETWEEN là
        /// viết tắt của hai điều kiện và bao gồm cả hai biên.
        /// </summary>
        public DataTable FilterByDiemBetween(decimal diemTu, decimal diemDen)
        {
            string sql = CotChon + " WHERE sv.DiemTB BETWEEN @Min AND @Max ORDER BY sv.DiemTB";

            var table = new DataTable();
            using (SqlConnection conn = DbConnectionFactory.Create())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add(TaoThamSoDiem("@Min", diemTu));
                cmd.Parameters.Add(TaoThamSoDiem("@Max", diemDen));

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    table.Load(reader);
                }
            }
            return table;
        }

        // =====================================================================
        // BAI 4 - Phan trang bang OFFSET - FETCH
        // =====================================================================

        /// <summary>
        /// Lay MOT trang du lieu.
        ///
        /// OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY yeu cau BAT BUOC
        /// co ORDER BY. So dong bo qua = (trang - 1) * kichThuocTrang.
        /// Offset va PageSize deu la tham so kieu Int.
        ///
        /// Ngoai trang du lieu con chay them COUNT(*) de biet tong so trang -
        /// neu khong co, giao dien khong the biet khi nao dung nut "Sau".
        /// </summary>
        public KetQuaPhanTrang LoadTrang(int chiSoTrang, int kichThuocTrang,
            string cotSapXep = null, bool giamDan = false)
        {
            if (chiSoTrang < 1) chiSoTrang = 1;
            if (kichThuocTrang < 1) kichThuocTrang = 10;

            int offset = (chiSoTrang - 1) * kichThuocTrang;
            string orderBy = SinhVienCot.BieuThucOrderBy(cotSapXep, giamDan);

            string sql =
                CotChon +
                " ORDER BY " + orderBy + "\n" +
                " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var table = new DataTable();
            using (SqlConnection conn = DbConnectionFactory.Create())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@Offset", SqlDbType.Int).Value = offset;
                    cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = kichThuocTrang;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        table.Load(reader);
                    }
                }

                int tong = DemTong(null, conn);

                return new KetQuaPhanTrang
                {
                    Bang = table,
                    TongSoDong = tong,
                    ChiSoTrang = chiSoTrang,
                    KichThuocTrang = kichThuocTrang
                };
            }
        }

        /// <summary>
        /// Dem tong so dong theo bo loc - dung CHUNG connection de khong phai mo
        /// them ket noi thu hai khi vua phan trang vua loc.
        /// </summary>
        private static int DemTong(TieuChiTimKiem tc, SqlConnection conn)
        {
            string sqlWhere = string.Empty;
            List<SqlParameter> thamSo = TaoDieuKien(tc, ref sqlWhere);

            string sql = "SELECT COUNT(*) FROM dbo.SinhVien sv JOIN dbo.Khoa k ON sv.MaKhoa = k.MaKhoa" + sqlWhere;

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                foreach (SqlParameter p in thamSo) cmd.Parameters.Add(p);

                object kq = cmd.ExecuteScalar();
                return kq == null || kq == DBNull.Value ? 0 : Convert.ToInt32(kq);
            }
        }

        /// <summary>
        /// Tim kiem DA TIEU CHI (bai 6) ket hop phan trang.
        /// Moi tieu chi bo trong se khong sinh ra dieu kien tuong ung.
        /// </summary>
        public KetQuaPhanTrang TimKiemPhanTrang(TieuChiTimKiem tc, int chiSoTrang, int kichThuocTrang)
        {
            if (tc == null) tc = new TieuChiTimKiem();
            if (chiSoTrang < 1) chiSoTrang = 1;
            if (kichThuocTrang < 1) kichThuocTrang = 10;

            string sqlWhere = string.Empty;
            List<SqlParameter> thamSo = TaoDieuKien(tc, ref sqlWhere);

            string orderBy = SinhVienCot.BieuThucOrderBy(tc.CotSapXep, tc.GiamDan);

            string sql =
                CotChon + sqlWhere + "\n" +
                " ORDER BY " + orderBy + "\n" +
                " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            string sqlDem = "SELECT COUNT(*) FROM dbo.SinhVien sv JOIN dbo.Khoa k ON sv.MaKhoa = k.MaKhoa" + sqlWhere;

            var table = new DataTable();
            using (SqlConnection conn = DbConnectionFactory.Create())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    foreach (SqlParameter p in thamSo) cmd.Parameters.Add(SaoChep(p));
                    cmd.Parameters.Add("@Offset", SqlDbType.Int).Value = (chiSoTrang - 1) * kichThuocTrang;
                    cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = kichThuocTrang;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        table.Load(reader);
                    }
                }

                int tong;
                using (SqlCommand cmdDem = new SqlCommand(sqlDem, conn))
                {
                    foreach (SqlParameter p in thamSo) cmdDem.Parameters.Add(SaoChep(p));
                    object kq = cmdDem.ExecuteScalar();
                    tong = kq == null || kq == DBNull.Value ? 0 : Convert.ToInt32(kq);
                }

                return new KetQuaPhanTrang
                {
                    Bang = table,
                    TongSoDong = tong,
                    ChiSoTrang = chiSoTrang,
                    KichThuocTrang = kichThuocTrang
                };
            }
        }

        // =====================================================================
        // BAI 5 - Doc mot dong chi tiet (dong dang chon tren DataGridView)
        // =====================================================================

        /// <summary>
        /// Doc chi tiet mot sinh vien theo MaSV bang SqlDataReader.
        ///
        /// Vi du ve xu ly DBNull: cot Email co the la NULL trong CSDL, khi do
        /// reader["Email"] la DBNull.Value chu khong phai null -> phai kiem tra
        /// truoc khi ep kieu, neu khong se nem InvalidCastException.
        /// </summary>
        public SinhVienChiTiet LayChiTiet(string maSV)
        {
            string sql = CotChon + " WHERE sv.MaSV = @MaSV";

            using (SqlConnection conn = DbConnectionFactory.Create())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@MaSV", SqlDbType.VarChar, 15).Value =
                    (maSV ?? string.Empty).Trim();

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return new SinhVienChiTiet();   // khong co dong nao

                    var sv = new SinhVienChiTiet
                    {
                        MaSV = DocChuoi(reader, "MaSV"),
                        HoTen = DocChuoi(reader, "HoTen"),
                        NgaySinh = DocNgay(reader, "NgaySinh"),
                        GioiTinhText = DocChuoi(reader, "GioiTinh"),
                        MaKhoa = DocChuoi(reader, "MaKhoa"),
                        TenKhoa = DocChuoi(reader, "TenKhoa"),
                        DiemTB = DocSo(reader, "DiemTB"),
                        Email = DocChuoi(reader, "Email")
                    };

                    sv.Tuoi = TinhTuoi(sv.NgaySinh);
                    return sv;
                }
            }
        }

        // =====================================================================
        // BAI 7 - Sap xep theo cot duoc chon
        // =====================================================================

        /// <summary>
        /// Danh sach sap xep KHONG phan trang (dung khi chi can xem thu tu).
        /// Cot duoc kiem tra qua whitelist truoc khi ghep vao ORDER BY.
        /// </summary>
        public DataTable SapXep(string cot, bool giamDan)
        {
            string sql = CotChon + " ORDER BY " + SinhVienCot.BieuThucOrderBy(cot, giamDan);

            var table = new DataTable();
            using (SqlConnection conn = DbConnectionFactory.Create())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    table.Load(reader);
                }
            }
            return table;
        }

        // =====================================================================
        // BAI 8 - Loc theo do tuoi
        // =====================================================================

        /// <summary>
        /// Loc theo khoang tuoi.
        ///
        /// Tuoi KHONG duoc luu trong CSDL nen phai suy ra tu NgaySinh. Cach don
        /// gian nhat la doi khoang tuoi thanh khoang NGAY SINH roi so sanh:
        ///     tuoi &gt;= 20  <=&gt;  NgaySinh &lt;= hom nay - 20 nam
        ///     tuoi &lt;= 25  &lt;=&gt;  NgaySinh &gt;  hom nay - 26 nam
        ///
        /// Lam o phia SQL se dung duoc INDEX tren NgaySinh; neu doi sang
        /// DATEDIFF(YEAR,...) trong WHERE thi SQL Server phai tinh cho tung
        /// dong va khong dung duoc index.
        /// </summary>
        public DataTable LocTheoTuoi(int? tuoiTu, int? tuoiDen)
        {
            string sql = CotChon + " WHERE 1 = 1";
            if (tuoiTu.HasValue) sql += " AND sv.NgaySinh <= @NgaySinhLonNhat";
            if (tuoiDen.HasValue) sql += " AND sv.NgaySinh > @NgaySinhNhoNhat";
            sql += " ORDER BY sv.NgaySinh DESC";

            DateTime homNay = DateTime.Today;

            var table = new DataTable();
            using (SqlConnection conn = DbConnectionFactory.Create())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                if (tuoiTu.HasValue)
                    cmd.Parameters.Add("@NgaySinhLonNhat", SqlDbType.Date).Value =
                        homNay.AddYears(-tuoiTu.Value);

                if (tuoiDen.HasValue)
                    cmd.Parameters.Add("@NgaySinhNhoNhat", SqlDbType.Date).Value =
                        homNay.AddYears(-(tuoiDen.Value + 1));

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    table.Load(reader);
                }
            }
            return table;
        }

        // =====================================================================
        // BAI 8 (phu tro) - Thong ke nhanh de doi chieu ket qua loc
        // =====================================================================

        /// <summary>Dem so sinh vien theo tung nhom tuoi - dung ExecuteScalar.</summary>
        public int DemTheoTuoi(int tuoi)
        {
            string sql = @"SELECT COUNT(*) FROM dbo.SinhVien
                           WHERE NgaySinh <= @LonNhat AND NgaySinh > @NhoNhat";

            DateTime homNay = DateTime.Today;
            using (SqlConnection conn = DbConnectionFactory.Create())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@LonNhat", SqlDbType.Date).Value = homNay.AddYears(-tuoi);
                cmd.Parameters.Add("@NhoNhat", SqlDbType.Date).Value = homNay.AddYears(-(tuoi + 1));
                conn.Open();
                object kq = cmd.ExecuteScalar();
                return kq == null || kq == DBNull.Value ? 0 : Convert.ToInt32(kq);
            }
        }

        // =====================================================================
        // Tien ich dung chung
        // =====================================================================

        /// <summary>Danh sach khoa cho ComboBox loc.</summary>
        public List<Khoa> DanhSachKhoa()
        {
            var ds = new List<Khoa>();
            using (SqlConnection conn = DbConnectionFactory.Create())
            using (SqlCommand cmd = new SqlCommand("SELECT MaKhoa, TenKhoa FROM dbo.Khoa ORDER BY TenKhoa", conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ds.Add(new Khoa
                        {
                            MaKhoa = Convert.ToString(reader["MaKhoa"]),
                            TenKhoa = Convert.ToString(reader["TenKhoa"])
                        });
                    }
                }
            }
            return ds;
        }

        /// <summary>
        /// Sinh menh de WHERE tu TieuChiTimKiem.
        ///
        /// Tra ve danh sach tham so tuong ung; sqlWhere duoc NOI THEM (truyen
        /// bang ref) chi voi cac menh de CO DINH. Moi gia tri deu qua tham so
        /// nen du nguoi dung nhap gi cung khong the thay doi cau lenh.
        /// </summary>
        private static List<SqlParameter> TaoDieuKien(TieuChiTimKiem tc, ref string sqlWhere)
        {
            var thamSo = new List<SqlParameter>();
            if (tc == null) return thamSo;

            if (!string.IsNullOrEmpty(tc.MaSV))
            {
                sqlWhere += " AND sv.MaSV LIKE @MaSV ESCAPE '\\'";
                thamSo.Add(SqlHelper.P("@MaSV", SqlDbType.VarChar, "%" + EscapeLike(tc.MaSV.Trim()) + "%", 40));
            }

            if (!string.IsNullOrEmpty(tc.HoTen))
            {
                sqlWhere += " AND sv.HoTen LIKE @HoTen ESCAPE '\\'";
                thamSo.Add(SqlHelper.P("@HoTen", SqlDbType.NVarChar, "%" + EscapeLike(tc.HoTen.Trim()) + "%", 200));
            }

            if (!string.IsNullOrEmpty(tc.MaKhoa))
            {
                sqlWhere += " AND sv.MaKhoa = @MaKhoa";
                thamSo.Add(SqlHelper.P("@MaKhoa", SqlDbType.VarChar, tc.MaKhoa, 10));
            }

            if (tc.GioiTinh.HasValue)
            {
                sqlWhere += " AND sv.GioiTinh = @GioiTinh";
                thamSo.Add(SqlHelper.P("@GioiTinh", SqlDbType.Bit, tc.GioiTinh.Value));
            }

            if (tc.DiemTu.HasValue)
            {
                sqlWhere += " AND sv.DiemTB >= @DiemTu";
                thamSo.Add(TaoThamSoDiem("@DiemTu", tc.DiemTu.Value));
            }

            if (tc.DiemDen.HasValue)
            {
                sqlWhere += " AND sv.DiemTB <= @DiemDen";
                thamSo.Add(TaoThamSoDiem("@DiemDen", tc.DiemDen.Value));
            }

            if (tc.TuoiTu.HasValue)
            {
                sqlWhere += " AND sv.NgaySinh <= @NgaySinhLonNhat";
                thamSo.Add(SqlHelper.P("@NgaySinhLonNhat", SqlDbType.Date,
                    DateTime.Today.AddYears(-tc.TuoiTu.Value)));
            }

            if (tc.TuoiDen.HasValue)
            {
                sqlWhere += " AND sv.NgaySinh > @NgaySinhNhoNhat";
                thamSo.Add(SqlHelper.P("@NgaySinhNhoNhat", SqlDbType.Date,
                    DateTime.Today.AddYears(-(tc.TuoiDen.Value + 1))));
            }

            return thamSo;
        }

        /// <summary>
        /// Vo hieu hoa ky tu dai dien cua LIKE trong chuoi nguoi dung nhap.
        ///
        /// Neu khong lam viec nay, go "%" thanh "%%%" va khop TOAN BO bang;
        /// go "_" se khop bat ky mot ky tu nao. Ky tu escape phai khop voi menh
        /// de ESCAPE '\' di kem trong cau lenh SQL.
        ///
        /// Thu tu quan trong: phai escape dau \ TRUOC, neu khong cac dau \ vua
        /// them o buoc sau se bi lap doi lan nua.
        /// </summary>
        private static string EscapeLike(string giaTri)
        {
            if (string.IsNullOrEmpty(giaTri)) return string.Empty;

            return giaTri
                .Replace("\\", "\\\\")
                .Replace("%", "\\%")
                .Replace("_", "\\_")
                .Replace("[", "\\[");
        }

        /// <summary>
        /// Tham so DECIMAL phai khai bao Precision/Scale, neu khong ADO.NET lay
        /// mac dinh (27,0) va phan le bi cat -> 8.75 thanh 9.
        /// </summary>
        private static SqlParameter TaoThamSoDiem(string ten, decimal giaTri)
        {
            var p = new SqlParameter(ten, SqlDbType.Decimal);
            p.Precision = 4;
            p.Scale = 2;
            p.Value = giaTri;
            return p;
        }

        /// <summary>Sao chep tham so de dung lai duoc cho command thu hai.</summary>
        private static SqlParameter SaoChep(SqlParameter goc)
        {
            var p = new SqlParameter(goc.ParameterName, goc.SqlDbType);
            p.Size = goc.Size;
            p.Precision = goc.Precision;
            p.Scale = goc.Scale;
            p.Value = goc.Value;
            return p;
        }

        // ---- Doc gia tri co kiem tra DBNull (bai 5) ----

        private static string DocChuoi(SqlDataReader reader, string cot)
        {
            object v = reader[cot];
            return v == null || v == DBNull.Value ? null : Convert.ToString(v);
        }

        private static DateTime? DocNgay(SqlDataReader reader, string cot)
        {
            object v = reader[cot];
            return v == null || v == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(v);
        }

        private static decimal? DocSo(SqlDataReader reader, string cot)
        {
            object v = reader[cot];
            return v == null || v == DBNull.Value ? (decimal?)null : Convert.ToDecimal(v);
        }

        /// <summary>
        /// Tuoi chinh xac: DATEDIFF(YEAR,...) chi dem so lan doi nam, nen nguoi
        /// sinh 31/12/2005 vao 01/01/2026 se bi tinh la 1 tuoi neu khong tru.
        /// </summary>
        public static int? TinhTuoi(DateTime? ngaySinh)
        {
            if (!ngaySinh.HasValue) return null;

            DateTime d = ngaySinh.Value.Date;
            DateTime homNay = DateTime.Today;

            int tuoi = homNay.Year - d.Year;
            if (d.AddYears(tuoi) > homNay) tuoi--;
            return tuoi < 0 ? 0 : tuoi;
        }
    }
}
