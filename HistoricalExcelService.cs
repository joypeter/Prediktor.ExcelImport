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
using Prediktor.Carbon.Configuration.Definitions.ModuleServices;
using Prediktor.Utilities;

namespace Prediktor.ExcelImport
{
    public class HistoricalExcelService
    {
        public static HistoricalExcelService Current;

        private readonly int MAX_ROWS = 50000;

        private readonly IObjectServiceOperations _objectServiceOperations;
        private readonly IInteractionService _interactionService;
        private readonly IHistoricalTimeUtility _historicalTimeUtility;
        private readonly IValueFormatter _valueFormatter;
        private IApplicationProperties _appliationProperties;

        private ThisAddIn _thisAddIn = ThisAddIn.G_ThisAddIn;
        private MainRegionViewModel _mainViewModel;

        //private IResult<IHistoricalTime> startTime;
        //private IResult<IHistoricalTime> endTime;

        private int _startCol = 1;
        private bool _isIncludeTimestamps = true;
        private bool _isTimestampsInFirstCol = false;
        private bool _isTimestampsInLocalZone = true;
        private bool _isQualityInSeperateCol = true;
        private bool _isUseCurrentTime = true;
        private bool _isAppendNewData = false;

        private bool _isDisplayTime = true;
        private bool _isDisplayQuality = true;

        private DateTime _actualEndtime = DateTime.MinValue;

        public HistoricalExcelService(MainRegionViewModel main,
            IEventContext eventContext, 
            IObjectServiceOperations objectServiceOperations,
            IInteractionService interactionService,
            IHistoricalTimeUtility historicalTimeUtility, 
            IValueFormatter valueFormatter,
            IApplicationProperties appliationProperties)
        {
            _mainViewModel = main;
            _historicalTimeUtility = historicalTimeUtility;
            _valueFormatter = valueFormatter;
            _objectServiceOperations = objectServiceOperations;
            _interactionService = interactionService;
            _appliationProperties = appliationProperties;
        }

        public void DataTableToExcel(bool displayTime, bool displayQuality, bool localTime,
                                     IHistoricalQuery[] queries, IHistoricalPropertyValue[] values)
        {
            int i = 0;
            int startrow = 1, row = 1;
            int qcol, tcol, vcol;
            int unitcols = 1;

            Excel.Worksheet sheet = ((Excel.Worksheet)_thisAddIn.Application.ActiveWorkbook.ActiveSheet);
            sheet.Cells.Clear();

            var objectInfoResults = _objectServiceOperations.GetObjectInfos(queries.Select(a => a.PropertyId.GetContext()).ToArray());
            var objectInfos = objectInfoResults.Where(a => a.Success).Select(a => a.Value).ToArray();
            var serviceInfoResults = _objectServiceOperations.GetServiceInfos(queries.Select(a => a.PropertyId.GetContext()).ToArray());
            var serviceInfos = serviceInfoResults.Where(a => a.Success).Select(a => a.Value).ToArray();

            vcol = _startCol;
            tcol = 0;
            qcol = 0;
            if (displayTime)
            {
                tcol = _startCol;
                vcol = tcol + 1;
                unitcols++;
            }
            if (displayQuality)
            {
                qcol = vcol + 1;
                unitcols++;
            }

            foreach (IHistoricalQuery query in queries)
            {
                row = startrow;

                row = WriteLabels(sheet, row, tcol, vcol, qcol, objectInfos[i], serviceInfos[i], query, localTime);

                //Write labels
                row++;
                WriteCommon(sheet, row, vcol, "Values");
                if (tcol > 0)
                    WriteCommon(sheet, row, tcol, "Timestamps");
                if (qcol > 0)
                    WriteCommon(sheet, row, qcol, "Qualities");

                //Write timezone
                row++;
                string timezone;
                if (localTime)
                    timezone = "Local time";
                else
                    timezone = "UTC time";
                if (tcol > 0)
                    WriteCommon(sheet, row, tcol, "(" + timezone + ")");

                vcol += unitcols;
                if (displayTime)
                    tcol = vcol - 1;
                if (displayQuality)
                    qcol = vcol + 1;

                i++;
            }

            vcol = _startCol;
            tcol = 0;
            qcol = 0;
            if (displayTime)
            {
                tcol = _startCol;
                vcol = tcol + 1;
            }
            if (displayQuality)
            {
                qcol = vcol + 1;
            }
            startrow = row + 1;

            foreach (IHistoricalPropertyValue value in values)
            {
                row = startrow;

                if (value.Values.Length == 0)
                {
                    WriteCommon(sheet, row, vcol, "<empty dataset>");
                    if (tcol > 0)
                        WriteCommon(sheet, row, tcol, "<empty dataset>");
                    if (qcol > 0)
                        WriteCommon(sheet, row, qcol, "<empty dataset>");
                }

                foreach (IHistoricalValue v in value.Values)
                {
                    WriteDataTableValue(sheet, row, tcol, vcol, qcol, v, localTime);
                    row++;
                }

                vcol += unitcols;
                if (displayTime)
                    tcol = vcol - 1;
                if (displayQuality)
                    qcol = vcol + 1;
            }

            sheet.Columns.AutoFit();
            _thisAddIn.CloseBrowse();
        }

