using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Lab07_ADONET_Connection.BUS;
using Lab07_ADONET_Connection.GUI.Helpers;

namespace Lab07_ADONET_Connection.GUI
{
    /// <summary>
    /// frmSoDoQuanHe - ve so do quan he cac bang cua QLSinhVien bang GDI+
    /// dua tren metadata doc tu INFORMATION_SCHEMA / sys.foreign_keys.
    ///
    /// Thay cho "anh so do quan he" trong san pham nop: so do duoc ve dong
    /// theo tung bang (hop + cot), khoa chinh to dam, duong khoa ngoai noi bang con -> bang cha.
    /// </summary>
    public class frmSoDoQuanHe : FormBase
    {
        private readonly TruyVanBUS _bus = new TruyVanBUS();

        private InfoStrip _hero;
        private Panel _canvas;
        private Label _lblThongBao;
        private List<BangVe> _dsBang;
        private List<QuanHeVe> _dsQuanHe;

        private sealed class BangVe
        {
            public string Ten;
            public Rectangle Khung;
            public List<CotVe> Cot = new List<CotVe>();
        }

        private sealed class CotVe
        {
            public string Ten;
            public string Kieu;
            public bool LaPk;
            public bool LaFk;
            public Rectangle Khung;
        }

        private sealed class QuanHeVe
        {
            public string BangCon;
            public string CotCon;
            public string BangCha;
            public string CotCha;
        }

        public frmSoDoQuanHe()
        {
            InitializeComponent();
            Load += (s, e) => TaiSoDo();
        }

        private void InitializeComponent()
        {
            Text = "Sơ đồ quan hệ CSDL QLSinhVien";
            ClientSize = new Size(1000, 660);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(880, 600);

            _hero = new InfoStrip();
            _hero.Dock = DockStyle.Top;
            _hero.Accent = UiTheme.Indigo;
            _hero.ChipText = "ERD";
            _hero.Title = "Sơ đồ quan hệ các bảng QLSinhVien";
            _hero.Subtitle = "Khoá chính (PK) và khoá ngoại (FK) đọc từ metadata CSDL";
            _hero.RightTop = "sys.foreign_keys";
            _hero.RightBottom = "INFORMATION_SCHEMA";

            var root = new Panel();
            root.Dock = DockStyle.Fill;
            root.BackColor = UiTheme.Canvas;
            root.Padding = new Padding(14);

            _canvas = new Panel();
            _canvas.SetBounds(14, 12, 972, 560);
            _canvas.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _canvas.BackColor = Color.White;
            _canvas.Paint += canvas_Paint;
            _canvas.Resize += (s, e) => _canvas.Invalidate();

            _lblThongBao = new Label();
            _lblThongBao.Name = "lblThongBaoSoDo";
            _lblThongBao.Text = "";
            _lblThongBao.Font = UiTheme.FontBody(9f);
            _lblThongBao.ForeColor = UiTheme.Muted;
            _lblThongBao.BackColor = Color.Transparent;
            _lblThongBao.SetBounds(14, 576, 972, 34);
            _lblThongBao.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            root.Controls.AddRange(new Control[] { _canvas, _lblThongBao });

            Controls.Add(root);
            Controls.Add(_hero);
            _hero.BringToFront();
        }

