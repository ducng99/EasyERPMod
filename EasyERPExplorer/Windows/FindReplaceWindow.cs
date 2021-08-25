using EasyERPExplorer.Models;
using EasyERPExplorer.Renderer;
using EasyERPExplorer.Windows.FindReplacePopups;
using ERPLoader.Models;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EasyERPExplorer.Windows
{
    class FindReplaceWindow : ImGuiDrawWindow
    {
        private readonly DirectoryModel SelectedMod;
        private readonly List<FindReplaceModel> FindReplaceTasks = new();
        private readonly string FindReplaceFilePath;

        public FindReplaceWindow(DirectoryModel mod)
        {
            SelectedMod = mod;
            FindReplaceFilePath = Path.Combine(mod.FullPath, "FindReplace.json");

            if (File.Exists(FindReplaceFilePath))
                FindReplaceTasks.AddRange(FindReplaceModel.FromJson(File.ReadAllText(FindReplaceFilePath)));
            else
                File.WriteAllText(FindReplaceFilePath, JsonSerializer.Serialize(FindReplaceTasks, new JsonSerializerOptions { WriteIndented = true }));
        }

        public override void Draw()
        {
            if (ImGui.Begin("Find & Replace for " + SelectedMod.Name))
            {
                ImGui.Text("Remember to click \"Save to File\" button below after finish editing. If not, no changes will be made!");
                ImGui.Separator();

                if (ImGui.Button("Save to File"))
                {
                    File.WriteAllText(FindReplaceFilePath, JsonSerializer.Serialize(FindReplaceTasks, new JsonSerializerOptions { WriteIndented = true }));
                }

                ImGui.SameLine();
                if (ImGui.Button("Close"))
                {
                    IsOpen = false;
                }

                ImGui.Separator();

                ImGui.Text("Tasks");

                foreach (var erpFileTask in FindReplaceTasks.ToArray())
                {
                    if (ImGui.TreeNodeEx("ErpFilePath: " + erpFileTask.ErpFilePath, ImGuiTreeNodeFlags.Framed))
                    {
                        ImGui.Text("Tasks:");

                        foreach (var textFileTask in erpFileTask.Tasks.ToArray())
                        {
                            if (ImGui.TreeNodeEx("File Name: " + textFileTask.FileName, ImGuiTreeNodeFlags.Framed))
                            {
                                ImGui.Text("Tasks:");

                                ImGui.SameLine();
                                if (ImGui.Button($"Add task##add-{erpFileTask.ErpFilePath}-{textFileTask.FileName}"))
                                {
                                    Window.DrawWindows.Add(new AddFindReplaceTaskPopup(textFileTask.Tasks));
                                }

                                uint i = 0;
                                foreach (var task in textFileTask.Tasks.ToArray())
                                {
                                    if (ImGui.TreeNodeEx($"[{i}]", ImGuiTreeNodeFlags.Framed))
                                    {
                                        if (ImGui.Button($"Edit##edit-{erpFileTask.ErpFilePath}-{textFileTask.FileName}-{i}"))
                                        {
                                            Window.DrawWindows.Add(new AddFindReplaceTaskPopup(textFileTask.Tasks, task));
                                        }

                                        ImGui.SameLine();
                                        ImGui.PushStyleColor(ImGuiCol.Button, 0xee0000ff);
                                        if (ImGui.Button("Remove"))
                                        {
                                            textFileTask.Tasks.Remove(task);
                                        }
                                        ImGui.PopStyleColor();

                                        ImGui.Text($"Search type: {task.SearchType}");
                                        ImGui.Text($"Search for: {task.SearchFor}");
                                        ImGui.Text($"Replace with: {task.ReplaceWith}");

                                        ImGui.TreePop();
                                    }

                                    ++i;
                                }

                                ImGui.TreePop();
                            }
                        }

                        ImGui.TreePop();
                    }
                }

                ImGui.End();
            }
        }
    }
}
