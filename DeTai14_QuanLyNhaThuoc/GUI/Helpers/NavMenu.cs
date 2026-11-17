using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace QuanLyNhaThuoc.GUI.Helpers
{
    /// <summary>
    /// Menu dieu huong ve tay theo nav-tree cua hcmue-portal: nhom co nhan, muc cao
    /// 36px thut le trai 32px, duong dan huong doc ben trai, muc dang chon to mau
    /// chinh va co dau hinh thoi tren duong dan huong.
    /// </summary>
    public class NavMenu : Control
    {
        public class MucMenu
        {
            public string Nhom { get; internal set; }
            public string Ten { get; internal set; }
            public string BieuTuong { get; internal set; }
            public Func<Form> TaoTrang { get; internal set; }
            internal Rectangle Vung;
        }

        private const int CaoMuc = 36, CaoNhom = 34;
        private readonly List<KeyValuePair<string, List<MucMenu>>> _nhom = new List<KeyValuePair<string, List<MucMenu>>>();
        private MucMenu _dangChon, _dangTro;

        public event Action<MucMenu> ChonMuc;

        public NavMenu()
        {
            DoubleBuffered = true;
            BackColor = Color.White;
            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        public void ThemNhom(string ten) { _nhom.Add(new KeyValuePair<string, List<MucMenu>>(ten, new List<MucMenu>())); }

        /// <summary>bieuTuong: ky tu font Segoe MDL2 Assets.</summary>
        public MucMenu ThemMuc(string ten, string bieuTuong, Func<Form> taoTrang)
        {
            var m = new MucMenu { Nhom = _nhom[_nhom.Count - 1].Key, Ten = ten, BieuTuong = bieuTuong, TaoTrang = taoTrang };
            _nhom[_nhom.Count - 1].Value.Add(m);
            return m;
        }

        public MucMenu DangChon { get { return _dangChon; } }

        public void Chon(MucMenu m)
        {
            _dangChon = m;
            Invalidate();
            if (ChonMuc != null) ChonMuc(m);
        }

        public MucMenu TimTheoTen(string ten)
        {
            foreach (var n in _nhom) foreach (MucMenu m in n.Value) if (m.Ten == ten) return m;
            return null;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int y = 12;
            using (var fNhom = UiTheme.FontBold(8.5f))
            using (var fMuc = UiTheme.FontBody(9.75f))
            using (var fChon = UiTheme.FontBold(9.75f))
            using (var fIcon = new Font("Segoe MDL2 Assets", 10.5f))
            using (var duong = new SolidBrush(UiTheme.Border))
            using (var chinh = new SolidBrush(UiTheme.Primary))
            {
                foreach (var nhom in _nhom)
                {
                    TextRenderer.DrawText(g, nhom.Key.ToUpperInvariant(), fNhom, new Rectangle(14, y, Width - 28, CaoNhom), UiTheme.Muted, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
                    y += CaoNhom;

                    List<MucMenu> ds = nhom.Value;
                    int dau = y, chiSoChon = ds.IndexOf(_dangChon);
                    int cuoi = dau + (ds.Count - 1) * CaoMuc + CaoMuc / 2;
                    int x = 22;
                    g.FillRectangle(duong, x, dau, 1, cuoi - dau);
                    g.FillEllipse(duong, x - 2, cuoi - 2, 5, 5);
                    if (chiSoChon >= 0)
                    {
                        // Doan duong dan huong to mau chinh tu dau nhom toi muc dang chon + dau hinh thoi.
                        int yc = dau + chiSoChon * CaoMuc + CaoMuc / 2;
                        g.FillRectangle(chinh, x, dau, 1, yc - dau);
                        g.FillPolygon(chinh, new[] { new Point(x, yc - 5), new Point(x + 5, yc), new Point(x, yc + 5), new Point(x - 5, yc) });
                    }

                    foreach (MucMenu m in ds)
                    {
                        m.Vung = new Rectangle(8, y, Width - 16, CaoMuc);
                        bool chon = m == _dangChon, tro = m == _dangTro && !chon;
                        if (tro)
                            using (GraphicsPath p = UiTheme.RoundedPath(new Rectangle(m.Vung.X + 26, y + 1, m.Vung.Width - 26, CaoMuc - 2), 8))
                            using (var b = new SolidBrush(UiTheme.ItemHover)) g.FillPath(b, p);
                        Color mau = chon ? UiTheme.Primary : (tro ? UiTheme.Ink : UiTheme.Muted);
                        TextRenderer.DrawText(g, m.BieuTuong, fIcon, new Rectangle(44, y, 22, CaoMuc), mau, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
                        TextRenderer.DrawText(g, m.Ten, chon ? fChon : fMuc, new Rectangle(70, y, Width - 80, CaoMuc), mau, TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                        y += CaoMuc;
                    }
                    y += 10;
                }
            }
        }

        private MucMenu Tai(Point p)
        {
            foreach (var n in _nhom) foreach (MucMenu m in n.Value) if (m.Vung.Contains(p)) return m;
            return null;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            MucMenu m = Tai(e.Location);
            Cursor = m == null ? Cursors.Default : Cursors.Hand;
            if (m != _dangTro) { _dangTro = m; Invalidate(); }
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e) { _dangTro = null; Invalidate(); base.OnMouseLeave(e); }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            MucMenu m = Tai(e.Location);
            if (m != null) Chon(m);
            base.OnMouseClick(e);
        }
    }

    /// <summary>Khung ve vien mong mot canh (phan cach sidebar / header nhu border-border-strong).</summary>
    public class KhungVien : Panel
    {
        public bool VienPhai { get; set; }
        public bool VienDuoi { get; set; }
        public bool VienTren { get; set; }

        public KhungVien() { DoubleBuffered = true; BackColor = Color.White; ResizeRedraw = true; }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (var pen = new Pen(UiTheme.Border))
            {
                if (VienPhai) e.Graphics.DrawLine(pen, Width - 1, 0, Width - 1, Height);
                if (VienDuoi) e.Graphics.DrawLine(pen, 0, Height - 1, Width, Height - 1);
                if (VienTren) e.Graphics.DrawLine(pen, 0, 0, Width, 0);
            }
        }
    }

    /// <summary>Logo: o vuong bo goc mau chinh chu "Rx" + ten he thong + dong phu.</summary>
    public class Logo : Control
    {
        public string PhuDe { get; set; }

        public Logo() { DoubleBuffered = true; BackColor = Color.White; PhuDe = "Quản lý nhà thuốc"; }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var o = new Rectangle(0, (Height - 36) / 2, 36, 36);
            using (GraphicsPath p = UiTheme.RoundedPath(o, 8))
            using (var b = new SolidBrush(UiTheme.Primary)) g.FillPath(b, p);
            using (var f = UiTheme.FontBold(11f))
                TextRenderer.DrawText(g, "Rx", f, o, Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            using (var b = new SolidBrush(UiTheme.Accent)) g.FillEllipse(b, o.Right - 9, o.Top + 3, 6, 6);
            using (var f1 = UiTheme.FontBold(10.5f))
                TextRenderer.DrawText(g, "Nhà thuốc EAUT", f1, new Point(48, Height / 2 - 19), UiTheme.Primary);
            using (var f2 = UiTheme.FontBody(8.75f))
                TextRenderer.DrawText(g, PhuDe, f2, new Point(48, Height / 2 + 1), UiTheme.Muted);
        }
    }

    /// <summary>Hinh tron chu cai dau ten (avatar) nhu UserMenu.</summary>
    public class AnhDaiDien : Control
    {
        public AnhDaiDien() { DoubleBuffered = true; BackColor = Color.White; Size = new Size(36, 36); }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var b = new SolidBrush(UiTheme.PrimaryBg)) e.Graphics.FillEllipse(b, 0, 0, Width - 1, Height - 1);
            string[] phan = (Text ?? "").Trim().Split(' ');
            string chu = phan.Length == 0 || phan[0].Length == 0 ? "?" : phan[phan.Length - 1].Substring(0, 1).ToUpperInvariant();
            using (var f = UiTheme.FontBold(11f))
                TextRenderer.DrawText(e.Graphics, chu, f, ClientRectangle, UiTheme.Primary, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }
}
