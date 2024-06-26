﻿using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RelinkFSMToolkit.Views;

/// <summary>
/// Interaction logic for TopMenuView.xaml
/// </summary>
public partial class TopMenuView : UserControl
{
    public TopMenuView()
    {
        InitializeComponent();
    }
}

public class MenuItemContainerTemplateSelector : ItemContainerTemplateSelector
{
    public override DataTemplate SelectTemplate(object item, ItemsControl parentItemsControl)
    {
        var key = new DataTemplateKey(item.GetType());
        return (DataTemplate)parentItemsControl.FindResource(key);
    }
}
