using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyNhaThuoc.BUS;
using QuanLyNhaThuoc.DTO;
using QuanLyNhaThuoc.GUI.Helpers;

namespace QuanLyNhaThuoc.GUI
{
    /// <summary>Trang tong quan (dashboard): 4 the chi so + bang canh bao kho.</summary>
    public class frmTongQuan : ManHinh, ILamMoi
    {
        private readonly ThongKeBUS _bus = new ThongKeBUS();
        private readonly Label[] _so = new Label[4];
        private readonly Label[] _phu = new Label[4];
        private readonly DataGridView _luoiCanhBao = Ui.Luoi("luoiCanhBao");

        public frmTongQuan() : base("Bảng điều khiển", "Tình hình bán hàng hôm nay và các thuốc cần chú ý trong kho", UiTheme.Primary, new Size(1060, 700))
        {
            // Trang nay dung the rieng thay cho khung trang mot khoi.
            Control vien = NoiDung.Parent;
            vien.BackColor = UiTheme.Canvas;
            vien.Padding = new Padding(0);
            NoiDung.BackColor = UiTheme.Canvas;

            var the = new TableLayoutPanel { Dock = DockStyle.Top, Height = 132, ColumnCount = 4, BackColor = UiTheme.Canvas, Margin = new Padding(0) };
            string[] nhan = { "Doanh thu hôm nay", "Doanh thu tháng này", "Sắp hết hàng", "Sắp hết hạn" };
            Color[] mau = { UiTheme.Primary, UiTheme.Primary, UiTheme.Amber, UiTheme.Accent };
            for (int i = 0; i < 4; i++)
            {
                the.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
                var o = new Panel { Dock = DockStyle.Fill, Margin = new Padding(i == 0 ? 0 : 8, 0, i == 3 ? 0 : 8, 16), BackColor = UiTheme.Border, Padding = new Padding(1) };
                var trong = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(18, 14, 12, 10) };
                var vach = new Panel { Dock = DockStyle.Left, Width = 3, BackColor = mau[i] };
                _phu[i] = new Label { Dock = DockStyle.Bottom, Height = 22, Font = UiTheme.FontBody(8.75f), ForeColor = UiTheme.Muted };
                _so[i] = new Label { Dock = DockStyle.Fill, Text = "—", Font = UiTheme.FontBold(20f), ForeColor = UiTheme.Ink, TextAlign = ContentAlignment.MiddleLeft };
                var t = new Label { Dock = DockStyle.Top, Height = 22, Text = nhan[i], Font = UiTheme.FontBody(9.25f), ForeColor = UiTheme.Muted };
                trong.Controls.Add(_so[i]);
                trong.Controls.Add(_phu[i]);
                trong.Controls.Add(t);
                o.Controls.Add(trong);
                o.Controls.Add(vach);
                the.Controls.Add(o, i, 0);
            }

            var khung = new Panel { Dock = DockStyle.Fill, BackColor = UiTheme.Border, Padding = new Padding(1) };
            var trongKhung = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            trongKhung.Controls.Add(_luoiCanhBao);
            trongKhung.Controls.Add(Ui.TieuDeKhung("Cảnh báo kho: tồn ≤ " + ThongKeBUS.NguongTon + " hoặc hạn dùng ≤ " + ThongKeBUS.SoNgayCanhBao + " ngày"));
            khung.Controls.Add(trongKhung);
            _luoiCanhBao.CellFormatting += ToMau;

            NoiDung.Controls.Add(khung);
            NoiDung.Controls.Add(the);
            Load += (s, e) => LamMoi();
        }

        private void ToMau(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || _luoiCanhBao.Columns[e.ColumnIndex].DataPropertyName != "CanhBao") return;
            string v = Convert.ToString(e.Value);
            e.CellStyle.ForeColor = v.Contains("hạn") ? UiTheme.Accent : UiTheme.Amber;
            e.CellStyle.Font = UiTheme.FontBold(9.25f);
        }

        public void LamMoi()
        {
            try
            {
                ThongKeTongQuan tk = _bus.TongQuan();
                _so[0].Text = tk.DoanhThuHomNay.ToString("N0") + " đ";
                _phu[0].Text = tk.SoHoaDonHomNay + " hóa đơn hôm nay";
                _so[1].Text = tk.DoanhThuThang.ToString("N0") + " đ";
                _phu[1].Text = "Từ ngày 01/" + DateTime.Today.ToString("MM/yyyy");
                _so[2].Text = tk.SoSapHetHang.ToString();
                _phu[2].Text = "trên " + tk.SoThuoc + " thuốc đang kinh doanh";
                _so[3].Text = tk.SoSapHetHan.ToString();
                _phu[3].Text = "trong " + ThongKeBUS.SoNgayCanhBao + " ngày tới";
                Ui.GanDuLieu(_luoiCanhBao, _bus.CanhBao());
            }
            catch (Exception ex) { Ui.BaoLoi(this, ex, "tải tổng quan"); }
        }
    }
}
