using System.IO;

namespace WPFDownloadTool.BusinessLayer.Download
{
    public class FileResumeStreamer : FileRenameStreamer
    {
        private readonly string _tempfile;        
        public FileResumeStreamer(string tempfile, string targetPathWithFileName, FileRenameCancelToken cancelToken)
            : base(targetPathWithFileName, cancelToken)
        {
            _tempfile = tempfile;
        }

        protected override FileStream GetInternalFileStream()
        {
            var fileStream = new FileStream(GetNewTempFileWithPath(), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite, BufferSize, useAsync: true);
            fileStream.Seek(FileStreamOffset, SeekOrigin.Begin);
            return fileStream;
        }

        protected override string GetInternalNewTempFileWithPath()
        {
            return _tempfile;
        }

        protected override void OnCleanup()
        {
            if (!_cancelToken.IsCanceld)
            {
                if (File.Exists(_targetPathWithFileName))
                    File.Delete(_targetPathWithFileName);

                File.Move(GetNewTempFileWithPath(), _targetPathWithFileName);
            }
        }
    }
}