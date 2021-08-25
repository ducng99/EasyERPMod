using EasyERPExplorer.Models;
using EasyERPExplorer.Renderer;
using ERPLoader;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace EasyERPExplorer.Windows
{
    class ModsExplorer : FileExplorer
    {
        public static ModsExplorer Instance { get; private set; }
        protected override DirectoryModel RootFolder { get; set; } = new(Settings.Instance.ModsFolderName);
        public DirectoryModel SelectedMod { get; private set; }

        private string NewModInput = "";

        public ModsExplorer()
        {
            Instance = this;
        }

        public override void Draw()
        {
            ImGui.SetNextWindowPos(new Vector2(Window.Instance.ClientSize.X / 2f + Padding.X * 0.5f, Padding.Y));
            ImGui.SetNextWindowSize(new Vector2(Window.Instance.ClientSize.X / 2f - Padding.X * 1.5f, Window.Instance.ClientSize.Y - Padding.Y * 2));

            if (ImGui.Begin("Mods Explorer", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoBringToFrontOnFocus))
            {
                // Menu
                if (ImGui.Button("Refresh"))
                {
                    RootFolder = new(RootFolder.FullPath);
                }

                ImGui.SameLine();
                if (ImGui.Button("Create new mod"))
                {
                    ImGui.OpenPopup("CreateModPopup");
                }

                if (ImGui.BeginPopup("CreateModPopup"))
                {
                    ImGui.Text("Mod name:"); ImGui.SameLine();
                    ImGui.InputText("", ref NewModInput, 260);

                    if (ImGui.Button("Create"))
                    {
                        CreateMod(NewModInput);
                        NewModInput = "";
                        ImGui.CloseCurrentPopup();
                    }

                    ImGui.EndPopup();
                }

                ImGui.Separator();
                if (SelectedMod != null)
                {
                    ImGui.Text($"Selected mod: {SelectedMod.Name}");
                    ImGui.SameLine();

                    if (ImGui.Button("Deselect"))
                    {
                        SelectedMod = null;
                        RootFolder = new(Settings.Instance.ModsFolderName);
                    }
                }
                else
                {
                    ImGui.Text("Select a mod from the list below");
                }

                ImGui.Separator();

                // Directory tree
                if (ImGui.TreeNodeEx(RootFolder.Name, ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.Framed))
                {
                    RootFolder.ProcessFolder();
                    if (SelectedMod == null)
                        ShowMods(RootFolder);
                    else
                        ShowSubDirectories(SelectedMod);

                    ImGui.TreePop();
                }

                ImGui.End();
            }
        }

        private void ShowMods(DirectoryModel directory)
        {
            foreach (var dir in directory.SubDirectories)
            {
                ImGui.Text(dir.Name);
                ImGui.SameLine();

                if (ImGui.Button($"Select mod##select-mod-{dir.Name}"))
                {
                    SelectedMod = dir;
                    RootFolder = SelectedMod;
                }
            }
        }

        private void CreateMod(string name)
        {
            name = name.ReplaceAllChars(@"/\:*?""<>|".ToCharArray(), '_');
            string newModPath = Path.Combine(Settings.Instance.ModsFolderName, name);
            Directory.CreateDirectory(newModPath);
            SelectedMod = new(Path.GetFullPath(newModPath));
            RootFolder = SelectedMod;
        }
    }
}
