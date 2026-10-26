using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lab06_ThreeLayer.GUI.Helpers
{
    /// <summary>
    /// Hộp thoại nhập một dòng văn bản (thay cho Microsoft.VisualBasic.Interaction.InputBox
    /// để project không cần tham chiếu Microsoft.VisualBasic).
    /// </summary>
    public static class NhapLieu
    {
        /// <summary>Trả về chuỗi đã Trim, hoặc null nếu người dùng bấm Hủy/Esc.</summary>
        public static string HoiChuoi(IWin32Window owner, string cauHoi, string tieuDe)
        {
            using (var hopThoai = new Form())
            {
                hopThoai.Text = tieuDe;
                hopThoai.FormBorderStyle = FormBorderStyle.FixedDialog;
                hopThoai.StartPosition = FormStartPosition.CenterParent;
                hopThoai.MinimizeBox = false;
                hopThoai.MaximizeBox = false;
                hopThoai.ShowIcon = false;
                hopThoai.ShowInTaskbar = false;
                hopThoai.ClientSize = new Size(400, 132);
                hopThoai.BackColor = UiTheme.Canvas;
                hopThoai.Font = UiTheme.FontBody(9.5f);

                Label lbl = new Label();
                lbl.Text = cauHoi;
                lbl.AutoSize = false;
                lbl.BackColor = Color.Transparent;
                lbl.SetBounds(16, 14, 368, 34);

                TextBox txt = new TextBox();
                txt.Name = "txtGiaTri";
                txt.Font = UiTheme.FontBody(10.5f);
                txt.BorderStyle = BorderStyle.FixedSingle;
                txt.SetBounds(16, 54, 368, 26);

                Button ok = new Button();
                ok.Name = "btnDongY";
                ok.Text = "Đồng ý";
                ok.DialogResult = DialogResult.OK;
                ok.SetBounds(214, 92, 80, 28);

                Button huy = new Button();
                huy.Name = "btnHuy";
                huy.Text = "Hủy";
                huy.DialogResult = DialogResult.Cancel;
                huy.SetBounds(304, 92, 80, 28);

                hopThoai.Controls.AddRange(new Control[] { lbl, txt, ok, huy });
                hopThoai.AcceptButton = ok;
                hopThoai.CancelButton = huy;

                if (hopThoai.ShowDialog(owner) != DialogResult.OK) return null;

                string kq = txt.Text.Trim();
                return kq.Length == 0 ? null : kq;
            }
        }

        /// <summary>Hộp thoại chỉ có nút OK - dùng để báo lỗi nghiệp vụ bằng chữ.</summary>
        public static void ThongBao(IWin32Window owner, string text, string tieuDe)
        {
            MessageBox.Show(owner, text, tieuDe, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
