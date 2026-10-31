using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Lab09_DataReaderSearch.BUS;
using Lab09_DataReaderSearch.DTO;
using Lab09_DataReaderSearch.GUI.Helpers;

namespace Lab09_DataReaderSearch.GUI
{
    /// <summary>
    /// BAI 10 - Xuat ket qua tim kiem ra CSV (bai tu thuc hien).
    ///
    /// Yeu cau "xuat DUNG tap du lieu dang hien thi": file CSV phai chua y
    /// nhung dong nguoi dung dang nhin, dung thu tu do, khong phai toan bo
    /// CSDL. Vi vay Form lay thang DataSource cua luoi - nhung gi luoi dang
    /// hien chinh la nhung gi duoc ghi ra.
    ///
    /// Yeu cau "ho tro tieng Viet UTF-8": file phai co BOM, neu khong Excel se
    /// hien sai dau. Form kiem tra lai BOM sau khi ghi va bao ket qua cho
    /// nguoi dung thay.
    /// </summary>
    public class frmBai10 : BaiFormBase
    {
        private readonly SinhVienBUS _bus = new SinhVienBUS();

        private LuoiSinhVien _luoi;
        private TextBox _txtTuKhoa;
        private ComboBox _cboKhoa;
        private Label _lblTrangThaiXuat;
        private Label _lblXemTruoc;
        private TextBox _txtXemTruoc;

        private string _duongDanCuoi;

        public frmBai10()
            : base("10", "Xuất kết quả tìm kiếm ra CSV",
                   "Chỉ xuất đúng tập đang hiển thị, mã hoá UTF-8 có BOM để Excel đọc đúng tiếng Việt",
                   UiTheme.Blue, "UTF-8 + BOM", "RFC 4180", "frmBai10",
                   new Size(1090, 760))
        {
            Badge = new BadgeLabel();
            Badge.Value = "CSV / UTF-8";
            Badge.BaseColor = UiTheme.Blue;
            Badge.SetBounds(NoiDung.Right - 170, 20, 150, 24);

            TaoDieuKhien();
            TaoLuoi();
            TaoVungXuat();
        }

        private void TaoDieuKhien()
        {
            CardPanel the = The(18, 18, 1042, 96);
            NoiDung.Controls.Add(the);

            Label tieuDe = TieuDeMuc("CHỌN TẬP DỮ LIỆU CẦN XUẤT", UiTheme.Blue);
            tieuDe.SetBounds(18, 14, 400, 22);
            the.Controls.Add(tieuDe);

            Label nhan = NhanTruong("Họ tên chứa");
            nhan.SetBounds(18, 42, 110, 18);
            the.Controls.Add(nhan);

            _txtTuKhoa = TaoO("HoTen", 18, 62, 220, "Để trống = tất cả");
            _txtTuKhoa.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; Loc(); }
            };
            the.Controls.Add(_txtTuKhoa);

            Label nhanKhoa = NhanTruong("Khoa");
            nhanKhoa.SetBounds(254, 42, 160, 18);
            the.Controls.Add(nhanKhoa);

            _cboKhoa = UiFactory.ComboBox("MaKhoa");
            _cboKhoa.SetBounds(254, 62, 190, 28);
            the.Controls.Add(_cboKhoa);

            AccentButton btnLoc = Nut("btnLoc", "Lọc", UiTheme.Blue, 462, 60, 100, 32);
            btnLoc.Click += (s, e) => Loc();
            the.Controls.Add(btnLoc);

