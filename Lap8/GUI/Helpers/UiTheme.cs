using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Lab08_SqlCommand_Parameters.GUI.Helpers
{
    /// <summary>
    /// Bang mau, font va tien ich ve giao dien dung chung (giu dong bo giao dien voi Lab 6).
    /// .NET Framework 4.8 khong co "Fluent Design" nen cac ham o day tu ve gradient,
    /// bo goc (gia lap bang Region) va do bong. Chi dung System.Drawing + WinForms, khong NuGet.
    /// </summary>
    public static class UiTheme
    {
        // ----------------------------------------------------------------
        // Mau nen tang
        // ----------------------------------------------------------------
        public static readonly Color Ink = Color.FromArgb(30, 34, 48);
        public static readonly Color InkSoft = Color.FromArgb(92, 101, 124);
        public static readonly Color Muted = Color.FromArgb(140, 148, 168);
        public static readonly Color Canvas = Color.FromArgb(244, 246, 251);
        public static readonly Color Card = Color.White;
        public static readonly Color Border = Color.FromArgb(224, 228, 238);
        public static readonly Color HeaderDeep = Color.FromArgb(28, 22, 74);
        public static readonly Color HeaderMid = Color.FromArgb(59, 42, 156);

        // Mau nhan
        public static readonly Color Violet = Color.FromArgb(124, 77, 255);
        public static readonly Color Indigo = Color.FromArgb(92, 107, 192);
        public static readonly Color Blue = Color.FromArgb(37, 99, 235);
        public static readonly Color Teal = Color.FromArgb(13, 148, 136);
        public static readonly Color Green = Color.FromArgb(22, 163, 74);
        public static readonly Color Amber = Color.FromArgb(217, 119, 6);
        public static readonly Color Orange = Color.FromArgb(234, 88, 12);
        public static readonly Color Rose = Color.FromArgb(219, 39, 119);
        public static readonly Color Red = Color.FromArgb(220, 38, 38);
        public static readonly Color Slate = Color.FromArgb(100, 116, 139);

        // ----------------------------------------------------------------
        // Font (Segoe UI co san tren Windows 10/11, ho tro tieng Viet)
        // ----------------------------------------------------------------
        public static Font FontTitle(float size) { return new Font("Segoe UI", size, FontStyle.Bold, GraphicsUnit.Point); }
        public static Font FontBody(float size) { return new Font("Segoe UI", size, FontStyle.Regular, GraphicsUnit.Point); }
        public static Font FontBold(float size) { return new Font("Segoe UI", size, FontStyle.Bold, GraphicsUnit.Point); }
        public static Font FontItalic(float size) { return new Font("Segoe UI", size, FontStyle.Italic, GraphicsUnit.Point); }
        public static Font FontMono(float size) { return new Font("Consolas", size, FontStyle.Regular, GraphicsUnit.Point); }

        public static Font FontIcon(float size)
        {
            return new Font("Segoe MDL2 Assets", size, FontStyle.Regular, GraphicsUnit.Point);
        }

        // ----------------------------------------------------------------
        // Tien ich mau
        // ----------------------------------------------------------------
        public static Color Lighten(Color c, double amount)
        {
            amount = Math.Max(0.0, Math.Min(1.0, amount));
            return Color.FromArgb(
                (int)(c.R + (255 - c.R) * amount),
                (int)(c.G + (255 - c.G) * amount),
                (int)(c.B + (255 - c.B) * amount));
        }

        public static Color Darken(Color c, double amount)
        {
            amount = Math.Max(0.0, Math.Min(1.0, amount));
            return Color.FromArgb(
                (int)(c.R * (1 - amount)),
                (int)(c.G * (1 - amount)),
                (int)(c.B * (1 - amount)));
        }

        public static Color Blend(Color a, Color b, double t)
        {
            t = Math.Max(0.0, Math.Min(1.0, t));
            return Color.FromArgb(
                (int)(a.R + (b.R - a.R) * t),
                (int)(a.G + (b.G - a.G) * t),
                (int)(a.B + (b.B - a.B) * t));
        }

        public static Color Tint(Color c, double amount)
        {
            return Blend(Color.White, c, amount);
        }

        // ----------------------------------------------------------------
        // Ve
        // ----------------------------------------------------------------
        public static void FillGradient(Graphics g, Rectangle bounds, Color from, Color to, float angle)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            int size = (int)(Math.Max(bounds.Width, bounds.Height) * 1.5);
            Rectangle big = new Rectangle(
                bounds.X + bounds.Width / 2 - size / 2,
                bounds.Y + bounds.Height / 2 - size / 2, size, size);

            using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(big, from, to, angle))
            {
                g.FillRectangle(brush, bounds);
            }
        }

        public static void DrawCard(Graphics g, Rectangle bounds, Color fill, Color border)
        {
            SmoothingOn(g);
            using (var path = RoundedPath(bounds, 10))
            using (var brush = new SolidBrush(fill))
            using (var pen = new Pen(border))
            {
                g.FillPath(brush, path);
                g.DrawPath(pen, path);
            }
        }

        public static void SmoothingOn(Graphics g)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        }

        public static System.Drawing.Drawing2D.GraphicsPath RoundedPath(Rectangle bounds, int radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            int r = Math.Max(0, Math.Min(radius, Math.Min(bounds.Width, bounds.Height) / 2));
            if (r == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            int d = r * 2;
            Rectangle arc = new Rectangle(bounds.Location, new Size(d, d));
            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - d;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - d;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }

        public static void RoundControl(Control control, int radius)
        {
            if (control == null || control.Width <= 0 || control.Height <= 0) return;
            using (var path = RoundedPath(new Rectangle(0, 0, control.Width, control.Height), radius))
            {
                Region old = control.Region;
                control.Region = new Region(path);
                if (old != null) old.Dispose();
            }
        }

        public static void DrawBadge(Graphics g, Rectangle bounds, Color baseColor, string text, Font font)
        {
            SmoothingOn(g);
            using (var path = RoundedPath(bounds, bounds.Height / 2))
            using (var brush = new SolidBrush(baseColor))
            {
                g.FillPath(brush, path);
            }

            using (var textBrush = new SolidBrush(Color.White))
            using (var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            {
                g.DrawString(text, font, textBrush, bounds, format);
            }
        }

        public static void DrawPill(Graphics g, Rectangle bounds, Color baseColor, string text, Font font)
        {
            SmoothingOn(g);
            Color back = Tint(baseColor, 0.14);
            using (var path = RoundedPath(bounds, bounds.Height / 2))
            using (var brush = new SolidBrush(back))
            using (var pen = new Pen(Tint(baseColor, 0.45)))
            {
                g.FillPath(brush, path);
                g.DrawPath(pen, path);
            }

            using (var textBrush = new SolidBrush(Darken(baseColor, 0.15)))
            using (var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            {
                g.DrawString(text, font, textBrush, bounds, format);
            }
        }

        public static void DrawSoftShadow(Graphics g, Rectangle bounds, int spread, int alpha)
        {
            for (int i = spread; i >= 1; i--)
            {
                int a = (int)(alpha * (1.0 - (double)i / (spread + 1)) * 0.25);
                if (a <= 0) continue;
                Rectangle r = new Rectangle(bounds.X - i, bounds.Y - i + 2, bounds.Width + i * 2, bounds.Height + i * 2);
                using (var path = RoundedPath(r, 10 + i))
                using (var brush = new SolidBrush(Color.FromArgb(a, 20, 24, 44)))
                {
                    g.FillPath(brush, path);
                }
            }
        }

        // ----------------------------------------------------------------
        // Tien ich chuoi / tep
        // ----------------------------------------------------------------
        public static string Fit(string text, int max)
        {
            if (text == null) return string.Empty;
            return text.Length <= max ? text : text.Substring(0, Math.Max(0, max - 1)) + "…";
        }

        public static string ReadAllTextSafe(string path)
        {
            try
            {
                return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static void TryDelete(string path)
        {
            try
            {
                if (File.Exists(path)) File.Delete(path);
            }
            catch (Exception) { }
        }

        public static List<string> SplitList(string text)
        {
            var list = new List<string>();
            if (string.IsNullOrEmpty(text)) return list;
            foreach (string s in text.Split(','))
            {
                string t = s.Trim();
                if (t.Length > 0) list.Add(t);
            }
            return list;
        }
    }
}
