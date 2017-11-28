using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using WPFDownloadTool.BusinessLayer;
using WPFDownloadTool.BusinessLayer.Download;
using WPFDownloadTool.BusinessLayer.Show;
using WPFDownloadTool.BusinessLayer.Url;
using WPFDownloadTool.Model;

namespace WPFDownloadTool.ViewModels
{
    public class DownloaderViewModel : Screen
    {
        private readonly ISelectFile _selectFile;
        private readonly IShow _show;        
        private readonly ICreateEngine _createEngine;
        private readonly IUrlService _urlService;

        public DownloaderViewModel(ISelectFile selectFile, IShow show, IUrlService urlservice, ICreateEngine createEngine)
        {         
            _selectFile = selectFile;
            _show = show;            
            _createEngine = createEngine;
            _urlService = urlservice;
        }

        protected override void OnInitialize()
        {
            DisplayName = "Downloader Tool";
            SelectedEngine = Engine.HttpWeb;            
        }

        public ObservableCollection<DownloadViewModel> Downloads { get; } = new ObservableCollection<DownloadViewModel>();

        public string Url { get; set; }

        public bool AreAddFilesPossible => _urlService.IsUrlValid(Url);

        public bool AreDownloadStartpossible { get; set; }
        public bool AreDownloadDetailsShown { get; set; }
        public bool AreDownloadListShow { get; set; }

        public double TotalDownloadProgress { get; set; }
        public double TotalDownloadSpeed { get; set; }

        public int FilesToDownload { get; set; }
        private int FilesDownloaded;
        public int TotalFilesToDownload { get; set; }        
        public Engine SelectedEngine { get; set; }
        
        public async void AddDownload()
        {
            var validFileName = await _urlService.GetValideDownloadFile(Url);
            if (validFileName == null)
            {
                _show.Message(Ressources.DownloaderViewModel.WrongURL);
                return;
            }
            
            var download = Download.NewDownload(_selectFile.ShowSaveFileDialog(Path.GetFileName(validFileName)), Url);
            if (download == null)
            {
                _show.Message(Ressources.DownloaderViewModel.ErrorCreateDownload);
                return;
            }
            var neu = new DownloadViewModel(download,_createEngine.For(SelectedEngine));            
            AreDownloadListShow = true;
            AreDownloadStartpossible = true;
            Downloads.Add(neu);                   
        }


        public void StartDownload()
        {
            if (!AreDownloadStartpossible)
                return;

            // Downloads starten
            AreDownloadStartpossible = false;
            AreDownloadDetailsShown = true;            

            foreach (var downloadInList in Downloads)
            {
                if (downloadInList.Download != null && downloadInList.Download.State != CurrentDownloadState.Finish)
                {                    
                    downloadInList.DownloadFile();
                    
                    downloadInList.DownloadComplete += DownloadComplete;
                    downloadInList.DownloadProgressChanged += DownloadProgressChanged;
                    downloadInList.DownloadCancel += ShowCancelDialoge;
                    downloadInList.DownloadPause += ShowPauseDialoge;
                }
            }

            SetTotalFilesProgress();
            SetFilesToDownloadProgress();            
        }

        private void ShowPauseDialoge(object sender, MyDownloadEventArgs args)
        {
            _show.Message(Ressources.DownloaderViewModel.PauseDownload);
        }

        private void ShowCancelDialoge(object sender, MyDownloadEventArgs args)
        {
            _show.Message(Ressources.DownloaderViewModel.CancelDownload);
        }

        private void DownloadProgressChanged(object sender, MyDownloadEventArgs eventArgs)
        {
            TotalDownloadSpeed =+ Downloads.Select(x => x)
                .Where(x => x.Download.State == CurrentDownloadState.Download)
                .Select(x => x.GetBytesPerSecondAsUnit()).Sum();
                       
        }

        private void SetFilesToDownloadProgress()
        { 
            FilesToDownload = Downloads.Count(x => x.Download.State == CurrentDownloadState.Download);            
        }

        private void SetTotalFilesProgress()
        {
            TotalFilesToDownload = Downloads.Count();
            
        }

        private void DownloadComplete(object sender,MyDownloadEventArgs eventArgs)
        {
            FilesDownloaded++;
            SetFilesToDownloadProgress();
            TotalDownloadProgress = FilesDownloaded / TotalFilesToDownload;

            if (Downloads.All(x => x.Download.State == CurrentDownloadState.Finish))
            {
                AllDownloadsComplete();
            }
        }

        private void AllDownloadsComplete()
        {
            FilesToDownload = 0;
            AreDownloadStartpossible = false;
            AreDownloadDetailsShown = false;
            AreDownloadListShow = false;

            Downloads.Clear();
        }

        public void CancelDownloads()
        {
            AreDownloadStartpossible = true;
            AreDownloadDetailsShown = false;

            Downloads
                .Select(x => x)
                .Where(x => x.Download.State == CurrentDownloadState.Download || x.Download.State == CurrentDownloadState.Pause)
                .ToList()                
                .ForEach(x => x.CancelDownload());

            foreach (var downloadViewModel in Downloads)
            {
                if (downloadViewModel.Download.State == CurrentDownloadState.Cancel)
                {
                    downloadViewModel.DownloadComplete -= DownloadComplete;
                    downloadViewModel.DownloadProgressChanged -= DownloadProgressChanged;
                    downloadViewModel.DownloadCancel -= ShowCancelDialoge;
                    downloadViewModel.DownloadPause -= ShowPauseDialoge;                    
                }                
            }

            SetTotalFilesProgress();
        }


        public void PauseDownloads()
        {

        }

    }
}
