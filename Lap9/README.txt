================================================================
LAB 9 - DOC, TIM KIEM VA LOC DU LIEU BANG SQLDATAREADER
IT3212 - Lap trinh .NET | Chuong 4
Sinh vien: Nguyen Truong Giang - MSSV 24000407
================================================================

1. MUC TIEU
----------------------------------------------------------------
- Doc du lieu theo mo hinh KET NOI bang SqlDataReader (forward-only,
  chi doc duoc khi connection con mo).
- Nap ket qua len DataGridView bang DataTable.Load(reader) va bang
  cach nap thu cong tung dong.
- Xay dung tim kiem / loc / sap xep / phan trang bang SQL THAM SO HOA.
- Dong bo dong dang chon tren luoi voi vung thong tin chi tiet.
- Xu ly DBNull dung cach (khong ep kieu truc tiep).
- Xuat ket qua dang hien thi ra CSV UTF-8 de mo bang Excel.

2. CHUAN BI
----------------------------------------------------------------
- SQL Server (LocalDB hoac SQL Server that) voi CSDL QLSinhVien co
  tu Lab 7 (bang SinhVien, Khoa, DangKy).
- Visual Studio 2022 (hoac MSBuild 4.8).
- .NET Framework 4.8 - dung System.Data.SqlClient, KHONG can NuGet.

BUOC 1 - Bo sung du lieu cho bai phan trang:
    sqlcmd -S "(localdb)\MSSQLLocalDB" -E ^
           -i "Lap9\SQL\Lab09_DuLieuTimKiem.sql" -f 65001

    Script them sinh vien SV023..SV046 (chay lai nhieu lan khong bi
    trung) va in ra so lieu kiem tra. Sau khi chay:
        TongSoSinhVien = 46
        SoTrang (10 dong/trang) = 5

    LUU Y: bang SinhVien co UNIQUE INDEX co dieu kien (UX_SinhVien_Email
    tao o Lab 8) nen MOI thao tac ghi deu can SET QUOTED_IDENTIFIER ON.
    Script da dat san o dau file - thieu dong do se bao loi 1934.

BUOC 2 - Kiem tra chuoi ket noi trong App.config:
    <connectionStrings>
      <add name="QLSinhVien" connectionString="..."/>
    </connectionStrings>
    Neu dung SQL Server that thi sua Data Source / User ID / Password.

BUOC 3 - Build:
    MSBuild.exe Lap9\Lab09_DataReaderSearch.csproj /t:Build ^
               /p:Configuration=Debug /v:minimal

3. CAU TRUC PROJECT
----------------------------------------------------------------
Lap9/
  App.config                     chuoi ket noi "QLSinhVien"
  Program.cs                     3 che do: giao dien / --kiemtra / --screenshots
  DTO/DoiTuong.cs                Khoa, TieuChiTimKiem, SinhVienCot (whitelist),
                                 SinhVienChiTiet, KetQuaPhanTrang, LoiNghiepVu
  DAL/DbConnectionFactory.cs     tao SqlConnection tu App.config
  DAL/SqlHelper.cs               ExecuteScalar / NonQuery / Reader / DataTable
  DAL/SinhVienDAL.cs             TOAN BO cau SQL cua Lab 9
  BUS/SinhVienBUS.cs             kiem tra du lieu + dich loi SqlException
  GUI/Helpers/UiTheme.cs         mau, font, tien ich ve hinh
  GUI/Helpers/UiControls.cs      control tu ve (card, nut, badge, info strip)
  GUI/Helpers/FormBase.cs        nen gradient + CaptureToBitmap
  GUI/Helpers/BaiFormBase.cs     khung chung cua 10 Form bai tap
  GUI/Helpers/DinhDang.cs        dinh dang ngay / diem / tien
  GUI/Helpers/LuoiSinhVien.cs    DataGridView dung chung (bai 5 + bai 9)
  GUI/Helpers/XuatCsv.cs         ghi CSV UTF-8 co BOM (bai 10)
  GUI/frmMain.cs                 cua so chinh (MDI) + dashboard
  GUI/frmBai1..frmBai10.cs       10 bai tap
  BaoCaoTools/KiemTraChucNang.cs kiem tra tu dong khong can giao dien
  BaoCaoTools/TrinhChupAnh.cs    tu dong chup anh bao cao
  SQL/Lab09_DuLieuTimKiem.sql    du lieu cho tim kiem / phan trang
  BaoCao/                        anh PNG + ket qua kiem tra

