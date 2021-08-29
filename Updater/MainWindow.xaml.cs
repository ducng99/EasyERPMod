using System;
using System.Diagnostics;
using System.Windows;

namespace Updater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UpdateInfo updateInfo;
        private bool IsAutoUpdate = false;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;

            string[] args = Environment.GetCommandLineArgs();

            if (args.Length > 0)
            {
                foreach (string arg in args)
                {
                    if (arg.Equals("/autoUpdate"))
                        IsAutoUpdate = true;
                }
            }

            var updateInfo = UpdateInfo.GetLatestRelease();

            if (updateInfo != null)
            {
                InitUpdateInfo(updateInfo);
            }
            else
            {
                if (!IsAutoUpdate)
                {
                    MessageBox.Show("No new update found.", "Updater", MessageBoxButton.OK);
                }

                Close();
            }
        }

        private void InitUpdateInfo(UpdateInfo info)
        {
            updateInfo = info;

            TitleText.Text = "New update found!";
            ChangeLogText.Text = "Changelog for " + info.Version;
            UpdateLog.Text = info.ChangeLog;
            DownloadButton.Click += DownloadButton_Click;

            WindowState = WindowState.Normal;
            DownloadButton.IsEnabled = true;
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            DownloadButton.IsEnabled = false;
            updateInfo.DownloadAndInstall(this);
            DownloadButton.Content = "Installing...";
        }

        public void OnInstallCompleted(ProcessStartInfo selfUpdater = null)
        {
            MessageBox.Show($"Installed EasyERPMod {updateInfo.Version} succeeded!", Name);

            if (selfUpdater != null)
                Process.Start(selfUpdater);
            Close();
        }

        public void OnInstallFailed(string error)
        {
            MessageBox.Show($"Failed to update EasyERPMod {updateInfo.Version}! Please try again later.\n\n{error}", Name, MessageBoxButton.OK, MessageBoxImage.Error);
            Close();
        }
    }
}
