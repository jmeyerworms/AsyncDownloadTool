using System;
using System.Threading.Tasks;

namespace ÜbungWPFDownloadTool.BusinessLayer.Url
{
    public interface IUrlService
    {        
        Task<string> GetValideDownloadFile(string url);
        bool IsUrlValid(string url);
    }
}