        public void EventListToExcel(bool displayTime, bool displayQuality, bool localTime,
                                      IHistoricalQuery[] queries, IHistoricalPropertyValue[] historicalPropertyValues)
        {
            int i = 0;
            int startrow = 1, row = 1;
            int qcol, tcol, vcol, icol;

            Excel.Worksheet sheet = ((Excel.Worksheet)_thisAddIn.Application.ActiveWorkbook.ActiveSheet);
            sheet.Cells.Clear();

            var objectInfoResults = _objectServiceOperations.GetObjectInfos(queries.Select(a => a.PropertyId.GetContext()).ToArray());
            var objectInfos = objectInfoResults.Where(a => a.Success).Select(a => a.Value).ToArray();
            var serviceInfoResults = _objectServiceOperations.GetServiceInfos(queries.Select(a => a.PropertyId.GetContext()).ToArray());
            var serviceInfos = serviceInfoResults.Where(a => a.Success).Select(a => a.Value).ToArray();

            icol = _startCol;
            vcol = _startCol + 1;
            tcol = 0;
            qcol = 0;
            if (displayTime)
            {
                tcol = _startCol;
                icol = _startCol + 1;
                vcol = _startCol + 2;
            }
            if (displayQuality)
            {
                qcol = vcol + 1;
            }

            foreach (IHistoricalQuery query in queries)
            {
                row = startrow;

                row = WriteLabels(sheet, row, _startCol, _startCol + i + 1, 0, objectInfos[i], serviceInfos[i], query, localTime);

                //Write value labels
                row++;
                if (tcol > 0)
                    WriteCommon(sheet, row, tcol, "Timestamps");
                WriteCommon(sheet, row, icol, "Item ID");
                WriteCommon(sheet, row, vcol, "Values");
                if (qcol > 0)
                    WriteCommon(sheet, row, qcol, "Qualities");

                //Write timezone
                row++;
                string timezone;
                if (localTime)
                    timezone = "Local time";
                else
                    timezone = "UTC time";
                if (tcol > 0)
                    WriteCommon(sheet, row, tcol, "(" + timezone + ")");

                i++;
            }

            int[] indexes = new int[historicalPropertyValues.Length];
            for (i = 0; i < indexes.Length; i++)
            {
                indexes[i] = 0;
            }

            while (true)
            {
                IHistoricalValue historicalValue = null;
                int t = 0;
                for (i = 0; i < indexes.Length; ++i)
                {
                    if (indexes[i] < historicalPropertyValues[i].Values.Length)
                    {
                        historicalValue = historicalPropertyValues[i].Values[indexes[i]];
                        t = i;
                        break;
                    }
                }

                if (historicalValue == null)
                {
                    break;
                }

                for (i = t + 1; i < indexes.Length; ++i)
                {
                    if (indexes[i] < historicalPropertyValues[i].Values.Length)
                    {
                        if (historicalPropertyValues[i].Values[indexes[i]].Time < historicalValue.Time)
                        {
                            historicalValue = historicalPropertyValues[i].Values[indexes[i]];
                            t = i;
                        }
                    }
                }

                //var formattedTime = localTime ? _valueFormatter.Format(historicalValue.Time.ToLocalTime()) : _valueFormatter.Format(historicalValue.Time);
                //var formattedValue = _valueFormatter.Format(historicalValue.Value);

                WriteCommon(sheet, row, icol, objectInfos[t].Name);
                WriteNumber(sheet, row, vcol, historicalValue.Value);

                if (tcol > 0)
                {
                    DateTime dt;
                    if (localTime)
                        dt = historicalValue.Time.ToLocalTime();
                    else
                        dt = historicalValue.Time;
                    WriteTime(sheet, row, tcol, dt);
                }

                if (qcol > 0)
                {
                    WriteCommon(sheet, row, qcol, historicalValue.Quality.Quality);
                }

                row++;
                indexes[t]++;
                if (row > MAX_ROWS)
                    break;
            }
            sheet.Columns.AutoFit();
            _thisAddIn.CloseBrowse();
        }

