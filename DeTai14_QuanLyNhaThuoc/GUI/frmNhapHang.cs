using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyNhaThuoc.BUS;
using QuanLyNhaThuoc.DTO;
using QuanLyNhaThuoc.GUI.Helpers;

namespace QuanLyNhaThuoc.GUI
{
    /// <summary>Lap phieu nhap: ton kho, gia nhap va han dung duoc cap nhat trong mot transaction.</summary>
    public class frmNhapHang : ManHinh
    {
        private readonly NhapHangBUS _bus = new NhapHangBUS();
        private readonly ComboBox _ncc = Ui.Combo("NCC"), _thuoc = Ui.Combo("Thuoc");
        private readonly TextBox _sl = Ui.O("SoLuong"), _gia = Ui.O("DonGia");
        private readonly DateTimePicker _hsd = new DateTimePicker { Name = "HanSuDung", Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy", Font = UiTheme.FontBody(10f) };
        private readonly DataGridView _luoi = Ui.Luoi("luoiPhieu");
        private readonly Label _lblTong = new Label { Dock = DockStyle.Bottom, Height = 48, Font = UiTheme.FontTitle(16f), ForeColor = UiTheme.Amber, TextAlign = ContentAlignment.MiddleRight, Padding = new Padding(0, 0, 16, 0), BackColor = Color.White };

        public frmNhapHang() : base("Nhập hàng", "Chọn nhà cung cấp → thêm từng lô thuốc (số lượng, giá nhập, hạn dùng) → lưu phiếu", UiTheme.Amber, new Size(1100, 680))
        {
            _hsd.Value = DateTime.Today.AddYears(1);
            var khung = Ui.KhungNhap(2, "Nhà cung cấp", _ncc, "Thuốc", _thuoc, "Số lượng", _sl, "Giá nhập (đ)", _gia, "Hạn sử dụng", _hsd);
            var nut = Ui.ThanhNut(
                Ui.Nut("btnThemDong", "Thêm dòng", UiTheme.Amber, (s, e) => ThemDong()),
                Ui.Nut("btnXoaDong", "Xóa dòng", UiTheme.Red, (s, e) => XoaDong(), true),
                Ui.Nut("btnLuu", "Lưu phiếu nhập", UiTheme.Teal, (s, e) => Luu()));

            NoiDung.Controls.Add(_luoi);
            NoiDung.Controls.Add(_lblTong);
            NoiDung.Controls.Add(nut);
            NoiDung.Controls.Add(khung);
            Load += (s, e) => NapDanhMuc();
        }

        private void NapDanhMuc()
        {
            try
            {
                _ncc.DataSource = new DanhMucBUS().GetNhaCungCap();
                _ncc.DisplayMember = "TenNCC";
                _ncc.ValueMember = "MaNCC";
                _thuoc.DataSource = new ThuocBUS().TimKiem(null, null);
                _thuoc.DisplayMember = "TenThuoc";
                _thuoc.ValueMember = "MaThuoc";
                CapNhat();
            }
            catch (Exception ex) { Ui.BaoLoi(this, ex, "tải danh mục"); }
        }

        public void ThemDong(string maThuoc = null, int soLuong = 0, decimal gia = 0)
        {
            try
            {
                if (maThuoc != null) { _thuoc.SelectedValue = maThuoc; _sl.Text = soLuong.ToString(); _gia.Text = gia.ToString("0"); }
                var r = _thuoc.SelectedItem as DataRowView;
                _bus.ThemDong(Convert.ToString(_thuoc.SelectedValue), r == null ? "" : (string)r["TenThuoc"],
                    (int)Ui.DocSo(_sl, "Số lượng"), Ui.DocSo(_gia, "Giá nhập"), _hsd.Value);
                _sl.Clear(); _gia.Clear();
                CapNhat();
            }
            catch (Exception ex) { Ui.BaoLoi(this, ex, "thêm dòng nhập"); }
        }

        private void XoaDong()
        {
            var d = _luoi.CurrentRow == null ? null : _luoi.CurrentRow.DataBoundItem as DongNhapHang;
            if (d == null) return;
            _bus.Dong.Remove(d);
            CapNhat();
        }

        private void Luu()
        {
            try
            {
                decimal tong = _bus.TongTien;
                int maPN = _bus.LuuPhieu(Convert.ToString(_ncc.SelectedValue));
                CapNhat();
                Ui.ThongBao(this, "Đã lưu phiếu nhập số " + maPN + " - " + tong.ToString("N0") + " đ. Tồn kho đã được cộng.");
            }
            catch (Exception ex) { Ui.BaoLoi(this, ex, "lưu phiếu nhập"); }
        }

        private void CapNhat()
        {
            Ui.GanDuLieu(_luoi, _bus.Dong.ToArray());
            _lblTong.Text = "Tổng tiền phiếu: " + _bus.TongTien.ToString("N0") + " đ";
        }
    }

