using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ÜbungWPFDownloadTool.BusinessLayer.Download
{
    public class FileRenameCancelToken
    {
        public bool IsCanceld { get; private set; }
        public void Cancel()
        {
            IsCanceld = true;
        }
    }

    public class FileRenameStreamer : IDisposable
    {                
        const int BufferSize = 64 * 1024;
        private FileStream _fileStream;
        private string _tempFileNameWithPath;
        private readonly string _targetPathWithFileName;
        private readonly FileRenameCancelToken _cancelToken;
        private readonly byte[] _byteBuffer;
        private long _fileStreamOffset = 0;
        private long _contentLenght;
        private long _currentBytesRead;

        public FileRenameStreamer(string targetPathWithFileName, FileRenameCancelToken cancelToken = null)
        {
            _targetPathWithFileName = targetPathWithFileName;
            _cancelToken = cancelToken;
            _byteBuffer = new byte[BufferSize];
        }

        public FileStream GetFileStream()
        {
            // tempfile existiert
            if (!string.IsNullOrEmpty(_tempFileNameWithPath))
            {                                
                _fileStream = new FileStream(GetNewTempFileWithPath(), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite, BufferSize, useAsync: true);
                _fileStream.Seek(_fileStreamOffset, SeekOrigin.Begin);                
                return _fileStream;
            }

            _fileStream = new FileStream(GetNewTempFileWithPath(), FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write, BufferSize,useAsync: true);
            return _fileStream;
        }

        public void RenameFileToOriginal()
        {
            if (_fileStream == null) return;

            if (File.Exists(GetOriginalFileWithPath()))
                File.Delete(GetOriginalFileWithPath());

            File.Move(GetNewTempFileWithPath(), GetOriginalFileWithPath());
        }

        public void SetStreamPosition(long offset)
        {
            _fileStreamOffset = offset;
        }

        public void SetContentLength(long contentLenght)
        {
            _contentLenght = contentLenght;
        }


        public string GetNewTempFileWithPath()
        {
            if (_tempFileNameWithPath == null)
            _tempFileNameWithPath = Path.GetTempFileName();
            Debug.WriteLine(_tempFileNameWithPath);
            return _tempFileNameWithPath;
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

        public void setCurrentBytesRead(long bytes)
        {
            _currentBytesRead = bytes;
        }

        public void AddCurrentBytesRead(long bytes)
        {
            _currentBytesRead += bytes;
        }

        public long GetTotalFileSize()
        {
            return _fileStreamOffset + _contentLenght;
        }


        public void Dispose()
        {
            if (_cancelToken != null)
            {
                if (_cancelToken.IsCanceld == false)
                {
                    if (File.Exists(GetOriginalFileWithPath()))
                        File.Delete(GetOriginalFileWithPath());

                    File.Move(GetNewTempFileWithPath(), GetOriginalFileWithPath());
                }
            }
            else
            {
                if (File.Exists(GetOriginalFileWithPath()))
                    File.Delete(GetOriginalFileWithPath());

                File.Move(GetNewTempFileWithPath(), GetOriginalFileWithPath());
            }


            _fileStream?.Dispose();
        }
    }
}
