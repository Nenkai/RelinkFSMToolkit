using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;

using GBFRDataTools.FSM.Entities;

namespace RelinkFSMToolkit.ViewModels;

/// <summary>
/// Represents a node on the graph.
/// </summary>
public partial class TransitionViewModel : ObservableObject
{
    [ObservableProperty]
    private Point _anchor;

    [ObservableProperty]
    public DoubleCollection _strokeDashArray = [];

    public string Title { get; set; }

    public NodeViewModel Source { get; set; }
    public NodeViewModel Target { get; set; }

    public ObservableCollection<BehaviorTreeComponent> ConditionComponents { get; set; } = [];
}
