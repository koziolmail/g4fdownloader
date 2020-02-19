using System;
using System.Collections.Generic;
using System.Text;

namespace Downloader
{

    class ConnectionModel
    {
        public string IpAddressText { get; set; } = "Podaj IP";
        public string UserText { get; set; } = "Podaj użytkownika";
        public string PassText { get; set; } = "Podaj hasło";
        public ConnectionModel()
        {
        }
    }
}
