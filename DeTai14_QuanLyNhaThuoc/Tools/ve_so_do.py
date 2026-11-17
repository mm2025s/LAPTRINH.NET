"""Ve cac so do phan tich thiet ke cho bao cao (PNG, 200 dpi) vao BaoCao/HinhAnh."""
import os

import matplotlib

matplotlib.use("Agg")
import matplotlib.pyplot as plt
from matplotlib.patches import Ellipse, FancyArrowPatch, FancyBboxPatch, Polygon, Rectangle

plt.rcParams["font.family"] = "Segoe UI"
GOC = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
RA = os.path.join(GOC, "BaoCao", "HinhAnh")
os.makedirs(RA, exist_ok=True)

TEAL, BLUE, AMBER, VIOLET, SLATE, RED = "#0d9488", "#2563eb", "#d97706", "#7c3aed", "#475569", "#dc2626"
INK = "#1e2230"


def nhat(c, a=0.12):
    c = c.lstrip("#")
    r, g, b = (int(c[i:i + 2], 16) for i in (0, 2, 4))
    return "#%02x%02x%02x" % tuple(int(255 - (255 - v) * a) for v in (r, g, b))


def ve(ten, w, h, ham):
    fig, ax = plt.subplots(figsize=(w, h))
    ax.set_xlim(0, w * 10)
    ax.set_ylim(0, h * 10)
    ax.axis("off")
    ham(ax)
    fig.savefig(os.path.join(RA, ten), dpi=200, bbox_inches="tight", facecolor="white")
    plt.close(fig)
    print("da ve", ten)


def hop(ax, x, y, w, h, chu, mau=BLUE, co=9, dam=False, nen=None, bo=0.6):
    ax.add_patch(FancyBboxPatch((x, y), w, h, boxstyle="round,pad=0,rounding_size=%s" % bo,
                                fc=nen or nhat(mau), ec=mau, lw=1.4))
    ax.text(x + w / 2, y + h / 2, chu, ha="center", va="center", fontsize=co, color=INK,
            fontweight="bold" if dam else "normal", wrap=True)


def mui_ten(ax, a, b, chu=None, mau=SLATE, kieu="-|>", net="-", co=8, lech=(0, 0.8)):
    ax.add_patch(FancyArrowPatch(a, b, arrowstyle=kieu, mutation_scale=12, color=mau, lw=1.2, linestyle=net))
    if chu:
        ax.text((a[0] + b[0]) / 2 + lech[0], (a[1] + b[1]) / 2 + lech[1], chu, ha="center", va="bottom",
                fontsize=co, color=mau, style="italic")


