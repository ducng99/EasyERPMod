using ERPLoader.Models;
using System.Collections.Generic;
using System.IO;

namespace ERPLoader
{
    class Program
    {
        public static readonly string F1GameDirectory = Directory.GetCurrentDirectory();
        public static readonly string ModsFolderPath = Path.Combine(F1GameDirectory, "_MODS");

        private static readonly List<ModModel> ModsList = new();

        static void Main(string[] args)
        {
            Directory.CreateDirectory(ModsFolderPath);

            string[] modsPaths = Directory.GetDirectories(ModsFolderPath);

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
    }
}
