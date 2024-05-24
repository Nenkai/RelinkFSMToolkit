using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using GBFRDataTools.FSM.Entities;

using GongSolutions.Wpf.DragDrop;

namespace RelinkFSMToolkit.ViewModels;

/// <summary>
/// Represents a node on the graph.
/// </summary>
public partial class NodeViewModel : ObservableObject, IDropTarget
{
    [ObservableProperty]
    private Point _location;

    [ObservableProperty]
    private Point _anchor;

    [ObservableProperty]
    private Size _size;

    [ObservableProperty]
    private Brush _borderBrush = Brushes.DimGray;

    [ObservableProperty]
    private CornerRadius _cornerRadius = new CornerRadius(5);

    [ObservableProperty]
    public int _guid;

    [ObservableProperty]
    public string _title;

    public ObservableCollection<NodeComponentViewModel> Components { get; set; } = [];

    public void DragOver(IDropInfo dropInfo)
    {
        if (dropInfo.Data is ComponentTreeViewItemViewModel)
        {
            dropInfo.Effects = DragDropEffects.Move;
        }
        else
            GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.DragOver(dropInfo);
    }

    public void Drop(IDropInfo dropInfo)
    {
        if (dropInfo.Data is ComponentTreeViewItemViewModel componentTvi)
        {
            AddComponentFromToolboxItem(componentTvi);
        }
        else
            GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.Drop(dropInfo);
    }

    [RelayCommand]
    public void OnComponentDelete(object param)
    {
        Components.Remove(param as NodeComponentViewModel);
    }

    [RelayCommand]
    public void OnDrop(object param)
    {
        var e = param as DragEventArgs;

        var data = e.Data.GetData("GongSolutions.Wpf.DragDrop"); // I have no idea why this is encapsulated to this.
        if (data is NodeComponentViewModel nodeComponent)
        {
            Components.Add(nodeComponent);
            nodeComponent.Parent.Components.Remove(nodeComponent);
            nodeComponent.Parent = this;
        }
        else if (data is ComponentTreeViewItemViewModel componentTvi)
        {
            AddComponentFromToolboxItem(componentTvi);
        }
    }

    private void AddComponentFromToolboxItem(ComponentTreeViewItemViewModel componentTvi)
    {
        BehaviorTreeComponent component = Activator.CreateInstance(componentTvi.ComponentType) as BehaviorTreeComponent;
        component.ComponentName = componentTvi.TreeViewName;
        Components.Add(new NodeComponentViewModel(this)
        {
            Component = component,
            Name = component.ToString(),
        });
    }
}
