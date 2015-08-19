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

        private readonly IObjectServiceOperations _objectServiceOperations;
        private readonly IInteractionService _interactionService;
        private readonly IHistoricalTimeUtility _historicalTimeUtility;
        private readonly IValueFormatter _valueFormatter;
        private IApplicationProperties _appliationProperties;

        private ThisAddIn _thisAddIn = ThisAddIn.G_ThisAddIn;
        private MainRegionViewModel _mainViewModel;

        private IResult<IHistoricalTime> startTime;
        private IResult<IHistoricalTime> endTime;

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

        public void WriteExcelTest()
        {
            WriteTest();
        }

        public bool ExportDataToExcel()
        {
            var excelViewModel = new ExportExcelDialogViewModel(_startCol,
                                                            _isIncludeTimestamps, _isTimestampsInFirstCol, 
                                                            _isTimestampsInLocalZone, _isQualityInSeperateCol);
            var excelDialog = new ExportExcelDialog(excelViewModel);
            var r = excelDialog.ShowDialog();
            int rt = 0;

            if (r.HasValue && r.Value)
            {
                _startCol = excelViewModel.SelectedStartInColumn.Col;
                _isIncludeTimestamps = excelViewModel.IsIncludeTimestamps;
                _isTimestampsInFirstCol = excelViewModel.IsTimestampsInFirstCol;
                _isTimestampsInLocalZone = excelViewModel.IsTimestampsInLocalZone;
                _isQualityInSeperateCol = excelViewModel.IsQuelityInSeperateCol;

                //WriteDataTable(_mainViewModel.ListViewModel, _mainViewModel.TimePeriodViewModel);
                rt = WriteEventlist(_mainViewModel.ListViewModel, _mainViewModel.TimePeriodViewModel);

                _thisAddIn.CloseBrowse();
                return true;
            }

            return false;
        }

        public bool UpdateDataToExcel()
        {
            UpdateExcelDialogViewModel viewModel = new UpdateExcelDialogViewModel(endTime, _isUseCurrentTime, _isAppendNewData);
            var updateDialog = new UpdateExcelDialog(viewModel);
            var r = updateDialog.ShowDialog();

            if (r.HasValue && r.Value)
            {
                _isUseCurrentTime = viewModel.IsUseCurrentTime;
                _isAppendNewData = viewModel.IsAppendNewData;

                if (viewModel.IsUseCurrentTime)
                    viewModel.NewTime = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss t\\M");

                var newtime = _historicalTimeUtility.Parse(viewModel.NewTime);
                if (newtime.Success)
                {
                    _mainViewModel.TimePeriodViewModel.EndTime = viewModel.NewTime;
                    if (!viewModel.IsAppendNewData)
                        WriteDataTable(_mainViewModel.ListViewModel, _mainViewModel.TimePeriodViewModel);
                    else
                    {
                        if (newtime.Value.AbsoluteTime > _actualEndtime)        //new time is newer than actualendtime
                        {
                            WriteDataTable(_mainViewModel.ListViewModel, _mainViewModel.TimePeriodViewModel);
                        }
                    }
                }
                
                return true;
            }

            return false;
        }

        private void WriteValue(Excel.Worksheet sheet, int row, int col, object value)
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

        public int WriteDataTable(HistoricalPropertyListViewModel listViewModel, 
                               HistoricalTimePeriodViewModel timePeriodViewModel)
        {
            Excel.Worksheet sheet = ((Excel.Worksheet)_thisAddIn.Application.ActiveWorkbook.ActiveSheet);
            sheet.Cells.Clear();

            var propertIds = listViewModel.GetHistoricalProperties();
            startTime = _historicalTimeUtility.Parse(timePeriodViewModel.StartTime);
            endTime = _historicalTimeUtility.Parse(timePeriodViewModel.EndTime);
            var historicalAggregate = timePeriodViewModel.SelectedAggregate;

            if (endTime.Success && startTime.Success && timePeriodViewModel.SelectedAggregate != null)
            {
                var historicalArguments = new HistoricalArguments(startTime.Value, endTime.Value, timePeriodViewModel.Resample, timePeriodViewModel.MaxValues);

                var objectInfoResutls = _objectServiceOperations.GetObjectInfos(propertIds.Select(a => a.GetContext()).ToArray());
                var objectInfos = objectInfoResutls.Where(a => a.Success).Select(a => a.Value).ToArray();

                int startrow = 1;
                int qcol = 0;
                int tcol = _startCol;
                int unitcols = 1;
                int col, row;

                if (objectInfos.Any())
                {
                    var serviceInfos = _objectServiceOperations.GetServiceInfos(propertIds.Select(a => a.GetContext()).ToArray());

                    var properties = propertIds.Select(a => new HistoricalPropertyRead(a, historicalAggregate.Id)).ToArray();
                    var result = _objectServiceOperations.GetHistoricalPropertyValues(historicalArguments, properties);

                    if (_isIncludeTimestamps && !_isTimestampsInFirstCol)
                        unitcols++;
                    if (_isQualityInSeperateCol)
                        unitcols++;

                    col = _startCol;
                    if (_isIncludeTimestamps)
                        col += 1;
                    else
                        tcol = 0;
                    if (_isQualityInSeperateCol)
                        col += 1;

                    for (int i = 0; i < objectInfos.Length; i++)
                    {
                        if (_isIncludeTimestamps && !_isTimestampsInFirstCol)
                            tcol = col - unitcols + 1;

                        if (_isQualityInSeperateCol)
                            qcol = col - 1;

                        //write Item ID
                        row = startrow;
                        WriteLabel(sheet, row, col, objectInfos[i].FullName, "Item ID");
                        if (tcol == _startCol)
                            WriteValue(sheet, row, tcol, "Item ID");

                        //write Item Description
                        row++;
                        WriteLabel(sheet, row, col, objectInfos[i].Description, "Item Description");
                        if (tcol == _startCol)
                            WriteValue(sheet, row, tcol, "Item Description");

                        //write Engineering Unit
                        row++;
                        WriteLabel(sheet, row, col, "", "Engineering Unit");
                        if (tcol == _startCol)
                            WriteValue(sheet, row, tcol, "Engineering Unit");

                        string link = serviceInfos[i].Value.Name;
                        string[] s = link.Split('/');
                        //write Data Source
                        row++;
                        WriteLabel(sheet, row, col, s[s.Length - 1], "Data Source"); 
                        if (tcol == _startCol)
                            WriteValue(sheet, row, tcol, "Data Source");

                        //write Location
                        row++;
                        WriteLabel(sheet, row, col, s[s.Length - 2], "Location");
                        if (tcol == _startCol)
                            WriteValue(sheet, row, tcol, "Location");

                        //write aggregation ID
                        row++;
                        WriteLabel(sheet, row, col, historicalAggregate.Id.ToString(), "Aggregation ID");
                        if (tcol == _startCol)
                            WriteValue(sheet, row, tcol, "Aggregation ID");

                        //Write aggretation name
                        row++;
                        WriteLabel(sheet, row, col, historicalAggregate.Name, "Aggregation Name");
                        if (tcol == _startCol)
                            WriteValue(sheet, row, tcol, "Aggregation Name");

                        //Write start time
                        row++;
                        if (startTime.Value.IsRelativeTime)
                            WriteTimeLabel(sheet, row, col, startTime.Value.RelativeTime, "Start Time");
                        else 
                        {
                            DateTime starttime;
                            if (_isTimestampsInLocalZone)
                                starttime = startTime.Value.AbsoluteTime.ToLocalTime();
                            else
                                starttime = startTime.Value.AbsoluteTime.ToUniversalTime();
                            WriteTimeLabel(sheet, row, col, starttime, "Start Time");
                        }
                        if (tcol == _startCol)
                            WriteValue(sheet, row, tcol, "Start Time");

                        //Write end time
                        row++;
                        if (endTime.Value.IsRelativeTime)
                            WriteTimeLabel(sheet, row, col, endTime.Value.RelativeTime, "End Time");
                        else
                        { 
                            DateTime endtime;
                            if (_isTimestampsInLocalZone)
                                endtime = endTime.Value.AbsoluteTime.ToLocalTime();
                            else
                                endtime = endTime.Value.AbsoluteTime.ToUniversalTime();
                            WriteTimeLabel(sheet, row, col, endtime, "End Time");
                        }
                        if (tcol == _startCol)
                            WriteValue(sheet, row, tcol, "End Time");

                        //Write resample intervals
                        row++;
                        WriteLabel(sheet, row, col, timePeriodViewModel.ReadInterval, "Resample interval(in seconds)");
                        if (tcol == _startCol)
                            WriteValue(sheet, row, tcol, "Resample interval(in seconds)");

                        //Write time zone
                        row++;
                        string timezone;
                        if (_isTimestampsInLocalZone)
                            timezone = "Local time";
                        else
                            timezone = "UTC time";
                        WriteLabel(sheet, row, col, timezone, "Timestamps time zone");
                        if (tcol == _startCol)
                            WriteValue(sheet, row, tcol, "Timestamps time zone");

                        //Write space
                        row++;

                        //Write labels
                        row++;
                        WriteValue(sheet, row, col, "Values");
                        if (tcol > 0)
                            WriteValue(sheet, row, tcol, "Timestamps");
                        if (qcol > 0)
                            WriteValue(sheet, row, qcol, "Qualities"); 
                        
                        //Write timezone
                        row++;
                        if (tcol > 0) 
                            WriteValue(sheet, row, tcol, "(" + timezone + ")");

                        //Write value
                        row++;
                        if (result[i].Success)
                        {
                            var v = result[i].Value;
                            WriteDataValue(sheet, row, col, tcol, qcol, v);
                        }

                        //col operate
                        col += unitcols;
                        if (_isIncludeTimestamps && _isTimestampsInFirstCol)
                            tcol = 0;
                    }
                }
            }

            sheet.Columns.AutoFit();
            return 0;
        }

        private void WriteDataValue(Excel.Worksheet sheet, int row, int col, int tcol, int qcol,
                            IHistoricalPropertyValue v)
        {
            if (v.Values.Length == 0)
            {
                WriteValue(sheet, row, col, "<empty dataset>");
                if (tcol > 0)
                    WriteValue(sheet, row, tcol, "<empty dataset>");
                if (qcol > 0)
                    WriteValue(sheet, row, qcol, "<empty dataset>");
            }

            for (int j = 0; j < v.Values.Length; j++)
            {
                //visualize value, timestamps and qualities.
                WriteValue(sheet, row, col, v.Values[j].Value);

                if (tcol > 0)
                {
                    DateTime dt;
                    if (_isTimestampsInLocalZone)
                        dt = v.Values[j].Time.ToLocalTime();
                    else
                        dt = v.Values[j].Time.ToUniversalTime();
                    WriteTime(sheet, row, tcol, dt);
                }

                if (qcol > 0)
                    WriteValue(sheet, row, qcol, v.Values[j].Quality.Quality);

                if (j == v.Values.Length - 1)
                {
                    if (_actualEndtime < v.Values[j].Time)
                        _actualEndtime = v.Values[j].Time;
                }

                row++;
            }
        }

        public int WriteEventlist(HistoricalPropertyListViewModel listViewModel,
                               HistoricalTimePeriodViewModel timePeriodViewModel)
        {
            Excel.Worksheet sheet = ((Excel.Worksheet)_thisAddIn.Application.ActiveWorkbook.ActiveSheet);
            sheet.Cells.Clear();

            var propertIds = listViewModel.GetHistoricalProperties();
            startTime = _historicalTimeUtility.Parse(timePeriodViewModel.StartTime);
            endTime = _historicalTimeUtility.Parse(timePeriodViewModel.EndTime);
            var historicalAggregate = timePeriodViewModel.SelectedAggregate;

            if (endTime.Success && startTime.Success && timePeriodViewModel.SelectedAggregate != null)
            {
                var historicalArguments = new HistoricalArguments(startTime.Value, endTime.Value, timePeriodViewModel.Resample, timePeriodViewModel.MaxValues);

                var objectInfoResutls = _objectServiceOperations.GetObjectInfos(propertIds.Select(a => a.GetContext()).ToArray());
                var objectInfos = objectInfoResutls.Where(a => a.Success).Select(a => a.Value).ToArray();

                int row = 1;
                int tcol = 0, qcol = 0;
                int lcol = 1, vcol = 2;
                if (_isDisplayTime)
                {
                    tcol = 1;
                    lcol++;
                    vcol++;
                }

                if (_isDisplayQuality)
                {
                    qcol = lcol + 1;
                    vcol = qcol + 1;
                }

                if (objectInfos.Any())
                {
                    var serviceInfos = _objectServiceOperations.GetServiceInfos(propertIds.Select(a => a.GetContext()).ToArray());

                    var properties = propertIds.Select(a => new HistoricalPropertyRead(a, historicalAggregate.Id)).ToArray();
                    var result = _objectServiceOperations.GetHistoricalPropertyValues(historicalArguments, properties);

                    WriteValue(sheet, row, vcol, "Value");
                    WriteValue(sheet, row, lcol, "Item ID");
                    if (qcol > 0)
                        WriteValue(sheet, row, qcol, "Quality");
                    if (tcol > 0)
                        WriteValue(sheet, row, tcol, "Time");
                    row++;

                    int[] indexes = new int[result.Length];
                    for (int i = 0; i < indexes.Length; i++)
                    {
                        indexes[i] = 0;
                    }

                    while (true)
                    {
                        IHistoricalValue historicalValue = null;
                        int t = 0;
                        for (int i = 0; i < indexes.Length; ++i)
                        {
                            if (indexes[i] < result[i].Value.Values.Length)
                            {
                                historicalValue = result[i].Value.Values[indexes[i]];
                                t = i;
                                break;
                            }
                        }

                        if (historicalValue == null)
                        {
                            break;
                        }

                        for (int i = t + 1; i < indexes.Length; ++i)
                        {
                            if (indexes[i] < result[i].Value.Values.Length)
                            {
                                if (result[i].Value.Values[indexes[i]].Time < historicalValue.Time)
                                {
                                    historicalValue = result[i].Value.Values[indexes[i]];
                                    t = i;
                                }
                            }
                        }

                        var formattedTime = _valueFormatter.Format(historicalValue.Time);
                        var formattedValue = _valueFormatter.Format(historicalValue.Value);

                        WriteValue(sheet, row, vcol, historicalValue.Value);
                        WriteValue(sheet, row, lcol, objectInfos[t].Name);

                        if (tcol > 0)
                        {
                            DateTime dt;
                            if (_isTimestampsInLocalZone)
                                dt = historicalValue.Time.ToLocalTime();
                            else
                                dt = historicalValue.Time.ToUniversalTime();
                            WriteTime(sheet, row, tcol, dt);
                        }

                        if (qcol > 0)
                        {
                            WriteValue(sheet, row, qcol, historicalValue.Quality.Quality);
                            //file.Write(historicalValue.Quality.Quality);
                            //file.Write(columnSeparator);
                        }

                        row++;
                        indexes[t]++;
                        if (row > 5000)
                        {
                            return 1;
                        }
                    }

                    sheet.Columns.AutoFit();
                }
            }

            return 0;
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
