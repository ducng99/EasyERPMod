using System.Collections.Generic;
using System.IO;

namespace EasyERPExplorer.Models
{
    class DirectoryModel : IOTemplate
    {
        public SortedSet<DirectoryModel> SubDirectories = new(new PathComparer());
        public SortedSet<FileModel> FilesInFolder = new(new PathComparer());

        public DirectoryModel(string path)
        {
            Name = new DirectoryInfo(path).Name;
            FullPath = path;
        }

        public void ProcessFolder()
        {
            var subdirs = Directory.EnumerateDirectories(FullPath);

            foreach (var dir in subdirs)
            {
                SubDirectories.Add(new(dir));
            }

            var files = Directory.EnumerateFiles(FullPath);

            foreach(var file in files)
            {
                FilesInFolder.Add(new(file));
            }
        }
    }
}
