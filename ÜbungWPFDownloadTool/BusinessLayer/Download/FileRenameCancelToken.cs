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
}