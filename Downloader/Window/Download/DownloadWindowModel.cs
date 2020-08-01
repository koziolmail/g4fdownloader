using Downloader.Download;
using GalaSoft.MvvmLight.Command;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Downloader {
    class DownloadWindowModel : IMainWindowModelAction {
        private readonly DownloadService DownloadService;
        private readonly ChooseDirectoryService ChooseFolderService;
        private readonly LinkService LinkService;
        private bool isLinkValid = false;
        private bool IsLinkValid {
            get { return isLinkValid; }
            set {
                isLinkValid = value;
                ChooseDestinationFolderCommand.RaiseCanExecuteChanged();
                DownloadCommand.RaiseCanExecuteChanged();
            }
        }
        private bool isDestinationPathSet = false;
        private bool IsDestinationPathSet {
            get { return isDestinationPathSet; }
            set {
                isDestinationPathSet = value;
                DownloadCommand.RaiseCanExecuteChanged();
            }
        }
        private bool isBusy = false;
        public bool IsBusy {
            get { return isBusy; }
            set {
                isBusy = value;
                Application.Current.Dispatcher.Invoke(() => {
                    DownloadCommand.RaiseCanExecuteChanged();
                    ChooseDestinationFolderCommand.RaiseCanExecuteChanged();
                });
            }
        }
        public DownloadWindowBinding Binding { get; private set; }
        public RelayCommand DownloadCommand { get; private set; }
        public RelayCommand ChooseDestinationFolderCommand { get; private set; }
        public DownloadWindowModel() { }
        public DownloadWindowModel(DownloadService downloadService, ChooseDirectoryService chooseFolderService, LinkService linkService) {
            DownloadService = downloadService;
            ChooseFolderService = chooseFolderService;
            LinkService = linkService;
            Binding = new DownloadWindowBinding(this);
            DownloadCommand = new RelayCommand(DownloadAsync, CanDownload);
            ChooseDestinationFolderCommand = new RelayCommand(ChooseDestination, CanChooseDestinationFolder);
        }

        public void ChooseDestination() {
            string path = ChooseFolderService.ChooseDestination();
            if (path != "") {
                Binding.DestinationPath = path;
                IsDestinationPathSet = true;

            }
        }
        public LinkDto DecalcLink(string base64Link) {
            LinkDto link = LinkService.DecalcLink(base64Link);
            IsLinkValid = link != null;

            return link;
        }
        public bool CanDownload() {
            if (!IsLinkValid)
                return false;

            if (!IsDestinationPathSet)
                return false;

            return !IsBusy;
        }
        async void DownloadAsync() {
            string res = await Task.Run(DownloadDo);
            MessageBox.Show(res);
            Binding.FileInfo = res;
        }
        async public Task<string> DownloadDo() {
            try {
                IsBusy = true;

                return await DownloadService.Download(Binding.LinkDto, Binding.DestinationPath, this);
            } finally {
                IsBusy = false;
            }
        }
        public bool CanChooseDestinationFolder() {
            if (!IsLinkValid)
                return false;

            return !IsBusy;
        }
        public void SetProgress(int percentValue, long fileSize, long downloadProgress, string currentFile, int fileInDirNo, int filesinDirCount) {
            Binding.PercentPrograssValue = percentValue;
            Binding.StringProgressValue = String.Format("{0:n0}kB / {1:n0}kB  - {2}% - plik {3} z {4} - {5}", downloadProgress / 1024L, fileSize / 1024L, percentValue, fileInDirNo, filesinDirCount, currentFile);
        }
    }
}