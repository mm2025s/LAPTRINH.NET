// Scratch: chạy từng bước để tìm bước bị treo.
using System;
using System.Threading;
using System.Windows.Forms;

static class Debug
{
    static void Buoc(string ten, Action a)
    {
        Console.WriteLine(">> " + ten + " ...");
        bool xong = false;
        var t = new Thread(() =>
        {
            try { a(); }
            catch (Exception ex) { Console.WriteLine("EXC " + ten + ": " + ex); }
            xong = true;
        });
        t.SetApartmentState(ApartmentState.STA);
        t.IsBackground = true;
        t.Start();
        if (!t.Join(8000)) Console.WriteLine("!! TREO o buoc: " + ten);
        else Console.WriteLine("   ok: " + ten);
    }

    static void Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        Buoc("Tao frmMain", () =>
        {
            var f = new Lab05_MultiForm.Forms.frmMain();
            Console.WriteLine("   created ok");
            f.Show();
            Console.WriteLine("   shown ok " + f.ClientSize);
            f.Close();
            Console.WriteLine("   closed ok");
        });

        Buoc("Tao frmSinhVien", () =>
        {
            var f = new Lab05_MultiForm.Forms.frmSinhVien();
            f.Show();
            Console.WriteLine("   sinhvien shown " + f.ClientSize);
            f.Close();
            Console.WriteLine("   sinhvien closed");
        });

        Buoc("Tao frmDanhSach", () =>
        {
            var f = new Lab05_MultiForm.Forms.frmDanhSach();
            f.Show();
            Console.WriteLine("   danhsach shown");
            f.Close();
            Console.WriteLine("   danhsach closed");
        });

        Console.WriteLine("DONE");
    }
}
