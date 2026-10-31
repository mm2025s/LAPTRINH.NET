using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Lab10_DisconnectedData.DTO;

namespace Lab10_DisconnectedData.DAL
{
    /// <summary>
    /// TANG DAL CUA LAB 10 - MO HINH NGAT KET NOI.
    ///
    /// Khac han Lab 9: o day khong giu SqlDataReader mo. DataAdapter lam viec
    /// theo 3 buoc, va buoc 2 chay khi KHONG con ket noi:
    ///     Fill(dataset)  -> mo ket noi, doc, dong ket noi, do du lieu vao bang
    ///     (xu ly tren bang trong bo nho: loc, sua, them, xoa...)
    ///     Update(bang)   -> mo ket noi, gui cac thay doi, dong ket noi
    ///
    /// Vi vay Adapter tu mo/dong ket noi khi Fill/Update neu connection dang
    /// dong - khong can viet conn.Open() thu cong (nhung phai Close dung luc).
    /// </summary>
    public class SinhVienDAL
    {
        public const string TenBangKhoa = "Khoa";
        public const string TenBangSinhVien = "SinhVien";
        public const string TenQuanHe = "FK_Khoa_SinhVien";

        // ------------------------------------------------------------------
        // BAI 1 - Fill DataTable bang SqlDataAdapter
        // ------------------------------------------------------------------

        /// <summary>
        /// Nap bang SinhVien vao DataTable roi DONG ket noi ngay.
        ///
        /// Dung using cho ca SqlConnection lan SqlDataAdapter thi ket noi duoc
        /// dong ngay sau Fill (Fill tu mo va tu dong, nhung KHONG tu dong lai
        /// connection ben ngoai - no chi dong cai no da mo).
        /// </summary>
        public DataTable GetStudentTable()
        {
            var table = new DataTable(TenBangSinhVien);

            using (var conn = DbConnectionFactory.Create())
            using (var adapter = new SqlDataAdapter(
                "SELECT MaSV, HoTen, NgaySinh, GioiTinh, MaKhoa, DiemTB, Email " +
                "FROM dbo.SinhVien ORDER BY MaSV", conn))
            {
                adapter.Fill(table);
            }

            return table;
        }

        /// <summary>Nap bang Khoa (danh muc nho) - dung cho ComboBox.</summary>
        public DataTable GetKhoaTable()
        {
            var table = new DataTable(TenBangKhoa);

            using (var conn = DbConnectionFactory.Create())
            using (var adapter = new SqlDataAdapter(
                "SELECT MaKhoa, TenKhoa FROM dbo.Khoa ORDER BY MaKhoa", conn))
            {
                adapter.Fill(table);
            }

            return table;
        }

        // ------------------------------------------------------------------
        // BAI 2 - DataSet chua nhieu bang
        // ------------------------------------------------------------------

        /// <summary>
        /// Nap ca Khoa va SinhVien vao MOT DataSet, dat ten bang ro rang.
        ///
        /// Ten bang quan trong: ds.Tables["SinhVien"] luon dung, con
        /// ds.Tables[1] se sai ngay khi ai do doi thu tu nap.
        /// </summary>
        public DataSet GetDataSet()
        {
            var ds = new DataSet("QLSinhVien");

            using (var conn = DbConnectionFactory.Create())
            {
                new SqlDataAdapter("SELECT MaKhoa, TenKhoa FROM dbo.Khoa ORDER BY MaKhoa", conn)
                    .Fill(ds, TenBangKhoa);

                new SqlDataAdapter(
                    "SELECT MaSV, HoTen, NgaySinh, GioiTinh, MaKhoa, DiemTB, Email " +
                    "FROM dbo.SinhVien ORDER BY MaSV", conn)
                    .Fill(ds, TenBangSinhVien);
            }

            return ds;
        }

        // ------------------------------------------------------------------
        // BAI 3 - DataRelation
        // ------------------------------------------------------------------

