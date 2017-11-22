using System;
using System.Collections.Generic;
using Markdown.Lexems;

namespace Markdown
{
    public class UnderscoresConsumer : ILexemConsumer
    {
        private readonly Dictionary<int, string> tagDictionary;

        public UnderscoresConsumer(Dictionary<int, string> tagDictionary)
        {
            this.tagDictionary = tagDictionary;
        }

        public int Consumes(string markdown, int i)
        {
            int j;
            for (j = i; j < markdown.Length && markdown[j] == '_'; j++);
            return j - i;
        }

        public ILexem Consume(string markdown, int i)
        {
            var length = Consumes(markdown, i);
            if (length == 0)
                throw new ArgumentException();
            var raw = markdown.Substring(i, length);
            if (tagDictionary.ContainsKey(length))
                return new PairableTagLexem(raw, tagDictionary[length]);
            return new TextLexem(raw);
        }
    }
}