================================================================================
HƯỚNG DẪN CHẠY DỰ ÁN VÀ THÔNG TIN BÀI TẬP LAB 04
Học phần: Lập trình .NET (IT3212) - Thiết kế WinForms và Điều Khiển Cơ Bản
Trường: Đại học Công nghệ Đông Á | Khoa Công nghệ Thông tin
================================================================================

1. CÁCH BUILD VÀ CHẠY CHƯƠNG TRÌNH
--------------------------------------------------------------------------------
Yêu cầu môi trường:
  - Visual Studio 2022 (Community/Professional/Enterprise)
  - .NET Framework 4.8 SDK (đã có sẵn trong Windows)
  - Windows 10/11 (WinForms chỉ chạy trên Windows)

Cách 1: Mở bằng Visual Studio 2022
  1. Mở file: IT3212_Labs.sln (ở thư mục gốc NET/)
     HOẶC mở trực tiếp: Lap4/Lab04_WinFormsBasic.csproj
  2. Nhấn F5 hoặc Ctrl+F5 để Build & Run
  3. Form Menu chính (frmMain) sẽ hiển thị với 6 nút bài tập

Cách 2: Build từ Developer Command Prompt
  cd /path/to/NET/Lap4
  msbuild Lab04_WinFormsBasic.csproj /p:Configuration=Debug
  bin\Debug\Lab04_WinFormsBasic.exe

LƯU Ý: Dự án này là Windows Forms App (.NET Framework 4.8).
        Không thể chạy bằng "dotnet run" như Console App (Lab 1-3).

2. MỤC TIÊU ĐÃ ĐẠT ĐƯỢC
--------------------------------------------------------------------------------
✅ Tạo ứng dụng Windows Forms và cấu hình thuộc tính Form.
✅ Sử dụng Label, TextBox, Button, ComboBox, DateTimePicker.
✅ Sử dụng CheckBox, RadioButton, PictureBox, CheckedListBox, ProgressBar.
✅ Đặt tên điều khiển theo quy ước: txt, lbl, btn, cbo, dtp, chk, rad, pic, clb.
✅ Bố trí giao diện với GroupBox, Panel theo vùng chức năng.
✅ Đọc dữ liệu từ control và hiển thị kết quả (MessageBox + Label).
✅ Xử lý sự kiện: Click, Load, ValueChanged, CheckedChanged, TextChanged, KeyDown.
✅ Validate dữ liệu đầu vào trước khi xử lý.
✅ Sử dụng OpenFileDialog để chọn file ảnh.
✅ Chuyển mô hình SinhVien từ Console lên giao diện WinForms.

3. CẤU TRÚC THƯ MỤC NGUỒN (Lap4)
--------------------------------------------------------------------------------
Lap4/
├── Lab04_WinFormsBasic.csproj         # Cấu hình dự án WinForms .NET Framework 4.8
├── Program.cs                         # Entry point - khởi chạy frmMain
│
├── Models/
│   └── SinhVien.cs                    # Lớp SinhVien mở rộng từ Lab 2 (thêm Khoa, GioiTinh...)
│
├── Forms/
│   ├── frmMain.cs + Designer          # Form menu chính - điều hướng 6 bài tập
│   ├── frmSinhVien.cs + Designer      # Bài 1-5: Thông tin sinh viên (đầy đủ controls)
│   ├── frmMayTinh.cs + Designer       # Bài 6: Máy tính 4 phép toán
│   ├── frmDangNhap.cs + Designer      # Bài 7: Form đăng nhập (toggle password)
│   ├── frmBMI.cs + Designer           # Bài 8: Tính chỉ số BMI + ProgressBar
│   ├── frmDangKyHocPhan.cs + Designer # Bài 9: Đăng ký học phần (CheckedListBox)
│   └── frmSanPham.cs + Designer       # Bài 10: Phiếu nhập sản phẩm
│
├── Images/                            # Thư mục chứa ảnh mẫu
├── BaoCao/                            # Thư mục ảnh chụp kết quả
├── README.txt                         # Tệp hướng dẫn này
└── BAO_CAO_KET_QUA.txt               # Báo cáo kết quả và dữ liệu thử

