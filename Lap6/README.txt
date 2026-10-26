# Lab 6 — Dữ liệu với DataGridView và mô hình 3 lớp

**Môn học:** IT3212 – Lập trình .NET
**Chương:** 4 (Dữ liệu và mô hình 3 lớp)
**Sinh viên:** 24000407
**Nền tảng:** C# / WinForms, .NET Framework 4.8 (không dùng NuGet)

Kế tiếp Lab 5 (xử lý sự kiện + đa Form). Lab 6 giữ nguyên giao diện phong cách Lab 5 nhưng tách
toàn bộ xử lý dữ liệu ra khỏi Form theo mô hình **DTO → DAL → BUS → GUI**.

---

## 1. Chạy chương trình

### Cách 1: Visual Studio 2022

1. Mở `Lab06_ThreeLayer.csproj`.
2. Chọn **Debug / AnyCPU**, nhấn **F5**.

### Cách 2: dòng lệnh (khi không có Visual Studio)

```powershell
powershell -ExecutionPolicy Bypass -File ..\scratch\build_lap6.ps1 -Run
```

Script gộp mã nguồn rồi biên dịch bằng `csc.exe` của .NET Framework 4.8.

### Tự chụp ảnh báo cáo

```powershell
powershell -ExecutionPolicy Bypass -File ..\scratch\build_lap6.ps1 -Shots
```

Ứng dụng tự mở từng Form, thao tác như người dùng và lưu ảnh vào `BaoCao/`.

---

## 2. Cấu trúc thư mục

```
Lap6/
├── DTO/
│   └── DoiTuong.cs        SinhVien, Khoa, SanPham, DanhMuc, KetQuaCSV, LoiNghiepVu
├── DAL/
│   └── DuLieuGiaLap.cs    SinhVienDAL, KhoaDAL, SanPhamDAL, DanhMucDAL (+ CSV)
├── BUS/
│   └── NghiepVu.cs        SinhVienBUS, KhoaBUS, SanPhamBUS, DanhMucBUS, DuLieuMau
├── GUI/
│   ├── frmMain            MDI + MenuStrip + StatusStrip + Dashboard
│   ├── frmSinhVien        DataGridView, CRUD, lọc, sắp xếp, CSV
│   ├── frmSanPham         tổng giá trị hàng tồn
│   ├── frmDanhMuc         TreeView danh mục - sản phẩm + ContextMenuStrip
│   ├── frmDanhMucSinhVien TreeView khoa - sinh viên
│   ├── frmSinhVienIcon    ListView + ImageList (biểu tượng tự vẽ)
│   ├── frmKienTruc        sơ đồ 3 lớp + chạy thử luồng gọi
│   ├── frmGioiThieu       trợ giúp
│   └── Helpers/           UiTheme, UiControls, FormBase, KiemTraChung, NhapLieu, DinhDang
├── BaoCaoTools/
│   └── TrinhChupAnh.cs    trình tự chụp ảnh báo cáo
├── Program.cs
├── Lab06_ThreeLayer.csproj
├── README.txt
└── BaoCao/                ảnh chụp + nhật ký + báo cáo .docx
```

**Chiều phụ thuộc:** `GUI → BUS → DAL → dữ liệu`.
GUI **không** tham chiếu `DAL`; DAL không biết BUS/GUI tồn tại.

---

## 3. Mười bài thực hành

| Bài | Nội dung | Lớp/Form chính |
|-----|----------|----------------|
| 1 | Tổ chức project 4 tầng, giải thích nhiệm vụ từng lớp | `DTO/`, `DAL/`, `BUS/`, `GUI/`, `frmKienTruc` |
| 2 | Hiển thị dữ liệu bằng DataGridView + BindingSource | `frmSinhVien`, `SinhVienBUS.GetAll()` |
| 3 | Thêm / sửa / xóa có kiểm tra dữ liệu | `SinhVienBUS.Add/Update/Delete`, `KiemTraDuLieu` |
| 4 | TreeView khoa → sinh viên | `SinhVienBUS.NhomTheoKhoa()`, `frmDanhMucSinhVien` |
| 5 | Tìm kiếm và sắp xếp | `SinhVienBUS.TimKiem/SapXep` |
| 6 | Dashboard + tổng giá trị hàng tồn | `SanPhamBUS.TongGiaTriTon`, `frmSanPham` |
| 7 | TreeView danh mục - sản phẩm + ContextMenuStrip | `DanhMucBUS`, `frmDanhMuc` |
| 8 | ListView + ImageList | `frmSinhVienIcon` |
| 9 | Nhập / xuất CSV | `SinhVienBUS.LuuCSV/DocCSV` → `SinhVienDAL` |
| 10 | Thống kê tổng hợp, giữ xác nhận thoát | `frmMain`, `frmMain_FormClosing` |

