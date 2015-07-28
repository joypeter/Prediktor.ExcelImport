using Prediktor.Configuration.BaseTypes.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Excel;

namespace Prediktor.ExcelImport
{
    class ExcelExportService
    {
        public void WriteExcelTest(Excel.Worksheet sheet)
        {
            sheet.Select();
            int cols = 10;

            sheet.Cells.ColumnWidth = 30;
            sheet.Range["A1"].ColumnWidth = 14;

            sheet.Range["A13"].Value2 = "Timestamps";
            sheet.Range["A14"].Value2 = "(Local time)";

            sheet.Range["A13"].AddComment("ssss");

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

            for (int row = 15; row < 30; row++)
            {
                sheet.Cells[row, 1] = DateTime.Now.ToString("M/d/yyyy h:mm");
                for (int col = 2; col < cols; col++)
                {
                    //sheet.Cells.get_Offset(row, col).Value2 = "row" + row.ToString() + ":" + "col" + col.ToString();
                    //sheet.Cells[row, col] = "row" + row.ToString() + ":" + "col" + col.ToString() + "many other";
                    sheet.Cells[1, col] = "ApisLogger1.ApisWorker1.Signal" + ((col-1)*100/71).ToString();
                    Excel.Range rg = sheet.Range[sheet.Cells[1, col], sheet.Cells[1, col]];
                    rg.Value = "dss0";
                        //.AddComment("Item ID");
                    sheet.Cells[4, col] = "Prediktor.ApisOPCHDAServer.1";
                    sheet.Cells[13, col] = "Local time";
                    sheet.Cells[13, col] = "Values";
                    sheet.Cells[row, col] = row / col;
                }
            }

            //Excel.Range rg = sheet.Range[sheet.Cells[3, 2], sheet.Cells[3, 3]];
            //rg.Value = "dddd";

            Excel.Range rg2 = sheet.Range[sheet.Cells[10, 2], sheet.Cells[10, 2]];
            rg2.AddComment("sss");
        }

        public void WriteExcelOrganizeAsTable(Excel.Worksheet sheet, bool displayOnlyFirstTime, bool displayQuality, 
                                              IPropertyId[] propertIds, IHistoricalArguments historicalArguments, 
                                              IHistoricalAggregate historicalAggregate)
        {
            /*//using (var file = new System.IO.StreamWriter(fileName))
            //{

            var objectInfoResutls = _objectServiceOperations.GetObjectInfos(propertIds.Select(a => a.GetContext()).ToArray());
            var objectInfos = objectInfoResutls.Where(a => a.Success).Select(a => a.Value).ToArray();

                file.Write(string.Format("% Start time (local timezone):{0}; End time (local timezone): {1}",
                    historicalArguments.StartTime.IsRelativeTime ? historicalArguments.StartTime.RelativeTime : historicalArguments.StartTime.AbsoluteTime.ToLocalTime().ToString(),
                    historicalArguments.EndTime.IsRelativeTime ? historicalArguments.EndTime.RelativeTime : historicalArguments.EndTime.AbsoluteTime.ToLocalTime().ToString()));

            //sheet.Cells[]

            //    file.WriteLine();

                if (objectInfos.Any())
                {
                    file.Write("% ");
                    for (int i = 0; i < objectInfos.Length; ++i)
                    {
                        //Service

                        //ID
                        file.Write(objectInfos[i].Name);

                        //Time


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
