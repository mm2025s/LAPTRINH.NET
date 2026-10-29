using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace BuildDocx7
{
    /// <summary>
    /// Sinh bao cao thuc hanh Lab 7 dang .docx (OpenXML) - chen anh chup THAT trong Lap7/BaoCao.
    ///
    /// Khong phu thuoc Microsoft Word: tu ghi cac phan XML toi thieu cua goi OPC
    /// (trang bia, muc luc, tieu de, bang ma nguon, bang du lieu, anh + chu thich).
    /// Tep duoc dong goi bang ZipArchive voi ten entry dung dau '/' dung chuan OPC.
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
            string lap = Path.Combine(root, "Lap7");
            string baoCao = Path.Combine(lap, "BaoCao");
            string outFile = Path.Combine(baoCao, "BaoCao_Lab07_24000407.docx");
            string work = Path.Combine(root, "scratch", "docx_build7");
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
        // NOI DUNG
        // =================================================================
        private static void TaoTrangBia(StringBuilder b)
        {
            b.Append(Para("center", "TRƯỜNG ĐẠI HỌC CÔNG NGHỆ ĐÔNG Á", true, 14, 40));
            b.Append(Para("center", "KHOA CÔNG NGHỆ THÔNG TIN", true, 14, 0));
            b.Append(Para("center", "•", false, 14, 0));
            b.Append(Para("center", "BÁO CÁO THỰC HÀNH", true, 16, 240));
            b.Append(Para("center", "MÔN: LẬP TRÌNH .NET (IT3212)", true, 14, 0));
            b.Append(Para("center", "BÀI THỰC HÀNH SỐ 07", true, 14, 120));
            b.Append(Para("center", "TẠO CSDL SQL SERVER, TỔNG QUAN ADO.NET", true, 15, 0));
            b.Append(Para("center", "VÀ KẾT NỐI CƠ SỞ DỮ LIỆU", true, 15, 320));
            b.Append(Para("center", "Sinh viên thực hiện: Nguyễn Trường Giang", false, 13, 120));
            b.Append(Para("center", "Mã số sinh viên: 24000407", false, 13, 0));
            b.Append(Para("center", "Giảng viên hướng dẫn: Khoa Công nghệ Thông tin", false, 13, 0));
            b.Append(Para("center", "Năm học 2025 - 2026", false, 13, 320));
            b.Append(Para("center", "Dự án: Lab07_ADONET_Connection (.NET Framework 4.8 - Windows Forms)", false, 12, 0));
            b.Append(Para("center", "Cơ sở dữ liệu: QLSinhVien (SQL Server)", false, 12, 0));
            b.Append(Para("center", "Chạy thử: Lab07_ADONET_Connection.exe --screenshots", false, 12, 0));
        }

        private static void TaoMucLuc(StringBuilder b)
        {
            b.Append(ParaPageBreak("center", "MỤC LỤC", true, 16, 240));
            string[] muc = new string[]
            {
                "1. Mục tiêu cần đạt được",
                "2. Công cụ và dữ liệu chuẩn bị",
                "3. Liên kết trong chuỗi bài lab",
                "4. Cấu trúc mã nguồn và cơ sở dữ liệu QLSinhVien",
                "5. Mười bài thực hành",
                "   5.1. Bài 1: Tạo cơ sở dữ liệu QLSinhVien",
                "   5.2. Bài 2: Tạo bảng Khoa và SinhVien (khóa chính, khóa ngoại, CHECK)",
                "   5.3. Bài 3: Tạo bảng HocPhan và DangKy (quan hệ nhiều - nhiều)",
                "   5.4. Bài 4: Thêm dữ liệu mẫu",
                "   5.5. Bài 5: Lớp kết nối và nút kiểm tra kết nối",
                "   5.6. Bài 6: Ràng buộc bổ sung và thử dữ liệu vi phạm",
                "   5.7. Bài 7: Chuỗi kết nối SQL Authentication",
                "   5.8. Bài 8: Đọc chuỗi kết nối từ App.config",
                "   5.9. Bài 9: Màn hình trạng thái kết nối",
                "   5.10. Bài 10: Sao lưu và phục hồi cơ sở dữ liệu",
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
            b.Append(Bullet("Tạo cơ sở dữ liệu quan hệ cho bài toán quản lý sinh viên."));
            b.Append(Bullet("Thiết lập khóa chính, khóa ngoại, ràng buộc và dữ liệu mẫu."));
            b.Append(Bullet("Hiểu vai trò của Connection trong ADO.NET."));
            b.Append(Bullet("Tạo lớp kết nối SQL Server và kiểm tra trạng thái kết nối."));
            b.Append(Bullet("Đọc chuỗi kết nối từ tệp cấu hình thay vì viết cứng trong mã nguồn."));

            // ---------- 2. Công cụ ----------
            b.Append(TieuDe1("2. CÔNG CỤ VÀ DỮ LIỆU CHUẨN BỊ"));
            b.Append(Bullet("SQL Server 2019/2022 hoặc SQL Server Express (máy demo dùng SQL Server Express LocalDB 2025)."));
            b.Append(Bullet("SQL Server Management Studio (SSMS) để chạy kịch bản .sql và xem dữ liệu."));
            b.Append(Bullet("Visual Studio 2022 - .NET Framework 4.8 - Windows Forms, thư viện System.Data.SqlClient."));
            b.Append(Bullet("Dự án: Lab07_ADONET_Connection; tệp kịch bản: Lab07_QLSinhVien.sql."));
            b.Append(Bullet("Cơ sở dữ liệu QLSinhVien được dùng liên tục từ Lab 7 đến Lab 16."));

            // ---------- 3. Liên kết ----------
            b.Append(TieuDe1("3. LIÊN KẾT TRONG CHUỖI BÀI LAB"));
            b.Append(Para("left", "Ngữ cảnh xuyên suốt: Lab 6 lưu dữ liệu trong bộ nhớ (DuLieuGiaLap.cs) và dữ liệu mất khi đóng ứng dụng. "
                + "Lab 7 thay phần ruột của tầng DAL bằng ADO.NET: dữ liệu được lưu thật trong SQL Server, "
                + "còn giao diện và nghiệp vụ giữ gần như nguyên vẹn.", false, 13, 120));
            b.Append(Para("left", "Cơ sở dữ liệu QLSinhVien tiếp tục được dùng cho các bài sau: Lab 8 (SqlCommand), "
                + "Lab 9 (SqlDataReader), Lab 10 (SqlDataAdapter) cho tới các bài tổng hợp ở Lab 15 - 16.", false, 13, 60));

            // ---------- 4. Cấu trúc ----------
            b.Append(TieuDe1("4. CẤU TRÚC MÃ NGUỒN VÀ CƠ SỞ DỮ LIỆU QLSinhVien"));
            b.Append(Para("left", "Dự án giữ đúng mô hình bốn tầng của Lab 6, chỉ thay tầng DAL bằng kết nối ADO.NET thật. "
                + "Chiều phụ thuộc một chiều: GUI → BUS → DAL → CSDL SQL Server.", false, 13, 120));
            b.Append(BangMa(
                "Lap7/",
                "├── Program.cs                        // Entry point, tham số --screenshots",
                "├── App.config                        // Chuỗi kết nối QLSinhVien (bài 8)",
                "├── DTO/DoiTuong.cs                   // Khoa, SinhVien, HocPhan, DangKy, TrangThaiKetNoi",
                "├── DAL/DbConnectionFactory.cs        // Tạo SqlConnection (bài 5, 7, 8)",
                "├── DAL/TruyVanDAL.cs                 // SqlDataReader / SqlDataAdapter / metadata",
                "├── BUS/NghiepVu.cs                   // KetNoiBUS, TruyVanBUS",
                "├── GUI/frmMain.cs                    // MDI + MenuStrip + StatusStrip + Dashboard",
                "├── GUI/frmKetNoi.cs                  // Bài 5, 7: nút btnKiemTra, SQL Auth",
                "├── GUI/frmTrangThai.cs               // Bài 9: trạng thái kết nối",
                "├── GUI/frmTruyVan.cs                 // Bài 5: DataGridView đọc dữ liệu mẫu",
                "├── GUI/frmSoDoQuanHe.cs              // Sơ đồ quan hệ (vẽ GDI+ từ metadata)",
                "├── GUI/Helpers/                      // UiTheme, UiControls, FormBase, DinhDang",
                "├── SQL/Lab07_QLSinhVien.sql          // Toàn bộ T-SQL của bài 1 - 4, 6, 10",
                "└── BaoCaoTools/TrinhChupAnh.cs       // Tự động chụp ảnh báo cáo",
                "",
                "Bảng 4.1: Cấu trúc thư mục dự án Lab07_ADONET_Connection"));

            b.Append(TieuDe3("Mô hình quan hệ dữ liệu"));
            b.Append(Bang(
                new string[] { "Bảng", "Khóa chính", "Khóa ngoại", "Ràng buộc chính" },
                new string[][]
                {
                    new string[] { "Khoa", "MaKhoa", "—", "UNIQUE(TenKhoa)" },
                    new string[] { "SinhVien", "MaSV", "MaKhoa → Khoa", "CHECK DiemTB ∈ [0,10]; UNIQUE(Email)" },
                    new string[] { "HocPhan", "MaHP", "—", "CHECK SoTinChi ∈ [1,6]" },
                    new string[] { "DangKy", "MaSV + MaHP", "MaSV → SinhVien; MaHP → HocPhan", "CHECK HocKy ∈ [1,3]; DEFAULT NgayDangKy" }
                },
                new int[] { 1300, 1500, 3100, 3170 },
                "Bảng 4.2: Bốn bảng của QLSinhVien và các ràng buộc"));

            // ---------- 5. Mười bài ----------
            b.Append(TieuDe1("5. MƯỜI BÀI THỰC HÀNH"));
            b.Append(Para("left", "Mỗi bài trình bày: yêu cầu, cách triển khai, đoạn mã/kịch bản chính và ảnh chụp kết quả chạy thực tế.", false, 13, 120));

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
            b.Append(Para("left", "Kịch bản Lab07_QLSinhVien.sql chạy thành công từ đầu trên SQL Server, tạo đủ 4 bảng và dữ liệu mẫu. "
                + "Dự án biên dịch thành công với .NET Framework 4.8 (csc.exe), không có lỗi. "
                + "Ứng dụng chạy được ở chế độ bình thường và ở chế độ tự chụp ảnh báo cáo.", false, 13, 120));
            b.Append(Bang(
                new string[] { "Kiểm tra", "Kết quả mong đợi", "Kết quả thực tế" },
                new string[][]
                {
                    new string[] { "Số bảng tạo được", "4 bảng", "Khoa, SinhVien, HocPhan, DangKy" },
                    new string[] { "Số bản ghi mẫu", "≥ 3 khoa, 10 SV, 5 HP", "3 khoa, 12 SV, 6 HP, 26 đăng ký" },
                    new string[] { "Chèn điểm DiemTB = 12", "Bị CHECK chặn", "Lỗi CK_SinhVien_Diem (bắt bằng TRY/CATCH)" },
                    new string[] { "Chèn Email trùng", "Bị UNIQUE chặn", "Lỗi UQ_SinhVien_Email" },
                    new string[] { "Chèn MaKhoa không tồn tại", "Bị khóa ngoại chặn", "Lỗi FK_SinhVien_Khoa" },
                    new string[] { "Chèn HocKy = 5", "Bị CHECK chặn", "Lỗi CK_DangKy_HocKy" },
                    new string[] { "Kiểm tra kết nối Windows Auth", "Kết nối thành công", "MessageBox: Kết nối thành công: QLSinhVien" },
                    new string[] { "Kiểm tra kết nối sai instance", "Báo lỗi, không sập", "SqlException được bắt, hiện thông báo lỗi" }
                },
                new int[] { 3100, 2900, 3070 },
                "Bảng 6.1: Tổng hợp kịch bản kiểm thử đã thực hiện"));

            b.Append(Para("left", "Bộ ảnh được tạo tự động bằng lệnh Lab07_ADONET_Connection.exe --screenshots: mỗi kịch bản chạy trong một tiến trình riêng, "
                + "mở Form thật, gọi sự kiện thật và chụp lại giao diện. Nhật ký nằm ở BaoCao/nhat_ky_chup_anh.txt.", false, 13, 60));

            // ---------- 7. Kết luận ----------
            b.Append(TieuLe("7. KẾT LUẬN VÀ ĐÁNH GIÁ"));
            b.Append(Para("left", "Báo cáo đã hoàn thành đủ 10/10 bài tập của Lab 7: tạo cơ sở dữ liệu SQL Server QLSinhVien với đầy đủ khóa chính, "
                + "khóa ngoại, ràng buộc và dữ liệu mẫu; xây dựng lớp kết nối ADO.NET và ứng dụng Windows Forms kiểm tra kết nối, xem dữ liệu mẫu.", false, 13, 120));
            b.Append(TieuDe3("Kiến thức đã vận dụng"));
            b.Append(Bullet("SqlConnection + chuỗi kết nối: mở/đóng kết nối đúng cách bằng using(...)."));
            b.Append(Bullet("SqlCommand, SqlDataReader để đọc dữ liệu tuần tự; SqlDataAdapter + DataTable để đổ vào DataGridView."));
            b.Append(Bullet("Ràng buộc toàn vẹn: PRIMARY KEY, FOREIGN KEY, CHECK, UNIQUE, DEFAULT bảo vệ dữ liệu ngay tại CSDL."));
            b.Append(Bullet("Đọc chuỗi kết nối từ App.config qua ConfigurationManager - không viết cứng trong mã nguồn."));
            b.Append(Bullet("Bắt và phân biệt SqlException (lỗi CSDL) với lỗi nghiệp vụ, hiển thị thân thiện cho người dùng."));
            b.Append(TieuDe3("Hạn chế và hướng phát triển"));
            b.Append(Bullet("Ứng dụng mới ở mức kết nối và đọc dữ liệu; các bài sau sẽ bổ sung thêm, sửa, xóa có tham số hóa để phòng SQL Injection."));
            b.Append(Bullet("Có thể chuyển sang Entity Framework ở phần nâng cao; tuy nhiên nắm vững ADO.NET là nền tảng bắt buộc."));
            b.Append(Para("left", "Kết luận: bài thực hành đã đạt các mục tiêu - tạo được CSDL quan hệ cho bài toán quản lý sinh viên, "
                + "hiểu vai trò của Connection trong ADO.NET và kiểm tra được trạng thái kết nối trong ứng dụng Windows Forms.", false, 13, 160));
        }

        // =================================================================
        // MƯỜI BÀI
        // =================================================================
        private static void Bai1(StringBuilder b, string baoCao)
        {
            b.Append(TieuDe2("5.1. Bài 1: Tạo cơ sở dữ liệu QLSinhVien"));
            b.Append(TieuDe3("Yêu cầu"));
            b.Append(Bullet("Tạo cơ sở dữ liệu QLSinhVien, chạy lại được nhiều lần mà không lỗi."));
            b.Append(TieuDe3("Cách triển khai"));
            b.Append(Bullet("Kiểm tra DB_ID trước khi tạo để tệp kịch bản có thể chạy lại an toàn."));
            b.Append(Bullet("Chuyển ngữ cảnh sang QLSinhVien bằng USE để các câu lệnh sau thao tác đúng CSDL."));
            b.Append(TieuDe3("Mã T-SQL chính"));
            b.Append(BangMa(
                "IF DB_ID(N'QLSinhVien') IS NULL",
                "    CREATE DATABASE QLSinhVien;",
                "GO",
                "USE QLSinhVien;",
                "GO",
                "",
                "Bảng 5.1: Tạo CSDL QLSinhVien an toàn khi chạy lại"));
            HinhAnh(b, baoCao, "Bai07_09_KetQuaSinhVien.png",
                "Hình 5.1: Kết quả truy vấn dữ liệu mẫu sau khi tạo CSDL QLSinhVien (JOIN SinhVien - Khoa)");
        }

        private static void Bai2(StringBuilder b, string baoCao)
        {
            b.Append(TieuDe2("5.2. Bài 2: Tạo bảng Khoa và SinhVien"));
            b.Append(TieuDe3("Yêu cầu"));
            b.Append(Bullet("Bảng Khoa: khóa chính MaKhoa; tên khoa không trùng."));
            b.Append(Bullet("Bảng SinhVien: khóa chính MaSV; khóa ngoại MaKhoa tham chiếu Khoa."));
            b.Append(Bullet("Ràng buộc CHECK điểm trung bình thuộc [0, 10]; dùng NVARCHAR cho dữ liệu tiếng Việt."));
            b.Append(TieuDe3("Mã T-SQL chính"));
            b.Append(BangMa(
                "CREATE TABLE Khoa",
                "(",
                "    MaKhoa  VARCHAR(10)   NOT NULL PRIMARY KEY,",
                "    TenKhoa NVARCHAR(100) NOT NULL UNIQUE",
                ");",
                "GO",
                "",
                "CREATE TABLE SinhVien",
                "(",
                "    MaSV     VARCHAR(15)  NOT NULL PRIMARY KEY,",
                "    HoTen    NVARCHAR(100) NOT NULL,",
                "    NgaySinh DATE         NOT NULL,",
                "    GioiTinh BIT          NOT NULL CONSTRAINT DF_SinhVien_GioiTinh DEFAULT 1,",
                "    MaKhoa   VARCHAR(10)  NOT NULL,",
                "    DiemTB   DECIMAL(4,2) NOT NULL CONSTRAINT DF_SinhVien_DiemTB DEFAULT 0,",
                "    Email    VARCHAR(100) NULL,",
                "    CONSTRAINT CK_SinhVien_Diem CHECK (DiemTB BETWEEN 0 AND 10),",
                "    CONSTRAINT FK_SinhVien_Khoa FOREIGN KEY (MaKhoa) REFERENCES Khoa(MaKhoa)",
                ");",
                "",
                "Bảng 5.2: Tạo bảng Khoa và SinhVien với khóa, CHECK và FK"));
            b.Append(TieuDe3("Giao diện xem dữ liệu"));
            HinhAnh(b, baoCao, "Bai07_05_TruyVanSinhVien.png",
                "Hình 5.2: Bảng SinhVien hiển thị trên DataGridView (kèm tên khoa qua JOIN)");
        }

        private static void Bai3(StringBuilder b, string baoCao)
        {
            b.Append(TieuDe2("5.3. Bài 3: Tạo bảng HocPhan và DangKy"));
            b.Append(TieuDe3("Yêu cầu"));
            b.Append(Bullet("HocPhan: khóa chính MaHP, số tín chỉ hợp lệ 1..6."));
            b.Append(Bullet("DangKy: quan hệ nhiều - nhiều giữa SinhVien và HocPhan; khóa chính phức hợp (MaSV, MaHP)."));
            b.Append(TieuDe3("Mã T-SQL chính"));
            b.Append(BangMa(
                "CREATE TABLE DangKy",
                "(",
                "    MaSV       VARCHAR(15)  NOT NULL,",
                "    MaHP       VARCHAR(10)  NOT NULL,",
                "    HocKy      INT          NOT NULL,",
                "    Diem       DECIMAL(4,2) NULL,",
                "    NgayDangKy DATE NOT NULL CONSTRAINT DF_DangKy_NgayDangKy DEFAULT GETDATE(),",
                "    CONSTRAINT PK_DangKy       PRIMARY KEY (MaSV, MaHP),",
                "    CONSTRAINT CK_DangKy_HocKy CHECK (HocKy BETWEEN 1 AND 3),",
                "    CONSTRAINT CK_DangKy_Diem  CHECK (Diem IS NULL OR Diem BETWEEN 0 AND 10),",
                "    CONSTRAINT FK_DangKy_SV    FOREIGN KEY (MaSV) REFERENCES SinhVien(MaSV),",
                "    CONSTRAINT FK_DangKy_HP    FOREIGN KEY (MaHP) REFERENCES HocPhan(MaHP)",
                ");",
                "",
                "Bảng 5.3: Bảng DangKy - khóa chính phức hợp và hai khóa ngoại"));
            b.Append(Para("left", "Sơ đồ quan hệ dưới đây được vẽ trực tiếp từ metadata của CSDL (sys.foreign_keys, INFORMATION_SCHEMA), "
                + "với ký hiệu 🔑 cho khóa chính và 🔗 cho khóa ngoại.", false, 13, 60));
            HinhAnh(b, baoCao, "Bai07_08_SoDoQuanHe.png",
                "Hình 5.3: Sơ đồ quan hệ các bảng của QLSinhVien (Khoa, SinhVien, HocPhan, DangKy)");
            HinhAnh(b, baoCao, "Bai07_07_TruyVanDangKy.png",
                "Hình 5.4: Bảng DangKy hiển thị trên DataGridView (JOIN SinhVien và HocPhan)");
        }

        private static void Bai4(StringBuilder b, string baoCao)
        {
            b.Append(TieuDe2("5.4. Bài 4: Thêm dữ liệu mẫu"));
            b.Append(TieuDe3("Yêu cầu"));
            b.Append(Bullet("Tối thiểu 3 khoa, 10 sinh viên, 5 học phần và dữ liệu đăng ký."));
            b.Append(Bullet("Dữ liệu có dấu tiếng Việt để kiểm chứng NVARCHAR hiển thị đúng."));
            b.Append(TieuDe3("Mã T-SQL chính"));
            b.Append(BangMa(
                "INSERT INTO Khoa(MaKhoa, TenKhoa) VALUES",
                "('CNTT', N'Công nghệ thông tin'),",
                "('OTO',  N'Công nghệ kỹ thuật ô tô'),",
                "('KT',   N'Kinh tế');",
                "GO",
                "",
                "INSERT INTO HocPhan(MaHP, TenHP, SoTinChi) VALUES",
                "('HP01', N'Lập trình .NET', 4),",
                "('HP02', N'Cơ sở dữ liệu',  3),",
                "('HP03', N'Lập trình hướng đối tượng', 3),",
                "('HP04', N'Mạng máy tính',  3),",
                "('HP05', N'Trí tuệ nhân tạo', 3),",
                "('HP06', N'An toàn thông tin', 2);",
                "GO",
                "",
                "Bảng 5.4: Dữ liệu mẫu khoa và học phần"));
            b.Append(Para("left", "Kịch bản thực tế thêm 12 sinh viên và 26 dòng đăng ký (vượt yêu cầu tối thiểu). "
                + "Số lượng kiểm chứng: 3 khoa, 12 sinh viên, 6 học phần, 26 đăng ký.", false, 13, 60));
            HinhAnh(b, baoCao, "Bai07_04_TruyVanKhoa.png",
                "Hình 5.5: Danh mục Khoa (3 dòng) sau khi thêm dữ liệu mẫu");
            HinhAnh(b, baoCao, "Bai07_06_TruyVanHocPhan.png",
                "Hình 5.6: Danh mục HocPhan (6 học phần) sau khi thêm dữ liệu mẫu");
        }

        private static void Bai5(StringBuilder b, string baoCao)
        {
            b.Append(TieuDe2("5.5. Bài 5: Lớp kết nối và nút kiểm tra kết nối"));
            b.Append(TieuDe3("Yêu cầu"));
            b.Append(Bullet("Tạo lớp DbConnectionFactory.Create() trả về SqlConnection."));
            b.Append(Bullet("Nút btnKiemTra: mở kết nối, báo thành công kèm tên CSDL; nếu lỗi thì bắt SqlException và báo lỗi."));
            b.Append(TieuDe3("Mã nguồn chính"));
            b.Append(BangMa(
                "// DAL/DbConnectionFactory.cs",
                "public static SqlConnection Create()",
                "{",
                "    return new SqlConnection(ChuoiKetNoi);",
                "}",
                "",
                "// GUI/frmKetNoi.cs",
                "private void btnKiemTra_Click(object sender, EventArgs e)",
                "{",
                "    TrangThaiKetNoi tt = _bus.KiemTraVoiChuoi(chuoiKetNoi, nguon);",
                "    if (tt.ThanhCong)",
                "        MessageBox.Show(\"Kết nối thành công: \" + tt.Database);",
                "    else",
                "        MessageBox.Show(\"Lỗi kết nối: \" + tt.ThongBao);",
                "}",
                "",
                "Bảng 5.5: Lớp kết nối và xử lý nút kiểm tra kết nối"));
            HinhAnh(b, baoCao, "Bai07_02_KetNoiWindows.png",
                "Hình 5.7: Màn hình kiểm tra kết nối - kết nối thành công tới QLSinhVien");
        }

        private static void Bai6(StringBuilder b, string baoCao)
        {
            b.Append(TieuDe2("5.6. Bài 6: Ràng buộc bổ sung và thử dữ liệu vi phạm"));
            b.Append(TieuDe3("Yêu cầu (tự làm)"));
            b.Append(Bullet("Thêm UNIQUE cho Email, DEFAULT cho ngày đăng ký, CHECK cho học kỳ."));
            b.Append(Bullet("Thử chèn dữ liệu vi phạm để kiểm chứng ràng buộc chặn đúng."));
            b.Append(TieuDe3("Cách triển khai"));
            b.Append(Bullet("Bọc các câu lệnh cố ý sai trong TRY/CATCH để kịch bản vẫn chạy hết và in thông báo lỗi."));
            b.Append(TieuDe3("Mã T-SQL chính"));
            b.Append(BangMa(
                "ALTER TABLE SinhVien",
                "    ADD CONSTRAINT UQ_SinhVien_Email UNIQUE (Email);",
                "GO",
                "",
                "BEGIN TRY",
                "    INSERT INTO SinhVien(MaSV, HoTen, NgaySinh, GioiTinh, MaKhoa, DiemTB, Email)",
                "    VALUES ('SV999', N'Nguyễn Văn Sai', '2005-01-01', 1, 'CNTT', 12, NULL);",
                "    PRINT N'  => KHÔNG chặn (sai)!';",
                "END TRY",
                "BEGIN CATCH",
                "    PRINT N'  => Đã chặn đúng: ' + ERROR_MESSAGE();",
                "END CATCH;",
                "",
                "Bảng 5.6: Thêm UNIQUE và thử dữ liệu vi phạm bằng TRY/CATCH"));
        }

        private static void Bai7(StringBuilder b, string baoCao)
        {
            b.Append(TieuDe2("5.7. Bài 7: Chuỗi kết nối SQL Authentication"));
            b.Append(TieuDe3("Yêu cầu (tự làm)"));
            b.Append(Bullet("Viết chuỗi kết nối dùng SQL Authentication thay cho Windows Authentication."));
            b.Append(Bullet("Tạo tài khoản SQL riêng với quyền tối thiểu."));
            b.Append(TieuDe3("Mã nguồn chính"));
            b.Append(BangMa(
                "// DAL/DbConnectionFactory.cs",
                "public static SqlConnection CreateSqlAuth(string mayChu, string csdl,",
                "                                          string taiKhoan, string matKhau)",
                "{",
                "    var builder = new SqlConnectionStringBuilder();",
                "    builder.DataSource = mayChu;",
                "    builder.InitialCatalog = csdl;",
                "    builder.UserID = taiKhoan;",
                "    builder.Password = matKhau;",
                "    builder.IntegratedSecurity = false;",
                "    return new SqlConnection(builder.ConnectionString);",
                "}",
                "",
                "-- T-SQL: tạo login và user quyền tối thiểu",
                "CREATE LOGIN app_qlsv WITH PASSWORD = 'MatKhau@123';",
                "USE QLSinhVien;",
                "CREATE USER app_qlsv FOR LOGIN app_qlsv;",
                "ALTER ROLE db_datareader ADD MEMBER app_qlsv;",
                "",
                "Bảng 5.7: SQL Authentication và tài khoản ứng dụng quyền tối thiểu"));
            HinhAnh(b, baoCao, "Bai07_10_LoiKetNoi.png",
                "Hình 5.8: Minh hoạ bắt lỗi SqlException khi chuỗi kết nối sai (không làm sập ứng dụng)");
        }

        private static void Bai8(StringBuilder b, string baoCao)
        {
            b.Append(TieuDe2("5.8. Bài 8: Đọc chuỗi kết nối từ App.config"));
            b.Append(TieuDe3("Yêu cầu (tự làm)"));
            b.Append(Bullet("Không viết cứng chuỗi kết nối trong mã nguồn; đọc từ App.config bằng ConfigurationManager."));
            b.Append(TieuDe3("Mã nguồn & cấu hình chính"));
            b.Append(BangMa(
                "<!-- App.config -->",
                "<connectionStrings>",
                "  <add name=\"QLSinhVien\"",
                "       connectionString=\"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=QLSinhVien;Integrated Security=True\"",
                "       providerName=\"System.Data.SqlClient\" />",
                "</connectionStrings>",
                "",
                "// DAL/DbConnectionFactory.cs",
                "ConnectionStringSettings css = ConfigurationManager.ConnectionStrings[TenChuoiKetNoi];",
                "if (css != null && !string.IsNullOrEmpty(css.ConnectionString))",
                "    return css.ConnectionString;",
                "",
                "Bảng 5.8: Chuỗi kết nối đặt ở App.config và đọc bằng ConfigurationManager"));
            b.Append(Para("left", "Khi không tìm thấy khoá trong App.config, lớp sẽ dùng chuỗi mặc định có ghi chú rõ ràng, "
                + "giúp ứng dụng vẫn chạy được ở môi trường khác. Cách đổi instance được ghi trong README.txt của dự án.", false, 13, 60));
        }

        private static void Bai9(StringBuilder b, string baoCao)
        {
            b.Append(TieuDe2("5.9. Bài 9: Màn hình trạng thái kết nối"));
            b.Append(TieuDe3("Yêu cầu (tự làm)"));
            b.Append(Bullet("Màn hình hiển thị DataSource, Database, ServerVersion, State và thời gian kết nối."));
            b.Append(TieuDe3("Mã nguồn chính"));
            b.Append(BangMa(
                "// BUS/NghiepVu.cs - KetNoiBUS.KiemTraVoiChuoi",
                "using (SqlConnection conn = DbConnectionFactory.Create(chuoiKetNoi))",
                "{",
                "    conn.Open();",
                "    tt.ThoiGianKetNoiMs = (long)(DateTime.Now - batDau).TotalMilliseconds;",
                "    tt.DataSource    = conn.DataSource;",
                "    tt.Database      = conn.Database;",
                "    tt.ServerVersion = conn.ServerVersion;",
                "    tt.State         = conn.State.ToString();",
                "}",
                "",
                "Bảng 5.9: Đọc các thuộc tính của SqlConnection để hiển thị trạng thái"));
            HinhAnh(b, baoCao, "Bai07_03_TrangThaiKetNoi.png",
                "Hình 5.9: Trang trạng thái kết nối - DataSource, Database, ServerVersion, State, thời gian kết nối");
        }

        private static void Bai10(StringBuilder b, string baoCao)
        {
            b.Append(TieuDe2("5.10. Bài 10: Sao lưu và phục hồi cơ sở dữ liệu"));
            b.Append(TieuDe3("Yêu cầu (tự làm)"));
            b.Append(Bullet("Sao lưu QLSinhVien ra tệp .bak; xoá thử một bản ghi rồi phục hồi để kiểm chứng."));
            b.Append(TieuDe3("Mã T-SQL chính"));
            b.Append(BangMa(
                "BACKUP DATABASE QLSinhVien",
                "TO DISK = N'C:\\Backup\\QLSinhVien.bak'",
                "WITH INIT, NAME = N'QLSinhVien-Full Backup';",
                "GO",
                "",
                "-- Sau khi xóa thử một bản ghi:",
                "RESTORE DATABASE QLSinhVien",
                "FROM DISK = N'C:\\Backup\\QLSinhVien.bak'",
                "WITH REPLACE, RECOVERY;",
                "GO",
                "",
                "Bảng 5.10: Sao lưu và phục hồi cơ sở dữ liệu QLSinhVien"));
            b.Append(Para("left", "Có thể thực hiện thao tác sao lưu/phục hồi trực tiếp trong SSMS bằng chuột phải CSDL → Tasks → "
                + "Back Up... / Restore.... Sau khi phục hồi, dữ liệu đã xoá thử xuất hiện trở lại, chứng tỏ bản sao lưu hợp lệ.", false, 13, 60));
            HinhAnh(b, baoCao, "Bai07_01_frmMain.png",
                "Hình 5.10: Màn hình chính ứng dụng - dashboard hiển thị số bản ghi từng bảng sau khi CSDL sẵn sàng");
        }

        // =================================================================
        // HANG MUC DO HOA
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

            string folder = Path.Combine(Path.GetDirectoryName(baoCao), "..", "scratch", "docx_build7", "word", "media");
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

        // =================================================================
        // DONG GOI OPC (ten entry dung dau '/')
        // =================================================================
        private static void DongGoi(string thuMuc, string outFile)
        {
            var ds = new List<string>();
            Colect(thuMuc, thuMuc, ds);

            ds.Sort(delegate (string a, string b)
            {
                if (a == "[Content_Types].xml") return -1;
                if (b == "[Content_Types].xml") return 1;
                return string.CompareOrdinal(a, b);
            });

            using (var ms = new MemoryStream())
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
        // PHAN XML TINH
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

        /// <summary>Khoi ma nguon: bang 1 o nen xam, font Courier New 9pt.</summary>
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

        /// <summary>Bang du lieu co dong tieu de to mau.</summary>
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
                + "<dc:title>Báo cáo thực hành Lab 7 - Tạo CSDL SQL Server, tổng quan ADO.NET và kết nối CSDL</dc:title>"
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
                + "<Application>BuildDocx7</Application><Company>ĐH Công nghệ Đông Á</Company>"
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
                + Run("Báo cáo thực hành Lab 7 · IT3212 - Lập trình .NET · 24000407", false, true, 9.5, false)
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
