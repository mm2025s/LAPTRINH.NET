using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lab07_ADONET_Connection.GUI.Helpers
{
    /// <summary>
    /// Lop co so cho moi Form cua Lab 7: dat font/ti le hien thi thong nhat,
    /// to nen gradient o vung client va ho tro chup anh bao cao.
    /// </summary>
    public class FormBase : Form
    {
        /// <summary>Neu != Color.Empty thi Form tu to nen gradient (vung client).</summary>
        public Color GradientFrom { get; set; }
        public Color GradientTo { get; set; }
        public float GradientAngle { get; set; }

        /// <summary>Mau soc trang tri duoi thanh tieu de (child form).</summary>
        public Color AccentWash { get; set; }

        public FormBase()
        {
            Font = UiTheme.FontBody(9f);
            AutoScaleMode = AutoScaleMode.None;
            BackColor = UiTheme.Canvas;
            StartPosition = FormStartPosition.CenterParent;
            DoubleBuffered = true;
            GradientFrom = Color.Empty;
            GradientTo = Color.Empty;
            GradientAngle = 120f;
            AccentWash = Color.Empty;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (GradientFrom == Color.Empty)
            {
                base.OnPaintBackground(e);
                return;
            }

            UiTheme.FillGradient(e.Graphics, e.ClipRectangle, GradientFrom, GradientTo, GradientAngle);

            if (AccentWash != Color.Empty && e.ClipRectangle.IntersectsWith(new Rectangle(0, 0, ClientSize.Width, 4)))
            {
                using (var brush = new SolidBrush(AccentWash))
                {
                    e.Graphics.FillRectangle(brush, new Rectangle(0, 0, ClientSize.Width, 4));
                }
            }
        }

        /// <summary>Render toan bo Form ra Bitmap - dung cho trinh tu chup anh bao cao.</summary>
        public Bitmap CaptureToBitmap()
        {
            int w = Math.Max(1, Width);
            int h = Math.Max(1, Height);
            var bmp = new Bitmap(w, h);
            DrawToBitmap(bmp, new Rectangle(0, 0, w, h));
            return bmp;
        }
    }
}