        /// <summary>
        /// Tao quan he cha-con giua Khoa.MaKhoa va SinhVien.MaKhoa.
        ///
        /// DataRelation chi tao duoc khi du lieu da toan ven: moi gia tri
        /// SinhVien.MaKhoa phai co trong Khoa.MaKhoa, neu khong
        /// ds.Relations.Add() nem InvalidConstraintException.
        ///
        /// createConstraints: true (mac dinh) se tao ForeignKeyConstraint tuong
        /// ung -> xoa mot dong Khoa con con sinh vien se bi chan, giong FK that
        /// trong CSDL. Day la diem khac biet so voi chi loc bang vong lap.
        /// </summary>
        public DataRelation TaoQuanHeKhoaSinhVien(DataSet ds)
        {
            if (ds == null) throw new ArgumentNullException("ds");
            if (ds.Relations.Contains(TenQuanHe))
                return ds.Relations[TenQuanHe];

            DataTable khoa = ds.Tables[TenBangKhoa];
            DataTable sinhVien = ds.Tables[TenBangSinhVien];

            if (khoa == null || sinhVien == null)
                throw new InvalidOperationException("DataSet phải có đủ bảng Khoa và SinhVien.");

            // createConstraints = false: ta tu tao ForeignKeyConstraint ben duoi de
            // dat duoc DeleteRule. Neu de true, ADO.NET gan DeleteRule = Cascade
            // MAC DINH - xoa mot dong Khoa se keo theo toan bo sinh vien cua khoa
            // do (khong bao loi). SQL Server thi mac dinh la NO ACTION - chan lai.
            var quanHe = new DataRelation(
                TenQuanHe,
                khoa.Columns["MaKhoa"],
                sinhVien.Columns["MaKhoa"],
                false);

            ds.Relations.Add(quanHe);

            // Rang buoc khoa ngoai tuong duong FK that trong CSDL
            var fk = new ForeignKeyConstraint(
                "FK_Khoa_SinhVien_Constraint",
                khoa.Columns["MaKhoa"],
                sinhVien.Columns["MaKhoa"]);

            fk.DeleteRule = Rule.None;                  // giống NO ACTION của SQL Server
            fk.UpdateRule = Rule.Cascade;               // đổi MaKhoa thì đẩy xuống bảng con
            fk.AcceptRejectRule = AcceptRejectRule.Cascade;

            sinhVien.Constraints.Add(fk);

            return quanHe;
        }

        /// <summary>
        /// Kiem tra du lieu co toan ven khoa ngoai TRUOC khi tao quan he.
        /// Tra ve danh sach MaKhoa "mo coi" (co trong SinhVien nhung khong co
        /// trong Khoa) de giao dien bao loi ro rang thay vi de .NET nem exception.
        /// </summary>
        public static string TimKhoaMoCoi(DataSet ds)
        {
            if (ds == null || !ds.Tables.Contains(TenBangKhoa) || !ds.Tables.Contains(TenBangSinhVien))
                return null;

            DataTable khoa = ds.Tables[TenBangKhoa];
            DataTable sinhVien = ds.Tables[TenBangSinhVien];

            var hopLe = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (DataRow r in khoa.Rows) hopLe.Add(Convert.ToString(r["MaKhoa"]));

            var moCoi = new System.Collections.Generic.List<string>();
            foreach (DataRow r in sinhVien.Rows)
            {
                string ma = Convert.ToString(r["MaKhoa"]);
                if (!hopLe.Contains(ma) && !moCoi.Contains(ma)) moCoi.Add(ma);
            }

            return moCoi.Count == 0 ? null : string.Join(", ", moCoi.ToArray());
        }

        // ------------------------------------------------------------------
        // BAI 4 - DataView: loc va sap xep trong bo nho
        // ------------------------------------------------------------------

