using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyNhaThuoc.BUS;
using QuanLyNhaThuoc.DTO;
using QuanLyNhaThuoc.GUI.Helpers;

namespace QuanLyNhaThuoc.GUI
{
    /// <summary>Hai danh muc nho: nhom thuoc va nha cung cap (moi tab mot CRUD).</summary>
    public class frmDanhMuc : ManHinh
    {
        private readonly DanhMucBUS _bus = new DanhMucBUS();
        private readonly DataGridView _luoiNhom = Ui.Luoi("luoiNhom"), _luoiNcc = Ui.Luoi("luoiNcc");
        private readonly TextBox _maNhom = Ui.O("MaNhom"), _tenNhom = Ui.O("TenNhom");
        private readonly TextBox _maNcc = Ui.O("MaNCC"), _tenNcc = Ui.O("TenNCC"), _dtNcc = Ui.O("DienThoai"), _dcNcc = Ui.O("DiaChi");

        public frmDanhMuc() : base("Nhóm thuốc · Nhà cung cấp", "Danh mục dùng chung · không xóa được mục đang được thuốc / phiếu nhập sử dụng", UiTheme.Amber, new Size(1000, 640))
        {
            var tab = new TabControl { Dock = DockStyle.Fill, Font = UiTheme.FontBody(10f) };

            var tNhom = new TabPage("Nhóm thuốc") { BackColor = Color.White };
            tNhom.Controls.Add(_luoiNhom);
            tNhom.Controls.Add(Ui.ThanhNut(
                Ui.Nut("btnThemNhom", "Thêm", UiTheme.Teal, (s, e) => LuuNhom(true)),
                Ui.Nut("btnSuaNhom", "Sửa", UiTheme.Blue, (s, e) => LuuNhom(false)),
                Ui.Nut("btnXoaNhom", "Xóa", UiTheme.Red, (s, e) => XoaNhom(), true)));
            tNhom.Controls.Add(Ui.KhungNhap(2, "Mã nhóm", _maNhom, "Tên nhóm", _tenNhom));
            _luoiNhom.SelectionChanged += (s, e) =>
            {
                DataRow r = Ui.DongChon(_luoiNhom);
                if (r != null) { _maNhom.Text = (string)r["MaNhom"]; _tenNhom.Text = (string)r["TenNhom"]; }
            };

            var tNcc = new TabPage("Nhà cung cấp") { BackColor = Color.White };
            tNcc.Controls.Add(_luoiNcc);
            tNcc.Controls.Add(Ui.ThanhNut(
                Ui.Nut("btnThemNcc", "Thêm", UiTheme.Teal, (s, e) => LuuNcc(true)),
                Ui.Nut("btnSuaNcc", "Sửa", UiTheme.Blue, (s, e) => LuuNcc(false)),
                Ui.Nut("btnXoaNcc", "Xóa", UiTheme.Red, (s, e) => XoaNcc(), true)));
            tNcc.Controls.Add(Ui.KhungNhap(2, "Mã NCC", _maNcc, "Tên NCC", _tenNcc, "Điện thoại", _dtNcc, "Địa chỉ", _dcNcc));
            _luoiNcc.SelectionChanged += (s, e) =>
            {
                DataRow r = Ui.DongChon(_luoiNcc);
                if (r == null) return;
                _maNcc.Text = (string)r["MaNCC"];
                _tenNcc.Text = (string)r["TenNCC"];
                _dtNcc.Text = Convert.ToString(r["DienThoai"]);
                _dcNcc.Text = Convert.ToString(r["DiaChi"]);
            };

            tab.TabPages.Add(tNhom);
            tab.TabPages.Add(tNcc);
            NoiDung.Controls.Add(tab);
            Load += (s, e) => Nap();
        }

        private void Nap()
        {
            try
            {
                Ui.GanDuLieu(_luoiNhom, _bus.GetNhomThuoc());
                Ui.GanDuLieu(_luoiNcc, _bus.GetNhaCungCap());
            }
            catch (Exception ex) { Ui.BaoLoi(this, ex, "tải danh mục"); }
        }

