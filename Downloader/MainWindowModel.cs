using ClassLibrary6;
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

namespace Downloader
{
    class MainWindowModel : IMainWindowModelAction
    {

        public MainWindowBinding Binding { get; private set; }
        public RelayCommand DownloadCommand { get; private set; }
        public RelayCommand ChooseDestinationFolderCommand { get; private set; }
        public MainWindowModel()
        {
            Binding = new MainWindowBinding(this);
            DownloadCommand = new RelayCommand(Download, CanDownload);
            ChooseDestinationFolderCommand = new RelayCommand(ChooseDestination, CanChooseDestinationFolder);
        }

        public LinkDto DecalcLink(string base64Link)
        {
            try
            {
                var base64EncodedBytes = System.Convert.FromBase64String(base64Link);
                var jsonLink = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                LinkDto link = Newtonsoft.Json.JsonConvert.DeserializeObject<LinkDto>(jsonLink);
                IsLinkValid = true;
                return link;
            }
            catch (Exception) {
                
                IsLinkValid = false;
                return null; 
            }
            finally
            {
                ChooseDestinationFolderCommand.RaiseCanExecuteChanged();
                DownloadCommand.RaiseCanExecuteChanged();
            }
        }

        private bool IsBusy { get; set; }
        
        public bool CanDownload()
        {
            if (!IsLinkValid)
                return false;

            if (!IsDestinationPathSet)
                return false;

            return !IsBusy;
        }

        
        public void Download()
        {
            DownloadAsync();
        }
        async void DownloadAsync()
        {
            await Task.Run(DownloadDo);
        }
        async public Task DownloadDo()
        {
            try
            {
                IsBusy = true;
                var clientParams = new WebDavClientParams
                {
                    BaseAddress = new Uri(Binding.LinkDto.Address),
                    Credentials = new NetworkCredential(Binding.LinkDto.User, Binding.LinkDto.Pass)
                };
                IWebDavClient client = new WebDav.WebDavClient(clientParams);
                using var response = await client.GetRawFile(Binding.LinkDto.Path);

                using var fileStream = File.Create(Binding.DestinationPath);
                response.Stream.CopyTo(fileStream);
                Binding.FileInfo = "Pobieranie skończone";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool IsLinkValid { get; set; } = false;
        private bool IsDestinationPathSet { get; set; } = false;
        public bool CanChooseDestinationFolder()
        {
            return IsLinkValid;
        }
        public void ChooseDestination()
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            if (!String.IsNullOrWhiteSpace(UserSettings.Default.DestinationFolder))
                dialog.InitialDirectory = UserSettings.Default.DestinationFolder;
            dialog.FileName = Path.GetFileName(Binding.LinkDto.Path);
            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;

                Binding.DestinationPath = path;
                UserSettings.Default.DestinationFolder = Path.GetDirectoryName(path);
                UserSettings.Default.Save();
                IsDestinationPathSet = true;
                DownloadCommand.RaiseCanExecuteChanged();
            }
        }
    }
}