using System.IO;
using System.Text.Json;

namespace ERPLoader
{
    public class Settings
    {
        public string F1GameDirectory { get; set; }
        public string ModsFolderName { get; set; }
        public string BackupFileExtension { get; set; }
        public string DisabledModsEndsWith { get; set; }
        public string FindReplaceFileName { get; set; }

        public bool LaunchGame { get; set; }

        public Settings()
        {
            F1GameDirectory = Directory.GetCurrentDirectory();
            ModsFolderName = "_MODS";
            BackupFileExtension = ".original";
            DisabledModsEndsWith = "_DISABLED";
            FindReplaceFileName = "FindReplace.json";
            LaunchGame = true;
        }

        public static Settings InitSettings()
        {
            string settingsFile = "settings.json";
            Settings EasyModSettings;

            if (File.Exists(settingsFile))
            {
                EasyModSettings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(settingsFile));
            }
            else
            {
                EasyModSettings = new();
                Logger.Warning($"A new {settingsFile} file has been created, please update your F1 game path in the file.");
            }

            File.WriteAllText(settingsFile, JsonSerializer.Serialize(EasyModSettings, new JsonSerializerOptions { WriteIndented = true }));

            return EasyModSettings;
        }
    }
}
