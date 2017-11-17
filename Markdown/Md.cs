using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Markdown
{
    public class Md
    {
        private readonly Dictionary<int, string> underscoresLengthToTag = new Dictionary<int, string>();


        public Md(params string[] tags)
        {
            for (int i = 0; i < tags.Length; i++)
            {
                var tag = tags[i];
                underscoresLengthToTag[i + 1] = tag;
            }
        }

        public string RenderToHtml(string markdown)
        {
            var nodes = SplitToUnderscoresSequencesAndText(markdown);
            var htmlTokens = TranslateMarkdownTokensToHtmlTokens(nodes);
            var html = string.Join("", htmlTokens);
            return html;
        }

        private static List<string> TranslateMarkdownTokensToHtmlTokens(IEnumerable<INode> nodes)
        {
            throw new NotImplementedException();
        }

        private static IEnumerable<INode> SplitToUnderscoresSequencesAndText(string markdown)
        {
            throw new NotImplementedException();
        }
    }

    [TestFixture]
    public class Md_ShouldRender
    {
        private Md md;

        [SetUp]
        public void SetUp()
        {
            md = new Md("em", "strong");
        }


        [Test]
        public void LeaveUntouched_TextWithoutUnderscores()
        {
            CheckMdOn("no underscores", "no underscores");
        }

        [Test]
        public void SingleUnderscores_ShouldBeReplacedWithTag()
        {
            CheckMdOn("_text_", "<em>text</em>");
        }

        private void CheckMdOn(string subject, string expected)
        {
            md.RenderToHtml(subject).Should().Be(expected);
        }
    }
}