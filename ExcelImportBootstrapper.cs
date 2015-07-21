using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
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
using Prediktor.Carbon.Configuration.Views;

namespace Prediktor.ExcelImport
{
    public class ExcelImportBootstrapper : WindsorBootstrapper
    {
        private string ioc_config = "Config//ioc.xml";
        private ITraceLog _log;

        public ExcelImportBootstrapper()
        {
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

            Container.Register(Component.For<ShellViewModel>()
                .ImplementedBy<ShellViewModel>().Named("ShellViewModel"));

            Container.Register(Component.For<IApplicationProperties>()
                .ImplementedBy<ApplicationProperties>()
                .Named("ApplicationProperties"));

            Container.Register(Component.For<Shell>()
                .ImplementedBy<Shell>()
                .Named("TheShell"));

            Container.Register(Component.For<SolutionExplorer2>()
                .ImplementedBy<SolutionExplorer2>()
                .Named("SolutionExplorer2"));

            Container.Register(Component.For<ConnectionDialogViewModel>()
                                   .ImplementedBy<ConnectionDialogViewModel>()
                                   .LifeStyle.Transient);

            RegisterTypeIfMissing(typeof(IRegionManager), typeof(RegionManager), true);
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

            Application.Current.Resources.Add("Telerik.Windows.Controls.Key", "Prediktor Telerik Application");
            SolutionExplorer2 se2 = ServiceLocator.Current.GetInstance<SolutionExplorer2>();

            Shell shell = (Shell)this.Shell;
            shell.AddSolutionExplorer2(se2);

            _log.DebugFormat("Initializing main region");
            IRegionManager regionManager = this.Container.Resolve<IRegionManager>();
            //IRegionManager regionManager = ServiceLocator.Current.GetInstance<IRegionManager>();
            regionManager.RegisterViewWithRegion("MainRegion", typeof(MainRegion));
            if (regionManager.Regions.ContainsRegionWithName("MainRegion"))
            {
                IRegion region = this.Container.Resolve<IRegionManager>().Regions["MainRegion"];
                var v = this.Container.Resolve<MainRegion>();
                region.Add(v, "View");
                region.Activate(v);
            }
            _log.DebugFormat("Main region initialized");

            _log.DebugFormat("Initializing TreeViewRegion");
            if (regionManager.Regions.ContainsRegionWithName("TreeViewRegion"))
            {
                _log.DebugFormat("Getting TreeViewRegion");
                IRegion region = this.Container.Resolve<IRegionManager>().Regions["TreeViewRegion"];
                _log.DebugFormat("Resolving SolutionExplorer");
                var r = this.Container.Resolve<Prediktor.Carbon.Configuration.Views.SolutionExplorer2>();
                _log.DebugFormat("Addding SolutionExplorer");
                region.Add(r, "SolutionExplorer");
                _log.DebugFormat("Activating SolutionExplorer");
                region.Activate(r);

                r.BottomToolbarVisibile = false;
            }
            _log.DebugFormat("TreeViewRegion Initialized");

            _log.DebugFormat("Initializing PropertyRegion");
            if (regionManager.Regions.ContainsRegionWithName("PropertyRegion"))
            {
                IRegion region = this.Container.Resolve<IRegionManager>().Regions["PropertyRegion"];
                var r = this.Container.Resolve<Prediktor.Carbon.Configuration.Views.PropertyEditor>();
                region.Add(r, "PropertyEditor");
                region.Activate(r);
            }
            _log.DebugFormat("PropertyRegion initialized");
        }

        public void Connect()
        {
            var shellViewModel = ((Window)Shell).DataContext as ShellViewModel;
            shellViewModel.ConnectCommand.Execute(null);
            _log.DebugFormat("Connected");
        }

        public void Browse()
        {
            Application.Current.MainWindow = (Window)this.Shell;
            Application.Current.MainWindow.Show();
            
            _log.DebugFormat("MainWindow displayed");
        }
    }
}
