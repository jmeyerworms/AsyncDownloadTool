using System;
using System.Diagnostics;
using Caliburn.Micro;
using FontAwesome.WPF;
using WPFDownloadTool.BusinessLayer;
using WPFDownloadTool.BusinessLayer.Download;
using WPFDownloadTool.Model;

namespace WPFDownloadTool.ViewModels
{
    public class DownloadViewModel : Screen
    {
        public double CurrentProgress { get; set; }
        public string DownloadSpeedAsString { get; set; }
        public double DownloadSpeed { get; set; }
        public string DownloadName { get; set; }
        public FontAwesomeIcon PauseIcon { get; set; } = FontAwesomeIcon.Pause;

        private readonly IDownloadService _downloadService;
        private DownloadProgressTracker DownloadTracker { get; set; }

        public Download Download { get; set; }
        public event EventHandler<MyDownloadEventArgs> DownloadComplete;
        public event EventHandler<MyDownloadEventArgs> DownloadProgressChanged;
        public event EventHandler<MyDownloadEventArgs> DownloadCancel;
        public event EventHandler<MyDownloadEventArgs> DownloadPause;

        public DownloadViewModel(Download download,IDownloadService downloadService)
        {
            if (download == null) return;
            
            this.Download = download;
            download.State = CurrentDownloadState.Stop;            
            this.DownloadName = download.TargetFileName;
            this._downloadService = downloadService;
            DownloadTracker = new DownloadProgressTracker(50, TimeSpan.FromMilliseconds(500));
            
            _downloadService.DownloadProgressChanged += DownloadServiceOnDownloadProgressChanged;
            _downloadService.DownloadComplete += DownloadServiceOnDownloadComplete;
            _downloadService.DownloadCancel += DownloadServiceOnDownloadCancel;
            _downloadService.DownloadPause += DownloadServiceOnDownloadPause;
        }

        private void DownloadServiceOnDownloadPause(object sender, MyDownloadEventArgs myDownloadEventArgs)
        {
            DownloadPause?.Invoke(sender, myDownloadEventArgs);
        }

        private void DownloadServiceOnDownloadProgressChanged(object sender, MyDownloadEventArgs myDownloadEventArgs)
        {            
            DownloadTracker.SetProgress(myDownloadEventArgs.CurrentFileSize, myDownloadEventArgs.TotalFileSize);

            DownloadSpeed = DownloadTracker.GetBytesPerSecond();
            DownloadSpeedAsString = DownloadTracker.GetBytesPerSecondString();
            CurrentProgress = DownloadTracker.GetProgress();

            DownloadProgressChanged?.Invoke(sender, myDownloadEventArgs);
        }

        private void DownloadServiceOnDownloadComplete(object sender, MyDownloadEventArgs myDownloadEventArgs)
        {
            CurrentProgress = 1.0;
            Download.State = CurrentDownloadState.Finish;
            Debug.WriteLine("Download finisch");            

            DownloadComplete?.Invoke(sender, myDownloadEventArgs);
        }

        private void DownloadServiceOnDownloadCancel(object sender, MyDownloadEventArgs myDownloadEventArgs)
        {
            DownloadTracker.NewFile();
            Download.State = CurrentDownloadState.Cancel;
            Debug.WriteLine("Download cancel");

            DownloadCancel?.Invoke(sender, myDownloadEventArgs);
        }

        public double GetBytesPerSecondAsUnit() => DownloadTracker.GetBytesPerSecondAsUnit();

        public void DownloadFile()
        {
            Download.State = CurrentDownloadState.Download;
            _downloadService.DownloadFile(Download);
        }

        public void CancelDownload()
        {
            Download.State = CurrentDownloadState.Cancel;
            _downloadService.CancelDownload(Download);            
        }

        public void PauseDownload()
        {            
            if (Download.State == CurrentDownloadState.Pause)
            {
                ResumeDownload();
                return;
            }

            PauseIcon = FontAwesomeIcon.Play;
            Download.State = CurrentDownloadState.Pause;

            _downloadService.PauseDownload(Download);                         
        }

        private void ResumeDownload()
        {
            PauseIcon = FontAwesomeIcon.Pause;
            Download.State = CurrentDownloadState.Download;

            _downloadService.ResumeDownload(Download);                            
        }
 
    }
}
