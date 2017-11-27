using System;
using System.Windows;
using Autofac;
using Caliburn.Micro;
using WPFDownloadTool.BusinessLayer;
using WPFDownloadTool.BusinessLayer.Download;
using WPFDownloadTool.BusinessLayer.Show;
using WPFDownloadTool.BusinessLayer.Url;
using WPFDownloadTool.Model;
using WPFDownloadTool.ViewModels;

namespace WPFDownloadTool
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
