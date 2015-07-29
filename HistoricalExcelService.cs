using Prediktor.Configuration.BaseTypes.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Excel;
using Prediktor.Configuration.BaseTypes.Implementation;
using Prediktor.ExcelImport.ViewModels;
using System.Windows.Forms;
using Prediktor.ExcelImport.Views;
using Prediktor.Carbon.Configuration.ViewModels;
using Prediktor.Configuration.Definitions;
using Prediktor.Carbon.Infrastructure.Definitions;
using Prediktor.Carbon.Configuration.Definitions.Events;

namespace Prediktor.ExcelImport
{
    public class HistoricalExcelService
    {
        private readonly IObjectServiceOperations _objectServiceOperations;
        private readonly IInteractionService _interactionService;
        private readonly IHistoricalTimeUtility _historicalTimeUtility;
        private readonly IValueFormatter _valueFormatter;
        

        private ThisAddIn _thisAddIn = ThisAddIn.G_ThisAddIn;
        private MainRegionViewModel _mainViewModel;

        //this constructor is for testing
        public HistoricalExcelService()
        {
        }

        public HistoricalExcelService(MainRegionViewModel main,
            IEventContext eventContext, 
            IObjectServiceOperations objectServiceOperations,
            IInteractionService interactionService,
            IHistoricalTimeUtility historicalTimeUtility, 
            IValueFormatter valueFormatter)
        {
            _mainViewModel = main;
            _historicalTimeUtility = historicalTimeUtility;
            _valueFormatter = valueFormatter;
            _objectServiceOperations = objectServiceOperations;
            _interactionService = interactionService;
        }

        public void WriteExcelTest()
        {
            WriteTest();
        }

        public bool ExportDataToExcel()
        {
            var excelViewModel = new ExportExcelDialogViewModel();
            var excelDialog = new ExportExcelDialog(excelViewModel);
            var r = excelDialog.ShowDialog();

            if (r.HasValue && r.Value)
            {
                WriteTest();
                //WriteExcel("", "", _mainViewModel.ListViewModel.GetHistoricalProperties(),
                //    _mainViewModel.TimePeriodViewModel)
                return true;
            }

            return false;
        }

        private void WriteExcel(ExportExcelDialogViewModel excelViewModel,
                               HistoricalPropertyListViewModel listViewModel, 
                                HistoricalPropertyListViewModel periodViewModel)
        {
            Excel.Worksheet sheet = ((Excel.Worksheet)_thisAddIn.Application.ActiveWorkbook.Sheets[1]);
            int startrow = 1;
            int startcol = 1;
            int col, row;

            var propertIds = listViewModel.GetHistoricalProperties();
            var objectInfoResutls = _objectServiceOperations.GetObjectInfos(propertIds.Select(a => a.GetContext()).ToArray());
            var objectInfos = objectInfoResutls.Where(a => a.Success).Select(a => a.Value).ToArray();
        }

