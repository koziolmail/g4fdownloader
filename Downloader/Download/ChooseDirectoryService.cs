using System;
using System.IO;

namespace Downloader.Download {
    public class ChooseDirectoryService {
        public string ChooseDestination() {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            if (!String.IsNullOrWhiteSpace(UserSettings.Default.DestinationFolder))
                dialog.InitialDirectory = UserSettings.Default.DestinationFolder;
            dialog.Title = "Wybierz katalog";
            dialog.Filter = "Katalog|*.katalog.zapisu";
            dialog.FileName = "wybierz";
            if (dialog.ShowDialog() == true) {
                string path = dialog.FileName;

                // Remove fake filename from resulting path
                path = Path.GetDirectoryName(path);
                // Remove fake filename from resulting path
                //path = path.Replace("\\wybierz.katalog.zapisu", "");
                //path = path.Replace(".katalog.zapisu", "");

                if (!System.IO.Directory.Exists(path)) {
                    System.IO.Directory.CreateDirectory(path);
                }

                
                UserSettings.Default.DestinationFolder = path;
                UserSettings.Default.Save();
                return path;
            }
            return "";
        }
    }
}
