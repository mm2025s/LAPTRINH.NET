using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Lab09_DataReaderSearch.BUS;
using Lab09_DataReaderSearch.GUI.Helpers;

namespace Lab09_DataReaderSearch.GUI
{
    /// <summary>
    /// BAI 9 - Dinh dang DataGridView (bai tu thuc hien).
    ///
    /// Bon viec phai lam:
    ///   1. Dinh dang NGAY  : dd/MM/yyyy thay vi MM/dd/yyyy hh:mm:ss cua .NET.
    ///   2. Dinh dang DIEM  : 2 chu so thap phan, canh phai de so thang hang.
    ///   3. Tieu de cot      : tieng Viet co dau, thu tu cot hop ly.
    ///   4. To mau dong      : do vang khi DiemTB &lt; 5, xanh la khi DiemTB &gt;= 8.
    ///
    /// Phan 1-3 nam trong lop LuoiSinhVien (CellFormatting + DatLaiTieuDe),
    /// phan 4 nam trong OnRowPrePaint. Form nay chi bat/tat va chinh nguong
    /// de nguoi hoc thay ro nguong da thay doi thi mau doi theo.
    /// </summary>
    public class frmBai9 : BaiFormBase
    {
        private readonly SinhVienBUS _bus = new SinhVienBUS();

        private LuoiSinhVien _luoi;
        private CheckBox _chkToMau;
        private NumericUpDown _numThap;
        private NumericUpDown _numCao;
        private Label _lblThongKe;

        public frmBai9()
            : base("9", "Định dạng DataGridView",
                   "Ngày dd/MM/yyyy, điểm 2 chữ số thập phân, tô màu dưới 5 và từ 8 trở lên",
                   UiTheme.Green, "CellFormatting", "RowPrePaint", "frmBai9",
                   new Size(1050, 720))
        {
            Badge = new BadgeLabel();
            Badge.Value = "định dạng + tô màu";
            Badge.BaseColor = UiTheme.Green;
            Badge.SetBounds(NoiDung.Right - 190, 20, 170, 24);

            TaoDieuKhien();
            TaoLuoi();
        }

