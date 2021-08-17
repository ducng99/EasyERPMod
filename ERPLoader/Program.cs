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
        public static Settings EasyModSettings { get; private set; } = new();
        public static string ModsFolderPath { get; private set; }

        private static readonly List<ModModel> ModsList = new();

        static void Main(string[] args)
        {
            Cleanup();
            PrintIntro();
            InitSettings();
            LoadMods();
            StartMods();

            var gameProcess = StartGame();

            if (gameProcess != null)
            {
                Logger.Log("Waiting for game exit...");
                Logger.Warning("Do not close this window if you want me to cleanup after you finish playing!");

                gameProcess.WaitForExit();

                Logger.Log("Game exited! Start restoring files...\n");
                Cleanup();

                Logger.Log("Done! Thanks for using EasyERPMod :D");
                System.Threading.Thread.Sleep(3000);
            }
        }

        private static void PrintIntro()
        {
            Console.WriteLine("[ ]----------------------[ ]");
            Console.WriteLine("[ ] EasyERPMod by Maxhyt [ ]");
            Console.WriteLine("[ ]----------------------[ ]");
            Console.WriteLine(@"
                              -_______----.._pBQ;                                                   
      ,^~|?|v^`          .!|JWRd9NRNND00R0dJ1lllt                                                   
rDDDW%9qyDMd8#@]    `~xtdDDDDD9qpDNGNNdRf6MduLn!!. .=<rx]xv|v7>            ``                       
 J007yvn|nYM8Q#Q^_:~vN00000ggdSdd6OgQgMZ9RDRDN0QQ##Qg0#l~*N:,:nQs|~,,,_-.` V|  `-_.                 
 `|88888gBBBBBQQ#@#BBBQQ8MM6%MMd0BBBBBQBBB############@@@@##QBQ#BQQQQ@@@@@#@@###QQBBKL.             
   IBQQ8B98#QQQB#NB@@BBBB#gN0DBB9BQBB#####B#QB#@@#QQ@#QR#@@@@@@@@@@####@@@@@@BD#BQQQ#DN:            
   O@@#@QQ#D####8@M@@###########B#8Q#QB#Q#Q#QBBQ#BBQ@#HyB###@@#####888B@@@@@@d@8###BD#MQVY*,==`     
    ,::!gD#DQ#BQB#d@@@@@@@@@@@@@@@#@###@###@##@##@@#@@@@@#####BB#BdqPZMQ#@@@@RBBBBB8Q#RVqMdKQDNsDMQ~
        ,DQQQBB@QQ@@@@869RRRRD0gg88888QQQBBBBB######@##@@@@@@@@@@@@@@@8=`|hdg@BQQBBB#D}  ,Me0qd%QgB!
          :}fMR0MV??~`                                              ```      .*uIzVv:               " + 
             "\n");
        }

        private static void InitSettings()
        {
            string settingsFile = "settings.json";

            if (File.Exists(settingsFile))
            {
                EasyModSettings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(settingsFile));
            }

            File.WriteAllText(settingsFile, JsonSerializer.Serialize(EasyModSettings, new JsonSerializerOptions() { WriteIndented = true }));
        }

        private static void LoadMods()
        {
            ModsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), EasyModSettings.ModsFolderName);
            Directory.CreateDirectory(ModsFolderPath);

            string[] modsPaths = Directory.GetDirectories(ModsFolderPath);

            if (modsPaths.Length == 0)
            {
                Logger.Log($"No mods found in \"{ModsFolderPath}\"");
            }

            foreach (string modPath in modsPaths)
            {
                string modName = new DirectoryInfo(modPath).Name;

                if (!modName.EndsWith("_DISABLED"))
                {
                    ModsList.Add(new ModModel(modPath));
                }
            }
        }

        private static void StartMods()
        {
            foreach (var mod in ModsList)
            {
                mod.Process();
            }
        }

        private static Process StartGame()
        {
            Regex F1GameNameRegex = new(@"^f1_.+\.exe$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Regex F1GameTitleRegex = new(@"^F1 \d{4}", RegexOptions.Compiled);

            foreach (string file in Directory.GetFiles(EasyModSettings.F1GameDirectory))
            {
                if (F1GameNameRegex.IsMatch(Path.GetFileName(file)))
                {
                    Logger.Log($"Starting game process \"{Path.GetFileName(file)}\"");
                    Process.Start(file);

                    // Game process will exit and start a new one, try to find based on window name
                    for (short i = 0; i < 10; i++)
                    {
                        System.Threading.Thread.Sleep(5000);

                        // For some reason, GetProcessesByName doesn't work
                        foreach (var process in Process.GetProcesses())
                        {
                            if (F1GameTitleRegex.IsMatch(process.MainWindowTitle))
                            {
                                return process;
                            }
                        }
                    }

                    Logger.Log("Cannot find game's window");
                }
            }

            Logger.Warning("Failed to start game");

            return null;
        }

        private static void Cleanup()
        {
            var originalFiles = Directory.GetFiles(EasyModSettings.F1GameDirectory, "*" + EasyModSettings.BackupFileExtension, SearchOption.AllDirectories);

            Parallel.ForEach(originalFiles, file =>
            {
                try
                {
                    string moddedFilePath = file.Substring(0, file.Length - EasyModSettings.BackupFileExtension.Length);

                    if (File.Exists(moddedFilePath))
                        File.Delete(moddedFilePath);

                    File.Move(file, moddedFilePath);
                }
                catch
                {
                    Logger.Error($"Failed to recover file at \"{file}\"");
                }
            });
        }
    }
}
