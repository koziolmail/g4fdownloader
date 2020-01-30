using ClassLibrary6;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace Downloader
{
    class MainWindowBinding : INotifyPropertyChanged
    {
        private readonly IMainWindowModelAction Actions;
        public MainWindowBinding(IMainWindowModelAction actions)
        {
            Actions = actions;
        }
        private string base64Link;
        public string Base64Link
        {
            get
            {
                return base64Link;
            }
            set
            {
                base64Link = value;
                LinkDto = Actions.DecalcLink(base64Link);
                
                if (LinkDto is LinkDto)
                    FileInfo = Path.GetFileName(LinkDto.Path);
                else
                    FileInfo = "";
            }
        }

        
        public LinkDto LinkDto { get; set; }

        private string fileInfo;
        public string FileInfo
        {
            get
            {
                return fileInfo;
            }
            set
            {
                fileInfo = value;
                OnPropertyChanged(nameof(FileInfo));
            }
        }

        private string destinationPath;
        public string DestinationPath
        {
            get
            {
                return destinationPath;
            }
            set
            {
                destinationPath = value;
                OnPropertyChanged(nameof(DestinationPath));
            }
            
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
