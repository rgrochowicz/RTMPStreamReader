using System;
using System.Windows.Forms;

namespace RTMPStreamReader
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                MessageBox.Show(@"Specify file");
                return;
            }
            string fname = args[0];

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(fname));
        }
    }
}