
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Media;
using System.IO;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Microsoft.Msagl;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Core.Geometry.Curves;

using Nodify;

using GBFRDataTools.FSM.Entities;
using RelinkFSMToolkit.Messages;
using GBFRDataTools.FSM;

namespace RelinkFSMToolkit.ViewModels;

public partial class EditorViewModel : ObservableObject
{
    public FSMParser FSM { get; set; }

    [ObservableProperty]
    public object _selectedNode;

    public ObservableCollection<NodeViewModel> Nodes { get; } = [];
    public ObservableCollection<ConnectionViewModel> Connections { get; } = [];

    public delegate void NodesLoadedDelegate();
    public event NodesLoadedDelegate OnNodesLoaded;

    private PropertyGridViewModel _propGridViewModel;

    public EditorViewModel() { }
    public EditorViewModel(PropertyGridViewModel propGridViewModel)
    {
        _propGridViewModel = propGridViewModel;

        WeakReferenceMessenger.Default.Register<FileSaveRequestMessage>(this, (recipient, message) =>
        {
            SaveFSMFile(message.Value);
        });
    }

    // Called by generated observable property _selectedNode
    partial void OnSelectedNodeChanged(object value)
    {
        
    }

    [RelayCommand]
    public void OnConnectionClicked(object param)
    {
        ;
    }

    [RelayCommand]
    public void OnComponentClicked(object param)
    {
        _propGridViewModel.SelectedObject = (param as NodeComponentViewModel).Component;
    }

    public void SaveFSMFile(string fileName)
    {
        FSMNode rootNode = BuildTreeFromCurrentGraph();

        Stream stream = File.OpenWrite(fileName);
        var builder = new FSMSerializer(rootNode);
        builder.WriteJson(stream);
    }

    public void Load()
    {
        _processedNodes.Clear();
        Nodes.Clear();
        Connections.Clear();

        int depth = 0;

        try
        {
            NodeViewModel root = CreateViewModelFromFSMNode(FSM.RootNode, FSM.AllNodes, ref depth);
            root.BorderBrush = Brushes.Red;
            root.CornerRadius = new CornerRadius(0);

            OnNodesLoaded?.Invoke();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Errored: {ex.Message}");
            WeakReferenceMessenger.Default.Send(new FSMFileLoadStateChangedMessage(false));
            return;
        }

        WeakReferenceMessenger.Default.Send(new FSMFileLoadStateChangedMessage(true));
    }

    private NodeViewModel CreateViewModelFromFSMNode(FSMNode node, List<FSMNode> nodeList, ref int depth)
    {
        if (_processedNodes.Contains(node.Guid))
            return null;

        _processedNodes.Add(node.Guid);

        NodeViewModel graphNode = GetNodeViewModel(node, depth * 400, 0);
        Nodes.Add(graphNode);

        for (int i = 0; i < node.BranchTransitions.Count; i++)
        {
            Transition trans = node.BranchTransitions[i];
            FSMNode toFsmNode = node.Children.FirstOrDefault(e => e.Guid == trans.FromNodeGuid);

            if (toFsmNode is null) // This is kinda weird, we're gonna use the full node list here, but this shouldn't ever be needed - at best the parent node is used
                toFsmNode = nodeList.FirstOrDefault(e => e.Guid == trans.FromNodeGuid);

            if (toFsmNode is null)
            {
                // This branch is invalid?
                Debug.WriteLine($"WARN: A transition refers to node {trans.ToNodeGuid}, but it does not exist in node list");
                continue;
            }

            NodeViewModel toNode = GetNodeViewModel(toFsmNode, (depth + 1) * 400, i * 200);
            if (!_processedNodes.Contains(toFsmNode.Guid))
            {
                int depth_ = depth + 1;
                CreateViewModelFromFSMNode(toFsmNode, nodeList, ref depth_);
            }

            ConnectionViewModel connection = Connections.FirstOrDefault(e => (e.Source == graphNode || e.Source == toNode) &&
                                                                             (e.Target == graphNode || e.Target == toNode));
            TransitionViewModel transition = new TransitionViewModel()
            {
                Source = graphNode,
                Target = toNode,
            };

            if (connection is null)
            {
                connection = new ConnectionViewModel()
                {
                    Source = graphNode,
                    Target = toNode,
                };
                Connections.Add(connection);
            }
            else
            {
                connection.ArrowHeadEnds = ArrowHeadEnds.Both;
            }

            connection.Transitions.Add(transition);

            if (trans.ConditionComponents.Count != 0)
            {
                connection.Title = $"{trans.ConditionComponents.Count} condition(s)";

                for (int j = 0; j < trans.ConditionComponents.Count; j++)
                    transition.ConditionComponents.Add(trans.ConditionComponents[j]);
            }
        }

        for (int i = 0; i < node.LeafTransitions.Count; i++)
        {

        }

        // When childLayerId_ is set, grab the first of the children
        if (node.ChildLayerId != -1 && node.Children.Count > 0)
        {
            int d = depth + 1;
            CreateViewModelFromFSMNode(node.Children[0], nodeList, ref d);
        }

        return graphNode;
    }

