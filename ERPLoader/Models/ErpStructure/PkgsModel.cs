using EgoEngineLibrary.Archive.Erp;
using EgoEngineLibrary.Data.Pkg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ERPLoader.Models
{
    public class PkgsModel
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

        public string ReadFile(string fileName)
        {
            // .json = 5 in length
            string fileNameWOExt = Path.GetExtension(fileName).ToLower().Equals(".xml") ? fileName[0..^5] : fileName;

            if (Packages.ContainsKey(fileNameWOExt))
            {
                try
                {
                    using var utf8Stream = new Utils.Utf8StringWriter();
                    Packages[fileNameWOExt].ExportPkg(utf8Stream);

                    return utf8Stream.GetStringBuilder().ToString();
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed reading file \"{fileName}\"");
                    Logger.FileWrite(ex.ToString(), Logger.MessageType.Error);
                }
            }

            return null;
        }

        public void WriteFile(string fileName, string content)
        {
            // .json = 5 in length
            string fileNameWOExt = Path.GetExtension(fileName).ToLower().Equals(".xml") ? fileName[0..^5] : fileName;

            if (Packages.ContainsKey(fileNameWOExt))
            {
                try
                {
                    using var stream = Utils.GetStreamFromString(content);
                    Packages[fileNameWOExt].ImportPkg(stream);
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed writing file \"{fileName}\"");
                    Logger.FileWrite(ex.ToString(), Logger.MessageType.Error);
                }
            }
        }

        public void Export(string packageFolderPath)
        {
            Directory.CreateDirectory(packageFolderPath);

            Parallel.ForEach(Packages, package =>
            {
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

                        Logger.Log($"Exported {package.Key}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("Failed exporting package " + package.Key);
                    Logger.FileWrite(ex.ToString(), Logger.MessageType.Error);
                }
            });
        }

        public void Import(string packageFolderPath)
        {
            Parallel.ForEach(Packages, package =>
            {
                try
                {
                    string filePath = Path.Combine(packageFolderPath, package.Key) + ".json";

                    if (File.Exists(filePath))
                    {
                        using var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                        package.Value.ImportPkg(fs);

                        Logger.Log($"Imported {package.Key}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("Failed importing package " + package.Key);
                    Logger.FileWrite(ex.ToString(), Logger.MessageType.Error);
                }
            });
        }
    }
}
