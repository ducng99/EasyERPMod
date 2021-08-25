using EasyERPExplorer.Renderer;
using ImGuiNET;
using System;
using System.Collections.Generic;
using static ERPLoader.Models.FindReplaceModel;

namespace EasyERPExplorer.Windows.FindReplacePopups
{
    class AddFindReplaceTaskPopup : ImGuiDrawWindow
    {
        private readonly IList<FindReplaceTask> ParentList;
        private readonly FindReplaceTask OriginalTask, CurrentTask;

        private static readonly string[] SearchTypesStrings = Enum.GetNames(typeof(SearchTypeEnum));

        public AddFindReplaceTaskPopup(IList<FindReplaceTask> parentList, FindReplaceTask current = null)
        {
            ParentList = parentList;
            OriginalTask = current ?? new FindReplaceTask();
            CurrentTask = current != null ? current.Clone() : new FindReplaceTask();
        }

        public override void Draw()
        {
            if (ImGui.Begin("Add new Find & Replace Task##add-task-" + GetHashCode(), ImGuiWindowFlags.AlwaysAutoResize))
            {
                ImGui.Text("Search type:"); ImGui.SameLine();

                int searchType = (int)CurrentTask.SearchType;
                ImGui.Combo($"##search-type", ref searchType, SearchTypesStrings, 2);
                CurrentTask.SearchType = (SearchTypeEnum)searchType;

                ImGui.Text("Search for:"); ImGui.SameLine();
                string searchFor = CurrentTask.SearchFor;
                ImGui.PushItemWidth(500);
                ImGui.InputText("##search-for", ref searchFor, 1024);
                ImGui.PopItemWidth();
                CurrentTask.SearchFor = searchFor;

                ImGui.Text("Replace with:"); ImGui.SameLine();
                string replaceWith = CurrentTask.ReplaceWith;
                ImGui.PushItemWidth(500);
                ImGui.InputText("##replace-with", ref replaceWith, 1024);
                ImGui.PopItemWidth();
                CurrentTask.ReplaceWith = replaceWith;

                if (ImGui.Button("Save"))
                {
                    ParentList.Remove(OriginalTask);
                    ParentList.Add(CurrentTask);
                    IsOpen = false;
                }
                ImGui.SameLine();
                if (ImGui.Button("Close"))
                {
                    IsOpen = false;
                }

                ImGui.End();
            }
        }
    }
}