        private void WriteTest()
        {
            Excel.Worksheet sheet = ((Excel.Worksheet)_thisAddIn.Application.ActiveWorkbook.Sheets[1]);

            sheet.Select();
            sheet.Cells.Clear();
            int signals = 10;
            int timerows = 30;
            int startcol = 2;

            sheet.Cells.ColumnWidth = 30;
            sheet.Range["A1"].ColumnWidth = 14;

            sheet.Range["A13"].Value2 = "Timestamps";
            sheet.Range["A14"].Value2 = "(Local time)";

            //sheet.Range["A13"].AddComment("ssss");

            //sheet.Rows[1] = "ddd";
            //sheet.Cells.get_Offset(1, 1).Value2 = "test";
            //sheet.Cells[1, 1] = "ddd";
            //sheet.Range[1, 13].Value2 = "ddd";

            //sheet.Rows.Width = 40;
            //sheet.Cells.NumberFormat = @"m/d/yyyy h:mm";

            //sheet.get_Range("B1", "B5").Value2 = 2312;
            //sheet.get_Range("1:1").AddComment("ddd");
            //sheet.Range["B2"].AddComment("Item ID");
            //Excel.Range rg = sheet.get_Range(sheet.Cells[1, 2], sheet.Cells[1, 3]); invalid
            //rg.Value = 3;
            //sheet.Columns[Type.Missing, "1:2"].

            for (int row = 15; row < timerows + 15; row++)
            {
                sheet.Cells[row, 1] = DateTime.Now.ToString("M/d/yyyy h:mm");
            }

            for (int col = startcol; col < startcol + signals; col++)
            {
                sheet.Cells[1, col] = "ApisLogger1.ApisWorker1.Signal" + (col - startcol + 1).ToString();
                sheet.Range[sheet.Cells[1, col], sheet.Cells[1, col]].AddComment("Item ID");
                sheet.Cells[4, col] = "Prediktor.ApisOPCHDAServer.1";
                sheet.Cells[13, col] = "Local time";
                sheet.Cells[13, col] = "Values";

                for (int row = 15; row < timerows + 15; row++)
                {
                    //sheet.Cells.get_Offset(row, col).Value2 = "row" + row.ToString() + ":" + "col" + col.ToString();
                    //sheet.Cells[row, col] = "row" + row.ToString() + ":" + "col" + col.ToString() + "many other";

                    //Excel.Range rg = 
                    //rg.Value = "dss0";
                    //.AddComment("Item ID");
                    sheet.Cells[row, col] = ((col - 1) * 100 / 71).ToString();
                }

            }

            //Excel.Range rg = sheet.Range[sheet.Cells[3, 2], sheet.Cells[3, 3]];
            //rg.Value = "dddd";

            Excel.Range rg2 = sheet.Range[sheet.Cells[10, 2], sheet.Cells[10, 2]];
            rg2.AddComment("sss");
        }

        private void Export()
        {
            //var endTime = _historicalTimeUtility.Parse("");
            //var startTime = _historicalTimeUtility.Parse("");
            //var historicalArguments = new HistoricalArguments(startTime.Value, endTime.Value, 1, 1);
            //var viewModel = new ExportDialogViewModel(_interactionService);
            //var exportDialog = new ExportDialog(viewModel);
            //var r = exportDialog.ShowDialog();
            //if (r.HasValue && r.Value)
            //{
            //    try
            //    {
            //        string columnSeparator = "\t";
            //        if (viewModel.IsOtherColumnSeparator && !string.IsNullOrEmpty(viewModel.ColumnSeparator))
            //        {
            //            columnSeparator = viewModel.ColumnSeparator;
            //        }

            //        string fileName = GetFileName(viewModel);

            //        var endTime = _historicalTimeUtility.Parse(TimePeriodViewModel.EndTime);
            //        var startTime = _historicalTimeUtility.Parse(TimePeriodViewModel.StartTime);
            //        if (endTime.Success && startTime.Success && TimePeriodViewModel.SelectedAggregate != null)
            //        {
                        //var historicalArguments = new HistoricalArguments(startTime.Value, endTime.Value, TimePeriodViewModel.Resample, TimePeriodViewModel.MaxValues);

            //            if (viewModel.IsRowEventList)
            //            {
            //                _hdaFileExportService.WriteAsciiFileOrganizeAsEventList(fileName, columnSeparator, EventListViewModel.DisplayQuality, ListViewModel.GetHistoricalProperties(), historicalArguments, TimePeriodViewModel.SelectedAggregate);
            //            }
            //            else
            //            {
            //                if (!viewModel.IsOrganizeDataRowByRow)
            //                {
            //                    _hdaFileExportService.WriteAsciiFileOrganizeAsTable(fileName, columnSeparator, ListViewModel.DisplayOnlyFirstTime, ListViewModel.DisplayQuality, ListViewModel.GetHistoricalProperties(), historicalArguments, TimePeriodViewModel.SelectedAggregate);
            //                }
            //                else
            //                {
            //                    _hdaFileExportService.WriteAsciiFileOrganizeRowByRow(fileName, columnSeparator, ListViewModel.DisplayOnlyFirstTime, ListViewModel.DisplayQuality, ListViewModel.GetHistoricalProperties(), historicalArguments, TimePeriodViewModel.SelectedAggregate);
            //                }
            //            }
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        _interactionService.ResultService.ReportResult(new Result("Export hda file failed!", e.Message));
            //    }
            //}
        }

