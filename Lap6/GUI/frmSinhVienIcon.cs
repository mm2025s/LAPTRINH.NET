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
    /// frmSinhVienIcon - Bài 8: ListView (chi tiết) + ImageList (biểu tượng tự vẽ).
    ///
    /// Không dùng tệp ảnh ngoài: biểu tượng được sinh bằng GDI+ (UiTheme.CreateAvatarImage)
    /// để project không cần thư mục Resources.
    /// </summary>
    public partial class frmSinhVienIcon : FormBase
    {
        private readonly SinhVienBUS _bus;
        private ImageList _bieuTuong;

        public frmSinhVienIcon()
        {
            InitializeComponent();
            _bus = new SinhVienBUS(new DAL.SinhVienDAL());
            DuLieuMau.NapSinhVien(_bus);
        }

        public frmSinhVienIcon(SinhVienBUS bus)
        {
            InitializeComponent();
            _bus = bus;
        }

        private void frmSinhVienIcon_Load(object sender, EventArgs e)
        {
            TaoBieuTuong();
            NapDanhSach(_bus.GetAll());
        }

        /// <summary>Tạo 3 nhóm biểu tượng: điểm ≥ 8 (xanh), 5 - 8 (vàng), dưới 5 (đỏ).</summary>
        private void TaoBieuTuong()
        {
            _bieuTuong = new ImageList();
            _bieuTuong.ImageSize = new Size(40, 40);
            _bieuTuong.ColorDepth = ColorDepth.Depth32Bit;
            _bieuTuong.TransparentColor = Color.Transparent;

            _bieuTuong.Images.Add("gioi", VeBieuTuong(UiTheme.Green, "★"));
            _bieuTuong.Images.Add("kha", VeBieuTuong(UiTheme.Amber, "✓"));
            _bieuTuong.Images.Add("yeu", VeBieuTuong(UiTheme.Red, "!"));

            lvSinhVien.LargeImageList = _bieuTuong;
            lvSinhVien.SmallImageList = _bieuTuong;
        }

        private static Bitmap VeBieuTuong(Color mau, string kyHieu)
        {
            var bmp = new Bitmap(40, 40);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                UiTheme.SmoothingOn(g);
                g.Clear(Color.Transparent);

                using (var brush = new SolidBrush(mau)) g.FillEllipse(brush, 1, 1, 38, 38);
                using (var pen = new Pen(Color.FromArgb(60, Color.White), 2f)) g.DrawEllipse(pen, 3, 3, 34, 34);

                using (var font = UiTheme.FontBold(14f))
                using (var brush = new SolidBrush(Color.White))
                using (var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    g.DrawString(kyHieu, font, brush, new RectangleF(0, 0, 40, 40), fmt);
                }
            }
            return bmp;
        }

        private string KhoaBieuTuong(double diem)
        {
            if (diem >= 8) return "gioi";
            if (diem >= 5) return "kha";
            return "yeu";
        }

        public void NapDanhSach(IList<SinhVien> ds)
        {
            lvSinhVien.BeginUpdate();
            lvSinhVien.Items.Clear();

            foreach (SinhVien sv in ds)
            {
                var item = new ListViewItem(sv.MaSV + " - " + sv.HoTen);
                item.SubItems.Add(sv.Khoa);
                item.SubItems.Add(sv.NgaySinh.ToString("dd/MM/yyyy"));
                item.SubItems.Add(sv.DiemTB.ToString("0.0"));
                item.SubItems.Add(KiemTra.XepLoai(sv.DiemTB));
                item.ImageKey = KhoaBieuTuong(sv.DiemTB);
                item.Tag = sv;
                lvSinhVien.Items.Add(item);
            }

            lvSinhVien.EndUpdate();
            lblDem.Text = string.Format("{0} sinh viên · {1} biểu tượng trong ImageList",
                lvSinhVien.Items.Count, _bieuTuong.Images.Count);
        }

        private void lvSinhVien_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvSinhVien.SelectedItems.Count == 0) return;

            SinhVien sv = lvSinhVien.SelectedItems[0].Tag as SinhVien;
            if (sv == null) return;

            lblTen.Text = sv.HoTen;
            lblPhu.Text = sv.MaSV + " · " + sv.Khoa + " · " + sv.GioiTinh + " · " + SinhVienBUS.TinhTuoi(sv.NgaySinh) + " tuổi";
            lblDiem.Text = "Điểm " + sv.DiemTB.ToString("0.0") + " - " + KiemTra.XepLoai(sv.DiemTB);
            lblDiem.ForeColor = KiemTra.MauXepLoai(sv.DiemTB);

            picAnh.Image = UiTheme.CreateAvatarImage(sv.HoTen, 96, KiemTra.MauXepLoai(sv.DiemTB));
        }

        private void btnChiTiet_Click(object sender, EventArgs e)
        {
            lvSinhVien.View = View.Details;
            GhiNhatKy("Chuyển ListView sang chế độ Details");
        }

        private void btnBieuTuong_Click(object sender, EventArgs e)
        {
            lvSinhVien.View = View.LargeIcon;
            GhiNhatKy("Chuyển ListView sang chế độ LargeIcon (hiện biểu tượng)");
        }

        private void btnDanhSachNho_Click(object sender, EventArgs e)
        {
            lvSinhVien.View = View.SmallIcon;
            GhiNhatKy("Chuyển ListView sang chế độ SmallIcon");
        }

        private void btnSapXepDiem_Click(object sender, EventArgs e)
        {
            NapDanhSach(_bus.SapXep(_bus.GetAll(), "diem", true));
            GhiNhatKy("Sắp xếp theo điểm giảm dần (BUS.SapXep)");
        }

        private void GhiNhatKy(string text)
        {
            frmMain main = MdiParent as frmMain;
            if (main != null) main.Log(text);
        }
    }
}
