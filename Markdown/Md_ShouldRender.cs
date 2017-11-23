using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Markdown.LexemConsumers;
using NUnit.Framework;

namespace Markdown
{
    [TestFixture]
    public class Md_ShouldRender
    {
        private Md md;

        [SetUp]
        public void SetUp()
        {
            md = new Md();
            md.AddLexemConsumer(new EscapeConsumer('\\'));
            md.AddLexemConsumer(new UnderscoresConsumer(new Dictionary<int, string>
            {
                {1, "em"},
                {2, "strong"}
            }));
        }

        [Timeout(1000)]
        [TestCase(@"no underscores", @"no underscores", TestName = "LeaveUntouched_TextWithoutUnderscores")]
        [TestCase(@"_text_", @"<em>text</em>", TestName = "SingleUnderscores_ShouldBeReplacedWithTagEm")]
        [TestCase(@"__text__", @"<strong>text</strong>", TestName =
            "DoubleUnderscores_ShouldBeReplacedWithTagStrong")]
        [TestCase(@"__t _text_ t__", @"<strong>t <em>text</em> t</strong>", TestName =
            "Nesting_ShouldBeReplacedWithNestedTags")]
        [TestCase(@"____интся____", @"____интся____", TestName = "UnknownSequenceOfUnderscores_ShouldBeText")]
        [TestCase(@"__text", @"__text", TestName = "NotPairedTag_ShouldBeText")]
//        [TestCase(@"*__text*", @"<em>__text</em>", TestName = "NotPairedTagInsidePair_ShouldBeText")]
        [TestCase(@"\_text_", @"_text_", TestName = "EscapeChar_ShouldPreventFormatingTagWithPair")]
        [TestCase(@"\_text\_", @"_text_", TestName = "EscapeChar_ShouldPreventFormatingBothTagsInPair")]
//        [TestCase(@"_*text*_", @"<em>*text*</em>", TestName = "Tag_ShouldNotWorkInsideSelfOrAlias")]
        [TestCase(@"__н _с ием.", @"__н _с ием.", TestName = "NotPairableTags_ShouldBeText")]
        [TestCase(@"и_ подчерки_ ", @"и_ подчерки_ ", TestName =
            "OpeningTagsFolowedByWhiteSpace_ShouldBeText")]
        [TestCase(@"_подчерки _н", @"_подчерки _н", TestName = "ClosingTagsLeadByWhiteSpace_ShouldBeText")]
        [TestCase(@"_две __пары_ разных__", @"<em>две __пары</em> разных__", TestName =
            "IfTwoTagsInterceptEachOther_ShouldWorkLeadingOne")]
        [TestCase(@"цифрами_12_3", @"цифрами_12_3", TestName =
            "Undersores_ShouldBeTextInsideWordsWithDigits")]
        [TestCase(@"qwe _qwe_ qwe", @"qwe <em>qwe</em> qwe", TestName = "TagsAroundTaggedArea_ShouldBeHandled")]
        [TestCase(@"\__q_", @"_<em>q</em>", TestName = "EscapedUnderscore_ShouldNotPreventUnderscoreTagging")]
        [TestCase(@"_\ q_", @"<em> q</em>", TestName = "EscapedWhitespace_ShouldNotPreventUnderscoreTagging")]
        [TestCase(@"\a_1_", @"a<em>1</em>", TestName = "EscapedLetterOrDigit_ShouldNotPreventUnderscoreTagging")]
        public void CheckMdOn(string subject, string expected)
        {
            md.RenderToHtml(subject).Should().Be(expected);
        }

        [TestCase(1000)]
        [TestCase(2000)]
        [TestCase(3000)]
        [TestCase(4000)]
        [TestCase(5000)]
        [TestCase(6000)]
        [TestCase(7000)]
        [TestCase(8000)]
        [TestCase(9000)]
        [TestCase(10000)]
        public void Rendering_ShouldFitInTime(int length)
        {
            var markdown = string.Concat(Enumerable.Repeat(@"_a ", length).Concat(Enumerable.Repeat(@" a_", length)));
            var timeLimit = new TimeSpan(TimeCoefficient * length);

            Action act = () => md.RenderToHtml(markdown);

            act.ExecutionTime().ShouldNotExceed(timeLimit);
        }

        private const long TimeCoefficient = 6000; // chosen so that the weakest test is passed
    }
}