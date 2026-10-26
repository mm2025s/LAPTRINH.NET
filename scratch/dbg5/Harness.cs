// Scratch: chạy từng bước để tìm bước bị treo (harness, /main:Harness).
using System;
using System.Threading;
using System.Windows.Forms;

public static class Harness
{
    static void Buoc(string ten, Action a)
    {
        Console.WriteLine(">> " + ten + " ...");
        var t = new Thread(() =>
        {
            try { a(); }
            catch (Exception ex) { Console.WriteLine("EXC " + ten + ": " + ex.GetType().Name + " - " + ex.Message); }
        });
        t.SetApartmentState(ApartmentState.STA);
        t.IsBackground = true;
        t.Start();
        if (!t.Join(8000)) Console.WriteLine("!! TREO o buoc: " + ten);
        else Console.WriteLine("   ok: " + ten);
        Console.Out.Flush();
    }

    public static void Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        Buoc("frmMain", () =>
        {
            var f = new Lab05_MultiForm.Forms.frmMain();
            Console.WriteLine("   created " + f.ClientSize);
            f.Show();
            Console.WriteLine("   shown");
        });

        Buoc("frmSinhVien", () =>
        {
            var f = new Lab05_MultiForm.Forms.frmSinhVien();
            f.Show();
            Console.WriteLine("   shown " + f.ClientSize);
        });

        Buoc("frmChiTiet", () =>
        {
            var f = new Lab05_MultiForm.Forms.frmChiTietSinhVien(new Lab05_MultiForm.Models.SinhVien("SV10", "Nguyễn Văn An", new DateTime(2005, 3, 15), 8.5));
            f.Show();
            Console.WriteLine("   shown " + f.ClientSize);
        });

        Buoc("frmDanhSach", () =>
        {
            var f = new Lab05_MultiForm.Forms.frmDanhSach();
            f.Show();
            Console.WriteLine("   shown " + f.ClientSize);
        });

        Buoc("frmDangNhap", () =>
        {
            var f = new Lab05_MultiForm.Forms.frmDangNhap();
            f.Show();
            Console.WriteLine("   shown " + f.ClientSize);
        });

        Buoc("frmTimKiem", () =>
        {
            var f = new Lab05_MultiForm.Forms.frmTimKiem(Lab05_MultiForm.Models.DuLieuMau.DanhSachSinhVien());
            f.Show();
            Console.WriteLine("   shown " + f.ClientSize);
        });

        Buoc("frmGioiThieu", () =>
        {
            var f = new Lab05_MultiForm.Forms.frmGioiThieu();
            f.Show();
            Console.WriteLine("   shown " + f.ClientSize);
        });

        Console.WriteLine("DONE");
        Console.Out.Flush();
    }
}
