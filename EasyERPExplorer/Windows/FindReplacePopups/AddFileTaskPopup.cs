using EasyERPExplorer.Renderer;
using ImGuiNET;
using System.Collections.Generic;
using System.Numerics;
using static ERPLoader.Models.FindReplaceModel;

namespace EasyERPExplorer.Windows.FindReplacePopups
{
    class AddFileTaskPopup : ImGuiDrawWindow
    {
        private readonly IList<FileTask> ParentList;
        private readonly FileTask OriginalTask, CurrentTask;

        private readonly Vector2 Position;
        private bool IsPositionSet = false;

        public AddFileTaskPopup(IList<FileTask> parentList, FileTask currentTask = null)
        {
            ParentList = parentList;
            OriginalTask = currentTask;
            CurrentTask = currentTask != null ? currentTask.Clone() : new FileTask();

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
                ImGui.Text("File name:"); ImGui.SameLine();

                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("Eg. mercedes!!!temp000.vtf.xml");
                    ImGui.EndTooltip();
                }

                string tmpFileName = CurrentTask.FileName;
                ImGui.PushItemWidth(300);
                ImGui.InputText("##file-name", ref tmpFileName, 260);
                ImGui.PopItemWidth();
                CurrentTask.FileName = tmpFileName;

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
