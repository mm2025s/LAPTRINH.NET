"""Sinh bao cao Word cho De tai 14 - Phan mem quan ly nha thuoc.

Chay: python Tools/tao_bao_cao.py  (sau khi da --kiemtra, --chupanh va ve_so_do.py)
"""
import os
import re

from docx import Document
from docx.enum.section import WD_SECTION
from docx.enum.table import WD_TABLE_ALIGNMENT
from docx.enum.text import WD_ALIGN_PARAGRAPH, WD_BREAK
from docx.oxml import OxmlElement
from docx.oxml.ns import qn
from docx.shared import Cm, Pt, RGBColor

GOC = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
ANH = os.path.join(GOC, "BaoCao", "HinhAnh")
RA = os.path.join(GOC, "BaoCao", "BaoCao_DeTai14_QuanLyNhaThuoc_24000407.docx")
FONT = "Times New Roman"
XANH = RGBColor(0x0D, 0x5C, 0x63)

doc = Document()

# ------------------------------------------------------------------ dinh dang chung
sec = doc.sections[0]
sec.page_height, sec.page_width = Cm(29.7), Cm(21)
sec.left_margin, sec.right_margin, sec.top_margin, sec.bottom_margin = Cm(3), Cm(2), Cm(2), Cm(2)

st = doc.styles["Normal"]
st.font.name = FONT
st.font.size = Pt(13)
st.element.rPr.rFonts.set(qn("w:eastAsia"), FONT)
st.paragraph_format.space_after = Pt(6)
st.paragraph_format.line_spacing = 1.3

for ten, co, truoc, sau in [("Heading 1", 16, 18, 12), ("Heading 2", 14, 12, 6), ("Heading 3", 13, 10, 4)]:
    h = doc.styles[ten]
    h.font.name = FONT
    h.font.size = Pt(co)
    h.font.bold = True
    h.font.italic = False
    h.font.color.rgb = XANH if ten != "Heading 3" else RGBColor(0x1E, 0x22, 0x30)
    h.element.rPr.rFonts.set(qn("w:eastAsia"), FONT)
    rf = h.element.rPr.rFonts
    rf.set(qn("w:ascii"), FONT)
    rf.set(qn("w:hAnsi"), FONT)
    h.paragraph_format.space_before = Pt(truoc)
    h.paragraph_format.space_after = Pt(sau)
    h.paragraph_format.keep_with_next = True

# Word tu cap nhat muc luc khi mo tep
cai_dat = doc.settings.element
upd = OxmlElement("w:updateFields")
upd.set(qn("w:val"), "true")
cai_dat.append(upd)

so_hinh = [0]
so_bang = [0]


def p(chu="", dam=False, nghieng=False, canh=None, co=None, sau=None, mau=None, thut=True):
    para = doc.add_paragraph()
    if canh == "giua":
        para.alignment = WD_ALIGN_PARAGRAPH.CENTER
    elif canh is None:
        para.alignment = WD_ALIGN_PARAGRAPH.JUSTIFY
        if thut:
            para.paragraph_format.first_line_indent = Cm(1)
    if sau is not None:
        para.paragraph_format.space_after = Pt(sau)
    chen_chu(para, chu, dam, nghieng, co, mau)
    return para


def chen_chu(para, chu, dam=False, nghieng=False, co=None, mau=None):
    """Ho tro **dam** trong chuoi."""
    for i, phan in enumerate(re.split(r"\*\*", chu)):
        if not phan:
            continue
        r = para.add_run(phan)
        r.bold = dam or (i % 2 == 1)
        r.italic = nghieng
        if co:
            r.font.size = Pt(co)
        if mau:
            r.font.color.rgb = mau


def gach(chu, cap=0):
    para = doc.add_paragraph(style="List Bullet" if cap == 0 else "List Bullet 2")
    para.alignment = WD_ALIGN_PARAGRAPH.JUSTIFY
    para.paragraph_format.space_after = Pt(3)
    chen_chu(para, chu)
    return para


dem_so = [0]


def so(chu, moi=False, deu=True):
    """Danh sach danh so tu tang; moi=True bat dau lai tu 1 (tranh noi so giua cac muc)."""
    dem_so[0] = 1 if moi else dem_so[0] + 1
    para = doc.add_paragraph()
    para.alignment = WD_ALIGN_PARAGRAPH.JUSTIFY if deu else WD_ALIGN_PARAGRAPH.LEFT
    para.paragraph_format.left_indent = Cm(0.8)
    para.paragraph_format.first_line_indent = Cm(-0.8)
    chu = "%d.	" % dem_so[0] + chu
    para.paragraph_format.tab_stops.add_tab_stop(Cm(0.8))
    para.paragraph_format.space_after = Pt(3)
    chen_chu(para, chu)


def h1(chu):
    para = doc.add_heading(chu, level=1)
    para.alignment = WD_ALIGN_PARAGRAPH.CENTER


def h2(chu):
    doc.add_heading(chu, level=2)


def h3(chu):
    doc.add_heading(chu, level=3)


def to_nen(o, mau):
    tcPr = o._tc.get_or_add_tcPr()
    shd = OxmlElement("w:shd")
    shd.set(qn("w:val"), "clear")
    shd.set(qn("w:color"), "auto")
    shd.set(qn("w:fill"), mau)
    tcPr.append(shd)


def bang(tieu_de, cot, dong, rong=None, co=11):
    so_bang[0] += 1
    cap = doc.add_paragraph()
    cap.alignment = WD_ALIGN_PARAGRAPH.CENTER
    cap.paragraph_format.keep_with_next = True
    cap.paragraph_format.space_before = Pt(6)
    chen_chu(cap, "Bảng %d. %s" % (so_bang[0], tieu_de), dam=True, co=12)
    t = doc.add_table(rows=1, cols=len(cot))
    t.style = "Table Grid"
    t.alignment = WD_TABLE_ALIGNMENT.CENTER
    for i, c in enumerate(cot):
        o = t.rows[0].cells[i]
        o.text = ""
        chen_chu(o.paragraphs[0], c, dam=True, co=co)
        to_nen(o, "D7ECEA")
    for d in dong:
        hang = t.add_row().cells
        for i, v in enumerate(d):
            hang[i].text = ""
            chen_chu(hang[i].paragraphs[0], str(v), co=co)
    for hang in t.rows:
        for o in hang.cells:
            for para in o.paragraphs:
                para.paragraph_format.keep_with_next = hang is t.rows[0]   # dong tieu de khong tach khoi than bang
                para.paragraph_format.space_after = Pt(2)
                para.paragraph_format.line_spacing = 1.1
                para.paragraph_format.first_line_indent = None
    if rong:
        for hang in t.rows:
            for i, w in enumerate(rong):
                hang.cells[i].width = Cm(w)
    doc.add_paragraph().paragraph_format.space_after = Pt(2)
    return t


def hinh(tep, chu_thich, rong=15.5):
    so_hinh[0] += 1
    para = doc.add_paragraph()
    para.alignment = WD_ALIGN_PARAGRAPH.CENTER
    para.paragraph_format.keep_with_next = True
    para.add_run().add_picture(os.path.join(ANH, tep), width=Cm(rong))
    cap = doc.add_paragraph()
    cap.alignment = WD_ALIGN_PARAGRAPH.CENTER
    chen_chu(cap, "Hình %d. %s" % (so_hinh[0], chu_thich), nghieng=True, co=12)


def ma(doan):
    """Doan ma nguon: font Consolas, nen xam, khong thut dong."""
    t = doc.add_table(rows=1, cols=1)
    t.alignment = WD_TABLE_ALIGNMENT.CENTER
    o = t.rows[0].cells[0]
    to_nen(o, "F3F5F8")
    o.width = Cm(16)
    o.text = ""
    dong = doan.strip("\n").split("\n")
    para = o.paragraphs[0]
    for i, d in enumerate(dong):
        r = para.add_run(d)
        r.font.name = "Consolas"
        r.font.size = Pt(9.5)
        r._element.rPr.rFonts.set(qn("w:eastAsia"), "Consolas")
        if i < len(dong) - 1:
            r.add_break()
    para.paragraph_format.line_spacing = 1.0
    para.paragraph_format.space_after = Pt(0)
    doc.add_paragraph().paragraph_format.space_after = Pt(2)


def ngat_trang():
    doc.add_paragraph().add_run().add_break(WD_BREAK.PAGE)


def truong(para, lenh):
    r = para.add_run()
    b = OxmlElement("w:fldChar")
    b.set(qn("w:fldCharType"), "begin")
    r._r.append(b)
    r2 = para.add_run()
    it = OxmlElement("w:instrText")
    it.set(qn("xml:space"), "preserve")
    it.text = lenh
    r2._r.append(it)
    r3 = para.add_run()
    s = OxmlElement("w:fldChar")
    s.set(qn("w:fldCharType"), "separate")
    r3._r.append(s)
    para.add_run("Nhấn F9 (hoặc chuột phải → Update Field) để cập nhật.")
    r4 = para.add_run()
    e = OxmlElement("w:fldChar")
    e.set(qn("w:fldCharType"), "end")
    r4._r.append(e)


def doc_tep(tuong_doi):
    with open(os.path.join(GOC, tuong_doi), encoding="utf-8-sig") as fh:
        return fh.read()


def trich(noi_dung, bat_dau, ket_thuc):
    i = noi_dung.index(bat_dau)
    j = noi_dung.index(ket_thuc, i)
    return noi_dung[i:j].rstrip()


