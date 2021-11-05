using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ERPLoader.Models
{
    public class ModModel
    {
        public string Name;
        public string ModPath;
        private readonly List<ErpFileModel> ErpFileModels = new();
        private readonly List<FindReplaceModel> FindReplaceList = new();
        private readonly List<string> FileReplaceList = new();

        private readonly Regex erpFolderRegex = new(@".+\.erp$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public ModModel(string path)
        {
            Name = new DirectoryInfo(path).Name;
            ModPath = path;

            Logger.Log($"Initializing mod \"{Name}\"");

            // ERP files mod
            var erpFileFolders = Directory.EnumerateDirectories(ModPath, "*.erp", SearchOption.AllDirectories);

            foreach (string erpFileFolder in erpFileFolders)
            {
                ErpFileModels.Add(new ErpFileModel(this, erpFileFolder));
            }

            // Find & Replace mod
            string findReplaceFile = Path.Combine(ModPath, Settings.Instance.FindReplaceFileName);
            if (File.Exists(findReplaceFile))
            {
                FindReplaceList.AddRange(FindReplaceModel.FromJson(File.ReadAllText(findReplaceFile)));

                var otherErpFiles = FindReplaceList.Where(fr => !ErpFileModels.Any(erp => erp.RelativePath.Equals(fr.ErpFilePath))).Select(fr => fr.ErpFilePath);

                foreach (var erpFilePath in otherErpFiles)
                {
                    ErpFileModels.Add(new ErpFileModel(this, erpFilePath, true));
                }
            }

            // File replace mod
            var filesInMod = Directory.EnumerateFiles(ModPath, "*", SearchOption.AllDirectories);
            foreach (var file in filesInMod)
            {
                string relativePath = Path.GetRelativePath(ModPath, file);
                if (!Utils.ContainParentFolder(relativePath, erpFolderRegex) && !Path.GetFileName(file).Equals(Settings.Instance.FindReplaceFileName))
                {
                    FileReplaceList.Add(relativePath);
                }
            }
        }

        public void Process()
        {
            FileReplaceList.ForEach(relativePath =>
            {
                string originalFile = Path.Combine(Settings.Instance.F1GameDirectory, relativePath);
                string moddedFile = Path.Combine(ModPath, relativePath);
                if (Utils.BackupOriginalFile(originalFile))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(originalFile));
                    File.Copy(moddedFile, originalFile, true);
                }
                else
                {
                    Logger.Warning($"[{Name}] Failed backing up file. This file will not be replaced/moded");
                }
            });

            ErpFileModels.ForEach(erpFileModel =>
            {
                erpFileModel.UnpackAndImport();

                var findReplaceModel = FindReplaceList.Find(m => m.ErpFilePath.Equals(erpFileModel.RelativePath));

                if (findReplaceModel != null)
                {
                    erpFileModel.FindAndReplace(findReplaceModel.Tasks);
                }

                erpFileModel.Repack();
            });

            Logger.Log($"[{Name}] Mod has been applied!");
            Logger.NewLine();
        }
    }
}
