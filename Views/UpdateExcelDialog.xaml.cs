using System.Windows;
using Prediktor.ExcelImport.ViewModels;

namespace Prediktor.ExcelImport.Views
{
    /// <summary>
    /// Interaction logic for ExportDialog.xaml
    /// </summary>
    public partial class UpdateExcelDialog : Window
    {
        public UpdateExcelDialog(UpdateExcelDialogViewModel updateExcelDialogViewModel)
        {
            InitializeComponent();

            DataContext = updateExcelDialogViewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
