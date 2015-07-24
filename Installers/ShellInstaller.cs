using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;
using Prediktor.Carbon.Infrastructure.Definitions;
using Prediktor.Carbon.Configuration.Resources;
using Prediktor.Carbon.Configuration.Definitions.ModuleServices;
using Prediktor.Log;

namespace Prediktor.ExcelImport.Installers
{
    public class ShellInstaller : IWindsorInstaller
    {
        private static ITraceLog _log = LogManager.GetLogger(typeof(ShellInstaller));
        public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            _log.Debug("Entering Install");
            container.Register(Component.For<IApplicationProperties>().ImplementedBy<ApplicationProperties>());

            container.Register(Component.For<MainRegionViewModel>()
                        .ImplementedBy<MainRegionViewModel>()
                        .ServiceOverrides(ServiceOverride.ForKey("resourceDictionaryProvider").Eq("TabbedResources"))
                        .Named("MainRegionViewModel"));

            container.Register(Component.For<MainRegion>()
                .ImplementedBy<MainRegion>());


            _log.Debug("Exiting Install");
        }

    }
}
