using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyNhaThuoc.BUS;
using QuanLyNhaThuoc.GUI.Helpers;

namespace QuanLyNhaThuoc.GUI
{
    /// <summary>Dang nhap: thanh cong thi DialogResult.OK, PhienDangNhap.HienTai da co nhan vien.</summary>
    public class frmDangNhap : FormBase
    {
        private readonly NhanVienBUS _bus = new NhanVienBUS();
        private readonly TextBox _txtTen = Ui.O("TenDangNhap");
        private readonly TextBox _txtMatKhau = Ui.O("MatKhau");
        private int _soLanSai;

        public frmDangNhap()
        {
            Text = "Đăng nhập - Quản lý nhà thuốc";
            ClientSize = new Size(440, 330);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;

            var hero = new KhungVien { Dock = DockStyle.Top, Height = 76, VienDuoi = true, Padding = new Padding(28, 0, 0, 1) };
            hero.Controls.Add(new Logo { Dock = DockStyle.Fill, PhuDe = "Đăng nhập để bắt đầu ca làm việc" });
            _txtMatKhau.UseSystemPasswordChar = true;

            var khung = Ui.KhungNhap(1, "Tên đăng nhập", _txtTen, "Mật khẩu", _txtMatKhau);
            khung.Padding = new Padding(28, 22, 28, 6);

            var btn = Ui.Nut("btnDangNhap", "Đăng nhập", UiTheme.Teal, (s, e) => DangNhap());
            var btnThoat = Ui.Nut("btnThoat", "Thoát", UiTheme.Slate, (s, e) => Close(), true);
            var thanh = Ui.ThanhNut(btn, btnThoat);
            thanh.Padding = new Padding(28, 8, 28, 8);
            var goiY = new Label { Dock = DockStyle.Fill, Text = "Tài khoản mẫu: admin / admin123 (Quản lý) · duocsi / 123456 (Dược sĩ)", ForeColor = UiTheme.Muted, Padding = new Padding(28, 8, 12, 0), BackColor = Color.White };

            Controls.Add(goiY);
            Controls.Add(thanh);
            Controls.Add(khung);
            Controls.Add(hero);
            AcceptButton = btn;
            BackColor = Color.White;
        }

        private void DangNhap()
        {
            try
            {
                _bus.DangNhap(_txtTen.Text, _txtMatKhau.Text);
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                _soLanSai++;
                _txtMatKhau.Clear();
                Ui.BaoLoi(this, ex, "đăng nhập");
                // Cham dan sau nhieu lan sai, han che do mat khau tu dong.
                if (_soLanSai >= 3) System.Threading.Thread.Sleep(Math.Min(5000, 1000 * (_soLanSai - 2)));
            }
        }
    }

    /// <summary>Nhan vien tu doi mat khau.</summary>
    public class frmDoiMatKhau : FormBase
    {
        public frmDoiMatKhau()
        {
            Text = "Đổi mật khẩu";
            ClientSize = new Size(420, 230);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;

            TextBox cu = Ui.O("MatKhauCu"), moi = Ui.O("MatKhau"), xn = Ui.O("XacNhan");
            cu.UseSystemPasswordChar = moi.UseSystemPasswordChar = xn.UseSystemPasswordChar = true;
            var khung = Ui.KhungNhap(1, "Mật khẩu hiện tại", cu, "Mật khẩu mới", moi, "Nhập lại", xn);
            var btn = Ui.Nut("btnLuu", "Đổi mật khẩu", UiTheme.Teal, (s, e) =>
            {
                try
                {
                    new NhanVienBUS().DoiMatKhau(cu.Text, moi.Text, xn.Text);
                    Ui.ThongBao(this, "Đã đổi mật khẩu.");
                    Close();
                }
                catch (Exception ex) { Ui.BaoLoi(this, ex, "đổi mật khẩu"); }
            });
            Controls.Add(Ui.ThanhNut(btn));
            Controls.Add(khung);
            Controls[0].Dock = DockStyle.Bottom;
            AcceptButton = btn;
        }
    }
}
