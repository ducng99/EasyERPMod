using EgoEngineLibrary.Archive.Erp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ERPLoader.Models
{
    public class ErpFileModel
    {
        private ErpFile erpFile;
        private ResourcesModel resourcesModel;
        private PkgsModel pkgsModel;
        private TexturesModel texturesModel;
        private XmlsModel xmlsModel;

        private readonly ModModel ModModelParent;
        private readonly string ErpModPath;
        public readonly string RelativePath;    // ModModel needs this for Find&Replace
        private readonly string ErpFilePath;

        private string PackagesFolderPath => Path.Combine(ErpModPath, "packages");
        private string TexturesFolderPath => Path.Combine(ErpModPath, "textures");
        private string XmlFilesFolderPath => Path.Combine(ErpModPath, "xmls");

        private readonly bool Initialized;

        public ErpFileModel(ModModel parent, string path, bool findReplaceOnly = false)
        {
            ModModelParent = parent;
            ErpModPath = path;
            RelativePath = findReplaceOnly ? path : Path.GetRelativePath(Path.Combine(Settings.Instance.ModsFolderName, ModModelParent.Name), ErpModPath).Trim('\\', '/');
            ErpFilePath = Path.Combine(Settings.Instance.F1GameDirectory, RelativePath);

            if (File.Exists(ErpFilePath))
            {
                Initialized = true;
            }
            else
            {
                Initialized = false;
                Logger.Warning($"[{ModModelParent.Name}] ERP file not found: {ErpFilePath}\nMake sure the path in your mod folder is correct!");
            }
        }

        public void UnpackAndImport()
        {
            if (Initialized && BackupOriginalFile())
            {
                Logger.Log($"[{ModModelParent.Name}] Patching {Path.GetFileName(ErpFilePath)}");

                try
                {
                    erpFile = new ErpFile();
                    using (var erpFileStream = File.Open(ErpFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        erpFile.Read(erpFileStream);

                    resourcesModel = new ResourcesModel(erpFile);
                    pkgsModel = new PkgsModel(resourcesModel);
                    texturesModel = new TexturesModel(resourcesModel);
                    xmlsModel = new XmlsModel(resourcesModel);

                    if (Directory.Exists(PackagesFolderPath))
                    {
                        pkgsModel.Import(PackagesFolderPath);
                    }

                    if (Directory.Exists(TexturesFolderPath))
                    {
                        texturesModel.Import(TexturesFolderPath);
                    }

                    if (Directory.Exists(XmlFilesFolderPath))
                    {
                        xmlsModel.Import(XmlFilesFolderPath);
                    }

                    Logger.Log($"[{ModModelParent.Name}] Patched {Path.GetFileName(ErpFilePath)}");
                }
                catch (Exception ex)
                {
                    Logger.Error($"[{ModModelParent.Name}] Failed patching {Path.GetFileName(ErpFilePath)}");
                    Logger.FileWrite(ex.ToString(), Logger.MessageType.Error);
                }
            }
        }

        public void Export()
        {
            if (Initialized)
            {
                Logger.Log($"[{ModModelParent.Name}] Extracting {Path.GetFileName(ErpFilePath)}");

                try
                {
                    erpFile = new ErpFile();
                    using (var erpFileStream = File.Open(ErpFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        erpFile.Read(erpFileStream);

                    resourcesModel = new ResourcesModel(erpFile);
                    pkgsModel = new PkgsModel(resourcesModel);
                    texturesModel = new TexturesModel(resourcesModel);
                    xmlsModel = new XmlsModel(resourcesModel);

                    pkgsModel.Export(PackagesFolderPath);
                    texturesModel.Export(TexturesFolderPath);
                    xmlsModel.Export(XmlFilesFolderPath);
                }
                catch (Exception ex)
                {
                    Logger.Error($"[{ModModelParent.Name}] Failed extracting {Path.GetFileName(ErpFilePath)}");
                    Logger.FileWrite(ex.ToString(), Logger.MessageType.Error);
                }
            }
        }

        public void FindAndReplace(IList<FindReplaceModel.FileTask> fileTasks)
        {
            if (xmlsModel != null && pkgsModel != null)
            {
                foreach (var fileTask in fileTasks)
                {
                    bool isXMLFile = true;
                    string fileContent = xmlsModel.ReadFile(fileTask.FileName);

                    if (string.IsNullOrEmpty(fileContent))
                    {
                        isXMLFile = false;
                        fileContent = pkgsModel.ReadFile(fileTask.FileName);
                    }

                    if (!string.IsNullOrEmpty(fileContent))
                    {
                        Logger.Log($"[{ModModelParent.Name}] [Find&Replace] Processing \"{fileTask.FileName}\"...");

                        foreach (var searchTask in fileTask.Tasks)
                        {
                            string replacedContent = "";

                            switch (searchTask.SearchType)
                            {
                                case FindReplaceModel.SearchTypeEnum.Exact:
                                    replacedContent = fileContent.Replace(searchTask.SearchFor, searchTask.ReplaceWith);
                                    break;
                                case FindReplaceModel.SearchTypeEnum.Regex:
                                    var regex = new Regex(searchTask.SearchFor);
                                    replacedContent = regex.Replace(fileContent, searchTask.ReplaceWith);
                                    break;
                                default:
                                    break;
                            }

                            if (!string.IsNullOrEmpty(replacedContent))
                            {
                                if (replacedContent.Equals(fileContent))
                                {
                                    Logger.Warning($"[{ModModelParent.Name}] [Find&Replace] Cannot find {searchTask.SearchType}\"{searchTask.SearchFor}\" in file \"{fileTask.FileName}\"");
                                }
                                else
                                {
                                    if (isXMLFile)
                                    {
                                        xmlsModel.WriteFile(fileTask.FileName, replacedContent);
                                    }
                                    else
                                    {
                                        pkgsModel.WriteFile(fileTask.FileName, replacedContent);
                                    }
                                }

                                Logger.Log($"[{ModModelParent.Name}] [Find&Replace] Patched \"{fileTask.FileName}\"");
                            }
                            else
                            {
                                Logger.Warning($"[{ModModelParent.Name}] [Find&Replace] Cannot find \"{searchTask.SearchFor}\" in file \"{fileTask.FileName}\"");
                            }
                        }
                    }
                    else
                    {
                        Logger.Warning($"[{ModModelParent.Name}] [Find&Replace] Cannot find file with name \"{fileTask.FileName}\" or file content cannot be read");
                    }
                }
            }
        }

        public void Repack()
        {
            if (erpFile != null)
            {
                using (var writer = File.Open(ErpFilePath, FileMode.Create, FileAccess.Write, FileShare.Read))
                    erpFile.Write(writer);

                Logger.Log($"[{ModModelParent.Name}] Packed {Path.GetFileName(ErpFilePath)}");
            }
            else
            {
                Logger.Error($"[{ModModelParent.Name}] ERP file not unpacked.");
            }
        }

        private bool BackupOriginalFile()
        {
            // If original file already exists, ignore
            // ! This is crucial to allow different mods modifying the same erp file without failing backup
            if (!File.Exists(ErpFilePath + Settings.Instance.BackupFileExtension))
            {
                try
                {
                    File.Copy(ErpFilePath, ErpFilePath + Settings.Instance.BackupFileExtension);
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"[{ModModelParent.Name}] Failed backing up file at {ErpFilePath}\nThis file will NOT be modded for your safety");
                    Logger.FileWrite(ex.ToString(), Logger.MessageType.Error);
                    return false;
                }
            }

            return true;
        }
    }
}
