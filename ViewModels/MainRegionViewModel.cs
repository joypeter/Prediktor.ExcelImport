using Microsoft.Practices.Prism.Events;
using Prediktor.Carbon.Configuration.ViewModels;
using Prediktor.Carbon.Infrastructure.Definitions;
using Prediktor.Carbon.Configuration.Definitions.ModuleServices;
using Prediktor.Carbon.Configuration.Definitions.Events;
using Prediktor.Configuration.BaseTypes.Definitions;
using Prediktor.Configuration.OpcHda.Definitions.Service;
using Prediktor.Carbon.Configuration.Definitions.ViewModels;
using Prediktor.Configuration.Persistence.Definitions;
using Prediktor.Configuration.Definitions;
using System.Linq;
using System.Collections.Specialized;
using Microsoft.Practices.Prism.Commands;
using System.Collections.Generic;
using Prediktor.Configuration.BaseTypes.Implementation;

namespace Prediktor.ExcelImport
{
    public class MainRegionViewModel : HistoryExplorerViewModel
    {
        private IEventAggregator _eventAggregator;
        private IEventContext _eventContext;
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
            : base(eventContext, objectServiceOperations,
                  interactionService, historicalTimeUtility,
                  columnNameService, historicalColumnService,
                  serializationService, helpExtension,
                  valueFormatter, eventAggregator,
                  hdaFileExportService, documentationService)
        {
            _eventAggregator = eventAggregator;
            _eventContext = eventContext;

            if (HistoricalExcelService.Current == null)
                HistoricalExcelService.Current = new HistoricalExcelService(this,
                    eventContext, objectServiceOperations, interactionService, historicalTimeUtility, valueFormatter, appliationProperties);

            ItemsHistoricalTimePeriodViewModel.Items.CollectionChanged += Items_CollectionChanged;

            ListViewModel.ExportCommand = new DelegateCommand(ExportPropertyList);
            ListViewModel.ExportCommandText = interactionService.TranslatingService.GetSystemText("Import");

            EventListViewModel.ExportCommand = new DelegateCommand(ExportEventList);
            EventListViewModel.ExportCommandText = interactionService.TranslatingService.GetSystemText("Import");

            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            _eventAggregator.GetEvent<SolutionExplorerSelectionChangedEvent>().Subscribe(
                SolutionExplorerSelectionChanged, ThreadOption.UIThread);
        }

        private void SolutionExplorerSelectionChanged(SolutionExplorerSelection obj)
        {
            IObjectId[] objs = obj.Selection.ToArray();

            _eventContext.ContextualEventAggregator.GetEvent<ObjectsAddedToViewEvent>().Publish(obj.Selection.ToArray());
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Remove)
                return;

            List<IPropertyId> ids = new List<IPropertyId>();
            foreach (var item in e.OldItems)
            {
                //only publish event when it's the last property for a given item
                if (ItemsHistoricalTimePeriodViewModel.Items.Where(
                        a => a.PropertyId.Equals((item as ItemHistoricalInfo).PropertyId)
                    ).Any()
                    )
                    return;

                ids.Add((item as ItemHistoricalInfo).PropertyId);
            }
            _eventAggregator.GetEvent<RemovePropertiesFromViewEvent>().Publish(ids.ToArray());
        }

        private void ExportPropertyList()
        {
            if (HistoricalExcelService.Current != null)
                HistoricalExcelService.Current.ExportDataToExcel();
        }

        private void ExportEventList()
        {
            if (HistoricalExcelService.Current != null)
                HistoricalExcelService.Current.ExportDataToExcel();
        }
    }
}
