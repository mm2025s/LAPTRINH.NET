using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Lab08_SqlCommand_Parameters.BUS;
using Lab08_SqlCommand_Parameters.DAL;
using Lab08_SqlCommand_Parameters.DTO;
using Lab08_SqlCommand_Parameters.GUI.Helpers;

namespace Lab08_SqlCommand_Parameters.GUI
{
    /// <summary>
    /// BAI 10 (tự thực hiện) - Lớp SqlHelper tối giản.
    ///
    /// SqlHelper (DAL/SqlHelper.cs) đóng gói 3 phương thức thực thi:
    ///     ExecuteScalar(sql, params)   -> object
    ///     ExecuteNonQuery(sql, params) -> int (số dòng)
    ///     ExecuteReader(sql, docDong, params) -> int (số dòng đã đọc)
    ///
    /// Điểm quan trọng về GIẢI PHÓNG TÀI NGUYÊN:
    ///     using (SqlConnection conn = ...)   -> conn.Close() + Dispose() kể cả khi có exception
    ///     using (SqlCommand cmd = ...)       -> cmd.Dispose()
    ///     using (SqlDataReader rd = ...)     -> rd.Close() trước khi đóng connection
    ///
    /// Form này chạy CÙNG các thao tác của bài 1, 2, 3 nhưng CHỈ qua SqlHelper
    /// để thấy code ngắn hơn bao nhiêu.
    /// </summary>
    public class frmBai10 : BaiFormBase
    {
        private readonly SinhVienBUS _bus = new SinhVienBUS();

        private Label _lblDem, _lblKetQuaReader, _lblKetQuaGhi;
        private ListBox _lstTen;
        private Label _lblThoiGian;

        public frmBai10()
            : base("10", "Lớp SqlHelper tối giản",
                   "Đóng gói ExecuteScalar · ExecuteNonQuery · ExecuteReader · using + Dispose",
                   UiTheme.Slate, "Lab08_DAL.SqlHelper", "SqlHelper.P(...) thay cho AddWithValue",
                   "Bài 10", new Size(860, 640))
        {
            DungGiaoDien();
            Load += (s, e) => ChayTatCa();
        }

