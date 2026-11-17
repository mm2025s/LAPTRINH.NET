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
    /// Ban hang tai quay: tim thuoc -> them vao gio -> (tuy chon) gan khach theo
    /// so dien thoai -> thanh toan. Hoa don duoc ghi trong mot transaction.
    /// </summary>
    public class frmBanHang : ManHinh
    {
        private readonly BanHangBUS _bus = new BanHangBUS();
        private readonly ThuocBUS _thuoc = new ThuocBUS();
        private readonly KhachHangBUS _khach = new KhachHangBUS();

        private readonly TextBox _tim = Ui.O("TuKhoa", 170);
        private readonly NumericUpDown _sl = new NumericUpDown { Name = "SoLuong", Minimum = 1, Maximum = 10000, Value = 1, Width = 70, Font = UiTheme.FontBody(10f) };
        private readonly DataGridView _luoiThuoc = Ui.Luoi("luoiThuoc");
        private readonly DataGridView _luoiGio = Ui.Luoi("luoiGio");
        private readonly TextBox _sdt = Ui.O("DienThoai"), _tenKhach = Ui.O("TenKhach"), _soDon = Ui.O("SoDonThuoc");
        private readonly Label _lblKhach = Ui.Nhan("Khách lẻ");
        private readonly Label _lblTong = new Label { Dock = DockStyle.Top, Height = 54, Font = UiTheme.FontTitle(20f), ForeColor = UiTheme.Teal, TextAlign = ContentAlignment.MiddleRight, Padding = new Padding(0, 0, 16, 0), BackColor = Color.White };
        private int? _maKH;

        public frmBanHang() : base("Bán hàng", "Tìm thuốc → thêm vào giỏ → thanh toán (kiểm tra tồn kho, hạn dùng, thuốc kê đơn)", UiTheme.Teal, new Size(1240, 740))
        {
            // ---- Trai: tim thuoc ----
            var trai = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 0, 6, 0) };
            trai.Controls.Add(_luoiThuoc);
            trai.Controls.Add(Ui.ThanhNut(_tim, Ui.Nut("btnTim", "Tìm", UiTheme.Blue, (s, e) => NapThuoc()), Ui.Nhan("SL"), _sl,
                Ui.Nut("btnThemGio", "Thêm vào giỏ →", UiTheme.Teal, (s, e) => ThemGio())));
            trai.Controls.Add(Ui.TieuDeKhung("Thuốc đang kinh doanh"));
            _tim.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) NapThuoc(); };
            _luoiThuoc.CellDoubleClick += (s, e) => ThemGio();

            // ---- Phai: gio hang + khach + thanh toan ----
            var phai = new Panel { Dock = DockStyle.Right, Width = 470, Padding = new Padding(6, 0, 0, 0) };
            var khungKhach = Ui.KhungNhap(1, "SĐT khách", _sdt, "Tên (khách mới)", _tenKhach, "Số đơn thuốc", _soDon, "Khách hàng", _lblKhach);
            var nutKhach = Ui.ThanhNut(Ui.Nut("btnTimKhach", "Tìm / thêm khách", UiTheme.Blue, (s, e) => GanKhach(), true),
                Ui.Nut("btnXoaDong", "Xóa dòng", UiTheme.Red, (s, e) => XoaDong(), true),
                Ui.Nut("btnHuy", "Hủy giỏ", UiTheme.Slate, (s, e) => HuyGio(), true));
            var nutTT = Ui.ThanhNut(Ui.Nut("btnThanhToan", "THANH TOÁN", UiTheme.Teal, (s, e) => ThanhToan()));
            nutTT.Dock = DockStyle.Bottom;
            nutTT.FlowDirection = FlowDirection.RightToLeft;

            phai.Controls.Add(_luoiGio);
            phai.Controls.Add(_lblTong);
            phai.Controls.Add(Ui.TieuDeKhung("Giỏ hàng"));
            phai.Controls.Add(nutKhach);
            phai.Controls.Add(khungKhach);
            phai.Controls.Add(nutTT);

            NoiDung.Controls.Add(trai);
            NoiDung.Controls.Add(phai);
            Load += (s, e) => { NapThuoc(); CapNhatGio(); };
        }

        public void NapThuoc()
        {
            try
            {
                Ui.GanDuLieu(_luoiThuoc, _thuoc.TimKiem(_tim.Text, null));
                // Quay ban hang khong can (va khong nen thay) gia von.
                _luoiThuoc.Columns["GiaNhap"].Visible = false;
                _luoiThuoc.Columns["TenThuoc"].FillWeight = 220;
            }
            catch (Exception ex) { Ui.BaoLoi(this, ex, "tìm thuốc"); }
        }

        /// <summary>Them thuoc dang chon (hoac theo ma - dung khi chup anh demo) vao gio.</summary>
        public void ThemGio(string maThuoc = null, int? soLuong = null)
        {
            try
            {
                if (maThuoc != null)
                    foreach (DataGridViewRow r in _luoiThuoc.Rows)
                        if ((string)r.Cells["MaThuoc"].Value == maThuoc) _luoiThuoc.CurrentCell = r.Cells["TenThuoc"];
                _bus.ThemVaoGio(Ui.DongChon(_luoiThuoc), soLuong ?? (int)_sl.Value);
                _sl.Value = 1;
                CapNhatGio();
            }
            catch (Exception ex) { Ui.BaoLoi(this, ex, "thêm vào giỏ"); }
        }

        private void XoaDong()
        {
            var d = _luoiGio.CurrentRow == null ? null : _luoiGio.CurrentRow.DataBoundItem as DongGioHang;
            if (d == null) return;
            _bus.XoaKhoiGio(d.MaThuoc);
            CapNhatGio();
        }

        private void HuyGio()
        {
            if (_bus.Gio.Count > 0 && !Ui.XacNhan(this, "Hủy toàn bộ giỏ hàng?")) return;
            _bus.Gio.Clear();
            _maKH = null;
            _sdt.Clear(); _tenKhach.Clear(); _soDon.Clear();
            _lblKhach.Text = "Khách lẻ";
            CapNhatGio();
        }

        /// <summary>Tim khach theo SDT; chua co va da nhap ten thi tao moi.</summary>
        public void GanKhach()
        {
            try
            {
                KhachHangDTO k = _khach.TimTheoDienThoai(_sdt.Text);
                if (k == null)
                {
                    if (_tenKhach.Text.Trim().Length == 0)
                        throw new LoiNhapLieu("TenKhach", "Chưa có khách với số này. Nhập tên để tạo khách mới.");
                    k = new KhachHangDTO { HoTen = _tenKhach.Text, DienThoai = _sdt.Text };
                    k.MaKH = _khach.Luu(k);
                }
                _maKH = k.MaKH;
                _lblKhach.Text = k.HoTen + " (" + k.DienThoai + ")";
            }
            catch (Exception ex) { Ui.BaoLoi(this, ex, "tìm khách hàng"); }
        }

        private void CapNhatGio()
        {
            Ui.GanDuLieu(_luoiGio, _bus.Gio.ToArray());
            if (_luoiGio.Columns.Contains("MaThuoc")) _luoiGio.Columns["MaThuoc"].Visible = false;
            _lblTong.Text = "Tổng: " + _bus.TongTien.ToString("N0") + " đ";
            _soDon.BackColor = _bus.CoThuocKeDon ? Color.FromArgb(255, 244, 229) : Color.White;
        }

        public void ThanhToan()
        {
            try
            {
                decimal tong = _bus.TongTien;
                if (_bus.Gio.Count > 0 && !Ui.XacNhan(this, "Thanh toán " + tong.ToString("N0") + " đ?")) return;
                int maHD = _bus.ThanhToan(_maKH, _soDon.Text);
                Ui.ThongBao(this, "Đã lập hóa đơn số " + maHD + " - " + tong.ToString("N0") + " đ.");
                _maKH = null;
                _sdt.Clear(); _tenKhach.Clear(); _soDon.Clear();
                _lblKhach.Text = "Khách lẻ";
                CapNhatGio();
                NapThuoc();     // ton kho vua thay doi
            }
            catch (Exception ex)
            {
                Ui.BaoLoi(this, ex, "thanh toán");
                NapThuoc();     // ton kho co the da doi do quay khac ban
            }
        }
    }
}
