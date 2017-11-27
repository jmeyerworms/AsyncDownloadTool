using System.Threading.Tasks;

namespace WPFDownloadTool.BusinessLayer.Url
{
    public interface IUrlService
    {        
        Task<string> GetValideDownloadFile(string url);
        bool IsUrlValid(string url);
    }
}