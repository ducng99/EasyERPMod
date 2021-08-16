using EgoEngineLibrary.Data.Pkg;
using System.IO;

namespace EgoEngineLibrary.Archive.Erp
{
    public static class ErpFragmentExtension
    {
        public static void ExportPkg(this ErpFragment fragment, TextWriter textWriter)
        {
            using var dataStream = fragment.GetDataStream(true);
            var package = PkgFile.ReadPkg(dataStream);
            package.WriteJson(textWriter);
        }

        public static void ImportPkg(this ErpFragment fragment, Stream stream)
        {
            var pkg = PkgFile.ReadJson(stream);
            using var ms = new MemoryStream();
            pkg.WritePkg(ms);
            fragment.SetData(ms.ToArray());
        }
    }
}
