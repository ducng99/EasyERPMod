using System;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;

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

            void CreateNewSettings()
            {
                EasyModSettings = new();
                Logger.Warning($"A new {SettingsFile} file has been created, please update your F1 game path in the file!");
            }

            if (File.Exists(SettingsFile))
            {
                try
                {
                    EasyModSettings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(SettingsFile));
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed parsing {SettingsFile}. Please fix the file or remove the file.");
                    Logger.FileWrite(ex.ToString(), Logger.MessageType.Error);

                    CreateNewSettings();
                }
            }
            else
            {
                CreateNewSettings();
            }

            // Write new settings if exist
            EasyModSettings.SaveSettings();
        }

        public void SaveSettings()
        {
            File.WriteAllText(SettingsFile, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
        }

        public bool Verify(bool checkGameExeExists = true)
        {
            if (string.IsNullOrWhiteSpace(F1GameDirectory)
                || string.IsNullOrWhiteSpace(ModsFolderName)
                || string.IsNullOrWhiteSpace(BackupFileExtension)
                || string.IsNullOrWhiteSpace(DisabledModsEndsWith)
                || string.IsNullOrWhiteSpace(FindReplaceFileName))
            {
                Logger.Error("Found empty value(s) in settings.json file. Please fix them or delete the file");
                return false;
            }

            if (checkGameExeExists)
            {
                Regex F1GameNameRegex = new(@"^f1_.+\.exe$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                bool foundF1Exe = false;

                if (Directory.Exists(F1GameDirectory))
                {
                    foreach (var file in Directory.EnumerateFiles(F1GameDirectory))
                    {
                        if (F1GameNameRegex.IsMatch(file))
                        {
                            foundF1Exe = true;
                            break;
                        }
                    }
                }

                if (!foundF1Exe)
                {
                    Logger.Error("F1 game path is incorrect! Please check your settings.json file and make sure the path is correctly pointing to F1 game folder");
                }

                return foundF1Exe;
            }

            return true;
        }
    }
}
