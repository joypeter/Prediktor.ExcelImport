using System;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using Prediktor.Carbon.Configuration.Definitions.ModuleServices;
using Prediktor.Carbon.Configuration.ViewModels;
using Prediktor.Carbon.Configuration.Views;
using Prediktor.Carbon.Infrastructure.Definitions;
using Prediktor.Configuration.Definitions;
using Prediktor.Log;
using Prediktor.Services.Definitions;
using System.Linq;

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
        private readonly IUACertificateUtility _uaCertificateUtility;
        private readonly string _title;
        private readonly IServiceFactory _serviceFactory;
        private readonly INetworkBrowser _networkBrowser;
        private static ITraceLog _log = LogManager.GetLogger(typeof(ShellViewModel));

        private string _configFile;

        private BrowseDialog browseDialog;
        private bool _connected;

        public ShellViewModel(IThemeProvider themeProvider, IEventAggregator eventAggregator, IInteractionService interactionService,
            IApplicationProperties applicationProperties, IApplicationFeatures applicationFeatures,
            IConfigApplicationService applicationService, string title, IServiceFactory serviceFactory, INetworkBrowser networkBrowser,
            IUACertificateUtility uaCertificateUtility)
        {
            _themeProvider = themeProvider;
            _eventAggregator = eventAggregator;
            _interactionService = interactionService;
            _applicationProperties = applicationProperties;
            _applicationFeatures = applicationFeatures;
            _applicationService = applicationService;
            _uaCertificateUtility = uaCertificateUtility;
            _title = title;
            _serviceFactory = serviceFactory;
            _networkBrowser = networkBrowser;
            InitializeTheme();
            ConnectCommand = new DelegateCommand(Connect);
            BrowseCommand = new DelegateCommand(Browse);
            UpdateCommand = new DelegateCommand(Update);
            CloseBrowseCommand = new DelegateCommand(CloseBrowse);
            AboutCommand = new DelegateCommand(About);
            HelpCommand = new DelegateCommand(Help);
            CertGenCommand = new DelegateCommand(GenerateCertificate);
            CertLocCommand = new DelegateCommand(OpenCertificateLocation);

            _configFile = _applicationService.CurrentFile;
            _connected = false;
        }

        private void Connect()
        {
            var appProperties = (ApplicationProperties)_applicationProperties;
            var viewModel = new ConnectionDialogViewModel(_eventAggregator, _interactionService, _serviceFactory, _networkBrowser, appProperties.LastUri);
            viewModel.CertificateCommand = new DelegateCommand(CheckCertificate);
            var connectionDialog = new ConnectionDialog(viewModel);
            var result = connectionDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                appProperties.LastUri = viewModel.LastUri;
                appProperties.Save();
                _connected = true;
            }
        }

        private void Browse()
        {
            if (!_connected)
                Connect();

            browseDialog = new BrowseDialog(this);
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

        private void Help()
        {
            //_documentationService.ShowDocumentation();
        }

        private void About()
        {
            var about = new StringBuilder();
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            var tr = _interactionService.TranslatingService;
            var dlg = _interactionService.DialogService;
            about.Append(tr.GetSystemText(_title));
            about.Append("\n");
            about.Append(tr.GetSystemText("Copyright © Prediktor AS"));
            about.Append("\n\n");
            about.Append(tr.GetSystemText("Version: "));
            about.Append(version);
            dlg.GetMessageDialog().ShowInfo(about.ToString(), tr.GetSystemText("About " + _title));
        }


        private void GenerateCertificate()
        {
            var appProperties = (ApplicationProperties)_applicationProperties;
            var viewModel = new CertificateDialogViewModel(_interactionService, _uaCertificateUtility);
            viewModel.CommonName = appProperties.CommonName;
            viewModel.Domain = appProperties.Domain;
            viewModel.Organization = appProperties.Organization;
            viewModel.SelectValidity = appProperties.Validity.Equals("") ? viewModel.ValidityList[0] :
                viewModel.ValidityList.Where<Validity>(V => V.Name == appProperties.Validity).First<Validity>();
            viewModel.SelectKeyLength = appProperties.KeyLength.Equals("") ? viewModel.KeyLengthList[0] :
                viewModel.KeyLengthList.Where<KeyLength>(K => K.Name == appProperties.KeyLength).First<KeyLength>();
            var certificateDialog = new CertificateDialog(viewModel);
            var result = certificateDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                appProperties.CommonName = viewModel.CommonName;
                appProperties.Domain = viewModel.Domain;
                appProperties.Organization = viewModel.Organization;
                appProperties.KeyLength = viewModel.SelectKeyLength.Name;
                appProperties.Validity = viewModel.SelectValidity.Name;
                appProperties.Save();
            }
        }

        private void OpenCertificateLocation()
        {
            string path = System.IO.Directory.GetCurrentDirectory();
            System.Diagnostics.Process.Start("Explorer.exe", _uaCertificateUtility.GetCertificatePath());
        }

        private void CheckCertificate()
        {
            if (!_uaCertificateUtility.IsCertificateExisted())
            {
                _interactionService.DialogService.GetMessageDialog().ShowInfo(
                    _interactionService.TranslatingService.GetSystemText("There is no UA certificate, please generate a new certificate first!"));

                GenerateCertificate();
            }
        }

        public ICommand ConnectCommand { get; private set; }
        public ICommand BrowseCommand { get; private set; }
        public ICommand UpdateCommand { get; private set; }
        public ICommand AboutCommand { get; private set; }
        public ICommand HelpCommand { get; private set; }
        public ICommand CloseBrowseCommand { get; private set; }
        public ICommand CertGenCommand { get; private set; }
        public ICommand CertLocCommand { get; private set; }

        private void InitializeTheme()
        {
            _log.Debug("Entering InitializeTheme");
            if (!UriParser.IsKnownScheme("pack"))
                new Application();

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
