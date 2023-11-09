using Caliburn.Micro;
using OKX.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace OKX.UI
{
    public class Bootstrapper : BootstrapperBase
    {
        private SimpleContainer container;

        public Bootstrapper()
        {
            Initialize();
            //Config.LoadConfig();
            //MenuViewModel.GetAccountInfo();
            //GetAccountInfo();
        }

        protected override void Configure()
        {
            container = new SimpleContainer();

            container.Instance(container);

            container
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<IEventAggregator, EventAggregator>()
               .Singleton<ShellViewModel>()
               .Singleton<AccountBalanceViewModel>()
               .Singleton<TradeOrderViewModel>()
               .Singleton<MarketTickerViewModel>()
               .Singleton<AssetCurrenciesViewModel>()
               .Singleton<AssetBalancesViewModel>()
               .Singleton<AssetAssetValuationViewModel>()
               .Singleton<MarketTickersViewModel>();
            //container.RegisterPhoneServices(RootFrame);
            //container
            //   .PerRequest<ShellViewModel>()
            //   .PerRequest<AccountBalanceViewModel>()
            //   .PerRequest<TradeOrderViewModel>()
            //   .PerRequest<MarketTickerViewModel>()
            //   .PerRequest<AssetCurrenciesViewModel>()
            //   .PerRequest<AssetBalancesViewModel>()
            //   .PerRequest<AssetAssetValuationViewModel>()
            //   .PerRequest<MarketTickersViewModel>();
            //.PerRequest<ExecuteViewModel>()
            //.PerRequest<EventAggregationViewModel>()
            //.PerRequest<DesignTimeViewModel>()
            //.PerRequest<ConductorViewModel>()
            //.PerRequest<BubblingViewModel>()
            //.PerRequest<NavigationSourceViewModel>()
            //.PerRequest<NavigationTargetViewModel>();
        }



        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            //Application.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            //var windowManager = IoC.Get<IWindowManager>();
            //var eventAggregator = IoC.Get<IEventAggregator>();
            //windowManager.ShowDialog(new SplashScreenViewModel(eventAggregator));
            //DisplayRootViewFor(typeof(MenuViewModel));


            //var splash = container.GetExportedValue<SplashScreenViewModel>();
            //var windowManager = IoC.Get<IWindowManager>();
            //windowManager.ShowDialog(splash);
            //// do your background work here using
            //var bw = new BackgroundWorker();
            //bw.DoWork += (s, e) =>
            //{
            //    // Do your background process here
            //};
            //bw.RunWorkerCompleted += (s, e) =>
            //{
            //    // close the splash window
            //    splash.TryClose();
            //    this.DisplayRootViewFor<MenuViewModel>();
            //};
            //bw.RunWorkerAsync();

            FrameworkCompatibilityPreferences.KeepTextBoxDisplaySynchronizedWithTextProperty = false;//小数点问题

            DisplayRootViewFor<ShellViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            //MessageBox.Show(e.Exception.Message, "An error as occurred", MessageBoxButton.OK);
        }
    }
}
