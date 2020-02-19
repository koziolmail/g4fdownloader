using System;
using System.Collections.Generic;
using System.Text;

namespace LinkCreator
{
    class LinkDto
    {
        public string Address { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
        public string Path { get; set; }

        public LinkDto(string address, string user, string pass, string path)
        {
            this.Address = address;
            this.User = user;
            this.Pass = pass;
            this.Path = path;
        }
    }
}
