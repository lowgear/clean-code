using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using FluentAssertions.Types;

namespace Markdown
{
    public class Md
    {
        private readonly SortedSet<string> markdownTags = new SortedSet<string>(StringComparer.Ordinal);
        private readonly Dictionary<string, Type> tagsToLexems = new Dictionary<string, Type>();

        /*private bool HandleSimpleTag(string markdown, ref int i, int end, string pattern, string tag)
        {
            if (!MayBeTagInAt(markdown, pattern, i) ||
                markdown.AtIs(i + pattern.Length, char.IsWhiteSpace))
                return false;

            int closingIndex;
            try
            {
                closingIndex = markdown
                    .FindAllFromTo(pattern, i + 1, end)
                    .First(j =>
                        MayBeTagInAt(markdown, pattern, j) && !markdown.AtIs(j - 1,
                            char.IsWhiteSpace));
            }
            catch (InvalidOperationException)
            {
                return false;
            }

            AddToResult("<");
            AddToResult(tag);
            AddToResult(">");

            i += pattern.Length;
            RenderToHtml(markdown, ref i, closingIndex);

            AddToResult("</");
            AddToResult(tag);
            AddToResult(">");

            i = closingIndex + pattern.Length;
            return true;
        }*/

        public void AddTag(string tag, Type tagType)
        {
            markdownTags.Add(tag);
            tagsToLexems[tag] = tagType;
        }

        public string RenderToHtml(string markdown)
        {
            var lexems = ToLexems(markdown).ToArray();
            var sb = new StringBuilder();
            lexems[0].Render(lexems, 0, lexems.Length, sb);
            return sb.ToString();
        }

        private IEnumerable<ILexem> ToLexems(string markdown)
        {
            for (var i = 0; i < markdown.Length;)
            {
                var j = Enumerable.Range(i, markdown.Length + 1)
                    .First(x => x == markdown.Length ||
                                markdownTags.Any(tag => markdown.HasAt(tag, x)));

                if (i != j)
                    yield return new TextLexem(markdown.Substring(i, j - i));
                i = j;

                var curTag = markdownTags
                    .Reverse()
                    .FirstOrDefault(tag => markdown.HasAt(tag, j));
                if (curTag != null)
                {
                    yield return (ILexem) tagsToLexems[curTag].GetConstructor(new[] {typeof(string)})
                        ?.Invoke(new object[] {curTag});
                    i = j + curTag.Length;
                }
            }
        }
    }

    internal class TextLexem : ILexem
    {
        public TextLexem(string str)
        {
            Raw = str;
            Rendered = str;
        }

        public string Raw { get; }
        public string Rendered { get; }

        public void Render(IList<ILexem> lexems, int curIndex, int end, StringBuilder sb)
        {
            sb.Append(Rendered);
            curIndex++;
            if (curIndex == end)
                return;
            lexems[curIndex].Render(lexems, curIndex, end, sb);
        }
    }
}