namespace Markdown
{
    public interface ILexemConsumer
    {
        int Consumes(string markdown, int i);
        ILexem Consume(string markdown, int i);
    }
}