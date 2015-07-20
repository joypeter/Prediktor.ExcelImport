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
        private TabContentViewModel _logViewTab;
        private LogViewModel _logViewModel;
        private TabContentViewModel _resultViewTab;
        public MainRegionViewModel(IEventAggregator eventAggregator, TabbedViewModel tabbedViewModel,
            LogViewModel logViewModel,
            ResultViewModel resultViewModel,
            IResourceDictionaryProvider resourceDictionaryProvider,
            IApplicationProperties appliationProperties)
        {
            _eventAggregator = eventAggregator;
            _appliationProperties = appliationProperties;
            _logViewModel = logViewModel;
            TabbedViewModel = tabbedViewModel;
            _resultViewTab = new TabContentViewModel("Results", "Results", false, () => OnShowResultViewChanged(false)) { Content = resultViewModel };
            _logViewTab = new TabContentViewModel("Debug Log", "Debug Log", false, () => OnShowDebugViewChanged(false)) { Content = logViewModel };
            LowerTabbedViewModel = new LowerTabViewModel();

            if (_appliationProperties.IsResultViewVisible)
                LowerTabbedViewModel.AddTabItem(_resultViewTab);
            _logViewModel.IsVisible = _appliationProperties.IsDebugViewVisible;
            if (_appliationProperties.IsDebugViewVisible)
                LowerTabbedViewModel.AddTabItem(_logViewTab);
            if (LowerTabbedViewModel.TabItems.Count > 0)
                LowerTabbedViewModel.SelectedItem = LowerTabbedViewModel.TabItems[0];
            ResourceDictionaryProvider = resourceDictionaryProvider;
        }

        private void OnShowDebugViewChanged(bool show)
        {
            _logViewModel.IsVisible = show;
            if (!show)
            {
                LowerTabbedViewModel.RemoveTabItem(_logViewTab);
            }
            else if (!LowerTabbedViewModel.TabItems.Contains(_logViewTab))
            {
                LowerTabbedViewModel.AddTabItem(_logViewTab);
                LowerTabbedViewModel.SelectedItem = _logViewTab;
            }
            _appliationProperties.IsDebugViewVisible = show;
            _appliationProperties.Save();
            RaisePropertyChanged(() => LowerTabVisibility);
        }

        private void OnShowResultViewChanged(bool show)
        {
            if (!show)
            {
                LowerTabbedViewModel.RemoveTabItem(_resultViewTab);
            }
            else if (!LowerTabbedViewModel.TabItems.Contains(_resultViewTab))
            {
                LowerTabbedViewModel.AddTabItem(_resultViewTab);
                LowerTabbedViewModel.SelectedItem = _resultViewTab;
            }
            _appliationProperties.IsResultViewVisible = show;
            _appliationProperties.Save();
            RaisePropertyChanged(() => LowerTabVisibility);
        }


        public TabbedViewModel TabbedViewModel
        {
            get;
            private set;
        }


        public LowerTabViewModel LowerTabbedViewModel
        {
            get;
            private set;
        }

        public Visibility LowerTabVisibility
        {
            get { return LowerTabbedViewModel.TabItems.Count > 0 ? Visibility.Visible : Visibility.Collapsed; }
        }


        public IResourceDictionaryProvider ResourceDictionaryProvider
        {
            get;
            private set;
        }
    }
}
