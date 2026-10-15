================================================================================
HƯỚNG DẪN CHẠY DỰ ÁN VÀ THÔNG TIN BÀI TẬP LAB 03
Học phần: Lập trình .NET (IT3212) - Kế thừa, Đa hình, Lớp trừu tượng và Interface
Trường: Đại học Công nghệ Đông Á
================================================================================

1. CÁCH BUILD VÀ CHẠY CHƯƠNG TRÌNH
--------------------------------------------------------------------------------
Yêu cầu môi trường: Đã cài .NET 8.0 SDK trở lên.

Mở Terminal và chuyển vào thư mục lap3:
    cd /Users/mmo2025s/mmo2025/NET/lap3

Để build kiểm tra lỗi:
    dotnet build

Để chạy chương trình:
    dotnet run

Sau khi chạy, giao diện Console sẽ hiển thị menu chọn bài tập từ 1 đến 10,
nhập 0 để thoát chương trình.

2. MỤC TIÊU ĐÃ ĐẠT ĐƯỢC
--------------------------------------------------------------------------------
- Mô hình hóa quan hệ kế thừa (is-a) và sử dụng từ khóa base để gọi constructor
  lớp cha (SinhVien : Nguoi).
- Ghi đè phương thức bằng virtual/override và khai thác tính đa hình tại thời
  điểm chạy (MoTa(), TinhHocPhi(), TinhLuong(), TinhThoiGian()).
- Thiết kế lớp trừu tượng (NguoiHoc, NhanVien, TaiLieu, PhuongTien) và interface
  (IValidatable, IPrintable, IXuatDuLieu, IMove, IPayment).
- Vận dụng nguyên lý OOP vào bài toán quản lý: danh sách đa hình, tìm kiếm,
  thống kê theo loại, xuất dữ liệu.

3. CẤU TRÚC THƯ MỤC NGUỒN (lap3)
--------------------------------------------------------------------------------
lap3/
├── Lab03_OOPAdvanced.csproj          # Cấu hình dự án .NET 8.0 Console
├── Lab03_OOPAdvanced.cs              # Menu chính điều hướng chọn chạy từng bài
│
├── [INTERFACE & LỚP CƠ SỞ]
├── Interfaces.cs                     # IValidatable, IPrintable, IXuatDuLieu
├── Nguoi.cs                          # Lớp cơ sở Nguoi (virtual MoTa)
├── SinhVien.cs                       # SinhVien : Nguoi, thực thi 3 interface
│
├── [LỚP TRỪU TƯỢNG & NGHIỆP VỤ]
├── NguoiHoc.cs                       # abstract NguoiHoc + ChinhQuy/LienThong/CaoDang
├── QuanLyNguoiHoc.cs                 # Tầng nghiệp vụ danh sách đa hình
├── NhanVien.cs                       # abstract NhanVien + VanPhong/KinhDoanh/GiangVien
├── PhuongTien.cs                     # interface IMove + XeMay/OTo/XeDap
├── TaiLieu.cs                        # abstract TaiLieu + Sach/TapChi/LuanVan
├── ThanhToan.cs                      # interface IPayment + TienMat/ChuyenKhoan/ViDienTu
├── SinhVienMoRong.cs                 # Bài 10: đa loại SV + QuanLySinhVienNangCao
│
├── [CÁC BÀI TẬP CÓ HƯỚNG DẪN]
├── Bai01_KeThua.cs                   # Bài 1: Kế thừa Nguoi - SinhVien (base)
├── Bai02_GhiDePhuongThuc.cs          # Bài 2: Ghi đè MoTa() virtual/override
├── Bai03_LopTruuTuong.cs             # Bài 3: Lớp trừu tượng NguoiHoc
├── Bai04_Interface.cs                # Bài 4: IValidatable & IPrintable
├── Bai05_DanhSachDaHinh.cs           # Bài 5: Danh sách đa hình
│
├── [CÁC BÀI TẬP TỰ THỰC HIỆN]
├── Bai06_HeThongNhanVien.cs          # Bài 6: Hệ thống nhân viên
├── Bai07_QuanLyPhuongTien.cs         # Bài 7: Quản lý phương tiện
├── Bai08_QuanLyThuVien.cs            # Bài 8: Quản lý tài liệu thư viện
├── Bai09_HeThongThanhToan.cs         # Bài 9: Hệ thống thanh toán
├── Bai10_NangCapQuanLySinhVien.cs    # Bài 10: Nâng cấp quản lý sinh viên
│
├── README.txt                        # Tệp hướng dẫn này
├── BAO_CAO_KET_QUA.txt               # Tệp báo cáo kết quả chạy từng bài
│
└── BaoCao/                           # THƯ MỤC BÁO CÁO NỘP GIẢNG VIÊN
    ├── BaoCao_Lab03_24000407.docx    # Báo cáo Word hoàn chỉnh (8 mục)
    ├── README.txt                    # Mô tả thư mục báo cáo
    └── HinhAnh/                      # Ảnh kết quả chạy + sơ đồ lớp UML
        ├── Bai01.png ... Bai10.png   # Ảnh kết quả 10 bài tập
        ├── Build.png                 # Ảnh kết quả build (0 warning, 0 error)
        ├── SoDoLop_KeThua.png        # Sơ đồ lớp: kế thừa và interface
        └── SoDoLop_TruuTuong.png     # Sơ đồ lớp: lớp trừu tượng NguoiHoc

