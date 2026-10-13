================================================================================
HƯỚNG DẪN CHẠY DỰ ÁN VÀ THÔNG TIN BÀI TẬP LAB 02
Học phần: Lập trình .NET (IT3212) - Lập trình hướng đối tượng
Trường: Đại học Công nghệ Đông Á
================================================================================

1. CÁCH BUILD VÀ CHẠY CHƯƠNG TRÌNH
--------------------------------------------------------------------------------
Yêu cầu môi trường: Đã cài .NET 8.0 SDK trở lên.

Mở Terminal và chuyển vào thư mục Lap2:
    cd /Users/mmo2025s/mmo2025/NET/Lap2

Để build kiểm tra lỗi:
    dotnet build

Để chạy chương trình:
    dotnet run

Sau khi chạy, giao diện Console sẽ hiển thị menu chọn bài tập từ 1 đến 10,
nhập 0 để thoát chương trình.

2. MỤC TIÊU ĐÃ ĐẠT ĐƯỢC
--------------------------------------------------------------------------------
- Khai báo lớp, trường dữ liệu (backing field private), thuộc tính (property) và
  phương thức trong C#.
- Khởi tạo đối tượng bằng constructor mặc định và constructor có tham số (nạp chồng).
- Áp dụng tính đóng gói: kiểm tra dữ liệu ngay tại setter của thuộc tính.
- Quản lý danh sách đối tượng bằng List<T>, tìm kiếm với Find / LINQ.

3. CẤU TRÚC THƯ MỤC NGUỒN (Lap2)
--------------------------------------------------------------------------------
Lap2/
├── Lab02_OOPBasics.csproj           # Cấu hình dự án .NET 8.0 Console
├── Lab02_OOPBasics.cs               # Menu chính điều hướng chọn chạy từng bài
│
├── [CÁC LỚP MÔ HÌNH - MODEL]
├── SinhVien.cs                       # Lớp SinhVien (đối tượng xuyên suốt Lab 2-4)
├── SanPham.cs                        # Lớp SanPham
├── TaiKhoanNganHang.cs               # Lớp tài khoản ngân hàng
├── NhanVien.cs                       # Lớp nhân viên
├── HinhChuNhat.cs                    # Lớp hình chữ nhật
├── QuanLySinhVien.cs                 # Lớp quản lý danh sách sinh viên (CRUD)
│
├── [CÁC BÀI TẬP CÓ HƯỚNG DẪN]
├── Bai01_KhaiBaoLop.cs               # Bài 1: Khai báo lớp SinhVien
├── Bai02_Constructor.cs              # Bài 2: Constructor & nạp chồng constructor
├── Bai03_DongGoi.cs                  # Bài 3: Đóng gói và kiểm tra dữ liệu
├── Bai04_PhuongThucNghiepVu.cs       # Bài 4: Phương thức TinhTuoi() & XepLoai()
├── Bai05_QuanLyDanhSach.cs           # Bài 5: Quản lý danh sách SinhVien
│
├── [CÁC BÀI TẬP TỰ THỰC HIỆN]
├── Bai06_LopSanPham.cs               # Bài 6: Lớp SanPham
├── Bai07_TaiKhoanNganHang.cs         # Bài 7: Lớp TaiKhoanNganHang
├── Bai08_LopNhanVien.cs              # Bài 8: Lớp NhanVien (bảng lương)
├── Bai09_HinhChuNhat.cs              # Bài 9: Lớp HinhChuNhat
├── Bai10_UngDungQuanLySinhVien.cs    # Bài 10: Ứng dụng quản lý sinh viên
│
├── README.txt                        # Tệp hướng dẫn này
└── BAO_CAO_KET_QUA.txt               # Tệp báo cáo kết quả chạy từng bài

4. DỮ LIỆU THỬ NGHIỆM TỪNG BÀI
--------------------------------------------------------------------------------
1. Bài 1: SV01 - Nguyễn An - 12/05/2005 - 8.2  => in thông tin 1 sinh viên.
2. Bài 2: So sánh constructor mặc định ("Chưa có") và constructor đầy đủ tham số.
3. Bài 3: Nhập họ tên rỗng và điểm 11 => bắt lỗi ArgumentException / OutOfRange.
4. Bài 4: SV04 - Phạm Dung - 20/12/2005 - 9.1 => Tuổi 20, xếp loại "Xuất sắc".
5. Bài 5: Thêm SV01, SV02 => in danh sách, tìm theo mã "SV02" => "Trần Bình".
6. Bài 6: 3 sản phẩm => tính thành tiền từng dòng và tổng giá trị kho 7,900,000.
7. Bài 7: TK001 (1,000,000) và TK002 (500,000) => nạp/rút/chuyển tiền, chặn số dư âm.
8. Bài 8: 3 nhân viên => bảng lương, tổng quỹ lương 42,000,000.
9. Bài 9: HCN 5x3 và 4.5x2.5 => chu vi, diện tích; kiểm tra cạnh âm bị bắt lỗi.
10. Bài 10: Menu CRUD: thêm / sửa / xóa / tìm theo mã, tên / sắp xếp / thống kê.
    Dữ liệu mẫu: 5 sinh viên SV01..SV05.

5. ĐIỂM NHẤN KỸ THUẬT (OOP)
--------------------------------------------------------------------------------
- Đóng gói: Tất cả thuộc tính dùng backing field private, kiểm tra tại setter.
  + HoTen: không được rỗng (string.IsNullOrWhiteSpace).
  + DiemTB: thuộc [0, 10].
  + DonGia / SoLuong / LuongCoBan / PhuCap: >= 0; HeSo > 0.
  + ChieuDai / ChieuRong: > 0.
  + SoDu: chỉ set private, đảm bảo không bao giờ âm.
- Constructor: mặc định tạo trạng thái hợp lệ; constructor đầy đủ dùng lại setter
  để tái sử dụng logic kiểm tra dữ liệu.
- Nghiệp vụ nằm trong lớp (TinhTuoi, XepLoai, ThanhTien, TinhLuong, ChuVi, DienTich),
  KHÔNG viết logic trong Main.
- Xử lý ngoại lệ: ArgumentException, ArgumentOutOfRangeException được ném tại setter
  và bắt gọn trong menu bài 10 để chương trình không bị crash.

6. KẾT QUẢ BUILD
--------------------------------------------------------------------------------
    dotnet build
    => Build succeeded. 0 Warning(s), 0 Error(s)
