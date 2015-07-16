using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;

namespace Prediktor.ExcelImport
{
    public partial class Ribbon
    {
        private void Ribbon_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void btnConfigure_Click(object sender, RibbonControlEventArgs e)
        {
            DialogManager.Current.Connect();
        }

        private void btnImport_Click(object sender, RibbonControlEventArgs e)
        {
            DialogManager.Current.Browse();
        }

    }
}
