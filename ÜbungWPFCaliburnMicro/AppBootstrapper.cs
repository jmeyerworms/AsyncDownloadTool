using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using ÜbungWPF.Viewmodel;
using ÜbungWPFCaliburnMicro.BusinessLayer;
using ÜbungWPFCaliburnMicro.ViewModels;

namespace ÜbungWPFCaliburnMicro
{
    class AppBootstrapper : BootstrapperBase
    {
        private SimpleContainer _container;

        public AppBootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            _container = new SimpleContainer();

            _container.PerRequest<ICustomerService, CustomerService>();

            _container.PerRequest<IWindowManager, WindowManager>();
            _container.PerRequest<KundenübersichtViewModel>();
            _container.PerRequest<KundenDetailsViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            var instance = _container.GetInstance(service, key);
            if (instance != null)
                return instance;

            throw new InvalidOperationException("Could not locate any instances.");
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<KundenübersichtViewModel>();
        }
    }
}
