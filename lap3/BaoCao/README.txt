================================================================================
THƯ MỤC BÁO CÁO - LAB 03: KẾ THỪA, ĐA HÌNH, LỚP TRỪU TƯỢNG VÀ INTERFACE
Học phần: Lập trình .NET (IT3212)
Sinh viên: Nguyễn Trường Giang - MSSV: 24000407
================================================================================

1. NỘI DUNG THƯ MỤC
--------------------------------------------------------------------------------
BaoCao/
├── BaoCao_Lab03_24000407.docx    # BÁO CÁO WORD hoàn chỉnh (nộp cho giảng viên)
├── HinhAnh/                      # Ảnh chụp kết quả chạy từng bài tập
│   ├── Bai01.png ... Bai10.png   # Ảnh kết quả 10 bài tập
│   ├── Build.png                 # Ảnh kết quả build dự án
│   ├── SoDoLop_KeThua.png        # Sơ đồ lớp: kế thừa và interface
│   └── SoDoLop_TruuTuong.png     # Sơ đồ lớp: lớp trừu tượng NguoiHoc
└── README.txt                    # Tệp mô tả này

2. BÁO CÁO WORD (BaoCao_Lab03_24000407.docx)
--------------------------------------------------------------------------------
- Trang bìa: thông tin trường, khoa, môn học, họ tên và MSSV sinh viên.
- Nội dung đầy đủ 8 mục:
    1. Mục tiêu cần đạt được
    2. Công cụ và dữ liệu chuẩn bị
    3. Liên kết trong chuỗi bài lab
    4. Cấu trúc mã nguồn dự án (kèm 2 sơ đồ lớp UML)
    5. Bài tập có hướng dẫn (Bài 1 - 5) — mã nguồn chính + ảnh kết quả
    6. Bài tập tự thực hiện (Bài 6 - 10) — mã nguồn chính + ảnh kết quả
    7. Kết quả build và kiểm thử — bảng tổng hợp kết quả 10 bài
    8. Kết luận và đánh giá
- Định dạng: chữ màu đen, font Times New Roman 13pt, khổ A4, đề mục rõ ràng,
  mỗi mục lớn bắt đầu từ trang mới, có tiêu đề đầu trang và số trang chân trang.
- Mã nguồn trong báo cáo dùng font Courier New 10pt, đặt trong khung nền xám.

3. LƯU Ý VỀ FONT TIẾNG VIỆT (đã khắc phục lỗi font)
--------------------------------------------------------------------------------
Trên macOS, các font đơn cách mặc định (Menlo, Monaco, SF Mono) KHÔNG chứa đầy
đủ bộ ký tự tiếng Việt: chúng thiếu 28 ký tự có dấu dùng trong bài lab này
(ả, ẻ, ỉ, ỏ, ủ, ở, ử, ấ, ầ, ế, ề, ố, ồ, ổ, ỗ, ắ, ẳ ...), nên khi chụp ảnh kết
quả các ký tự này hiện thành ô vuông (□).

Cách khắc phục đã áp dụng:
- Ảnh kết quả (HinhAnh/) được tạo lại bằng font Courier New — đây là font đơn
  cách (monospace) thật sự và có đầy đủ bộ ký tự tiếng Việt. Đã kiểm tra bằng
  công cụ fontTools trước khi xuất ảnh.
- Báo cáo Word khai báo font cho cả 4 thuộc tính w:ascii, w:hAnsi, w:cs và
  w:eastAsia trên mọi đoạn văn bản, để Word không tự thay thế bằng font thiếu
  ký tự tiếng Việt.

4. ẢNH KẾT QUẢ (HinhAnh/)
--------------------------------------------------------------------------------
Mỗi bài tập có một ảnh chụp kết quả chạy chương trình (dạng cửa sổ terminal),
thể hiện đúng lệnh `dotnet run`, lựa chọn menu và kết quả xuất ra màn hình.
Hai sơ đồ lớp UML được vẽ theo đúng ký pháp: nét liền + tam giác rỗng cho quan
hệ kế thừa, nét đứt + tam giác rỗng cho quan hệ thực thi interface.

5. CÁCH CHẠY LẠI CHƯƠNG TRÌNH
--------------------------------------------------------------------------------
    cd /Users/mmo2025s/mmo2025/NET/lap3
    dotnet build        # Kiểm tra biên dịch
    dotnet run          # Chạy chương trình

Sau khi chạy, nhập số từ 1 đến 10 để chọn bài tập, nhập 0 để thoát.

6. KẾT QUẢ BUILD
--------------------------------------------------------------------------------
    cd /Users/mmo2025s/mmo2025/NET/lap3 && dotnet build
    => Build succeeded. 0 Warning(s), 0 Error(s)
