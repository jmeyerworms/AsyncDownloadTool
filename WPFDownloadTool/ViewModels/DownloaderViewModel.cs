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

        public string Url { get; set; } = @"http://ipv4.download.thinkbroadband.com/100MB.zip";

        public bool AreAddFilesPossible => _urlService.IsUrlValid(Url);

        public bool AreDownloadStartpossible { get; set; }
        public bool AreDownloadDetailsShown { get; set; }
        public bool AreDownloadListShow { get; set; }

        public int TotalDownloadProgress { get; set; }
        public int ToTalFilesToDownload { get; set; }
        public double TotalDownloadSpeed { get; set; }

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
                    downloadInList.DownloadCancel += (sender, args) => _show.Message(Ressources.DownloaderViewModel.CancelDownload);
                    downloadInList.DownloadPause += (sender, args) => _show.Message(Ressources.DownloaderViewModel.PauseDownload);
                }
            }

            SetTotalFilesProgress();
        }

        private void DownloadProgressChanged(object sender, MyDownloadEventArgs eventArgs)
        {
            TotalDownloadSpeed =+ Downloads.Select(x => x)
                .Where(x => x.Download.State == CurrentDownloadState.Download)
                .Select(x => x.GetBytesPerSecondAsUnit()).Sum();
        }

        private void SetTotalFilesProgress()
        {
            ToTalFilesToDownload = Downloads.Count(x => x.Download.State == CurrentDownloadState.Download);
        }

        private void DownloadComplete(object sender,MyDownloadEventArgs eventArgs)
        {
            SetTotalFilesProgress();

            if (Downloads.All(x => x.Download.State == CurrentDownloadState.Finish))
            {
                AllDownloadsComplete();
            }
        }

        private void AllDownloadsComplete()
        {
            ToTalFilesToDownload = 0;
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
                .Where(x => x.Download.State == CurrentDownloadState.Download)
                .ToList()
                .ForEach(x => x.CancelDownload());

            SetTotalFilesProgress();
        }


        public void PauseDownloads()
        {

        }

    }
}
