using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebDav;
using System.Linq;

namespace Downloader.Download {
    class DownloadService {
        private IWebDavClient Client;
        private IMainWindowModelAction Action;
        async internal Task Download(LinkDto linkDto, string destinationDir, IMainWindowModelAction action) {
            Action = action;
            using (Client = new WebDavClient(ConnectParams(linkDto))) {
                await DownloadFolder(linkDto.Path, destinationDir);
            }
        }
        async private Task DownloadFolder(string resourceToDownloadUri, string destinationDir) {
            PropfindResponse resourceProperties = await Client.Propfind(resourceToDownloadUri);
            if (resourceProperties.IsSuccessful) {
                foreach (var nextResource in resourceProperties.Resources) {
                    if (nextResource.IsCollection) {
                        if (nextResource.Uri != resourceToDownloadUri) {
                            string[] resourceParts = nextResource.Uri.Split(Path.AltDirectorySeparatorChar);
                            var dirName = (string)resourceParts.GetValue(resourceParts.Count()-2);
                            var newDestinationDir = Path.Combine(destinationDir, dirName);
                            Directory.CreateDirectory(newDestinationDir);
                            await DownloadFolder(nextResource.Uri, newDestinationDir);
                        }
                    } else
                        await DownloadFile(nextResource, destinationDir);
                }
            }
        }
        async private Task DownloadFile(WebDavResource fileResource, string destinationDir) {
            using (var response = await Client.GetRawFile(fileResource.Uri)) {
                string decodedResourceUri = HttpUtility.UrlDecode(fileResource.Uri);
                string newFilePath = Path.Combine(destinationDir, Path.GetFileName(decodedResourceUri));
                using (var fileStream = File.Create(newFilePath)) {
                    byte[] buffer = new byte[8 * 1024];
                    long currentPosition = 0;
                    int len;
                    while ((len = await response.Stream.ReadAsync(buffer, 0, buffer.Length)) > 0) {
                        await fileStream.WriteAsync(buffer, 0, len);
                        currentPosition += len;
                        SetProgress(currentPosition, fileResource.ContentLength);
                    }
                }
            }
        }

        public void SetProgress(long currentPosition, long? length) {
            if (length != 0) {
                double? perc = ((double)currentPosition / length) * 100;
                if (perc.HasValue) {
                    Action.SetProgress((int)perc.Value);
                }
            }
        }

        internal WebDavClientParams ConnectParams(LinkDto linkDto) {
            var clientParams = new WebDavClientParams {
                BaseAddress = new Uri(linkDto.Address),
                Credentials = new NetworkCredential(linkDto.User, linkDto.Pass)
            };
            return clientParams;
        }
    }
}
