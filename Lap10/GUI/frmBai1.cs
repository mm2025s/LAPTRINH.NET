using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Lab10_DisconnectedData.BUS;
using Lab10_DisconnectedData.DTO;
using Lab10_DisconnectedData.GUI.Helpers;

namespace Lab10_DisconnectedData.GUI
{
    /// <summary>
    /// BAI 1 - Fill DataTable bang SqlDataAdapter.
    ///
    /// Diem can nho: SqlDataAdapter.Fill() TU MO ket noi neu connection dang
    /// dong, doc xong TU DONG ket noi tro lai. Vi vay khong can viet
    /// conn.Open() truoc Fill - nhung cung KHONG duoc quen dong connection
    /// ben ngoai (dung using).
    ///
    /// Form do thoi gian Fill de thay chi phi that: mot lan di ve server, sau
    /// do moi thao tac tren DataTable deu chay trong bo nho.
    /// </summary>
    public class frmBai1 : BaiFormBase
    {
        private readonly SinhVienBUS _bus = new SinhVienBUS();

        private LuoiSinhVien _luoi;
        private Label _lblKetNoi;
        private Label _lblSoDong;
        private Label _lblThoiGian;
        private TextBox _txtSql;

        private DataTable _table;
        private long _thoiGianFill;

        public frmBai1()
            : base("1", "Fill DataTable bằng SqlDataAdapter",
                   "adapter.Fill(table) tự mở/đóng kết nối — dữ liệu sau đó nằm hoàn toàn trong bộ nhớ",
                   UiTheme.Blue, "SqlDataAdapter", "Fill(DataTable)", "frmBai1",
                   new Size(1080, 720))
        {
            Badge = new BadgeLabel();
            Badge.Value = "Connected → Disconnected";
            Badge.BaseColor = UiTheme.Blue;
            Badge.SetBounds(NoiDung.Right - 200, 20, 180, 24);

            TaoThanhCongCu();
            TaoLuoi();
            TaoThongKe();
        }

        private void TaoThanhCongCu()
        {
            CardPanel the = The(18, 18, 1044, 96);
            NoiDung.Controls.Add(the);

            Label tieuDe = TieuDeMuc("NẠP BẢNG BẰNG ADAPTER", UiTheme.Blue);
            tieuDe.SetBounds(18, 12, 300, 22);
            the.Controls.Add(tieuDe);

            the.Controls.Add(Nut("btnNap", "Fill DataTable", UiTheme.Blue, 18, 44, 160));
            the.Controls.Add(Nut("btnNapLai", "Nạp lại (đo thời gian)", UiTheme.Teal, 190, 44, 190, 36, true));

            _txtSql = new TextBox();
            _txtSql.Multiline = true;
            _txtSql.ReadOnly = true;
            _txtSql.BorderStyle = BorderStyle.None;
            _txtSql.Font = UiTheme.FontMono(8f);
            _txtSql.BackColor = Color.FromArgb(30, 34, 48);
            _txtSql.ForeColor = Color.FromArgb(190, 232, 200);
            _txtSql.SetBounds(400, 30, 626, 56);
            _txtSql.Text =
                "var table = new DataTable(\"SinhVien\");\r\n" +
                "new SqlDataAdapter(\"SELECT ... FROM dbo.SinhVien\", conn).Fill(table);";
            the.Controls.Add(_txtSql);

            Tim("btnNap").Click += (s, e) => Nap();
            Tim("btnNapLai").Click += (s, e) => Nap();
        }

        private void TaoLuoi()
        {
            CardPanel the = The(18, 126, 1044, 470);
            the.Padding = new Padding(1);
            NoiDung.Controls.Add(the);

            _luoi = new LuoiSinhVien();
            _luoi.Dock = DockStyle.Fill;
            _luoi.BatToMau = false;
            the.Controls.Add(_luoi);
        }

        private void TaoThongKe()
        {
            CardPanel the = The(18, 608, 1044, 72);
            NoiDung.Controls.Add(the);

            _lblKetNoi = UiFactory.Label("—", 9f, UiTheme.InkSoft, false);
            _lblKetNoi.SetBounds(18, 12, 500, 22);
            the.Controls.Add(_lblKetNoi);

            _lblSoDong = UiFactory.Label("—", 12f, UiTheme.Blue, true);
            _lblSoDong.SetBounds(18, 34, 320, 28);
            the.Controls.Add(_lblSoDong);

            _lblThoiGian = UiFactory.Label("—", 9f, UiTheme.Muted, false);
            _lblThoiGian.SetBounds(360, 38, 660, 22);
            the.Controls.Add(_lblThoiGian);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Nap();
        }

        private void Nap()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                var dongHo = Stopwatch.StartNew();
                _table = _bus.GetStudentTable();
                dongHo.Stop();
                _thoiGianFill = dongHo.ElapsedMilliseconds;

                _luoi.GanDuLieu(_table);

                _lblKetNoi.Text =
                    "adapter.Fill() đã mở kết nối, đọc dữ liệu rồi đóng kết nối — hiện connection đã Close.";
                _lblSoDong.Text = _table.Rows.Count + " dòng  ·  " + _table.Columns.Count + " cột";
                _lblThoiGian.Text = "Thời gian Fill: " + _thoiGianFill + " ms  ·  " +
                    "tên bảng: \"" + _table.TableName + "\"  ·  RowState đầu tiên: " +
                    (_table.Rows.Count > 0 ? _table.Rows[0].RowState.ToString() : "—");

                KetQua("Đã nạp " + _table.Rows.Count + " sinh viên vào DataTable trong " +
                    _thoiGianFill + " ms.", true);
            }
            catch (LoiNghiepVu ex) { HienThiLoiNghiepVu(ex); }
            catch (Exception ex) { LoiChung(ex); }
            finally { Cursor = Cursors.Default; }
        }
    }
}
