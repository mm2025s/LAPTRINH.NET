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
    /// BAI 7 - Sap xep theo lua chon (bai tu thuc hien).
    ///
    /// Diem mau chot cua bai nay: TEN COT KHONG THE truyen qua SqlParameter.
    /// Tham so chi thay cho GIA TRI ("WHERE HoTen = @HoTen"), khong thay cho
    /// ten cot hay ten bang -> buoc phai noi chuoi.
    ///
    /// Cach an toan dung trong bai:
    ///   1. ComboBox chi chua cac cot trong whitelist SinhVienCot.
    ///   2. Truoc khi noi vao ORDER BY, ten cot duoc doi chieu lai voi whitelist.
    ///   3. Chieu sap xep khong phai chuoi tu do ma la bool GiamDan.
    ///
    /// Nho vay du nguoi dung co sua duoc gia tri gui len, chuoi la cung khong
    /// bao gio lot vao cau lenh.
    /// </summary>
    public class frmBai7 : BaiFormBase
    {
        private readonly SinhVienBUS _bus = new SinhVienBUS();

        private LuoiSinhVien _luoi;
        private ComboBox _cboCot;
        private RadioButton _radTang;
        private RadioButton _radGiam;
        private Label _lblOrderBy;
        private TextBox _txtThuTanCong;

        public frmBai7()
            : base("7", "Sắp xếp theo lựa chọn",
                   "Chỉ dùng cột trong danh sách hợp lệ — tên cột KHÔNG truyền được qua SqlParameter",
                   UiTheme.Blue, "ORDER BY an toàn", "whitelist cột", "frmBai7",
                   new Size(1050, 720))
        {
            Badge = new BadgeLabel();
            Badge.Value = "whitelist ORDER BY";
            Badge.BaseColor = UiTheme.Blue;
            Badge.SetBounds(NoiDung.Right - 180, 20, 160, 24);

            TaoDieuKhien();
            TaoLuoi();
        }

        private void TaoDieuKhien()
        {
            CardPanel the = The(18, 18, 1002, 190);
            NoiDung.Controls.Add(the);

            Label tieuDe = TieuDeMuc("CHỌN CỘT VÀ CHIỀU SẮP XẾP", UiTheme.Blue);
            tieuDe.SetBounds(18, 14, 500, 22);
            the.Controls.Add(tieuDe);

            Label nhanCot = NhanTruong("Cột");
            nhanCot.SetBounds(18, 42, 120, 18);
            the.Controls.Add(nhanCot);

            _cboCot = UiFactory.ComboBox("Cot");
            _cboCot.SetBounds(18, 62, 230, 28);
            foreach (KeyValuePair<string, string> kv in SinhVienCot.DanhSach)
                _cboCot.Items.Add(new CotItem(kv.Key, kv.Value));
            _cboCot.SelectedIndex = 0;
            the.Controls.Add(_cboCot);

            Label nhanChieu = NhanTruong("Chiều");
            nhanChieu.SetBounds(266, 42, 120, 18);
            the.Controls.Add(nhanChieu);

            _radTang = new RadioButton();
            _radTang.Name = "radTang";
            _radTang.Text = "Tăng dần (ASC)";
            _radTang.Checked = true;
            _radTang.Font = UiTheme.FontBody(9.5f);
            _radTang.ForeColor = UiTheme.Ink;
            _radTang.SetBounds(266, 62, 140, 24);
            the.Controls.Add(_radTang);

            _radGiam = new RadioButton();
            _radGiam.Name = "radGiam";
            _radGiam.Text = "Giảm dần (DESC)";
            _radGiam.Font = UiTheme.FontBody(9.5f);
            _radGiam.ForeColor = UiTheme.Ink;
            _radGiam.SetBounds(266, 88, 140, 24);
            the.Controls.Add(_radGiam);

            AccentButton btnSapXep = Nut("btnSapXep", "Sắp xếp", UiTheme.Blue, 420, 62, 120, 36);
            btnSapXep.Click += (s, e) => SapXep();
            the.Controls.Add(btnSapXep);

            _lblOrderBy = UiFactory.Label("", 8.25f, UiTheme.Ink, false);
            _lblOrderBy.Font = UiTheme.FontMono(8.25f);
            _lblOrderBy.SetBounds(560, 62, 424, 26);
            the.Controls.Add(_lblOrderBy);

            // --- Khu thu "tan cong" ten cot
            Label nhanThu = NhanTruong("Thử truyền tên cột không hợp lệ (mô phỏng tấn công)");
            nhanThu.SetBounds(18, 108, 420, 18);
            the.Controls.Add(nhanThu);

            _txtThuTanCong = TaoO("ThuTanCong", 18, 128, 420, "sv.MaSV; DROP TABLE SinhVien --");
            the.Controls.Add(_txtThuTanCong);

            AccentButton btnThu = Nut("btnThuTanCong", "Thử sắp xếp", UiTheme.Red, 450, 126, 140, 32);
            btnThu.Click += (s, e) => ThuTanCong();
            the.Controls.Add(btnThu);

            Label ghiChu = UiFactory.Label(
                "Chuỗi trên bị ChuanHoa() loại bỏ, quay về MaSV. Nếu nối thẳng vào ORDER BY thì câu lệnh đã bị chèn.",
                8.25f, UiTheme.Muted, false);
            ghiChu.SetBounds(600, 130, 384, 40);
            the.Controls.Add(ghiChu);
        }

        private void TaoLuoi()
        {
            CardPanel the = The(18, 218, 1002, 388);
            the.Padding = new Padding(1);
            NoiDung.Controls.Add(the);

            _luoi = new LuoiSinhVien();
            _luoi.Dock = DockStyle.Fill;
            the.Controls.Add(_luoi);
        }

        private class CotItem
        {
            public string Cot { get; private set; }
            public string Nhan { get; private set; }

            public CotItem(string cot, string nhan) { Cot = cot; Nhan = nhan; }

            public override string ToString() { return Nhan; }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            SapXep();
        }

        private void SapXep()
        {
            try
            {
                var item = _cboCot.SelectedItem as CotItem;
                string cot = item == null ? SinhVienCot.MaSV : item.Cot;
                bool giamDan = _radGiam.Checked;

                DataTable table = _bus.SapXep(cot, giamDan);
                _luoi.GanDuLieu(table);

                string orderBy = SinhVienCot.BieuThucOrderBy(cot, giamDan);
                _lblOrderBy.Text = "ORDER BY " + orderBy;

                KetQua(string.Format("Sắp xếp theo {0} {1} — {2} dòng.",
                    TenCot(cot), giamDan ? "giảm dần" : "tăng dần", table.Rows.Count), true);
            }
            catch (Exception ex) { LoiChung(ex); }
        }

        private static string TenCot(string cot)
        {
            string nhan;
            return SinhVienCot.DanhSach.TryGetValue(cot, out nhan) ? nhan : cot;
        }

        /// <summary>
        /// Mo phong nguoi dung gui thang chuoi ten cot len server.
        /// ChuanHoa() phai dua ve MaSV -> ORDER BY khong doi, du lieu nguyen ven.
        /// </summary>
        private void ThuTanCong()
        {
            try
            {
                string docHai = UiFactory.Val(_txtThuTanCong);
                if (string.IsNullOrEmpty(docHai))
                {
                    KetQua("Hãy nhập chuỗi cần thử (ví dụ: sv.MaSV; DROP TABLE SinhVien --).", false);
                    return;
                }

                string cotAnToan = SinhVienCot.ChuanHoa(docHai);
                string orderBy = SinhVienCot.BieuThucOrderBy(docHai, _radGiam.Checked);

                DataTable table = _bus.SapXep(docHai, _radGiam.Checked);
                _luoi.GanDuLieu(table);

                _lblOrderBy.Text = "ORDER BY " + orderBy;

                bool biLoai = !string.Equals(cotAnToan, docHai.Trim(), StringComparison.OrdinalIgnoreCase);

                if (biLoai)
                    KetQua(string.Format(
                        "Đã chặn: \"{0}\" không có trong danh sách cột hợp lệ → dùng \"{1}\". Bảng còn {2} dòng.",
                        docHai, cotAnToan, table.Rows.Count), true);
                else
                    KetQua("\"" + docHai + "\" hợp lệ nên được chấp nhận (đúng thiết kế).", true);
            }
            catch (Exception ex) { LoiChung(ex); }
        }
    }
}
