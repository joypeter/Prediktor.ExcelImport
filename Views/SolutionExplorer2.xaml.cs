using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Prediktor.Carbon.Configuration.Definitions.Views;
using Prediktor.Carbon.Configuration.ViewModels;
using Prediktor.Carbon.Infrastructure.Definitions;
using Prediktor.Configuration.Definitions;
using Prediktor.Ioc;
using Prediktor.Log;
using Telerik.Windows.Controls;

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
                    RadTreeViewItem currentClicked = VisualUpwardSearch(e.OriginalSource as DependencyObject);
                    if (currentClicked == null)
                    {
                        return;
                    }

                    List<IObjectId> objectIds = new List<IObjectId>();
                    var item = ((FrameworkElement)e.OriginalSource).DataContext as ITreeNode;

                    if (item != null && item.Id != null && item.Id is IObjectId)
                    {
                        objectIds.Add((IObjectId)item.Id);
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

        static Telerik.Windows.Controls.RadTreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is Telerik.Windows.Controls.RadTreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as Telerik.Windows.Controls.RadTreeViewItem;
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

        private void AddToViewModel(ITreeNode item)
        {
            if (item.CanHaveChildren)
            {
                foreach (var child in item.Children)
                {
                    AddToViewModel(child);
                }
            }

            var vm = DataContext as SolutionExplorerViewModel;
            if (vm != null && !vm.SelectedItems.Contains(item))
            {
                vm.SelectedItems.Add(item);
                vm.SelectedItemsChangedCommand.Execute(null);
            }
        }

        private void RemoveFromViewModel(ITreeNode item)
        {
            if (item.CanHaveChildren)
            {
                foreach (var child in item.Children)
                {
                    RemoveFromViewModel(child);
                }
            }

            var vm = DataContext as SolutionExplorerViewModel;
            if (vm != null && vm.SelectedItems.Contains(item))
            {
                vm.SelectedItems.Remove(item);
                vm.SelectedItemsChangedCommand.Execute(null);
            }
        }

        private void treeView_Checked(object sender, RoutedEventArgs e)
        {
            RadTreeViewItem currentChecked = e.OriginalSource as RadTreeViewItem;
            var item = currentChecked.DataContext as ITreeNode;

            bool isInitiallyChecked = (e as RadTreeViewCheckEventArgs).IsUserInitiated;
            if (!isInitiallyChecked)
            {
                e.Handled = true;
                return;
            }

            AddToViewModel(item);

            e.Handled = true;
        }

        private void treeView_Unchecked(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            RadTreeViewItem currentChecked = e.OriginalSource as RadTreeViewItem;
            var item = currentChecked.DataContext as ITreeNode;

            bool isInitiallyChecked = (e as RadTreeViewCheckEventArgs).IsUserInitiated;
            if (!isInitiallyChecked)
            {
                e.Handled = true;
                return;
            }

            RemoveFromViewModel(item);

            e.Handled = true;
        }
    }
}
