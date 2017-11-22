using System;
using System.Collections.Generic;
using System.Linq;

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
            return 0 <= i && i < str.Length && predicate(str[i]);
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

        public static IEnumerable<char> CharsFromTo(this string str, int from, int to)
        {
            var i = Math.Max(from, 0);
            var end = Math.Min(to - 1, str.Length - 1);
            for (; i < end; i++)
                yield return str[i];
        }
    }
}