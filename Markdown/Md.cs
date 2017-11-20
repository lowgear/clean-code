using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace Markdown
{
    public class Md
    {
        protected delegate bool MarkdownHandler(string markdown, ref int i, int end);

        protected readonly List<MarkdownHandler> handlers = new List<MarkdownHandler>();
        protected readonly StringBuilder sb = new StringBuilder();

        public Md()
        {
            //ORDER IS IMPORTANT!!!
            handlers.Add(HandleBold);
            handlers.Add(HandleItalic);
        }

        private bool HandleItalic(string markdown, ref int i, int end)
        {
            return HandleSimpleTag(markdown, ref i, end, "_", "em");
        }

        private bool HandleBold(string markdown, ref int i, int end)
        {
            return HandleSimpleTag(markdown, ref i, end, "__", "strong");
        }

        private bool HandleSimpleTag(string markdown, ref int i, int end, string pattern, string tag)
        {
            if (!markdown.HasAt(pattern, i) || !markdown.AtIs(i + pattern.Length, c => !char.IsWhiteSpace(c)))
                return false;

            int closingIndex;
            try
            {
                closingIndex = markdown
                    .FindAllFromTo(pattern, i + 1, end)
                    .First(j => !char.IsWhiteSpace(markdown[j]));
            }
            catch (InvalidOperationException)
            {
                return false;
            }

            sb.Append("<");
            sb.Append(tag);
            sb.Append(">");

            i += pattern.Length;
            RenderToHtml(markdown, ref i, closingIndex);

            sb.Append("</");
            sb.Append(tag);
            sb.Append(">");

            i = closingIndex + pattern.Length;
            return true;
        }

        public string RenderToHtml(string markdown)
        {
            sb.Clear();
            var i = 0;
            RenderToHtml(markdown, ref i, markdown.Length);
            return sb.ToString();
        }

        private void RenderToHtml(string markdown, ref int i, int end)
        {
            while (i < end)
            {
                var matchedSomeHandler = false;
                foreach (var markdownHandler in handlers)
                {
                    if (!markdownHandler(markdown, ref i, end)) continue;
                    matchedSomeHandler = true;
                    break;
                }
                if (matchedSomeHandler) continue;
                sb.Append(markdown[i]);
                i++;
            }
        }
    }

    [TestFixture]
    public class Md_ShouldRender
    {
        private Md md;

        [SetUp]
        public void SetUp()
        {
            md = new Md();
        }


        [Test]
        public void LeaveUntouched_TextWithoutUnderscores()
        {
            CheckMdOn("no underscores", "no underscores");
        }

        [Test]
        public void SingleUnderscores_ShouldBeReplacedWithTagEm()
        {
            CheckMdOn("_text_", "<em>text</em>");
        }

        [Test]
        public void DoubleUnderscores_ShouldBeReplacedWithTagStrong()
        {
            CheckMdOn("__text__", "<strong>text</strong>");
        }

        private void CheckMdOn(string subject, string expected)
        {
            md.RenderToHtml(subject).Should().Be(expected);
        }
    }
}