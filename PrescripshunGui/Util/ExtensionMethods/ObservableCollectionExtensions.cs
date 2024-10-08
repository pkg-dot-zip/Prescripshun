using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PrescripshunGui.Util.ExtensionMethods;

internal static class ObservableCollectionExtensions
{
    public static void AddAll<T>(this ObservableCollection<T> collection, params T[] elements)
    {
        foreach (var element in elements) collection.Add(element);
    }

    public static void AddAll<T>(this ObservableCollection<T> collection, IEnumerable<T> elements)
    {
        foreach (var element in elements) collection.Add(element);
    }
}