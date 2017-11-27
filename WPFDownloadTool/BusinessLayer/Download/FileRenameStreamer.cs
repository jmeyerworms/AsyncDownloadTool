using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ÜbungWPFDownloadTool.BusinessLayer.Download
{
    public abstract class FileRenameStreamer : IDisposable
    {
        protected string TempFileNameWithPath;
        protected readonly string _targetPathWithFileName;
        protected long FileStreamOffset = 0;
        protected int BufferSize = 64 * 1024;
        protected readonly FileRenameCancelToken _cancelToken;


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

        protected abstract FileStream GetInternalFileStream();
        public FileStream GetFileStream()
        {
            _fileStream = GetInternalFileStream();

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


        public string GetNewTempFileWithPath()
        {
            if (TempFileNameWithPath == null)
            TempFileNameWithPath = Path.GetTempFileName();
            
            return TempFileNameWithPath;
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

        protected abstract void OnCleanup();
        public void Dispose()
        {
            OnCleanup();
            _fileStream?.Dispose();
        }

        
    }
}
