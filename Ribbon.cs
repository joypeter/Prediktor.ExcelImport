﻿using System;
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

    }
}