# ================================================================== TRANG BIA
khung = doc.add_table(rows=1, cols=1)
khung.style = "Table Grid"
o = khung.rows[0].cells[0]
o.text = ""
for chu, co, dam, truoc in [
    ("TRƯỜNG ĐẠI HỌC CÔNG NGHỆ ĐÔNG Á", 14, True, 18), ("KHOA CÔNG NGHỆ THÔNG TIN", 14, True, 0),
    ("―――――――――――", 12, False, 0), ("", 12, False, 20),
    ("BÁO CÁO BÀI TẬP LỚN", 22, True, 30), ("MÔN: LẬP TRÌNH .NET (IT3212)", 15, True, 6),
    ("", 12, False, 24), ("ĐỀ TÀI 14", 16, True, 10),
    ("XÂY DỰNG PHẦN MỀM QUẢN LÝ NHÀ THUỐC", 20, True, 4), ("BẰNG CÔNG NGHỆ .NET", 20, True, 4),
    ("", 12, False, 24),
]:
    para = o.add_paragraph()
    para.alignment = WD_ALIGN_PARAGRAPH.CENTER
    para.paragraph_format.space_before = Pt(truoc)
    chen_chu(para, chu, dam=dam, co=co, mau=XANH if co >= 20 else None)
for chu in ["Sinh viên thực hiện:  Nguyễn Trường Giang", "Mã số sinh viên:        24000407",
            "Giảng viên hướng dẫn: Bộ môn Công nghệ phần mềm", "Công nghệ: C# · .NET Framework 4.8 · WinForms · SQL Server · ADO.NET"]:
    para = o.add_paragraph()
    para.paragraph_format.left_indent = Cm(3)
    chen_chu(para, chu, co=13)
para = o.add_paragraph()
para.alignment = WD_ALIGN_PARAGRAPH.CENTER
para.paragraph_format.space_before = Pt(30)
chen_chu(para, "Hà Nội, tháng 11 năm 2026", dam=True, co=13)
o.paragraphs[0]._p.getparent().remove(o.paragraphs[0]._p)

# ================================================================== MUC LUC
doc.add_section(WD_SECTION.NEW_PAGE)
p("MỤC LỤC", dam=True, canh="giua", co=16, mau=XANH)
truong(doc.add_paragraph(), 'TOC \\o "1-3" \\h \\z \\u')

# so trang o chan trang (tu phan muc luc tro di)
sec2 = doc.sections[-1]
sec2.footer.is_linked_to_previous = False
fp = sec2.footer.paragraphs[0]
fp.alignment = WD_ALIGN_PARAGRAPH.CENTER
truong(fp, "PAGE")
for r in fp.runs:
    if r.text.startswith("Nhấn"):
        r.text = "1"

ngat_trang()
p("DANH MỤC TỪ VIẾT TẮT", dam=True, canh="giua", co=14, mau=XANH)
bang("Từ viết tắt", ["Viết tắt", "Ý nghĩa"], [
    ("CSDL", "Cơ sở dữ liệu"), ("GUI", "Graphical User Interface - tầng giao diện"),
    ("BUS", "Business Logic Layer - tầng nghiệp vụ"), ("DAL", "Data Access Layer - tầng truy cập dữ liệu"),
    ("DTO", "Data Transfer Object - đối tượng truyền dữ liệu giữa các tầng"),
    ("SP", "Stored Procedure - thủ tục lưu trữ trên SQL Server"),
    ("TVP", "Table-Valued Parameter - tham số kiểu bảng"), ("NCC", "Nhà cung cấp"),
    ("HĐ", "Hóa đơn"), ("ERD", "Entity Relationship Diagram - sơ đồ thực thể liên kết"),
    ("UC", "Use case - ca sử dụng"), ("ĐVT", "Đơn vị tính"),
], rong=[3, 12.5])

# ================================================================== LOI MO DAU
ngat_trang()
h1("LỜI MỞ ĐẦU")
p("Nhà thuốc bán lẻ là mô hình kinh doanh phổ biến nhưng có đặc thù riêng: danh mục hàng hóa lớn, mỗi loại thuốc có "
  "hạn sử dụng, một số thuốc chỉ được bán khi có đơn của bác sĩ, và tồn kho thay đổi liên tục qua từng lần bán. "
  "Ở nhiều nhà thuốc nhỏ, các công việc này vẫn được ghi chép bằng sổ tay hoặc bảng tính, dẫn tới sai lệch tồn kho, "
  "bán nhầm thuốc hết hạn và khó tổng hợp doanh thu.")
p("Đề tài 14 \"Xây dựng phần mềm quản lý nhà thuốc bằng công nghệ .NET\" vận dụng toàn bộ kiến thức của học phần "
  "Lập trình .NET (IT3212) qua chuỗi bài thực hành: lập trình hướng đối tượng C#, WinForms nhiều form, mô hình 3 lớp, "
  "ADO.NET với SqlCommand và tham số hóa, DataTable, Stored Procedure, transaction và chuẩn hóa xử lý lỗi. Kết quả "
  "là một phần mềm chạy được, có dữ liệu mẫu, bộ kiểm thử tự động và tài liệu thiết kế đầy đủ.")
p("Báo cáo gồm sáu chương: (1) Tổng quan đề tài; (2) Khảo sát và phân tích yêu cầu; (3) Phân tích thiết kế hệ thống "
  "với các sơ đồ chức năng, use case, hoạt động, tuần tự, kiến trúc, CSDL, triển khai và vận hành; (4) Cài đặt; "
  "(5) Kết quả chương trình; (6) Kiểm thử. Phần cuối là kết luận và hướng phát triển.")

# ================================================================== CHUONG 1
ngat_trang()
h1("CHƯƠNG 1. TỔNG QUAN ĐỀ TÀI")
h2("1.1. Lý do chọn đề tài")
p("Quản lý nhà thuốc là bài toán có đủ các thành phần của một hệ thống thông tin quản lý: danh mục (thuốc, nhóm thuốc, "
  "nhà cung cấp, khách hàng), giao dịch (bán hàng, nhập hàng), báo cáo (doanh thu, thuốc bán chạy) và phân quyền người "
  "dùng. Đồng thời bài toán có những ràng buộc nghiệp vụ chặt chẽ mà nếu cài đặt sai sẽ gây hậu quả thật: bán vượt "
  "tồn kho, bán thuốc đã hết hạn, bán thuốc kê đơn khi không có đơn. Vì vậy đây là đề tài phù hợp để vận dụng và kiểm "
  "chứng các kỹ thuật đã học, đặc biệt là transaction và kiểm tra nghiệp vụ nhiều lớp.")
h2("1.2. Mục tiêu")
for g in ["Xây dựng phần mềm desktop quản lý nhà thuốc chạy trên Windows, kết nối SQL Server.",
          "Tổ chức mã nguồn theo kiến trúc 3 lớp GUI - BUS - DAL, DTO dùng chung, giao diện không chứa câu lệnh SQL.",
          "Toàn bộ truy cập dữ liệu qua Stored Procedure có tham số; nghiệp vụ bán hàng và nhập hàng thực hiện trọn vẹn trong một transaction.",
          "Bảo đảm toàn vẹn dữ liệu: tồn kho luôn khớp với chứng từ nhập - bán, không bao giờ âm.",
          "Phân quyền Quản lý / Dược sĩ, mật khẩu được băm có salt.",
          "Có bộ kiểm thử tự động và tài liệu phân tích thiết kế đầy đủ."]:
    gach(g)
h2("1.3. Phạm vi")
p("Phần mềm phục vụ một nhà thuốc bán lẻ với nhiều máy quầy dùng chung một CSDL. Phạm vi gồm: quản lý danh mục, bán "
  "hàng tại quầy, nhập hàng theo lô, tra cứu hóa đơn, cảnh báo kho và thống kê doanh thu. Ngoài phạm vi: quản lý "
  "tồn kho chi tiết theo từng lô khi bán (FEFO), in hóa đơn ra máy in nhiệt, liên thông dữ liệu với cơ quan quản lý dược "
  "và thanh toán điện tử.")
