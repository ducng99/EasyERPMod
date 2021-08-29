using ERPLoader.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ERPLoader
{
    class Program
    {
        private static readonly List<ModModel> ModsList = new();

        static void Main(string[] args)
        {
            Logger.FileWrite("===========EasyERPMod START===========");

            bool isOnlyCleanup = false;

            foreach (string arg in args)
            {
                if (arg.Equals("/cleanOnly"))
                    isOnlyCleanup = true;
            }

            Settings.InitSettings();

            if (Settings.Instance.Verify())
            {
                PrintIntro();
                Cleanup();

                if (!isOnlyCleanup)
                {
                    LoadMods();
                    StartMods();

                    if (Settings.Instance.LaunchGame)
                    {
                        var gameProcess = StartGame();

                        if (gameProcess != null)
                        {
                            Logger.Log("Waiting for game exit...");
                            Logger.Warning("Do not close this window if you want me to cleanup after you finish playing!");
                            Logger.Log("It's fine if you want to close this window now :) Just run Cleanup.bat file if you want to restore files for multiplayer.");

                            gameProcess.WaitForExit();

                            Logger.Log("Game exited! Start restoring files...");
                            Cleanup();
                        }
                    }

                    Logger.Log("Done! Thanks for using EasyERPMod :D");
                    System.Threading.Thread.Sleep(3000);
                }
            }
            else
            {
                Logger.Warning("Found errors in your settings.json file. Please fix it and restart the app");
                Logger.NewLine();
                Logger.Log("Press any key to exit...");
                Console.ReadKey();
            }

            Logger.FileWrite("===========EasyERPMod EXIT===========");
            Logger.NewLine();
        }

        private static void PrintIntro()
        {
            var version = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString(3);
            var versionStrLen = $"v{version} ".Length;

            Console.Title = "EasyERPMod v" + version;

            Console.WriteLine("[ ]----------------------" + "-".Multiply(versionStrLen) + "[ ]");
            Console.WriteLine($"[ ] EasyERPMod by Maxhyt v{version} [ ]");
            Console.WriteLine("[ ]----------------------" + "-".Multiply(versionStrLen) + "[ ]");

            Console.WriteLine(@"
                              -_______----.._pBQ;
      ,^~|?|v^`          .!|JWRd9NRNND00R0dJ1lllt
rDDDW%9qyDMd8#@]    `~xtdDDDDD9qpDNGNNdRf6MduLn!!.=.=<rx]xv|v7>
 J007yvn|nYM8Q#Q^_:~vN00000ggdSdd6OgQgMZ9RDRDN0QQ##Q:.        :Qs|~,,,_     |    _
 `|88888gBBBBBQQ#@#BBBQQ8MM6%MMd0BBBBBQBBB############@@@@##QBQ#BQQQQ@@@@@#@@###QQBBKL.
   IBQQ8B98#QQQB#NB@@BBBB#gN0DBB9BQBB#####B#QB#@@#QQ@#QR#@@@@@@@@@@####@@@@@@BD#BQQQ#DN:
   O@@#@QQ#D####8@M@@###########B#8Q#QB#Q#Q#QBBQ#BBQ@#HyB###@@#####888B@@@@@@d@8###BD#MQVY*,==`
    ,::!gD#DQ#BQB#d@@@@@@@@@@@@@@@#@###@###@##@##@@#@@@@@#####BB#BdqPZMQ#@@@@RBBBBB8Q#RVqMdKQDNsDMQ~
        ,DQQQBB@QQ@@@@869RRRRD0gg88888QQQBBBBB######@##@@@@@@@@@@@@@@@8=`|hdg@BQQBBB#D}  ,Me0qd%QgB!
          :{fMR0MV??~`                                              ```      .*uIzVv:" + "\n");
        }

        private static void LoadMods()
        {
            Directory.CreateDirectory(Settings.Instance.ModsFolderName);

            string[] modsPaths = Directory.GetDirectories(Settings.Instance.ModsFolderName);

            if (modsPaths.Length == 0)
            {
                Logger.Log($"No mods found in \"{Settings.Instance.ModsFolderName}\"");
            }

            foreach (string modPath in modsPaths)
            {
                string modName = new DirectoryInfo(modPath).Name;

                if (!modName.EndsWith(Settings.Instance.DisabledModsEndsWith))
                {
                    ModsList.Add(new ModModel(modPath));
                }
            }
        }

        private static void StartMods()
        {
            ModsList.Sort((mod1, mod2) => mod1.Name.CompareTo(mod2.Name));
            ModsList.ForEach(mod => mod.Process());
        }

        private static Process StartGame()
        {
            Regex F1GameNameRegex = new(@"^f1_.+\.exe$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Regex F1GameTitleRegex = new(@"^F1 \d{4}", RegexOptions.Compiled);

            foreach (string file in Directory.GetFiles(Settings.Instance.F1GameDirectory))
            {
                if (F1GameNameRegex.IsMatch(Path.GetFileName(file)))
                {
                    Logger.Log($"Starting game process \"{Path.GetFileName(file)}\"");
                    Process.Start(file);

                    // Game process will exit and start a new one, try to find based on window's name
                    for (short i = 0; i < 60; i++)
                    {
                        System.Threading.Thread.Sleep(1000);

                        Logger.FileWrite($"Wait for game window try {i + 1}");

                        foreach (var process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension(file)))
                        {
                            if (!string.IsNullOrEmpty(process.MainWindowTitle))
                            {
                                Logger.FileWrite("Found window: " + process.MainWindowTitle);
                            }

                            if (F1GameTitleRegex.IsMatch(process.MainWindowTitle))
                            {
                                Logger.FileWrite("Found F1 game window!");
                                return process;
                            }
                        }
                    }

                    Logger.Warning("Cannot find game's window");
                    break;
                }
            }

            Logger.Warning("Failed to start game");

            return null;
        }

        private static void Cleanup()
        {
            Logger.Log("Start recovering original files...");

            var originalFiles = Directory.EnumerateFiles(Settings.Instance.F1GameDirectory, "*" + Settings.Instance.BackupFileExtension, SearchOption.AllDirectories);

            Parallel.ForEach(originalFiles, file =>
            {
                try
                {
                    string moddedFilePath = file.Substring(0, file.Length - Settings.Instance.BackupFileExtension.Length);

                    if (File.Exists(moddedFilePath))
                        File.Delete(moddedFilePath);

                    File.Move(file, moddedFilePath);
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed to recover file at \"{file}\"");
                    Logger.FileWrite(ex.ToString(), Logger.MessageType.Error);
                }
            });

            Logger.Log("Finished restoring original files");
        }
    }
}
