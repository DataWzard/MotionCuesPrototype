using System;
using System.Windows.Forms;
namespace MotionCuesPrototype
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MotionCuesContext());
        }
    }
}
