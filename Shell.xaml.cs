using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Prediktor.Carbon.Infrastructure.Definitions;
using Prediktor.Carbon.Infrastructure.Behaviors.Implementation;
using Prediktor.Carbon.Configuration.Views;
using System.Collections.ObjectModel;

namespace Prediktor.ExcelImport
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Shell : ThemedWindow
    {
        private ObservableCollection<SolutionExplorer2> SE;
        private ObservableCollection<MainRegion> MainRegion;
        public Shell(ShellViewModel viewModel)
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
