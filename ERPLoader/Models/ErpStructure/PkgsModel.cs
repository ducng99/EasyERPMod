using EgoEngineLibrary.Archive.Erp;
using EgoEngineLibrary.Data.Pkg;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace ERPLoader.Models
{
    public class PkgsModel
    {
        private readonly ConcurrentDictionary<string, ErpFragment> Packages = new();

        public PkgsModel(ResourcesModel resourcesModel)
        {
            Parallel.ForEach(resourcesModel.Resources, resource =>
            {
                Parallel.ForEach(resource.Fragments, fragment =>
                {
                    try
                    {
                        using var ds = fragment.GetDecompressDataStream(true);
                        if (PkgFile.IsPkgFile(ds))
                        {
                            string fileName = ResourcesModel.GetFragmentFileName(resource, fragment);

                            string fileNameWIndex = "";

                            for (uint i = 0; string.IsNullOrWhiteSpace(fileNameWIndex); i++)
                            {
                                string tmpFileNameWIndex = i > 0 ? fileName + $".{i}" : fileName;
                                if (!Packages.ContainsKey(tmpFileNameWIndex))
                                {
                                    fileNameWIndex = tmpFileNameWIndex;
                                }
                            }

                            Packages.TryAdd(fileNameWIndex, fragment);
                        }
                    }
                    catch { }
                });
            });
        }

        public string ReadFile(string fileName)
        {
            string fileNameWOExt = Path.GetExtension(fileName).ToLower().Equals(".json") ? Path.GetFileNameWithoutExtension(fileName) : fileName;

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
            string fileNameWOExt = Path.GetExtension(fileName).ToLower().Equals(".json") ? Path.GetFileNameWithoutExtension(fileName) : fileName;

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