4. DỮ LIỆU THỬ NGHIỆM TỪNG BÀI
--------------------------------------------------------------------------------
BÀI 1-5 (frmSinhVien):
  Mã SV: SV001 | Họ tên: Nguyễn Văn An
  Ngày sinh: 15/03/2004 => Tuổi: 22
  Khoa: Công nghệ thông tin | Giới tính: Nam
  Sở thích: Đọc sách + Lập trình
  Ảnh: Chọn bất kỳ file JPG/PNG từ máy tính

BÀI 6 (frmMayTinh - Máy tính 4 phép toán):
  25 + 17 = 42
  100 - 38.5 = 61.5
  12.5 × 4 = 50
  100 ÷ 0 => Lỗi "Không thể chia cho 0"

BÀI 7 (frmDangNhap):
  Tài khoản hợp lệ: admin/123456 | user/password | sinhvien/sv2026
  Sai 3 lần => Tài khoản bị khóa
  Toggle mật khẩu: nhấn 👁️ hoặc CheckBox "Hiển thị mật khẩu"

BÀI 8 (frmBMI):
  Chiều cao: 170 cm | Cân nặng: 60 kg => BMI = 20.76 (Bình thường ✅)
  Chiều cao: 160 cm | Cân nặng: 40 kg => BMI = 15.63 (Thiếu cân nặng ⚠️)
  Chiều cao: 175 cm | Cân nặng: 100 kg => BMI = 32.65 (Béo phì 🚨)

BÀI 9 (frmDangKyHocPhan):
  Chọn: Lập trình .NET (3TC) + CSDL (3TC) + Lập trình Web (3TC) + Mạng (2TC)
  Tổng = 11 TC => cảnh báo thiếu (< 12 TC)
  Thêm Trí tuệ nhân tạo (3TC) => Tổng = 14 TC ✅ hợp lệ

BÀI 10 (frmSanPham):
  Mã: SP001 | Tên: Laptop Dell XPS 15 | Loại: Điện tử
  Số lượng: 5 | Đơn giá: 25,000,000
  => Thành tiền tự động: 125,000,000 VNĐ
  Ngày nhập: 20/10/2026

5. QUY ƯỚC ĐẶT TÊN CONTROL
--------------------------------------------------------------------------------
Tiền tố  | Loại control        | Ví dụ
---------|---------------------|------------------------
txt      | TextBox             | txtMaSV, txtHoTen
lbl      | Label               | lblTuoi, lblPhanLoai
btn      | Button              | btnHienThi, btnDong
cbo      | ComboBox            | cboKhoa, cboLoai
dtp      | DateTimePicker      | dtpNgaySinh, dtpNgayNhap
chk      | CheckBox            | chkDocSach, chkGhiNho
rad      | RadioButton         | radNam, radNu
pic      | PictureBox          | picAnh, picSanPham
clb      | CheckedListBox      | clbMonHoc
grp      | GroupBox            | grpThongTin, grpKetQua
pnl      | Panel               | pnlBtns, pnlChonNhanh
frm      | Form                | frmMain, frmSinhVien

6. ĐIỂM NHẤN KỸ THUẬT (WINFORMS)
--------------------------------------------------------------------------------
- Tất cả Form dùng Designer.cs tách biệt (đúng pattern Visual Studio).
- Validate dữ liệu với string.IsNullOrWhiteSpace, double.TryParse, int.TryParse.
- DateTimePicker.ValueChanged tính tuổi real-time (Bài 3).
- CheckedListBox + BeginInvoke để cập nhật tổng tín chỉ real-time (Bài 9).
- TextBox.TextChanged tính thành tiền real-time (Bài 10).
- Toggle PasswordChar bằng CheckBox + Button riêng (Bài 7).
- ProgressBar trực quan hóa chỉ số BMI (Bài 8).
- OpenFileDialog với Filter ảnh và SizeMode.Zoom (Bài 5, 10).
- Màu sắc phân loại (xanh/cam/đỏ) dựa trên ngưỡng giá trị (Bài 3, 8, 9).

7. KẾT QUẢ BUILD
--------------------------------------------------------------------------------
Mở trong Visual Studio 2022:
  Build => Build Succeeded | 0 Error(s) | 0 Warning(s)

================================================================================
