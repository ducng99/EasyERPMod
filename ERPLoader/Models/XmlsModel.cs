using EgoEngineLibrary.Archive.Erp;
using EgoEngineLibrary.Xml;
using System.Collections.Generic;
using System.IO;

namespace ERPLoader.Models
{
    class XmlsModel
    {
        public readonly Dictionary<string, ErpFragment> XmlFiles = new();

        public XmlsModel(ResourcesModel resourcesModel)
        {
            foreach (var resource in resourcesModel.Resources)
            {
                foreach (var fragment in resource.Fragments)
                {
                    try
                    {
                        using var ds = fragment.GetDecompressDataStream(true);
                        if (XmlFile.IsXmlFile(ds))
                        {
                            var fileName = ResourcesModel.GetFragmentFileName(resource, fragment);
                            XmlFiles.Add(fileName, fragment);
                        }
                    }
                    catch
                    {
                        // TODO: log
                    }
                }
            }
        }

        public void Export(string folderPath)
        {
            string xmlsFolderPath = Path.Combine(folderPath, "xmls");
            Directory.CreateDirectory(xmlsFolderPath);

            foreach (var xmlFile in XmlFiles)
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
                catch
                {
                    Logger.Error("Failed exporting xml file " + xmlFile.Key);
                }
            }
        }

        public void Import(ref ResourcesModel resourcesModel, string folderPath)
        {
            string xmlsFolderPath = Path.Combine(folderPath, "xmls");

            foreach (var xmlFile in XmlFiles)
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
                    catch
                    {
                        Logger.Error("Failed importing XML file " + xmlFile.Key);
                    }
                }
            }
        }
    }
}
