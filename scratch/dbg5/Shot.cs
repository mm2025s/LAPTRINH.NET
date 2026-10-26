// Scratch: chụp thử 1 form ra PNG để kiểm tra giao diện.
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;

public static class Harness
{
    public static void Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var t = new Thread(() =>
        {
            try
            {
                string ten = args[0];
                Form f;
                if (ten == "main") f = new Lab05_MultiForm.Forms.frmMain();
                else if (ten == "sv") f = new Lab05_MultiForm.Forms.frmSinhVien();
                else f = new Lab05_MultiForm.Forms.frmDangNhap();

                f.Show();
                f.WmShowWindowDone();
            }
            catch (Exception ex) { Console.WriteLine("EXC: " + ex); }
        });
        t.SetApartmentState(ApartmentState.STA);
        t.Start();
        t.Join(5000);
        Console.WriteLine("xong");
    }
}

static class Ext
{
    public static void WmShowWindowDone(this Form f) { }
}