h2("1.4. Công nghệ sử dụng")
bang("Công nghệ và công cụ", ["Thành phần", "Lựa chọn", "Vai trò"], [
    ("Ngôn ngữ", "C# (LangVersion latest)", "Toàn bộ mã nguồn ứng dụng"),
    ("Nền tảng", ".NET Framework 4.8", "Chạy ứng dụng Windows Forms"),
    ("Giao diện", "Windows Forms", "Một cửa sổ kiểu portal (sidebar + header), các trang nhúng tự co giãn"),
    ("Truy cập dữ liệu", "ADO.NET (System.Data.SqlClient)", "SqlConnection, SqlCommand, SqlParameter, SqlDataAdapter, TVP"),
    ("CSDL", "SQL Server 2019+ / LocalDB", "10 bảng, 2 kiểu bảng, 30 Stored Procedure"),
    ("IDE", "Visual Studio 2022 hoặc VS Code", "Biên dịch, gỡ lỗi (QuanLyNhaThuoc.sln, .vscode)"),
    ("Quản trị CSDL", "SSMS, sqlcmd", "Chạy script tạo CSDL và dữ liệu mẫu"),
    ("Tài liệu", "Python matplotlib, python-docx", "Vẽ sơ đồ và sinh báo cáo"),
], rong=[3.2, 5, 7.8])
h2("1.5. Vận dụng kiến thức từ các bài thực hành")
bang("Kiến thức các bài lab được áp dụng", ["Bài lab", "Nội dung đã học", "Áp dụng trong đề tài"], [
    ("Lab 1-3", "C# cơ bản, lớp, kế thừa, interface", "6 interface (IDoiTuong, ICrudDAL<T>, IGhiNhatKy, IGioHang<T>, IBaoCao, ILamMoi); 4 lớp trừu tượng; ngoại lệ kế thừa Exception"),
    ("Lab 4-5", "WinForms, nhiều form", "11 form; frmMain mở form con, đăng nhập / đăng xuất theo vòng lặp"),
    ("Lab 6", "DataGridView, mô hình 3 lớp", "Tách GUI - BUS - DAL; lưới dữ liệu định dạng tự động theo tên cột"),
    ("Lab 7", "Tạo CSDL, ràng buộc, App.config", "CSDL QLNhaThuoc với PK, FK, CHECK, UNIQUE; chuỗi kết nối trong App.config"),
    ("Lab 8", "SqlCommand, SqlParameter, OUTPUT", "Mọi lệnh đều tham số hóa; @MaHD, @MaPN, @MaKHMoi trả về qua OUTPUT"),
    ("Lab 9", "SqlDataReader, tìm kiếm LIKE an toàn", "Đọc đăng nhập bằng reader; tìm thuốc có ESCAPE ký tự %, _, ["),
    ("Lab 10", "DataAdapter, DataTable", "Danh sách nạp vào DataTable qua SqlDataAdapter rồi gắn lên lưới"),
    ("Lab 11", "CRUD bằng Stored Procedure, THROW, xóa mềm, chuẩn hóa lỗi", "30 SP; mã lỗi 51001-51010; ngừng kinh doanh thuốc (xóa mềm); XuLyLoi + nhật ký"),
    ("Lab 12 (mở rộng)", "Master-detail, giao dịch nhiều bước", "Hóa đơn - chi tiết, phiếu nhập - chi tiết trong một TRANSACTION với TVP"),
], rong=[2.6, 5.4, 8])

# ================================================================== CHUONG 2
ngat_trang()
h1("CHƯƠNG 2. KHẢO SÁT VÀ PHÂN TÍCH YÊU CẦU")
h2("2.1. Quy trình nghiệp vụ tại nhà thuốc")
p("Qua khảo sát hoạt động của một nhà thuốc bán lẻ, quy trình làm việc hằng ngày gồm các bước sau:")
so(moi=True, chu="**Đầu ca:** dược sĩ đăng nhập, xem tình hình doanh thu và danh sách thuốc sắp hết hàng, sắp hết hạn.")
so("**Bán hàng:** khách mô tả nhu cầu hoặc đưa đơn thuốc; dược sĩ tìm thuốc, kiểm tra còn hàng, còn hạn, với thuốc kê đơn thì ghi nhận số đơn; tính tiền và giao thuốc. Khách quen được nhận diện bằng số điện thoại.")
so("**Nhập hàng:** khi thuốc sắp hết, quản lý đặt hàng nhà cung cấp; khi hàng về, lập phiếu nhập ghi số lượng, giá nhập và hạn dùng của lô.")
so("**Cuối ngày / cuối kỳ:** quản lý xem doanh thu, thuốc bán chạy, đối chiếu hóa đơn.")
p("Những khó khăn khi làm thủ công: tồn kho trên sổ lệch với thực tế; không phát hiện kịp thuốc sắp hết hạn; hai người "
  "cùng bán một mặt hàng sắp hết dẫn tới bán vượt số lượng; tổng hợp doanh thu tốn thời gian.")
h2("2.2. Tác nhân của hệ thống")
bang("Tác nhân", ["Tác nhân", "Mô tả", "Quyền"], [
    ("Dược sĩ", "Nhân viên đứng quầy bán thuốc", "Bán hàng, tra cứu thuốc, quản lý khách hàng, tra cứu hóa đơn, xem cảnh báo kho, đổi mật khẩu"),
    ("Quản lý", "Chủ hoặc người quản lý nhà thuốc; kế thừa toàn bộ quyền của Dược sĩ", "Thêm: quản lý thuốc, nhập hàng, nhóm thuốc, nhà cung cấp, thống kê doanh thu, quản lý nhân viên"),
], rong=[2.6, 5.4, 8])
h2("2.3. Yêu cầu chức năng")
bang("Yêu cầu chức năng", ["Mã", "Chức năng", "Mô tả", "Tác nhân"], [
    ("F01", "Đăng nhập / đăng xuất", "Xác thực bằng tên đăng nhập và mật khẩu; tài khoản bị khóa không đăng nhập được", "Tất cả"),
    ("F02", "Đổi mật khẩu", "Nhập mật khẩu cũ, mật khẩu mới (≥ 6 ký tự) và xác nhận", "Tất cả"),
    ("F03", "Bán hàng", "Tìm thuốc, giỏ hàng, gắn khách theo SĐT, lập hóa đơn", "Dược sĩ, Quản lý"),
    ("F04", "Tra cứu thuốc", "Tìm theo mã / tên, lọc theo nhóm; xem giá, tồn kho, hạn dùng", "Dược sĩ, Quản lý"),
    ("F05", "Quản lý khách hàng", "Thêm, sửa, tìm; xem số hóa đơn và tổng tiền đã mua", "Dược sĩ, Quản lý"),
    ("F06", "Tra cứu hóa đơn", "Lọc theo khoảng ngày, xem chi tiết từng hóa đơn", "Dược sĩ, Quản lý"),
    ("F07", "Cảnh báo kho", "Thuốc tồn ≤ 20, thuốc hết hạn hoặc còn ≤ 90 ngày", "Dược sĩ, Quản lý"),
    ("F08", "Quản lý thuốc", "Thêm, sửa, xóa / ngừng kinh doanh thuốc", "Quản lý"),
    ("F09", "Nhập hàng", "Lập phiếu nhập theo nhà cung cấp, mỗi dòng có SL, giá nhập, hạn dùng", "Quản lý"),
    ("F10", "Nhóm thuốc, nhà cung cấp", "CRUD danh mục; không xóa mục đang được sử dụng", "Quản lý"),
    ("F11", "Thống kê doanh thu", "Biểu đồ doanh thu theo ngày, top 10 thuốc bán chạy", "Quản lý"),
    ("F12", "Quản lý nhân viên", "Tạo tài khoản, phân quyền, khóa, đặt lại mật khẩu", "Quản lý"),
    ("F13", "Nhật ký · hiệu năng", "Xem ai làm gì, khi nào, thành công / thất bại; doanh thu theo nhân viên; thời gian xử lý từng nghiệp vụ", "Quản lý"),
    ("F14", "In báo cáo", "In hóa đơn bán lẻ và báo cáo doanh thu (xem trước, in ra máy in hoặc PDF)", "Dược sĩ, Quản lý"),
], rong=[1.3, 3.6, 8, 3.1], co=10.5)
h2("2.4. Yêu cầu phi chức năng")
for g in ["**Toàn vẹn dữ liệu:** tồn kho không âm; hóa đơn / phiếu nhập ghi đủ hoặc không ghi gì (transaction).",
          "**Bảo mật:** mật khẩu lưu dạng băm SHA-256 kèm salt; không nối chuỗi SQL (chống SQL Injection); kiểm tra quyền ở tầng BUS chứ không chỉ ẩn nút.",
          "**Dễ dùng:** thông báo lỗi bằng tiếng Việt, chỉ rõ ô cần sửa; giao diện co giãn theo kích thước cửa sổ.",
          "**Bảo trì:** kiến trúc 3 lớp; quy tắc nghiệp vụ quan trọng đặt trong Stored Procedure để áp dụng thống nhất cho mọi máy quầy.",
          "**Truy vết:** lỗi kỹ thuật ghi tệp log kèm mã tham chiếu; mọi thao tác ghi dữ liệu, đăng nhập / đăng xuất được ghi vào bảng NhatKy kể cả khi thất bại.",
          "**Hiệu năng:** thao tác bán hàng phản hồi dưới 1 giây với vài nghìn mặt hàng."]:
    gach(g)
h2("2.5. Quy tắc nghiệp vụ")
bang("Quy tắc nghiệp vụ và nơi cài đặt", ["Mã", "Quy tắc", "Cài đặt tại"], [
    ("BR01", "Không bán vượt tồn kho, kể cả khi hai quầy cùng bán một lúc", "BUS (giỏ hàng) + sp_HoaDon_Tao (UPDATE ... WHERE TonKho >= SoLuong) + CHECK TonKho >= 0"),
    ("BR02", "Không bán thuốc đã hết hạn", "BUS + sp_HoaDon_Tao (THROW 51005)"),
    ("BR03", "Thuốc kê đơn phải ghi số đơn thuốc của bác sĩ", "BanHangBUS.ThanhToan"),
    ("BR04", "Giá bán ghi vào hóa đơn là giá tại thời điểm bán", "sp_HoaDon_Tao lấy Thuoc.GiaBan lúc lập"),
    ("BR05", "Tồn kho chỉ thay đổi qua nhập hàng và bán hàng, không sửa tay", "sp_Thuoc_Update không có tham số TonKho"),
    ("BR06", "Không nhập lô thuốc đã hết hạn", "NhapHangBUS + sp_PhieuNhap_Tao"),
    ("BR07", "Thuốc đã có giao dịch thì chỉ ngừng kinh doanh, không xóa", "sp_Thuoc_Delete (xóa mềm IsActive = 0)"),
    ("BR08", "Không xóa nhóm thuốc / NCC / khách hàng đang được sử dụng", "sp_*_Delete (THROW 51009)"),
    ("BR09", "Số điện thoại khách hàng là duy nhất", "UNIQUE + sp_KhachHang_Save (THROW 51008)"),
    ("BR10", "Quản lý không được tự khóa hoặc tự hạ quyền chính mình", "NhanVienBUS.CapNhat"),
], rong=[1.4, 7.4, 7.2], co=10.5)

