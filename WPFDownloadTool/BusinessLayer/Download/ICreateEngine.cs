namespace ÜbungWPFDownloadTool.BusinessLayer.Download
{
    public interface ICreateEngine
    {
        IDownloadService For(Engine engine);
    }
}