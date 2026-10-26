# Lab 5 — Xử lý sự kiện, kiểm tra dữ liệu và ứng dụng đa Form

Môn: IT3212 – Lập trình .NET  |  Chương 3  |  Project: `Lab05_MultiForm`  |  .NET Framework 4.8 (WinForms)

## 1. Cách chạy

1. Mở `IT3212_Labs.sln` bằng Visual Studio 2022 → chuột phải `Lab05_MultiForm` → **Set as Startup Project** → `F5`.
2. Hoặc chạy trực tiếp: `Lap5\bin\Debug\Lab05_MultiForm.exe` (sau khi build).
3. Tự động sinh ảnh báo cáo (không cần thao tác tay):

```
Lab05_MultiForm.exe --screenshots
```

Ảnh và nhật ký được ghi vào thư mục `Lap5\BaoCao\`:
- `Bai05_01_frmMain.png` … `Bai05_14_*.png`: ảnh kết quả từng bài.
- `nhat_ky_chup_anh.txt`: nhật ký mô tả dữ liệu đã nhập và kết quả từng kịch bản.

## 2. Cấu trúc mã nguồn

```
Lap5/
├── Program.cs                 // Entry point, tham số --screenshots
├── Lab05_MultiForm.csproj
├── Models/
│   ├── SinhVien.cs            // Lớp SinhVien: thuộc tính có kiểm tra, Clone(), TinhTuoi, XepLoai
│   └── Khoa.cs                // Lớp Khoa + DuLieuMau (danh sách khoa, 5 sinh viên mẫu)
├── Helpers/
│   ├── KiemTra.cs             // Hàm dùng chung: kiểm tra mã, họ tên, điểm, ngày sinh; chặn nhập số
│   ├── UiTheme.cs             // Bảng màu, font, hiệu ứng gradient, bóng đổ
│   ├── UiControls.cs          // GradientPanel, CardPanel, AccentButton, BadgeLabel, UiFactory
│   ├── RoundedPictureBox.cs   // Ảnh đại diện bo tròn
│   └── FormBase.cs            // Nền gradient cho Form + CaptureToBitmap
├── Forms/
│   ├── frmMain.cs             // Bài 5, 9, 10: MDI + MenuStrip + StatusStrip + Dashboard
│   ├── frmSinhVien.cs         // Bài 1, 2, 3: nhập liệu, ErrorProvider, KeyPress, TextChanged
│   ├── frmChiTietSinhVien.cs  // Bài 4: truyền dữ liệu bằng constructor + thuộc tính
│   ├── frmDanhSach.cs         // Bài 4, 5: danh sách sinh viên, mở chi tiết qua sự kiện
│   ├── frmDangNhap.cs         // Bài 6: đăng nhập, sai 3 lần khóa 30 giây
│   ├── frmTimKiem.cs          // Bài 7: hộp thoại tìm kiếm
│   ├── frmChonKhoa.cs         // Bài 8: hộp thoại chọn khoa
│   ├── frmKiemTraDuLieu.cs    // Bài 10: minh họa hàm kiểm tra dùng chung
│   └── frmGioiThieu.cs        // Trợ giúp
└── BaoCaoTools/TrinhChupAnh.cs // Bộ tự động chụp ảnh báo cáo
```

## 3. Dữ liệu thử

| Bài | Thao tác | Kết quả mong đợi |
|-----|----------|------------------|
| 1 | Mã `sv`, họ tên trống, điểm `12` → **Lưu** | ErrorProvider hiện lỗi ở 3 ô, không lưu |
| 1 | `SV10`, `  nguyễn   văn   an  `, `8.5` | Huy hiệu "SẴN SÀNG LƯU", tên tự chuẩn hóa |
| 2 | Gõ `9.755` vào ô điểm | KeyPress chỉ nhận `9.75` (2 chữ số lẻ) |
| 3 | Nhập dần từng ô | Preview cập nhật ngay khi TextChanged; đổi khoa → SelectedIndexChanged |
| 4 | Chọn một SV → **Xem chi tiết** | frmChiTietSinhVien nhận dữ liệu qua constructor, sửa và lưu ngược bằng thuộc tính |
| 5 | Mở 2 lần cùng một form | Hệ thống kích hoạt form đã mở, không tạo trùng |
| 6 | `admin` / `admin123` | Đăng nhập thành công |
| 6 | Sai mật khẩu 3 lần | Nút bị khóa, đếm ngược 30 giây |
| 7 | Từ khóa `nguyễn` | 1 kết quả: SV01 Nguyễn Văn An |
| 8 | Chọn khoa "Kinh tế" | Trả kết quả về form gọi qua thuộc tính |
| 9 | Đóng frmMain → chọn **No** | `e.Cancel = true`, ứng dụng không thoát |
| 10 | Xem Dashboard | 4 phiếu chỉ số (tổng SV, điểm TB, SV giỏi, khoa) + đồng hồ StatusStrip |

## 4. Điểm nhấn kỹ thuật

- **ErrorProvider** thay cho MessageBox khi kiểm tra từng ô; `ValidateInput()` gom toàn bộ kiểm tra.
- **Hàm dùng chung** `KiemTra.*` dùng cho nhiều Form, tránh lặp mã kiểm tra.
- **KeyPress** chặn ký tự không hợp lệ ngay khi gõ (chỉ số, một dấu thập phân, giới hạn chữ số lẻ).
- **Truyền dữ liệu 3 cách**: constructor (`frmChiTietSinhVien(SinhVien)`), thuộc tính (`SinhVien`, `KhoaDuocChon`), delegate/event (`SinhVienDaLuu`, `MoChiTiet`).
- **MDI + MenuStrip**: `OpenChildForm` kiểm tra form con đã mở theo `Tag`/kiểu; menu "Window" tự liệt kê form con.
- **FormClosing**: xác nhận thoát; `e.CloseReason` phân biệt đóng bởi người dùng và tắt máy.

## 5. Ảnh báo cáo

Toàn bộ ảnh nằm trong `Lap5/BaoCao/` và được sinh tự động bởi chế độ `--screenshots`
(dùng `Form.DrawToBitmap` nên vẫn chụp được khi phiên làm việc không có màn hình tương tác).