# ---------------------------------------------------------------- 1. Kien truc
def kien_truc(ax):
    tang = [
        ("GUI - Tầng giao diện (WinForms)", TEAL, "frmDangNhap · frmMain · frmBanHang · frmThuoc · frmNhapHang\nfrmHoaDon · frmThongKe · frmKhachHang · frmDanhMuc · frmNhanVien\nChỉ thu thập dữ liệu, hiển thị kết quả; không chứa SQL"),
        ("BUS - Tầng nghiệp vụ", BLUE, "NhanVienBUS (đăng nhập, băm mật khẩu, phân quyền) · ThuocBUS · KhachHangBUS\nBanHangBUS (giỏ hàng, thuốc kê đơn) · NhapHangBUS · ThongKeBUS · DanhMucBUS\nXuLyLoi: phân loại lỗi Nhập liệu / Nghiệp vụ / SQL / Hệ thống + ghi nhật ký"),
        ("DAL - Tầng truy cập dữ liệu (ADO.NET)", AMBER, "Db: SqlConnection · SqlCommand (CommandType.StoredProcedure) · SqlParameter\nSqlDataAdapter → DataTable · Table-Valued Parameter cho giỏ hàng / phiếu nhập\nNhanVienDAL · ThuocDAL · KhachHangDAL · GiaoDichDAL · ThongKeDAL · DanhMucDAL"),
        ("SQL Server - CSDL QLNhaThuoc", VIOLET, "10 bảng (có NhatKy) · khóa chính / khóa ngoại / CHECK / UNIQUE · 2 kiểu bảng (TVP)\n30 Stored Procedure: CRUD, nhật ký, hiệu năng, sp_HoaDon_Tao và sp_PhieuNhap_Tao (TRANSACTION + XACT_ABORT)\nTHROW 51001-51010 trả lỗi nghiệp vụ về ứng dụng"),
    ]
    y = 64
    for i, (ten, mau, mo_ta) in enumerate(tang):
        ax.add_patch(FancyBboxPatch((4, y), 92, 14, boxstyle="round,pad=0,rounding_size=1", fc=nhat(mau, .10), ec=mau, lw=1.6))
        ax.text(7, y + 11.2, ten, fontsize=11, fontweight="bold", color=mau, va="center")
        ax.text(7, y + 5.2, mo_ta, fontsize=8.6, color=INK, va="center", linespacing=1.5)
        if i < 3:
            mui_ten(ax, (40, y), (40, y - 6), "gọi", mau=SLATE, lech=(-4, -0.4))
            mui_ten(ax, (60, y - 6), (60, y), "kết quả / ngoại lệ đã dịch", mau=SLATE, net="--", lech=(13, -0.4))
        y -= 20
    ax.add_patch(FancyBboxPatch((99, 6), 18, 72, boxstyle="round,pad=0,rounding_size=1", fc=nhat(SLATE, .08), ec=SLATE, lw=1.2, ls="--"))
    ax.text(108, 72, "DTO", fontsize=11, fontweight="bold", color=SLATE, ha="center")
    ax.text(108, 42, "ThuocDTO\nKhachHangDTO\nNhanVienDTO\nDongGioHang\nDongNhapHang\nThongKeTongQuan\n\nLoiNhapLieu\nLoiNghiepVu\nLoiCsdl\n\nPhienDangNhap", fontsize=8.2, ha="center", va="center", color=INK, linespacing=1.35)
    ax.text(108, 10, "dùng chung\ncho mọi tầng", fontsize=7.5, ha="center", color=SLATE, style="italic")


# ---------------------------------------------------------------- 2. Use case
def nguoi(ax, x, y, ten):
    ax.add_patch(Ellipse((x, y + 7), 3.2, 3.2, fc="white", ec=INK, lw=1.3))
    ax.plot([x, x], [y + 5.4, y], color=INK, lw=1.3)
    ax.plot([x - 3, x + 3], [y + 3.6, y + 3.6], color=INK, lw=1.3)
    ax.plot([x, x - 2.6], [y, y - 4], color=INK, lw=1.3)
    ax.plot([x, x + 2.6], [y, y - 4], color=INK, lw=1.3)
    ax.text(x, y - 6.5, ten, ha="center", fontsize=9.5, fontweight="bold")


def uc(ax, x, y, chu, mau=BLUE, w=26):
    ax.add_patch(Ellipse((x, y), w, 6.4, fc=nhat(mau, .14), ec=mau, lw=1.3))
    ax.text(x, y, chu, ha="center", va="center", fontsize=8.4)


def use_case(ax):
    ax.add_patch(Rectangle((22, 3), 96, 94, fc="none", ec=SLATE, lw=1.2))
    ax.text(70, 94, "Hệ thống quản lý nhà thuốc", ha="center", fontsize=11, fontweight="bold", color=SLATE)
    nguoi(ax, 8, 70, "Dược sĩ")
    nguoi(ax, 8, 22, "Quản lý")
    mui_ten(ax, (8, 31), (8, 59), kieu="-|>", mau=INK)
    ax.text(10, 46, "kế thừa", fontsize=7.5, style="italic", color=SLATE)

    ds = [(42, 86, "Đăng nhập / Đăng xuất"), (42, 77, "Đổi mật khẩu"), (42, 68, "Bán hàng - lập hóa đơn"),
          (42, 59, "Tra cứu thuốc"), (42, 50, "Quản lý khách hàng"), (42, 41, "Tra cứu hóa đơn"), (42, 32, "Xem cảnh báo kho")]
    ql = [(42, 22, "Quản lý thuốc"), (42, 12, "Nhập hàng"), (96, 22, "Quản lý nhóm thuốc, NCC"),
          (96, 12, "Thống kê doanh thu"), (96, 32, "Quản lý nhân viên")]
    for x, y, t in ds:
        uc(ax, x, y, t, TEAL)
        ax.plot([11, x - 13], [72, y], color=TEAL, lw=.8)
    for x, y, t in ql:
        uc(ax, x, y, t, VIOLET)
        ax.plot([11, x - 13], [24, y], color=VIOLET, lw=.8)

    inc = [(96, 76, "Kiểm tra tồn kho"), (96, 66, "Kiểm tra hạn dùng"), (96, 56, "Kiểm tra đơn thuốc\n(thuốc kê đơn)"), (96, 46, "Tìm / thêm khách\ntheo SĐT")]
    for x, y, t in inc:
        uc(ax, x, y, t, AMBER, w=24)
        mui_ten(ax, (55, 68), (x - 12, y), mau=AMBER, net="--", kieu="->")
    ax.text(74, 74, "«include»", fontsize=7.5, color=AMBER, style="italic")
    ax.text(76, 54, "«include»", fontsize=7.5, color=AMBER, style="italic")
    ax.text(57, 35, "", fontsize=7)


