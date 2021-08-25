using EasyERPExplorer.Models;
using EasyERPExplorer.Renderer;
using ImGuiNET;
using System.Text.RegularExpressions;

namespace EasyERPExplorer.Windows
{
    abstract class FileExplorer : ImGuiDrawWindow
    {
        private static readonly Regex AllowedFileTypes = new(@".+\.(erp|json|xml|dds)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        protected abstract DirectoryModel RootFolder { get; set; }

        protected void ShowSubDirectories(DirectoryModel directory)
        {
            foreach (var dir in directory.SubDirectories)
            {
                if (ImGui.TreeNodeEx(dir.Name, ImGuiTreeNodeFlags.Framed))
                {
                    dir.ProcessFolder();
                    ShowSubDirectories(dir);
                    ShowFiles(dir);

                    ImGui.TreePop();
                }
            }
        }

        protected void ShowFiles(DirectoryModel directory)
        {
            foreach (var file in directory.FilesInFolder)
            {
                if (AllowedFileTypes.IsMatch(file.Name) && ImGui.TreeNodeEx(file.Name, ImGuiTreeNodeFlags.Bullet))
                {
                    file.Click();
                    ImGui.TreePop();
                }
            }
        }
    }
}
