using System;
using System.Drawing.Text;
using System.IO;

namespace WPFDownloadTool.BusinessLayer.Download
{
    public abstract class FileRenameStreamer : IDisposable
    {
        protected string TempFileNameWithPath;
        protected long FileStreamOffset = 0;

        private readonly FileRenameCancelToken _cancelToken;        
        private readonly string _targetPathWithFileName;        
        private int BufferSize = 64 * 1024;
        private FileStream _fileStream;         
        private readonly byte[] _byteBuffer;  
        private long _contentLenght;
        private long _currentBytesRead;        

        public FileRenameStreamer(string targetPathWithFileName, FileRenameCancelToken cancelToken)
        {
            _targetPathWithFileName = targetPathWithFileName;
            _cancelToken = cancelToken ?? new FileRenameCancelToken();
            _byteBuffer = new byte[BufferSize];
        }

        protected virtual void SetFileStreamConfig(FileStream stream)
        {
        }

        public FileStream GetFileStream()
        {
            _fileStream = new FileStream(GetNewTempFileWithPath(), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite, BufferSize, useAsync: true);
            SetFileStreamConfig(_fileStream);

            return _fileStream;
        }

        private void RenameFileToOriginal()
        {
            if (_fileStream == null) return;

            if (File.Exists(GetOriginalFileWithPath()))
                File.Delete(GetOriginalFileWithPath());

            File.Move(GetNewTempFileWithPath(), GetOriginalFileWithPath());
        }
        
        public void SetStreamPosition(long offset)
        {
            FileStreamOffset = offset;
        }

        public void SetContentLength(long contentLenght)
        {
            _contentLenght = contentLenght;
        }

        protected abstract string GetInternalNewTempFileWithPath();
        public string GetNewTempFileWithPath()
        {
            return GetInternalNewTempFileWithPath();            
        }

        private string GetOriginalFileWithPath()
        {
            return _targetPathWithFileName;
        }

        public byte[] GetByteBuffer()
        {
            return _byteBuffer;
        }

        public long GetCurrentBytesRead()
        {
            return _currentBytesRead;
        }

        public void SetCurrentBytesRead(long bytes)
        {
            _currentBytesRead = bytes;
        }

        public void AddCurrentBytesRead(long bytes)
        {
            _currentBytesRead += bytes;
        }


        public long GetTotalFileSize()
        {
            return FileStreamOffset + _contentLenght;
        }
        
        public void Dispose()
        {
            if (!_cancelToken.IsCanceld)
            {
                if (File.Exists(_targetPathWithFileName))
                    File.Delete(_targetPathWithFileName);

                File.Move(GetNewTempFileWithPath(), _targetPathWithFileName);
            }

            _fileStream?.Dispose();
        }

        
    }
}
