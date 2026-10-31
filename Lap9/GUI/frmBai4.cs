using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Lab09_DataReaderSearch.BUS;
using Lab09_DataReaderSearch.DTO;
using Lab09_DataReaderSearch.GUI.Helpers;

namespace Lab09_DataReaderSearch.GUI
{
    /// <summary>
    /// BAI 4 - Phan trang bang OFFSET - FETCH.
    ///
    /// Vi sao phai co ca COUNT(*): OFFSET-FETCH chi tra ve MOT trang. Neu khong
    /// dem tong so dong thi giao dien khong biet con trang sau hay khong, khong
    /// tinh duoc "Trang 3/5".
    ///
    /// Vi sao ORDER BY co them sv.MaSV: neu cot sap xep bi trung gia tri, thu tu
    /// giua cac dong la khong xac dinh -> mot dong co the xuat hien o 2 trang.
    /// </summary>
    public class frmBai4 : BaiFormBase
    {
        private readonly SinhVienBUS _bus = new SinhVienBUS();

        private LuoiSinhVien _luoi;
        private Label _lblTomTat;
        private Label _lblOffset;
        private TextBox _txtTrang;
        private ComboBox _cboKichThuoc;
        private ComboBox _cboSapXep;
        private CheckBox _chkGiamDan;

        private int _chiSoTrang = 1;
        private int _tongSoTrang = 1;

        public frmBai4()
            : base("4", "Phân trang bằng OFFSET – FETCH",
                   "OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY (ORDER BY bắt buộc)",
                   UiTheme.Amber, "OFFSET / FETCH", "COUNT(*) để tính số trang", "frmBai4",
                   new Size(1010, 700))
        {
            Badge = new BadgeLabel();
            Badge.Value = "OFFSET – FETCH";
            Badge.BaseColor = UiTheme.Amber;
            Badge.SetBounds(NoiDung.Right - 160, 20, 140, 24);

            TaoDieuKhien();
            TaoLuoi();
        }

        private void TaoDieuKhien()
        {
            CardPanel the = The(18, 18, 962, 148);
            NoiDung.Controls.Add(the);

            Label tieuDe = TieuDeMuc("ĐIỀU HƯỚNG TRANG", UiTheme.Amber);
            tieuDe.SetBounds(18, 14, 300, 22);
            the.Controls.Add(tieuDe);

            AccentButton btnTruoc = Nut("btnTrangTruoc", "◀  Trước", UiTheme.Slate, 18, 46, 110, 34, true);
            btnTruoc.Click += (s, e) => ChuyenTrang(_chiSoTrang - 1);
            the.Controls.Add(btnTruoc);

            Label nhanTrang = NhanTruong("Trang");
            nhanTrang.SetBounds(140, 42, 46, 20);
            the.Controls.Add(nhanTrang);

            _txtTrang = TaoO("Trang", 140, 62, 54, "1");
            _txtTrang.TextAlign = HorizontalAlignment.Center;
            _txtTrang.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    int so;
                    if (int.TryParse(UiFactory.Val(_txtTrang), out so)) ChuyenTrang(so);
                }
            };
            the.Controls.Add(_txtTrang);

            AccentButton btnSau = Nut("btnTrangSau", "Sau  ▶", UiTheme.Amber, 204, 60, 110, 34);
            btnSau.Click += (s, e) => ChuyenTrang(_chiSoTrang + 1);
            the.Controls.Add(btnSau);

            // Kich thuoc trang
            Label nhanKT = NhanTruong("Số dòng/trang");
            nhanKT.SetBounds(336, 42, 110, 20);
            the.Controls.Add(nhanKT);

            _cboKichThuoc = UiFactory.ComboBox("KichThuocTrang");
            _cboKichThuoc.SetBounds(336, 62, 90, 28);
            _cboKichThuoc.Items.AddRange(new object[] { 5, 10, 20, 46 });
            _cboKichThuoc.SelectedIndex = 1;
            _cboKichThuoc.SelectedIndexChanged += (s, e) => ChuyenTrang(1);
            the.Controls.Add(_cboKichThuoc);

            // Sap xep (bai 7 dung lai whitelist nay)
            Label nhanSX = NhanTruong("Sắp xếp theo");
            nhanSX.SetBounds(446, 42, 120, 20);
            the.Controls.Add(nhanSX);

