using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Prediktor.Carbon.Infrastructure.Definitions;

namespace Prediktor.ExcelImport
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ImportDialog : Prediktor.Carbon.Infrastructure.Behaviors.Implementation.ThemedWindow
    {
        public ImportDialog()
        {
            InitializeComponent();
        }

        /*
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
        */
    }
}
