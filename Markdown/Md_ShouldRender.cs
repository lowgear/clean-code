using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
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

        [Test]
        public void Rendering_ShouldFitInTime()
        {
            var str = string.Concat(Enumerable.Repeat("_ ", 100000));
            Action act = () => md.RenderToHtml(str);
            act.ExecutionTime().ShouldNotExceed(new TimeSpan(0,0,0,0, 1000));
        }
    }
}