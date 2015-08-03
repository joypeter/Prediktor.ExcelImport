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
                //WriteTest();
                //WriteExcel("", "", _mainViewModel.ListViewModel.GetHistoricalProperties(),
                //    _mainViewModel.TimePeriodViewModel)
                WriteExcel("", "", excelViewModel, _mainViewModel.ListViewModel, _mainViewModel.TimePeriodViewModel);
                return true;
            }

            return false;
        }

        public void WriteExcel(string computer, string dataSource,
                               ExportExcelDialogViewModel excelViewModel,
                               HistoricalPropertyListViewModel listViewModel, 
                               HistoricalTimePeriodViewModel timePeriodViewModel)
        {
            Excel.Worksheet sheet = ((Excel.Worksheet)_thisAddIn.Application.ActiveWorkbook.ActiveSheet);
            sheet.Cells.Clear();

            var propertIds = listViewModel.GetHistoricalProperties();
            var endTime = _historicalTimeUtility.Parse(timePeriodViewModel.EndTime);
            var startTime = _historicalTimeUtility.Parse(timePeriodViewModel.StartTime);
            var historicalAggregate = timePeriodViewModel.SelectedAggregate;

            bool isIncludeTimestamps = excelViewModel.IsIncludeTimestamps;
            bool isTimestampsInFirstCol = excelViewModel.IsTimestampsInFirstCol;
            bool isTimestampsInLocalZone = excelViewModel.IsTimestampsInLocalZone;
            bool isQuelityInSeperateCol = excelViewModel.IsQuelityInSeperateCol;

            if (endTime.Success && startTime.Success && timePeriodViewModel.SelectedAggregate != null)
            {
                var historicalArguments = new HistoricalArguments(startTime.Value, endTime.Value, timePeriodViewModel.Resample, timePeriodViewModel.MaxValues);

                var objectInfoResutls = _objectServiceOperations.GetObjectInfos(propertIds.Select(a => a.GetContext()).ToArray());
                var objectInfos = objectInfoResutls.Where(a => a.Success).Select(a => a.Value).ToArray();

                var serviceInfos = _objectServiceOperations.GetServiceInfos(propertIds.Select(a => a.GetContext()).ToArray());

                var properties = propertIds.Select(a => new HistoricalPropertyRead(a, historicalAggregate.Id)).ToArray();
                var result = _objectServiceOperations.GetHistoricalPropertyValues(historicalArguments, properties);

                int startrow = 1;
                int startcol = 1;
                int qcol = 0;
                int tcol = startcol;
                int unitcols = 1;
                int col, row;

                if (objectInfos.Any())
                {
                    if (isIncludeTimestamps && !isTimestampsInFirstCol)
                        unitcols++;
                    if (isQuelityInSeperateCol)
                        unitcols++;

                    col = startcol;
                    if (isIncludeTimestamps)
                        col += 1;
                    else
                        tcol = 0;
                    if (isQuelityInSeperateCol)
                        col += 1;

                    for (int i = 0; i < objectInfos.Length; i++)
                    {
                        if (isIncludeTimestamps && !isTimestampsInFirstCol)
                            tcol = col - unitcols + 1;

                        if (isQuelityInSeperateCol)
                            qcol = col - 1;

                        //write Item ID
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

                        string link = serviceInfos[i].Value.Name;
                        string[] s = link.Split('/');
                        //write Data Source
                        row++;
                        sheet.Cells[row, col] = s[s.Length - 1];
                        sheet.Range[sheet.Cells[row, col], sheet.Cells[row, col]].AddComment("Data Source");

                        //write Location
                        row++;
                        sheet.Cells[row, col] = s[s.Length - 2];
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
                        //historicalArguments.StartTime.AbsoluteTime.ToLocalTime().ToString()
                        if (isTimestampsInLocalZone)
                            sheet.Cells[row, col] = startTime.Value.AbsoluteTime.ToLocalTime();
                        else
                            sheet.Cells[row, col] = startTime.Value.AbsoluteTime.ToUniversalTime();
                        sheet.Range[sheet.Cells[row, col], sheet.Cells[row, col]].NumberFormatLocal = "m/d/yyyy hh:mm";
                        sheet.Range[sheet.Cells[row, col], sheet.Cells[row, col]].AddComment("Start Time");

                        //Write end time
                        row++;
                        if (isTimestampsInLocalZone)
                            sheet.Cells[row, col] = endTime.Value.AbsoluteTime.ToLocalTime();
                        else
                            sheet.Cells[row, col] = endTime.Value.AbsoluteTime.ToUniversalTime();
                        //sheet.Cells[row, col] = _valueFormatter.Format(endTime.Value.AbsoluteTime);
                        sheet.Range[sheet.Cells[row, col], sheet.Cells[row, col]].NumberFormatLocal = "m/d/yyyy hh:mm";
                        sheet.Range[sheet.Cells[row, col], sheet.Cells[row, col]].AddComment("End Time");

                        //Write resample intervals
                        row++;
                        sheet.Cells[row, col] = timePeriodViewModel.ReadInterval;
                        sheet.Range[sheet.Cells[row, col], sheet.Cells[row, col]].AddComment("Resample interval(in seconds)");

                        //Write time zone
                        row++;
                        if (isTimestampsInLocalZone)
                            sheet.Cells[row, col] = "Local time";
                        else
                            sheet.Cells[row, col] = "UTC time";
                        sheet.Range[sheet.Cells[row, col], sheet.Cells[row, col]].AddComment("Timestamps time zone");

                        //Write space
                        row++;

                        //Write labels
                        row++;
                        sheet.Cells[row, col] = "Values";
                        if (tcol > 0)
                            sheet.Cells[row, tcol] = "Timestamps";
                        if (qcol > 0)
                            sheet.Cells[row, qcol] = "Qualities";

                        //Write timezone
                        row++;
                        if (tcol > 0) 
                        { 
                            if (isTimestampsInLocalZone)
                                sheet.Cells[row, tcol] = "(Local time)";
                            else
                                sheet.Cells[row, tcol] = "(UTC time)";
                        }

                        //Write value
                        row++;
                        if (result[i].Success)
                        {
                            var v = result[i].Value;
                            if (v.Values.Length == 0)
                            {
                                sheet.Cells[row, col] = "<empty dataset>";
                                if (tcol > 0)
                                    sheet.Cells[row, tcol] = "<empty dataset>";
                                if (qcol > 0)
                                    sheet.Cells[row, qcol] = "<empty dataset>";
                            }

                            for (int j = 0; j < v.Values.Length; j++ )
                            {
                                //formattedTime = _valueFormatter.Format(v.Values[j].Time.ToLocalTime());
                                //formattedValue = _valueFormatter.Format(v.Values[j].Value);

                                //visualize value, timestamps and qualities.
                                sheet.Cells[row, col] = v.Values[j].Value;

                                if (tcol > 0) 
                                {
                                    sheet.Range[sheet.Cells[row, tcol], sheet.Cells[row, tcol]].NumberFormatLocal = "m/d/yyyy hh:mm";
                                    if (isTimestampsInLocalZone)
                                        sheet.Cells[row, tcol] = v.Values[j].Time.ToLocalTime();
                                    else
                                        sheet.Cells[row, tcol] = v.Values[j].Time.ToUniversalTime();
                                }
                                
                                if (qcol > 0)
                                    sheet.Cells[row, qcol] = v.Values[j].Quality.Quality;

                                row++;
                            }
                        }

                        //col operate
                        col += unitcols;
                        if (isIncludeTimestamps && isTimestampsInFirstCol)
                            tcol = 0;
                    }
                }
            }

            sheet.Columns.AutoFit();

            _thisAddIn.CloseBrowse();
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
