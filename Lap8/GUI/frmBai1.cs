using System;
using System.Drawing;
using System.Windows.Forms;
using Lab08_SqlCommand_Parameters.BUS;
using Lab08_SqlCommand_Parameters.DTO;
using Lab08_SqlCommand_Parameters.GUI.Helpers;

namespace Lab08_SqlCommand_Parameters.GUI
{
    /// <summary>
    /// BAI 1 - Dem so sinh vien bang ExecuteScalar.
    ///
    /// ExecuteScalar tra ve object: co the la int, co the la NULL hoac DBNull.
    /// Vi vay DAL dung Convert.ToInt32(...) de chuyen doi AN TOAN thay vi ep kieu (int).
    /// </summary>
    public class frmBai1 : BaiFormBase
    {
        private readonly SinhVienBUS _bus = new SinhVienBUS();

        private Label _lblSoLuong;
        private TextBox _txtSql;

        public frmBai1()
            : base("1", "Đếm số sinh viên bằng ExecuteScalar",
                   "SqlCommand · ExecuteScalar · Convert.ToInt32(object)",
                   UiTheme.Violet, "ExecuteScalar", "SELECT COUNT(*) FROM dbo.SinhVien",
                   "Bài 1", new Size(720, 460))
        {
            DungGiaoDien();
            Load += (s, e) => Dem();
        }

        private void DungGiaoDien()
        {
            var card = The(18, 18, 660, 150);
            card.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            Label lblTieuDe = TieuDeMuc("Số lượng sinh viên trong CSDL QLSinhVien", UiTheme.Violet);
            lblTieuDe.SetBounds(24, 16, 480, 22);

            _lblSoLuong = new Label();
            _lblSoLuong.Name = "lblSoLuong";
            _lblSoLuong.Text = "—";
            _lblSoLuong.Font = UiTheme.FontTitle(46f);
            _lblSoLuong.ForeColor = UiTheme.Ink;
            _lblSoLuong.BackColor = Color.Transparent;
            _lblSoLuong.SetBounds(22, 44, 260, 78);

            Label lblDonVi = UiFactory.Label("sinh viên", 12f, UiTheme.Muted, false);
            lblDonVi.SetBounds(220, 88, 200, 30);

            Badge = new BadgeLabel();
            Badge.BaseColor = UiTheme.Violet;
            Badge.Value = "ExecuteScalar";
            Badge.SetBounds(430, 60, 150, 28);

            AccentButton btnDem = Nut("btnDem", "Đếm lại", UiTheme.Violet, 430, 98, 150, 36);
            btnDem.Click += (s, e) => Dem();

            card.Controls.AddRange(new Control[] { lblTieuDe, _lblSoLuong, lblDonVi, Badge, btnDem });

            // ---- Cau lenh SQL dang chay ----
            Label lblSql = TieuDeMuc("Câu lệnh đang thực thi", UiTheme.Violet);
            lblSql.SetBounds(18, 186, 400, 22);

            _txtSql = TaoO("txtSql", 18, 212, 660, null, true);
            _txtSql.Font = UiTheme.FontMono(10f);
            _txtSql.Text = "SELECT COUNT(*) FROM dbo.SinhVien";
            _txtSql.Height = 30;

            Label lblGiaiThich = UiFactory.Label(
                "ExecuteScalar dùng cho câu lệnh trả về ĐÚNG MỘT giá trị ở dòng đầu, cột đầu.\n" +
                "Kết quả là object → Convert.ToInt32(...) vừa chuyển kiểu vừa xử lý NULL (NULL → 0).",
                9.25f, UiTheme.InkSoft, false);
            lblGiaiThich.SetBounds(18, 254, 660, 56);

            NoiDung.Controls.AddRange(new Control[] { card, lblSql, _txtSql, lblGiaiThich });
        }

        private void Dem()
        {
            try
            {
                int soLuong = _bus.CountStudents();
                _lblSoLuong.Text = soLuong.ToString();
                Badge.Value = "ExecuteScalar";
                Badge.SetColor(UiTheme.Violet);
                KetQua(string.Format("SELECT COUNT(*) trả về {0} → kiểu int sau Convert.ToInt32(...).", soLuong), true);
            }
            catch (LoiNghiepVu ex)
            {
                _lblSoLuong.Text = "?";
                HienThiLoiNghiepVu(ex);
            }
            catch (Exception ex)
            {
                _lblSoLuong.Text = "?";
                LoiChung(ex);
            }
        }
    }
}
