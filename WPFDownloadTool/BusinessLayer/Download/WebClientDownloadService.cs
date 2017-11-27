using System;
using System.ComponentModel;
using System.Net;
using WPFDownloadTool.Model;

namespace WPFDownloadTool.BusinessLayer.Download
{
    public class WebClientDownloadService : IDownloadService
    {                
        private WebClient _webClient;
               
        public event EventHandler<MyDownloadEventArgs> DownloadComplete;
        public event EventHandler<MyDownloadEventArgs> DownloadProgressChanged;
        public event EventHandler<MyDownloadEventArgs> DownloadCancel;
        public event EventHandler<MyDownloadEventArgs> DownloadPause;

        public Engine Engine => Engine.WebClient;

        public void CancelDownload(Model.Download download)
        {
            if (_webClient.IsBusy)
                _webClient.CancelAsync();
        }

        public async void DownloadFile(Model.Download download)
        {
            try
            {
                using (_webClient = new WebClient())
                {
                    _webClient.DownloadFileCompleted += WebClientOnDownloadFileCompleted;
                    _webClient.DownloadProgressChanged += WebClientOnDownloadProgressChanged;
                    await _webClient.DownloadFileTaskAsync(new Uri(download.SourcePath),
                        download.TargetPathWithFileName);
                }
            }
            catch (Exception e)
            {
                DownloadCancel?.Invoke(this, new MyDownloadEventArgs());
            }
        }

        public void ResumeDownload(Model.Download download)
        {
            DownloadFile(download);
        }

        public void PauseDownload(Model.Download download)
        {
            if (_webClient.IsBusy)
                _webClient.CancelAsync();
        }

        private void WebClientOnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs downloadProgressChangedEventArgs)
        {
            OnDownloadProgressChanged(new MyProgress() { TotalFileSize = downloadProgressChangedEventArgs.TotalBytesToReceive, CurrentFileSize = downloadProgressChangedEventArgs.BytesReceived });
        }

        private void WebClientOnDownloadFileCompleted(object sender, AsyncCompletedEventArgs asyncCompletedEventArgs)
        {
            if (asyncCompletedEventArgs.Cancelled == true)
            {                
                DownloadCancel?.Invoke(this, new MyDownloadEventArgs());
            }
            else
            {
                DownloadComplete?.Invoke(this, new MyDownloadEventArgs());                
            }
            
        }

        public void OnDownloadProgressChanged(MyProgress value)
        {            
            var args = new MyDownloadEventArgs()
            {
                TotalFileSize = value.TotalFileSize,
                CurrentFileSize = value.CurrentFileSize
            };
            DownloadProgressChanged?.Invoke(this, args);
        }
    }
}
