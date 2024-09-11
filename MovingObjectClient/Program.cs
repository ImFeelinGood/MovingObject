using System;
using System.Windows.Forms;

namespace MovingObjectClient
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
            // Menjalankan form utama, yaitu ClientForm
            Application.Run(new ClientForm());
        }
    }
}
