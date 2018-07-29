using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace OneDumper
{
    static class Program
    {
        /// <summary>
        /// Uygulamanın ana girdi noktası.
        /// </summary>
        /// 

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
        }
        public static string tmpDir = Path.GetTempPath().Replace("\\", "/") + "OL_" + RandomString(25);
        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        static void OnProcessExit(object sender, EventArgs e)
        {
            if (Directory.Exists(tmpDir)) Directory.Delete(tmpDir, true);
        }


}
}
