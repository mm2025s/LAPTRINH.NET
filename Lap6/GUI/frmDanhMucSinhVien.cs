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
    /// frmDanhMucSinhVien - Bài 4: cây khoa → sinh viên.
    ///
    /// Dữ liệu nhóm theo khoa do BUS.NhomTheoKhoa() cung cấp; Form chỉ vẽ cây.
    /// Chọn một sinh viên -> xem chi tiết bằng sự kiện SelectedNode.
    /// </summary>
    public partial class frmDanhMucSinhVien : FormBase
    {
        private readonly SinhVienBUS _bus;

        public frmDanhMucSinhVien()
        {
            InitializeComponent();
            _bus = new SinhVienBUS(new DAL.SinhVienDAL());
            DuLieuMau.NapSinhVien(_bus);
        }

        public frmDanhMucSinhVien(SinhVienBUS bus)
        {
            InitializeComponent();
            _bus = bus;
        }

        private void frmDanhMucSinhVien_Load(object sender, EventArgs e)
        {
            NapCay();
        }

        public void NapCay()
        {
            tvKhoa.BeginUpdate();
            tvKhoa.Nodes.Clear();

            Dictionary<string, List<SinhVien>> nhom = _bus.NhomTheoKhoa();
            int tong = 0;
            foreach (KeyValuePair<string, List<SinhVien>> cap in nhom)
            {
                double diemTB = 0;
                foreach (SinhVien sv in cap.Value) diemTB += sv.DiemTB;
                if (cap.Value.Count > 0) diemTB = diemTB / cap.Value.Count;

                // Sắp xếp sinh viên trong khoa theo tên (BUS chưa cung cấp hàm riêng cho bước này)
                List<SinhVien> ds = _bus.SapXep(cap.Value, "ten", false);

                TreeNode nutKhoa = new TreeNode(string.Format("{0}  ({1} sinh viên · ĐTB {2:0.00})",
                    cap.Key, ds.Count, diemTB));
                nutKhoa.Tag = cap.Key;
                nutKhoa.ForeColor = UiTheme.Ink;

                foreach (SinhVien sv in ds)
                {
                    TreeNode nut = new TreeNode(string.Format("{0}  -  {1}  ({2:0.0})", sv.MaSV, sv.HoTen, sv.DiemTB));
                    nut.Tag = sv;
                    nut.ForeColor = sv.DiemTB >= 5 ? UiTheme.InkSoft : UiTheme.Red;
                    nutKhoa.Nodes.Add(nut);
                }

                tvKhoa.Nodes.Add(nutKhoa);
                tong += ds.Count;
            }

            tvKhoa.EndUpdate();
            if (tvKhoa.Nodes.Count > 0) tvKhoa.Nodes[0].Expand();

            lblDem.Text = string.Format("{0} khoa · {1} sinh viên", nhom.Count, tong);
            if (tvKhoa.Nodes.Count > 0)
            {
                tvKhoa.SelectedNode = tvKhoa.Nodes[0];
                HienThi(tvKhoa.Nodes[0]);
            }
        }

        private void tvKhoa_AfterSelect(object sender, TreeViewEventArgs e)
        {
            HienThi(e.Node);
        }

        private void HienThi(TreeNode nut)
        {
            if (nut == null) return;

            SinhVien sv = nut.Tag as SinhVien;
            if (sv != null)
            {
                lblTieuDe.Text = sv.HoTen;
                lblPhu.Text = "Mã " + sv.MaSV + " · " + sv.Khoa + " · " + sv.GioiTinh;
                lblDiem.Text = "Điểm " + sv.DiemTB.ToString("0.0") + " - " + KiemTra.XepLoai(sv.DiemTB);
                lblDiem.ForeColor = KiemTra.MauXepLoai(sv.DiemTB);
                lblChiTiet.Text = string.Format("Ngày sinh {0} ({1} tuổi)\r\nEmail: {2}",
                    DinhDang.Ngay(sv.NgaySinh), SinhVienBUS.TinhTuoi(sv.NgaySinh),
                    string.IsNullOrEmpty(sv.Email) ? "(chưa có)" : sv.Email);
                return;
            }

            string khoa = nut.Tag as string;
            if (khoa != null)
            {
                lblTieuDe.Text = khoa;
                lblPhu.Text = nut.Nodes.Count + " sinh viên thuộc khoa";
                lblDiem.Text = string.Empty;
                lblChiTiet.Text = "Chọn một sinh viên trong cây để xem chi tiết.\r\n" +
                    "Cây được tạo từ BUS.NhomTheoKhoa() - GUI không tự nhóm dữ liệu.";
            }
        }

        private void btnMoRong_Click(object sender, EventArgs e)
        {
            tvKhoa.ExpandAll();
        }

        private void btnThuGon_Click(object sender, EventArgs e)
        {
            tvKhoa.CollapseAll();
            if (tvKhoa.Nodes.Count > 0) tvKhoa.Nodes[0].Expand();
        }

        private void btnXemSinhVienGioi_Click(object sender, EventArgs e)
        {
            // Lọc bằng BUS, rồi dựng lại cây theo kết quả (Form không tự lọc)
            List<SinhVien> gioi = _bus.TimKiem(string.Empty, string.Empty);
            tvKhoa.BeginUpdate();
            tvKhoa.Nodes.Clear();

            TreeNode nutGioi = new TreeNode(string.Format("Sinh viên điểm cao (≥ 8.0)"));
            nutGioi.Tag = "Sinh viên điểm cao";
            foreach (SinhVien sv in gioi)
            {
                if (sv.DiemTB < 8) continue;
                TreeNode nut = new TreeNode(string.Format("{0}  -  {1}  ({2:0.0})", sv.MaSV, sv.HoTen, sv.DiemTB));
                nut.Tag = sv;
                nut.ForeColor = UiTheme.Green;
                nutGioi.Nodes.Add(nut);
            }
            nutGioi.Text = string.Format("Sinh viên điểm cao (≥ 8.0) · {0} sinh viên", nutGioi.Nodes.Count);
            tvKhoa.Nodes.Add(nutGioi);
            nutGioi.Expand();
            tvKhoa.EndUpdate();

            lblDem.Text = string.Format("Lọc: {0} sinh viên đạt ≥ 8.0", nutGioi.Nodes.Count);
            GhiNhatKy("Lọc theo điểm ≥ 8.0 qua BUS");
        }

        private void btnNapLai_Click(object sender, EventArgs e)
        {
            NapCay();
            GhiNhatKy("Nạp lại cây khoa - sinh viên");
        }

        private void GhiNhatKy(string text)
        {
            frmMain main = MdiParent as frmMain;
            if (main != null) main.Log(text);
        }
    }
}
