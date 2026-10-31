using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Lab10_DisconnectedData.BUS;
using Lab10_DisconnectedData.DTO;
using Lab10_DisconnectedData.GUI.Helpers;

namespace Lab10_DisconnectedData.GUI
{
    /// <summary>
    /// BAI 7 - Theo doi RowState truoc khi goi Update.
    ///
    /// DataTable khong giu "danh sach dong da sua": MOI DataRow tu mang RowState
    /// (Unchanged / Added / Modified / Deleted). Muon biet adapter.Update se gui
    /// bao nhieu lenh, phai QUET RowState chu khong dem so dong.
    ///
    /// GetChanges() tra ve mot DataTable MOI chi gom cac dong da thay doi, giu
    /// nguyen RowState - dung de xem truoc se gui gi, hoac gui di cho nhe.
    ///
    /// Form nay khong ghi xuong CSDL: chi quan sat trang thai. Nho vay an toan
    /// khi thuc hanh tren CSDL that.
    /// </summary>
    public class frmBai7 : BaiFormBase
    {
        private readonly SinhVienBUS _bus = new SinhVienBUS();

        private LuoiSinhVien _luoiGoc;
        private Label _lblTongHop;
        private Label _lblGetChanges;
        private DataGridView _luoiTheoDoi;

        private DataTable _table;

        public frmBai7()
            : base("7", "Theo dõi RowState trước khi Update",
                   "Mỗi DataRow tự mang RowState · GetChanges() lọc ra đúng những dòng sẽ được gửi",
                   UiTheme.Orange, "RowState", "GetChanges()", "frmBai7",
                   new Size(1150, 760))
        {
            Badge = new BadgeLabel();
            Badge.Value = "Chỉ quan sát";
            Badge.BaseColor = UiTheme.Orange;
            Badge.SetBounds(NoiDung.Right - 165, 20, 145, 24);

            TaoThanhCongCu();
            TaoLuoiChinh();
            TaoBangTheoDoi();
            TaoVungGetChanges();
        }

        private void TaoThanhCongCu()
        {
            CardPanel the = The(18, 18, 1114, 74);
            NoiDung.Controls.Add(the);

            the.Controls.Add(Nut("btnNap", "Nạp bảng", UiTheme.Orange, 18, 20, 130));
            the.Controls.Add(Nut("btnSuaDiem", "Sửa điểm 3 dòng đầu", UiTheme.Amber, 160, 20, 190, 36, true));
            the.Controls.Add(Nut("btnThem", "Thêm 1 dòng", UiTheme.Green, 362, 20, 140, 36, true));
            the.Controls.Add(Nut("btnXoa", "Xóa dòng đang chọn", UiTheme.Red, 514, 20, 180, 36, true));
            the.Controls.Add(Nut("btnLamMoi", "Làm mới thống kê", UiTheme.Teal, 706, 20, 160, 36, true));

            Tim("btnNap").Click += (s, e) => Nap();
            Tim("btnSuaDiem").Click += (s, e) => SuaDiem();
            Tim("btnThem").Click += (s, e) => ThemDong();
            Tim("btnXoa").Click += (s, e) => XoaDong();
            Tim("btnLamMoi").Click += (s, e) => CapNhat();
        }

        private void TaoLuoiChinh()
        {
            Label n = TieuDeMuc("BẢNG DỮ LIỆU (SỬA TRỰC TIẾP ĐỂ THẤY ROWSTATE ĐỔI)", UiTheme.Orange);
            n.SetBounds(18, 100, 600, 22);
            NoiDung.Controls.Add(n);

            CardPanel the = The(18, 124, 1114, 300);
            the.Padding = new Padding(1);
            NoiDung.Controls.Add(the);

            _luoiGoc = new LuoiSinhVien();
            _luoiGoc.Dock = DockStyle.Fill;
            _luoiGoc.ChoSua = true;
            _luoiGoc.BatToMau = false;
            _luoiGoc.CellValueChanged += (s, e) => CapNhat();
            _luoiGoc.RowsRemoved += (s, e) => CapNhat();
            the.Controls.Add(_luoiGoc);
        }

