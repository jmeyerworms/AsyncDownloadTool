using System.IO;

namespace WPFDownloadTool.BusinessLayer.Download
{
    public class NewFileRenameStreamer : FileRenameStreamer
    {
        public NewFileRenameStreamer(string targetPathWithFileName, FileRenameCancelToken cancelToken)
            : base(targetPathWithFileName, cancelToken)
        {
        }

        protected override string GetInternalNewTempFileWithPath()
        {
            if (TempFileNameWithPath == null)
                TempFileNameWithPath = Path.GetTempFileName();

            return TempFileNameWithPath;
        }


    }
}