using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ERPLoader.Models
{
    class ModModel
    {
        public string Name;
        private readonly List<ErpFileModel> ErpFileModels = new();
        private readonly List<FindReplaceModel> FindReplaceList = new();

        public ModModel(string path)
        {
            Name = new DirectoryInfo(path).Name;

            Logger.Log($"Enabling mod \"{Name}\"");

            var erpFileFolders = Directory.EnumerateDirectories(path, "*.erp", SearchOption.AllDirectories);

            foreach (string erpFileFolder in erpFileFolders)
            {
                ErpFileModels.Add(new ErpFileModel(this, erpFileFolder));
            }

            string findReplaceFile = Path.Combine(path, "FindReplace.json");
            if (File.Exists(findReplaceFile))
            {
                FindReplaceList.AddRange(FindReplaceModel.FromJson(File.ReadAllText(findReplaceFile)));

                var otherErpFiles = FindReplaceList.Where(fr => !ErpFileModels.Any(erp => erp.RelativePath.Equals(fr.ErpFilePath))).Select(fr => fr.ErpFilePath);

                foreach (var erpFilePath in otherErpFiles)
                {
                    // erpFilePath will be called in GetRelativePath with mod folder, however we have none.
                    // The method return original full path and might break things.
                    // TODO: Add a model type(?) to ErpFileModel
                    ErpFileModels.Add(new ErpFileModel(this, erpFilePath, true));
                }
            }
        }

        public void Process()
        {
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

            Logger.Log($"[{Name}] Patching ERP files completed!");
        }
    }
}
