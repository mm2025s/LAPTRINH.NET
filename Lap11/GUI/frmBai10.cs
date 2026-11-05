using System;
using System.Drawing;
using System.Windows.Forms;
using Lab11_CRUD_StoredProcedure.BUS;
using Lab11_CRUD_StoredProcedure.GUI.Helpers;

namespace Lab11_CRUD_StoredProcedure.GUI
{
    /// <summary>
    /// BAI 10: Chuan hoa xu ly loi. Moi nut gay mot loi that; bang tren la
    /// nhung gi nguoi dung thay, khung duoi la nhat ky cho lap trinh vien.
    /// </summary>
    public class frmBai10 : FormBase
    {
        private readonly DataGridView _luoi = new DataGridView();
        private readonly TextBox _txtNhatKy = new TextBox();

        public frmBai10()
        {
            Text = "Bài 10 - Chuẩn hóa xử lý lỗi";
            ClientSize = new Size(1150, 720);
            StartPosition = FormStartPosition.CenterScreen;

            _luoi.Dock = DockStyle.Fill;
            _luoi.ReadOnly = true;
            _luoi.AllowUserToAddRows = false;
            _luoi.RowHeadersVisible = false;
            _luoi.BackgroundColor = Color.White;
            _luoi.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _luoi.Columns.Add("TinhHuong", "Tình huống");
            _luoi.Columns.Add("Loai", "Loại lỗi");
            _luoi.Columns.Add("ThongBao", "Thông báo cho người dùng");
            _luoi.Columns[0].FillWeight = 34;
            _luoi.Columns[1].FillWeight = 12;
            _luoi.Columns[2].FillWeight = 54;

            _txtNhatKy.Dock = DockStyle.Bottom;
            _txtNhatKy.Height = 240;
            _txtNhatKy.Multiline = true;
            _txtNhatKy.ReadOnly = true;
            _txtNhatKy.WordWrap = false;
            _txtNhatKy.ScrollBars = ScrollBars.Both;
            _txtNhatKy.Font = UiTheme.FontMono(9f);

            var thanhNut = new FlowLayoutPanel();
            thanhNut.Dock = DockStyle.Top;
            thanhNut.Height = 104;
            thanhNut.Padding = new Padding(10);

            var nutTatCa = new AccentButton();
            nutTatCa.Name = "btnChayTatCa";
            nutTatCa.Text = "Chạy tất cả";
            nutTatCa.BaseColor = UiTheme.Blue;
            nutTatCa.Size = new Size(130, 36);
            nutTatCa.Click += (s, e) => ChayTatCa();
            thanhNut.Controls.Add(nutTatCa);

            foreach (TinhHuongLoi th in TinhHuongLoi.DanhSach())
            {
                TinhHuongLoi hienTai = th;
                var nut = new Button();
                nut.Text = th.Ten;
                nut.AutoSize = true;
                nut.Height = 36;
                nut.Click += (s, e) => Chay(hienTai);
                thanhNut.Controls.Add(nut);
            }

            // Thu tu Add quyet dinh Dock: control Fill phai duoc them dau tien.
            Controls.Add(_luoi);
            Controls.Add(_txtNhatKy);
            Controls.Add(thanhNut);
        }

        public void ChayTatCa()
        {
            _luoi.Rows.Clear();
            foreach (TinhHuongLoi th in TinhHuongLoi.DanhSach()) Chay(th);
        }

        private void Chay(TinhHuongLoi th)
        {
            try
            {
                th.Lenh();
                _luoi.Rows.Add(th.Ten, "-", "Không phát sinh lỗi.");
            }
            catch (Exception ex)
            {
                ThongTinLoi tt = XuLyLoi.PhanLoai(ex, th.Ten);
                _luoi.Rows.Add(th.Ten, tt.TenLoai, tt.ThongBao);
            }
            _txtNhatKy.Text = XuLyLoi.DocNhatKy();
        }
    }
}
