using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Lab09_DataReaderSearch.BUS;
using Lab09_DataReaderSearch.DTO;
using Lab09_DataReaderSearch.GUI.Helpers;

namespace Lab09_DataReaderSearch.GUI
{
    /// <summary>
    /// BAI 6 - Tim kiem da tieu chi (bai tu thuc hien).
    ///
    /// Ket hop 6 tieu chi: ma, ho ten, khoa, gioi tinh, khoang diem, khoang tuoi.
    /// Tieu chi nao BO TRONG thi KHONG sinh ra menh de WHERE tuong ung.
    ///
    /// Ben trong van dung phan trang: khi ket qua dai, nguoi dung van xem duoc
    /// tung trang thay vi phai cuon mot bang 46 dong.
    /// </summary>
    public class frmBai6 : BaiFormBase
    {
        private readonly SinhVienBUS _bus = new SinhVienBUS();

        private LuoiSinhVien _luoi;
        private ComboBox _cboKhoa;
        private ComboBox _cboGioiTinh;
        private Label _lblSql;
        private Label _lblDemDieuKien;
        private Label _lblTrang;
        private Label _lblTongKet;

        private int _chiSoTrang = 1;
        private int _tongSoTrang = 1;
        private int _tongSoDong;

        public frmBai6()
            : base("6", "Tìm kiếm đa tiêu chí",
                   "Kết hợp mã, họ tên, khoa, giới tính, khoảng điểm và khoảng tuổi — bỏ trống tiêu chí nào thì không lọc theo tiêu chí đó",
                   UiTheme.Rose, "6 tiêu chí", "WHERE động + phân trang", "frmBai6",
                   new Size(1090, 760))
        {
            Badge = new BadgeLabel();
            Badge.Value = "AND có điều kiện";
            Badge.BaseColor = UiTheme.Rose;
            Badge.SetBounds(NoiDung.Right - 170, 20, 150, 24);

            TaoBoLoc();
            TaoLuoi();
            TaoThanhTrang();
        }

