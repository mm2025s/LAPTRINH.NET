using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QuanLyNhaThuoc.DTO;
using QuanLyNhaThuoc.GUI.Helpers;

namespace QuanLyNhaThuoc.GUI
{
    /// <summary>
    /// Cua so chinh theo bo cuc ApplicationLayout cua hcmue-portal:
    ///   [Sider 256px: Logo | menu nhom | the nguoi dung]  [Header 64px: breadcrumb ... ca lam viec]
    ///                                                      [Noi dung: trang dang chon, nen #f4f4f6]
    /// Moi chuc nang la mot trang (ManHinh) nhung vao vung noi dung, khong mo cua so rieng.
    /// </summary>
    public class frmMain : FormBase
    {
        private readonly NavMenu _menu = new NavMenu { Dock = DockStyle.Fill };
        private readonly Panel _vungTrang = new Panel { Dock = DockStyle.Fill, BackColor = UiTheme.Canvas };
        private readonly Label _lblNhom = new Label { AutoSize = true, Font = UiTheme.FontBody(9.75f), ForeColor = UiTheme.Muted };
        private readonly Label _lblTrang = new Label { AutoSize = true, Font = UiTheme.FontBold(9.75f), ForeColor = UiTheme.Ink };
        private readonly Dictionary<NavMenu.MucMenu, Form> _trang = new Dictionary<NavMenu.MucMenu, Form>();
        private Form _trangHienTai;

        /// <summary>Dang xuat: Program se hien lai man hinh dang nhap.</summary>
        public bool DangXuat { get; private set; }

        public frmMain()
        {
            NhanVienDTO nv = PhienDangNhap.HienTai;
            Text = "Nhà thuốc EAUT - Phần mềm quản lý";
            ClientSize = new Size(1320, 820);
            MinimumSize = new Size(1040, 640);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = UiTheme.Canvas;

            // ---------------- Sider ----------------
            var sider = new KhungVien { Dock = DockStyle.Left, Width = 256, VienPhai = true };
            var dauSider = new KhungVien { Dock = DockStyle.Top, Height = 64, VienDuoi = true, Padding = new Padding(16, 0, 1, 1) };
            dauSider.Controls.Add(new Logo { Dock = DockStyle.Fill, PhuDe = "Quầy thuốc · " + nv.TenVaiTro });

            var cuoiSider = new KhungVien { Dock = DockStyle.Bottom, Height = 104, VienTren = true, Padding = new Padding(16, 12, 12, 8) };
            var avatar = new AnhDaiDien { Text = nv.HoTen, Location = new Point(16, 14) };
            var ten = new Label { Text = nv.HoTen, AutoSize = true, Location = new Point(60, 14), Font = UiTheme.FontBold(9.5f), ForeColor = UiTheme.Ink };
            var vaiTro = new Label { Text = "@" + nv.TenDangNhap + " · " + nv.TenVaiTro, AutoSize = true, Location = new Point(60, 34), Font = UiTheme.FontBody(8.75f), ForeColor = UiTheme.Muted };
            var doiMk = Ui.Nut("btnDoiMatKhau", "Đổi mật khẩu", UiTheme.Slate, (s, e) => new frmDoiMatKhau().ShowDialog(this), true);
            var dangXuat = Ui.Nut("btnDangXuat", "Đăng xuất", UiTheme.Red, (s, e) => { DangXuat = true; Close(); }, true);
            doiMk.SetBounds(16, 62, 112, 32);
            dangXuat.SetBounds(134, 62, 104, 32);
            cuoiSider.Controls.AddRange(new Control[] { avatar, ten, vaiTro, doiMk, dangXuat });

            var vungMenu = new Panel { Dock = DockStyle.Fill, Padding = new Padding(4, 4, 5, 0), BackColor = Color.White, AutoScroll = true };
            vungMenu.Controls.Add(_menu);
            sider.Controls.Add(vungMenu);
            sider.Controls.Add(cuoiSider);
            sider.Controls.Add(dauSider);

            // ---------------- Menu theo vai tro ----------------
            _menu.ThemNhom("Tổng quan");
            _menu.ThemMuc("Bảng điều khiển", "", () => new frmTongQuan());
            _menu.ThemNhom("Bán hàng");
            _menu.ThemMuc("Bán hàng tại quầy", "", () => new frmBanHang());
            _menu.ThemMuc("Tra cứu hóa đơn", "", () => new frmHoaDon());
            _menu.ThemMuc("Khách hàng", "", () => new frmKhachHang());
            _menu.ThemNhom("Kho thuốc");
            _menu.ThemMuc("Danh mục thuốc", "", () => new frmThuoc());
            if (nv.LaQuanLy)
            {
                _menu.ThemMuc("Nhập hàng", "", () => new frmNhapHang());
                _menu.ThemMuc("Nhóm thuốc · NCC", "", () => new frmDanhMuc());
                _menu.ThemNhom("Quản trị");
                _menu.ThemMuc("Thống kê doanh thu", "", () => new frmThongKe());
                _menu.ThemMuc("Nhân viên", "", () => new frmNhanVien());
                _menu.ThemMuc("Nhật ký · Hiệu năng", "", () => new frmNhatKy());
            }
            _menu.ChonMuc += HienTrang;

            // ---------------- Header ----------------
            var header = new KhungVien { Dock = DockStyle.Top, Height = 64, VienDuoi = true };
            var ca = new Label
            {
                AutoSize = true, Anchor = AnchorStyles.Top | AnchorStyles.Right, Font = UiTheme.FontBody(9.25f), ForeColor = UiTheme.Muted,
                Text = "Ca làm việc · " + DateTime.Today.ToString("dddd, dd/MM/yyyy", new System.Globalization.CultureInfo("vi-VN"))
            };
            header.Controls.AddRange(new Control[] { _lblNhom, _lblTrang, ca });
            header.Resize += (s, e) =>
            {
                _lblNhom.Location = new Point(24, 22);
                _lblTrang.Location = new Point(_lblNhom.Right + 2, 22);
                ca.Location = new Point(header.Width - ca.Width - 24, 23);
            };

            var phai = new Panel { Dock = DockStyle.Fill, BackColor = UiTheme.Canvas };
            phai.Controls.Add(_vungTrang);
            phai.Controls.Add(header);

            Controls.Add(phai);
            Controls.Add(sider);

            Load += (s, e) => { if (_trangHienTai == null) _menu.Chon(_menu.TimTheoTen("Bảng điều khiển")); };
        }

