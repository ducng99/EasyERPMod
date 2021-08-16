using EgoEngineLibrary.Archive.Erp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyERPMod.Models
{
    class ResourcesModel
    {
        private const string QEscString = "%3F";
        private const string FragNameDelim = "!!!";

        public List<ErpResource> Resources { get; private set; }

        public ResourcesModel(ErpFile data)
        {
            Resources = data.Resources;
        }
        /*
        public void Export(string folderPath)
        {
            foreach (var resource in Resources)
            {
                try
                {
                    var outputDir = Path.Combine(folderPath, resource.Folder);
                    Directory.CreateDirectory(outputDir);

                    for (var i = 0; i < resource.Fragments.Count; ++i)
                    {
                        var fragment = resource.Fragments[i];
                        var fileName = GetFragmentFileName(resource, fragment, i);
                        var filePath = Path.Combine(outputDir, fileName);

                        using var fs = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
                        using var decompressStream = fragment.GetDecompressDataStream(true);
                        decompressStream.CopyTo(fs);
                    }
                }
                catch (Exception)
                {
                    Logger.Error("Failed to export " + resource.FileName);
                }
            }
        }

        private static string GetFragmentFileName(ErpResource resource, ErpFragment fragment, int fragmentIndex)
        {
            var name = resource.FileName;
            name = name.Replace("?", QEscString);
            name = Path.GetFileNameWithoutExtension(name) + FragNameDelim
                + fragment.Name + fragmentIndex.ToString("000")
                + Path.GetExtension(name);
            return name;
        }*/
    }
}
