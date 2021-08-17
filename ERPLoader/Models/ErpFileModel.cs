using EgoEngineLibrary.Archive.Erp;
using System;
using System.IO;

namespace ERPLoader.Models
{
    class ErpFileModel
    {
        private readonly ModModel ModModelParent;
        private readonly string ErpModPath;
        private readonly string RelativePath;
        private readonly string ErpFilePath;

        private bool Initialized = false;

        public ErpFileModel(ModModel parent, string path)
        {
            ModModelParent = parent;
            ErpModPath = path;
            RelativePath = Path.GetRelativePath(Path.Combine(Program.ModsFolderPath, parent.Name), path);
            ErpFilePath = Path.Combine(Program.EasyModSettings.F1GameDirectory, RelativePath).Trim('\\', '/');

            if (File.Exists(ErpFilePath))
            {
                Initialized = true;
            }
            else
            {
                Logger.Warning($"[{ModModelParent.Name}] ERP file not found: {ErpFilePath}\nMake sure the path in your mod folder is correct!");
            }
        }

        public void Process()
        {
            if (Initialized && BackupOriginalFile())
            {
                Logger.Log($"[{ModModelParent.Name}] Patching {Path.GetFileName(ErpFilePath)}");

                try
                {
                    var erpFile = new ErpFile();
                    using (var erpFileStream = File.Open(ErpFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        erpFile.Read(erpFileStream);

                    var resourcesModel = new ResourcesModel(erpFile);

                    string packagesFolderPath = Path.Combine(ErpModPath, "packages");
                    string texturesFolderPath = Path.Combine(ErpModPath, "textures");
                    string xmlFilesFolderPath = Path.Combine(ErpModPath, "xmls");

                    if (Directory.Exists(packagesFolderPath))
                    {
                        var packagesModel = new PkgsModel(resourcesModel);
                        packagesModel.Import(ErpModPath);
                    }

                    if (Directory.Exists(texturesFolderPath))
                    {
                        var texturesModel = new TexturesModel(resourcesModel);
                        texturesModel.Import(ErpModPath);
                    }

                    if (Directory.Exists(xmlFilesFolderPath))
                    {
                        var xmlsModel = new XmlsModel(resourcesModel);
                        xmlsModel.Import(ref resourcesModel, ErpModPath);
                    }

                    using (var writer = File.Open(ErpFilePath, FileMode.Create, FileAccess.Write, FileShare.Read))
                        erpFile.Write(writer);

                    Logger.Log($"[{ModModelParent.Name}] Patched {Path.GetFileName(ErpFilePath)}");
                }
                catch
                {
                    Logger.Error($"[{ModModelParent.Name}] Failed patching {Path.GetFileName(ErpFilePath)}");
                }
            }
        }

        private bool BackupOriginalFile()
        {
            // If original file already exists, ignore
            if (!File.Exists(ErpFilePath + Program.EasyModSettings.BackupFileExtension))
            {
                try
                {
                    File.Copy(ErpFilePath, ErpFilePath + Program.EasyModSettings.BackupFileExtension);
                    return true;
                }
                catch
                {
                    Logger.Error($"[{ModModelParent.Name}] Failed backing up file at {ErpFilePath}\nThis file will NOT be modded for your safety");
                    return false;
                }
            }

            return true;
        }
    }
}