# ---------------------------------------------------------------- 3. ERD
def bang(ax, x, y, ten, cot, mau=BLUE, w=24):
    h = 3.4 * (len(cot) + 1)
    ax.add_patch(Rectangle((x, y - h), w, h, fc="white", ec=mau, lw=1.4))
    ax.add_patch(Rectangle((x, y - 3.4), w, 3.4, fc=mau, ec=mau))
    ax.text(x + w / 2, y - 1.7, ten, ha="center", va="center", color="white", fontsize=9, fontweight="bold")
    for i, c in enumerate(cot):
        dam = c.startswith("PK")
        ax.text(x + 1, y - 3.4 * (i + 1.5), c, va="center", fontsize=7.6, fontweight="bold" if dam else "normal",
                color=RED if c.startswith("FK") else INK)
    return (x, y, w, h)


def erd(ax):
    b = {}
    b["NV"] = bang(ax, 2, 96, "NhanVien", ["PK MaNV", "HoTen", "TenDangNhap (UQ)", "MatKhauHash", "Salt", "VaiTro (CK)", "IsActive"], VIOLET)
    b["HD"] = bang(ax, 36, 96, "HoaDon", ["PK MaHD", "NgayLap", "FK MaNV", "FK MaKH (NULL)", "TongTien", "GhiChu"], TEAL)
    b["KH"] = bang(ax, 70, 96, "KhachHang", ["PK MaKH", "HoTen", "DienThoai (UQ)", "DiaChi"], BLUE)
    b["CTHD"] = bang(ax, 36, 66, "ChiTietHoaDon", ["PK,FK MaHD", "PK,FK MaThuoc", "SoLuong (CK > 0)", "DonGia"], TEAL)
    b["T"] = bang(ax, 70, 66, "Thuoc", ["PK MaThuoc", "TenThuoc", "FK MaNhom", "DonViTinh", "GiaNhap, GiaBan", "TonKho (CK >= 0)", "HanSuDung", "CanKeDon", "IsActive"], AMBER, w=26)
    b["NT"] = bang(ax, 104, 66, "NhomThuoc", ["PK MaNhom", "TenNhom (UQ)"], AMBER, w=20)
    b["PN"] = bang(ax, 2, 50, "PhieuNhap", ["PK MaPN", "NgayNhap", "FK MaNV", "FK MaNCC", "TongTien"], VIOLET)
    b["CTPN"] = bang(ax, 36, 32, "ChiTietPhieuNhap", ["PK,FK MaPN", "PK,FK MaThuoc", "SoLuong", "DonGiaNhap", "HanSuDung"], VIOLET)
    b["NCC"] = bang(ax, 2, 18, "NhaCungCap", ["PK MaNCC", "TenNCC", "DienThoai", "DiaChi"], SLATE)

    def noi(p, q, nhan_p="1", nhan_q="n"):
        ax.plot([p[0], q[0]], [p[1], q[1]], color=SLATE, lw=1.1)
        ax.text(p[0] + (q[0] - p[0]) * .1, p[1] + (q[1] - p[1]) * .1 + .8, nhan_p, fontsize=8, color=RED, fontweight="bold")
        ax.text(p[0] + (q[0] - p[0]) * .88, p[1] + (q[1] - p[1]) * .88 + .8, nhan_q, fontsize=8, color=RED, fontweight="bold")

    noi((26, 88), (36, 88))            # NV - HD
    noi((70, 88), (60, 88))            # KH - HD
    noi((48, 75.6), (48, 66))          # HD - CTHD
    noi((70, 58), (60, 58))            # T - CTHD
    noi((104, 62), (96, 62))           # NT - T
    noi((14, 69.8), (14, 50))          # NV - PN
    noi((14, 18), (14, 30.6))          # NCC - PN
    noi((26, 40), (36, 26))            # PN - CTPN
    noi((80, 31.4), (60, 24))          # T - CTPN
    ax.set_ylim(-6, 100)
    ax.text(62, -4, "Kiểu bảng (TVP): DongHoaDonType(MaThuoc, SoLuong) · DongPhieuNhapType(MaThuoc, SoLuong, DonGiaNhap, HanSuDung)",
            ha="center", fontsize=8, color=SLATE, style="italic")


