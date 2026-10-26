using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lab06_ThreeLayer.GUI.Helpers
{
    /// <summary>
    /// Lớp cơ sở cho mọi Form của Lab 6: đặt font/tỉ lệ hiển thị thống nhất,
    /// tô nền gradient ở vùng client và hỗ trợ chụp ảnh báo cáo.
    /// </summary>
    public class FormBase : Form
    {
        /// <summary>Nếu != Color.Empty thì Form tự tô nền gradient (vùng client).</summary>
        public Color GradientFrom { get; set; }
        public Color GradientTo { get; set; }
        public float GradientAngle { get; set; }

        /// <summary>Màu sọc trang trí dưới thanh tiêu đề (child form).</summary>
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

        /// <summary>
        /// Render toàn bộ Form (không hiện lên màn hình) ra Bitmap -
        /// dùng cho trình "Tự chụp ảnh báo cáo" của ứng dụng.
        /// </summary>
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
