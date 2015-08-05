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
        public static ThisAddIn G_ThisAddIn;

        ExcelImportBootstrapper Bootstrapper;
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            System.Windows.Forms.Application.Idle += OnIdle;
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            
        }

        private void OnIdle(Object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Idle -= OnIdle;
            
            try
            {
                Bootstrapper = new ExcelImportBootstrapper();
                Bootstrapper.Run();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ExcelImport failed to run bootstrapper: " + ex.ToString());
            }
        }


        public void Connect()
        {
            Bootstrapper.Connect();
        }

        public void Browse()
        {
            Bootstrapper.Browse();
        }

        public void CloseBrowse()
        {
            Bootstrapper.CloseBrowse();
        }

        public void Update()
        {
            Bootstrapper.Update();
        }

        protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            Ribbon apisExcelImport = new Ribbon();
            apisExcelImport.TestEvent += ThisAddIn_Test;
            apisExcelImport.ConnectMethod = this.Connect;
            apisExcelImport.BrowseMethod = this.Browse;
            apisExcelImport.UpdateMethod = this.Update;

            return Globals.Factory.GetRibbonFactory().CreateRibbonManager(new Microsoft.Office.Tools.Ribbon.IRibbonExtension[] { apisExcelImport });
        }

        /* Test writing excel */
        public void ThisAddIn_Test(object sender, System.EventArgs e)
        {
            //HistoricalExcelService excelService = new HistoricalExcelService();
            //excelService.WriteExcelTest();
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

            G_ThisAddIn = this;
        }
        
        #endregion
    }
}
