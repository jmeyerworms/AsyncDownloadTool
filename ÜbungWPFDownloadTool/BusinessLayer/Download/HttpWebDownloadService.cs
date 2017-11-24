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
        private IProgress<MyProgress> _onProgressChanged;
        
        public event EventHandler<MyDownloadEventArgs> DownloadComplete;
        public event EventHandler<MyDownloadEventArgs> DownloadProgressChanged;
        public event EventHandler<MyDownloadEventArgs> DownloadCancel;        

        public Engine Engine => Engine.HttpWeb;
        
        public void CancelDownload(Model.Download download)
        {
            download.FileRenameCancelToken.Cancel();
            _cancellationTokenSource.Cancel();
        }

        private void StartnewDownload(Model.Download download)
        {            
            download.FileRenameStreamer = new FileRenameStreamer(download.TargetPathWithFileName, download.FileRenameCancelToken);            
        }
        

        public async void DownloadFile(Model.Download download)
        {
            try
            {
                download.FileRenameCancelToken = new FileRenameCancelToken();
                _cancellationTokenSource = new CancellationTokenSource();
                _cancellationToken = _cancellationTokenSource.Token;

                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(new Uri(download.SourcePath));
                if (download.FileRenameStreamer != null && download.FileRenameCancelToken != null && download.FileRenameCancelToken.IsCanceld)
                {                 
                    request.AddRange(download.GetBytesFromAllParts());                    
                }
                else
                {
                    StartnewDownload(download);
                }

                _onProgressChanged = new Progress<MyProgress>(OnDownloadProgressChanged);
                
                var OldPercentDone = 0;


                //using (var response = (HttpWebResponse)await request.GetResponseAsync())
                using (var response = (HttpWebResponse) await request.GetResponseAsync())
                {
                    download.FileRenameStreamer.SetContentLength(response.ContentLength);
                    var data = response.GetResponseStream();

                    using (download.FileRenameStreamer)
                    {
                        var byteBuffer = download.FileRenameStreamer.GetByteBuffer();
                        download.FileRenameStreamer.SetStreamPosition(download.GetBytesFromAllParts());

                        using (var output = download.FileRenameStreamer.GetFileStream())
                        {
                            var bytesRead = 0;
                            download.StartDownloadPart();

                            do
                            {
                                try
                                {
                                    bytesRead = await data.ReadAsync(byteBuffer, 0, byteBuffer.Length,
                                        _cancellationToken);
                                }
                                catch (Exception)
                                {
                                    DownloadCancel?.Invoke(this, new MyDownloadEventArgs());
                                    return;
                                }

                                download.AddBytesToPart(bytesRead);
                                download.FileRenameStreamer.AddCurrentBytesRead(bytesRead);
                                
                                if (bytesRead <= 0)
                                    continue;
                                await output.WriteAsync(byteBuffer, 0, bytesRead, _cancellationToken);

                                SendDownloadProgress(download, OldPercentDone);

                            } while (bytesRead > 0);

                            download.FinishDownloadPart();
                            download.GetLogFromAllParts();

                            DownloadComplete?.Invoke(this, new MyDownloadEventArgs());
                        }

                    }
                }
            }
            catch (OperationCanceledException)
            {
                DownloadCancel?.Invoke(this, new MyDownloadEventArgs());
            }
            catch (WebException)
            {
                DownloadCancel?.Invoke(this, new MyDownloadEventArgs());
            }
        }

        private void SendDownloadProgress(Model.Download download,double oldPercentDone)
        {            
            var myProgress = new MyProgress()
            {
                CurrentFileSize = download.FileRenameStreamer.GetCurrentBytesRead(),
                TotalFileSize = download.FileRenameStreamer.GetTotalFileSize()
            };            
            download.TotalFileSize = download.FileRenameStreamer.GetTotalFileSize();

            var percentDone = (double)myProgress.CurrentFileSize / myProgress.TotalFileSize;

            if (!(percentDone - oldPercentDone > 0.05)) return;

            oldPercentDone = percentDone;
            _onProgressChanged?.Report(myProgress);
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
