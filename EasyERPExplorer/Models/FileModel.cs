using EasyERPExplorer.Renderer;
using EasyERPExplorer.Windows;
using ERPLoader;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace EasyERPExplorer.Models
{
    class FileModel : IOTemplate
    {
        private readonly DirectoryModel ParentDirectory;

        public FileModel(string path, DirectoryModel parentDirectory)
        {
            Name = Path.GetFileName(path);
            FullPath = path;
            ParentDirectory = parentDirectory;
        }

        public void Click()
        {
            if (Path.GetExtension(Name).ToLower().Equals(".erp"))
            {
                Window.DrawWindows.Where(d => d.GetType().Name.Equals(nameof(AddERPToModPopup))).ToList().ForEach(d => d.IsOpen = false);
                Window.DrawWindows.Add(new AddERPToModPopup(this));
            }
            else if (Name.Equals(Settings.Instance.FindReplaceFileName))
            {
                if (!Window.DrawWindows.Any(d => d.GetType().Name.Equals(nameof(FindReplaceWindow)) && ((FindReplaceWindow)d).ModFolder.Equals(ParentDirectory)))
                    Window.DrawWindows.Add(new FindReplaceWindow(ParentDirectory));
            }
            else
            {
                Process.Start("explorer.exe", FullPath);
            }
        }
    }
}
