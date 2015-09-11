using System.Windows.Controls;
using Prediktor.Ioc;
using Prediktor.Carbon.Configuration.ViewModels;
using Microsoft.Practices.Prism.Commands;
using Prediktor.Carbon.Infrastructure.Definitions;
using System.Collections.Specialized;
using System.Collections.Generic;
using Prediktor.Configuration.BaseTypes.Definitions;
using Microsoft.Practices.Prism.Events;
using Prediktor.Carbon.Configuration.Definitions.Events;

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
