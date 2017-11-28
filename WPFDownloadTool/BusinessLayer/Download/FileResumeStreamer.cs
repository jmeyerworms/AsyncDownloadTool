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

        protected override void SetFileStreamConfig(FileStream stream)
        {
            stream.Seek(FileStreamOffset, SeekOrigin.Begin);            
        }

        protected override string GetInternalNewTempFileWithPath()
        {
            return _tempfile;
        }
    }
}