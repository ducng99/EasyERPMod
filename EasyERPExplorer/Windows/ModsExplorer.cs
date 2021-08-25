using EasyERPExplorer.Models;
using EasyERPExplorer.Renderer;
using ImGuiNET;
using System.Numerics;

namespace EasyERPExplorer.Windows
{
    class ModsExplorer : FileExplorer
    {
        protected override DirectoryModel RootFolder { get; set; } = new(Program.EasyERPSettings.ModsFolderName);

        public override void Draw()
        {
            ImGui.SetNextWindowPos(new Vector2(Window.Instance.ClientSize.X / 2f + Padding.X * 0.5f, Padding.Y));
            ImGui.SetNextWindowSize(new Vector2(Window.Instance.ClientSize.X / 2f - Padding.X * 1.5f, Window.Instance.ClientSize.Y - Padding.Y * 2));

            if (ImGui.Begin("Mods Explorer"))
            {
                // Directory tree
                if (ImGui.TreeNodeEx(RootFolder.Name, ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.Framed))
                {
                    RootFolder.ProcessFolder();
                    ShowSubDirectories(RootFolder);
                    ShowFiles(RootFolder);

                    ImGui.TreePop();
                }

                ImGui.End();
            }
        }
    }
}