        private void TaoBangTheoDoi()
        {
            Label n = TieuDeMuc("THỐNG KÊ THEO ROWSTATE", UiTheme.Orange);
            n.SetBounds(18, 438, 400, 22);
            NoiDung.Controls.Add(n);

            CardPanel the = The(18, 462, 1114, 110);
            NoiDung.Controls.Add(the);

            _lblTongHop = UiFactory.Label("—", 11f, UiTheme.Orange, true);
            _lblTongHop.SetBounds(18, 14, 1078, 26);
            the.Controls.Add(_lblTongHop);

            // Bang nho 4 dong: trang thai / so dong / lenh SQL tuong ung
            _luoiTheoDoi = new DataGridView();
            _luoiTheoDoi.Name = "luoiTheoDoi";
            _luoiTheoDoi.ReadOnly = true;
            _luoiTheoDoi.AllowUserToAddRows = false;
            _luoiTheoDoi.AllowUserToDeleteRows = false;
            _luoiTheoDoi.RowHeadersVisible = false;
            _luoiTheoDoi.ColumnHeadersVisible = true;
            _luoiTheoDoi.BorderStyle = BorderStyle.None;
            _luoiTheoDoi.BackgroundColor = Color.White;
            _luoiTheoDoi.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _luoiTheoDoi.ColumnHeadersHeight = 28;
            _luoiTheoDoi.RowTemplate.Height = 24;
            _luoiTheoDoi.Font = UiTheme.FontBody(9f);
            _luoiTheoDoi.EnableHeadersVisualStyles = false;
            _luoiTheoDoi.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(246, 247, 251);
            _luoiTheoDoi.ColumnHeadersDefaultCellStyle.Font = UiTheme.FontBold(8.5f);
            _luoiTheoDoi.ColumnHeadersDefaultCellStyle.ForeColor = UiTheme.InkSoft;
            _luoiTheoDoi.GridColor = Color.FromArgb(236, 239, 245);
            _luoiTheoDoi.SetBounds(18, 44, 1078, 58);
            the.Controls.Add(_luoiTheoDoi);
        }

        private void TaoVungGetChanges()
        {
            CardPanel the = The(18, 584, 1114, 112);
            NoiDung.Controls.Add(the);

            Label n = TieuDeMuc("KẾT QUẢ GetChanges()", UiTheme.Orange);
            n.SetBounds(18, 10, 400, 22);
            the.Controls.Add(n);

            _lblGetChanges = UiFactory.Label("—", 9.25f, UiTheme.InkSoft, false);
            _lblGetChanges.SetBounds(18, 36, 1078, 24);
            the.Controls.Add(_lblGetChanges);

            Label g = UiFactory.Label(
                "DataTable.GetChanges() trả về bảng mới chỉ chứa dòng Added/Modified/Deleted. " +
                "Nếu null → adapter.Update() sẽ không gửi lệnh nào. Đây là lý do phải gọi CapNhat " +
                "TRƯỚC AcceptChanges.",
                8.25f, UiTheme.Muted, false);
            g.SetBounds(18, 62, 1078, 42);
            the.Controls.Add(g);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Nap();
        }

        private void Nap()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                _table = _bus.GetStudentTable();
                _luoiGoc.GanDuLieu(_table);
                CapNhat();

