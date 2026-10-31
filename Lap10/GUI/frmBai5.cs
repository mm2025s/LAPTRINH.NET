using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Lab10_DisconnectedData.BUS;
using Lab10_DisconnectedData.DTO;
using Lab10_DisconnectedData.GUI.Helpers;

namespace Lab10_DisconnectedData.GUI
{
    /// <summary>
    /// BAI 5 - Cap nhat du lieu qua SqlCommandBuilder.
    ///
    /// Luong chay:
    ///   1. Adapter + CommandBuilder -> Fill(table)
    ///   2. Nguoi dung sua truc tiep tren DataGridView -> DataRow doi RowState
    ///   3. adapter.Update(table) -> sinh lenh INSERT/UPDATE/DELETE tuong ung
    ///
    /// Ba dieu de bai nay chay dung:
    ///   - Cau SELECT phai tra ve KHOA CHINH (MaSV). Thieu khoa chinh thi
    ///     CommandBuilder khong sinh duoc lenh UPDATE.
    ///   - Phai goi EndEdit() truoc Update, neu khong dong dang sua tren luoi
    ///     chua duoc ghi vao DataRow va se khong duoc gui di.
    ///   - Update() KHONG tu AcceptChanges: phai tu goi, neu khong bam Luu hai
    ///     lan se gui lai dung nhung thay doi do (va co the loi trung khoa).
    ///
    /// Form dung transaction o tang DAL: mot dong loi thi huy toan bo, khong de
    /// CSDL o trang thai nua voi.
    /// </summary>
    public class frmBai5 : BaiFormBase
    {
        private readonly SinhVienBUS _bus = new SinhVienBUS();

        private LuoiSinhVien _luoi;
        private SqlDataAdapter _adapter;
        private SqlConnection _conn;
        private DataTable _table;
        private Label _lblTrangThai;
        private Label _lblHuongDan;

        public frmBai5()
            : base("5", "Cập nhật bằng SqlCommandBuilder",
                   "Sửa trực tiếp trên lưới → adapter.Update(table) → INSERT / UPDATE / DELETE",
                   UiTheme.Green, "SqlCommandBuilder", "adapter.Update()", "frmBai5",
                   new Size(1150, 740))
        {
            Badge = new BadgeLabel();
            Badge.Value = "Đọc & ghi CSDL";
            Badge.BaseColor = UiTheme.Green;
            Badge.SetBounds(NoiDung.Right - 175, 20, 155, 24);

            TaoThanhCongCu();
            TaoLuoi();
            TaoThongTin();
        }

        private void TaoThanhCongCu()
        {
            CardPanel the = The(18, 18, 1114, 96);
            NoiDung.Controls.Add(the);

            the.Controls.Add(Nut("btnNap", "Nạp dữ liệu", UiTheme.Green, 18, 30, 140));
            the.Controls.Add(Nut("btnLuu", "Lưu thay đổi", UiTheme.Blue, 170, 30, 150));
            the.Controls.Add(Nut("btnHuyThayDoi", "Hoàn tác (RejectChanges)", UiTheme.Red, 332, 30, 200, 36, true));

            _lblHuongDan = UiFactory.Label(
                "Sửa ô bất kỳ · thêm dòng bằng dòng trống cuối lưới · chọn dòng rồi nhấn Delete để xóa.",
                9f, UiTheme.InkSoft, false);
            _lblHuongDan.SetBounds(548, 30, 550, 40);
            the.Controls.Add(_lblHuongDan);

            Tim("btnNap").Click += (s, e) => Nap();
            Tim("btnLuu").Click += (s, e) => Luu();
            Tim("btnHuyThayDoi").Click += (s, e) => HoanTac();
        }

        private void TaoLuoi()
        {
            CardPanel the = The(18, 126, 1114, 500);
            the.Padding = new Padding(1);
            NoiDung.Controls.Add(the);

            _luoi = new LuoiSinhVien();
            _luoi.Dock = DockStyle.Fill;
            _luoi.ChoSua = true;          // cho phép sửa/thêm/xóa trực tiếp
            _luoi.BatToMau = false;
            _luoi.CellValueChanged += (s, e) => CapNhatTrangThai();
            _luoi.UserDeletedRow += (s, e) => CapNhatTrangThai();
            the.Controls.Add(_luoi);
        }

