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
    /// BAI 5 - Hien thi chi tiet dong duoc chon.
    ///
    /// Khi nguoi dung bam mot dong tren luoi, Form doc MaSV cua dong do roi
    /// goi lai DAL de lay DU lieu day du bang SqlDataReader (khong dung du lieu
    /// da co tren luoi) - dung theo yeu cau "dong bo dong chon voi vung chi tiet".
    ///
    /// Cac diem xu ly DBNull:
    ///   - Cot Email co the la NULL -> reader["Email"] la DBNull.Value.
    ///   - Cot DangKy (neu khong co) tra ve null.
    ///   - NgaySinh NULL -> Tuoi = null.
    /// </summary>
    public class frmBai5 : BaiFormBase
    {
        private readonly SinhVienBUS _bus = new SinhVienBUS();

        private LuoiSinhVien _luoi;
        private readonly Dictionary<string, Label> _oChiTiet = new Dictionary<string, Label>();
        private Label _lblMaDangChon;
        private Label _lblEmailCanhBao;

        public frmBai5()
            : base("5", "Hiển thị chi tiết dòng được chọn",
                   "SelectionChanged → đọc MaSV → SqlDataReader lấy chi tiết đầy đủ",
                   UiTheme.Indigo, "SelectionChanged", "xử lý DBNull", "frmBai5",
                   new Size(1050, 700))
        {
            Badge = new BadgeLabel();
            Badge.Value = "DataGridView → chi tiết";
            Badge.BaseColor = UiTheme.Indigo;
            Badge.SetBounds(NoiDung.Right - 190, 20, 170, 24);

            TaoLuoi();
            TaoVungChiTiet();
        }

        private void TaoLuoi()
        {
            CardPanel the = The(18, 18, 600, 588);
            the.Padding = new Padding(1);
            NoiDung.Controls.Add(the);

            _luoi = new LuoiSinhVien();
            _luoi.Dock = DockStyle.Fill;
            _luoi.DongDuocChon += (s, row) => HienThiChiTiet(LuoiSinhVien.Chuoi(row, "MaSV"));
            the.Controls.Add(_luoi);
        }

        private void TaoVungChiTiet()
        {
            CardPanel the = The(630, 18, 384, 588);
            NoiDung.Controls.Add(the);

            Label tieuDe = TieuDeMuc("THÔNG TIN CHI TIẾT", UiTheme.Indigo);
            tieuDe.SetBounds(18, 14, 300, 22);
            the.Controls.Add(tieuDe);

            _lblMaDangChon = UiFactory.Label("Chưa chọn dòng nào", 11f, UiTheme.Ink, true);
            _lblMaDangChon.SetBounds(18, 42, 348, 26);
            the.Controls.Add(_lblMaDangChon);

            Panel ke = DuongKe(18, 74, 348);
            the.Controls.Add(ke);

            TaoDongChiTiet(the, "HoTen", "Họ và tên", 86);
            TaoDongChiTiet(the, "NgaySinh", "Ngày sinh", 124);
            TaoDongChiTiet(the, "Tuoi", "Tuổi", 162);
            TaoDongChiTiet(the, "GioiTinh", "Giới tính", 200);
            TaoDongChiTiet(the, "Khoa", "Khoa", 238);
            TaoDongChiTiet(the, "DiemTB", "Điểm trung bình", 276);
            TaoDongChiTiet(the, "Email", "Email", 314);

            _lblEmailCanhBao = UiFactory.Label("", 8.25f, UiTheme.Amber, false);
            _lblEmailCanhBao.SetBounds(18, 350, 348, 20);
            the.Controls.Add(_lblEmailCanhBao);

            Panel ke2 = DuongKe(18, 378, 348);
            the.Controls.Add(ke2);

            Label nhanXepLoai = NhanTruong("Xếp loại theo điểm");
            nhanXepLoai.SetBounds(18, 388, 200, 20);
            the.Controls.Add(nhanXepLoai);

            TaoDongChiTiet(the, "XepLoai", "", 408);

            // Giai thich ve DBNull ngay tren Form
            Label ghiChu = UiFactory.Label(
                "Email để trống trong CSDL là DBNull.Value —\n" +
                "nếu ép kiểu trực tiếp sẽ ném InvalidCastException,\n" +
                "nên phải kiểm tra trước khi Convert.",
                8.25f, UiTheme.Muted, false);
            ghiChu.SetBounds(18, 452, 348, 60);
            the.Controls.Add(ghiChu);

            Label nhanSql = NhanTruong("Truy vấn chi tiết");
            nhanSql.SetBounds(18, 516, 200, 20);
            the.Controls.Add(nhanSql);

            var sql = new TextBox();
            sql.Multiline = true;
            sql.ReadOnly = true;
            sql.BorderStyle = BorderStyle.None;
            sql.Font = UiTheme.FontMono(7.75f);
            sql.BackColor = Color.FromArgb(30, 34, 48);
            sql.ForeColor = Color.FromArgb(190, 232, 200);
            sql.SetBounds(18, 538, 348, 40);
            sql.Text = "WHERE sv.MaSV = @MaSV";
            the.Controls.Add(sql);
        }

        private void TaoDongChiTiet(CardPanel the, string ten, string nhan, int y)
        {
            if (!string.IsNullOrEmpty(nhan))
            {
                Label l = NhanTruong(nhan);
                l.SetBounds(18, y, 140, 18);
                the.Controls.Add(l);
            }

            Label giaTri = UiFactory.Label("—", 9.5f, UiTheme.Ink, true);
            giaTri.Name = "lbl_" + ten;
            giaTri.SetBounds(string.IsNullOrEmpty(nhan) ? 18 : 160, y - 1,
                string.IsNullOrEmpty(nhan) ? 348 : 206, 24);
            giaTri.Font = UiTheme.FontBold(9.5f);
            the.Controls.Add(giaTri);

            _oChiTiet[ten] = giaTri;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            NapDanhSach();
        }

        private void NapDanhSach()
        {
            try
            {
                DataTable table = _bus.LoadStudents();
                _luoi.GanDuLieu(table);
                GhiChu("Đã nạp " + table.Rows.Count + " sinh viên. Chọn một dòng để xem chi tiết.");
            }
            catch (Exception ex) { LoiChung(ex); }
        }

        private void HienThiChiTiet(string maSV)
        {
            try
            {
                if (string.IsNullOrEmpty(maSV)) { XoaTrang(); return; }

                SinhVienChiTiet sv = _bus.LayChiTiet(maSV);

                if (sv == null || !sv.CoDuLieu)
                {
                    XoaTrang();
                    _lblMaDangChon.Text = "Không tìm thấy " + maSV;
                    KetQua("reader.Read() trả về false — mã \"" + maSV + "\" không còn trong CSDL.", false);
                    return;
                }

                _lblMaDangChon.Text = sv.MaSV + "  ·  " + sv.HoTen;

                Gan("HoTen", sv.HoTen);
                Gan("NgaySinh", sv.NgaySinh.HasValue ? sv.NgaySinh.Value.ToString("dd/MM/yyyy") : null);
                Gan("Tuoi", sv.Tuoi.HasValue ? sv.Tuoi.Value + " tuổi" : null);
                Gan("GioiTinh", sv.GioiTinhText);
                Gan("Khoa", sv.MaKhoa + " — " + sv.TenKhoa);
                Gan("DiemTB", sv.DiemTB.HasValue ? sv.DiemTB.Value.ToString("0.00") + " / 10" : null);
                Gan("Email", sv.Email);
                Gan("XepLoai", XepLoai(sv.DiemTB));

                _lblEmailCanhBao.Text = string.IsNullOrEmpty(sv.Email)
                    ? "Email = DBNull.Value  (hiển thị \"—\")"
                    : "";

                // To mau o diem theo nguong (dong bo voi bai 9)
                Label lblDiem = _oChiTiet["DiemTB"];
                if (sv.DiemTB.HasValue)
                {
                    if (sv.DiemTB.Value < 5m) lblDiem.ForeColor = UiTheme.Red;
                    else if (sv.DiemTB.Value >= 8m) lblDiem.ForeColor = UiTheme.Green;
                    else lblDiem.ForeColor = UiTheme.Ink;
                }

                KetQua("Đang chọn: " + sv.MaSV + " — " + sv.HoTen + " (" + sv.TenKhoa + ").", true);
            }
            catch (Exception ex) { LoiChung(ex); }
        }

        private void Gan(string ten, string giaTri)
        {
            Label l;
            if (!_oChiTiet.TryGetValue(ten, out l)) return;
            l.Text = string.IsNullOrEmpty(giaTri) ? "—" : giaTri;
            l.ForeColor = string.IsNullOrEmpty(giaTri) ? UiTheme.Muted : UiTheme.Ink;
        }

        private void XoaTrang()
        {
            foreach (Label l in _oChiTiet.Values)
            {
                l.Text = "—";
                l.ForeColor = UiTheme.Muted;
            }
            _lblMaDangChon.Text = "Chưa chọn dòng nào";
            _lblEmailCanhBao.Text = "";
        }

        private static string XepLoai(decimal? diem)
        {
            if (!diem.HasValue) return null;

            decimal d = diem.Value;
            if (d >= 9m) return "Xuất sắc";
            if (d >= 8m) return "Giỏi";
            if (d >= 6.5m) return "Khá";
            if (d >= 5m) return "Trung bình";
            return "Yếu (dưới 5)";
        }
    }
}
