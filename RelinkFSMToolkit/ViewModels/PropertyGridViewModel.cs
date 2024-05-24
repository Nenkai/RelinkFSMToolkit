using CommunityToolkit.Mvvm.ComponentModel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelinkFSMToolkit.ViewModels;

public partial class PropertyGridViewModel : ObservableObject
{
    [ObservableProperty]
    public object _selectedObject;
}
