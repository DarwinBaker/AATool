using System;
using System.Windows.Forms;

namespace AATool
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (var main = new Main())
                main.Run(); 
        }
    }
}
