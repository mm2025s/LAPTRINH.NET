using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Lab09_DataReaderSearch.GUI.Helpers
{
    /// <summary>
    /// DataGridView da cau hinh san cho Lab 9.
    ///
    /// Gop vao mot lop vi 8/10 bai deu can cung mot kieu luoi: chi doc, chon
    /// nguyen dong, dinh dang ngay/diem, to mau theo diem va hien thi tieng Viet.
    /// Nho vay moi Form bai tap chi con viet phan logic tim kiem.
    ///
    /// BAI 9 duoc cai dat o day: dinh dang cot + to mau dong theo DiemTB.
    /// </summary>
    public class LuoiSinhVien : DataGridView
    {
        /// <summary>Nguong diem de to mau (bai 9).</summary>
        public decimal NguongThap { get; set; }
        public decimal NguongCao { get; set; }

        public bool BatToMau { get; set; }

        public Color MauThap { get; set; }
        public Color MauCao { get; set; }

        /// <summary>Phat sinh khi nguoi dung chon mot dong khac (bai 5).</summary>
        public event EventHandler<DataGridViewRow> DongDuocChon;

        private static readonly Color HeaderBack = Color.FromArgb(246, 247, 251);
        private static readonly Color ZebraBack = Color.FromArgb(251, 252, 254);

        public LuoiSinhVien()
        {
            NguongThap = 5m;
            NguongCao = 8m;
            BatToMau = true;
            MauThap = Color.FromArgb(255, 235, 238);   // hong nhat
            MauCao = Color.FromArgb(232, 248, 238);    // xanh la nhat

            Dock = DockStyle.Fill;
            Name = "luoiSinhVien";
            ReadOnly = true;
            AllowUserToAddRows = false;
            AllowUserToDeleteRows = false;
            AllowUserToResizeRows = false;
            AllowUserToOrderColumns = true;
            MultiSelect = false;
            SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            AutoGenerateColumns = true;
            BackgroundColor = Color.White;
            BorderStyle = BorderStyle.None;
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            GridColor = Color.FromArgb(236, 239, 245);
            RowHeadersVisible = false;
            EnableHeadersVisualStyles = false;
            ColumnHeadersHeight = 36;
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            RowTemplate.Height = 30;
            Font = UiTheme.FontBody(9.5f);
            AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = ZebraBack };

            DefaultCellStyle = new DataGridViewCellStyle
            {
                SelectionBackColor = Color.FromArgb(238, 234, 255),
                SelectionForeColor = UiTheme.Ink,
                Padding = new Padding(6, 0, 6, 0),
                Font = UiTheme.FontBody(9.5f)
            };

            ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = HeaderBack,
                ForeColor = UiTheme.InkSoft,
                Font = UiTheme.FontBold(8.75f),
                SelectionBackColor = HeaderBack,
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(6, 0, 6, 0)
            };

            CellFormatting += XuLyDinhDang;
            SelectionChanged += XuLyChonDong;
            DataBindingComplete += XuLySauKhiGanDuLieu;
        }

        /// <summary>Gan DataTable va dat lai tieu de cot theo tieng Viet.</summary>
        public void GanDuLieu(DataTable table)
        {
            DataSource = null;
            if (table != null) DataSource = table;

            DatLaiTieuDe();
            if (CurrentRow == null && Rows.Count > 0) CurrentCell = Rows[0].Cells[0];
        }

        /// <summary>
        /// Thu tu cot + tieu de tieng Viet. Chi dat tieu de cho cot CO trong
        /// ket qua (cau SQL cua tung bai chon cac cot khac nhau).
        /// </summary>
        private void DatLaiTieuDe()
        {
            if (Columns.Count == 0) return;

            Dat("MaSV", "Mã SV", 70);
            Dat("HoTen", "Họ tên", 170);
            Dat("NgaySinh", "Ngày sinh", 90);
            Dat("GioiTinh", "Giới tính", 70);
            Dat("MaKhoa", "Mã khoa", 70);
            Dat("TenKhoa", "Tên khoa", 150);
            Dat("DiemTB", "Điểm TB", 70);
            Dat("Email", "Email", 170);
            Dat("Tuoi", "Tuổi", 55);
            Dat("SoLuong", "Số SV", 70);
        }

        private void Dat(string ten, string tieuDe, int rong)
        {
            if (Columns.Contains(ten))
            {
                DataGridViewColumn c = Columns[ten];
                c.HeaderText = tieuDe;
                if (rong > 0) c.FillWeight = rong;
            }
        }

        private void XuLySauKhiGanDuLieu(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            try
            {
                // Cot MaKhoa chi huu ich khi khong co TenKhoa.
                if (Columns.Contains("TenKhoa") && Columns.Contains("MaKhoa"))
                    Columns["MaKhoa"].Visible = false;
            }
            catch (Exception) { }
        }

        // ---------------------------------------------------------------- BAI 9

        /// <summary>
        /// Dinh dang hien thi theo kieu du lieu cua cot (bai 9).
        /// CellFormatting chay cho tung o nen phai giu that nhe.
        /// </summary>
        private void XuLyDinhDang(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            string ten = Columns[e.ColumnIndex].Name;

            if (string.Equals(ten, "NgaySinh", StringComparison.OrdinalIgnoreCase))
            {
                if (e.Value is DateTime)
                {
                    e.Value = ((DateTime)e.Value).ToString("dd/MM/yyyy");
                    e.FormattingApplied = true;
                }
            }
            else if (string.Equals(ten, "DiemTB", StringComparison.OrdinalIgnoreCase))
            {
                if (e.Value != null && e.Value != DBNull.Value)
                {
                    e.Value = Convert.ToDecimal(e.Value).ToString("0.00");
                    e.FormattingApplied = true;
                    e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }
            else if (string.Equals(ten, "Tuoi", StringComparison.OrdinalIgnoreCase)
                  || string.Equals(ten, "SoLuong", StringComparison.OrdinalIgnoreCase)
                  || string.Equals(ten, "MaSV", StringComparison.OrdinalIgnoreCase))
            {
                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            // O trong (DBNull / null) hien thi bang dau gach de nguoi doc phan biet
            // "khong co du lieu" voi "chuoi rong".
            if (e.Value == null || e.Value == DBNull.Value)
            {
                e.Value = "—";
                e.FormattingApplied = true;
                e.CellStyle.ForeColor = UiTheme.Muted;
            }
        }

        // ---------------------------------------------------------------- BAI 9

        protected override void OnRowPrePaint(DataGridViewRowPrePaintEventArgs e)
        {
            base.OnRowPrePaint(e);
            if (!BatToMau || e.RowIndex < 0) return;

            DataGridViewRow row = Rows[e.RowIndex];
            if (row.IsNewRow) return;
            if (!Columns.Contains("DiemTB")) return;

            decimal? diem = LayDiem(row);

            Color nen = Color.Empty;
            if (diem.HasValue)
            {
                if (diem.Value < NguongThap) nen = MauThap;
                else if (diem.Value >= NguongCao) nen = MauCao;
            }

            // To ca dong: phai bo qua dong dang duoc chon de khong de mau
            // selection cua DataGridView bi ghi de.
            if (row.Selected && nen == Color.Empty) return;

            Color mauNen = row.Selected ? Color.FromArgb(238, 234, 255) : (nen == Color.Empty ? Color.Empty : nen);

            foreach (DataGridViewCell cell in row.Cells)
            {
                if (mauNen == Color.Empty)
                {
                    if (cell.Style.BackColor != Color.Empty) cell.Style.BackColor = Color.Empty;
                }
                else if (cell.Style.BackColor != mauNen)
                {
                    cell.Style.BackColor = mauNen;
                }
            }
        }

        private decimal? LayDiem(DataGridViewRow row)
        {
            object v = row.Cells["DiemTB"].Value;
            if (v == null || v == DBNull.Value) return null;
            try { return Convert.ToDecimal(v); }
            catch (Exception) { return null; }
        }

        // ---------------------------------------------------------------- BAI 5

        private void XuLyChonDong(object sender, EventArgs e)
        {
            if (!Focused && !ContainsFocus) return;   // tranh ban su kien khi nap du lieu

            EventHandler<DataGridViewRow> h = DongDuocChon;
            if (h == null) return;

            DataGridViewRow row = CurrentRow;
            if (row == null || row.IsNewRow) return;
            if (row.Index < 0 || row.Index >= Rows.Count) return;

            h(this, row);
        }

        /// <summary>Doc gia tri mot o theo ten cot, tra ve null khi o trong.</summary>
        public static object GiaTri(DataGridViewRow row, string cot)
        {
            if (row == null || row.DataGridView == null) return null;
            if (!row.DataGridView.Columns.Contains(cot)) return null;

            object v = row.Cells[cot].Value;
            return v == DBNull.Value ? null : v;
        }

        public static string Chuoi(DataGridViewRow row, string cot)
        {
            object v = GiaTri(row, cot);
            return v == null ? string.Empty : Convert.ToString(v);
        }
    }
}
