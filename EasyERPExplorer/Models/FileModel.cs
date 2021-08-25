using System.IO;

namespace EasyERPExplorer.Models
{
    class FileModel : IOTemplate
    {
        public FileModel(string path)
        {
            Name = Path.GetFileName(path);
            FullPath = path;
        }

        public void Click()
        {

        }
    }
}
