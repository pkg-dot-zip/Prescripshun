namespace PrescripshunLib.ExtensionMethods;

public static class IListExtensions
{
    /// <summary>
    /// Adds all of the <paramref name="elements"/> to the end of the <paramref name="collection"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection">Collection to add the <paramref name="elements"/> to.</param>
    /// <param name="elements">Elements to add to <paramref name="collection"/>.</param>
    public static void AddAll<T>(this IList<T> collection, params T[] elements)
    {
        collection.AddAll(elements as IEnumerable<T>);
    }

    /// <summary>
    /// Adds all of the <paramref name="elements"/> to the end of the <paramref name="collection"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection">Collection to add the <paramref name="elements"/> to.</param>
    /// <param name="elements">Elements to add to <paramref name="collection"/>.</param>
    public static void AddAll<T>(this IList<T> collection, IEnumerable<T> elements)
    {
        foreach (var element in elements) collection.Add(element);
    }
}