using System.Collections.Generic;
using System.Text;

namespace Markdown
{
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