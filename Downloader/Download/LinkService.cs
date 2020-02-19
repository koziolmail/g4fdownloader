using System;
using System.Collections.Generic;
using System.Text;

namespace Downloader.Download {
    public class LinkService {
        public LinkDto DecalcLink(string base64Link) {
            try {
                var base64EncodedBytes = System.Convert.FromBase64String(base64Link);
                var jsonLink = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                LinkDto link = Newtonsoft.Json.JsonConvert.DeserializeObject<LinkDto>(jsonLink);
                return link;
            } catch (Exception) {
                return null;
            }
        }
    }
}