    private Dictionary<int, NodeViewModel> _guidToNodeVm = [];
    private HashSet<int> _processedNodes = [];

    private NodeViewModel GetNodeViewModel(FSMNode node, int x, int y)
    {
        if (_guidToNodeVm.TryGetValue(node.Guid, out NodeViewModel nodeViewModel))
            return nodeViewModel;

        nodeViewModel = new NodeViewModel()
        {
            Guid = node.Guid,
            Title = node.Guid.ToString(),
            Location = new Point(x, y),
        };

        if (node.ExecutionComponents.Count == 0)
            nodeViewModel.BorderBrush = Brushes.Black;

        foreach (BehaviorTreeComponent component in node.ExecutionComponents)
        {
            nodeViewModel.Components.Add(new NodeComponentViewModel(nodeViewModel)
            {
                Name = component.ToString(),
                Component = component,
            });
        }

        _guidToNodeVm.Add(node.Guid, nodeViewModel);

        return nodeViewModel;
    }

    public FSMNode BuildTreeFromCurrentGraph()
    {
        Dictionary<int, FSMNode> fsmNodes = [];

        foreach (ConnectionViewModel connection in Connections)
        {
            foreach (TransitionViewModel transition in connection.Transitions)
            {
                if (!fsmNodes.TryGetValue(connection.Source.Guid, out FSMNode sourceFsmNode))
                {
                    sourceFsmNode = new FSMNode();
                    sourceFsmNode.Guid = connection.Source.Guid;

                    Transition fsmTransition = new Transition(transition.Source.Guid, connection.Target.Guid);
                    foreach (BehaviorTreeComponent conditionComponent in transition.ConditionComponents)
                        fsmTransition.ConditionComponents.Add(conditionComponent);

                    foreach (NodeComponentViewModel componentViewModel in connection.Source.Components)
                        sourceFsmNode.ExecutionComponents.Add(componentViewModel.Component);

                    sourceFsmNode.BranchTransitions.Add(fsmTransition);

                    fsmNodes.Add(sourceFsmNode.Guid, sourceFsmNode);
                }

                if (!fsmNodes.TryGetValue(connection.Target.Guid, out FSMNode targetFsmNode))
                {
                    targetFsmNode = new FSMNode();
                    targetFsmNode.Guid = connection.Target.Guid;

                    foreach (NodeComponentViewModel componentViewModel in connection.Target.Components)
                        targetFsmNode.ExecutionComponents.Add(componentViewModel.Component);

                    fsmNodes.Add(targetFsmNode.Guid, targetFsmNode);
                }

                if (sourceFsmNode.Children.Find(e => e.Guid == targetFsmNode.Guid) is null)
                    sourceFsmNode.Children.Add(targetFsmNode);
            }
        }

        var root = fsmNodes[FSM.RootNode.Guid];

        // Build tail index
        int idx = 0;
        int tailIndex = GetTailIndex(root);
        root.TailIndexOfChildNodeGuids = idx;
        return root;
    }

    public int GetTailIndex(FSMNode node)
    {
        int cnt = 0;
        for (int i = 0; i < node.Children.Count; i++)
        {
            cnt++;
            cnt += GetTailIndex(node.Children[i]);
        }
        return cnt;
    }

}
