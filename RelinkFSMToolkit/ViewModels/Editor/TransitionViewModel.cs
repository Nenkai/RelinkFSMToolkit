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

using Nodify;

namespace RelinkFSMToolkit.ViewModels;

/// <summary>
/// Represents a transition between two nodes on the graph.
/// </summary>
public partial class TransitionViewModel : ObservableObject
{
    public NodeViewModel Source { get; set; }
    public NodeViewModel Target { get; set; }

    public ObservableCollection<BehaviorTreeComponent> ConditionComponents { get; set; } = [];
}
