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
    /// BAI 10 - Hop nhat du lieu bang DataTable.Merge.
    ///
    /// Merge gop hai bang cung cau truc, so sanh theo KHOA CHINH:
    ///   - mã chưa có  → thêm dòng mới
    ///   - mã đã có    → cập nhật theo tham số preserveChanges
    ///
    /// preserveChanges:
    ///   false  → ưu tiên bảng ĐẾN (dữ liệu mới thắng)
    ///   true   → ưu tiên bảng ĐI  (bảng hiện có thắng)
    ///
    /// Nếu bảng không có PrimaryKey, Merge chỉ cộng dồn dòng và có thể sinh
    /// dòng TRÙNG KHÓA. Đó là lỗi hay gặp nhất khi dùng Merge.
    /// </summary>
    public class frmBai10 : BaiFormBase
    {
        private readonly SinhVienBUS _bus = new SinhVienBUS();

        private LuoiSinhVien _luoiGoc;
        private LuoiSinhVien _luoiBoSung;
        private LuoiSinhVien _luoiKetQua;
        private Label _lblThongKe;
        private Label _lblCheDo;

        private DataTable _goc;
        private DataTable _boSung;

        public frmBai10()
            : base("10", "Hợp nhất dữ liệu bằng Merge",
                   "Hai DataTable từ hai nguồn → Merge theo PrimaryKey → thêm mới & cập nhật",
                   UiTheme.Indigo, "DataTable.Merge", "xử lý trùng khóa", "frmBai10",
                   new Size(1180, 780))
        {
            Badge = new BadgeLabel();
            Badge.Value = "2 nguồn → 1 bảng";
            Badge.BaseColor = UiTheme.Indigo;
            Badge.SetBounds(NoiDung.Right - 175, 20, 155, 24);

            TaoThanhCongCu();
            TaoBaVung();
            TaoThongKe();
        }

        private void TaoThanhCongCu()
        {
            CardPanel the = The(18, 18, 1144, 76);
            NoiDung.Controls.Add(the);

            the.Controls.Add(Nut("btnNap", "Nạp 2 nguồn", UiTheme.Indigo, 18, 20, 140));
            the.Controls.Add(Nut("btnMerge", "Merge (bản mới thắng)", UiTheme.Green, 170, 20, 210));
            the.Controls.Add(Nut("btnMergeGiuCu", "Merge (giữ bản cũ)", UiTheme.Teal, 392, 20, 190, 36, true));
            the.Controls.Add(Nut("btnThuKhongKhoa", "Thử Merge không PrimaryKey", UiTheme.Red, 594, 20, 230, 36, true));

            _lblCheDo = UiFactory.Label("—", 8.75f, UiTheme.Muted, false);
            _lblCheDo.SetBounds(836, 20, 290, 40);
            the.Controls.Add(_lblCheDo);

            Tim("btnNap").Click += (s, e) => Nap();
            Tim("btnMerge").Click += (s, e) => Merge(false);
            Tim("btnMergeGiuCu").Click += (s, e) => Merge(true);
            Tim("btnThuKhongKhoa").Click += (s, e) => ThuKhongKhoa();
        }

        private void TaoBaVung()
        {
            int w = 370;

            Label n1 = TieuDeMuc("NGUỒN 1 — CSDL (đang có)", UiTheme.Indigo);
            n1.SetBounds(18, 102, 360, 22);
            NoiDung.Controls.Add(n1);

            CardPanel t1 = The(18, 126, w, 330);
            t1.Padding = new Padding(1);
            NoiDung.Controls.Add(t1);

            _luoiGoc = new LuoiSinhVien();
            _luoiGoc.Dock = DockStyle.Fill;
            _luoiGoc.BatToMau = false;
            t1.Controls.Add(_luoiGoc);

            Label n2 = TieuDeMuc("NGUỒN 2 — BẢNG BỔ SUNG", UiTheme.Indigo);
            n2.SetBounds(400, 102, 360, 22);
            NoiDung.Controls.Add(n2);

            CardPanel t2 = The(400, 126, w, 330);
            t2.Padding = new Padding(1);
            NoiDung.Controls.Add(t2);

            _luoiBoSung = new LuoiSinhVien();
            _luoiBoSung.Dock = DockStyle.Fill;
            _luoiBoSung.BatToMau = false;
            t2.Controls.Add(_luoiBoSung);

            Label n3 = TieuDeMuc("KẾT QUẢ SAU MERGE", UiTheme.Green);
            n3.SetBounds(782, 102, 380, 22);
            NoiDung.Controls.Add(n3);

            CardPanel t3 = The(782, 126, w, 330);
            t3.Padding = new Padding(1);
            NoiDung.Controls.Add(t3);

            _luoiKetQua = new LuoiSinhVien();
            _luoiKetQua.Dock = DockStyle.Fill;
            _luoiKetQua.BatToMau = false;
            t3.Controls.Add(_luoiKetQua);
        }

        private void TaoThongKe()
        {
            CardPanel the = The(18, 468, 1144, 250);
            NoiDung.Controls.Add(the);

            Label t = TieuDeMuc("DIỄN GIẢI", UiTheme.Indigo);
            t.SetBounds(18, 12, 300, 22);
            the.Controls.Add(t);

            _lblThongKe = UiFactory.Label("—", 9.25f, UiTheme.InkSoft, false);
            _lblThongKe.SetBounds(18, 40, 1108, 196);
            the.Controls.Add(_lblThongKe);
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

                _goc = _bus.LayBangRutGon();                          // 3 cột, có PrimaryKey
                _boSung = SinhVienBUS.TaoBangNguonThuHai();           // 4 dòng: 2 trùng mã, 2 mới

                _luoiGoc.GanDuLieu(_goc);
                _luoiBoSung.GanDuLieu(_boSung);
                _luoiKetQua.GanDuLieu(null);

                _lblThongKe.Text =
                    "Nguồn 1 (CSDL): " + _goc.Rows.Count + " dòng, có PrimaryKey = " +
                    (_goc.PrimaryKey.Length > 0 ? _goc.PrimaryKey[0].ColumnName : "không") + ".\n" +
                    "Nguồn 2 (bổ sung): " + _boSung.Rows.Count + " dòng — gồm SV001, SV002 (đã có, điểm khác) " +
                    "và SV900, SV901 (mới hoàn toàn).\n" +
                    "Bấm một trong hai nút Merge để xem cách xử lý trùng khóa.";

                KetQua("Đã nạp 2 nguồn: " + _goc.Rows.Count + " và " + _boSung.Rows.Count + " dòng.", true);
            }
            catch (LoiNghiepVu ex) { HienThiLoiNghiepVu(ex); }
            catch (Exception ex) { LoiChung(ex); }
            finally { Cursor = Cursors.Default; }
        }

        private void Merge(bool giuBanCu)
        {
            try
            {
                if (_goc == null) { GhiChu("Hãy nạp 2 nguồn trước."); return; }

                DataTable ketQua = SinhVienBUS.GopHaiBang(_goc, _boSung, giuBanCu);

                _luoiKetQua.GanDuLieu(ketQua);
                _lblCheDo.Text = giuBanCu
                    ? "preserveChanges = TRUE → bảng hiện có thắng"
                    : "preserveChanges = FALSE → bảng đến thắng";

                // Diễn giải: mã nào mới, mã nào bị đổi điểm
                var cu = new System.Collections.Generic.Dictionary<string, decimal>();
                foreach (DataRow r in _goc.Rows)
                    cu[Convert.ToString(r["MaSV"])] = Convert.ToDecimal(r["DiemTB"]);

                var moi = new System.Collections.Generic.List<string>();
                var doiDiem = new System.Collections.Generic.List<string>();
                var giuNguyen = new System.Collections.Generic.List<string>();

                foreach (DataRow r in _boSung.Rows)
                {
                    string ma = Convert.ToString(r["MaSV"]);
                    decimal diemMoi = Convert.ToDecimal(r["DiemTB"]);

                    if (!cu.ContainsKey(ma)) { moi.Add(ma); continue; }

                    if (cu[ma] == diemMoi) giuNguyen.Add(ma);
                    else if (giuBanCu) giuNguyen.Add(ma + " (giữ " + cu[ma].ToString("0.00") + ")");
                    else doiDiem.Add(ma + " (" + cu[ma].ToString("0.00") + " → " + diemMoi.ToString("0.00") + ")");
                }

                var sb = new System.Text.StringBuilder();
                sb.AppendLine("Nguồn 1: " + _goc.Rows.Count + " dòng · Nguồn 2: " + _boSung.Rows.Count +
                              " dòng · Kết quả: " + ketQua.Rows.Count + " dòng");
                sb.AppendLine();
                sb.AppendLine("• Thêm mới (" + moi.Count + "): " + (moi.Count == 0 ? "—" : string.Join(", ", moi.ToArray())));
                sb.AppendLine("• Cập nhật điểm (" + doiDiem.Count + "): " + (doiDiem.Count == 0 ? "—" : string.Join(", ", doiDiem.ToArray())));
                sb.AppendLine("• Giữ nguyên (" + giuNguyen.Count + "): " + (giuNguyen.Count == 0 ? "—" : string.Join(", ", giuNguyen.ToArray())));
                sb.AppendLine();
                sb.AppendLine("Merge so sánh theo PrimaryKey (MaSV), không theo số dòng, nên kết quả là " +
                    (_goc.Rows.Count + moi.Count) + " dòng chứ không phải " + (_goc.Rows.Count + _boSung.Rows.Count) + ".");

                _lblThongKe.Text = sb.ToString();

                KetQua("Merge xong: " + ketQua.Rows.Count + " dòng (thêm " + moi.Count +
                    ", cập nhật " + doiDiem.Count + ").", true);
            }
            catch (LoiNghiepVu ex) { HienThiLoiNghiepVu(ex); }
            catch (Exception ex) { LoiChung(ex); }
        }

        /// <summary>
        /// Co tinh bo PrimaryKey roi Merge -> sinh dong trung khoa.
        /// Day la loi hay gap: Merge khong biet dong nao la "cung mot ban ghi"
        /// nen cong don tat ca.
        /// </summary>
        private void ThuKhongKhoa()
        {
            try
            {
                if (_goc == null) { GhiChu("Hãy nạp 2 nguồn trước."); return; }

                DataTable khongKhoa = _goc.Copy();
                khongKhoa.PrimaryKey = null;

                DataTable boSungKhongKhoa = _boSung.Copy();
                boSungKhongKhoa.PrimaryKey = null;

                khongKhoa.Merge(boSungKhongKhoa);

                int soMa = khongKhoa.Rows.Count;
                var dem = new System.Collections.Generic.Dictionary<string, int>();
                foreach (DataRow r in khongKhoa.Rows)
                {
                    string ma = Convert.ToString(r["MaSV"]);
                    dem[ma] = dem.ContainsKey(ma) ? dem[ma] + 1 : 1;
                }

                var trung = new System.Collections.Generic.List<string>();
                foreach (var kv in dem) if (kv.Value > 1) trung.Add(kv.Key + " ×" + kv.Value);

                _luoiKetQua.GanDuLieu(khongKhoa);
                _lblCheDo.Text = "KHÔNG có PrimaryKey → sinh dòng trùng";

                _lblThongKe.Text = string.Format(
                    "Không đặt PrimaryKey: {0} dòng (đúng ra chỉ nên {1}).\n" +
                    "Mã bị trùng: {2}\n\n" +
                    "Kết luận: Merge so sánh theo PrimaryKey. Thiếu khóa chính thì nó coi mọi dòng là khác nhau " +
                    "và cộng dồn — bảng kết quả có SV001 hai lần với hai điểm khác nhau.",
                    soMa, _goc.Rows.Count + 2, trung.Count == 0 ? "—" : string.Join(", ", trung.ToArray()));

                KetQua("Đây là lỗi cần tránh: " + soMa + " dòng thay vì " + (_goc.Rows.Count + 2) + ".", false);
            }
            catch (Exception ex) { LoiChung(ex); }
        }
    }
}
