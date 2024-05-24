using CommunityToolkit.Mvvm.ComponentModel;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RelinkFSMToolkit.ViewModels;

public partial class TreeViewItemViewModel : ObservableObject
{
    [ObservableProperty]
    public Enum _iconKind;

    [ObservableProperty]
    public Visibility _visibility = Visibility.Visible;

    [ObservableProperty]
    public string _treeViewName = "No Name";

    [ObservableProperty]
    public bool _isExpanded = false;

    [ObservableProperty]
    public bool _canDrop;

    public ObservableCollection<object> DisplayedItems { get; set; } = [];

    public TreeViewItemViewModel(string name)
    {
        TreeViewName = name;
    }
}