# ================================================================== CHUONG 3
ngat_trang()
h1("CHƯƠNG 3. PHÂN TÍCH THIẾT KẾ HỆ THỐNG")
h2("3.1. Sơ đồ phân rã chức năng")
p("Hệ thống được chia thành bốn nhóm chức năng: Hệ thống, Danh mục, Nghiệp vụ và Báo cáo (Hình 1).")
hinh("SD04_PhanRaChucNang.png", "Sơ đồ phân rã chức năng")
h2("3.2. Sơ đồ use case")
p("Hình 2 thể hiện hai tác nhân. Quản lý kế thừa toàn bộ ca sử dụng của Dược sĩ và có thêm các chức năng quản trị. "
  "Ca sử dụng Bán hàng bao gồm (include) các bước kiểm tra tồn kho, hạn dùng, đơn thuốc và tìm khách theo số điện thoại.")
hinh("SD02_UseCase.png", "Sơ đồ use case tổng quát")
h3("3.2.1. Đặc tả use case Bán hàng")
bang("Đặc tả UC Bán hàng - lập hóa đơn", ["Mục", "Nội dung"], [
    ("Tác nhân", "Dược sĩ (hoặc Quản lý)"),
    ("Tiền điều kiện", "Đã đăng nhập; thuốc cần bán đang kinh doanh"),
    ("Luồng chính", "1. Tìm thuốc theo tên / mã.  2. Chọn thuốc, nhập số lượng, bấm \"Thêm vào giỏ\"; hệ thống kiểm tra hạn dùng và tồn kho, trùng thuốc thì cộng dồn.  "
                    "3. Lặp lại bước 1-2.  4. (Tùy chọn) nhập SĐT khách, tìm hoặc tạo khách mới.  5. Nhập số đơn thuốc nếu giỏ có thuốc kê đơn.  "
                    "6. Bấm \"Thanh toán\" và xác nhận.  7. Hệ thống gọi sp_HoaDon_Tao, trừ tồn kho, ghi hóa đơn và chi tiết trong một transaction.  8. Thông báo số hóa đơn, làm trống giỏ."),
    ("Luồng rẽ nhánh", "2a. Thuốc hết hạn → báo lỗi, không thêm.  2b. Vượt tồn kho → báo số lượng còn lại.  "
                       "5a. Có thuốc kê đơn nhưng chưa nhập số đơn → yêu cầu nhập.  7a. Quầy khác vừa bán làm tồn kho không đủ → ROLLBACK, báo lỗi, tải lại tồn kho."),
    ("Hậu điều kiện", "Có một hóa đơn mới; tồn kho mỗi thuốc giảm đúng số lượng đã bán"),
], rong=[3.2, 12.8], co=11)
h3("3.2.2. Đặc tả use case Nhập hàng")
bang("Đặc tả UC Nhập hàng", ["Mục", "Nội dung"], [
    ("Tác nhân", "Quản lý"),
    ("Tiền điều kiện", "Đã đăng nhập với vai trò Quản lý; thuốc đã có trong danh mục"),
    ("Luồng chính", "1. Chọn nhà cung cấp.  2. Với mỗi lô: chọn thuốc, nhập số lượng, giá nhập, hạn sử dụng, bấm \"Thêm dòng\".  "
                    "3. Bấm \"Lưu phiếu nhập\".  4. sp_PhieuNhap_Tao ghi phiếu, chi tiết, cộng tồn kho, cập nhật giá nhập và hạn dùng gần nhất trong một transaction."),
    ("Luồng rẽ nhánh", "2a. Hạn dùng ≤ hôm nay → từ chối.  2b. Thuốc đã có trong phiếu → yêu cầu xóa dòng cũ.  3a. Phiếu rỗng → báo lỗi."),
    ("Hậu điều kiện", "Tồn kho tăng; giá nhập bằng giá lô mới"),
], rong=[3.2, 12.8], co=11)
h3("3.2.3. Đặc tả use case Đăng nhập")
bang("Đặc tả UC Đăng nhập", ["Mục", "Nội dung"], [
    ("Tác nhân", "Dược sĩ, Quản lý"),
    ("Luồng chính", "1. Nhập tên đăng nhập, mật khẩu.  2. BUS lấy salt và hash qua sp_NhanVien_LayDeDangNhap.  "
                    "3. Băm SHA-256(salt + mật khẩu) và so khớp.  4. Lưu nhân viên vào PhienDangNhap, mở màn hình chính với menu theo vai trò."),
    ("Luồng rẽ nhánh", "3a. Sai tên hoặc sai mật khẩu → cùng một thông báo (không tiết lộ tài khoản có tồn tại).  "
                       "3b. Tài khoản bị khóa → báo liên hệ Quản lý.  3c. Sai từ 3 lần → mỗi lần thử sau bị chậm thêm 1 giây."),
], rong=[3.2, 12.8], co=11)
h2("3.3. Sơ đồ hoạt động nghiệp vụ bán hàng")
p("Hình 3 mô tả luồng hoạt động của nghiệp vụ quan trọng nhất. Tồn kho được kiểm tra hai lần: lần đầu ở giỏ hàng "
  "(phản hồi ngay cho dược sĩ), lần hai trong Stored Procedure ngay tại thời điểm ghi (chống bán vượt khi nhiều quầy cùng bán).")
hinh("SD05_HoatDongBanHang.png", "Sơ đồ hoạt động - bán hàng", rong=12.5)
h2("3.4. Sơ đồ tuần tự lập hóa đơn")
p("Hình 4 thể hiện thông điệp đi qua các tầng khi dược sĩ bấm Thanh toán. Cả giỏ hàng được chuyển xuống SQL Server "
  "trong một lần gọi bằng tham số kiểu bảng, nên số lần truy cập mạng không phụ thuộc số dòng hàng.")
hinh("SD06_TuanTuLapHoaDon.png", "Sơ đồ tuần tự - lập hóa đơn")
h2("3.5. Kiến trúc hệ thống")
p("Ứng dụng tổ chức theo mô hình 3 lớp (Hình 5). Mỗi tầng chỉ gọi tầng ngay dưới nó; DTO là các lớp dữ liệu thuần "
  "dùng chung. Ngoại lệ SqlException phát sinh ở DAL được tầng BUS dịch sang ba loại lỗi của ứng dụng trước khi tới giao diện.")
hinh("SD01_KienTruc3Lop.png", "Kiến trúc 3 lớp của phần mềm")
bang("Trách nhiệm từng tầng", ["Tầng", "Thư mục", "Trách nhiệm"], [
    ("GUI", "GUI/", "Thu thập dữ liệu từ control, gọi BUS, hiển thị kết quả và lỗi. Không chứa SQL, không gọi DAL."),
    ("BUS", "BUS/", "Kiểm tra dữ liệu nhập, quy tắc nghiệp vụ, phân quyền, giỏ hàng; dịch lỗi SQL; ghi nhật ký."),
    ("DAL", "DAL/", "Mở kết nối, gọi Stored Procedure với SqlParameter đúng kiểu, chuyển kết quả thành DataTable / DTO."),
    ("DTO", "DTO/", "Lớp dữ liệu (ThuocDTO, DongGioHang...), lớp ngoại lệ, phiên đăng nhập. Không tham chiếu WinForms."),
    ("CSDL", "SQL/", "Bảng, ràng buộc, kiểu bảng, Stored Procedure chứa quy tắc toàn vẹn và transaction."),
], rong=[2, 2.4, 11.6])
h3("3.5.1. Thiết kế hướng đối tượng")
p("Các tầng giao tiếp với nhau qua interface, các phần dùng chung đặt ở lớp trừu tượng để lớp con chỉ cài đặt phần riêng. "
  "Toàn bộ interface khai báo trong DTO/GiaoDien.cs nên cả DAL, BUS và GUI đều tham chiếu được mà không phụ thuộc vòng.")
