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

        private ICommand _windowClosing;
        private string _configFile;

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
            _windowClosing = new DelegateCommand<System.ComponentModel.CancelEventArgs>(OnWindowClosing);

            _configFile = _applicationService.CurrentFile;

            _eventAggregator.GetEvent<AddedServiceEvent>().Subscribe(OnServiceAdded, ThreadOption.UIThread);
        }


        private void OnExitCommand()
        {
            //if (DoNew())
            //{
            _eventAggregator.GetEvent<ClosingEvent>().Publish(new Close());
            //Application.Current.Shutdown();
            //}
        }

        private void OnServiceAdded(ServiceAdded obj)
        {
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

        public ICommand ConnectCommand { get; private set; }

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


        private void OnWindowClosing(System.ComponentModel.CancelEventArgs ea)
        {
            //if (DoNew())
            _eventAggregator.GetEvent<ClosingEvent>().Publish(new Close());
            //else
            //ea.Cancel = true;
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

        public ICommand WindowClosing
        {
            get { return _windowClosing; }
        }
    }
}
