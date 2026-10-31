using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Lab09_DataReaderSearch.BUS;
using Lab09_DataReaderSearch.GUI.Helpers;

namespace Lab09_DataReaderSearch.GUI
{
    /// <summary>
    /// BAI 2 - Tim kiem gan dung theo ho ten.
    ///
    /// Trong tam: ky tu % phai nam trong GIA TRI THAM SO, khong ghep vao SQL.
    ///     cmd.Parameters.Add("@Keyword", SqlDbType.NVarChar, 100).Value = "%" + keyword + "%";
    ///
    /// Form co san vai nut thu nhanh (go "nguyen", "tran", "  an  ") va mot o
    /// nhap tu do de thu cac truong hop kho: chuoi rong, ky tu % cua nguoi dung,
    /// chuoi chua dau nhay don (thu SQL Injection).
    /// </summary>
    public class frmBai2 : BaiFormBase
    {
        private readonly SinhVienBUS _bus = new SinhVienBUS();

        private LuoiSinhVien _luoi;
        private TextBox _txtTuKhoa;
        private Label _lblSql;

        public frmBai2()
            : base("2", "Tìm kiếm gần đúng theo họ tên",
                   "LIKE @Keyword — ký tự % nằm trong giá trị tham số, không nối chuỗi SQL",
                   UiTheme.Blue, "LIKE + SqlParameter", "NVarChar(100)", "frmBai2",
                   new Size(1010, 700))
        {
            Badge = new BadgeLabel();
            Badge.Value = "LIKE @Keyword";
            Badge.BaseColor = UiTheme.Blue;
            Badge.SetBounds(NoiDung.Right - 160, 20, 140, 24);

            TaoThanhCongCu();
            TaoLuoi();
        }

        private void TaoThanhCongCu()
        {
            CardPanel the = The(18, 18, 962, 168);
            NoiDung.Controls.Add(the);

            Label tieuDe = TieuDeMuc("TỪ KHÓA CẦN TÌM TRONG HỌ TÊN", UiTheme.Blue);
            tieuDe.SetBounds(18, 14, 420, 22);
            the.Controls.Add(tieuDe);

            Label nhan = NhanTruong("Họ tên chứa");
            nhan.SetBounds(18, 44, 120, 20);
            the.Controls.Add(nhan);

            _txtTuKhoa = TaoO("HoTen", 18, 64, 250, "Ví dụ: Nguyễn");
            _txtTuKhoa.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    Tim();
                }
            };
            _txtTuKhoa.TextChanged += (s, e) => CapNhatSql();
            the.Controls.Add(_txtTuKhoa);

            AccentButton btnTim = Nut("btnTim", "Tìm kiếm", UiTheme.Blue, 280, 62, 130, 30);
            btnTim.Click += (s, e) => Tim();
            the.Controls.Add(btnTim);

            AccentButton btnTatCa = Nut("btnTatCa", "Tải lại tất cả", UiTheme.Slate, 420, 62, 140, 30, true);
            btnTatCa.Click += (s, e) => NapTatCa();
            the.Controls.Add(btnTatCa);

            // Cac nut thu nhanh
            Label nhanThu = NhanTruong("Thử nhanh");
            nhanThu.SetBounds(18, 100, 120, 20);
            the.Controls.Add(nhanThu);

            string[] mau = { "Nguyễn", "Trần", "Thị", "An", "  an  " };
            int x = 18;
            foreach (string tu in mau)
            {
                string tuKhoa = tu;
                AccentButton nut = Nut("btnMau_" + Math.Abs(tu.GetHashCode() % 10000),
                    tu.Trim() == tu ? tu : "\"" + tu + "\"", UiTheme.Indigo, x, 120, 84, 30, true);
                nut.Click += (s, e) =>
                {
                    UiFactory.SetVal(_txtTuKhoa, tuKhoa);
                    Tim();
                };
                the.Controls.Add(nut);
                x += 92;
            }

            _lblSql = UiFactory.Label("", 8.25f, UiTheme.Muted, false);
            _lblSql.Font = UiTheme.FontMono(8f);
            _lblSql.SetBounds(500, 100, 446, 44);
            the.Controls.Add(_lblSql);

            CapNhatSql();
        }

        private void TaoLuoi()
        {
            CardPanel the = The(18, 196, 962, 410);
            the.Padding = new Padding(1);
            NoiDung.Controls.Add(the);

            _luoi = new LuoiSinhVien();
            _luoi.Dock = DockStyle.Fill;
            the.Controls.Add(_luoi);
        }

        private void CapNhatSql()
        {
            string tuKhoa = UiFactory.Val(_txtTuKhoa);
            _lblSql.Text = "WHERE sv.HoTen LIKE @Keyword\n@Keyword = \"%" + tuKhoa + "%\"";
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
                GhiChu("Hiển thị toàn bộ " + table.Rows.Count + " sinh viên — hãy nhập từ khóa để lọc.");
            }
            catch (Exception ex) { LoiChung(ex); }
        }

        private void Tim()
        {
            try
            {
                string tuKhoa = UiFactory.Val(_txtTuKhoa);
                DataTable table = _bus.SearchByName(tuKhoa);
                _luoi.GanDuLieu(table);

                if (table.Rows.Count == 0)
                    KetQua("Không có sinh viên nào có họ tên chứa \"" + tuKhoa + "\".", false);
                else
                    KetQua("Tìm thấy " + table.Rows.Count + " sinh viên có họ tên chứa \"" + tuKhoa + "\".", true);

                CapNhatSql();
            }
            catch (Lab09_DataReaderSearch.DTO.LoiNghiepVu ex)
            {
                HienThiLoi(ex, Tim);
            }
            catch (Exception ex) { LoiChung(ex); }
        }
    }
}
