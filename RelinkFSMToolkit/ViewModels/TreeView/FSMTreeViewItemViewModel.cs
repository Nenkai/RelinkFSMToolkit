using CommunityToolkit.Mvvm.ComponentModel;

using MahApps.Metro.IconPacks;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RelinkFSMToolkit.ViewModels;

public partial class FSMTreeViewItemViewModel : TreeViewItemViewModel
{
    public FSMParser FSM { get; set; }

    public FSMTreeViewItemViewModel(string name)
        : base(name)
    {
        this.IconKind = PackIconVaadinIconsKind.Automation;
    }
}
