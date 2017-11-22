using System.Collections.Generic;
using System.Text;

namespace Markdown.Lexems
{
    internal class TextLexem : ILexem
    {
        public TextLexem(string str)
        {
            Raw = str;
        }

        public string Raw { get; }

        public int Render(IList<ILexem> lexems, int curIndex, int end, StringBuilder sb)
        {
            sb.Append(Raw);
            return 1;
        }
    }
}