using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Lab09_DataReaderSearch.BUS;
using Lab09_DataReaderSearch.DTO;
using Lab09_DataReaderSearch.GUI.Helpers;

namespace Lab09_DataReaderSearch.GUI
{
    /// <summary>
    /// BAI 8 - Loc theo tuoi (bai tu thuc hien).
    ///
    /// Kho du nhat cua bai nay: CSDL khong luu TUOI, chi luu NGAY SINH, va
    /// "trong khoang tuoi" la mot khai niem phu thuoc NGAY HOM NAY.
    ///
    /// Cach lam dung: doi khoang tuoi thanh khoang NGAY SINH roi so sanh.
    ///     tuoi >= 20  <=>  NgaySinh <= homNay - 20 nam
    ///     tuoi <= 25  <=>  NgaySinh >  homNay - 26 nam
    ///
    /// Vi sao la -26 chu khong phai -25: nguoi sinh ngay homNay - 25 nam vua
    /// tron 25 tuoi, van thuoc nhom "<= 25". Nguoi sinh homNay - 26 nam thi
    /// da 26 tuoi -> phai loai. Dau ">" (khong lay bien) thuc hien dung viec do.
    ///
    /// Cach SAI thuong gap: WHERE DATEDIFF(YEAR, NgaySinh, GETDATE()) BETWEEN
    /// @TuoiTu AND @TuoiDen. Ham boc cot lam SQL Server phai tinh cho tung
    /// dong -> khong dung duoc index, va DATEDIFF nam con dem sai voi nguoi
    /// chua toi sinh nhat.
    /// </summary>
    public class frmBai8 : BaiFormBase
    {
        private readonly SinhVienBUS _bus = new SinhVienBUS();

        private LuoiSinhVien _luoi;
        private TextBox _txtTuoiTu;
        private TextBox _txtTuoiDen;
        private Label _lblDoiChieu;
        private Label _lblNgaySinh;

        public frmBai8()
            : base("8", "Lọc theo tuổi",
                   "Tuổi suy ra từ Ngày sinh: đổi khoảng tuổi thành khoảng ngày để dùng được index",
                   UiTheme.Orange, "Ngày sinh chính xác", "DATEDIFF năm dễ sai", "frmBai8",
                   new Size(1050, 720))
        {
            Badge = new BadgeLabel();
            Badge.Value = "tuổi → khoảng ngày";
            Badge.BaseColor = UiTheme.Orange;
            Badge.SetBounds(NoiDung.Right - 190, 20, 170, 24);

            TaoDieuKhien();
            TaoLuoi();
        }

        private void TaoDieuKhien()
        {
            CardPanel the = The(18, 18, 1002, 210);
            NoiDung.Controls.Add(the);

            Label tieuDe = TieuDeMuc("KHOẢNG TUỔI CẦN LỌC", UiTheme.Orange);
            tieuDe.SetBounds(18, 14, 400, 22);
            the.Controls.Add(tieuDe);

            Label nhanTu = NhanTruong("Tuổi từ");
            nhanTu.SetBounds(18, 44, 110, 18);
            the.Controls.Add(nhanTu);

            _txtTuoiTu = TaoO("TuoiTu", 18, 64, 110, "VD: 19");
            _txtTuoiTu.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; Loc(); }
            };
            the.Controls.Add(_txtTuoiTu);

            Label nhanDen = NhanTruong("Tuổi đến");
            nhanDen.SetBounds(146, 44, 110, 18);
            the.Controls.Add(nhanDen);

            _txtTuoiDen = TaoO("TuoiDen", 146, 64, 110, "VD: 23");
            the.Controls.Add(_txtTuoiDen);

            AccentButton btnLoc = Nut("btnLoc", "Lọc theo tuổi", UiTheme.Orange, 278, 62, 140, 30);
            btnLoc.Click += (s, e) => Loc();
            the.Controls.Add(btnLoc);