        private void DungGiaoDien()
        {
            // ---- Cot 1: ExecuteScalar ----
            var c1 = The(18, 18, 258, 130);
            Label t1 = TieuDeMuc("ExecuteScalar", UiTheme.Violet);
            t1.SetBounds(18, 12, 220, 22);

            _lblDem = new Label();
            _lblDem.Font = UiTheme.FontTitle(30f);
            _lblDem.ForeColor = UiTheme.Ink;
            _lblDem.BackColor = Color.Transparent;
            _lblDem.SetBounds(16, 38, 220, 46);
            _lblDem.Text = "—";

            Label m1 = UiFactory.Label("COUNT(*) → int", 8.75f, UiTheme.Muted, false);
            m1.SetBounds(18, 92, 220, 22);
            c1.Controls.AddRange(new Control[] { t1, _lblDem, m1 });

            // ---- Cot 2: ExecuteNonQuery ----
            var c2 = The(286, 18, 258, 130);
            Label t2 = TieuDeMuc("ExecuteNonQuery", UiTheme.Teal);
            t2.SetBounds(18, 12, 220, 22);

            _lblKetQuaGhi = new Label();
            _lblKetQuaGhi.Font = UiTheme.FontTitle(20f);
            _lblKetQuaGhi.ForeColor = UiTheme.Ink;
            _lblKetQuaGhi.BackColor = Color.Transparent;
            _lblKetQuaGhi.SetBounds(16, 44, 220, 40);
            _lblKetQuaGhi.Text = "—";

            Label m2 = UiFactory.Label("Cập nhật điểm 1 mã SV", 8.75f, UiTheme.Muted, false);
            m2.SetBounds(18, 92, 220, 22);
            c2.Controls.AddRange(new Control[] { t2, _lblKetQuaGhi, m2 });

            // ---- Cot 3: ExecuteReader ----
            var c3 = The(554, 18, 270, 130);
            Label t3 = TieuDeMuc("ExecuteReader", UiTheme.Blue);
            t3.SetBounds(18, 12, 220, 22);

            _lblKetQuaReader = new Label();
            _lblKetQuaReader.Font = UiTheme.FontTitle(20f);
            _lblKetQuaReader.ForeColor = UiTheme.Ink;
            _lblKetQuaReader.BackColor = Color.Transparent;
            _lblKetQuaReader.SetBounds(16, 44, 230, 40);
            _lblKetQuaReader.Text = "—";

            Label m3 = UiFactory.Label("Đọc tuần tự MaSV + HoTen", 8.75f, UiTheme.Muted, false);
            m3.SetBounds(18, 92, 230, 22);
            c3.Controls.AddRange(new Control[] { t3, _lblKetQuaReader, m3 });

            // ---- ListBox ket qua reader ----
            var cardList = The(18, 160, 400, 300);
            cardList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;

            Label tList = TieuDeMuc("Dữ liệu đọc qua SqlHelper.ExecuteReader", UiTheme.Blue);
            tList.SetBounds(18, 12, 360, 22);

            _lstTen = new ListBox();
            _lstTen.Name = "lstTen";
            _lstTen.SetBounds(16, 40, 368, 244);
            _lstTen.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _lstTen.Font = UiTheme.FontMono(9.5f);
            _lstTen.BorderStyle = BorderStyle.FixedSingle;
            _lstTen.IntegralHeight = false;
            cardList.Controls.AddRange(new Control[] { tList, _lstTen });

            // ---- Ma nguon SqlHelper ----
            var cardCode = The(430, 160, 394, 300);
            cardCode.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            Label tCode = TieuDeMuc("Chữ ký lớp SqlHelper", UiTheme.Slate);
            tCode.SetBounds(18, 12, 340, 22);

            var txtCode = new TextBox();
            txtCode.Multiline = true;
            txtCode.ReadOnly = true;
            txtCode.ScrollBars = ScrollBars.Vertical;
            txtCode.BorderStyle = BorderStyle.None;
            txtCode.BackColor = Color.FromArgb(30, 34, 48);
            txtCode.ForeColor = Color.FromArgb(214, 222, 235);
            txtCode.Font = UiTheme.FontMono(8.5f);
            txtCode.SetBounds(16, 40, 362, 244);
            txtCode.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtCode.Text =
                "public static class SqlHelper\r\n" +
                "{\r\n" +
                "    // 1 giá trị (COUNT, MAX, SUM...)\r\n" +
                "    public static object ExecuteScalar(\r\n" +
                "        string sql, IEnumerable<SqlParameter> ts);\r\n" +
                "\r\n" +
                "    // số dòng bị ảnh hưởng\r\n" +
                "    public static int ExecuteNonQuery(\r\n" +
                "        string sql, IEnumerable<SqlParameter> ts);\r\n" +
                "\r\n" +
                "    // đọc từng dòng qua delegate\r\n" +
                "    public static int ExecuteReader(string sql,\r\n" +
                "        Action<SqlDataReader> docDong,\r\n" +
                "        IEnumerable<SqlParameter> ts);\r\n" +
                "\r\n" +
                "    // tạo tham số đúng kiểu (thay AddWithValue)\r\n" +
                "    public static SqlParameter P(string ten,\r\n" +
                "        SqlDbType kieu, object giaTri, int doDai);\r\n" +
                "}\r\n" +
                "\r\n" +
                "// Bên trong mỗi hàm luôn có:\r\n" +
                "using (SqlConnection conn =\r\n" +
                "         DbConnectionFactory.Create())\r\n" +
                "using (SqlCommand cmd =\r\n" +
                "         new SqlCommand(sql, conn))\r\n" +
                "{ conn.Open(); ... }  // Dispose tự gọi";
            cardCode.Controls.AddRange(new Control[] { tCode, txtCode });

            // ---- Nut chay ----
            AccentButton btnChay = Nut("btnChay", "Chạy lại 3 thao tác qua SqlHelper", UiTheme.Slate, 18, 472, 300);
            btnChay.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnChay.Click += (s, e) => ChayTatCa();

            _lblThoiGian = UiFactory.Label("", 9f, UiTheme.Muted, false);
            _lblThoiGian.SetBounds(330, 480, 494, 24);
            _lblThoiGian.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            NoiDung.Controls.AddRange(new Control[] { c1, c2, c3, cardList, cardCode, btnChay, _lblThoiGian });
        }

