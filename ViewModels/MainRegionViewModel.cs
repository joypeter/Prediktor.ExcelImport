using System.Windows;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;
using Prediktor.Carbon.Configuration.ViewModels;
using Prediktor.Carbon.Infrastructure.Definitions;
using Prediktor.Carbon.Configuration.Definitions.ModuleServices;
using Prediktor.Carbon.Configuration.Definitions.Events;
using Prediktor.Configuration.BaseTypes.Definitions;
using Prediktor.Configuration.OpcHda.Definitions.Service;
using Prediktor.Carbon.Configuration.Definitions.ViewModels;
using Prediktor.Configuration.Persistence.Definitions;
using Prediktor.Configuration.BaseTypes.Implementation;
using Prediktor.Configuration.Definitions;
using System.Windows.Input;
using System.Linq;
using System;
using System.IO;
using System.Collections.Specialized;
using Microsoft.Practices.Prism.Commands;
using Prediktor.Carbon.Configuration.Views;
using Prediktor.Utilities;
using Prediktor.ExcelImport.ViewModels;
using Prediktor.ExcelImport.Views;

namespace Prediktor.ExcelImport
{
    public class MainRegionViewModel : BaseHistoricalViewModel, IContentViewModel
    {
        private readonly IEventContext _eventContext;
        private readonly IInteractionService _interactionService;
        private readonly IHistoricalTimeUtility _historicalTimeUtility;
        private readonly IHdaFileExportService _hdaFileExportService;
        private IEventAggregator _eventAggregator;
        private IApplicationProperties _appliationProperties;

        private SubscriptionToken _solutionSelectionChangedToken;

        //bind to Import button
        private bool _hasItems;

        public MainRegionViewModel(IEventAggregator eventAggregator, 
            IResourceDictionaryProvider resourceDictionaryProvider,
            IApplicationProperties appliationProperties, 
            IEventContext eventContext, 
            IObjectServiceOperations objectServiceOperations,
            IInteractionService interactionService,
            IHistoricalTimeUtility historicalTimeUtility, 
            IPropertyNameService columnNameService, 
            IHistoricalColumnService historicalColumnService,
            ISerializationService serializationService, 
            IHelpExtension helpExtension, 
            IValueFormatter valueFormatter, 
            IHdaFileExportService hdaFileExportService, 
            IDocumentationService documentationService)
            : base(eventContext, objectServiceOperations)
        {
            _eventContext = eventContext;
            _interactionService = interactionService;
            _historicalTimeUtility = historicalTimeUtility;
            _eventAggregator = eventAggregator;
            _hdaFileExportService = hdaFileExportService;
            _appliationProperties = appliationProperties;

            ResourceDictionaryProvider = resourceDictionaryProvider;

            TimePeriodViewModel = new HistoricalTimePeriodViewModel(eventContext, objectServiceOperations, historicalTimeUtility,
                interactionService, helpExtension, documentationService);
            ListViewModel = new HistoricalPropertyListViewModel(eventContext, objectServiceOperations, columnNameService,
                historicalColumnService, interactionService, serializationService, valueFormatter);
            EventListViewModel = new HistoricalEventListViewModel(eventContext, objectServiceOperations, columnNameService,
                historicalColumnService, interactionService, serializationService, valueFormatter);
            ChartModel = new HistoricalChartViewModel(eventContext, objectServiceOperations, interactionService, columnNameService, valueFormatter, serializationService);
            ChartModel.Legend.CollectionChanged += Legend_CollectionChanged;
            ExcelService = new HistoricalExcelService(this, eventContext, objectServiceOperations, interactionService, historicalTimeUtility, valueFormatter);

            ExportCommand = new DelegateCommand(Export);
            SubscribeEvents();
        }

        public bool HasItems
        {
            get { return _hasItems; }
            set
            {
                _hasItems = value;
                RaisePropertyChanged(() => HasItems);
            }
        }

        public IResourceDictionaryProvider ResourceDictionaryProvider
        {
            get;
            private set;
        }

        public HistoricalTimePeriodViewModel TimePeriodViewModel
        {
            get;
            private set;
        }

        public HistoricalPropertyListViewModel ListViewModel
        {
            get;
            private set;
        }

        public HistoricalEventListViewModel EventListViewModel
        {
            get;
            private set;
        }

        public HistoricalChartViewModel ChartModel
        {
            get;
            private set;
        }

        public HistoricalExcelService ExcelService
        {
            get;
            private set;
        }

        public ICommand ExportCommand { get; private set; }

        private void SubscribeEvents()
        {
            _solutionSelectionChangedToken = _eventAggregator.GetEvent<SolutionExplorerSelectionChangedEvent>().Subscribe(
                SolutionExplorerSelectionChanged, ThreadOption.UIThread);
        }

        private void Legend_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (ChartModel.Legend.Count > 0) 
            {
                HasItems = true;
            }
            else
            {
                HasItems = false;
            }
        }

        private void Export()
        {
            ExcelService.ExportDataToExcel();
        }

        private void SolutionExplorerSelectionChanged(SolutionExplorerSelection obj)
        {
            IObjectId [] objs = obj.Selection.ToArray();
            _eventContext.ContextualEventAggregator.GetEvent<ObjectsAddedToViewEvent>().Publish(objs);
            if (0 < objs.Count<IObjectId>())
            {
                HasItems = true;
            }
            else
            {
                HasItems = false;
            }
        }
        private void UnsubscribeEvents()
        {
            _eventAggregator.GetEvent<SolutionExplorerSelectionChangedEvent>().Unsubscribe(
                    _solutionSelectionChangedToken);
        }

        public bool IsRemovable()
        {
            return true;
        }

        public event EventHandler<EventArgs<IContentViewModel>> NotifyRemoveContent;

        private void OnNotifyRemoved(EventArgs<IContentViewModel> ev)
        {
            var e = NotifyRemoveContent;
            if (e != null)
                e(this, ev);
        }

        public void Clear()
        {
            UnsubscribeEvents();
            ListViewModel.Clear();
            ChartModel.Clear();
        }

        public void Restored()
        {
        }
    }
}