        /// <summary>Hien trang cua muc menu; moi trang chi tao mot lan va giu nguyen trang thai (vi du gio hang).</summary>
        private void HienTrang(NavMenu.MucMenu m)
        {
            Form trang;
            if (!_trang.TryGetValue(m, out trang) || trang.IsDisposed)
            {
                trang = m.TaoTrang();
                _trang[m] = trang;
                ((ManHinh)trang).NhungVao(_vungTrang);
            }
            HienForm(trang, m.Nhom);
            var lm = trang as ILamMoi;      // trang tong quan / nhat ky: luon tai so lieu moi
            if (lm != null) lm.LamMoi();
        }

        private void HienForm(Form trang, string nhom)
        {
            if (_trangHienTai != null && _trangHienTai != trang) _trangHienTai.Hide();
            trang.Show();
            trang.BringToFront();
            _trangHienTai = trang;
            _lblNhom.Text = nhom + "  /";
            _lblTrang.Text = ((ManHinh)trang).TieuDe;
            _lblTrang.Left = _lblNhom.Right + 2;
        }

        /// <summary>Mo mot trang bat ky (dung cho cong cu chup anh); neu da co trang cung loai thi dung lai.</summary>
        public void Mo(Form f)
        {
            foreach (var cap in _trang)
            {
                if (cap.Value.GetType() == f.GetType() && !cap.Value.IsDisposed)
                {
                    f.Dispose();
                    _menu.Chon(cap.Key);
                    return;
                }
            }
            string nhom = "Chức năng";
            foreach (var t in new[] { "Bảng điều khiển", "Bán hàng tại quầy", "Tra cứu hóa đơn", "Khách hàng", "Danh mục thuốc", "Nhập hàng", "Nhóm thuốc · NCC", "Thống kê doanh thu", "Nhân viên", "Nhật ký · Hiệu năng" })
            {
                NavMenu.MucMenu m = _menu.TimTheoTen(t);
                if (m != null && CungLoai(m, f)) { nhom = m.Nhom; _trang[m] = f; ChonKhongTao(m); break; }
            }
            ((ManHinh)f).NhungVao(_vungTrang);
            HienForm(f, nhom);
        }

        private static bool CungLoai(NavMenu.MucMenu m, Form f)
        {
            using (Form mau = m.TaoTrang()) return mau.GetType() == f.GetType();
        }

        private void ChonKhongTao(NavMenu.MucMenu m)
        {
            _menu.ChonMuc -= HienTrang;
            _menu.Chon(m);
            _menu.ChonMuc += HienTrang;
        }

        public void LamMoi()
        {
            var lm = _trangHienTai as ILamMoi;
            if (lm != null) lm.LamMoi();
        }
    }
}
