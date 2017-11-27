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
            download.FileRenameCancelToken = new FileRenameCancelToken();
            var fileResume = new FileResumeStreamer(download.TempFileName,download.TargetPathWithFileName, download.FileRenameCancelToken);            

            await RunDownload(
                download, 
                request =>request.AddRange(download.GetBytesFromAllParts()),
                fileResume);
        }

        public void PauseDownload(Model.Download download)
        {
            download.FileRenameCancelToken.Cancel();
            _cancellationTokenSource.Cancel();
        }


        public async void DownloadFile(Model.Download download)
        {
            download.FileRenameCancelToken = new FileRenameCancelToken();
            var fileRenameStreamer = new NewFileRenameStreamer(download.TargetPathWithFileName, download.FileRenameCancelToken);
            download.TempFileName = fileRenameStreamer.GetNewTempFileWithPath();

            await RunDownload(
                download,
                request => request.AddRange(0),
                fileRenameStreamer);
        }

        private async Task RunDownload(Model.Download download, Action<HttpWebRequest> configureRequest,FileRenameStreamer fileRenameStreamer)
        {
            try
            {                
                _cancellationTokenSource = new CancellationTokenSource();

                var request = (HttpWebRequest) WebRequest.Create(new Uri(download.SourcePath));

                configureRequest(request);
                
                using (var response = await request.GetResponseAsync())
                {
                    fileRenameStreamer.SetContentLength(response.ContentLength);
                    
                    var data = response.GetResponseStream();

                    using (fileRenameStreamer)
                    {
                        var byteBuffer = fileRenameStreamer.GetByteBuffer();
                        fileRenameStreamer.SetStreamPosition(download.GetBytesFromAllParts());
                        download.TotalFileSize = fileRenameStreamer.GetTotalFileSize();

                        await MakeDownload(download, data, byteBuffer, _cancellationTokenSource.Token, fileRenameStreamer);
                    }
                }
            }
            catch (IOException)
            {
                DownloadCancel?.Invoke(this, new MyDownloadEventArgs());
            }
            catch (OperationCanceledException)
            {
                if (download.State == CurrentDownloadState.Cancel)
                    DeleteDownload(download);
                else if (download.State == CurrentDownloadState.Pause)
                    DownloadPause?.Invoke(this, new MyDownloadEventArgs());
                else
                    DeleteDownload(download);

            }
            catch (WebException)
            {
                DownloadCancel?.Invoke(this, new MyDownloadEventArgs());
            }
        }

        private void CompleteDownload(Model.Download download)
        {
            DownloadComplete?.Invoke(this, new MyDownloadEventArgs());
            download.FileRenameCancelToken = null;                        
        }
        private void DeleteDownload(Model.Download download)
        {
            DownloadCancel?.Invoke(this, new MyDownloadEventArgs());
        }

        private async Task MakeDownload(Model.Download download, Stream data, byte[] byteBuffer, CancellationToken cancellationToken, FileRenameStreamer fileRenameStreamer)
        {            
            using (var output = fileRenameStreamer.GetFileStream())
            {
                var onProgressChanged = new Progress<MyProgress>(OnDownloadProgressChanged);
                var oldPercentDone = 0.0;
                var bytesRead = 0;
                
                download.StartDownloadPart();
                do
                {                   
                    bytesRead = await data.ReadAsync(byteBuffer, 0, byteBuffer.Length, cancellationToken);

                    download.AddBytesToPart(bytesRead);
                    fileRenameStreamer.AddCurrentBytesRead(bytesRead);

                    if (bytesRead <= 0)
                        continue;

                    await output.WriteAsync(byteBuffer, 0, bytesRead, cancellationToken);
                    
                    oldPercentDone = SendDownloadProgress(onProgressChanged, download, oldPercentDone);
                } while (bytesRead > 0);

                download.FinishDownloadPart();
                download.DumpLogFromAllParts();

                CompleteDownload(download);
            }
        }

        private static double SendDownloadProgress(IProgress<MyProgress> onProgressChanged, Model.Download download, double oldPercentDone)
        {            
            var myProgress = new MyProgress()
            {
                CurrentFileSize =  download.GetBytesFromAllParts(),
                TotalFileSize = download.TotalFileSize
            };            

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
