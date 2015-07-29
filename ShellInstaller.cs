using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;
using Prediktor.Carbon.Infrastructure.Definitions;
using Prediktor.Carbon.Configuration.Resources;
using Prediktor.Carbon.Configuration.Definitions.ModuleServices;
using Prediktor.Log;
using Prediktor.Configuration.BaseTypes.Definitions;
using Prediktor.Configuration.Honeystore.Implementation;

namespace Prediktor.ExcelImport
{
    public class ShellInstaller : IWindsorInstaller
    {
        private static ITraceLog _log = LogManager.GetLogger(typeof(ShellInstaller));
        public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            _log.Debug("Entering Install");
            container.Register(Component.For<IApplicationProperties>().ImplementedBy<ApplicationProperties>());

            container.Register(Component.For<Shell>()
                .ImplementedBy<Shell>()
                .Named("TheShell"));
            container.Register(Component.For<ShellViewModel>()
                .ImplementedBy<ShellViewModel>().Named("ShellViewModel"));

            container.Register(Component.For<MainRegion>()
                .ImplementedBy<MainRegion>());
            container.Register(Component.For<MainRegionViewModel>()
                .ImplementedBy<MainRegionViewModel>()
                .Named("MainRegionViewModel"));

            container.Register(Component.For<SolutionExplorer2>()
                .ImplementedBy<SolutionExplorer2>()
                .Named("SolutionExplorer2"));
            container.Register(Component.For<SolutionExplorer2ViewModel>()
                .ImplementedBy<SolutionExplorer2ViewModel>()
                .Named("SolutionExplorerViewModel"));

            _log.Debug("Exiting Install");
        }

    }
}
