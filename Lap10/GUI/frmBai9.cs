using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Lab10_DisconnectedData.BUS;
using Lab10_DisconnectedData.DTO;
using Lab10_DisconnectedData.GUI.Helpers;

namespace Lab10_DisconnectedData.GUI
{
    /// <summary>
    /// BAI 9 - Luu va doc DataSet bang XML.
    ///
    /// Hai che do ghi XML:
    ///   WriteXml(path)                        -> CHI du lieu
    ///   WriteXml(path, WriteSchema)           -> du lieu + XSD (schema)
    ///
    /// Chi ghi du lieu thi doc lai se mat kieu (diem thanh string), mat khoa
    /// chinh, mat quan he. Ghi kem schema thi file tu mo ta duoc: doc lai van
    /// co decimal, van giu PrimaryKey va DataRelation.
    ///
    /// Form cho thay ro: cung mot DataSet, ghi 2 file, roi doc lai file chi co
    /// du lieu de thay no mat gi.
    /// </summary>
    public class frmBai9 : BaiFormBase
    {
        private readonly SinhVienBUS _bus = new SinhVienBUS();

        private LuoiSinhVien _luoiKhoa;
        private LuoiSinhVien _luoiSinhVien;
        private Label _lblKetQuaDoc;
        private Label _lblSchema;
        private TextBox _txtXemTruoc;

        private string _tepSchema;
        private string _tepDuLieu;

        public frmBai9()
            : base("9", "Lưu và đọc DataSet bằng XML",
                   "WriteXml + WriteSchema → đọc lại đầy đủ kiểu dữ liệu, khóa chính, quan hệ, KHÔNG cần CSDL",
                   UiTheme.Slate, "XML", "không cần kết nối", "frmBai9",
                   new Size(1150, 770))
        {
            Badge = new BadgeLabel();
            Badge.Value = "Offline sau khi lưu";
            Badge.BaseColor = UiTheme.Slate;
            Badge.SetBounds(NoiDung.Right - 195, 20, 175, 24);

            string thuMuc = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Lab10_XML");
            _tepSchema = Path.Combine(thuMuc, "QLSinhVien_schema.xml");
            _tepDuLieu = Path.Combine(thuMuc, "QLSinhVien_dulieu.xml");

            TaoThanhCongCu();
            TaoHaiLuoi();
            TaoXemTruoc();
        }

        private void TaoThanhCongCu()
        {
            CardPanel the = The(18, 18, 1114, 78);
            NoiDung.Controls.Add(the);

            the.Controls.Add(Nut("btnGhiSchema", "Ghi XML + schema", UiTheme.Slate, 18, 22, 180));
            the.Controls.Add(Nut("btnGhiDuLieu", "Ghi XML chỉ dữ liệu", UiTheme.Muted, 210, 22, 190, 36, true));
            the.Controls.Add(Nut("btnDocLai", "Xóa bộ nhớ & đọc lại", UiTheme.Blue, 412, 22, 200));

            _lblSchema = UiFactory.Label("Chưa ghi tệp nào.", 8.75f, UiTheme.Muted, false);
            _lblSchema.SetBounds(632, 22, 466, 40);
            the.Controls.Add(_lblSchema);

            Tim("btnGhiSchema").Click += (s, e) => Ghi(true);
            Tim("btnGhiDuLieu").Click += (s, e) => Ghi(false);
            Tim("btnDocLai").Click += (s, e) => DocLai();
        }

        private void TaoHaiLuoi()
        {
            Label n1 = TieuDeMuc("KHOA", UiTheme.Slate);
            n1.SetBounds(18, 104, 200, 22);
            NoiDung.Controls.Add(n1);

            CardPanel the1 = The(18, 128, 340, 200);
            the1.Padding = new Padding(1);
            NoiDung.Controls.Add(the1);

            _luoiKhoa = new LuoiSinhVien();
            _luoiKhoa.Dock = DockStyle.Fill;
            _luoiKhoa.BatToMau = false;
            the1.Controls.Add(_luoiKhoa);

            Label n2 = TieuDeMuc("SINHVIEN", UiTheme.Slate);
            n2.SetBounds(374, 104, 300, 22);
            NoiDung.Controls.Add(n2);

            CardPanel the2 = The(374, 128, 758, 200);
            the2.Padding = new Padding(1);
            NoiDung.Controls.Add(the2);

            _luoiSinhVien = new LuoiSinhVien();
            _luoiSinhVien.Dock = DockStyle.Fill;
            _luoiSinhVien.BatToMau = false;
            the2.Controls.Add(_luoiSinhVien);
        }

