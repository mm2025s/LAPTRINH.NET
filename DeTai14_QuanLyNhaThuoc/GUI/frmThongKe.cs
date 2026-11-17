using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QuanLyNhaThuoc.BUS;
using QuanLyNhaThuoc.DTO;
using QuanLyNhaThuoc.GUI.Helpers;

namespace QuanLyNhaThuoc.GUI
{
    /// <summary>Thong ke doanh thu theo ngay (bieu do cot tu ve) va top thuoc ban chay.</summary>
    public class frmThongKe : ManHinh
    {
        private readonly ThongKeBUS _bus = new ThongKeBUS();
        private readonly DateTimePicker _tu = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 130 };
        private readonly DateTimePicker _den = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 130 };
        private readonly Panel _bieuDo = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
        private readonly DataGridView _top = Ui.Luoi("luoiTop");
        private readonly Label _tong = Ui.Nhan("", true);
        private DataTable _doanhThu;

        public frmThongKe() : base("Thống kê doanh thu", "Doanh thu theo ngày · 10 thuốc bán chạy nhất trong khoảng thời gian", UiTheme.Violet, new Size(1200, 720))
        {
            _tu.Value = DateTime.Today.AddDays(-29);
            _bieuDo.Paint += VeBieuDo;
            _bieuDo.Resize += (s, e) => _bieuDo.Invalidate();

            var phai = new Panel { Dock = DockStyle.Right, Width = 470, Padding = new Padding(10, 0, 0, 0) };
            phai.Controls.Add(_top);
            phai.Controls.Add(Ui.TieuDeKhung("Top 10 thuốc bán chạy"));

            var trai = new Panel { Dock = DockStyle.Fill };
            trai.Controls.Add(_bieuDo);
            trai.Controls.Add(Ui.TieuDeKhung("Doanh thu theo ngày (nghìn đồng)"));

            NoiDung.Controls.Add(trai);
            NoiDung.Controls.Add(phai);
            NoiDung.Controls.Add(Ui.ThanhNut(Ui.Nhan("Từ ngày"), _tu, Ui.Nhan("đến"), _den, Ui.Nut("btnXem", "Xem thống kê", UiTheme.Violet, (s, e) => Nap()),
                Ui.Nut("btnIn", "In báo cáo", UiTheme.Violet, (s, e) => In(), true), _tong));
            Load += (s, e) => Nap();
        }

        private void Nap()
        {
            try
            {
                _doanhThu = _bus.DoanhThuTheoNgay(_tu.Value, _den.Value);
                Ui.GanDuLieu(_top, _bus.TopThuoc(_tu.Value, _den.Value));
                decimal tong = _doanhThu.AsEnumerable().Sum(r => r.Field<decimal>("DoanhThu"));
                int soHD = _doanhThu.AsEnumerable().Sum(r => r.Field<int>("SoHoaDon"));
                _tong.Text = "Tổng: " + tong.ToString("N0") + " đ · " + soHD + " hóa đơn";
                _bieuDo.Invalidate();
            }
            catch (Exception ex) { Ui.BaoLoi(this, ex, "thống kê"); }
        }

        public IBaoCao BaoCao() { return new BaoCaoDoanhThu(_tu.Value, _den.Value, _doanhThu ?? _bus.DoanhThuTheoNgay(_tu.Value, _den.Value)); }

        private void In()
        {
            try { InBaoCao.XemTruoc(this, BaoCao()); }
            catch (Exception ex) { Ui.BaoLoi(this, ex, "in báo cáo"); }
        }

        private void VeBieuDo(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            UiTheme.SmoothingOn(g);
            if (_doanhThu == null || _doanhThu.Rows.Count == 0)
            {
                TextRenderer.DrawText(g, "Không có hóa đơn trong khoảng đã chọn.", Font, _bieuDo.ClientRectangle, UiTheme.Muted);
                return;
            }

            Rectangle vung = new Rectangle(64, 20, _bieuDo.Width - 84, _bieuDo.Height - 64);
            decimal max = _doanhThu.AsEnumerable().Max(r => r.Field<decimal>("DoanhThu"));
            max = Math.Max(1, Math.Ceiling(max / 100000m) * 100000m);

            using (var pen = new Pen(UiTheme.Border))
            using (var font = UiTheme.FontBody(8f))
            {
                for (int i = 0; i <= 4; i++)
                {
                    int y = vung.Bottom - vung.Height * i / 4;
                    g.DrawLine(pen, vung.Left, y, vung.Right, y);
                    TextRenderer.DrawText(g, (max * i / 4 / 1000m).ToString("N0"), font, new Rectangle(0, y - 8, 58, 16), UiTheme.Muted, TextFormatFlags.Right);
                }

                int n = _doanhThu.Rows.Count;
                float buoc = vung.Width / (float)n;
                float rong = Math.Max(4f, buoc * 0.62f);
                using (var brush = new SolidBrush(UiTheme.Violet))
                {
                    for (int i = 0; i < n; i++)
                    {
                        DataRow r = _doanhThu.Rows[i];
                        float h = (float)(r.Field<decimal>("DoanhThu") / max) * vung.Height;
                        float x = vung.Left + i * buoc + (buoc - rong) / 2;
                        g.FillRectangle(brush, x, vung.Bottom - h, rong, h);
                        if (n <= 16 || i % (n / 10 + 1) == 0)
                            TextRenderer.DrawText(g, r.Field<DateTime>("Ngay").ToString("dd/MM"), font,
                                new Rectangle((int)(x + rong / 2) - 24, vung.Bottom + 6, 48, 16), UiTheme.InkSoft, TextFormatFlags.HorizontalCenter);
                    }
                }
            }
        }
    }
}