        private void TaoDieuKhien()
        {
            CardPanel the = The(18, 18, 1002, 128);
            NoiDung.Controls.Add(the);

            Label tieuDe = TieuDeMuc("TÙY CHỌN ĐỊNH DẠNG", UiTheme.Green);
            tieuDe.SetBounds(18, 14, 400, 22);
            the.Controls.Add(tieuDe);

            _chkToMau = new CheckBox();
            _chkToMau.Name = "chkToMau";
            _chkToMau.Text = "Tô màu theo điểm";
            _chkToMau.Checked = true;
            _chkToMau.Font = UiTheme.FontBody(9.5f);
            _chkToMau.ForeColor = UiTheme.Ink;
            _chkToMau.SetBounds(18, 48, 170, 24);
            _chkToMau.CheckedChanged += (s, e) =>
            {
                if (_luoi != null)
                {
                    _luoi.BatToMau = _chkToMau.Checked;
                    _luoi.Invalidate();
                }
            };
            the.Controls.Add(_chkToMau);

            Label nhanThap = NhanTruong("Ngưỡng đỏ (nhỏ hơn)");
            nhanThap.SetBounds(200, 44, 170, 18);
            the.Controls.Add(nhanThap);

            _numThap = new NumericUpDown();
            _numThap.Name = "numThap";
            _numThap.DecimalPlaces = 1;
            _numThap.Increment = 0.5m;
            _numThap.Minimum = 0m;
            _numThap.Maximum = 10m;
            _numThap.Value = 5m;
            _numThap.Font = UiTheme.FontBody(10f);
            _numThap.SetBounds(200, 64, 90, 26);
            _numThap.ValueChanged += (s, e) => CapNhatNguong();
            the.Controls.Add(_numThap);

            Label nhanCao = NhanTruong("Ngưỡng xanh (từ)");
            nhanCao.SetBounds(310, 44, 170, 18);
            the.Controls.Add(nhanCao);

            _numCao = new NumericUpDown();
            _numCao.Name = "numCao";
            _numCao.DecimalPlaces = 1;
            _numCao.Increment = 0.5m;
            _numCao.Minimum = 0m;
            _numCao.Maximum = 10m;
            _numCao.Value = 8m;
            _numCao.Font = UiTheme.FontBody(10f);
            _numCao.SetBounds(310, 64, 90, 26);
            _numCao.ValueChanged += (s, e) => CapNhatNguong();
            the.Controls.Add(_numCao);

            // Chu giai mau
            Panel oDo = new Panel();
            oDo.SetBounds(430, 66, 14, 14);
            oDo.BackColor = Color.FromArgb(255, 205, 212);
            the.Controls.Add(oDo);

            Label lblDo = UiFactory.Label("Điểm dưới ngưỡng đỏ", 8.75f, UiTheme.InkSoft, false);
            lblDo.SetBounds(450, 62, 150, 20);
            the.Controls.Add(lblDo);

            Panel oXanh = new Panel();
            oXanh.SetBounds(610, 66, 14, 14);
            oXanh.BackColor = Color.FromArgb(196, 238, 214);
            the.Controls.Add(oXanh);

            Label lblXanh = UiFactory.Label("Điểm từ ngưỡng xanh", 8.75f, UiTheme.InkSoft, false);
            lblXanh.SetBounds(630, 62, 160, 20);
            the.Controls.Add(lblXanh);

            _lblThongKe = UiFactory.Label("", 8.5f, UiTheme.Muted, false);
            _lblThongKe.SetBounds(18, 96, 966, 20);
            the.Controls.Add(_lblThongKe);
        }

        private void TaoLuoi()
        {
            CardPanel the = The(18, 156, 1002, 450);
            the.Padding = new Padding(1);
            NoiDung.Controls.Add(the);

            _luoi = new LuoiSinhVien();
            _luoi.Dock = DockStyle.Fill;
            the.Controls.Add(_luoi);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Nap();
        }

        private void CapNhatNguong()
        {
            if (_luoi == null) return;

            _luoi.NguongThap = _numThap.Value;
            _luoi.NguongCao = _numCao.Value;
            _luoi.Invalidate();

            ThongKe();
        }

        private void Nap()
        {
            try
            {
                DataTable table = _bus.LoadStudents();
                _luoi.GanDuLieu(table);

                _luoi.NguongThap = _numThap.Value;
                _luoi.NguongCao = _numCao.Value;
                _luoi.Invalidate();

                ThongKe();
                KetQua("Đã nạp " + table.Rows.Count + " sinh viên với định dạng ngày/điểm và tô màu theo ngưỡng.", true);
            }
            catch (Exception ex) { LoiChung(ex); }
        }

        /// <summary>Dem so dong theo tung nhom mau de doi chieu bang mat thuong.</summary>
        private void ThongKe()
        {
            try
            {
                DataTable table = _luoi.DataSource as DataTable;
                if (table == null) { _lblThongKe.Text = ""; return; }

                int do_ = 0, xanh = 0, giua = 0;
                foreach (DataRow row in table.Rows)
                {
                    object v = row["DiemTB"];
                    if (v == null || v == DBNull.Value) continue;

                    decimal d = Convert.ToDecimal(v);
                    if (d < _numThap.Value) do_++;
                    else if (d >= _numCao.Value) xanh++;
                    else giua++;
                }

                _lblThongKe.Text = string.Format(
                    "Tổng {0} dòng · đỏ (< {1:0.0}): {2} · xanh (≥ {3:0.0}): {4} · còn lại: {5}",
                    table.Rows.Count, _numThap.Value, do_, _numCao.Value, xanh, giua);
            }
            catch (Exception) { }
        }
    }
}