# ---------------------------------------------------------------- 4. Phan ra chuc nang
def bfd(ax):
    hop(ax, 45, 88, 40, 8, "PHẦN MỀM QUẢN LÝ NHÀ THUỐC", TEAL, 10.5, True)
    nhom = [
        ("Hệ thống", VIOLET, ["Đăng nhập / Đăng xuất", "Đổi mật khẩu", "Quản lý nhân viên", "Phân quyền theo vai trò"]),
        ("Danh mục", BLUE, ["Thuốc", "Nhóm thuốc", "Nhà cung cấp", "Khách hàng"]),
        ("Nghiệp vụ", AMBER, ["Bán hàng - lập hóa đơn", "Nhập hàng - phiếu nhập", "Tra cứu hóa đơn", "Cảnh báo tồn kho / hạn dùng"]),
        ("Báo cáo", RED, ["Tổng quan trong ngày", "Doanh thu theo ngày", "Top 10 thuốc bán chạy"]),
    ]
    ax.plot([16, 114], [80, 80], color=SLATE, lw=1.1)
    ax.plot([65, 65], [88, 80], color=SLATE, lw=1.1)
    for i, (ten, mau, con) in enumerate(nhom):
        x = 2 + i * 33
        ax.plot([x + 14, x + 14], [80, 76], color=SLATE, lw=1.1)
        hop(ax, x, 68, 28, 8, ten, mau, 10, True)
        for j, c in enumerate(con):
            y = 56 - j * 12
            ax.plot([x + 3, x + 3, x + 6], [68, y + 4, y + 4], color=mau, lw=1)
            hop(ax, x + 6, y, 24, 8, c, mau, 8.4)


# ---------------------------------------------------------------- 5. Hoat dong ban hang
def kim_cuong(ax, x, y, chu, mau=AMBER):
    ax.add_patch(Polygon([(x, y + 5), (x + 13, y), (x, y - 5), (x - 13, y)], fc=nhat(mau, .16), ec=mau, lw=1.3))
    ax.text(x, y, chu, ha="center", va="center", fontsize=8)


def hoat_dong(ax):
    ax.add_patch(Ellipse((40, 136), 3, 3, fc=INK))
    buoc = [(126, "Tìm thuốc theo tên / mã"), (114, "Chọn thuốc, nhập số lượng, bấm\n\"Thêm vào giỏ\"")]
    mui_ten(ax, (40, 134.5), (40, 130))
    for y, t in buoc:
        hop(ax, 22, y, 36, 8, t, TEAL, 8.5)
    mui_ten(ax, (40, 126), (40, 122))
    mui_ten(ax, (40, 114), (40, 108))
    kim_cuong(ax, 40, 103, "Còn hạn và\nđủ tồn kho?")
    hop(ax, 66, 99, 30, 8, "Báo lỗi nghiệp vụ,\nkhông thêm vào giỏ", RED, 8.2)
    mui_ten(ax, (53, 103), (66, 103), "Không", RED)
    mui_ten(ax, (81, 107), (81, 118), mau=RED)
    ax.plot([81, 58], [118, 118], color=RED, lw=1.2)
    mui_ten(ax, (40, 98), (40, 92), "Có", lech=(3, -1))
    hop(ax, 22, 84, 36, 8, "Cộng vào giỏ (trùng thuốc thì\ncộng dồn) · cập nhật tổng tiền", TEAL, 8.2)
    mui_ten(ax, (40, 84), (40, 79))
    kim_cuong(ax, 40, 74, "Mua thêm?")
    ax.plot([27, 14, 14], [74, 74, 130], color=SLATE, lw=1.2)
    mui_ten(ax, (14, 130), (22, 130), "Có", lech=(-4, 0.5))
    mui_ten(ax, (40, 69), (40, 63), "Không", lech=(4, -1))
    hop(ax, 22, 55, 36, 8, "(Tùy chọn) nhập SĐT khách:\ntìm hoặc tạo khách mới", BLUE, 8.2)
    mui_ten(ax, (40, 55), (40, 50))
    kim_cuong(ax, 40, 45, "Có thuốc kê đơn\nmà chưa nhập\nsố đơn?")
    hop(ax, 66, 41, 30, 8, "Yêu cầu nhập số đơn\nthuốc của bác sĩ", RED, 8.2)
    mui_ten(ax, (53, 45), (66, 45), "Có", RED)
    mui_ten(ax, (40, 40), (40, 34), "Không", lech=(4, -1))
    hop(ax, 18, 25, 44, 9, "sp_HoaDon_Tao (1 TRANSACTION):\nkiểm tra lại tồn kho · trừ kho · ghi HoaDon + ChiTietHoaDon", VIOLET, 8.2)
    mui_ten(ax, (40, 25), (40, 20))
    kim_cuong(ax, 40, 15, "Thành công?")
    hop(ax, 66, 11, 30, 8, "ROLLBACK · báo \"không đủ\ntồn kho\" · tải lại tồn kho", RED, 8.2)
    mui_ten(ax, (53, 15), (66, 15), "Không", RED)
    mui_ten(ax, (40, 10), (40, 5), "Có", lech=(3, -1))
    hop(ax, 22, -3, 36, 8, "Thông báo số hóa đơn,\nlàm trống giỏ", TEAL, 8.2)
    ax.set_ylim(-8, 140)


