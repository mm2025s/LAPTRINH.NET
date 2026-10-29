================================================================================
BÁO CÁO THỰC HÀNH LAB 7 - IT3212 - LẬP TRÌNH .NET
Tạo CSDL SQL Server, tổng quan ADO.NET và kết nối CSDL
================================================================================

Sinh viên thực hiện : Nguyễn Trường Giang
Mã số sinh viên     : 24000407
Dự án               : Lab07_ADONET_Connection
Cơ sở dữ liệu       : QLSinhVien (dùng liên tục từ Lab 7 đến Lab 16)

--------------------------------------------------------------------------------
1. CẤU TRÚC THƯ MỤC
--------------------------------------------------------------------------------
Lap7/
├── Program.cs                        // Entry point; tham số --screenshots
├── App.config                        // Chuỗi kết nối QLSinhVien (bài 8)
├── Lab07_ADONET_Connection.csproj    // Khai báo project (có System.Configuration)
├── DTO/DoiTuong.cs                   // Khoa, SinhVien, HocPhan, DangKy, TrangThaiKetNoi
├── DAL/DbConnectionFactory.cs        // Tạo SqlConnection (bài 5, 7, 8)
├── DAL/TruyVanDAL.cs                 // SqlDataReader / SqlDataAdapter / metadata
├── BUS/NghiepVu.cs                   // KetNoiBUS, TruyVanBUS
├── GUI/
│   ├── frmMain.cs                    // MDI + MenuStrip + StatusStrip + Dashboard
│   ├── frmKetNoi.cs                  // Bài 5, 7: nút btnKiemTra, SQL Auth
│   ├── frmTrangThai.cs               // Bài 9: trạng thái kết nối
│   ├── frmTruyVan.cs                 // Bài 5: DataGridView đọc dữ liệu mẫu
│   ├── frmSoDoQuanHe.cs              // Sơ đồ quan hệ (vẽ GDI+ từ metadata)
│   └── Helpers/                      // UiTheme, UiControls, FormBase, DinhDang
├── SQL/Lab07_QLSinhVien.sql          // Toàn bộ T-SQL của bài 1 - 4, 6, 10
└── BaoCaoTools/TrinhChupAnh.cs       // Tự động chụp ảnh báo cáo

--------------------------------------------------------------------------------
2. TÊN INSTANCE SQL SERVER ĐANG DÙNG
--------------------------------------------------------------------------------
Máy demo dùng SQL Server Express LocalDB:

    (localdb)\MSSQLLocalDB

Các tùy chọn khác (sửa trong App.config hoặc trong frmKetNoi):
    * SQL Server Express (instance mặc định)  : .\SQLEXPRESS
    * SQL Server 2019/2022 (instance ".")     : .
    * LocalDB                                : (localdb)\MSSQLLocalDB

Cách tìm tên instance: mở SSMS, trong hộp Connect to Server bấm "Server name"
→ "Browse for more..." → Database Engine, hoặc chạy "SELECT @@SERVERNAME;".

--------------------------------------------------------------------------------
3. TẠO CƠ SỞ DỮ LIỆU
--------------------------------------------------------------------------------
Cách 1 - SSMS: mở tệp SQL/Lab07_QLSinhVien.sql rồi nhấn Execute (F5).
Cách 2 - dòng lệnh:

    sqlcmd -S "(localdb)\MSSQLLocalDB" -E -i Lap7\SQL\Lab07_QLSinhVien.sql

Cách 3 - qua script build:

    powershell -File scratch\build_lap7.ps1 -Db

Kịch bản có thể chạy lại nhiều lần: nó tự kiểm tra và xoá/tạo lại các bảng.

Kết quả mong đợi sau khi chạy:
    Khoa = 3 bảng ghi, SinhVien = 12, HocPhan = 6, DangKy = 26.

--------------------------------------------------------------------------------
4. ĐỔI CHUỖI KẾT NỐI
--------------------------------------------------------------------------------
- Cách chuẩn (bài 8): sửa thuộc tính connectionString của mục "QLSinhVien"
  trong tệp App.config. Ứng dụng đọc qua ConfigurationManager, KHÔNG cần sửa code.