        private void LuuNhom(bool moi)
        {
            try { _bus.LuuNhomThuoc(new NhomThuocDTO { MaNhom = _maNhom.Text, TenNhom = _tenNhom.Text }, moi); Nap(); }
            catch (Exception ex) { Ui.BaoLoi(this, ex, "lưu nhóm thuốc"); }
        }

        private void XoaNhom()
        {
            if (!Ui.XacNhan(this, "Xóa nhóm " + _maNhom.Text + "?")) return;
            try { _bus.XoaNhomThuoc(_maNhom.Text); Nap(); }
            catch (Exception ex) { Ui.BaoLoi(this, ex, "xóa nhóm thuốc"); }
        }

        private void LuuNcc(bool moi)
        {
            try
            {
                _bus.LuuNhaCungCap(new NhaCungCapDTO { MaNCC = _maNcc.Text, TenNCC = _tenNcc.Text, DienThoai = _dtNcc.Text, DiaChi = _dcNcc.Text }, moi);
                Nap();
            }
            catch (Exception ex) { Ui.BaoLoi(this, ex, "lưu nhà cung cấp"); }
        }

        private void XoaNcc()
        {
            if (!Ui.XacNhan(this, "Xóa nhà cung cấp " + _maNcc.Text + "?")) return;
            try { _bus.XoaNhaCungCap(_maNcc.Text); Nap(); }
            catch (Exception ex) { Ui.BaoLoi(this, ex, "xóa nhà cung cấp"); }
        }
    }

    /// <summary>Khach hang: tim theo ten/SDT, them, sua, xoa (khach chua co hoa don).</summary>
    public class frmKhachHang : ManHinh
    {
        private readonly KhachHangBUS _bus = new KhachHangBUS();
        private readonly DataGridView _luoi = Ui.Luoi("luoiKhach");
        private readonly TextBox _tim = Ui.O("TuKhoa", 220), _ten = Ui.O("HoTen"), _dt = Ui.O("DienThoai"), _dc = Ui.O("DiaChi");
        private int? _maKH;

