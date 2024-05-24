using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using Microsoft.Msagl.Core.Geometry.Curves;
using System.Windows.Media;
using System.Windows;

namespace RelinkFSMToolkit;

static class Utils
{
    public static int GetInt32(this JsonProperty jsonProperty, string name)
    {
        if (!jsonProperty.Value.TryGetProperty(name, out JsonElement propValue) || !propValue.TryGetInt32(out int val))
            throw new InvalidDataException($"Component '{jsonProperty.Name}' is missing or invalid mandatory '{name}' property.");

        return val;
    }

    public static float GetSingle(this JsonProperty jsonProperty, string name)
    {
        if (!jsonProperty.Value.TryGetProperty(name, out JsonElement propValue) || !propValue.TryGetSingle(out float val))
            throw new InvalidDataException($"Component '{jsonProperty.Name}' is missing or invalid mandatory '{name}' property.");

        return val;
    }

    public static uint GetUInt32(this JsonProperty jsonProperty, string name)
    {
        if (!jsonProperty.Value.TryGetProperty(name, out JsonElement propValue) || !propValue.TryGetUInt32(out uint val))
            throw new InvalidDataException($"Component '{jsonProperty.Name}' is missing or invalid mandatory '{name}' property.");

        return val;
    }

    public static ulong GetUInt64(this JsonProperty jsonProperty, string name)
    {
        if (!jsonProperty.Value.TryGetProperty(name, out JsonElement propValue) || !propValue.TryGetUInt64(out ulong val))
            throw new InvalidDataException($"Component '{jsonProperty.Name}' is missing or invalid mandatory '{name}' property.");

        return val;
    }

    public static string GetString(this JsonProperty jsonProperty, string name)
    {
        if (!jsonProperty.Value.TryGetProperty(name, out JsonElement propValue))
            throw new InvalidDataException($"Component '{jsonProperty.Name}' is missing or invalid mandatory '{name}' property.");

        return propValue.GetString();
    }

    public static bool GetBool(this JsonProperty jsonProperty, string name)
    {
        if (!jsonProperty.Value.TryGetProperty(name, out JsonElement propValue))
            throw new InvalidDataException($"Component '{jsonProperty.Name}' is missing or invalid mandatory '{name}' property.");

        return propValue.GetBoolean();
    }

    public static ICurve CreateCurve(double w, double h)
    {
        return CurveFactory.CreateRectangle(w, h, new Microsoft.Msagl.Core.Geometry.Point());
    }

    public static TChild FindVisualChild<TChild>(DependencyObject obj, int index = 0) where TChild : DependencyObject
    {
        for (int i = index; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(obj, i);
            if (child != null && child is TChild)
            {
                return (TChild)child;
            }
            else
            {
                TChild childOfChild = FindVisualChild<TChild>(child);
                if (childOfChild != null)
                    return childOfChild;
            }
        }
        return null;
    }

}