        private int WriteLabels(Excel.Worksheet sheet,
            int row, int tcol, int vcol, int qcol,
            IObjectInfo objectInfo, IServiceInfo serviceInfo, IHistoricalQuery query,
            bool localTime)
        {
            //write Item ID
            WriteLabel(sheet, row, vcol, objectInfo.FullName, "Item ID");
            if (tcol == _startCol)
                WriteCommon(sheet, row, tcol, "Item ID");

            //write Item Description
            row++;
            WriteLabel(sheet, row, vcol, objectInfo.Description, "Item Description");
            if (tcol == _startCol)
                WriteCommon(sheet, row, tcol, "Item Description");

            //write Engineering Unit
            row++;
            WriteLabel(sheet, row, vcol, "", "Engineering Unit");
            if (tcol == _startCol)
                WriteCommon(sheet, row, tcol, "Engineering Unit");

            string link = serviceInfo.Name;
            string[] s = link.Split('/');
            //write Data Source
            row++;
            WriteLabel(sheet, row, vcol, s[s.Length - 1], "Data Source");
            if (tcol == _startCol)
                WriteCommon(sheet, row, tcol, "Data Source");

            //write Location
            row++;
            WriteLabel(sheet, row, vcol, s[s.Length - 2], "Location");
            if (tcol == _startCol)
                WriteCommon(sheet, row, tcol, "Location");

            //write aggregation ID
            row++;
            WriteLabel(sheet, row, vcol, query.Aggregate.Id.ToString(), "Aggregation ID");
            if (tcol == _startCol)
                WriteCommon(sheet, row, tcol, "Aggregation ID");

            //Write aggretation name
            row++;
            WriteLabel(sheet, row, vcol, query.Aggregate.Name, "Aggregation Name");
            if (tcol == _startCol)
                WriteCommon(sheet, row, tcol, "Aggregation Name");

            //Write start time
            row++;
            if (query.StartTime.IsRelativeTime)
                WriteTimeLabel(sheet, row, vcol, query.StartTime.RelativeTime, "Start Time");
            else
            {
                DateTime starttime;
                if (localTime)
                    starttime = query.StartTime.AbsoluteTime.ToLocalTime();
                else
                    starttime = query.StartTime.AbsoluteTime.ToUniversalTime();
                WriteTimeLabel(sheet, row, vcol, starttime, "Start Time");
            }
            if (tcol == _startCol)
                WriteCommon(sheet, row, tcol, "Start Time");

            //Write end time
            row++;
            if (query.EndTime.IsRelativeTime)
                WriteTimeLabel(sheet, row, vcol, query.EndTime.RelativeTime, "End Time");
            else
            {
                DateTime endtime;
                if (localTime)
                    endtime = query.EndTime.AbsoluteTime.ToLocalTime();
                else
                    endtime = query.EndTime.AbsoluteTime.ToUniversalTime();
                WriteTimeLabel(sheet, row, vcol, endtime, "End Time");
            }
            if (tcol == _startCol)
                WriteCommon(sheet, row, tcol, "End Time");

            //Write resample intervals
            row++;
            WriteLabel(sheet, row, vcol, query.Resample, "Resample interval(in seconds)");
            if (tcol == _startCol)
                WriteCommon(sheet, row, tcol, "Resample interval(in seconds)");

            //Write time zone
            row++;
            string timezone;
            if (localTime)
                timezone = "Local time";
            else
                timezone = "UTC time";
            WriteLabel(sheet, row, vcol, timezone, "Timestamps time zone");
            if (tcol == _startCol)
                WriteCommon(sheet, row, tcol, "Timestamps time zone");

            //Write space
            row++;

            return row;
        }

        private void WriteDataTableValue(Excel.Worksheet sheet,
            int row, int tcol, int vcol, int qcol,
            IHistoricalValue v, bool localTime)
        {
            WriteNumber(sheet, row, vcol, v.Value);

            if (tcol > 0)
            {
                DateTime dt;
                if (localTime)
                    dt = v.Time.ToLocalTime();
                else
                    dt = v.Time.ToUniversalTime();
                WriteTime(sheet, row, tcol, dt);
            }

            if (qcol > 0)
                WriteCommon(sheet, row, qcol, v.Quality.Quality);
        }

