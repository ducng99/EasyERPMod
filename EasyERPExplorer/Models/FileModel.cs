using EasyERPExplorer.Renderer;
using EasyERPExplorer.Windows;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
            if (Path.GetExtension(Name).ToLower().Equals(".erp"))
            {
                Window.DrawWindows.Where(d => d.GetType().Name.Equals(nameof(AddERPToModPopup))).ToList().ForEach(d => d.IsOpen = false);
                Window.DrawWindows.Add(new AddERPToModPopup(this));
            }
            else if (Name.Equals("FindReplace.json"))
            {

            }
            else
            {
                Process.Start("explorer.exe", FullPath);
            }
        }
    }
}
