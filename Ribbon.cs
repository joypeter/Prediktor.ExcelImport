using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;

namespace Prediktor.ExcelImport
{
    public partial class Ribbon : IRibbonExtension
    {
        public delegate void BrowseDelegate();
        public delegate void UpdateDelegate();
        public BrowseDelegate BrowseMethod { get; set; }
        public UpdateDelegate UpdateMethod { get; set; }
        public event EventHandler TestEvent;

        private void Ribbon_Load(object sender, RibbonUIEventArgs e)
        {
            btnTest.Visible = false;
        }

        private void btnImport_Click(object sender, RibbonControlEventArgs e)
        {
            BrowseMethod();
        }

        private void btnTest_Click(object sender, RibbonControlEventArgs e)
        {
            TestEvent.Invoke(null, null);
        }

        private void btnUpdate_Click(object sender, RibbonControlEventArgs e)
        {
            UpdateMethod();
        }
    }
}
