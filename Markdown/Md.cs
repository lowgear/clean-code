using System.Collections.Generic;
using System.Linq;
using System.Text;
using Markdown.LexemConsumers;
using Markdown.Lexems;
using MoreLinq;

namespace Markdown
{
    public class Md
    {
        private readonly HashSet<ILexemConsumer> lexemConsumers = new HashSet<ILexemConsumer>();

        public void AddLexemConsumer(ILexemConsumer lexemConsumer)
        {
            lexemConsumers.Add(lexemConsumer);
        }

        public string RenderToHtml(string markdown)
        {
            var lexems = ToLexems(markdown).ToArray();
            var sb = new StringBuilder();
            RenderRange(lexems, 0, lexems.Length, sb);
            return sb.ToString();
        }

        private IEnumerable<ILexem> ToLexems(string markdown)
        {
            var lastLexemEndedAt = 0;
            for (var index = 0; index < markdown.Length; index++)
            {
                var bestConsumer = lexemConsumers.MaxBy(consumer => consumer.Consumes(markdown, index));

                var lexemLength = bestConsumer.Consumes(markdown, index);
                if (lexemLength == 0)
                    continue;

                if (index != lastLexemEndedAt)
                    yield return new TextLexem(markdown.Substring(lastLexemEndedAt, index - lastLexemEndedAt));

                yield return bestConsumer.Consume(markdown, index);

                index = index + lexemLength - 1;
                lastLexemEndedAt = index + 1;
            }

            if (lastLexemEndedAt != markdown.Length)
                yield return new TextLexem(markdown.Substring(lastLexemEndedAt));
        }

        public static void RenderRange(IList<ILexem> lexems, int from, int to, StringBuilder sb)
        {
            var curIndex = from;
            while (curIndex < to)
                curIndex += lexems[curIndex].Render(lexems, curIndex, to, sb);
        }
    }
}