        public void WriteExcel(string computer, string dataSource,
                               ExportExcelDialogViewModel excelViewModel,
                               HistoricalPropertyListViewModel listViewModel, 
                               HistoricalTimePeriodViewModel timePeriodViewModel)
        {
            Excel.Worksheet sheet = ((Excel.Worksheet)_thisAddIn.Application.ActiveWorkbook.Sheets[1]);

            var propertIds = listViewModel.GetHistoricalProperties();
            var endTime = _historicalTimeUtility.Parse(timePeriodViewModel.EndTime);
            var startTime = _historicalTimeUtility.Parse(timePeriodViewModel.StartTime);
            var historicalAggregate = timePeriodViewModel.SelectedAggregate;

            if (endTime.Success && startTime.Success && timePeriodViewModel.SelectedAggregate != null)
            {
                var historicalArguments = new HistoricalArguments(startTime.Value, endTime.Value, timePeriodViewModel.Resample, timePeriodViewModel.MaxValues);

                var objectInfoResutls = _objectServiceOperations.GetObjectInfos(propertIds.Select(a => a.GetContext()).ToArray());
                var objectInfos = objectInfoResutls.Where(a => a.Success).Select(a => a.Value).ToArray();

                var properties = propertIds.Select(a => new HistoricalPropertyRead(a, historicalAggregate.Id)).ToArray();
                var result = _objectServiceOperations.GetHistoricalPropertyValues(historicalArguments, properties);

                //file.Write(string.Format("% Start time (local timezone):{0}; End time (local timezone): {1}",
                //    historicalArguments.StartTime.IsRelativeTime ? historicalArguments.StartTime.RelativeTime : historicalArguments.StartTime.AbsoluteTime.ToLocalTime().ToString(),
                //    historicalArguments.EndTime.IsRelativeTime ? historicalArguments.EndTime.RelativeTime : historicalArguments.EndTime.AbsoluteTime.ToLocalTime().ToString()));

                //sheet.Cells[]

                //    file.WriteLine();

                int startrow = 1;
                int startcol = 1;
                int col, row;

                if (objectInfos.Any())
                {
                    for (int i = 0; i < objectInfos.Length; i++)
                    {
                        //write Item ID
                        col = i + startcol + 1;
                        row = startrow;
                        sheet.Cells[row, col] = objectInfos[i].FullName;
                        sheet.Range[sheet.Cells[row, col], sheet.Cells[row, col]].AddComment("Item ID");

                        //write Item Description
                        row++;
                        sheet.Cells[row, col] = objectInfos[i].Description;
                        sheet.Range[sheet.Cells[row, col], sheet.Cells[row, col]].AddComment("Item Description");

                        //write Engineering Unit
                        row++;
                        sheet.Cells[row, col] = "";
                        sheet.Range[sheet.Cells[row, col], sheet.Cells[row, col]].AddComment("Engineering Unit");

                        //write Data Source
                        row++;
                        sheet.Cells[row, col] = dataSource;
                        sheet.Range[sheet.Cells[row, col], sheet.Cells[row, col]].AddComment("Data Source");

                        //write Location
                        row++;
                        sheet.Cells[row, col] = computer;
                        sheet.Range[sheet.Cells[row, col], sheet.Cells[row, col]].AddComment("Location");

                        //write aggregation ID
                        row++;
                        sheet.Cells[row, col] = historicalAggregate.Id.ToString();
                        sheet.Range[sheet.Cells[row, col], sheet.Cells[row, col]].AddComment("Aggregation ID");

                        //Write aggretation name
                        row++;
                        sheet.Cells[row, col] = historicalAggregate.Name;
                        sheet.Range[sheet.Cells[row, col], sheet.Cells[row, col]].AddComment("Aggregation Name");

                        //Write start time
                        row++;
                        sheet.Cells[row, col] = historicalArguments.StartTime.AbsoluteTime;
                        sheet.Range[sheet.Cells[row, col], sheet.Cells[row, col]].AddComment("Start Time");

                        //Write end time
                        row++;
                        sheet.Cells[row, col] = historicalArguments.EndTime.AbsoluteTime;
                        sheet.Range[sheet.Cells[row, col], sheet.Cells[row, col]].AddComment("End Time");

                        //Write resample intervals
                        row++;
                        sheet.Cells[row, col] = historicalArguments.EndTime.AbsoluteTime;
                        sheet.Range[sheet.Cells[row, col], sheet.Cells[row, col]].AddComment("End Time");

                        //Write time zone
                        row++;
                        sheet.Cells[row, col] = "Local time";
                        sheet.Range[sheet.Cells[row, col], sheet.Cells[row, col]].AddComment("End Time");

                        //Write space
                        row++;

                        //Write labels
                        row++;
                        sheet.Cells[row, col] = "Values";

                        //Write space
                        row++;

                        //Write value
                        row++;
                        string formattedTime = string.Empty;
                        string quality = string.Empty;
                        string formattedValue = string.Empty;
                        if (result[i].Success)
                        {
                            var v = result[i].Value;
                            for (int j = 0; j < v.Values.Length; j++ )
                            {
                                formattedTime = _valueFormatter.Format(v.Values[j].Time);
                                formattedValue = _valueFormatter.Format(v.Values[j].Value);
                                quality = v.Values[j].Quality.Quality;

                                sheet.Cells[row, col] = formattedValue;
                                row++;
                            }
                        }

                        //if (!displayOnlyFirstTime || i == 0)
                        //{
                        //    file.Write(formattedTime);
                        //    file.Write(columnSeparator);
                        //}

                        //if (displayQuality)
                        //{
                        //    file.Write(quality);
                        //    file.Write(columnSeparator);
                        //}

                        //file.Write(formattedValue);
                        //file.Write(columnSeparator);
                    }
                }
            }

            /*        for (int i = 0; i < objectInfos.Length; ++i)
                    {
                        //Service

                        //ID
                        file.Write(objectInfos[i].Name);

                        //Time


                        //if (!displayOnlyFirstTime || i == 0)
                        //{
                        //    file.Write(columnSeparator);
                        //}

                        //if (displayQuality)
                        //{
                        //    file.Write(columnSeparator);
                        //}
                        //file.Write(columnSeparator);
                    }

                    file.WriteLine();

                    file.Write("% ");
                    for (int i = 0; i < objectInfos.Length; ++i)
                    {
                        file.Write(historicalAggregate.Name);
                        if (!displayOnlyFirstTime || i == 0)
                        {
                            file.Write(columnSeparator);
                        }

                        if (displayQuality)
                        {
                            file.Write(columnSeparator);
                        }
                        file.Write(columnSeparator);
                    }

                    file.WriteLine();

                    file.Write("% ");
                    for (int i = 0; i < objectInfos.Length; ++i)
                    {
                        if (!displayOnlyFirstTime || i == 0)
                        {
                            file.Write("Time UTC");
                            file.Write(columnSeparator);
                        }

                        if (displayQuality)
                        {
                            file.Write("Quality");
                            file.Write(columnSeparator);
                        }

                        file.Write("Value");
                        file.Write(columnSeparator);
                    }

                    file.WriteLine();
                }

                var properties = propertIds.Select(a => new HistoricalPropertyRead(a, historicalAggregate.Id)).ToArray();

                var result = _objectServiceOperations.GetHistoricalPropertyValues(historicalArguments, properties);

                int row = 0;
                bool quit = false;
                while (!quit)
                {
                    quit = true;
                    for (int i = 0; i < result.Length; i++)
                    {
                        string formattedTime = string.Empty;
                        string quality = string.Empty;
                        string formattedValue = string.Empty;

                        if (result[i].Success)
                        {
                            var v = result[i].Value;
                            if (row < v.Values.Length)
                            {
                                quit = false;
                                formattedTime = _valueFormatter.Format(v.Values[row].Time);
                                formattedValue = _valueFormatter.Format(v.Values[row].Value);
                                quality = v.Values[row].Quality.Quality;
                            }
                        }

                        if (!displayOnlyFirstTime || i == 0)
                        {
                            file.Write(formattedTime);
                            file.Write(columnSeparator);
                        }

                        if (displayQuality)
                        {
                            file.Write(quality);
                            file.Write(columnSeparator);
                        }

                        file.Write(formattedValue);
                        file.Write(columnSeparator);
                    }
                    row++;
                    file.WriteLine();
                }
            }*/
        }
    }
}
