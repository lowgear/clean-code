using System;
using System.Collections.Generic;
using System.Text;

namespace Markdown
{
    public class EscapeConsumer : ILexemConsumer
    {
        private readonly char escapeChar;

        public EscapeConsumer(char escapeChar)
        {
            this.escapeChar = escapeChar;
        }

        public int Consumes(string markdown, int i)
        {
            if (markdown[i] != escapeChar) return 0;
            return Math.Min(2, markdown.Length - i);
        }

        public ILexem Consume(string markdown, int i)
        {
            var consumes = Consumes(markdown, i);
            if (consumes == 0)
                throw new ArgumentException();
            if (consumes == 1)
                return new TextLexem(escapeChar.ToString());
            return new EscapeLexem(markdown.Substring(i, Math.Min(2, markdown.Length - i)), markdown[i + 1]);
        }
    }

    public class EscapeLexem : ILexem
    {
        private readonly char escapedChar;

        public EscapeLexem(string raw, char escapedChar)
        {
            this.escapedChar = escapedChar;
            Raw = raw;
        }

        public string Raw { get; }
        public string Rendered => escapedChar.ToString();

        public void Render(IList<ILexem> lexems, int curIndex, int end, StringBuilder sb)
        {
            sb.Append(escapedChar);
            curIndex++;
            if (curIndex < lexems.Count)
                lexems[curIndex].Render(lexems, curIndex, end, sb);
        }
    }
}