using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace BuildDocx
{
    /// <summary>
    /// Sinh báo cáo thực hành dạng .docx (OpenXML) cho Lab 5, chèn ảnh chụp thật trong Lap5/BaoCao.
    /// Không phụ thuộc Microsoft Office/Word: tự đóng gói các phần XML tối thiểu
    /// nhưng vẫn giữ đúng bố cục (trang bìa, mục lục, tiêu đề, bảng mã nguồn, chú thích hình).
    /// </summary>
    public static class Program
    {
        private const string A4W = "11906";
        private const string A4H = "16838";
        private const long EmuPerInch = 914400;
        private const double MaxWidthInch = 6.3;

        private static readonly List<Hinh> HinhDangDung = new List<Hinh>();
        private static int _idHinh;

        private sealed class Hinh
        {
            public string TenFile;
            public string Rid;
        }

        public static int Main(string[] args)
        {
            string root = args.Length > 0 ? args[0] : @"d:\LAPTRINH.NET";
            string lap5 = Path.Combine(root, "Lap5");
            string baoCao = Path.Combine(lap5, "BaoCao");
            string outFile = Path.Combine(baoCao, "BaoCao_Lab05_24000407.docx");
            string work = Path.Combine(root, "scratch", "docx_build");
            if (Directory.Exists(work)) Directory.Delete(work, true);
            Directory.CreateDirectory(work);

            StringBuilder body = new StringBuilder();
            TaoTrangBia(body);
            TaoMucLuc(body);
            TaoNoiDung(body, baoCao);

            // Đóng gói
            string word = Path.Combine(work, "word");
            Directory.CreateDirectory(word);
            Directory.CreateDirectory(Path.Combine(word, "_rels"));
            Directory.CreateDirectory(Path.Combine(work, "_rels"));
            Directory.CreateDirectory(Path.Combine(work, "docProps"));

            File.WriteAllText(Path.Combine(work, "[Content_Types].xml"), ContentTypes(), new UTF8Encoding(false));
            File.WriteAllText(Path.Combine(work, "_rels", ".rels"), RootRels(), new UTF8Encoding(false));
            File.WriteAllText(Path.Combine(work, "docProps", "core.xml"), CoreProps(), new UTF8Encoding(false));
            File.WriteAllText(Path.Combine(work, "docProps", "app.xml"), AppProps(), new UTF8Encoding(false));
            File.WriteAllText(Path.Combine(word, "styles.xml"), Styles(), new UTF8Encoding(false));
            File.WriteAllText(Path.Combine(word, "settings.xml"), Settings(), new UTF8Encoding(false));
            File.WriteAllText(Path.Combine(word, "header1.xml"), Header(), new UTF8Encoding(false));
            File.WriteAllText(Path.Combine(word, "footer1.xml"), Footer(), new UTF8Encoding(false));
            File.WriteAllText(Path.Combine(word, "_rels", "document.xml.rels"), RelsDocument(), new UTF8Encoding(false));

            string document = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>"
                + "<w:document xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\""
                + " xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\""
                + " xmlns:wp=\"http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing\""
                + " xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\""
                + " xmlns:pic=\"http://schemas.openxmlformats.org/drawingml/2006/picture\">"
                + "<w:body>" + body + SectPr() + "</w:body></w:document>";
            File.WriteAllText(Path.Combine(word, "document.xml"), document, new UTF8Encoding(false));

            if (File.Exists(outFile)) File.Delete(outFile);
            ZipFile.CreateFromDirectory(work, outFile, CompressionLevel.Optimal, false, Encoding.UTF8);

            Console.WriteLine("Da tao: " + outFile);
            Console.WriteLine("So hinh: " + HinhDangDung.Count);
            return 0;
        }

        // =================================================================
        // NỘI DUNG
        // =================================================================
        private static void TaoTrangBia(StringBuilder b)
        {
            b.Append(Para("center", "TRƯỜNG ĐẠI HỌC CÔNG NGHỆ ĐÔNG Á", true, 14, 40));
            b.Append(Para("center", "KHOA CÔNG NGHỆ THÔNG TIN", true, 14, 0));
            b.Append(Para("center", "•", false, 14, 0));
            b.Append(Para("center", "BÁO CÁO THỰC HÀNH", true, 16, 240));
            b.Append(Para("center", "MÔN: LẬP TRÌNH .NET (IT3212)", true, 14, 0));
            b.Append(Para("center", "BÀI THỰC HÀNH SỐ 05", true, 14, 120));
            b.Append(Para("center", "XỬ LÝ SỰ KIỆN, KIỂM TRA DỮ LIỆU VÀ ỨNG DỤNG ĐA FORM", true, 15, 320));
            b.Append(Para("center", "Sinh viên thực hiện: Nguyễn Trường Giang", false, 13, 120));
            b.Append(Para("center", "Mã số sinh viên: 24000407", false, 13, 0));
            b.Append(Para("center", "Giảng viên hướng dẫn: Khoa Công nghệ Thông tin", false, 13, 0));
            b.Append(Para("center", "Năm học 2025 - 2026", false, 13, 320));
            b.Append(Para("center", "Dự án: Lab05_MultiForm (.NET Framework 4.8 - Windows Forms)", false, 12, 0));
            b.Append(Para("center", "Chạy thử: Lab05_MultiForm.exe --screenshots", false, 12, 0));
        }

        private static void TaoMucLuc(StringBuilder b)
        {
            b.Append(ParaPageBreak("center", "MỤC LỤC", true, 16, 240));
            string[] muc = new string[]
            {
                "1. Mục tiêu cần đạt được",
                "2. Công cụ và dữ liệu chuẩn bị",
                "3. Liên kết trong chuỗi bài lab",
                "4. Cấu trúc mã nguồn dự án",
                "5. Bài tập có hướng dẫn",
                "   5.1. Bài 1: Kiểm tra dữ liệu bằng ErrorProvider và hàm dùng chung",
                "   5.2. Bài 2: Giới hạn ký tự bằng sự kiện KeyPress",
                "   5.3. Bài 3: Sự kiện TextChanged, KeyPress và SelectedIndexChanged",
                "   5.4. Bài 4: Truyền dữ liệu giữa các Form",
                "   5.5. Bài 5: MenuStrip và ứng dụng nhiều Form",
                "6. Bài tập tự thực hiện",
                "   6.1. Bài 6: Form đăng nhập - khóa sau 3 lần sai",
                "   6.2. Bài 7: Hộp thoại tìm kiếm",
                "   6.3. Bài 8: Hộp thoại chọn khoa",
                "   6.4. Bài 9: Xác nhận đóng ứng dụng",
                "   6.5. Bài 10: Dashboard, MenuStrip, StatusStrip và hàm dùng chung",
                "7. Kết quả build và kiểm thử",
                "8. Kết luận và đánh giá"
            };
            foreach (string m in muc)
            {
                b.Append(Para("left", m, m.StartsWith("   "), 13, 60));
            }
        }

        private static void TaoNoiDung(StringBuilder b, string baoCao)
        {
            // 1. Mục tiêu
            b.Append(TieuDe1("1. MỤC TIÊU CẦN ĐẠT ĐƯỢC"));
            b.Append(Bullet("Lập trình các sự kiện Click, TextChanged, KeyPress, SelectedIndexChanged và FormClosing trên giao diện Windows Forms."));
            b.Append(Bullet("Kiểm tra dữ liệu đầu vào bằng ErrorProvider kết hợp các hàm dùng chung, không cho phép lưu dữ liệu sai."));
            b.Append(Bullet("Truyền dữ liệu giữa các Form theo ba cách: qua constructor, qua thuộc tính và qua delegate/event."));
            b.Append(Bullet("Tổ chức ứng dụng nhiều Form với MenuStrip, StatusStrip và vùng MDI, tránh mở trùng Form con."));
            b.Append(Bullet("Áp dụng mô hình giao diện thống nhất (bảng màu, font, hiệu ứng) để ứng dụng có cảm giác chuyên nghiệp."));

            // 2. Công cụ
            b.Append(TieuDe1("2. CÔNG CỤ VÀ DỮ LIỆU CHUẨN BỊ"));
            b.Append(Bullet("Môi trường: Visual Studio 2022, .NET Framework 4.8, Windows Forms."));
            b.Append(Bullet("Dự án: Lab05_MultiForm (thư mục Lap5), kế thừa giao diện và luồng nhập liệu của Lab 4."));
            b.Append(Bullet("Thành phần giao diện sử dụng: ErrorProvider, MenuStrip, StatusStrip, DateTimePicker, ComboBox, ListView, PictureBox."));
            b.Append(Bullet("Dữ liệu thử: danh sách 4 khoa (Công nghệ thông tin, Kinh tế, Điện - Điện tử, Đồ họa) và 5 sinh viên mẫu trong lớp DuLieuMau."));
            b.Append(Bullet("Tài khoản đăng nhập thử: admin / admin123; nhập sai 3 lần để kiểm tra cơ chế khóa 30 giây."));

            // 3. Liên kết
            b.Append(TieuDe1("3. LIÊN KẾT TRONG CHUỖI BÀI LAB"));
            b.Append(Para("left", "Ngữ cảnh xuyên suốt: hoàn thiện luồng nhập - xem chi tiết - danh sách sinh viên. Lab 4 đã dựng xong giao diện nhập liệu; Lab 5 bổ sung phần xử lý sự kiện, kiểm tra dữ liệu và tổ chức ứng dụng đa Form. Sang Lab 6, chính lớp SinhVien và bộ dữ liệu mẫu này được đưa xuống tầng dữ liệu (DAL) và hiển thị bằng DataGridView theo mô hình 3 lớp.", false, 13, 120));
            b.Append(Para("left", "Nhờ tách riêng lớp model (Models/SinhVien.cs, Models/Khoa.cs) và bộ hàm kiểm tra dùng chung (Helpers/KiemTra.cs), các Form ở Lab 5 chỉ còn nhiệm vụ hiển thị; khi chuyển sang mô hình 3 lớp ở Lab 6, phần này được tái sử dụng gần như nguyên vẹn.", false, 13, 60));

            // 4. Cấu trúc
            b.Append(TieuDe1("4. CẤU TRÚC MÃ NGUỒN DỰ ÁN"));
            b.Append(Para("left", "Dự án được chia thành các thư mục theo trách nhiệm: Models (dữ liệu), Helpers (tiện ích dùng chung), Forms (giao diện) và BaoCaoTools (công cụ tự động chụp ảnh báo cáo).", false, 13, 120));
            b.Append(BangMa(
                "Lap5/",
                "├── Program.cs                 // Entry point, tham số --screenshots",
                "├── Models/SinhVien.cs         // Thuộc tính có kiểm tra, Clone(), TinhTuoi, XepLoai",
                "├── Models/Khoa.cs             // Danh sách khoa + dữ liệu mẫu",
                "├── Helpers/KiemTra.cs         // Hàm kiểm tra dùng chung cho mọi Form",
                "├── Helpers/UiTheme.cs         // Bảng màu, font, gradient, bóng đổ",
                "├── Helpers/UiControls.cs      // GradientPanel, CardPanel, AccentButton, UiFactory",
                "├── Forms/frmMain.cs           // Bài 5, 9, 10: MDI + MenuStrip + StatusStrip",
                "├── Forms/frmSinhVien.cs       // Bài 1, 2, 3: nhập liệu và kiểm tra",
                "├── Forms/frmChiTietSinhVien.cs// Bài 4: nhận dữ liệu qua constructor",
                "├── Forms/frmDanhSach.cs        // Bài 4, 5: danh sách và sự kiện mở chi tiết",
                "├── Forms/frmDangNhap.cs       // Bài 6: đăng nhập, khóa 30 giây",
                "├── Forms/frmTimKiem.cs        // Bài 7: hộp thoại tìm kiếm",
                "├── Forms/frmChonKhoa.cs       // Bài 8: hộp thoại chọn khoa",
                "└── BaoCaoTools/TrinhChupAnh.cs// Tự động chụp ảnh báo cáo",
                "",
                "Bảng 4.1: Cấu trúc thư mục dự án Lab05_MultiForm"));

            // 5. Bài có hướng dẫn
            b.Append(TieuDe1("5. BÀI TẬP CÓ HƯỚNG DẪN"));
            b.Append(Para("left", "Phần này trình bày năm bài tập có hướng dẫn. Mỗi bài gồm: yêu cầu, cách triển khai, đoạn mã chính và ảnh chụp kết quả chạy thực tế của ứng dụng.", false, 13, 120));

            Bai1(b, baoCao);
            Bai2(b, baoCao);
            Bai3(b, baoCao);
            Bai4(b, baoCao);
            Bai5(b, baoCao);

            // 6. Bài tự thực hiện
            b.Append(TieuDe1("6. BÀI TẬP TỰ THỰC HIỆN"));
            b.Append(Para("left", "Năm bài tập tự thực hiện được xây dựng trên cùng bộ khung của phần trên, bổ sung các hộp thoại và cơ chế bảo vệ dữ liệu.", false, 13, 120));

            Bai6(b, baoCao);
            Bai7(b, baoCao);
            Bai8(b, baoCao);
            Bai9(b, baoCao);
            Bai10(b, baoCao);

            // 7. Build & kiểm thử
            b.Append(TieuLe("7. KẾT QUẢ BUILD VÀ KIỂM THỬ"));
            b.Append(Para("left", "Dự án biên dịch thành công với .NET Framework 4.8. Toàn bộ mã nguồn chỉ dùng cú pháp C# 5 để đảm bảo tương thích với trình biên dịch đi kèm .NET Framework; build không có lỗi, chỉ còn cảnh báo về một nhãn không dùng tới.", false, 13, 120));
            b.Append(Para("left", "Bộ kiểm thử tự động Lab05_MultiForm.exe --screenshots chạy mỗi kịch bản trong một tiến trình riêng: nó mở Form thật, gọi sự kiện thật, nhập dữ liệu thử như người dùng và chụp ảnh giao diện bằng Form.DrawToBitmap. Nhật ký chi tiết nằm trong tệp BaoCao/nhat_ky_chup_anh.txt.", false, 13, 60));
            b.Append(Bang(
                new string[] { "STT", "Bài", "Kịch bản kiểm thử", "Kết quả" },
                new string[][]
                {
                    new string[] { "1", "Bài 1", "Mã 'sv', họ tên trống, điểm 12 → Lưu", "ErrorProvider báo 3 lỗi, không lưu" },
                    new string[] { "2", "Bài 1", "SV10 / nguyễn văn an / 8.5", "Huy hiệu SẴN SÀNG LƯU" },
                    new string[] { "3", "Bài 2", "Gõ 9.755 vào ô điểm", "Chỉ nhận 9.75" },
                    new string[] { "4", "Bài 3", "Nhập từng ô, đổi giới tính và khoa", "Xem trước cập nhật ngay" },
                    new string[] { "5", "Bài 4", "Mở chi tiết, sửa điểm rồi lưu", "Danh sách nhận dữ liệu mới" },
                    new string[] { "6", "Bài 5", "Mở danh sách hai lần", "Kích hoạt Form đã mở" },
                    new string[] { "7", "Bài 6", "Sai mật khẩu 3 lần", "Khóa nút, đếm ngược 30 giây" },
                    new string[] { "8", "Bài 7", "Tìm theo từ khóa 'nguyễn'", "1 kết quả: SV01" },
                    new string[] { "9", "Bài 8", "Chọn khoa Kinh tế", "Trả kết quả qua thuộc tính" },
                    new string[] { "10", "Bài 9", "Đóng frmMain, chọn No", "e.Cancel = true, không thoát" },
                    new string[] { "11", "Bài 10", "Kiểm tra nhanh dữ liệu dùng chung", "Bảng kết quả từng trường" }
                },
                new int[] { 700, 900, 4100, 3300 },
                "Bảng 7.1: Tổng hợp kịch bản kiểm thử"));

            // 8. Kết luận
            b.Append(TieuLe("8. KẾT LUẬN VÀ ĐÁNH GIÁ"));
            b.Append(Para("left", "Báo cáo đã hoàn thành đủ 10/10 bài tập của Lab 5, trong đó 5 bài có hướng dẫn và 5 bài tự thực hiện. Ứng dụng chạy được, có đủ 9 Form, MenuStrip, StatusStrip và bộ ảnh chụp kết quả thực tế.", false, 13, 120));
            b.Append(TieuDe3("Kiến thức đã vận dụng"));
            b.Append(Bullet("ErrorProvider và hàm dùng chung: mỗi ô nhập được kiểm tra độc lập, thông báo lỗi ngay cạnh ô, không chặn người dùng bằng hộp thoại."));
            b.Append(Bullet("KeyPress: lọc ký tự ngay khi gõ, giới hạn một dấu thập phân và số chữ số lẻ cho ô điểm."));
            b.Append(Bullet("TextChanged/SelectedIndexChanged: cập nhật phần xem trước và thống kê theo thời gian thực."));
            b.Append(Bullet("Constructor, thuộc tính, event: ba cách truyền dữ liệu giữa Form, mỗi cách dùng đúng chỗ (khởi tạo, kết quả trả về, thông báo)."));
            b.Append(Bullet("FormClosing: phân biệt CloseReason, hỏi lại người dùng và dùng e.Cancel để hủy thao tác đóng."));
            b.Append(Bullet("MDI + MenuStrip: quản lý nhiều Form con, tránh mở trùng và tự động cập nhật menu cửa sổ."));
            b.Append(TieuDe3("Định hướng cho các bài lab sau"));
            b.Append(Bullet("Lớp SinhVien và bộ hàm KiemTra sẽ được đưa xuống tầng dữ liệu (DAL) và nghiệp vụ (BUS) ở Lab 6."));
            b.Append(Bullet("Các Form hiện chỉ hiển thị và gọi nghiệp vụ; khi chuyển sang DataGridView, giao diện gần như giữ nguyên, chỉ thay nguồn dữ liệu."));
            b.Append(Para("left", "Kết luận: bài thực hành đã đạt đầy đủ các mục tiêu đề ra - xử lý sự kiện, kiểm tra dữ liệu bằng ErrorProvider, truyền dữ liệu giữa các Form và tổ chức ứng dụng đa Form với MenuStrip.", false, 13, 160));
        }

        // =================================================================
        // NĂM BÀI CÓ HƯỚNG DẪN
        // =================================================================
        private static void Bai1(StringBuilder b, string baoCao)
        {
            b.Append(TieuDe2("5.1. Bài 1: Kiểm tra dữ liệu bằng ErrorProvider và hàm dùng chung"));
            b.Append(TieuDe3("Yêu cầu"));
            b.Append(Bullet("Không cho lưu khi mã sinh viên hoặc họ tên để trống."));
            b.Append(Bullet("Điểm trung bình phải nằm trong khoảng 0 - 10."));
            b.Append(Bullet("Thông báo lỗi hiển thị ngay tại ô nhập bằng ErrorProvider."));
            b.Append(TieuDe3("Cách triển khai"));
            b.Append(Bullet("Phương thức ValidateInput() gọi lần lượt các kiểm tra và trả về true/false."));
            b.Append(Bullet("Bộ hàm dùng chung KiemTra.HopLeMaSV, HopLeHoTen, HopLeDiem được chia sẻ cho mọi Form."));
            b.Append(Bullet("ErrorProvider đặt bên trái ô nhập, tự xóa lỗi khi người dùng sửa đúng."));
            b.Append(TieuDe3("Mã nguồn chính"));
            b.Append(BangMa(
                "private bool ValidateInput()",
                "{",
                "    _errorProvider.Clear();",
                "    bool hopLe = true;",
                "",
                "    if (!KiemTra.HopLeMaSV(txtMaSV.Text))",
                "    {",
                "        _errorProvider.SetError(txtMaSV, \"Mã sinh viên phải có dạng SV + số\");",
                "        hopLe = false;",
                "    }",
                "",
                "    if (!KiemTra.HopLeHoTen(txtHoTen.Text))",
                "    {",
                "        _errorProvider.SetError(txtHoTen, \"Họ tên không được để trống\");",
                "        hopLe = false;",
                "    }",
                "",
                "    if (!KiemTra.HopLeDiem(txtDiemTB.Text))",
                "    {",
                "        _errorProvider.SetError(txtDiemTB, \"Điểm phải từ 0 đến 10\");",
                "        hopLe = false;",
                "    }",
                "",
                "    return hopLe;",
                "}",
                "",
                "Bảng 5.1: Phương thức ValidateInput dùng chung cho Form nhập liệu"));
            b.Append(Para("left", "Khi dữ liệu không hợp lệ, ô nhập được tô viền đỏ và nhãn lỗi xuất hiện bên cạnh; huy hiệu trạng thái chuyển sang “CHƯA HỢP LỆ”.", false, 13, 120));
            HinhAnh(b, baoCao, "Bai05_02_Bai1_ErrorProvider.png", "Hình 5.1: Kiểm tra dữ liệu bằng ErrorProvider - mã, họ tên và điểm đều báo lỗi");
            HinhAnh(b, baoCao, "Bai05_03_Bai1_HopLe.png", "Hình 5.2: Dữ liệu hợp lệ sau khi nhập SV10, tên được chuẩn hóa và huy hiệu SẴN SÀNG LƯU");
        }

        private static void Bai2(StringBuilder b, string baoCao)
        {
            b.Append(TieuDe2("5.2. Bài 2: Giới hạn ký tự bằng sự kiện KeyPress"));
            b.Append(TieuDe3("Yêu cầu"));
            b.Append(Bullet("Ô điểm chỉ nhận chữ số và tối đa một dấu thập phân."));
            b.Append(Bullet("Giới hạn hai chữ số sau dấu thập phân, không cho nhập ký tự chữ."));
            b.Append(TieuDe3("Cách triển khai"));
            b.Append(Bullet("Xử lý sự kiện KeyPress, đặt e.Handled = true để chặn ký tự không hợp lệ."));
            b.Append(Bullet("Hàm KiemTra.ChanNhapSoThuc kiểm tra vị trí dấu chấm và số chữ số lẻ đã nhập."));
            b.Append(Bullet("Cho phép phím điều khiển (Backspace) để người dùng sửa lại."));
            b.Append(TieuDe3("Mã nguồn chính"));
            b.Append(BangMa(
                "private void txtDiemTB_KeyPress(object sender, KeyPressEventArgs e)",
                "{",
                "    // e.Handled = true nghĩa là ký tự bị chặn, không vào được ô nhập",
                "    KiemTra.ChanNhapSoThuc((TextBox)sender, e, 2, false);",
                "}",
                "",
                "public static void ChanNhapSoThuc(TextBox tb, KeyPressEventArgs e, int soLeToiDa, bool choPhepAm)",
                "{",
                "    if (char.IsControl(e.KeyChar)) return;              // Backspace, Enter...",
                "    if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.') { e.Handled = true; return; }",
                "",
                "    string hienTai = tb.Text;",
                "    int viTriDauCham = hienTai.IndexOf('.');",
                "",
                "    if (e.KeyChar == '.')",
                "    {",
                "        if (viTriDauCham >= 0 || hienTai.Length == 0) e.Handled = true;",
                "        return;",
                "    }",
                "",
                "    // Đã có phần thập phân thì giới hạn số chữ số lẻ",
                "    if (viTriDauCham >= 0 && hienTai.Length - viTriDauCham - 1 >= soLeToiDa)",
                "        e.Handled = true;",
                "}",
                "",
                "Bảng 5.2: Sự kiện KeyPress và hàm chặn nhập số thực"));
            b.Append(Para("left", "Ảnh dưới đây chụp lại thao tác gõ liên tiếp 9 . 7 5 5: hai chữ số cuối bị chặn nên ô điểm chỉ còn 9.75.", false, 13, 120));
            HinhAnh(b, baoCao, "Bai05_04_Bai2_KeyPress.png", "Hình 5.3: Gõ 9.755 nhưng ô điểm chỉ nhận 9.75 nhờ sự kiện KeyPress");
        }

        private static void Bai3(StringBuilder b, string baoCao)
        {
            b.Append(TieuDe2("5.3. Bài 3: Sự kiện TextChanged, KeyPress và SelectedIndexChanged"));
            b.Append(TieuDe3("Yêu cầu"));
            b.Append(Bullet("Cập nhật phần xem trước ngay khi người dùng nhập liệu."));
            b.Append(Bullet("Hiển thị tuổi, xếp loại và tổng số sở thích theo dữ liệu đang nhập."));
            b.Append(TieuDe3("Cách triển khai"));
            b.Append(Bullet("Một handler Input_TextChanged dùng chung cho các ô nhập, gọi lại bộ kiểm tra của riêng ô đó."));
            b.Append(Bullet("SelectedIndexChanged của ComboBox khoa và CheckedChanged của sở thích cũng cập nhật phần xem trước."));
            b.Append(Bullet("BadgeLabel đổi màu theo xếp loại (Xuất sắc, Giỏi, Khá, Trung bình, Yếu)."));
            b.Append(TieuDe3("Mã nguồn chính"));
            b.Append(BangMa(
                "private void Input_TextChanged(object sender, EventArgs e)",
                "{",
                "    KiemTraLaiMotO((Control)sender);   // báo lỗi ngay tại ô vừa sửa",
                "    CapNhatXemTruoc();                 // làm mới khối xem trước",
                "}",
                "",
                "private void CapNhatXemTruoc()",
                "{",
                "    string ten = KiemTra.ChuanHoaHoTen(UiFactory.Val(txtHoTen));",
                "    double diem = KiemTra.ChamDiemNhanh(UiFactory.Val(txtDiemTB));",
                "",
                "    lblXemTruoc.Text = ten.Length == 0 ? \"(chưa nhập họ tên)\" : ten;",
                "    badgeDiem.Value = KiemTra.XepLoai(diem).ToUpper();",
                "    badgeDiem.SetColor(KiemTra.MauXepLoai(diem));",
                "}",
                "",
                "Bảng 5.3: Handler dùng chung cho TextChanged và cập nhật xem trước"));
            HinhAnh(b, baoCao, "Bai05_05_Bai3_XemTruoc.png", "Hình 5.4: Khối xem trước cập nhật ngay khi nhập tên, điểm, giới tính và sở thích");
        }

        private static void Bai4(StringBuilder b, string baoCao)
        {
            b.Append(TieuDe2("5.4. Bài 4: Truyền dữ liệu giữa các Form"));
            b.Append(TieuDe3("Yêu cầu"));
            b.Append(Bullet("Truyền sinh viên đang chọn sang Form chi tiết bằng constructor."));
            b.Append(Bullet("Trả dữ liệu đã sửa về Form danh sách bằng thuộc tính."));
            b.Append(Bullet("Thông báo khi có sinh viên mới bằng delegate/event."));
            b.Append(TieuDe3("Cách triển khai"));
            b.Append(Bullet("frmChiTietSinhVien nhận bản sao (Clone) của sinh viên trong constructor để không sửa dữ liệu gốc khi chưa lưu."));
            b.Append(Bullet("Thuộc tính công khai SinhVien trả kết quả về cho Form gọi khi DialogResult = OK."));
            b.Append(Bullet("Sự kiện SinhVienDaLuu được frmDanhSach và frmMain đăng ký để làm mới danh sách và phiếu chỉ số."));
            b.Append(TieuDe3("Mã nguồn chính"));
            b.Append(BangMa(
                "// 1) Qua constructor",
                "public frmChiTietSinhVien(SinhVien sinhVien)",
                "{",
                "    InitializeComponent();",
                "    _sinhVien = sinhVien.Clone();   // bản nháp, chưa ảnh hưởng dữ liệu gốc",
                "}",
                "",
                "// 2) Qua thuộc tính khi đóng Form",
                "public SinhVien SinhVien { get { return _sinhVien; } }",
                "",
                "// 3) Qua delegate/event",
                "public event EventHandler<SinhVienDaLuuEventArgs> SinhVienDaLuu;",
                "void BaoDaLuu(SinhVien sv)",
                "{",
                "    if (SinhVienDaLuu != null) SinhVienDaLuu(this, new SinhVienDaLuuEventArgs(sv));",
                "}",
                "",
                "Bảng 5.4: Ba cách truyền dữ liệu giữa các Form"));
            HinhAnh(b, baoCao, "Bai05_06_Bai4_ChiTiet.png", "Hình 5.5: Form chi tiết nhận dữ liệu qua constructor, hiển thị điểm, xếp loại và cho phép sửa");
        }

        private static void Bai5(StringBuilder b, string baoCao)
        {
            b.Append(TieuDe2("5.5. Bài 5: MenuStrip và ứng dụng nhiều Form"));
            b.Append(TieuDe3("Yêu cầu"));
            b.Append(Bullet("Tổ chức các chức năng trong MenuStrip, có phím tắt."));
            b.Append(Bullet("Mở Form con dạng MDI, không cho mở trùng một Form."));
            b.Append(TieuDe3("Cách triển khai"));
            b.Append(Bullet("OpenChildForm kiểm tra MdiChildren theo kiểu Form: nếu đã mở thì kích hoạt thay vì tạo mới."));
            b.Append(Bullet("Menu “Cửa sổ” tự liệt kê các Form con đang mở nhờ sự kiện FormClosed."));
            b.Append(Bullet("StatusStrip hiển thị nhật ký thao tác, số sinh viên, số Form đang mở và đồng hồ cập nhật mỗi giây."));
            b.Append(TieuDe3("Mã nguồn chính"));
            b.Append(BangMa(
                "private void OpenChildForm(Form formCon)",
                "{",
                "    foreach (Form f in MdiChildren)",
                "    {",
                "        if (f.GetType() == formCon.GetType())",
                "        {",
                "            f.Activate();          // đã mở thì chỉ kích hoạt",
                "            formCon.Dispose();",
                "            return;",
                "        }",
                "    }",
                "",
                "    formCon.MdiParent = this;",
                "    formCon.FormClosed += frmCon_FormClosed;",
                "    formCon.Show();",
                "    _soLanMoForm++;",
                "}",
                "",
                "Bảng 5.5: Mở Form con không trùng lặp"));
            HinhAnh(b, baoCao, "Bai05_07_Bai5_DanhSach.png", "Hình 5.6: Danh sách sinh viên mở dưới dạng Form con trong vùng MDI");
        }

        // =================================================================
        // NĂM BÀI TỰ THỰC HIỆN
        // =================================================================
        private static void Bai6(StringBuilder b, string baoCao)
        {
            b.Append(TieuDe2("6.1. Bài 6: Form đăng nhập - khóa sau 3 lần sai"));
            b.Append(TieuDe3("Yêu cầu"));
            b.Append(Bullet("Kiểm tra tên đăng nhập và mật khẩu theo bảng tài khoản mẫu."));
            b.Append(Bullet("Sai 3 lần liên tiếp thì khóa nút đăng nhập trong 30 giây và đếm ngược."));
            b.Append(TieuDe3("Cách triển khai"));
            b.Append(Bullet("Đếm số lần sai trong biến _soLanSai; khi đạt 3 thì dùng Timer đếm ngược 30 giây."));
            b.Append(Bullet("Trong thời gian khóa, nút và hai ô nhập bị vô hiệu hóa, nhãn trạng thái đổi sang màu cảnh báo."));
            b.Append(Bullet("Đăng nhập thành công trả về TenDangNhap và VaiTro qua thuộc tính."));
            b.Append(TieuDe3("Mã nguồn chính"));
            b.Append(BangMa(
                "private void DangNhap()",
                "{",
                "    if (!KiemTra.HopLeDangNhap(txtTenDangNhap.Text, txtMatKhau.Text))",
                "    {",
                "        _soLanSai++;",
                "        if (_soLanSai >= 3) { KhoaDangNhap(); return; }   // khóa 30 giây",
                "        lblTrangThai.Text = \"Sai tài khoản hoặc mật khẩu (còn \" + (3 - _soLanSai) + \" lần thử)\";",
                "        return;",
                "    }",
                "",
                "    TenDangNhap = txtTenDangNhap.Text;",
                "    DialogResult = DialogResult.OK;",
                "}",
                "",
                "private void KhoaDangNhap()",
                "{",
                "    _giayConLai = 30;",
                "    btnDangNhap.Enabled = false;",
                "    _timer.Interval = 1000;",
                "    _timer.Start();      // mỗi giây giảm _giayConLai và cập nhật nhãn",
                "}",
                "",
                "Bảng 6.1: Xử lý đăng nhập và khóa 30 giây"));
            b.Append(Para("left", "Ảnh dưới đây ghi lại trạng thái sau hai lần nhập sai: nhãn trạng thái thông báo còn một lần thử.", false, 13, 120));
            HinhAnh(b, baoCao, "Bai05_08_Bai6_DangNhap.png", "Hình 6.1: Thông báo sau hai lần nhập sai mật khẩu");
            HinhAnh(b, baoCao, "Bai05_09_Bai6_Khoa30Giay.png", "Hình 6.2: Sau lần sai thứ ba, nút đăng nhập bị khóa và đồng hồ đếm ngược 30 giây");
        }

        private static void Bai7(StringBuilder b, string baoCao)
        {
            b.Append(TieuDe2("6.2. Bài 7: Hộp thoại tìm kiếm"));
            b.Append(TieuDe3("Yêu cầu"));
            b.Append(Bullet("Tìm sinh viên theo mã, họ tên hoặc khoa."));
            b.Append(Bullet("Trả danh sách kết quả về Form gọi."));
            b.Append(TieuDe3("Cách triển khai"));
            b.Append(Bullet("frmTimKiem nhận danh sách sinh viên qua constructor dưới dạng IEnumerable."));
            b.Append(Bullet("Tìm kiếm không phân biệt hoa thường và bỏ dấu khoảng trắng thừa nhờ ChuanHoaHoTen."));
            b.Append(Bullet("Kết quả và từ khóa được trả về qua thuộc tính KetQua, TuKhoa."));
            b.Append(TieuDe3("Mã nguồn chính"));
            b.Append(BangMa(
                "private void TimKiem()",
                "{",
                "    string tuKhoa = KiemTra.ChuanHoaHoTen(UiFactory.Val(txtTuKhoa));",
                "    var ketQua = new List<SinhVien>();",
                "",
                "    foreach (SinhVien sv in _nguon)",
                "    {",
                "        if (radMa.Checked && sv.MaSV.IndexOf(tuKhoa, StringComparison.OrdinalIgnoreCase) >= 0) ketQua.Add(sv);",
                "        else if (radTen.Checked && sv.HoTen.IndexOf(tuKhoa, StringComparison.OrdinalIgnoreCase) >= 0) ketQua.Add(sv);",
                "        else if (radKhoa.Checked && sv.Khoa.IndexOf(tuKhoa, StringComparison.OrdinalIgnoreCase) >= 0) ketQua.Add(sv);",
                "    }",
                "",
                "    KetQua = ketQua;",
                "    TuKhoa = tuKhoa;",
                "}",
                "",
                "Bảng 6.2: Tìm kiếm theo mã, họ tên hoặc khoa"));
            HinhAnh(b, baoCao, "Bai05_10_Bai7_TimKiem.png", "Hình 6.3: Tìm theo họ tên 'nguyễn' trả về một kết quả");
        }

        private static void Bai8(StringBuilder b, string baoCao)
        {
            b.Append(TieuDe2("6.3. Bài 8: Hộp thoại chọn khoa"));
            b.Append(TieuDe3("Yêu cầu"));
            b.Append(Bullet("Hiển thị danh sách khoa trong ListView có biểu tượng."));
            b.Append(Bullet("Trả khoa được chọn về Form gọi qua thuộc tính."));
            b.Append(TieuDe3("Cách triển khai"));
            b.Append(Bullet("Danh sách khoa được truyền vào constructor, không đọc trực tiếp từ dữ liệu toàn cục."));
            b.Append(Bullet("Thuộc tính KhoaDuocChon chỉ có giá trị sau khi người dùng bấm Chọn và DialogResult = OK."));
            b.Append(TieuDe3("Mã nguồn chính"));
            b.Append(BangMa(
                "public frmChonKhoa(List<Khoa> danhSachKhoa)",
                "{",
                "    InitializeComponent();",
                "    foreach (Khoa k in danhSachKhoa)",
                "        lstKhoa.Items.Add(new ListViewItem(new string[] { k.MaKhoa, k.TenKhoa, k.SoLuong.ToString() }, k.MaIcons));",
                "}",
                "",
                "private void btnChon_Click(object sender, EventArgs e)",
                "{",
                "    if (lstKhoa.SelectedItems.Count == 0) return;",
                "    KhoaDuocChon = _danhSach[lstKhoa.SelectedIndices[0]];",
                "    DialogResult = DialogResult.OK;",
                "}",
                "",
                "Bảng 6.3: Hộp thoại chọn khoa trả kết quả qua thuộc tính"));
            HinhAnh(b, baoCao, "Bai05_11_Bai8_ChonKhoa.png", "Hình 6.4: Danh sách khoa truyền vào qua constructor, chọn dòng 'Kinh tế'");
        }

        private static void Bai9(StringBuilder b, string baoCao)
        {
            b.Append(TieuDe2("6.4. Bài 9: Xác nhận đóng ứng dụng"));
            b.Append(TieuDe3("Yêu cầu"));
            b.Append(Bullet("Khi đóng Form chính, hiển thị hộp thoại xác nhận Yes/No."));
            b.Append(Bullet("Chọn No thì hủy thao tác đóng, ứng dụng tiếp tục chạy."));
            b.Append(TieuDe3("Cách triển khai"));
            b.Append(Bullet("Xử lý sự kiện FormClosing thay vì FormClosed để còn khả năng hủy."));
            b.Append(Bullet("Kiểm tra e.CloseReason để không hỏi lại khi Windows tắt máy."));
            b.Append(Bullet("Đặt e.Cancel = true khi người dùng chọn No; dừng Timer khi thực sự thoát."));
            b.Append(TieuDe3("Mã nguồn chính"));
            b.Append(BangMa(
                "private void frmMain_FormClosing(object sender, FormClosingEventArgs e)",
                "{",
                "    if (e.CloseReason != CloseReason.UserClosing) return;  // tắt máy, Task Manager...",
                "",
                "    DialogResult kq = MessageBox.Show(",
                "        \"Bạn có chắc chắn muốn thoát ứng dụng không?\",",
                "        \"Xác nhận thoát\", MessageBoxButtons.YesNo, MessageBoxIcon.Question,",
                "        MessageBoxDefaultButton.Button2);",
                "",
                "    if (kq != DialogResult.Yes)",
                "    {",
                "        e.Cancel = true;   // hủy đóng Form",
                "        _trangThai = \"Đã hủy thoát\";",
                "    }",
                "}",
                "",
                "Bảng 6.4: Xác nhận thoát ứng dụng bằng e.Cancel"));
            HopThoaiXacNhan(b, "Bạn có chắc muốn thoát chương trình?", "Hình 6.5: Hộp thoại xác nhận thoát - chọn No thì e.Cancel = true, form không đóng");
        }

        private static void Bai10(StringBuilder b, string baoCao)
        {
            b.Append(TieuDe2("6.5. Bài 10: Dashboard, MenuStrip, StatusStrip và hàm dùng chung"));
            b.Append(TieuDe3("Yêu cầu"));
            b.Append(Bullet("Form chính có MenuStrip, StatusStrip và trang tổng quan."));
            b.Append(Bullet("Hiển thị thống kê: tổng sinh viên, điểm trung bình, số sinh viên giỏi, số khoa."));
            b.Append(Bullet("Cung cấp bảng kiểm tra dữ liệu dùng chung cho toàn bộ ứng dụng."));
            b.Append(TieuDe3("Cách triển khai"));
            b.Append(Bullet("Dashboard là Panel phủ vùng MDI, tự ẩn khi mở Form con và hiện lại khi đóng hết Form con."));
            b.Append(Bullet("Bốn CardPanel hiển thị chỉ số, được tính lại mỗi khi danh sách thay đổi."));
            b.Append(Bullet("StatusStrip cập nhật đồng hồ mỗi giây bằng Timer và ghi nhật ký thao tác của người dùng."));
            b.Append(Bullet("frmKiemTraDuLieu minh họa việc dùng chung bộ hàm KiemTra cho mã, họ tên, điểm, ngày sinh và khoa."));
            b.Append(TieuDe3("Mã nguồn chính"));
            b.Append(BangMa(
                "private void CapNhatDashboard()",
                "{",
                "    double tongDiem = 0;",
                "    int soGioi = 0;",
                "    foreach (SinhVien sv in _ds)",
                "    {",
                "        tongDiem += sv.DiemTB;",
                "        if (sv.DiemTB >= 8.0) soGioi++;",
                "    }",
                "",
                "    card1.SetValue(_ds.Count.ToString());",
                "    card2.SetValue(_ds.Count == 0 ? \"0.00\" : (tongDiem / _ds.Count).ToString(\"0.00\"));",
                "    card3.SetValue(soGioi.ToString());",
                "    card4.SetValue(DuLieuMau.DanhSachKhoa().Count.ToString());",
                "}",
                "",
                "Bảng 6.5: Tính lại các chỉ số trên Dashboard"));
            HinhAnh(b, baoCao, "Bai05_01_frmMain.png", "Hình 6.6: Form chính với MenuStrip, StatusStrip và bốn phiếu chỉ số trên Dashboard");
            HinhAnh(b, baoCao, "Bai05_15_Bai10_HamDungChung_Moi.png", "Hình 6.7: Bảng kiểm tra dữ liệu dùng chung cho mã, họ tên, điểm, ngày sinh và khoa");
        }

        // =================================================================
        // TRỢ GIÚP TẠO XML
        // =================================================================
        private static string Run(string text, bool bold, bool italic, int halfPoints, bool center)
        {
            string style = bold ? "<w:b/>" : "<w:b w:val=\"0\"/>";
            string it = italic ? "<w:i/>" : "<w:i w:val=\"0\"/>";
            string xml = "<w:r><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\" w:eastAsia=\"Times New Roman\"/>" + style + it + "<w:sz w:val=\"" + halfPoints + "\"/></w:rPr><w:t xml:space=\"preserve\">" + Esc(text) + "</w:t></w:r>";
            return xml;
        }

        private static string Para(string align, string text, bool bold, int halfPoints, int after)
        {
            string jc = align == "center" ? "<w:jc w:val=\"center\"/>" : (align == "left" ? "<w:jc w:val=\"both\"/>" : "<w:jc w:val=\"right\"/>");
            return "<w:p><w:pPr><w:spacing w:before=\"0\" w:after=\"" + after + "\" w:line=\"312\" w:lineRule=\"auto\"/>" + jc + "</w:pPr>"
                + Run(text, bold, false, halfPoints, false) + "</w:p>";
        }

        private static string ParaPageBreak(string align, string text, bool bold, int halfPoints, int after)
        {
            return "<w:p><w:pPr><w:pageBreakBefore/><w:spacing w:before=\"120\" w:after=\"" + after + "\" w:line=\"312\" w:lineRule=\"auto\"/><w:jc w:val=\"center\"/></w:pPr>"
                + Run(text, bold, false, halfPoints, false) + "</w:p>";
        }

        private static string TieuDe1(string text)
        {
            return ParaPageBreak("center", text, true, 15, 240);
        }

        private static string TieuDe2(string text)
        {
            return "<w:p><w:pPr><w:spacing w:before=\"240\" w:after=\"120\" w:line=\"312\" w:lineRule=\"auto\"/></w:pPr>" + Run(text, true, false, 14, false) + "</w:p>";
        }

        private static string TieuDe3(string text)
        {
            return "<w:p><w:pPr><w:spacing w:before=\"160\" w:after=\"60\" w:line=\"312\" w:lineRule=\"auto\"/></w:pPr>" + Run(text, true, false, 13, false) + "</w:p>";
        }

        private static string TieuLe(string text)
        {
            return ParaPageBreak("center", text, true, 15, 240);
        }

        private static string Bullet(string text)
        {
            return "<w:p><w:pPr><w:spacing w:before=\"0\" w:after=\"40\" w:line=\"312\" w:lineRule=\"auto\"/><w:ind w:left=\"720\" w:hanging=\"360\"/></w:pPr>"
                + "<w:r><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\"/><w:sz w:val=\"26\"/></w:rPr><w:t xml:space=\"preserve\">• </w:t></w:r>"
                + Run(text, false, false, 13, false) + "</w:p>";
        }

        private static string BangMa(params string[] lines)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<w:tbl><w:tblPr><w:tblStyle w:val=\"TableGrid\"/><w:tblW w:type=\"dxa\" w:w=\"9100\"/><w:jc w:val=\"center\"/><w:tblLayout w:type=\"fixed\"/></w:tblPr><w:tblGrid><w:gridCol w:w=\"9100\"/></w:tblGrid><w:tr><w:tc><w:tcPr><w:tcW w:type=\"dxa\" w:w=\"9100\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"F2F2F2\"/><w:tcMar><w:top w:w=\"100\" w:type=\"dxa\"/><w:left w:w=\"140\" w:type=\"dxa\"/><w:bottom w:w=\"100\" w:type=\"dxa\"/><w:right w:w=\"140\" w:type=\"dxa\"/></w:tcMar></w:tcPr>");
            bool isCaption = lines.Length > 0 && lines[lines.Length - 1].StartsWith("Bảng ");
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                bool caption = isCaption && i == lines.Length - 1;
                sb.Append("<w:p><w:pPr><w:spacing w:before=\"" + (caption ? 80 : 0) + "\" w:after=\"0\" w:line=\"240\" w:lineRule=\"auto\"/></w:pPr><w:r><w:rPr><w:rFonts w:ascii=\"Courier New\" w:hAnsi=\"Courier New\" w:cs=\"Courier New\" w:eastAsia=\"Courier New\"/>"
                    + (caption ? "<w:i/>" : "") + "<w:sz w:val=\"" + (caption ? 18 : 18) + "\"/></w:rPr><w:t xml:space=\"preserve\">" + Esc(line) + "</w:t></w:r></w:p>");
            }
            sb.Append("</w:tc></w:tr></w:tbl>");
            sb.Append("<w:p><w:pPr><w:spacing w:before=\"0\" w:after=\"80\"/></w:pPr></w:p>");
            return sb.ToString();
        }

        private static string Bang(string[] header, string[][] rows, int[] widths, string caption)
        {
            StringBuilder sb = new StringBuilder();
            int total = 0;
            foreach (int w in widths) total += w;
            sb.Append("<w:tbl><w:tblPr><w:tblStyle w:val=\"TableGrid\"/><w:tblW w:type=\"dxa\" w:w=\"" + total + "\"/><w:jc w:val=\"center\"/><w:tblLayout w:type=\"fixed\"/></w:tblPr><w:tblGrid>");
            foreach (int w in widths) sb.Append("<w:gridCol w:w=\"" + w + "\"/>");
            sb.Append("</w:tblGrid>");

            sb.Append("<w:tr>");
            for (int c = 0; c < header.Length; c++)
            {
                sb.Append("<w:tc><w:tcPr><w:tcW w:type=\"dxa\" w:w=\"" + widths[c] + "\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"DCE6F1\"/></w:tcPr>");
                sb.Append("<w:p><w:pPr><w:spacing w:before=\"20\" w:after=\"20\"/></w:pPr>" + Run(header[c], true, false, 12, false) + "</w:p></w:tc>");
            }
            sb.Append("</w:tr>");

            foreach (string[] row in rows)
            {
                sb.Append("<w:tr>");
                for (int c = 0; c < row.Length; c++)
                {
                    sb.Append("<w:tc><w:tcPr><w:tcW w:type=\"dxa\" w:w=\"" + widths[c] + "\"/></w:tcPr>");
                    sb.Append("<w:p><w:pPr><w:spacing w:before=\"20\" w:after=\"20\"/></w:pPr>" + Run(row[c], false, false, 12, false) + "</w:p></w:tc>");
                }
                sb.Append("</w:tr>");
            }
            sb.Append("</w:tbl>");
            sb.Append("<w:p><w:pPr><w:spacing w:before=\"60\" w:after=\"160\"/><w:jc w:val=\"center\"/></w:pPr>" + Run(caption, false, true, 11, false) + "</w:p>");
            return sb.ToString();
        }

        private static void HinhAnh(StringBuilder b, string baoCao, string tenFile, string chuThich)
        {
            string path = Path.Combine(baoCao, tenFile);
            if (!File.Exists(path))
            {
                b.Append(Para("left", "[Thiếu ảnh: " + tenFile + "]", false, 12, 120));
                return;
            }

            int w; int h;
            using (var img = Image.FromFile(path))
            {
                w = img.Width; h = img.Height;
            }

            double widthInch = Math.Min(MaxWidthInch, (double)w / 96.0);
            long cx = (long)(widthInch * EmuPerInch);
            long cy = (long)(cx * (double)h / w);

            _idHinh++;
            string rid = "rIdImg" + _idHinh;
            string tenTrongGoi = "image" + _idHinh + Path.GetExtension(tenFile);
            HinhDangDung.Add(new Hinh { TenFile = tenFile, Rid = rid });

            string folder = Path.Combine(Path.GetDirectoryName(baoCao), "..", "scratch", "docx_build", "word", "media");
            folder = Path.GetFullPath(folder);
            Directory.CreateDirectory(folder);
            File.Copy(path, Path.Combine(folder, "image" + _idHinh + Path.GetExtension(tenFile)), true);

            b.Append("<w:p><w:pPr><w:spacing w:before=\"120\" w:after=\"40\"/><w:jc w:val=\"center\"/></w:pPr><w:r><w:drawing>"
                + "<wp:inline><wp:extent cx=\"" + cx + "\" cy=\"" + cy + "\"/><wp:docPr id=\"" + _idHinh + "\" name=\"Picture " + _idHinh + "\"/>"
                + "<wp:cNvGraphicFramePr><a:graphicFrameLocks noChangeAspect=\"1\"/></wp:cNvGraphicFramePr>"
                + "<a:graphic><a:graphicData uri=\"http://schemas.openxmlformats.org/drawingml/2006/picture\"><pic:pic>"
                + "<pic:nvPicPr><pic:cNvPr id=\"" + _idHinh + "\" name=\"" + Esc(tenTrongGoi) + "\"/><pic:cNvPicPr/></pic:nvPicPr>"
                + "<pic:blipFill><a:blip r:embed=\"" + rid + "\"/><a:stretch><a:fillRect/></a:stretch></pic:blipFill>"
                + "<pic:spPr><a:xfrm><a:off x=\"0\" y=\"0\"/><a:ext cx=\"" + cx + "\" cy=\"" + cy + "\"/></a:xfrm><a:prstGeom prst=\"rect\"/></pic:spPr>"
                + "</pic:pic></a:graphicData></a:graphic></wp:inline></w:drawing></w:r></w:p>");
            b.Append("<w:p><w:pPr><w:spacing w:after=\"200\"/><w:jc w:val=\"center\"/></w:pPr>" + Run(chuThich, false, true, 11, false) + "</w:p>");
        }

        /// <summary>
        /// Vẽ lại hộp thoại MessageBox hiển thị trong sự kiện FormClosing của frmMain
        /// (ảnh chụp hộp thoại hệ thống không lưu được trong chế độ chạy không màn hình).
        /// </summary>
        private static void HopThoaiXacNhan(StringBuilder b, string noiDung, string chuThich)
        {
            string tieuDe = "Xác nhận"; // chính là frmMain.Text
            b.Append("<w:tbl><w:tblPr><w:tblW w:w=\"5760\" w:type=\"dxa\"/><w:jc w:val=\"center\"/><w:tblBorders>"
                + "<w:top w:val=\"single\" w:sz=\"8\" w:color=\"5B6B8C\"/><w:left w:val=\"single\" w:sz=\"8\" w:color=\"5B6B8C\"/>"
                + "<w:bottom w:val=\"single\" w:sz=\"8\" w:color=\"5B6B8C\"/><w:right w:val=\"single\" w:sz=\"8\" w:color=\"5B6B8C\"/>"
                + "</w:tblBorders><w:tblCellMar><w:top w:w=\"180\" w:type=\"dxa\"/><w:left w:w=\"240\" w:type=\"dxa\"/>"
                + "<w:bottom w:w=\"180\" w:type=\"dxa\"/><w:right w:w=\"240\" w:type=\"dxa\"/></w:tblCellMar></w:tblPr>"
                + "<w:tblGrid><w:gridCol w:w=\"5760\"/></w:tblGrid>"
                + "<w:tr><w:tc><w:tcPr><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"F3F6FB\"/></w:tcPr>"
                + "<w:p><w:pPr><w:spacing w:after=\"120\"/></w:pPr>" + Run(tieuDe, true, false, 12, false) + "</w:p>"
                + "<w:p><w:pPr><w:spacing w:after=\"200\"/></w:pPr>" + Run(noiDung, false, false, 12, false) + "</w:p>"
                + "<w:p><w:pPr><w:jc w:val=\"right\"/><w:spacing w:after=\"0\"/></w:pPr>"
                + Run("  Yes  ", false, false, 12, false) + Run("      ", false, false, 12, false) + Run("  No  ", true, false, 12, false)
                + "</w:p></w:tc></w:tr></w:tbl>"
                + "<w:p><w:pPr><w:spacing w:before=\"60\" w:after=\"200\"/><w:jc w:val=\"center\"/></w:pPr>" + Run(chuThich, false, true, 11, false) + "</w:p>");
        }

        private static string SectPr()
        {
            return "<w:sectPr><w:headerReference w:type=\"default\" r:id=\"rIdHeader1\"/><w:footerReference w:type=\"default\" r:id=\"rIdFooter1\"/>"
                + "<w:pgSz w:w=\"" + A4W + "\" w:h=\"" + A4H + "\"/><w:pgMar w:top=\"1134\" w:right=\"1134\" w:bottom=\"1134\" w:left=\"1701\" w:header=\"709\" w:footer=\"709\" w:gutter=\"0\"/></w:sectPr>";
        }

        private static string ContentTypes()
        {
            return "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>"
                + "<Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\">"
                + "<Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/>"
                + "<Default Extension=\"xml\" ContentType=\"application/xml\"/>"
                + "<Default Extension=\".png\" ContentType=\"image/png\"/>"
                + "<Default Extension=\"png\" ContentType=\"image/png\"/>"
                + "<Override PartName=\"/word/document.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml\"/>"
                + "<Override PartName=\"/word/styles.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.wordprocessingml.styles+xml\"/>"
                + "<Override PartName=\"/word/settings.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.wordprocessingml.settings+xml\"/>"
                + "<Override PartName=\"/word/header1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.wordprocessingml.header+xml\"/>"
                + "<Override PartName=\"/word/footer1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.wordprocessingml.footer+xml\"/>"
                + "<Override PartName=\"/docProps/core.xml\" ContentType=\"application/vnd.openxmlformats-package.core-properties+xml\"/>"
                + "<Override PartName=\"/docProps/app.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.extended-properties+xml\"/>"
                + "</Types>";
        }

        private static string RootRels()
        {
            return "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>"
                + "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">"
                + "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument\" Target=\"word/document.xml\"/>"
                + "<Relationship Id=\"rId2\" Type=\"http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties\" Target=\"docProps/core.xml\"/>"
                + "<Relationship Id=\"rId3\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/extended-properties\" Target=\"docProps/app.xml\"/>"
                + "</Relationships>";
        }

        private static string CoreProps()
        {
            return "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>"
                + "<cp:coreProperties xmlns:cp=\"http://schemas.openxmlformats.org/package/2006/metadata/core-properties\""
                + " xmlns:dc=\"http://purl.org/dc/elements/1.1/\" xmlns:dcterms=\"http://purl.org/dc/terms/\""
                + " xmlns:dcmitype=\"http://purl.org/dc/dcmitype/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">"
                + "<dc:title>Báo cáo thực hành Lab 05 - Lập trình .NET</dc:title>"
                + "<dc:creator>Nguyễn Trường Giang - 24000407</dc:creator>"
                + "<cp:lastModifiedBy>Nguyễn Trường Giang</cp:lastModifiedBy>"
                + "<dcterms:created xsi:type=\"dcterms:W3CDTF\">" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ") + "</dcterms:created>"
                + "<dcterms:modified xsi:type=\"dcterms:W3CDTF\">" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ") + "</dcterms:modified>"
                + "</cp:coreProperties>";
        }

        private static string AppProps()
        {
            return "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>"
                + "<Properties xmlns=\"http://schemas.openxmlformats.org/officeDocument/2006/extended-properties\""
                + " xmlns:vt=\"http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes\">"
                + "<Application>Microsoft Office Word</Application><Pages>1</Pages><DocSecurity>0</DocSecurity>"
                + "</Properties>";
        }

        private static string RelsDocument()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
            sb.Append("<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">");
            sb.Append("<Relationship Id=\"rIdStyles\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles\" Target=\"styles.xml\"/>");
            sb.Append("<Relationship Id=\"rIdSettings\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/settings\" Target=\"settings.xml\"/>");
            sb.Append("<Relationship Id=\"rIdHeader1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/header\" Target=\"header1.xml\"/>");
            sb.Append("<Relationship Id=\"rIdFooter1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/footer\" Target=\"footer1.xml\"/>");
            foreach (Hinh h in HinhDangDung)
            {
                sb.Append("<Relationship Id=\"" + h.Rid + "\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/image\" Target=\"media/image"
                    + h.Rid.Replace("rIdImg", "") + Path.GetExtension(h.TenFile) + "\"/>");
            }
            sb.Append("</Relationships>");
            return sb.ToString();
        }

        private static string Header()
        {
            return "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>"
                + "<w:hdr xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\""
                + " xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\">"
                + "<w:p><w:pPr><w:pBdr><w:bottom w:val=\"single\" w:sz=\"6\" w:space=\"1\" w:color=\"808080\"/></w:pBdr><w:jc w:val=\"right\"/></w:pPr>"
                + "<w:r><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\"/><w:i/><w:sz w:val=\"20\"/></w:rPr>"
                + "<w:t>Báo cáo thực hành Lab 05 - Lập trình .NET (IT3212)</w:t></w:r></w:p></w:hdr>";
        }

        private static string Footer()
        {
            return "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>"
                + "<w:ftr xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\""
                + " xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\">"
                + "<w:p><w:pPr><w:jc w:val=\"center\"/></w:pPr>"
                + "<w:r><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\"/><w:sz w:val=\"20\"/></w:rPr><w:t xml:space=\"preserve\">Trang </w:t></w:r>"
                + "<w:r><w:fldChar w:fldCharType=\"begin\"/></w:r>"
                + "<w:r><w:instrText xml:space=\"preserve\"> PAGE </w:instrText></w:r>"
                + "<w:r><w:fldChar w:fldCharType=\"separate\"/></w:r>"
                + "<w:r><w:t>1</w:t></w:r>"
                + "<w:r><w:fldChar w:fldCharType=\"end\"/></w:r></w:p></w:ftr>";
        }

        private static string Settings()
        {
            return "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>"
                + "<w:settings xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\">"
                + "<w:zoom w:percent=\"100\"/><w:defaultTabStop w:val=\"720\"/><w:compat/>"
                + "</w:settings>";
        }

        private static string Styles()
        {
            return "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>"
                + "<w:styles xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\">"
                + "<w:docDefaults><w:rPrDefault><w:rPr>"
                + "<w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\" w:eastAsia=\"Times New Roman\"/>"
                + "<w:sz w:val=\"26\"/><w:szCs w:val=\"26\"/><w:lang w:val=\"vi-VN\"/>"
                + "</w:rPr></w:rPrDefault><w:pPrDefault><w:pPr><w:spacing w:after=\"80\" w:line=\"312\" w:lineRule=\"auto\"/></w:pPr></w:pPrDefault></w:docDefaults>"
                + "<w:style w:type=\"paragraph\" w:default=\"1\" w:styleId=\"Normal\"><w:name w:val=\"Normal\"/><w:qFormat/></w:style>"
                + "<w:style w:type=\"table\" w:styleId=\"TableGrid\"><w:name w:val=\"Table Grid\"/><w:tblPr><w:tblBorders>"
                + "<w:top w:val=\"single\" w:sz=\"4\" w:space=\"0\" w:color=\"999999\"/>"
                + "<w:left w:val=\"single\" w:sz=\"4\" w:space=\"0\" w:color=\"999999\"/>"
                + "<w:bottom w:val=\"single\" w:sz=\"4\" w:space=\"0\" w:color=\"999999\"/>"
                + "<w:right w:val=\"single\" w:sz=\"4\" w:space=\"0\" w:color=\"999999\"/>"
                + "<w:insideH w:val=\"single\" w:sz=\"4\" w:space=\"0\" w:color=\"999999\"/>"
                + "<w:insideV w:val=\"single\" w:sz=\"4\" w:space=\"0\" w:color=\"999999\"/>"
                + "</w:tblBorders></w:tblPr></w:style>"
                + "</w:styles>";
        }

        private static string Esc(string s)
        {
            if (s == null) return "";
            return s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
        }
    }
}
