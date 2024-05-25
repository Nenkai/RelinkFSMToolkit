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
/// Represents a connection on the graph (which can represent start & end connection).
/// </summary>
public partial class ConnectionViewModel : ObservableObject
{
    [ObservableProperty]
    private Point _anchor;

    [ObservableProperty]
    public ArrowHeadEnds _arrowHeadEnds = ArrowHeadEnds.End;

    public string Title { get; set; }

    public NodeViewModel Source { get; set; }
    public NodeViewModel Target { get; set; }

    public ObservableCollection<TransitionViewModel> Transitions { get; set; } = [];
}
