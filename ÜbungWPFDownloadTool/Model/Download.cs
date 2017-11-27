using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ÜbungWPFDownloadTool.BusinessLayer.Download;
using ÜbungWPFDownloadTool.ViewModels;

namespace ÜbungWPFDownloadTool.Model
{
    public class Download : DownloadParts
    {
        private Download()
        {
        }

        public long TotalFileSize { get; set; }
        private readonly List<DownloadParts> _downloadParts = new List<DownloadParts>();
        public string TargetPathWithFileName { get; set; }
        public string TargetFileName { get; set; }
        public string TempFileName { get; set; }
        public string TargetPath { get; private set; }
        public string SourcePath { get; private set; }
        public FileRenameCancelToken FileRenameCancelToken { get; set; }

        public CurrentDownloadState State { get; set; }

        public static Download NewDownload(string targetPath,string sourchePath)
        {
            if (string.IsNullOrEmpty(targetPath) || string.IsNullOrEmpty(sourchePath)) return null;

            return new Download()
            {
                TargetPathWithFileName = targetPath,
                TargetFileName = Path.GetFileName(targetPath),
                TargetPath = Path.GetDirectoryName(targetPath) + @"\",
                SourcePath = sourchePath,
                State = CurrentDownloadState.Stop
            };
        }

        public void StartDownloadPart()
        {
            long offset = 0;

            var lastPartNotFinished = _downloadParts.LastOrDefault(x => x.Finished == false);
            if (lastPartNotFinished != null) offset = lastPartNotFinished.Bytes;

            Debug.WriteLine("StartDownloadPart offset" + offset);
            _downloadParts.Add(new DownloadParts(){ Offset = offset });            
        }

        public void AddBytesToPart(long bytes)
        {
            if (_downloadParts.Count == 0) return;

            _downloadParts.Last().Bytes += bytes;
            //Debug.WriteLine("AddBytesToPart .Bytes: " + _downloadParts.Last().Bytes);
        }

        public void FinishDownloadPart()
        {
            if (_downloadParts.Count == 0) return;            
            _downloadParts.Last().Finished = true;

            Debug.WriteLine("FinishDownloadPart: finisch part");
        }

        public long GetBytesFromAllParts()
        {
          //  Debug.WriteLine("GetBytesFromAllParts:" + _downloadParts.Sum(x => x.Bytes));
            return _downloadParts.Sum(x => x.Bytes);
        }

        public void DumpLogFromAllParts()
        {
            foreach (var part in _downloadParts)
            {
                Debug.WriteLine("part offset " + part.Offset + "bytes " + part.Bytes + " finished " + part.Finished);
            }
        }
    }
}