            AccentButton btnDoiChieu = Nut("btnDoiChieu", "Đối chiếu DATEDIFF", UiTheme.Indigo, 430, 62, 180, 30, true);
            btnDoiChieu.Click += (s, e) => DoiChieu();
            the.Controls.Add(btnDoiChieu);

            AccentButton btnTatCa = Nut("btnTatCa", "Tất cả", UiTheme.Slate, 622, 62, 90, 30, true);
            btnTatCa.Click += (s, e) => NapTatCa();
            the.Controls.Add(btnTatCa);

            // Cac nhom tuoi thu nhanh
            Label nhanNhanh = NhanTruong("Nhóm tuổi");
            nhanNhanh.SetBounds(18, 106, 120, 18);
            the.Controls.Add(nhanNhanh);

            TaoNutNhom(the, "18 – 20", 18, 20, 18);
            TaoNutNhom(the, "21 – 22", 21, 22, 140);
            TaoNutNhom(the, "23 – 24", 23, 24, 262);
            TaoNutNhom(the, "Từ 25", 25, null, 384);

            _lblNgaySinh = UiFactory.Label("", 8.25f, UiTheme.InkSoft, false);
            _lblNgaySinh.Font = UiTheme.FontMono(8f);
            _lblNgaySinh.SetBounds(18, 146, 520, 24);
            the.Controls.Add(_lblNgaySinh);

            _lblDoiChieu = UiFactory.Label("", 8.25f, UiTheme.Muted, false);
            _lblDoiChieu.SetBounds(18, 172, 966, 30);
            the.Controls.Add(_lblDoiChieu);

