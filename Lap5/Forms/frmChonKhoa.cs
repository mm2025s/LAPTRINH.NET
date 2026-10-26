using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Lab05_MultiForm.Helpers;
using Lab05_MultiForm.Models;

namespace Lab05_MultiForm.Forms
{
    /// <summary>
    /// Bài 8: Form chọn khoa.
    /// - Danh sách khoa được TRUYỀN VÀO qua constructor (không đọc biến toàn cục,
    ///   không tự gọi DuLieuMau trong Form này).
    /// - Khoa được chọn trả về qua thuộc tính KhoaDuocChon + DialogResult.OK.
    /// </summary>
    public partial class frmChonKhoa : FormBase
    {
        private readonly List<Khoa> _danhSach;

        /// <summary>Khoa người dùng đã chọn (null nếu Hủy).</summary>
        public Khoa KhoaDuocChon { get; private set; }

        public frmChonKhoa(List<Khoa> danhSachKhoa)
        {
            InitializeComponent();

            _danhSach = danhSachKhoa ?? new List<Khoa>();
        }

        private void frmChonKhoa_Load(object sender, EventArgs e)
        {
            lstKhoa.Items.Clear();
            foreach (Khoa k in _danhSach)
            {
                var item = new ListViewItem(new[] { k.MaKhoa, k.TenKhoa });
                item.Tag = k;
                lstKhoa.Items.Add(item);
            }

            if (lstKhoa.Items.Count > 0) lstKhoa.Items[0].Selected = true;
            lblDem.Text = _danhSach.Count + " khoa được truyền vào qua constructor.";
            lblChon.Text = "Chưa chọn khoa";
        }

        private void lstKhoa_SelectedIndexChanged(object sender, EventArgs e)
        {
            Khoa k = KhoaDangChon();
            lblChon.Text = k == null ? "Chưa chọn khoa"
                : string.Format("Đang chọn: {0} ({1})", k.TenKhoa, k.MaKhoa);
            lblChon.ForeColor = k == null ? UiTheme.Muted : UiTheme.Indigo;
            btnChon.Text = k == null ? "Chọn" : "Chọn " + k.MaKhoa;
        }

        private void lstKhoa_DoubleClick(object sender, EventArgs e)
        {
            btnChon_Click(sender, e);
        }

        private Khoa KhoaDangChon()
        {
            if (lstKhoa.SelectedItems.Count == 0) return null;
            return lstKhoa.SelectedItems[0].Tag as Khoa;
        }

        private void btnChon_Click(object sender, EventArgs e)
        {
            Khoa k = KhoaDangChon();
            if (k == null)
            {
                MessageBox.Show("Vui lòng chọn một khoa trong danh sách.", "Chưa chọn khoa",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            KhoaDuocChon = k;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            KhoaDuocChon = null;
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
