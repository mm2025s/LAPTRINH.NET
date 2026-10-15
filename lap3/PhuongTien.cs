using System;

namespace Lap3
{
    /// <summary>
    /// BÀI 7 (tự thực hiện): QUẢN LÝ PHƯƠNG TIỆN
    /// Interface IMove định nghĩa hợp đồng di chuyển cho mọi phương tiện.
    /// </summary>
    public interface IMove
    {
        /// <summary>Tốc độ tối đa (km/h) của phương tiện.</summary>
        double TocDoToiDa { get; }

        /// <summary>Chi phí vận hành cho mỗi km (đồng/km).</summary>
        double ChiPhiMoiKm { get; }

        /// <summary>Ước tính thời gian di chuyển trên một quãng đường (giờ).</summary>
        double TinhThoiGian(double quangDuongKm);

        /// <summary>Ước tính chi phí vận hành cho một quãng đường (đồng).</summary>
        double TinhChiPhi(double quangDuongKm);

        string MoTa();
    }

    /// <summary>Lớp trừu tượng dùng chung cho mọi phương tiện, cài đặt sẵn IMove.</summary>
    public abstract class PhuongTien : IMove
    {
        private string _ten = string.Empty;

        public string Ten
        {
            get { return _ten; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Tên phương tiện không được rỗng.");
                _ten = value.Trim();
            }
        }

        public abstract double TocDoToiDa { get; }
        public abstract double ChiPhiMoiKm { get; }

        protected PhuongTien() { _ten = "Chưa có"; }

        protected PhuongTien(string ten) { Ten = ten; }

        /// <summary>
        /// Phương thức ảo: mặc định thời gian = quãng đường / tốc độ tối đa.
        /// Lớp con có thể ghi đè nếu có quy tắc riêng.
        /// </summary>
        public virtual double TinhThoiGian(double quangDuongKm)
        {
            if (quangDuongKm < 0)
                throw new ArgumentOutOfRangeException("quangDuongKm", "Quãng đường phải >= 0.");
            if (TocDoToiDa <= 0) return 0;
            return quangDuongKm / TocDoToiDa;
        }

        public virtual double TinhChiPhi(double quangDuongKm)
        {
            if (quangDuongKm < 0)
                throw new ArgumentOutOfRangeException("quangDuongKm", "Quãng đường phải >= 0.");
            return quangDuongKm * ChiPhiMoiKm;
        }

        public virtual string MoTa()
        {
            return string.Format("{0,-10} | Tốc độ tối đa: {1,6:0.#} km/h | Chi phí: {2,10:N0} đ/km",
                Ten, TocDoToiDa, ChiPhiMoiKm);
        }
    }

    /// <summary>Xe máy: 60 km/h, 1.200 đ/km.</summary>
    public class XeMay : PhuongTien
    {
        public override double TocDoToiDa { get { return 60; } }
        public override double ChiPhiMoiKm { get { return 1200; } }

        public XeMay() : base() { }
        public XeMay(string ten) : base(ten) { }
    }

    /// <summary>Ô tô: 100 km/h, 4.500 đ/km.</summary>
    public class OTo : PhuongTien
    {
        public override double TocDoToiDa { get { return 100; } }
        public override double ChiPhiMoiKm { get { return 4500; } }

        public OTo() : base() { }
        public OTo(string ten) : base(ten) { }
    }

    /// <summary>
    /// Xe đạp: 20 km/h, chi phí gần như bằng 0 (bảo dưỡng 100 đ/km).
    /// Ghi đè TinhThoiGian() để mô phỏng tốc độ thực tế chỉ đạt ~70% tốc độ tối đa.
    /// </summary>
    public class XeDap : PhuongTien
    {
        public override double TocDoToiDa { get { return 20; } }
        public override double ChiPhiMoiKm { get { return 100; } }

        public XeDap() : base() { }
        public XeDap(string ten) : base(ten) { }

        public override double TinhThoiGian(double quangDuongKm)
        {
            // Tốc độ thực tế chỉ đạt 70% tốc độ tối đa
            return base.TinhThoiGian(quangDuongKm) / 0.7;
        }
    }
}
