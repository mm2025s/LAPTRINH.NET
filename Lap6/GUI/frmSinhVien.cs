using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Lab06_ThreeLayer.BUS;
using Lab06_ThreeLayer.DTO;
using Lab06_ThreeLayer.GUI.Helpers;

namespace Lab06_ThreeLayer.GUI
{
    /// <summary>
    /// frmSinhVien - Bài 2 (DataGridView + BindingSource), Bài 3 (thêm/sửa/xóa + kiểm tra dữ liệu),
    /// Bài 5 (tìm kiếm - sắp xếp), Bài 9 (nhập/xuất CSV).
    ///
    /// Form CHỈ giao tiếp với tầng BUS. Không có một lời gọi DAL nào trong lớp này -
    /// đó là điều kiện của mô hình 3 lớp: GUI không biết dữ liệu nằm ở đâu.
    /// </summary>
    public partial class frmSinhVien : FormBase
    {
        private readonly SinhVienBUS _bus;
        private readonly KhoaBUS _busKhoa;

        private readonly BindingSource _nguon = new BindingSource();
        private readonly ErrorProvider _loi = new ErrorProvider();
        private bool _dangSua;
        private bool _dangNap;

        public frmSinhVien(SinhVienBUS bus, KhoaBUS busKhoa)
        {
            InitializeComponent();

            _bus = bus;
            _busKhoa = busKhoa;
        }

        /// <summary>Số dòng đang hiển thị (dùng cho trình chụp ảnh và Dashboard).</summary>
        public int SoDongHienThi
        {
            get { return dgvSinhVien.Rows.Count; }
        }

        // =====================================================================
        // KHỞI TẠO
        // =====================================================================
        private void frmSinhVien_Load(object sender, EventArgs e)
        {
            _loi.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            _loi.ContainerControl = this;

            foreach (Khoa k in _busKhoa.GetAll()) cboKhoa.Items.Add(k);
            if (cboKhoa.Items.Count > 0) cboKhoa.SelectedIndex = 0;

            cboKhoaFilter.Items.Clear();
            cboKhoaFilter.Items.Add("Tất cả khoa");
            foreach (Khoa k in _busKhoa.GetAll()) cboKhoaFilter.Items.Add(k);
            cboKhoaFilter.SelectedIndex = 0;

            dgvSinhVien.AutoGenerateColumns = false;
            _nguon.DataSource = new List<SinhVien>();
            dgvSinhVien.DataSource = _nguon;

            NapDuLieu();
            XoaTrang();
        }

        private void NapDuLieu()
        {
            _dangNap = true;
            try
            {
                string tuKhoa = UiFactory.Val(txtTimKiem);
                string khoa = cboKhoaFilter.SelectedIndex <= 0 ? string.Empty : cboKhoaFilter.Text;

                List<SinhVien> ds = _bus.TimKiem(tuKhoa, khoa);

                // Sắp xếp do BUS quyết định (Form chỉ nói tiêu chí)
                if (luoiXepTheo != null && luoiXepTheo.Length > 0)
                {
                    ds = _bus.SapXep(ds, luoiXepTheo, luoiGiam);
                }

                _nguon.DataSource = ds;
                lblDem.Text = string.Format("{0} / {1} sinh viên", ds.Count, _bus.SoLuong);
                lblDem.ForeColor = ds.Count == _bus.SoLuong ? UiTheme.Muted : UiTheme.Violet;
            }
            catch (Exception ex)
            {
                BaoLoi("Nạp dữ liệu", ex);
            }
            finally
            {
                _dangNap = false;
            }
        }

        // =====================================================================
        // CHỌN DÒNG -> ĐỔ LÊN Ô NHẬP
        // =====================================================================
        private void dgvSinhVien_SelectionChanged(object sender, EventArgs e)
        {
            if (_dangNap) return;
            DataGridViewRow row = dgvSinhVien.CurrentRow;
            if (row == null) return;
            SinhVien sv = row.DataBoundItem as SinhVien;
            if (sv == null) return;

            DoLenForm(sv);
        }

        private void DoLenForm(SinhVien sv)
        {
            txtMaSV.Text = sv.MaSV;
            txtHoTen.Text = sv.HoTen;
            dtpNgaySinh.Value = sv.NgaySinh < dtpNgaySinh.MinDate ? dtpNgaySinh.MinDate : sv.NgaySinh;
            radNam.Checked = sv.GioiTinh == "Nam";
            radNu.Checked = sv.GioiTinh != "Nam";
            txtDiemTB.Text = sv.DiemTB.ToString("0.0");
            txtEmail.Text = sv.Email;
            ChonKhoa(sv.Khoa);

            _dangSua = true;
            btnThem.Enabled = true;
            btnSua.Enabled = true;
            btnXoa.Enabled = true;
            txtMaSV.ReadOnly = true;
            capNhatTrangThai();
        }

