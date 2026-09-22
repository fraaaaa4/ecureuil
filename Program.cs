using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Ecureuil
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            try
            {
                Application.Run(new mainWindow());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fatal Error: " + ex.ToString(), "Ecureuil Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show("Thread Error: " + (e.Exception != null ? e.Exception.ToString() : "Unknown"), "Ecureuil Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Domain Error: " + (e.ExceptionObject != null ? e.ExceptionObject.ToString() : "Unknown"), "Ecureuil Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}