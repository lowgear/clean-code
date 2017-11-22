using System.Collections.Generic;
using System.Text;

namespace Markdown
{
    public interface ILexem
    {
        string Raw { get; }
        string Rendered { get; }
        void Render(IList<ILexem> lexems, int curIndex, int end, StringBuilder sb);
    }
}