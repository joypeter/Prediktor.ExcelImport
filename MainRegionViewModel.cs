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
using Microsoft.Practices.Prism.Commands;
using Prediktor.Carbon.Configuration.Views;
using Prediktor.Utilities;

namespace Prediktor.ExcelImport
{
    public class MainRegionViewModel : BaseHistoricalViewModel, IContentViewModel, IState
    {
        private readonly IEventContext _eventContext;
        private readonly IInteractionService _interactionService;
        private readonly IHistoricalTimeUtility _historicalTimeUtility;
        private readonly IHdaFileExportService _hdaFileExportService;
        private IEventAggregator _eventAggregator;
        private IApplicationProperties _appliationProperties;
        private bool _activated;

        private SubscriptionToken _solutionSelectionChangedToken;
        private SubscriptionToken _addItemsToCurrentHistoryViewToken;

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

            ExportCommand = new DelegateCommand(Export);
            SubscribeEvents();
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
        public ICommand ExportCommand { get; private set; }

        private void SubscribeEvents()
        {
            _solutionSelectionChangedToken = _eventAggregator.GetEvent<SolutionExplorerSelectionChangedEvent>().Subscribe(
                SolutionExplorerSelectionChanged, ThreadOption.UIThread);
            _addItemsToCurrentHistoryViewToken = _eventAggregator.GetEvent<AddItemsToCurrentHistoryViewEvent>().Subscribe(AddItemsToCurrentHistoryView,
                                                                                     ThreadOption.UIThread, false,
                                                                                     a => _activated);
        }

        private string GetFileName(ExportDialogViewModel exportDialogViewModel)
        {
            if (exportDialogViewModel.AppendStartDate)
            {
                var dateTime = DateTime.Now;
                try
                {
                    dateTime = DateTime.Parse(TimePeriodViewModel.StartTime);
                }
                catch (Exception)
                {
                }

                var filename = Path.GetFileNameWithoutExtension(exportDialogViewModel.File) + "_" +
                               dateTime.ToString("yyyy-MM-dd");

                var fileext = Path.GetExtension(exportDialogViewModel.File);

                return Path.Combine(Path.GetDirectoryName(exportDialogViewModel.File), filename + fileext);
            }
            else
            {
                return exportDialogViewModel.File;
            }
        }

        private void Export()
        {
            var viewModel = new ExportDialogViewModel(_interactionService);
            var exportDialog = new ExportDialog(viewModel);
            var r = exportDialog.ShowDialog();
            if (r.HasValue && r.Value)
            {
                try
                {
                    string columnSeparator = "\t";
                    if (viewModel.IsOtherColumnSeparator && !string.IsNullOrEmpty(viewModel.ColumnSeparator))
                    {
                        columnSeparator = viewModel.ColumnSeparator;
                    }

                    string fileName = GetFileName(viewModel);

                    var endTime = _historicalTimeUtility.Parse(TimePeriodViewModel.EndTime);
                    var startTime = _historicalTimeUtility.Parse(TimePeriodViewModel.StartTime);
                    if (endTime.Success && startTime.Success && TimePeriodViewModel.SelectedAggregate != null)
                    {
                        var historicalArguments = new HistoricalArguments(startTime.Value, endTime.Value, TimePeriodViewModel.Resample, TimePeriodViewModel.MaxValues);

                        if (viewModel.IsRowEventList)
                        {
                            _hdaFileExportService.WriteAsciiFileOrganizeAsEventList(fileName, columnSeparator, EventListViewModel.DisplayQuality, ListViewModel.GetHistoricalProperties(), historicalArguments, TimePeriodViewModel.SelectedAggregate);
                        }
                        else
                        {
                            if (!viewModel.IsOrganizeDataRowByRow)
                            {
                                _hdaFileExportService.WriteAsciiFileOrganizeAsTable(fileName, columnSeparator, ListViewModel.DisplayOnlyFirstTime, ListViewModel.DisplayQuality, ListViewModel.GetHistoricalProperties(), historicalArguments, TimePeriodViewModel.SelectedAggregate);
                            }
                            else
                            {
                                _hdaFileExportService.WriteAsciiFileOrganizeRowByRow(fileName, columnSeparator, ListViewModel.DisplayOnlyFirstTime, ListViewModel.DisplayQuality, ListViewModel.GetHistoricalProperties(), historicalArguments, TimePeriodViewModel.SelectedAggregate);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _interactionService.ResultService.ReportResult(new Result("Export hda file failed!", e.Message));
                }
            }
        }

        private void AddItemsToCurrentHistoryView(IObjectId[] obj)
        {
            _eventContext.ContextualEventAggregator.GetEvent<ObjectsAddedToViewEvent>().Publish(obj);
        }

        private void SolutionExplorerSelectionChanged(SolutionExplorerSelection obj)
        {
             _eventContext.ContextualEventAggregator.GetEvent<ObjectsAddedToViewEvent>().Publish(obj.Selection.ToArray());
        }
        private void UnsubscribeEvents()
        {
            _eventAggregator.GetEvent<SolutionExplorerSelectionChangedEvent>().Unsubscribe(
                    _solutionSelectionChangedToken);
            _eventAggregator.GetEvent<AddItemsToCurrentHistoryViewEvent>().Unsubscribe(_addItemsToCurrentHistoryViewToken);
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

        public void Write(IApplicationStateWriter writer)
        {
            writer.Write("TimePeriod", TimePeriodViewModel);
            writer.Write("ListView", ListViewModel);
            writer.Write("Chart", ChartModel);
        }

        public void Read(IApplicationStateReader reader)
        {
            reader.Read("TimePeriod", TimePeriodViewModel);
            reader.Read("ListView", ListViewModel);
            reader.Read("Chart", ChartModel);
        }


        public void Restored()
        {
        }
    }
}
