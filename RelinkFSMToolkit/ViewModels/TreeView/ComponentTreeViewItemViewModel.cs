using CommunityToolkit.Mvvm.ComponentModel;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RelinkFSMToolkit.ViewModels;

public partial class ComponentTreeViewItemViewModel : TreeViewItemViewModel
{
    public Type ComponentType { get; set; }

    public ComponentTreeViewItemViewModel(string name, Type type)
        : base(name)
    {
        ComponentType = type;
    }
}
