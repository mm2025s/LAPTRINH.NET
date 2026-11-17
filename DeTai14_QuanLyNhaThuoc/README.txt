DE TAI 14 - XAY DUNG PHAN MEM QUAN LY NHA THUOC BANG CONG NGHE .NET
=====================================================================
Mon: Lap trinh .NET (IT3212) - Nguyen Truong Giang - 24000407
Nen tang: C# - .NET Framework 4.8 - WinForms - SQL Server - ADO.NET

1. CAI DAT VA CHAY
  a) Visual Studio 2022: mo QuanLyNhaThuoc.sln, nhan F5.
     Lan chay dau, neu may chua co CSDL QLNhaThuoc, chuong trinh hoi va tu
     tao CSDL + du lieu mau tu thu muc SQL (chep vao bin\Debug\SQL khi build).
  b) VS Code: File > Open Folder... chon thu muc DeTai14_QuanLyNhaThuoc,
     cai extension "C#" (ms-dotnettools.csharp) theo goi y, nhan F5.
     Ctrl+Shift+B = build (dotnet build). Terminal > Run Task co san:
     "kiem thu (--kiemtra)" va "tao lai CSDL (--caidat)".
  c) Dong lenh / thu cong:
       dotnet build QuanLyNhaThuoc.csproj
       bin\Debug\QuanLyNhaThuoc.exe --caidat      (tao lai CSDL + du lieu mau)
     hoac chay 2 script trong SQL\ bang SSMS / sqlcmd.
     CANH BAO: script 01 xoa va tao lai toan bo bang cua CSDL QLNhaThuoc.
  d) Khong dung LocalDB: sua Data Source trong App.config (vd .\SQLEXPRESS).
  Yeu cau: Windows 10/11, .NET Framework 4.8, SQL Server Express hoac LocalDB.

2. TAI KHOAN MAU
  admin  / admin123  (Quan ly - day du chuc nang)
  duocsi / 123456    (Duoc si - ban hang, thuoc, khach hang, hoa don)

3. CHUC NANG
  Dang nhap (mat khau SHA-256 + salt), phan quyen 2 vai tro
  Ban hang: gio hang, kiem tra ton kho / han dung / thuoc ke don,
            hoa don ghi trong 1 TRANSACTION (Table-Valued Parameter)
  Nhap hang theo lo (SL, gia nhap, han dung), canh bao kho
  Danh muc thuoc, nhom thuoc, nha cung cap, khach hang, nhan vien
  Tra cuu hoa don, thong ke doanh thu theo ngay, top 10 thuoc ban chay
  Xu ly loi tap trung, nhat ky bin\Debug\logs\loi_yyyyMMdd.log

4. KIEM TRA VA BAO CAO
  QuanLyNhaThuoc.exe --kiemtra  -> BaoCao\kiem_thu.txt (22/22 dat)
  QuanLyNhaThuoc.exe --chupanh  -> BaoCao\HinhAnh\MH*.png
  QuanLyNhaThuoc.exe --chupthucte -> BaoCao\HinhAnh\TH*.png (chay that, co ban hang that)
  python Tools\ve_so_do.py      -> BaoCao\HinhAnh\SD*.png (8 so do)
  python Tools\tao_bao_cao.py   -> BaoCao\BaoCao_DeTai14_...docx
  Ban PDF: BaoCao\BaoCao_DeTai14_QuanLyNhaThuoc_24000407.pdf (36 trang)

5. CAU TRUC
  SQL\    script CSDL + du lieu mau     DTO\  doi tuong, ngoai le
  DAL\    goi Stored Procedure          BUS\  nghiep vu, xu ly loi
  GUI\    cua so kieu portal + 11 trang          Tools\ kiem thu, chup anh, bao cao
