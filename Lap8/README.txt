================================================================================
BÁO CÁO THỰC HÀNH LAB 8 - IT3212 - LẬP TRÌNH .NET
SqlCommand, SqlParameter và các phương thức thực thi (Chương 4)
================================================================================

Sinh viên thực hiện : Nguyễn Trường Giang
Mã số sinh viên     : 24000407
Dự án               : Lab08_SqlCommand_Parameters
Cơ sở dữ liệu       : QLSinhVien (tạo từ Lab 7, dùng tiếp cho Lab 9 → Lab 16)

--------------------------------------------------------------------------------
1. MỤC TIÊU ĐÃ THỰC HIỆN
--------------------------------------------------------------------------------
- Tạo và cấu hình SqlCommand cho cả 3 kiểu lệnh: Text và StoredProcedure.
- Phân biệt rõ 3 phương thức thực thi:
      ExecuteScalar    → trả về MỘT giá trị (dòng đầu, cột đầu) dạng object.
      ExecuteReader    → trả về SqlDataReader (forward-only, read-only).
      ExecuteNonQuery  → trả về SỐ DÒNG bị ảnh hưởng (INSERT/UPDATE/DELETE).
- Dùng SqlParameter ở 100% câu lệnh có dữ liệu người dùng → chống SQL Injection.
- Gọi Stored Procedure có tham số vào và tham số OUTPUT.
- Xử lý DBNull.Value / DBNull khi ghi và khi đọc dữ liệu.

--------------------------------------------------------------------------------
2. CẤU TRÚC THƯ MỤC
--------------------------------------------------------------------------------
Lap8/
├── Program.cs                            // Entry point; chế độ --screenshots
├── App.config                            // Chuỗi kết nối QLSinhVien
├── Lab08_SqlCommand_Parameters.csproj    // Project .NET Framework 4.8
├── DTO/DoiTuong.cs                       // Khoa, SinhVien, ThongKeKhoa, KetQuaThaoTac
├── DAL/
│   ├── DbConnectionFactory.cs            // Tạo SqlConnection từ App.config
│   ├── KhoaDAL.cs                        // Danh sách khoa cho ComboBox
│   ├── SinhVienDAL.cs                    // Bài 1 → 9: nơi chứa SqlCommand/SqlParameter
│   └── SqlHelper.cs                      // Bài 10: lớp tiện ích tối giản
├── BUS/SinhVienBUS.cs                    // Kiểm tra dữ liệu + chuyển lỗi SQL → tiếng Việt
├── GUI/
│   ├── frmMain.cs                        // MDI, menu 10 bài, dashboard
│   ├── frmBai1.cs … frmBai10.cs          // Mỗi bài một Form riêng
│   └── Helpers/                          // UiTheme, UiControls, FormBase, DinhDang,
│                                         // BaiFormBase, NhapSinhVienPanel
├── SQL/Lab08_StoredProcedures.sql        // 3 stored procedure + 10 SV bổ sung
└── BaoCaoTools/TrinhChupAnh.cs           // Tự động chụp ảnh báo cáo

Kiến trúc 3 lớp: GUI → BUS → DAL → CSDL.
Chỉ tầng DAL biết tên bảng/cột và biết SqlCommand; GUI không có một dòng SQL nào.

--------------------------------------------------------------------------------
3. CHUẨN BỊ CƠ SỞ DỮ LIỆU (BẮT BUỘC, LÀM TRƯỚC KHI CHẠY)
--------------------------------------------------------------------------------
Bước 1 - Tạo CSDL từ Lab 7 (nếu chưa có):

    sqlcmd -S "(localdb)\MSSQLLocalDB" -E -i Lap7\SQL\Lab07_QLSinhVien.sql

Bước 2 - Chạy script bổ sung của Lab 8:

    sqlcmd -S "(localdb)\MSSQLLocalDB" -E -i Lap8\SQL\Lab08_StoredProcedures.sql

Script này làm 3 việc:
    (a) Bổ sung 10 sinh viên SV013 → SV022 (tổng 22 SV, đủ để thử lọc/thống kê).
    (b) Tạo 3 stored procedure:
            sp_SinhVien_TheoKhoa   @MaKhoa VARCHAR(10)                  (bài 5)
            sp_SinhVien_TimTheoMa  @MaSV VARCHAR(15)                    (bài 6)
            sp_SinhVien_Them       ... @SoLuongHienTai INT OUTPUT       (bài 9)
    (c) Thay ràng buộc UNIQUE(Email) bằng UNIQUE INDEX có điều kiện
            WHERE Email IS NOT NULL
        Lý do: Lab 7 đặt UQ_SinhVien_Email UNIQUE(Email) nên chỉ cho phép MỘT
        giá trị NULL. Bài 3 truyền DBNull.Value khi bỏ trống email, sang sinh
        viên thứ hai sẽ bị lỗi "Cannot insert duplicate key … (<NULL>)".
        Index có điều kiện vẫn chống trùng email nhưng cho phép nhiều NULL.

