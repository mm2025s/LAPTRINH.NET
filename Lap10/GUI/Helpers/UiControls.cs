using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Lab10_DisconnectedData.GUI.Helpers
{
    /// <summary>
    /// UI dung chung cho moi Form cua Lab 8 (giu dong bo voi Lab 7):
    /// - GradientPanel : panel nen gradient
    /// - CardPanel     : panel trang bo goc + vien
    /// - AccentButton  : nut bo goc, doi mau khi hover / nhan
    /// - BadgeLabel    : nhan dang "vien thuoc" (pill) hien thi trang thai
    /// - InfoStrip     : dai thong tin dau Form (tieu de + phu de)
    /// - UiFactory     : tao nhanh TextBox / ComboBox / Label dong bo theme
    /// </summary>
    public class GradientPanel : Panel
    {
        public Color ColorFrom { get; set; }
        public Color ColorTo { get; set; }
        public float Angle { get; set; }

        public GradientPanel()
        {
            ColorFrom = UiTheme.HeaderDeep;
            ColorTo = UiTheme.HeaderMid;
            Angle = 135f;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            DoubleBuffered = true;
            BackColor = ColorFrom;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            UiTheme.FillGradient(e.Graphics, ClientRectangle, ColorFrom, ColorTo, Angle);
        }
    }

    public class CardPanel : Panel
    {
        public int CornerRadius { get; set; }
        public Color BorderColor { get; set; }
        public Color FillColor { get; set; }

        public CardPanel()
        {
            CornerRadius = 12;
            BorderColor = UiTheme.Border;
            FillColor = UiTheme.Card;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            DoubleBuffered = true;
            BackColor = Color.Transparent;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            Rectangle r = new Rectangle(0, 0, Width - 1, Height - 1);
            UiTheme.SmoothingOn(e.Graphics);
            using (GraphicsPath path = UiTheme.RoundedPath(r, CornerRadius))
            using (var brush = new SolidBrush(FillColor))
            using (var pen = new Pen(BorderColor))
            {
                e.Graphics.FillPath(brush, path);
                e.Graphics.DrawPath(pen, path);
            }
        }
    }

    public class AccentButton : Button
    {
        public Color BaseColor { get; set; }
        public int CornerRadius { get; set; }
        public bool OutlineOnly { get; set; }

        private bool _hover;
        private bool _down;

        public AccentButton()
        {
            BaseColor = UiTheme.Violet;
            CornerRadius = 9;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.MouseOverBackColor = Color.Transparent;
            FlatAppearance.MouseDownBackColor = Color.Transparent;
            BackColor = Color.Transparent;
            ForeColor = Color.White;
            Font = UiTheme.FontBold(9.75f);
            Cursor = Cursors.Hand;
            UseVisualStyleBackColor = false;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            TabStop = true;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            UiTheme.SmoothingOn(g);

            Rectangle r = new Rectangle(0, 0, Width - 1, Height - 1);
            bool enabled = Enabled;

            Color top;
            Color bottom;
            if (OutlineOnly)
            {
                top = _down ? UiTheme.Tint(BaseColor, 0.25) : (_hover ? UiTheme.Tint(BaseColor, 0.12) : Color.White);
                bottom = top;
            }
            else
            {
                top = UiTheme.Lighten(BaseColor, _hover ? 0.16 : 0.06);
                bottom = BaseColor;
                if (_down) { Color t = top; top = bottom; bottom = t; }
            }

            if (!enabled)
            {
                top = UiTheme.Blend(top, Color.FromArgb(210, 214, 224), 0.75);
                bottom = UiTheme.Blend(bottom, Color.FromArgb(210, 214, 224), 0.75);
            }

            using (GraphicsPath path = UiTheme.RoundedPath(r, CornerRadius))
            {
                if (!OutlineOnly)
                {
                    using (var brush = new LinearGradientBrush(new Rectangle(0, 0, Width, Math.Max(1, Height)), top, bottom, LinearGradientMode.Vertical))
                    {
                        g.FillPath(brush, path);
                    }
                }
                else
                {
                    using (var brush = new SolidBrush(top)) g.FillPath(brush, path);
                    using (var pen = new Pen(UiTheme.Tint(BaseColor, 0.55))) g.DrawPath(pen, path);
                }

                if (Focused && ShowFocusCues && enabled)
                {
                    Rectangle inner = Rectangle.Inflate(r, -3, -3);
                    using (GraphicsPath p2 = UiTheme.RoundedPath(inner, Math.Max(0, CornerRadius - 2)))
                    using (var pen = new Pen(Color.FromArgb(120, OutlineOnly ? BaseColor : Color.White), 1.4f))
                    {
                        pen.DashStyle = DashStyle.Dot;
                        g.DrawPath(pen, p2);
                    }
                }
            }

            Color textColor = !enabled ? Color.FromArgb(130, 138, 156) : (OutlineOnly ? UiTheme.Darken(BaseColor, 0.12) : Color.White);

            TextRenderer.DrawText(g, Text, Font, r, textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _hover = false; _down = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs mevent) { _down = true; Invalidate(); base.OnMouseDown(mevent); }
        protected override void OnMouseUp(MouseEventArgs mevent) { _down = false; Invalidate(); base.OnMouseUp(mevent); }
        protected override void OnGotFocus(EventArgs e) { Invalidate(); base.OnGotFocus(e); }
        protected override void OnLostFocus(EventArgs e) { Invalidate(); base.OnLostFocus(e); }
        protected override void OnEnabledChanged(EventArgs e) { Invalidate(); base.OnEnabledChanged(e); }
        protected override void OnResize(EventArgs e) { base.OnResize(e); Invalidate(); }
    }

    public class BadgeLabel : Control
    {
        public Color BaseColor { get; set; }
        private string _value = string.Empty;

        public string Value
        {
            get { return _value; }
            set { _value = value ?? string.Empty; Invalidate(); }
        }

        public BadgeLabel()
        {
            BaseColor = UiTheme.Indigo;
            Font = UiTheme.FontBold(8.5f);
            Size = new Size(120, 24);
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            UiTheme.DrawPill(e.Graphics, new Rectangle(0, 0, Width - 1, Height - 1), BaseColor, _value, Font);
        }

        public void SetColor(Color color)
        {
            BaseColor = color;
            Invalidate();
        }
    }

    public class InfoStrip : Control
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string RightTop { get; set; }
        public string RightBottom { get; set; }
        public Color Accent { get; set; }
        public string ChipText { get; set; }

        public InfoStrip()
        {
            Accent = UiTheme.Violet;
            ChipText = string.Empty;
            Title = string.Empty;
            Subtitle = string.Empty;
            RightTop = string.Empty;
            RightBottom = string.Empty;
            Height = 88;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            UiTheme.SmoothingOn(g);

            Color c1 = Color.FromArgb(26, 24, 62);
            Color c2 = UiTheme.Blend(Accent, Color.FromArgb(20, 18, 50), 0.62);
            UiTheme.FillGradient(g, ClientRectangle, c1, c2, 20f);

            using (var b1 = new SolidBrush(Color.FromArgb(22, 255, 255, 255)))
            {
                g.FillEllipse(b1, Width - 210, -70, 260, 260);
                g.FillEllipse(b1, Width - 150, 30, 120, 120);
            }

            int x = 24;
            if (!string.IsNullOrEmpty(ChipText))
            {
                Rectangle chip = new Rectangle(x, (Height - 44) / 2, 44, 44);
                using (var brush = new SolidBrush(Color.FromArgb(60, 255, 255, 255)))
                    g.FillEllipse(brush, chip);
                using (var pen = new Pen(UiTheme.Tint(Accent, 0.65), 1.6f))
                    g.DrawEllipse(pen, chip);
                using (var font = UiTheme.FontBold(15f))
                using (var brush = new SolidBrush(Color.White))
                using (var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    g.DrawString(ChipText, font, brush, new RectangleF(chip.X, chip.Y, chip.Width, chip.Height), fmt);
                }
                x += 60;
            }

            using (var font = UiTheme.FontTitle(15f))
            {
                Rectangle vungTieuDe = new Rectangle(x, 6, Width - x - 24 - (string.IsNullOrEmpty(RightTop) ? 0 : 150), Height - 26);
                TextRenderer.DrawText(g, Title, font, vungTieuDe, Color.White,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            }

            if (!string.IsNullOrEmpty(Subtitle))
            {
                using (var font = UiTheme.FontBody(8.75f))
                using (var brush = new SolidBrush(Color.FromArgb(205, 208, 235)))
                {
                    g.DrawString(Subtitle, font, brush, x, Height - 32);
                }
            }

            int right = Width - 24;
            if (!string.IsNullOrEmpty(RightTop))
            {
                using (var font = UiTheme.FontBold(9f))
                using (var brush = new SolidBrush(UiTheme.Tint(Accent, 0.75)))
                using (var fmt = new StringFormat { Alignment = StringAlignment.Far })
                {
                    g.DrawString(RightTop, font, brush, new RectangleF(0, 16, right, 22), fmt);
                }
            }
            if (!string.IsNullOrEmpty(RightBottom))
            {
                using (var font = UiTheme.FontBody(8.5f))
                using (var brush = new SolidBrush(Color.FromArgb(180, 185, 215)))
                using (var fmt = new StringFormat { Alignment = StringAlignment.Far })
                {
                    g.DrawString(RightBottom, font, brush, new RectangleF(0, Height - 34, right, 22), fmt);
                }
            }

            using (var pen = new Pen(Color.FromArgb(40, 255, 255, 255)))
            {
                g.DrawLine(pen, 0, Height - 1, Width, Height - 1);
            }
        }
    }

    /// <summary>Factory tao control dong bo theme.</summary>
    public static class UiFactory
    {
        public static Label Label(string text, float size, Color color, bool bold)
        {
            return new Label
            {
                Text = text,
                AutoSize = false,
                Height = (int)Math.Ceiling(size * 2.1f),
                Font = bold ? UiTheme.FontBold(size) : UiTheme.FontBody(size),
                ForeColor = color,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };
        }

        public static Label FieldLabel(string text)
        {
            Label l = Label(text.ToUpperInvariant(), 7.75f, UiTheme.Muted, true);
            l.Height = 20;
            l.ForeColor = Color.FromArgb(120, 128, 148);
            return l;
        }

        public static TextBox TextBox(string name, string placeholder, bool readOnly)
        {
            var tb = new TextBox
            {
                Name = name,
                Font = UiTheme.FontBody(10.5f),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = readOnly ? Color.FromArgb(246, 247, 251) : Color.White,
                ReadOnly = readOnly,
                Height = 30,
                Tag = placeholder
            };
            if (!string.IsNullOrEmpty(placeholder))
            {
                tb.Text = placeholder;
                tb.ForeColor = SystemColors.GrayText;
                tb.GotFocus += (s, e) => { if (IsPlaceholder(tb)) { tb.Text = ""; tb.ForeColor = SystemColors.WindowText; } };
                tb.LostFocus += (s, e) => { if (tb.Text.Length == 0) { tb.Text = placeholder; tb.ForeColor = SystemColors.GrayText; } };
            }
            return tb;
        }

        private static bool IsPlaceholder(TextBox tb)
        {
            return tb.Tag is string && tb.ForeColor == SystemColors.GrayText;
        }

        public static string Val(TextBox tb)
        {
            if (tb == null) return string.Empty;
            return IsPlaceholder(tb) ? string.Empty : tb.Text.Trim();
        }

        public static void SetVal(TextBox tb, string value)
        {
            if (tb == null) return;
            tb.Text = value ?? string.Empty;
            tb.ForeColor = SystemColors.WindowText;
        }

        public static ComboBox ComboBox(string name)
        {
            return new ComboBox
            {
                Name = name,
                Font = UiTheme.FontBody(10f),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                Height = 28,
                BackColor = Color.White
            };
        }

        public static Label Caption(string text)
        {
            return Label(text, 8.25f, UiTheme.Muted, false);
        }

        public static Label Value(string text)
        {
            return Label(text, 9.5f, UiTheme.Ink, true);
        }
    }
}
