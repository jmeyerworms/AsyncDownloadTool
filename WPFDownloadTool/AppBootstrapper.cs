using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Autofac;
using Autofac.Core;
using ÜbungWPFDownloadTool.BusinessLayer;
using ÜbungWPFDownloadTool.BusinessLayer.Download;
using ÜbungWPFDownloadTool.BusinessLayer.Show;
using ÜbungWPFDownloadTool.BusinessLayer.Url;
using ÜbungWPFDownloadTool.Model;
using ÜbungWPFDownloadTool.ViewModels;

namespace ÜbungWPFDownloadTool
{
    class AppBootstrapper : BootstrapperBase
    {
        private static IContainer Container { get; set; }        

        public AppBootstrapper()
        {
            Initialize();            
        }

        protected override void Configure()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<WindowManager>().As<IWindowManager>();
            builder.RegisterType<Download>();
            builder.RegisterType<Show>().As<IShow>();
            builder.RegisterType<UrlService>().As<IUrlService>();
            builder.RegisterType<SelectFile>().As<ISelectFile>();

            builder.RegisterType<HttpWebDownloadService>().As<IDownloadService>();
            builder.RegisterType<WebClientDownloadService>().As<IDownloadService>();

            builder.RegisterType<CreateEngine>().As<ICreateEngine>();
            builder.RegisterType<DownloaderViewModel>();
            Container = builder.Build();        

        }

        protected override object GetInstance(Type service, string key)
        {
            return Container.Resolve(service);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {            
            DisplayRootViewFor<ViewModels.DownloaderViewModel>();
        }

    }
}
