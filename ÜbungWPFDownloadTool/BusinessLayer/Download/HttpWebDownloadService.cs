using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ÜbungWPFDownloadTool.Model;
using ÜbungWPFDownloadTool.ViewModels;

namespace ÜbungWPFDownloadTool.BusinessLayer.Download
{
    public class HttpWebDownloadService : IDownloadService
    {
        private CancellationToken _cancellationToken;
        private CancellationTokenSource _cancellationTokenSource;        
        
        public event EventHandler<MyDownloadEventArgs> DownloadComplete;
        public event EventHandler<MyDownloadEventArgs> DownloadProgressChanged;
        public event EventHandler<MyDownloadEventArgs> DownloadCancel;
        public event EventHandler<MyDownloadEventArgs> DownloadPause;

        public Engine Engine => Engine.HttpWeb;
        
        public void CancelDownload(Model.Download download)
        {
            download.FileRenameCancelToken.Cancel();
            _cancellationTokenSource.Cancel();
        }

        public async void ResumeDownload(Model.Download download)
        {
            await RunDownload(
                download, 
                request => request.AddRange(download.GetBytesFromAllParts()));
        }

        public void PauseDownload(Model.Download download)
        {
            download.FileRenameCancelToken.Cancel();
            _cancellationTokenSource.Cancel();            
        }


        public async void DownloadFile(Model.Download download)
        {
            await RunDownload(
                download, 
                request => download.FileRenameStreamer = new FileRenameStreamer(download.TargetPathWithFileName, download.FileRenameCancelToken),
                DownloadCancel
                );
        }

        private async Task RunDownload(Model.Download download, Action<HttpWebRequest> configureRequest, EventHandler<MyDownloadEventArgs> downloadCancelPauseHandler)
        {
            try
            {
                download.FileRenameCancelToken = new FileRenameCancelToken();
                _cancellationTokenSource = new CancellationTokenSource();
                _cancellationToken = _cancellationTokenSource.Token;

                var request = (HttpWebRequest) WebRequest.Create(new Uri(download.SourcePath));

                configureRequest(request);
                
                using (var response = await request.GetResponseAsync())
                {
                    download.FileRenameStreamer.SetContentLength(response.ContentLength);
                    var data = response.GetResponseStream();

                    using (download.FileRenameStreamer)
                    {
                        var byteBuffer = download.FileRenameStreamer.GetByteBuffer();
                        download.FileRenameStreamer.SetStreamPosition(download.GetBytesFromAllParts());

                        await MakeDownload(download, data, byteBuffer);
                    }
                }
            }
            catch (IOException)
            {
                DownloadCancel?.Invoke(this, new MyDownloadEventArgs());
            }
            catch (OperationCanceledException)
            {
                downloadCancelPauseHandler?.Invoke(this, new MyDownloadEventArgs());
            }
            catch (WebException)
            {
                DownloadCancel?.Invoke(this, new MyDownloadEventArgs());
            }
        }

        private async Task MakeDownload(Model.Download download, Stream data, byte[] byteBuffer)
        {
            using (var output = download.FileRenameStreamer.GetFileStream())
            {
                var onProgressChanged = new Progress<MyProgress>(OnDownloadProgressChanged);
                var oldPercentDone = 0.0;
                var bytesRead = 0;

                download.StartDownloadPart();
                do
                {                   
                    bytesRead = await data.ReadAsync(byteBuffer, 0, byteBuffer.Length, _cancellationToken);

                    download.AddBytesToPart(bytesRead);
                    download.FileRenameStreamer.AddCurrentBytesRead(bytesRead);

                    if (bytesRead <= 0)
                        continue;

                    await output.WriteAsync(byteBuffer, 0, bytesRead, _cancellationToken);

                    oldPercentDone = SendDownloadProgress(onProgressChanged, download, oldPercentDone);
                } while (bytesRead > 0);

                download.FinishDownloadPart();
                download.DumpLogFromAllParts();

                DownloadComplete?.Invoke(this, new MyDownloadEventArgs());
            }
        }

        private static double SendDownloadProgress(IProgress<MyProgress> onProgressChanged, Model.Download download, double oldPercentDone)
        {            
            var myProgress = new MyProgress()
            {
                CurrentFileSize =  download.GetBytesFromAllParts(),
                TotalFileSize = download.FileRenameStreamer.GetTotalFileSize()
            };

            download.TotalFileSize = download.FileRenameStreamer.GetTotalFileSize();

            var percentDone = (double)myProgress.CurrentFileSize / myProgress.TotalFileSize;

            if (!(percentDone - oldPercentDone > 0.05))
                return oldPercentDone;

            onProgressChanged?.Report(myProgress);

            return percentDone;
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
