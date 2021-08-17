using System.IO;

namespace ERPLoader
{
    public class Settings
    {
        public string F1GameDirectory { get; set; }
        public string ModsFolderName { get; set; }

        public Settings()
        {
            F1GameDirectory = Directory.GetCurrentDirectory();
            ModsFolderName = "_MODS";
        }
    }
}
