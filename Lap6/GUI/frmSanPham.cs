using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Lab06_ThreeLayer.BUS;
using Lab06_ThreeLayer.DTO;
using Lab06_ThreeLayer.GUI.Helpers;

namespace Lab06_ThreeLayer.GUI
{
    /// <summary>
    /// frmSanPham - Bài 6: DataGridView cho sản phẩm, tính TỔNG GIÁ TRỊ HÀNG TỒN.
    ///
    /// Điểm cần nhớ: công thức "đơn giá × số lượng" KHÔNG viết ở Form.
    /// Form chỉ gọi SanPhamBUS.TongGiaTriTon() và hiển thị kết quả.
    /// </summary>
    public partial class frmSanPham : FormBase
    {
        private readonly SanPhamBUS _bus;
        private readonly KhoaBUS _khoa = new KhoaBUS();
        private int _lanKiemTra;

        public frmSanPham()
        {
            InitializeComponent();

            // Cho phép chạy Form độc lập (không qua frmMain) - vẫn tạo BUS với DAL riêng
            _bus = new SanPhamBUS(new DAL.SanPhamDAL(), new DAL.DanhMucDAL());
            DuLieuMau.NapSanPham(_bus, new DanhMucBUS(new DAL.DanhMucDAL(), new DAL.SanPhamDAL()));
        }

        public frmSanPham(SanPhamBUS bus)
        {
            InitializeComponent();

            _bus = bus ?? new SanPhamBUS(new DAL.SanPhamDAL(), new DAL.DanhMucDAL());
        }

        private void frmSanPham_Load(object sender, EventArgs e)
        {
            dgvSanPham.AutoGenerateColumns = false;
            NapLuoi(_bus.GetAll());
            CapNhatTong();
        }

        private void NapLuoi(IList<SanPham> ds)
        {
            dgvSanPham.Rows.Clear();
            int i = 1;
            foreach (SanPham sp in ds)
            {
                int idx = dgvSanPham.Rows.Add(
                    i++.ToString("00"),
                    sp.MaSP,
                    sp.TenSP,
                    sp.MaDanhMuc,
                    DinhDang.Tien(sp.DonGia),
                    sp.SoLuongTon,
                    DinhDang.Tien(sp.GiaTriTon));

                DataGridViewRow row = dgvSanPham.Rows[idx];
                row.Tag = sp;

                // Hàng tồn thấp (< 10) được tô đỏ để dễ thấy
                if (sp.SoLuongTon < 10)
                {
                    row.Cells[5].Style.ForeColor = UiTheme.Red;
                    row.Cells[5].Style.Font = UiTheme.FontBold(9.25f);
                }
                row.Cells[6].Style.ForeColor = UiTheme.Teal;
                row.Cells[6].Style.Font = UiTheme.FontBold(9.25f);
            }

            lblDem.Text = string.Format("{0} sản phẩm · {1} đơn vị tồn",
                ds.Count, TongSoLuongTon(ds));
        }

        private static int TongSoLuongTon(IList<SanPham> ds)
        {
            int tong = 0;
            foreach (SanPham sp in ds) tong += sp.SoLuongTon;
            return tong;
        }

        /// <summary>Bài 6: cộng giá trị tồn bằng hàm của tầng BUS.</summary>
        private void CapNhatTong()
        {
            List<SanPham> ds = _bus.GetAll();
            double tong = _bus.TongGiaTriTon(ds);

            lblTongGiaTri.Text = DinhDang.Tien(tong);
            lblTongChiTiet.Text = string.Format(
                "{0} sản phẩm - tổng {1} đơn vị tồn kho", ds.Count, TongSoLuongTon(ds));

            frmMain main = MdiParent as frmMain;
            if (main != null) main.YeuCauCapNhat();
        }

        // =====================================================================
        // BÀI 6: TÌM + SẮP XẾP + KIỂM TRA LẠI TỔNG
        // =====================================================================
        private void txtTim_TextChanged(object sender, EventArgs e)
        {
            NapLuoi(_bus.TimKiem(UiFactory.Val(txtTim)));
        }

        private void btnSapXepTon_Click(object sender, EventArgs e)
        {
            NapLuoi(_bus.SapXep(_bus.GetAll(), "ton", true));
            GhiNhatKy("Sắp xếp theo số lượng tồn giảm dần");
        }

        private void btnSapXepGiaTri_Click(object sender, EventArgs e)
        {
            NapLuoi(_bus.SapXep(_bus.GetAll(), "giatri", true));
            GhiNhatKy("Sắp xếp theo giá trị tồn giảm dần");
        }

        /// <summary>
        /// Kiểm tra lại tổng bằng cách tính thủ công trên lưới và so với BUS -
        /// demo rằng quy tắc nghiệp vụ chỉ nằm ở BUS (sửa BUS là mọi Form cập nhật theo).
        /// </summary>
        private void btnKiemTraTong_Click(object sender, EventArgs e)
        {
            double theoLuoi = 0;
            foreach (DataGridViewRow row in dgvSanPham.Rows)
            {
                SanPham sp = row.Tag as SanPham;
                if (sp != null) theoLuoi += sp.DonGia * sp.SoLuongTon;
            }

            double theoBus = _bus.TongGiaTriTon(_bus.GetAll());
            _lanKiemTra++;

            if (Math.Abs(theoLuoi - theoBus) < 0.5)
            {
                lblKetQua.Text = string.Format(
                    "Lần {0}: khớp - lưới {1} = BUS {2}",
                    _lanKiemTra, DinhDang.Tien(theoLuoi), DinhDang.Tien(theoBus));
                lblKetQua.ForeColor = UiTheme.Green;
            }
            else
            {
                lblKetQua.Text = string.Format("Lần {0}: KHÔNG khớp (lưới {1} / BUS {2})",
                    _lanKiemTra, DinhDang.Tien(theoLuoi), DinhDang.Tien(theoBus));
                lblKetQua.ForeColor = UiTheme.Red;
            }

            GhiNhatKy(lblKetQua.Text);
        }

        private void GhiNhatKy(string text)
        {
            frmMain main = MdiParent as frmMain;
            if (main != null) main.Log(text);
        }
    }
}
