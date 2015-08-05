using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Reflection;
using System.IO;
using System.Deployment.Application;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.ServiceLocation;
using Prediktor.Carbon.Configuration;
using Prediktor.Carbon.Configuration.ViewModels;
using Prediktor.Carbon.Configuration.Windsor;
using Prediktor.Carbon.Infrastructure.Implementation;
using Prediktor.Configuration.Windsor;
using Prediktor.Ioc;
using Prediktor.Log;
using PrismContrib.WindsorExtensions;
using Prediktor.Carbon.Configuration.Definitions.ModuleServices;

namespace Prediktor.ExcelImport
{
    public class ExcelImportBootstrapper : WindsorBootstrapper
    {
        private string ioc_config = "Config//ioc.xml";
        private ITraceLog _log;

        public ExcelImportBootstrapper()
        {
            //To check: use Assembly.GetExecutingAssembly().CodeBase or Assembly.GetExecutingAssembly().Locatioon?
            string dllstr = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            string dllDirectory = new Uri(dllstr).LocalPath;
            string deployDataDirectory = ApplicationDeployment.CurrentDeployment.DataDirectory;
            // set Environment.CurrentDirectory so later when FileInfo is allocated it will 
            // use the value Environment.CurrentDirectory as its Directory property to find config/uaclient.xml
            Environment.CurrentDirectory = dllDirectory;
            ioc_config = deployDataDirectory + "/" + ioc_config;

            Prediktor.Log.Log4NetLog.Configure();
            LogManager.TraceLogFactory = (name) => new Prediktor.Log.Log4NetLog(name);
            _log = LogManager.GetLogger(typeof(ExcelImportBootstrapper));
        }

        protected override IWindsorContainer CreateContainer()
        {
            _log.DebugFormat("Starting to create ioc container");
            IWindsorContainer c = string.IsNullOrEmpty(ioc_config) ? base.CreateContainer() : IocHelper.Container(ioc_config);
            _log.DebugFormat("Ioc container created");
            return c;
        }


        protected override DependencyObject CreateShell()
        {
            return ServiceLocator.Current.GetInstance<Shell>();
        }

        private void FluentConfiguration()
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
            var ea = Prediktor.Log.Log4NetLog.FindAppender<EventAggregatorAppender>();
            if (ea != null)
            {
                ea.EventAggregator = Container.Resolve<IEventAggregator>();
            }
            _log.DebugFormat("Fluent ioc configuration done");
        }

        protected override void ConfigureModuleCatalog()
        {
            _log.DebugFormat("Starting to configure module catalog");
            Type m = typeof(Prediktor.Carbon.Configuration.Hive.HiveModule);
            ModuleCatalog.AddModule(new ModuleInfo(m.Name, m.AssemblyQualifiedName));
            m = typeof(ConfigurationModule);
            ModuleCatalog.AddModule(new ModuleInfo(m.Name, m.AssemblyQualifiedName));
            _log.DebugFormat("Module catalog configured");

        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            FluentConfiguration();
        }

        protected override void InitializeShell()
        {
            _log.DebugFormat("Starting to initialize shell");
            base.InitializeShell();
            _log.DebugFormat("Shell initialized");
        }

        protected override void InitializeModules()
        {
            base.InitializeModules();

            //To use Telerik library a resource item must be added
            Application.Current.Resources.Add("Telerik.Windows.Controls.Key", "Prediktor Telerik Application");
        }

        public void Connect()
        {
            var shellViewModel = ((Window)Shell).DataContext as ShellViewModel;
            shellViewModel.ConnectCommand.Execute(null);
            _log.DebugFormat("Connected");
        }

        public void Browse()
        {
            var shellViewModel = ((Window)Shell).DataContext as ShellViewModel;
            shellViewModel.BrowseCommand.Execute(null);
            _log.DebugFormat("Browsed");
        }

        public void Update()
        {
            var shellViewModel = ((Window)Shell).DataContext as ShellViewModel;
            shellViewModel.UpdateCommand.Execute(null);
            _log.DebugFormat("Update");
        }

        public void CloseBrowse()
        {
            var shellViewModel = ((Window)Shell).DataContext as ShellViewModel;

            shellViewModel.CloseBrowseCommand.Execute(null);
            _log.DebugFormat("CloseBrowse");
        }
    }
}
