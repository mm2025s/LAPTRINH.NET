using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace BuildDocx6
{
    /// <summary>
    /// Sinh báo cáo thực hành Lab 6 dạng .docx (OpenXML) - chèn ảnh chụp THẬT trong Lap6/BaoCao.
    ///
    /// Không phụ thuộc Microsoft Word: tự ghi các phần XML tối thiểu của gói OPC
    /// (trang bìa, mục lục, tiêu đề, bảng mã nguồn, bảng dữ liệu, ảnh + chú thích).
    /// Tệp được đóng gói bằng ZipArchive với tên entry dùng dấu '/' đúng chuẩn OPC.
    /// </summary>
    public static class Program
    {
        private const string A4W = "11906";
        private const string A4H = "16838";
        private const long EmuPerInch = 914400;
        private const double MaxWidthInch = 6.3;

        private static readonly List<string> AnhDaDung = new List<string>();

        public static int Main(string[] args)
        {
            string root = args.Length > 0 ? args[0] : @"d:\LAPTRINH.NET";
            string lap = Path.Combine(root, "Lap6");
            string baoCao = Path.Combine(lap, "BaoCao");
            string outFile = Path.Combine(baoCao, "BaoCao_Lab06_24000407.docx");
            string work = Path.Combine(root, "scratch", "docx_build6");
            if (Directory.Exists(work)) Directory.Delete(work, true);
            Directory.CreateDirectory(work);

            StringBuilder body = new StringBuilder();
            TaoTrangBia(body);
            TaoMucLuc(body);
            TaoNoiDung(body, baoCao);

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
            DongGoi(work, outFile);

            Console.WriteLine("Da tao: " + outFile);
            Console.WriteLine("So hinh: " + AnhDaDung.Count);
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
            b.Append(Para("center", "BÀI THỰC HÀNH SỐ 06", true, 14, 120));
            b.Append(Para("center", "DỮ LIỆU VỚI DATAGRIDVIEW VÀ MÔ HÌNH 3 LỚP", true, 15, 320));
            b.Append(Para("center", "Sinh viên thực hiện: Nguyễn Trường Giang", false, 13, 120));
            b.Append(Para("center", "Mã số sinh viên: 24000407", false, 13, 0));
            b.Append(Para("center", "Giảng viên hướng dẫn: Khoa Công nghệ Thông tin", false, 13, 0));
            b.Append(Para("center", "Năm học 2025 - 2026", false, 13, 320));
            b.Append(Para("center", "Dự án: Lab06_ThreeLayer (.NET Framework 4.8 - Windows Forms)", false, 12, 0));
            b.Append(Para("center", "Chạy thử: Lab06_ThreeLayer.exe --screenshots", false, 12, 0));
        }

        private static void TaoMucLuc(StringBuilder b)
        {
            b.Append(ParaPageBreak("center", "MỤC LỤC", true, 16, 240));
            string[] muc = new string[]
            {
                "1. Mục tiêu cần đạt được",
                "2. Công cụ và dữ liệu chuẩn bị",
                "3. Liên kết trong chuỗi bài lab",
                "4. Kiến trúc 3 lớp và cấu trúc mã nguồn",
                "5. Mười bài thực hành",
                "   5.1. Bài 1: Tổ chức project theo mô hình 3 lớp",
                "   5.2. Bài 2: Hiển thị dữ liệu bằng DataGridView và BindingSource",
                "   5.3. Bài 3: Thêm, sửa, xóa với kiểm tra dữ liệu ở tầng BUS",
                "   5.4. Bài 4: TreeView khoa - sinh viên",
                "   5.5. Bài 5: Tìm kiếm và sắp xếp",
                "   5.6. Bài 6: Dashboard và tổng giá trị hàng tồn",
                "   5.7. Bài 7: TreeView danh mục - sản phẩm và ContextMenuStrip",
                "   5.8. Bài 8: ListView và ImageList",
                "   5.9. Bài 9: Nhập và xuất CSV",
                "   5.10. Bài 10: Thống kê tổng hợp và xác nhận thoát",
                "6. Kết quả build và kiểm thử",
                "7. Kết luận và đánh giá"
            };
            foreach (string m in muc)
            {
                b.Append(Para("left", m, m.StartsWith("   "), 13, 60));
            }
        }

        private static void TaoNoiDung(StringBuilder b, string baoCao)
        {
            // ---------- 1. Mục tiêu ----------
            b.Append(TieuDe1("1. MỤC TIÊU CẦN ĐẠT ĐƯỢC"));
            b.Append(Bullet("Tổ chức ứng dụng Windows Forms theo mô hình 3 lớp: DTO (dữ liệu), DAL (lưu trữ), BUS (nghiệp vụ) và GUI (giao diện)."));
            b.Append(Bullet("Hiển thị và thao tác dữ liệu bằng DataGridView liên kết BindingSource."));
            b.Append(Bullet("Đặt toàn bộ quy tắc kiểm tra dữ liệu ở tầng BUS, GUI chỉ hiển thị thông báo lỗi trả về."));
            b.Append(Bullet("Trình bày dữ liệu phân cấp bằng TreeView (khoa - sinh viên, danh mục - sản phẩm) và danh sách có biểu tượng bằng ListView + ImageList."));
            b.Append(Bullet("Tính toán thống kê (điểm trung bình, tổng giá trị hàng tồn) ở tầng nghiệp vụ, không tính trong Form."));
            b.Append(Bullet("Nhập và xuất dữ liệu qua tệp CSV theo đúng luồng Form → BUS → DAL."));

            // ---------- 2. Công cụ ----------
            b.Append(TieuDe1("2. CÔNG CỤ VÀ DỮ LIỆU CHUẨN BỊ"));
            b.Append(Bullet("Môi trường: Visual Studio 2022, .NET Framework 4.8, Windows Forms, không dùng gói NuGet."));
            b.Append(Bullet("Dự án: Lap6/Lab06_ThreeLayer.csproj, kế thừa giao diện và bộ kiểm tra dữ liệu của Lab 5."));
            b.Append(Bullet("Thành phần: DataGridView, BindingSource, ErrorProvider, TreeView, ContextMenuStrip, ListView, ImageList, MenuStrip, StatusStrip."));
            b.Append(Bullet("Dữ liệu mẫu: 3 khoa (Công nghệ thông tin, Kinh tế, Điện - Điện tử), mỗi khoa 3 sinh viên; 3 danh mục với 6 sản phẩm."));
            b.Append(Bullet("Dữ liệu được nạp qua tầng BUS nên cũng phải qua kiểm tra nghiệp vụ như dữ liệu nhập tay."));

            b.Append(TieuDe3("Bảng dữ liệu sinh viên mẫu"));
            b.Append(Bang(
                new string[] { "Mã", "Họ tên", "Khoa", "Điểm" },
                new string[][]
                {
                    new string[] { "SV01", "Nguyễn Văn An", "Công nghệ thông tin", "8.7" },
                    new string[] { "SV02", "Trần Thị Bình", "Công nghệ thông tin", "7.4" },
                    new string[] { "SV03", "Lê Hoàng Cường", "Công nghệ thông tin", "6.2" },
                    new string[] { "SV04", "Phạm Thu Dung", "Kinh tế", "9.1" },
                    new string[] { "SV05", "Hoàng Minh Đức", "Kinh tế", "4.8" },
                    new string[] { "SV06", "Vũ Thị Hằng", "Kinh tế", "7.9" },
                    new string[] { "SV07", "Đặng Quốc Khánh", "Điện - Điện tử", "6.8" },
                    new string[] { "SV08", "Bùi Lan Anh", "Điện - Điện tử", "8.2" },
                    new string[] { "SV09", "Ngô Tuấn Kiệt", "Điện - Điện tử", "5.5" }
                },
                new int[] { 900, 2900, 3400, 800 },
                "Bảng 2.1: Dữ liệu sinh viên mẫu (DuLieuMau.NapSinhVien)"));

            b.Append(TieuDe3("Bảng dữ liệu sản phẩm mẫu"));
            b.Append(Bang(
                new string[] { "Mã", "Tên sản phẩm", "Danh mục", "Đơn giá", "SL tồn", "Giá trị tồn" },
                new string[][]
                {
                    new string[] { "SP01", "Laptop Dell XPS 15", "DM01", "25.000.000", "5", "125.000.000" },
                    new string[] { "SP02", "Chuột không dây Logitech", "DM01", "350.000", "42", "14.700.000" },
                    new string[] { "SP03", "Bàn phím cơ Keychron K2", "DM01", "1.850.000", "12", "22.200.000" },
                    new string[] { "SP04", "Bút bi Thiên Long TL-027", "DM02", "5.000", "300", "1.500.000" },
                    new string[] { "SP05", "Vở Campus 200 trang", "DM02", "18.000", "150", "2.700.000" },
                    new string[] { "SP06", "Nồi cơm điện Sharp 1.8L", "DM03", "1.450.000", "8", "11.600.000" }
                },
                new int[] { 800, 2600, 1000, 1400, 800, 1500 },
                "Bảng 2.2: Sáu sản phẩm thuộc ba danh mục (đơn vị: đồng)"));

            // ---------- 3. Liên kết ----------
            b.Append(TieuDe1("3. LIÊN KẾT TRONG CHUỖI BÀI LAB"));
            b.Append(Para("left", "Ngữ cảnh xuyên suốt: hoàn thiện luồng nhập - xem chi tiết - danh sách sinh viên. "
                + "Lab 4 dựng giao diện, Lab 5 xử lý sự kiện, kiểm tra dữ liệu và tổ chức đa Form. "
                + "Lab 6 giữ nguyên cách bố trí giao diện nhưng chuyển toàn bộ dữ liệu xuống tầng DAL, "
                + "quy tắc kiểm tra xuống tầng BUS và hiển thị danh sách bằng DataGridView.", false, 13, 120));
            b.Append(Para("left", "Nhờ vậy Lab 7 (dữ liệu với ADO.NET) chỉ cần thay phần ruột của tầng DAL, "
                + "còn giao diện và nghiệp vụ gần như không phải sửa.", false, 13, 60));

            // ---------- 4. Kiến trúc ----------
            b.Append(TieuDe1("4. KIẾN TRÚC 3 LỚP VÀ CẤU TRÚC MÃ NGUỒN"));
            b.Append(Para("left", "Dự án chia thành bốn thư mục theo trách nhiệm. Chiều phụ thuộc một chiều: "
                + "GUI → BUS → DAL → dữ liệu. GUI không tham chiếu DAL, DAL không biết BUS và GUI tồn tại.", false, 13, 120));
            b.Append(BangMa(
                "Lap6/",
                "├── Program.cs                    // Entry point, tham số --screenshots",
                "├── DTO/DoiTuong.cs               // SinhVien, Khoa, SanPham, DanhMuc, KetQuaCSV, LoiNghiepVu",
                "├── DAL/DuLieuGiaLap.cs           // SinhVienDAL, KhoaDAL, SanPhamDAL, DanhMucDAL (+ CSV)",
                "├── BUS/NghiepVu.cs               // SinhVienBUS, KhoaBUS, SanPhamBUS, DanhMucBUS, DuLieuMau",
                "├── GUI/frmMain.cs                // Bài 6: MDI + MenuStrip + StatusStrip + Dashboard",
                "├── GUI/frmSinhVien.cs            // Bài 2, 3, 5, 9: DataGridView, CRUD, lọc, CSV",
                "├── GUI/frmDanhMucSinhVien.cs     // Bài 4: TreeView khoa - sinh viên",
                "├── GUI/frmSanPham.cs             // Bài 6: tổng giá trị hàng tồn",
                "├── GUI/frmDanhMuc.cs             // Bài 7: TreeView danh mục + ContextMenuStrip",
                "├── GUI/frmSinhVienIcon.cs        // Bài 8: ListView + ImageList",
                "├── GUI/frmKienTruc.cs            // Bài 1: sơ đồ 3 lớp + chạy thử luồng gọi",
                "├── GUI/frmGioiThieu.cs           // Trợ giúp",
                "├── GUI/Helpers/                  // UiTheme, UiControls, FormBase, KiemTraChung, NhapLieu",
                "└── BaoCaoTools/TrinhChupAnh.cs   // Tự động chụp ảnh báo cáo",
                "",
                "Bảng 4.1: Cấu trúc thư mục dự án Lab06_ThreeLayer"));

            b.Append(TieuDe3("Nhiệm vụ của từng tầng"));
            b.Append(Bang(
                new string[] { "Tầng", "Lớp tiêu biểu", "Trách nhiệm" },
                new string[][]
                {
                    new string[] { "DTO", "SinhVien, SanPham", "Chỉ chứa thuộc tính + Clone(), không logic, không giao diện" },
                    new string[] { "DAL", "SinhVienDAL", "Lưu/đọc dữ liệu (List trong bộ nhớ, tệp CSV)" },
                    new string[] { "BUS", "SinhVienBUS", "Kiểm tra dữ liệu, quy tắc nghiệp vụ, điều phối DAL" },
                    new string[] { "GUI", "frmSinhVien", "Hiển thị, nhận thao tác người dùng, chỉ gọi BUS" }
                },
                new int[] { 900, 2600, 5400 },
                "Bảng 4.2: Trách nhiệm của bốn tầng"));

            // ---------- 5. Mười bài ----------
            b.Append(TieuDe1("5. MƯỜI BÀI THỰC HÀNH"));
            b.Append(Para("left", "Mỗi bài trình bày: yêu cầu, cách triển khai, đoạn mã chính và ảnh chụp kết quả chạy thực tế của ứng dụng.", false, 13, 120));

            Bai1(b, baoCao);
            Bai2(b, baoCao);
            Bai3(b, baoCao);
            Bai4(b, baoCao);
            Bai5(b, baoCao);
            Bai6(b, baoCao);
            Bai7(b, baoCao);
            Bai8(b, baoCao);
            Bai9(b, baoCao);
            Bai10(b, baoCao);

            // ---------- 6. Build & kiểm thử ----------
            b.Append(TieuLe("6. KẾT QUẢ BUILD VÀ KIỂM THỬ"));
            b.Append(Para("left", "Dự án biên dịch thành công với .NET Framework 4.8 (csc 4.8.9221 - cú pháp C# 5), "
                + "không có lỗi. Ứng dụng chạy được ở chế độ bình thường và ở chế độ tự chụp ảnh báo cáo.", false, 13, 120));
            b.Append(Para("left", "Các tình huống dữ liệu sai sau đây đã được thử và đều bị tầng BUS chặn trước khi tới DAL:", false, 13, 60));
            b.Append(Bang(
                new string[] { "Dữ liệu nhập", "Thông báo của BUS", "Kết quả trên GUI" },
                new string[][]
                {
                    new string[] { "Mã SV01 (đã tồn tại)", "Trùng mã sinh viên: SV01 đã tồn tại.", "ErrorProvider tại ô Mã SV, lưới không thêm dòng" },
                    new string[] { "Họ tên “Nguyễn 2”", "Họ tên không được chứa chữ số.", "ErrorProvider tại ô Họ và tên" },
                    new string[] { "Điểm 12", "Điểm trung bình phải thuộc [0, 10].", "ErrorProvider tại ô Điểm" },
                    new string[] { "Ngày sinh năm 2024", "Tuổi 2 không hợp lệ (yêu cầu 16 - 80).", "ErrorProvider tại ô Ngày sinh" },
                    new string[] { "Xóa danh mục còn 3 sản phẩm", "Danh mục còn sản phẩm, không thể xóa.", "Cây không đổi, thông báo hiện ở khung chi tiết" },
                    new string[] { "Nhập lại tệp CSV đã xuất", "(không có lỗi)", "9 dòng, 0 dòng lỗi, dữ liệu trùng khớp" }
                },
                new int[] { 2900, 3000, 3000 },
                "Bảng 6.1: Tổng hợp kịch bản kiểm thử"));
            b.Append(Bang(
                new string[] { "Kịch bản", "Ảnh minh hoạ", "Kết quả" },
                new string[][]
                {
                    new string[] { "Mở form sinh viên, chọn dòng SV03", "Bai06_02_DataGridView.png", "Lưới 9 dòng, ô nhập được đổ dữ liệu" },
                    new string[] { "Lưu dữ liệu sai (mã trùng, tên có số, điểm 12)", "Bai06_03_KiemTraLoi.png", "Bị chặn, không ghi dữ liệu" },
                    new string[] { "Lọc khoa Kinh tế + sắp xếp điểm", "Bai06_04_TimSapXep.png", "3 dòng, đúng thứ tự" },
                    new string[] { "Xóa danh mục còn sản phẩm", "Bai06_07_TreeDanhMuc.png", "BUS từ chối, cây giữ nguyên" },
                    new string[] { "Xuất CSV rồi nhập lại", "Bai06_09_CSV.png", "9 dòng, 0 dòng lỗi" },
                    new string[] { "Đóng frmMain, chọn No", "Bai06_12_XacNhanThoat.png", "e.Cancel = true, ứng dụng không thoát" }
                },
                new int[] { 3400, 2600, 2900 },
                "Bảng 6.2: Đối chiếu kịch bản với ảnh chụp"));
            b.Append(Para("left", "Bộ ảnh được tạo tự động bằng lệnh Lab06_ThreeLayer.exe --screenshots: mỗi kịch bản chạy trong một tiến trình riêng, "
                + "mở Form thật, gọi sự kiện thật và chụp lại giao diện. Nhật ký nằm ở BaoCao/nhat_ky_chup_anh.txt.", false, 13, 60));

            // ---------- 7. Kết luận ----------
            b.Append(TieuLe("7. KẾT LUẬN VÀ ĐÁNH GIÁ"));
            b.Append(Para("left", "Báo cáo đã hoàn thành đủ 10/10 bài tập của Lab 6: ứng dụng có 8 Form, dữ liệu được tổ chức theo mô hình 3 lớp, "
                + "hiển thị bằng DataGridView, TreeView và ListView, có nhập/xuất CSV và bộ ảnh chụp kết quả thực tế.", false, 13, 120));
            b.Append(TieuDe3("Kiến thức đã vận dụng"));
            b.Append(Bullet("Phân tầng rõ ràng: DAL chỉ lưu trữ, BUS giữ quy tắc, GUI chỉ hiển thị. Quy tắc nghiệp vụ viết một lần, dùng cho mọi Form."));
            b.Append(Bullet("LoiNghiepVu mang tên trường bị lỗi, giúp ErrorProvider báo đúng ô nhập mà không cần GUI tự kiểm tra lại."));
            b.Append(Bullet("DataGridView khai báo cột tường minh (AutoGenerateColumns = false) để lỗi đổi tên thuộc tính xuất hiện ngay khi biên dịch."));
            b.Append(Bullet("Tìm kiếm, sắp xếp, nhóm dữ liệu và tính tổng đều thực hiện ở BUS; Form chỉ gửi tiêu chí và hiển thị kết quả."));
            b.Append(Bullet("TreeView + ContextMenuStrip cho cấu trúc phân cấp; ListView + ImageList cho danh sách có biểu tượng (hình tự vẽ bằng GDI+)."));
            b.Append(Bullet("Tiêm DAL vào BUS qua constructor: các Form dùng chung một kho dữ liệu mà không cần biến tĩnh toàn cục."));
            b.Append(TieuDe3("Hạn chế và hướng phát triển"));
            b.Append(Bullet("Dữ liệu hiện lưu trong bộ nhớ, mất khi đóng ứng dụng (chỉ tệp CSV được giữ lại)."));
            b.Append(Bullet("Sang Lab 7, thay phần ruột của tầng DAL bằng ADO.NET (SqlConnection, SqlCommand, SqlDataAdapter) là có thể lưu vào SQL Server."));
            b.Append(Para("left", "Kết luận: bài thực hành đã đạt các mục tiêu - tổ chức dự án theo mô hình 3 lớp, hiển thị và thao tác dữ liệu bằng DataGridView, "
                + "kiểm tra dữ liệu tập trung ở tầng nghiệp vụ và thể hiện dữ liệu phân cấp bằng TreeView, ListView.", false, 13, 160));
        }

        // =================================================================
        // MƯỜI BÀI
        // =================================================================
        private static void Bai1(StringBuilder b, string baoCao)
        {
            b.Append(TieuDe2("5.1. Bài 1: Tổ chức project theo mô hình 3 lớp"));
            b.Append(TieuDe3("Yêu cầu"));
            b.Append(Bullet("Tách dữ liệu, truy xuất dữ liệu, nghiệp vụ và giao diện thành các thư mục/lớp riêng."));
            b.Append(Bullet("Form không được gọi trực tiếp lớp truy xuất dữ liệu."));
            b.Append(Bullet("Mọi lỗi nghiệp vụ phải được báo cho giao diện bằng một ngoại lệ có thông tin trường bị sai."));
            b.Append(TieuDe3("Cách triển khai"));
            b.Append(Bullet("Tạo bốn thư mục DTO, DAL, BUS, GUI; mỗi lớp chỉ phụ thuộc về phía sau nó."));
            b.Append(Bullet("Ngoại lệ LoiNghiepVu(Truong, ThongBao) nằm ở DTO để BUS ném và GUI bắt."));
            b.Append(Bullet("frmMain khởi tạo DAL một lần, tiêm vào BUS rồi truyền BUS xuống các Form con qua constructor."));
            b.Append(TieuDe3("Mã nguồn chính"));
            b.Append(BangMa(
                "// GUI/frmMain.cs",
                "private readonly SinhVienDAL _dalSV = new SinhVienDAL();",
                "private readonly SinhVienBUS _busSV;",
                "",
                "public frmMain()",
                "{",
                "    InitializeComponent();",
                "",
                "    // Tiêm DAL vào BUS: cùng một DAL => cùng một kho dữ liệu",
                "    _busSV = new SinhVienBUS(_dalSV);",
                "",
                "    // Dữ liệu mẫu cũng phải qua kiểm tra nghiệp vụ của BUS",
                "    DuLieuMau.NapSinhVien(_busSV);",
                "}",
                "",
                "// BUS/NghiepVu.cs",
                "public void Add(DTO.SinhVien sv)",
                "{",
                "    KiemTraDuLieu(sv, false);   // nghiệp vụ ở đây",
                "    _dal.Add(sv);               // chỉ khi hợp lệ mới ghi",
                "}",
                "",
                "// DTO/DoiTuong.cs",
                "public class LoiNghiepVu : Exception",
                "{",
                "    public string Truong { get; private set; }",
                "}",
                "",
                "Bảng 5.1: Khởi tạo ba tầng và luồng gọi trong nghiệp vụ"));
            b.Append(Para("left", "Form Kiến trúc 3 lớp (menu Trợ giúp) vẽ sơ đồ GUI → BUS → DAL và có nút “Chạy thử luồng”: "
                + "thử thêm bản ghi có họ tên chứa chữ số, BUS ném LoiNghiepVu(\"HoTen\", …) và DAL không hề nhận bản ghi đó.", false, 13, 120));
            HinhAnh(b, baoCao, "Bai06_10_KienTruc.png", "Hình 5.1: Sơ đồ 3 lớp và kết quả chạy thử - BUS chặn dữ liệu sai trước khi tới DAL");
        }

        private static void Bai2(StringBuilder b, string baoCao)
        {
            b.Append(TieuDe2("5.2. Bài 2: Hiển thị dữ liệu bằng DataGridView và BindingSource"));
            b.Append(TieuDe3("Yêu cầu"));
            b.Append(Bullet("Hiển thị danh sách sinh viên dạng bảng, có tiêu đề cột tiếng Việt."));
            b.Append(Bullet("Dữ liệu lấy từ tầng BUS, không lấy trực tiếp từ DAL."));
            b.Append(TieuDe3("Cách triển khai"));
            b.Append(Bullet("Khai báo cột tường minh (AutoGenerateColumns = false) và gắn DataPropertyName theo thuộc tính của DTO."));
            b.Append(Bullet("BindingSource là cầu nối giữa danh sách do BUS trả về và DataGridView."));
            b.Append(Bullet("Chọn một dòng sẽ đổ dữ liệu lên các ô nhập bên trái để sửa."));
            b.Append(TieuDe3("Mã nguồn chính"));
            b.Append(BangMa(
                "// GUI/frmSinhVien.cs",
                "private void NapDuLieu()",
                "{",
                "    string tuKhoa = UiFactory.Val(txtTimKiem);",
                "    string khoa   = cboKhoaFilter.SelectedIndex <= 0 ? \"\" : cboKhoaFilter.Text;",
                "",
                "    List<SinhVien> ds = _bus.TimKiem(tuKhoa, khoa);  // Form chỉ gọi BUS",
                "    if (!string.IsNullOrEmpty(luoiXepTheo))",
                "        ds = _bus.SapXep(ds, luoiXepTheo, luoiGiam);",
                "",
                "    _nguon.DataSource = ds;",
                "}",
                "",
                "// GUI/frmSinhVien.Designer.cs",
                "dgvSinhVien.AutoGenerateColumns = false;",
                "ThemCot(\"HoTen\",  \"Họ và tên\", 130);",
                "ThemCot(\"DiemTB\", \"Điểm\",       45);",
                "dgvSinhVien.DataSource = bindingSource;",
                "",
                "Bảng 5.2: Nạp dữ liệu qua BUS vào DataGridView"));
            HinhAnh(b, baoCao, "Bai06_02_DataGridView.png", "Hình 5.2: DataGridView liên kết BindingSource - 9 sinh viên, dòng SV03 đang chọn được đổ lên ô nhập");
        }

        private static void Bai3(StringBuilder b, string baoCao)
        {
            b.Append(TieuDe2("5.3. Bài 3: Thêm, sửa, xóa với kiểm tra dữ liệu ở tầng BUS"));
            b.Append(TieuDe3("Yêu cầu"));
            b.Append(Bullet("Thêm sinh viên mới, sửa và xóa sinh viên đang chọn."));
            b.Append(Bullet("Không lưu dữ liệu sai: mã trùng, họ tên có chữ số, điểm ngoài 0 - 10, tuổi ngoài 16 - 80, khoa không có trong danh mục."));
            b.Append(Bullet("Thông báo lỗi hiển thị đúng ô nhập bằng ErrorProvider."));
            b.Append(TieuDe3("Cách triển khai"));
            b.Append(Bullet("BUS kiểm tra toàn bộ dữ liệu và ném LoiNghiepVu(truong, thongBao) ở lỗi đầu tiên."));
            b.Append(Bullet("GUI bắt LoiNghiepVu, dùng chính tên trường để chọn control và gọi ErrorProvider.SetError."));
            b.Append(Bullet("Sau khi xóa phải nạp lại lưới để phản ánh kho dữ liệu mới."));
            b.Append(TieuDe3("Mã nguồn chính"));
            b.Append(BangMa(
                "// BUS: quy tắc nằm ở đây, không ở Form",
                "if (sv.HoTen.Any(char.IsDigit))",
                "    throw new LoiNghiepVu(\"HoTen\", \"Họ tên không được chứa chữ số.\");",
                "if (sv.DiemTB < 0 || sv.DiemTB > 10)",
                "    throw new LoiNghiepVu(\"DiemTB\", \"Điểm trung bình phải thuộc [0, 10].\");",
                "",
                "// GUI: chỉ hiển thị lại thông báo của BUS",
                "try",
                "{",
                "    if (laCapNhat) _bus.Update(sv);",
                "    else           _bus.Add(sv);",
                "    NapDuLieu();",
                "}",
                "catch (LoiNghiepVu ex)",
                "{",
                "    Control o = TimTheoTruong(ex.Truong);",
                "    if (o != null) { _loi.SetError(o, ex.Message); o.Focus(); }",
                "}",
                "",
                "Bảng 5.3: BUS kiểm tra, GUI chỉ hiển thị lỗi"));
            HinhAnh(b, baoCao, "Bai06_03_KiemTraLoi.png", "Hình 5.3: Lưu dữ liệu sai - BUS từ chối, ErrorProvider báo đúng ô, lưới không thêm dòng");
        }

        private static void Bai4(StringBuilder b, string baoCao)
        {
            b.Append(TieuDe2("5.4. Bài 4: TreeView khoa - sinh viên"));
            b.Append(TieuDe3("Yêu cầu"));
            b.Append(Bullet("Hiển thị danh sách sinh viên nhóm theo khoa dưới dạng cây hai cấp."));
            b.Append(Bullet("Chọn một sinh viên để xem chi tiết (điểm, xếp loại, tuổi, email)."));
            b.Append(TieuDe3("Cách triển khai"));
            b.Append(Bullet("BUS.NhomTheoKhoa() trả về Dictionary&lt;khoa, danh sách sinh viên&gt;, Form chỉ vẽ nút cây."));
            b.Append(Bullet("Mỗi nút mang Tag là đối tượng tương ứng (Khoa hoặc SinhVien) để xử lý sự kiện AfterSelect."));
            b.Append(Bullet("Sinh viên dưới 5.0 điểm được tô đỏ ngay trên cây để dễ nhận biết."));
            b.Append(TieuDe3("Mã nguồn chính"));
            b.Append(BangMa(
                "// BUS/NghiepVu.cs",
                "public Dictionary<string, List<DTO.SinhVien>> NhomTheoKhoa()",
                "{",
                "    var kq = new Dictionary<string, List<DTO.SinhVien>>();",
                "    foreach (DTO.SinhVien sv in _dal.GetAll())",
                "    {",
                "        string khoa = string.IsNullOrEmpty(sv.Khoa) ? \"(Chưa xếp khoa)\" : sv.Khoa;",
                "        if (!kq.ContainsKey(khoa)) kq[khoa] = new List<DTO.SinhVien>();",
                "        kq[khoa].Add(sv);",
                "    }",
                "    return kq;",
                "}",
                "",
                "// GUI/frmDanhMucSinhVien.cs",
                "TreeNode nutKhoa = new TreeNode(string.Format(\"{0}  ({1} sinh viên · ĐTB {2:0.00})\",",
                "    cap.Key, ds.Count, diemTB));",
                "nutKhoa.Tag = cap.Key;",
                "tvKhoa.Nodes.Add(nutKhoa);",
                "",
                "Bảng 5.4: Nhóm dữ liệu ở BUS, vẽ cây ở GUI"));
            HinhAnh(b, baoCao, "Bai06_05_TreeKhoa.png", "Hình 5.4: TreeView khoa - sinh viên, chọn một sinh viên để xem chi tiết");
        }

        private static void Bai5(StringBuilder b, string baoCao)
        {
            b.Append(TieuDe2("5.5. Bài 5: Tìm kiếm và sắp xếp"));
            b.Append(TieuDe3("Yêu cầu"));
            b.Append(Bullet("Tìm theo mã, họ tên hoặc khoa; kết hợp với lọc theo khoa."));
            b.Append(Bullet("Sắp xếp theo mã, họ tên, khoa hoặc điểm; bấm lại tiêu đề cột để đổi chiều."));
            b.Append(Bullet("Thao tác tìm và sắp xếp không được làm thay đổi dữ liệu gốc."));
            b.Append(TieuDe3("Cách triển khai"));
            b.Append(Bullet("BUS.TimKiem và BUS.SapXep trả về danh sách mới (LINQ), dữ liệu trong DAL giữ nguyên."));
            b.Append(Bullet("Sự kiện TextChanged của ô tìm và SelectedIndexChanged của ComboBox khoa cùng gọi NapDuLieu()."));
            b.Append(Bullet("Form chỉ nhớ tiêu chí sắp xếp (tên cột + chiều), việc sắp xếp do BUS quyết định."));
            b.Append(TieuDe3("Mã nguồn chính"));
            b.Append(BangMa(
                "// BUS/NghiepVu.cs",
                "public List<DTO.SinhVien> TimKiem(string tuKhoa, string khoa)",
                "{",
                "    string k = (tuKhoa ?? \"\").Trim().ToLowerInvariant();",
                "    return _dal.GetAll().Where(sv =>",
                "            (k.Length == 0 || sv.MaSV.ToLowerInvariant().Contains(k)",
                "                           || sv.HoTen.ToLowerInvariant().Contains(k))",
                "            && (string.IsNullOrEmpty(khoa) || sv.Khoa == khoa))",
                "        .ToList();",
                "}",
                "",
                "public List<DTO.SinhVien> SapXep(IEnumerable<DTO.SinhVien> nguon,",
                "                                 string tieuChi, bool giamDan)",
                "{",
                "    switch ((tieuChi ?? \"\").ToLowerInvariant())",
                "    {",
                "        case \"ten\":  return giamDan ? nguon.OrderByDescending(s => s.HoTen).ToList()",
                "                                    : nguon.OrderBy(s => s.HoTen).ToList();",
                "        case \"diem\": return giamDan ? nguon.OrderByDescending(s => s.DiemTB).ToList()",
                "                                    : nguon.OrderBy(s => s.DiemTB).ToList();",
                "        default:      return nguon.OrderBy(s => s.MaSV).ToList();",
                "    }",
                "}",
                "",
                "Bảng 5.5: Tìm kiếm và sắp xếp ở tầng BUS"));
            HinhAnh(b, baoCao, "Bai06_04_TimSapXep.png", "Hình 5.5: Lọc khoa Kinh tế và sắp xếp theo điểm - kết quả do BUS trả về");
        }

        private static void Bai6(StringBuilder b, string baoCao)
        {
            b.Append(TieuDe2("5.6. Bài 6: Dashboard và tổng giá trị hàng tồn"));
            b.Append(TieuDe3("Yêu cầu"));
            b.Append(Bullet("Form chính hiển thị các chỉ số tổng hợp: số sinh viên, điểm trung bình, số sản phẩm, tổng giá trị hàng tồn."));
            b.Append(Bullet("Tổng giá trị hàng tồn = Σ(đơn giá × số lượng tồn), phải tính ở tầng nghiệp vụ."));
            b.Append(TieuDe3("Cách triển khai"));
            b.Append(Bullet("SanPhamBUS.TongGiaTriTon(IEnumerable&lt;SanPham&gt;) dùng LINQ Sum; Form chỉ hiển thị kết quả."));
            b.Append(Bullet("Mỗi Form con đóng lại đều yêu cầu Dashboard cập nhật lại số liệu."));
            b.Append(Bullet("Nút “Kiểm tra lại tổng” tính thủ công trên lưới rồi so sánh với BUS để chứng minh công thức duy nhất nằm ở BUS."));
            b.Append(TieuDe3("Mã nguồn chính"));
            b.Append(BangMa(
                "// BUS/NghiepVu.cs",
                "public double TongGiaTriTon(IEnumerable<DTO.SanPham> ds)",
                "{",
                "    if (ds == null) return 0;",
                "    return ds.Sum(x => x.DonGia * x.SoLuongTon);",
                "}",
                "",
                "// GUI/frmSanPham.cs",
                "private void CapNhatTong()",
                "{",
                "    List<SanPham> ds = _bus.GetAll();",
                "    double tong = _bus.TongGiaTriTon(ds);",
                "    lblTongGiaTri.Text = DinhDang.Tien(tong);",
                "}",
                "",
                "Bảng 5.6: Tổng hàng tồn tính ở BUS"));
            b.Append(Para("left", "Với dữ liệu mẫu: 25.000.000×5 + 350.000×42 + 1.850.000×12 + 5.000×300 + 18.000×150 + 1.450.000×8 "
                + "= 163.500.000 đồng, khớp với kết quả hiển thị trên Form.", false, 13, 120));
            HinhAnh(b, baoCao, "Bai06_06_SanPham.png", "Hình 5.6: Danh sách sản phẩm và tổng giá trị hàng tồn do BUS tính");
            HinhAnh(b, baoCao, "Bai06_01_frmMain.png", "Hình 5.7: Form chính - menu, StatusStrip và bốn phiếu chỉ số tổng hợp");
        }

        private static void Bai7(StringBuilder b, string baoCao)
        {
            b.Append(TieuDe2("5.7. Bài 7: TreeView danh mục - sản phẩm và ContextMenuStrip"));
            b.Append(TieuDe3("Yêu cầu"));
            b.Append(Bullet("Hiển thị danh mục cấp 1 và sản phẩm cấp 2; mỗi danh mục có số lượng sản phẩm."));
            b.Append(Bullet("Chuột phải mở menu ngữ cảnh để thêm, đổi tên, xóa danh mục."));
            b.Append(Bullet("Không cho xóa danh mục còn sản phẩm."));
            b.Append(TieuDe3("Cách triển khai"));
            b.Append(Bullet("DanhMucBUS.XoaDanhMuc kiểm tra số sản phẩm trước khi gọi DAL, ném LoiNghiepVu nếu còn."));
            b.Append(Bullet("Thêm và đổi tên dùng hộp thoại nhập liệu NhapLieu.HoiChuoi (tự viết, không phụ thuộc Microsoft.VisualBasic)."));
            b.Append(Bullet("Sau mỗi thao tác thành công, cây được dựng lại từ BUS nên luôn phản ánh dữ liệu thật."));
            b.Append(TieuDe3("Mã nguồn chính"));
            b.Append(BangMa(
                "// BUS/NghiepVu.cs",
                "public void XoaDanhMuc(string ma)",
                "{",
                "    if (SanPhamCua(ma).Count > 0)",
                "        throw new LoiNghiepVu(\"DanhMuc\", \"Danh mục còn sản phẩm, không thể xóa.\");",
                "    if (!_dal.Delete(ma))",
                "        throw new LoiNghiepVu(\"DanhMuc\", \"Không tìm thấy danh mục \" + ma + \".\");",
                "}",
                "",
                "// GUI/frmDanhMuc.cs",
                "try { _bus.XoaDanhMuc(dm.MaDanhMuc); NapCay(); }",
                "catch (LoiNghiepVu ex) { BaoLoi(ex.Message); }",
                "",
                "// Menu ngữ cảnh gắn cho cây",
                "menuNgang.Items.AddRange(new ToolStripItem[] {",
                "    mnuThemDanhMuc, mnuDoiTen, new ToolStripSeparator(), mnuXoa });",
                "tvDanhMuc.ContextMenuStrip = menuNgang;",
                "",
                "Bảng 5.7: Quy tắc xóa danh mục nằm ở BUS"));
            HinhAnh(b, baoCao, "Bai06_07_TreeDanhMuc.png", "Hình 5.8: TreeView danh mục - sản phẩm và thông báo từ chối xóa danh mục còn hàng");
        }

        private static void Bai8(StringBuilder b, string baoCao)
        {
            b.Append(TieuDe2("5.8. Bài 8: ListView và ImageList"));
            b.Append(TieuDe3("Yêu cầu"));
            b.Append(Bullet("Hiển thị danh sách sinh viên ở dạng biểu tượng, có phân loại theo học lực."));
            b.Append(Bullet("Chuyển đổi giữa ba chế độ xem: LargeIcon, SmallIcon và Details."));
            b.Append(TieuDe3("Cách triển khai"));
            b.Append(Bullet("ImageList chứa ba ảnh 40×40 tự vẽ bằng GDI+: ★ (≥ 8.0), ✓ (5.0 - 7.9), ! (< 5.0)."));
            b.Append(Bullet("Mỗi ListViewItem có ImageKey tương ứng học lực và Tag là đối tượng SinhVien."));
            b.Append(Bullet("Chọn một dòng sẽ hiển thị ảnh đại diện (chữ cái đầu của tên) và thông tin chi tiết."));
            b.Append(TieuDe3("Mã nguồn chính"));
            b.Append(BangMa(
                "// GUI/frmSinhVienIcon.cs",
                "private static Bitmap VeBieuTuong(Color mau, string kyHieu)",
                "{",
                "    var bmp = new Bitmap(40, 40);",
                "    using (Graphics g = Graphics.FromImage(bmp))",
                "    {",
                "        UiTheme.SmoothingOn(g);",
                "        g.Clear(Color.Transparent);",
                "        using (var brush = new SolidBrush(mau)) g.FillEllipse(brush, 1, 1, 38, 38);",
                "        using (var font = UiTheme.FontBold(14f))",
                "        using (var b = new SolidBrush(Color.White))",
                "        using (var fmt = new StringFormat {",
                "                   Alignment = StringAlignment.Center,",
                "                   LineAlignment = StringAlignment.Center })",
                "            g.DrawString(kyHieu, font, b, new RectangleF(0, 0, 40, 40), fmt);",
                "    }",
                "    return bmp;",
                "}",
                "",
                "// ImageList + ListView",
                "_bieuTuong.Images.Add(\"gioi\", VeBieuTuong(UiTheme.Green, \"★\"));",
                "lvSinhVien.LargeImageList = _bieuTuong;",
                "item.ImageKey = KhoaBieuTuong(sv.DiemTB);",
                "",
                "Bảng 5.8: Biểu tượng tự vẽ và gắn vào ListView"));
            HinhAnh(b, baoCao, "Bai06_08_ListView.png", "Hình 5.9: ListView dạng biểu tượng - ba nhóm học lực với biểu tượng tự vẽ");
        }

        private static void Bai9(StringBuilder b, string baoCao)
        {
            b.Append(TieuDe2("5.9. Bài 9: Nhập và xuất CSV"));
            b.Append(TieuDe3("Yêu cầu"));
            b.Append(Bullet("Xuất toàn bộ danh sách sinh viên ra tệp CSV mở được bằng Excel (tiếng Việt không lỗi font)."));
            b.Append(Bullet("Nhập lại từ tệp CSV, bỏ qua dòng sai định dạng và báo số dòng lỗi."));
            b.Append(TieuDe3("Cách triển khai"));
            b.Append(Bullet("Form gọi BUS.LuuCSV/DocCSV; BUS kiểm tra đường dẫn rồi gọi DAL - đúng đường đi của mô hình 3 lớp."));
            b.Append(Bullet("DAL ghi UTF-8 có BOM, bọc ngoặc kép cho ô chứa dấu phẩy hoặc dấu nháy."));
            b.Append(Bullet("Đọc CSV tự tách trường có ngoặc kép và đếm số dòng lỗi thay vì làm sập ứng dụng."));
            b.Append(TieuDe3("Mã nguồn chính"));
            b.Append(BangMa(
                "// DAL/DuLieuGiaLap.cs",
                "public int LuuCSV(string duongDan)",
                "{",
                "    var sb = new StringBuilder();",
                "    sb.AppendLine(HeaderCSV);",
                "    foreach (DTO.SinhVien sv in _data)",
                "    {",
                "        sb.AppendLine(string.Join(\",\", new[] {",
                "            ChuanHoaCSV(sv.MaSV), ChuanHoaCSV(sv.HoTen),",
                "            sv.NgaySinh.ToString(\"yyyy-MM-dd\"),",
                "            ChuanHoaCSV(sv.Khoa),",
                "            sv.DiemTB.ToString(\"0.0\", CultureInfo.InvariantCulture) }));",
                "    }",
                "    File.WriteAllText(duongDan, sb.ToString(), new UTF8Encoding(true)); // có BOM",
                "    return _data.Count;",
                "}",
                "",
                "// GUI/frmSinhVien.cs - chỉ gọi BUS",
                "int so = _bus.LuuCSV(hopThoai.FileName);",
                "int soDongLoi;",
                "int soDong = _bus.DocCSV(hopThoai.FileName, out soDongLoi);",
                "",
                "Bảng 5.9: Xuất/nhập CSV qua BUS đến DAL"));
            HinhAnh(b, baoCao, "Bai06_09_CSV.png", "Hình 5.10: Xuất CSV thành công - BUS chuyển yêu cầu cho DAL ghi tệp");
        }

        private static void Bai10(StringBuilder b, string baoCao)
        {
            b.Append(TieuDe2("5.10. Bài 10: Thống kê tổng hợp và xác nhận thoát"));
            b.Append(TieuDe3("Yêu cầu"));
            b.Append(Bullet("StatusStrip hiển thị trạng thái thao tác, số form đã mở và đồng hồ hệ thống."));
            b.Append(Bullet("Menu “Cửa sổ” liệt kê các form con đang mở để chuyển nhanh."));
            b.Append(Bullet("Hỏi lại khi người dùng đóng ứng dụng; chọn No thì không thoát."));
            b.Append(TieuDe3("Cách triển khai"));
            b.Append(Bullet("Form con là MDI, không mở trùng loại Form; mỗi Form con gọi Log() để cập nhật StatusStrip."));
            b.Append(Bullet("Thống kê trên Dashboard lấy từ BUS, không đếm trên lưới."));
            b.Append(Bullet("FormClosing kiểm tra CloseReason, dùng e.Cancel = true khi người dùng chọn No."));
            b.Append(TieuDe3("Mã nguồn chính"));
            b.Append(BangMa(
                "private void frmMain_FormClosing(object sender, FormClosingEventArgs e)",
                "{",
                "    if (e.CloseReason == CloseReason.UserClosing)",
                "    {",
                "        DialogResult r = MessageBox.Show(this,",
                "            \"Bạn có chắc chắn muốn thoát ứng dụng không?\",",
                "            \"Xác nhận\", MessageBoxButtons.YesNo, MessageBoxIcon.Question,",
                "            MessageBoxDefaultButton.Button2);",
                "",
                "        if (r != DialogResult.Yes)",
                "        {",
                "            e.Cancel = true;   // hủy thao tác đóng",
                "            Log(\"Người dùng chọn No -> e.Cancel = true\");",
                "            return;",
                "        }",
                "    }",
                "    Log(\"Thoát ứng dụng\");",
                "}",
                "",
                "Bảng 5.10: Xác nhận thoát ứng dụng bằng e.Cancel"));
            HinhAnh(b, baoCao, "Bai06_11_GioiThieu.png", "Hình 5.11: Form trợ giúp - mục tiêu 10 bài, bản đồ form, phím tắt, cấu trúc thư mục");
            HopThoaiXacNhan(b, "Bạn có chắc chắn muốn thoát ứng dụng không?",
                "Hình 5.12: Hộp thoại xác nhận thoát - chọn No thì e.Cancel = true, ứng dụng không đóng");
        }

        // =================================================================
        // HẠNG MỤC ĐỒ HOẠ
        // =================================================================
        private static void HinhAnh(StringBuilder b, string baoCao, string tenFile, string chuThich)
        {
            string path = Path.Combine(baoCao, tenFile);
            if (!File.Exists(path))
            {
                b.Append(Para("left", "[Thiếu ảnh: " + tenFile + "]", false, 12, 120));
                return;
            }

            int w;
            int h;
            using (var img = Image.FromFile(path))
            {
                w = img.Width;
                h = img.Height;
            }

            double widthInch = Math.Min(MaxWidthInch, (double)w / 96.0);
            long cx = (long)(widthInch * EmuPerInch);
            long cy = (long)(cx * (double)h / w);

            int id = AnhDaDung.Count + 1;
            string rid = "rIdImg" + id;
            string tenTrongGoi = "image" + id + Path.GetExtension(tenFile);
            AnhDaDung.Add(tenTrongGoi);

            string folder = Path.Combine(Path.GetDirectoryName(baoCao), "..", "scratch", "docx_build6", "word", "media");
            folder = Path.GetFullPath(folder);
            Directory.CreateDirectory(folder);
            File.Copy(path, Path.Combine(folder, tenTrongGoi), true);

            b.Append("<w:p><w:pPr><w:spacing w:before=\"120\" w:after=\"40\"/><w:jc w:val=\"center\"/></w:pPr><w:r><w:drawing>"
                + "<wp:inline><wp:extent cx=\"" + cx + "\" cy=\"" + cy + "\"/><wp:docPr id=\"" + id + "\" name=\"Picture " + id + "\"/>"
                + "<wp:cNvGraphicFramePr><a:graphicFrameLocks noChangeAspect=\"1\"/></wp:cNvGraphicFramePr>"
                + "<a:graphic><a:graphicData uri=\"http://schemas.openxmlformats.org/drawingml/2006/picture\"><pic:pic>"
                + "<pic:nvPicPr><pic:cNvPr id=\"" + id + "\" name=\"" + Esc(tenTrongGoi) + "\"/><pic:cNvPicPr/></pic:nvPicPr>"
                + "<pic:blipFill><a:blip r:embed=\"" + rid + "\"/><a:stretch><a:fillRect/></a:stretch></pic:blipFill>"
                + "<pic:spPr><a:xfrm><a:off x=\"0\" y=\"0\"/><a:ext cx=\"" + cx + "\" cy=\"" + cy + "\"/></a:xfrm><a:prstGeom prst=\"rect\"/></pic:spPr>"
                + "</pic:pic></a:graphicData></a:graphic></wp:inline></w:drawing></w:r></w:p>");
            b.Append("<w:p><w:pPr><w:spacing w:after=\"200\"/><w:jc w:val=\"center\"/></w:pPr>" + Run(chuThich, false, true, 11, false) + "</w:p>");
        }

        /// <summary>Dựng lại hộp thoại xác nhận (MessageBox hệ thống không chụp được tự động).</summary>
        private static void HopThoaiXacNhan(StringBuilder b, string noiDung, string chuThich)
        {
            b.Append("<w:tbl><w:tblPr><w:tblW w:w=\"6000\" w:type=\"dxa\"/><w:jc w:val=\"center\"/><w:tblBorders>"
                + "<w:top w:val=\"single\" w:sz=\"8\" w:color=\"5B6B8C\"/><w:left w:val=\"single\" w:sz=\"8\" w:color=\"5B6B8C\"/>"
                + "<w:bottom w:val=\"single\" w:sz=\"8\" w:color=\"5B6B8C\"/><w:right w:val=\"single\" w:sz=\"8\" w:color=\"5B6B8C\"/>"
                + "</w:tblBorders><w:tblCellMar><w:top w:w=\"180\" w:type=\"dxa\"/><w:left w:w=\"240\" w:type=\"dxa\"/>"
                + "<w:bottom w:w=\"180\" w:type=\"dxa\"/><w:right w:w=\"240\" w:type=\"dxa\"/></w:tblCellMar></w:tblPr>"
                + "<w:tblGrid><w:gridCol w:w=\"6000\"/></w:tblGrid>"
                + "<w:tr><w:tc><w:tcPr><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"F3F6FB\"/></w:tcPr>"
                + "<w:p><w:pPr><w:spacing w:after=\"120\"/></w:pPr>" + Run("Xác nhận", true, false, 12, false) + "</w:p>"
                + "<w:p><w:pPr><w:spacing w:after=\"200\"/></w:pPr>" + Run(noiDung, false, false, 12, false) + "</w:p>"
                + "<w:p><w:pPr><w:jc w:val=\"right\"/><w:spacing w:after=\"0\"/></w:pPr>"
                + Run("  Yes  ", false, false, 12, false) + Run("      ", false, false, 12, false) + Run("  No  ", true, false, 12, false)
                + "</w:p></w:tc></w:tr></w:tbl>"
                + "<w:p><w:pPr><w:spacing w:before=\"60\" w:after=\"200\"/><w:jc w:val=\"center\"/></w:pPr>" + Run(chuThich, false, true, 11, false) + "</w:p>");
        }

        // =================================================================
        // ĐÓNG GÓI OPC (tên entry dùng dấu '/')
        // =================================================================
        private static void DongGoi(string thuMuc, string outFile)
        {
            var ds = new List<string>();
            Colect(thuMuc, thuMuc, ds);

            // [Content_Types].xml phải đứng đầu gói
            ds.Sort(delegate (string a, string b)
            {
                if (a == "[Content_Types].xml") return -1;
                if (b == "[Content_Types].xml") return 1;
                return string.CompareOrdinal(a, b);
            });

            using (MemoryStream ms = new MemoryStream())
            {
                using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
                {
                    foreach (string rel in ds)
                    {
                        string ten = rel.Replace('\\', '/');
                        var entry = zip.CreateEntry(ten, CompressionLevel.Optimal);
                        byte[] bytes = File.ReadAllBytes(Path.Combine(thuMuc, rel));
                        using (Stream s = entry.Open()) s.Write(bytes, 0, bytes.Length);
                    }
                }
                File.WriteAllBytes(outFile, ms.ToArray());
            }
        }

        private static void Colect(string goc, string thuMuc, List<string> kq)
        {
            foreach (string f in Directory.GetFiles(thuMuc))
                kq.Add(f.Substring(goc.Length + 1));
            foreach (string d in Directory.GetDirectories(thuMuc))
                Colect(goc, d, kq);
        }

        // =================================================================
        // PHẦN XML TĨNH
        // =================================================================
        private static string Esc(string s)
        {
            if (s == null) return string.Empty;
            return s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;")
                    .Replace("\"", "&quot;");
        }

        private static string Run(string text, bool bold, bool italic, double size, bool mono)
        {
            string rpr = "<w:rPr>"
                + (mono ? "<w:rFonts w:ascii=\"Courier New\" w:hAnsi=\"Courier New\" w:cs=\"Courier New\"/>" : "<w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\"/>")
                + (bold ? "<w:b/>" : string.Empty)
                + (italic ? "<w:i/>" : string.Empty)
                + "<w:sz w:val=\"" + ((int)Math.Round(size * 2)) + "\"/>"
                + "<w:szCs w:val=\"" + ((int)Math.Round(size * 2)) + "\"/>"
                + "</w:rPr>";
            return "<w:r>" + rpr + "<w:t xml:space=\"preserve\">" + Esc(text) + "</w:t></w:r>";
        }

        private static string Para(string align, string text, bool bold, double size, int after)
        {
            return "<w:p><w:pPr><w:jc w:val=\"" + align + "\"/><w:spacing w:after=\"" + after + "\"/></w:pPr>"
                + Run(text, bold, false, size, false) + "</w:p>";
        }

        private static string ParaPageBreak(string align, string text, bool bold, double size, int after)
        {
            return "<w:p><w:pPr><w:pageBreakBefore/><w:jc w:val=\"" + align + "\"/><w:spacing w:after=\"" + after + "\"/></w:pPr>"
                + Run(text, bold, false, size, false) + "</w:p>";
        }

        private static string Bullet(string text)
        {
            return "<w:p><w:pPr><w:numPr><w:ilvl w:val=\"0\"/><w:numId w:val=\"1\"/></w:numPr>"
                + "<w:spacing w:after=\"80\"/><w:ind w:left=\"567\" w:hanging=\"284\"/></w:pPr>"
                + Run("•  ", false, false, 13, false) + Run(text, false, false, 13, false) + "</w:p>";
        }

        private static string TieuDe1(string text)
        {
            return "<w:p><w:pPr><w:pageBreakBefore/><w:spacing w:before=\"0\" w:after=\"200\"/></w:pPr>"
                + Run(text, true, false, 15, false) + "</w:p>";
        }

        private static string TieuDe2(string text)
        {
            return "<w:p><w:pPr><w:spacing w:before=\"260\" w:after=\"140\"/></w:pPr>"
                + Run(text, true, false, 13.5, false) + "</w:p>";
        }

        private static string TieuDe3(string text)
        {
            return "<w:p><w:pPr><w:spacing w:before=\"180\" w:after=\"90\"/></w:pPr>"
                + Run(text, true, true, 12.5, false) + "</w:p>";
        }

        private static string TieuLe(string text)
        {
            return TieuDe1(text);
        }

        /// <summary>Khối mã nguồn: bảng 1 ô nền xám, font Courier New 9pt.</summary>
        private static string BangMa(params string[] lines)
        {
            var sb = new StringBuilder();
            sb.Append("<w:tbl><w:tblPr><w:tblW w:w=\"9070\" w:type=\"dxa\"/><w:tblBorders>");
            sb.Append("<w:top w:val=\"single\" w:sz=\"4\" w:color=\"C8CCD8\"/><w:left w:val=\"single\" w:sz=\"4\" w:color=\"C8CCD8\"/>");
            sb.Append("<w:bottom w:val=\"single\" w:sz=\"4\" w:color=\"C8CCD8\"/><w:right w:val=\"single\" w:sz=\"4\" w:color=\"C8CCD8\"/>");
            sb.Append("</w:tblBorders><w:tblCellMar><w:top w:w=\"90\" w:type=\"dxa\"/><w:left w:w=\"140\" w:type=\"dxa\"/>");
            sb.Append("<w:bottom w:w=\"90\" w:type=\"dxa\"/><w:right w:w=\"140\" w:type=\"dxa\"/></w:tblCellMar></w:tblPr>");
            sb.Append("<w:tblGrid><w:gridCol w:w=\"9070\"/></w:tblGrid><w:tr><w:tc>");
            sb.Append("<w:tcPr><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"F6F7FA\"/></w:tcPr>");

            string caption = string.Empty;
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (i == lines.Length - 1 && line.StartsWith("Bảng ")) { caption = line; break; }
                sb.Append("<w:p><w:pPr><w:spacing w:after=\"0\"/></w:pPr>");
                sb.Append(Run(line.Length == 0 ? " " : line, false, false, 9, true));
                sb.Append("</w:p>");
            }
            sb.Append("</w:tc></w:tr></w:tbl>");
            sb.Append("<w:p><w:pPr><w:spacing w:before=\"60\" w:after=\"180\"/><w:jc w:val=\"center\"/></w:pPr>");
            sb.Append(Run(caption, false, true, 11, false) + "</w:p>");
            return sb.ToString();
        }

        /// <summary>Bảng dữ liệu có dòng tiêu đề tô màu.</summary>
        private static string Bang(string[] tieuDe, string[][] duLieu, int[] rong, string caption)
        {
            var sb = new StringBuilder();
            sb.Append("<w:tbl><w:tblPr><w:tblW w:w=\"9070\" w:type=\"dxa\"/><w:tblBorders>");
            sb.Append("<w:top w:val=\"single\" w:sz=\"4\" w:color=\"9AA3B5\"/><w:left w:val=\"single\" w:sz=\"4\" w:color=\"9AA3B5\"/>");
            sb.Append("<w:bottom w:val=\"single\" w:sz=\"4\" w:color=\"9AA3B5\"/><w:right w:val=\"single\" w:sz=\"4\" w:color=\"9AA3B5\"/>");
            sb.Append("<w:insideH w:val=\"single\" w:sz=\"4\" w:color=\"C8CCD8\"/><w:insideV w:val=\"single\" w:sz=\"4\" w:color=\"C8CCD8\"/>");
            sb.Append("</w:tblBorders><w:tblCellMar><w:top w:w=\"60\" w:type=\"dxa\"/><w:left w:w=\"90\" w:type=\"dxa\"/>");
            sb.Append("<w:bottom w:w=\"60\" w:type=\"dxa\"/><w:right w:w=\"90\" w:type=\"dxa\"/></w:tblCellMar></w:tblPr>");
            sb.Append("<w:tblGrid>");
            for (int i = 0; i < rong.Length; i++) sb.Append("<w:gridCol w:w=\"" + rong[i] + "\"/>");
            sb.Append("</w:tblGrid>");

            sb.Append("<w:tr><w:trPr><w:tblHeader/></w:trPr>");
            for (int i = 0; i < tieuDe.Length; i++)
            {
                sb.Append("<w:tc><w:tcPr><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"E4F0EE\"/>"
                    + "<w:tcW w:w=\"" + rong[i] + "\" w:type=\"dxa\"/></w:tcPr>"
                    + "<w:p><w:pPr><w:spacing w:after=\"0\"/></w:pPr>" + Run(tieuDe[i], true, false, 10.5, false) + "</w:p></w:tc>");
            }
            sb.Append("</w:tr>");

            foreach (string[] dong in duLieu)
            {
                sb.Append("<w:tr>");
                for (int i = 0; i < dong.Length; i++)
                {
                    sb.Append("<w:tc><w:tcPr><w:tcW w:w=\"" + rong[i] + "\" w:type=\"dxa\"/></w:tcPr>"
                        + "<w:p><w:pPr><w:spacing w:after=\"0\"/></w:pPr>" + Run(dong[i], false, false, 10.5, false) + "</w:p></w:tc>");
                }
                sb.Append("</w:tr>");
            }

            sb.Append("</w:tbl>");
            sb.Append("<w:p><w:pPr><w:spacing w:before=\"60\" w:after=\"180\"/><w:jc w:val=\"center\"/></w:pPr>");
            sb.Append(Run(caption, false, true, 11, false) + "</w:p>");
            return sb.ToString();
        }

        private static string SectPr()
        {
            return "<w:sectPr><w:headerReference w:type=\"default\" r:id=\"rIdHeader1\"/><w:footerReference w:type=\"default\" r:id=\"rIdFooter1\"/>"
                + "<w:pgSz w:w=\"" + A4W + "\" w:h=\"" + A4H + "\"/>"
                + "<w:pgMar w:top=\"1134\" w:right=\"1134\" w:bottom=\"1134\" w:left=\"1701\" w:header=\"709\" w:footer=\"709\" w:gutter=\"0\"/></w:sectPr>";
        }

        private static string ContentTypes()
        {
            return "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>"
                + "<Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\">"
                + "<Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/>"
                + "<Default Extension=\"xml\" ContentType=\"application/xml\"/>"
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
                + " xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">"
                + "<dc:title>Báo cáo thực hành Lab 6 - Dữ liệu với DataGridView và mô hình 3 lớp</dc:title>"
                + "<dc:subject>IT3212 - Lập trình .NET</dc:subject>"
                + "<dc:creator>Nguyễn Trường Giang - 24000407</dc:creator>"
                + "<cp:lastModifiedBy>24000407</cp:lastModifiedBy>"
                + "<dcterms:created xsi:type=\"dcterms:W3CDTF\">" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ") + "</dcterms:created>"
                + "<dcterms:modified xsi:type=\"dcterms:W3CDTF\">" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ") + "</dcterms:modified>"
                + "</cp:coreProperties>";
        }

        private static string AppProps()
        {
            return "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>"
                + "<Properties xmlns=\"http://schemas.openxmlformats.org/officeDocument/2006/extended-properties\""
                + " xmlns:vt=\"http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes\">"
                + "<Application>BuildDocx6</Application><Company>ĐH Công nghệ Đông Á</Company>"
                + "</Properties>";
        }

        private static string Styles()
        {
            return "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>"
                + "<w:styles xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\">"
                + "<w:docDefaults><w:rPrDefault><w:rPr>"
                + "<w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:eastAsia=\"Times New Roman\" w:cs=\"Times New Roman\"/>"
                + "<w:sz w:val=\"26\"/><w:szCs w:val=\"26\"/><w:lang w:val=\"vi-VN\"/>"
                + "</w:rPr></w:rPrDefault>"
                + "<w:pPrDefault><w:pPr><w:spacing w:after=\"120\" w:line=\"300\" w:lineRule=\"auto\"/></w:pPr></w:pPrDefault>"
                + "</w:docDefaults>"
                + "<w:style w:type=\"paragraph\" w:default=\"1\" w:styleId=\"Normal\"><w:name w:val=\"Normal\"/></w:style>"
                + "</w:styles>";
        }

        private static string Settings()
        {
            return "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>"
                + "<w:settings xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\">"
                + "<w:zoom w:percent=\"100\"/><w:defaultTabStop w:val=\"709\"/>"
                + "<w:compat><w:compatSetting w:name=\"compatibilityMode\" w:uri=\"http://schemas.microsoft.com/office/word\" w:val=\"15\"/></w:compat>"
                + "</w:settings>";
        }

        private static string Header()
        {
            return "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>"
                + "<w:hdr xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\">"
                + "<w:p><w:pPr><w:jc w:val=\"right\"/><w:pBdr><w:bottom w:val=\"single\" w:sz=\"4\" w:color=\"C8CCD8\"/></w:pBdr></w:pPr>"
                + Run("Báo cáo thực hành Lab 6 · IT3212 - Lập trình .NET · 24000407", false, true, 9.5, false)
                + "</w:p></w:hdr>";
        }

        private static string Footer()
        {
            return "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>"
                + "<w:ftr xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\">"
                + "<w:p><w:pPr><w:jc w:val=\"center\"/></w:pPr>"
                + Run("Trang ", false, false, 10, false)
                + "<w:fldSimple w:instr=\" PAGE \"><w:r><w:t>1</w:t></w:r></w:fldSimple>"
                + Run(" / ", false, false, 10, false)
                + "<w:fldSimple w:instr=\" NUMPAGES \"><w:r><w:t>1</w:t></w:r></w:fldSimple>"
                + "</w:p></w:ftr>";
        }

        private static string RelsDocument()
        {
            var sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
            sb.Append("<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">");
            sb.Append("<Relationship Id=\"rIdStyles\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles\" Target=\"styles.xml\"/>");
            sb.Append("<Relationship Id=\"rIdSettings\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/settings\" Target=\"settings.xml\"/>");
            sb.Append("<Relationship Id=\"rIdHeader1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/header\" Target=\"header1.xml\"/>");
            sb.Append("<Relationship Id=\"rIdFooter1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/footer\" Target=\"footer1.xml\"/>");
            for (int i = 0; i < AnhDaDung.Count; i++)
            {
                sb.Append("<Relationship Id=\"rIdImg" + (i + 1) + "\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/image\"");
                sb.Append(" Target=\"media/" + AnhDaDung[i] + "\"/>");
            }
            sb.Append("</Relationships>");
            return sb.ToString();
        }
    }
}
