using System.Windows;
using System.Collections.ObjectModel;

namespace Prediktor.ExcelImport
{
    /// <summary>
    /// Interaction logic for BrowseDialog.xaml
    /// </summary>
    public partial class BrowseDialog : Window
    {
        private ObservableCollection<SolutionExplorer2> SE;
        private ObservableCollection<MainRegion> MainRegion;
        public BrowseDialog(ShellViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        public void AddSolutionExplorer2(SolutionExplorer2 se2)
        {
            SE = new ObservableCollection<SolutionExplorer2>();
            SE.Add(se2);
            this.TreeViewUC.ItemsSource = SE;
        }

        public void AddSolutionMainRegion(MainRegion mainRegion)
        {
            MainRegion = new ObservableCollection<MainRegion>();
            MainRegion.Add(mainRegion);
            this.MainToolbar.ItemsSource = MainRegion;
        }
    }
}
