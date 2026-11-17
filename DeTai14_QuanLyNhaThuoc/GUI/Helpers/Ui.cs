using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyNhaThuoc.BUS;

namespace QuanLyNhaThuoc.GUI.Helpers
{
    /// <summary>
    /// Bo dung giao dien dung chung: luoi, nut, khung nhap, thong bao loi.
    /// Moi Form dung Dock/TableLayoutPanel nen tu co gian theo kich thuoc cua so.
    /// </summary>
    public static class Ui
    {
        /// <summary>Che do chup anh / kiem tra tu dong: khong hien hop thoai chan luong.</summary>
        public static bool ChayTuDong { get; set; }

        /// <summary>Chup anh thuc te: van hien thong bao nhung tu dong dong y cac hop xac nhan Yes/No.</summary>
        public static bool TuXacNhan { get; set; }

        private static readonly Dictionary<string, string> TieuDeCot = new Dictionary<string, string>
        {
            { "MaThuoc", "Mã thuốc" }, { "TenThuoc", "Tên thuốc" }, { "TenNhom", "Nhóm" }, { "DonViTinh", "ĐVT" },
            { "GiaNhap", "Giá nhập" }, { "GiaBan", "Giá bán" }, { "TonKho", "Tồn kho" }, { "HanSuDung", "Hạn dùng" },
            { "CanKeDon", "Kê đơn" }, { "MaNhom", "Mã nhóm" }, { "SoThuoc", "Số thuốc" }, { "MaNCC", "Mã NCC" },
            { "TenNCC", "Tên nhà cung cấp" }, { "DienThoai", "Điện thoại" }, { "DiaChi", "Địa chỉ" }, { "MaKH", "Mã KH" },
            { "HoTen", "Họ tên" }, { "SoHoaDon", "Số HĐ" }, { "TongMua", "Tổng mua" }, { "MaHD", "Số HĐ" },
            { "NgayLap", "Ngày lập" }, { "NhanVien", "Nhân viên" }, { "KhachHang", "Khách hàng" }, { "TongTien", "Tổng tiền" },
            { "GhiChu", "Số đơn thuốc" }, { "SoLuong", "SL" }, { "DonGia", "Đơn giá" }, { "ThanhTien", "Thành tiền" },
            { "Ngay", "Ngày" }, { "DoanhThu", "Doanh thu" }, { "SoLuongBan", "SL bán" }, { "CanhBao", "Cảnh báo" },
            { "MaNV", "Mã NV" }, { "TenDangNhap", "Tên đăng nhập" }, { "VaiTro", "Vai trò" }, { "IsActive", "Hoạt động" },
            { "DonGiaNhap", "Giá nhập" }, { "ThoiGian", "Thời gian" }, { "HanhDong", "Hành động" }, { "DoiTuong", "Đối tượng" },
            { "ChiTiet", "Chi tiết" }, { "KetQua", "Kết quả" }, { "ThoiGianMs", "ms" }, { "TrungBinhHD", "TB / HĐ" },
            { "SoLan", "Số lần" }, { "SoLanLoi", "Lỗi" }, { "TrungBinhMs", "TB (ms)" }, { "LonNhatMs", "Max (ms)" }
        };

        private static readonly HashSet<string> CotTien = new HashSet<string>
            { "GiaNhap", "GiaBan", "TongMua", "TongTien", "DonGia", "ThanhTien", "DoanhThu", "DonGiaNhap", "TrungBinhHD" };

        public static DataGridView Luoi(string ten)
        {
            var g = new DataGridView
            {
                Name = ten,
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(240, 240, 242),
                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 40,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                Font = UiTheme.FontBody(9.5f)
            };
            g.RowTemplate.Height = 34;
            g.DefaultCellStyle.SelectionBackColor = UiTheme.PrimaryBg;
            g.DefaultCellStyle.SelectionForeColor = UiTheme.Ink;
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
            g.ColumnHeadersDefaultCellStyle.ForeColor = UiTheme.Ink;
            g.ColumnHeadersDefaultCellStyle.Font = UiTheme.FontBold(9f);
            g.DefaultCellStyle.Padding = new Padding(4, 0, 4, 0);
            g.ColumnHeadersDefaultCellStyle.Padding = new Padding(4, 0, 4, 0);
            g.ColumnHeadersDefaultCellStyle.SelectionBackColor = g.ColumnHeadersDefaultCellStyle.BackColor;
            g.DataBindingComplete += (s, e) => DinhDangCot(g);
            return g;
        }

