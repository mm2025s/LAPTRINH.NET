using System;

namespace Lab05_MultiForm.Forms
{
    /// <summary>
    /// Cờ dùng cho chế độ chụp ảnh báo cáo (Lab05_MultiForm.exe --screenshots).
    ///
    /// Tiến trình CHA bật cờ này để các Form tự đóng ngay sau khi hiển thị,
    /// tránh treo message loop. Tiến trình CON (chụp một kịch bản) để cờ = false
    /// vì kịch bản tự quyết định thời điểm đóng Form.
    /// </summary>
    public static class CheDoChupAnh
    {
        /// <summary>True khi đang chụp ảnh tự động: Form tự đóng sau khi Shown.</summary>
        public static bool DangChup;
    }
}
