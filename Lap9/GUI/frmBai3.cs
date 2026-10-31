using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Lab09_DataReaderSearch.BUS;
using Lab09_DataReaderSearch.DTO;
using Lab09_DataReaderSearch.GUI.Helpers;

namespace Lab09_DataReaderSearch.GUI
{
    /// <summary>
    /// BAI 3 - Loc theo khoa va khoang diem.
    ///
    /// Dac diem: cau SQL duoc DUNG DAN theo bo loc nguoi dung chon, nhung phan
    /// noi them chi la menh de co dinh (" AND sv.MaKhoa = @MaKhoa"), con moi
    /// gia tri van di qua SqlParameter.
    ///
    /// Form cung cho doi chieu hai cach viet dieu kien tren cung mot khoang:
    ///   - DiemTB >= @DiemTu AND DiemTB <= @DiemDen
    ///   - DiemTB BETWEEN @Min AND @Max
    /// Hai cach phai cho ra cung so dong (BETWEEN bao gom ca hai bien).
    /// </summary>
    public class frmBai3 : BaiFormBase
    {
        private readonly SinhVienBUS _bus = new SinhVienBUS();

        private LuoiSinhVien _luoi;
        private ComboBox _cboKhoa;
        private TextBox _txtDiemTu;
        private TextBox _txtDiemDen;
        private Label _lblSql;
        private Label _lblDoiChieu;

        public frmBai3()
            : base("3", "Lọc theo khoa và khoảng điểm",
                   "Dựng WHERE động nhưng mọi giá trị đều là SqlParameter; so sánh với BETWEEN",
                   UiTheme.Teal, "WHERE động", "BETWEEN", "frmBai3",
                   new Size(1010, 700))
        {
            Badge = new BadgeLabel();
            Badge.Value = "WHERE + @Param";
            Badge.BaseColor = UiTheme.Teal;
            Badge.SetBounds(NoiDung.Right - 160, 20, 140, 24);

            TaoBoLoc();
            TaoLuoi();
        }

        private void TaoBoLoc()
        {
            CardPanel the = The(18, 18, 962, 196);
            NoiDung.Controls.Add(the);

            Label tieuDe = TieuDeMuc("BỘ LỌC", UiTheme.Teal);
            tieuDe.SetBounds(18, 14, 300, 22);
            the.Controls.Add(tieuDe);

            // --- Khoa
            Label nhanKhoa = NhanTruong("Khoa");
            nhanKhoa.SetBounds(18, 44, 120, 20);
            the.Controls.Add(nhanKhoa);

            _cboKhoa = UiFactory.ComboBox("MaKhoa");
            _cboKhoa.SetBounds(18, 64, 230, 28);
            _cboKhoa.SelectedIndexChanged += (s, e) => CapNhatSql();
            the.Controls.Add(_cboKhoa);

            // --- Diem tu
            Label nhanTu = NhanTruong("Điểm từ");
            nhanTu.SetBounds(268, 44, 100, 20);
            the.Controls.Add(nhanTu);

            _txtDiemTu = TaoO("DiemTu", 268, 64, 90, "0.00");
            _txtDiemTu.TextChanged += (s, e) => CapNhatSql();
            the.Controls.Add(_txtDiemTu);

            // --- Diem den
            Label nhanDen = NhanTruong("Điểm đến");
            nhanDen.SetBounds(370, 44, 100, 20);
            the.Controls.Add(nhanDen);

            _txtDiemDen = TaoO("DiemDen", 370, 64, 90, "10.00");
            _txtDiemDen.TextChanged += (s, e) => CapNhatSql();
            the.Controls.Add(_txtDiemDen);

            AccentButton btnLoc = Nut("btnLoc", "Lọc", UiTheme.Teal, 480, 62, 100, 30);
            btnLoc.Click += (s, e) => Loc();
            the.Controls.Add(btnLoc);

            AccentButton btnBetween = Nut("btnBetween", "So sánh với BETWEEN", UiTheme.Indigo, 590, 62, 190, 30, true);
            btnBetween.Click += (s, e) => SoSanhBetween();
            the.Controls.Add(btnBetween);

            AccentButton btnTatCa = Nut("btnTatCa", "Bỏ lọc", UiTheme.Slate, 790, 62, 90, 30, true);
            btnTatCa.Click += (s, e) => BoLoc();
            the.Controls.Add(btnTatCa);

            // --- Nut thu nhanh cac khoang diem
            Label nhanThu = NhanTruong("Khoảng gợi ý");
            nhanThu.SetBounds(18, 104, 120, 20);
            the.Controls.Add(nhanThu);

            TaoNutKhoang(the, "Dưới 5 (yếu)", 0m, 4.99m, 18);
            TaoNutKhoang(the, "5 – 6.5 (TB)", 5m, 6.5m, 148);
            TaoNutKhoang(the, "6.5 – 8 (khá)", 6.5m, 8m, 282);
            TaoNutKhoang(the, "Từ 8 (giỏi)", 8m, 10m, 414);

            _lblSql = UiFactory.Label("", 8f, UiTheme.Muted, false);
            _lblSql.Font = UiTheme.FontMono(7.75f);
            _lblSql.SetBounds(18, 158, 620, 32);
            the.Controls.Add(_lblSql);

            _lblDoiChieu = UiFactory.Label("", 8.5f, UiTheme.Indigo, true);
            _lblDoiChieu.SetBounds(650, 158, 296, 32);
            the.Controls.Add(_lblDoiChieu);

            CapNhatSql();
        }