bang("Interface của hệ thống", ["Interface", "Vai trò", "Lớp cài đặt"], [
    ("IDoiTuong", "Đối tượng nghiệp vụ có khóa chính và mô tả ngắn", "DoiTuongCoSo → NhanVienDTO, ThuocDTO, NhomThuocDTO, NhaCungCapDTO, KhachHangDTO"),
    ("ICrudDAL<T>", "Hợp đồng CRUD: TimKiem, Them, Sua, Xoa", "DalCoSo<T> → ThuocDAL, KhachHangDAL"),
    ("IGhiNhatKy", "Nơi ghi nhật ký người dùng", "NhatKyDAL"),
    ("IGioHang<T>", "Danh sách dòng tạm trước khi lưu", "BanHangBUS, NhapHangBUS"),
    ("IBaoCao", "Nguồn dữ liệu của một báo cáo in", "BaoCaoCoSo → BaoCaoHoaDon, BaoCaoDoanhThu"),
    ("ILamMoi", "Trang tự tải lại số liệu khi được mở", "frmTongQuan, frmNhatKy"),
], rong=[2.8, 5.6, 7.6], co=10.5)
bang("Cây kế thừa chính", ["Lớp cơ sở", "Lớp con", "Phần dùng chung"], [
    ("DoiTuongCoSo (abstract)", "5 DTO danh mục", "Bắt buộc KhoaChinh, MoTa(); nhật ký gọi MoTa() theo đa hình"),
    ("DalCoSo<T> (abstract)", "ThuocDAL, KhachHangDAL", "Cài ICrudDAL<T>; lớp con ánh xạ 4 thao tác sang Stored Procedure"),
    ("BusCoSo (abstract)", "8 lớp BUS", "Doc(): dịch lỗi SQL; Ghi(): đo thời gian + ghi nhật ký"),
    ("BaoCaoCoSo (abstract)", "BaoCaoHoaDon, BaoCaoDoanhThu", "Tiêu đề, cột, dữ liệu; lớp con tính dòng tổng"),
    ("FormBase → ManHinh", "12 trang giao diện", "Khung trang, tiêu đề, nhúng vào cửa sổ chính"),
    ("Exception / ArgumentException", "LoiNghiepVu, LoiCsdl, LoiNhapLieu", "Ba loại lỗi của ứng dụng"),
], rong=[4.2, 4.6, 7.2], co=10.5)
h2("3.6. Thiết kế cơ sở dữ liệu")
h3("3.6.1. Sơ đồ thực thể liên kết")
hinh("SD03_ERD.png", "Sơ đồ thực thể liên kết CSDL QLNhaThuoc")
h3("3.6.2. Mô tả các bảng")
bang("Bảng Thuoc", ["Cột", "Kiểu", "Ràng buộc", "Ý nghĩa"], [
    ("MaThuoc", "VARCHAR(15)", "PK", "Mã thuốc"), ("TenThuoc", "NVARCHAR(150)", "NOT NULL", "Tên thuốc"),
    ("MaNhom", "VARCHAR(10)", "FK → NhomThuoc", "Nhóm thuốc"), ("DonViTinh", "NVARCHAR(20)", "NOT NULL", "Viên, hộp, chai..."),
    ("GiaNhap", "DECIMAL(12,0)", "≥ 0", "Giá nhập lô gần nhất"), ("GiaBan", "DECIMAL(12,0)", "> 0", "Giá bán hiện hành"),
    ("TonKho", "INT", "CHECK ≥ 0", "Số lượng tồn"), ("HanSuDung", "DATE", "NULL", "Hạn của lô gần hết hạn nhất"),
    ("CanKeDon", "BIT", "DEFAULT 0", "Thuốc kê đơn"), ("IsActive", "BIT", "DEFAULT 1", "0 = ngừng kinh doanh"),
], rong=[3, 3.6, 3.6, 5.8], co=11)
bang("Bảng HoaDon và ChiTietHoaDon", ["Bảng.Cột", "Kiểu", "Ràng buộc", "Ý nghĩa"], [
    ("HoaDon.MaHD", "INT IDENTITY", "PK", "Số hóa đơn"), ("HoaDon.NgayLap", "DATETIME2(0)", "DEFAULT SYSDATETIME()", "Thời điểm lập"),
    ("HoaDon.MaNV", "INT", "FK → NhanVien", "Người bán"), ("HoaDon.MaKH", "INT", "FK → KhachHang, NULL", "NULL = khách lẻ"),
    ("HoaDon.TongTien", "DECIMAL(14,0)", "NOT NULL", "Tổng tiền"), ("HoaDon.GhiChu", "NVARCHAR(200)", "NULL", "Số đơn thuốc"),
    ("ChiTietHoaDon.(MaHD, MaThuoc)", "INT, VARCHAR(15)", "PK, FK", "Mỗi thuốc một dòng"),
    ("ChiTietHoaDon.SoLuong", "INT", "CHECK > 0", "Số lượng bán"), ("ChiTietHoaDon.DonGia", "DECIMAL(12,0)", "NOT NULL", "Giá tại thời điểm bán"),
], rong=[4.6, 3.4, 3.8, 4.2], co=10.5)
bang("Các bảng còn lại", ["Bảng", "Cột chính", "Ghi chú"], [
    ("NhanVien", "MaNV, HoTen, TenDangNhap (UQ), MatKhauHash VARBINARY(32), Salt, VaiTro, IsActive", "VaiTro CHECK IN ('QuanLy','DuocSi')"),
    ("NhomThuoc", "MaNhom, TenNhom (UQ)", "6 nhóm mẫu"),
    ("NhaCungCap", "MaNCC, TenNCC, DienThoai, DiaChi", "4 NCC mẫu"),
    ("KhachHang", "MaKH IDENTITY, HoTen, DienThoai (UQ), DiaChi", "Nhận diện khách quen bằng SĐT"),
    ("PhieuNhap", "MaPN IDENTITY, NgayNhap, MaNV, MaNCC, TongTien", ""),
    ("ChiTietPhieuNhap", "(MaPN, MaThuoc) PK, SoLuong, DonGiaNhap, HanSuDung", "Hạn dùng ghi theo lô"),
    ("NhatKy", "MaNK IDENTITY, ThoiGian, MaNV (FK, NULL), TenDangNhap, HanhDong, DoiTuong, ChiTiet, ThanhCong, ThoiGianMs", "Nhật ký và hiệu năng; chỉ ghi thêm nên không vẽ trong ERD"),
], rong=[3.2, 8.8, 4], co=10.5)
h3("3.6.3. Danh sách Stored Procedure")
bang("Stored Procedure của hệ thống", ["Nhóm", "Thủ tục", "Chức năng"], [
    ("Nhân viên", "sp_NhanVien_LayDeDangNhap, _GetAll, _Insert, _Update, _DoiMatKhau", "Đăng nhập, quản lý tài khoản"),
    ("Danh mục", "sp_NhomThuoc_GetAll / _Save / _Delete, sp_NhaCungCap_GetAll / _Save / _Delete", "CRUD; _Save gộp thêm và sửa"),
    ("Thuốc", "sp_Thuoc_Search, _Insert, _Update, _Delete", "Tìm kiếm có ESCAPE; xóa mềm nếu đã có giao dịch"),
    ("Khách hàng", "sp_KhachHang_Search, _Save (OUTPUT @MaKHMoi), _Delete", ""),
    ("Bán hàng", "sp_HoaDon_Tao (TVP + TRANSACTION), sp_HoaDon_Search, sp_HoaDon_ChiTiet", "Lập và tra cứu hóa đơn"),
    ("Nhập hàng", "sp_PhieuNhap_Tao (TVP + TRANSACTION)", "Lập phiếu, cộng kho"),
    ("Thống kê", "sp_ThongKe_TongQuan, _DoanhThuTheoNgay, _TopThuoc, _CanhBao", "Màn hình chính và báo cáo"),
    ("Nhật ký · hiệu năng", "sp_NhatKy_Ghi, sp_NhatKy_Search, sp_ThongKe_HieuNangNhanVien, sp_ThongKe_HieuNangThaoTac", "Ghi / tra cứu nhật ký, hiệu năng theo nhân viên và nghiệp vụ"),
], rong=[2.6, 8.4, 5], co=10.5)
bang("Mã lỗi nghiệp vụ trả về bằng THROW", ["Mã", "Ý nghĩa", "Mã", "Ý nghĩa"], [
    ("51001", "Tên đăng nhập đã tồn tại", "51006", "Thuốc đang có giao dịch"),
    ("51002", "Không tìm thấy bản ghi / dữ liệu không hợp lệ", "51007", "Hóa đơn hoặc phiếu nhập rỗng"),
    ("51003", "Mã thuốc đã tồn tại", "51008", "SĐT khách đã tồn tại"),
    ("51004", "Không đủ tồn kho", "51009", "Danh mục đang được sử dụng"),
    ("51005", "Thuốc / lô đã hết hạn", "51010", "Mã danh mục đã tồn tại"),
], rong=[1.6, 6.4, 1.6, 6.4], co=10.5)
h2("3.7. Sơ đồ triển khai")
p("Phần mềm triển khai theo mô hình client - server (Hình 8): mỗi máy quầy cài ứng dụng WinForms, tất cả kết nối tới "
  "một SQL Server chung qua ADO.NET. Mọi quy tắc toàn vẹn nằm ở CSDL nên dữ liệu vẫn đúng khi nhiều quầy bán đồng thời. "
  "Khi chỉ dùng một máy, có thể chạy SQL Server LocalDB ngay trên máy đó.")
hinh("SD07_TrienKhai.png", "Sơ đồ triển khai")
h2("3.8. Sơ đồ vận hành")
p("Hình 9 mô tả vòng vận hành hằng ngày của nhà thuốc khi dùng phần mềm. Cảnh báo kho trên màn hình chính là điểm "
  "nối giữa bán hàng và nhập hàng: khi một thuốc xuống dưới ngưỡng, quản lý lập phiếu nhập; hàng nhập xong lập tức có thể bán.")
hinh("SD08_QuyTrinhVanHanh.png", "Sơ đồ vận hành hằng ngày")

