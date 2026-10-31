using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Lab10_DisconnectedData.BUS;
using Lab10_DisconnectedData.GUI.Helpers;

namespace Lab10_DisconnectedData.GUI
{
    /// <summary>
    /// BAI 6 - Tao DataTable thu cong (khong can CSDL).
    ///
    /// Bảng điểm được dựng hoàn toàn trong bộ nhớ: DataColumn + DataRow +
    /// rang buộc, không có connection nào.
    ///
    /// Ba điểm đáng chú ý:
    ///   - Cot XepLoai là cot TINH (DataColumn.Expression): DataTable tự tính
    ///     lại mỗi khi Diem thay đổi, không phải gán tay.
    ///   - Rang buoc CK_Diem_0_10 tương đương CHECK trong SQL: nhập 11 điểm sẽ
    ///     ném lỗi ngay trên client, không cần tới server.
    ///   - PrimaryKey chặn trùng MaSV: thêm mã đã có sẽ ném lỗi.
    /// </summary>
    public class frmBai6 : BaiFormBase
    {
        private readonly SinhVienBUS _bus = new SinhVienBUS();

        private LuoiSinhVien _luoi;
        private Label _lblCotTinh;
        private TextBox _txtMa;
        private TextBox _txtTen;
        private NumericUpDown _numDiem;

        private DataTable _table;

        public frmBai6()
            : base("6", "Tạo DataTable thủ công",
                   "DataColumn + DataRow + Expression + Constraint — hoàn toàn trong bộ nhớ, không kết nối CSDL",
                   UiTheme.Indigo, "DataTable", "không cần CSDL", "frmBai6",
                   new Size(1140, 730))
        {
            Badge = new BadgeLabel();
            Badge.Value = "Offline 100%";
            Badge.BaseColor = UiTheme.Indigo;
            Badge.SetBounds(NoiDung.Right - 165, 20, 145, 24);

            TaoThanhCongCu();
            TaoLuoi();
            TaoFormThem();
            TaoGiaiThich();
        }

        private void TaoThanhCongCu()
        {
            CardPanel the = The(18, 18, 1104, 78);
            NoiDung.Controls.Add(the);

            the.Controls.Add(Nut("btnTao", "Tạo bảng điểm", UiTheme.Indigo, 18, 22, 160));

            _lblCotTinh = UiFactory.Label(
                "Cột XepLoai là cột TÍNH — sửa Điểm ở bảng dưới, Xếp loại tự cập nhật.",
                9.25f, UiTheme.InkSoft, false);
            _lblCotTinh.SetBounds(192, 24, 890, 30);
            the.Controls.Add(_lblCotTinh);

            Tim("btnTao").Click += (s, e) => TaoBang();
        }

        private void TaoLuoi()
        {
            CardPanel the = The(18, 108, 1104, 340);
            the.Padding = new Padding(1);
            NoiDung.Controls.Add(the);

            _luoi = new LuoiSinhVien();
            _luoi.Dock = DockStyle.Fill;
            _luoi.BatToMau = false;
            _luoi.ChoSua = true;
            _luoi.CellValueChanged += (s, e) =>
            {
                // Cot tinh tu cap nhat, nhung luoi can ve lai
                if (e.RowIndex >= 0) _luoi.InvalidateRow(e.RowIndex);
            };
            the.Controls.Add(_luoi);
        }

        private void TaoFormThem()
        {
            CardPanel the = The(18, 460, 1104, 104);
            NoiDung.Controls.Add(the);

            Label t = TieuDeMuc("THÊM DÒNG BẰNG CODE (KIỂM TRA RÀNG BUỘC)", UiTheme.Indigo);
            t.SetBounds(18, 8, 500, 20);
            the.Controls.Add(t);

            Label n1 = NhanTruong("Mã SV"); n1.SetBounds(18, 34, 100, 18); the.Controls.Add(n1);
            _txtMa = TaoO("txtMa", 18, 54, 120);
            _txtMa.Text = "SV106";
            _txtMa.ForeColor = Color.Black;
            the.Controls.Add(_txtMa);

            Label n2 = NhanTruong("Họ tên"); n2.SetBounds(150, 34, 100, 18); the.Controls.Add(n2);
            _txtTen = TaoO("txtTen", 150, 54, 240);
            _txtTen.Text = "Nguyễn Thị Phương";
            _txtTen.ForeColor = Color.Black;
            the.Controls.Add(_txtTen);

            Label n3 = NhanTruong("Điểm"); n3.SetBounds(402, 34, 100, 18); the.Controls.Add(n3);
            _numDiem = new NumericUpDown();
            _numDiem.Name = "numDiem";
            _numDiem.DecimalPlaces = 1;
            _numDiem.Minimum = 0m;
            _numDiem.Maximum = 15m;      // cố ý cho vượt 10 để thử ràng buộc
            _numDiem.Value = 8.5m;
            _numDiem.Font = UiTheme.FontBody(10f);
            _numDiem.SetBounds(402, 56, 90, 26);
            the.Controls.Add(_numDiem);

            the.Controls.Add(Nut("btnThem", "Thêm dòng", UiTheme.Green, 508, 52, 140));
            the.Controls.Add(Nut("btnThuTrung", "Thử thêm mã trùng", UiTheme.Red, 660, 52, 170, 36, true));
            the.Controls.Add(Nut("btnThuVuot", "Thử điểm 11 (vi phạm CHECK)", UiTheme.Red, 840, 52, 230, 36, true));

            Tim("btnThem").Click += (s, e) => ThemDong(false);
            Tim("btnThuTrung").Click += (s, e) => ThemDong(true);
            Tim("btnThuVuot").Click += (s, e) => ThuVuotDiem();
        }

