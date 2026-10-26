using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Lab05_MultiForm.Helpers;
using Lab05_MultiForm.Models;

namespace Lab05_MultiForm.Forms
{
    /// <summary>
    /// frmChiTietSinhVien - Bài 4: Form chi tiết nhận đối tượng SinhVien qua CONSTRUCTOR.
    ///
    /// Nguyên tắc: không truy cập trực tiếp control private của Form khác.
    ///   - Dữ liệu vào  : constructor  frmChiTietSinhVien(SinhVien sv)
    ///   - Dữ liệu ra   : thuộc tính public SinhVien + DialogResult.OK
    /// Form sửa trên BẢN SAO được truyền vào; Form gọi chỉ cập nhật dữ liệu gốc khi DialogResult = OK.
    /// </summary>
    public partial class frmChiTietSinhVien : FormBase
    {
        private readonly SinhVien _sinhVien;
        private readonly string _nguon;
        private Image _anhRieng;

        /// <summary>Dữ liệu sau khi chỉnh sửa (dùng khi DialogResult = OK).</summary>
        public SinhVien SinhVien { get { return _sinhVien; } }

        public frmChiTietSinhVien(SinhVien sinhVien)
            : this(sinhVien, "từ frmSinhVien")
        {
        }

        public frmChiTietSinhVien(SinhVien sinhVien, string nguon)
        {
            InitializeComponent();

            // Không cho null làm sập Form - tạo đối tượng rỗng để hiển thị trạng thái
            _sinhVien = sinhVien ?? new SinhVien();
            _nguon = nguon ?? string.Empty;

            Text = "Chi tiết sinh viên: " + _sinhVien.MaSV;

            // Nạp sẵn dữ liệu để có thể đọc (SinhVien, MoTaDuLieu) ngay sau khi tạo Form
            NapDanhSachKhoa();
            HienThiDuLieu();
        }

        /// <summary>Chuỗi mô tả dữ liệu đang hiển thị - dùng cho nhật ký báo cáo.</summary>
        public string MoTaDuLieu
        {
            get
            {
                return string.Format("{0} - {1} - {2:dd/MM/yyyy} - {3:0.0} ({4}) - {5} - {6}",
                    _sinhVien.MaSV, _sinhVien.HoTen, _sinhVien.NgaySinh, _sinhVien.DiemTB,
                    _sinhVien.XepLoai(), _sinhVien.Khoa, _sinhVien.GioiTinh);
            }
        }

        private void frmChiTietSinhVien_Load(object sender, EventArgs e)
        {
            NapDanhSachKhoa();
            HienThiDuLieu();
        }

        private void NapDanhSachKhoa()
        {
            cboKhoa.Items.Clear();
            foreach (Khoa k in DuLieuMau.DanhSachKhoa()) cboKhoa.Items.Add(k);
        }

        /// <summary>Đổ dữ liệu đối tượng ra control (chỉ đọc thuộc tính public).</summary>
        private void HienThiDuLieu()
        {
            lblMa.Text = string.IsNullOrEmpty(_sinhVien.MaSV) ? "—" : _sinhVien.MaSV;
            lblHoTen.Text = string.IsNullOrEmpty(_sinhVien.HoTen) ? "—" : _sinhVien.HoTen;
            lblDiem.Text = _sinhVien.DiemTB.ToString("N2");
            lblNgaySinh.Text = _sinhVien.NgaySinh.ToString("dd/MM/yyyy");
            lblTuoi.Text = KiemTra.TinhTuoi(_sinhVien.NgaySinh) + " tuổi";
            lblGioiTinh.Text = string.IsNullOrEmpty(_sinhVien.GioiTinh) ? "—" : _sinhVien.GioiTinh;
            lblKhoa.Text = string.IsNullOrEmpty(_sinhVien.Khoa) ? "Chưa chọn khoa" : _sinhVien.Khoa;
            lblSoThich.Text = string.IsNullOrEmpty(_sinhVien.SoThich) ? "Chưa chọn sở thích" : _sinhVien.SoThich;
            lblNguon.Text = "Dữ liệu nhận qua constructor " + _nguon;

            badgeXepLoai.Value = _sinhVien.XepLoai().ToUpperInvariant();
            badgeXepLoai.SetColor(KiemTra.MauXepLoai(_sinhVien.DiemTB));

            // Ảnh: ảnh tự chọn hoặc ảnh mặc định vẽ từ họ tên
            if (_anhRieng != null) { _anhRieng.Dispose(); _anhRieng = null; }
            if (!string.IsNullOrEmpty(_sinhVien.AnhDaiDien) && File.Exists(_sinhVien.AnhDaiDien))
            {
                try
                {
                    // Đọc vào bộ nhớ để không khoá file cho các Form khác
                    using (Image goc = Image.FromFile(_sinhVien.AnhDaiDien)) _anhRieng = new Bitmap(goc);
                    picAnh.Image = _anhRieng;
                }
                catch (Exception)
                {
                    picAnh.Image = UiTheme.CreateAvatarImage(_sinhVien.HoTen, 140);
                }
            }
            else
            {
                picAnh.Image = UiTheme.CreateAvatarImage(_sinhVien.HoTen, 140);
            }

            // Thanh điểm 0 - 10
            pnlDiemBar.Width = Math.Max(0, (int)(_sinhVien.DiemTB / 10.0 * pnlDiemBarFill.Width));
            lblDiemThang.Text = string.Format("{0:0.0} / 10", _sinhVien.DiemTB);

            // Điền hộp thoại chỉnh sửa
            UiFactory.SetVal(txtHoTen, _sinhVien.HoTen);
            UiFactory.SetVal(txtDiem, _sinhVien.DiemTB.ToString("0.0"));
            if (_sinhVien.NgaySinh >= dtpNgaySinh.MinDate && _sinhVien.NgaySinh <= dtpNgaySinh.MaxDate)
                dtpNgaySinh.Value = _sinhVien.NgaySinh;
            radNam.Checked = _sinhVien.GioiTinh != "Nữ";
            radNu.Checked = _sinhVien.GioiTinh == "Nữ";
            foreach (object o in cboKhoa.Items)
                if (o.ToString() == _sinhVien.Khoa) cboKhoa.SelectedItem = o;
            if (cboKhoa.SelectedIndex < 0 && cboKhoa.Items.Count > 0) cboKhoa.SelectedIndex = 0;
            UiFactory.SetVal(txtSoThich, _sinhVien.SoThich);
        }

