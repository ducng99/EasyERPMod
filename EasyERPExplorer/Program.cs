using EasyERPExplorer.Renderer;
using EasyERPExplorer.Windows;
using ERPLoader;
using System;

namespace EasyERPExplorer
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            Logger.FileWrite("===========EasyERPExplorer START===========");

            Settings.InitSettings();

            if (Settings.Instance.Verify(false))
            {
                // TODO: Remove this quick and dirty ****
                try
                {
                    GameFolderExplorer gameFolderExplorer = new();
                    ModsExplorer modsExplorer = new();

                    Window.DrawWindows.Add(gameFolderExplorer);
                    Window.DrawWindows.Add(modsExplorer);

                    Window wnd = new();
                    wnd.Run();
                }
                catch (Exception ex)
                {
                    Logger.Error("An error has occured! Please report with log files in \"Logs\" folder.");
                    Logger.FileWrite(ex.ToString(), Logger.MessageType.Error);
                }
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
