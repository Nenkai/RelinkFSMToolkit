using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using GBFRDataTools.FSM.Entities;

namespace RelinkFSMToolkit.ViewModels;

public partial class NodeComponentViewModel : ObservableObject
{
    [ObservableProperty]
    public string _name;

    public BehaviorTreeComponent Component { get; set; }

    public NodeViewModel Parent { get; set; }
    public NodeComponentViewModel(NodeViewModel parent)
    {
        Parent = parent;
    }

    [RelayCommand]
    public void OnComponentDelete(object param)
    {
        ;
    }
}
