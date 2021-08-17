using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ERPLoader.Models
{
    class ModModel
    {
        public string Name;
        private readonly List<ErpFileModel> ErpFileModels = new();

        public ModModel(string path)
        {
            Name = new DirectoryInfo(path).Name;

            Logger.Log($"Enabling mod \"{Name}\"");

            string[] erpFileFolders = Directory.GetDirectories(path, "*.erp", SearchOption.AllDirectories);

            foreach (string erpFileFolder in erpFileFolders)
            {
                ErpFileModels.Add(new ErpFileModel(this, erpFileFolder));
            }
        }

        public void Process()
        {
            Parallel.ForEach(ErpFileModels, erpFileModel => erpFileModel.Process());

            Logger.Log($"[{Name}] Patching ERP files completed!");
        }
    }
}
