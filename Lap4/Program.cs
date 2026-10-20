using System;
using System.Windows.Forms;

namespace Lab04_WinFormsBasic
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Forms.frmMain());
        }
    }
}
