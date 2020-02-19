using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security;
using System.Text;

namespace Downloader
{
    public class LinkDto
    {
        public string Path { get; set; }
        public string Address { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
    }
}
