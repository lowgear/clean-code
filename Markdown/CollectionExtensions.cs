using System.Collections.Generic;

namespace Markdown
{
    public static class CollectionExtensions
    {
        public static void AddAll<T>(this ICollection<T> collection, IEnumerable<T> toAdd)
        {
            foreach (var element in toAdd)
                collection.Add(element);
        }

        public static void RemoveAll<T>(this ICollection<T> collection, IEnumerable<T> toRemove)
        {
            foreach (var element in toRemove)
                collection.Remove(element);
        }
    }
}