        // =================================================================
        // TAI METADATA VA TINH TOA DO
        // =================================================================
        public void TaiSoDo()
        {
            try
            {
                _dsQuanHe = new List<QuanHeVe>();
                DataTable qh = _bus.GetQuanHe();
                foreach (DataRow r in qh.Rows)
                {
                    _dsQuanHe.Add(new QuanHeVe
                    {
                        BangCon = Convert.ToString(r["BangCon"]),
                        CotCon = Convert.ToString(r["CotCon"]),
                        BangCha = Convert.ToString(r["BangCha"]),
                        CotCha = Convert.ToString(r["CotCha"])
                    });
                }

                string[] thuTu = { "Khoa", "SinhVien", "HocPhan", "DangKy" };
                _dsBang = new List<BangVe>();
                foreach (string ten in thuTu)
                {
                    var b = new BangVe();
                    b.Ten = ten;
                    DataTable cot = _bus.GetCotCuaBang(ten);
                    foreach (DataRow r in cot.Rows)
                    {
                        string tenCot = Convert.ToString(r["COLUMN_NAME"]);
                        string kieu = Convert.ToString(r["DATA_TYPE"]);
                        bool pk = Convert.ToInt32(r["IS_PK"]) == 1;
                        bool fk = LaKhoaNgoai(ten, tenCot);
                        b.Cot.Add(new CotVe { Ten = tenCot, Kieu = kieu, LaPk = pk, LaFk = fk });
                    }
                    _dsBang.Add(b);
                }

                BoTri();
                _canvas.Invalidate();
                _lblThongBao.ForeColor = UiTheme.Green;
                _lblThongBao.Text = string.Format("Đã vẽ {0} bảng và {1} quan hệ khoá ngoại.", _dsBang.Count, _dsQuanHe.Count);
            }
            catch (Exception ex)
            {
                _lblThongBao.ForeColor = UiTheme.Red;
                _lblThongBao.Text = "Không đọc được metadata: " + ex.Message;
            }
        }

        private bool LaKhoaNgoai(string bang, string cot)
        {
            foreach (QuanHeVe qh in _dsQuanHe)
                if (qh.BangCon == bang && qh.CotCon == cot) return true;
            return false;
        }

        private void BoTri()
        {
            int boxW = 250;
            int rowH = 24;
            int headerH = 34;
            // Vi tri 4 bang: Khoa (tren trai), SinhVien (duoi trai), HocPhan (tren phai), DangKy (duoi phai)
            int[,] viTri = { { 40, 30 }, { 40, 250 }, { 420, 30 }, { 420, 250 } };

            for (int i = 0; i < _dsBang.Count; i++)
            {
                BangVe b = _dsBang[i];
                int x = viTri[i % 4, 0];
                int y = viTri[i % 4, 1];
                b.Khung = new Rectangle(x, y, boxW, headerH + b.Cot.Count * rowH + 8);

                for (int c = 0; c < b.Cot.Count; c++)
                {
                    b.Cot[c].Khung = new Rectangle(x + 1, y + headerH + c * rowH, boxW - 2, rowH);
                }
            }
        }

        // =================================================================
        // VE
        // =================================================================
        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            if (_dsBang == null || _dsBang.Count == 0) return;

            Graphics g = e.Graphics;
            UiTheme.SmoothingOn(g);

            // 1) Ve cac duong khoa ngoai truoc (nam duoi hop)
            foreach (QuanHeVe qh in _dsQuanHe)
            {
                BangVe con = TimBang(qh.BangCon);
                BangVe cha = TimBang(qh.BangCha);
                if (con == null || cha == null) continue;

                Rectangle rCon = TimCot(con, qh.CotCon);
                Rectangle rCha = TimCot(cha, qh.CotCha);
                if (rCon.IsEmpty || rCha.IsEmpty) continue;

                VeDuongFk(g, rCon, rCha);
            }

            // 2) Ve hop bang
            foreach (BangVe b in _dsBang)
            {
                VeBang(g, b);
            }

            // 3) Chu thich
            VeChuThich(g);
        }

        private BangVe TimBang(string ten)
        {
            foreach (BangVe b in _dsBang) if (b.Ten == ten) return b;
            return null;
        }

        private static Rectangle TimCot(BangVe b, string tenCot)
        {
            foreach (CotVe c in b.Cot) if (c.Ten == tenCot) return c.Khung;
            return Rectangle.Empty;
        }

