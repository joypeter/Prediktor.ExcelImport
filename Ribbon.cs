using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;

namespace Prediktor.ExcelImport
{
    public partial class Ribbon : IRibbonExtension
    {
        public delegate void ConnectDelegate();
        public delegate void BrowseDelegate();
        public ConnectDelegate ConnectMethod { get; set; }
        public BrowseDelegate BrowseMethod { get; set; } 

        public event EventHandler TestEvent;

        private void Ribbon_Load(object sender, RibbonUIEventArgs e)
        {
            
        }

        private void btnConfigure_Click(object sender, RibbonControlEventArgs e)
        {
            ConnectMethod();
        }

        private void btnImport_Click(object sender, RibbonControlEventArgs e)
        {
            BrowseMethod();
        }

        private void btnTest_Click(object sender, RibbonControlEventArgs e)
        {
            //TestEvent.Invoke(null, null);
        }

    }
}