        private void ChayTatCa()
        {
            DateTime batDau = DateTime.Now;
            int loi = 0;

            // ---------- 1. ExecuteScalar ----------
            try
            {
                object kq = SqlHelper.ExecuteScalar("SELECT COUNT(*) FROM dbo.SinhVien");
                int n = Convert.ToInt32(kq);
                _lblDem.Text = n.ToString();
            }
            catch (Exception ex)
            {
                _lblDem.Text = "Lỗi";
                loi++;
                GhiChu("ExecuteScalar: " + ex.Message);
            }

            // ---------- 2. ExecuteReader ----------
            try
            {
                var dong = new List<string>();
                int soDong = SqlHelper.ExecuteReader(
                    "SELECT MaSV, HoTen FROM dbo.SinhVien ORDER BY HoTen",
                    rd => dong.Add(string.Format("{0,-8} {1}",
                        rd.GetString(rd.GetOrdinal("MaSV")),
                        rd.GetString(rd.GetOrdinal("HoTen")))));

                _lstTen.BeginUpdate();
                _lstTen.Items.Clear();
                foreach (string s in dong) _lstTen.Items.Add(s);
                _lstTen.EndUpdate();

                _lblKetQuaReader.Text = soDong + " dòng";
            }
            catch (Exception ex)
            {
                _lblKetQuaReader.Text = "Lỗi";
                loi++;
                GhiChu("ExecuteReader: " + ex.Message);
            }

            // ---------- 3. ExecuteNonQuery ----------
            // Cap nhat lai dung diem dang co (khong thay doi du lieu) de minh hoa
            // so dong anh huong ma khong lam sai lech CSDL mau.
            try
            {
                List<SinhVien> ds = _bus.DocDanhSach();
                if (ds.Count > 0)
                {
                    SinhVien sv = ds[0];
                    int rows = SqlHelper.ExecuteNonQuery(
                        "UPDATE dbo.SinhVien SET DiemTB = @DiemTB WHERE MaSV = @MaSV",
                        new[]
                        {
                            SqlHelper.P("@DiemTB", System.Data.SqlDbType.Decimal, sv.DiemTB),
                            SqlHelper.P("@MaSV", System.Data.SqlDbType.VarChar, sv.MaSV, 15)
                        });

                    _lblKetQuaGhi.Text = rows + " dòng";
                    if (rows == 0) GhiChu("ExecuteNonQuery trả về 0 dòng: không tìm thấy " + sv.MaSV);
                }
                else
                {
                    _lblKetQuaGhi.Text = "0";
                }
            }
            catch (Exception ex)
            {
                _lblKetQuaGhi.Text = "Lỗi";
                loi++;
                GhiChu("ExecuteNonQuery: " + ex.Message);
            }

            long ms = (long)(DateTime.Now - batDau).TotalMilliseconds;
            _lblThoiGian.Text = string.Format("3 thao tác qua SqlHelper hoàn tất trong {0} ms lúc {1}.",
                ms, DateTime.Now.ToString("HH:mm:ss"));

            if (loi == 0)
                KetQua("SqlHelper đóng gói 3 phương thức thực thi; mọi using(...) đều tự Dispose kể cả khi có lỗi.", true);
        }
    }
}
