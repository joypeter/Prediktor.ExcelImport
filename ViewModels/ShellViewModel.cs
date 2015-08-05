using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using Prediktor.Carbon.Configuration.Definitions.Events;
using Prediktor.Carbon.Configuration.Definitions.ModuleServices;
using Prediktor.Carbon.Configuration.ViewModels;
using Prediktor.Carbon.Configuration.Views;
using Prediktor.Carbon.Infrastructure.Definitions;
using Prediktor.Carbon.Infrastructure.Definitions.Events;
using Prediktor.Configuration.Definitions;
using Prediktor.Log;
using Prediktor.Services.Definitions;
using Prediktor.ExcelImport.Views;
using Prediktor.ExcelImport.ViewModels;

namespace Prediktor.ExcelImport
{
    public class ShellViewModel : NotificationObject
    {
        private readonly IThemeProvider _themeProvider;
        private readonly IEventAggregator _eventAggregator;
        private readonly IInteractionService _interactionService;
        private readonly IApplicationProperties _applicationProperties;
        private readonly IApplicationFeatures _applicationFeatures;
        private readonly IConfigApplicationService _applicationService;
        private readonly string _title;
        private readonly IServiceFactory _serviceFactory;
        private readonly INetworkBrowser _networkBrowser;
        private static ITraceLog _log = LogManager.GetLogger(typeof(ShellViewModel));

        private string _configFile;

        private BrowseDialog browseDialog;

        public ShellViewModel(IThemeProvider themeProvider, IEventAggregator eventAggregator, IInteractionService interactionService,
            IApplicationProperties applicationProperties, IApplicationFeatures applicationFeatures,
            IConfigApplicationService applicationService, string title, IServiceFactory serviceFactory, INetworkBrowser networkBrowser)
        {
            _themeProvider = themeProvider;
            _eventAggregator = eventAggregator;
            _interactionService = interactionService;
            _applicationProperties = applicationProperties;
            _applicationFeatures = applicationFeatures;
            _applicationService = applicationService;
            _title = title;
            _serviceFactory = serviceFactory;
            _networkBrowser = networkBrowser;
            InitializeTheme();
            ConnectCommand = new DelegateCommand(Connect);
            BrowseCommand = new DelegateCommand(Browse);
            UpdateCommand = new DelegateCommand(Update);
            CloseBrowseCommand = new DelegateCommand(CloseBrowse);

            _configFile = _applicationService.CurrentFile;
        }

        private void Connect()
        {
            var appProperties = (ApplicationProperties)_applicationProperties;
            var viewModel = new ConnectionDialogViewModel(_eventAggregator, _interactionService, _serviceFactory, _networkBrowser, appProperties.LastUri);
            var connectionDialog = new ConnectionDialog(viewModel);
            var result = connectionDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                appProperties.LastUri = viewModel.LastUri;
                appProperties.Save();
            }
        }

        private void Browse()
        {
            browseDialog = new BrowseDialog();
            browseDialog.Title = Title;

            //Due to a bug in Prism v4, regions can not be added either from xaml or programatically
            //, so here we add the view directly to the ItemControl
            _log.DebugFormat("Initializing main region");
            MainRegion mainRegion = ServiceLocator.Current.GetInstance<MainRegion>();
            browseDialog.AddSolutionMainRegion(mainRegion);
            _log.DebugFormat("Main region initialized");

            _log.DebugFormat("Initializing TreeViewRegion");
            SolutionExplorer2 se2 = ServiceLocator.Current.GetInstance<SolutionExplorer2>();
            browseDialog.AddSolutionExplorer2(se2);
            _log.DebugFormat("TreeViewRegion Initialized");

            browseDialog.ShowDialog();
        }

        private void Update()
        {
            if (HistoricalExcelService.Current != null)
                HistoricalExcelService.Current.UpdateDataToExcel();
        }

        private void CloseBrowse()
        {
            if (browseDialog != null)
                browseDialog.Close();
        }

        public ICommand ConnectCommand { get; private set; }
        public ICommand BrowseCommand { get; private set; }
        public ICommand UpdateCommand { get; private set; }
        public ICommand CloseBrowseCommand { get; private set; }

        private void InitializeTheme()
        {
            _log.Debug("Entering InitializeTheme");
            if (!UriParser.IsKnownScheme("pack"))
                new System.Windows.Application();

            var rd = _themeProvider.GetDefaultTheme();
            UpdateTheme(rd);

            _log.Debug("Exiting InitializeTheme");
        }

        private void UpdateTheme(ResourceDictionary rd)
        {
            System.Windows.Application.Current.Resources.MergedDictionaries.Add(rd);
        }

        public string Title
        {
            get
            {
                string t = _title;
                if (!string.IsNullOrEmpty(_configFile))
                {
                    t = _title + " - " + _configFile;
                }
                return t;
            }
        }
    }
}
