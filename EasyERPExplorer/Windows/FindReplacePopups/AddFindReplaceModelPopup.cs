using EasyERPExplorer.Renderer;
using ImGuiNET;
using System.Collections.Generic;
using ERPLoader.Models;

namespace EasyERPExplorer.Windows.FindReplacePopups
{
    class AddFindReplaceModelPopup : ImGuiDrawWindow
    {
        private readonly IList<FindReplaceModel> ParentList;
        private readonly FindReplaceModel OriginalTask, CurrentTask;

        public AddFindReplaceModelPopup(IList<FindReplaceModel> parentList, FindReplaceModel currentTask = null)
        {
            ParentList = parentList;
            OriginalTask = currentTask;
            CurrentTask = currentTask != null ? currentTask.Clone() : new FindReplaceModel();
        }

        public override void Draw()
        {
            if (ImGui.Begin("Find & Replace Task##add-task-" + GetHashCode(), ImGuiWindowFlags.AlwaysAutoResize))
            {
                ImGui.Text("File name:"); ImGui.SameLine();

                string tmpFilePath = CurrentTask.ErpFilePath;
                ImGui.PushItemWidth(500);
                ImGui.InputText("##file-path", ref tmpFilePath, 1024);
                ImGui.PopItemWidth();
                CurrentTask.ErpFilePath = tmpFilePath;

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