        /// <summary>
        /// Tra ve DataView tren bang SinhVien trong bo nho.
        ///
        /// DataView KHONG truy van lai CSDL: no chi la mot lop nhin (view) tren
        /// DataTable da co san. Doi RowFilter/Sort la tinh lai ngay lap tuc.
        ///
        /// RowFilter la mot BIEU THUC dang chuoi, KHONG phai tham so hoa duoc ->
        /// gia tri chuoi phai duoc escape (nhan doi dau nhay don) truoc khi ghep.
        /// So thi ghep truc tiep nhung phai dung InvariantCulture, neu khong
        /// tren may dat dau phay thap phan se ra "DiemTB >= 7,5" -> loi cu phap.
        /// </summary>
        public DataView TaoViewSinhVien(DataSet ds, string maKhoa, decimal? diemToiThieu)
        {
            if (ds == null || !ds.Tables.Contains(TenBangSinhVien))
                throw new InvalidOperationException("DataSet chưa có bảng SinhVien.");

            var view = new DataView(ds.Tables[TenBangSinhVien]);

            var dieuKien = new System.Collections.Generic.List<string>();

            if (!string.IsNullOrEmpty(maKhoa))
                dieuKien.Add(string.Format("MaKhoa = '{0}'", EscapeRowFilter(maKhoa)));

            if (diemToiThieu.HasValue)
                dieuKien.Add(string.Format("DiemTB >= {0}",
                    diemToiThieu.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)));

            view.RowFilter = string.Join(" AND ", dieuKien.ToArray());
            view.Sort = "DiemTB DESC, HoTen ASC";

            return view;
        }

        /// <summary>
        /// Escape gia tri chuoi truoc khi ghep vao RowFilter.
        /// Trong bieu thuc DataView, dau nhay don duoc viet lap doi - giong T-SQL.
        /// Khong lam buoc nay thi ho ten "O'Neil" se lam hong ca bieu thuc.
        /// </summary>
        public static string EscapeRowFilter(string giaTri)
        {
            return (giaTri ?? string.Empty).Replace("'", "''");
        }

        /// <summary>Mo ta bieu thuc dang dung de hien thi cho nguoi hoc doi chieu.</summary>
        public static string MoTaRowFilter(string maKhoa, decimal? diemToiThieu)
        {
            var phan = new System.Collections.Generic.List<string>();
            if (!string.IsNullOrEmpty(maKhoa)) phan.Add("MaKhoa = '" + EscapeRowFilter(maKhoa) + "'");
            if (diemToiThieu.HasValue)
                phan.Add("DiemTB >= " + diemToiThieu.Value.ToString(
                    System.Globalization.CultureInfo.InvariantCulture));

            return phan.Count == 0 ? "(không lọc)" : string.Join(" AND ", phan.ToArray());
        }

        // ------------------------------------------------------------------
        // BAI 5 - Cap nhat bang SqlCommandBuilder
        // ------------------------------------------------------------------

        /// <summary>
        /// Tao adapter co du 4 lenh (Select/Insert/Update/Delete) de co the
        /// gui thay doi ve CSDL.
        ///
        /// SqlCommandBuilder doc cau SELECT cua adapter roi TU SINH ra Insert /
        /// Update / Delete. Dieu kien: cau SELECT phai tra ve khoa chinh
        /// (MaSV) - thieu khoa chinh thi builder khong sinh duoc lenh Update
        /// va adapter.Update() se nem InvalidOperationException.
        ///
        /// Luu y: builder chi phu hop bai tap co ban, mot bang, khoa don.
        /// Voi khoa phuc tap hoac JOIN nhieu bang phai viet lenh tay.
        /// </summary>
        public SqlDataAdapter TaoAdapterCapNhat(out SqlConnection conn, out DataTable table)
        {
            conn = DbConnectionFactory.Create();

            var adapter = new SqlDataAdapter(
                "SELECT MaSV, HoTen, NgaySinh, GioiTinh, MaKhoa, DiemTB, Email " +
                "FROM dbo.SinhVien", conn);

            // SqlCommandBuilder sinh lenh theo kieu LAZY: ngay sau khi tao, adapter
            // van chua co Insert/Update/DeleteCommand. Phai goi tuong minh
            // GetInsertCommand() / GetUpdateCommand() / GetDeleteCommand() de
            // builder phan tich cau SELECT va sinh lenh tuong ung.
            var builder = new SqlCommandBuilder(adapter);
            adapter.InsertCommand = builder.GetInsertCommand();
            adapter.UpdateCommand = builder.GetUpdateCommand();
            adapter.DeleteCommand = builder.GetDeleteCommand();

            table = new DataTable(TenBangSinhVien);
            adapter.Fill(table);

            // Sau Fill, moi dong deu la Unchanged -> Update() chua lam gi.
            // Chi khi nguoi dung sua tren luoi thi RowState moi doi.
            table.AcceptChanges();

            return adapter;
        }

