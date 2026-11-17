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
    /// Danh muc thuoc: tim kiem cho moi nguoi; them/sua/xoa chi Quan ly.
    /// Ton kho va han dung khong sua tay - chi thay doi qua ban hang va nhap hang.
    /// </summary>
    public class frmThuoc : ManHinh
    {
        private readonly ThuocBUS _bus = new ThuocBUS();
        private readonly TextBox _tim = Ui.O("TuKhoa", 220);
        private readonly ComboBox _locNhom = Ui.Combo("LocNhom");
        private readonly TextBox _ma = Ui.O("MaThuoc"), _ten = Ui.O("TenThuoc"), _dvt = Ui.O("DonViTinh"), _gia = Ui.O("GiaBan");
        private readonly ComboBox _nhom = Ui.Combo("MaNhom");
        private readonly CheckBox _keDon = new CheckBox { Name = "CanKeDon", Text = "Thuốc kê đơn (bán phải có đơn bác sĩ)", AutoSize = true };
        private readonly DataGridView _luoi = Ui.Luoi("luoiThuoc");

        public frmThuoc() : base("Danh mục thuốc", "Tra cứu tồn kho, giá bán, hạn dùng · thêm / sửa / ngừng kinh doanh thuốc", UiTheme.Blue, new Size(1180, 720))
        {
            bool ql = PhienDangNhap.HienTai.LaQuanLy;
            var khung = Ui.KhungNhap(2, "Mã thuốc", _ma, "Tên thuốc", _ten, "Nhóm thuốc", _nhom, "Đơn vị tính", _dvt, "Giá bán (đ)", _gia, "", _keDon);
            var nut = Ui.ThanhNut(
                Ui.Nhan("Tìm"), _tim, _locNhom, Ui.Nut("btnTim", "Tìm", UiTheme.Blue, (s, e) => Nap()),
                Ui.Nut("btnThem", "Thêm", UiTheme.Teal, (s, e) => Luu(true)),
                Ui.Nut("btnSua", "Sửa", UiTheme.Blue, (s, e) => Luu(false)),
                Ui.Nut("btnXoa", "Xóa", UiTheme.Red, (s, e) => Xoa(), true),
                Ui.Nut("btnMoi", "Làm mới", UiTheme.Slate, (s, e) => LamMoi(), true));
            foreach (string t in new[] { "btnThem", "btnSua", "btnXoa" }) nut.Controls[t].Enabled = ql;
            khung.Enabled = ql;

            _luoi.SelectionChanged += (s, e) => HienDong();
            _tim.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) Nap(); };

            NoiDung.Controls.Add(_luoi);
            NoiDung.Controls.Add(nut);
            NoiDung.Controls.Add(khung);
            Load += (s, e) => { NapNhom(); Nap(); };
        }

        private void NapNhom()
        {
            try
            {
                DataTable nhom = new DanhMucBUS().GetNhomThuoc();
                _nhom.DataSource = nhom.Copy();
                _nhom.DisplayMember = "TenNhom";
                _nhom.ValueMember = "MaNhom";

                DataTable loc = nhom.Copy();
                DataRow tatCa = loc.NewRow();
                tatCa["MaNhom"] = "";
                tatCa["TenNhom"] = "Tất cả nhóm";
                loc.Rows.InsertAt(tatCa, 0);
                _locNhom.DataSource = loc;
                _locNhom.DisplayMember = "TenNhom";
                _locNhom.ValueMember = "MaNhom";
            }
            catch (Exception ex) { Ui.BaoLoi(this, ex, "tải nhóm thuốc"); }
        }

        public void Nap()
        {
            try { Ui.GanDuLieu(_luoi, _bus.TimKiem(_tim.Text, Convert.ToString(_locNhom.SelectedValue))); }
            catch (Exception ex) { Ui.BaoLoi(this, ex, "tìm thuốc"); }
        }

        private void HienDong()
        {
            DataRow r = Ui.DongChon(_luoi);
            if (r == null) return;
            _ma.Text = (string)r["MaThuoc"];
            _ma.ReadOnly = true;     // khong doi khoa chinh khi sua
            _ten.Text = (string)r["TenThuoc"];
            _nhom.SelectedValue = r["MaNhom"];
            _dvt.Text = (string)r["DonViTinh"];
            _gia.Text = ((decimal)r["GiaBan"]).ToString("0");
            _keDon.Checked = (bool)r["CanKeDon"];
        }

        private void Luu(bool themMoi)
        {
            try
            {
                var t = new ThuocDTO
                {
                    MaThuoc = _ma.Text, TenThuoc = _ten.Text, MaNhom = Convert.ToString(_nhom.SelectedValue),
                    DonViTinh = _dvt.Text, GiaBan = Ui.DocSo(_gia, "Giá bán"), CanKeDon = _keDon.Checked
                };
                if (themMoi && _ma.ReadOnly) throw new LoiNhapLieu("MaThuoc", "Đang chọn thuốc có sẵn. Bấm \"Làm mới\" trước khi thêm thuốc mới.");
                if (themMoi) _bus.Them(t); else _bus.CapNhat(t);
                Nap();
                Ui.ThongBao(this, themMoi ? "Đã thêm thuốc. Tồn kho = 0 cho tới khi nhập hàng." : "Đã cập nhật thuốc.");
            }
            catch (Exception ex) { Ui.BaoLoi(this, ex, themMoi ? "thêm thuốc" : "sửa thuốc"); }
        }

        private void Xoa()
        {
            DataRow r = Ui.DongChon(_luoi);
            if (r == null || !Ui.XacNhan(this, "Xóa thuốc \"" + r["TenThuoc"] + "\"?\nThuốc đã có giao dịch sẽ chuyển sang ngừng kinh doanh.")) return;
            try { _bus.Xoa((string)r["MaThuoc"]); LamMoi(); }
            catch (Exception ex) { Ui.BaoLoi(this, ex, "xóa thuốc"); }
        }

        private void LamMoi()
        {
            _ma.ReadOnly = false;
            _ma.Clear(); _ten.Clear(); _dvt.Clear(); _gia.Clear(); _tim.Clear();
            _keDon.Checked = false;
            _locNhom.SelectedIndex = 0;
            Nap();
            _luoi.ClearSelection();
            _ma.Focus();
        }
    }
}