        private void TaoBoLoc()
        {
            CardPanel the = The(18, 18, 1042, 232);
            NoiDung.Controls.Add(the);

            Label tieuDe = TieuDeMuc("TIÊU CHÍ TÌM KIẾM — ĐỂ TRỐNG NGHĨA LÀ KHÔNG LỌC", UiTheme.Rose);
            tieuDe.SetBounds(18, 14, 600, 22);
            the.Controls.Add(tieuDe);

            // Dong 1: ma, ho ten, khoa
            TaoOTruong(the, "Mã sinh viên", "MaSV", 18, 42, 150, "VD: SV01");
            TaoOTruong(the, "Họ tên chứa", "HoTen", 186, 42, 240, "VD: Nguyễn");

            Label nhanKhoa = NhanTruong("Khoa");
            nhanKhoa.SetBounds(444, 42, 200, 18);
            the.Controls.Add(nhanKhoa);

            _cboKhoa = UiFactory.ComboBox("MaKhoa");
            _cboKhoa.SetBounds(444, 62, 200, 28);
            _cboKhoa.SelectedIndexChanged += (s, e) => CapNhatSql();
            the.Controls.Add(_cboKhoa);

            // Dong 2: gioi tinh, khoang diem, khoang tuoi
            Label nhanGioiTinh = NhanTruong("Giới tính");
            nhanGioiTinh.SetBounds(18, 108, 120, 18);
            the.Controls.Add(nhanGioiTinh);

            _cboGioiTinh = UiFactory.ComboBox("GioiTinh");
            _cboGioiTinh.SetBounds(18, 128, 150, 28);
            _cboGioiTinh.Items.AddRange(new object[] { "(Tất cả)", "Nam", "Nữ" });
            _cboGioiTinh.SelectedIndex = 0;
            _cboGioiTinh.SelectedIndexChanged += (s, e) => CapNhatSql();
            the.Controls.Add(_cboGioiTinh);

            TaoOTruong(the, "Điểm từ", "DiemTu", 186, 108, 92, "0.00");
            TaoOTruong(the, "Điểm đến", "DiemDen", 288, 108, 92, "10.00");
            TaoOTruong(the, "Tuổi từ", "TuoiTu", 398, 108, 92, "VD: 19");
            TaoOTruong(the, "Tuổi đến", "TuoiDen", 500, 108, 92, "VD: 23");

            // Dong 3: nut thao tac
            AccentButton btnTim = Nut("btnTim", "Tìm kiếm", UiTheme.Rose, 18, 178, 130, 36);
            btnTim.Click += (s, e) => Tim(1);
            the.Controls.Add(btnTim);

            AccentButton btnXoa = Nut("btnXoaTieuChi", "Xóa tiêu chí", UiTheme.Slate, 158, 178, 130, 36, true);
            btnXoa.Click += (s, e) => XoaTieuChi();
            the.Controls.Add(btnXoa);

            _lblDemDieuKien = UiFactory.Label("0 điều kiện WHERE", 8.5f, UiTheme.Muted, true);
            _lblDemDieuKien.SetBounds(300, 186, 200, 20);
            the.Controls.Add(_lblDemDieuKien);

            _lblSql = UiFactory.Label("", 7.75f, UiTheme.Muted, false);
            _lblSql.Font = UiTheme.FontMono(7.75f);
            _lblSql.SetBounds(510, 176, 522, 46);
            the.Controls.Add(_lblSql);

            // Ghi chu
            Label ghiChu = UiFactory.Label(
                "Mọi giá trị đều là SqlParameter. Tiêu chí bỏ trống không sinh ra AND — nhờ vậy người dùng " +
                "không phải nhập đủ 6 ô, và câu SQL luôn đúng cú pháp.",
                8.25f, UiTheme.Muted, false);
            ghiChu.SetBounds(18, 214, 1014, 18);
            the.Controls.Add(ghiChu);
        }

        private void TaoOTruong(CardPanel the, string nhan, string ten, int x, int y, int rong, string goiY)
        {
            Label l = NhanTruong(nhan);
            l.SetBounds(x, y, rong, 18);
            the.Controls.Add(l);

            TextBox tb = TaoO(ten, x, y + 20, rong, goiY);
            tb.TextChanged += (s, e) => CapNhatSql();
            the.Controls.Add(tb);
        }

        private void TaoLuoi()
        {
            CardPanel the = The(18, 260, 1042, 402);
            the.Padding = new Padding(1);
            NoiDung.Controls.Add(the);

            _luoi = new LuoiSinhVien();
            _luoi.Dock = DockStyle.Fill;
            the.Controls.Add(_luoi);
        }

        private void TaoThanhTrang()
        {
            CardPanel the = The(18, 672, 1042, 56);
            NoiDung.Controls.Add(the);

            AccentButton btnTruoc = Nut("btnTrangTruoc", "◀ Trước", UiTheme.Slate, 14, 11, 100, 34, true);
            btnTruoc.Click += (s, e) => Tim(_chiSoTrang - 1);
            the.Controls.Add(btnTruoc);

            _lblTrang = UiFactory.Label("Trang 1/1", 9.5f, UiTheme.Ink, true);
            _lblTrang.SetBounds(126, 18, 220, 22);
            _lblTrang.TextAlign = ContentAlignment.MiddleCenter;
            the.Controls.Add(_lblTrang);

            AccentButton btnSau = Nut("btnTrangSau", "Sau ▶", UiTheme.Rose, 358, 11, 100, 34);
            btnSau.Click += (s, e) => Tim(_chiSoTrang + 1);
            the.Controls.Add(btnSau);

            _lblTongKet = UiFactory.Label("", 8.75f, UiTheme.Muted, false);
            _lblTongKet.SetBounds(480, 18, 548, 22);
            the.Controls.Add(_lblTongKet);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            try
            {
                List<Khoa> ds = _bus.DanhSachKhoa();
                _cboKhoa.Items.Clear();
                _cboKhoa.Items.Add("(Tất cả khoa)");
                foreach (Khoa k in ds) _cboKhoa.Items.Add(k);
                _cboKhoa.SelectedIndex = 0;
            }
            catch (Exception ex) { LoiChung(ex); return; }

            Tim(1);
        }