# ================================================================== CHUONG 4
ngat_trang()
h1("CHƯƠNG 4. CÀI ĐẶT")
h2("4.1. Yêu cầu môi trường")
bang("Yêu cầu cài đặt", ["Thành phần", "Yêu cầu tối thiểu"], [
    ("Hệ điều hành", "Windows 10 / 11"),
    ("Runtime", ".NET Framework 4.8 (có sẵn trên Windows 10 1903 trở lên)"),
    ("CSDL", "SQL Server 2016 SP1 trở lên, SQL Server Express hoặc LocalDB (cần CREATE OR ALTER, DROP ... IF EXISTS, kiểu bảng)"),
    ("Phát triển", "Visual Studio 2022 với workload .NET desktop development; SSMS hoặc sqlcmd"),
], rong=[3.5, 12.5])
h2("4.2. Các bước cài đặt")
so(moi=True, chu="**Cách nhanh nhất (Visual Studio 2022):** mở tệp DeTai14_QuanLyNhaThuoc\\QuanLyNhaThuoc.sln, nhấn F5. Lần chạy đầu, nếu máy chưa có CSDL QLNhaThuoc, chương trình hỏi và tự chạy hai script trong thư mục SQL (được chép vào bin\\Debug\\SQL khi biên dịch). Lệnh QuanLyNhaThuoc.exe --caidat cũng tạo lại CSDL và dữ liệu mẫu. Các bước dưới đây dành cho cài đặt thủ công.")
so(chu="**Visual Studio Code:** mở thư mục DeTai14_QuanLyNhaThuoc, cài extension C# (ms-dotnettools.csharp) theo gợi ý, nhấn F5. Cấu hình có sẵn trong .vscode\\launch.json (kiểu gỡ lỗi clr cho .NET Framework) và .vscode\\tasks.json (biên dịch bằng dotnet build). Ctrl+Shift+B để biên dịch; menu Terminal → Run Task có sẵn tác vụ chạy kiểm thử và tạo lại CSDL.")
so(chu="**Tạo CSDL thủ công:** mở SSMS, kết nối tới SQL Server, chạy lần lượt SQL\\01_TaoCSDL_QLNhaThuoc.sql rồi SQL\\02_DuLieuMau.sql. Hoặc dùng dòng lệnh:")
ma(r"""cd DeTai14_QuanLyNhaThuoc\SQL
sqlcmd -S (localdb)\MSSQLLocalDB -E -i 01_TaoCSDL_QLNhaThuoc.sql
sqlcmd -S (localdb)\MSSQLLocalDB -E -i 02_DuLieuMau.sql""")
so("**Cấu hình chuỗi kết nối:** nếu không dùng LocalDB, sửa Data Source trong App.config (ví dụ .\\SQLEXPRESS):")
ma(doc_tep("App.config").split("<connectionStrings>")[1].split("</connectionStrings>")[0].strip())
so("**Biên dịch:** mở IT3212_Labs.sln (hoặc QuanLyNhaThuoc.csproj) bằng Visual Studio, chọn QuanLyNhaThuoc làm Startup Project, nhấn F5. Hoặc:")
ma(r"""msbuild DeTai14_QuanLyNhaThuoc\QuanLyNhaThuoc.csproj /p:Configuration=Release""")
so("**Đăng nhập:** admin / admin123 (Quản lý) hoặc duocsi / 123456 (Dược sĩ). Nên đổi mật khẩu ngay sau lần đầu.")
so("**Kiểm tra cài đặt:** chạy QuanLyNhaThuoc.exe --kiemtra; kết quả ghi vào BaoCao\\kiem_thu.txt.")
so("**Sao lưu định kỳ:** BACKUP DATABASE QLNhaThuoc TO DISK = N'D:\\Backup\\QLNhaThuoc.bak' WITH INIT;")
h2("4.3. Cấu trúc mã nguồn")
ma(r"""DeTai14_QuanLyNhaThuoc\
  SQL\01_TaoCSDL_QLNhaThuoc.sql   Bảng, ràng buộc, kiểu bảng, 30 Stored Procedure
  SQL\02_DuLieuMau.sql            Tài khoản, 22 thuốc, 2 phiếu nhập, 60 hóa đơn mẫu
  DTO\DoiTuong.cs                 ThuocDTO, KhachHangDTO, DongGioHang, PhienDangNhap...
  DTO\GiaoDien.cs                 6 interface, lớp trừu tượng DoiTuongCoSo
  DTO\LoiUngDung.cs               LoiNhapLieu, LoiNghiepVu, LoiCsdl
  DAL\Db.cs                       Kết nối, gọi SP, tạo SqlParameter / TVP
  DAL\NhaThuocDAL.cs              NhanVienDAL, ThuocDAL, GiaoDichDAL, ThongKeDAL...
  BUS\NhaThuocBUS.cs              Kiểm tra dữ liệu, phân quyền, giỏ hàng, mật khẩu
  BUS\BusCoSo.cs                  Lớp cơ sở BUS, ghi nhật ký, NhatKyBUS
  BUS\BaoCaoBUS.cs                BaoCaoHoaDon, BaoCaoDoanhThu (IBaoCao)
  BUS\XuLyLoi.cs                  Phân loại lỗi, dịch SqlException, ghi nhật ký
  GUI\frm*.cs                     12 màn hình (có frmNhatKy)
  GUI\Helpers\                    Ui, UiTheme, NavMenu, InBaoCao (bộ in báo cáo)
  Tools\KiemThu.cs                Bộ kiểm thử tự động (--kiemtra)
  Tools\ChupAnh.cs                Chụp màn hình cho báo cáo (--chupanh)
  App.config                      Chuỗi kết nối QLNhaThuoc""")
h2("4.4. Các kỹ thuật cài đặt chính")
h3("4.4.1. Lập hóa đơn trong một transaction với tham số kiểu bảng")
p("Thay vì gọi nhiều lệnh INSERT từ C#, toàn bộ giỏ hàng được đóng thành một DataTable và truyền vào thủ tục bằng "
  "tham số kiểu bảng dbo.DongHoaDonType. Thủ tục bật XACT_ABORT để mọi lỗi đều rollback. Điểm mấu chốt là câu UPDATE "
  "chỉ trừ kho những dòng còn đủ hàng; nếu số dòng được cập nhật ít hơn số dòng giỏ hàng nghĩa là có thuốc không đủ, "
  "toàn bộ giao dịch bị hủy. Nhờ vậy hai quầy cùng bán một thuốc sắp hết không thể làm tồn kho âm.")
sql = doc_tep("SQL/01_TaoCSDL_QLNhaThuoc.sql")
ma(trich(sql, "    BEGIN TRANSACTION;\n\n        UPDATE t SET t.TonKho", "CREATE PROCEDURE dbo.sp_HoaDon_Search"))
p("Phía C#, tầng DAL chuyển giỏ hàng thành DataTable và nhận số hóa đơn qua tham số OUTPUT:")
dal = doc_tep("DAL/NhaThuocDAL.cs")
ma(trich(dal, "        public int TaoHoaDon(", "        public int TaoPhieuNhap("))
h3("4.4.2. Mật khẩu băm có salt")
p("Mỗi tài khoản có một salt ngẫu nhiên 12 byte. Mật khẩu được băm SHA-256(salt + mật khẩu) theo mã hóa UTF-16LE, "
  "trùng với HASHBYTES('SHA2_256', NVARCHAR) của SQL Server, nhờ đó script dữ liệu mẫu tạo được tài khoản mà ứng dụng "
  "đăng nhập được. Thông báo khi sai tên và khi sai mật khẩu là một, để không lộ tài khoản nào đang tồn tại.")
bus = doc_tep("BUS/NhaThuocBUS.cs")
ma(trich(bus, "        public static byte[] BamMatKhau(", "        private static string TaoSalt()"))
h3("4.4.3. Chuẩn hóa xử lý lỗi")
p("Kế thừa Lab 11, mọi lệnh DAL được gọi qua XuLyLoi.Chay. SqlException mang mã 51001-51010 do THROW sinh ra được "
  "chuyển thành LoiNghiepVu với nguyên văn thông báo tiếng Việt; lỗi trùng khóa (2601/2627) và vi phạm ràng buộc (547) "
  "cũng là lỗi nghiệp vụ; còn lại là LoiCsdl: người dùng chỉ thấy câu ngắn kèm mã tham chiếu, chi tiết kỹ thuật được "
  "ghi vào logs\\loi_yyyyMMdd.log.")
ma(trich(doc_tep("BUS/XuLyLoi.cs"), "        public static Exception Dich(", "            string ma = GhiNhatKy(thaoTac, ex);"))
h3("4.4.4. Phân quyền hai lớp")
p("Giao diện chỉ dựng các mục menu thuộc quyền (nhóm Quản trị, Nhập hàng... chỉ có với Quản lý). Tuy nhiên ẩn nút "
  "không đủ an toàn, nên mọi phương thức ghi dữ liệu quan trọng ở BUS đều gọi KiemTra.YeuCauQuanLy() trước khi tới DAL. "
  "Phép thử TC03 chứng minh tài khoản Dược sĩ gọi thẳng ThuocBUS.Them vẫn bị từ chối.")
