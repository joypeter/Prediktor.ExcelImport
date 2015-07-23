using System.Windows;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;
using Prediktor.Carbon.Configuration.ViewModels;
using Prediktor.Carbon.Infrastructure.Definitions;
using Prediktor.Carbon.Configuration.Definitions.ModuleServices;

namespace Prediktor.ExcelImport
{
    public class MainRegionViewModel : NotificationObject
    {
        private IEventAggregator _eventAggregator;
        private IApplicationProperties _appliationProperties;
        public MainRegionViewModel(IEventAggregator eventAggregator, TabbedViewModel tabbedViewModel,
            IResourceDictionaryProvider resourceDictionaryProvider,
            IApplicationProperties appliationProperties)
        {
            _eventAggregator = eventAggregator;
            _appliationProperties = appliationProperties;
            TabbedViewModel = tabbedViewModel;

            ResourceDictionaryProvider = resourceDictionaryProvider;
        }

        public TabbedViewModel TabbedViewModel
        {
            get;
            private set;
        }


        public IResourceDictionaryProvider ResourceDictionaryProvider
        {
            get;
            private set;
        }
    }
}