        private void ChonKhoa(string tenKhoa)
        {
            for (int i = 0; i < cboKhoa.Items.Count; i++)
            {
                if (string.Equals(cboKhoa.Items[i].ToString(), tenKhoa, StringComparison.OrdinalIgnoreCase))
                {
                    cboKhoa.SelectedIndex = i;
                    return;
                }
            }
        }

        private void capNhatTrangThai()
        {
            lblTrangThai.Text = _dangSua ? "Đang sửa: " + txtMaSV.Text : "Đang thêm mới";
            lblTrangThai.ForeColor = _dangSua ? UiTheme.Amber : UiTheme.Green;
        }

        // =====================================================================
        // BÀI 3: THÊM / SỬA / XÓA
        // =====================================================================
        private void btnThem_Click(object sender, EventArgs e)
        {
            XoaTrang();
            txtHoTen.Focus();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (!_dangSua) { BaoLoiNgan("Hãy chọn một dòng trong lưới để sửa."); return; }
            Luu(false);
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            Luu(_dangSua);
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (!_dangSua) { BaoLoiNgan("Hãy chọn một dòng trong lưới để xóa."); return; }

            DialogResult r = MessageBox.Show(this,
                "Xóa sinh viên " + txtMaSV.Text + " - " + txtHoTen.Text + "?",
                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (r != DialogResult.Yes) return;

            try
            {
                _bus.Delete(txtMaSV.Text);
                NapDuLieu();
                XoaTrang();
                BaoThanhCong("Đã xóa " + txtMaSV.Text);
            }
            catch (LoiNghiepVu ex)
            {
                _loi.SetError(btnXoa, ex.Message);
                BaoLoiNgan("BUS từ chối: " + ex.Message);
            }
        }

        /// <summary>
        /// Gom dữ liệu trên Form thành DTO, để BUS kiểm tra toàn bộ quy tắc nghiệp vụ.
        /// Lỗi trả về là LoiNghiepVu kèm tên trường -> hiển thị đúng ô nhập.
        /// </summary>
        private void Luu(bool laCapNhat)
        {
            _loi.Clear();

            SinhVien sv = new SinhVien();
            sv.MaSV = UiFactory.Val(txtMaSV);
            sv.HoTen = UiFactory.Val(txtHoTen);
            sv.NgaySinh = dtpNgaySinh.Value;
            sv.GioiTinh = radNam.Checked ? "Nam" : "Nữ";
            sv.Khoa = cboKhoa.Text;
            sv.Email = UiFactory.Val(txtEmail);

            double diem;
            if (!double.TryParse(UiFactory.Val(txtDiemTB), NumberStyles.Float, CultureInfo.CurrentCulture, out diem)
                && !double.TryParse(UiFactory.Val(txtDiemTB), NumberStyles.Float, CultureInfo.InvariantCulture, out diem))
            {
                _loi.SetError(txtDiemTB, "Điểm phải là số.");
                BaoLoiNgan("Điểm phải là số.");
                txtDiemTB.Focus();
                return;
            }
            sv.DiemTB = diem;

            try
            {
                if (laCapNhat) _bus.Update(sv);
                else _bus.Add(sv);

                NapDuLieu();
                _dangSua = true;
                BaoThanhCong((laCapNhat ? "Đã cập nhật " : "Đã thêm ") + sv.MaSV);
            }
            catch (LoiNghiepVu ex)
            {
                // BUS nói sai ở trường nào -> ErrorProvider báo đúng ô đó
                Control o = TimTheoTruong(ex.Truong);
                if (o != null)
                {
                    _loi.SetError(o, ex.Message);
                    o.Focus();
                }
                BaoLoiNgan("BUS từ chối: " + ex.Message);
            }
            catch (Exception ex)
            {
                BaoLoi("Lưu dữ liệu", ex);
            }
        }

        private Control TimTheoTruong(string truong)
        {
            switch (truong)
            {
                case "MaSV": return txtMaSV;
                case "HoTen": return txtHoTen;
                case "NgaySinh": return dtpNgaySinh;
                case "DiemTB": return txtDiemTB;
                case "Email": return txtEmail;
                case "Khoa": return cboKhoa;
                default: return null;
            }
        }

        private void XoaTrang()
        {
            _dangSua = false;
            _loi.Clear();

            txtMaSV.Clear();
            txtMaSV.ReadOnly = false;
            txtHoTen.Clear();
            dtpNgaySinh.Value = new DateTime(2005, 1, 1);
            radNam.Checked = true;
            txtDiemTB.Clear();
            txtEmail.Clear();
            if (cboKhoa.Items.Count > 0) cboKhoa.SelectedIndex = 0;

            btnThem.Enabled = true;
            btnSua.Enabled = false;
            btnXoa.Enabled = false;
            capNhatTrangThai();
        }

        // =====================================================================
        // BÀI 2: SỰ KIỆN KeyPress CHO Ô ĐIỂM
        // =====================================================================
        private void txtDiemTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            KiemTra.ChanNhapSoThuc(txtDiemTB, e, 2, false);
        }

