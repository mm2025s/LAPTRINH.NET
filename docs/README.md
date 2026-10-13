# Học .NET — Lộ trình từ số 0 đến Backend Developer

Repo học tập cá nhân trên macOS (Apple Silicon).

- **Ngôn ngữ:** C#
- **Nền tảng:** .NET (bản LTS mới nhất)
- **Đích:** ASP.NET Core Web API + Entity Framework Core
- **Editor gợi ý:** VS Code + extension `C# Dev Kit` (hoặc Rider nếu có license)

---

## 1. Cài đặt môi trường

```bash
brew install dotnet          # cài .NET SDK
dotnet --version             # kiểm tra
dotnet --list-sdks
```

Nếu `dotnet` không có trong PATH sau khi cài:

```bash
echo 'export PATH="/opt/homebrew/opt/dotnet/bin:$PATH"' >> ~/.zshrc
source ~/.zshrc
```

VS Code extensions cần thiết:

```bash
code --install-extension ms-dotnettools.csdevkit
code --install-extension ms-dotnettools.csharp
```

---

## 2. Cấu trúc repo

```
NET/
├── HocNet.sln              # Solution gốc, chứa tất cả project bài học
├── README.md               # File này — lộ trình học
├── docs/                   # Ghi chú, cheat sheet
├── Bai01_HelloWorld/       # Console app đầu tiên
├── Bai02_BienVaKieuDuLieu/ # Biến, kiểu dữ liệu, ép kiểu
├── Bai03_CauTrucDieuKhien/ # if/else, switch, vòng lặp
├── Bai04_Ham/              # Hàm, tham số, ref/out
├── Bai05_OOP/              # Class, Object, kế thừa, interface, đa hình
├── Bai06_Collection/       # List, Dictionary, HashSet
├── Bai07_LINQ/             # LINQ cơ bản → nâng cao
├── Bai08_AsyncAwait/       # Bất đồng bộ
├── Bai09_FileJson/         # Đọc ghi file, System.Text.Json
├── Bai10_WebApi/           # ASP.NET Core Web API (Minimal API)
├── Bai11_WebApi_EFCore/    # Web API + Entity Framework Core + SQLite
└── Bai12_DuAnCuoiKhoa/     # Dự án tổng hợp (REST API hoàn chỉnh)
```

---

## 3. Lộ trình học (12 tuần)

### Giai đoạn 1 — C# căn bản (Tuần 1–3)

| Bài | Nội dung | Mục tiêu |
| --- | --- | --- |
| 01 | Hello World, `dotnet run`, cấu trúc project | Hiểu SDK, csproj, entry point |
| 02 | Biến, kiểu dữ liệu, `var`, ép kiểu, hằng số | Nắm value type vs reference type |
| 03 | `if/else`, `switch`, `for`, `foreach`, `while` | Viết được logic rẽ nhánh, lặp |
| 04 | Hàm, tham số mặc định, `ref`/`out`, nạp chồng | Tách code thành hàm sạch |

**Bài tập:** máy tính bỏ túi console, đoán số, xử lý chuỗi.

### Giai đoạn 2 — OOP & cấu trúc dữ liệu (Tuần 4–6)

| Bài | Nội dung | Mục tiêu |
| --- | --- | --- |
| 05 | Class, property, constructor, `static`, kế thừa, `abstract`, `interface`, đa hình, `record` | Mô hình hóa nghiệp vụ bằng class |
| 06 | `List<T>`, `Dictionary<K,V>`, `HashSet<T>`, `IEnumerable<T>` | Chọn đúng collection |
| 07 | LINQ: `Where`, `Select`, `OrderBy`, `GroupBy`, `Join`, `Any/All`, deferred execution | Truy vấn dữ liệu kiểu khai báo |
| 08 | `async/await`, `Task`, `HttpClient`, xử lý lỗi `try/catch` | Code bất đồng bộ đúng cách |
| 09 | Đọc/ghi file, `System.Text.Json`, serialize/deserialize | Lưu trữ dữ liệu đơn giản |

**Bài tập:** quản lý sinh viên, quản lý thư viện sách (console + lưu JSON).

### Giai đoạn 3 — Web & Backend (Tuần 7–10)

| Bài | Nội dung | Mục tiêu |
| --- | --- | --- |
| 10 | ASP.NET Core, Minimal API, routing, `IResult`, DI cơ bản | Chạy được API đầu tiên |
| 11 | REST chuẩn, DTO, validation, middleware, `appsettings.json` | API đúng chuẩn |
| 12 | EF Core: `DbContext`, migration, quan hệ 1-n/n-n, LINQ to Entities | Làm việc với database |
| 13 | SQLite/PostgreSQL, repository/service pattern, xử lý lỗi tập trung | Kiến trúc project thực tế |
| 14 | JWT Authentication & Authorization, Swagger/OpenAPI | Bảo mật API |

### Giai đoạn 4 — Dự án tổng hợp (Tuần 11–12)

Xây một REST API hoàn chỉnh, ví dụ **Quản lý công việc (Todo/Project Management)**:

- CRUD đầy đủ, phân trang, lọc, sắp xếp
- EF Core + migration + seed data
- JWT auth, phân quyền theo role
- Swagger docs, xử lý lỗi chuẩn (`ProblemDetails`)
- Unit test với xUnit
- (Tùy chọn) Docker + deploy

---

## 4. Lệnh .NET hay dùng

```bash
dotnet new console -o Bai01_HelloWorld     # tạo console app
dotnet new webapi -o Bai10_WebApi          # tạo web api
dotnet new sln -n HocNet                   # tạo solution
dotnet sln add Bai01_HelloWorld            # thêm project vào solution
dotnet run --project Bai01_HelloWorld      # chạy 1 project
dotnet build                               # build toàn solution
dotnet add package <TenPackage>            # thêm NuGet package
dotnet new gitignore                       # tạo .gitignore chuẩn
dotnet ef migrations add Init              # EF Core (cần cài dotnet-ef)
dotnet watch run --project Bai10_WebApi    # dev server tự reload
```

Cài EF Core CLI:

```bash
dotnet tool install --global dotnet-ef
export PATH="$PATH:$HOME/.dotnet/tools"
```

---

## 5. Ghi chú cá nhân

> Thêm ghi chú của bạn ở đây sau mỗi buổi học. Ví dụ: khái niệm chưa hiểu, câu hỏi cần tìm hiểu thêm.

- [ ] Ngày ___ / ___ :