    /// <summary>Tra cuu hoa don theo khoang ngay, xem chi tiet tung hoa don.</summary>
    public class frmHoaDon : ManHinh
    {
        private readonly BanHangBUS _bus = new BanHangBUS();
        private readonly DateTimePicker _tu = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 130 };
        private readonly DateTimePicker _den = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 130 };
        private readonly DataGridView _luoi = Ui.Luoi("luoiHoaDon"), _chiTiet = Ui.Luoi("luoiChiTiet");
        private readonly Label _tong = Ui.Nhan("", true);

        public frmHoaDon() : base("Tra cứu hóa đơn", "Lọc theo khoảng ngày · chọn hóa đơn để xem chi tiết thuốc đã bán", UiTheme.Blue, new Size(1150, 700))
        {
            _tu.Value = DateTime.Today.AddDays(-7);
            var khungCT = new Panel { Dock = DockStyle.Bottom, Height = 230 };
            khungCT.Controls.Add(_chiTiet);
            khungCT.Controls.Add(Ui.TieuDeKhung("Chi tiết hóa đơn"));
            _luoi.SelectionChanged += (s, e) =>
            {
                DataRow r = Ui.DongChon(_luoi);
                if (r == null) return;
                try { Ui.GanDuLieu(_chiTiet, _bus.ChiTietHoaDon((int)r["MaHD"])); }
                catch (Exception ex) { Ui.BaoLoi(this, ex, "xem chi tiết"); }
            };

            NoiDung.Controls.Add(_luoi);
            NoiDung.Controls.Add(khungCT);
            NoiDung.Controls.Add(Ui.ThanhNut(Ui.Nhan("Từ ngày"), _tu, Ui.Nhan("đến"), _den, Ui.Nut("btnTim", "Tìm hóa đơn", UiTheme.Blue, (s, e) => Nap()),
                Ui.Nut("btnIn", "In hóa đơn", UiTheme.Blue, (s, e) => In(), true), _tong));
            Load += (s, e) => Nap();
        }

        /// <summary>Bao cao hoa don dang chon (IBaoCao) - dung cho in va cho cong cu chup anh.</summary>
        public IBaoCao BaoCaoDangChon()
        {
            DataRow r = Ui.DongChon(_luoi);
            if (r == null) throw new LoiNhapLieu("HoaDon", "Chưa chọn hóa đơn cần in.");
            return new BaoCaoHoaDon(r, _bus.ChiTietHoaDon((int)r["MaHD"]));
        }

        private void In()
        {
            try { InBaoCao.XemTruoc(this, BaoCaoDangChon()); }
            catch (Exception ex) { Ui.BaoLoi(this, ex, "in hóa đơn"); }
        }

        private void Nap()
        {
            try
            {
                DataTable t = _bus.TimHoaDon(_tu.Value, _den.Value);
                Ui.GanDuLieu(_luoi, t);
                object tong = t.Compute("SUM(TongTien)", "");
                _tong.Text = t.Rows.Count + " hóa đơn · " + (tong == DBNull.Value ? 0m : (decimal)tong).ToString("N0") + " đ";
            }
            catch (Exception ex) { Ui.BaoLoi(this, ex, "tìm hóa đơn"); }
        }
    }
}
