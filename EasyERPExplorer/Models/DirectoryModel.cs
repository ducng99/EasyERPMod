using System.IO;

namespace EasyERPExplorer.Models
{
    class DirectoryModel
    {
        public string Name { get; private set; }
        public bool IsExpanded { get; set; }
        public string FullPath { get; private set; }

        public DirectoryModel(string path)
        {
            Name = new DirectoryInfo(path).Name;
            IsExpanded = false;
            FullPath = path;
        }
    }
}
