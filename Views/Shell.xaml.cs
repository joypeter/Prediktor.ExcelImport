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
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell : Window
    {
        public Shell(ShellViewModel model)
        {
            InitializeComponent();
            DataContext = model;
        }
    }
}
