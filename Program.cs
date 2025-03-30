using System;
using System.Windows.Forms;

namespace FourDRenderer
{
    static class Program
    {
        /// <summary>
        /// main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}