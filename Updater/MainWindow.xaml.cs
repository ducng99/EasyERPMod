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
        }

        public void Start()
        {
            string[] args = Environment.GetCommandLineArgs();

            foreach (string arg in args)
            {
                if (arg.Equals("/autoUpdate", StringComparison.Ordinal))
                    IsAutoUpdate = true;
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
                    MessageBox.Show("No new update found.", Name, MessageBoxButton.OK);
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
            DownloadButton.IsEnabled = true;

            Show();
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            DownloadButton.IsEnabled = false;
            updateInfo.DownloadAndInstall(this);
            DownloadButton.Content = "Installing...";
        }

        public void OnInstallCompleted(ProcessStartInfo selfUpdater = null)
        {
            MessageBox.Show($"Install EasyERPMod {updateInfo.Version} succeeded!", Name);

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
