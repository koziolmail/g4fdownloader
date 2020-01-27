using System;
using System.Collections.Generic;
using System.Text;

namespace Downloader
{
    class MainWindowModel
    {
        public ConnectionModel ConnectionModel { get; set; }
        
        public MainWindowModel()
        {
            ConnectionModel = new ConnectionModel();
        }
    }
}
