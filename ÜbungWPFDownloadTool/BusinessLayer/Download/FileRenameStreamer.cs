using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ÜbungWPFDownloadTool.BusinessLayer.Download
{
    public class FileRenameStreamer : IDisposable
    {                
        const int BufferSize = 64 * 1024;
        private FileStream _fileStream;
        private string _tempFileNameWithPath;
        private readonly string _targetPathWithFileName;
        private readonly byte[] _byteBuffer;

        public FileRenameStreamer(string targetPathWithFileName)
        {
            _targetPathWithFileName = targetPathWithFileName;
            _byteBuffer = new byte[BufferSize];
        }

        public FileStream GetFileStream()
        {            
            _fileStream = new FileStream(GetTempFileWithPath(), FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write, BufferSize,
                useAsync: true);
            return _fileStream;
        }

        private string GetTempFileWithPath()
        {
            if (_tempFileNameWithPath == null)
            _tempFileNameWithPath = Path.GetTempFileName();

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
        public void RenameFileToOriginal()
        {
            if (_fileStream == null) return;

            if (File.Exists(GetOriginalFileWithPath()))
                File.Delete(GetOriginalFileWithPath());
            
            File.Move(GetTempFileWithPath(), GetOriginalFileWithPath());
        }

        public void Dispose()
        {
            if (File.Exists(GetOriginalFileWithPath()))
                File.Delete(GetOriginalFileWithPath());

            File.Move(GetTempFileWithPath(), GetOriginalFileWithPath());

            _fileStream?.Dispose();
        }
    }
}
