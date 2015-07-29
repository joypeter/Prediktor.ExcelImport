using System.Windows;
using Prediktor.ExcelImport.ViewModels;

namespace Prediktor.ExcelImport.Views
{
    /// <summary>
    /// Interaction logic for ExportDialog.xaml
    /// </summary>
    public partial class ExportExcelDialog : Window
    {
        public ExportExcelDialog(ExportExcelDialogViewModel exportExcelDialogViewModel)
        {
            InitializeComponent();

            DataContext = exportExcelDialogViewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
