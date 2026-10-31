using System;
using System.Data;
using System.Data.SqlClient;
using Lab10_DisconnectedData.DAL;
using Lab10_DisconnectedData.DTO;

namespace Lab10_DisconnectedData.BUS
{
    /// <summary>
    /// TANG BUS CUA LAB 10.
    ///
    /// Vai tro: kiem tra dieu kien nghiep vu TRUOC khi goi DAL, va dich loi
    /// SqlException sang tieng Viet. Form khong bao gio cham vao DAL truc tiep.
    ///
    /// Diem khac Lab 9: o day du lieu da nam trong bo nho, nen phan lon kiem
    /// tra la ve DataTable/DataView chu khong phai ve tham so SQL.
    /// </summary>
    public class SinhVienBUS
    {
        private readonly SinhVienDAL _dal = new SinhVienDAL();

        // ------------------------------------------------------------------
        // Bai 1 - 2
        // ------------------------------------------------------------------

        public DataTable GetStudentTable()
        {
            return XuLy(_dal.GetStudentTable, "nạp bảng SinhVien");
        }

        public DataTable GetKhoaTable()
        {
            return XuLy(_dal.GetKhoaTable, "nạp bảng Khoa");
        }

        public DataSet GetDataSet()
        {
            return XuLy(_dal.GetDataSet, "nạp DataSet");
        }

        // ------------------------------------------------------------------
        // Bai 3
        // ------------------------------------------------------------------

        /// <summary>
        /// Tao quan he Khoa - SinhVien, nhung kiem tra toan ven truoc.
        /// Neu du lieu co MaKhoa mo coi thi bao ro gia tri nao sai, thay vi de
        /// DataRelation nem InvalidConstraintException kho hieu.
        /// </summary>
        public DataRelation TaoQuanHe(DataSet ds)
        {
            if (ds == null) throw new LoiNghiepVu(null, "Chưa có DataSet.");

            string moCoi = SinhVienDAL.TimKhoaMoCoi(ds);
            if (moCoi != null)
                throw new LoiNghiepVu(null,
                    "Không tạo được quan hệ: SinhVien có MaKhoa không tồn tại trong Khoa (" + moCoi + ").");

            return XuLy(() => _dal.TaoQuanHeKhoaSinhVien(ds), "tạo DataRelation");
        }

        /// <summary>
        /// Dung DataRelation de lay danh sach sinh vien cua mot khoa.
        /// So sanh voi cach loc thuong (vong lap/DataView) de thay cung ket qua.
        /// </summary>
        public DataTable LaySinhVienCuaKhoa(DataSet ds, string maKhoa, out int soSanhVoiLoc)
        {
            if (ds == null) throw new LoiNghiepVu(null, "Chưa có DataSet.");
            if (string.IsNullOrEmpty(maKhoa)) throw new LoiNghiepVu(null, "Chưa chọn khoa.");

            DataRelation qh = ds.Relations.Contains(SinhVienDAL.TenQuanHe)
                ? ds.Relations[SinhVienDAL.TenQuanHe]
                : TaoQuanHe(ds);

            DataTable khoa = ds.Tables[SinhVienDAL.TenBangKhoa];
            DataTable sinhVien = ds.Tables[SinhVienDAL.TenBangSinhVien];

            DataRow cha = null;
            foreach (DataRow r in khoa.Rows)
            {
                if (string.Equals(Convert.ToString(r["MaKhoa"]), maKhoa, StringComparison.OrdinalIgnoreCase))
                { cha = r; break; }
            }

            if (cha == null) throw new LoiNghiepVu(null, "Không tìm thấy khoa " + maKhoa + ".");

            DataRow[] con = cha.GetChildRows(qh);

            // Doi chieu: dem bang cach loc thu cong
            int dem = 0;
            foreach (DataRow r in sinhVien.Rows)
            {
                if (string.Equals(Convert.ToString(r["MaKhoa"]), maKhoa, StringComparison.OrdinalIgnoreCase)) dem++;
            }
            soSanhVoiLoc = dem;

            // Sao chep sang bang moi de khong phu thuoc vao DataRow[] cua DataSet
            DataTable ketQua = sinhVien.Clone();
            foreach (DataRow r in con) ketQua.ImportRow(r);
            return ketQua;
        }

        // ------------------------------------------------------------------
        // Bai 4
        // ------------------------------------------------------------------

        public DataView TaoView(DataSet ds, string maKhoa, decimal? diemToiThieu)
        {
            if (diemToiThieu.HasValue && (diemToiThieu.Value < 0m || diemToiThieu.Value > 10m))
                throw new LoiNghiepVu("DiemToiThieu", "Điểm tối thiểu phải thuộc [0, 10].");

            return XuLy(() => _dal.TaoViewSinhVien(ds, maKhoa, diemToiThieu), "tạo DataView");
        }

        public string MoTaRowFilter(string maKhoa, decimal? diemToiThieu)
        {
            return SinhVienDAL.MoTaRowFilter(maKhoa, diemToiThieu);
        }

        /// <summary>Kiem tra DataView va cach loc thu cong ra cung so dong.</summary>
        public int DemBangView(DataSet ds, string maKhoa, decimal? diemToiThieu)
        {
            DataView view = TaoView(ds, maKhoa, diemToiThieu);
            return view.Count;
        }

        // ------------------------------------------------------------------
        // Bai 5 - cap nhat
        // ------------------------------------------------------------------

