using ClassLibrary6;
using System;
using System.Collections.Generic;
using System.Text;

namespace Downloader
{
    interface IMainWindowModelAction
    {
        LinkDto DecalcLink(string base64Link);
    }
}