        /// <summary>Gan DataTable hoac danh sach DTO; tieu de cot va dinh dang tu dat theo ten cot.</summary>
        public static void GanDuLieu(DataGridView g, object duLieu)
        {
            g.DataSource = null;
            g.DataSource = duLieu;
        }

        private static void DinhDangCot(DataGridView g)
        {
            foreach (DataGridViewColumn c in g.Columns)
            {
                string tieuDe;
                if (TieuDeCot.TryGetValue(c.DataPropertyName, out tieuDe)) c.HeaderText = tieuDe;
                if (CotTien.Contains(c.DataPropertyName))
                {
                    c.DefaultCellStyle.Format = "N0";
                    c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                else if (c.ValueType == typeof(DateTime))
                    c.DefaultCellStyle.Format = c.DataPropertyName == "NgayLap" || c.DataPropertyName == "ThoiGian" ? "dd/MM/yyyy HH:mm:ss" : "dd/MM/yyyy";
                else if (c.ValueType == typeof(int))
                    c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            if (g.Columns.Contains("MaNhom") && g.Columns.Contains("TenNhom")) g.Columns["MaNhom"].Visible = false;
            if (g.Columns.Contains("CanKeDon")) g.Columns["CanKeDon"].FillWeight = 45;
        }

        /// <summary>Dong dang chon cua luoi gan DataTable (null neu chua chon).</summary>
        public static DataRow DongChon(DataGridView g)
        {
            if (g.CurrentRow == null) return null;
            var v = g.CurrentRow.DataBoundItem as DataRowView;
            return v == null ? null : v.Row;
        }

        public static AccentButton Nut(string ten, string chu, Color mau, EventHandler xuLy, bool vien = false)
        {
            var b = new AccentButton { Name = ten, Text = chu, BaseColor = mau, OutlineOnly = vien, Height = 36, Margin = new Padding(0, 0, 8, 0) };
            b.Width = TextRenderer.MeasureText(chu, UiTheme.FontBold(9.75f)).Width + 36;
            if (xuLy != null) b.Click += xuLy;
            return b;
        }

        public static FlowLayoutPanel ThanhNut(params Control[] ds)
        {
            var p = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 52, Padding = new Padding(12, 8, 12, 8), BackColor = Color.White, WrapContents = false };
            foreach (Control c in ds)
            {
                c.Margin = new Padding(0, c is Label ? 8 : 0, 8, 0);
                p.Controls.Add(c);
            }
            return p;
        }

        public static TextBox O(string ten, int rong = 160)
        {
            return new TextBox { Name = ten, Width = rong, Font = UiTheme.FontBody(10f) };
        }

        public static ComboBox Combo(string ten, int rong = 180)
        {
            return new ComboBox { Name = ten, Width = rong, DropDownStyle = ComboBoxStyle.DropDownList, Font = UiTheme.FontBody(10f) };
        }

        public static Label Nhan(string chu, bool dam = false)
        {
            return new Label { Text = chu, AutoSize = true, Font = dam ? UiTheme.FontBold(9.5f) : UiTheme.FontBody(9.5f), ForeColor = UiTheme.InkSoft };
        }

        /// <summary>Khung nhap 2 cot nhan - o nhap, xep theo hang.</summary>
        public static TableLayoutPanel KhungNhap(int soCap, params object[] nhanVaControl)
        {
            var t = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(12, 10, 12, 6), BackColor = Color.White, ColumnCount = soCap * 2 };
            for (int i = 0; i < soCap; i++)
            {
                t.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / soCap));
            }
            for (int i = 0; i < nhanVaControl.Length; i += 2)
            {
                var nhan = Nhan((string)nhanVaControl[i]);
                nhan.Margin = new Padding(0, 8, 8, 4);
                var c = (Control)nhanVaControl[i + 1];
                c.Margin = new Padding(0, 4, 24, 4);
                if (!(c is CheckBox)) c.Dock = DockStyle.Fill;
                t.Controls.Add(nhan);
                t.Controls.Add(c);
            }
            return t;
        }

        public static Label TieuDeKhung(string chu)
        {
            return new Label { Text = chu.ToUpperInvariant(), Dock = DockStyle.Top, Height = 30, Padding = new Padding(12, 8, 0, 0), Font = UiTheme.FontBold(8.75f), ForeColor = UiTheme.Teal, BackColor = Color.White };
        }

        /// <summary>Hien loi theo loai; focus o nhap sai neu biet ten truong.</summary>
        public static void BaoLoi(Form f, Exception ex, string thaoTac)
        {
            ThongTinLoi tt = XuLyLoi.PhanLoai(ex, thaoTac);
            if (!string.IsNullOrEmpty(tt.Truong))
            {
                Control[] c = f.Controls.Find(tt.Truong, true);
                if (c.Length > 0) c[0].Focus();
            }
            if (ChayTuDong) return;
            MessageBox.Show(f, tt.ThongBao,
                tt.Loai == LoaiLoi.NhapLieu ? "Dữ liệu chưa hợp lệ" : tt.Loai == LoaiLoi.NghiepVu ? "Không thể thực hiện" : "Lỗi hệ thống",
                MessageBoxButtons.OK, tt.Loai == LoaiLoi.NhapLieu || tt.Loai == LoaiLoi.NghiepVu ? MessageBoxIcon.Warning : MessageBoxIcon.Error);
        }

        public static void ThongBao(Form f, string chu)
        {
            if (!ChayTuDong) MessageBox.Show(f, chu, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static bool XacNhan(Form f, string chu)
        {
            return ChayTuDong || TuXacNhan || MessageBox.Show(f, chu, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes;
        }

        /// <summary>Doc so nguyen duong tu o nhap.</summary>
        public static decimal DocSo(TextBox o, string ten)
        {
            decimal v;
            string s = o.Text.Trim().Replace(".", "").Replace(",", "");
            if (!decimal.TryParse(s, out v)) throw new DTO.LoiNhapLieu(o.Name, ten + " phải là số.");
            return v;
        }
    }

    /// <summary>
    /// Lop co so cua cac trang chuc nang. Theo bo cuc hcmue-portal: tieu de trang +
    /// mo ta nho tren nen xam, noi dung trong cac khoi trang. Trang duoc nhung vao
    /// vung noi dung cua frmMain (NhungVao), van mo rieng duoc nhu Form thuong.
    /// </summary>
    public class ManHinh : FormBase
    {
        protected readonly Panel NoiDung;

        /// <summary>Ten trang hien tren breadcrumb.</summary>
        public string TieuDe { get; private set; }

        public ManHinh(string tieuDe, string phuDe, Color mau, Size kichThuoc)
        {
            TieuDe = tieuDe;
            Text = tieuDe + " - Quản lý nhà thuốc";
            ClientSize = kichThuoc;
            MinimumSize = new Size(900, 560);
            StartPosition = FormStartPosition.CenterParent;
            ShowIcon = false;
            BackColor = UiTheme.Canvas;

            var dau = new Panel { Dock = DockStyle.Top, Height = 76, BackColor = UiTheme.Canvas };
            dau.Controls.Add(new Label { Text = tieuDe, AutoSize = true, Location = new Point(24, 18), Font = UiTheme.FontBold(16f), ForeColor = UiTheme.Ink });
            dau.Controls.Add(new Label { Text = phuDe, AutoSize = true, Location = new Point(26, 50), Font = UiTheme.FontBody(9.25f), ForeColor = UiTheme.Muted });

            // Khung trang co vien mong, chua toan bo noi dung cua trang.
            NoiDung = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            var vo = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24, 0, 24, 24), BackColor = UiTheme.Canvas };
            var vien = new Panel { Dock = DockStyle.Fill, Padding = new Padding(1), BackColor = UiTheme.Border };
            vien.Controls.Add(NoiDung);
            vo.Controls.Add(vien);

            // Fill them TRUOC, Top them SAU: WinForms xep Dock tu cuoi danh sach len.
            Controls.Add(vo);
            Controls.Add(dau);
        }

        /// <summary>Nhung trang vao vung noi dung cua cua so chinh.</summary>
        public void NhungVao(Control vungChua)
        {
            TopLevel = false;
            MinimumSize = Size.Empty;   // trang co gian theo vung noi dung, ke ca man hinh nho
            FormBorderStyle = FormBorderStyle.None;
            Dock = DockStyle.Fill;
            vungChua.Controls.Add(this);
            Show();
        }
    }
}
