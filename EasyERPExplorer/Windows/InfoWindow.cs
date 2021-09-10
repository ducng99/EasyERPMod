using EasyERPExplorer.Renderer;
using ERPLoader;
using ImGuiNET;
using System.Diagnostics;

namespace EasyERPExplorer.Windows
{
    class InfoWindow : ImGuiDrawWindow
    {
        private bool ShowSettingsWindow = false;
        private bool ShowLicenseWindow = false;
        private bool ShowAuthorWindow = false;
        private bool ShowAppInfoWindow = false;

        public override void Draw()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("Settings"))
                {
                    ImGui.MenuItem("View/Edit", null, ref ShowSettingsWindow);
                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("About"))
                {
                    ImGui.MenuItem("License", null, ref ShowLicenseWindow);
                    ImGui.MenuItem("Author", null, ref ShowAuthorWindow);
                    ImGui.MenuItem("EasyERPMod", null, ref ShowAppInfoWindow);
                    ImGui.EndMenu();
                }

                ImGui.EndMainMenuBar();
            }
#if DEBUG
            ImGui.ShowStyleEditor();
#endif

            if (ShowSettingsWindow && ImGui.Begin("Settings", ImGuiWindowFlags.AlwaysAutoResize))
            {
                if (ImGui.Button("Save"))
                {
                    Settings.Instance.SaveSettings();
                }
                ImGui.SameLine();
                if (ImGui.Button("Close"))
                {
                    ShowSettingsWindow = false;
                }
                ImGui.Separator();

                if (ImGui.BeginCombo("Font size", $"{Settings.Instance.ExplorerSettings.FontSize + 12}px"))
                {
                    for (short i = 0; i < ImGuiController.Fonts.Count; i++)
                    {
                        if (ImGui.Selectable($"{i + 12}px"))
                        {
                            Settings.Instance.ExplorerSettings.FontSize = i;
                        }
                    }

                    ImGui.EndCombo();
                }

                ImGui.End();
            }

            if (ShowLicenseWindow && ImGui.Begin("License", ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoCollapse))
            {
                string license = @"Copyright (C) 2021  Duc Nguyen

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.";

                ImGui.Text(license);
                if (ImGui.Button("Close"))
                {
                    ShowLicenseWindow = false;
                }
            }

            if (ShowAuthorWindow && ImGui.Begin("Author", ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoCollapse))
            {
                ImGui.Text("Hi, I'm Tom. A developer who cannot find a job then get too bored and create apps for fun."); ImGui.NewLine();
                ImGui.Text("For now, that's all :)");

                ImGui.NewLine();
                if (ImGui.Button("Close"))
                {
                    ShowAuthorWindow = false;
                }
            }

            if (ShowAppInfoWindow && ImGui.Begin("EasyERPMod", ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoCollapse))
            {
                string version = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString(3);
                ImGui.BulletText("EasyERPMod version " + version);
                ImGui.NewLine();
                ImGui.Text("The app is created to help simplify modding process for F1 games using Ego Engine Modding.");
                if (ImGui.Button("GitHub"))
                {
                    Process.Start(new ProcessStartInfo { FileName = "https://github.com/ducng99/EasyERPMod", UseShellExecute = true });
                }
                ImGui.SameLine();
                if (ImGui.Button("RaceDepartment"))
                {
                    Process.Start(new ProcessStartInfo { FileName = "https://www.racedepartment.com/downloads/easyerpmod-make-modding-easier.44824/", UseShellExecute = true });
                }

                ImGui.Separator();
                if (ImGui.Button("Close"))
                {
                    ShowAppInfoWindow = false;
                }
            }
        }
    }
}
