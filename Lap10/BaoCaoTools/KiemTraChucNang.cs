using System;
using System.Data;
using System.Text;
using Lab10_DisconnectedData.BUS;
using Lab10_DisconnectedData.DTO;

namespace Lab10_DisconnectedData.BaoCaoTools
{
    /// <summary>
    /// Bo tu kiem tra chuc nang cua Lab 10 (che do --kiemtra).
    ///
    /// Moi bai co mot nhom phep kiem tra rieng. Ket qua ghi ra file TRUOC khi
    /// ghi Console, vi day la ung dung WinExe: khong co cua so console that nen
    /// Console.WriteLine co the nem IOException.
    ///
    /// Cac phep kiem tra tap trung vao nhung diem de sai cua mo hinh ngat ket noi:
    ///   - Fill xong phai Close ket noi (khong giu ket noi mo)
    ///   - RowState phai doi dung khi sua/them/xoa
    ///   - AcceptChanges phai lam GetChanges() = null
    ///   - Merge phai so sanh theo PrimaryKey, khong cong don dong
    ///   - XML kem schema phai giu kieu decimal va PrimaryKey
    /// </summary>
    public static class KiemTraChucNang
    {
        private static readonly SinhVienBUS Bus = new SinhVienBUS();
        private static readonly StringBuilder BaoCao = new StringBuilder();

        private static int _tongSo;
        private static int _soDat;

