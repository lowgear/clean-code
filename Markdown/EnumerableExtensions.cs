using System.Collections.Generic;
using System.Linq;

namespace Markdown
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Excluding<T>(this IEnumerable<T> enumerable, IEnumerable<T> toExclude)
        {
            return enumerable.Where(e => !toExclude.Contains(e));
        }

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