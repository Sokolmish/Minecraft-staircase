using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Globalization;


namespace Minecraft_staircase
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Properties.Settings.Default.Language);
            Application.Run(new FormMain());
        }
    }

    static public class Lang
    {
        public static System.Resources.ResourceManager HintsResource
        {
            get
            {
                switch (Properties.Settings.Default.Language)
                {
                    case "ru-RU":
                        return ResourceHintsRu.ResourceManager;
                    default:
                        return ResourceHintsEn.ResourceManager;
                }
            }
        }

        public static string GetHint(string name)
        {
            return HintsResource.GetString(name);
        }
    }
}
