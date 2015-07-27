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
using Prediktor.Ioc;
using Prediktor.Carbon.Configuration.Definitions.Views;
using Prediktor.Carbon.Configuration.ViewModels;
using Telerik.Windows.Controls.ChartView;

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

            //this.TabbedView.ResourceDictionaryProvider = viewModel.ResourceDictionaryProvider;

            SelectView(0);
        }

        private void SelectView(int i)
        {
            viewTab.SelectedIndex = i;
            if (i == 0)
            {
                tableViewButton.IsEnabled = false;
                eventListViewButton.IsEnabled = true;
                graphViewButton.IsEnabled = true;
            }
            if (i == 1)
            {
                tableViewButton.IsEnabled = true;
                eventListViewButton.IsEnabled = false;
                graphViewButton.IsEnabled = true;
            }
            else if (i == 2)
            {
                tableViewButton.IsEnabled = true;
                eventListViewButton.IsEnabled = true;
                graphViewButton.IsEnabled = false;
            }
        }

        private void tableViewButton_Click(object sender, RoutedEventArgs e)
        {
            SelectView(0);
        }

        private void eventListViewButton_Click(object sender, RoutedEventArgs e)
        {
            SelectView(1);
        }

        private void graphViewButton_Click(object sender, RoutedEventArgs e)
        {
            SelectView(2);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //TODO: reset zoom
            /*
            HistoryTrendTab.
            this.HistoryTrend.Zoom = new Size(1, 1);
            this.HistoryTrend.PanOffset = new Point(0, 0);
             * */
        }
    }
}