                KetQua("Đã nạp " + _table.Rows.Count +
                    " dòng. Tất cả đang Unchanged vì vừa Fill xong.", true);
            }
            catch (LoiNghiepVu ex) { HienThiLoiNghiepVu(ex); }
            catch (Exception ex) { LoiChung(ex); }
            finally { Cursor = Cursors.Default; }
        }

        private void SuaDiem()
        {
            try
            {
                if (_table == null) { GhiChu("Hãy nạp bảng trước."); return; }

                int sua = 0;
                for (int i = 0; i < _table.Rows.Count && sua < 3; i++)
                {
                    DataRow row = _table.Rows[i];
                    if (row.RowState == DataRowState.Deleted) continue;

                    decimal diem = Convert.ToDecimal(row["DiemTB"]);
                    row["DiemTB"] = Math.Min(10m, diem + 0.5m);
                    sua++;
                }

                _luoiGoc.Refresh();
                CapNhat();
                KetQua("Đã cộng 0.5 điểm cho " + sua + " dòng đầu → RowState chuyển Modified.", true);
            }
            catch (Exception ex) { LoiChung(ex); }
        }

        private void ThemDong()
        {
            try
            {
                if (_table == null) { GhiChu("Hãy nạp bảng trước."); return; }

                string ma = "SVTMP" + (_table.Rows.Count + 1).ToString("000");

                DataRow row = _table.NewRow();
                row["MaSV"] = ma;
                row["HoTen"] = "Sinh viên tạm (chưa lưu)";
                row["NgaySinh"] = new DateTime(2005, 1, 1);
                row["GioiTinh"] = true;
                row["MaKhoa"] = "CNTT";
                row["DiemTB"] = 7.5m;
                _table.Rows.Add(row);

                _luoiGoc.GanDuLieu(_table);
                ChuThichTrangThai();

                CapNhat();
                KetQua("Đã thêm dòng " + ma + " (chỉ trong bộ nhớ) → RowState = Added.", true);
            }
            catch (Exception ex) { LoiChung(ex); }
        }

        private void XoaDong()
        {
            try
            {
                if (_table == null || _luoiGoc.CurrentRow == null)
                {
                    GhiChu("Hãy chọn một dòng trên lưới để xóa.");
                    return;
                }

                DataRowView drv = _luoiGoc.CurrentRow.DataBoundItem as DataRowView;
                if (drv == null) { GhiChu("Không xác định được dòng."); return; }

                string ma = Convert.ToString(drv.Row["MaSV"]);
                drv.Row.Delete();          // Delete() -> RowState = Deleted (chưa mất khỏi bảng)

                CapNhat();
                KetQua("Đã đánh dấu xóa " + ma + " → RowState = Deleted. " +
                    "Dòng vẫn nằm trong bảng với RowState Deleted cho tới khi AcceptChanges.", true);
            }
            catch (Exception ex) { LoiChung(ex); }
        }

        private void CapNhat()
        {
            try
            {
                if (_table == null) return;

                ThongKeRowState tk = _bus.DemTheoRowState(_table);
                _lblTongHop.Text = tk.ToString();
                _lblTongHop.ForeColor = tk.TongThayDoi > 0 ? UiTheme.Orange : UiTheme.Green;

                // Bang dien giai: 4 trang thai + lenh SQL tuong ung
                var bang = new DataTable("RowState");
                bang.Columns.Add("Trạng thái", typeof(string));
                bang.Columns.Add("Số dòng", typeof(int));
                bang.Columns.Add("Lệnh sẽ gửi", typeof(string));

                bang.Rows.Add("Unchanged", tk.KhongDoi, "không gửi gì");
                bang.Rows.Add("Added", tk.ThemMoi, "INSERT");
                bang.Rows.Add("Modified", tk.DaSua, "UPDATE");
                bang.Rows.Add("Deleted", tk.DaXoa, "DELETE");

                _luoiTheoDoi.DataSource = bang;

                DataTable thayDoi = _bus.LayThayDoi(_table);
                _lblGetChanges.Text = thayDoi == null
                    ? "GetChanges() = null → chưa có thay đổi nào, Update() sẽ không làm gì."
                    : "GetChanges() = " + thayDoi.Rows.Count + " dòng (" + MoTaTrangThai(thayDoi) + ")";
            }
            catch (Exception) { }
        }

        private string MoTaTrangThai(DataTable t)
        {
            ThongKeRowState tk = _bus.DemTheoRowState(t);
            return "Added " + tk.ThemMoi + " · Modified " + tk.DaSua + " · Deleted " + tk.DaXoa;
        }

        /// <summary>Ghi lai vao dong trang thai khi vua them dong de nguoi hoc thay ro.</summary>
        private void ChuThichTrangThai()
        {
            if (_table != null && _table.Rows.Count > 0)
            {
                DataRow cuoi = _table.Rows[_table.Rows.Count - 1];
                GhiChu("Dòng cuối: RowState = " + cuoi.RowState + " (cột RowState không có trong CSDL).");
            }
        }
    }
}