# ---------------------------------------------------------------- 6. Tuan tu lap hoa don
def tuan_tu(ax):
    ds = [("Dược sĩ", 12), (":frmBanHang", 32), (":BanHangBUS", 58), (":GiaoDichDAL", 84), ("SQL Server\nsp_HoaDon_Tao", 110)]
    for ten, x in ds:
        hop(ax, x - 10, 84, 20, 8, ten, BLUE if x > 12 else TEAL, 8.5, True)
        ax.plot([x, x], [84, 4], color=SLATE, lw=.9, ls="--")
    tin = [
        (12, 32, 78, "1. Bấm \"THANH TOÁN\""),
        (32, 58, 72, "2. ThanhToan(maKH, soDonThuoc)"),
        (58, 58, 66, "3. Kiểm tra: giỏ rỗng?\nthuốc kê đơn có số đơn?"),
        (58, 84, 58, "4. TaoHoaDon(maNV, maKH, ghiChu, Gio)"),
        (84, 84, 52, "5. Gio → DataTable\n(tham số kiểu bảng)"),
        (84, 110, 44, "6. EXEC sp_HoaDon_Tao @ChiTiet, @MaHD OUTPUT"),
        (110, 110, 36, "7. BEGIN TRAN · UPDATE TonKho\nWHERE TonKho >= SoLuong\nINSERT HoaDon, ChiTietHoaDon · COMMIT"),
        (110, 84, 26, "8. @MaHD  (hoặc THROW 51004 + ROLLBACK)"),
        (84, 58, 20, "9. maHD  (SqlException → LoiNghiepVu)"),
        (58, 32, 14, "10. maHD · làm trống giỏ"),
        (32, 12, 8, "11. \"Đã lập hóa đơn số ...\""),
    ]
    for a, b, y, t in tin:
        if a == b:
            ax.add_patch(FancyArrowPatch((a, y + 2), (a, y - 2), connectionstyle="arc3,rad=-1.2", arrowstyle="-|>", mutation_scale=10, color=AMBER, lw=1.1))
            ax.text(a + 4, y, t, fontsize=7.6, va="center", color=INK)
        else:
            mui_ten(ax, (a, y), (b, y), t, mau=INK if b > a else SLATE, net="-" if b > a else "--", co=7.8, lech=(0, .4))