h3("4.4.5. Tìm kiếm an toàn")
p("sp_Thuoc_Search coi tham số rỗng là \"không lọc\" và vô hiệu hóa ký tự đại diện của LIKE bằng ESCAPE, nên từ khóa "
  "\"10%\" chỉ tìm đúng thuốc có chuỗi \"10%\" trong tên (Povidine 10%), thay vì mọi thuốc có số 10.")
h3("4.4.6. Thiết kế giao diện")
p("Giao diện được thiết kế theo bố cục ApplicationLayout của cổng thông tin hcmue-portal (Next.js + Ant Design) và chuyển sang WinForms: "
  "một cửa sổ duy nhất gồm thanh bên trái rộng 256 px (logo, menu theo nhóm, thẻ người dùng), thanh tiêu đề cao 64 px có breadcrumb "
  "\"Nhóm / Trang\" và ngày của ca làm việc, vùng nội dung nền xám nhạt chứa trang đang chọn. Mỗi chức năng là một trang (lớp ManHinh) "
  "được nhúng vào vùng nội dung thay vì mở cửa sổ rời, nên giỏ hàng đang dở vẫn được giữ khi chuyển sang trang khác.")
bang("Quy ước giao diện", ["Thành phần", "Quy ước"], [
    ("Màu chính", "Cerulean #124874: nút chính, mục menu đang chọn, tiêu đề khối"),
    ("Màu nhấn", "Jasper #CF373D: nút xóa / đăng xuất, cảnh báo sắp hết hạn"),
    ("Nền và viền", "Nền #F4F4F6, khối nội dung trắng, viền mảnh #EAEAEA"),
    ("Menu", "Nhóm có nhãn; đường dẫn hướng dọc bên trái, mục đang chọn tô màu chính và có dấu hình thoi (NavMenu tự vẽ)"),
    ("Nút", "Phẳng, bo góc 6 px: nút chính tô nền; nút phụ nền trắng viền xám, đổi viền sang màu chính khi rê chuột"),
    ("Biểu tượng", "Font Segoe MDL2 Assets có sẵn trên Windows, không cần tệp ảnh"),
    ("Phân quyền", "Menu chỉ dựng các mục thuộc quyền của vai trò đang đăng nhập"),
], rong=[3.2, 12.8], co=11)
h3("4.4.7. Nhật ký người dùng và hiệu năng nghiệp vụ")
p("Mọi lớp BUS kế thừa BusCoSo. Thao tác đọc gọi Doc(); thao tác ghi gọi Ghi(), hàm này đo thời gian bằng Stopwatch và "
  "ghi một dòng vào bảng NhatKy cả khi thành công lẫn khi thất bại (kèm lý do). Việc ghi nhật ký đi qua interface IGhiNhatKy "
  "và được bọc try/catch: nếu ghi lỗi thì chỉ chuyển vào tệp log, giao dịch của người dùng vẫn thành công.")
ma(trich(doc_tep("BUS/BusCoSo.cs"), "        protected static T Ghi<T>(", "    /// <summary>Tra cuu nhat ky"))
p("Trang Nhật ký · Hiệu năng (chỉ Quản lý) truy vấn sp_NhatKy_Search theo khoảng ngày và từ khóa, kèm hai bảng: "
  "doanh thu theo nhân viên (sp_ThongKe_HieuNangNhanVien) và số lần, số lỗi, thời gian xử lý trung bình / lớn nhất "
  "của từng nghiệp vụ (sp_ThongKe_HieuNangThaoTac).")
h3("4.4.8. Truy vấn trong form")
p("Ngoài truy vấn qua Stored Procedure, trang Nhật ký lọc ngay trên dữ liệu đã tải bằng DataView.RowFilter khi người dùng gõ "
  "vào ô Lọc nhanh hoặc chọn Chỉ thất bại, không gửi lệnh nào xuống CSDL; số dòng lỗi và thời gian trung bình "
  "tính bằng DataTable.Compute. Trang Thống kê và Tra cứu hóa đơn tính tổng bằng LINQ / Compute trên DataTable.")
ma(trich(doc_tep("GUI/frmNhatKy.cs"), "        private void Loc()", "            int hien ="))
h3("4.4.9. Báo cáo in")
p("Hai báo cáo cài interface IBaoCao: BaoCaoHoaDon (in hóa đơn cho khách) và BaoCaoDoanhThu. Lớp InBaoCao vẽ mọi IBaoCao "
  "bằng PrintDocument: xem trước, tự sang trang khi dài, in ra máy in hoặc Microsoft Print to PDF. Phần mềm chưa dùng "
  "Crystal Reports vì cần cài thêm runtime của SAP; nhờ tách qua IBaoCao, có thể thêm tệp .rpt dùng IBaoCao.DuLieu làm nguồn mà không sửa nghiệp vụ.")
h2("4.5. Hướng dẫn vận hành")
bang("Thao tác theo vai trò", ["Công việc", "Thao tác trên phần mềm"], [
    ("Bán thuốc", "Bán hàng → gõ tên thuốc, Enter → chọn dòng, nhập SL → \"Thêm vào giỏ\" (hoặc nhấp đúp) → nhập SĐT khách nếu có → nhập số đơn với thuốc kê đơn → THANH TOÁN"),
    ("Thêm thuốc mới", "Danh mục thuốc → Làm mới → nhập mã, tên, nhóm, ĐVT, giá bán → Thêm. Thuốc mới có tồn kho 0 cho tới khi nhập hàng"),
    ("Nhập hàng", "Nhập hàng → chọn NCC → chọn thuốc, SL, giá nhập, hạn dùng → Thêm dòng (lặp lại) → Lưu phiếu nhập"),
    ("Xem cảnh báo", "Màn hình chính tự hiển thị thuốc sắp hết hàng / sắp hết hạn mỗi khi quay lại cửa sổ"),
    ("Báo cáo", "Thống kê doanh thu → chọn khoảng ngày → Xem thống kê → In báo cáo; Tra cứu hóa đơn → chọn hóa đơn → In hóa đơn"),
    ("Nhật ký", "Quản trị → Nhật ký · Hiệu năng → chọn khoảng ngày, từ khóa → Truy vấn; gõ vào ô Lọc nhanh để lọc tại chỗ"),
    ("Tài khoản", "Nhân viên → Tạo tài khoản / khóa (bỏ chọn \"Đang hoạt động\") / Đặt lại mật khẩu"),
], rong=[3.2, 12.8], co=11)

# ================================================================== CHUONG 5
ngat_trang()
h1("CHƯƠNG 5. KẾT QUẢ CHƯƠNG TRÌNH")
p("Các hình dưới đây được chụp từ phần mềm chạy trên CSDL mẫu (22 thuốc, 60 hóa đơn trong 30 ngày).")
for tep, chu, mo_ta in [
    ("MH01_DangNhap.png", "Màn hình đăng nhập", "Đăng nhập bằng tài khoản nhân viên; Enter để xác nhận."),
    ("MH02_ManHinhChinh.png", "Màn hình chính (Quản lý)", "Bố cục portal: thanh bên trái theo nhóm chức năng, breadcrumb trên cùng; trang Bảng điều khiển có bốn thẻ chỉ số và bảng cảnh báo kho."),
    ("MH11_ManHinhChinh_DuocSi.png", "Màn hình chính (Dược sĩ)", "Tài khoản Dược sĩ: menu chỉ còn các nhóm Tổng quan, Bán hàng và Danh mục thuốc."),
    ("MH03_BanHang.png", "Màn hình bán hàng", "Trái: danh sách thuốc còn kinh doanh (ẩn giá nhập). Phải: thông tin khách, giỏ hàng, tổng tiền và nút Thanh toán."),
    ("MH04_DanhMucThuoc.png", "Danh mục thuốc", "Tìm và lọc theo nhóm; chọn dòng để sửa, mã thuốc bị khóa khi sửa."),
    ("MH05_NhapHang.png", "Nhập hàng", "Phiếu nhập với hai lô thuốc, tổng tiền phiếu cập nhật tức thì."),
    ("MH06_HoaDon.png", "Tra cứu hóa đơn", "Danh sách hóa đơn 7 ngày gần nhất và chi tiết của hóa đơn đang chọn."),
    ("MH07_ThongKe.png", "Thống kê doanh thu", "Biểu đồ cột doanh thu 30 ngày và 10 thuốc bán chạy nhất."),
    ("MH08_KhachHang.png", "Khách hàng", "Danh sách khách kèm số hóa đơn và tổng tiền đã mua."),
    ("MH09_DanhMuc.png", "Nhóm thuốc và nhà cung cấp", "Hai danh mục trên hai tab, mỗi tab đủ Thêm / Sửa / Xóa."),
    ("MH10_NhanVien.png", "Quản lý nhân viên", "Tạo tài khoản, phân quyền, khóa và đặt lại mật khẩu."),
    ("MH12_NhatKy.png", "Nhật ký · Hiệu năng", "Nhật ký thao tác (thất bại tô đỏ), lọc nhanh tại chỗ; hiệu năng bán hàng theo nhân viên và thời gian xử lý theo nghiệp vụ."),
    ("MH13_InHoaDon.png", "In hóa đơn bán lẻ", "Bản in hóa đơn do InBaoCao vẽ từ BaoCaoHoaDon."),
    ("MH14_InBaoCaoDoanhThu.png", "In báo cáo doanh thu", "Báo cáo doanh thu 14 ngày, có dòng tổng."),
]:
    so_muc5 = so_hinh[0] - 7
    h2("5.%d. %s" % (so_muc5, chu))
    p(mo_ta)
    hinh(tep, chu, rong=10 if tep.startswith(("MH13", "MH14")) else 15.5)