---

## 4. Quy tắc nghiệp vụ (viết một lần ở BUS)

`SinhVienBUS.KiemTraDuLieu` kiểm tra:

- Mã sinh viên: 2 – 10 ký tự, không trùng khi thêm mới.
- Họ tên: tối thiểu 2 ký tự, **không chứa chữ số**.
- Điểm trung bình: 0 – 10.
- Ngày sinh: không ở tương lai, tuổi 16 – 80.
- Email (nếu có): đúng định dạng `a@b.c`.
- Khoa: phải có trong danh mục khoa.

Khi vi phạm, BUS ném `LoiNghiepVu(truong, thongBao)`. GUI bắt lỗi này và gọi
`ErrorProvider.SetError()` đúng ô nhập — không đâu khác lặp lại quy tắc.

`DanhMucBUS.XoaDanhMuc` từ chối xóa danh mục còn sản phẩm.

---

## 5. Dữ liệu mẫu

3 khoa, mỗi khoa 3 sinh viên (đúng yêu cầu chuẩn bị):

| Mã | Họ tên | Khoa | Điểm |
|----|--------|------|------|
| SV01 | Nguyễn Văn An | Công nghệ thông tin | 8.7 |
| SV02 | Trần Thị Bình | Công nghệ thông tin | 7.4 |
| SV03 | Lê Hoàng Cường | Công nghệ thông tin | 6.2 |
| SV04 | Phạm Thu Dung | Kinh tế | 9.1 |
| SV05 | Hoàng Minh Đức | Kinh tế | 4.8 |
| SV06 | Vũ Thị Hằng | Kinh tế | 7.9 |
| SV07 | Đặng Quốc Khánh | Điện - Điện tử | 6.8 |
| SV08 | Bùi Lan Anh | Điện - Điện tử | 8.2 |
| SV09 | Ngô Tuấn Kiệt | Điện - Điện tử | 5.5 |

Sản phẩm: 6 mặt hàng thuộc 3 danh mục (Điện tử, Văn phòng phẩm, Gia dụng).

---

## 6. Bộ kiểm thử dữ liệu thử nhanh

| Thử | Kỳ vọng |
|-----|---------|
| Thêm `SV01` (đã có) | BUS báo *Trùng mã sinh viên* |
| Họ tên `Nguyễn 2` | BUS báo *Họ tên không được chứa chữ số* |
| Điểm `12` | BUS báo *Điểm trung bình phải thuộc [0, 10]* |
| Ngày sinh năm 2024 | BUS báo *Tuổi không hợp lệ (16 – 80)* |
| Xóa danh mục "Điện tử" | BUS báo *Danh mục còn sản phẩm, không thể xóa* |
| Xuất CSV rồi nhập lại | 9 dòng, 0 dòng lỗi, dữ liệu không đổi |

---

## 7. Điểm kỹ thuật đáng chú ý

- **Không dùng biến toàn cục cho dữ liệu:** `frmMain` tạo DAL một lần rồi tiêm vào BUS, các Form
  con nhận BUS qua constructor → cùng một kho dữ liệu, vẫn đúng mô hình 3 lớp.
- **DataGridView khai báo cột tường minh** (`AutoGenerateColumns = false`, `DataPropertyName`)
  để bus/BUS đổi tên thuộc tính là lỗi biên dịch, không lỗi thầm khi chạy.
- **Sắp xếp ở BUS, không ở lưới:** Form chỉ gửi tiêu chí (`"ten"`, `"diem"`, `"khoa"`).
- **ImageList không cần tệp ảnh:** biểu tượng ★ / ✓ / ! được `Graphics` vẽ ra `Bitmap` 40×40.
- **Chuẩn bị Lab 7:** thay ruột `DuLieuGiaLap.cs` bằng ADO.NET là xong, GUI và BUS không sửa.
