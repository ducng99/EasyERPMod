using System.IO;

namespace ERPLoader
{
    class Settings
    {
        public string F1GameDirectory { get; set; }
        public string ModsFolderName { get; set; }
        public string BackupFileExtension { get; set; }
        public string DisabledModsEndsWith { get; set; }

        public bool LaunchGame { get; set; }

        public Settings()
        {
            F1GameDirectory = Directory.GetCurrentDirectory();
            ModsFolderName = "_MODS";
            BackupFileExtension = ".original";
            DisabledModsEndsWith = "_DISABLED";
            LaunchGame = true;
        }
    }
}