h2("5.%d. Kết quả chạy thực tế các tình huống" % (so_muc5 + 1))
p("Các hình sau được chụp trực tiếp từ màn hình khi chạy phần mềm thật (kể cả hộp thoại thông báo), thể hiện cách phần mềm phản ứng với từng tình huống nghiệp vụ.")
for tep, chu in [("TH01_DangNhapSai.png", "Đăng nhập sai mật khẩu - thông báo chung, không lộ tài khoản"),
                 ("TH02_VuotTonKho.png", "Thêm 500 viên Augmentin khi kho chỉ còn 15 - bị chặn ngay ở giỏ hàng"),
                 ("TH03_ThieuSoDonThuoc.png", "Giỏ có thuốc kê đơn (Amoxicillin) nhưng chưa nhập số đơn thuốc"),
                 ("TH04_ThanhToanThanhCong.png", "Thanh toán thành công cho khách quen tìm theo số điện thoại"),
                 ("TH05_HoaDonVuaLap.png", "Hóa đơn vừa lập xuất hiện trong tra cứu hóa đơn"),
                 ("TH06_ManHinhChinhSauBan.png", "Màn hình chính cập nhật doanh thu hôm nay sau khi bán")]:
    hinh(tep, chu, rong=14.5)

# ================================================================== CHUONG 6
ngat_trang()
h1("CHƯƠNG 6. KIỂM THỬ")
h2("6.1. Phương pháp")
p("Bộ kiểm thử tự động (Tools\\KiemThu.cs, chạy bằng QuanLyNhaThuoc.exe --kiemtra) gọi đúng các lớp BUS mà giao diện "
  "dùng, trên CSDL thật. Dữ liệu thử có tiền tố KT và được xóa sạch khi kết thúc, kể cả khi có phép thử thất bại. "
  "Phép thử cuối cùng đối chiếu: tổng số lượng nhập trừ tổng số lượng bán phải bằng tổng tồn kho, chứng minh dữ liệu gốc "
  "không bị ảnh hưởng. Một số tình huống khó tạo bằng giao diện (quầy khác bán mất hàng giữa chừng, thuốc vừa hết hạn) "
  "được mô phỏng bằng cách sửa trực tiếp dữ liệu trong CSDL trước khi gọi nghiệp vụ.")
h2("6.2. Kết quả kiểm thử")
kq = doc_tep("BaoCao/kiem_thu.txt")
dong_tc = []
cuoi = None
for dong in kq.splitlines():
    m = re.match(r"\s+\[(DAT|SAI)\] (.*)", dong)
    if m:
        cuoi = [len(dong_tc) + 1, m.group(2), "", "Đạt" if m.group(1) == "DAT" else "KHÔNG ĐẠT"]
        dong_tc.append(cuoi)
    elif dong.strip().startswith("->") and cuoi:
        cuoi[2] = dong.strip()[2:].strip()
MO_TA = {
    "TC01": "Sai mật khẩu bị từ chối", "TC02": "Đăng nhập duocsi / 123456 (hash C# khớp HASHBYTES)",
    "TC03": "Dược sĩ không được thêm thuốc (chặn ở BUS)", "TC04": "Giá bán ≤ 0 bị từ chối",
    "TC05": "Thêm thuốc mới, mã viết hoa, tồn kho = 0", "TC06": "Trùng mã thuốc (THROW 51003)",
    "TC07": "Tìm \"10%\": % là ký tự thường", "TC08": "Không nhập lô đã hết hạn",
    "TC09": "Lưu phiếu nhập: tồn KT01 = 50, KT02 = 20", "TC10": "Giá nhập, hạn dùng cập nhật theo lô",
    "TC11": "Trùng thuốc trong giỏ thì cộng dồn", "TC12": "Thêm vượt tồn kho bị chặn ở giỏ hàng",
    "TC13": "Thanh toán: tồn 50 → 40, tổng tiền 50.000", "TC14": "Thuốc kê đơn bắt buộc số đơn",
    "TC15": "SP kiểm tra lại tồn kho khi quầy khác vừa bán (51004)", "TC16": "Rollback: không sinh hóa đơn, tồn kho giữ nguyên",
    "TC17": "Thuốc hết hạn không vào được giỏ", "TC18": "Xóa thuốc có giao dịch → ngừng kinh doanh, giữ lịch sử",
    "TC19": "Không xóa nhóm đang có thuốc (51009)", "TC20": "Không trùng SĐT khách (51008)",
    "TC21": "Lỗi SQL kỹ thuật → thông báo thân thiện + mã tham chiếu",
    "TC23": "ThuocDAL dùng qua interface ICrudDAL<T>, kế thừa DalCoSo",
    "TC24": "Thao tác ghi được ghi nhật ký kèm thời gian xử lý",
    "TC25": "Thao tác thất bại cũng được ghi nhật ký (bán vượt tồn)",
    "TC26": "Báo cáo doanh thu (IBaoCao) có dữ liệu và dòng tổng",
}
for d in dong_tc:
    ma_tc = d[1].split(" ")[0]
    if ma_tc in MO_TA:
        d[1] = ma_tc + ": " + MO_TA[ma_tc]
    elif d[1].startswith("Du lieu goc"):
        d[1] = "Dữ liệu gốc: tồn kho = tổng nhập - tổng bán sau dọn dẹp"
    if len(d[2]) > 90:
        d[2] = d[2][:87] + "..."
bang("Kết quả kiểm thử tự động", ["STT", "Trường hợp kiểm thử", "Kết quả thực tế", "Đánh giá"], dong_tc, rong=[1.1, 6.6, 6.5, 1.8], co=9.5)
tong = re.search(r"KET QUA: (\d+)/(\d+)", kq)
p("Kết quả: **%s/%s** phép thử đạt. Ngoài kiểm thử tự động, các màn hình ở Chương 5 được kiểm tra trực quan về bố cục, "
  "định dạng tiền tệ, ngày tháng và phân quyền hiển thị." % (tong.group(1), tong.group(2)))

# ================================================================== KET LUAN
ngat_trang()
h1("KẾT LUẬN VÀ HƯỚNG PHÁT TRIỂN")
h2("Kết quả đạt được")
for g in ["Hoàn thành phần mềm quản lý nhà thuốc với 14 nhóm chức năng, 12 màn hình, chạy được trên dữ liệu mẫu.",
          "Áp dụng đầy đủ kiến thức các bài lab: OOP, WinForms, mô hình 3 lớp, ADO.NET tham số hóa, DataTable, Stored Procedure, THROW, xóa mềm, transaction và chuẩn hóa xử lý lỗi.",
          "Bảo đảm toàn vẹn dữ liệu ở mức CSDL: hóa đơn và phiếu nhập ghi trọn vẹn trong một transaction; tồn kho không thể âm kể cả khi nhiều quầy bán đồng thời.",
          "Bảo mật cơ bản: mật khẩu băm có salt, chống SQL Injection, phân quyền kiểm tra ở tầng nghiệp vụ.",
          "Thiết kế hướng đối tượng với 6 interface và 4 lớp trừu tượng; nhật ký người dùng, hiệu năng nghiệp vụ và báo cáo in.",
          "Có bộ kiểm thử tự động %s phép thử và đầy đủ sơ đồ phân tích thiết kế." % tong.group(2)]:
    gach(g)
h2("Hạn chế")
for g in ["Tồn kho quản lý theo tổng của mỗi thuốc, chưa tách từng lô khi bán (chưa bán theo nguyên tắc hết hạn trước - xuất trước).",
          "Báo cáo in dùng PrintDocument, chưa dùng Crystal Reports; chưa có đơn vị quy đổi (hộp - vỉ - viên).",
          "Chưa có chức năng trả hàng / hủy hóa đơn và kiểm kê kho."]:
    gach(g)
h2("Hướng phát triển")
for g in ["Quản lý tồn kho theo lô và tự động chọn lô gần hết hạn khi bán (FEFO).",
          "In hóa đơn bằng máy in nhiệt, quét mã vạch thuốc khi bán.",
          "Báo cáo bằng Crystal Reports, xuất Excel; cảnh báo khi thời gian xử lý nghiệp vụ vượt ngưỡng.",
          "Chuyển sang ASP.NET Core Web API + Entity Framework Core để dùng trên nhiều chi nhánh."]:
    gach(g)

h1("TÀI LIỆU THAM KHẢO")
for t in ["Microsoft Learn, \"ADO.NET overview\", learn.microsoft.com/dotnet/framework/data/adonet.",
          "Microsoft Learn, \"Table-valued parameters\", learn.microsoft.com/sql/relational-databases/tables/use-table-valued-parameters-database-engine.",
          "Microsoft Learn, \"THROW (Transact-SQL)\" và \"SET XACT_ABORT (Transact-SQL)\".",
          "Microsoft Learn, \"Windows Forms documentation\", learn.microsoft.com/dotnet/desktop/winforms.",
          "Bộ môn Công nghệ phần mềm, Khoa CNTT - Trường ĐH Công nghệ Đông Á, Tài liệu thực hành Lập trình .NET (IT3212), Lab 1 - Lab 12."]:
    so(t, moi=t.startswith("Microsoft Learn, \"ADO"), deu=False)

doc.save(RA)
print("Da tao", RA)
