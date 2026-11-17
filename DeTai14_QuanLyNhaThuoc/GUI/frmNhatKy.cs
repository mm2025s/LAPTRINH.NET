using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyNhaThuoc.BUS;
using QuanLyNhaThuoc.DTO;
using QuanLyNhaThuoc.GUI.Helpers;

namespace QuanLyNhaThuoc.GUI
{
    /// <summary>
    /// Trang quan tri: nhat ky nguoi dung + hieu nang nghiep vu.
    ///   - Truy van CSDL: sp_NhatKy_Search theo khoang ngay va tu khoa.
    ///   - Truy van trong form: o "Lọc nhanh" loc ngay tren DataView (RowFilter),
    ///     khong goi lai CSDL; chi so tren cung tinh bang DataTable.Compute.
    /// </summary>
    public class frmNhatKy : ManHinh, ILamMoi
    {
        private readonly NhatKyBUS _bus = new NhatKyBUS();
        private readonly DateTimePicker _tu = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 120 };
        private readonly DateTimePicker _den = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 120 };
        private readonly TextBox _tuKhoa = Ui.O("TuKhoa", 150), _locNhanh = Ui.O("LocNhanh", 150);
        private readonly CheckBox _chiLoi = new CheckBox { Text = "Chỉ thất bại", AutoSize = true, Margin = new Padding(8, 8, 4, 0) };
        private readonly DataGridView _luoi = Ui.Luoi("luoiNhatKy"), _luoiNV = Ui.Luoi("luoiHieuNangNV"), _luoiTT = Ui.Luoi("luoiHieuNangTT");
        private readonly Label _tomTat = Ui.Nhan("", true);
        private DataTable _nhatKy;

        public frmNhatKy() : base("Nhật ký · Hiệu năng", "Ai đã làm gì, khi nào, thành công hay thất bại · hiệu năng bán hàng của nhân viên và thời gian xử lý từng nghiệp vụ", UiTheme.Primary, new Size(1240, 760))
        {
            _tu.Value = DateTime.Today.AddDays(-6);

            var duoi = new TableLayoutPanel { Dock = DockStyle.Bottom, Height = 250, ColumnCount = 2, Padding = new Padding(0, 8, 0, 0) };
            duoi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            duoi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            duoi.Controls.Add(Khoi("Hiệu năng bán hàng theo nhân viên", _luoiNV), 0, 0);
            duoi.Controls.Add(Khoi("Thời gian xử lý theo nghiệp vụ (từ nhật ký)", _luoiTT), 1, 0);

            _locNhanh.TextChanged += (s, e) => Loc();
            _chiLoi.CheckedChanged += (s, e) => Loc();
            _tuKhoa.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) LamMoi(); };
            _luoi.CellFormatting += (s, e) =>
            {
                if (e.RowIndex >= 0 && _luoi.Columns[e.ColumnIndex].DataPropertyName == "KetQua" && Convert.ToString(e.Value) == "Thất bại")
                    e.CellStyle.ForeColor = UiTheme.Accent;
            };

            NoiDung.Controls.Add(_luoi);
            NoiDung.Controls.Add(duoi);
            NoiDung.Controls.Add(Ui.ThanhNut(Ui.Nhan("Lọc nhanh"), _locNhanh, _chiLoi, _tomTat));
            NoiDung.Controls.Add(Ui.ThanhNut(Ui.Nhan("Từ"), _tu, Ui.Nhan("đến"), _den, Ui.Nhan("Từ khóa"), _tuKhoa,
                Ui.Nut("btnXem", "Truy vấn", UiTheme.Primary, (s, e) => LamMoi())));
            Load += (s, e) => LamMoi();
        }

        private static Control Khoi(string tieuDe, DataGridView luoi)
        {
            var p = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 6, 0) };
            p.Controls.Add(luoi);
            p.Controls.Add(Ui.TieuDeKhung(tieuDe));
            return p;
        }

        public void LamMoi()
        {
            try
            {
                _nhatKy = _bus.TimKiem(_tu.Value, _den.Value, _tuKhoa.Text);
                Ui.GanDuLieu(_luoi, _nhatKy.DefaultView);
                if (_luoi.Columns.Contains("MaNK")) _luoi.Columns["MaNK"].Visible = false;
                if (_luoi.Columns.Contains("ChiTiet")) _luoi.Columns["ChiTiet"].FillWeight = 260;
                if (_luoi.Columns.Contains("ThoiGian")) _luoi.Columns["ThoiGian"].FillWeight = 150;
                Ui.GanDuLieu(_luoiNV, _bus.HieuNangNhanVien(_tu.Value, _den.Value));
                Ui.GanDuLieu(_luoiTT, _bus.HieuNangThaoTac(_tu.Value, _den.Value));
                Loc();
            }
            catch (Exception ex) { Ui.BaoLoi(this, ex, "tra cứu nhật ký"); }
        }

        /// <summary>Truy van trong form: RowFilter tren DataView, khong gui lenh nao xuong CSDL.</summary>
        private void Loc()
        {
            if (_nhatKy == null) return;
            string k = _locNhanh.Text.Trim().Replace("'", "''").Replace("[", "[[]").Replace("*", "[*]").Replace("%", "[%]");
            string dk = k.Length == 0 ? "" :
                string.Format("(HanhDong LIKE '%{0}%' OR ChiTiet LIKE '%{0}%' OR TenDangNhap LIKE '%{0}%' OR DoiTuong LIKE '%{0}%')", k);
            if (_chiLoi.Checked) dk = (dk.Length == 0 ? "" : dk + " AND ") + "KetQua = 'Thất bại'";
            _nhatKy.DefaultView.RowFilter = dk;

            int hien = _nhatKy.DefaultView.Count;
            object loi = _nhatKy.Compute("COUNT(MaNK)", "KetQua = 'Thất bại'");
            object tb = _nhatKy.Compute("AVG(ThoiGianMs)", "");
            _tomTat.Text = "Hiển thị " + hien + "/" + _nhatKy.Rows.Count + " dòng · " + loi + " thất bại · TB "
                + (tb == DBNull.Value ? 0 : Convert.ToInt32(tb)) + " ms";
        }
    }
}
