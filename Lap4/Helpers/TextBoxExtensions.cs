using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lab04_WinFormsBasic.Helpers
{
    /// <summary>
    /// .NET Framework 4.8 không có thuộc tính PlaceholderText cho TextBox (chỉ có trên .NET Core 3.0+).
    /// Lớp tiện ích này mô phỏng PlaceholderText: hiển thị chữ gợi ý mờ khi ô nhập trống.
    /// </summary>
    public static class TextBoxExtensions
    {
        public static void SetPlaceholderText(this TextBox textBox, string placeholder)
        {
            if (textBox == null) return;

            textBox.Tag = placeholder;

            // Designer có thể gán thuộc tính nhiều lần -> gỡ handler cũ trước khi gắn lại
            textBox.GotFocus -= TextBox_GotFocus;
            textBox.LostFocus -= TextBox_LostFocus;
            textBox.TextChanged -= TextBox_TextChanged;

            textBox.GotFocus += TextBox_GotFocus;
            textBox.LostFocus += TextBox_LostFocus;
            textBox.TextChanged += TextBox_TextChanged;

            ShowPlaceholder(textBox);
        }

        private static bool IsShowingPlaceholder(TextBox tb) =>
            tb.Tag is string && tb.ForeColor == SystemColors.GrayText;

        private static void ShowPlaceholder(TextBox tb)
        {
            if (string.IsNullOrEmpty(tb.Text) && tb.Tag is string placeholder)
            {
                tb.Text = placeholder;
                tb.ForeColor = SystemColors.GrayText;
            }
        }

        private static void TextBox_GotFocus(object sender, EventArgs e)
        {
            var tb = (TextBox)sender;
            if (IsShowingPlaceholder(tb))
            {
                tb.Text = string.Empty;
                tb.ForeColor = SystemColors.WindowText;
            }
        }

        private static void TextBox_LostFocus(object sender, EventArgs e)
        {
            var tb = (TextBox)sender;
            if (string.IsNullOrEmpty(tb.Text))
            {
                ShowPlaceholder(tb);
            }
        }

        private static void TextBox_TextChanged(object sender, EventArgs e)
        {
            var tb = (TextBox)sender;
            if (IsShowingPlaceholder(tb) && tb.Text != (string)tb.Tag)
            {
                tb.ForeColor = SystemColors.WindowText;
            }
        }

        /// <summary>
        /// Lấy giá trị thực của ô nhập (bỏ qua chữ placeholder).
        /// </summary>
        public static string GetRealText(this TextBox tb)
        {
            if (tb == null) return string.Empty;
            return IsShowingPlaceholder(tb) ? string.Empty : tb.Text;
        }

        /// <summary>
        /// Gán PlaceholderText cho nhiều TextBox cùng lúc. Dùng trong Designer thay cho thuộc tính PlaceholderText.
        /// </summary>
        public static void SetPlaceholders(this Control owner, params object[] pairs)
        {
            if (owner == null) return;

            for (int i = 0; i + 1 < pairs.Length; i += 2)
            {
                var tb = pairs[i] as TextBox;
                var text = pairs[i + 1] as string;
                if (tb != null && text != null)
                {
                    tb.SetPlaceholderText(text);
                }
            }
        }
    }
}
