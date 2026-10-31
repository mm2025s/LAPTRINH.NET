using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Lab08_SqlCommand_Parameters.DTO;

namespace Lab08_SqlCommand_Parameters.GUI.Helpers
{
    /// <summary>
    /// Nhom o nhap thong tin sinh vien dung chung cho Bai 3 (INSERT) va Bai 9 (Stored Procedure).
    ///
    /// Dac diem quan trong ve mat ADO.NET:
    ///   - ComboBox MaKhoa gan DisplayMember = TenKhoa, ValueMember = MaKhoa
    ///     -> SelectedValue chinh la gia tri se dua vao SqlParameter @MaKhoa.
    ///   - GioiTinh dung 2 RadioButton (Nam/Nu) -> SqlDbType.Bit.
    ///   - Email de trong se duoc chuyen thanh DBNull.Value o tang DAL.
    ///
    /// Ten cac control trung voi ten truong trong CSDL (MaSV, HoTen, NgaySinh,
    /// MaKhoa, DiemTB, Email) de form cha focus dung o khi co loi nghiep vu.
    /// </summary>
    public class NhapSinhVienPanel : UserControl
    {
        public TextBox TxtMaSV { get; private set; }
        public TextBox TxtHoTen { get; private set; }
        public DateTimePicker DtpNgaySinh { get; private set; }
        public RadioButton RadNam { get; private set; }
        public RadioButton RadNu { get; private set; }
        public ComboBox CboKhoa { get; private set; }
        public TextBox TxtDiemTB { get; private set; }
        public TextBox TxtEmail { get; private set; }

        public NhapSinhVienPanel()
        {
            BackColor = Color.Transparent;
            Height = 210;
            DungGiaoDien();
        }

        private void DungGiaoDien()
        {
            int cot1 = 0, cot2 = 372;

            // ---------------- Cot 1: MaSV, HoTen, NgaySinh, GioiTinh ----------------
            Label l1 = UiFactory.FieldLabel("Mã sinh viên *");
            l1.SetBounds(cot1, 0, 340, 20);
            TxtMaSV = TaoO("MaSV", cot1, 22, 340, "Ví dụ: SV023");

            Label l2 = UiFactory.FieldLabel("Họ tên *");
            l2.SetBounds(cot1, 62, 340, 20);
            TxtHoTen = TaoO("HoTen", cot1, 84, 340, "Ví dụ: Nguyễn Văn An");

            Label l3 = UiFactory.FieldLabel("Ngày sinh *");
            l3.SetBounds(cot1, 124, 160, 20);

            DtpNgaySinh = new DateTimePicker();
            DtpNgaySinh.Name = "NgaySinh";
            DtpNgaySinh.Format = DateTimePickerFormat.Custom;
            DtpNgaySinh.CustomFormat = "dd/MM/yyyy";
            DtpNgaySinh.Font = UiTheme.FontBody(10f);
            DtpNgaySinh.SetBounds(cot1, 146, 150, 28);
            DtpNgaySinh.MaxDate = DateTime.Today;
            DtpNgaySinh.Value = new DateTime(2005, 1, 1);

            Label l4 = UiFactory.FieldLabel("Giới tính *");
            l4.SetBounds(cot1 + 168, 124, 172, 20);

            RadNam = new RadioButton();
            RadNam.Name = "RadNam";
            RadNam.Text = "Nam";
            RadNam.Font = UiTheme.FontBody(9.75f);
            RadNam.Checked = true;
            RadNam.SetBounds(cot1 + 170, 148, 70, 24);

            RadNu = new RadioButton();
            RadNu.Name = "RadNu";
            RadNu.Text = "Nữ";
            RadNu.Font = UiTheme.FontBody(9.75f);
            RadNu.SetBounds(cot1 + 246, 148, 70, 24);

            // ---------------- Cot 2: MaKhoa, DiemTB, Email ----------------
            Label l5 = UiFactory.FieldLabel("Khoa *");
            l5.SetBounds(cot2, 0, 388, 20);
            CboKhoa = UiFactory.ComboBox("MaKhoa");
            CboKhoa.SetBounds(cot2, 22, 388, 28);
            CboKhoa.DisplayMember = "TenKhoa";
            CboKhoa.ValueMember = "MaKhoa";

            Label l6 = UiFactory.FieldLabel("Điểm trung bình * (0 - 10)");
            l6.SetBounds(cot2, 62, 388, 20);
            TxtDiemTB = TaoO("DiemTB", cot2, 84, 180, "Ví dụ: 8.25");

            Label l7 = UiFactory.FieldLabel("Email (có thể để trống → DBNull.Value)");
            l7.SetBounds(cot2, 124, 388, 20);
            TxtEmail = TaoO("Email", cot2, 146, 388, "Ví dụ: an@eaut.edu.vn");

            Controls.AddRange(new Control[]
            {
                l1, TxtMaSV, l2, TxtHoTen, l3, DtpNgaySinh, l4, RadNam, RadNu,
                l5, CboKhoa, l6, TxtDiemTB, l7, TxtEmail
            });
        }