        /// <summary>
        /// Gui thay doi cua DataTable ve CSDL.
        ///
        /// Adapter.Update() duyet tung dong theo RowState:
        ///   Added    -> INSERT
        ///   Modified -> UPDATE
        ///   Deleted  -> DELETE
        /// va tra ve TONG so dong bi anh huong.
        /// </summary>
        public KetQuaCapNhat CapNhat(SqlDataAdapter adapter, SqlConnection conn, DataTable table)
        {
            var kq = new KetQuaCapNhat();

            try
            {
                if (adapter == null || table == null)
                    throw new InvalidOperationException("Chưa nạp dữ liệu để cập nhật.");

                if (table.GetChanges() == null)
                {
                    kq.ThongDiep = "Không có thay đổi nào cần gửi (mọi dòng đang Unchanged).";
                    return kq;
                }

                if (conn.State != ConnectionState.Open) conn.Open();

                // Dung transaction: neu mot lenh loi thi huy toan bo, khong de
                // CSDL o trang thai nua voi.
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        adapter.SelectCommand.Transaction = tran;
                        if (adapter.InsertCommand != null) adapter.InsertCommand.Transaction = tran;
                        if (adapter.UpdateCommand != null) adapter.UpdateCommand.Transaction = tran;
                        if (adapter.DeleteCommand != null) adapter.DeleteCommand.Transaction = tran;

                        kq.SoDongAnhHuong = adapter.Update(table);
                        tran.Commit();
                    }
                    catch (Exception)
                    {
                        try { tran.Rollback(); } catch (Exception) { }
                        throw;
                    }
                }

                // Update() KHONG tu goi AcceptChanges. Sau khi ghi thanh cong
                // phai goi de dua cac dong ve Unchanged; neu khong, bam Luu lan
                // hai se gui lai dung nhung thay doi do.
                table.AcceptChanges();

