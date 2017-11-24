using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Ink;
using Caliburn.Micro;
using Microsoft.Win32;
using ÜbungWPFDownloadTool.BusinessLayer;
using ÜbungWPFDownloadTool.BusinessLayer.Download;
using ÜbungWPFDownloadTool.Model;
using FontAwesome.WPF;

namespace ÜbungWPFDownloadTool.ViewModels
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

        public DownloadViewModel(Download download,IDownloadService downloadService)
        {
            if (download == null) return;
            
            this.Download = download;
            download.State = CurrentDownloadState.Stop;            
            this.DownloadName = download.TargetFileName;
            this._downloadService = downloadService;
            DownloadTracker = new DownloadProgressTracker(50, TimeSpan.FromMilliseconds(500));

            _downloadService.DownloadComplete += DownloadServiceOnDownloadComplete;
            _downloadService.DownloadProgressChanged += DownloadServiceOnDownloadProgressChanged;
            _downloadService.DownloadCancel += DownloadServiceOnDownloadCancel;
        }

        private void DownloadServiceOnDownloadProgressChanged(object sender, MyDownloadEventArgs myDownloadEventArgs)
        {
            
            DownloadTracker.SetProgress(Download.GetBytesFromAllParts(), Download.TotalFileSize);

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
            Download.FileRenameCancelToken = null;
            Download.FileRenameStreamer = null;

            DownloadComplete?.Invoke(sender, myDownloadEventArgs);
        }

        private void DownloadServiceOnDownloadCancel(object sender, MyDownloadEventArgs myDownloadEventArgs)
        {
            DownloadTracker.NewFile();
            Download.State = CurrentDownloadState.Cancel;
            //CurrentProgress = 0;
            //Download.CurrentFileSize = 0;
            //Download.TotalFileSize = 0;
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
            _downloadService.CancelDownload(Download);

            Download.State = CurrentDownloadState.Cancel;
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

            try
            {
                _downloadService.CancelDownload(Download);                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }            
        }

        private void ResumeDownload()
        {
            PauseIcon = FontAwesomeIcon.Pause;
            Download.State = CurrentDownloadState.Download;

            try
            {
                _downloadService.DownloadFile(Download);                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
        }
 
    }
}
