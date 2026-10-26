using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Lab06_ThreeLayer.BUS;
using Lab06_ThreeLayer.DTO;
using Lab06_ThreeLayer.GUI.Helpers;

namespace Lab06_ThreeLayer.GUI
{
    /// <summary>
    /// frmDanhMuc - Bài 7: TreeView danh mục -> sản phẩm + ContextMenuStrip.
    ///
    /// Thao tác trên cây (thêm, đổi tên, xóa) đều được chuyển cho DanhMucBUS;
    /// quy tắc "danh mục còn sản phẩm thì không cho xóa" nằm ở tầng nghiệp vụ.
    /// </summary>
    public partial class frmDanhMuc : FormBase
    {
        private readonly DanhMucBUS _bus;

        public frmDanhMuc()
        {
            InitializeComponent();
            _bus = new DanhMucBUS(new DAL.DanhMucDAL(), new DAL.SanPhamDAL());
            DuLieuMau.NapSanPham(new SanPhamBUS(new DAL.SanPhamDAL(), new DAL.DanhMucDAL()), _bus);
        }

        public frmDanhMuc(DanhMucBUS bus)
        {
            InitializeComponent();
            _bus = bus;
        }

        // =====================================================================
        // NẠP CÂY
        // =====================================================================
        private void frmDanhMuc_Load(object sender, EventArgs e)
        {
            NapCay();
        }

        /// <summary>Dựng lại cây: cấp 1 = danh mục, cấp 2 = sản phẩm thuộc danh mục.</summary>
        public void NapCay()
        {
            tvDanhMuc.BeginUpdate();
            tvDanhMuc.Nodes.Clear();

            int tongSP = 0;
            foreach (DanhMuc dm in _bus.GetAll())
            {
                TreeNode nutDM = new TreeNode(dm.TenDanhMuc);
                nutDM.Tag = dm;
                nutDM.Name = "DM_" + dm.MaDanhMuc;

                List<SanPham> ds = _bus.SanPhamCua(dm.MaDanhMuc);
                foreach (SanPham sp in ds)
                {
                    TreeNode nutSP = new TreeNode(sp.TenSP + "  (" + DinhDang.Tien(sp.DonGia) + ")");
                    nutSP.Tag = sp;
                    nutSP.ForeColor = UiTheme.InkSoft;
                    nutDM.Nodes.Add(nutSP);
                }
                tongSP += ds.Count;

                nutDM.Text = string.Format("{0}  ·  {1} sản phẩm", dm.TenDanhMuc, ds.Count);
                tvDanhMuc.Nodes.Add(nutDM);
            }

            tvDanhMuc.ExpandAll();
            tvDanhMuc.EndUpdate();

            lblDem.Text = string.Format("{0} danh mục · {1} sản phẩm", _bus.GetAll().Count, tongSP);
            HienThiChiTiet(tvDanhMuc.SelectedNode == null ? (tvDanhMuc.Nodes.Count > 0 ? tvDanhMuc.Nodes[0] : null) : tvDanhMuc.SelectedNode);
        }

        private void tvDanhMuc_AfterSelect(object sender, TreeViewEventArgs e)
        {
            HienThiChiTiet(e.Node);
        }

        private void HienThiChiTiet(TreeNode nut)
        {
            if (nut == null)
            {
                lblChonTen.Text = "Chưa chọn mục nào";
                lblChonLoai.Text = string.Empty;
                lblChonChiTiet.Text = string.Empty;
                return;
            }

            DanhMuc dm = nut.Tag as DanhMuc;
            if (dm != null)
            {
                List<SanPham> ds = _bus.SanPhamCua(dm.MaDanhMuc);
                double tong = 0;
                foreach (SanPham sp in ds) tong += sp.GiaTriTon;

                lblChonTen.Text = dm.TenDanhMuc;
                lblChonLoai.Text = "Danh mục " + dm.MaDanhMuc + "  ·  " + ds.Count + " sản phẩm";
                lblChonChiTiet.Text = "Tổng giá trị tồn: " + DinhDang.Tien(tong);
                lblChonLoai.ForeColor = UiTheme.Indigo;
                return;
            }

            SanPham p = nut.Tag as SanPham;
            if (p != null)
            {
                lblChonTen.Text = p.TenSP;
                lblChonLoai.Text = "Sản phẩm " + p.MaSP + "  ·  danh mục " + p.MaDanhMuc;
                lblChonChiTiet.Text = string.Format("Đơn giá {0}  ·  tồn {1}  ·  giá trị {2}",
                    DinhDang.Tien(p.DonGia), p.SoLuongTon, DinhDang.Tien(p.GiaTriTon));
                lblChonLoai.ForeColor = UiTheme.Teal;
            }
        }