4. CAC CHE DO CHAY
----------------------------------------------------------------
Mo giao dien:
    Lap9\bin\Debug\Lab09_DataReaderSearch.exe

Kiem tra tu dong (khong hien Form, khong sua CSDL):
    Lab09_DataReaderSearch.exe --kiemtra
    -> ghi bin\Debug\kiem_tra_nhanh.txt va BaoCao\kiem_tra_nhanh.txt

Sinh lai anh bao cao:
    Lab09_DataReaderSearch.exe --screenshots
    -> BaoCao\*.png + BaoCao\nhat_ky_chup_anh.txt
    -> BaoCao_Csv\KetQua_SinhVien.csv (san pham that cua bai 10)
    Moi kich ban chay trong tien trinh rieng, watchdog 45 giay.

5. DOI CHIEU 10 BAI
----------------------------------------------------------------
Bai 1 - Nap SqlDataReader vao DataTable
    DataTable.Load(reader) va cach nap thu cong (while reader.Read()).
    Cau SQL co JOIN Khoa lay TenKhoa va CASE GioiTinh -> 'Nam'/'Nu'.
    Form in so dong / so cot / thoi gian chay cua tung cach.

Bai 2 - Tim kiem gan dung theo ho ten
    WHERE sv.HoTen LIKE @Keyword ESCAPE '\'
    Gia tri tham so = "%" + EscapeLike(tuKhoa) + "%" -> KHONG noi chuoi.
    EscapeLike() vo hieu hoa %, _, [ va \ do nguoi dung go: neu khong,
    go "%" se khop toan bo bang. Form co nut thu SQL Injection.

Bai 3 - Loc theo khoa va khoang diem
    Cau SQL duoc DUNG DAN theo bo loc, chi noi cac menh de CO DINH
    (" AND sv.MaKhoa = @MaKhoa"), gia tri van qua SqlParameter.
    Nut "So sanh voi BETWEEN" chay ca hai cach va doi chieu so dong.

Bai 4 - Phan trang bang OFFSET - FETCH
    ORDER BY <cot> <chieu>, sv.MaSV ASC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
    Kem mot lan SELECT COUNT(*) de biet tong so trang.
    Luon co khoa phu sv.MaSV: neu cot sap xep bi trung gia tri, thu tu
    giua cac dong la khong xac dinh va mot dong co the hien o 2 trang.

Bai 5 - Chi tiet dong duoc chon
    SelectionChanged -> doc MaSV -> SqlDataReader lay chi tiet day du.
    Xu ly DBNull: reader["Email"] co the la DBNull.Value, phai kiem tra
    truoc khi Convert, neu khong nem InvalidCastException.

Bai 6 - Tim kiem da tieu chi (tu thuc hien)
    6 tieu chi: ma, ho ten, khoa, gioi tinh, khoang diem, khoang tuoi.
    Tieu chi bo trong KHONG sinh ra menh de AND. Form hien thi so dieu
    kien va cau SQL sinh ra de nguoi hoc doi chieu.

Bai 7 - Sap xep theo lua chon (tu thuc hien)
    Ten cot KHONG the truyen qua SqlParameter (tham so chi thay GIA TRI).
    Cach an toan: ComboBox chi chua cot trong whitelist SinhVienCot, va
    ChuanHoa() doi chieu lai truoc khi noi vao ORDER BY; chieu sap xep la
    bool chu khong phai chuoi. Form co o "thu tan cong" de chung minh
    chuoi la bi loai bo va bang khong bi xoa.

Bai 8 - Loc theo tuoi (tu thuc hien)
    Tuoi suy ra tu NgaySinh. Doi khoang tuoi thanh khoang NGAY SINH:
        tuoi >= 20  <=>  NgaySinh <= homNay - 20 nam
        tuoi <= 25  <=>  NgaySinh >  homNay - 26 nam
    Ly do -26: nguoi sinh homNay - 25 nam vua tron 25 tuoi, van thuoc
    nhom "<= 25"; dau ">" loai dung nguoi da 26 tuoi.
    Cach lam nay dung duoc INDEX tren NgaySinh; nguoc lai
    DATEDIFF(YEAR, NgaySinh, GETDATE()) trong WHERE phai tinh tung dong
    va con dem sai voi nguoi chua toi sinh nhat. Nut "Doi chieu
    DATEDIFF" chay truc tiep ca hai de thay chenh lech that.

Bai 9 - Dinh dang DataGridView (tu thuc hien)
    - Ngay: dd/MM/yyyy (CellFormatting).
    - Diem: 2 chu so thap phan, canh phai.
    - Tieu de cot tieng Viet, thu tu cot hop ly.
    - To mau ca dong: do khi DiemTB < 5, xanh la khi DiemTB >= 8
      (OnRowPrePaint). Nguong chinh duoc ngay tren Form.

Bai 10 - Xuat ket qua ra CSV (tu thuc hien)
    Chi xuat DUNG tap dang hien thi (lay thang DataSource cua luoi).
    - UTF-8 CO BOM: new UTF8Encoding(true). Khong co BOM thi Excel doc
      theo ANSI va hien "Nguy?n".
    - Escape theo RFC 4180: gia tri co dau phay / dau nhay / xuong dong
      phai boc trong "..." va lap doi moi " ben trong.
    - Kiem tra lai BOM sau khi ghi va hien thi 10 dong dau de doi chieu.

6. DU LIEU THU
----------------------------------------------------------------
46 sinh vien (SV001..SV046), 3 khoa CNTT / OTO / KT.
- Nam sinh 2001..2007 -> du nhieu nhom tuoi de thu bai 8.
- Diem trai tu 3.x den 9.x -> co ca nhom do (< 5) va xanh (>= 8).
- 8 sinh vien Email NULL -> thu DBNull o bai 5.
- 46 dong / 10 dong moi trang = 5 trang -> du de thu phan trang.
Mot so gia tri de thu nhanh:
    Ho ten chua "Nguyen" -> nhieu ket qua
    Ho ten chua "zzz"    -> 0 ket qua
    Diem 7.00 - 9.00     -> 21 sinh vien
    Tuoi 20 - 22         -> 35 sinh vien

7. GHI CHU KY THUAT
----------------------------------------------------------------
- Kien truc 3 tang: GUI -> BUS -> DAL. Chi DAL chua SqlCommand /
  SqlDataReader / SqlParameter; GUI khong co mot dong SQL nao.
- Khong dung AddWithValue: moi tham so khai bao kieu tuong minh
  (SqlDbType.NVarChar/VarChar/Decimal/Date/Bit/Int). Rieng DECIMAL phai
  dat Precision=4, Scale=2 neu khong phan le bi cat (8.75 -> 9).
- SqlDataReader chi doc duoc khi connection con mo -> moi lan doc nam
  trong khoi using(...) cua connection; khi can giu du lieu lau thi
  Load() vao DataTable roi dong ket noi.
- Phan trang luon di kem COUNT(*): chi OFFSET-FETCH thi khong biet khi
  nao dung nut "Sau" va khong tinh duoc "Trang 2/5".
- Sap xep: whitelist cot o tang DTO, kiem tra lai o tang BUS. Ten cot
  la bi thay bang MaSV, khong bao gio lot vao cau lenh.
- Ghi file CSV luon dat truoc moi lenh Console: WinExe khong co console
  handle nen Console.WriteLine co the nem IOException.
- Ket qua tu kiem tra: 57/57 phep kiem tra dat (xem
  BaoCao\kiem_tra_nhanh.txt).

8. SAN PHAM BAO CAO
----------------------------------------------------------------
BaoCao\Bai09_01_frmMain.png              cua so chinh + dashboard
BaoCao\Bai09_02_Bai1_DataTableLoad.png   bai 1
BaoCao\Bai09_03_Bai2_TimKiem.png         bai 2
BaoCao\Bai09_04_Bai3_LocKhoaDiem.png     bai 3
BaoCao\Bai09_05_Bai4_PhanTrang.png       bai 4 (dang o trang 2)
BaoCao\Bai09_06_Bai5_ChiTiet.png         bai 5
BaoCao\Bai09_07_Bai6_DaTieuChi.png       bai 6
BaoCao\Bai09_08_Bai7_SapXep.png          bai 7
BaoCao\Bai09_09_Bai8_LocTuoi.png         bai 8
BaoCao\Bai09_10_Bai9_DinhDang.png        bai 9
BaoCao\Bai09_11_Bai10_XuatCsv.png        bai 10
BaoCao\kiem_tra_nhanh.txt                ket qua 57 phep kiem tra
BaoCao_Csv\KetQua_SinhVien.csv           file CSV that do bai 10 xuat ra
