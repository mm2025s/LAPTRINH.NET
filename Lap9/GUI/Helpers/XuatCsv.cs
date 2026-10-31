using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Lab09_DataReaderSearch.GUI.Helpers
{
    /// <summary>
    /// Xuat ket qua dang hien thi ra CSV (bai 10).
    ///
    /// Ba yeu cau ky thuat de file mo duoc bang Excel ma khong loi tieng Viet:
    ///
    /// 1. UTF-8 CO BOM. Neu ghi UTF-8 khong BOM, Excel (nhat la ban cu) se
    ///    doc theo bang ma ANSI cua Windows va hien "Nguy?n" thay vi "Nguyễn".
    ///    new UTF8Encoding(true) chinh la co BOM (EF BB BF o dau file).
    ///
    /// 2. ESCAPE dung chuan CSV (RFC 4180): gia tri co dau phay, dau nhay kep
    ///    hoac xuong dong phai duoc boc trong "..." va moi " ben trong phai
    ///    duoc lap doi thanh "". Neu khong, cot se bi lech.
    ///
    /// 3. Giu NGAY va DIEM o dang de doc (dd/MM/yyyy, 0.00) chu khong ghi
    ///    gia tri DateTime tho, vi CSV khong co kieu du lieu.
    /// </summary>
    public static class XuatCsv
    {
        /// <summary>
        /// Ghi DataTable ra file CSV UTF-8 co BOM.
        /// Tra ve so dong du lieu da ghi (khong tinh dong tieu de).
        /// </summary>
        public static int Ghi(DataTable table, string duongDan, params string[] cotBoQua)
        {
            if (table == null) throw new ArgumentNullException("table");
            if (string.IsNullOrEmpty(duongDan)) throw new ArgumentException("Đường dẫn trống.");

            var boQua = new HashSet<string>(cotBoQua ?? new string[0], StringComparer.OrdinalIgnoreCase);

            var cot = new List<DataColumn>();
            foreach (DataColumn c in table.Columns)
            {
                if (!boQua.Contains(c.ColumnName)) cot.Add(c);
            }

            using (var writer = new StreamWriter(duongDan, false, new UTF8Encoding(true)))
            {
                // Dong tieu de
                for (int i = 0; i < cot.Count; i++)
                {
                    if (i > 0) writer.Write(',');
                    writer.Write(Escape(TieuDe(cot[i].ColumnName)));
                }
                writer.Write("\r\n");

                int soDong = 0;
                foreach (DataRow row in table.Rows)
                {
                    for (int i = 0; i < cot.Count; i++)
                    {
                        if (i > 0) writer.Write(',');
                        writer.Write(Escape(DinhDangGiaTri(row[cot[i]])));
                    }
                    writer.Write("\r\n");
                    soDong++;
                }

                return soDong;
            }
        }

        /// <summary>Bo sung BOM cho file CSV da co (dung khi file duoc tao tu noi khac).</summary>
        public static bool CoBom(string duongDan)
        {
            if (!File.Exists(duongDan)) return false;

            using (var fs = new FileStream(duongDan, FileMode.Open, FileAccess.Read))
            {
                if (fs.Length < 3) return false;
                return fs.ReadByte() == 0xEF && fs.ReadByte() == 0xBB && fs.ReadByte() == 0xBF;
            }
        }

        /// <summary>Mo file bang ung dung mac dinh cua Windows (thuong la Excel).</summary>
        public static void MoFile(string duongDan)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = duongDan,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không mở được tệp: " + ex.Message, "Xuất CSV",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ------------------------------------------------------------------

        /// <summary>Tieu de cot tieng Viet cho file CSV (khac ten cot trong CSDL).</summary>
        private static string TieuDe(string tenCot)
        {
            switch (tenCot)
            {
                case "MaSV": return "Mã SV";
                case "HoTen": return "Họ tên";
                case "NgaySinh": return "Ngày sinh";
                case "GioiTinh": return "Giới tính";
                case "MaKhoa": return "Mã khoa";
                case "TenKhoa": return "Tên khoa";
                case "DiemTB": return "Điểm TB";
                case "Email": return "Email";
                case "Tuoi": return "Tuổi";
                default: return tenCot;
            }
        }

        private static string DinhDangGiaTri(object giaTri)
        {
            if (giaTri == null || giaTri == DBNull.Value) return "";

            if (giaTri is DateTime) return ((DateTime)giaTri).ToString("dd/MM/yyyy");
            if (giaTri is decimal) return ((decimal)giaTri).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
            if (giaTri is double) return ((double)giaTri).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
            if (giaTri is bool) return ((bool)giaTri) ? "Nam" : "Nữ";

            return Convert.ToString(giaTri);
        }

        /// <summary>Boc gia tri trong "..." khi can, va lap doi dau " theo RFC 4180.</summary>
        private static string Escape(string giaTri)
        {
            if (string.IsNullOrEmpty(giaTri)) return "";

            bool canBoc = giaTri.IndexOf(',') >= 0
                       || giaTri.IndexOf('"') >= 0
                       || giaTri.IndexOf('\n') >= 0
                       || giaTri.IndexOf('\r') >= 0;

            if (!canBoc) return giaTri;

            return "\"" + giaTri.Replace("\"", "\"\"") + "\"";
        }
    }
}