        public SqlDataAdapter TaoAdapterCapNhat(out SqlConnection conn, out DataTable table)
        {
            SqlConnection c = null;
            DataTable t = null;

            try
            {
                SqlDataAdapter adapter = _dal.TaoAdapterCapNhat(out c, out t);
                conn = c;
                table = t;
                return adapter;
            }
            catch (SqlException ex)
            {
                if (c != null) try { c.Dispose(); } catch (Exception) { }
                throw new LoiNghiepVu(null, MoTaLoiSql(ex));
            }
        }

        public KetQuaCapNhat CapNhat(SqlDataAdapter adapter, SqlConnection conn, DataTable table)
        {
            return _dal.CapNhat(adapter, conn, table);
        }

        // ------------------------------------------------------------------
        // Bai 6 - 10
        // ------------------------------------------------------------------

        public DataTable TaoBangDiemThuCong()
        {
            return XuLy(_dal.TaoBangDiemThuCong, "tạo DataTable thủ công");
        }

        public ThongKeRowState DemTheoRowState(DataTable table)
        {
            return SinhVienDAL.DemTheoRowState(table);
        }

        public DataTable LayThayDoi(DataTable table)
        {
            return SinhVienDAL.LayThayDoi(table);
        }

        /// <summary>
        /// AcceptChanges: chot cac thay doi trong bo nho, dua moi dong ve Unchanged.
        ///
        /// HAU QUA RAT QUAN TRONG: sau AcceptChanges, GetChanges() tra ve null
        /// nen DataAdapter.Update() se KHONG gui gi xuong CSDL nua. Vay neu bam
        /// "Chấp nhận" truoc khi bấm "Lưu" thi thay doi bi mat (chi mat o phia
        /// client, CSDL khong doi). Bai 8 dung chinh tinh huong nay de giang.
        /// </summary>
        public void ChapNhanThayDoi(DataTable table)
        {
            if (table == null) throw new LoiNghiepVu(null, "Chưa có bảng dữ liệu.");
            table.AcceptChanges();
        }

        /// <summary>RejectChanges: huy moi thay doi, quay ve ban da chot truoc do.</summary>
        public void HoanTacThayDoi(DataTable table)
        {
            if (table == null) throw new LoiNghiepVu(null, "Chưa có bảng dữ liệu.");
            table.RejectChanges();
        }

        public void LuuXml(DataSet ds, string duongDan)
        {
            if (ds == null) throw new LoiNghiepVu(null, "Chưa có DataSet để lưu.");
            if (string.IsNullOrEmpty(duongDan)) throw new LoiNghiepVu(null, "Chưa chọn đường dẫn lưu.");

            XuLy(() => { _dal.LuuDataSetXml(ds, duongDan); return 0; }, "lưu DataSet ra XML");
        }

        public DataSet DocXml(string duongDan)
        {
            if (string.IsNullOrEmpty(duongDan)) throw new LoiNghiepVu(null, "Chưa chọn tệp XML.");
            return XuLy(() => _dal.DocDataSetXml(duongDan), "đọc DataSet từ XML");
        }

        public static DataTable GopHaiBang(DataTable nguon, DataTable boSung, bool giuBanCu)
        {
            return SinhVienDAL.GopHaiBang(nguon, boSung, giuBanCu);
        }

        public static DataTable TaoBangNguonThuHai()
        {
            return SinhVienDAL.TaoBangNguonThuHai();
        }

        public DataTable LayBangRutGon()
        {
            return XuLy(_dal.LayBangRutGon, "nạp bảng rút gọn");
        }

        // ------------------------------------------------------------------
        // Tien ich
        // ------------------------------------------------------------------

        /// <summary>Boc mot thao tac DAL: doi SqlException thanh LoiNghiepVu de GUI hien thi gon.</summary>
        private static T XuLy<T>(Func<T> thaoTac, string tenThaoTac)
        {
            try
            {
                return thaoTac();
            }
            catch (LoiNghiepVu)
            {
                throw;
            }
            catch (SqlException ex)
            {
                throw new LoiNghiepVu(null, "Không thể " + tenThaoTac + ": " + MoTaLoiSql(ex));
            }
            catch (InvalidOperationException ex)
            {
                throw new LoiNghiepVu(null, "Không thể " + tenThaoTac + ": " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new LoiNghiepVu(null, "Lỗi khi " + tenThaoTac + ": " + ex.Message);
            }
        }

        /// <summary>Dich mot so ma loi SQL Server thuong gap sang thong bao de hieu.</summary>
        public static string MoTaLoiSql(SqlException ex)
        {
            switch (ex.Number)
            {
                case -2: return "Hết thời gian chờ kết nối (timeout). Kiểm tra SQL Server đang chạy.";
                case 53: return "Không kết nối được máy chủ. Kiểm tra Data Source trong App.config.";
                case -1: return "Không mở được kết nối tới CSDL.";
                case 4060: return "Không mở được CSDL. Kiểm tra tên CSDL QLSinhVien.";
                case 18456: return "Đăng nhập thất bại. Kiểm tra User ID / Password.";
                case 208: return "Không tìm thấy bảng. Hãy chạy script CSDL ở Lab 7.";
                case 547: return "Vi phạm khóa ngoại: dữ liệu này đang được bảng khác tham chiếu.";
                case 2627: return "Trùng khóa chính: mã này đã tồn tại trong CSDL.";
                case 515: return "Cột bắt buộc đang bị bỏ trống (NULL).";
                default: return "Lỗi SQL " + ex.Number + ": " + ex.Message;
            }
        }
    }
}