        private void TaoXemTruoc()
        {
            CardPanel the = The(18, 344, 1114, 340);
            NoiDung.Controls.Add(the);

            Label t = TieuDeMuc("NỘI DUNG TỆP XML (100 dòng đầu)", UiTheme.Slate);
            t.SetBounds(18, 12, 400, 22);
            the.Controls.Add(t);

            _lblKetQuaDoc = UiFactory.Label("—", 9f, UiTheme.InkSoft, false);
            _lblKetQuaDoc.SetBounds(420, 12, 678, 22);
            the.Controls.Add(_lblKetQuaDoc);

            _txtXemTruoc = new TextBox();
            _txtXemTruoc.Name = "txtXemTruoc";
            _txtXemTruoc.Multiline = true;
            _txtXemTruoc.ReadOnly = true;
            _txtXemTruoc.ScrollBars = ScrollBars.Both;
            _txtXemTruoc.WordWrap = false;
            _txtXemTruoc.BorderStyle = BorderStyle.None;
            _txtXemTruoc.Font = UiTheme.FontMono(7.75f);
            _txtXemTruoc.BackColor = Color.FromArgb(30, 34, 48);
            _txtXemTruoc.ForeColor = Color.FromArgb(190, 232, 200);
            _txtXemTruoc.SetBounds(18, 40, 1078, 288);
            the.Controls.Add(_txtXemTruoc);
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

                DataSet ds = _bus.GetDataSet();
                try { _bus.TaoQuanHe(ds); } catch (LoiNghiepVu) { }

                _luoiKhoa.GanDuLieu(ds.Tables["Khoa"]);
                _luoiSinhVien.GanDuLieu(ds.Tables["SinhVien"]);

                KetQua("Đã nạp DataSet " + ds.Tables.Count + " bảng. Bấm \"Ghi XML + schema\" để lưu.", true);
            }
            catch (LoiNghiepVu ex) { HienThiLoiNghiepVu(ex); }
            catch (Exception ex) { LoiChung(ex); }
            finally { Cursor = Cursors.Default; }
        }

        private void Ghi(bool kemSchema)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                DataSet ds = _bus.GetDataSet();
                try { _bus.TaoQuanHe(ds); } catch (LoiNghiepVu) { }

                string duongDan = kemSchema ? _tepSchema : _tepDuLieu;
                Directory.CreateDirectory(Path.GetDirectoryName(duongDan));

                if (kemSchema) ds.WriteXml(duongDan, XmlWriteMode.WriteSchema);
                else ds.WriteXml(duongDan, XmlWriteMode.IgnoreSchema);

                var fi = new FileInfo(duongDan);

                _lblSchema.Text = (kemSchema ? "CÓ schema" : "CHỈ dữ liệu") + " · " +
                    fi.Name + " · " + fi.Length.ToString("N0") + " byte";

                _txtXemTruoc.Text = DocDau(duongDan, 100);

                KetQua("Đã ghi " + fi.Name + " (" + fi.Length + " byte). " +
                    (kemSchema
                        ? "Tệp này tự mô tả được: giữ kiểu decimal, PrimaryKey và DataRelation."
                        : "Tệp này CHỈ có dữ liệu — đọc lại sẽ mất kiểu và khóa chính."), true);
            }
            catch (LoiNghiepVu ex) { HienThiLoiNghiepVu(ex); }
            catch (Exception ex) { LoiChung(ex); }
            finally { Cursor = Cursors.Default; }
        }

        private void DocLai()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                if (!File.Exists(_tepSchema))
                {
                    GhiChu("Chưa có tệp có schema — bấm \"Ghi XML + schema\" trước.");
                    return;
                }

                // Xoa sach bang trong bo nho: doc lai tu con so 0, khong dung CSDL
                _luoiKhoa.GanDuLieu(null);
                _luoiSinhVien.GanDuLieu(null);

                DataSet ds = _bus.DocXml(_tepSchema);

                _luoiKhoa.GanDuLieu(ds.Tables["Khoa"]);
                _luoiSinhVien.GanDuLieu(ds.Tables["SinhVien"]);

                DataTable sv = ds.Tables["SinhVien"];
                string kieuDiem = sv.Columns.Contains("DiemTB")
                    ? sv.Columns["DiemTB"].DataType.Name
                    : "—";

                _lblKetQuaDoc.Text = string.Format(
                    "Đọc lại: {0} bảng · SinhVien {1} dòng · kiểu DiemTB = {2} · PrimaryKey = {3} · quan hệ = {4}",
                    ds.Tables.Count, sv.Rows.Count, kieuDiem,
                    sv.PrimaryKey.Length > 0 ? sv.PrimaryKey[0].ColumnName : "(không có)",
                    ds.Relations.Count);

                _txtXemTruoc.Text = DocDau(_tepSchema, 100);

                KetQua("Đọc lại thành công từ XML — không mở kết nối CSDL. Vẫn giữ " +
                    ds.Tables.Count + " bảng và " + ds.Relations.Count + " quan hệ.", true);
            }
            catch (LoiNghiepVu ex) { HienThiLoiNghiepVu(ex); }
            catch (Exception ex) { LoiChung(ex); }
            finally { Cursor = Cursors.Default; }
        }

        private static string DocDau(string duongDan, int soDong)
        {
            try
            {
                var dong = File.ReadAllLines(duongDan);
                int n = Math.Min(soDong, dong.Length);
                var sb = new System.Text.StringBuilder();
                for (int i = 0; i < n; i++) sb.AppendLine(dong[i]);
                if (dong.Length > n) sb.AppendLine("... (" + (dong.Length - n) + " dòng nữa)");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                return "Không đọc được tệp: " + ex.Message;
            }
        }
    }
}