        // =====================================================================
        // BÀI 5: TÌM KIẾM - SẮP XẾP
        // =====================================================================
        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            NapDuLieu();
        }

        private void cboKhoaFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            NapDuLieu();
        }

        private void btnXoaLoc_Click(object sender, EventArgs e)
        {
            UiFactory.SetVal(txtTimKiem, string.Empty);
            cboKhoaFilter.SelectedIndex = 0;
            luoiXepTheo = null;
            luoiGiam = false;
            NapDuLieu();
            BaoThanhCong("Đã xoá bộ lọc");
        }

        private void dgvSinhVien_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string ten = dgvSinhVien.Columns[e.ColumnIndex].DataPropertyName;
            string tieuChi;
            switch (ten)
            {
                case "HoTen": tieuChi = "ten"; break;
                case "DiemTB": tieuChi = "diem"; break;
                case "Khoa": tieuChi = "khoa"; break;
                default: tieuChi = "ma"; break;
            }

            if (luoiXepTheo == tieuChi) luoiGiam = !luoiGiam;
            else { luoiXepTheo = tieuChi; luoiGiam = false; }

            NapDuLieu();
            BaoThanhCong("Sắp xếp theo " + dgvSinhVien.Columns[e.ColumnIndex].HeaderText + (luoiGiam ? " (giảm dần)" : " (tăng dần)"));
        }

        // =====================================================================
        // BÀI 9: CSV - Form gọi BUS, BUS gọi DAL
        // =====================================================================
        private void btnXuatCSV_Click(object sender, EventArgs e)
        {
            using (var hopThoai = new SaveFileDialog())
            {
                hopThoai.Title = "Xuất danh sách sinh viên ra CSV";
                hopThoai.Filter = "Tệp CSV (*.csv)|*.csv";
                hopThoai.FileName = "sinhvien_lab6.csv";
                if (hopThoai.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    int so = _bus.LuuCSV(hopThoai.FileName);
                    lblTapTin.Text = "Đã xuất " + so + " dòng: " + hopThoai.FileName;
                    BaoThanhCong("Đã xuất CSV (" + so + " dòng)");
                }
                catch (Exception ex)
                {
                    BaoLoi("Xuất CSV", ex);
                }
            }
        }

        private void btnNhapCSV_Click(object sender, EventArgs e)
        {
            using (var hopThoai = new OpenFileDialog())
            {
                hopThoai.Title = "Nhập danh sách sinh viên từ CSV";
                hopThoai.Filter = "Tệp CSV (*.csv)|*.csv";

                string goiY = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sinhvien_lab6.csv");
                if (File.Exists(goiY))
                {
                    hopThoai.InitialDirectory = Path.GetDirectoryName(goiY);
                    hopThoai.FileName = Path.GetFileName(goiY);
                }

                if (hopThoai.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    int soDongLoi;
                    int so = _bus.DocCSV(hopThoai.FileName, out soDongLoi);
                    NapDuLieu();
                    lblTapTin.Text = "Đã nhập " + so + " dòng từ " + Path.GetFileName(hopThoai.FileName)
                        + (soDongLoi > 0 ? " (bỏ qua " + soDongLoi + " dòng lỗi)" : string.Empty);
                    BaoThanhCong("Đã nhập CSV (" + so + " dòng)");
                }
                catch (Exception ex)
                {
                    BaoLoi("Nhập CSV", ex);
                }
            }
        }

        /// <summary>Dùng cho trình chụp ảnh: xuất CSV thẳng ra đường dẫn chỉ định.</summary>
        public string XuatCSV(string duongDan)
        {
            int so = _bus.LuuCSV(duongDan);
            lblTapTin.Text = "Đã xuất " + so + " dòng: " + duongDan;
            BaoThanhCong("Đã xuất CSV (" + so + " dòng)");
            return duongDan;
        }

        // =====================================================================
        // TIỆN ÍCH GIAO DIỆN
        // =====================================================================
        private void BaoThanhCong(string text)
        {
            lblTrangThai.Text = text;
            lblTrangThai.ForeColor = UiTheme.Green;
            lblLoi.Text = string.Empty;
            frmMain main = MdiParent as frmMain;
            if (main != null) { main.Log(text); main.YeuCauCapNhat(); }
        }

        private void BaoLoiNgan(string text)
        {
            lblLoi.Text = text;
            lblLoi.ForeColor = UiTheme.Red;
        }

        private void BaoLoi(string noi, Exception ex)
        {
            lblLoi.Text = "Lỗi khi " + noi.ToLowerInvariant() + ": " + ex.Message;
            lblLoi.ForeColor = UiTheme.Red;
            Console.WriteLine("[frmSinhVien] " + noi + ": " + ex);
        }
    }
}
