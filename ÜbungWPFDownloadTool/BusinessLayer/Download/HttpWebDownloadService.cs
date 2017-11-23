using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
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
        
        public void CancelDownload()
        {
            _cancellationTokenSource.Cancel();
        }

        public async void DownloadFile(Model.Download download)
        {            
            try
            {                
                _cancellationTokenSource = new CancellationTokenSource();
                _cancellationToken = _cancellationTokenSource.Token;

                _onProgressChanged = new Progress<MyProgress>(OnDownloadProgressChanged);

                var request = WebRequest.Create(new Uri(download.SourcePath));
                request.Method = WebRequestMethods.Http.Get;

                var currentBytesRead = download.CurrentFileSize;
                var oldPercentDone = 0.0;

                using (var response = (HttpWebResponse)await request.GetResponseAsync())
                {
                    var data = response.GetResponseStream();
                                        
                    using (var file = new FileRenameStreamer(download.TargetPathWithFileName))
                    {
                        var byteBuffer = file.GetByteBuffer();

                        using (var output = file.GetFileStream())
                        {
                            var bytesRead = 0;
                            do
                            {                               
                               bytesRead = await data.ReadAsync(byteBuffer, 0, byteBuffer.Length, _cancellationToken);
                                    currentBytesRead += bytesRead;
                                    if (bytesRead <= 0)
                                        continue;
                                    await output.WriteAsync(byteBuffer, 0, bytesRead, _cancellationToken);                                  
                                var myProgress = new MyProgress() { CurrentFileSize = currentBytesRead, TotalFileSize = response.ContentLength };
                                var percentDone = (double)myProgress.CurrentFileSize / myProgress.TotalFileSize;
                                if (!(percentDone - oldPercentDone > 0.05))
                                    continue;
                                oldPercentDone = percentDone;
                                _onProgressChanged?.Report(myProgress);   
                            }
                            while (bytesRead > 0);

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
