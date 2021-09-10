using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Updater
{
    public class UpdateInfo
    {
        private struct GithubResponseAsset
        {
            public string browser_download_url { get; set; }
            public uint size { get; set; }
        }

        private struct GithubResponse
        {
            public string tag_name { get; set; }
            public string body { get; set; }
            public IList<GithubResponseAsset> assets { get; set; }
        }

        private static readonly HttpClient WebClient = new();
        private static readonly string GithubReleaseURL = "https://api.github.com/repos/ducng99/EasyERPMod/releases/latest";

        public string ChangeLog { get; private set; }
        public string Version { get; private set; }
        public string DownloadURL { get; private set; }
        public uint Size { get; private set; }

        private static readonly string DownloadedFilePath = Path.Combine(Path.GetTempPath(), "EasyERPMod_Temp.zip");
        private static readonly string FolderExtracted = Path.Combine(Path.GetTempPath(), "EasyERPMod_TempEx");
        private static readonly string SelfUpdateFilePath = Path.Combine(Path.GetTempPath(), "EasySelfUpdater.bat");
        private static readonly string SelfUpdateBatContent = $@"
@ECHO off

:LOOP
TASKLIST | FIND /I ""{Process.GetCurrentProcess().ProcessName}"" >NUL 2>&1
IF ERRORLEVEL 1 (
  GOTO CONTINUE
) ELSE (
  TIMEOUT /T 2 /NOBREAK
  GOTO LOOP
)

:CONTINUE
MOVE /Y %1 %2
";

        static UpdateInfo()
        {
            WebClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/99.0.7113.93 Safari/537.36");
        }

        public static UpdateInfo GetLatestRelease()
        {
            var task = Task.Run(async () =>
            {
                try
                {
                    UpdateInfo info = new();

                    string response = await WebClient.GetStringAsync(GithubReleaseURL);
                    var githubResponse = JsonSerializer.Deserialize<GithubResponse>(response);

                    if (githubResponse.assets.Count > 0)
                    {
                        info.ChangeLog = githubResponse.body;
                        info.Version = githubResponse.tag_name;
                        info.DownloadURL = githubResponse.assets[0].browser_download_url;
                        info.Size = githubResponse.assets[0].size;

                        var currentVersion = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
                        string[] tmpVersion = info.Version.TrimStart('v').Split('.');
                        var releaseVersion = new Version(int.Parse(tmpVersion[0]), int.Parse(tmpVersion[1]), int.Parse(tmpVersion[2]));

                        if (releaseVersion > currentVersion && !string.IsNullOrEmpty(info.DownloadURL))
                        {
                            return info;
                        }
                    }

                    return null;
                }
                catch
                {
                    return null;
                }
            });

            task.Wait();
            return task.Result;
        }

        public async void DownloadAndInstall(MainWindow window)
        {
            try
            {
                using (var httpStream = await WebClient.GetStreamAsync(DownloadURL))
                {
                    if (File.Exists(DownloadedFilePath)) File.Delete(DownloadedFilePath);
                    using var fileStream = new FileStream(DownloadedFilePath, FileMode.Create, FileAccess.Write, FileShare.Read);
                    await httpStream.CopyToAsync(fileStream);
                }

                // Kill running exe
                var exeFiles = Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.exe");
                foreach (var exeFile in exeFiles)
                {
                    if (!Path.GetFileNameWithoutExtension(exeFile).Equals(Process.GetCurrentProcess().ProcessName))
                    {
                        foreach (var process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension(exeFile)))
                        {
                            if (process.MainModule.FileName.Equals(exeFile))
                                process.Kill();
                        }
                    }
                }

                // Install
                if (Directory.Exists(FolderExtracted))
                    Directory.Delete(FolderExtracted, true);
                ZipFile.ExtractToDirectory(DownloadedFilePath, FolderExtracted);

                string realFolderPath = Path.Combine(FolderExtracted, "EasyERPMod");
                var filesInExtractedFolder = Directory.EnumerateFiles(realFolderPath, "*", SearchOption.AllDirectories);

                ProcessStartInfo psi = null;
                Regex modsFolderCheck = new(@"^_MODS.+", RegexOptions.Compiled);

                foreach (var file in filesInExtractedFolder)
                {
                    string relativePath = Path.GetRelativePath(realFolderPath, file);

                    if (!modsFolderCheck.IsMatch(relativePath))
                    {
                        try
                        {
                            if (Path.GetFileName(relativePath).Equals(Process.GetCurrentProcess().ProcessName + ".exe"))
                            {
                                string newUpdaterPath = relativePath + "_new";
                                File.Move(file, newUpdaterPath, true);
                                File.WriteAllText(SelfUpdateFilePath, SelfUpdateBatContent);

                                psi = new()
                                {
                                    FileName = SelfUpdateFilePath,
                                    Arguments = $"\"{newUpdaterPath}\" \"{relativePath}\"",
                                    WorkingDirectory = Directory.GetCurrentDirectory(),
                                    CreateNoWindow = true,
                                    WindowStyle = ProcessWindowStyle.Hidden
                                };
                            }
                            else
                            {
                                string directoryPath = Path.GetDirectoryName(relativePath);
                                if (!string.IsNullOrEmpty(directoryPath))
                                    new DirectoryInfo(directoryPath).Create();
                                File.Move(file, relativePath, true);
                            }
                        }
                        catch (IOException ex)
                        {
                            throw new Exception($"Failed when installing \"{file}\"\n", ex);
                        }
                    }
                }

                window.OnInstallCompleted(psi);
            }
            catch (Exception ex)
            {
                window.OnInstallFailed($"{ex}");
            }
            finally
            {
                // Cleanup
                if (Directory.Exists(FolderExtracted))
                    Directory.Delete(FolderExtracted, true);
                if (File.Exists(DownloadedFilePath))
                    File.Delete(DownloadedFilePath);
            }
        }
    }
}
