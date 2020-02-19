using System;

namespace Downloader
{
    public interface IErrorHandler
    {
        void HandleError(Exception ex);
    }
}