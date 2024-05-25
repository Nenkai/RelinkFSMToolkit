using AvalonDock.Themes;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using RelinkFSMToolkit.Messages;

using MahApps.Metro.IconPacks;

using Microsoft.Win32;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RelinkFSMToolkit.ViewModels;

public partial class ConnectionEditorViewModel : ObservableObject
{
    public ObservableCollection<TransitionViewModel> Transitions { get; private set; } = [];

    public ConnectionEditorViewModel() { }

    public ConnectionEditorViewModel(EditorViewModel editorViewModel)
    {

    }
}