Kết quả mong đợi sau khi chạy: SinhVien = 22 bản ghi, có 3 procedure ở trên.

Ghi chú: script dùng CREATE OR ALTER và các khối IF EXISTS nên chạy lại nhiều lần
đều an toàn. Riêng filtered index cần SET QUOTED_IDENTIFIER ON (đã có sẵn ở đầu
script, vì sqlcmd mặc định là OFF).

--------------------------------------------------------------------------------
4. TÊN INSTANCE SQL SERVER
--------------------------------------------------------------------------------
Máy demo dùng LocalDB:

    (localdb)\MSSQLLocalDB

Tùy chọn khác — sửa thuộc tính connectionString của mục "QLSinhVien" trong
Lap8\App.config, KHÔNG cần sửa mã nguồn:

    SQL Server Express : Data Source=.\SQLEXPRESS;Initial Catalog=QLSinhVien;Integrated Security=True
    SQL Server 2019/22 : Data Source=.;Initial Catalog=QLSinhVien;Integrated Security=True
    SQL Authentication  : Data Source=.\SQLEXPRESS;Initial Catalog=QLSinhVien;User ID=app_qlsv;Password=MatKhau@123

Nếu không tìm thấy khóa trong App.config, DbConnectionFactory dùng chuỗi mặc định
trỏ tới (localdb)\MSSQLLocalDB.

--------------------------------------------------------------------------------
5. BIÊN DỊCH VÀ CHẠY
--------------------------------------------------------------------------------
Cách 1 - Visual Studio 2022:
    Mở IT3212_Labs.sln → chuột phải project Lab08_SqlCommand_Parameters →
    "Set as Startup Project" → F5.

Cách 2 - MSBuild + dòng lệnh:

    msbuild Lap8\Lab08_SqlCommand_Parameters.csproj /p:Configuration=Debug
    Lap8\bin\Debug\Lab08_SqlCommand_Parameters.exe

Chế độ chụp ảnh báo cáo (không cần thao tác tay):

    Lab08_SqlCommand_Parameters.exe --screenshots
    Lab08_SqlCommand_Parameters.exe --screenshots Chup03_Bai2_ExecuteReader

Ảnh lưu ở Lap8\BaoCao\*.png, nhật ký ở Lap8\BaoCao\nhat_ky_chup_anh.txt.
Trình chụp ảnh chỉ chạy các bài THAO TÁC ĐỌC nên không làm thay đổi dữ liệu mẫu.

--------------------------------------------------------------------------------
6. MƯỜI BÀI THỰC HÀNH (đối chiếu sản phẩm)
--------------------------------------------------------------------------------
PHẦN A - BÀI TẬP CÓ HƯỚNG DẪN

Bài 1  CountStudents() bằng ExecuteScalar
       → frmBai1 + SinhVienDAL.CountStudents()
       Kết quả: Convert.ToInt32(object) vừa chuyển kiểu vừa xử lý NULL.

Bài 2  Đọc danh sách bằng ExecuteReader vào ListBox
       → frmBai2 + SinhVienDAL.DocDanhSach()
       Mỗi reader.Read() tiến một dòng; ListBox hiển thị "MaSV - HoTen - Điểm".

Bài 3  INSERT bằng ExecuteNonQuery + SqlParameter (Email → DBNull.Value)
       → frmBai3 + SinhVienDAL.ThemSinhVien()
       Mọi tham số khai báo đúng SqlDbType và độ dài cột; @DiemTB có
       Precision = 4, Scale = 2 thay cho AddWithValue.

Bài 4  UPDATE DiemTB theo MaSV, báo "Không tìm thấy mã" khi rows = 0
       → frmBai4 + SinhVienDAL.CapNhatDiem()
       Thử nhập  ' OR '1'='1  để thấy tham số hóa chặn được SQL Injection.

Bài 5  Gọi Stored Procedure sp_SinhVien_TheoKhoa
       → frmBai5 + SinhVienDAL.SinhVienTheoKhoa()
       CommandType.StoredProcedure; tên tham số @MaKhoa khớp với thủ tục.

PHẦN B - BÀI TẬP TỰ THỰC HIỆN

Bài 6  Tìm sinh viên theo mã (ExecuteReader + SqlParameter)
       → frmBai6 + SinhVienDAL.TimTheoMa()
       reader.Read() = false → thông báo "Không tìm thấy", không gọi GetXxx.

Bài 7  MAX(DiemTB) theo khoa bằng ExecuteScalar, xử lý NULL
       → frmBai7 + SinhVienDAL.DiemCaoNhatCuaKhoa() / ThongKeDiemCaoNhatTheoKhoa()
       Khoa rỗng → MAX = NULL → ExecuteScalar trả DBNull.Value → hiển thị "— (NULL)".
       Có thêm bảng COUNT/MAX/AVG toàn bộ khoa để đối chiếu.

