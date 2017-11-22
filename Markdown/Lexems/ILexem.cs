using System.Collections.Generic;
using System.Text;

namespace Markdown.Lexems
{
    public interface ILexem
    {
        string Raw { get; }
        int Render(IList<ILexem> lexems, int curIndex, int end, StringBuilder sb);
    }
}