            _cboSapXep = UiFactory.ComboBox("CotSapXep");
            _cboSapXep.SetBounds(446, 62, 190, 28);
            foreach (KeyValuePair<string, string> kv in SinhVienCot.DanhSach)
                _cboSapXep.Items.Add(new CotItem(kv.Key, kv.Value));
            _cboSapXep.SelectedIndex = 0;
            _cboSapXep.SelectedIndexChanged += (s, e) => ChuyenTrang(1);
            the.Controls.Add(_cboSapXep);

            _chkGiamDan = new CheckBox();
            _chkGiamDan.Name = "chkGiamDan";
            _chkGiamDan.Text = "Giảm dần";
            _chkGiamDan.Font = UiTheme.FontBody(9.25f);
            _chkGiamDan.ForeColor = UiTheme.InkSoft;
            _chkGiamDan.SetBounds(648, 66, 90, 24);
            _chkGiamDan.CheckedChanged += (s, e) => ChuyenTrang(1);
            the.Controls.Add(_chkGiamDan);

            AccentButton btnDau = Nut("btnTrangDau", "Trang đầu", UiTheme.Indigo, 750, 60, 100, 34, true);
            btnDau.Click += (s, e) => ChuyenTrang(1);
            the.Controls.Add(btnDau);

            AccentButton btnCuoi = Nut("btnTrangCuoi", "Trang cuối", UiTheme.Indigo, 858, 60, 100, 34, true);
            btnCuoi.Click += (s, e) => ChuyenTrang(_tongSoTrang);
            the.Controls.Add(btnCuoi);

            _lblTomTat = UiFactory.Label("", 9.25f, UiTheme.Ink, true);
            _lblTomTat.SetBounds(18, 106, 480, 24);
            the.Controls.Add(_lblTomTat);

            _lblOffset = UiFactory.Label("", 8.25f, UiTheme.Muted, false);
            _lblOffset.Font = UiTheme.FontMono(8f);
            _lblOffset.SetBounds(504, 106, 454, 24);
            the.Controls.Add(_lblOffset);
        }

        private void TaoLuoi()
        {
            CardPanel the = The(18, 176, 962, 430);
            the.Padding = new Padding(1);
            NoiDung.Controls.Add(the);

            _luoi = new LuoiSinhVien();
            _luoi.Dock = DockStyle.Fill;
            the.Controls.Add(_luoi);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            ChuyenTrang(1);
        }

        /// <summary>Item cho ComboBox cot sap xep: hien thi nhan, giu ten cot that.</summary>
        private class CotItem
        {
            public string Cot { get; private set; }
            public string Nhan { get; private set; }

            public CotItem(string cot, string nhan)
            {
                Cot = cot;
                Nhan = nhan;
            }

            public override string ToString() { return Nhan; }
        }

        private int KichThuocTrang()
        {
            object v = _cboKichThuoc.SelectedItem;
            return v == null ? SinhVienBUS.KichThuocTrangMacDinh : Convert.ToInt32(v);
        }

        private string CotSapXep()
        {
            var item = _cboSapXep.SelectedItem as CotItem;
            return item == null ? SinhVienCot.MaSV : item.Cot;
        }

        private void ChuyenTrang(int trangMoi)
        {
            try
            {
                int kichThuoc = KichThuocTrang();

                if (trangMoi < 1) trangMoi = 1;

                KetQuaPhanTrang kq = _bus.LoadTrang(trangMoi, kichThuoc, CotSapXep(), _chkGiamDan.Checked);

                // Chặn trên chỉ biết được SAU khi đã đếm tổng số dòng.
                if (kq.ChiSoTrang > kq.TongSoTrang)
                {
                    ChuyenTrang(kq.TongSoTrang);
                    return;
                }

                _chiSoTrang = kq.ChiSoTrang;
                _tongSoTrang = kq.TongSoTrang;

                _luoi.GanDuLieu(kq.Bang as DataTable);

                UiFactory.SetVal(_txtTrang, _chiSoTrang.ToString());
                _lblTomTat.Text = kq.MoTa;

                int offset = (_chiSoTrang - 1) * kichThuoc;
                _lblOffset.Text = string.Format("OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", offset, kichThuoc);

                // Bat/tat nut dieu huong theo trang hien tai
                var btnTruoc = Tim("btnTrangTruoc") as Control;
                var btnSau = Tim("btnTrangSau") as Control;
                if (btnTruoc != null) btnTruoc.Enabled = kq.CoTrangTruoc;
                if (btnSau != null) btnSau.Enabled = kq.CoTrangSau;

                KetQua(string.Format("Đang xem {0} — nhảy cóc offset {1}.", kq.MoTa, offset), true);
            }
            catch (Exception ex)
            {
                LoiChung(ex);
            }
        }
    }
}
