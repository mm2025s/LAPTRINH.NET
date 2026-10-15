using System;

namespace Lap3
{
    /// <summary>
    /// BÀI 9 (tự thực hiện): HỆ THỐNG THANH TOÁN
    /// Interface IPayment quy định hợp đồng cho mọi hình thức thanh toán.
    /// </summary>
    public interface IPayment
    {
        string TenPhuongThuc { get; }

        /// <summary>Kiểm tra số tiền và điều kiện của từng phương thức.</summary>
        bool KiemTraHopLe(decimal soTien, out string message);

        /// <summary>Ghi nhận giao dịch; trả về mã giao dịch.</summary>
        string GhiNhanGiaoDich(decimal soTien);

        /// <summary>Phí giao dịch của từng phương thức.</summary>
        decimal TinhPhi(decimal soTien);
    }

    /// <summary>
    /// Lớp trừu tượng cung cấp phần dùng chung: kiểm tra số tiền dương,
    /// sinh mã giao dịch. Lớp con chỉ cần cài đặt phần riêng.
    /// </summary>
    public abstract class PhuongThucThanhToan : IPayment
    {
        private static int _boDem = 0;

        public abstract string TenPhuongThuc { get; }

        public abstract decimal TinhPhi(decimal soTien);

        /// <summary>
        /// Phương thức ảo: kiểm tra cơ bản dùng chung.
        /// Lớp con gọi base.KiemTraHopLe() rồi bổ sung điều kiện riêng.
        /// </summary>
        public virtual bool KiemTraHopLe(decimal soTien, out string message)
        {
            if (soTien <= 0)
            {
                message = "Số tiền phải lớn hơn 0.";
                return false;
            }
            message = string.Empty;
            return true;
        }

        public virtual string GhiNhanGiaoDich(decimal soTien)
        {
            _boDem++;
            return string.Format("GD{0:0000}", _boDem);
        }
    }

    /// <summary>Thanh toán tiền mặt: không phí, không giới hạn.</summary>
    public class ThanhToanTienMat : PhuongThucThanhToan
    {
        public override string TenPhuongThuc { get { return "Tiền mặt"; } }

        public override decimal TinhPhi(decimal soTien)
        {
            return 0;
        }
    }

    /// <summary>Chuyển khoản: miễn phí dưới 10 triệu, trên mức đó thu 0,05%.</summary>
    public class ChuyenKhoan : PhuongThucThanhToan
    {
        public const decimal NguongMienPhi = 10000000m;
        public const decimal TyLePhi = 0.0005m;

        public string SoTaiKhoan { get; set; }

        public ChuyenKhoan() { SoTaiKhoan = "Chưa có"; }
        public ChuyenKhoan(string soTaiKhoan) { SoTaiKhoan = soTaiKhoan; }

        public override string TenPhuongThuc { get { return "Chuyển khoản"; } }

        public override decimal TinhPhi(decimal soTien)
        {
            return soTien <= NguongMienPhi ? 0 : soTien * TyLePhi;
        }
    }

    /// <summary>
    /// Ví điện tử: phí cố định 2.000 đ, hạn mức mỗi giao dịch 20 triệu.
    /// Ghi đè KiemTraHopLe() để thêm ràng buộc hạn mức.
    /// </summary>
    public class ViDienTu : PhuongThucThanhToan
    {
        public const decimal PhiCoDinh = 2000m;
        public const decimal HanMucGiaoDich = 20000000m;

        public string TenVi { get; set; }

        public ViDienTu() { TenVi = "Chưa có"; }
        public ViDienTu(string tenVi) { TenVi = tenVi; }

        public override string TenPhuongThuc { get { return "Ví điện tử (" + TenVi + ")"; } }

        public override decimal TinhPhi(decimal soTien)
        {
            return PhiCoDinh;
        }

        public override bool KiemTraHopLe(decimal soTien, out string message)
        {
            // Tái sử dụng kiểm tra của lớp cha
            if (!base.KiemTraHopLe(soTien, out message)) return false;

            if (soTien > HanMucGiaoDich)
            {
                message = string.Format("Vượt hạn mức {0:N0} đ mỗi giao dịch.", HanMucGiaoDich);
                return false;
            }
            return true;
        }
    }
}