        // =====================================================================
        // CONTEXT MENU: THÊM / ĐỔI TÊN / XÓA
        // =====================================================================
        private void mnuThemDanhMuc_Click(object sender, EventArgs e)
        {
            string ma = Hoi("Nhập mã danh mục mới (VD: DM04):", "Thêm danh mục");
            if (string.IsNullOrEmpty(ma)) return;

            string ten = Hoi("Nhập tên danh mục:", "Thêm danh mục");
            if (string.IsNullOrEmpty(ten)) return;

            try
            {
                _bus.ThemDanhMuc(ma, ten);
                NapCay();
                GhiNhatKy("Đã thêm danh mục " + ma.ToUpperInvariant() + " - " + ten);
            }
            catch (LoiNghiepVu ex)
            {
                BaoLoi(ex.Message);
            }
        }

        private void mnuDoiTen_Click(object sender, EventArgs e)
        {
            DanhMuc dm = LayDanhMucDangChon();
            if (dm == null) { BaoLoi("Hãy chọn một danh mục (cấp 1) để đổi tên."); return; }

            string ten = Hoi("Tên mới cho danh mục " + dm.MaDanhMuc + ":", "Đổi tên danh mục");
            if (string.IsNullOrEmpty(ten)) return;

            try
            {
                _bus.DoiTenDanhMuc(dm.MaDanhMuc, ten);
                NapCay();
                GhiNhatKy("Đã đổi tên " + dm.MaDanhMuc + " thành " + ten);
            }
            catch (LoiNghiepVu ex)
            {
                BaoLoi(ex.Message);
            }
        }

        private void mnuXoa_Click(object sender, EventArgs e)
        {
            DanhMuc dm = LayDanhMucDangChon();
            if (dm == null) { BaoLoi("Hãy chọn một danh mục (cấp 1) để xóa."); return; }

            try
            {
                _bus.XoaDanhMuc(dm.MaDanhMuc);
                NapCay();
                GhiNhatKy("Đã xóa danh mục " + dm.MaDanhMuc);
            }
            catch (LoiNghiepVu ex)
            {
                // Ví dụ: "Danh mục còn sản phẩm, không thể xóa."
                BaoLoi(ex.Message);
            }
        }

        private void mnuNapLai_Click(object sender, EventArgs e)
        {
            NapCay();
            GhiNhatKy("Nạp lại cây danh mục");
        }

        private DanhMuc LayDanhMucDangChon()
        {
            TreeNode nut = tvDanhMuc.SelectedNode;
            if (nut == null) return null;
            return nut.Tag as DanhMuc;
        }

        private string Hoi(string cauHoi, string tieuDe)
        {
            // Hàm nhập liệu dùng chung của Lab 6 (không phụ thuộc Microsoft.VisualBasic)
            return NhapLieu.HoiChuoi(this, cauHoi, tieuDe);
        }

        private void BaoLoi(string text)
        {
            lblKetQua.Text = "BUS từ chối: " + text;
            lblKetQua.ForeColor = UiTheme.Red;
            GhiNhatKy("BUS từ chối: " + text);
        }

        private void GhiNhatKy(string text)
        {
            frmMain main = MdiParent as frmMain;
            if (main != null) main.Log(text);
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            mnuThemDanhMuc_Click(sender, e);
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            mnuXoa_Click(sender, e);
        }
    }
}