        public frmKhachHang() : base("Khách hàng", "Khách quen được nhận diện bằng số điện thoại · lịch sử mua hàng", UiTheme.Blue, new Size(1050, 640))
        {
            NoiDung.Controls.Add(_luoi);
            NoiDung.Controls.Add(Ui.ThanhNut(Ui.Nhan("Tìm"), _tim,
                Ui.Nut("btnTim", "Tìm", UiTheme.Blue, (s, e) => Nap()),
                Ui.Nut("btnThem", "Thêm", UiTheme.Teal, (s, e) => Luu(true)),
                Ui.Nut("btnSua", "Sửa", UiTheme.Blue, (s, e) => Luu(false)),
                Ui.Nut("btnXoa", "Xóa", UiTheme.Red, (s, e) => Xoa(), true)));
            NoiDung.Controls.Add(Ui.KhungNhap(3, "Họ tên", _ten, "Điện thoại", _dt, "Địa chỉ", _dc));
            _luoi.SelectionChanged += (s, e) =>
            {
                DataRow r = Ui.DongChon(_luoi);
                if (r == null) return;
                _maKH = (int)r["MaKH"];
                _ten.Text = (string)r["HoTen"];
                _dt.Text = (string)r["DienThoai"];
                _dc.Text = Convert.ToString(r["DiaChi"]);
            };
            _tim.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) Nap(); };
            Load += (s, e) => Nap();
        }

        private void Nap()
        {
            try { Ui.GanDuLieu(_luoi, _bus.TimKiem(_tim.Text)); }
            catch (Exception ex) { Ui.BaoLoi(this, ex, "tìm khách hàng"); }
        }

        private void Luu(bool moi)
        {
            try
            {
                if (!moi && _maKH == null) throw new LoiNhapLieu("HoTen", "Chọn khách hàng cần sửa.");
                _bus.Luu(new KhachHangDTO { MaKH = moi ? null : _maKH, HoTen = _ten.Text, DienThoai = _dt.Text, DiaChi = _dc.Text });
                Nap();
            }
            catch (Exception ex) { Ui.BaoLoi(this, ex, "lưu khách hàng"); }
        }

        private void Xoa()
        {
            if (_maKH == null || !Ui.XacNhan(this, "Xóa khách hàng " + _ten.Text + "?")) return;
            try { _bus.Xoa(_maKH.Value); _maKH = null; Nap(); }
            catch (Exception ex) { Ui.BaoLoi(this, ex, "xóa khách hàng"); }
        }
    }

    /// <summary>Quan ly tai khoan nhan vien (chi Quan ly).</summary>
    public class frmNhanVien : ManHinh
    {
        private readonly NhanVienBUS _bus = new NhanVienBUS();
        private readonly DataGridView _luoi = Ui.Luoi("luoiNhanVien");
        private readonly TextBox _ten = Ui.O("HoTen"), _tdn = Ui.O("TenDangNhap"), _mk = Ui.O("MatKhau");
        private readonly ComboBox _vaiTro = Ui.Combo("VaiTro");
        private readonly CheckBox _hoatDong = new CheckBox { Text = "Đang hoạt động", Checked = true, AutoSize = true };
        private int _maNV;

        public frmNhanVien() : base("Nhân viên", "Tạo tài khoản · phân quyền Quản lý / Dược sĩ · khóa tài khoản · đặt lại mật khẩu", UiTheme.Violet, new Size(1000, 620))
        {
            _mk.UseSystemPasswordChar = true;
            _vaiTro.Items.AddRange(new object[] { VaiTroNV.DuocSi, VaiTroNV.QuanLy });
            _vaiTro.SelectedIndex = 0;

            NoiDung.Controls.Add(_luoi);
            NoiDung.Controls.Add(Ui.ThanhNut(
                Ui.Nut("btnThem", "Tạo tài khoản", UiTheme.Teal, (s, e) => Them()),
                Ui.Nut("btnSua", "Lưu thay đổi", UiTheme.Blue, (s, e) => Sua()),
                Ui.Nut("btnDatLaiMK", "Đặt lại mật khẩu", UiTheme.Amber, (s, e) => DatLai(), true)));
            NoiDung.Controls.Add(Ui.KhungNhap(2, "Họ tên", _ten, "Tên đăng nhập", _tdn, "Mật khẩu", _mk, "Vai trò", _vaiTro, "", _hoatDong));
            _luoi.SelectionChanged += (s, e) =>
            {
                DataRow r = Ui.DongChon(_luoi);
                if (r == null) return;
                _maNV = (int)r["MaNV"];
                _ten.Text = (string)r["HoTen"];
                _tdn.Text = (string)r["TenDangNhap"];
                _vaiTro.SelectedItem = (string)r["VaiTro"];
                _hoatDong.Checked = (bool)r["IsActive"];
            };
            Load += (s, e) => Nap();
        }

        private void Nap()
        {
            try { Ui.GanDuLieu(_luoi, _bus.GetAll()); }
            catch (Exception ex) { Ui.BaoLoi(this, ex, "tải nhân viên"); }
        }

        private void Them()
        {
            try
            {
                _bus.Them(new NhanVienDTO { HoTen = _ten.Text, TenDangNhap = _tdn.Text, VaiTro = (string)_vaiTro.SelectedItem }, _mk.Text);
                _mk.Clear();
                Nap();
            }
            catch (Exception ex) { Ui.BaoLoi(this, ex, "tạo tài khoản"); }
        }

        private void Sua()
        {
            try
            {
                _bus.CapNhat(new NhanVienDTO { MaNV = _maNV, HoTen = _ten.Text, VaiTro = (string)_vaiTro.SelectedItem, IsActive = _hoatDong.Checked });
                Nap();
            }
            catch (Exception ex) { Ui.BaoLoi(this, ex, "cập nhật nhân viên"); }
        }

        private void DatLai()
        {
            try
            {
                _bus.DatLaiMatKhau(_maNV, _mk.Text);
                _mk.Clear();
                Ui.ThongBao(this, "Đã đặt lại mật khẩu cho " + _tdn.Text + ".");
            }
            catch (Exception ex) { Ui.BaoLoi(this, ex, "đặt lại mật khẩu"); }
        }
    }
}
