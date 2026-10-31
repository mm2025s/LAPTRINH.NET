using System;
using System.Drawing;
using System.Windows.Forms;
using Lab08_SqlCommand_Parameters.DTO;

namespace Lab08_SqlCommand_Parameters.GUI.Helpers
{
    /// <summary>
    /// Lop co so cho 10 Form bai tap cua Lab 8.
    ///
    /// Moi Form deu co cung bo cuc:
    ///   [ InfoStrip ]  dai tieu de (so bai + ten bai + phuong thuc ADO.NET)
    ///   [ Panel    ]   vung noi dung (Form con tu them control)
    ///   [ Status   ]   dong thong bao ket qua / loi o cuoi Form
    ///
    /// Nho vay giao dien 10 bai dong bo, va Form chi tap trung vao logic SqlCommand.
    /// </summary>
    public class BaiFormBase : FormBase
    {
        protected InfoStrip Hero;
        protected Panel NoiDung;
        protected Label LblKetQua;
        protected BadgeLabel Badge;

        private readonly string _tenTep;

        protected BaiFormBase(string soBai, string tieuDe, string phuDe, Color accent,
            string phaiTren, string phaiDuoi, string tenTep, Size kichThuoc)
        {
            _tenTep = tenTep;

            Text = "Bài " + soBai + " - " + tieuDe;
            ClientSize = kichThuoc;
            MinimumSize = new Size(Math.Max(720, kichThuoc.Width - 160), Math.Max(480, kichThuoc.Height - 100));
            StartPosition = FormStartPosition.CenterParent;

            Hero = new InfoStrip();
            Hero.Dock = DockStyle.Top;
            Hero.Accent = accent;
            Hero.ChipText = soBai;
            Hero.Title = tieuDe;
            Hero.Subtitle = phuDe;
            Hero.RightTop = phaiTren;
            Hero.RightBottom = phaiDuoi;

            NoiDung = new Panel();
            NoiDung.Dock = DockStyle.Fill;
            NoiDung.BackColor = UiTheme.Canvas;
            NoiDung.Padding = new Padding(18);
            NoiDung.AutoScroll = true;

            LblKetQua = new Label();
            LblKetQua.Name = "lblKetQua";
            LblKetQua.Dock = DockStyle.Bottom;
            LblKetQua.Height = 42;
            LblKetQua.Text = "Sẵn sàng.";
            LblKetQua.Font = UiTheme.FontBody(9.25f);
            LblKetQua.ForeColor = UiTheme.InkSoft;
            LblKetQua.BackColor = Color.White;
            LblKetQua.TextAlign = ContentAlignment.MiddleLeft;
            LblKetQua.Padding = new Padding(20, 0, 12, 0);
            LblKetQua.AutoEllipsis = true;

            Controls.Add(NoiDung);
            Controls.Add(LblKetQua);
            Controls.Add(Hero);
            Hero.BringToFront();
        }

        /// <summary>Ghi thong bao ket qua xuong dong trang thai cuoi Form.</summary>
        protected void KetQua(string text, bool thanhCong)
        {
            LblKetQua.Text = (thanhCong ? "✔  " : "✖  ") + text;
            LblKetQua.ForeColor = thanhCong ? UiTheme.Green : UiTheme.Red;
        }

        /// <summary>Ghi thong bao trung tinh (huong dan, so lieu...).</summary>
        protected void GhiChu(string text)
        {
            LblKetQua.Text = "•  " + text;
            LblKetQua.ForeColor = UiTheme.InkSoft;
        }

        protected void HienThiLoiNghiepVu(LoiNghiepVu ex)
        {
            KetQua(ex.Message, false);
        }

        protected void LoiChung(Exception ex)
        {
            KetQua(ex.Message, false);
        }

        /// <summary>Tieu de muc nho ben trong vung noi dung.</summary>
        protected static Label TieuDeMuc(string text, Color accent)
        {
            Label l = UiFactory.Label(text.ToUpperInvariant(), 8.25f, accent, true);
            l.Height = 22;
            return l;
        }

        /// <summary>Nhan ten truong (chu hoa nho mau xam) dat ngay tren o nhap.</summary>
        protected static Label NhanTruong(string text)
        {
            return UiFactory.FieldLabel(text);
        }

        /// <summary>Tao mot o TextBox theo theme, co the dat vi tri.</summary>
        protected static TextBox TaoO(string name, int x, int y, int w, string placeholder = null, bool readOnly = false)
        {
            TextBox tb = UiFactory.TextBox(name, placeholder, readOnly);
            tb.SetBounds(x, y, w, 28);
            return tb;
        }

        /// <summary>Dong trang tri ngan giua cac nhom control.</summary>
        protected static Panel DuongKe(int x, int y, int w)
        {
            var p = new Panel();
            p.SetBounds(x, y, w, 1);
            p.BackColor = UiTheme.Border;
            return p;
        }

        /// <summary>Card tron chua cac control con (dung cho nhom thao tac).</summary>
        protected static CardPanel The(int x, int y, int w, int h)
        {
            var card = new CardPanel();
            card.SetBounds(x, y, w, h);
            return card;
        }

        protected static AccentButton Nut(string name, string text, Color mau, int x, int y, int w, int h = 36, bool outline = false)
        {
            var b = new AccentButton();
            b.Name = name;
            b.Text = text;
            b.BaseColor = mau;
            b.OutlineOnly = outline;
            b.SetBounds(x, y, w, h);
            return b;
        }

        /// <summary>
        /// Hien thi loi theo truong: neu LoiNghiepVu chi ro truong nao thi focus
        /// vao o nhap tuong ung (tra ve control qua delegate tim kiem).
        /// </summary>
        protected void HienThiLoi(LoiNghiepVu ex, Func<string, Control> timTheoTen)
        {
            KetQua(ex.Message, false);
            if (string.IsNullOrEmpty(ex.Truong) || timTheoTen == null) return;

            Control c = timTheoTen(ex.Truong);
            if (c != null)
            {
                c.Focus();
                var tb = c as TextBox;
                if (tb != null && !tb.ReadOnly) tb.SelectAll();
            }
        }

        /// <summary>Tim control theo Name trong toan bo cay control.</summary>
        protected Control Tim(string ten)
        {
            return TimTrong(NoiDung, ten) ?? TimTrong(this, ten);
        }

        private static Control TimTrong(Control goc, string ten)
        {
            if (goc == null) return null;
            if (goc.Name == ten) return goc;
            foreach (Control c in goc.Controls)
            {
                Control kq = TimTrong(c, ten);
                if (kq != null) return kq;
            }
            return null;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            try
            {
                if (!string.IsNullOrEmpty(_tenTep)) GhiChu("Bài thực hành " + _tenTep + " · " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            }
            catch (Exception) { }
        }
    }
}
