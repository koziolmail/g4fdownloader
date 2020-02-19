using System;
using System.Collections.Generic;
using System.Text;

namespace Downloader {
    interface IMainWindowModelAction {
        LinkDto DecalcLink(string base64Link);
        void SetProgress(int value);

        bool IsBusy { get; set; }
    }
}
