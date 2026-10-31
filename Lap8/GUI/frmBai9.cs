using System;
using System.Drawing;
using System.Windows.Forms;
using Lab08_SqlCommand_Parameters.BUS;
using Lab08_SqlCommand_Parameters.DTO;
using Lab08_SqlCommand_Parameters.GUI.Helpers;

namespace Lab08_SqlCommand_Parameters.GUI
{
    /// <summary>
    /// BAI 9 (tự thực hiện) - Tham số đầu ra (OUTPUT parameter).
    ///
    /// Thu tuc sp_SinhVien_Them vua INSERT vua tra ve tong so sinh vien qua
    /// @SoLuongHienTai. Phia C# phai:
    ///   1. Khai bao SqlParameter voi Direction = ParameterDirection.Output
    ///      TRUOC khi goi ExecuteNonQuery.
    ///   2. Sau khi lenh chay xong moi doc p.Value (luc chua chay, Value = null).
    /// </summary>
    public class frmBai9 : BaiFormBase
    {
        private readonly SinhVienBUS _bus = new SinhVienBUS();

        private NhapSinhVienPanel _nhap;
        private Label _lblOutput;
        private Label _lblTongTruoc;
        private BadgeLabel _badge;

        public frmBai9()
            : base("9", "Tham số đầu ra (OUTPUT parameter)",
                   "sp_SinhVien_Them · ParameterDirection.Output · @SoLuongHienTai",
                   UiTheme.Orange, "StoredProcedure + OUTPUT", "INSERT ...; SELECT @SoLuongHienTai = COUNT(*)",
                   "Bài 9", new Size(860, 640))
        {
            DungGiaoDien();
            Load += frmBai9_Load;
        }

        private void DungGiaoDien()
        {
            var card = The(18, 18, 806, 226);
            card.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            Label lblTd = TieuDeMuc("Dữ liệu sinh viên thêm qua thủ tục", UiTheme.Orange);
            lblTd.SetBounds(20, 12, 500, 22);

            _nhap = new NhapSinhVienPanel();
            _nhap.SetBounds(20, 38, 800, 210);
            _nhap.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            card.Controls.AddRange(new Control[] { lblTd, _nhap });

            // ---- Ket qua OUTPUT ----
            var cardOut = The(18, 258, 806, 96);
            cardOut.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            Label lblOutTd = TieuDeMuc("Giá trị nhận qua tham số OUTPUT", UiTheme.Orange);
            lblOutTd.SetBounds(20, 10, 500, 22);

            _lblOutput = new Label();
            _lblOutput.Name = "lblOutput";
            _lblOutput.Font = UiTheme.FontTitle(17f);
            _lblOutput.ForeColor = UiTheme.Ink;
            _lblOutput.BackColor = Color.Transparent;
            _lblOutput.SetBounds(20, 36, 760, 36);
            _lblOutput.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _lblOutput.Text = "@SoLuongHienTai = —";

            cardOut.Controls.AddRange(new Control[] { lblOutTd, _lblOutput });

            _badge = new BadgeLabel();
            _badge.BaseColor = UiTheme.Orange;
            _badge.Value = "ParameterDirection.Output";
            _badge.SetBounds(18, 366, 230, 26);

            _lblTongTruoc = UiFactory.Label("", 9.75f, UiTheme.InkSoft, false);
            _lblTongTruoc.SetBounds(260, 366, 564, 26);
            _lblTongTruoc.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            AccentButton btnThem = Nut("btnThem", "Thêm qua thủ tục (OUTPUT)", UiTheme.Orange, 18, 402, 240);
            btnThem.Click += (s, e) => Them();

            AccentButton btnXoaTrang = Nut("btnXoaTrang", "Xóa trắng", UiTheme.Slate, 268, 402, 120, 36, true);
            btnXoaTrang.Click += (s, e) => { _nhap.XoaTrang(); GhiChu("Đã xóa trắng các ô nhập."); };

            AccentButton btnDem = Nut("btnDem", "Đếm hiện tại", UiTheme.Violet, 398, 402, 140, 36, true);
            btnDem.Click += (s, e) => CapNhatTongTruoc();

            Label lblGiaiThich = UiFactory.Label(
                "SqlParameter pOut = new SqlParameter(\"@SoLuongHienTai\", SqlDbType.Int);\r\n" +
                "pOut.Direction = ParameterDirection.Output;   // khai báo TRƯỚC khi ExecuteNonQuery\r\n" +
                "cmd.ExecuteNonQuery();  int tong = Convert.ToInt32(pOut.Value);   // đọc SAU khi chạy",
                9f, UiTheme.InkSoft, false);
            lblGiaiThich.Font = UiTheme.FontMono(9f);
            lblGiaiThich.SetBounds(18, 448, 806, 60);
            lblGiaiThich.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            NoiDung.Controls.AddRange(new Control[]
                { card, cardOut, _badge, _lblTongTruoc, btnThem, btnXoaTrang, btnDem, lblGiaiThich });
        }

        private void frmBai9_Load(object sender, EventArgs e)
        {
            try
            {
                _nhap.NapKhoa(_bus.DanhSachKhoa());
                CapNhatTongTruoc();
            }
            catch (LoiNghiepVu ex) { HienThiLoiNghiepVu(ex); }
            catch (Exception ex) { LoiChung(ex); }
        }

        private void CapNhatTongTruoc()
        {
            try
            {
                int n = _bus.CountStudents();
                _lblTongTruoc.Text = "Tổng số sinh viên trước khi thêm: " + n;
            }
            catch (LoiNghiepVu)
            {
                _lblTongTruoc.Text = "Tổng số sinh viên: ?";
            }
        }

        private void Them()
        {
            try
            {
                SinhVien sv = _nhap.LaySinhVien();
                KetQuaThaoTac kq = _bus.ThemQuaThuTuc(sv);

                _lblOutput.Text = "@SoLuongHienTai = " + (kq.GiaTriOutput.HasValue ? kq.GiaTriOutput.Value.ToString() : "NULL");
                _badge.Value = "Đã nhận OUTPUT";
                _badge.SetColor(UiTheme.Green);

                KetQua(kq.ThongBao, true);
                _nhap.XoaTrang();
                CapNhatTongTruoc();
            }
            catch (LoiNghiepVu ex)
            {
                _badge.Value = "Lỗi";
                _badge.SetColor(UiTheme.Red);
                HienThiLoi(ex, Tim);
            }
            catch (Exception ex)
            {
                LoiChung(ex);
            }
        }
    }
}
