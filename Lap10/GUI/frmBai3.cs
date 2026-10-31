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
    /// BAI 3 - Thiet lap DataRelation Khoa - SinhVien.
    ///
    /// DataRelation la QUAN HE CHA-CON trong bo nho, tuong duong khoa ngoai
    /// trong CSDL. Sau khi tao:
    ///   - khoa.GetChildRows("FK_Khoa_SinhVien") lay ngay cac sinh vien cua khoa
    ///     do ma khong can cau SQL nao;
    ///   - co the bat ForeignKeyConstraint: khong cho xoa khoa con sinh vien,
    ///     giong FK that.
    ///
    /// Diem can canh bao: ds.Relations.Add() se nem InvalidConstraintException
    /// neu co MaKhoa "mo coi" (co trong SinhVien nhung khong co trong Khoa).
    /// Vi vay Form kiem tra toan ven truoc va bao ro gia tri sai.
    /// </summary>
    public class frmBai3 : BaiFormBase
    {
        private readonly SinhVienBUS _bus = new SinhVienBUS();

        private LuoiSinhVien _luoiKhoa;
        private LuoiSinhVien _luoiCon;
        private ComboBox _cboKhoa;
        private Label _lblQuanHe;
        private Label _lblDoiChieu;
        private TextBox _txtCachDung;

        private DataSet _ds;

        public frmBai3()
            : base("3", "Thiết lập DataRelation Khoa – SinhVien",
                   "Mối quan hệ cha–con trong bộ nhớ · GetChildRows() thay cho câu SQL JOIN",
                   UiTheme.Teal, "DataRelation", "GetChildRows()", "frmBai3",
                   new Size(1150, 740))
        {
            Badge = new BadgeLabel();
            Badge.Value = "FK trong bộ nhớ";
            Badge.BaseColor = UiTheme.Teal;
            Badge.SetBounds(NoiDung.Right - 175, 20, 155, 24);

            TaoThanhCongCu();
            TaoBaVung();
        }

        private void TaoThanhCongCu()
        {
            CardPanel the = The(18, 18, 1114, 100);
            NoiDung.Controls.Add(the);

            the.Controls.Add(Nut("btnNap", "Nạp DataSet", UiTheme.Teal, 18, 34, 140));
            the.Controls.Add(Nut("btnTaoQuanHe", "Tạo DataRelation", UiTheme.Green, 170, 34, 160));
            the.Controls.Add(Nut("btnThuXoa", "Thử xóa khoa còn con", UiTheme.Red, 342, 34, 190, 36, true));

            Label nhan = NhanTruong("Chọn khoa");
            nhan.SetBounds(556, 18, 200, 18);
            the.Controls.Add(nhan);

            _cboKhoa = UiFactory.ComboBox("cboKhoa");
            _cboKhoa.SetBounds(556, 38, 240, 28);
            _cboKhoa.SelectedIndexChanged += (s, e) => HienThiCon();
            the.Controls.Add(_cboKhoa);

            _lblQuanHe = UiFactory.Label("Chưa tạo quan hệ.", 9f, UiTheme.Muted, false);
            _lblQuanHe.SetBounds(810, 34, 288, 50);
            the.Controls.Add(_lblQuanHe);

            Tim("btnNap").Click += (s, e) => Nap();
            Tim("btnTaoQuanHe").Click += (s, e) => TaoQuanHe(false);
            Tim("btnThuXoa").Click += (s, e) => ThuXoa();
        }

        private void TaoBaVung()
        {
            // Khoa
            Label n1 = TieuDeMuc("BẢNG CHA — KHOA", UiTheme.Teal);
            n1.SetBounds(18, 128, 300, 22);
            NoiDung.Controls.Add(n1);

            CardPanel theKhoa = The(18, 152, 360, 250);
            theKhoa.Padding = new Padding(1);
            NoiDung.Controls.Add(theKhoa);

            _luoiKhoa = new LuoiSinhVien();
            _luoiKhoa.Dock = DockStyle.Fill;
            _luoiKhoa.BatToMau = false;
            _luoiKhoa.DongDuocChon += (s, row) =>
            {
                if (_cboKhoa.Items.Count == 0) return;
                string ma = LuoiSinhVien.Chuoi(row, "MaKhoa");
                for (int i = 0; i < _cboKhoa.Items.Count; i++)
                {
                    if (Convert.ToString(((DataRowView)_cboKhoa.Items[i])["MaKhoa"]) == ma)
                    { _cboKhoa.SelectedIndex = i; return; }
                }
            };
            theKhoa.Controls.Add(_luoiKhoa);

            // Con
            Label n2 = TieuDeMuc("BẢNG CON — GetChildRows(\"FK_Khoa_SinhVien\")", UiTheme.Teal);
            n2.SetBounds(394, 128, 500, 22);
            NoiDung.Controls.Add(n2);

            CardPanel theCon = The(394, 152, 738, 250);
            theCon.Padding = new Padding(1);
            NoiDung.Controls.Add(theCon);

            _luoiCon = new LuoiSinhVien();
            _luoiCon.Dock = DockStyle.Fill;
            theCon.Controls.Add(_luoiCon);

            // Giai thich
            CardPanel theGiai = The(18, 416, 1114, 264);
            NoiDung.Controls.Add(theGiai);

            Label t = TieuDeMuc("CÁCH DÙNG & ĐỐI CHIẾU", UiTheme.Teal);
            t.SetBounds(18, 14, 400, 22);
            theGiai.Controls.Add(t);

            _lblDoiChieu = UiFactory.Label(
                "Tạo quan hệ rồi chọn một khoa để xem các dòng con.",
                9.25f, UiTheme.InkSoft, false);
            _lblDoiChieu.SetBounds(18, 44, 1078, 40);
            theGiai.Controls.Add(_lblDoiChieu);

            _txtCachDung = new TextBox();
            _txtCachDung.Multiline = true;
            _txtCachDung.ReadOnly = true;
            _txtCachDung.BorderStyle = BorderStyle.None;
            _txtCachDung.Font = UiTheme.FontMono(8.5f);
            _txtCachDung.BackColor = Color.FromArgb(30, 34, 48);
            _txtCachDung.ForeColor = Color.FromArgb(190, 232, 200);
            _txtCachDung.SetBounds(18, 92, 1078, 156);
            _txtCachDung.Text =
                "ds.Relations.Add(new DataRelation(\r\n" +
                "        \"FK_Khoa_SinhVien\",\r\n" +
                "        ds.Tables[\"Khoa\"].Columns[\"MaKhoa\"],\r\n" +
                "        ds.Tables[\"SinhVien\"].Columns[\"MaKhoa\"], true));\r\n" +
                "\r\n" +
                "DataRow khoa = ...;                            // dong cha\r\n" +
                "DataRow[] con = khoa.GetChildRows(\"FK_Khoa_SinhVien\");\r\n" +
                "\r\n" +
                "// Ngược lại: từ dòng con lấy cha\r\n" +
                "DataRow cha = svRow.GetParentRow(\"FK_Khoa_SinhVien\");";
            theGiai.Controls.Add(_txtCachDung);
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

                _cboKhoa.DataSource = _ds.Tables["Khoa"];
                _cboKhoa.DisplayMember = "TenKhoa";
                _cboKhoa.ValueMember = "MaKhoa";

                string moCoi = DAL.SinhVienDAL.TimKhoaMoCoi(_ds);
                _lblQuanHe.Text = moCoi == null
                    ? "Dữ liệu toàn vẹn: mọi MaKhoa trong SinhVien đều có trong Khoa."
                    : "CẢNH BÁO: MaKhoa mồ côi = " + moCoi;
                _lblQuanHe.ForeColor = moCoi == null ? UiTheme.Green : UiTheme.Red;

                KetQua("Đã nạp 2 bảng. Bấm \"Tạo DataRelation\" để nối chúng trong bộ nhớ.", true);
            }
            catch (LoiNghiepVu ex) { HienThiLoiNghiepVu(ex); }
            catch (Exception ex) { LoiChung(ex); }
            finally { Cursor = Cursors.Default; }
        }

        private void TaoQuanHe(bool imLang)
        {
            try
            {
                if (_ds == null) { Nap(); return; }

                DataRelation qh = _bus.TaoQuanHe(_ds);

                _lblQuanHe.Text = "Đã tạo \"" + qh.RelationName + "\" · bảng cha: " +
                    qh.ParentTable.TableName + " · bảng con: " + qh.ChildTable.TableName +
                    " · constraint: " + (qh.ParentTable.Constraints.Count > 0 ? "có" : "không");
                _lblQuanHe.ForeColor = UiTheme.Green;

                if (!imLang)
                    KetQua("Đã tạo DataRelation " + qh.RelationName + " (kèm ForeignKeyConstraint).", true);

                HienThiCon();
            }
            catch (LoiNghiepVu ex) { HienThiLoiNghiepVu(ex); }
            catch (Exception ex) { LoiChung(ex); }
        }

        private void HienThiCon()
        {
            try
            {
                if (_ds == null || _cboKhoa.SelectedValue == null) return;

                if (_ds.Relations.Count == 0)
                {
                    _luoiCon.GanDuLieu(null);
                    GhiChu("Chưa có DataRelation — bấm \"Tạo DataRelation\" trước.");
                    return;
                }

                string maKhoa = Convert.ToString(_cboKhoa.SelectedValue);
                int soSanh;
                DataTable con = _bus.LaySinhVienCuaKhoa(_ds, maKhoa, out soSanh);

                _luoiCon.GanDuLieu(con);

                bool khop = con.Rows.Count == soSanh;
                _lblDoiChieu.Text = string.Format(
                    "Khoa {0}: GetChildRows() → {1} sinh viên · lọc thủ công từng dòng → {2} sinh viên · {3}",
                    maKhoa, con.Rows.Count, soSanh,
                    khop ? "KHỚP (quan hệ dùng đúng cột MaKhoa)" : "LỆCH — kiểm tra lại khóa");

                KetQua("Khoa " + maKhoa + " có " + con.Rows.Count +
                    " sinh viên (lấy bằng quan hệ, không chạy SQL).", khop);
            }
            catch (LoiNghiepVu ex) { HienThiLoiNghiepVu(ex); }
            catch (Exception ex) { LoiChung(ex); }
        }

        /// <summary>
        /// Thu xoa mot dong Khoa con sinh vien -> ForeignKeyConstraint chan lai.
        /// Day la bang chung DataRelation mang theo rang buoc chu khong chi de loc.
        /// </summary>
        private void ThuXoa()
        {
            try
            {
                if (_ds == null || _ds.Relations.Count == 0)
                {
                    GhiChu("Hãy tạo DataRelation trước khi thử xóa.");
                    return;
                }

                if (_cboKhoa.SelectedValue == null) return;
                string maKhoa = Convert.ToString(_cboKhoa.SelectedValue);

                DataRow cha = null;
                foreach (DataRow r in _ds.Tables["Khoa"].Rows)
                    if (Convert.ToString(r["MaKhoa"]) == maKhoa) { cha = r; break; }

                if (cha == null) return;

                try
                {
                    cha.Delete();
                    _ds.Tables["Khoa"].RejectChanges();   // khong de lai dau vet
                    KetQua("Xóa được — dữ liệu bất thường: khoa này không còn sinh viên nào.", false);
                }
                catch (InvalidConstraintException ex)
                {
                    _ds.Tables["Khoa"].RejectChanges();
                    KetQua("ForeignKeyConstraint đã chặn: " + ex.Message.Split('\n')[0], true);
                }
            }
            catch (Exception ex) { LoiChung(ex); }
        }
    }
}