        private void TaoGiaiThich()
        {
            CardPanel the = The(18, 572, 1104, 46);
            NoiDung.Controls.Add(the);

            Label g = UiFactory.Label(
                "cotXepLoai.Expression = \"IIF(Diem >= 8, 'Giỏi', IIF(Diem >= 6.5, 'Khá', IIF(Diem >= 5, 'Trung bình', 'Yếu')))\"  ·  " +
                "table.Constraints.Add(\"CK_Diem_0_10\", cotDiem, \"Diem >= 0 AND Diem <= 10\")",
                8.5f, UiTheme.Muted, false);
            g.Font = UiTheme.FontMono(8f);
            g.SetBounds(18, 12, 1068, 24);
            the.Controls.Add(g);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            TaoBang();
        }

        private void TaoBang()
        {
            try
            {
                _table = _bus.TaoBangDiemThuCong();
                _luoi.GanDuLieu(_table);
                _luoi.ReadOnly = false;

                // Cot tinh khong cho sua tay
                if (_luoi.Columns.Contains("XepLoai")) _luoi.Columns["XepLoai"].ReadOnly = true;

                _lblCotTinh.Text = "Bảng \"" + _table.TableName + "\" có " + _table.Columns.Count +
                    " cột, " + _table.Rows.Count + " dòng, " + _table.Constraints.Count +
                    " ràng buộc, PrimaryKey = " +
                    (_table.PrimaryKey.Length > 0 ? _table.PrimaryKey[0].ColumnName : "(chưa có)"); 

                KetQua("Tạo DataTable trong bộ nhớ với 4 cột (một cột biểu thức) và 2 ràng buộc.", true);
            }
            catch (Exception ex) { LoiChung(ex); }
        }

        private void ThemDong(bool thuTrung)
        {
            try
            {
                if (_table == null) { GhiChu("Hãy tạo bảng trước."); return; }

                string ma = thuTrung ? "SV101" : _txtMa.Text.Trim();
                string ten = thuTrung ? "Bản sao của SV101" : _txtTen.Text.Trim();
                decimal diem = _numDiem.Value;

                DataRow row = _table.NewRow();
                row["MaSV"] = ma;
                row["HoTen"] = ten;
                row["Diem"] = diem;
                _table.Rows.Add(row);

                _luoi.GanDuLieu(_table);
                KetQua("Đã thêm " + ma + " → XếpLoai tự tính = \"" + row["XepLoai"] + "\".", true);
            }
            catch (ConstraintException ex)
            {
                KetQua("Ràng buộc chặn lại: " + ex.Message, false);
            }
            catch (Exception ex) { LoiChung(ex); }
        }

        private void ThuVuotDiem()
        {
            try
            {
                if (_table == null) { GhiChu("Hãy tạo bảng trước."); return; }

                DataRow row = _table.NewRow();
                row["MaSV"] = "SV999";
                row["HoTen"] = "Điểm không hợp lệ";
                _numDiem.Value = 11m;
                row["Diem"] = 11m;
                _table.Rows.Add(row);

                KetQua("Không ngờ: điểm 11 vẫn được thêm — ràng buộc chưa đúng.", false);
            }
            catch (ConstraintException ex)
            {
                KetQua("CHECK trên client đã chặn điểm 11: " + ex.Message, true);
            }
            catch (Exception ex)
            {
                KetQua("Đã chặn: " + ex.Message, true);
            }
            finally
            {
                _numDiem.Value = 8.5m;
            }
        }
    }
}
