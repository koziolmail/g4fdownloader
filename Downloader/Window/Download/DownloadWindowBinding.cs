using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Downloader {
    class DownloadWindowBinding : INotifyPropertyChanged {
        private readonly IMainWindowModelAction Actions;
        public DownloadWindowBinding(IMainWindowModelAction actions) {
            Actions = actions;
        }
        private string base64Link;
        public string Base64Link {
            get {
                return base64Link;
            }
            set {
                base64Link = value;
                LinkDto = Actions.DecalcLink(base64Link);

                if (LinkDto is LinkDto) {
                    string[] resourceParts = LinkDto.Path.Split(Path.AltDirectorySeparatorChar);
                    FileInfo = (string)resourceParts.GetValue(resourceParts.Count() - 2);
                }
                else
                    FileInfo = "";
            }
        }


        public LinkDto LinkDto { get; set; }
        private string fileInfo = "Informacja o pobieranym filmie";
        public string FileInfo {
            get {
                return fileInfo;
            }
            set {
                fileInfo = value;
                OnPropertyChanged(nameof(FileInfo));
            }
        }

        private string destinationPath;
        public string DestinationPath {
            get {
                return destinationPath;
            }
            set {
                destinationPath = value;
                OnPropertyChanged(nameof(DestinationPath));
            }

        }

        private int percentPrograssValue;
        public int PercentPrograssValue {
            get {
                return percentPrograssValue;
            }
            set {
                percentPrograssValue = value;
                OnPropertyChanged(nameof(PercentPrograssValue));
            }
        }

        private string stringProgressValue;
        public string StringProgressValue {
            get {
                return stringProgressValue;
            }
            set {
                stringProgressValue = value;
                OnPropertyChanged(nameof(StringProgressValue));
            }
        }
        


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
