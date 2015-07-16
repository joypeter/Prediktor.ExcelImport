namespace Prediktor.ExcelImport
{
    partial class Ribbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public Ribbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tab1 = this.Factory.CreateRibbonTab();
            this.apisExcelImport = this.Factory.CreateRibbonGroup();
            this.btnConfigure = this.Factory.CreateRibbonButton();
            this.btnImport = this.Factory.CreateRibbonButton();
            this.btnUpdate = this.Factory.CreateRibbonButton();
            this.tab1.SuspendLayout();
            this.apisExcelImport.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tab1.Groups.Add(this.apisExcelImport);
            this.tab1.Label = "TabAddIns";
            this.tab1.Name = "tab1";
            // 
            // apisExcelImport
            // 
            this.apisExcelImport.Items.Add(this.btnConfigure);
            this.apisExcelImport.Items.Add(this.btnImport);
            this.apisExcelImport.Items.Add(this.btnUpdate);
            this.apisExcelImport.Label = "ApisExcelImport";
            this.apisExcelImport.Name = "apisExcelImport";
            // 
            // btnConfigure
            // 
            this.btnConfigure.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnConfigure.Label = "Configure";
            this.btnConfigure.Name = "btnConfigure";
            this.btnConfigure.ShowImage = true;
            this.btnConfigure.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnConfigure_Click);
            // 
            // btnImport
            // 
            this.btnImport.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnImport.Label = "Import";
            this.btnImport.Name = "btnImport";
            this.btnImport.ShowImage = true;
            this.btnImport.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnImport_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnUpdate.Label = "Update";
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.ShowImage = true;
            // 
            // Ribbon
            // 
            this.Name = "Ribbon";
            this.RibbonType = "Microsoft.Excel.Workbook";
            this.Tabs.Add(this.tab1);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.Ribbon_Load);
            this.tab1.ResumeLayout(false);
            this.tab1.PerformLayout();
            this.apisExcelImport.ResumeLayout(false);
            this.apisExcelImport.PerformLayout();

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup apisExcelImport;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnConfigure;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnImport;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnUpdate;
    }

    partial class ThisRibbonCollection
    {
        internal Ribbon Ribbon
        {
            get { return this.GetRibbon<Ribbon>(); }
        }
    }
}
