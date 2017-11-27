using System;
using System.Collections.Generic;
using System.Linq;

namespace ÜbungWPFDownloadTool.BusinessLayer.Download
{
    public class CreateEngine : ICreateEngine
    {
        private readonly IEnumerable<Func<IDownloadService>> _createDownloadServices;

        public CreateEngine(IEnumerable<Func<IDownloadService>> createDownloadServices)
        {
            _createDownloadServices = createDownloadServices;
        }
        public IDownloadService For(Engine engine)
        {
            return _createDownloadServices.Select(x => x()).Single(x => x.Engine == engine);
        }
    }
}
