using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Prediktor.ExcelImport
{
    public partial class TestExcelForm : Form
    {
        public TestExcelForm()
        {
            InitializeComponent();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            var v = historicalTimePeriodView1.DataContext;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