        private void TaoNutKhoang(CardPanel the, string ten, decimal tu, decimal den, int x)
        {
            AccentButton nut = Nut("btnKhoang_" + x, ten, UiTheme.Teal, x, 124, 120, 28, true);
            nut.Click += (s, e) =>
            {
                UiFactory.SetVal(_txtDiemTu, tu.ToString("0.00"));
                UiFactory.SetVal(_txtDiemDen, den.ToString("0.00"));
                Loc();
            };
            the.Controls.Add(nut);
        }

        private void TaoLuoi()
        {
            CardPanel the = The(18, 224, 962, 382);
            the.Padding = new Padding(1);
            NoiDung.Controls.Add(the);

            _luoi = new LuoiSinhVien();
            _luoi.Dock = DockStyle.Fill;
            the.Controls.Add(_luoi);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            NapKhoa();
            Loc();
        }

        private void NapKhoa()
        {
            try
            {
                List<Khoa> ds = _bus.DanhSachKhoa();
                _cboKhoa.Items.Clear();
                _cboKhoa.Items.Add("(Tất cả khoa)");
                foreach (Khoa k in ds) _cboKhoa.Items.Add(k);
                _cboKhoa.SelectedIndex = 0;
            }
            catch (Exception ex) { LoiChung(ex); }
        }

        private string MaKhoaDangChon()
        {
            var k = _cboKhoa.SelectedItem as Khoa;
            return k == null ? null : k.MaKhoa;
        }

        private decimal? DocDiem(TextBox tb, string ten)
        {
            string s = UiFactory.Val(tb).Replace(',', '.');
            if (string.IsNullOrEmpty(s)) return null;

            decimal d;
            if (!decimal.TryParse(s, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out d))
                throw new LoiNghiepVu(ten, "Giá trị \"" + s + "\" không phải là số hợp lệ.");

            return d;
        }

        private void CapNhatSql()
        {
            string maKhoa = MaKhoaDangChon();
            string sql = "SELECT ... FROM SinhVien sv JOIN Khoa k ...\nWHERE 1 = 1";

            if (!string.IsNullOrEmpty(maKhoa)) sql += " AND sv.MaKhoa = @MaKhoa";
            if (!string.IsNullOrEmpty(UiFactory.Val(_txtDiemTu))) sql += " AND sv.DiemTB >= @DiemTu";
            if (!string.IsNullOrEmpty(UiFactory.Val(_txtDiemDen))) sql += " AND sv.DiemTB <= @DiemDen";

            _lblSql.Text = sql;
        }

        private void Loc()
        {
            try
            {
                string maKhoa = MaKhoaDangChon();
                decimal? tu = DocDiem(_txtDiemTu, "DiemTu");
                decimal? den = DocDiem(_txtDiemDen, "DiemDen");

                DataTable table = _bus.FilterByKhoaAndDiem(maKhoa, tu, den);
                _luoi.GanDuLieu(table);
                CapNhatSql();

                string moTa = maKhoa == null ? "tất cả khoa" : "khoa " + maKhoa;
                KetQua("Lọc " + moTa + ", điểm " + (tu.HasValue ? tu.Value.ToString("0.00") : "0") +
                       " – " + (den.HasValue ? den.Value.ToString("0.00") : "10") +
                       " → " + table.Rows.Count + " sinh viên.", true);

                _lblDoiChieu.Text = "";
            }
            catch (LoiNghiepVu ex)
            {
                HienThiLoi(ex, Tim);
            }
            catch (Exception ex) { LoiChung(ex); }
        }

        /// <summary>
        /// Chay lai dung khoang diem do bang BETWEEN roi so sanh voi ket qua
        /// cua dieu kien >= / &lt;=. Hai cach phai bang nhau.
        /// </summary>
        private void SoSanhBetween()
        {
            try
            {
                decimal? tu = DocDiem(_txtDiemTu, "DiemTu");
                decimal? den = DocDiem(_txtDiemDen, "DiemDen");

                if (!tu.HasValue || !den.HasValue)
                {
                    KetQua("Cần nhập đủ cả điểm từ và điểm đến để so sánh với BETWEEN.", false);
                    return;
                }

                // BETWEEN khong loc theo khoa, nen phai so sanh tren cung tap: dung khoa = Tat ca
                if (!string.IsNullOrEmpty(MaKhoaDangChon()))
                {
                    KetQua("BETWEEN không lọc theo khoa — hãy chọn \"(Tất cả khoa)\" trước khi so sánh.", false);
                    return;
                }

                DataTable a = _bus.FilterByKhoaAndDiem(null, tu, den);
                DataTable b = _bus.FilterByDiemBetween(tu.Value, den.Value);

                bool giong = a.Rows.Count == b.Rows.Count;

                _lblDoiChieu.Text = string.Format(">= / <=: {0} dòng\nBETWEEN: {1} dòng{2}",
                    a.Rows.Count, b.Rows.Count, giong ? "  ✔ giống nhau" : "  ✖ khác nhau");

                if (giong)
                    KetQua("Hai cách viết điều kiện cho cùng kết quả: " + a.Rows.Count + " sinh viên trong [" +
                           tu.Value.ToString("0.00") + ", " + den.Value.ToString("0.00") + "].", true);
                else
                    KetQua("Hai cách cho kết quả khác nhau — kiểm tra lại điều kiện.", false);
            }
            catch (LoiNghiepVu ex) { KetQua(ex.Message, false); }
            catch (Exception ex) { LoiChung(ex); }
        }

        private void BoLoc()
        {
            UiFactory.SetVal(_txtDiemTu, "");
            UiFactory.SetVal(_txtDiemDen, "");
            if (_cboKhoa.Items.Count > 0) _cboKhoa.SelectedIndex = 0;
            Loc();
        }
    }
}
