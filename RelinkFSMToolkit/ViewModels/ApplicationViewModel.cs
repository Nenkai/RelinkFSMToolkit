using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Win32;

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using MahApps.Metro.IconPacks;
using GBFRDataTools.Entities.Quest;

using RelinkFSMToolkit.Messages;

namespace RelinkFSMToolkit.ViewModels;

public partial class ApplicationViewModel : ObservableObject
{
    public EditorViewModel EditorViewModel { get; set; }
    public ExplorerViewModel ExplorerViewModel { get; set; }
    public PropertyGridViewModel PropertyGridViewModel { get; set; }
    public ToolboxViewModel ToolboxViewModel { get; set; }
    public TopMenuViewModel TopMenuViewModel { get; set; }

    public ApplicationViewModel() { }

    public ApplicationViewModel(EditorViewModel editorViewModel, 
        ExplorerViewModel explorerViewModel, 
        PropertyGridViewModel propertyGridViewModel, 
        ToolboxViewModel toolboxViewModel,
        TopMenuViewModel topMenuViewModel)
    {
        EditorViewModel = editorViewModel;
        ExplorerViewModel = explorerViewModel;
        PropertyGridViewModel = propertyGridViewModel;
        ToolboxViewModel = toolboxViewModel;
        TopMenuViewModel = topMenuViewModel;

        WeakReferenceMessenger.Default.Register<FileOpenRequestMessage>(this, (recipient, message) =>
        {
            ProcessFileLoadedChanged(message.Value);
        });
    }

    public void ProcessFileLoadedChanged(string file)
    {
        string fileName = Path.GetFileNameWithoutExtension(file);
        if (fileName.Contains("baseinfo", StringComparison.Ordinal))
        {
            var data = File.ReadAllBytes(file);
            var questInfo = BaseInfo.Read(data);
            int questId = questInfo.GetQuestId();

            var questTreeItem = new TreeViewItemViewModel($"Quest ({questId:X6})")
            {
                IconKind = PackIconMaterialKind.Script,
                IsExpanded = true,
            };

            var fsmRoot = new TreeViewItemViewModel("FSM")
            {
                IconKind = PackIconRemixIconKind.FlowChart,
            };
            questTreeItem.DisplayedItems.Add(fsmRoot);

            string dir = @"D:\\Games\\SteamLibrary\\steamapps\\common\\Granblue Fantasy Relink\\ext_1.2.1";

            for (int i = 0; i < questInfo.FsmDataList.Count; i++)
            {
                FsmDataInfo fsmFile = questInfo.FsmDataList[i];
                string fsmFileName = $"quest_{questId:x}_{fsmFile.Suffix:x}_fsm_ingame.msg";
                string fsmFilePath = Path.Combine(dir, "system", "fsm", "quest", fsmFileName);

                var parser = new FSMParser();
                parser.Parse(fsmFilePath);

                fsmRoot.DisplayedItems.Add(new FSMTreeViewItemViewModel($"[{i}] {fsmFile.Name}")
                {
                    FSM = parser,
                });
            }

            ExplorerViewModel.AddItem($"quest_{questId:X6}", questTreeItem);
        }
        else
        {
            var parser = new FSMParser();
            parser.Parse(file);
            EditorViewModel.FSM = parser;
            EditorViewModel.Load();

            var fsm = new FSMTreeViewItemViewModel("FSM")
            {
                IconKind = PackIconRemixIconKind.FlowChart,
                FSM = parser,
            };

            ExplorerViewModel.AddItem("fsm", fsm);
        }
    }
}
