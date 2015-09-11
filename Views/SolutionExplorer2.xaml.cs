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
using Microsoft.Practices.Prism.Events;
using Prediktor.Carbon.Configuration.Definitions.Events;
using Prediktor.Configuration.BaseTypes.Definitions;
using Prediktor.Configuration.BaseTypes.Implementation;
using Prediktor.Configuration.OpcHda.Implementation;
using Prediktor.Configuration.UA.Implementation.Node;
using Prediktor.Configuration.OpcHda.Implementation.Item;

namespace Prediktor.ExcelImport
{
    /// <summary>
    /// Interaction logic for SolutionExplorer2.xaml
    /// </summary>
    [AvoidAutoIocRegister]
    public partial class SolutionExplorer2 : UserControl
    {
        private static ITraceLog _log = LogManager.GetLogger(typeof(SolutionExplorer2));
        private IEventAggregator _eventAggregator;
        public SolutionExplorer2(SolutionExplorerViewModel viewModel, IEventAggregator eventAggregator)
        {
            _log.Debug("Create");
            InitializeComponent();
            _log.Debug("Component Initialized");
            DataContext = viewModel;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<RemovePropertiesFromViewEvent>().Subscribe(OnPropertiesRemoved, ThreadOption.UIThread, false, a => true);
            _log.Debug("Created");
        }

        private void UnCheckedItemByPropertyID(IPropertyId propertyID)
        {
            var vm = DataContext as SolutionExplorerViewModel;
            foreach (var treeViewItem in treeView.ChildrenOfType<RadTreeViewItem>())
            {
                ITreeNode item = treeViewItem.DataContext as ITreeNode;
                if ((propertyID as PropertyId).ContextId is OpcItemId)
                { //HDA item
                    OpcHierarchicalItemId hierachicalID = item.Id as OpcHierarchicalItemId;
                    if (hierachicalID == null) continue;
                    if (hierachicalID.OpcItemId == (propertyID as PropertyId).ContextId)
                    {
                        //item.IsSelected is two-way bound to treeViewItem.IsChecked;
                        item.IsSelected = false;
                        treeViewItem.IsSelected = false;
                        vm.SelectedItems.Remove(item);
                    }
                }
                else if ((propertyID as PropertyId).ContextId is StringNodeId)
                { //UA item
                    if (item.Id == (propertyID as PropertyId).ContextId)
                    {
                        //item.IsSelected is two-way bound to treeViewItem.IsChecked;
                        item.IsSelected = false;
                        treeViewItem.IsSelected = false;
                        vm.SelectedItems.Remove(item);
                    }
                }
            }
        }

        private void OnPropertiesRemoved(IPropertyId[] properties)
        {
            if (properties != null && properties.Length > 0)
            {
                for (int i = 0; i < properties.Length; i++)
                {
                    var p = properties[i];
                    UnCheckedItemByPropertyID(p);
                }
            }
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

        static RadTreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is RadTreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as RadTreeViewItem;
        }

        private void RadTreeView_LoadOnDemand(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement)
            {
                var treeViewItem = (FrameworkElement)e.OriginalSource as RadTreeViewItem;
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

        private void RadTreeView_ItemPrepared(object sender, RadTreeViewItemPreparedEventArgs e)
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
                item.IsSelected = true;
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
                item.IsSelected = false;
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

            currentChecked.IsSelected = true;
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

            currentChecked.IsSelected = false;
            RemoveFromViewModel(item);

            e.Handled = true;
        }
    }
}
