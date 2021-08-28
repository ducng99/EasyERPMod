using EgoEngineLibrary.Archive.Erp;
using EgoEngineLibrary.Xml;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace ERPLoader.Models
{
    public class XmlsModel
    {
        public readonly ConcurrentDictionary<string, ErpFragment> XmlFiles = new();

        public XmlsModel(ResourcesModel resourcesModel)
        {
            Parallel.ForEach(resourcesModel.Resources, resource =>
            {
                Parallel.ForEach(resource.Fragments, fragment =>
                {
                    try
                    {
                        using var ds = fragment.GetDecompressDataStream(true);
                        if (XmlFile.IsXmlFile(ds))
                        {
                            var fileName = ResourcesModel.GetFragmentFileName(resource, fragment);
                            if (!XmlFiles.TryAdd(fileName, fragment))
                            {
                                // Doesn't seem to happen, just to be sure, let's log it
                                Logger.Warning($"Duplicated file found! Name: \"{fileName}\"");
                            }
                        }
                    }
                    catch { }
                });
            });
        }

        public string ReadFile(string fileName)
        {
            // .xml = 4 in length
            string fileNameWOExt = Path.GetExtension(fileName).ToLower().Equals(".xml") ? fileName[0..^4] : fileName;

            if (XmlFiles.ContainsKey(fileNameWOExt))
            {
                try
                {
                    using var utf8Stream = new Utils.Utf8StringWriter();
                    XmlFiles[fileNameWOExt].ExportXML(utf8Stream);

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
            // .xml = 4 in length
            string fileNameWOExt = Path.GetExtension(fileName).ToLower().Equals(".xml") ? fileName[0..^4] : fileName;

            if (XmlFiles.ContainsKey(fileNameWOExt))
            {
                try
                {
                    using var stream = Utils.GetStreamFromString(content);
                    XmlFiles[fileNameWOExt].ImportXML(stream);
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed writing file \"{fileName}\"");
                    Logger.FileWrite(ex.ToString(), Logger.MessageType.Error);
                }
            }
        }

        public void Export(string xmlsFolderPath)
        {
            Directory.CreateDirectory(xmlsFolderPath);

            Parallel.ForEach(XmlFiles, xmlFile =>
            {
                try
                {
                    string filePath = Path.Combine(xmlsFolderPath, xmlFile.Key) + ".xml";

                    if (File.Exists(filePath))
                    {
                        // idk if this can happen, just to be sure
                        Logger.Warning($"Found duplicate file at \"{filePath}\". This file will NOT be overwritten.");
                    }
                    else
                    {
                        using var fs = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
                        using var sw = new StreamWriter(fs);
                        xmlFile.Value.ExportXML(sw);

                        Logger.Log($"Exported {xmlFile.Key}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("Failed exporting xml file " + xmlFile.Key);
                    Logger.FileWrite(ex.ToString(), Logger.MessageType.Error);
                }
            });
        }

        public void Import(string xmlsFolderPath)
        {
            Parallel.ForEach(XmlFiles, xmlFile =>
            {
                string filePath = Path.Combine(xmlsFolderPath, xmlFile.Key) + ".xml";

                if (File.Exists(filePath))
                {
                    try
                    {
                        using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                            xmlFile.Value.ImportXML(fs);

                        Logger.Log($"Imported {xmlFile.Key}");
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Failed importing XML file " + xmlFile.Key);
                        Logger.FileWrite(ex.ToString(), Logger.MessageType.Error);
                    }
                }
            });
        }
    }
}