        private static TextBox TaoO(string name, int x, int y, int w, string placeholder)
        {
            TextBox tb = UiFactory.TextBox(name, placeholder, false);
            tb.SetBounds(x, y, w, 28);
            return tb;
        }

        /// <summary>Do danh sach khoa vao ComboBox (ValueMember = MaKhoa).</summary>
        public void NapKhoa(List<Khoa> dsKhoa)
        {
            CboKhoa.DataSource = null;
            CboKhoa.DataSource = dsKhoa;
            CboKhoa.DisplayMember = "TenKhoa";
            CboKhoa.ValueMember = "MaKhoa";
            if (dsKhoa.Count > 0) CboKhoa.SelectedIndex = 0;
        }

        /// <summary>Gom gia tri tren form thanh DTO de dua xuong BUS.</summary>
        public SinhVien LaySinhVien()
        {
            return new SinhVien
            {
                MaSV = UiFactory.Val(TxtMaSV),
                HoTen = UiFactory.Val(TxtHoTen),
                NgaySinh = DtpNgaySinh.Value.Date,
                GioiTinh = RadNam.Checked,
                MaKhoa = CboKhoa.SelectedValue == null ? string.Empty : Convert.ToString(CboKhoa.SelectedValue),
                DiemTB = DocDiemTuForm(),
                Email = UiFactory.Val(TxtEmail)
            };
        }

        private decimal DocDiemTuForm()
        {
            decimal d;
            string t = UiFactory.Val(TxtDiemTB).Replace(',', '.');
            return decimal.TryParse(t, System.Globalization.NumberStyles.Number,
                System.Globalization.CultureInfo.InvariantCulture, out d) ? d : 0m;
        }

        /// <summary>Xoa trang cac o nhap, giu nguyen khoa dang chon.</summary>
        public void XoaTrang()
        {
            UiFactory.SetVal(TxtMaSV, string.Empty);
            UiFactory.SetVal(TxtHoTen, string.Empty);
            UiFactory.SetVal(TxtDiemTB, string.Empty);
            UiFactory.SetVal(TxtEmail, string.Empty);
            DtpNgaySinh.Value = new DateTime(2005, 1, 1);
            RadNam.Checked = true;
            TxtMaSV.Focus();
        }

        /// <summary>Dien du lieu cua mot sinh vien co san (phuc vu thu cap nhat / kiem tra).</summary>
        public void DienDuLieu(SinhVien sv)
        {
            if (sv == null) return;
            UiFactory.SetVal(TxtMaSV, sv.MaSV);
            UiFactory.SetVal(TxtHoTen, sv.HoTen);
            UiFactory.SetVal(TxtDiemTB, sv.DiemTB.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture));
            UiFactory.SetVal(TxtEmail, sv.Email);
            try { DtpNgaySinh.Value = sv.NgaySinh; } catch (ArgumentOutOfRangeException) { }
            RadNam.Checked = sv.GioiTinh;
            RadNu.Checked = !sv.GioiTinh;
            try
            {
                if (!string.IsNullOrEmpty(sv.MaKhoa) && CboKhoa.DataSource != null)
                    CboKhoa.SelectedValue = sv.MaKhoa;
            }
            catch (ArgumentException) { }
        }
    }
}
