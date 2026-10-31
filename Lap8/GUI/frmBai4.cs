using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Lab08_SqlCommand_Parameters.BUS;
using Lab08_SqlCommand_Parameters.DTO;
using Lab08_SqlCommand_Parameters.GUI.Helpers;

namespace Lab08_SqlCommand_Parameters.GUI
{
    /// <summary>
    /// BAI 4 - Cap nhat DiemTB theo MaSV bang tham so.
    ///
    /// KHONG ghep ma sinh vien vao chuoi SQL. Neu viet:
    ///     "UPDATE ... WHERE MaSV = '" + txtMaSV.Text + "'"
    /// nguoi dung co the nhap  ' OR '1'='1  -> cap nhat toan bo bang.
    /// Dung @MaSV lam tham so thi chuoi do chi la DU LIEU, khong phai lenh.
    ///
    /// Khi ExecuteNonQuery tra ve 0 dong -> khong tim thay ma sinh vien.
    /// </summary>
    public class frmBai4 : BaiFormBase
    {
        private readonly SinhVienBUS _bus = new SinhVienBUS();

        private TextBox _txtMaSV, _txtDiem;
        private ComboBox _cboChonNhanh;
        private Label _lblHienTai, _lblSoDong;
        private bool _dangNapCombo;

        public frmBai4()
            : base("4", "Cập nhật điểm bằng tham số",
                   "UPDATE ... WHERE MaSV = @MaSV · rows = 0 → không tìm thấy",
                   UiTheme.Amber, "ExecuteNonQuery", "SqlDbType.Decimal { Precision = 4, Scale = 2 }",
                   "Bài 4", new Size(760, 540))
        {
            DungGiaoDien();
            Load += frmBai4_Load;
        }

        private void DungGiaoDien()
        {
            var card = The(18, 18, 706, 190);
            card.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            Label lblTieuDe = TieuDeMuc("Cập nhật điểm trung bình", UiTheme.Amber);
            lblTieuDe.SetBounds(20, 12, 500, 22);

            Label l1 = NhanTruong("Mã sinh viên cần cập nhật");
            l1.SetBounds(20, 42, 300, 20);

            _txtMaSV = TaoO("txtMaSV", 20, 64, 200, "Ví dụ: SV001");

            _cboChonNhanh = UiFactory.ComboBox("cboChonNhanh");
            _cboChonNhanh.SetBounds(232, 64, 250, 28);
            _cboChonNhanh.SelectedIndexChanged += cboChonNhanh_SelectedIndexChanged;

            Badge = new BadgeLabel();
            Badge.BaseColor = UiTheme.Amber;
            Badge.Value = "Chọn nhanh";
            Badge.SetBounds(500, 64, 130, 28);
            Badge.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            Label l2 = NhanTruong("Điểm trung bình mới (0 - 10)");
            l2.SetBounds(20, 104, 300, 20);

            _txtDiem = TaoO("txtDiem", 20, 126, 200, "Ví dụ: 9.25");

            _lblHienTai = UiFactory.Label("Điểm hiện tại: —", 9.5f, UiTheme.InkSoft, false);
            _lblHienTai.SetBounds(232, 130, 404, 24);
            _lblHienTai.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            card.Controls.AddRange(new Control[] { lblTieuDe, l1, _txtMaSV, _cboChonNhanh, Badge, l2, _txtDiem, _lblHienTai });

            // ---- Cau lenh SQL ----
            Label lblSqlTd = TieuDeMuc("Câu lệnh đang thực thi", UiTheme.Amber);
            lblSqlTd.SetBounds(18, 222, 400, 22);

            var lblSql = new Label();
            lblSql.Font = UiTheme.FontMono(9.5f);
            lblSql.ForeColor = UiTheme.InkSoft;
            lblSql.BackColor = Color.White;
            lblSql.BorderStyle = BorderStyle.FixedSingle;
            lblSql.Padding = new Padding(10);
            lblSql.SetBounds(18, 246, 706, 44);
            lblSql.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblSql.Text = "UPDATE dbo.SinhVien SET DiemTB = @DiemTB WHERE MaSV = @MaSV";

            AccentButton btnCapNhat = Nut("btnCapNhat", "Cập nhật điểm", UiTheme.Amber, 18, 306, 180);
            btnCapNhat.Click += (s, e) => CapNhat();

            AccentButton btnXem = Nut("btnXem", "Xem điểm hiện tại", UiTheme.Slate, 208, 306, 180, 36, true);
            btnXem.Click += (s, e) => XemDiem();

            _lblSoDong = UiFactory.Label("", 10f, UiTheme.Ink, true);
            _lblSoDong.SetBounds(404, 312, 320, 26);
            _lblSoDong.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _lblSoDong.TextAlign = ContentAlignment.MiddleRight;

            Label lblGiaiThich = UiFactory.Label(
                "Thử nhập mã \"' OR '1'='1\" để thấy câu lệnh tham số hóa KHÔNG bị chèn lệnh: " +
                "chuỗi đó chỉ được so sánh như một giá trị MaSV bình thường.\n" +
                "SqlParameter @DiemTB khai báo Precision = 4, Scale = 2 khớp cột DECIMAL(4,2).",
                9f, UiTheme.Muted, false);
            lblGiaiThich.SetBounds(18, 356, 706, 60);
            lblGiaiThich.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            NoiDung.Controls.AddRange(new Control[] { card, lblSqlTd, lblSql, btnCapNhat, btnXem, _lblSoDong, lblGiaiThich });
        }

