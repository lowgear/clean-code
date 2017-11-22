using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Markdown.Lexems
{
    public class PairableTagLexem : ILexem
    {
        private readonly string tag;

        public PairableTagLexem(string raw, string tag)
        {
            this.tag = tag;
            Raw = raw;
        }

        public string Raw { get; }

        public int Render(IList<ILexem> lexems, int curIndex, int end, StringBuilder sb)
        {
            if (!MayBeOpening(lexems, curIndex, end))
            {
                sb.Append(Raw);
                return 1;
            }

            var pairIndex = Enumerable.Range(curIndex + 1, end - curIndex - 1)
                .FirstOrDefault(i => IsPair(lexems, i));
            if (pairIndex == default(int))
            {
                sb.Append(Raw);
                return 1;
            }

            sb.Append(MakeOpeningTag());
            Md.RenderRange(lexems, curIndex + 1, pairIndex, sb);
            sb.Append(MakeClosingTag());
            return pairIndex - curIndex + 1;
        }

        private bool MayBeOpening(IList<ILexem> lexems, int curIndex, int end)
        {
            return !(curIndex + 1 < end &&
                     !(lexems[curIndex + 1] is EscapeLexem) &&
                     char.IsWhiteSpace(lexems[curIndex + 1].Raw[0])) &&
                   !IsBetweenLettersOrDigits(lexems, curIndex);
        }

        private bool MayBeClosing(IList<ILexem> lexems, int curIndex)
        {
            return !(curIndex - 1 >= 0 &&
                     !(lexems[curIndex - 1] is EscapeLexem) &&
                    char.IsWhiteSpace(lexems[curIndex - 1].Raw.Last())) &&
                   !IsBetweenLettersOrDigits(lexems, curIndex);
        }

        private string MakeOpeningTag()
        {
            return "<" + tag + ">";
        }

        private string MakeClosingTag()
        {
            return "</" + tag + ">";
        }

        private bool IsPair(IList<ILexem> lexems, int i)
        {
            var lexem = lexems[i];
            var prevLexem = lexems[i - 1];
            return lexem is PairableTagLexem tagLexem &&
                   tagLexem.MayBeClosing(lexems, i) &&
                   lexem.Raw == Raw &&
                   !char.IsWhiteSpace(prevLexem.Raw.Last());
        }

        private static bool IsBetweenLettersOrDigits(IList<ILexem> lexems, int curIndex)
        {
            return curIndex - 1 >= 0 &&
                   !(lexems[curIndex - 1] is EscapeLexem) &&
                   char.IsLetterOrDigit(lexems[curIndex - 1].Raw.Last()) &&
                   curIndex + 1 < lexems.Count &&
                   !(lexems[curIndex + 1] is EscapeLexem) &&
                   char.IsLetterOrDigit(lexems[curIndex + 1].Raw[0]);
        }
    }
}