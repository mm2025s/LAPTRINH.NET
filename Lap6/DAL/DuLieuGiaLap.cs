using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Lab06_ThreeLayer.DAL
{
    /// <summary>
    /// ============================================================
    /// TẦNG DAL GIẢ LẬP (Simulated Data Access Layer)
    /// ============================================================
    /// Nhiệm vụ: chỉ lo LƯU TRỮ / ĐỌC DỮ LIỆU (ở đây là List trong bộ nhớ + tệp CSV).
    /// KHÔNG kiểm tra nghiệp vụ (trùng mã, điểm hợp lệ...) - việc đó thuộc tầng BUS.
    /// Sang Lab 7, chỉ cần thay phần ruột của lớp này bằng ADO.NET (SqlConnection...)
    /// mà giao diện và BUS không phải sửa.
    ///
    /// Quy ước: dữ liệu "kho" là danh sách private _data, mọi truy cập phải qua DAL.
    /// Lớp này KHÔNG bị Form dùng trực tiếp.
    /// </summary>
    public class SinhVienDAL
    {
        private readonly List<DTO.SinhVien> _data = new List<DTO.SinhVien>();

        /// <summary>Lấy toàn bộ dữ liệu. Trả về BẢN SAO để nơi gọi không sửa được kho dữ liệu.</summary>
        public List<DTO.SinhVien> GetAll()
        {
            var ketQua = new List<DTO.SinhVien>(_data.Count);
            foreach (DTO.SinhVien sv in _data) ketQua.Add(sv.Clone());
            return ketQua;
        }

        /// <summary>Lấy một bản ghi theo mã (null nếu không có).</summary>
        public DTO.SinhVien GetByMa(string maSV)
        {
            DTO.SinhVien sv = _data.Find(x => string.Equals(x.MaSV, maSV, StringComparison.OrdinalIgnoreCase));
            return sv == null ? null : sv.Clone();
        }

        public void Add(DTO.SinhVien sv)
        {
            if (sv == null) throw new ArgumentNullException("sv");
            _data.Add(sv.Clone());
        }

        /// <summary>Cập nhật theo khóa MaSV. Trả về false nếu không tìm thấy bản ghi.</summary>
        public bool Update(DTO.SinhVien sv)
        {
            if (sv == null) throw new ArgumentNullException("sv");
            int i = _data.FindIndex(x => string.Equals(x.MaSV, sv.MaSV, StringComparison.OrdinalIgnoreCase));
            if (i < 0) return false;
            _data[i] = sv.Clone();
            return true;
        }

        public bool Delete(string maSV)
        {
            int i = _data.FindIndex(x => string.Equals(x.MaSV, maSV, StringComparison.OrdinalIgnoreCase));
            if (i < 0) return false;
            _data.RemoveAt(i);
            return true;
        }

        public int Count
        {
            get { return _data.Count; }
        }

        // =================================================================
        // BÀI 9: LƯU / ĐỌC CSV (nhiệm vụ của DAL)
        // =================================================================
        public const string HeaderCSV = "MaSV,HoTen,NgaySinh,GioiTinh,Khoa,DiemTB,Email,LoaiSinhVien";

        /// <summary>Ghi toàn bộ dữ liệu ra tệp CSV (UTF-8 có BOM để Excel đọc đúng tiếng Việt).</summary>
        public int LuuCSV(string duongDan)
        {
            string thuMuc = Path.GetDirectoryName(duongDan);
            if (!string.IsNullOrEmpty(thuMuc)) Directory.CreateDirectory(thuMuc);

            var sb = new StringBuilder();
            sb.AppendLine(HeaderCSV);
            foreach (DTO.SinhVien sv in _data)
            {
                sb.AppendLine(string.Join(",", new[]
                {
                    ChuanHoaCSV(sv.MaSV),
                    ChuanHoaCSV(sv.HoTen),
                    sv.NgaySinh.ToString("yyyy-MM-dd"),
                    ChuanHoaCSV(sv.GioiTinh),
                    ChuanHoaCSV(sv.Khoa),
                    sv.DiemTB.ToString("0.0", CultureInfo.InvariantCulture),
                    ChuanHoaCSV(sv.Email),
                    ChuanHoaCSV(sv.LoaiSinhVien)
                }));
            }

            File.WriteAllText(duongDan, sb.ToString(), new UTF8Encoding(true));
            return _data.Count;
        }

        /// <summary>
        /// Đọc CSV vào kho dữ liệu. Dòng sai định dạng được bỏ qua và đếm lại
        /// (DAL không im lặng "đoán" dữ liệu sai, nhưng cũng không làm sập ứng dụng).
        /// </summary>
        public int DocCSV(string duongDan, out int soDongLoi)
        {
            soDongLoi = 0;
            if (!File.Exists(duongDan)) return 0;

            string[] lines = File.ReadAllLines(duongDan, Encoding.UTF8);
            if (lines.Length == 0) return 0;

            _data.Clear();
            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;

                string[] p = TachCSV(lines[i]);
                if (p.Length < 6) { soDongLoi++; continue; }

                try
                {
                    var sv = new DTO.SinhVien
                    {
                        MaSV = p[0],
                        HoTen = p[1],
                        NgaySinh = DateTime.ParseExact(p[2], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        GioiTinh = p[3],
                        Khoa = p[4],
                        DiemTB = double.Parse(p[5], CultureInfo.InvariantCulture),
                        Email = p.Length > 6 ? p[6] : string.Empty,
                        LoaiSinhVien = p.Length > 7 && p[7].Length > 0 ? p[7] : "Thường"
                    };
                    _data.Add(sv);
                }
                catch (FormatException)
                {
                    soDongLoi++;
                }
            }

            return _data.Count;
        }

        // --------------------- tiện ích CSV ---------------------
        private static string ChuanHoaCSV(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            if (value.IndexOfAny(new[] { ',', '"', '\n' }) >= 0)
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            return value;
        }

        private static string[] TachCSV(string line)
        {
            var fields = new List<string>();
            var sb = new StringBuilder();
            bool trongNgoac = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (trongNgoac)
                {
                    if (c == '"' && i + 1 < line.Length && line[i + 1] == '"') { sb.Append('"'); i++; }
                    else if (c == '"') trongNgoac = false;
                    else sb.Append(c);
                }
                else
                {
                    if (c == '"') trongNgoac = true;
                    else if (c == ',') { fields.Add(sb.ToString().Trim()); sb.Length = 0; }
                    else sb.Append(c);
                }
            }
            fields.Add(sb.ToString().Trim());
            return fields.ToArray();
        }
    }

    /// <summary>Khoa - DAL giả lập.</summary>
    public class KhoaDAL
    {
        private readonly List<DTO.Khoa> _data = new List<DTO.Khoa>();

        public List<DTO.Khoa> GetAll()
        {
            return new List<DTO.Khoa>(_data);
        }

        public void Add(DTO.Khoa k)
        {
            _data.Add(k);
        }

        public bool Exists(string tenKhoa)
        {
            return _data.Any(x => string.Equals(x.TenKhoa, tenKhoa, StringComparison.OrdinalIgnoreCase));
        }
    }

    /// <summary>Sản phẩm - DAL giả lập.</summary>
    public class SanPhamDAL
    {
        private readonly List<DTO.SanPham> _data = new List<DTO.SanPham>();

        public List<DTO.SanPham> GetAll()
        {
            var kq = new List<DTO.SanPham>(_data.Count);
            foreach (DTO.SanPham sp in _data) kq.Add(sp.Clone());
            return kq;
        }

        public void Add(DTO.SanPham sp)
        {
            _data.Add(sp.Clone());
        }

        public bool Update(DTO.SanPham sp)
        {
            int i = _data.FindIndex(x => x.MaSP == sp.MaSP);
            if (i < 0) return false;
            _data[i] = sp.Clone();
            return true;
        }

        public bool Delete(string maSP)
        {
            int i = _data.FindIndex(x => x.MaSP == maSP);
            if (i < 0) return false;
            _data.RemoveAt(i);
            return true;
        }

        public bool Exists(string maSP)
        {
            return _data.Any(x => string.Equals(x.MaSP, maSP, StringComparison.OrdinalIgnoreCase));
        }

        public void Clear()
        {
            _data.Clear();
        }
    }

    /// <summary>Danh mục - DAL giả lập.</summary>
    public class DanhMucDAL
    {
        private readonly List<DTO.DanhMuc> _data = new List<DTO.DanhMuc>();

        public List<DTO.DanhMuc> GetAll()
        {
            return new List<DTO.DanhMuc>(_data);
        }

        public void Add(DTO.DanhMuc dm)
        {
            _data.Add(dm);
        }

        public bool Exists(string maDanhMuc)
        {
            return _data.Any(x => string.Equals(x.MaDanhMuc, maDanhMuc, StringComparison.OrdinalIgnoreCase));
        }

        public bool DoiTen(string maDanhMuc, string tenMoi)
        {
            DTO.DanhMuc dm = _data.Find(x => x.MaDanhMuc == maDanhMuc);
            if (dm == null) return false;
            dm.TenDanhMuc = tenMoi;
            return true;
        }

        public bool Delete(string maDanhMuc)
        {
            int i = _data.FindIndex(x => x.MaDanhMuc == maDanhMuc);
            if (i < 0) return false;
            _data.RemoveAt(i);
            return true;
        }
    }
}
