LAB 10 - DATAADAPTER, DATATABLE, DATASET VA DU LIEU NGAT KET NOI
=================================================================
Chuong 4 - IT3212 Lap trinh .NET
Sinh vien : Nguyen Truong Giang - MSSV 24000407
CSDL      : QLSinhVien (SQL Server)
Nen tang  : .NET Framework 4.8 - WinForms - Visual Studio 2022

-----------------------------------------------------------------
1. NOI DUNG
-----------------------------------------------------------------
Chuyen tu mo hinh KET NOI (SqlDataReader - Lab 9) sang mo hinh NGAT KET
NOI: nap du lieu mot lan vao bo nho, xu ly tren do, chi ghi nguoc CSDL
khi thuc su can.

Nam bai co huong dan (1-5) bam theo mau trong de cuong; nam bai tu thuc
hien (6-10) mo rong dung ky thuat do.

  Bai 1  Fill DataTable bang SqlDataAdapter
  Bai 2  DataSet chua nhieu bang (Khoa + SinhVien)
  Bai 3  DataRelation Khoa - SinhVien + GetChildRows
  Bai 4  Loc va sap xep bang DataView (RowFilter + Sort)
  Bai 5  Cap nhat bang SqlCommandBuilder (adapter.Update)
  Bai 6  Tao DataTable thu cong (Expression + rang buoc)   [tu thuc hien]
  Bai 7  Theo doi RowState truoc khi Update                [tu thuc hien]
  Bai 8  AcceptChanges va RejectChanges                    [tu thuc hien]
  Bai 9  Luu va doc DataSet bang XML                       [tu thuc hien]
  Bai 10 Hop nhat du lieu bang DataTable.Merge             [tu thuc hien]

-----------------------------------------------------------------
2. CAU TRUC PROJECT
-----------------------------------------------------------------
Lap10\
  DTO\DoiTuong.cs                Khoa, LoiNghiepVu, ThongKeRowState, KetQuaCapNhat
  DAL\DbConnectionFactory.cs     Doc chuoi ket noi tu App.config
  DAL\SqlHelper.cs               ExecuteScalar / NonQuery / Reader / DataTable
  DAL\SinhVienDAL.cs             TOAN BO thao tac DataAdapter/DataSet/DataView
  BUS\SinhVienBUS.cs             Kiem tra du lieu + dich loi SQL sang tieng Viet
  GUI\frmMain.cs                 Cua so chinh (MDI) + menu 10 bai
  GUI\frmBai1..10.cs             10 Form bai tap
  GUI\Helpers\                   UiTheme, UiControls, FormBase, BaiFormBase,
                                 LuoiSinhVien, DinhDang
  BaoCaoTools\TrinhChupAnh.cs    Che do --screenshots
  BaoCaoTools\KiemTraChucNang.cs Che do --kiemtra
  BaoCao\                        Anh chup + ket qua kiem tra
  Program.cs                     Diem vao, xu ly tham so dong lenh
  App.config                     Chuoi ket noi "QLSinhVien"

Nguyen tac: Form KHONG goi DAL truc tiep. Form -> BUS -> DAL.

-----------------------------------------------------------------
3. CHUAN BI CSDL
-----------------------------------------------------------------
  - SQL Server / LocalDB voi CSDL QLSinhVien (script cua Lab 7).
  - Du lieu hien tai: 46 sinh vien, 3 khoa (CNTT 18, KT 14, OTO 14).
  - Khong can chay them script nao. Lab 10 chi DOC du lieu, NGOAI TRU
    khi ban tu bam "Luu thay doi" o Bai 5.

  Chuoi ket noi mac dinh (App.config):
    Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=QLSinhVien;
    Integrated Security=True

  Neu may ban dung instance khac, sua lai <connectionStrings> trong
  App.config. Khong can build lai, chi can sua file
  bin\Debug\Lab10_DisconnectedData.exe.config tuong ung.

  LUU Y AN TOAN: Bai 5 ghi that xuong CSDL. Hay backup QLSinhVien truoc
  khi thu, hoac chi thu tren ban sao.

-----------------------------------------------------------------
4. CACH CHAY
-----------------------------------------------------------------
a) Mo bang Visual Studio 2022
   - Mo IT3212_Labs.sln o thu muc goc, chon project Lab10_DisconnectedData
     lam Startup Project, nhan F5.
   - Hoac mo truc tiep Lap10\Lab10_DisconnectedData.csproj.

b) Chay bang dong lenh
   Build:
     msbuild Lap10\Lab10_DisconnectedData.csproj /t:Rebuild /p:Configuration=Debug

   Chay giao dien:
     Lap10\bin\Debug\Lab10_DisconnectedData.exe

   Tu kiem tra chuc nang (khong can giao dien, khong ghi CSDL):
     Lap10\bin\Debug\Lab10_DisconnectedData.exe --kiemtra
     -> ket qua ghi vao Lap10\BaoCao\kiem_tra_nhanh.txt

   Chup anh bao cao:
     Lap10\bin\Debug\Lab10_DisconnectedData.exe --screenshots
     -> 11 anh PNG ghi vao Lap10\BaoCao

   Chi chup mot kich ban:
     Lap10\bin\Debug\Lab10_DisconnectedData.exe --screenshots --kichban=Chup05_Bai4

