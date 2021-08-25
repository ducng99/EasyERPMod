using System.IO;
using System.Text.Json;

namespace ERPLoader
{
    public class Settings
    {
        private static readonly string SettingsFile = "settings.json";
        public static Settings Instance { get; private set; }

        public string F1GameDirectory { get; set; }
        public string ModsFolderName { get; set; }
        public string BackupFileExtension { get; set; }
        public string DisabledModsEndsWith { get; set; }
        public string FindReplaceFileName { get; set; }

        public bool LaunchGame { get; set; }

        public Settings()
        {
            Instance = this;

            F1GameDirectory = Directory.GetCurrentDirectory();
            ModsFolderName = "_MODS";
            BackupFileExtension = ".original";
            DisabledModsEndsWith = "_DISABLED";
            FindReplaceFileName = "FindReplace.json";
            LaunchGame = true;
        }

        public static void InitSettings()
        {
            Settings EasyModSettings;

            if (File.Exists(SettingsFile))
            {
                EasyModSettings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(SettingsFile));
            }
            else
            {
                EasyModSettings = new();
                Logger.Warning($"A new {SettingsFile} file has been created, please update your F1 game path in the file.");
            }

            // Write new settings if exist
            EasyModSettings.SaveSettings();
        }

        public void SaveSettings()
        {
            File.WriteAllText(SettingsFile, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
        }
    }
}
