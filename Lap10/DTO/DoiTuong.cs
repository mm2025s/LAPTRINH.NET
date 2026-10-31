using System;

namespace Lab10_DisconnectedData.DTO
{
    /// <summary>Mot dong bang Khoa, dung cho ComboBox va luoi Khoa.</summary>
    public class Khoa
    {
        public string MaKhoa { get; set; }
        public string TenKhoa { get; set; }

        public override string ToString()
        {
            return MaKhoa + " - " + TenKhoa;
        }
    }

    /// <summary>
    /// Loi nghiep vu co chi ro truong nao sai, de giao dien focus dung o nhap.
    /// Giong Lab 6 - 9 de cac lab dung chung mot kieu loi.
    /// </summary>
    public class LoiNghiepVu : Exception
    {
        public string Truong { get; private set; }

        public LoiNghiepVu(string truong, string thongBao) : base(thongBao)
        {
            Truong = truong;
        }
    }

    /// <summary>
    /// Thong ke trang thai cac dong cua mot DataTable - phuc vu bai 7.
    ///
    /// DataTable KHONG luu "dong nao da bi sua" o mot danh sach rieng: moi
    /// DataRow tu mang co RowState (Unchanged / Added / Modified / Deleted).
    /// Vi vay muon biet se co bao nhieu lenh INSERT/UPDATE/DELETE gui xuong
    /// server, ta phai quet RowState chu khong dem so dong cua bang.
    /// </summary>
    public class ThongKeRowState
    {
        public int KhongDoi { get; set; }
        public int ThemMoi { get; set; }
        public int DaSua { get; set; }
        public int DaXoa { get; set; }

        public int TongThayDoi
        {
            get { return ThemMoi + DaSua + DaXoa; }
        }

        public int TongDong
        {
            get { return KhongDoi + ThemMoi + DaSua + DaXoa; }
        }

        public override string ToString()
        {
            return string.Format("Không đổi {0} · Thêm {1} · Sửa {2} · Xóa {3} → {4} thay đổi sẽ gửi",
                KhongDoi, ThemMoi, DaSua, DaXoa, TongThayDoi);
        }
    }

    /// <summary>Ket qua mot lan goi DataAdapter.Update.</summary>
    public class KetQuaCapNhat
    {
        public int SoDongAnhHuong { get; set; }
        public bool CoLoi { get; set; }
        public string ThongDiep { get; set; }
    }
}
