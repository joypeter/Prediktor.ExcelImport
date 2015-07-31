using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Prediktor.Carbon.Configuration.Definitions.Views;
using Prediktor.Carbon.Configuration.ViewModels;
using Telerik.Windows.Controls.ChartView;
using Prediktor.Utilities;

namespace Prediktor.ExcelImport
{
    /// <summary>
    /// Interaction logic for HistoryTrend.xaml
    /// </summary>
    public partial class HistoryTrend : UserControl
    {
        public HistoryTrend()
        {
            this.DataContextChanged += HistoryTrend_DataContextChanged;
            InitializeComponent();
        }

        void HistoryTrend_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var oldVm = e.OldValue as HistoricalChartViewModel;
            if (oldVm != null)
            {
                oldVm.Data.CollectionChanged -= Data_CollectionChanged;
            }

            var vm = DataContext as HistoricalChartViewModel;
            if (vm != null)
            {
                vm.Data.CollectionChanged += Data_CollectionChanged;
                vm.ColorChanged += vm_ColorChanged;
            }
            UpdateSeries();
        }

        void vm_ColorChanged(object sender, EventArgs<Color> e)
        {
            UpdateSeries();
        }

        void Data_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateSeries();
        }

        private void UpdateSeries()
        {
            var m = DataContext as HistoricalChartViewModel;
            if (m != null)
            {
                this.Chart.Series.Clear();
                for (int i = 0; i < m.Data.Count; i++)
                {
                    var cat = new PropertyNameDataPointBinding() { PropertyName = "Time" };
                    var val = new PropertyNameDataPointBinding() { PropertyName = "Value" };
                    var ls = new LineSeries();
                    ls.CategoryBinding = cat;
                    ls.ValueBinding = val;
                    ls.ItemsSource = m.Data[i];
                    ls.Stroke = new SolidColorBrush(m.Legend[i].Color);
                    ls.StrokeThickness = 1;
                    this.Chart.Series.Add(ls);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Chart.Zoom = new Size(1, 1);
            Chart.PanOffset = new Point(0, 0);
        }
    }
}
