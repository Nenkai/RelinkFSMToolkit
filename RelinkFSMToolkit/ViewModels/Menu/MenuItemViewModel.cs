using CommunityToolkit.Mvvm.ComponentModel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;

namespace RelinkFSMToolkit.ViewModels;

public partial class MenuItemViewModel : ObservableObject, IMenuItemViewModel
{
    [ObservableProperty]
    public string _header;

    [ObservableProperty]
    public bool _checked;

    [ObservableProperty]
    public bool _enabled;

    [ObservableProperty]
    public ICommand _command;

    [ObservableProperty]
    public object _parameter;

    [ObservableProperty]
    public Enum _iconKind;

    public ObservableCollection<IMenuItemViewModel> MenuItems { get; set; } = [];
}
