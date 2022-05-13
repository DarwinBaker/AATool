using System;
using System.Threading;
using System.Windows.Forms;

namespace AATool
{
    public static class Program
    {
        private static void GlobalUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e) =>
            Debug.SaveReport(e.ExceptionObject as Exception);

        private static void GlobalThreadExceptionHandler(object sender, ThreadExceptionEventArgs e) =>
            Debug.SaveReport(e.Exception);

        [STAThread]
        static void Main()
        {
            //add crash reporting events
            AppDomain.CurrentDomain.UnhandledException += GlobalUnhandledExceptionHandler;
            Application.ThreadException += GlobalThreadExceptionHandler;

            //start application
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (var main = new Main())
                main.Run();
        }
    }
}
