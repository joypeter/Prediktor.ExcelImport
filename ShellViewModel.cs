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
        private ICommand _lightThemeCommand;
        private ICommand _darkThemeCommand;
        private ICommand _blueThemeCommand;
        private ICommand _aboutCommand;

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

            _aboutCommand = new DelegateCommand(OnAboutCommand);

            _lightThemeCommand = new DelegateCommand(OnLightThemeCommand);
            _darkThemeCommand = new DelegateCommand(OnDarkThemeCommand);
            _blueThemeCommand = new DelegateCommand(OnBlueThemeCommand);

            _configFile = _applicationService.CurrentFile;

            _eventAggregator.GetEvent<AddedServiceEvent>().Subscribe(OnServiceAdded, ThreadOption.UIThread);
            _eventAggregator.GetEvent<FileActiveEvent>().Subscribe(OnActiveFileChanged);
        }


        private void OnActiveFileChanged(string file)
        {
            _configFile = file;
            RaisePropertyChanged(() => Title);
        }

        private bool CanSave(FileCapabilites fileCap)
        {
            return (fileCap & FileCapabilites.Save) == FileCapabilites.Save;
        }

        private bool CanSaveAs(FileCapabilites fileCap)
        {
            return (fileCap & FileCapabilites.SaveAs) == FileCapabilites.SaveAs;
        }

        private bool CanOpen(FileCapabilites fileCap)
        {
            return (fileCap & FileCapabilites.Open) == FileCapabilites.Open;
        }

        private bool CanNew(FileCapabilites fileCap)
        {
            return (fileCap & FileCapabilites.New) == FileCapabilites.New;
        }

        private void OnSaveAsCommand()
        {
            if (CanSaveAs(_applicationFeatures.FileCapabilities))
            {
                string s = _interactionService.DialogService.GetFileDialog().SaveFileDialog("Apis Configuration Files|*.acf");
                if (!string.IsNullOrEmpty(s))
                {
                    _eventAggregator.GetEvent<FileSaveAsEvent>().Publish(s);
                }
            }
        }

        private void OnSaveCommand()
        {
            if (CanSave(_applicationFeatures.FileCapabilities))
            {
                if (string.IsNullOrWhiteSpace(this._applicationService.CurrentFile))
                {
                    OnSaveAsCommand();
                }
                else
                {
                    _eventAggregator.GetEvent<FileSaveAsEvent>().Publish(this._applicationService.CurrentFile);
                }
            }
        }

        private void OnOpenCommand()
        {
            if (CanOpen(_applicationFeatures.FileCapabilities))
            {
                string s = _interactionService.DialogService.GetFileDialog().OpenFileDialog("Apis Configuration Files|*.acf");
                if (s != null)
                {
                    _eventAggregator.GetEvent<ClearConfiguration>().Publish(string.Empty);
                    _eventAggregator.GetEvent<FileOpenEvent>().Publish(new FileOpenInfo(s, false));
                }
            }
        }


        private void OnNewCommand()
        {
            if (CanNew(_applicationFeatures.FileCapabilities))
            {
                _eventAggregator.GetEvent<ClearConfiguration>().Publish(string.Empty);
            }
        }

        private void OnAboutCommand()
        {
            var about = new StringBuilder();
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            var tr = _interactionService.TranslatingService;
            var dlg = _interactionService.DialogService;
            about.Append(tr.GetSystemText(_title));
            about.Append("\n");
            about.Append(tr.GetSystemText("Copyright © Prediktor as"));
            about.Append("\n\n");
            about.Append(tr.GetSystemText("Version: "));
            about.Append(version);
            dlg.GetMessageDialog().ShowInfo(about.ToString(), tr.GetSystemText("About " + _title));
        }

        private void OnHelpCommand()
        {
            //_documentationService.ShowDocumentation();
        }

        private void OnExitCommand()
        {
            //if (DoNew())
            //{
            _eventAggregator.GetEvent<ClosingEvent>().Publish(new Close());
            Application.Current.Shutdown();
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

        private void OnLightThemeCommand()
        {
            ChangeTheme("pack://application:,,,/Prediktor.Carbon.Style;component/LightTheme.xaml");
        }

        private void OnDarkThemeCommand()
        {
            ChangeTheme("pack://application:,,,/Prediktor.Carbon.Style;component/DarkTheme.xaml");
        }

        private void OnBlueThemeCommand()
        {
            ChangeTheme("pack://application:,,,/Prediktor.Carbon.Style;component/BlueTheme.xaml");
        }

        private void ChangeTheme(string theme)
        {
            try
            {
                var rd = new ResourceDictionary();
                rd.Source = new Uri(theme);
                UpdateTheme(rd);
            }
            catch (Exception e)
            {
                _log.Error("Error when changing theme", e);
                _interactionService.DialogService.GetMessageDialog().ShowError("Error when changing theme: " + e.Message, "Theme Error");
            }

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

        
        public ICommand LightThemeCommand
        {
            get { return _lightThemeCommand; }
        }

        public ICommand DarkThemeCommand
        {
            get { return _darkThemeCommand; }
        }

        public ICommand BlueThemeCommand
        {
            get { return _blueThemeCommand; }
        }
    }
}
