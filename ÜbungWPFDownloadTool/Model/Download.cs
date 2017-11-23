using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ÜbungWPFDownloadTool.ViewModels;

namespace ÜbungWPFDownloadTool.Model
{
    public class Download
    {
        private Download()
        {
        }

        public long TotalFileSize { get; set; }
        public long CurrentFileSize { get; set; }

        public string TargetPathWithFileName { get; set; }
        public string TargetFileName { get; set; }
        public string TargetPath { get; private set; }
        public string SourcePath { get; private set; }

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
    }
}
