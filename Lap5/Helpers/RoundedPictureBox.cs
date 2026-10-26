using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Lab05_MultiForm.Helpers
{
    /// <summary>
    /// PictureBox bo góc + khung viền nhẹ. Không thể "bo góc" ảnh trực tiếp nên
    /// ta vẽ ảnh vào một GraphicsPath bo tròn rồi vẽ viền.
    /// </summary>
    public class RoundedPictureBox : Control
    {
        private Image _image;
        public int CornerRadius { get; set; }
        public Color BorderColor { get; set; }
        public Color BackFill { get; set; }

        /// <summary>Góc neo ảnh: "Center" | "Left" | "Top" (dùng cho banner minh hoạ).</summary>
        public string Align { get; set; }

        public Image Image
        {
            get { return _image; }
            set
            {
                if (_image != null && _image != value && !(_image is Bitmap && ((Bitmap)_image).Tag is bool))
                {
                    // Không dispose ảnh được chia sẻ giữa nhiều control; chỉ bỏ tham chiếu.
                }
                _image = value;
                Invalidate();
            }
        }

        public RoundedPictureBox()
        {
            CornerRadius = 12;
            BorderColor = UiTheme.Border;
            BackFill = Color.FromArgb(244, 246, 251);
            Align = "Center";
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            // Không đặt BackColor = Transparent: control tự vẽ nền trong OnPaint,
            // còn BackColor trong suốt sẽ ném ArgumentException khi cha không hỗ trợ.
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            UiTheme.SmoothingOn(g);

            Rectangle r = new Rectangle(0, 0, Width - 1, Height - 1);
            using (GraphicsPath path = UiTheme.RoundedPath(r, CornerRadius))
            using (var fill = new SolidBrush(BackFill))
            using (var pen = new Pen(BorderColor))
            {
                g.FillPath(fill, path);
                g.DrawPath(pen, path);

                if (_image != null)
                {
                    GraphicsState state = g.Save();
                    g.SetClip(path, CombineMode.Intersect);
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    Rectangle dest;
                    if (Align == "Left")
                    {
                        // Ảnh nằm bên trái, cắt theo chiều cao (giống thumbnail)
                        int w = (int)Math.Round((double)_image.Width * Height / _image.Height);
                        dest = new Rectangle(r.X, r.Y, (int)(r.Width * 0.62), r.Height);
                        int h = r.Height;
                        int wKeep = (int)Math.Round((double)_image.Width * h / _image.Height);
                        dest = new Rectangle(r.X, r.Y, Math.Min(r.Width, wKeep), h);
                        g.DrawImage(_image, dest);
                    }
                    else if (Align == "Fill")
                    {
                        // Lấp đầy vùng (banner)
                        double scale = Math.Max((double)r.Width / _image.Width, (double)r.Height / _image.Height);
                        int w = (int)(_image.Width * scale);
                        int h = (int)(_image.Height * scale);
                        dest = new Rectangle(r.X + (r.Width - w) / 2, r.Y + (r.Height - h) / 2, w, h);
                        g.DrawImage(_image, dest);
                    }
                    else
                    {
                        double scale = Math.Min((double)r.Width / _image.Width, (double)r.Height / _image.Height);
                        int w = (int)(_image.Width * scale);
                        int h = (int)(_image.Height * scale);
                        dest = new Rectangle(r.X + (r.Width - w) / 2, r.Y + (r.Height - h) / 2, w, h);
                        g.DrawImage(_image, dest);
                    }

                    g.Restore(state);
                }
                else
                {
                    using (var brush = new SolidBrush(Color.FromArgb(120, 128, 150)))
                    using (var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    using (var font = UiTheme.FontBody(8.5f))
                    {
                        g.DrawString("Chưa có ảnh", font, brush, r, fmt);
                    }
                }
            }
        }
    }
}
