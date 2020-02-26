using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using WebDav;

namespace Downloader.Download {
    class DownloadService {
        private IWebDavClient Client;
        private IMainWindowModelAction Action;
        async internal Task<String> Download(LinkDto linkDto, string destinationDir, IMainWindowModelAction action) {
            try {
                Action = action;
                using (Client = new WebDavClient(ConnectParams(linkDto))) {
                    await DownloadFolder(linkDto.Path, destinationDir);
                }
            }catch(Exception ex) {
                return ex.ToString();
            }
            return "";
        }
        async private Task DownloadFolder(string resourceToDownloadUri, string destinationDir) {
            PropfindResponse resourceProperties = await Client.Propfind(resourceToDownloadUri);
            if (resourceProperties.IsSuccessful) {
                int resourceNo = 0;
                int fileCount = 0;
                foreach (var nextResource in resourceProperties.Resources) {
                    if (!nextResource.IsCollection) {
                        fileCount++;
                    }
                }
                foreach (var nextResource in resourceProperties.Resources) {
                if (nextResource.IsCollection) {
                    if (nextResource.Uri != resourceToDownloadUri) {
                        string[] resourceParts = nextResource.Uri.Split(Path.AltDirectorySeparatorChar);
                        var dirName = (string)resourceParts.GetValue(resourceParts.Count() - 2);
                        var newDestinationDir = Path.Combine(destinationDir, dirName);
                        Directory.CreateDirectory(newDestinationDir);
                        await DownloadFolder(nextResource.Uri, newDestinationDir);
                    }
                } else {
                    resourceNo++;
                    await DownloadFile(nextResource, destinationDir, resourceNo, fileCount);
                }
                }
            }
        }
        async private Task DownloadFile(WebDavResource fileResource, string destinationDir, int fileInDirNo, int filesinDirCount) {
            using (var response = await Client.GetRawFile(fileResource.Uri)) {
                string decodedResourceUri = HttpUtility.UrlDecode(fileResource.Uri);
                string newFilePath = Path.Combine(destinationDir, Path.GetFileName(decodedResourceUri));
                string newFileName = Path.GetFileName(newFilePath);
                using (var fileStream = File.Create(newFilePath)) {
                    byte[] buffer = new byte[8 * 1024];
                    long currentPosition = 0;
                    int len;
                    while ((len = await response.Stream.ReadAsync(buffer, 0, buffer.Length)) > 0) {
                        await fileStream.WriteAsync(buffer, 0, len);
                        currentPosition += len;
                        SetProgress(currentPosition, fileResource.ContentLength??0, newFileName, fileInDirNo, filesinDirCount);
                    }
                }
            }
        }

        public void SetProgress(long currentPosition, long fileLength, string currentFileName, int fileInDirNo, int filesinDirCount) {
            if (fileLength != 0) {
                double? perc = (double)currentPosition / fileLength * 100;
                if (perc.HasValue) {
                    Action.SetProgress((int)perc.Value, fileLength, currentPosition, currentFileName, fileInDirNo, filesinDirCount);
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
