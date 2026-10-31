using System;
using System.Drawing;
using System.Windows.Forms;
using Lab08_SqlCommand_Parameters.BUS;
using Lab08_SqlCommand_Parameters.DTO;
using Lab08_SqlCommand_Parameters.GUI.Helpers;

namespace Lab08_SqlCommand_Parameters.GUI
{
    /// <summary>
    /// BAI 8 (tự thực hiện) - Xóa sinh viên có xác nhận.
    ///
    /// Dung ExecuteNonQuery cho DELETE. Tinh huong kho da hoc:
    ///   - Sinh viên còn đăng ký học phần -> SQL Server chặn bằng khóa ngoại
    ///     FK_DangKy_SV (SqlException.Number = 547).
    ///   - Form hoi nguoi dung: xóa cả đăng ký rồi xóa sinh viên, hoặc hủy.
    ///   - Không tìm thấy mã -> ExecuteNonQuery trả về 0 dòng.
    /// </summary>
    public class frmBai8 : BaiFormBase
    {
        private readonly SinhVienBUS _bus = new SinhVienBUS();

        private TextBox _txtMaSV;
        private Label _lblThongTin, _lblDangKy;
        private BadgeLabel _badge;

        public frmBai8()
            : base("8", "Xóa sinh viên có xác nhận",
                   "ExecuteNonQuery · DELETE · SqlException 547 (khóa ngoại)",
                   UiTheme.Red, "ExecuteNonQuery", "DELETE FROM dbo.SinhVien WHERE MaSV = @MaSV",
                   "Bài 8", new Size(780, 520))
        {
            DungGiaoDien();
        }

        private void DungGiaoDien()
        {
            var card = The(18, 18, 726, 190);
            card.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            Label lblTd = TieuDeMuc("Xóa sinh viên theo mã", UiTheme.Red);
            lblTd.SetBounds(20, 12, 400, 22);

            Label l1 = NhanTruong("Mã sinh viên cần xóa");
            l1.SetBounds(20, 42, 300, 20);

            _txtMaSV = TaoO("txtMaSV", 20, 64, 220, "Ví dụ: SV022");
            _txtMaSV.Font = UiTheme.FontMono(11f);
            _txtMaSV.TextChanged += (s, e) => XemTruoc();

            AccentButton btnKiemTra = Nut("btnKiemTra", "Kiểm tra", UiTheme.Slate, 252, 62, 120, 32, true);
            btnKiemTra.Click += (s, e) => XemTruoc();

            AccentButton btnXoa = Nut("btnXoa", "Xóa…", UiTheme.Red, 384, 62, 140, 32);
            btnXoa.Click += (s, e) => Xoa();

            _badge = new BadgeLabel();
            _badge.BaseColor = UiTheme.Slate;
            _badge.Value = "Chưa kiểm tra";
            _badge.SetBounds(560, 66, 152, 26);
            _badge.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            _lblThongTin = UiFactory.Label("—", 10.5f, UiTheme.Ink, true);
            _lblThongTin.SetBounds(20, 112, 692, 26);
            _lblThongTin.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            _lblDangKy = UiFactory.Label("", 9.5f, UiTheme.InkSoft, false);
            _lblDangKy.SetBounds(20, 142, 692, 40);
            _lblDangKy.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            card.Controls.AddRange(new Control[] { lblTd, l1, _txtMaSV, btnKiemTra, btnXoa, _badge, _lblThongTin, _lblDangKy });

            // ---- Cau lenh SQL ----
            Label lblSqlTd = TieuDeMuc("Câu lệnh đang thực thi", UiTheme.Red);
            lblSqlTd.SetBounds(18, 222, 400, 22);

            var lblSql = new Label();
            lblSql.Font = UiTheme.FontMono(9.5f);
            lblSql.ForeColor = UiTheme.InkSoft;
            lblSql.BackColor = Color.White;
            lblSql.BorderStyle = BorderStyle.FixedSingle;
            lblSql.Padding = new Padding(10);
            lblSql.SetBounds(18, 246, 726, 44);
            lblSql.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblSql.Text = "DELETE FROM dbo.SinhVien WHERE MaSV = @MaSV";

            Label lblGiaiThich = UiFactory.Label(
                "Nếu sinh viên còn đăng ký học phần, SQL Server trả SqlException số 547 (FOREIGN KEY).\n" +
                "Ứng dụng bắt mã lỗi này và hỏi người dùng có muốn xóa luôn các đăng ký hay không.\n" +
                "Trước khi xóa thật, luôn hiện hộp thoại xác nhận (MessageBox Yes/No).",
                9f, UiTheme.Muted, false);
            lblGiaiThich.SetBounds(18, 304, 726, 80);
            lblGiaiThich.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            NoiDung.Controls.AddRange(new Control[] { card, lblSqlTd, lblSql, lblGiaiThich });
        }

