using System;

namespace Lap3
{
    /// <summary>
    /// Hợp đồng (contract) kiểm tra tính hợp lệ của dữ liệu đối tượng.
    /// Lớp nào thực thi interface này phải tự cài đặt nội dung của IsValid().
    /// </summary>
    public interface IValidatable
    {
        bool IsValid(out string message);
    }

    /// <summary>
    /// Hợp đồng xuất nội dung hiển thị của đối tượng ra màn hình Console.
    /// </summary>
    public interface IPrintable
    {
        string ToDisplayText();
    }

    /// <summary>
    /// Hợp đồng xuất dữ liệu đối tượng ra các định dạng trao đổi (CSV, JSON).
    /// Được tái sử dụng ở tầng nghiệp vụ (BUS) trong các bài lab sau.
    /// </summary>
    public interface IXuatDuLieu
    {
        string XuatCsv();

        string XuatJson();
    }
}
