namespace Markdown
{
    public interface INode
    {
    }

    class TagNode : INode
    {
        public int Length { get; }

        public TagNode(int length)
        {
            Length = length;
        }
    }

    class TextNode : INode
    {
        public string Text { get; }

        public TextNode(string text)
        {
            Text = text;
        }
    }
}