using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            lexems[0].Render(lexems, 0, lexems.Length, sb);
            return sb.ToString();
        }

        private IEnumerable<ILexem> ToLexems(string markdown)
        {
            var lastLexemEndedAt = 0;
            for (var i = 0; i < markdown.Length;)
            {
                var bestConsumer = lexemConsumers.MaxBy(consumer => consumer.Consumes(markdown, i));

                var lexemLength = bestConsumer.Consumes(markdown, i);
                if (lexemLength == 0)
                {
                    i++;
                    continue;
                }
                if (i != lastLexemEndedAt)
                    yield return new TextLexem(markdown.Substring(lastLexemEndedAt, i - lastLexemEndedAt));
                yield return bestConsumer.Consume(markdown, i);

                i = i + lexemLength;
                lastLexemEndedAt = i;
            }
            if (lastLexemEndedAt != markdown.Length)
                yield return new TextLexem(markdown.Substring(lastLexemEndedAt));
        }
    }
}