        // =====================================================================
        // CHẾ ĐỘ CHỈNH SỬA
        // =====================================================================
        private void chkChinhSua_CheckedChanged(object sender, EventArgs e)
        {
            bool sua = chkChinhSua.Checked;
            pnlSua.Visible = sua;
            btnLuu.Visible = sua;
            btnHuy.Visible = sua;
            lblGoiY.Text = sua
                ? "Đang chỉnh sửa bản sao dữ liệu - bấm Lưu để trả kết quả về Form gọi (DialogResult.OK)."
                : "Bật \"Chỉnh sửa\" để sửa họ tên, điểm, khoa rồi bấm Lưu để trả dữ liệu về Form gọi.";
        }

        private void txtDiem_KeyPress(object sender, KeyPressEventArgs e)
        {
            KiemTra.ChanNhapDiem(sender, e);
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            string loi;
            double diem;

            if (!KiemTra.HopLeHoTen(UiFactory.Val(txtHoTen), out loi))
            {
                errorProvider1.SetError(txtHoTen, loi);
                return;
            }
            errorProvider1.SetError(txtHoTen, string.Empty);

            if (!KiemTra.HopLeDiem(UiFactory.Val(txtDiem), out diem, out loi))
            {
                errorProvider1.SetError(txtDiem, loi);
                return;
            }
            errorProvider1.SetError(txtDiem, string.Empty);

            if (!KiemTra.HopLeNgaySinh(dtpNgaySinh.Value, out loi))
            {
                errorProvider1.SetError(dtpNgaySinh, loi);
                return;
            }
            errorProvider1.SetError(dtpNgaySinh, string.Empty);

            // Cập nhật đúng đối tượng được truyền vào (Form gọi truyền bản sao nên an toàn)
            _sinhVien.HoTen = KiemTra.ChuanHoaHoTen(UiFactory.Val(txtHoTen));
            _sinhVien.DiemTB = diem;
            _sinhVien.NgaySinh = dtpNgaySinh.Value;
            _sinhVien.GioiTinh = radNu.Checked ? "Nữ" : "Nam";
            _sinhVien.Khoa = cboKhoa.Text;
            _sinhVien.SoThich = UiFactory.Val(txtSoThich);

            HienThiDuLieu();
            chkChinhSua.Checked = false;
            ChinhSua = true;

            // Trả kết quả về Form gọi qua DialogResult (không dùng biến toàn cục)
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            chkChinhSua.Checked = false;
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            if (chkChinhSua.Checked)
            {
                DialogResult r = MessageBox.Show("Bạn đang chỉnh sửa dữ liệu chưa lưu. Đóng và bỏ thay đổi?",
                    "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r == DialogResult.No) return;
            }
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void frmChiTietSinhVien_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Khi đóng bằng nút X (không qua nút Đóng) mà còn đang sửa thì xác nhận
            if (e.CloseReason == CloseReason.UserClosing && chkChinhSua.Checked)
            {
                DialogResult r = MessageBox.Show("Bạn đang chỉnh sửa dữ liệu chưa lưu. Đóng và bỏ thay đổi?",
                    "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (r == DialogResult.No) { e.Cancel = true; return; }
            }
            if (DialogResult == DialogResult.None) DialogResult = DialogResult.Cancel;
        }

        /// <summary>Đánh dấu đã có chỉnh sửa (Form gọi dựa vào DialogResult thay cho cờ này).</summary>
        public bool ChinhSua { get; private set; }
    }
}
