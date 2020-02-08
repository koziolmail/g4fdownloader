using Downloader.Download;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WebDav;

namespace Downloader {
    class MainWindowModel : IMainWindowModelAction {
        private readonly DownloadService DownloadService;

        public MainWindowBinding Binding { get; private set; }
        public RelayCommand DownloadCommand { get; private set; }
        public RelayCommand ChooseDestinationFolderCommand { get; private set; }
        public MainWindowModel() { }
        public MainWindowModel(DownloadService downloadService) {
            DownloadService = downloadService;
            Binding = new MainWindowBinding(this);
            DownloadCommand = new RelayCommand(Download, CanDownload);
            ChooseDestinationFolderCommand = new RelayCommand(ChooseDestination, CanChooseDestinationFolder);
        }

        public LinkDto DecalcLink(string base64Link) {
            try {
                var base64EncodedBytes = System.Convert.FromBase64String(base64Link);
                var jsonLink = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                LinkDto link = Newtonsoft.Json.JsonConvert.DeserializeObject<LinkDto>(jsonLink);
                IsLinkValid = true;
                return link;
            } catch (Exception) {

                IsLinkValid = false;
                return null;
            } finally {
                ChooseDestinationFolderCommand.RaiseCanExecuteChanged();
                DownloadCommand.RaiseCanExecuteChanged();
            }
        }

        private bool IsBusy { get; set; }

        public bool CanDownload() {
            if (!IsLinkValid)
                return false;

            if (!IsDestinationPathSet)
                return false;

            return !IsBusy;
        }

        public void Download() {
            DownloadAsync();
        }
        async void DownloadAsync() {
            await Task.Run(DownloadDo);
        }
        async public Task DownloadDo() {
            try {
                IsBusy = true;
                await DownloadService.Download(Binding.LinkDto, Binding.DestinationPath, this);
            } finally {
                IsBusy = false;
            }
        }

        private bool IsLinkValid { get; set; } = false;
        private bool IsDestinationPathSet { get; set; } = false;
        public bool CanChooseDestinationFolder() {
            return IsLinkValid;
        }
        public void ChooseDestination() {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            if (!String.IsNullOrWhiteSpace(UserSettings.Default.DestinationFolder))
                dialog.InitialDirectory = UserSettings.Default.DestinationFolder;
            dialog.Title = "Select a Directory"; // instead of default "Save As"
            dialog.Filter = "Directory|*.this.directory"; // Prevents displaying files
            dialog.FileName = "select"; // Filename will then be "select.this.directory"
            if (dialog.ShowDialog() == true) {
                string path = dialog.FileName;

                // Remove fake filename from resulting path
                path = path.Replace("\\select.this.directory", "");
                path = path.Replace(".this.directory", "");
                // If user has changed the filename, create the new directory
                if (!System.IO.Directory.Exists(path)) {
                    System.IO.Directory.CreateDirectory(path);
                }
                // Our final value is in path

                Binding.DestinationPath = path;
                UserSettings.Default.DestinationFolder = path;
                UserSettings.Default.Save();
                IsDestinationPathSet = true;
                DownloadCommand.RaiseCanExecuteChanged();
            }
        }
        public void SetProgress(int value) {
            Binding.ProgressValue = value;
        }
    }
}