using System;
using Markdown.Lexems;

namespace Markdown.LexemConsumers
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

            var raw = markdown.Substring(i, 2);
            var escapedChar = markdown[i + 1];
            return new EscapeLexem(raw, escapedChar);
        }
    }
}