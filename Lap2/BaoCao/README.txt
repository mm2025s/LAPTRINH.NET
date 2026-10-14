================================================================================
THƯ MỤC BÁO CÁO - LAB 02: LẬP TRÌNH HƯỚNG ĐỐI TƯỢNG
Học phần: Lập trình .NET (IT3212)
Sinh viên: Nguyễn Trường Giang - MSSV: 24000407
================================================================================

1. NỘI DUNG THƯ MỤC
--------------------------------------------------------------------------------
BaoCao/
├── BaoCao_Lab02_24000407.docx    # BÁO CÁO WORD hoàn chỉnh (nộp cho giảng viên)
├── HinhAnh/                      # Ảnh chụp kết quả chạy từng bài tập
│   ├── Bai01.png ... Bai10.png   # Ảnh kết quả 10 bài tập
│   └── Build.png                 # Ảnh kết quả build solution
└── README.txt                    # Tệp mô tả này

2. BÁO CÁO WORD (BaoCao_Lab02_24000407.docx)
--------------------------------------------------------------------------------
- Trang bìa: thông tin trường, khoa, môn học, họ tên và MSSV sinh viên.
- Nội dung đầy đủ 8 mục:
    1. Mục tiêu cần đạt được
    2. Công cụ và dữ liệu chuẩn bị
    3. Liên kết trong chuỗi bài lab
    4. Cấu trúc mã nguồn dự án
    5. Bài tập có hướng dẫn (Bài 1 - 5) — kèm ảnh kết quả
    6. Bài tập tự thực hiện (Bài 6 - 10) — kèm ảnh kết quả
    7. Kết quả build và kiểm thử
    8. Kết luận và đánh giá
- Định dạng: chữ màu đen, font Times New Roman 13pt, khổ A4, đề mục rõ ràng,
  có bảng thông tin sinh viên và bảng cấu trúc mã nguồn.

3. ẢNH KẾT QUẢ (HinhAnh/)
--------------------------------------------------------------------------------
Mỗi bài tập có một ảnh chụp kết quả chạy chương trình (dạng cửa sổ terminal),
thể hiện đúng lệnh `dotnet run`, lựa chọn menu và kết quả xuất ra màn hình.

4. CÁCH CHẠY LẠI CHƯƠNG TRÌNH
--------------------------------------------------------------------------------
    cd /Users/mmo2025s/mmo2025/NET/Lap2
    dotnet build        # Kiểm tra biên dịch
    dotnet run          # Chạy chương trình

Sau khi chạy, nhập số từ 1 đến 10 để chọn bài tập, nhập 0 để thoát.

5. KẾT QUẢ BUILD
--------------------------------------------------------------------------------
    dotnet build IT3212_Labs.sln
    => Build succeeded. 0 Warning(s), 0 Error(s)