4. DỮ LIỆU THỬ NGHIỆM TỪNG BÀI
--------------------------------------------------------------------------------
1. Bài 1: Nguoi("Nguyễn Văn A", 08/04/1990) và SinhVien("SV01", "Nguyễn An",
   12/05/2005, 8.2, "CNTT") => chứng minh quan hệ is-a và ép kiểu an toàn.
2. Bài 2: So sánh Nguoi.MoTa() và SinhVien.MoTa() (có gọi base.MoTa());
   danh sách List<Nguoi> gồm 4 phần tử => mỗi phần tử in theo lớp thực tế.
3. Bài 3: Chính quy 18 tín chỉ (450.000 đ/TC) và Liên thông 15 tín chỉ
   (520.000 đ/TC) => tổng 15.900.000 đ; thử số tín chỉ -5 => bắt lỗi.
4. Bài 4: 3 sinh viên: hợp lệ; thiếu mã; điểm 12 => chỉ 1 hợp lệ.
5. Bài 5: List<NguoiHoc> gồm An (18 TC), Bình (15 TC), Cường (20 TC + phí
   thực hành) => tổng học phí và sắp xếp giảm dần.
6. Bài 6: 6 nhân viên (2 văn phòng, 2 kinh doanh, 2 giảng viên) => bảng lương
   và tổng quỹ lương, thống kê theo chức danh.
7. Bài 7: Xe máy 60 km/h (1.200 đ/km), Ô tô 100 km/h (4.500 đ/km),
   Xe đạp 20 km/h (100 đ/km) trên hành trình 150 km; thử quãng đường âm.
8. Bài 8: 2 sách, 2 tạp chí, 2 luận văn => tìm theo tiêu đề "C#", lọc theo
   kiểu, thống kê và in thời hạn mượn theo loại.
9. Bài 9: 3 phương thức x 3 mức tiền (500k, 15tr, 25tr) => ví điện tử từ chối
   giao dịch 25tr do vượt hạn mức 20tr.
10. Bài 10: 5 sinh viên 3 loại (Chính quy, Liên thông, Vừa làm vừa học) =>
    tìm theo mã/tên, lọc theo loại, thống kê %, tổng học phí, sắp xếp, xuất
    CSV và JSON.

5. ĐIỂM NHẤN KỸ THUẬT (OOP NÂNG CAO)
--------------------------------------------------------------------------------
- Kế thừa: SinhVien : Nguoi dùng base(ten, ngaySinh); SinhVienMoRong : SinhVien
  cho thấy kế thừa nhiều tầng (Nguoi -> SinhVien -> SinhVienMoRong).
- virtual/override: Nguoi.MoTa() là virtual, SinhVien.MoTa() override và gọi
  base.MoTa() để tái sử dụng hành vi lớp cha thay vì viết lại.
- abstract: NguoiHoc, NhanVien, TaiLieu, PhuongTien, SinhVienMoRong đều không
  thể khởi tạo trực tiếp; phương thức trừu tượng buộc lớp con cài đặt.
- interface: IValidatable, IPrintable, IXuatDuLieu, IMove, IPayment chỉ khai
  báo hợp đồng; lớp cài đặt chi tiết. Có thể gọi thành viên qua tham chiếu
  interface mà không cần biết lớp cụ thể.
- Đa hình: Một List<NguoiHoc> / List<NhanVien> / List<IMove> chứa nhiều lớp con;
  lời gọi TinhHocPhi()/TinhLuong()/TinhThoiGian() được quyết định tại thời điểm
  chạy dựa trên kiểu thực tế của đối tượng.
- Đóng gói vẫn được giữ vững: mọi thuộc tính dùng backing field private và
  kiểm tra dữ liệu tại setter (kế thừa từ Lab 2).

6. KẾT QUẢ BUILD
--------------------------------------------------------------------------------
    dotnet build
    => Build succeeded. 0 Warning(s), 0 Error(s)
