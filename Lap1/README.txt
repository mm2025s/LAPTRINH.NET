================================================================================
HƯỚNG DẪN CHẠY DỰ ÁN VÀ THÔNG TIN BÀI TẬP LAB 01
Học phần: Lập trình .NET (IT3212)
Trường: Đại học Công nghệ Đông Á
================================================================================

1. CÁCH BUILD VÀ CHẠY CHƯƠNG TRÌNH
--------------------------------------------------------------------------------
Yêu cầu môi trường: Đã cài .NET 8.0 SDK trở lên.

Mở Terminal và chuyển vào thư mục Lap1:
    cd /Users/mmo2025s/mmo2025/NET/Lap1

Để build kiểm tra lỗi:
    dotnet build

Để chạy chương trình:
    dotnet run

Sau khi chạy, giao diện Console sẽ hiển thị menu chọn bài tập từ 1 đến 10,
nhập 0 để thoát chương trình.

2. CÁC TỆP NGUỒN ĐÃ HOÀN THÀNH (10/10 BÀI TẬP)
--------------------------------------------------------------------------------
1. Bai01_ThongTinCaNhan.cs:
   - Nhập họ tên, năm sinh; tính tuổi gần đúng = DateTime.Now.Year - namSinh.
   - Dữ liệu thử: Họ tên = "Nguyễn Văn A", Năm sinh = 2002 -> Tuổi: 24.

2. Bai02_TinhHinhTru.cs:
   - Nhập r, h (kiểm tra > 0); tính Sxq = 2 * PI * r * h, V = PI * r^2 * h.
   - Dữ liệu thử: r = 3.5, h = 10 -> Sxq = 219.91; V = 384.85.

3. Bai03_XepLoaiHocTap.cs:
   - Nhập điểm (0..10); xếp loại Xuất sắc, Giỏi, Khá, Trung bình, Yếu.
   - Dữ liệu thử: Điểm 8.5 -> Giỏi; Điểm 11 -> Báo lỗi.

4. Bai04_MenuTongVaBangCuuChuong.cs:
   - Menu lặp cho phép tính tổng 1..n hoặc in bảng cửu chương 7.
   - Dữ liệu thử: n = 5 -> Tổng = 15; Chọn 2 -> In bảng cửu chương 7.

5. Bai05_XuLyNgoaiLePhapChia.cs:
   - Nhập 2 số nguyên a, b; chia a / b; bắt lỗi FormatException và DivideByZeroException.
   - Dữ liệu thử: a = 10, b = 2 -> 5; a = 10, b = 0 -> "Không thể chia cho 0".

6. Bai06_GiaiPhuongTrinhBacHai.cs:
   - Giải ax^2 + bx + c = 0; xét đầy đủ trường hợp a=0, delta < 0, delta = 0, delta > 0.
   - Dữ liệu thử: a = 1, b = -3, c = 2 -> x1 = 2.00, x2 = 1.00.

7. Bai07_ThongKeDaySo.cs:
   - Nhập n số nguyên; tính tổng, trung bình, max, min, đếm số chẵn.
   - Dữ liệu thử: n = 5; dãy {2, 5, 8, 1, 4} -> Tổng = 20, TB = 4.00, Max = 8, Min = 1, Chẵn = 3.

8. Bai08_KiemTraSoNguyenTo.cs:
   - In các số nguyên tố không vượt quá n; tối ưu kiểm tra căn bậc hai.
   - Dữ liệu thử: n = 20 -> 2 3 5 7 11 13 17 19.

9. Bai09_DocSo0Den99.cs:
   - Dùng switch đọc số nguyên từ 0 đến 99 bằng tiếng Việt (xử lý mười, mốt, tư, lăm).
   - Dữ liệu thử: 25 -> "hai mươi lăm"; 21 -> "hai mươi mốt"; 14 -> "mười bốn".

10. Bai10_VeHinhKyTu.cs:
    - Menu vẽ tam giác vuông đặc, tam giác vuông rỗng và hình vuông rỗng bằng ký tự *.
    - Dữ liệu thử: Chọn 1 với h = 3, chọn 2 với h = 3, chọn 3 với a = 3.

3. TỆP BÁO CÁO KẾT QUẢ
--------------------------------------------------------------------------------
Xem tệp "BAO_CAO_KET_QUA.txt" trong thư mục Lap1 để xem chi tiết kết quả chạy
và chụp màn hình cho từng bài tập.
