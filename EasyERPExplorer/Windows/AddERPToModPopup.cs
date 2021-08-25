using EasyERPExplorer.Models;
using EasyERPExplorer.Renderer;
using ERPLoader;
using ERPLoader.Models;
using ImGuiNET;
using System.IO;

namespace EasyERPExplorer.Windows
{
    class AddERPToModPopup : ImGuiDrawWindow
    {
        private readonly FileModel ErpFile;
        public static readonly string Name = "AddERPToModPopup";

        public AddERPToModPopup(FileModel erpFile)
        {
            ErpFile = erpFile;
        }

        public override void Draw()
        {
            if (ImGui.Begin(Name, ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize))
            {
                if (ModsExplorer.Instance.SelectedMod != null)
                {
                    ImGui.Text($"Add \"{ErpFile.Name}\" to mod \"{ModsExplorer.Instance.SelectedMod.Name}\"?");
                    if (ImGui.Button("Yes"))
                    {
                        AddERPFileToMod(ModsExplorer.Instance.SelectedMod);
                        IsOpen = false;
                    }

                    ImGui.SameLine();
                    if (ImGui.Button("No"))
                    {
                        IsOpen = false;
                    }
                }
                else
                {
                    ImGui.Text("Select a mod in Mods Explorer window first.");
                    if (ImGui.Button("Close"))
                    {
                        IsOpen = false;
                    }
                }

                ImGui.End();
            }
        }

        private void AddERPFileToMod(DirectoryModel modDir)
        {
            string relativePath = Path.GetRelativePath(Settings.Instance.F1GameDirectory, ErpFile.FullPath);
            string modERPPath = Path.Combine(modDir.FullPath, relativePath);

            Directory.CreateDirectory(modERPPath);

            var modModel = new ModModel(modDir.FullPath);
            var erpFileModel = new ErpFileModel(modModel, modERPPath);
            erpFileModel.Export();
        }
    }
}
