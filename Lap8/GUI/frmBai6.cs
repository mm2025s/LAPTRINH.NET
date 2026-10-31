using System;
using System.Drawing;
using System.Windows.Forms;
using Lab08_SqlCommand_Parameters.BUS;
using Lab08_SqlCommand_Parameters.DTO;
using Lab08_SqlCommand_Parameters.GUI.Helpers;

namespace Lab08_SqlCommand_Parameters.GUI
{
    /// <summary>
    /// BAI 6 (tự thực hiện) - Tìm sinh viên theo mã.
    ///
    /// Dung ExecuteReader + SqlParameter. reader.Read() tra ve false khi khong
    /// co dong nao -> phai thong bao "khong tim thay", khong duoc goi
    /// reader.GetString(...) tren con tro rong (se nem InvalidOperationException).
    /// </summary>
    public class frmBai6 : BaiFormBase
    {
        private readonly SinhVienBUS _bus = new SinhVienBUS();

        private TextBox _txtMaSV;
        private Label _lblMaSV, _lblHoTen, _lblNgaySinh, _lblGioiTinh, _lblKhoa, _lblDiem, _lblEmail;
        private Panel _panelKetQua;
        private Label _lblTrangThai;
        private BadgeLabel _badgeTrangThai;

        public frmBai6()
            : base("6", "Tìm sinh viên theo mã",
                   "ExecuteReader + SqlParameter · reader.Read() = false → không tìm thấy",
                   UiTheme.Rose, "ExecuteReader", "WHERE sv.MaSV = @MaSV",
                   "Bài 6", new Size(800, 560))
        {
            DungGiaoDien();
        }

        private void DungGiaoDien()
        {
            var cardTim = The(18, 18, 746, 96);
            cardTim.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            Label lblTd = TieuDeMuc("Nhập mã sinh viên", UiTheme.Rose);
            lblTd.SetBounds(20, 10, 400, 22);

            _txtMaSV = TaoO("txtMaSV", 20, 38, 220, "Ví dụ: SV001");
            _txtMaSV.Font = UiTheme.FontMono(11f);
            _txtMaSV.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; TimKiem(); }
            };

            AccentButton btnTim = Nut("btnTim", "Tìm kiếm", UiTheme.Rose, 252, 36, 150, 34);
            btnTim.Click += (s, e) => TimKiem();

            AccentButton btnNgauNhien = Nut("btnNgauNhien", "Mã không tồn tại", UiTheme.Slate, 412, 36, 160, 34, true);
            btnNgauNhien.Click += (s, e) =>
            {
                UiFactory.SetVal(_txtMaSV, "SV999");
                TimKiem();
            };

            _badgeTrangThai = new BadgeLabel();
            _badgeTrangThai.BaseColor = UiTheme.Slate;
            _badgeTrangThai.Value = "Chưa tìm";
            _badgeTrangThai.SetBounds(600, 40, 126, 28);
            _badgeTrangThai.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            cardTim.Controls.AddRange(new Control[] { lblTd, _txtMaSV, btnTim, btnNgauNhien, _badgeTrangThai });

            // ---- The chi tiet ----
            _panelKetQua = The(18, 128, 746, 320);
            _panelKetQua.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            Label lblChiTiet = TieuDeMuc("Chi tiết sinh viên", UiTheme.Rose);
            lblChiTiet.SetBounds(20, 12, 400, 22);
            _panelKetQua.Controls.Add(lblChiTiet);

            _lblMaSV = TaoDongChiTiet("Mã sinh viên", 20, 46);
            _lblHoTen = TaoDongChiTiet("Họ tên", 20, 92);
            _lblNgaySinh = TaoDongChiTiet("Ngày sinh", 20, 138);
            _lblGioiTinh = TaoDongChiTiet("Giới tính", 20, 184);
            _lblKhoa = TaoDongChiTiet("Khoa", 20, 230);
            _lblDiem = TaoDongChiTiet("Điểm trung bình", 20, 276);
            _lblEmail = TaoDongChiTiet("Email", 20, 322);
            _lblEmail.AutoEllipsis = true;

            _lblTrangThai = UiFactory.Label(
                "Nhập mã sinh viên rồi nhấn Enter hoặc bấm “Tìm kiếm”.",
                9.5f, UiTheme.Muted, false);
            _lblTrangThai.SetBounds(18, 456, 746, 40);
            _lblTrangThai.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            NoiDung.Controls.AddRange(new Control[] { cardTim, _panelKetQua, _lblTrangThai });
        }

        private Label TaoDongChiTiet(string ten, int x, int y)
        {
            Label lTen = UiFactory.FieldLabel(ten);
            lTen.SetBounds(x, y, 200, 20);

            var lGiaTri = new Label();
            lGiaTri.Font = UiTheme.FontBody(11f);
            lGiaTri.ForeColor = UiTheme.Ink;
            lGiaTri.BackColor = Color.Transparent;
            lGiaTri.SetBounds(x, y + 20, 700, 26);
            lGiaTri.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lGiaTri.Text = "—";

            _panelKetQua.Controls.AddRange(new Control[] { lTen, lGiaTri });
            return lGiaTri;
        }

        private void TimKiem()
        {
            string ma = UiFactory.Val(_txtMaSV);

            try
            {
                SinhVien sv = _bus.TimTheoMa(ma);

                if (sv == null)
                {
                    XoaChiTiet();
                    _badgeTrangThai.Value = "Không tìm thấy";
                    _badgeTrangThai.SetColor(UiTheme.Red);
                    _lblTrangThai.ForeColor = UiTheme.Red;
                    _lblTrangThai.Text = "reader.Read() trả về false → không có dòng nào cho mã \"" + ma + "\".";
                    KetQua("Không tìm thấy sinh viên có mã \"" + ma + "\".", false);
                    return;
                }

                _lblMaSV.Text = sv.MaSV;
                _lblHoTen.Text = sv.HoTen;
                _lblNgaySinh.Text = sv.NgaySinh.ToString("dd/MM/yyyy");
                _lblGioiTinh.Text = sv.GioiTinhText;
                _lblKhoa.Text = sv.MaKhoa + (string.IsNullOrEmpty(sv.TenKhoa) ? "" : "  ·  " + sv.TenKhoa);
                _lblDiem.Text = sv.DiemTB.ToString("0.00");
                _lblEmail.Text = string.IsNullOrEmpty(sv.Email) ? "(NULL)" : sv.Email;

                _badgeTrangThai.Value = "Tìm thấy";
                _badgeTrangThai.SetColor(UiTheme.Green);
                _lblTrangThai.ForeColor = UiTheme.InkSoft;
                _lblTrangThai.Text = "Đọc được 1 dòng bằng ExecuteReader (forward-only, read-only).";
                KetQua("Tìm thấy " + sv.HoTen + " (" + sv.MaSV + ").", true);
            }
            catch (LoiNghiepVu ex)
            {
                XoaChiTiet();
                _badgeTrangThai.Value = "Lỗi";
                _badgeTrangThai.SetColor(UiTheme.Red);
                HienThiLoi(ex, Tim);
            }
            catch (Exception ex)
            {
                XoaChiTiet();
                LoiChung(ex);
            }
        }

        private void XoaChiTiet()
        {
            _lblMaSV.Text = _lblHoTen.Text = _lblNgaySinh.Text = "—";
            _lblGioiTinh.Text = _lblKhoa.Text = _lblDiem.Text = _lblEmail.Text = "—";
        }
    }
}
