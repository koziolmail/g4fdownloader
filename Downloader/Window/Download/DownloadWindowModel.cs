using Downloader.Download;
using GalaSoft.MvvmLight.Command;
using System;
using System.Threading.Tasks;

namespace Downloader {
    class DownloadWindowModel : IMainWindowModelAction {
        private readonly DownloadService DownloadService;
        private readonly ChooseDirectoryService ChooseFolderService;
        private readonly LinkService LinkService;

        public DownloadWindowBinding Binding { get; private set; }
        public RelayCommand DownloadCommand { get; private set; }
        public RelayCommand ChooseDestinationFolderCommand { get; private set; }
        public DownloadWindowModel() { }
        public DownloadWindowModel(DownloadService downloadService, ChooseDirectoryService chooseFolderService, LinkService linkService) {
            DownloadService = downloadService;
            ChooseFolderService = chooseFolderService;
            LinkService = linkService;
            Binding = new DownloadWindowBinding(this);
            DownloadCommand = new RelayCommand(Download, CanDownload);
            ChooseDestinationFolderCommand = new RelayCommand(ChooseDestination, CanChooseDestinationFolder);
        }

        public void ChooseDestination() {
            string path = ChooseFolderService.ChooseDestination();
            if (path != "") {
                Binding.DestinationPath = path;
                IsDestinationPathSet = true;
                DownloadCommand.RaiseCanExecuteChanged();
            }
        }

        public LinkDto DecalcLink(string base64Link) {
            LinkDto link = LinkService.DecalcLink(base64Link);
            IsLinkValid = link != null;
            ChooseDestinationFolderCommand.RaiseCanExecuteChanged();
            DownloadCommand.RaiseCanExecuteChanged();
            return link;
        }

        public bool IsBusy { get; set; }

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

        public void SetProgress(int value) {
            Binding.ProgressValue = value;
        }
    }
}