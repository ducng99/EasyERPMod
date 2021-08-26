using EasyERPExplorer.Renderer;
using ImGuiNET;
using System.Collections.Generic;
using ERPLoader.Models;
using System.Numerics;

namespace EasyERPExplorer.Windows.FindReplacePopups
{
    class AddFindReplaceModelPopup : ImGuiDrawWindow
    {
        private readonly IList<FindReplaceModel> ParentList;
        private readonly FindReplaceModel OriginalTask, CurrentTask;

        private readonly Vector2 Position;
        private bool IsPositionSet = false;

        public AddFindReplaceModelPopup(IList<FindReplaceModel> parentList, FindReplaceModel currentTask = null)
        {
            ParentList = parentList;
            OriginalTask = currentTask;
            CurrentTask = currentTask != null ? currentTask.Clone() : new FindReplaceModel();

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
                ImGui.Text("ERP file relative path:"); ImGui.SameLine();
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text(@"Eg. 2021_asset_groups\f1_2021_vehicle_package\teams\common.erp");
                    ImGui.EndTooltip();
                }

                string tmpFilePath = CurrentTask.ErpFilePath;
                ImGui.PushItemWidth(500);
                ImGui.InputText("##file-path", ref tmpFilePath, 1024);
                ImGui.PopItemWidth();
                if (tmpFilePath.IsPathValid())
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
