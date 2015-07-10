namespace Prediktor.ExcelImport
{
    partial class ApisExcelImportRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public ApisExcelImportRibbon()
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
            this.excelImportGroup = this.Factory.CreateRibbonGroup();
            this.configButton = this.Factory.CreateRibbonButton();
            this.importButton = this.Factory.CreateRibbonButton();
            this.updateButton = this.Factory.CreateRibbonButton();
            this.tab1.SuspendLayout();
            this.excelImportGroup.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tab1.Groups.Add(this.excelImportGroup);
            this.tab1.Label = "TabAddIns";
            this.tab1.Name = "tab1";
            // 
            // excelImportGroup
            // 
            this.excelImportGroup.Items.Add(this.configButton);
            this.excelImportGroup.Items.Add(this.importButton);
            this.excelImportGroup.Items.Add(this.updateButton);
            this.excelImportGroup.Label = "ApisExcelImport";
            this.excelImportGroup.Name = "excelImportGroup";
            // 
            // configButton
            // 
            this.configButton.Label = "Configure";
            this.configButton.Name = "configButton";
            this.configButton.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.button1_Click);
            // 
            // importButton
            // 
            this.importButton.Label = "Import";
            this.importButton.Name = "importButton";
            this.importButton.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.button2_Click);
            // 
            // updateButton
            // 
            this.updateButton.Label = "Update";
            this.updateButton.Name = "updateButton";
            // 
            // ApisExcelImportRibbon
            // 
            this.Name = "ApisExcelImportRibbon";
            this.RibbonType = "Microsoft.Excel.Workbook";
            this.Tabs.Add(this.tab1);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.ApisExcelImportRibbon_Load);
            this.tab1.ResumeLayout(false);
            this.tab1.PerformLayout();
            this.excelImportGroup.ResumeLayout(false);
            this.excelImportGroup.PerformLayout();

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup excelImportGroup;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton configButton;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton importButton;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton updateButton;
    }

    partial class ThisRibbonCollection
    {
        internal ApisExcelImportRibbon ApisExcelImportRibbon
        {
            get { return this.GetRibbon<ApisExcelImportRibbon>(); }
        }
    }
}
