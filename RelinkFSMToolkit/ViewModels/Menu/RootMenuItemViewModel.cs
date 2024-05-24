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

public partial class RootMenuItemViewModel : ObservableObject, IMenuItemViewModel
{
    [ObservableProperty]
    public string _header;

    public ObservableCollection<IMenuItemViewModel> MenuItems { get; set; } = [];
}
