# Cheat sheet C# — dành cho người mới

## 1. Kiểu dữ liệu

| Kiểu | Kích thước | Ví dụ | Ghi chú |
| --- | --- | --- | --- |
| `int` | 4 byte | `int tuoi = 25;` | số nguyên 32-bit |
| `long` | 8 byte | `long danSo = 100_000_000;` | dấu `_` cho dễ đọc |
| `double` | 8 byte | `double pi = 3.14;` | số thực |
| `decimal` | 16 byte | `decimal gia = 19.99m;` | **dùng cho tiền tệ** |
| `bool` | 1 byte | `bool ok = true;` | |
| `char` | 2 byte | `char c = 'A';` | 1 ký tự, nháy đơn |
| `string` | — | `string s = "xin chào";` | bất biến (immutable) |
| `DateTime` | 8 byte | `DateTime.Now` | |

Value type (lưu giá trị): `int`, `double`, `bool`, `char`, `struct`, `enum`.
Reference type (lưu địa chỉ): `string`, `class`, `array`, `List<T>`, `object`.

```csharp
int a = 10;
int b = a;      // copy giá trị
b = 20;         // a vẫn = 10

var list1 = new List<int> { 1, 2 };
var list2 = list1;   // copy ĐỊA CHỈ (cùng trỏ 1 object)
list2.Add(3);        // list1 cũng có 3!
```

## 2. Ép kiểu

```csharp
double d = 9.8;
int i = (int)d;              // ép tường minh, mất phần thập phân → 9
int j = Convert.ToInt32(d);  // làm tròn → 10
string s = "123";
int k = int.Parse(s);        // lỗi nếu không hợp lệ
int.TryParse(s, out int m);  // an toàn, trả về bool
var n = (int)d;              // boxing/unboxing với object
```

## 3. Chuỗi

```csharp
string ten = "An";
string chao = $"Xin chào {ten}, {DateTime.Now:dd/MM/yyyy}";  // nội suy
string raw = """Xuống dòng "thoải mái" """;                  // raw string (C# 11+)

ten.Length; ten.ToUpper(); ten.Trim(); ten.Contains("A");
ten.Replace("A", "B"); ten.Split(','); string.Join(", ", arr);
string.IsNullOrWhiteSpace(ten);
```

## 4. OOP — bộ khung

```csharp
public interface IAnimal
{
    string Name { get; }
    void Speak();                       // mặc định public
}

public abstract class Animal : IAnimal
{
    public string Name { get; init; }   // init: gán khi khởi tạo
    protected Animal(string name) => Name = name;

    public virtual void Speak() => Console.WriteLine("...");
    public abstract string Sound { get; }
}

public sealed class Dog : Animal
{
    public Dog(string name) : base(name) { }
    public override string Sound => "Gâu gâu";
    public override void Speak() => Console.WriteLine($"{Name}: {Sound}");
}

// record: so sánh theo giá trị, tự sinh Equals/ToString
public record SanPham(string Ten, decimal Gia);
```

| Từ khóa | Ý nghĩa |
| --- | --- |
| `virtual` / `override` | cho phép / thực hiện ghi đè |
| `abstract` | bắt buộc lớp con phải triển khai |
| `sealed` | không cho kế thừa / ghi đè nữa |
| `static` | thuộc về class, không cần tạo object |
| `readonly` | gán 1 lần (trong constructor hoặc khai báo) |
| `const` | hằng số biên dịch, phải là literal |

## 5. LINQ

```csharp
var nums = new[] { 5, 3, 8, 1, 9, 2 };

var chan = nums.Where(x => x % 2 == 0);          // lọc
var binhPhuong = nums.Select(x => x * x);         // chiếu
var sapXep = nums.OrderByDescending(x => x);      // sắp xếp
var tong = nums.Sum();
var max = nums.Max();
var coSo8 = nums.Any(x => x == 8);
var tatCaDuong = nums.All(x => x > 0);
var dauTien = nums.First(x => x > 4);             // ném lỗi nếu không có
var coThe = nums.FirstOrDefault(x => x > 100);    // null nếu không có
var nhom = nums.GroupBy(x => x % 2);              // nhóm
var mang = chan.ToList();                         // THỰC THI ngay

// Deferred execution: `chan` chưa chạy cho tới khi duyệt (foreach/ToList)
```

## 6. Async / Await

```csharp
// ❌ Tránh: async void (chỉ dùng cho event handler)
// ❌ Tránh: .Result / .Wait() → dễ deadlock
// ✅ Luôn await tới cùng

public async Task<string> LayDuLieuAsync(CancellationToken ct = default)
{
    using var http = new HttpClient();
    var json = await http.GetStringAsync("https://api.example.com", ct);
    return json;
}

// Chạy song song
var t1 = LayAAsync();
var t2 = LayBAsync();
await Task.WhenAll(t1, t2);
```

## 7. Xử lý lỗi

```csharp
try
{
    var kq = int.Parse("abc");
}
catch (FormatException ex)
{
    Console.WriteLine($"Sai định dạng: {ex.Message}");
}
catch (Exception ex) when (ex is not OperationCanceledException)
{
    // bắt tổng quát, có filter
}
finally
{
    // luôn chạy (dọn tài nguyên)
}

// Ném lỗi đúng cách
if (gia < 0)
    throw new ArgumentOutOfRangeException(nameof(gia), "Giá không được âm");
```

## 8. Nullable

```csharp
string? ten = null;               // cho phép null
int doDai = ten?.Length ?? 0;     // ?. và ?? (toán tử null-coalescing)
ten ??= "mặc định";               // gán nếu null
if (ten is not null) { }
ArgumentNullException.ThrowIfNull(ten);
```

## 9. Cấu trúc chương trình

```csharp
// Top-level statements (Program.cs hiện đại)
using System.Text;

Console.WriteLine("Hello .NET!");

// Vẫn có thể khai báo hàm cục bộ
static int Cong(int a, int b) => a + b;
```

## 10. Lệnh CLI cần nhớ

```bash
dotnet new console -o TenProject
dotnet run --project TenProject
dotnet build
dotnet add package Newtonsoft.Json
dotnet add reference ../Lib/Lib.csproj
```