# ---------------------------------------------------------------- 7. Trien khai
def trien_khai(ax):
    ax.set_ylim(0, 92)
    for i, (ten, x) in enumerate([("Máy quầy 1 (Dược sĩ)", 4), ("Máy quầy 2 (Dược sĩ)", 4), ("Máy quản lý", 4)]):
        y = 66 - i * 26
        ax.add_patch(Rectangle((x, y), 34, 20, fc=nhat(TEAL, .08), ec=TEAL, lw=1.3))
        ax.text(x + 17, y + 16.5, "«thiết bị» " + ten, ha="center", fontsize=8.6, fontweight="bold", color=TEAL)
        hop(ax, x + 3, y + 2.5, 28, 10, "QuanLyNhaThuoc.exe\n.NET Framework 4.8 · App.config\nlogs\\loi_yyyyMMdd.log", TEAL, 7.6, nen="white")
        mui_ten(ax, (38, y + 10), (68, 50), kieu="<|-|>", mau=SLATE)
    ax.text(53, 84, "TCP 1433 / Shared Memory\nADO.NET SqlClient\n(Windows hoặc SQL Authentication)", ha="center", fontsize=7.8, color=SLATE, style="italic")
    ax.add_patch(Rectangle((68, 22), 46, 56, fc=nhat(VIOLET, .08), ec=VIOLET, lw=1.3))
    ax.text(91, 74, "«máy chủ» SQL Server 2019+ / LocalDB", ha="center", fontsize=8.8, fontweight="bold", color=VIOLET)
    hop(ax, 72, 52, 38, 16, "CSDL QLNhaThuoc\n10 bảng · 2 kiểu bảng · 30 Stored Procedure", VIOLET, 8.2, nen="white")
    hop(ax, 72, 28, 38, 16, "Sao lưu định kỳ\nBACKUP DATABASE QLNhaThuoc\nTO DISK = '...\\QLNhaThuoc.bak'", SLATE, 8, nen="white")


# ---------------------------------------------------------------- 8. Quy trinh van hanh
def van_hanh(ax):
    buoc = [
        ("1. Đầu ca", "Nhân viên đăng nhập\nxem màn hình tổng quan\n& cảnh báo kho", TEAL),
        ("2. Bán hàng", "Tìm thuốc → giỏ hàng\n→ thanh toán\n(tồn kho tự trừ)", TEAL),
        ("3. Theo dõi kho", "Cảnh báo sắp hết hàng\n(≤ 20) và sắp hết hạn\n(≤ 90 ngày)", AMBER),
        ("4. Nhập hàng", "Quản lý lập phiếu nhập\ntheo lô: SL, giá, hạn\n(tồn kho tự cộng)", AMBER),
        ("5. Cuối ngày", "Thống kê doanh thu,\ntop thuốc bán chạy,\ntra cứu hóa đơn", VIOLET),
    ]
    for i, (ten, mo_ta, mau) in enumerate(buoc):
        x = 2 + i * 24
        hop(ax, x, 20, 20, 22, "", mau)
        ax.text(x + 10, 37.5, ten, ha="center", fontsize=9.5, fontweight="bold", color=mau)
        ax.text(x + 10, 27.5, mo_ta, ha="center", va="center", fontsize=7.8, color=INK, linespacing=1.4)
        if i < 4:
            mui_ten(ax, (x + 20, 31), (x + 24, 31), mau=SLATE)
    ax.add_patch(FancyArrowPatch((84, 20), (36, 20), connectionstyle="arc3,rad=-0.35", arrowstyle="-|>", mutation_scale=12, color=AMBER, lw=1.2, ls="--"))
    ax.text(60, 6, "hàng mới nhập sẵn sàng để bán tiếp", ha="center", fontsize=8, color=AMBER, style="italic")
    ax.add_patch(FancyArrowPatch((60, 42), (84, 42), connectionstyle="arc3,rad=-0.4", arrowstyle="-|>", mutation_scale=12, color=RED, lw=1.2, ls="--"))
    ax.text(72, 50, "cảnh báo kích hoạt nhập hàng", ha="center", fontsize=8, color=RED, style="italic")


ve("SD01_KienTruc3Lop.png", 12, 8.4, kien_truc)
ve("SD02_UseCase.png", 12, 10, use_case)
ve("SD03_ERD.png", 12.6, 10, erd)
ve("SD04_PhanRaChucNang.png", 13.2, 9.8, bfd)
ve("SD05_HoatDongBanHang.png", 10, 14.6, hoat_dong)
ve("SD06_TuanTuLapHoaDon.png", 12.4, 9.4, tuan_tu)
ve("SD07_TrienKhai.png", 11.6, 8, trien_khai)
ve("SD08_QuyTrinhVanHanh.png", 12.2, 5.6, van_hanh)
