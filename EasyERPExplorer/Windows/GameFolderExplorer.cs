using EasyERPExplorer.Renderer;
using EasyERPExplorer.Models;
using ImGuiNET;
using System.Numerics;

namespace EasyERPExplorer.Windows
{
    class GameFolderExplorer : FileExplorer
    {
        protected override DirectoryModel RootFolder { get; set; } = new(Program.EasyERPSettings.F1GameDirectory);

        public override void Draw()
        {
            ImGui.SetNextWindowPos(Padding);
            ImGui.SetNextWindowSize(new Vector2(Window.Instance.ClientSize.X / 2f - Padding.X * 1.5f, Window.Instance.ClientSize.Y - Padding.Y * 2));

            if (ImGui.Begin("F1 Game Directory", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse))
            {
                // Menu
                if (ImGui.Button("Refresh"))
                {
                    RootFolder = new(Program.EasyERPSettings.F1GameDirectory);
                    RootFolder.ProcessFolder();
                }

                ImGui.SameLine();
                if (ImGui.Button("Set F1 Game Folder"))
                {
                    ImGui.OpenPopup("SetF1Folder");
                }

                if (ImGui.BeginPopup("SetF1Folder"))
                {
                    ImGui.Text("Current F1 path:");
                    string tmpPath = Program.EasyERPSettings.F1GameDirectory;
                    var pathLength = ImGui.CalcTextSize(tmpPath).X + 100;

                    ImGui.PushItemWidth(pathLength);
                    ImGui.InputText("", ref tmpPath, 32767);
                    ImGui.PopItemWidth();

                    Program.EasyERPSettings.F1GameDirectory = tmpPath;

                    if (ImGui.Button("Save"))
                    {
                        Program.EasyERPSettings.SaveSettings();
                        RootFolder = new(Program.EasyERPSettings.F1GameDirectory);
                        ImGui.CloseCurrentPopup();
                    }

                    ImGui.EndPopup();
                }

                ImGui.Separator();

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
