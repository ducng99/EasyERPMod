using EasyERPExplorer.Renderer;
using EasyERPExplorer.Models;
using ImGuiNET;
using System.Numerics;
using ERPLoader;
using System.IO;

namespace EasyERPExplorer.Windows
{
    class GameFolderExplorer : FileExplorer
    {
        public static GameFolderExplorer Instance;
        protected override DirectoryModel RootFolder { get; set; }

        public GameFolderExplorer()
        {
            Instance = this;

            if (Directory.Exists(Settings.Instance.F1GameDirectory))
                RootFolder = new(Settings.Instance.F1GameDirectory);
            else
                RootFolder = new(Directory.GetCurrentDirectory());
        }

        public override void Draw()
        {
            ImGui.SetNextWindowPos(new Vector2(Padding.X, Padding.Y + 20));
            ImGui.SetNextWindowSize(new Vector2(Window.Instance.ClientSize.X / 2f - Padding.X * 1.5f, Window.Instance.ClientSize.Y - Padding.Y * 2));

            if (ImGui.Begin("Game Directory Explorer", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoBringToFrontOnFocus))
            {
                // Menu
                if (ImGui.Button("Refresh"))
                {
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
                    string tmpPath = Settings.Instance.F1GameDirectory;
                    var pathLength = ImGui.CalcTextSize(tmpPath).X + 100;

                    ImGui.PushItemWidth(pathLength);
                    ImGui.InputText("", ref tmpPath, 32767);
                    ImGui.PopItemWidth();

                    Settings.Instance.F1GameDirectory = tmpPath;

                    if (ImGui.Button("Save"))
                    {
                        if (Settings.Instance.Verify())
                        {
                            Settings.Instance.SaveSettings();
                            RootFolder = new(Settings.Instance.F1GameDirectory);
                            ImGui.CloseCurrentPopup();
                        }
                        else
                        {
                            ImGui.OpenPopup("SetF1FailedPopup");
                        }
                    }

                    if (ImGui.BeginPopup("SetF1FailedPopup"))
                    {
                        ImGui.Text("F1 directory is invalid, please fix the path!");

                        if (ImGui.Button("Close##close-popup-SetF1FailedPopup"))
                        {
                            ImGui.CloseCurrentPopup();
                        }

                        ImGui.EndPopup();
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
