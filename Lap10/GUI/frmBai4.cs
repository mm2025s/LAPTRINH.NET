using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Lab10_DisconnectedData.BUS;
using Lab10_DisconnectedData.DTO;
using Lab10_DisconnectedData.GUI.Helpers;

namespace Lab10_DisconnectedData.GUI
{
    /// <summary>
    /// BAI 4 - Loc va sap xep bang DataView.
    ///
    /// DataView la mot "lop nhin" tren DataTable: khong sao chep du lieu, khong
    /// truy van lai CSDL. Doi RowFilter / Sort la loc va sap xep lai NGAY trong
    /// bo nho - dung la loi ich chinh cua mo hinh ngat ket noi.
    ///
    /// Diem nguy hiem: RowFilter la BIEU THUC dang chuoi nen khong tham so hoa
    /// duoc. Gia tri chuoi phai escape dau nhay don ('' thay cho '), so phai
    /// dung InvariantCulture (mays Viá»‡t Nam co the sinh "DiemTB >= 7,5").
    /// </summary>
    public class frmBai4 : BaiFormBase
    {
        private readonly SinhVienBUS _bus = new SinhVienBUS();

        private LuoiSinhVien _luoi;
        private ComboBox _cboKhoa;
        private NumericUpDown _numDiem;
        private CheckBox _chkDungDiem;
        private Label _lblFilter;
        private Label _lblConLai;
        private Label _lblTocDo;
        private TextBox _txtThuTanCong;

        private DataSet _ds;

        public frmBai4()
            : base("4", "Lọc và sắp xếp bằng DataView",
                   "RowFilter + Sort thao tác trên bộ nhớ — không truy vấn lại CSDL",
                   UiTheme.Amber, "DataView", "RowFilter · Sort", "frmBai4",
                   new Size(1150, 740))
        {
            Badge = new BadgeLabel();
            Badge.Value = "0 truy vấn SQL";
            Badge.BaseColor = UiTheme.Amber;
            Badge.SetBounds(NoiDung.Right - 165, 20, 145, 24);

            TaoBoLoc();
            TaoLuoi();
            TaoGiaiThich();
        }

        private void TaoBoLoc()
        {
            CardPanel the = The(18, 18, 1114, 132);
            NoiDung.Controls.Add(the);

            the.Controls.Add(Nut("btnNap", "Nạp DataSet", UiTheme.Amber, 18, 20, 140));
            the.Controls.Add(Nut("btnLoc", "Áp dụng lọc", UiTheme.Orange, 170, 20, 140));

            Label n1 = NhanTruong("Khoa");
            n1.SetBounds(330, 8, 200, 18);
            the.Controls.Add(n1);

            _cboKhoa = UiFactory.ComboBox("cboKhoa");
            _cboKhoa.SetBounds(330, 28, 220, 28);
            _cboKhoa.SelectedIndexChanged += (s, e) => ApDungLoc();
            the.Controls.Add(_cboKhoa);

            Label n2 = NhanTruong("Điểm tối thiểu");
            n2.SetBounds(570, 8, 200, 18);
            the.Controls.Add(n2);

            _numDiem = new NumericUpDown();
            _numDiem.Name = "numDiem";
            _numDiem.DecimalPlaces = 2;
            _numDiem.Increment = 0.25m;
            _numDiem.Minimum = 0m;
            _numDiem.Maximum = 10m;
            _numDiem.Value = 7m;
            _numDiem.Font = UiTheme.FontBody(10f);
            _numDiem.SetBounds(570, 30, 90, 26);
            _numDiem.ValueChanged += (s, e) => { if (_chkDungDiem.Checked) ApDungLoc(); };
            the.Controls.Add(_numDiem);

            _chkDungDiem = new CheckBox();
            _chkDungDiem.Name = "chkDungDiem";
            _chkDungDiem.Text = "Bật lọc điểm";
            _chkDungDiem.Font = UiTheme.FontBody(9f);
            _chkDungDiem.ForeColor = UiTheme.InkSoft;
            _chkDungDiem.Checked = true;
            _chkDungDiem.SetBounds(668, 32, 130, 22);
            _chkDungDiem.CheckedChanged += (s, e) => ApDungLoc();
            the.Controls.Add(_chkDungDiem);

            Label n3 = NhanTruong("Biểu thức RowFilter đang dùng");
            n3.SetBounds(18, 66, 300, 18);
            the.Controls.Add(n3);

            _lblFilter = new Label();
            _lblFilter.Name = "lblFilter";
            _lblFilter.Font = UiTheme.FontMono(9f);
            _lblFilter.ForeColor = UiTheme.Orange;
            _lblFilter.TextAlign = ContentAlignment.MiddleLeft;
            _lblFilter.BackColor = Color.FromArgb(255, 249, 240);
            _lblFilter.BorderStyle = BorderStyle.FixedSingle;
            _lblFilter.SetBounds(18, 88, 700, 30);
            _lblFilter.Text = "(chưa lọc)";
            the.Controls.Add(_lblFilter);

            _lblConLai = UiFactory.Label("—", 10f, UiTheme.Ink, true);
            _lblConLai.SetBounds(732, 88, 150, 30);
            the.Controls.Add(_lblConLai);

            _lblTocDo = UiFactory.Label("—", 8.5f, UiTheme.Muted, false);
            _lblTocDo.SetBounds(880, 88, 220, 30);
            the.Controls.Add(_lblTocDo);

            Tim("btnNap").Click += (s, e) => Nap();
            Tim("btnLoc").Click += (s, e) => ApDungLoc();
        }