Bài 8  Xóa sinh viên theo mã (ExecuteNonQuery, xác nhận, xử lý khóa ngoại)
       → frmBai8 + SinhVienDAL.XoaSinhVien()
       MessageBox Yes/No trước khi xóa; bắt SqlException.Number == 547
       (FK_DangKy_SV) và hỏi có muốn xóa luôn các đăng ký.

Bài 9  Stored Procedure thêm sinh viên + tham số OUTPUT
       → frmBai9 + SinhVienDAL.ThemSinhVienQuaThuTuc()
       pOut.Direction = ParameterDirection.Output khai báo TRƯỚC ExecuteNonQuery,
       đọc pOut.Value SAU khi chạy.

Bài 10 Lớp SqlHelper tối giản
       → DAL/SqlHelper.cs + frmBai10
       ExecuteScalar / ExecuteNonQuery / ExecuteReader / ExecuteDataTable, mọi
       phương thức dùng using cho SqlConnection, SqlCommand, SqlDataReader
       → tài nguyên luôn được giải phóng kể cả khi có exception.
       Thêm P() / POutput() để tạo SqlParameter đúng kiểu.

--------------------------------------------------------------------------------
7. DỮ LIỆU THỬ
--------------------------------------------------------------------------------
Khoa       : CNTT (Công nghệ thông tin), OTO (Ô tô), KT (Kế toán)
SinhVien   : 22 bản ghi SV001 → SV022
             Điểm thấp nhất 4.80 (SV005), cao nhất 9.55 (SV017)
             SV005, SV009, SV012 có đăng ký học phần → dùng để thử bài 8 (lỗi 547)
             SV001, SV008, SV017, SV013, SV020 thuộc CNTT → dùng để thử bài 5, 7

Bài thử nhanh gợi ý:
    Bài 1  → mở Form, đọc số 22.
    Bài 2  → bấm "Đọc bằng ExecuteReader", ListBox có 22 dòng.
    Bài 3  → nhập SV023 / Nguyễn Văn Minh / 08.25 / email trống → thêm thành công,
             để trống email lần nữa với SV024 cũng thành công (nhờ filtered index).
    Bài 4  → nhập SV001, điểm 9.10; sau đó nhập mã SV999 → "Không tìm thấy mã".
    Bài 5  → chọn khoa CNTT → 9 sinh viên; OTO → 6; KT → 7.
    Bài 6  → SV017 → Lâm Thu Trang; SV999 → không tìm thấy.
    Bài 7  → chọn khoa bất kỳ, cột "Điểm cao nhất" hiển thị số; khoa rỗng → "— (NULL)".
    Bài 8  → SV005 → cảnh báo "còn 2 đăng ký"; chọn Yes để xóa kèm.
    Bài 9  → nhập SV023 → @SoLuongHienTai = 23.
    Bài 10 → bấm chạy lại, 3 ô kết quả đều có số.

--------------------------------------------------------------------------------
8. SẢN PHẨM NỘP
--------------------------------------------------------------------------------
- Project Lab08_SqlCommand_Parameters (biên dịch và chạy được)
- SQL/Lab08_StoredProcedures.sql      (3 stored procedure + 10 SV bổ sung)
- BaoCao/*.png                        (ảnh 10 Form kết quả)
- README.txt này                      (cách chạy, dữ liệu thử, đối chiếu 10 bài)

--------------------------------------------------------------------------------
9. GHI CHÚ KỸ THUẬT ĐÁNG CHÚ Ý
--------------------------------------------------------------------------------
1. Không dùng AddWithValue: nó suy kiểu từ giá trị C# nên dễ sai với DECIMAL
   (mất phần lẻ), NVARCHAR (chuyển thành VARCHAR mất dấu tiếng Việt) hoặc DATE.
   Luôn khai báo tường minh SqlDbType + Size.

2. ExecuteScalar trả về object, có thể là DBNull.Value. Với COUNT thì
   Convert.ToInt32(null) = 0 là an toàn; với MAX/AVG phải kiểm tra DBNull
   trước khi chuyển sang decimal.

3. SqlDataReader chỉ đọc được khi connection còn mở → mọi thao tác đọc dữ liệu
   phải nằm TRONG khối using (SqlConnection).

4. Reader forward-only: không quay lại dòng trước, không sửa dữ liệu. Cần duyệt
   nhiều lần thì đổ vào List<T> hoặc DataTable.

5. Tham số OUTPUT phải khai báo Direction trước khi ExecuteNonQuery và chỉ đọc
   Value sau khi lệnh đã chạy xong.

6. ExecuteNonQuery trả về số dòng bị ảnh hưởng — dùng nó để phát hiện
   "không tìm thấy mã" (0 dòng) thay vì phải SELECT đếm trước.

================================================================================