        private TextBox O(string ten) { return Tim(ten) as TextBox; }

        private decimal? DocThapPhan(TextBox tb, string ten)
        {
            string s = UiFactory.Val(tb).Replace(',', '.');
            if (string.IsNullOrEmpty(s)) return null;

            decimal d;
            if (!decimal.TryParse(s, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out d))
                throw new LoiNghiepVu(ten, "Giá trị \"" + s + "\" không hợp lệ.");
            return d;
        }

        private int? DocNguyen(TextBox tb, string ten)
        {
            string s = UiFactory.Val(tb);
            if (string.IsNullOrEmpty(s)) return null;

            int i;
            if (!int.TryParse(s, out i))
                throw new LoiNghiepVu(ten, "Giá trị \"" + s + "\" không phải số nguyên.");
            return i;
        }

        /// <summary>Doc bo loc tu Form. Gia tri rong -> null (khong loc).</summary>
        private TieuChiTimKiem DocTieuChi()
        {
            var tc = new TieuChiTimKiem();

            TextBox maSV = O("MaSV");
            TextBox hoTen = O("HoTen");
            if (maSV != null) tc.MaSV = UiFactory.Val(maSV);
            if (hoTen != null) tc.HoTen = UiFactory.Val(hoTen);

            var k = _cboKhoa.SelectedItem as Khoa;
            tc.MaKhoa = k == null ? null : k.MaKhoa;

            if (_cboGioiTinh.SelectedIndex == 1) tc.GioiTinh = true;
            else if (_cboGioiTinh.SelectedIndex == 2) tc.GioiTinh = false;
            else tc.GioiTinh = null;

            TextBox diemTu = O("DiemTu");
            TextBox diemDen = O("DiemDen");
            TextBox tuoiTu = O("TuoiTu");
            TextBox tuoiDen = O("TuoiDen");

            if (diemTu != null) tc.DiemTu = DocThapPhan(diemTu, "DiemTu");
            if (diemDen != null) tc.DiemDen = DocThapPhan(diemDen, "DiemDen");
            if (tuoiTu != null) tc.TuoiTu = DocNguyen(tuoiTu, "TuoiTu");
            if (tuoiDen != null) tc.TuoiDen = DocNguyen(tuoiDen, "TuoiDen");

            return tc;
        }

        private void CapNhatSql()
        {
            try
            {
                var tc = DocTieuChi();
                int dem = 0;
                string sql = "WHERE 1 = 1";

                if (!string.IsNullOrEmpty(tc.MaSV)) { sql += " AND sv.MaSV LIKE @MaSV"; dem++; }
                if (!string.IsNullOrEmpty(tc.HoTen)) { sql += " AND sv.HoTen LIKE @HoTen"; dem++; }
                if (!string.IsNullOrEmpty(tc.MaKhoa)) { sql += " AND sv.MaKhoa = @MaKhoa"; dem++; }
                if (tc.GioiTinh.HasValue) { sql += " AND sv.GioiTinh = @GioiTinh"; dem++; }
                if (tc.DiemTu.HasValue) { sql += " AND sv.DiemTB >= @DiemTu"; dem++; }
                if (tc.DiemDen.HasValue) { sql += " AND sv.DiemTB <= @DiemDen"; dem++; }
                if (tc.TuoiTu.HasValue) { sql += " AND sv.NgaySinh <= @NgaySinhLonNhat"; dem++; }
                if (tc.TuoiDen.HasValue) { sql += " AND sv.NgaySinh > @NgaySinhNhoNhat"; dem++; }

                _lblSql.Text = sql;
                _lblDemDieuKien.Text = dem + " điều kiện WHERE";
                _lblDemDieuKien.ForeColor = dem == 0 ? UiTheme.Muted : UiTheme.Rose;
            }
            catch (LoiNghiepVu)
            {
                // Dang go do - chua can bao loi
            }
        }

