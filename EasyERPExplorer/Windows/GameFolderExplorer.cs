using EasyERPExplorer.Renderer;
using EasyERPExplorer.Models;
using ImGuiNET;
using System.Numerics;

namespace EasyERPExplorer.Windows
{
    class GameFolderExplorer : ImGuiDrawTask
    {
        private string F1GameDirectory => Program.EasyERPSettings.F1GameDirectory;

        public override void Draw()
        {
            if (ImGui.Begin("F1 Game Directory", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse))
            {
                ImGui.SetWindowPos(new Vector2(10, 10));
                ImGui.SetWindowSize(Window)

                ImGui.End();
            }
        }
    }
}
