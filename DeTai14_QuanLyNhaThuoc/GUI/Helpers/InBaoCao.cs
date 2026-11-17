using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using QuanLyNhaThuoc.DTO;

namespace QuanLyNhaThuoc.GUI.Helpers
{
    /// <summary>
    /// Bo in bao cao dung chung cho moi IBaoCao (thay cho Crystal Reports khi may
    /// khong co SAP Crystal Reports runtime): xem truoc bang PrintPreviewDialog,
    /// in ra may in that hoac "Microsoft Print to PDF". Tu dong sang trang.
    /// </summary>
    public static class InBaoCao
    {
        public static void XemTruoc(IWin32Window chu, IBaoCao bc)
        {
            int dong = 0;
            using (var doc = new PrintDocument { DocumentName = bc.TieuDe })
            using (var dlg = new PreviewDialog(doc))
            {
                doc.BeginPrint += (s, e) => dong = 0;
                doc.PrintPage += (s, e) =>
                {
                    dong = VeTrang(e.Graphics, e.MarginBounds, bc, dong);
                    e.HasMorePages = dong < bc.DuLieu.Rows.Count;
                };
                dlg.ShowDialog(chu);
            }
        }

        /// <summary>Ve mot trang bat dau tu dong 'tuDong'; tra ve chi so dong ke tiep chua ve.</summary>
        public static int VeTrang(Graphics g, Rectangle vung, IBaoCao bc, int tuDong)
        {
            using (var fTieuDe = new Font("Segoe UI", 15f, FontStyle.Bold))
            using (var fPhu = new Font("Segoe UI", 9.5f))
            using (var fDau = new Font("Segoe UI", 9.5f, FontStyle.Bold))
            using (var fThan = new Font("Segoe UI", 9.5f))
            using (var vien = new Pen(UiTheme.Border))
            using (var nenDau = new SolidBrush(UiTheme.PrimaryBg))
            using (var chuChinh = new SolidBrush(UiTheme.Primary))
            using (var chuThuong = new SolidBrush(UiTheme.Ink))
            using (var chuNhat = new SolidBrush(UiTheme.Muted))
            {
                float y = vung.Top;
                g.DrawString("NHÀ THUỐC EAUT", fDau, chuChinh, vung.Left, y);
                string inLuc = "In lúc " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                g.DrawString(inLuc, fPhu, chuNhat, vung.Right - g.MeasureString(inLuc, fPhu).Width, y);
                y += 34;
                if (tuDong == 0)
                {
                    SizeF kt = g.MeasureString(bc.TieuDe, fTieuDe);
                    g.DrawString(bc.TieuDe, fTieuDe, chuThuong, vung.Left + (vung.Width - kt.Width) / 2, y);
                    y += kt.Height + 2;
                    SizeF kp = g.MeasureString(bc.PhuDe, fPhu);
                    g.DrawString(bc.PhuDe, fPhu, chuNhat, vung.Left + (vung.Width - kp.Width) / 2, y);
                    y += kp.Height + 16;
                }

                // Do rong cot: cot dau (ten) lay phan con lai, cac cot so chia deu.
                int n = bc.Cot.Count;
                float rongSo = Math.Min(120f, vung.Width * 0.6f / Math.Max(1, n - 1));
                float[] x = new float[n + 1];
                x[0] = vung.Left;
                x[1] = vung.Right - rongSo * (n - 1);
                for (int i = 2; i <= n; i++) x[i] = x[i - 1] + rongSo;
                const float cao = 26f;

                g.FillRectangle(nenDau, vung.Left, y, vung.Width, cao);
                for (int i = 0; i < n; i++) O(g, bc.Cot[i].Value, fDau, chuChinh, x[i], y, x[i + 1] - x[i], cao, i > 0);
                y += cao;

                int r = tuDong;
                for (; r < bc.DuLieu.Rows.Count && y + cao * 2 < vung.Bottom; r++)
                {
                    DataRow d = bc.DuLieu.Rows[r];
                    for (int i = 0; i < n; i++)
                        O(g, DinhDang(d[bc.Cot[i].Key]), fThan, chuThuong, x[i], y, x[i + 1] - x[i], cao, i > 0);
                    y += cao;
                    g.DrawLine(vien, vung.Left, y, vung.Right, y);
                }

                if (r >= bc.DuLieu.Rows.Count)
                {
                    y += 8;
                    SizeF kt = g.MeasureString(bc.DongTong, fDau);
                    g.DrawString(bc.DongTong, fDau, chuChinh, vung.Right - kt.Width, y);
                }
                return r;
            }
        }

        private static void O(Graphics g, string chu, Font f, Brush b, float x, float y, float rong, float cao, bool phai)
        {
            var dd = new StringFormat { LineAlignment = StringAlignment.Center, Alignment = phai ? StringAlignment.Far : StringAlignment.Near, Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap };
            g.DrawString(chu, f, b, new RectangleF(x + 6, y, rong - 12, cao), dd);
        }

        private static string DinhDang(object v)
        {
            if (v == null || v == DBNull.Value) return "";
            if (v is DateTime) return ((DateTime)v).ToString("dd/MM/yyyy");
            if (v is decimal) return ((decimal)v).ToString("N0");
            return Convert.ToString(v);
        }

        /// <summary>Hop thoai xem truoc co nut In (chon may in, ke ca Microsoft Print to PDF).</summary>
        private class PreviewDialog : PrintPreviewDialog
        {
            public PreviewDialog(PrintDocument doc)
            {
                Document = doc;
                Width = 900; Height = 1000;
                StartPosition = FormStartPosition.CenterParent;
                ShowIcon = false;
                Text = "Xem trước khi in - " + doc.DocumentName;
                ((Form)this).WindowState = FormWindowState.Normal;
            }

            protected override void OnShown(EventArgs e)
            {
                base.OnShown(e);
                // Nut In mac dinh in ngay; doi thanh mo hop chon may in.
                foreach (Control c in Controls)
                {
                    var ts = c as ToolStrip;
                    if (ts == null || ts.Items.Count == 0) continue;
                    var nutIn = ts.Items[0] as ToolStripButton;
                    if (nutIn == null) continue;
                    var moi = new ToolStripButton("In...") { DisplayStyle = ToolStripItemDisplayStyle.Text };
                    moi.Click += (s, ev) =>
                    {
                        using (var pd = new PrintDialog { Document = Document, UseEXDialog = true })
                            if (pd.ShowDialog(this) == DialogResult.OK) Document.Print();
                    };
                    ts.Items.Insert(0, moi);
                    nutIn.Visible = false;
                }
            }
        }
    }
}
