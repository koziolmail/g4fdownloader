using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using WebDav;

namespace Downloader.Download {
    class DownloadService {
        private IWebDavClient Client;
        private IMainWindowModelAction Action;
        private bool pause = false;
        async internal Task<String> Download(LinkDto linkDto, string destinationDir, IMainWindowModelAction action) {
            try {
                Action = action;
                using (Client = new WebDavClient(ConnectParams(linkDto))) {
                    await DownloadFolder(linkDto.Path, destinationDir);
                }
            } catch (Exception ex) {
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
            string decodedResourceUri = HttpUtility.UrlDecode(fileResource.Uri);

            string newFilePath = Path.Combine(destinationDir, Path.GetFileName(decodedResourceUri));

            FileInfo fileinfo = new FileInfo(newFilePath);
            if (fileinfo.Exists && fileinfo.Length == fileResource.ContentLength)
                return;

            if (fileinfo.Exists)
                await DownloadFileFromLastPosition(fileinfo, fileInDirNo, filesinDirCount, fileResource);
            else
                await DownloadFileFromBeginning(fileinfo, fileInDirNo, filesinDirCount, fileResource);
        }
        async private Task DownloadFileFromBeginning(FileInfo fileinfo, int fileInDirNo, int filesinDirCount, WebDavResource fileResource) {
            long downloadSize = fileResource.ContentLength ?? 0;
            using (var response = await Client.GetRawFile(fileResource.Uri)) {
                using (var fileStream = fileinfo.Create()) {
                    await WriteWebDavToFile(downloadSize, response.Stream, fileinfo, fileInDirNo, filesinDirCount, fileStream, 0);
                }
            }
        }
        private static readonly int BLOCK_SIZE = 8 * 1024;
        async private Task DownloadFileFromLastPosition(FileInfo fileinfo, int fileInDirNo, int filesinDirCount, WebDavResource fileResource) {
            long starPos = (fileinfo.Length / BLOCK_SIZE) * BLOCK_SIZE;
            var downloadParameters = new GetFileParameters {
                Headers = new List<KeyValuePair<string, string>> {
                    new KeyValuePair<string, string>("Range", "bytes=" + starPos.ToString() + "-" + fileResource.ContentLength)
                }.AsReadOnly()
            };
            using (var response = await Client.GetRawFile(fileResource.Uri, downloadParameters)) {

                using (var fileStream = fileinfo.OpenWrite()) {
                    fileStream.Position = starPos;
                    long downloadSize = fileResource.ContentLength ?? 0;
                    await WriteWebDavToFile(downloadSize, response.Stream, fileinfo, fileInDirNo, filesinDirCount, fileStream, starPos);
                }
            }
        }

        async private Task WriteWebDavToFile(long downloadSize, Stream readStream, FileInfo fileinfo, int fileInDirNo, int filesinDirCount, FileStream fileStream, long startPosition) {
            byte[] buffer = new byte[BLOCK_SIZE];
            long currentPosition = startPosition;
            int len;
            while ((len = await readStream.ReadAsync(buffer, 0, buffer.Length)) > 0) {
                while (pause) {
                    Thread.Sleep(1000);
                }
                await fileStream.WriteAsync(buffer, 0, len);
                currentPosition += len;
                SetProgress(currentPosition, downloadSize, fileinfo.Name, fileInDirNo, filesinDirCount);
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

        internal void PauseResume() {
            pause = !pause;
        }
    }
}
