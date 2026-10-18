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
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = (System.Net.SecurityProtocolType)3072 | (System.Net.SecurityProtocolType)768 | System.Net.SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Ssl3;
            }
            catch { }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new mainWindow());
        }
    }
}