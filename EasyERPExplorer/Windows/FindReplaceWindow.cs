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

                ImGui.Text("Tasks"); ImGui.SameLine();

                if (ImGui.Button("Add ERP file##add-model"))
                {
                    Window.DrawWindows.Add(new AddFindReplaceModelPopup(FindReplaceTasks));
                }

                for (int erpFileTaskIndex = 0; erpFileTaskIndex < FindReplaceTasks.Count; erpFileTaskIndex++)
                {
                    var erpFileTask = FindReplaceTasks[erpFileTaskIndex];

                    if (ImGui.TreeNodeEx($"ErpFilePath: {erpFileTask.ErpFilePath}##tree-{erpFileTaskIndex}", ImGuiTreeNodeFlags.Framed))
                    {
                        if (ImGui.Button($"Edit##edit-{erpFileTaskIndex}"))
                        {
                            Window.DrawWindows.Add(new AddFindReplaceModelPopup(FindReplaceTasks, erpFileTask));
                        }

                        ImGui.SameLine();
                        ImGui.PushStyleColor(ImGuiCol.Button, 0xee0000ff);
                        if (ImGui.Button($"Remove##remove-{erpFileTaskIndex}"))
                        {
                            FindReplaceTasks.Remove(erpFileTask);
                        }
                        ImGui.PopStyleColor();

                        ImGui.Text("Tasks:");

                        ImGui.SameLine();
                        if (ImGui.Button($"Add text file##add-{erpFileTaskIndex}"))
                        {
                            Window.DrawWindows.Add(new AddFileTaskPopup(erpFileTask.Tasks));
                        }

                        for (int textFileTaskIndex = 0; textFileTaskIndex < erpFileTask.Tasks.Count; textFileTaskIndex++)
                        {
                            var textFileTask = erpFileTask.Tasks[textFileTaskIndex];

                            if (ImGui.TreeNodeEx($"File Name: {textFileTask.FileName}##tree-{erpFileTaskIndex}-{textFileTaskIndex}", ImGuiTreeNodeFlags.Framed))
                            {
                                if (ImGui.Button($"Edit##edit-{erpFileTaskIndex}-{textFileTaskIndex}"))
                                {
                                    Window.DrawWindows.Add(new AddFileTaskPopup(erpFileTask.Tasks, textFileTask));
                                }

                                ImGui.SameLine();
                                ImGui.PushStyleColor(ImGuiCol.Button, 0xee0000ff);
                                if (ImGui.Button($"Remove##remove-{erpFileTaskIndex}-{textFileTaskIndex}"))
                                {
                                    erpFileTask.Tasks.Remove(textFileTask);
                                }
                                ImGui.PopStyleColor();

                                ImGui.Text("Tasks:");

                                ImGui.SameLine();
                                if (ImGui.Button($"Add task##add-{erpFileTaskIndex}-{textFileTaskIndex}"))
                                {
                                    Window.DrawWindows.Add(new AddFindReplaceTaskPopup(textFileTask.Tasks));
                                }

                                for (int taskIndex = 0; taskIndex < textFileTask.Tasks.Count; taskIndex++)
                                {
                                    var task = textFileTask.Tasks[taskIndex];

                                    if (ImGui.TreeNodeEx($"[{taskIndex}]", ImGuiTreeNodeFlags.Framed))
                                    {
                                        if (ImGui.Button($"Edit##edit-{erpFileTaskIndex}-{textFileTaskIndex}-{taskIndex}"))
                                        {
                                            Window.DrawWindows.Add(new AddFindReplaceTaskPopup(textFileTask.Tasks, task));
                                        }

                                        ImGui.SameLine();
                                        ImGui.PushStyleColor(ImGuiCol.Button, 0xee0000ff);
                                        if (ImGui.Button($"Remove##remove-{erpFileTaskIndex}-{textFileTaskIndex}-{taskIndex}"))
                                        {
                                            textFileTask.Tasks.Remove(task);
                                        }
                                        ImGui.PopStyleColor();

                                        ImGui.Text($"Search type: {task.SearchType}");
                                        ImGui.Text($"Search for: {task.SearchFor}");
                                        ImGui.Text($"Replace with: {task.ReplaceWith}");

                                        ImGui.TreePop();
                                    }
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
