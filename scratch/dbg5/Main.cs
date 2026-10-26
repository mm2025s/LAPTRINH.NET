// Scratch: chạy chương trình Lab 5 với output ghi ra file để chẩn đoán.
using System;
using System.IO;
using System.Windows.Forms;

static class Debug
{
    static void Main(string[] args)
    {
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (s, e) => Console.WriteLine("FATAL UI: " + e.Exception);
        AppDomain.CurrentDomain.UnhandledException += (s, e) => Console.WriteLine("FATAL: " + e.ExceptionObject);
        try
        {
            var f = new Lab05_MultiForm.Forms.frmMain();
            Console.WriteLine("frmMain created: " + f.Width + "x" + f.Height);
            f.Show();
            Console.WriteLine("shown");
        }
        catch (Exception ex)
        {
            Console.WriteLine("EXC: " + ex);
        }
    }
}
