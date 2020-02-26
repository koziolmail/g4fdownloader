using System;
using System.Collections.Generic;
using System.Text;

namespace Downloader {
    interface IMainWindowModelAction {
        LinkDto DecalcLink(string base64Link);
        void SetProgress(int value, long fileSize, long currentFileProgress, string currentFile, int fileInDirNo, int filesinDirCount);

        bool IsBusy { get; set; }
    }
}