        private void Tim(int trang)
        {
            try
            {
                if (trang < 1) trang = 1;

                TieuChiTimKiem tc = DocTieuChi();
                KetQuaPhanTrang kq = _bus.TimKiem(tc, trang, SinhVienBUS.KichThuocTrangMacDinh);

                if (kq.ChiSoTrang > kq.TongSoTrang && kq.TongSoDong > 0)
                {
                    Tim(kq.TongSoTrang);
                    return;
                }

                _chiSoTrang = kq.ChiSoTrang;
                _tongSoTrang = kq.TongSoTrang;
                _tongSoDong = kq.TongSoDong;

                _luoi.GanDuLieu(kq.Bang as DataTable);
                _lblTrang.Text = string.Format("Trang {0}/{1}", _chiSoTrang, _tongSoTrang);

                var btnTruoc = Tim("btnTrangTruoc") as Control;
                var btnSau = Tim("btnTrangSau") as Control;
                if (btnTruoc != null) btnTruoc.Enabled = kq.CoTrangTruoc;
                if (btnSau != null) btnSau.Enabled = kq.CoTrangSau;

                _lblTongKet.Text = _tongSoDong == 0
                    ? "Không có sinh viên nào khớp tiêu chí."
                    : string.Format("{0} sinh viên khớp tiêu chí · trang {1}/{2} · 10 dòng/trang",
                        _tongSoDong, _chiSoTrang, _tongSoTrang);

                if (_tongSoDong == 0)
                    KetQua("Không tìm thấy sinh viên nào khớp tất cả tiêu chí đã nhập.", false);
                else
                    KetQua("Tìm thấy " + _tongSoDong + " sinh viên (" + ChiTietTieuChi(tc) + ").", true);

                CapNhatSql();
            }
            catch (LoiNghiepVu ex)
            {
                HienThiLoi(ex, Tim);
            }
            catch (Exception ex) { LoiChung(ex); }
        }

        private static string ChiTietTieuChi(TieuChiTimKiem tc)
        {
            var phan = new List<string>();
            if (!string.IsNullOrEmpty(tc.MaSV)) phan.Add("mã ~ \"" + tc.MaSV + "\"");
            if (!string.IsNullOrEmpty(tc.HoTen)) phan.Add("tên ~ \"" + tc.HoTen + "\"");
            if (!string.IsNullOrEmpty(tc.MaKhoa)) phan.Add("khoa " + tc.MaKhoa);
            if (tc.GioiTinh.HasValue) phan.Add(tc.GioiTinh.Value ? "nam" : "nữ");
            if (tc.DiemTu.HasValue) phan.Add("điểm ≥ " + tc.DiemTu.Value.ToString("0.00"));
            if (tc.DiemDen.HasValue) phan.Add("điểm ≤ " + tc.DiemDen.Value.ToString("0.00"));
            if (tc.TuoiTu.HasValue) phan.Add("tuổi ≥ " + tc.TuoiTu.Value);
            if (tc.TuoiDen.HasValue) phan.Add("tuổi ≤ " + tc.TuoiDen.Value);

            return phan.Count == 0 ? "không lọc" : string.Join(", ", phan);
        }

        private void XoaTieuChi()
        {
            foreach (string ten in new[] { "MaSV", "HoTen", "DiemTu", "DiemDen", "TuoiTu", "TuoiDen" })
            {
                TextBox tb = O(ten);
                if (tb != null) UiFactory.SetVal(tb, "");
            }

            if (_cboKhoa.Items.Count > 0) _cboKhoa.SelectedIndex = 0;
            _cboGioiTinh.SelectedIndex = 0;

            Tim(1);
        }
    }
}
