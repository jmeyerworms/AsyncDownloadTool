using System;
using System.Net;
using System.Threading.Tasks;

namespace WPFDownloadTool.BusinessLayer.Url
{
    public class UrlService : IUrlService
    {
        public async Task<string> GetValideDownloadFile(string url)
        {
            var valideDownloadPath = await GetValideDownloadPath(url);            
            if (ValidateUrlAndGetUri(valideDownloadPath) != null)
            {
                return valideDownloadPath;
            }
            return null;
        }

        private async Task<string> GetValideDownloadPath(string url)
        {
            try
            {
                var request = await WebRequest.Create(url).GetResponseAsync();
                return request.ResponseUri.AbsoluteUri;
            }
            catch (WebException)
            {
                return null;
            }
            
        }

        public bool IsUrlValid(string url) => ValidateUrlAndGetUri(url) != null;

        private Uri ValidateUrlAndGetUri(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                if (!string.IsNullOrEmpty(url))
                {
                    return TryGetUri(url);
                }
            }

            return null;
        }
         
        private Uri TryGetUri(string url)
        {
            try
            {
                return new Uri(url);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}