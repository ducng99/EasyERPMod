using EasyERPExplorer.Renderer;
using EasyERPExplorer.Windows;
using ERPLoader;

namespace EasyERPExplorer
{
    class Program
    {
        public static Settings EasyERPSettings = Settings.InitSettings();

        [System.STAThread]
        static void Main()
        {
            GameFolderExplorer gameFolderExplorer = new();
            ModsExplorer modsExplorer = new();

            Window.DrawWindows.Add(gameFolderExplorer);
            Window.DrawWindows.Add(modsExplorer);

            Window wnd = new Window();
            wnd.Run();
        }
    }
}
