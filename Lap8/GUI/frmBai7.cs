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
    /// BAI 7 (tự thực hiện) - Thống kê điểm cao nhất bằng ExecuteScalar.
    ///
    /// MAX(DiemTB) tra ve NULL khi khoa chua co sinh vien. ExecuteScalar cung
    /// tra ve DBNull.Value trong truong hop do. Neu ep kieu truc tiep:
    ///     decimal d = (decimal)cmd.ExecuteScalar();   -> InvalidCastException
    /// Phai kiem tra DBNull truoc khi chuyen doi (xem SinhVienDAL.DiemCaoNhatCuaKhoa).
    ///
    /// Form nay so sanh 2 cach doc cung mot con so:
    ///   - ExecuteScalar : SELECT MAX(DiemTB) ...
    ///   - ExecuteReader : SELECT COUNT(*), MAX(...), AVG(...) theo khoa
    /// </summary>
    public class frmBai7 : BaiFormBase
    {
        private readonly SinhVienBUS _bus = new SinhVienBUS();

        private DataGridView _luoi;
        private ComboBox _cboKhoa;
        private Label _lblKetQuaScalar;
        private BadgeLabel _badge;

        public frmBai7()
            : base("7", "Thống kê điểm cao nhất",
                   "ExecuteScalar · MAX(DiemTB) · xử lý DBNull / NULL",
                   UiTheme.Green, "ExecuteScalar + ExecuteReader", "SELECT MAX(DiemTB) FROM dbo.SinhVien WHERE MaKhoa = @MaKhoa",
                   "Bài 7", new Size(880, 600))
        {
            DungGiaoDien();
            Load += frmBai7_Load;
        }

        private void DungGiaoDien()
        {
            Label lblChon = UiFactory.Label("Khoa:", 9.75f, UiTheme.Ink, true);
            lblChon.SetBounds(18, 22, 60, 26);

            _cboKhoa = UiFactory.ComboBox("cboKhoa");
            _cboKhoa.SetBounds(78, 22, 240, 28);
            _cboKhoa.DisplayMember = "TenKhoa";
            _cboKhoa.ValueMember = "MaKhoa";
            _cboKhoa.SelectedIndexChanged += (s, e) => LayMotKhoa();

            AccentButton btnScalar = Nut("btnScalar", "MAX bằng ExecuteScalar", UiTheme.Green, 328, 18, 200, 36);
            btnScalar.Click += (s, e) => LayMotKhoa();

            AccentButton btnTatCa = Nut("btnTatCa", "Thống kê tất cả khoa", UiTheme.Teal, 538, 18, 190, 36, true);
            btnTatCa.Click += (s, e) => LayTatCa();

            _badge = new BadgeLabel();
            _badge.BaseColor = UiTheme.Green;
            _badge.Value = "0 khoa";
            _badge.SetBounds(738, 22, 120, 28);
            _badge.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            // ---- Ket qua ExecuteScalar cho 1 khoa ----
            var cardScalar = The(18, 66, 844, 84);
            cardScalar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            Label lblTd = TieuDeMuc("Kết quả ExecuteScalar cho khoa đang chọn", UiTheme.Green);
            lblTd.SetBounds(20, 10, 500, 22);

            _lblKetQuaScalar = new Label();
            _lblKetQuaScalar.Name = "lblKetQuaScalar";
            _lblKetQuaScalar.Font = UiTheme.FontTitle(17f);
            _lblKetQuaScalar.ForeColor = UiTheme.Ink;
            _lblKetQuaScalar.BackColor = Color.Transparent;
            _lblKetQuaScalar.SetBounds(20, 36, 800, 38);
            _lblKetQuaScalar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _lblKetQuaScalar.Text = "—";
            cardScalar.Controls.AddRange(new Control[] { lblTd, _lblKetQuaScalar });

            // ---- Bang thong ke ----
            _luoi = new DataGridView();
            _luoi.Name = "luoiThongKe";
            _luoi.SetBounds(18, 166, 844, 330);
            _luoi.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _luoi.BackgroundColor = Color.White;
            _luoi.BorderStyle = BorderStyle.FixedSingle;
            _luoi.RowHeadersVisible = false;
            _luoi.AllowUserToAddRows = false;
            _luoi.ReadOnly = true;
            _luoi.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _luoi.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _luoi.Font = UiTheme.FontBody(9.5f);
            _luoi.ColumnHeadersDefaultCellStyle.Font = UiTheme.FontBold(9.5f);
            _luoi.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(226, 244, 233);
            _luoi.EnableHeadersVisualStyles = false;
            _luoi.RowTemplate.Height = 28;
            _luoi.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 253, 250);
            _luoi.DataError += (s, e) => { e.ThrowException = false; };

            Label lblGiaiThich = UiFactory.Label(
                "Khoa chưa có sinh viên → MAX(DiemTB) = NULL → ExecuteScalar trả về DBNull.Value.\n" +
                "Cột “Điểm cao nhất” hiển thị “— (NULL)” thay vì làm ứng dụng bị lỗi ép kiểu.",
                9f, UiTheme.Muted, false);
            lblGiaiThich.SetBounds(18, 502, 844, 44);
            lblGiaiThich.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            NoiDung.Controls.AddRange(new Control[] { lblChon, _cboKhoa, btnScalar, btnTatCa, _badge, cardScalar, _luoi, lblGiaiThich });
        }

        private void frmBai7_Load(object sender, EventArgs e)
        {
            try
            {
                _cboKhoa.DataSource = _bus.DanhSachKhoa();
                _cboKhoa.DisplayMember = "TenKhoa";
                _cboKhoa.ValueMember = "MaKhoa";
                LayTatCa();
            }
            catch (LoiNghiepVu ex) { HienThiLoiNghiepVu(ex); }
            catch (Exception ex) { LoiChung(ex); }
        }

        /// <summary>Doc MAX(DiemTB) cua 1 khoa bang ExecuteScalar.</summary>
        private void LayMotKhoa()
        {
            string maKhoa = _cboKhoa.SelectedValue == null ? string.Empty : Convert.ToString(_cboKhoa.SelectedValue);
            if (string.IsNullOrEmpty(maKhoa)) return;

            try
            {
                decimal? caoNhat = _bus.DiemCaoNhatCuaKhoa(maKhoa);

                if (caoNhat.HasValue)
                {
                    _lblKetQuaScalar.Text = string.Format("Khoa {0}: điểm cao nhất = {1:0.00}", maKhoa, caoNhat.Value);
                    _lblKetQuaScalar.ForeColor = UiTheme.Ink;
                    KetQua(string.Format("ExecuteScalar trả về decimal {0:0.00} cho khoa {1}.", caoNhat.Value, maKhoa), true);
                }
                else
                {
                    _lblKetQuaScalar.Text = "Khoa " + maKhoa + ": MAX(DiemTB) = NULL (chưa có sinh viên)";
                    _lblKetQuaScalar.ForeColor = UiTheme.Amber;
                    KetQua("ExecuteScalar trả về DBNull.Value → đã xử lý an toàn, không bị lỗi ép kiểu.", true);
                }
            }
            catch (LoiNghiepVu ex)
            {
                _lblKetQuaScalar.Text = "—";
                HienThiLoiNghiepVu(ex);
            }
            catch (Exception ex)
            {
                _lblKetQuaScalar.Text = "—";
                LoiChung(ex);
            }
        }

        /// <summary>Bang thong ke toan bo khoa (ExecuteReader, xử lý NULL từng cột).</summary>
        private void LayTatCa()
        {
            try
            {
                List<ThongKeKhoa> ds = _bus.ThongKeDiemCaoNhatTheoKhoa();
                _luoi.DataSource = ds;

                if (_luoi.Columns.Contains("DiemCaoNhatText"))
                {
                    _luoi.Columns["DiemCaoNhatText"].HeaderText = "Điểm cao nhất";
                    _luoi.Columns["DiemCaoNhat"].Visible = false;
                }
                if (_luoi.Columns.Contains("DiemTrungBinhText"))
                {
                    _luoi.Columns["DiemTrungBinhText"].HeaderText = "Điểm trung bình";
                    _luoi.Columns["DiemTrungBinh"].Visible = false;
                }
                if (_luoi.Columns.Contains("MaKhoa")) _luoi.Columns["MaKhoa"].HeaderText = "Mã khoa";
                if (_luoi.Columns.Contains("TenKhoa")) _luoi.Columns["TenKhoa"].HeaderText = "Tên khoa";
                if (_luoi.Columns.Contains("SoSinhVien")) _luoi.Columns["SoSinhVien"].HeaderText = "Số sinh viên";

                _badge.Value = ds.Count + " khoa";
                KetQua(string.Format("Đã thống kê {0} khoa (COUNT, MAX, AVG xử lý NULL an toàn).", ds.Count), true);
            }
            catch (LoiNghiepVu ex)
            {
                _luoi.DataSource = null;
                Badge.Value = "Lỗi";
                HienThiLoiNghiepVu(ex);
            }
            catch (Exception ex)
            {
                _luoi.DataSource = null;
                LoiChung(ex);
            }
        }
    }
}