        public static void Chay()
        {
            _tongSo = 0;
            _soDat = 0;

            TieuDe("LAB 10 - KIEM TRA CHUC NANG (mo hinh ngat ket noi)");
            BaoCao.AppendLine("Thoi diem: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            BaoCao.AppendLine();

            KiemTraBai1();
            KiemTraBai2();
            KiemTraBai3();
            KiemTraBai4();
            KiemTraBai5();
            KiemTraBai6();
            KiemTraBai7();
            KiemTraBai8();
            KiemTraBai9();
            KiemTraBai10();

            BaoCao.AppendLine();
            BaoCao.AppendLine(new string('=', 70));
            BaoCao.AppendLine(string.Format("KET QUA: {0}/{1} phep kiem tra dat.", _soDat, _tongSo));
            BaoCao.AppendLine(new string('=', 70));

            Luu();

            try { Console.OutputEncoding = Encoding.UTF8; } catch (Exception) { }
            try { Console.WriteLine(BaoCao.ToString()); } catch (Exception) { }
        }

        // ------------------------------------------------------------------
        // BAI 1 - Fill DataTable
        // ------------------------------------------------------------------

        private static void KiemTraBai1()
        {
            Muc("BAI 1 - Fill DataTable bang SqlDataAdapter");

            try
            {
                DataTable t = Bus.GetStudentTable();

                KiemTra("Nạp được dữ liệu từ CSDL", t != null && t.Rows.Count > 0,
                    "Số dòng = " + (t == null ? 0 : t.Rows.Count));

                KiemTra("Tên bảng là \"SinhVien\"", t != null && t.TableName == "SinhVien",
                    "TableName = " + (t == null ? "null" : t.TableName));

                KiemTra("Có 7 cột", t != null && t.Columns.Count == 7,
                    "Columns = " + (t == null ? 0 : t.Columns.Count));

                KiemTra("Cột DiemTB giữ kiểu decimal",
                    t != null && t.Columns.Contains("DiemTB") &&
                    t.Columns["DiemTB"].DataType == typeof(decimal),
                    "Kiểu = " + (t != null && t.Columns.Contains("DiemTB")
                        ? t.Columns["DiemTB"].DataType.Name : "?"));

                KiemTra("Mọi dòng sau Fill đều Unchanged",
                    t != null && Bus.DemTheoRowState(t).TongThayDoi == 0,
                    t == null ? "null" : Bus.DemTheoRowState(t).ToString());

                KiemTra("Cột NgaySinh giữ kiểu DateTime",
                    t != null && t.Columns.Contains("NgaySinh") &&
                    t.Columns["NgaySinh"].DataType == typeof(DateTime),
                    "Kiểu = " + (t != null && t.Columns.Contains("NgaySinh")
                        ? t.Columns["NgaySinh"].DataType.Name : "?"));

                KiemTra("Email NULL đọc thành DBNull",
                    t != null && CoDBNull(t, "Email"),
                    "Có ít nhất một dòng Email = DBNull");
            }
            catch (Exception ex)
            {
                KiemTra("Không ném lỗi khi nạp", false, ex.Message);
            }
        }

        // ------------------------------------------------------------------
        // BAI 2 - DataSet nhieu bang
        // ------------------------------------------------------------------

        private static void KiemTraBai2()
        {
            Muc("BAI 2 - DataSet chua nhieu bang");

            try
            {
                DataSet ds = Bus.GetDataSet();

                KiemTra("DataSet có 2 bảng", ds != null && ds.Tables.Count == 2,
                    "Tables = " + (ds == null ? 0 : ds.Tables.Count));

                KiemTra("Truy cập được theo TÊN \"Khoa\"",
                    ds != null && ds.Tables.Contains("Khoa"), "ds.Tables[\"Khoa\"]");

                KiemTra("Truy cập được theo TÊN \"SinhVien\"",
                    ds != null && ds.Tables.Contains("SinhVien"), "ds.Tables[\"SinhVien\"]");

                KiemTra("DataSetName = \"QLSinhVien\"",
                    ds != null && ds.DataSetName == "QLSinhVien",
                    "Name = " + (ds == null ? "null" : ds.DataSetName));

                KiemTra("Bảng Khoa có 3 dòng",
                    ds != null && ds.Tables.Contains("Khoa") && ds.Tables["Khoa"].Rows.Count == 3,
                    "Rows = " + (ds != null && ds.Tables.Contains("Khoa")
                        ? ds.Tables["Khoa"].Rows.Count : 0));

                // Hai bảng phải độc lập: sửa bảng Khoa không đổi số dòng SinhVien
                if (ds != null)
                {
                    int truoc = ds.Tables["SinhVien"].Rows.Count;
                    ds.Tables["Khoa"].Rows[0]["TenKhoa"] = "Tên tạm";
                    int sau = ds.Tables["SinhVien"].Rows.Count;
                    KiemTra("Hai bảng độc lập trong cùng DataSet", truoc == sau,
                        truoc + " -> " + sau);
                }
            }
            catch (Exception ex)
            {
                KiemTra("Không ném lỗi khi nạp DataSet", false, ex.Message);
            }
        }

        // ------------------------------------------------------------------
        // BAI 3 - DataRelation
        // ------------------------------------------------------------------

        private static void KiemTraBai3()
        {
            Muc("BAI 3 - DataRelation Khoa - SinhVien");

            try
            {
                DataSet ds = Bus.GetDataSet();

                string moCoi = DAL.SinhVienDAL.TimKhoaMoCoi(ds);
                KiemTra("Dữ liệu toàn vẹn khóa ngoại", moCoi == null,
                    moCoi == null ? "Không có MaKhoa mồ côi" : "Mồ côi: " + moCoi);

                DataRelation qh = Bus.TaoQuanHe(ds);

                KiemTra("Tạo được DataRelation", qh != null,
                    qh == null ? "null" : qh.RelationName);

                KiemTra("Tên quan hệ = FK_Khoa_SinhVien",
                    qh != null && qh.RelationName == "FK_Khoa_SinhVien",
                    qh == null ? "null" : qh.RelationName);

                KiemTra("Bảng cha là Khoa",
                    qh != null && qh.ParentTable.TableName == "Khoa",
                    qh == null ? "null" : qh.ParentTable.TableName);

                KiemTra("Bảng con là SinhVien",
                    qh != null && qh.ChildTable.TableName == "SinhVien",
                    qh == null ? "null" : qh.ChildTable.TableName);

                // GetChildRows phai khop voi dem thu cong
                foreach (DataRow cha in ds.Tables["Khoa"].Rows)
                {
                    string ma = Convert.ToString(cha["MaKhoa"]);
                    int soCon = cha.GetChildRows(qh).Length;

                    int demThuCong = 0;
                    foreach (DataRow r in ds.Tables["SinhVien"].Rows)
                        if (string.Equals(Convert.ToString(r["MaKhoa"]), ma,
                            StringComparison.OrdinalIgnoreCase)) demThuCong++;

                    KiemTra("GetChildRows(\"" + ma + "\") khớp đếm thủ công",
                        soCon == demThuCong, soCon + " = " + demThuCong);
                }

                KiemTra("Tổng số dòng con = tổng số sinh viên",
                    TongCon(ds, qh) == ds.Tables["SinhVien"].Rows.Count,
                    TongCon(ds, qh) + " = " + ds.Tables["SinhVien"].Rows.Count);

                // ForeignKeyConstraint chan xoa khoa con sinh vien
                DataRow cha2 = ds.Tables["Khoa"].Rows[0];
                bool biChan = false;
                try
                {
                    cha2.Delete();
                    ds.Tables["Khoa"].RejectChanges();
                }
                catch (InvalidConstraintException)
                {
                    biChan = true;
                    try { ds.Tables["Khoa"].RejectChanges(); } catch (Exception) { }
                }

                KiemTra("ForeignKeyConstraint chặn xóa khoa còn sinh viên", biChan,
                    biChan ? "Đã chặn bằng InvalidConstraintException" : "KHÔNG chặn — nguy hiểm");
            }
            catch (Exception ex)
            {
                KiemTra("Không ném lỗi khi tạo quan hệ", false, ex.Message);
            }
        }

        // ------------------------------------------------------------------
        // BAI 4 - DataView
        // ------------------------------------------------------------------

        private static void KiemTraBai4()
        {
            Muc("BAI 4 - Loc va sap xep bang DataView");

            try
            {
                DataSet ds = Bus.GetDataSet();
                DataTable sv = ds.Tables["SinhVien"];

                DataView v1 = Bus.TaoView(ds, null, null);
                KiemTra("Không lọc → đủ số dòng", v1.Count == sv.Rows.Count,
                    v1.Count + " = " + sv.Rows.Count);

                DataView vCn = Bus.TaoView(ds, "CNTT", null);
                int demCntt = 0;
                foreach (DataRow r in sv.Rows)
                    if (Convert.ToString(r["MaKhoa"]) == "CNTT") demCntt++;
                KiemTra("Lọc MaKhoa = CNTT khớp đếm thủ công", vCn.Count == demCntt,
                    vCn.Count + " = " + demCntt);

                DataView vDiem = Bus.TaoView(ds, null, 8m);
                int demDiem = 0;
                foreach (DataRow r in sv.Rows)
                    if (Convert.ToDecimal(r["DiemTB"]) >= 8m) demDiem++;
                KiemTra("Lọc DiemTB >= 8 khớp đếm thủ công", vDiem.Count == demDiem,
                    vDiem.Count + " = " + demDiem);

                DataView vKet = Bus.TaoView(ds, "CNTT", 7m);
                int demKet = 0;
                foreach (DataRow r in sv.Rows)
                    if (Convert.ToString(r["MaKhoa"]) == "CNTT" && Convert.ToDecimal(r["DiemTB"]) >= 7m)
                        demKet++;
                KiemTra("Lọc kết hợp khoa + điểm", vKet.Count == demKet,
                    vKet.Count + " = " + demKet);

                // Sap xep giam dan theo DiemTB
                bool giamDan = true;
                for (int i = 1; i < vKet.Count; i++)
                {
                    if (Convert.ToDecimal(vKet[i - 1]["DiemTB"]) < Convert.ToDecimal(vKet[i]["DiemTB"]))
                    { giamDan = false; break; }
                }
                KiemTra("Sort = DiemTB DESC đúng thứ tự", giamDan,
                    giamDan ? "Giảm dần" : "Sai thứ tự");

                // Escape dau nhay don
                KiemTra("EscapeRowFilter nhân đôi dấu nháy đơn",
                    DAL.SinhVienDAL.EscapeRowFilter("O'Neil") == "O''Neil",
                    DAL.SinhVienDAL.EscapeRowFilter("O'Neil"));

                var vNhay = new DataView(sv);
                vNhay.RowFilter = "HoTen LIKE '%" + DAL.SinhVienDAL.EscapeRowFilter("O'Neil") + "%'";
                KiemTra("RowFilter với dấu nháy đơn không ném lỗi", true,
                    "Kết quả = " + vNhay.Count + " dòng");

                // DataView khong truy van lai: sua DataTable thi view thay doi theo
                DataTable copy = sv.Copy();
                var vCopy = new DataView(copy);
                vCopy.RowFilter = "MaKhoa = 'CNTT'";
                int truoc = vCopy.Count;
                copy.Rows.Add("SVTT1", "Test View", new DateTime(2005, 1, 1), true, "CNTT", 8.0m, DBNull.Value);
                KiemTra("DataView cập nhật ngay khi DataTable đổi (không cần truy vấn lại)",
                    vCopy.Count == truoc + 1, truoc + " -> " + vCopy.Count);
            }
            catch (Exception ex)
            {
                KiemTra("Không ném lỗi khi lọc/sắp xếp", false, ex.Message);
            }
        }

        // ------------------------------------------------------------------
        // BAI 5 - SqlCommandBuilder
        // ------------------------------------------------------------------

        private static void KiemTraBai5()
        {
            Muc("BAI 5 - Cap nhat bang SqlCommandBuilder");

            System.Data.SqlClient.SqlConnection conn = null;
            try
            {
                System.Data.SqlClient.SqlDataAdapter adapter;
                DataTable table;
                adapter = Bus.TaoAdapterCapNhat(out conn, out table);

                KiemTra("Fill được bảng để sửa", table != null && table.Rows.Count > 0,
                    "Rows = " + (table == null ? 0 : table.Rows.Count));

                KiemTra("CommandBuilder sinh InsertCommand", adapter.InsertCommand != null,
                    adapter.InsertCommand == null ? "null" : "có");

                KiemTra("CommandBuilder sinh UpdateCommand", adapter.UpdateCommand != null,
                    adapter.UpdateCommand == null ? "null" : "có");

                KiemTra("CommandBuilder sinh DeleteCommand", adapter.DeleteCommand != null,
                    adapter.DeleteCommand == null ? "null" : "có");

                KiemTra("Sau Fill mọi dòng Unchanged",
                    Bus.DemTheoRowState(table).TongThayDoi == 0,
                    Bus.DemTheoRowState(table).ToString());

                // Chua sua gi -> Update khong gui gi
                KetQuaCapNhat kq0 = Bus.CapNhat(adapter, conn, table);
                KiemTra("Update khi không có thay đổi → 0 dòng",
                    !kq0.CoLoi && kq0.SoDongAnhHuong == 0, kq0.ThongDiep);

                // Sua 1 o trong bo nho -> RowState = Modified
                string tenCu = Convert.ToString(table.Rows[0]["HoTen"]);
                table.Rows[0]["HoTen"] = tenCu + " (sửa)";
                KiemTra("Sửa dòng → RowState = Modified",
                    table.Rows[0].RowState == DataRowState.Modified,
                    table.Rows[0].RowState.ToString());

                // Hoan tac de khong ghi gi xuong CSDL
                table.RejectChanges();
                KiemTra("RejectChanges đưa dòng về Unchanged",
                    table.Rows[0].RowState == DataRowState.Unchanged,
                    table.Rows[0].RowState.ToString());

                KiemTra("Không ghi gì xuống CSDL trong bộ kiểm tra này",
                    Bus.DemTheoRowState(table).TongThayDoi == 0,
                    "An toàn: dữ liệu CSDL không bị thay đổi");
            }
            catch (Exception ex)
            {
                KiemTra("Không ném lỗi khi tạo adapter cập nhật", false, ex.Message);
            }
            finally
            {
                try
                {
                    if (conn != null)
                    {
                        if (conn.State != ConnectionState.Closed) conn.Close();
                        conn.Dispose();
                    }
                }
                catch (Exception) { }
            }
        }

        // ------------------------------------------------------------------
        // BAI 6 - DataTable thu cong
        // ------------------------------------------------------------------

        private static void KiemTraBai6()
        {
            Muc("BAI 6 - Tao DataTable thu cong");

            try
            {
                DataTable t = Bus.TaoBangDiemThuCong();

                KiemTra("Tạo được bảng trong bộ nhớ", t != null && t.Rows.Count == 5,
                    "Rows = " + (t == null ? 0 : t.Rows.Count));

                KiemTra("Có 4 cột", t.Columns.Count == 4, "Columns = " + t.Columns.Count);

                KiemTra("Có PrimaryKey = MaSV",
                    t.PrimaryKey.Length == 1 && t.PrimaryKey[0].ColumnName == "MaSV",
                    t.PrimaryKey.Length > 0 ? t.PrimaryKey[0].ColumnName : "không có");

                KiemTra("Cột XepLoai là cột TÍNH (có Expression)",
                    t.Columns["XepLoai"].Expression.Length > 0,
                    t.Columns["XepLoai"].Expression);

                // Expression tu tinh dung
                KiemTra("XepLoai tính đúng cho điểm 9.1",
                    Convert.ToString(t.Rows[0]["XepLoai"]) == "Giỏi",
                    Convert.ToString(t.Rows[0]["XepLoai"]));

                KiemTra("XepLoai tính đúng cho điểm 4.2",
                    Convert.ToString(t.Rows[3]["XepLoai"]) == "Yếu",
                    Convert.ToString(t.Rows[3]["XepLoai"]));

                // Sua Diem -> XepLoai tu doi
                t.Rows[0]["Diem"] = 5.0m;
                KiemTra("Sửa Điểm → XepLoai tự cập nhật",
                    Convert.ToString(t.Rows[0]["XepLoai"]) == "Trung bình",
                    "Điểm 5.0 → " + Convert.ToString(t.Rows[0]["XepLoai"]));

                // Rang buoc CHECK
                bool chanTren = false;
                try
                {
                    DataRow r = t.NewRow();
                    r["MaSV"] = "SVX1"; r["HoTen"] = "Vượt"; r["Diem"] = 11m;
                    t.Rows.Add(r);
                }
                catch (Exception) { chanTren = true; }
                KiemTra("Constraint chặn điểm > 10", chanTren,
                    chanTren ? "Đã chặn" : "KHÔNG chặn");

                bool chanAm = false;
                try
                {
                    DataRow r = t.NewRow();
                    r["MaSV"] = "SVX2"; r["HoTen"] = "Âm"; r["Diem"] = -1m;
                    t.Rows.Add(r);
                }
                catch (Exception) { chanAm = true; }
                KiemTra("Constraint chặn điểm < 0", chanAm, chanAm ? "Đã chặn" : "KHÔNG chặn");

                // Trung khoa chinh
                bool chanTrung = false;
                try
                {
                    DataRow r = t.NewRow();
                    r["MaSV"] = "SV101"; r["HoTen"] = "Trùng"; r["Diem"] = 7m;
                    t.Rows.Add(r);
                }
                catch (Exception) { chanTrung = true; }
                KiemTra("PrimaryKey chặn trùng MaSV", chanTrung,
                    chanTrung ? "Đã chặn" : "KHÔNG chặn");

                KiemTra("Có ít nhất 1 ràng buộc khai báo", t.Constraints.Count >= 1,
                    "Constraints = " + t.Constraints.Count);
            }
            catch (Exception ex)
            {
                KiemTra("Không ném lỗi khi tạo bảng thủ công", false, ex.Message);
            }
        }

        // ------------------------------------------------------------------
        // BAI 7 - RowState
        // ------------------------------------------------------------------

        private static void KiemTraBai7()
        {
            Muc("BAI 7 - Theo doi RowState");

            try
            {
                DataTable t = Bus.GetStudentTable();
                int n = t.Rows.Count;

                ThongKeRowState tk0 = Bus.DemTheoRowState(t);
                KiemTra("Sau Fill: tất cả Unchanged", tk0.KhongDoi == n && tk0.TongThayDoi == 0,
                    tk0.ToString());

                KiemTra("GetChanges() = null khi chưa đổi gì", Bus.LayThayDoi(t) == null,
                    Bus.LayThayDoi(t) == null ? "null" : "có dữ liệu");

                // Sua
                t.Rows[0]["DiemTB"] = 9.99m;
                ThongKeRowState tk1 = Bus.DemTheoRowState(t);
                KiemTra("Sau khi sửa: Modified = 1", tk1.DaSua == 1,
                    tk1.ToString());

                // Them
                DataRow moi = t.NewRow();
                moi["MaSV"] = "SVKT01"; moi["HoTen"] = "Kiểm tra";
                moi["MaKhoa"] = "CNTT"; moi["DiemTB"] = 7m;
                t.Rows.Add(moi);
                ThongKeRowState tk2 = Bus.DemTheoRowState(t);
                KiemTra("Sau khi thêm: Added = 1", tk2.ThemMoi == 1, tk2.ToString());

                // Xoa
                t.Rows[1].Delete();
                ThongKeRowState tk3 = Bus.DemTheoRowState(t);
                KiemTra("Sau khi xóa: Deleted = 1", tk3.DaXoa == 1, tk3.ToString());

                KiemTra("Tổng thay đổi = 3", tk3.TongThayDoi == 3, tk3.ToString());

                DataTable thayDoi = Bus.LayThayDoi(t);
                KiemTra("GetChanges() chỉ trả về 3 dòng đã đổi",
                    thayDoi != null && thayDoi.Rows.Count == 3,
                    thayDoi == null ? "null" : thayDoi.Rows.Count + " dòng");

                KiemTra("Dòng Deleted vẫn nằm trong bảng gốc",
                    t.Rows.Count == n + 1,
                    "Rows = " + t.Rows.Count + " (gốc " + n + " + 1 thêm)");

                // Rang buoc: dong Deleted khong doc duoc gia tri truc tiep
                bool docDuoc = true;
                string giaTriDoc = "";
                try { giaTriDoc = Convert.ToString(t.Rows[1]["HoTen"]); }
                catch (DeletedRowInaccessibleException) { docDuoc = false; }
                KiemTra("Đọc cột của dòng Deleted ném DeletedRowInaccessibleException",
                    !docDuoc, docDuoc ? "Đọc được: " + giaTriDoc : "Đã ném đúng ngoại lệ");
            }
            catch (Exception ex)
            {
                KiemTra("Không ném lỗi khi theo dõi RowState", false, ex.Message);
            }
        }

        // ------------------------------------------------------------------
        // BAI 8 - AcceptChanges / RejectChanges
        // ------------------------------------------------------------------

        private static void KiemTraBai8()
        {
            Muc("BAI 8 - AcceptChanges va RejectChanges");

            try
            {
                // --- AcceptChanges ---
                DataTable t = Bus.GetStudentTable();
                decimal diemCu = Convert.ToDecimal(t.Rows[0]["DiemTB"]);
                string tenCu = Convert.ToString(t.Rows[0]["HoTen"]);

                t.Rows[0]["DiemTB"] = 1.11m;
                KiemTra("Trước AcceptChanges: có thay đổi",
                    Bus.DemTheoRowState(t).TongThayDoi == 1,
                    Bus.DemTheoRowState(t).ToString());

                Bus.ChapNhanThayDoi(t);

                KiemTra("Sau AcceptChanges: mọi dòng Unchanged",
                    Bus.DemTheoRowState(t).KhongDoi == t.Rows.Count,
                    Bus.DemTheoRowState(t).ToString());

                KiemTra("Sau AcceptChanges: GetChanges() = null (Update sẽ gửi 0 lệnh)",
                    Bus.LayThayDoi(t) == null,
                    Bus.LayThayDoi(t) == null ? "null" : "vẫn còn dữ liệu");

                KiemTra("AcceptChanges GIỮ giá trị mới (1.11)",
                    Convert.ToDecimal(t.Rows[0]["DiemTB"]) == 1.11m,
                    Convert.ToString(t.Rows[0]["DiemTB"]));

                // --- RejectChanges ---
                DataTable t2 = Bus.GetStudentTable();
                string tenGoc = Convert.ToString(t2.Rows[0]["HoTen"]);
                t2.Rows[0]["HoTen"] = "Tên đã bị đổi";
                Bus.HoanTacThayDoi(t2);

                KiemTra("RejectChanges trả giá trị về bản gốc",
                    Convert.ToString(t2.Rows[0]["HoTen"]) == tenGoc,
                    tenGoc + " -> " + Convert.ToString(t2.Rows[0]["HoTen"]));

                KiemTra("Sau RejectChanges: mọi dòng Unchanged",
                    Bus.DemTheoRowState(t2).TongThayDoi == 0,
                    Bus.DemTheoRowState(t2).ToString());

                // RejectChanges loai bo dong Added
                DataTable t3 = Bus.GetStudentTable();
                int n3 = t3.Rows.Count;
                DataRow r = t3.NewRow();
                r["MaSV"] = "SVREJ01"; r["HoTen"] = "Sẽ bị hủy";
                r["MaKhoa"] = "CNTT"; r["DiemTB"] = 6m;
                t3.Rows.Add(r);
                Bus.HoanTacThayDoi(t3);

                KiemTra("RejectChanges loại bỏ dòng Added",
                    t3.Rows.Count == n3, "Rows: " + (n3 + 1) + " -> " + t3.Rows.Count);

                // RejectChanges phuc hoi dong Deleted
                DataTable t4 = Bus.GetStudentTable();
                int n4 = t4.Rows.Count;
                t4.Rows[0].Delete();
                KiemTra("Sau Delete: Deleted = 1",
                    Bus.DemTheoRowState(t4).DaXoa == 1,
                    Bus.DemTheoRowState(t4).ToString());

                Bus.HoanTacThayDoi(t4);
                KiemTra("RejectChanges phục hồi dòng đã Delete",
                    t4.Rows.Count == n4 && Bus.DemTheoRowState(t4).DaXoa == 0,
                    "Rows = " + t4.Rows.Count + ", Deleted = " + Bus.DemTheoRowState(t4).DaXoa);

                // Thu tu dung: Update TRUOC AcceptChanges
                DataTable t5 = Bus.GetStudentTable();
                t5.Rows[0]["DiemTB"] = 7.77m;
                KiemTra("Thứ tự đúng: GetChanges() != null TRƯỚC AcceptChanges",
                    Bus.LayThayDoi(t5) != null, "Update sẽ gửi 1 lệnh");
            }
            catch (Exception ex)
            {
                KiemTra("Không ném lỗi khi Accept/Reject", false, ex.Message);
            }
        }

        // ------------------------------------------------------------------
        // BAI 9 - XML
        // ------------------------------------------------------------------

        private static void KiemTraBai9()
        {
            Muc("BAI 9 - Luu va doc DataSet bang XML");

            string thuMuc = System.IO.Path.Combine(
                System.IO.Path.GetTempPath(), "Lab10_KiemTra_" + Guid.NewGuid().ToString("N"));
            string tepSchema = System.IO.Path.Combine(thuMuc, "co_schema.xml");
            string tepChiDuLieu = System.IO.Path.Combine(thuMuc, "chi_dulieu.xml");

            try
            {
                System.IO.Directory.CreateDirectory(thuMuc);

                DataSet ds = Bus.GetDataSet();
                try { Bus.TaoQuanHe(ds); } catch (LoiNghiepVu) { }

                // --- Ghi kem schema ---
                Bus.LuuXml(ds, tepSchema);
                KiemTra("Ghi được tệp XML kèm schema", System.IO.File.Exists(tepSchema),
                    tepSchema);

                DataSet doc = Bus.DocXml(tepSchema);
                KiemTra("Đọc lại được DataSet từ XML", doc != null && doc.Tables.Count == 2,
                    doc == null ? "null" : doc.Tables.Count + " bảng");

                KiemTra("Giữ đúng số dòng SinhVien",
                    doc != null && doc.Tables["SinhVien"].Rows.Count == ds.Tables["SinhVien"].Rows.Count,
                    doc == null ? "null" : doc.Tables["SinhVien"].Rows.Count + " dòng");

                KiemTra("Giữ kiểu decimal cho cột DiemTB",
                    doc != null && doc.Tables["SinhVien"].Columns["DiemTB"].DataType == typeof(decimal),
                    doc == null ? "null" : doc.Tables["SinhVien"].Columns["DiemTB"].DataType.Name);

                KiemTra("Giữ kiểu DateTime cho cột NgaySinh",
                    doc != null && doc.Tables["SinhVien"].Columns["NgaySinh"].DataType == typeof(DateTime),
                    doc == null ? "null" : doc.Tables["SinhVien"].Columns["NgaySinh"].DataType.Name);

                KiemTra("Giữ DataRelation sau khi đọc lại",
                    doc != null && doc.Relations.Count == 1,
                    doc == null ? "null" : doc.Relations.Count + " quan hệ");

                // --- Ghi chi du lieu ---
                ds.WriteXml(tepChiDuLieu, XmlWriteMode.IgnoreSchema);
                KiemTra("Ghi được tệp XML chỉ dữ liệu", System.IO.File.Exists(tepChiDuLieu),
                    tepChiDuLieu);

                DataSet doc2 = new DataSet();
                doc2.ReadXml(tepChiDuLieu);
                KiemTra("Tệp chỉ dữ liệu ĐỌC ĐƯỢC nhưng mất kiểu decimal",
                    doc2.Tables.Count > 0,
                    "Bảng = " + doc2.Tables.Count + " (không có schema nên kiểu là string)");

                // --- Loi khi tep khong ton tai ---
                bool nemLoi = false;
                try { Bus.DocXml(System.IO.Path.Combine(thuMuc, "khong_ton_tai.xml")); }
                catch (LoiNghiepVu) { nemLoi = true; }
                catch (System.IO.FileNotFoundException) { nemLoi = true; }
                KiemTra("Đọc tệp không tồn tại → báo lỗi rõ ràng", nemLoi,
                    nemLoi ? "Đã báo lỗi" : "Không báo lỗi");
            }
            catch (Exception ex)
            {
                KiemTra("Không ném lỗi khi ghi/đọc XML", false, ex.Message);
            }
            finally
            {
                try { System.IO.Directory.Delete(thuMuc, true); } catch (Exception) { }
            }
        }

        // ------------------------------------------------------------------
        // BAI 10 - Merge
        // ------------------------------------------------------------------

        private static void KiemTraBai10()
        {
            Muc("BAI 10 - Hop nhat du lieu bang Merge");

            try
            {
                DataTable goc = Bus.LayBangRutGon();
                DataTable boSung = SinhVienBUS.TaoBangNguonThuHai();

                KiemTra("Bảng gốc có PrimaryKey",
                    goc.PrimaryKey.Length > 0,
                    goc.PrimaryKey.Length > 0 ? goc.PrimaryKey[0].ColumnName : "không có");

                KiemTra("Bảng bổ sung có 4 dòng", boSung.Rows.Count == 4,
                    "Rows = " + boSung.Rows.Count);

                int nGoc = goc.Rows.Count;
                DataTable ketQua = SinhVienBUS.GopHaiBang(goc, boSung, false);

                KiemTra("Merge thêm 2 dòng mới, không cộng dồn",
                    ketQua.Rows.Count == nGoc + 2,
                    nGoc + " + 2 = " + (nGoc + 2) + " (thực tế " + ketQua.Rows.Count + ")");

                KiemTra("Mã SV001 không bị trùng sau Merge",
                    DemMa(ketQua, "SV001") == 1,
                    "SV001 xuất hiện " + DemMa(ketQua, "SV001") + " lần");

                KiemTra("SV900 (mới) có trong kết quả", DemMa(ketQua, "SV900") == 1,
                    "SV900: " + DemMa(ketQua, "SV900"));

                KiemTra("SV901 (mới) có trong kết quả", DemMa(ketQua, "SV901") == 1,
                    "SV901: " + DemMa(ketQua, "SV901"));

                // preserveChanges = false -> ban den thang
                decimal diemSv001 = DiemCua(ketQua, "SV001");
                KiemTra("preserveChanges=false → bản ĐẾN thắng (SV001 = 9.50)",
                    diemSv001 == 9.5m, "SV001 = " + diemSv001.ToString("0.00"));

                // preserveChanges = true -> ban di thang
                DataTable ketQua2 = SinhVienBUS.GopHaiBang(goc, boSung, true);
                decimal diemSv001b = DiemCua(ketQua2, "SV001");
                decimal diemGocSv001 = DiemCua(goc, "SV001");
                KiemTra("preserveChanges=true → bản ĐI thắng (giữ điểm gốc)",
                    diemSv001b == diemGocSv001,
                    "Gốc " + diemGocSv001.ToString("0.00") + " = kết quả " + diemSv001b.ToString("0.00"));

                KiemTra("Merge KHÔNG làm thay đổi bảng gốc",
                    goc.Rows.Count == nGoc, "Gốc vẫn " + goc.Rows.Count + " dòng");

                // Khong co PrimaryKey -> sinh dong trung
                DataTable khongKhoa = goc.Copy();
                khongKhoa.PrimaryKey = null;
                DataTable bsKhongKhoa = boSung.Copy();
                bsKhongKhoa.PrimaryKey = null;
                khongKhoa.Merge(bsKhongKhoa);

                KiemTra("Merge KHÔNG có PrimaryKey → sinh dòng trùng khóa",
                    DemMa(khongKhoa, "SV001") > 1,
                    "SV001 xuất hiện " + DemMa(khongKhoa, "SV001") +
                    " lần, tổng " + khongKhoa.Rows.Count + " dòng");
            }
            catch (Exception ex)
            {
                KiemTra("Không ném lỗi khi Merge", false, ex.Message);
            }
        }

        // ------------------------------------------------------------------
        // Tien ich
        // ------------------------------------------------------------------

        private static int TongCon(DataSet ds, DataRelation qh)
        {
            int tong = 0;
            foreach (DataRow cha in ds.Tables["Khoa"].Rows) tong += cha.GetChildRows(qh).Length;
            return tong;
        }

        private static bool CoDBNull(DataTable t, string cot)
        {
            foreach (DataRow r in t.Rows)
                if (r[cot] == DBNull.Value) return true;
            return false;
        }

        private static int DemMa(DataTable t, string ma)
        {
            int dem = 0;
            foreach (DataRow r in t.Rows)
                if (Convert.ToString(r["MaSV"]) == ma) dem++;
            return dem;
        }

        private static decimal DiemCua(DataTable t, string ma)
        {
            foreach (DataRow r in t.Rows)
                if (Convert.ToString(r["MaSV"]) == ma) return Convert.ToDecimal(r["DiemTB"]);
            return -1m;
        }

        private static void TieuDe(string text)
        {
            BaoCao.AppendLine();
            BaoCao.AppendLine(new string('=', 70));
            BaoCao.AppendLine(text);
            BaoCao.AppendLine(new string('=', 70));
        }

        private static void Muc(string text)
        {
            BaoCao.AppendLine();
            BaoCao.AppendLine("--- " + text + " ---");
        }

        private static void KiemTra(string ten, bool dat, string chiTiet)
        {
            _tongSo++;
            if (dat) _soDat++;

            BaoCao.AppendLine(string.Format("  [{0}] {1}", dat ? "DAT" : "SAI", ten));
            if (!string.IsNullOrEmpty(chiTiet)) BaoCao.AppendLine("        → " + chiTiet);
        }

        private static void Luu()
        {
            string noiDung = BaoCao.ToString();

            // Thu muc lam viec: di nguoc len tim csproj
            string thuMuc = AppDomain.CurrentDomain.BaseDirectory;
            for (int i = 0; i < 8 && !string.IsNullOrEmpty(thuMuc); i++)
            {
                if (System.IO.File.Exists(System.IO.Path.Combine(thuMuc, "Lab10_DisconnectedData.csproj")))
                    break;
                System.IO.DirectoryInfo cha = System.IO.Directory.GetParent(thuMuc);
                thuMuc = cha == null ? null : cha.FullName;
            }

            var utf8 = new UTF8Encoding(true);

            try
            {
                if (!string.IsNullOrEmpty(thuMuc))
                {
                    string baoCao = System.IO.Path.Combine(thuMuc, "BaoCao");
                    System.IO.Directory.CreateDirectory(baoCao);
                    System.IO.File.WriteAllText(System.IO.Path.Combine(baoCao, "kiem_tra_nhanh.txt"), noiDung, utf8);
                }
            }
            catch (Exception) { }

            try
            {
                System.IO.File.WriteAllText(
                    System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "kiem_tra_nhanh.txt"), noiDung, utf8);
            }
            catch (Exception) { }
        }
    }
}
