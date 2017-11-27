using System.IO;

namespace WPFDownloadTool.BusinessLayer.Download
{
    public class NewFileRenameStreamer : FileRenameStreamer
    {
        public NewFileRenameStreamer(string targetPathWithFileName, FileRenameCancelToken cancelToken)
            : base(targetPathWithFileName, cancelToken)
        {
        }

        protected override FileStream GetInternalFileStream()
        {
            var fileStream = new FileStream(TempFileNameWithPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite, BufferSize, useAsync: true);            
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