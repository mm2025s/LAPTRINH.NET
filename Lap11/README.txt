LAB 11 - BAI 10: CHUAN HOA XU LY LOI
=================================================================
Chuong 4 - IT3212 Lap trinh .NET
Nen tang: .NET Framework 4.8 - WinForms - CSDL QLSinhVien

PHAN DA HOAN THANH
  Chi Bai 10. Cac bai 1-9 chua co giao dien va tang DAL/BUS.
  SQL\Lab11_StoredProcedures.sql co san thu tuc CRUD (dung de gay loi that).

PHAN LOAI LOI (BUS\XuLyLoi.cs)
  Nhap lieu  LoiNhapLieu  Sua lai o nhap la xong, khong ghi nhat ky.
  Nghiep vu  LoiNghiepVu  THROW 5000x, trung UNIQUE (2601/2627), FK/CHECK (547).
  SQL        LoiCsdl      Cau ngan + ma tham chieu, chi tiet ghi nhat ky.
  He thong   Exception    Loi ngoai du kien, xu ly nhu loi SQL.

  Nhat ky: bin\Debug\logs\loi_yyyyMMdd.log (Number, Class, State,
  Procedure, Line, StackTrace), tra cuu theo ma tham chieu.

CACH CHAY
  1. Sao luu CSDL, roi trong thu muc Lap11\SQL chay:
       sqlcmd -S (localdb)\MSSQLLocalDB -E -f 65001 -i Lab11_StoredProcedures.sql
  2. Mo IT3212_Labs.sln, chon Lab11_CRUD_StoredProcedure, nhan F5.
  3. Tu kiem tra : Lab11_CRUD_StoredProcedure.exe --kiemtra
     Chup anh    : Lab11_CRUD_StoredProcedure.exe --chupanh

DU LIEU THU
  8 tinh huong trong BUS\TinhHuongLoi.cs. Khong tinh huong nao lam doi
  du lieu: moi lenh ghi deu bi CSDL tu choi.

KHOI PHUC CSDL
  SQL\Backup\QLSinhVien_TruocLab11.bak la ban sao luu truoc khi chay script
  (khong dua len git).
