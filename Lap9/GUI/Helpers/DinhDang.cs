using System;
using System.Collections.Generic;
using System.Globalization;

namespace Lab09_DataReaderSearch.GUI.Helpers
{
    /// <summary>
    /// Ham dinh dang du lieu hien thi o tang giao dien (khong chua nghiep vu, khong chua du lieu).
    /// </summary>
    public static class DinhDang
    {
        public static string So(double value, string format)
        {
            return value.ToString(format);
        }

        public static string Diem(decimal? value)
        {
            return value.HasValue
                ? value.Value.ToString("0.00", CultureInfo.InvariantCulture)
                : "-";
        }

        public static string Tien(double value)
        {
            return string.Format("{0:N0} đ", value);
        }

        public static string Ngay(DateTime d)
        {
            return d.ToString("dd/MM/yyyy");
        }

        public static string NgayGio(DateTime d)
        {
            return d.ToString("dd/MM/yyyy HH:mm:ss");
        }

        public static string GioiTinh(bool nam)
        {
            return nam ? "Nam" : "Nữ";
        }

        /// <summary>So sanh khong phan biet hoa/thuong + bo khoang trang - tien loc bang.</summary>
        public static bool Chua(string nguon, string tuKhoa)
        {
            if (string.IsNullOrEmpty(tuKhoa)) return true;
            if (nguon == null) return false;
            return nguon.IndexOf(tuKhoa.Trim(), StringComparison.CurrentCultureIgnoreCase) >= 0;
        }
    }
}
