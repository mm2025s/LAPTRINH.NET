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
    /// BAI 2 - Doc danh sach bang ExecuteReader.
    ///
    /// SqlDataReader la con tro doc MOT CHIEU (forward-only) va CHI DOC (read-only):
    /// khong the quay lai dong da doc, khong sua duoc du lieu. Doi lai no rat nhanh
    /// va khong nap toan bo bang vao bo nho nhu DataTable.
    /// </summary>
    public class frmBai2 : BaiFormBase
    {
        private readonly SinhVienBUS _bus = new SinhVienBUS();

        private ListBox _lstSinhVien;
        private Label _lblDem;
        private TextBox _txtSql;

        public frmBai2()
            : base("2", "Đọc danh sách bằng ExecuteReader",
                   "SqlDataReader · forward-only · read-only",
                   UiTheme.Blue, "ExecuteReader", "SELECT MaSV, HoTen, DiemTB ... ORDER BY HoTen",
                   "Bài 2", new Size(820, 560))
        {
            DungGiaoDien();
            Load += (s, e) => Nap();
        }

        private void DungGiaoDien()
        {
            Label lblSql = TieuDeMuc("Câu lệnh đang thực thi", UiTheme.Blue);
            lblSql.SetBounds(18, 16, 400, 22);

            _txtSql = TaoO("txtSql", 18, 40, 760, null, true);
            _txtSql.Font = UiTheme.FontMono(9.25f);
            _txtSql.Text = "SELECT MaSV, HoTen, DiemTB FROM dbo.SinhVien ORDER BY HoTen";
            _txtSql.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _txtSql.Height = 28;

            var card = The(18, 84, 760, 360);
            card.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            Label lblTieuDe = TieuDeMuc("Danh sách sinh viên (ListBox do từ SqlDataReader)", UiTheme.Blue);
            lblTieuDe.SetBounds(20, 12, 520, 22);

            _lblDem = UiFactory.Label("0 dòng", 9.25f, UiTheme.InkSoft, true);
            _lblDem.SetBounds(560, 12, 180, 22);
            _lblDem.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _lblDem.TextAlign = ContentAlignment.MiddleRight;

            _lstSinhVien = new ListBox();
            _lstSinhVien.Name = "lstSinhVien";
            _lstSinhVien.SetBounds(20, 40, 720, 298);
            _lstSinhVien.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _lstSinhVien.Font = UiTheme.FontMono(9.75f);
            _lstSinhVien.BorderStyle = BorderStyle.FixedSingle;
            _lstSinhVien.IntegralHeight = false;
            _lstSinhVien.BackColor = Color.White;

            card.Controls.AddRange(new Control[] { lblTieuDe, _lblDem, _lstSinhVien });

            AccentButton btnNap = Nut("btnNap", "Đọc bằng ExecuteReader", UiTheme.Blue, 18, 456, 220);
            btnNap.Click += (s, e) => Nap();

            AccentButton btnXoa = Nut("btnXoa", "Xóa ListBox", UiTheme.Slate, 248, 456, 140, 36, true);
            btnXoa.Click += (s, e) =>
            {
                _lstSinhVien.Items.Clear();
                _lblDem.Text = "0 dòng";
                GhiChu("Đã xóa nội dung ListBox (dữ liệu vẫn còn trong CSDL).");
            };

            Label lblGiaiThich = UiFactory.Label(
                "Mỗi vòng lặp reader.Read() tiến một dòng; reader[\"TenCot\"] hoặc reader.GetXxx(ordinal) lấy giá trị.",
                9f, UiTheme.Muted, false);
            lblGiaiThich.SetBounds(400, 462, 380, 24);
            lblGiaiThich.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;

            NoiDung.Controls.AddRange(new Control[] { lblSql, _txtSql, card, btnNap, btnXoa, lblGiaiThich });
        }

        private void Nap()
        {
            try
            {
                List<SinhVien> ds = _bus.DocDanhSach();

                _lstSinhVien.BeginUpdate();
                _lstSinhVien.Items.Clear();
                foreach (SinhVien sv in ds) _lstSinhVien.Items.Add(sv.ToString());
                _lstSinhVien.EndUpdate();

                _lblDem.Text = ds.Count + " dòng";
                KetQua(string.Format("Đã đọc {0} dòng bằng ExecuteReader (forward-only, read-only).", ds.Count), true);
            }
            catch (LoiNghiepVu ex)
            {
                HienThiLoiNghiepVu(ex);
            }
            catch (Exception ex)
            {
                LoiChung(ex);
            }
        }
    }
}