        private void WriteCommon(Excel.Worksheet sheet, int row, int col, object value)
        {
            //sheet.Range[sheet.Cells[row, col], sheet.Cells[row, col]].Clear();
            sheet.Cells[row, col] = value;
        }

        private void WriteLabel(Excel.Worksheet sheet, int row, int col, object value, string comment)
        {
            //sheet.Range[sheet.Cells[row, col], sheet.Cells[row, col]].Clear();
            sheet.Cells[row, col] = value;
            if (comment != null && !comment.Equals(""))
                sheet.Range[sheet.Cells[row, col], sheet.Cells[row, col]].AddComment(comment);
        }

        private void WriteTimeLabel(Excel.Worksheet sheet, int row, int col, object value, string comment)
        {
            //sheet.Range[sheet.Cells[row, col], sheet.Cells[row, col]].Clear();
            sheet.Range[sheet.Cells[row, col], sheet.Cells[row, col]].NumberFormatLocal = "m/d/yyyy hh:mm";
            sheet.Cells[row, col] = value;
            if (comment != null && !comment.Equals(""))
                sheet.Range[sheet.Cells[row, col], sheet.Cells[row, col]].AddComment(comment);
        }

        private void WriteTime(Excel.Worksheet sheet, int row, int col, DateTime value)
        {
            //sheet.Range[sheet.Cells[row, col], sheet.Cells[row, col]].Clear();
            sheet.Range[sheet.Cells[row, col], sheet.Cells[row, col]].NumberFormatLocal = "m/d/yyyy hh:mm";
            sheet.Cells[row, col] = value;
        }

        private void WriteNumber(Excel.Worksheet sheet, int row, int col, object value)
        {
            //sheet.Range[sheet.Cells[row, col], sheet.Cells[row, col]].NumberFormatLocal = "0";
            sheet.Cells[row, col] = value;
        }      

        private bool ExportExcelDialog()
        {
            var excelViewModel = new ExportExcelDialogViewModel(_startCol,
                                                            _isIncludeTimestamps, _isTimestampsInFirstCol,
                                                            _isTimestampsInLocalZone, _isQualityInSeperateCol);
            var excelDialog = new ExportExcelDialog(excelViewModel);
            var r = excelDialog.ShowDialog();

            if (r.HasValue && r.Value)
            {
                _startCol = excelViewModel.SelectedStartInColumn.Col;
                _isIncludeTimestamps = excelViewModel.IsIncludeTimestamps;
                _isTimestampsInFirstCol = excelViewModel.IsTimestampsInFirstCol;
                _isTimestampsInLocalZone = excelViewModel.IsTimestampsInLocalZone;
                _isQualityInSeperateCol = excelViewModel.IsQuelityInSeperateCol;

                //WriteDataTable(_mainViewModel.ListViewModel, _mainViewModel.TimePeriodViewModel);
                //rt = WriteEventlist(_mainViewModel.ListViewModel, _mainViewModel.TimePeriodViewModel);

                _thisAddIn.CloseBrowse();
                return true;
            }

            return false;
        }

        public bool UpdateDataToExcel()
        {
            //UpdateExcelDialogViewModel viewModel = new UpdateExcelDialogViewModel(endTime, _isUseCurrentTime, _isAppendNewData);
            //var updateDialog = new UpdateExcelDialog(viewModel);
            //var r = updateDialog.ShowDialog();

            //if (r.HasValue && r.Value)
            //{
            //    _isUseCurrentTime = viewModel.IsUseCurrentTime;
            //    _isAppendNewData = viewModel.IsAppendNewData;

            //    if (viewModel.IsUseCurrentTime)
            //        viewModel.NewTime = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss t\\M");

            //    var newtime = _historicalTimeUtility.Parse(viewModel.NewTime);
            //    if (newtime.Success)
            //    {
            //        //_mainViewModel.TimePeriodViewModel.EndTime = viewModel.NewTime;
            //        if (!viewModel.IsAppendNewData)
            //            ;
            //        //WriteDataTable(_mainViewModel.ListViewModel, _mainViewModel.TimePeriodViewModel);
            //        else
            //        {
            //            if (newtime.Value.AbsoluteTime > _actualEndtime)        //new time is newer than actualendtime
            //            {
            //                // WriteDataTable(_mainViewModel.ListViewModel, _mainViewModel.TimePeriodViewModel);
            //            }
            //        }
            //    }

            //    return true;
            //}

            return false;
        }

        public void WriteExcelTest()
        {
            WriteTest();
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
    }
}
