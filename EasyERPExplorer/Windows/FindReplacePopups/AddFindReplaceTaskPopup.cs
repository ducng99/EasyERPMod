using EasyERPExplorer.Renderer;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using static ERPLoader.Models.FindReplaceModel;

namespace EasyERPExplorer.Windows.FindReplacePopups
{
    class AddFindReplaceTaskPopup : ImGuiDrawWindow
    {
        private readonly IList<FindReplaceTask> ParentList;
        private readonly FindReplaceTask OriginalTask, CurrentTask;

        private static readonly string[] SearchTypesStrings = Enum.GetNames(typeof(SearchTypeEnum));

        private readonly Vector2 Position;
        private bool IsPositionSet = false;

        public AddFindReplaceTaskPopup(IList<FindReplaceTask> parentList, FindReplaceTask currentTask = null)
        {
            ParentList = parentList;
            OriginalTask = currentTask ?? new FindReplaceTask();
            CurrentTask = currentTask != null ? currentTask.Clone() : new FindReplaceTask();

            Position = ImGui.GetIO().MousePos;
        }

        public override void Draw()
        {
            if (!IsPositionSet)
            {
                ImGui.SetNextWindowPos(Position);
                IsPositionSet = true;
            }

            if (ImGui.Begin("Find & Replace Task##add-task-" + GetHashCode(), ImGuiWindowFlags.AlwaysAutoResize))
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