        private void VeBang(Graphics g, BangVe b)
        {
            Rectangle k = b.Khung;

            // bong + than
            UiTheme.DrawSoftShadow(g, new Rectangle(k.X, k.Y, k.Width, k.Height), 5, 60);
            using (GraphicsPath path = UiTheme.RoundedPath(k, 8))
            using (var brush = new SolidBrush(Color.White))
            using (var pen = new Pen(UiTheme.Border))
            {
                g.FillPath(brush, path);
                g.DrawPath(pen, path);
            }

            // header
            Rectangle header = new Rectangle(k.X, k.Y, k.Width, 34);
            using (GraphicsPath hp = UiTheme.RoundedPath(new Rectangle(header.X, header.Y, header.Width, header.Height + 8), 8))
            using (var brush = new SolidBrush(UiTheme.Indigo))
            {
                Region old = g.Clip;
                g.SetClip(new Rectangle(header.X, header.Y, header.Width, header.Height));
                g.FillPath(brush, hp);
                g.Clip = old;
            }

            using (var brush = new SolidBrush(Color.White))
            using (var font = UiTheme.FontBold(10.5f))
            {
                g.DrawString("dbo." + b.Ten, font, brush, header.X + 12, header.Y + 8);
            }

            // cot
            using (var fontCot = UiTheme.FontBody(9f))
            using (var fontPk = UiTheme.FontBold(9f))
            using (var brushInk = new SolidBrush(UiTheme.Ink))
            using (var brushKieu = new SolidBrush(UiTheme.Muted))
            using (var fmtRight = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center })
            using (var fmtLeft = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center })
            {
                foreach (CotVe c in b.Cot)
                {
                    string nhan = (c.LaPk ? "🔑 " : (c.LaFk ? "🔗 " : "    ")) + c.Ten;
                    g.DrawString(nhan, c.LaPk ? fontPk : fontCot, brushInk,
                        new RectangleF(c.Khung.X + 6, c.Khung.Y, c.Khung.Width - 12, c.Khung.Height), fmtLeft);
                    g.DrawString(c.Kieu, fontCot, brushKieu,
                        new RectangleF(c.Khung.X + 6, c.Khung.Y, c.Khung.Width - 12, c.Khung.Height), fmtRight);
                }
            }

            using (var pen = new Pen(UiTheme.Border))
            {
                g.DrawLine(pen, k.X, k.Y + 34, k.Right, k.Y + 34);
            }
        }

        private void VeDuongFk(Graphics g, Rectangle rCon, Rectangle rCha)
        {
            Point p1 = new Point(rCon.Left, rCon.Top + rCon.Height / 2);
            Point p2 = new Point(rCha.Right, rCha.Top + rCha.Height / 2);

            // neu bang cha nam ben trai thi lay canh trai
            if (rCha.Left > rCon.Right)
            {
                p2 = new Point(rCha.Left, rCha.Top + rCha.Height / 2);
            }
            else
            {
                p1 = new Point(rCon.Right, rCon.Top + rCon.Height / 2);
                p2 = new Point(rCha.Right, rCha.Top + rCha.Height / 2);
            }

            using (var pen = new Pen(UiTheme.Teal, 2f))
            {
                pen.EndCap = LineCap.ArrowAnchor;
                pen.DashStyle = DashStyle.Solid;
                int midX = (p1.X + p2.X) / 2;
                g.DrawLine(pen, p1, new Point(midX, p1.Y));
                g.DrawLine(pen, new Point(midX, p1.Y), new Point(midX, p2.Y));
                g.DrawLine(pen, new Point(midX, p2.Y), p2);
            }

            using (var dot = new SolidBrush(UiTheme.Teal))
            {
                g.FillEllipse(dot, p1.X - 3, p1.Y - 3, 6, 6);
            }
        }

        private void VeChuThich(Graphics g)
        {
            int x = 40;
            int y = _canvas.Height - 40;

            using (var dot = new SolidBrush(UiTheme.Teal))
            {
                g.FillEllipse(dot, x, y, 10, 10);
            }
            using (var font = UiTheme.FontBody(9f))
            using (var brush = new SolidBrush(UiTheme.InkSoft))
            {
                g.DrawString("Đường khoá ngoại (con → cha)", font, brush, x + 18, y - 3);
                g.DrawString("🔑 Khoá chính", font, brush, x + 230, y - 3);
                g.DrawString("🔗 Khoá ngoại", font, brush, x + 350, y - 3);
            }
        }
    }
}
