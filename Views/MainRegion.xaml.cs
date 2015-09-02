using System.Windows;
using System.Windows.Controls;
using Prediktor.Ioc;
using Prediktor.Carbon.Configuration.Definitions.Views;

namespace Prediktor.ExcelImport
{
    /// <summary>
    /// Interaction logic for UserControl2.xaml
    /// </summary>
    [AvoidAutoIocRegister]
    public partial class MainRegion : UserControl
    {
        public MainRegion(MainRegionViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