        /// <summary>Đọc trước thông tin sinh viên + số đăng ký để người dùng biết hậu quả.</summary>
        private void XemTruoc()
        {
            string ma = UiFactory.Val(_txtMaSV);
            if (ma.Length == 0)
            {
                _lblThongTin.Text = "—";
                _lblDangKy.Text = string.Empty;
                _badge.Value = "Chưa kiểm tra";
                _badge.SetColor(UiTheme.Slate);
                return;
            }

            try
            {
                SinhVien sv = _bus.TimTheoMa(ma);
                if (sv == null)
                {
                    _lblThongTin.Text = "Không tìm thấy sinh viên \"" + ma + "\".";
                    _lblDangKy.Text = string.Empty;
                    _badge.Value = "Không tồn tại";
                    _badge.SetColor(UiTheme.Amber);
                    return;
                }

                int soDangKy = _bus.DemDangKy(ma);
                _lblThongTin.Text = string.Format("{0} · {1} · {2} · điểm {3:0.00}",
                    sv.MaSV, sv.HoTen, sv.MaKhoa, sv.DiemTB);
                _lblDangKy.Text = soDangKy > 0
                    ? string.Format("Sinh viên này đang có {0} đăng ký học phần → xóa trực tiếp sẽ bị chặn bởi khóa ngoại (lỗi 547).", soDangKy)
                    : "Sinh viên này không còn đăng ký học phần → có thể xóa trực tiếp.";

                _badge.Value = soDangKy > 0 ? soDangKy + " đăng ký" : "Xóa được";
                _badge.SetColor(soDangKy > 0 ? UiTheme.Amber : UiTheme.Green);
            }
            catch (LoiNghiepVu ex)
            {
                _lblThongTin.Text = "—";
                _lblDangKy.Text = string.Empty;
                HienThiLoiNghiepVu(ex);
            }
            catch (Exception ex)
            {
                LoiChung(ex);
            }
        }

        private void Xoa()
        {
            string ma = UiFactory.Val(_txtMaSV);
            if (ma.Length == 0)
            {
                KetQua("Vui lòng nhập mã sinh viên cần xóa.", false);
                _txtMaSV.Focus();
                return;
            }

            DialogResult xacNhan = MessageBox.Show(this,
                "Bạn có chắc chắn muốn xóa sinh viên \"" + ma + "\"?\nThao tác này không thể hoàn tác.",
                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (xacNhan != DialogResult.Yes)
            {
                GhiChu("Người dùng đã hủy thao tác xóa (e.Cancel tương đương).");
                return;
            }

            try
            {
                int rows = _bus.XoaSinhVien(ma);
                if (rows > 0)
                {
                    KetQua("Đã xóa " + rows + " bản ghi sinh viên \"" + ma + "\".", true);
                    _badge.Value = "Đã xóa";
                    _badge.SetColor(UiTheme.Green);
                    UiFactory.SetVal(_txtMaSV, string.Empty);
                    _lblThongTin.Text = "—";
                    _lblDangKy.Text = string.Empty;
                }
                else
                {
                    KetQua("Không tìm thấy mã \"" + ma + "\" (ExecuteNonQuery trả về 0 dòng).", false);
                }
            }
            catch (LoiNghiepVu ex)
            {
                // Loi 547: con dang ky -> hoi nguoi dung xoa kem
                int soDangKy = 0;
                try { soDangKy = _bus.DemDangKy(ma); } catch (Exception) { }

                KetQua(ex.Message, false);
                _badge.Value = "Bị chặn (FK)";
                _badge.SetColor(UiTheme.Red);

                if (soDangKy > 0)
                {
                    DialogResult xoaKem = MessageBox.Show(this,
                        "Không thể xóa vì sinh viên còn " + soDangKy + " đăng ký học phần (khóa ngoại FK_DangKy_SV).\n\n" +
                        "Bạn có muốn xóa luôn " + soDangKy + " đăng ký này rồi xóa sinh viên không?",
                        "Vi phạm khóa ngoại (SqlException 547)",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                    if (xoaKem == DialogResult.Yes)
                    {
                        try
                        {
                            KetQuaThaoTac kq = _bus.XoaKemDangKy(ma);
                            KetQua(kq.ThongBao, kq.ThanhCong);
                            _badge.Value = "Đã xóa";
                            _badge.SetColor(UiTheme.Green);
                            UiFactory.SetVal(_txtMaSV, string.Empty);
                            _lblThongTin.Text = "—";
                            _lblDangKy.Text = string.Empty;
                        }
                        catch (LoiNghiepVu ex2)
                        {
                            HienThiLoiNghiepVu(ex2);
                        }
                    }
                    else
                    {
                        GhiChu("Đã hủy. Dữ liệu không thay đổi.");
                    }
                }
            }
            catch (Exception ex)
            {
                LoiChung(ex);
            }
        }
    }
}
