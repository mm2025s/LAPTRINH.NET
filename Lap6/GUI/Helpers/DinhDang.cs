using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;

namespace Lab06_ThreeLayer.GUI
{
    /// <summary>
    /// Hàm định dạng/tiện ích dùng chung ở tầng giao diện (không chứa nghiệp vụ, không chứa dữ liệu).
    /// </summary>
    public static class DinhDang
    {
        public static string So(double value, string format)
        {
            return value.ToString(format);
        }

        public static string Tien(double value)
        {
            return string.Format("{0:N0} đ", value);
        }

        public static string Ngay(DateTime d)
        {
            return d.ToString("dd/MM/yyyy");
        }

        /// <summary>Lấy tên thuộc tính public của một lớp (dùng khi sinh CSV / cấu hình cột).</summary>
        public static List<string> TenThuocTinh(Type type)
        {
            var ds = new List<string>();
            foreach (PropertyInfo p in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                ds.Add(p.Name);
            return ds;
        }

        public static string GiaTriThuocTinh(object o, string tenThuocTinh)
        {
            PropertyInfo p = o.GetType().GetProperty(tenThuocTinh);
            if (p == null) return string.Empty;
            object v = p.GetValue(o, null);
            return v == null ? string.Empty : v.ToString();
        }

        public static string DocFile(string path)
        {
            return File.Exists(path) ? File.ReadAllText(path, Encoding.UTF8) : string.Empty;
        }
    }

    /// <summary>
    /// Chuyển đổi hiển thị BindingList/SortedBindingList sang chuỗi (dùng cho nhật ký kiểm thử).
    /// </summary>
    public static class BanSao
    {
        public static string MoTa(IList<object> ds, Func<object, string> dinhDang)
        {
            var sb = new StringBuilder();
            foreach (object o in ds) sb.AppendLine(dinhDang(o));
            return sb.ToString();
        }

        public static string MoTa<T>(BindingList<T> ds, Func<T, string> dinhDang)
        {
            var sb = new StringBuilder();
            foreach (T o in ds) sb.AppendLine(dinhDang(o));
            return sb.ToString();
        }
    }
}
