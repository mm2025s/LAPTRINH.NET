using System;

namespace Lap3
{
    /// <summary>
    /// BÀI 8 (tự thực hiện): QUẢN LÝ TÀI LIỆU THƯ VIỆN
    /// Lớp trừu tượng TaiLieu; các lớp con Sach, TapChi, LuanVan.
    /// </summary>
    public abstract class TaiLieu : IPrintable
    {
        private string _maTaiLieu = string.Empty;
        private string _tieuDe = string.Empty;
        private int _namXuatBan;

        public string MaTaiLieu
        {
            get { return _maTaiLieu; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Mã tài liệu không được rỗng.");
                _maTaiLieu = value.Trim();
            }
        }

        public string TieuDe
        {
            get { return _tieuDe; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Tiêu đề không được rỗng.");
                _tieuDe = value.Trim();
            }
        }

        public int NamXuatBan
        {
            get { return _namXuatBan; }
            set
            {
                if (value < 1900 || value > DateTime.Today.Year)
                    throw new ArgumentOutOfRangeException("NamXuatBan", "Năm xuất bản không hợp lệ.");
                _namXuatBan = value;
            }
        }

        /// <summary>Tên loại tài liệu — dùng để tìm kiếm / thống kê theo kiểu.</summary>
        public abstract string LoaiTaiLieu { get; }

        /// <summary>Thời hạn cho mượn (ngày) — mỗi loại tài liệu quy định khác nhau.</summary>
        public abstract int SoNgayMuonToiDa { get; }

        protected TaiLieu()
        {
            _maTaiLieu = "Chưa có";
            _tieuDe = "Chưa có";
            _namXuatBan = DateTime.Today.Year;
        }

        protected TaiLieu(string ma, string tieuDe, int namXuatBan)
        {
            MaTaiLieu = ma;
            TieuDe = tieuDe;
            NamXuatBan = namXuatBan;
        }

        /// <summary>
        /// Phương thức ảo mô tả chi tiết tài liệu; lớp con bổ sung thông tin riêng.
        /// </summary>
        public virtual string MoTa()
        {
            return string.Format("{0,-8} {1,-14} {2,-26} {3,6}",
                MaTaiLieu, LoaiTaiLieu, TieuDe, NamXuatBan);
        }

        public string ToDisplayText()
        {
            return string.Format("[{0}] {1} - {2} ({3})", MaTaiLieu, TieuDe, LoaiTaiLieu, NamXuatBan);
        }
    }

    /// <summary>Sách: có tác giả và nhà xuất bản.</summary>
    public class Sach : TaiLieu
    {
        public string TacGia { get; set; }
        public string NhaXuatBan { get; set; }

        public Sach() { TacGia = "Chưa có"; NhaXuatBan = "Chưa có"; }

        public Sach(string ma, string tieuDe, int nam, string tacGia, string nhaXuatBan)
            : base(ma, tieuDe, nam)
        {
            TacGia = tacGia;
            NhaXuatBan = nhaXuatBan;
        }

        public override string LoaiTaiLieu { get { return "Sách"; } }
        public override int SoNgayMuonToiDa { get { return 30; } }

        public override string MoTa()
        {
            return base.MoTa() + " | Tác giả: " + TacGia + " | NXB: " + NhaXuatBan;
        }
    }

    /// <summary>Tạp chí: có số phát hành và kỳ phát hành.</summary>
    public class TapChi : TaiLieu
    {
        public int SoPhatHanh { get; set; }
        public string KyPhatHanh { get; set; }

        public TapChi() { KyPhatHanh = "Chưa có"; }

        public TapChi(string ma, string tieuDe, int nam, int soPhatHanh, string ky)
            : base(ma, tieuDe, nam)
        {
            SoPhatHanh = soPhatHanh;
            KyPhatHanh = ky;
        }

        public override string LoaiTaiLieu { get { return "Tạp chí"; } }
        public override int SoNgayMuonToiDa { get { return 7; } }

        public override string MoTa()
        {
            return base.MoTa() + " | Số: " + SoPhatHanh + " | Kỳ: " + KyPhatHanh;
        }
    }

    /// <summary>Luận văn: có tác giả, cấp bậc và chỉ cho mượn tại chỗ.</summary>
    public class LuanVan : TaiLieu
    {
        public string TacGia { get; set; }
        public string CapBac { get; set; }

        public LuanVan() { TacGia = "Chưa có"; CapBac = "Chưa có"; }

        public LuanVan(string ma, string tieuDe, int nam, string tacGia, string capBac)
            : base(ma, tieuDe, nam)
        {
            TacGia = tacGia;
            CapBac = capBac;
        }

        public override string LoaiTaiLieu { get { return "Luận văn"; } }

        /// <summary>Luận văn chỉ được đọc tại chỗ nên không cho mượn (0 ngày).</summary>
        public override int SoNgayMuonToiDa { get { return 0; } }

        public override string MoTa()
        {
            return base.MoTa() + " | Tác giả: " + TacGia + " | Bậc: " + CapBac;
        }
    }
}
