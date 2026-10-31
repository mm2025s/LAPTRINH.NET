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
    /// BAI 8 - AcceptChanges va RejectChanges.
    ///
    /// Hai lenh nay CHI tac dong trong bo nho, KHONG lien quan gi toi CSDL:
    ///
    ///   AcceptChanges()  chot: moi dong ve Unchanged, RowVersion goc duoc cap
    ///                    nhat = gia tri hien tai.
    ///                    HAU QUA: GetChanges() tra ve null -> adapter.Update()
    ///                    khong gui gi nua. Bam "Chấp nhận" TRƯỚC khi "Lưu"
    ///                    = thay doi bi mat (CSDL khong doi, khong bao loi).
    ///
    ///   RejectChanges()  huy: dong sua quay ve gia tri goc, dong Added bi loai
    ///                    bo, dong Deleted duoc khoi phuc.
    ///                    HAU QUA: cai gi da Update xuong CSDL roi thi KHONG
    ///                    lay lai duoc bang lenh nay.
    ///
    /// Form co bang doi chieu 3 buoc de nguoi hoc thay ro su khac nhau.
    /// </summary>
    public class frmBai8 : BaiFormBase
    {
        private readonly SinhVienBUS _bus = new SinhVienBUS();

        private LuoiSinhVien _luoi;
        private Label _lblTrangThai;
        private Label _lblGiaiThich;
        private Label _lblLichSu;
        private DataGridView _luoiDoiChieu;

        private DataTable _table;
        private readonly System.Text.StringBuilder _lichSu = new System.Text.StringBuilder();

        public frmBai8()
            : base("8", "AcceptChanges và RejectChanges",
                   "Chốt hay hủy thay đổi TRONG BỘ NHỚ — không liên quan tới CSDL",
                   UiTheme.Rose, "Accept / Reject", "chỉ trong bộ nhớ", "frmBai8",
                   new Size(1150, 760))
        {
            Badge = new BadgeLabel();
            Badge.Value = "Trong bộ nhớ";
            Badge.BaseColor = UiTheme.Rose;
            Badge.SetBounds(NoiDung.Right - 150, 20, 130, 24);

            TaoThanhCongCu();
            TaoLuoi();
            TaoDoiChieu();
        }

        private void TaoThanhCongCu()
        {
            CardPanel the = The(18, 18, 1114, 78);
            NoiDung.Controls.Add(the);

            the.Controls.Add(Nut("btnNap", "Nạp bảng", UiTheme.Rose, 18, 22, 120));
            the.Controls.Add(Nut("btnChapNhan", "Chấp nhận (AcceptChanges)", UiTheme.Green, 150, 22, 230));
            the.Controls.Add(Nut("btnHoanTac", "Hoàn tác (RejectChanges)", UiTheme.Red, 392, 22, 220, 36, true));

            _lblTrangThai = UiFactory.Label("—", 10f, UiTheme.InkSoft, true);
            _lblTrangThai.SetBounds(632, 24, 466, 26);
            the.Controls.Add(_lblTrangThai);

            Tim("btnNap").Click += (s, e) => Nap();
            Tim("btnChapNhan").Click += (s, e) => ChapNhan();
            Tim("btnHoanTac").Click += (s, e) => HoanTac();
        }

        private void TaoLuoi()
        {
            CardPanel the = The(18, 108, 700, 540);
            the.Padding = new Padding(1);
            NoiDung.Controls.Add(the);

            _luoi = new LuoiSinhVien();
            _luoi.Dock = DockStyle.Fill;
            _luoi.ChoSua = true;
            _luoi.BatToMau = false;
            _luoi.CellValueChanged += (s, e) => CapNhat();
            _luoi.RowsRemoved += (s, e) => CapNhat();
            the.Controls.Add(_luoi);
        }

        private void TaoDoiChieu()
        {
            CardPanel thePhai = The(730, 108, 402, 540);
            NoiDung.Controls.Add(thePhai);

            Label t = TieuDeMuc("ẢNH HƯỞNG TỚI UPDATE", UiTheme.Rose);
            t.SetBounds(18, 12, 360, 22);
            thePhai.Controls.Add(t);

            _luoiDoiChieu = new DataGridView();
            _luoiDoiChieu.Name = "luoiDoiChieu";
            _luoiDoiChieu.ReadOnly = true;
            _luoiDoiChieu.AllowUserToAddRows = false;
            _luoiDoiChieu.AllowUserToDeleteRows = false;
            _luoiDoiChieu.RowHeadersVisible = false;
            _luoiDoiChieu.BorderStyle = BorderStyle.None;
            _luoiDoiChieu.BackgroundColor = Color.White;
            _luoiDoiChieu.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _luoiDoiChieu.ColumnHeadersHeight = 30;
            _luoiDoiChieu.RowTemplate.Height = 26;
            _luoiDoiChieu.Font = UiTheme.FontBody(8.75f);
            _luoiDoiChieu.EnableHeadersVisualStyles = false;
            _luoiDoiChieu.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(246, 247, 251);
            _luoiDoiChieu.ColumnHeadersDefaultCellStyle.Font = UiTheme.FontBold(8.25f);
            _luoiDoiChieu.ColumnHeadersDefaultCellStyle.ForeColor = UiTheme.InkSoft;
            _luoiDoiChieu.GridColor = Color.FromArgb(236, 239, 245);
            _luoiDoiChieu.SetBounds(18, 42, 366, 120);
            thePhai.Controls.Add(_luoiDoiChieu);

            _lblGiaiThich = UiFactory.Label("", 8.5f, UiTheme.InkSoft, false);
            _lblGiaiThich.SetBounds(18, 172, 366, 130);
            thePhai.Controls.Add(_lblGiaiThich);

            Label t2 = TieuDeMuc("NHẬT KÝ THAO TÁC", UiTheme.Rose);
            t2.SetBounds(18, 308, 300, 22);
            thePhai.Controls.Add(t2);

            _lblLichSu = UiFactory.Label("(chưa có)", 8.25f, UiTheme.Muted, false);
            _lblLichSu.SetBounds(18, 334, 366, 190);
            thePhai.Controls.Add(_lblLichSu);
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

                _table = _bus.GetStudentTable();
                _luoi.GanDuLieu(_table);
                _lichSu.Length = 0;
                _lichSu.AppendLine("• Nạp " + _table.Rows.Count + " dòng → tất cả Unchanged");

                CapNhat();
                KetQua("Đã nạp. Hãy sửa vài ô rồi thử Chấp nhận / Hoàn tác để thấy khác biệt.", true);
            }
            catch (LoiNghiepVu ex) { HienThiLoiNghiepVu(ex); }
            catch (Exception ex) { LoiChung(ex); }
            finally { Cursor = Cursors.Default; }
        }

        private void ChapNhan()
        {
            try
            {
                if (_table == null) { GhiChu("Hãy nạp bảng trước."); return; }

                _luoi.EndEdit();

                ThongKeRowState truoc = _bus.DemTheoRowState(_table);
                if (truoc.TongThayDoi == 0)
                {
                    GhiChu("Không có thay đổi nào để chấp nhận (mọi dòng đang Unchanged).");
                    return;
                }

                _bus.ChapNhanThayDoi(_table);

                _lichSu.AppendLine(string.Format("• AcceptChanges: chốt {0} thay đổi → Unchanged",
                    truoc.TongThayDoi));

                CapNhat();
                KetQua("Đã CHỐT " + truoc.TongThayDoi +
                    " thay đổi. CẢNH BÁO: GetChanges() giờ = null nên bấm Lưu sẽ KHÔNG gửi gì xuống CSDL.", true);
            }
            catch (LoiNghiepVu ex) { HienThiLoiNghiepVu(ex); }
            catch (Exception ex) { LoiChung(ex); }
        }

        private void HoanTac()
        {
            try
            {
                if (_table == null) { GhiChu("Hãy nạp bảng trước."); return; }

                ThongKeRowState truoc = _bus.DemTheoRowState(_table);
                if (truoc.TongThayDoi == 0)
                {
                    GhiChu("Không có thay đổi nào để hoàn tác.");
                    return;
                }

                _bus.HoanTacThayDoi(_table);
                _luoi.Refresh();

                _lichSu.AppendLine(string.Format("• RejectChanges: hủy {0} thay đổi chưa chốt",
                    truoc.TongThayDoi));

                CapNhat();
                KetQua("Đã HỦY " + truoc.TongThayDoi +
                    " thay đổi trong bộ nhớ: dòng sửa về giá trị gốc, dòng Added bị bỏ, dòng Deleted được phục hồi.", true);
            }
            catch (LoiNghiepVu ex) { HienThiLoiNghiepVu(ex); }
            catch (Exception ex) { LoiChung(ex); }
        }

        private void CapNhat()
        {
            if (_table == null) return;

            ThongKeRowState tk = _bus.DemTheoRowState(_table);
            DataTable thayDoi = _bus.LayThayDoi(_table);

            _lblTrangThai.Text = tk.ToString();
            _lblTrangThai.ForeColor = tk.TongThayDoi > 0 ? UiTheme.Orange : UiTheme.Green;

            var bang = new DataTable("DoiChieu");
            bang.Columns.Add("Tình huống", typeof(string));
            bang.Columns.Add("GetChanges()", typeof(string));
            bang.Columns.Add("Update() gửi", typeof(string));

            bang.Rows.Add("Vừa sửa, CHƯA chốt", tk.TongThayDoi + " dòng", tk.TongThayDoi + " lệnh");
            bang.Rows.Add("Sau AcceptChanges", "null", "0 lệnh  ← mất thay đổi");
            bang.Rows.Add("Sau RejectChanges", "null", "0 lệnh  ← quay về bản gốc");

            _luoiDoiChieu.DataSource = bang;

            _lblGiaiThich.Text =
                "AcceptChanges không ghi gì xuống CSDL — nó chỉ đánh dấu \"đã xử lý\" trong bộ nhớ.\n\n" +
                "Vì Update() chỉ gửi các dòng CÓ trong GetChanges(), thứ tự đúng luôn là:\n" +
                "    1) Update(table)   → ghi CSDL\n" +
                "    2) AcceptChanges() → chốt trạng thái\n\n" +
                "Đảo ngược thứ tự = mất dữ liệu mà không có lỗi nào báo.";

            _lblLichSu.Text = _lichSu.Length == 0 ? "(chưa có)" : _lichSu.ToString();
        }
    }
}