        private void TaoThongTin()
        {
            CardPanel the = The(18, 638, 1114, 42);
            NoiDung.Controls.Add(the);

            _lblTrangThai = UiFactory.Label("Chưa nạp dữ liệu.", 9.5f, UiTheme.InkSoft, true);
            _lblTrangThai.SetBounds(18, 10, 1078, 24);
            the.Controls.Add(_lblTrangThai);
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

                DongKetNoiCu();

                _adapter = _bus.TaoAdapterCapNhat(out _conn, out _table);
                _luoi.GanDuLieu(_table);
                CapNhatTrangThai();

                KetQua("Đã nạp " + _table.Rows.Count +
                    " sinh viên. CommandBuilder đã sinh sẵn Insert/Update/Delete từ câu SELECT.", true);
            }
            catch (LoiNghiepVu ex) { HienThiLoiNghiepVu(ex); }
            catch (Exception ex) { LoiChung(ex); }
            finally { Cursor = Cursors.Default; }
        }

        private void Luu()
        {
            try
            {
                if (_adapter == null || _table == null) { GhiChu("Hãy nạp dữ liệu trước."); return; }

                // Bat buoc: chot o dang sua tren luoi vao DataRow
                _luoi.EndEdit();

                // Van con o dang mo thi Update se bo qua gia tri vua go
                if (_luoi.IsCurrentCellInEditMode)
                {
                    GhiChu("Đang sửa một ô — nhấn Enter hoặc bấm ra ngoài rồi lưu lại.");
                    return;
                }

                Cursor = Cursors.WaitCursor;
                KetQuaCapNhat kq = _bus.CapNhat(_adapter, _conn, _table);
                Cursor = Cursors.Default;

                if (kq.CoLoi)
                {
                    KetQua("Không lưu được: " + kq.ThongDiep, false);
                    CapNhatTrangThai();
                    return;
                }

                CapNhatTrangThai();
                KetQua(kq.ThongDiep + "  (sau đó AcceptChanges → mọi dòng về Unchanged)", true);
            }
            catch (LoiNghiepVu ex) { HienThiLoiNghiepVu(ex); }
            catch (Exception ex) { LoiChung(ex); }
            finally { Cursor = Cursors.Default; }
        }

        /// <summary>
        /// RejectChanges: huy moi thay doi CHUA GUI, dua bang ve ban da chot.
        /// Luu y: chi co tac dung voi thay doi trong bo nho; cai gi da Update
        /// xuong CSDL roi thi RejectChanges khong lay lai duoc.
        /// </summary>
        private void HoanTac()
        {
            try
            {
                if (_table == null) { GhiChu("Chưa nạp dữ liệu."); return; }

                ThongKeRowState truoc = _bus.DemTheoRowState(_table);
                _bus.HoanTacThayDoi(_table);
                _luoi.Refresh();
                CapNhatTrangThai();

                KetQua("RejectChanges: đã hủy " + truoc.TongThayDoi +
                    " thay đổi chưa gửi. Lưu ý: dữ liệu đã Update xuống CSDL thì không hoàn tác được.", true);
            }
            catch (LoiNghiepVu ex) { HienThiLoiNghiepVu(ex); }
            catch (Exception ex) { LoiChung(ex); }
        }

        private void CapNhatTrangThai()
        {
            if (_table == null) { _lblTrangThai.Text = "Chưa nạp dữ liệu."; return; }

            ThongKeRowState tk = _bus.DemTheoRowState(_table);
            _lblTrangThai.Text = tk.ToString();

            _lblTrangThai.ForeColor = tk.TongThayDoi > 0 ? UiTheme.Orange : UiTheme.Green;
        }

        private void DongKetNoiCu()
        {
            try
            {
                if (_conn != null)
                {
                    if (_conn.State != ConnectionState.Closed) _conn.Close();
                    _conn.Dispose();
                    _conn = null;
                }
                if (_adapter != null)
                {
                    _adapter.Dispose();
                    _adapter = null;
                }
            }
            catch (Exception) { }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            DongKetNoiCu();
            base.OnFormClosed(e);
        }
    }
}