                kq.ThongDiep = "Đã ghi " + kq.SoDongAnhHuong + " thay đổi vào CSDL.";
                return kq;
            }
            catch (Exception ex)
            {
                kq.CoLoi = true;
                kq.ThongDiep = ex.Message;
                return kq;
            }
        }

        // ------------------------------------------------------------------
        // BAI 6 - Tao DataTable thu cong (khong can CSDL)
        // ------------------------------------------------------------------

        /// <summary>
        /// Tao bang diem hoan toan trong bo nho: khong connection, khong SQL.
        ///
        /// Cot XepLoai la cot TINH (Expression): gia tri duoc DataTable tu tinh
        /// lai moi khi Diem thay doi, khong phai gan tay. Bieu thuc cua
        /// DataColumn chi ho tro mot tap ham nho (IIF, ISNULL, LEN, SUBSTRING,
        /// CONVERT...) chu khong phai C#.
        /// </summary>
        public DataTable TaoBangDiemThuCong()
        {
            var table = new DataTable("BangDiem");

            // Cot 1: MaSV - khoa chinh, khong cho trung, khong cho rong
            var cotMa = new DataColumn("MaSV", typeof(string));
            cotMa.MaxLength = 15;
            cotMa.AllowDBNull = false;
            cotMa.Unique = true;
            table.Columns.Add(cotMa);

            // Cot 2: HoTen
            var cotTen = new DataColumn("HoTen", typeof(string));
            cotTen.AllowDBNull = false;
            cotTen.MaxLength = 100;
            table.Columns.Add(cotTen);

            // Cot 3: Diem - co rang buoc mien gia tri nhu CHECK trong SQL
            var cotDiem = new DataColumn("Diem", typeof(decimal));
            cotDiem.AllowDBNull = false;
            cotDiem.DefaultValue = 0m;
            table.Columns.Add(cotDiem);

            // Cot 4: XepLoai - COT TINH, tu cap nhat theo Diem
            var cotXepLoai = new DataColumn("XepLoai", typeof(string));
            cotXepLoai.Expression =
                "IIF(Diem >= 8, 'Giỏi', IIF(Diem >= 6.5, 'Khá', IIF(Diem >= 5, 'Trung bình', 'Yếu')))";
            table.Columns.Add(cotXepLoai);

            table.PrimaryKey = new[] { cotMa };

            // Rang buoc mien gia tri - tuong duong CHECK (Diem BETWEEN 0 AND 10).
            // DataTable KHONG co lop CheckConstraint nhu SQL: cach dung chuan la
            // bat ColumnChanging va nem loi khi gia tri nam ngoai mien hop le.
            table.ColumnChanging += delegate (object s, DataColumnChangeEventArgs e)
            {
                if (e.Column != cotDiem || e.ProposedValue == null || e.ProposedValue == DBNull.Value) return;

                decimal giaTri = Convert.ToDecimal(e.ProposedValue);
                if (giaTri < 0m || giaTri > 10m)
                    throw new ConstraintException(
                        "Điểm phải thuộc [0, 10] (ràng buộc CK_Diem_0_10): giá trị " + giaTri + " không hợp lệ.");
            };

            // Du lieu mau: 5 dong
            table.Rows.Add("SV101", "Nguyễn Văn An", 9.1m);
            table.Rows.Add("SV102", "Trần Thị Bình", 7.4m);
            table.Rows.Add("SV103", "Lê Văn Cường", 5.5m);
            table.Rows.Add("SV104", "Phạm Thị Dung", 4.2m);
            table.Rows.Add("SV105", "Hoàng Văn Em", 6.8m);

            table.AcceptChanges();
            return table;
        }

        // ------------------------------------------------------------------
        // BAI 8 - AcceptChanges / RejectChanges
        // ------------------------------------------------------------------

        /// <summary>Dem so dong theo tung RowState (bai 7).</summary>
        public static ThongKeRowState DemTheoRowState(DataTable table)
        {
            var tk = new ThongKeRowState();
            if (table == null) return tk;

            foreach (DataRow row in table.Rows)
            {
                switch (row.RowState)
                {
                    case DataRowState.Unchanged: tk.KhongDoi++; break;
                    case DataRowState.Added: tk.ThemMoi++; break;
                    case DataRowState.Modified: tk.DaSua++; break;
                    case DataRowState.Deleted: tk.DaXoa++; break;
                }
            }

            return tk;
        }

        /// <summary>
        /// Liet ke cac dong da thay doi kem trang thai, de hien thi truoc khi Update.
        ///
        /// GetChanges() tra ve mot DataTable MOI chi chua cac dong da thay doi
        /// (giu nguyen RowState cua chung) - dung de gui di nhe hon ca bang,
        /// hoac de xem truoc se gui gi.
        /// </summary>
        public static DataTable LayThayDoi(DataTable table)
        {
            if (table == null) return null;
            return table.GetChanges();
        }

        // ------------------------------------------------------------------
        // BAI 9 - Luu / doc DataSet XML
        // ------------------------------------------------------------------

        /// <summary>
        /// Ghi DataSet ra XML KEM SCHEMA.
        ///
        /// WriteXml(path) chi ghi du lieu -> doc lai khong biet kieu cot, khoa
        /// chinh, rang buoc. WriteXml(path, XmlWriteMode.WriteSchema) ghi kem
        /// XSD nen file tu mo ta duoc: doc lai van co dung kieu decimal, van
        /// giu khoa chinh va quan he.
        /// </summary>
        public void LuuDataSetXml(DataSet ds, string duongDan)
        {
            if (ds == null) throw new ArgumentNullException("ds");
            if (string.IsNullOrEmpty(duongDan)) throw new ArgumentException("Đường dẫn trống.");

            ds.WriteXml(duongDan, XmlWriteMode.WriteSchema);
        }

        /// <summary>
        /// Doc lai DataSet tu XML - KHONG can ket noi CSDL.
        /// ReadXml tu doc ca schema neu file co, nen khong can ReadXmlSchema rieng.
        /// </summary>
        public DataSet DocDataSetXml(string duongDan)
        {
            if (!File.Exists(duongDan))
                throw new FileNotFoundException("Không tìm thấy tệp XML: " + duongDan);

            var ds = new DataSet();
            ds.ReadXml(duongDan, XmlReadMode.ReadSchema);
            return ds;
        }

        // ------------------------------------------------------------------
        // BAI 10 - Merge hai DataTable
        // ------------------------------------------------------------------

        /// <summary>
        /// Gop hai bang cung cau truc bang DataTable.Merge.
        ///
        /// Merge so sanh theo KHOA CHINH (PrimaryKey):
        ///   - ma chua co -> them dong moi
        ///   - ma da co  -> cap nhat theo MissingSchemaAction / quy tac hien hanh
        ///
        /// Tham so preserveChanges:
        ///   false (mac dinh) -> uu tien ban DEN (du lieu moi thang)
        ///   true             -> uu tien ban DI (giu gia tri dang co)
        ///
        /// Neu hai bang khong cung khoa, Merge chi cong don dong va co the sinh
        /// dong trung -> phai dat PrimaryKey truoc khi gop.
        /// </summary>
        public static DataTable GopHaiBang(DataTable nguon, DataTable boSung, bool giuBanCu)
        {
            if (nguon == null) throw new ArgumentNullException("nguon");
            if (boSung == null) throw new ArgumentNullException("boSung");

            if (nguon.PrimaryKey == null || nguon.PrimaryKey.Length == 0)
                throw new InvalidOperationException("Bảng nguồn phải có PrimaryKey trước khi Merge.");

            var banSao = nguon.Copy();
            banSao.Merge(boSung, giuBanCu, MissingSchemaAction.Add);
            return banSao;
        }

        /// <summary>
        /// Bang "nguon thu hai" cho bai 10: vai sinh vien moi va vai sinh vien
        /// da co nhung diem khac - de thay ro Merge vua them vua cap nhat.
        /// </summary>
        public static DataTable TaoBangNguonThuHai()
        {
            var table = new DataTable("SinhVien");

            table.Columns.Add("MaSV", typeof(string));
            table.Columns.Add("HoTen", typeof(string));
            table.Columns.Add("DiemTB", typeof(decimal));

            table.Rows.Add("SV001", "Nguyễn Văn An", 9.5m);   // da co -> cap nhat diem
            table.Rows.Add("SV002", "Trần Thị Bình", 8.9m);   // da co -> cap nhat diem
            table.Rows.Add("SV900", "Đỗ Thị Mới", 7.7m);      // moi -> them
            table.Rows.Add("SV901", "Vũ Văn Mới", 6.6m);      // moi -> them

            table.PrimaryKey = new[] { table.Columns["MaSV"] };
            table.AcceptChanges();
            return table;
        }

        /// <summary>Bang rut gon (3 cot) tu CSDL de thu Merge.</summary>
        public DataTable LayBangRutGon()
        {
            var table = new DataTable("SinhVien");

            using (var conn = DbConnectionFactory.Create())
            using (var adapter = new SqlDataAdapter(
                "SELECT MaSV, HoTen, DiemTB FROM dbo.SinhVien ORDER BY MaSV", conn))
            {
                adapter.Fill(table);
            }

            table.PrimaryKey = new[] { table.Columns["MaSV"] };
            table.AcceptChanges();
            return table;
        }
    }
}
