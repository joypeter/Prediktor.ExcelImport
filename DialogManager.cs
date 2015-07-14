using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Prediktor.Configuration.Windsor;
using Prediktor.Carbon.Configuration.Views;
using Prediktor.Carbon.Configuration.ViewModels;
using Prediktor.Carbon.Infrastructure.Implementation;
using Prediktor.Ioc;
using Prediktor.Log;
using Prediktor.Carbon.Infrastructure.Definitions;
using Prediktor.Carbon.Configuration.Definitions.ModuleServices;

namespace Prediktor.ExcelImport
{
    public class DialogManager
    {
        private string ioc_config = "Config//ioc.xml";
        private static ITraceLog _log = LogManager.GetLogger(typeof(DialogManager));
        private readonly IApplicationProperties _applicationProperties;

        //private DialogViewModel dialogViewModel;

        public IWindsorContainer Container { get; protected set; }

        protected DependencyObject ConnectionDialog { get; set; }

        static DialogManager instance = null;
        public static DialogManager Current
        {
            get
            {
                if (instance==null)
                {
                    instance = new DialogManager();
                }
                return instance;
            }
        }

        public DialogManager()
        {
            
        }

        private IThemeProvider _themeProvider;
        private void InitializeTheme()
        {
            _log.Debug("Entering InitializeTheme");
            if (!UriParser.IsKnownScheme("pack"))
                new System.Windows.Application();

            _themeProvider = Container.Resolve<IThemeProvider>();

            var rd = _themeProvider.GetDefaultTheme();
            UpdateTheme(rd);

            _log.Debug("Exiting InitializeTheme");
        }

        private void UpdateTheme(ResourceDictionary rd)
        {
            //System.Windows.Application.Current.Resources.MergedDictionaries.Clear();
            System.Windows.Application.Current.Resources.MergedDictionaries.Add(rd);
        }

        private IWindsorContainer CreateContainer()
        {
            //IWindsorContainer c = new WindsorContainer();

            IWindsorContainer c = string.IsNullOrEmpty(ioc_config) ? new WindsorContainer() : IocHelper.Container(ioc_config);
            //IWindsorContainer c = string.IsNullOrEmpty(ioc_config) ? base.CreateContainer() : IocHelper.Container(ioc_config);

            return c;
        }

        public void Initialize()
        {
            Container = CreateContainer();
            InitializeContainer();
            InitializeTheme();
            //dialogViewModel = new DialogViewModel();
        }

        public void Connect()
        {
            //dialogViewModel.ConnectCommand.Execute(null);
            ConnectionDialog = this.CreateConnectDialog();
            var result = ((ConnectionDialog)ConnectionDialog).ShowDialog();
            if (result.HasValue && result.Value)
            {
            }

            //w.Show();

            //Container["ConnectionDialog"].
        }

        public DependencyObject CreateConnectDialog()
        {
            return Container.Resolve<ConnectionDialog>();
            //return ServiceLocator.Current.GetInstance<ConnectionDialog>();
        }

        private void InitializeContainer()
        {
            _log.DebugFormat("Fluent ioc configuration");
            List<IWindsorInstaller> installers = new List<IWindsorInstaller>();

            var ass = typeof(ServiceLayerBootstrapper).Assembly;
            installers.Add(FromAssembly.Instance(ass));
            ass = typeof(Prediktor.Carbon.Configuration.Windsor.Installers.PrismSolutionInstaller).Assembly;
            installers.Add(FromAssembly.Instance(ass));
            installers.Add(FromAssembly.This());
            _log.DebugFormat("Install container");
            Container.Install(installers.ToArray());
            _log.DebugFormat("Container installed");
            //var ea = Prediktor.Log.Log4NetLog.FindAppender<EventAggregatorAppender>();
            //if (ea != null)
            //{
            //    ea.EventAggregator = Container.Resolve<IEventAggregator>();
            //}
            _log.DebugFormat("Fluent ioc configuration done");

            //Container.Register(Component.For<DialogViewModel>()
            //    .ImplementedBy<DialogViewModel>().Named("DialogViewModel"));

            Container.Register(Component.For<ConnectionDialogViewModel>()
                                   .ImplementedBy<ConnectionDialogViewModel>()
                                   .Named("ConnectionDialogViewModel"));

            Container.Register(Component.For<ConnectionDialog>()
                .ImplementedBy<ConnectionDialog>()
                .Named("ConnectionDialog")
                .LifeStyle.Transient);
            
        }
    }
}
