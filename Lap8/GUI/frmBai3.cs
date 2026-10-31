using System;
using System.Drawing;
using System.Windows.Forms;
using Lab08_SqlCommand_Parameters.BUS;
using Lab08_SqlCommand_Parameters.DTO;
using Lab08_SqlCommand_Parameters.GUI.Helpers;

namespace Lab08_SqlCommand_Parameters.GUI
{
    /// <summary>
    /// BAI 3 - Them sinh vien bang ExecuteNonQuery voi cau SQL tham so hoa.
    ///
    /// Diem mau chot:
    ///   - Cau INSERT dung @MaSV, @HoTen... KHONG noi chuoi -> chong SQL Injection.
    ///   - Moi tham so khai bao DUNG SqlDbType va DO DAI cua cot.
    ///   - Email de trong -> DBNull.Value (khong phai chuoi rong).
    ///   - ExecuteNonQuery tra ve SO DONG BI ANH HUONG -> hien thi thong bao.
    /// </summary>
    public class frmBai3 : BaiFormBase
    {
        private readonly SinhVienBUS _bus = new SinhVienBUS();

        private NhapSinhVienPanel _nhap;
        private Label _lblSql;
        private Label _lblSoDong;

        public frmBai3()
            : base("3", "Thêm sinh viên bằng ExecuteNonQuery",
                   "INSERT tham số hóa · SqlDbType · DBNull.Value",
                   UiTheme.Teal, "ExecuteNonQuery", "INSERT INTO dbo.SinhVien (...) VALUES (@...)",
                   "Bài 3", new Size(860, 620))
        {
            DungGiaoDien();
            Load += frmBai3_Load;
        }

        private void DungGiaoDien()
        {
            var card = The(18, 18, 806, 226);
            card.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            Label lblTieuDe = TieuDeMuc("Thông tin sinh viên cần thêm", UiTheme.Teal);
            lblTieuDe.SetBounds(20, 12, 500, 22);

            _nhap = new NhapSinhVienPanel();
            _nhap.SetBounds(20, 38, 800, 210);
            _nhap.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            card.Controls.AddRange(new Control[] { lblTieuDe, _nhap });

            // ---- Cau lenh SQL ----
            Label lblSqlTd = TieuDeMuc("Câu lệnh đang thực thi (tham số hóa)", UiTheme.Teal);
            lblSqlTd.SetBounds(18, 258, 500, 22);

            _lblSql = new Label();
            _lblSql.Name = "lblSql";
            _lblSql.Font = UiTheme.FontMono(9f);
            _lblSql.ForeColor = UiTheme.InkSoft;
            _lblSql.BackColor = Color.White;
            _lblSql.BorderStyle = BorderStyle.FixedSingle;
            _lblSql.Padding = new Padding(10);
            _lblSql.SetBounds(18, 282, 806, 74);
            _lblSql.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _lblSql.Text =
                "INSERT INTO dbo.SinhVien (MaSV, HoTen, NgaySinh, GioiTinh, MaKhoa, DiemTB, Email)\r\n" +
                "VALUES (@MaSV, @HoTen, @NgaySinh, @GioiTinh, @MaKhoa, @DiemTB, @Email)";

            // ---- Nut thao tac ----
            AccentButton btnThem = Nut("btnThem", "Thêm bản ghi", UiTheme.Teal, 18, 370, 180);
            btnThem.Click += (s, e) => Them();

            AccentButton btnXoaTrang = Nut("btnXoaTrang", "Xóa trắng", UiTheme.Slate, 208, 370, 130, 36, true);
            btnXoaTrang.Click += (s, e) => { _nhap.XoaTrang(); GhiChu("Đã xóa trắng các ô nhập."); };

            AccentButton btnDem = Nut("btnDem", "Đếm lại (ExecuteScalar)", UiTheme.Violet, 348, 370, 220, 36, true);
            btnDem.Click += (s, e) => DemLai();

            _lblSoDong = UiFactory.Label("", 10.5f, UiTheme.Ink, true);
            _lblSoDong.SetBounds(588, 376, 236, 26);
            _lblSoDong.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _lblSoDong.TextAlign = ContentAlignment.MiddleRight;

            Label lblGiaiThich = UiFactory.Label(
                "Email bỏ trống sẽ được truyền là DBNull.Value. @DiemTB khai báo Precision = 4, Scale = 2 " +
                "đúng với cột DECIMAL(4,2) — cách an toàn thay cho AddWithValue.",
                9f, UiTheme.Muted, false);
            lblGiaiThich.SetBounds(18, 418, 806, 46);
            lblGiaiThich.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            NoiDung.Controls.AddRange(new Control[]
                { card, lblSqlTd, _lblSql, btnThem, btnXoaTrang, btnDem, _lblSoDong, lblGiaiThich });
        }

        private void frmBai3_Load(object sender, EventArgs e)
        {
            try
            {
                _nhap.NapKhoa(_bus.DanhSachKhoa());
                DemLai();
            }
            catch (LoiNghiepVu ex) { HienThiLoiNghiepVu(ex); }
            catch (Exception ex) { LoiChung(ex); }
        }

        private void Them()
        {
            try
            {
                SinhVien sv = _nhap.LaySinhVien();
                KetQuaThaoTac kq = _bus.ThemSinhVien(sv);

                KetQua(kq.ThongBao + "  (ExecuteNonQuery trả về " + kq.SoDong + ")", kq.ThanhCong);
                if (kq.ThanhCong)
                {
                    _nhap.XoaTrang();
                    DemLai();
                }
            }
            catch (LoiNghiepVu ex)
            {
                HienThiLoi(ex, Tim);
            }
            catch (Exception ex)
            {
                LoiChung(ex);
            }
        }

        private void DemLai()
        {
            try
            {
                int n = _bus.CountStudents();
                _lblSoDong.Text = "Tổng số sinh viên: " + n;
            }
            catch (LoiNghiepVu)
            {
                _lblSoDong.Text = "Tổng số sinh viên: ?";
            }
        }
    }
}
