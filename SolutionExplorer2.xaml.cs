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
using Prediktor.Carbon.Configuration.Definitions.Views;
using Prediktor.Carbon.Configuration.ViewModels;
using Prediktor.Carbon.Infrastructure.Definitions;
using Prediktor.Configuration.Definitions;
using Prediktor.Ioc;
using Prediktor.Log;
using Microsoft.Practices.Prism.Events;
using Prediktor.Carbon.Configuration.Definitions.Events;

namespace Prediktor.ExcelImport
{
    /// <summary>
    /// Interaction logic for SolutionExplorer2.xaml
    /// </summary>
    [AvoidAutoIocRegister]
    public partial class SolutionExplorer2 : UserControl
    {
        private static ITraceLog _log = LogManager.GetLogger(typeof(SolutionExplorer2));
        public SolutionExplorer2(SolutionExplorerViewModel viewModel)
        {
            _log.Debug("Create");
            InitializeComponent();
            _log.Debug("Component Initialized");
            DataContext = viewModel;
            _log.Debug("Created");
        }

        public void treeView_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var vm = DataContext as SolutionExplorerViewModel;
                if (vm != null)
                {
                    List<IObjectId> objectIds = new List<IObjectId>();
                    foreach (var selectedItem in vm.SelectedItems)
                    {
                        ITreeNode treeViewModel = selectedItem as ITreeNode;
                        if (treeViewModel != null && treeViewModel.Id != null && treeViewModel.Id is IObjectId)
                        {
                            objectIds.Add((IObjectId)treeViewModel.Id);
                        }
                    }
                    if (objectIds.Count > 0)
                    {
                        DataObject data = new DataObject();
                        IDragData content = new DragData(objectIds.ToArray());
                        data.SetData(typeof(IDragData), content);
                        // Inititate the drag-and-drop operation.
                        DragDrop.DoDragDrop(this, data, DragDropEffects.Copy | DragDropEffects.Move);
                    }
                }
            }
        }

        private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Telerik.Windows.Controls.RadTreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);
            var treeView = sender as Telerik.Windows.Controls.RadTreeView;
            if (treeViewItem != null)
            {
                bool selectAndFocus = false;
                if (treeView != null)
                {
                    if (!treeView.SelectedItems.Contains(treeViewItem.DataContext))
                    {
                        treeView.SelectedItems.Clear();
                        selectAndFocus = true;
                    }
                }
                if (selectAndFocus)
                {
                    treeViewItem.IsSelected = true;
                    treeViewItem.Focus();
                }
                e.Handled = true;
            }
        }

        static Telerik.Windows.Controls.RadTreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is Telerik.Windows.Controls.RadTreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as Telerik.Windows.Controls.RadTreeViewItem;
        }

        private void RadTreeView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var vm = DataContext as SolutionExplorerViewModel;
            var treeView = sender as Telerik.Windows.Controls.RadTreeView;
            if (vm != null && treeView != null && !vm.HoldSelectionChangedNotification)
            {
                var selectedItem = new List<ITreeNode>(treeView.SelectedItems.OfType<ITreeNode>());
                vm.SelectedItems = selectedItem;
                bool holdNotification = false;
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    holdNotification = Keyboard.IsKeyDown(Key.H);

                if (!holdNotification)
                    vm.SelectedItemsChangedCommand.Execute(null);
            }
        }

        private void RadTreeView_LoadOnDemand(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement)
            {
                var treeViewItem = (FrameworkElement)e.OriginalSource as Telerik.Windows.Controls.RadTreeViewItem;
                var t = ((FrameworkElement)e.OriginalSource).DataContext as ITreeNode;
                Action<int> a = itemCount =>
                {
                    treeViewItem.IsExpanded = true;
                    if (itemCount == 0 && treeViewItem != null)
                    {
                        treeViewItem.IsLoadingOnDemand = false;
                        treeViewItem.IsLoadOnDemandEnabled = false;
                    }
                };
                if (t != null)
                    t.LoadChildren(a);
            }
        }

        private void RadTreeView_ItemPrepared(object sender, Telerik.Windows.Controls.RadTreeViewItemPreparedEventArgs e)
        {
            var vm = DataContext as SolutionExplorerViewModel;
            var t = e.PreparedItem.DataContext as ITreeNode;
            if (vm != null && t != null)
            {
                e.PreparedItem.IsLoadOnDemandEnabled = t.CanHaveChildren && (t.Children == null || t.Children.Count == 0);
            }
        }

        private void SelectBtn_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as SolutionExplorerViewModel;
            if (vm != null && !vm.HoldSelectionChangedNotification)
            {
                var selectedItem = new List<ITreeNode>(this.treeView.CheckedItems.OfType<ITreeNode>());
                vm.SelectedItems = selectedItem;
                bool holdNotification = false;
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    holdNotification = Keyboard.IsKeyDown(Key.H);

                if (!holdNotification)
                    vm.SelectedItemsChangedCommand.Execute(null);
            }
        }
    }
}
