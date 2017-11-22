using System.Collections.Generic;
using System.Text;

namespace Markdown.Lexems
{
    public class EscapeLexem : ILexem
    {
        private readonly char escapedChar;

        public EscapeLexem(string raw, char escapedChar)
        {
            this.escapedChar = escapedChar;
            Raw = raw;
        }

        public string Raw { get; }

        public int Render(IList<ILexem> lexems, int curIndex, int end, StringBuilder sb)
        {
            sb.Append(escapedChar);
            return 1;
        }
    }
}