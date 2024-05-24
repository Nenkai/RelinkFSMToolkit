using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

using Xceed.Wpf.Toolkit.PropertyGrid;

using Nodify;

using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Routing;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.Core.Layout;

using RelinkFSMToolkit.ViewModels;

namespace RelinkFSMToolkit.Views;

/// <summary>
/// Interaction logic for EditorView.xaml
/// </summary>
public partial class EditorView : UserControl
{
    public EditorView()
    {
        InitializeComponent();
        DataContextChanged += EditorView_DataContextChanged;
    }

    private void EditorView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (DataContext is EditorViewModel editorViewModel)
        {
            editorViewModel.OnNodesLoaded += EditorViewModel_NodesLoaded;
        }
    }

    private void EditorViewModel_NodesLoaded()
    {
        // Ensure to update everything
        Editor.UpdateLayout();
        // Nodes are now loaded. Proceed to create an automated layout
        
        var graph = new GeometryGraph();
        foreach (NodeViewModel nodeViewModel in Editor.Items)
        {
            Nodify.ItemContainer nodeItemContainer = (Nodify.ItemContainer)(Editor.ItemContainerGenerator.ContainerFromItem(nodeViewModel));

            // Getting the ContentPresenter of myListBoxItem
            ContentPresenter nodeContentPresenter = Utils.FindVisualChild<ContentPresenter>(nodeItemContainer);

            // Finding textBlock from the DataTemplate that is set on that ContentPresenter
            DataTemplate dt = nodeContentPresenter.ContentTemplate;
            Nodify.StateNode node = (Nodify.StateNode)dt.FindName("Node", nodeContentPresenter);

            graph.Nodes.Add(new Microsoft.Msagl.Core.Layout.Node(Utils.CreateCurve(nodeViewModel.Size.Width, nodeViewModel.Size.Height), nodeViewModel));
        }

        foreach (TransitionViewModel connection in Editor.Connections)
        {
            graph.Edges.Add(new Edge(graph.FindNodeByUserData(connection.Source), graph.FindNodeByUserData(connection.Target), 100, 100, 5));
        }

        var settings = new SugiyamaLayoutSettings
        {
            Transformation = PlaneTransformation.Rotation(Math.PI / 2), // Make LR (Left-To-Right)
            EdgeRoutingSettings = { EdgeRoutingMode = EdgeRoutingMode.StraightLine },
        };

        var layout = new LayeredLayout(graph, settings);
        layout.Run();

        foreach (var graphNode in graph.Nodes)
            (graphNode.UserData as NodeViewModel).Location = new Point(graphNode.Center.X, graphNode.Center.Y);

        var firstNode = graph.Nodes.First().UserData as NodeViewModel;
        Editor.BringIntoView(firstNode.Location, animated: false);
    }

    private void PropertyGrid_SelectedObjectChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        var grid = sender as PropertyGrid;

        foreach (PropertyItem prop in grid.Properties)
        {
            if (prop.PropertyType == typeof(string))
                continue;

            prop.IsExpanded = true; //This will expand the property.
            prop.IsExpandable = false; //This will remove the ability to toggle the expanded state.
        }

    }

    private void LineConnection_MouseDown(object sender, MouseButtonEventArgs e)
    {
        ;
    }

    private void Editor_Selected(object sender, RoutedEventArgs e)
    {

    }

    private void MenuItem_Click(object sender, RoutedEventArgs e)
    {

    }
}
