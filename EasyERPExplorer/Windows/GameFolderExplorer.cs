using EasyERPExplorer.Renderer;
using EasyERPExplorer.Models;
using ImGuiNET;
using System.Numerics;
using System.Collections.Generic;
using ERPLoader;

namespace EasyERPExplorer.Windows
{
    class GameFolderExplorer : FileExplorer
    {
        public static GameFolderExplorer Instance;
        protected override DirectoryModel RootFolder { get; set; } = new(Settings.Instance.F1GameDirectory);

        public readonly HashSet<ImGuiDrawWindow> AdditionalDrawings = new();

        public GameFolderExplorer()
        {
            Instance = this;
        }

        public override void Draw()
        {
            ImGui.SetNextWindowPos(Padding);
            ImGui.SetNextWindowSize(new Vector2(Window.Instance.ClientSize.X / 2f - Padding.X * 1.5f, Window.Instance.ClientSize.Y - Padding.Y * 2));

            if (ImGui.Begin("F1 Game Directory", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoBringToFrontOnFocus))
            {
                // Menu
                if (ImGui.Button("Refresh"))
                {
                    RootFolder = new(Settings.Instance.F1GameDirectory);
                }

                ImGui.SameLine();
                if (ImGui.Button("Set F1 Game Folder"))
                {
                    ImGui.OpenPopup("SetF1Folder");
                }

                if (ImGui.BeginPopup("SetF1Folder"))
                {
                    ImGui.Text("Current F1 path:");
                    string tmpPath = Settings.Instance.F1GameDirectory;
                    var pathLength = ImGui.CalcTextSize(tmpPath).X + 100;

                    ImGui.PushItemWidth(pathLength);
                    ImGui.InputText("", ref tmpPath, 32767);
                    ImGui.PopItemWidth();

                    Settings.Instance.F1GameDirectory = tmpPath;

                    if (ImGui.Button("Save"))
                    {
                        Settings.Instance.SaveSettings();
                        RootFolder = new(Settings.Instance.F1GameDirectory);
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

            foreach (var drawing in AdditionalDrawings)
            {
                drawing.Draw();
            }

            AdditionalDrawings.RemoveWhere(drawing => !drawing.IsOpen);
        }
    }
}
