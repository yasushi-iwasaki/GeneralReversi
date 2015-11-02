using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using GeneralReversi;

namespace GeneralReversiGui
{
    static class Program
    {
        [DllImport("Kernel32.dll")]
        static extern bool AllocConsole();

        [DllImport("Kernel32.dll")]
        static extern bool FreeConsole();

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string arg = args.Length > 0 ? args[0] : null;

            if (Control.ModifierKeys == Keys.Shift || arg == "ffo")
            {
                if (Environment.OSVersion.ToString().Contains("Windows"))
                {
                    AllocConsole();

                    Exe.Main(null);

                    FreeConsole();
                }
                else
                {
                    Exe.Main(null);
                }
            }
            else if (Control.ModifierKeys == Keys.Control || arg == "log")
            {
                Application.Run(new Form1(true));
            }
            else
            {
                Application.Run(new Form1(false));
            }
        }
    }
}
