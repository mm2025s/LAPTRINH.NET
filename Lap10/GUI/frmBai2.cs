using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Lab10_DisconnectedData.BUS;
using Lab10_DisconnectedData.DTO;
using Lab10_DisconnectedData.GUI.Helpers;

namespace Lab10_DisconnectedData.GUI
{
    /// <summary>
    /// BAI 2 - DataSet chua NHIEU bang.
    ///
    /// DataSet la mot CSDL thu nho trong bo nho: nhieu DataTable + quan he +
    /// rang buoc. O day nap Khoa va SinhVien vao cung mot DataSet voi TEN BANG
    /// ro rang.
    ///
    /// Vi sao phai dat ten: ds.Tables["SinhVien"] luon dung, con ds.Tables[1]
    /// se sai ngay khi ai do doi thu tu nap hoac chen them bang.
    /// </summary>
    public class frmBai2 : BaiFormBase
    {
        private readonly SinhVienBUS _bus = new SinhVienBUS();

        private LuoiSinhVien _luoiKhoa;
        private LuoiSinhVien _luoiSinhVien;
        private Label _lblTongKet;

        private DataSet _ds;

        public frmBai2()
            : base("2", "DataSet chứa nhiều bảng",
                   "Một DataSet · hai DataTable tên rõ ràng: \"Khoa\" và \"SinhVien\"",
                   UiTheme.Violet, "DataSet", "Tables[\"tên\"]", "frmBai2",
                   new Size(1120, 730))
        {
            Badge = new BadgeLabel();
            Badge.Value = "1 DataSet · 2 bảng";
            Badge.BaseColor = UiTheme.Violet;
            Badge.SetBounds(NoiDung.Right - 190, 20, 170, 24);

            TaoThanhCongCu();
            TaoHaiLuoi();
        }

        private void TaoThanhCongCu()
        {
            CardPanel the = The(18, 18, 1084, 78);
            NoiDung.Controls.Add(the);

            the.Controls.Add(Nut("btnNap", "Nạp DataSet", UiTheme.Violet, 18, 26, 150));
            the.Controls.Add(Nut("btnChiTiet", "Xem cấu trúc DataSet", UiTheme.Indigo, 180, 26, 190, 36, true));

            _lblTongKet = UiFactory.Label("Chưa nạp dữ liệu.", 9.25f, UiTheme.Muted, false);
            _lblTongKet.SetBounds(388, 26, 676, 30);
            the.Controls.Add(_lblTongKet);

            Tim("btnNap").Click += (s, e) => Nap();
            Tim("btnChiTiet").Click += (s, e) => XemCauTruc();
        }

        private void TaoHaiLuoi()
        {
            // Luoi Khoa (tren)
            CardPanel theKhoa = The(18, 106, 1084, 190);
            theKhoa.Padding = new Padding(1);
            NoiDung.Controls.Add(theKhoa);

            Label nhan1 = TieuDeMuc("DS.TABLES[\"KHOA\"]", UiTheme.Violet);
            nhan1.SetBounds(18, 92, 300, 22);
            NoiDung.Controls.Add(nhan1);

            _luoiKhoa = new LuoiSinhVien();
            _luoiKhoa.Dock = DockStyle.Fill;
            _luoiKhoa.BatToMau = false;
            theKhoa.Controls.Add(_luoiKhoa);

            // Luoi SinhVien (duoi)
            Label nhan2 = TieuDeMuc("DS.TABLES[\"SINHVIEN\"]", UiTheme.Violet);
            nhan2.SetBounds(18, 306, 320, 22);
            NoiDung.Controls.Add(nhan2);

            CardPanel theSv = The(18, 330, 1084, 350);
            theSv.Padding = new Padding(1);
            NoiDung.Controls.Add(theSv);

            _luoiSinhVien = new LuoiSinhVien();
            _luoiSinhVien.Dock = DockStyle.Fill;
            _luoiSinhVien.BatToMau = false;
            theSv.Controls.Add(_luoiSinhVien);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Nap();
        }

        private void Nap()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                _ds = _bus.GetDataSet();

                _luoiKhoa.GanDuLieu(_ds.Tables["Khoa"]);
                _luoiSinhVien.GanDuLieu(_ds.Tables["SinhVien"]);

                DataTable khoa = _ds.Tables["Khoa"];
                DataTable sv = _ds.Tables["SinhVien"];

                _lblTongKet.Text = string.Format(
                    "DataSet \"{0}\" · {1} bảng · Khoa: {2} dòng · SinhVien: {3} dòng · quan hệ: {4}",
                    _ds.DataSetName, _ds.Tables.Count, khoa.Rows.Count, sv.Rows.Count,
                    _ds.Relations.Count == 0 ? "chưa tạo" : _ds.Relations.Count + " quan hệ");

                KetQua("Nạp xong " + _ds.Tables.Count + " bảng vào một DataSet (duy nhất một lần mở kết nối).", true);
            }
            catch (LoiNghiepVu ex) { HienThiLoiNghiepVu(ex); }
            catch (Exception ex) { LoiChung(ex); }
            finally { Cursor = Cursors.Default; }
        }

        /// <summary>
        /// In cau truc DataSet ra dong trang thai: ten bang, so dong, so cot va
        /// kieu du lieu - de thay DataSet mang theo ca SCHEMA chu khong chi du lieu.
        /// </summary>
        private void XemCauTruc()
        {
            try
            {
                if (_ds == null) { Nap(); if (_ds == null) return; }

                var moTa = new System.Text.StringBuilder();
                foreach (DataTable t in _ds.Tables)
                {
                    moTa.Append("[" + t.TableName + ": " + t.Rows.Count + " dòng / " + t.Columns.Count + " cột] ");
                    foreach (DataColumn c in t.Columns)
                    {
                        moTa.Append(c.ColumnName + " (" + c.DataType.Name + ") ");
                    }
                    moTa.Append("| ");
                }

                _lblTongKet.Text = moTa.ToString();
                GhiChu("Kiểu dữ liệu trong DataSet giữ nguyên như trong CSDL — xem chi tiết ở dòng trên.");
            }
            catch (Exception ex) { LoiChung(ex); }
        }
    }
}
