using ERPLoader.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ERPLoader
{
    class Program
    {
        public static Settings EasyModSettings { get; private set; } = new();
        public static string ModsFolderPath { get; private set; }

        private static readonly List<ModModel> ModsList = new();

        static void Main(string[] args)
        {
            PrintIntro();

            InitSettings();

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

            foreach (var mod in ModsList)
            {
                mod.Process();
            }
        }

        private static void PrintIntro()
        {
            Console.WriteLine("//====== EasyERPMod by Maxhyt ======\\\\");
            Console.WriteLine(@"
          '9oLxIk99hoxLo9'
          :WWWWd%WW%bWWWW,
                1WWr
         QWWq`zXMWWMXz`OWWm
         QWWS.&WWWWWW&`OWWm
         ~<<!  ~WWWW~  !<<~
              .QWWWWQ.
             1MWWWWWW&1
          .wNWWWWWWWWWWNw`
          ,WWWWWWWWWWWWWW,
          ,WWWWWWWWWWWWWW,
          `NWWWWWWWWWWWWN`
           +WWWWWWWWWWWW+
            *WWWWWWWWWW*
          ,Wl;mWWWWWWm;nW,
          `+LI:9WWWWU,IL+`
         HWWU'**WWWW+1'9WWK
         mWWd .\++++<. SWWQ
         !rr;.WWWWWWWW';rr!
             `99999999`" + "\n");
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
    }
}