-----------------------------------------------------------------
5. DU LIEU THU
-----------------------------------------------------------------
  CSDL hien co: 46 sinh vien, diem tu 3.20 den 9.80.
    - 5 sinh vien diem < 5.0      (loc "yeu" o Bai 4)
    - 19 sinh vien diem >= 8.0    (loc "gioi" o Bai 4)
    - 8 sinh vien Email = NULL    (kiem tra DBNull o Bai 1)

  Du lieu tao trong bo nho (khong co trong CSDL):
    Bai 6  Bang "BangDiem": SV101..SV105 voi diem 9.1 / 7.4 / 5.5 / 4.2 / 6.8
    Bai 10 Nguon 2: SV001 (9.5), SV002 (8.9), SV900 (7.7), SV901 (6.6)
           -> SV001, SV002 da co trong CSDL (trung khoa)
           -> SV900, SV901 moi hoan toan

-----------------------------------------------------------------
6. CAC DIEM KY THUAT DA KIEM CHUNG
-----------------------------------------------------------------
Build: 0 loi, 0 canh bao.
Tu kiem tra (--kiemtra): 89/89 phep dat.

Nhung diem duoc kiem tra tu dong:

  Fill
    - DataTable giu dung ten bang "SinhVien", 7 cot.
    - Cot DiemTB van la decimal, NgaySinh van la DateTime.
    - Sau Fill, adapter da dong ket noi; moi dong Unchanged.

  DataRelation
    - GetChildRows() khop 100% voi dem thu cong (18/14/14 = 46).
    - ForeignKeyConstraint chan xoa khoa con sinh vien.

  DataView
    - RowFilter + Sort cho ket qua dung, khong truy van lai CSDL.
    - EscapeRowFilter nhan doi dau nhay don (O'Neil -> O''Neil).
    - Decimals dung InvariantCulture: "DiemTB >= 7.5", khong phai "7,5".

  SqlCommandBuilder
    - Sinh du Insert/Update/Delete tu cau SELECT co khoa chinh.
    - Luu y: builder sinh lenh theo kieu LAZY - phai goi
      GetInsertCommand()/GetUpdateCommand()/GetDeleteCommand() tuong minh,
      neu chi "using (new SqlCommandBuilder(adapter))" thi ba lenh van null.
    - Update() khong tu AcceptChanges -> phai tu goi, neu khong bam Luu
      lan hai se gui lai dung nhung thay doi do.

  RowState
    - Sua -> Modified, them -> Added, xoa -> Deleted (dong van o lai
      trong bang, doc gia tri truc tiep nem DeletedRowInaccessibleException).
    - GetChanges() tra ve dung 3 dong da doi.

  AcceptChanges / RejectChanges
    - AcceptChanges: moi dong ve Unchanged, GetChanges() = null.
      => Bam "Chap nhan" TRUOC "Luu" = mat thay doi, CSDL khong doi,
      khong co loi nao bao. Day la bay chinh cua bai 8.
    - RejectChanges: dong sua ve gia tri goc, dong Added bi loai,
      dong Deleted duoc phuc hoi.

  XML
    - WriteXml + WriteSchema giu kieu decimal, DateTime, PrimaryKey,
      DataRelation.
    - WriteXml khong schema: doc lai duoc nhung mat kieu va quan he.

  Merge
    - So sanh theo PrimaryKey: 46 + 2 dong moi = 48 (khong phai 50).
    - preserveChanges = true -> ban cu thang; false -> ban moi thang.
    - Bo PrimaryKey roi Merge -> sinh dong trung khoa (SV001 xuat hien
      2 lan). Day la loi hay gap nhat khi dung Merge.

-----------------------------------------------------------------
7. KET QUA BAO CAO
-----------------------------------------------------------------
  BaoCao\Bai10_01_frmMain.png .. Bai10_11_Bai10_Merge.png  (11 anh)
  BaoCao\kiem_tra_nhanh.txt       Ket qua 89 phep tu kiem tra
  BaoCao\nhat_ky_chup_anh.txt     Nhat ky chup anh (thoi gian, kich thuoc)

-----------------------------------------------------------------
8. XU LY SU CO
-----------------------------------------------------------------
  "Khong ket noi duoc CSDL"
    -> Kiem tra SQL Server da chay, ten instance trong App.config dung,
       CSDL QLSinhVien da duoc tao bang script Lab 7.

  "Cannot find constraint" khi tao DataRelation
    -> Xay ra khi dung createConstraints = true ma bang cha khong co
       khoa chinh tuong ung. Lab 10 tao ForeignKeyConstraint thu cong
       (xem DAL\SinhVienDAL.cs, TaoQuanHeKhoaSinhVien) de dat duoc
       DeleteRule = None giong NO ACTION cua SQL Server.

  "Loi 1934" khi bam Luu o Bai 5
    -> CSDL co filtered unique index tren Email, can SET QUOTED_IDENTIFIER
       ON. Thu chay lai sau khi bat tuy chon nay trong SSMS.

  Bai 5 ghi nham du lieu
    -> Dung RejectChanges truoc khi bam Luu de huy thay doi trong bo nho.
       Neu da Update xuong CSDL roi thi phai khoi phuc tu ban backup.

=================================================================
