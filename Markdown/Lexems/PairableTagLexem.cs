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

        private static bool MayBeOpening(IList<ILexem> lexems, int curIndex, int end)
        {
            var nextLexemExists = curIndex + 1 < end;
            bool AndIsNotEscapeLexem() => !(lexems[curIndex + 1] is EscapeLexem);
            bool WhichStartsWithWhiteSpace() => char.IsWhiteSpace(lexems[curIndex + 1].Raw[0]);

            return !(nextLexemExists && AndIsNotEscapeLexem() && WhichStartsWithWhiteSpace()) &&
                   !IsBetweenLettersOrDigits(lexems, curIndex);
        }

        private static bool MayBeClosing(IList<ILexem> lexems, int curIndex)
        {
            var previousLexemExists = curIndex - 1 >= 0;
            bool AndIsNotEscapeLexem() => !(lexems[curIndex - 1] is EscapeLexem);
            bool WhichEndsWithWhiteSpace() => char.IsWhiteSpace(lexems[curIndex - 1].Raw.Last());

            return !(previousLexemExists && AndIsNotEscapeLexem() && WhichEndsWithWhiteSpace()) &&
                   !IsBetweenLettersOrDigits(lexems, curIndex);
        }

        private static bool IsBetweenLettersOrDigits(IList<ILexem> lexems, int curIndex)
        {
            var end = lexems.Count;

            var nextLexemExists = curIndex + 1 < end;
            bool AndNextIsNotEscapeLexem() => !(lexems[curIndex + 1] is EscapeLexem);
            bool WhichStartsWithLetterOrDigit() => char.IsLetterOrDigit(lexems[curIndex + 1].Raw[0]);

            var previousLexemExists = curIndex - 1 >= 0;
            bool AndPrevIsNotEscapeLexem() => !(lexems[curIndex - 1] is EscapeLexem);
            bool WhichEndsWithLetterOrDigit() => char.IsLetterOrDigit(lexems[curIndex - 1].Raw.Last());

            return previousLexemExists &&
                   AndPrevIsNotEscapeLexem() &&
                   WhichEndsWithLetterOrDigit() &&
                   nextLexemExists &&
                   AndNextIsNotEscapeLexem() &&
                   WhichStartsWithLetterOrDigit();
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
            return lexem is PairableTagLexem &&
                   MayBeClosing(lexems, i) &&
                   lexem.Raw == Raw &&
                   !char.IsWhiteSpace(prevLexem.Raw.Last());
        }
    }
}