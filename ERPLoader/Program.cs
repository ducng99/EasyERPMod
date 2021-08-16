using EasyERPMod.Models;
using EgoEngineLibrary.Archive.Erp;
using System.IO;

namespace EasyERPMod
{
    class Program
    {
        static void Main(string[] args)
        {
            var erpFile = new ErpFile();
            erpFile.Read(File.Open(args[0], FileMode.Open, FileAccess.Read, FileShare.Read));

            var resourcesModel = new ResourcesModel(erpFile);
            var pkgsModel = new PkgsModel(resourcesModel);

            var folderPath = Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "tmp")).FullName;

            pkgsModel.Export(folderPath);

            erpFile.Write(File.Open(args[0] + "1", FileMode.Create, FileAccess.Write, FileShare.Read));
        }
    }
}
