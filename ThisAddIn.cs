using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Excel;
using System.Windows;

namespace Prediktor.ExcelImport
{
    public partial class ThisAddIn
    {
        ExcelImportBootstrapper Bootstrapper;
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            System.Windows.Forms.Application.Idle += OnIdle;

            //
            //Application.

            //System.Windows.Forms.Application.re
            //this.
            //System.Windows.Forms.Application.Resources.Add("Telerik.Windows.Controls.Key", "Prediktor Telerik Application");
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            
        }

        private void OnIdle(Object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Idle -= OnIdle;
            Bootstrapper = new ExcelImportBootstrapper();
            Bootstrapper.Run();
        }


        public void Connnect()
        {
            Bootstrapper.Connect();
        }

        public void Browse()
        {
            Bootstrapper.Browse();
        }

        protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            Ribbon apisExcelImport = new Ribbon();
            apisExcelImport.TestEvent += ThisAddIn_Test;
            apisExcelImport.ConnectMethod = this.Connnect;
            apisExcelImport.BrowseMethod = this.Browse;

            return Globals.Factory.GetRibbonFactory().CreateRibbonManager(new Microsoft.Office.Tools.Ribbon.IRibbonExtension[] { apisExcelImport });
        }

        /* Test writing excel */
        public void ThisAddIn_Test(object sender, System.EventArgs e)
        {
            //Excel.Worksheet newWorksheet;
            //newWorksheet = (Excel.Worksheet)this.Application.Worksheets.Add(
            //    missing, missing, missing, missing);
            TestExcelForm excelform = new TestExcelForm();
            if (excelform.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            { 
                Excel.Worksheet sheet = ((Excel.Worksheet)this.Application.ActiveWorkbook.Sheets[1]);

                ExcelExportService excelService = new ExcelExportService();
                excelService.WriteExcelTest(sheet);
            }
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
