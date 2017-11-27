using System;
using System.IO;

namespace ÜbungWPFDownloadTool.BusinessLayer.Download
{
    public class FileResumeStreamer : FileRenameStreamer
    {
        public FileResumeStreamer(string tempfile, string targetPathWithFileName, FileRenameCancelToken cancelToken)
            : base(targetPathWithFileName, cancelToken)
        {
            TempFileNameWithPath = tempfile;
        }

        protected override FileStream GetInternalFileStream()
        {
            var fileStream = new FileStream(TempFileNameWithPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite, BufferSize, useAsync: true);
            fileStream.Seek(FileStreamOffset, SeekOrigin.Begin);
            return fileStream;
        }
       
        protected override void OnCleanup()
        {
            if (!_cancelToken.IsCanceld)
            {
                if (File.Exists(_targetPathWithFileName))
                    File.Delete(_targetPathWithFileName);

                File.Move(TempFileNameWithPath, _targetPathWithFileName);
            }
        }
    }
}