        private void TaoLuoi()
        {
            CardPanel the = The(18, 162, 1114, 400);
            the.Padding = new Padding(1);
            NoiDung.Controls.Add(the);

            _luoi = new LuoiSinhVien();
            _luoi.Dock = DockStyle.Fill;
            the.Controls.Add(_luoi);
        }

        private void TaoGiaiThich()
        {
            CardPanel the = The(18, 574, 1114, 106);
            NoiDung.Controls.Add(the);

            Label t = TieuDeMuc("THỬ GIÁ TRỊ CÓ DẤU NHÁY ĐƠN (ESCAPE ROWFILTER)", UiTheme.Amber);
            t.SetBounds(18, 8, 500, 20);
            the.Controls.Add(t);

            _txtThuTanCong = TaoO("txtThuTanCong", 18, 34, 380,
                "Nhập tên có dấu nháy, ví dụ: O'Neil");
            _txtThuTanCong.Text = "O'Neil";
            _txtThuTanCong.ForeColor = Color.Black;
            the.Controls.Add(_txtThuTanCong);

            the.Controls.Add(Nut("btnThuLoc", "Lọc theo chuỗi này", UiTheme.Orange, 410, 32, 180));

            Label ghiChu = UiFactory.Label(
                "RowFilter là biểu thức dạng chuỗi nên KHÔNG tham số hóa được: giá trị phải nhân đôi " +
                "dấu nháy đơn (O''Neil) trước khi ghép, nếu không biểu thức sẽ vỡ.",
                8.5f, UiTheme.Muted, false);
            ghiChu.SetBounds(606, 28, 492, 60);
            the.Controls.Add(ghiChu);

            Tim("btnThuLoc").Click += (s, e) => ThuLocChuoi();
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

                _ds = _bus.GetDataSet();

                DataTable khoa = _ds.Tables["Khoa"].Copy();
                DataRow tatCa = khoa.NewRow();
                tatCa["MaKhoa"] = DBNull.Value;
                tatCa["TenKhoa"] = "(Tất cả khoa)";
                khoa.Rows.InsertAt(tatCa, 0);

                _cboKhoa.DataSource = khoa;
                _cboKhoa.DisplayMember = "TenKhoa";
                _cboKhoa.ValueMember = "MaKhoa";

                ApDungLoc();
            }
            catch (LoiNghiepVu ex) { HienThiLoiNghiepVu(ex); }
            catch (Exception ex) { LoiChung(ex); }
            finally { Cursor = Cursors.Default; }
        }

        private void ApDungLoc()
        {
            try
            {
                if (_ds == null) return;

                string maKhoa = null;
                if (_cboKhoa.SelectedValue != null && _cboKhoa.SelectedValue != DBNull.Value)
                    maKhoa = Convert.ToString(_cboKhoa.SelectedValue);

                decimal? diem = _chkDungDiem.Checked ? (decimal?)_numDiem.Value : null;

                var dongHo = System.Diagnostics.Stopwatch.StartNew();
                DataView view = _bus.TaoView(_ds, maKhoa, diem);
                dongHo.Stop();

                _luoi.DataSource = view;
                _luoi.DatLaiTieuDe();

                _lblFilter.Text = _bus.MoTaRowFilter(maKhoa, diem) + "   |   Sort = DiemTB DESC, HoTen ASC";
                _lblConLai.Text = view.Count + " / " + _ds.Tables["SinhVien"].Rows.Count + " dòng";
                _lblTocDo.Text = dongHo.Elapsed.TotalMilliseconds.ToString("0.0") + " ms (chỉ trong bộ nhớ)";
            }
            catch (LoiNghiepVu ex) { HienThiLoiNghiepVu(ex); }
            catch (Exception ex) { LoiChung(ex); }
        }

        /// <summary>
        /// Thu loc theo mot chuoi CO DAU NHAY DON de chung minh escape hoat dong.
        /// Khong dung RowFilter ghep tay o day ma goi thang BUS -> DAL da escape.
        /// </summary>
        private void ThuLocChuoi()
        {
            try
            {
                if (_ds == null) return;

                string giaTri = _txtThuTanCong.Text.Trim();
                string daEscape = DAL.SinhVienDAL.EscapeRowFilter(giaTri);

                var view = new DataView(_ds.Tables["SinhVien"]);
                view.RowFilter = "HoTen LIKE '%" + daEscape + "%'";
                _luoi.DataSource = view;

                KetQua("RowFilter = HoTen LIKE '%" + daEscape + "%'  →  " + view.Count +
                    " dòng. Chuỗi gốc: \"" + giaTri + "\" → escape thành \"" + daEscape + "\".", true);
                _lblFilter.Text = "HoTen LIKE '%" + daEscape + "%'";
            }
            catch (Exception ex) { LoiChung(ex); }
        }
    }
}
