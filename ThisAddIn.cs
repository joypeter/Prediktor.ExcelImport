using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Excel;

namespace Prediktor.ExcelImport
{
    public partial class ThisAddIn
    {
        private Ribbon Ribbon;

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            //Git test
            DialogManager.Current.Initialize();

            //Excel.Workbook newWorkbook = this.Application.Workbooks.Add(missing);

            //Excel.Worksheet newWorksheet;
            //newWorksheet = (Excel.Worksheet)this.Application.Worksheets.Add(
            //    missing, missing, missing, missing);
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            Ribbon = new Ribbon();
            Ribbon.TestEvent += new System.EventHandler(ThisAddIn_Test);
            return Globals.Factory.GetRibbonFactory().CreateRibbonManager(new Microsoft.Office.Tools.Ribbon.IRibbonExtension[] { Ribbon });
        }

        /* Test writing excel */
        public void ThisAddIn_Test(object sender, System.EventArgs e)
        {
            //Excel.Worksheet newWorksheet;
            //newWorksheet = (Excel.Worksheet)this.Application.Worksheets.Add(
            //    missing, missing, missing, missing);
            Excel.Worksheet sheet = ((Excel.Worksheet)this.Application.ActiveWorkbook.Sheets[1]);

            ExcelExportService excelService = new ExcelExportService();
            excelService.WriteExcelTest(sheet);
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
