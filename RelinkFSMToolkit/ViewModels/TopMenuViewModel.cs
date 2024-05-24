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

public partial class TopMenuViewModel : ObservableObject
{
    public ObservableCollection<IMenuItemViewModel> MenuItems { get; set; } = [];
    public ObservableCollection<MenuItemViewModel> _themeMenuItems = [];

    [ObservableProperty]
    public Theme _dockManagerTheme;

    private readonly EditorViewModel _editorViewModel;

    private MenuItemViewModel _menuItem_SaveCurrentFSMGraph;

    public TopMenuViewModel() { }

    public TopMenuViewModel(EditorViewModel editorViewModel)
    {
        _editorViewModel = editorViewModel;

        _menuItem_SaveCurrentFSMGraph = new MenuItemViewModel()
        {
            Header = "Current FSM Graph",
            Command = new RelayCommand(OnSave),
            IconKind = PackIconMaterialKind.Graph,
            Checked = false,
        };

        WeakReferenceMessenger.Default.Register<FSMFileLoadStateChangedMessage>(this, (recipient, message) 
            => _menuItem_SaveCurrentFSMGraph.Enabled = message.Value);

        BuildMenu();
    }

    public void BuildMenu()
    {
        var fileMenuItem = new RootMenuItemViewModel()
        {
            Header = "File",
            MenuItems = [new MenuItemViewModel()
            {
                Header = "New",
                Enabled = true,
                MenuItems = [new MenuItemViewModel()
                {
                    Header = "Quest",
                    Command = new RelayCommand(OnNewQuestClicked),
                    IconKind = PackIconMaterialKind.Script,
                    Enabled = true,
                }]
            },
            new MenuItemViewModel()
            {
                Header = "Open File",
                Command = new RelayCommand(OnOpenFileClicked),
                IconKind = PackIconMaterialKind.FileMoveOutline,
                Enabled = true,
            },
            MenuSeparatorViewModel.Default,
            new MenuItemViewModel()
            {
                Header = "Save",
                MenuItems = [_menuItem_SaveCurrentFSMGraph],
                Enabled = true,
                
            },
            MenuSeparatorViewModel.Default,
            new MenuItemViewModel()
            {
                Header = "Exit",
                Command = new RelayCommand(OnExit),
                IconKind = PackIconMaterialKind.ExitToApp,
                Enabled = true,
            }]
        };
        MenuItems.Add(fileMenuItem);

        var viewMenuItem = new RootMenuItemViewModel()
        {
            Header = "View"
        };
        MenuItems.Add(viewMenuItem);

        var themesMenuItem = new MenuItemViewModel()
        {
            Header = "Themes"
        };
        viewMenuItem.MenuItems.Add(themesMenuItem);

        foreach (var theme in ThemeManager.AvailableThemes)
        {
            ICommand themeChangedCommand = new RelayCommand<string>(OnThemeChanged);
            var themeMenuItem = new MenuItemViewModel
            {
                Header = theme,
                Command = themeChangedCommand,
                Parameter = theme,
                Enabled = true,
            };

            if (ThemeManager.ActiveTheme == theme)
            {
                themeMenuItem.Checked = true;
                SetDockManagerTheme(theme);
            }

            themesMenuItem.MenuItems.Add(themeMenuItem);
            _themeMenuItems.Add(themeMenuItem);
        }
    }

    public void OnOpenFileClicked()
    {
        OpenFileDialog openFile = new OpenFileDialog();
        openFile.CheckFileExists = true;

        if (openFile.ShowDialog() == true)
            WeakReferenceMessenger.Default.Send(new FileOpenRequestMessage(openFile.FileName));
    }

    public void OnSave()
    {
        SaveFileDialog saveFile = new SaveFileDialog();
        saveFile.Filter = "JSON Files|*.json|" +
                          "MessagePack|*.msg";

        if (saveFile.ShowDialog() == true)
            WeakReferenceMessenger.Default.Send(new FileSaveRequestMessage(saveFile.FileName));
    }

    public void OnExit()
    {

    }

    public void OnNewQuestClicked()
    {

    }

    public void OnThemeChanged(string parameter)
    {
        ThemeManager.SetTheme(parameter);
        SetDockManagerTheme(parameter);

        foreach (var i in _themeMenuItems)
            i.Checked = i.Header == parameter;
    }

    private void SetDockManagerTheme(string theme)
    {
        if (theme == "Dark")
            DockManagerTheme = new Vs2013DarkTheme();
        else
            DockManagerTheme = new Vs2013LightTheme();
    }
}
