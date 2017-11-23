using System;
using System.Collections.Generic;
using System.Linq;
using Markdown.Lexems;

namespace Markdown.LexemConsumers
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
            var lengthOfUndescoreSequence = Enumerable.Range(0, markdown.Length)
                .First(length => i + length >= markdown.Length || markdown[i + length] != '_');
            return lengthOfUndescoreSequence;
        }

        public ILexem Consume(string markdown, int i)
        {
            var length = Consumes(markdown, i);
            if (length == 0)
                throw new ArgumentException("Zero chars were consumed which probably will lead to an infinite loop.");
            var raw = markdown.Substring(i, length);
            if (tagDictionary.ContainsKey(length))
                return new PairableTagLexem(raw, tagDictionary[length]);
            return new TextLexem(raw);
        }
    }
}