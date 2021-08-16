using EgoEngineLibrary.Archive.Erp;
using EgoEngineLibrary.Data.Pkg;
using System.Collections.Generic;
using System.IO;

namespace EasyERPMod.Models
{
    class PkgsModel
    {
        private readonly Dictionary<string, ErpFragment> Packages = new Dictionary<string, ErpFragment>();

        public PkgsModel(ResourcesModel resourcesModel)
        {
            foreach (var resource in resourcesModel.Resources)
            {
                foreach (var fragment in resource.Fragments)
                {
                    try
                    {
                        using var ds = fragment.GetDecompressDataStream(true);
                        if (PkgFile.IsPkgFile(ds))
                        {
                            string fileName = ResourcesModel.GetFragmentFileName(resource, fragment);
                            Packages.Add(fileName, fragment);
                        }
                    }
                    catch
                    {
                        //Logger.Error("Failed when trying to import packages from " + resource.FileName);
                    }
                }
            }
        }

        public void Export(string folderPath)
        {
            string packageFolderPath = Path.Combine(folderPath, "packages");
            Directory.CreateDirectory(packageFolderPath);

            foreach (var package in Packages)
            {
                Logger.Log($"Exporting {package.Key}...");

                try
                {
                    string filePath = Path.Combine(packageFolderPath, package.Key) + ".json";

                    if (File.Exists(filePath))
                    {
                        // idk if this can happen, just to be sure
                        Logger.Warning($"Found duplicate file at \"{filePath}\". This file will NOT be overwritten.");
                    }
                    else
                    {
                        using var fs = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
                        using var sw = new StreamWriter(fs);
                        package.Value.ExportPkg(sw);
                    }
                }
                catch
                {
                    Logger.Error("Failed exporting package " + package.Key);
                }
            }
        }

        public void Import(string folderPath)
        {
            string packageFolderPath = Path.Combine(folderPath, "packages");

            foreach (var package in Packages)
            {
                Logger.Log($"Importing {package.Key}...");

                try
                {
                    string filePath = Path.Combine(packageFolderPath, package.Key) + ".json";

                    if (File.Exists(filePath))
                    {
                        using var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                        package.Value.ImportPkg(fs);
                    }
                }
                catch
                {
                    Logger.Error("Failed importing package " + package.Key);
                }
            }
        }
    }
}
