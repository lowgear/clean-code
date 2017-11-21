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

        protected readonly List<MarkdownHandler> Handlers = new List<MarkdownHandler>();

        protected readonly Dictionary<MarkdownHandler, IEnumerable<MarkdownHandler>> HandlerBlocks =
            new Dictionary<MarkdownHandler, IEnumerable<MarkdownHandler>>();

        protected readonly HashSet<MarkdownHandler> ExludedHandlers = new HashSet<MarkdownHandler>();
        protected readonly StringBuilder Sb = new StringBuilder();

        protected const char Escape = '\\';

        public Md()
        {
            //ORDER IS IMPORTANT!!!
            Handlers.Add(HandleBold);
            HandlerBlocks[HandleBold] = new List<MarkdownHandler> {HandleBold};

            Handlers.Add(HandleItalic);
            HandlerBlocks[HandleItalic] = new List<MarkdownHandler> {HandleItalic};

//            Handlers.Add(HandleCode);
//            HandlerBlocks[HandleCode] = new List<MarkdownHandler> { HandleItalic, HandleBold, HandleCode };
        }

        private bool HandleItalic(string markdown, ref int i, int end)
        {
            return HandleSimpleTag(markdown, ref i, end, "_", "em") ||
                   HandleSimpleTag(markdown, ref i, end, "*", "em");
        }

        private bool HandleBold(string markdown, ref int i, int end)
        {
            return HandleSimpleTag(markdown, ref i, end, "__", "strong") ||
                   HandleSimpleTag(markdown, ref i, end, "**", "strong");
        }

//        private bool HandleCode(string markdown, ref int i, int end)
//        {
//            return HandleSimpleTag(markdown, ref i, end, "```", "code");
//        }

        private bool HandleSimpleTag(string markdown, ref int i, int end, string pattern, string tag)
        {
            if (!MayBeTagInAt(markdown, pattern, i) ||
                markdown.AtIs(i + pattern.Length, char.IsWhiteSpace))
                return false;

            int closingIndex;
            try
            {
                closingIndex = markdown
                    .FindAllFromTo(pattern, i + 1, end)
                    .First(j =>
                        MayBeTagInAt(markdown, pattern, j) && !markdown.AtIs(j - 1,
                            char.IsWhiteSpace));
            }
            catch (InvalidOperationException)
            {
                return false;
            }

            Sb.Append("<");
            Sb.Append(tag);
            Sb.Append(">");

            i += pattern.Length;
            RenderToHtml(markdown, ref i, closingIndex);

            Sb.Append("</");
            Sb.Append(tag);
            Sb.Append(">");

            i = closingIndex + pattern.Length;
            return true;
        }

        public string RenderToHtml(string markdown)
        {
            Sb.Clear();
            var i = 0;
            RenderToHtml(markdown, ref i, markdown.Length);
            return Sb.ToString();
        }

        private void RenderToHtml(string markdown, ref int i, int end)
        {
            while (i < end)
            {
                if (markdown[i] == Escape)
                {
                    HandleEscape(markdown, ref i);
                    continue;
                }
                var matchedSomeHandler = false;
                foreach (var markdownHandler in Handlers.Excluding(ExludedHandlers))
                {
                    if (!TryHandler(markdown, ref i, end, markdownHandler))
                        continue;
                    matchedSomeHandler = true;
                    break;
                }
                if (matchedSomeHandler) continue;

                Sb.Append(markdown[i]);
                i++;
            }
        }

        private bool TryHandler(string markdown, ref int i, int end, MarkdownHandler markdownHandler)
        {
            IEnumerable<MarkdownHandler> blockedByCurrentHandler =
                HandlerBlocks[markdownHandler]
                    .Excluding(ExludedHandlers)
                    .ToArray();
            ExludedHandlers.AddAll(blockedByCurrentHandler);
            var currentHandlerSucceeded = markdownHandler(markdown, ref i, end);
            ExludedHandlers.RemoveAll(blockedByCurrentHandler);
            return currentHandlerSucceeded;
        }

        private void HandleEscape(string markdown, ref int i)
        {
            if (i + 1 < markdown.Length)
            {
                Sb.Append(markdown[i + 1]);
                i += 2;
            }
            else
            {
                Sb.Append(Escape);
                i++;
            }
        }

        private static bool MayBeTagInAt(string str, string pattern, int i)
        {
            var length = pattern.Length;
            return str.HasAt(pattern, i) &&
                   !str.AtIs(i - 1, pattern.Contains) &&
                   !str.AtIs(i + length, pattern.Contains) &&
                   !(str.AtIs(i - 1, char.IsLetterOrDigit) &&
                     str.AtIs(i + length, char.IsLetterOrDigit) &&
                     str.CharsFromTo(i, i + length).All(c => c == '_'));
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


        [TestCase(@"no underscores", @"no underscores", TestName = "LeaveUntouched_TextWithoutUnderscores")]
        [TestCase(@"_text_", @"<em>text</em>", TestName = "SingleUnderscores_ShouldBeReplacedWithTagEm")]
        [TestCase(@"__text__", @"<strong>text</strong>", TestName = "DoubleUnderscores_ShouldBeReplacedWithTagStrong")]
        [TestCase(@"__*text*__", @"<strong><em>text</em></strong>", TestName = "Nesting_ShouldBeReplacedWithNestedTags")]
        [TestCase(@"____интся____", @"____интся____", TestName = "UnknownSequenceOfUnderscores_ShouldBeText")]
        [TestCase(@"__text", @"__text", TestName = "NotPairedTag_ShouldBeText")]
        [TestCase(@"*__text*", @"<em>__text</em>", TestName = "NotPairedTagInsidePair_ShouldBeText")]
        [TestCase(@"\_text_", @"_text_", TestName = "EscapeChar_ShouldPreventFormatingTagWithPair")]
        [TestCase(@"\_text\_", @"_text_", TestName = "EscapeChar_ShouldPreventFormatingBothTagsInPair")]
        [TestCase(@"_*text*_", @"<em>*text*</em>", TestName = "Tag_ShouldNotWorkInsideSelfOrAlias")]
        [TestCase(@"__н _с ием.", @"__н _с ием.", TestName = "NotPairableTags_ShouldBeText")]
        [TestCase(@"и_ подчерки_ ", @"и_ подчерки_ ", TestName = "OpeningTagsFolowedByWhiteSpace_ShouldBeText")]
        [TestCase(@"_подчерки _н", @"_подчерки _н", TestName = "ClosingTagsLeadByWhiteSpace_ShouldBeText")]
        [TestCase(@"_две __пары_ разных__", @"<em>две __пары</em> разных__", TestName = "IfTwoTagsInterceptEachOther_ShouldWorkLeadingOne")]
        [TestCase(@"цифрами_12_3", @"цифрами_12_3", TestName = "Undersores_ShouldBeTextInsideWordsWithDigits")]
        public void CheckMdOn(string subject, string expected)
        {
            md.RenderToHtml(subject).Should().Be(expected);
        }

        //        [Test]
        //        public void CodeTag_ShouldBlockAllTags()
        //        {
        //            CheckMdOn(@"```_text_ _sdqf_```", "<code>_text_ _sdqf_</code>");
        //        }
    }
}