        private void frmBai4_Load(object sender, EventArgs e)
        {
            try
            {
                NapComboChonNhanh();
            }
            catch (LoiNghiepVu ex) { HienThiLoiNghiepVu(ex); }
            catch (Exception ex) { LoiChung(ex); }
        }

        private void NapComboChonNhanh()
        {
            List<SinhVien> ds = _bus.DocDanhSach();

            _dangNapCombo = true;
            _cboChonNhanh.Items.Clear();
            _cboChonNhanh.Items.Add("— Chọn mã sinh viên —");
            foreach (SinhVien sv in ds) _cboChonNhanh.Items.Add(sv);
            _cboChonNhanh.SelectedIndex = 0;
            _dangNapCombo = false;
        }

        private void cboChonNhanh_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_dangNapCombo) return;

            var sv = _cboChonNhanh.SelectedItem as SinhVien;
            if (sv == null) return;

            UiFactory.SetVal(_txtMaSV, sv.MaSV);
            UiFactory.SetVal(_txtDiem, sv.DiemTB.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture));
            _lblHienTai.Text = "Điểm hiện tại của " + sv.HoTen + ": " + sv.DiemTB.ToString("0.00");
        }

        private void CapNhat()
        {
            try
            {
                KetQuaThaoTac kq = _bus.CapNhatDiem(UiFactory.Val(_txtMaSV), UiFactory.Val(_txtDiem));
                _lblSoDong.Text = "ExecuteNonQuery → " + kq.SoDong + " dòng";
                KetQua(kq.ThongBao, kq.ThanhCong);
            }
            catch (LoiNghiepVu ex)
            {
                HienThiLoi(ex, Tim);
            }
            catch (Exception ex)
            {
                LoiChung(ex);
            }
        }

        private void XemDiem()
        {
            try
            {
                SinhVien sv = _bus.TimTheoMa(UiFactory.Val(_txtMaSV));
                if (sv == null)
                {
                    _lblHienTai.Text = "Không tìm thấy sinh viên này.";
                    KetQua("Không tìm thấy mã \"" + UiFactory.Val(_txtMaSV) + "\".", false);
                    return;
                }

                _lblHienTai.Text = string.Format("{0} · {1} · điểm {2:0.00}",
                    sv.MaSV, sv.HoTen, sv.DiemTB);
                UiFactory.SetVal(_txtDiem, sv.DiemTB.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture));
                KetQua("Đã đọc điểm hiện tại bằng ExecuteReader.", true);
            }
            catch (LoiNghiepVu ex)
            {
                HienThiLoi(ex, Tim);
            }
            catch (Exception ex)
            {
                LoiChung(ex);
            }
        }
    }
}
