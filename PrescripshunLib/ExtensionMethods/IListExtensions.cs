namespace PrescripshunLib.ExtensionMethods
{
    public static class IListExtensions
    {
        public static void AddAll<T>(this IList<T> collection, params T[] elements)
        {
            foreach (var element in elements) collection.Add(element);
        }

        public static void AddAll<T>(this IList<T> collection, IEnumerable<T> elements)
        {
            foreach (var element in elements) collection.Add(element);
        }
    }
}