            Label ghiChu = UiFactory.Label(
                "Người sinh 31/12/2005: DATEDIFF(YEAR,...) đã trả về 1 tuổi từ ngày 01/01/2006 — " +
                "sai vì chưa tới sinh nhật. Cách đổi sang khoảng NgàySinh dưới đây xử lý đúng.",
                8.25f, UiTheme.Muted, false);
            ghiChu.SetBounds(552, 146, 432, 44);
            the.Controls.Add(ghiChu);
        }

        private void TaoNutNhom(CardPanel the, string nhan, int tu, int? den, int x)
        {
            AccentButton nut = Nut("btnNhom_" + x, nhan, UiTheme.Orange, x, 124, 112, 28, true);
            nut.Click += (s, e) =>
            {
                UiFactory.SetVal(_txtTuoiTu, tu.ToString());
                UiFactory.SetVal(_txtTuoiDen, den.HasValue ? den.Value.ToString() : "");
                Loc();
            };
            the.Controls.Add(nut);
        }

        private void TaoLuoi()
        {
            CardPanel the = The(18, 238, 1002, 368);
            the.Padding = new Padding(1);
            NoiDung.Controls.Add(the);

            _luoi = new LuoiSinhVien();
            _luoi.Dock = DockStyle.Fill;
            the.Controls.Add(_luoi);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            NapTatCa();
        }

        private void NapTatCa()
        {
            try
            {
                DataTable table = _bus.LoadStudents();
                _luoi.GanDuLieu(table);

                var btnChuThich = _lblNgaySinh;
                btnChuThich.Text = "Ngày hôm nay: " + DateTime.Today.ToString("dd/MM/yyyy") +
                                   " — hiển thị toàn bộ " + table.Rows.Count + " sinh viên.";
                _lblDoiChieu.Text = "";
                GhiChu("Nhập khoảng tuổi rồi bấm \"Lọc theo tuổi\".");
            }
            catch (Exception ex) { LoiChung(ex); }
        }

        private int? DocTuoi(TextBox tb, string ten)
        {
            string s = UiFactory.Val(tb);
            if (string.IsNullOrEmpty(s)) return null;

            int i;
            if (!int.TryParse(s, out i))
                throw new LoiNghiepVu(ten, "Tuổi phải là số nguyên (giá trị \"" + s + "\").");
            return i;
        }

        private void Loc()
        {
            try
            {
                int? tu = DocTuoi(_txtTuoiTu, "TuoiTu");
                int? den = DocTuoi(_txtTuoiDen, "TuoiDen");

                DataTable table = _bus.LocTheoTuoi(tu, den);
                _luoi.GanDuLieu(table);

                // Hien thi khoang NgaySinh tuong ung de thay ro cach doi
                DateTime homNay = DateTime.Today;
                string moTa = "NgàySinh";
                if (tu.HasValue) moTa += " <= " + homNay.AddYears(-tu.Value).ToString("dd/MM/yyyy");
                if (den.HasValue) moTa += (tu.HasValue ? " AND " : "") + " NgàySinh > " + homNay.AddYears(-(den.Value + 1)).ToString("dd/MM/yyyy");
                _lblNgaySinh.Text = moTa;

                string nhanTuoi = (tu.HasValue ? tu.Value.ToString() : "…") + " – " + (den.HasValue ? den.Value.ToString() : "…");
                KetQua("Tuổi " + nhanTuoi + " → " + table.Rows.Count + " sinh viên.", true);
            }
            catch (LoiNghiepVu ex)
            {
                HienThiLoi(ex, Tim);
            }
            catch (Exception ex) { LoiChung(ex); }
        }

        /// <summary>
        /// Doi chieu voi cach tinh SAI (DATEDIFF nam) de thay chenh lech.
        /// Chay truc tiep tren CSDL nen ket qua la con so that, khong phai ly thuyet.
        /// </summary>
        private void DoiChieu()
        {
            try
            {
                int? tu = DocTuoi(_txtTuoiTu, "TuoiTu");
                int? den = DocTuoi(_txtTuoiDen, "TuoiDen");

                if (!tu.HasValue || !den.HasValue)
                {
                    KetQua("Cần nhập đủ tuổi từ và tuổi đến để đối chiếu.", false);
                    return;
                }

                DataTable dung = _bus.LocTheoTuoi(tu, den);

                string sqlSai = @"SELECT COUNT(*) FROM dbo.SinhVien
                                  WHERE DATEDIFF(YEAR, NgaySinh, GETDATE()) BETWEEN @Tu AND @Den";

                object kq;
                using (var conn = Lab09_DataReaderSearch.DAL.DbConnectionFactory.Create())
                using (var cmd = new System.Data.SqlClient.SqlCommand(sqlSai, conn))
                {
                    cmd.Parameters.Add("@Tu", SqlDbType.Int).Value = tu.Value;
                    cmd.Parameters.Add("@Den", SqlDbType.Int).Value = den.Value;
                    conn.Open();
                    kq = cmd.ExecuteScalar();
                }

                int sai = kq == null || kq == DBNull.Value ? 0 : Convert.ToInt32(kq);
                int chenh = sai - dung.Rows.Count;

                if (chenh == 0)
                    _lblDoiChieu.Text = string.Format(
                        "Đối chiếu tuổi {0}–{1}: cách đúng = {2} SV, DATEDIFF(YEAR) = {3} SV → trùng nhau (khoảng này chưa có ca sinh nhật chưa tới).",
                        tu, den, dung.Rows.Count, sai);
                else
                    _lblDoiChieu.Text = string.Format(
                        "Đối chiếu tuổi {0}–{1}: cách đúng = {2} SV, DATEDIFF(YEAR) = {3} SV → DATEDIFF lệch {4} SV vì đếm năm chưa trừ ngày chưa tới sinh nhật.",
                        tu, den, dung.Rows.Count, sai, Math.Abs(chenh));

                _lblDoiChieu.ForeColor = chenh == 0 ? UiTheme.Green : UiTheme.Orange;

                KetQua(string.Format("Cách đúng: {0} SV · DATEDIFF(YEAR): {1} SV · chênh lệch {2}.",
                    dung.Rows.Count, sai, chenh), chenh == 0);
            }
            catch (Exception ex) { LoiChung(ex); }
        }
    }
}