            AccentButton btnTatCa = Nut("btnTatCa", "Tất cả", UiTheme.Slate, 572, 60, 100, 32, true);
            btnTatCa.Click += (s, e) =>
            {
                UiFactory.SetVal(_txtTuKhoa, "");
                if (_cboKhoa.Items.Count > 0) _cboKhoa.SelectedIndex = 0;
                Loc();
            };
            the.Controls.Add(btnTatCa);
        }

        private void TaoLuoi()
        {
            CardPanel the = The(18, 124, 620, 432);
            the.Padding = new Padding(1);
            NoiDung.Controls.Add(the);

            _luoi = new LuoiSinhVien();
            _luoi.Name = "luoiSinhVien";
            _luoi.Dock = DockStyle.Fill;
            the.Controls.Add(_luoi);
        }

        private void TaoVungXuat()
        {
            CardPanel the = The(656, 124, 404, 432);
            NoiDung.Controls.Add(the);

            Label tieuDe = TieuDeMuc("XUẤT CSV", UiTheme.Blue);
            tieuDe.SetBounds(18, 14, 300, 22);
            the.Controls.Add(tieuDe);

            AccentButton btnLuu = Nut("btnXuatCsv", "Xuất CSV…", UiTheme.Green, 18, 44, 160, 36);
            btnLuu.Click += (s, e) => Xuat();
            the.Controls.Add(btnLuu);

            AccentButton btnMo = Nut("btnMoFile", "Mở tệp vừa xuất", UiTheme.Slate, 190, 44, 190, 36, true);
            btnMo.Click += (s, e) =>
            {
                if (string.IsNullOrEmpty(_duongDanCuoi) || !File.Exists(_duongDanCuoi))
                {
                    KetQua("Chưa có tệp nào được xuất trong phiên này.", false);
                    return;
                }
                XuatCsv.MoFile(_duongDanCuoi);
            };
            the.Controls.Add(btnMo);

            _lblTrangThaiXuat = UiFactory.Label("", 8.75f, UiTheme.Muted, false);
            _lblTrangThaiXuat.SetBounds(18, 90, 366, 44);
            the.Controls.Add(_lblTrangThaiXuat);

            Panel ke = DuongKe(18, 140, 366);
            the.Controls.Add(ke);

            Label nhanXem = NhanTruong("Xem trước nội dung CSV (10 dòng đầu)");
            nhanXem.SetBounds(18, 150, 300, 18);
            the.Controls.Add(nhanXem);

            _txtXemTruoc = new TextBox();
            _txtXemTruoc.Multiline = true;
            _txtXemTruoc.ReadOnly = true;
            _txtXemTruoc.ScrollBars = ScrollBars.Both;
            _txtXemTruoc.WordWrap = false;
            _txtXemTruoc.BorderStyle = BorderStyle.None;
            _txtXemTruoc.Font = UiTheme.FontMono(8f);
            _txtXemTruoc.BackColor = Color.FromArgb(30, 34, 48);
            _txtXemTruoc.ForeColor = Color.FromArgb(190, 232, 200);
            _txtXemTruoc.SetBounds(18, 172, 366, 246);
            the.Controls.Add(_txtXemTruoc);

            _lblXemTruoc = UiFactory.Label("", 8f, UiTheme.Muted, false);
            _lblXemTruoc.SetBounds(18, 420, 366, 18);
            the.Controls.Add(_lblXemTruoc);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            try
            {
                List<Khoa> ds = _bus.DanhSachKhoa();
                _cboKhoa.Items.Clear();
                _cboKhoa.Items.Add("(Tất cả khoa)");
                foreach (Khoa k in ds) _cboKhoa.Items.Add(k);
                _cboKhoa.SelectedIndex = 0;
            }
            catch (Exception ex) { LoiChung(ex); }

            Loc();
        }

        private void Loc()
        {
            try
            {
                string tuKhoa = UiFactory.Val(_txtTuKhoa);
                var k = _cboKhoa.SelectedItem as Khoa;

                DataTable table = _bus.LoadStudents();

                // Loc o phia client chi cho phan xem truoc/xuat - du lieu dang
                // hien thi tren luoi chinh la tap se duoc ghi ra file.
                DataTable ketQua = table.Clone();
                foreach (DataRow row in table.Rows)
                {
                    if (!string.IsNullOrEmpty(tuKhoa) &&
                        !DinhDang.Chua(Convert.ToString(row["HoTen"]), tuKhoa)) continue;

                    if (k != null && !string.Equals(Convert.ToString(row["MaKhoa"]), k.MaKhoa,
                            StringComparison.OrdinalIgnoreCase)) continue;

                    ketQua.ImportRow(row);
                }

                _luoi.GanDuLieu(ketQua);
                _lblTrangThaiXuat.Text = "Tập hiện tại: " + ketQua.Rows.Count + " dòng";
                _lblXemTruoc.Text = "";

                if (ketQua.Rows.Count == 0)
                    KetQua("Không có dòng nào để xuất với bộ lọc hiện tại.", false);
                else
                    KetQua("Đang hiển thị " + ketQua.Rows.Count + " sinh viên — bấm \"Xuất CSV\" để ghi ra tệp.", true);
            }
            catch (Exception ex) { LoiChung(ex); }
        }

        private void Xuat()
        {
            try
            {
                var table = _luoi.DataSource as DataTable;
                if (table == null || table.Rows.Count == 0)
                {
                    KetQua("Không có dữ liệu để xuất.", false);
                    return;
                }

                string tenGoiY = "KetQua_SinhVien_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";

                using (var dialog = new SaveFileDialog())
                {
                    dialog.Title = "Xuất kết quả tìm kiếm ra CSV";
                    dialog.Filter = "Tệp CSV (*.csv)|*.csv|Tất cả tệp (*.*)|*.*";
                    dialog.FileName = tenGoiY;
                    dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    dialog.OverwritePrompt = true;

                    if (dialog.ShowDialog(this) != DialogResult.OK) return;

                    _duongDanCuoi = dialog.FileName;
                }

                // Ghi file - bo cot MaKhoa vi da co TenKhoa de doc hon
                int soDong = XuatCsv.Ghi(table, _duongDanCuoi, "MaKhoa");

                bool coBom = XuatCsv.CoBom(_duongDanCuoi);
                long kichThuoc = new FileInfo(_duongDanCuoi).Length;

                _lblTrangThaiXuat.Text = string.Format(
                    "{0} dòng đã ghi\n{1}\nUTF-8 BOM: {2} · {3} byte",
                    soDong, Path.GetFileName(_duongDanCuoi), coBom ? "có" : "KHÔNG",
                    kichThuoc);
                _lblTrangThaiXuat.ForeColor = coBom ? UiTheme.Green : UiTheme.Red;

                DocXemTruoc(_duongDanCuoi);

                KetQua(string.Format("Đã xuất {0} dòng ra {1} (UTF-8 BOM: {2}). Mở bằng Excel sẽ không lỗi tiếng Việt.",
                    soDong, Path.GetFileName(_duongDanCuoi), coBom ? "có" : "không"), coBom);
            }
            catch (IOException ex)
            {
                KetQua("Không ghi được tệp (có thể tệp đang mở trong Excel): " + ex.Message, false);
            }
            catch (Exception ex) { LoiChung(ex); }
        }

        /// <summary>Doc 10 dong dau cua file vua ghi de nguoi dung kiem tra ngay.</summary>
        private void DocXemTruoc(string duongDan)
        {
            try
            {
                var sb = new StringBuilder();
                int dem = 0;

                using (var reader = new StreamReader(duongDan, Encoding.UTF8, true))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null && dem < 10)
                    {
                        sb.AppendLine(line);
                        dem++;
                    }
                }

                _txtXemTruoc.Text = sb.ToString();
                _lblXemTruoc.Text = "Hiển thị " + Math.Min(dem, 10) + " dòng đầu · phân cách bằng dấu phẩy · CRLF";
            }
            catch (Exception ex)
            {
                _lblXemTruoc.Text = "Không đọc lại được tệp: " + ex.Message;
            }
        }
    }
}
