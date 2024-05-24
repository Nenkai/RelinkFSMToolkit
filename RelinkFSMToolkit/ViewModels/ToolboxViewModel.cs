using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using GBFRDataTools.FSM.Entities;

using GongSolutions.Wpf.DragDrop;

using MahApps.Metro.IconPacks;

using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace RelinkFSMToolkit.ViewModels;

public partial class ToolboxViewModel : ObservableObject, IDragSource
{
    private EditorViewModel _editorViewModel;

    private Dictionary<string, TreeViewItemViewModel> _idToItem = [];
    public ObservableCollection<TreeViewItemViewModel> DisplayedItems { get; set; } = [];

    public ToolboxViewModel() { }
    public ToolboxViewModel(EditorViewModel editorViewModel)
    {
        _editorViewModel = editorViewModel;

        var componentTvm = new TreeViewItemViewModel("Components")
        {
            IsExpanded = true,
            IconKind = PackIconMaterialKind.Chip
        };

        AddItem("components", componentTvm);

        IEnumerable<Type> componentTypes = Assembly.GetAssembly(typeof(BehaviorTreeComponent)).GetTypes()
            .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(BehaviorTreeComponent)));

        // Register all conditions
        var conditionsTvm = new TreeViewItemViewModel("Conditions")
        {
            IconKind = PackIconBootstrapIconsKind.QuestionOctagonFill,
        };
        AddItem("conditions", conditionsTvm, parent: componentTvm);

        var questConditions = new TreeViewItemViewModel("Quest")
        {
            IconKind = PackIconMaterialKind.Script,
        };
        AddItem("quest_conditions", questConditions, parent: conditionsTvm);

        var enemyConditions = new TreeViewItemViewModel("Enemy")
        {
            IconKind = PackIconMaterialKind.SwordCross,
        };
        AddItem("enemy_conditions", enemyConditions, parent: conditionsTvm);

        foreach (Type conditionType in componentTypes.Where(e => e.IsSubclassOf(typeof(ConditionComponent))))
        {
            var elem = new ComponentTreeViewItemViewModel(conditionType.Name, conditionType)
            {
                IconKind = PackIconMaterialKind.Memory,
            };

            if (conditionType.IsSubclassOf(typeof(QuestConditionComponent)))
            {
                AddItem(conditionType.Name, elem, parent: questConditions);
            }
            else if (conditionType.Name.StartsWith("Em"))
            {
                AddItem(conditionType.Name, elem, parent: enemyConditions);
            }
            else
            {
                AddItem(conditionType.Name, elem, parent: conditionsTvm);
            }
        }

        // Register all actions
        var actionsTvm = new TreeViewItemViewModel("Actions")
        {
            IconKind = PackIconBootstrapIconsKind.ExclamationTriangleFill,
        };
        AddItem("actions", actionsTvm, parent: componentTvm);

        var questActions = new TreeViewItemViewModel("Quest")
        {
            IconKind = PackIconMaterialKind.Script,
        };
        AddItem("quest_actions", questActions, parent: actionsTvm);

        var enemyActions = new TreeViewItemViewModel("Enemy")
        {
            IconKind = PackIconMaterialKind.SwordCross,
        };
        AddItem("enemy_actions", enemyActions, parent: actionsTvm);

        foreach (Type actionType in componentTypes.Where(e => e.IsSubclassOf(typeof(ActionComponent))))
        {
            var elem = new ComponentTreeViewItemViewModel(actionType.Name, actionType)
            {
                IconKind = PackIconMaterialKind.Memory,
            };

            if (actionType.IsSubclassOf(typeof(QuestActionComponent)))
            {
                AddItem(actionType.Name, elem, parent: questActions);
            }
            else if (actionType.Name.StartsWith("Em"))
            {
                AddItem(actionType.Name, elem, parent: enemyActions);
            }
            else
            {
                AddItem(actionType.Name, elem, parent: actionsTvm);
            }
        }
    }

    public void AddItem(string id, TreeViewItemViewModel item, TreeViewItemViewModel parent = null)
    {
        if (!_idToItem.ContainsKey(id))
        {
            _idToItem.Add(id, item);

            if (parent is null)
                DisplayedItems.Add(item);
            else
                parent.DisplayedItems.Add(item);
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

    public void StartDrag(IDragInfo dragInfo)
    {
        GongSolutions.Wpf.DragDrop.DragDrop.DefaultDragHandler.StartDrag(dragInfo);
    }

    public bool CanStartDrag(IDragInfo dragInfo)
    {
        var item = dragInfo.SourceItem as TreeViewItemViewModel;
        if (item.DisplayedItems.Count > 0)
            return false;

        return GongSolutions.Wpf.DragDrop.DragDrop.DefaultDragHandler.CanStartDrag(dragInfo);
    }

    public void Dropped(IDropInfo dropInfo)
    {
        GongSolutions.Wpf.DragDrop.DragDrop.DefaultDragHandler.Dropped(dropInfo);
    }

    public void DragDropOperationFinished(DragDropEffects operationResult, IDragInfo dragInfo)
    {
        GongSolutions.Wpf.DragDrop.DragDrop.DefaultDragHandler.DragDropOperationFinished(operationResult, dragInfo);
    }

    public void DragCancelled()
    {
    }

    public bool TryCatchOccurredException(Exception exception)
    {
        return false;
    }
}
