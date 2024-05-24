using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace RelinkFSMToolkit;

public static class ThemeManager
{
    private static readonly string _assemblyName = Assembly.GetEntryAssembly()?.GetName().Name;

    private static readonly Dictionary<string, List<Uri>> _themesUris = [];
    private static readonly Dictionary<string, List<ResourceDictionary>> _themesResources = [];

    public static string ActiveTheme { get; private set; }

    private static readonly List<string> _availableThemes = [];
    public static IReadOnlyCollection<string> AvailableThemes => _availableThemes;

    static ThemeManager()
    {
        PreloadTheme("Dark");
        PreloadTheme("Light");
    }

    private static List<ResourceDictionary> FindExistingResources(List<Uri> uris)
    {
        var result = new List<ResourceDictionary>();
        foreach (var d in Application.Current.Resources.MergedDictionaries)
        {
            if (d.Source != null && uris.Contains(d.Source))
            {
                result.Add(d);
            }
        }

        return result;
    }

    private static void PreloadTheme(string themeName)
    {
        if (!_themesUris.TryGetValue(themeName, out var preload))
        {
            preload = new List<Uri>(3)
            {
                new($"pack://application:,,,/{nameof(RelinkFSMToolkit)};component/Themes/{themeName}.xaml"),
            };

            if (_assemblyName != null)
            {
                preload.Add(new Uri($"pack://application:,,,/{_assemblyName};component/Themes/{themeName}.xaml"));
            }

            _themesUris.Add(themeName, preload);
        }

        var resources = FindExistingResources(preload);
        if (resources.Count == 0)
        {
            for (int i = 0; i < preload.Count; i++)
            {
                try
                {
                    resources.Add(new ResourceDictionary
                    {
                        Source = preload[i]
                    });
                }
                catch
                {

                }
            }
        }
        else if (ActiveTheme == null)
        {
            ActiveTheme = themeName;
        }

        _themesResources.Add(themeName, resources);
        _availableThemes.Add(themeName);
    }

    public static void SetTheme(string themeName)
    {
        if (!_themesResources.ContainsKey(themeName))
        {
            PreloadTheme(themeName);
        }

        // Load new theme if it is valid
        if (_themesResources.TryGetValue(themeName, out var resources))
        {
            foreach (var res in resources)
            {
                Application.Current.Resources.MergedDictionaries.Add(res);
            }

            // Unload current theme
            if (ActiveTheme != null)
            {
                foreach (var res in _themesResources[ActiveTheme])
                {
                    Application.Current.Resources.MergedDictionaries.Remove(res);
                }
            }

            ActiveTheme = themeName;
        }
    }
}
