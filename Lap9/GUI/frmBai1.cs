using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Lab09_DataReaderSearch.BUS;
using Lab09_DataReaderSearch.GUI.Helpers;

namespace Lab09_DataReaderSearch.GUI
{
    /// <summary>
    /// BAI 1 - Nap SqlDataReader vao DataTable.
    ///
    /// So sanh hai cach nap cung mot ket qua:
    ///   (a) DataTable.Load(reader)  -> mot lenh, ADO.NET tu tao cot theo schema
    ///   (b) while (reader.Read())   -> tu tao cot, tu doc tung truong
    ///
    /// Ca hai deu phai cho ra cung so dong; Form in so dong va thoi gian chay
    /// de thay ro cach (a) ngan gon hon nhieu.
    /// </summary>
    public class frmBai1 : BaiFormBase
    {
        private readonly SinhVienBUS _bus = new SinhVienBUS();

        private LuoiSinhVien _luoi;
        private Label _lblSoSanh;
        private TextBox _txtSql;

        public frmBai1()
            : base("1", "Nạp SqlDataReader vào DataTable",
                   "DataTable.Load(reader) và cách nạp thủ công cho kết quả như nhau",
                   UiTheme.Violet, "ExecuteReader", "DataTable.Load", "frmBai1",
                   new Size(1010, 700))
        {
            Badge = new BadgeLabel();
            Badge.Value = "DataTable.Load";
            Badge.BaseColor = UiTheme.Violet;
            Badge.SetBounds(NoiDung.Right - 160, 20, 140, 24);

            TaoThanhCongCu();
            TaoLuoi();
            TaoKhucSoSanh();
        }

        private void TaoThanhCongCu()
        {
            CardPanel the = The(18, 18, 962, 104);
            NoiDung.Controls.Add(the);

            Label tieuDe = TieuDeMuc("NẠP DỮ LIỆU BẰNG SqlDataReader", UiTheme.Violet);
            tieuDe.SetBounds(18, 14, 420, 22);
            the.Controls.Add(tieuDe);

            AccentButton btnLoad = Nut("btnLoad", "Nạp bằng DataTable.Load", UiTheme.Violet, 18, 46, 226, 38);
            btnLoad.Click += (s, e) => Nap(true);
            the.Controls.Add(btnLoad);

            AccentButton btnThuCong = Nut("btnThuCong", "Nạp thủ công (reader.Read)", UiTheme.Indigo, 256, 46, 226, 38, true);
            btnThuCong.Click += (s, e) => Nap(false);
            the.Controls.Add(btnThuCong);

            _lblSoSanh = UiFactory.Label("Chưa nạp dữ liệu.", 9.25f, UiTheme.InkSoft, false);
            _lblSoSanh.SetBounds(500, 52, 444, 26);
            the.Controls.Add(_lblSoSanh);

            _txtSql = new TextBox();
            _txtSql.Name = "txtSql";
            _txtSql.Multiline = true;
            _txtSql.ReadOnly = true;
            _txtSql.ScrollBars = ScrollBars.Vertical;
            _txtSql.Font = UiTheme.FontMono(8f);
            _txtSql.BackColor = Color.FromArgb(30, 34, 48);
            _txtSql.ForeColor = Color.FromArgb(190, 232, 200);
            _txtSql.BorderStyle = BorderStyle.None;
            _txtSql.SetBounds(18, 92, 926, 0);   // an di, giu de tham chieu SQL
            _txtSql.Text = "SELECT sv.MaSV, sv.HoTen, ... FROM SinhVien sv JOIN Khoa k ON ...";
            the.Controls.Add(_txtSql);
        }

        private void TaoLuoi()
        {
            CardPanel the = The(18, 132, 962, 470);
            the.Padding = new Padding(1);
            NoiDung.Controls.Add(the);

            _luoi = new LuoiSinhVien();
            _luoi.Name = "luoiSinhVien";
            _luoi.Dock = DockStyle.Fill;
            the.Controls.Add(_luoi);
        }

        private void TaoKhucSoSanh()
        {
            Label ghiChu = UiFactory.Label(
                "DataTable.Load() đọc cả result set một lần theo schema có sẵn; " +
                "cách thủ công phải tự khai báo DataColumn và tự đọc từng trường bằng GetXxx/this[string].",
                8.5f, UiTheme.Muted, false);
            ghiChu.SetBounds(18, 610, 962, 22);
            NoiDung.Controls.Add(ghiChu);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Nap(true);
        }

        private void Nap(bool dungLoad)
        {
            try
            {
                var dongHo = Stopwatch.StartNew();
                DataTable table;
                string cach;

                if (dungLoad)
                {
                    table = _bus.LoadStudents();
                    cach = "DataTable.Load(reader)";
                    if (Badge != null)
                    {
                        Badge.Value = "DataTable.Load";
                        Badge.SetColor(UiTheme.Violet);
                    }
                }
                else
                {
                    int soDong;
                    table = _bus.NapThuCong(out soDong);
                    cach = "reader.Read() thủ công";
                    if (Badge != null)
                    {
                        Badge.Value = "reader.Read()";
                        Badge.SetColor(UiTheme.Indigo);
                    }
                }

                dongHo.Stop();
                _luoi.GanDuLieu(table);

                _lblSoSanh.Text = string.Format("{0}: {1} dòng · {2} cột · {3} ms",
                    cach, table.Rows.Count, table.Columns.Count, dongHo.ElapsedMilliseconds);

                GhiChu(string.Format("{0} nạp xong {1} dòng trong {2} ms — SqlDataReader đã được đóng cùng connection.",
                    cach, table.Rows.Count, dongHo.ElapsedMilliseconds));
            }
            catch (Exception ex)
            {
                LoiChung(ex);
            }
        }
    }
}
