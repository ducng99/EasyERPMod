using EasyERPExplorer.Renderer;
using EasyERPExplorer.Windows;
using ERPLoader;
using System;

namespace EasyERPExplorer
{
    class Program
    {
        [System.STAThread]
        static void Main()
        {
            Logger.FileWrite("===========EasyERPExplorer START===========");

            Settings.InitSettings();

            if (Settings.Instance.Verify())
            {
                GameFolderExplorer gameFolderExplorer = new();
                ModsExplorer modsExplorer = new();

                Window.DrawWindows.Add(gameFolderExplorer);
                Window.DrawWindows.Add(modsExplorer);

                Window wnd = new Window();
                wnd.Run();
            }
            else
            {
                Logger.Error("Cannot verify settings.json file. Please make sure the file contain valid info.");
                Logger.NewLine();
                Logger.Log("Press any to exit...");
                Console.ReadKey();
            }

            Logger.FileWrite("===========EasyERPExplorer EXIT===========");
        }
    }
}