- Nếu không tìm thấy khoá trong App.config, lớp DbConnectionFactory sẽ dùng
  chuỗi mặc định: Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=QLSinhVien;
  Integrated Security=True

- Trên màn hình "Kiểm tra kết nối" (frmKetNoi) có thể sửa chuỗi trực tiếp và
  bấm "Nạp lại từ App.config" để khôi phục.

- Mẫu chuỗi theo từng kiểu xác thực:
    Windows Auth : Data Source=<instance>;Initial Catalog=QLSinhVien;Integrated Security=True
    SQL Auth     : Data Source=<instance>;Initial Catalog=QLSinhVien;User ID=app_qlsv;Password=MatKhau@123

--------------------------------------------------------------------------------
5. BIÊN DỊCH VÀ CHẠY
--------------------------------------------------------------------------------
(Project dùng .NET Framework 4.8, biên dịch bằng csc.exe, không cần NuGet.)

    powershell -File scratch\build_lap7.ps1            # chỉ biên dịch
    powershell -File scratch\build_lap7.ps1 -Run       # biên dịch và mở ứng dụng
    powershell -File scratch\build_lap7.ps1 -Shots     # biên dịch và chụp ảnh báo cáo
    powershell -File scratch\build_lap7.ps1 -Db -Shots # tạo CSDL rồi chụp ảnh

Với Visual Studio 2022: mở IT3212_Labs.sln, chọn project Lab07_ADONET_Connection
làm startup rồi nhấn F5. Nhớ chạy kịch bản SQL trước khi chạy ứng dụng.

Chế độ chụp ảnh (không cần thao tác tay):

    Lab07_ADONET_Connection.exe --screenshots [tên_kịch_bản]

Ảnh lưu ở Lap7/BaoCao/*.png, nhật ký ở Lap7/BaoCao/nhat_ky_chup_anh.txt.

--------------------------------------------------------------------------------
6. MƯỜI BÀI THỰC HÀNH (đối chiếu sản phẩm)
--------------------------------------------------------------------------------
Bài 1  Tạo cơ sở dữ liệu QLSinhVien                 -> SQL/Lab07_QLSinhVien.sql
Bài 2  Bảng Khoa, SinhVien (PK, FK, CHECK, NVARCHAR) -> SQL + frmTruyVan (xem SinhVien)
Bài 3  Bảng HocPhan, DangKy (khóa phức hợp)          -> SQL + frmSoDoQuanHe + frmTruyVan
Bài 4  Dữ liệu mẫu (3 khoa, 12 SV, 6 HP, 26 ĐK)      -> SQL
Bài 5  Lớp kết nối và nút btnKiemTra                 -> DAL/DbConnectionFactory + frmKetNoi
Bài 6  UNIQUE Email, DEFAULT, CHECK học kỳ + thử vi phạm -> SQL (khối TRY/CATCH)
Bài 7  Chuỗi kết nối SQL Authentication              -> frmKetNoi + CreateSqlAuth
Bài 8  Đọc chuỗi kết nối từ App.config               -> App.config + DbConnectionFactory
Bài 9  Màn hình trạng thái kết nối                   -> frmTrangThai
Bài 10 Sao lưu và phục hồi cơ sở dữ liệu             -> SQL (khối BACKUP/RESTORE)

--------------------------------------------------------------------------------
7. SẢN PHẨM NỘP
--------------------------------------------------------------------------------
- SQL/Lab07_QLSinhVien.sql                (chạy được từ đầu trên CSDL trống)
- Project Lab07_ADONET_Connection         (có hàm kiểm tra kết nối)
- BaoCao/*.png                            (ảnh sơ đồ quan hệ và kết quả truy vấn)
- BaoCao/BaoCao_Lab07_24000407.docx       (báo cáo Word)
- README.txt này                          (tên instance + cách đổi chuỗi kết nối)
================================================================================
