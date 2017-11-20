using System;
using System.Collections.Generic;

namespace Markdown
{
    public static class StringExtensions
    {
        public static bool HasAt(this string str, string pattern, int i)
        {
            var offset = i;
            var end = Math.Min(i + pattern.Length, str.Length);
            for (; i < end; i++)
                if (str[i] != pattern[i - offset])
                    return false;
            return true;
        }

        public static bool AtIs(this string str, int i, Func<char, bool> predicate)
        {
            return i < str.Length && predicate(str[i]);
        }

        public static IEnumerable<int> FindAllFromTo(this string str, string pattern, int from, int to)
        {
            var end = to - pattern.Length;
            for (var i = from; i <= end; i++)
            {
                if (str.HasAt(pattern, i))
                    yield return i;
            }
        }
    }
}