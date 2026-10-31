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
    /// BAI 5 - Goi Stored Procedure sp_SinhVien_TheoKhoa.
    ///
    /// Ba dieu bat buoc khi goi thu tuc:
    ///   1. cmd.CommandType = CommandType.StoredProcedure  (mac dinh la Text)
    ///   2. cmd.CommandText = TEN THU TUC (khong kem tham so)
    ///   3. Ten tham so trong C# PHAI KHOP ten tham so cua thu tuc (@MaKhoa)
    /// </summary>
    public class frmBai5 : BaiFormBase
    {
        private readonly SinhVienBUS _bus = new SinhVienBUS();

        private ComboBox _cboKhoa;
        private DataGridView _luoi;
        private Label _lblTong;
        private Label _lblDoanMa;

        public frmBai5()
            : base("5", "Gọi Stored Procedure tìm sinh viên theo khoa",
                   "CommandType.StoredProcedure · cmd.Parameters.Add(\"@MaKhoa\", ...)",
                   UiTheme.Indigo, "sp_SinhVien_TheoKhoa", "@MaKhoa VARCHAR(10)",
                   "Bài 5", new Size(880, 600))
        {
            DungGiaoDien();
            Load += frmBai5_Load;
        }

        private void DungGiaoDien()
        {
            Label lblKhoa = UiFactory.Label("Chọn khoa:", 9.75f, UiTheme.Ink, true);
            lblKhoa.SetBounds(18, 22, 90, 26);

            _cboKhoa = UiFactory.ComboBox("cboKhoa");
            _cboKhoa.SetBounds(110, 22, 260, 28);
            _cboKhoa.DisplayMember = "TenKhoa";
            _cboKhoa.ValueMember = "MaKhoa";
            _cboKhoa.SelectedIndexChanged += (s, e) => GoiThuTuc();

            AccentButton btnGoi = Nut("btnGoi", "Gọi thủ tục", UiTheme.Indigo, 382, 18, 160, 36);
            btnGoi.Click += (s, e) => GoiThuTuc();

            Badge = new BadgeLabel();
            Badge.BaseColor = UiTheme.Indigo;
            Badge.Value = "0 dòng";
            Badge.SetBounds(552, 22, 110, 28);

            _lblTong = UiFactory.Label("", 9.5f, UiTheme.InkSoft, false);
            _lblTong.SetBounds(674, 24, 188, 24);
            _lblTong.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _lblTong.TextAlign = ContentAlignment.MiddleRight;

            _luoi = new DataGridView();
            _luoi.Name = "luoiSinhVien";
            _luoi.SetBounds(18, 66, 844, 400);
            _luoi.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _luoi.BackgroundColor = Color.White;
            _luoi.BorderStyle = BorderStyle.FixedSingle;
            _luoi.RowHeadersVisible = false;
            _luoi.AllowUserToAddRows = false;
            _luoi.AllowUserToDeleteRows = false;
            _luoi.ReadOnly = true;
            _luoi.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _luoi.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _luoi.Font = UiTheme.FontBody(9.5f);
            _luoi.ColumnHeadersDefaultCellStyle.Font = UiTheme.FontBold(9.5f);
            _luoi.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(232, 234, 248);
            _luoi.EnableHeadersVisualStyles = false;
            _luoi.RowTemplate.Height = 26;
            _luoi.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 254);
            _luoi.DataError += (s, e) => { e.ThrowException = false; };

            var cardSql = The(18, 476, 844, 66);
            cardSql.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            Label lblTd = TieuDeMuc("Đoạn mã gọi thủ tục", UiTheme.Indigo);
            lblTd.SetBounds(16, 6, 400, 20);

            _lblDoanMa = new Label();
            _lblDoanMa.Font = UiTheme.FontMono(8.75f);
            _lblDoanMa.ForeColor = UiTheme.InkSoft;
            _lblDoanMa.BackColor = Color.Transparent;
            _lblDoanMa.SetBounds(16, 28, 812, 34);
            _lblDoanMa.Text = "cmd = new SqlCommand(\"dbo.sp_SinhVien_TheoKhoa\", conn);\r\n" +
                              "cmd.CommandType = CommandType.StoredProcedure;  cmd.Parameters.Add(\"@MaKhoa\", SqlDbType.VarChar, 10).Value = maKhoa;";
            cardSql.Controls.AddRange(new Control[] { lblTd, _lblDoanMa });

            NoiDung.Controls.AddRange(new Control[] { lblKhoa, _cboKhoa, btnGoi, Badge, _lblTong, _luoi, cardSql });
        }

        private void frmBai5_Load(object sender, EventArgs e)
        {
            try
            {
                _cboKhoa.DataSource = _bus.DanhSachKhoa();
                _cboKhoa.DisplayMember = "TenKhoa";
                _cboKhoa.ValueMember = "MaKhoa";
                if (_cboKhoa.Items.Count > 0) GoiThuTuc();
            }
            catch (LoiNghiepVu ex) { HienThiLoiNghiepVu(ex); }
            catch (Exception ex) { LoiChung(ex); }
        }

        private void GoiThuTuc()
        {
            string maKhoa = _cboKhoa.SelectedValue == null ? string.Empty : Convert.ToString(_cboKhoa.SelectedValue);
            if (string.IsNullOrEmpty(maKhoa)) return;

            try
            {
                List<SinhVien> ds = _bus.SinhVienTheoKhoa(maKhoa);
                _luoi.DataSource = ds;

                if (_luoi.Columns.Contains("GioiTinhText"))
                {
                    _luoi.Columns["GioiTinhText"].HeaderText = "Giới tính";
                    _luoi.Columns["GioiTinh"].Visible = false;
                }
                if (_luoi.Columns.Contains("TenKhoa")) _luoi.Columns["TenKhoa"].Visible = false;

                Badge.Value = ds.Count + " dòng";
                Badge.SetColor(ds.Count > 0 ? UiTheme.Indigo : UiTheme.Slate);

                _lblTong.Text = string.Format("Khoa {0}: {1} sinh viên", maKhoa, ds.Count);
                KetQua(ds.Count > 0
                    ? string.Format("sp_SinhVien_TheoKhoa @MaKhoa = '{0}' trả về {1} dòng.", maKhoa, ds.Count)
                    : string.Format("sp_SinhVien_TheoKhoa @MaKhoa = '{0}' không có sinh viên nào.", maKhoa), ds.Count > 0);
            }
            catch (LoiNghiepVu ex)
            {
                _luoi.DataSource = null;
                Badge.Value = "Lỗi";
                Badge.SetColor(UiTheme.Red);
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
