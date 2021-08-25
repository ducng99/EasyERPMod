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

        protected virtual void ShowSubDirectories(DirectoryModel directory)
        {
            foreach (var dir in directory.SubDirectories)
            {
                if (ImGui.TreeNodeEx(dir.Name, ImGuiTreeNodeFlags.Framed))
                {
                    if (ImGui.Button("Show in Explorer##show-explorer-" + dir.FullPath))
                    {
                        System.Diagnostics.Process.Start("explorer.exe", dir.FullPath);
                    }

                    dir.ProcessFolder();
                    ShowSubDirectories(dir);
                    ShowFiles(dir);

                    ImGui.TreePop();
                }
            }
        }

        protected virtual void ShowFiles(DirectoryModel directory)
        {

            foreach (var file in directory.FilesInFolder)
            {
                if (AllowedFileTypes.IsMatch(file.Name))
                {
                    ImGui.Bullet(); ImGui.SameLine();

                    if (ImGui.Selectable($"{file.Name}##open-file-{file.FullPath}"))
                        file.Click();
                }
            }
        }
    }
}
