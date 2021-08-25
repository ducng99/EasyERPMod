using EasyERPExplorer.Renderer;
using EasyERPExplorer.Windows;
using ERPLoader;

namespace EasyERPExplorer
{
    class Program
    {
        [System.STAThread]
        static void Main()
        {
            Settings.InitSettings();

            GameFolderExplorer gameFolderExplorer = new();
            ModsExplorer modsExplorer = new();

            Window.DrawWindows.Add(gameFolderExplorer);
            Window.DrawWindows.Add(modsExplorer);

            Window wnd = new Window();
            wnd.Run();
        }
    }
}
