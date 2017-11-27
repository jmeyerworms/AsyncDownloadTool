using System;
using ÜbungWPFDownloadTool.ViewModels;

namespace ÜbungWPFDownloadTool.BusinessLayer.Download
{
    public interface IDownloadService
    {
        Engine Engine { get; }

        void CancelDownload(Model.Download download);
        void DownloadFile(Model.Download download);
        void ResumeDownload(Model.Download download);
        void PauseDownload(Model.Download download);

        void OnDownloadProgressChanged(MyProgress value);

        event EventHandler<MyDownloadEventArgs> DownloadComplete;
        event EventHandler<MyDownloadEventArgs> DownloadProgressChanged;
        event EventHandler<MyDownloadEventArgs> DownloadCancel;
        event EventHandler<MyDownloadEventArgs> DownloadPause;
    }
}
