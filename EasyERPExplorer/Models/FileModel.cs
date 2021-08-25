using EasyERPExplorer.Windows;
using ERPLoader.Models;
using ImGuiNET;
using System.Diagnostics;
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
            if (Path.GetExtension(Name).ToLower().Equals(".erp"))
            {
                GameFolderExplorer.Instance.AdditionalDrawings.Add(new AddERPToModPopup(this));
            }
            else
            {
                Process.Start("explorer.exe", FullPath);
            }
        }
    }
}
