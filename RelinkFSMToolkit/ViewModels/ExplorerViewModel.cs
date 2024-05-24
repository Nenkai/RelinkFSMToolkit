using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace RelinkFSMToolkit.ViewModels;

public partial class ExplorerViewModel : ObservableObject
{
    private EditorViewModel _editorViewModel;

    private Dictionary<string, TreeViewItemViewModel> _idToItem = [];
    public ObservableCollection<TreeViewItemViewModel> DisplayedItems { get; set; } = [];

    public ExplorerViewModel() { }
    public ExplorerViewModel(EditorViewModel editorViewModel)
    {
        _editorViewModel = editorViewModel;
    }

    public void AddItem(string id, TreeViewItemViewModel item)
    {
        if (!_idToItem.ContainsKey(id))
        {
            _idToItem.Add(id, item);
            DisplayedItems.Add(item);
        }
    }

    public void Clear()
    {
        _idToItem.Clear();
        DisplayedItems.Clear();
    }

    [RelayCommand]
    public void OnItemSelected(object obj)
    {
        switch (obj)
        {
            case FSMTreeViewItemViewModel fsm:
                _editorViewModel.FSM = fsm.FSM;
                _editorViewModel.Load();
                break;
        }
    }
}
